using B2X.Admin.Core.Entities;

namespace B2X.Admin.Core.Interfaces;

/// <summary>
/// Repository interface for Product operations
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    /// <summary>Gets a product by SKU</summary>
    Task<Product?> GetBySkuAsync(Guid tenantId, string sku, CancellationToken ct = default);

    /// <summary>Gets a product by slug</summary>
    Task<Product?> GetBySlugAsync(Guid tenantId, string slug, CancellationToken ct = default);

    /// <summary>Gets products by category ID</summary>
    Task<IEnumerable<Product>> GetByCategoryAsync(Guid tenantId, Guid categoryId, CancellationToken ct = default);

    /// <summary>Gets products by brand ID</summary>
    Task<IEnumerable<Product>> GetByBrandAsync(Guid tenantId, Guid brandId, CancellationToken ct = default);

    /// <summary>Gets featured products</summary>
    Task<IEnumerable<Product>> GetFeaturedAsync(Guid tenantId, int take = 10, CancellationToken ct = default);

    /// <summary>Gets new products (GetNewestAsync)</summary>
    Task<IEnumerable<Product>> GetNewestAsync(Guid tenantId, int take = 10, CancellationToken ct = default);

    /// <summary>Searches products by name or description</summary>
    Task<(IEnumerable<Product>, int)> SearchAsync(Guid tenantId, string searchTerm, int pageNumber, int pageSize, CancellationToken ct = default);

    /// <summary>Gets product with all related data (variants, images, documents)</summary>
    Task<Product?> GetWithDetailsAsync(Guid tenantId, Guid id, CancellationToken ct = default);

    /// <summary>Gets paginated products</summary>
    Task<(IEnumerable<Product> Items, int Total)> GetPagedAsync(Guid tenantId, int pageNumber, int pageSize, CancellationToken ct = default);
}
