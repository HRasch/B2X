using B2X.Categories.Models;
using B2X.Types.DTOs;

namespace B2X.Categories.Handlers;

/// <summary>
/// Repository interface for category operations
/// </summary>
public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, Guid tenantId);
    Task<PagedResult<Category>> GetPagedAsync(
        Guid tenantId,
        string? searchTerm = null,
        Guid? parentId = null,
        bool? isActive = null,
        int pageNumber = 1,
        int pageSize = 50,
        string? sortBy = "DisplayOrder",
        bool sortDescending = false);
    Task<List<Category>> GetHierarchyAsync(Guid tenantId, Guid? rootCategoryId = null);
    Task<List<Category>> GetChildrenAsync(Guid parentId, Guid tenantId);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Guid id, Guid tenantId);
    Task<bool> ExistsAsync(Guid id, Guid tenantId);
}

/// <summary>
/// In-memory repository implementation for categories
/// </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly Dictionary<(Guid, Guid), Category> _categories = new();

    public Task<Category?> GetByIdAsync(Guid id, Guid tenantId)
    {
        _categories.TryGetValue((id, tenantId), out var category);
        return Task.FromResult(category);
    }

    public Task<PagedResult<Category>> GetPagedAsync(
        Guid tenantId,
        string? searchTerm = null,
        Guid? parentId = null,
        bool? isActive = null,
        int pageNumber = 1,
        int pageSize = 50,
        string? sortBy = "DisplayOrder",
        bool sortDescending = false)
    {
        var query = _categories.Values.Where(c => c.TenantId == tenantId);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c => c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                   (c.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (parentId.HasValue)
        {
            query = query.Where(c => c.ParentId == parentId);
        }

        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }

        // Apply sorting
        query = sortBy?.ToLowerInvariant() switch
        {
            "name" => sortDescending ? query.OrderByDescending(c => c.Name, StringComparer.Ordinal) : query.OrderBy(c => c.Name, StringComparer.Ordinal),
            "createdat" => sortDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
            "updatedat" => sortDescending ? query.OrderByDescending(c => c.UpdatedAt) : query.OrderBy(c => c.UpdatedAt),
            _ => sortDescending ? query.OrderByDescending(c => c.DisplayOrder) : query.OrderBy(c => c.DisplayOrder)
        };

        // Materialize query to avoid multiple enumeration (CA1851)
        var materializedQuery = query.ToList();
        var totalCount = materializedQuery.Count;
        var items = materializedQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(new PagedResult<Category>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
    }

    public Task<List<Category>> GetHierarchyAsync(Guid tenantId, Guid? rootCategoryId = null)
    {
        var allCategories = _categories.Values.Where(c => c.TenantId == tenantId).ToList();

        if (rootCategoryId.HasValue)
        {
            var root = allCategories.FirstOrDefault(c => c.Id == rootCategoryId);
            if (root == null)
                return Task.FromResult(new List<Category>());

            return Task.FromResult(BuildHierarchy(new List<Category> { root }, allCategories));
        }

        // Get root categories (no parent)
        var roots = allCategories.Where(c => !c.ParentId.HasValue).ToList();
        return Task.FromResult(BuildHierarchy(roots, allCategories));
    }

    public Task<List<Category>> GetChildrenAsync(Guid parentId, Guid tenantId)
    {
        var children = _categories.Values
            .Where(c => c.TenantId == tenantId && c.ParentId == parentId)
            .ToList();
        return Task.FromResult(children);
    }

    public Task AddAsync(Category category)
    {
        _categories[(category.Id, category.TenantId)] = category;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Category category)
    {
        var key = (category.Id, category.TenantId);
        if (!_categories.ContainsKey(key))
        {
            throw new InvalidOperationException($"Category {category.Id} not found for tenant {category.TenantId}");
        }
        _categories[key] = category;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, Guid tenantId)
    {
        _categories.Remove((id, tenantId));
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id, Guid tenantId)
    {
        return Task.FromResult(_categories.ContainsKey((id, tenantId)));
    }

    private List<Category> BuildHierarchy(List<Category> categories, List<Category> allCategories)
    {
        foreach (var category in categories)
        {
            category.Children = allCategories
                .Where(c => c.ParentId == category.Id)
                .OrderBy(c => c.DisplayOrder)
                .ToList();

            if (category.Children.Any())
            {
                BuildHierarchy(category.Children, allCategories);
            }
        }

        return categories.OrderBy(c => c.DisplayOrder).ToList();
    }
}
