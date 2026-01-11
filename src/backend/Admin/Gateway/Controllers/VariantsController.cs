using B2X.Catalog.Application.Commands;
using B2X.Variants.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace B2X.Admin.Controllers;

/// <summary>
/// Admin Variants Management API
/// Provides full CRUD operations for product variants management
/// </summary>
[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Policy = "CatalogManager")]
public class VariantsController : ControllerBase
{
    private readonly ILogger<VariantsController> _logger;
    private readonly IMessageBus _messageBus;

    public VariantsController(ILogger<VariantsController> logger, IMessageBus messageBus)
    {
        _logger = logger;
        _messageBus = messageBus;
    }

    /// <summary>
    /// Creates a new variant
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(VariantDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateVariant(
        [FromBody] B2X.Catalog.Application.Commands.CreateVariantDto createDto,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var command = new B2X.Catalog.Application.Commands.CreateVariantCommand(createDto);

            // Execute command via Wolverine
            var result = await _messageBus.InvokeAsync<VariantDto>(command);

            return CreatedAtAction(
                nameof(GetVariant),
                new { id = result.Id },
                result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating variant");
            return BadRequest(new { Message = "Failed to create variant", Error = ex.Message });
        }
    }

    /// <summary>
    /// Gets a variant by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VariantDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetVariant(Guid id, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        try
        {
            var query = new B2X.Catalog.Application.Commands.GetVariantByIdQuery(id, tenantId);
            var result = await _messageBus.InvokeAsync<VariantDto>(query);

            if (result == null)
            {
                return NotFound(new { Message = "Variant not found" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving variant {Id}", id);
            return NotFound(new { Message = "Variant not found" });
        }
    }

    /// <summary>
    /// Gets a variant by SKU
    /// </summary>
    [HttpGet("sku/{sku}")]
    [ProducesResponseType(typeof(VariantDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetVariantBySku(string sku, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        try
        {
            var query = new B2X.Catalog.Application.Commands.GetVariantBySkuQuery(sku, tenantId);
            var result = await _messageBus.InvokeAsync<VariantDto>(query);

            if (result == null)
            {
                return NotFound(new { Message = "Variant not found" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving variant by SKU {Sku}", sku);
            return NotFound(new { Message = "Variant not found" });
        }
    }

    /// <summary>
    /// Gets variants for a product with pagination
    /// </summary>
    [HttpGet("product/{productId}")]
    [ProducesResponseType(typeof(PagedResult<VariantDto>), 200)]
    public async Task<IActionResult> GetVariantsByProduct(
        Guid productId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new B2X.Catalog.Application.Commands.GetVariantsByProductQuery(productId, tenantId, page, pageSize);

            var result = await _messageBus.InvokeAsync<PagedResult<VariantDto>>(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving variants for product {ProductId}", productId);
            return BadRequest(new { Message = "Failed to retrieve variants" });
        }
    }

    /// <summary>
    /// Updates a variant
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(VariantDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateVariant(
        Guid id,
        [FromBody] B2X.Catalog.Application.Commands.UpdateVariantDto updateDto,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var command = new B2X.Catalog.Application.Commands.UpdateVariantCommand(id, updateDto);

            var result = await _messageBus.InvokeAsync<VariantDto>(command);

            if (result == null)
            {
                return NotFound(new { Message = "Variant not found" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating variant {Id}", id);
            return BadRequest(new { Message = "Failed to update variant", Error = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a variant
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteVariant(Guid id, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        try
        {
            var command = new B2X.Catalog.Application.Commands.DeleteVariantCommand(id);
            await _messageBus.PublishAsync(command);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting variant {Id}", id);
            return NotFound(new { Message = "Variant not found or could not be deleted" });
        }
    }

    /// <summary>
    /// Searches variants
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResult<VariantDto>), 200)]
    public async Task<IActionResult> SearchVariants(
        [FromQuery] string q,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { Message = "Search query is required" });
        }

        try
        {
            var query = new B2X.Catalog.Application.Commands.SearchVariantsQuery(q, tenantId, page, pageSize);

            var result = await _messageBus.InvokeAsync<PagedResult<VariantDto>>(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching variants with query '{Query}'", q);
            return BadRequest(new { Message = "Search failed" });
        }
    }
}
