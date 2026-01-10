using B2X.Store.ServiceClients;
using B2X.Variants.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    private readonly IVariantsServiceClient _variantsService;
    private readonly ILogger<VariantsController> _logger;

    public VariantsController(
        IVariantsServiceClient variantsService,
        ILogger<VariantsController> logger)
    {
        _variantsService = variantsService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a variant by SKU
    /// </summary>
    [HttpGet("sku/{sku}")]
    [ProducesResponseType(typeof(B2X.Variants.Models.VariantDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetVariantBySku(string sku, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        var variant = await _variantsService.GetVariantBySkuAsync(sku, tenantId);
        if (variant == null)
        {
            return NotFound(new { Message = "Variant not found" });
        }

        return Ok(variant);
    }

    /// <summary>
    /// Gets a variant by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(B2X.Variants.Models.VariantDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetVariantById(Guid id, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        var variant = await _variantsService.GetVariantByIdAsync(id, tenantId);
        if (variant == null)
        {
            return NotFound(new { Message = "Variant not found" });
        }

        return Ok(variant);
    }

    /// <summary>
    /// Gets variants for a specific product
    /// </summary>
    [HttpGet("product/{productId}")]
    [ProducesResponseType(typeof(PagedResult<B2X.Variants.Models.VariantDto>), 200)]
    public async Task<IActionResult> GetVariantsByProduct(
        Guid productId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _variantsService.GetVariantsByProductAsync(productId, tenantId, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Searches variants
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResult<B2X.Variants.Models.VariantDto>), 200)]
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

        var result = await _variantsService.SearchVariantsAsync(q, tenantId, page, pageSize);
        return Ok(result);
    }
}
