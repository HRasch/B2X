using B2Connect.Catalog.Models;
using B2Connect.Catalog.Services;

namespace B2Connect.Catalog.Handlers;

/// <summary>
/// Handler interface for product queries
/// </summary>
public interface IProductQueryHandler
{
    Task<ProductDto?> GetByIdAsync(Guid tenantId, Guid productId, CancellationToken cancellationToken = default);
    Task<PagedResult<ProductDto>> GetPagedAsync(Guid tenantId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<PagedResult<ProductDto>> SearchAsync(Guid tenantId, string searchTerm, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handler for product queries
/// </summary>
public class ProductQueryHandler : IProductQueryHandler
{
    private readonly IProductService _productService;
    private readonly ISearchIndexService _searchIndex;

    public ProductQueryHandler(IProductService productService, ISearchIndexService searchIndex)
    {
        _productService = productService;
        _searchIndex = searchIndex;
    }

    public Task<ProductDto?> GetByIdAsync(Guid tenantId, Guid productId, CancellationToken cancellationToken = default)
    {
        return _productService.GetByIdAsync(tenantId, productId, cancellationToken);
    }

    public Task<PagedResult<ProductDto>> GetPagedAsync(Guid tenantId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        return _productService.GetPagedAsync(tenantId, pageNumber, pageSize, cancellationToken);
    }

    public Task<PagedResult<ProductDto>> SearchAsync(Guid tenantId, string searchTerm, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        return _searchIndex.SearchAsync(tenantId, searchTerm, pageNumber, pageSize, cancellationToken);
    }
}
