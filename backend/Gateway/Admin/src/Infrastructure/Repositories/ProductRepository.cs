using B2Connect.Admin.Core.Entities;
using B2Connect.Admin.Core.Interfaces;
using B2Connect.Admin.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.Admin.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Product operations
/// </summary>
public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(CatalogDbContext context) : base(context)
    {
    }

    public Task<Product?> GetBySkuAsync(Guid tenantId, string sku, CancellationToken ct = default)
    {
        return _dbSet.FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Sku == sku, ct);
    }

    public Task<Product?> GetBySlugAsync(Guid tenantId, string slug, CancellationToken ct = default)
    {
        return _dbSet.FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Slug == slug, ct);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid tenantId, Guid categoryId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(p => p.TenantId == tenantId && p.CategoryId == categoryId)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Product>> GetByBrandAsync(Guid tenantId, Guid brandId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(p => p.TenantId == tenantId && p.BrandId == brandId)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Product>> GetFeaturedAsync(Guid tenantId, int take = 10, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(p => p.TenantId == tenantId && p.IsFeatured && p.IsActive)
            .OrderBy(p => p.CreatedAt)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Product>> GetNewestAsync(Guid tenantId, int take = 10, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(p => p.TenantId == tenantId && p.IsNew && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task<(IEnumerable<Product>, int)> SearchAsync(Guid tenantId, string searchTerm, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var lowerTerm = searchTerm.ToLower(System.Globalization.CultureInfo.CurrentCulture);
        var query = _dbSet
            .Where(p => p.TenantId == tenantId && p.IsActive && p.Sku.ToLower(System.Globalization.CultureInfo.CurrentCulture).Contains(lowerTerm));

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public Task<Product?> GetWithDetailsAsync(Guid tenantId, Guid id, CancellationToken ct = default)
    {
        return _dbSet
            .Where(p => p.TenantId == tenantId && p.Id == id)
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .Include(p => p.Documents)
            .Include(p => p.AttributeValues)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<(IEnumerable<Product> Items, int Total)> GetPagedAsync(Guid tenantId, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var query = _dbSet.Where(p => p.TenantId == tenantId && p.IsActive);
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }
}
