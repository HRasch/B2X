using B2Connect.CatalogService.Models;

namespace B2Connect.CatalogService.Repositories;

/// <summary>
/// Repository interface for Category operations
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
    /// <summary>Gets a category by slug</summary>
    Task<Category?> GetBySlugAsync(string slug);

    /// <summary>Gets all root categories (without parent)</summary>
    Task<IEnumerable<Category>> GetRootCategoriesAsync();

    /// <summary>Gets child categories of a parent</summary>
    Task<IEnumerable<Category>> GetChildCategoriesAsync(Guid parentId);

    /// <summary>Gets category with all products</summary>
    Task<Category?> GetWithProductsAsync(Guid id);

    /// <summary>Gets complete category hierarchy</summary>
    Task<IEnumerable<Category>> GetHierarchyAsync();

    /// <summary>Gets all active categories</summary>
    Task<IEnumerable<Category>> GetActiveAsync();
}
