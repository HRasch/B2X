using Microsoft.EntityFrameworkCore;
using B2Connect.CatalogService.Data;
using B2Connect.CatalogService.Models;

namespace B2Connect.CatalogService.Repositories;

/// <summary>
/// Repository implementation for Product operations
/// </summary>
public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(CatalogDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetBySkuAsync(string sku)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Sku == sku);
    }

    public async Task<Product?> GetBySlugAsync(string slug)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId)
    {
        return await _dbSet
            .Where(p => p.ProductCategories.Any(pc => pc.CategoryId == categoryId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByBrandAsync(Guid brandId)
    {
        return await _dbSet
            .Where(p => p.BrandId == brandId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetFeaturedAsync(int take = 10)
    {
        return await _dbSet
            .Where(p => p.IsFeatured && p.IsActive)
            .OrderBy(p => p.CreatedAt)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetNewAsync(int take = 10)
    {
        return await _dbSet
            .Where(p => p.IsNew && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
    {
        var lowerTerm = searchTerm.ToLower();
        return await _dbSet
            .Where(p => p.IsActive && p.Sku.ToLower().Contains(lowerTerm))
            .ToListAsync();
    }

    public async Task<Product?> GetWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(p => p.Brand)
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .Include(p => p.Variants)
                .ThenInclude(v => v.AttributeValues)
            .Include(p => p.Images)
            .Include(p => p.Documents)
            .Include(p => p.AttributeValues)
                .ThenInclude(av => av.Attribute)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<(IEnumerable<Product> Items, int Total)> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = _dbSet.Where(p => p.IsActive);
        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
