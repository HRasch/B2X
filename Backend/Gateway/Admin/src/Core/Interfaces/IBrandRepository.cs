using B2Connect.Admin.Core.Entities;

namespace B2Connect.Admin.Core.Interfaces;

/// <summary>
/// Repository interface for Brand operations
/// </summary>
public interface IBrandRepository : IRepository<Brand>
{
    /// <summary>Gets a brand by slug</summary>
    Task<Brand?> GetBySlugAsync(Guid tenantId, string slug, CancellationToken ct = default);

    /// <summary>Gets all active brands</summary>
    Task<IEnumerable<Brand>> GetActiveBrandsAsync(Guid tenantId, CancellationToken ct = default);

    /// <summary>Gets brand with all products</summary>
    Task<Brand?> GetWithProductsAsync(Guid tenantId, Guid id, CancellationToken ct = default);

    /// <summary>Gets brands paginated</summary>
    Task<(IEnumerable<Brand> Items, int Total)> GetPagedAsync(Guid tenantId, int pageNumber, int pageSize, CancellationToken ct = default);
}
