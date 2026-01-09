using B2X.Tenancy.Infrastructure.Data;
using B2X.Tenancy.Models;
using B2X.Types.Domain;
using Microsoft.EntityFrameworkCore;

namespace B2X.Tenancy.Repositories;

/// <summary>
/// EF Core implementation of ITenantRepository.
/// </summary>
public class TenantRepository : ITenantRepository
{
    private readonly TenancyDbContext _context;

    public TenantRepository(TenancyDbContext context)
    {
        _context = context;
    }

    public Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var normalizedSlug = slug.ToLowerInvariant();
        return _context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Slug.ToLower() == normalizedSlug, cancellationToken);
    }

    public async Task<IReadOnlyList<Tenant>> GetAllAsync(
        TenantStatus? status = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Tenants.AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLowerInvariant();
            query = query.Where(t =>
                t.Name.ToLower().Contains(term) ||
                t.Slug.ToLower().Contains(term));
        }

        return await query
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<(IReadOnlyList<Tenant> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        TenantStatus? status = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Tenants.AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLowerInvariant();
            query = query.Where(t =>
                t.Name.ToLower().Contains(term) ||
                t.Slug.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

        var items = await query
            .OrderBy(t => t.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return (items, totalCount);
    }

    public async Task<Tenant> CreateAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        tenant.CreatedAt = DateTime.UtcNow;
        tenant.UpdatedAt = DateTime.UtcNow;

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return tenant;
    }

    public async Task<Tenant> UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        tenant.UpdatedAt = DateTime.UtcNow;

        _context.Tenants.Update(tenant);
        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return tenant;
    }

    public Task<bool> SlugExistsAsync(string slug, Guid? excludeTenantId = null, CancellationToken cancellationToken = default)
    {
        var normalizedSlug = slug.ToLowerInvariant();
        var query = _context.Tenants.Where(t => t.Slug.ToLower() == normalizedSlug);

        if (excludeTenantId.HasValue)
        {
            query = query.Where(t => t.Id != excludeTenantId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public Task<int> CountAsync(TenantStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Tenants.AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        return query.CountAsync(cancellationToken);
    }
}
