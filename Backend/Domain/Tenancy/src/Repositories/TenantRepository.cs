using B2Connect.Tenancy.Infrastructure.Data;
using B2Connect.Tenancy.Models;
using B2Connect.Types.Domain;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.Tenancy.Repositories;

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
            .ToListAsync(cancellationToken);
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

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(t => t.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Tenant> CreateAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        tenant.CreatedAt = DateTime.UtcNow;
        tenant.UpdatedAt = DateTime.UtcNow;

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);

        return tenant;
    }

    public async Task<Tenant> UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        tenant.UpdatedAt = DateTime.UtcNow;

        _context.Tenants.Update(tenant);
        await _context.SaveChangesAsync(cancellationToken);

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

/// <summary>
/// EF Core implementation of ITenantDomainRepository.
/// </summary>
public class TenantDomainRepository : ITenantDomainRepository
{
    private readonly TenancyDbContext _context;

    public TenantDomainRepository(TenancyDbContext context)
    {
        _context = context;
    }

    public Task<TenantDomain?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.TenantDomains
            .AsNoTracking()
            .Include(d => d.Tenant)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public Task<TenantDomain?> GetByDomainNameAsync(string domainName, CancellationToken cancellationToken = default)
    {
        var normalizedDomain = NormalizeDomainName(domainName);
        return _context.TenantDomains
            .AsNoTracking()
            .Include(d => d.Tenant)
            .FirstOrDefaultAsync(d => d.DomainName.ToLower() == normalizedDomain, cancellationToken);
    }

    public async Task<IReadOnlyList<TenantDomain>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.TenantDomains
            .AsNoTracking()
            .Where(d => d.TenantId == tenantId)
            .OrderByDescending(d => d.IsPrimary)
            .ThenBy(d => d.DomainName)
            .ToListAsync(cancellationToken);
    }

    public Task<TenantDomain?> GetPrimaryDomainAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return _context.TenantDomains
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.TenantId == tenantId && d.IsPrimary, cancellationToken);
    }

    public async Task<Guid?> ResolveTenantIdAsync(string domainName, CancellationToken cancellationToken = default)
    {
        var normalizedDomain = NormalizeDomainName(domainName);
        var domain = await _context.TenantDomains
            .AsNoTracking()
            .Where(d => d.DomainName.ToLower() == normalizedDomain)
            .Where(d => d.VerificationStatus == DomainVerificationStatus.Verified)
            .Select(d => new { d.TenantId })
            .FirstOrDefaultAsync(cancellationToken);

        return domain?.TenantId;
    }

    public async Task<TenantDomain> CreateAsync(TenantDomain domain, CancellationToken cancellationToken = default)
    {
        domain.DomainName = NormalizeDomainName(domain.DomainName);
        domain.CreatedAt = DateTime.UtcNow;
        domain.UpdatedAt = DateTime.UtcNow;

        _context.TenantDomains.Add(domain);
        await _context.SaveChangesAsync(cancellationToken);

        return domain;
    }

    public async Task<TenantDomain> UpdateAsync(TenantDomain domain, CancellationToken cancellationToken = default)
    {
        domain.UpdatedAt = DateTime.UtcNow;

        _context.TenantDomains.Update(domain);
        await _context.SaveChangesAsync(cancellationToken);

        return domain;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var domain = await _context.TenantDomains.FindAsync([id], cancellationToken);
        if (domain != null)
        {
            _context.TenantDomains.Remove(domain);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public Task<bool> DomainExistsAsync(string domainName, Guid? excludeDomainId = null, CancellationToken cancellationToken = default)
    {
        var normalizedDomain = NormalizeDomainName(domainName);
        var query = _context.TenantDomains.Where(d => d.DomainName.ToLower() == normalizedDomain);

        if (excludeDomainId.HasValue)
        {
            query = query.Where(d => d.Id != excludeDomainId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task SetPrimaryAsync(Guid tenantId, Guid domainId, CancellationToken cancellationToken = default)
    {
        // Use a transaction to ensure atomicity
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Unset all other primary domains for this tenant
            await _context.TenantDomains
                .Where(d => d.TenantId == tenantId && d.IsPrimary)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(d => d.IsPrimary, false)
                    .SetProperty(d => d.UpdatedAt, DateTime.UtcNow),
                    cancellationToken);

            // Set the new primary domain
            await _context.TenantDomains
                .Where(d => d.Id == domainId && d.TenantId == tenantId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(d => d.IsPrimary, true)
                    .SetProperty(d => d.UpdatedAt, DateTime.UtcNow),
                    cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<IReadOnlyList<TenantDomain>> GetDomainsWithExpiringSslAsync(
        int daysUntilExpiry,
        CancellationToken cancellationToken = default)
    {
        var expiryThreshold = DateTime.UtcNow.AddDays(daysUntilExpiry);
        return await _context.TenantDomains
            .AsNoTracking()
            .Where(d => d.SslStatus == SslStatus.Active)
            .Where(d => d.SslExpiresAt.HasValue && d.SslExpiresAt.Value <= expiryThreshold)
            .Include(d => d.Tenant)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TenantDomain>> GetPendingVerificationDomainsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.TenantDomains
            .AsNoTracking()
            .Where(d => d.VerificationStatus == DomainVerificationStatus.Pending)
            .Where(d => d.Type == DomainType.CustomDomain)
            .Where(d => !d.VerificationExpiresAt.HasValue || d.VerificationExpiresAt.Value > DateTime.UtcNow)
            .Include(d => d.Tenant)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Normalizes a domain name for consistent storage and lookup.
    /// </summary>
    private static string NormalizeDomainName(string domainName)
    {
        return domainName.Trim().ToLowerInvariant();
    }
}
