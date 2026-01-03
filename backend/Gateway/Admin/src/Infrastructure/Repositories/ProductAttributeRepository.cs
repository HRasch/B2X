using B2Connect.Admin.Core.Entities;
using B2Connect.Admin.Core.Interfaces;
using B2Connect.Admin.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.Admin.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for ProductAttribute operations
/// </summary>
public class ProductAttributeRepository : Repository<ProductAttribute>, IProductAttributeRepository
{
    public ProductAttributeRepository(CatalogDbContext context) : base(context)
    {
    }

    public Task<ProductAttribute?> GetByCodeAsync(string code)
    {
        return _dbSet.FirstOrDefaultAsync(a => a.Code == code);
    }

    public async Task<IEnumerable<ProductAttribute>> GetActiveAsync()
    {
        return await _dbSet
            .Where(a => a.IsActive)
            .OrderBy(a => a.DisplayOrder)
            .ToListAsync();
    }

    public Task<ProductAttribute?> GetWithOptionsAsync(Guid id)
    {
        return _dbSet
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
