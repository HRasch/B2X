using B2X.Admin.Core.Entities;
using B2X.Admin.Core.Interfaces;
using B2X.Admin.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2X.Admin.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Category operations
/// </summary>
public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(CatalogDbContext context) : base(context)
    {
    }

    public Task<Category?> GetBySlugAsync(Guid tenantId, string slug, CancellationToken ct = default)
    {
        return _dbSet.FirstOrDefaultAsync(c => c.TenantId == tenantId && c.Slug == slug, ct);
    }

    public async Task<IEnumerable<Category>> GetRootCategoriesAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(c => c.TenantId == tenantId && c.ParentCategoryId == null && c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task<IEnumerable<Category>> GetChildCategoriesAsync(Guid tenantId, Guid parentId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(c => c.TenantId == tenantId && c.ParentCategoryId == parentId && c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(ct).ConfigureAwait(false);
    }

    public Task<Category?> GetWithProductsAsync(Guid tenantId, Guid id, CancellationToken ct = default)
    {
        return _dbSet
            .Where(c => c.TenantId == tenantId && c.Id == id)
            .Include(c => c.ChildCategories)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<Category>> GetHierarchyAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(c => c.TenantId == tenantId && c.IsActive && c.ParentCategoryId == null)
            .Include(c => c.ChildCategories)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(c => c.TenantId == tenantId && c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(ct).ConfigureAwait(false);
    }
}
