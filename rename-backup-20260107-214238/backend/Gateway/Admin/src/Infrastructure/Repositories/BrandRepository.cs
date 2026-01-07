using B2X.Admin.Core.Entities;
using B2X.Admin.Core.Interfaces;
using B2X.Admin.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace B2X.Admin.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Brand operations
/// </summary>
public class BrandRepository : Repository<Brand>, IBrandRepository
{
    public BrandRepository(CatalogDbContext context) : base(context)
    {
    }

    public Task<Brand?> GetBySlugAsync(Guid tenantId, string slug, CancellationToken ct = default)
    {
        return _dbSet.FirstOrDefaultAsync(b => b.TenantId == tenantId && b.Slug == slug, ct);
    }

    public async Task<IEnumerable<Brand>> GetActiveBrandsAsync(Guid tenantId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(b => b.TenantId == tenantId && b.IsActive)
            .OrderBy(b => b.DisplayOrder)
            .ToListAsync(ct).ConfigureAwait(false);
    }

    public Task<Brand?> GetWithProductsAsync(Guid tenantId, Guid id, CancellationToken ct = default)
    {
        return _dbSet
            .Where(b => b.TenantId == tenantId && b.Id == id)
            .Include(b => b.Products)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<(IEnumerable<Brand> Items, int Total)> GetPagedAsync(Guid tenantId, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var query = _dbSet.Where(b => b.TenantId == tenantId && b.IsActive);
        var total = await query.CountAsync(ct).ConfigureAwait(false);
        var items = await query
            .OrderBy(b => b.DisplayOrder)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct).ConfigureAwait(false);

        return (items, total);
    }
}
