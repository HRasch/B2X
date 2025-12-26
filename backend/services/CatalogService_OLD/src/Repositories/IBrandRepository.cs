using B2Connect.CatalogService.Models;

namespace B2Connect.CatalogService.Repositories;

/// <summary>
/// Repository interface for Brand operations
/// </summary>
public interface IBrandRepository : IRepository<Brand>
{
    /// <summary>Gets a brand by slug</summary>
    Task<Brand?> GetBySlugAsync(string slug);

    /// <summary>Gets all active brands</summary>
    Task<IEnumerable<Brand>> GetActiveAsync();

    /// <summary>Gets brand with all products</summary>
    Task<Brand?> GetWithProductsAsync(Guid id);

    /// <summary>Gets brands paginated</summary>
    Task<(IEnumerable<Brand> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize);
}
