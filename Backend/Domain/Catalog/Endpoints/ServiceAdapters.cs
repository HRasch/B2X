using System.Linq;
using B2Connect.CatalogService.Models;
using B2Connect.CatalogService.Services;

namespace B2Connect.Catalog.Endpoints;

public class ProductServiceAdapter : IProductService
{
    private readonly B2Connect.CatalogService.Services.IProductService _productService;

    public ProductServiceAdapter(B2Connect.CatalogService.Services.IProductService productService)
    {
        _productService = productService;
    }

    public async Task<dynamic?> GetBySkuAsync(Guid tenantId, string sku, CancellationToken ct = default)
    {
        var paged = await _productService.SearchAsync(tenantId, sku, 1, 1, ct);
        var item = paged?.Items?.FirstOrDefault();
        return item;
    }
}

public class SearchIndexAdapter : ISearchIndexService
{
    private readonly B2Connect.CatalogService.Services.ISearchIndexService _searchService;

    public SearchIndexAdapter(B2Connect.CatalogService.Services.ISearchIndexService searchService)
    {
        _searchService = searchService;
    }

    public async Task<dynamic> SearchAsync(Guid tenantId, string query, CancellationToken ct = default)
    {
        var result = await _searchService.SearchAsync(tenantId, query, 1, 20, ct);
        return result;
    }
}
