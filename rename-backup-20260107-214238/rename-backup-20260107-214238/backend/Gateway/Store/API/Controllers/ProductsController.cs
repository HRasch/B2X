using B2X.Shared.Infrastructure.ServiceClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2X.Store.Controllers;

/// <summary>
/// Example controller demonstrating Service Discovery usage
/// This controller calls other microservices using Aspire Service Discovery
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ICatalogServiceClient _catalogService;
    private readonly ILocalizationServiceClient _localizationService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        ICatalogServiceClient catalogService,
        ILocalizationServiceClient localizationService,
        ILogger<ProductsController> logger)
    {
        _catalogService = catalogService;
        _localizationService = localizationService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a product by SKU with localized name
    /// Demonstrates calling multiple services via Service Discovery
    /// </summary>
    [HttpGet("{sku}")]
    public async Task<IActionResult> GetProduct(string sku, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        // Call Catalog Service (via Service Discovery: http://catalog-service)
        var product = await _catalogService.GetProductBySkuAsync(sku, tenantId);
        if (product == null)
        {
            return NotFound(new { Message = "Product not found" });
        }

        // Call Localization Service (via Service Discovery: http://localization-service)
        var locale = Request.Headers.AcceptLanguage.FirstOrDefault() ?? "en";
        var localizedName = await _localizationService.GetTranslationAsync(
            $"product.{sku}.name",
            locale,
            tenantId);

        return Ok(new
        {
            product.Id,
            product.Sku,
            Name = localizedName ?? product.Name,
            product.Price,
            product.TenantId
        });
    }

    /// <summary>
    /// Searches products
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts(
        [FromQuery] string q,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        var products = await _catalogService.SearchProductsAsync(q, tenantId);
        return Ok(products);
    }
}
