using Microsoft.EntityFrameworkCore;
using B2Connect.CatalogService.Data;
using B2Connect.CatalogService.Models;

namespace B2Connect.CatalogService.Repositories;

/// <summary>
/// Repository implementation for Brand operations
/// </summary>
public class BrandRepository : Repository<Brand>, IBrandRepository
{
    public BrandRepository(CatalogDbContext context) : base(context)
    {
    }

    public async Task<Brand?> GetBySlugAsync(string slug)
    {
        return await _dbSet.FirstOrDefaultAsync(b => b.Slug == slug);
    }

    public async Task<IEnumerable<Brand>> GetActiveAsync()
    {
        return await _dbSet
            .Where(b => b.IsActive)
            .OrderBy(b => b.DisplayOrder)
            .ToListAsync();
    }

    public async Task<Brand?> GetWithProductsAsync(Guid id)
    {
        return await _dbSet
            .Include(b => b.Products)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<(IEnumerable<Brand> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = _dbSet.Where(b => b.IsActive);
        var total = await query.CountAsync();
        var items = await query
            .OrderBy(b => b.DisplayOrder)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
