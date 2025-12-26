using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WolverineFx;
using B2Connect.CatalogService.CQRS.Commands;
using B2Connect.CatalogService.CQRS;

namespace B2Connect.CatalogService.Controllers;

/// <summary>
/// API Controller for Product WRITE operations (Commands)
/// Uses Wolverine message bus to invoke command handlers
/// All commands are validated automatically
/// </summary>
[ApiController]
[Route("api/v2/[controller]")]
[Produces("application/json")]
public class ProductsCommandController : ControllerBase
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<ProductsCommandController> _logger;

    public ProductsCommandController(
        IMessageBus messageBus,
        ILogger<ProductsCommandController> logger)
    {
        _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Create a new product
    /// POST /api/v2/products
    /// Synchronous command - returns immediately
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CommandResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CommandResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        // Extract tenant from JWT claim or X-Tenant-ID header
        command.TenantId = GetTenantId();

        _logger.LogInformation("Creating product {Sku}", command.Sku);

        try
        {
            // Wolverine invokes CreateProductCommandHandler automatically
            // ValidationException is caught and returns 400
            var result = await _messageBus.InvokeAsync(command, cancellationToken);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(
                nameof(GetProductById),
                new { id = result.Id },
                result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new CommandResult
            {
                Success = false,
                ErrorMessage = "Validation failed",
                Errors = ex.Message.Split(';')
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new CommandResult { Success = false, ErrorMessage = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing product
    /// PUT /api/v2/products/{id}
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CommandResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        command.TenantId = GetTenantId();
        command.ProductId = id;

        try
        {
            var result = await _messageBus.InvokeAsync(command, cancellationToken);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return NoContent();
        }
        catch (ValidationException ex)
        {
            return BadRequest(new CommandResult
            {
                Success = false,
                ErrorMessage = "Validation failed"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new CommandResult { Success = false, ErrorMessage = ex.Message });
        }
    }

    /// <summary>
    /// Delete a product
    /// DELETE /api/v2/products/{id}
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand
        {
            TenantId = GetTenantId(),
            ProductId = id
        };

        try
        {
            var result = await _messageBus.InvokeAsync(command, cancellationToken);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new CommandResult { Success = false, ErrorMessage = ex.Message });
        }
    }

    private Guid GetTenantId()
    {
        // Extract tenant from JWT claims (primary) or X-Tenant-ID header (fallback)
        try
        {
            // Check JWT claims for tenant_id
            var tenantClaim = HttpContext.User.FindFirst("tenant_id");
            if (tenantClaim != null && Guid.TryParse(tenantClaim.Value, out var tenantFromClaim))
            {
                _logger.LogDebug("Tenant extracted from JWT claim: {TenantId}", tenantFromClaim);
                return tenantFromClaim;
            }

            // Fallback: Check X-Tenant-ID header
            if (HttpContext.Request.Headers.TryGetValue("X-Tenant-ID", out var headerValue))
            {
                if (Guid.TryParse(headerValue, out var tenantFromHeader))
                {
                    _logger.LogDebug("Tenant extracted from X-Tenant-ID header: {TenantId}", tenantFromHeader);
                    return tenantFromHeader;
                }
            }

            // No tenant found - unauthorized
            _logger.LogWarning("No tenant ID found in JWT claims or X-Tenant-ID header");
            throw new UnauthorizedAccessException("Tenant ID not found in request. Provide 'tenant_id' claim in JWT or 'X-Tenant-ID' header.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting tenant ID");
            throw;
        }
    }
}
