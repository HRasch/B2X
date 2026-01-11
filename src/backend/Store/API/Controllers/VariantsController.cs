using B2X.Catalog.Application.Commands;
using B2X.Catalog.Models;
using B2X.Types.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace B2X.Store.Controllers;

/// <summary>
/// Read-only Variants API for Store Frontend
/// Provides access to product variants for browsing and purchasing
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "StoreAccess")]
public class VariantsController : ControllerBase
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<VariantsController> _logger;

    public VariantsController(
        IMessageBus messageBus,
        ILogger<VariantsController> logger)
    {
        _messageBus = messageBus;
        _logger = logger;
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
            var query = new GetVariantBySkuQuery(sku, tenantId);
            var variant = await _messageBus.InvokeAsync<Variant>(query);

            if (variant == null)
            {
                return NotFound(new { Message = "Variant not found" });
            }

            return Ok(variant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving variant by SKU {Sku}", sku);
            return NotFound(new { Message = "Variant not found" });
        }
    }

    /// <summary>
    /// Gets a variant by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VariantDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetVariantById(Guid id, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        try
        {
            var query = new GetVariantByIdQuery(id, tenantId);
            var variant = await _messageBus.InvokeAsync<Variant>(query);

            if (variant == null)
            {
                return NotFound(new { Message = "Variant not found" });
            }

            return Ok(variant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving variant {Id}", id);
            return NotFound(new { Message = "Variant not found" });
        }
    }

    /// <summary>
    /// Gets variants for a specific product
    /// </summary>
    [HttpGet("product/{productId}")]
    [ProducesResponseType(typeof(PagedResult<Variant>), 200)]
    public async Task<IActionResult> GetVariantsByProduct(
        Guid productId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new GetVariantsByProductQuery(productId, tenantId, page, pageSize);
            var result = await _messageBus.InvokeAsync<PagedResult<Variant>>(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving variants for product {ProductId}", productId);
            return BadRequest(new { Message = "Failed to retrieve variants" });
        }
    }

    /// <summary>
    /// Searches variants
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResult<Variant>), 200)]
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
            var query = new SearchVariantsQuery(q, tenantId, page, pageSize);
            var result = await _messageBus.InvokeAsync<PagedResult<Variant>>(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching variants with query '{Query}'", q);
            return BadRequest(new { Message = "Search failed" });
        }
    }
}
