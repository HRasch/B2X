using Microsoft.EntityFrameworkCore;
using B2Connect.CatalogService.Data;
using B2Connect.CatalogService.Models;

namespace B2Connect.CatalogService.Repositories;

/// <summary>
/// Repository implementation for ProductAttribute operations
/// </summary>
public class ProductAttributeRepository : Repository<ProductAttribute>, IProductAttributeRepository
{
    public ProductAttributeRepository(CatalogDbContext context) : base(context)
    {
    }

    public async Task<ProductAttribute?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.Code == code);
    }

    public async Task<IEnumerable<ProductAttribute>> GetActiveAsync()
    {
        return await _dbSet
            .Where(a => a.IsActive)
            .OrderBy(a => a.DisplayOrder)
            .ToListAsync();
    }

    public async Task<ProductAttribute?> GetWithOptionsAsync(Guid id)
    {
        return await _dbSet
            .Include(a => a.Options)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<ProductAttribute>> GetSearchableAsync()
    {
        return await _dbSet
            .Where(a => a.IsActive && a.IsSearchable)
            .OrderBy(a => a.DisplayOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductAttribute>> GetFilterableAsync()
    {
        return await _dbSet
            .Where(a => a.IsActive && a.IsFilterable)
            .Include(a => a.Options)
            .OrderBy(a => a.DisplayOrder)
            .ToListAsync();
    }
}
