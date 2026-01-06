using B2Connect.Admin.Core.Entities;

namespace B2Connect.Admin.Core.Interfaces;

/// <summary>
/// Repository interface for Category operations
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
    /// <summary>Gets a category by slug</summary>
    Task<Category?> GetBySlugAsync(Guid tenantId, string slug, CancellationToken ct = default);

    /// <summary>Gets all root categories (without parent)</summary>
    Task<IEnumerable<Category>> GetRootCategoriesAsync(Guid tenantId, CancellationToken ct = default);

    /// <summary>Gets child categories of a parent</summary>
    Task<IEnumerable<Category>> GetChildCategoriesAsync(Guid tenantId, Guid parentId, CancellationToken ct = default);

    /// <summary>Gets category with all products</summary>
    Task<Category?> GetWithProductsAsync(Guid tenantId, Guid id, CancellationToken ct = default);

    /// <summary>Gets complete category hierarchy</summary>
    Task<IEnumerable<Category>> GetHierarchyAsync(Guid tenantId, CancellationToken ct = default);

    /// <summary>Gets all active categories</summary>
    Task<IEnumerable<Category>> GetActiveCategoriesAsync(Guid tenantId, CancellationToken ct = default);
}
