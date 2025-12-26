using B2Connect.Admin.Core.Entities;

namespace B2Connect.Admin.Core.Interfaces;

/// <summary>
/// Repository interface for Product operations
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    /// <summary>Gets a product by SKU</summary>
    Task<Product?> GetBySkuAsync(string sku);

    /// <summary>Gets a product by slug</summary>
    Task<Product?> GetBySlugAsync(string slug);

    /// <summary>Gets products by category ID</summary>
    Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId);

    /// <summary>Gets products by brand ID</summary>
    Task<IEnumerable<Product>> GetByBrandAsync(Guid brandId);

    /// <summary>Gets featured products</summary>
    Task<IEnumerable<Product>> GetFeaturedAsync(int take = 10);

    /// <summary>Gets new products</summary>
    Task<IEnumerable<Product>> GetNewAsync(int take = 10);

    /// <summary>Searches products by name or description</summary>
    Task<IEnumerable<Product>> SearchAsync(string searchTerm);

    /// <summary>Gets product with all related data (variants, images, documents)</summary>
    Task<Product?> GetWithDetailsAsync(Guid id);

    /// <summary>Gets paginated products</summary>
    Task<(IEnumerable<Product> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize);
}
