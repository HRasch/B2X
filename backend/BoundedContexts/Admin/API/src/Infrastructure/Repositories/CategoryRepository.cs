using Microsoft.EntityFrameworkCore;
using B2Connect.Admin.Infrastructure.Data;
using B2Connect.Admin.Core.Entities;
using B2Connect.Admin.Core.Interfaces;

namespace B2Connect.Admin.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Category operations
/// </summary>
public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(CatalogDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetBySlugAsync(string slug)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Slug == slug);
    }

    public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
    {
        return await _dbSet
            .Where(c => c.ParentCategoryId == null && c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetChildCategoriesAsync(Guid parentId)
    {
        return await _dbSet
            .Where(c => c.ParentCategoryId == parentId && c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    public async Task<Category?> GetWithProductsAsync(Guid id)
    {
        return await _dbSet
            .Include(c => c.ProductCategories)
                .ThenInclude(pc => pc.Product)
            .Include(c => c.ChildCategories)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Category>> GetHierarchyAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .Include(c => c.ChildCategories)
            .Where(c => c.ParentCategoryId == null)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetActiveAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }
}

