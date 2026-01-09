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
    private readonly ISearchServiceClient _searchService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        ICatalogServiceClient catalogService,
        ILocalizationServiceClient localizationService,
        ISearchServiceClient searchService,
        ILogger<ProductsController> logger)
    {
        _catalogService = catalogService;
        _localizationService = localizationService;
        _searchService = searchService;
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
    /// Searches products using Elasticsearch
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts(
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromQuery] string q = "*",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sector = null)
    {
        var locale = Request.Headers.AcceptLanguage.FirstOrDefault() ?? "en";
        var request = new SearchRequestDto(
            Query: q,
            Page: page,
            PageSize: pageSize,
            Sector: sector,
            Locale: locale);

        var response = await _searchService.SearchProductsAsync(request, tenantId);
        return Ok(response);
    }
}
