using System.Linq;

namespace B2X.Catalog.Endpoints;

public class ProductServiceAdapter : IProductService
{
    private readonly B2X.Catalog.Services.IProductService _productService;

    public ProductServiceAdapter(B2X.Catalog.Services.IProductService productService)
    {
        _productService = productService;
    }

    public async Task<dynamic?> GetBySkuAsync(Guid tenantId, string sku, CancellationToken ct = default)
    {
        var paged = await _productService.SearchAsync(tenantId, sku, 1, 1, ct).ConfigureAwait(false);
        var item = paged?.Items?.FirstOrDefault();
        return item;
    }

    public async Task<B2X.Catalog.Models.PagedResult<B2X.Catalog.Models.ProductDto>> SearchAsync(Guid tenantId, string searchTerm, int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
    {
        return await _productService.SearchAsync(tenantId, searchTerm, pageNumber, pageSize, ct).ConfigureAwait(false);
    }
}

public class SearchIndexAdapter : ISearchIndexService
{
    private readonly B2X.Catalog.Services.ISearchIndexService _searchService;

    public SearchIndexAdapter(B2X.Catalog.Services.ISearchIndexService searchService)
    {
        _searchService = searchService;
    }

    public async Task<B2X.Catalog.Models.PagedResult<B2X.Catalog.Models.ProductDto>> SearchAsync(Guid tenantId, string searchTerm, int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
    {
        return await _searchService.SearchAsync(tenantId, searchTerm, pageNumber, pageSize, ct).ConfigureAwait(false);
    }
}
