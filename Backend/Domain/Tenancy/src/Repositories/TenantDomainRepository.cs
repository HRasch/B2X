using B2Connect.Tenancy.Infrastructure.Data;
using B2Connect.Tenancy.Models;
using B2Connect.Types.Domain;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.Tenancy.Repositories;

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
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public Task<TenantDomain?> GetByDomainNameAsync(string domainName, CancellationToken cancellationToken = default)
    {
        var normalizedDomain = domainName.ToLowerInvariant();
        return _context.TenantDomains
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.DomainName.ToLower() == normalizedDomain, cancellationToken);
    }

    public async Task<IReadOnlyList<TenantDomain>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.TenantDomains
            .AsNoTracking()
            .Where(d => d.TenantId == tenantId)
            .OrderBy(d => d.IsPrimary ? 0 : 1)
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
        var normalizedDomain = domainName.ToLowerInvariant();
        var domain = await _context.TenantDomains
            .AsNoTracking()
            .Include(d => d.Tenant)
            .FirstOrDefaultAsync(d =>
                d.DomainName.ToLower() == normalizedDomain &&
                d.VerificationStatus == DomainVerificationStatus.Verified &&
                d.Tenant.Status == TenantStatus.Active,
                cancellationToken);

        return domain?.TenantId;
    }

    public async Task<TenantDomain> CreateAsync(TenantDomain domain, CancellationToken cancellationToken = default)
    {
        _context.TenantDomains.Add(domain);
        await _context.SaveChangesAsync(cancellationToken);
        return domain;
    }

    public async Task<TenantDomain> UpdateAsync(TenantDomain domain, CancellationToken cancellationToken = default)
    {
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
        var normalizedDomain = domainName.ToLowerInvariant();
        var query = _context.TenantDomains.AsNoTracking()
            .Where(d => d.DomainName.ToLower() == normalizedDomain);

        if (excludeDomainId.HasValue)
        {
            query = query.Where(d => d.Id != excludeDomainId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task SetPrimaryAsync(Guid tenantId, Guid domainId, CancellationToken cancellationToken = default)
    {
        // Unset all primary domains for this tenant
        var tenantDomains = await _context.TenantDomains
            .Where(d => d.TenantId == tenantId)
            .ToListAsync(cancellationToken);

        foreach (var domain in tenantDomains)
        {
            domain.IsPrimary = false;
        }

        // Set the specified domain as primary
        var primaryDomain = tenantDomains.FirstOrDefault(d => d.Id == domainId);
        if (primaryDomain != null)
        {
            primaryDomain.IsPrimary = true;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    // SSL methods will be implemented in Phase 3
    // public Task<IReadOnlyList<TenantDomain>> GetDomainsWithExpiringSslAsync(
    //     int daysUntilExpiry,
    //     CancellationToken cancellationToken = default)
    // {
    //     var expiryDate = DateTime.UtcNow.AddDays(daysUntilExpiry);
    //     return _context.TenantDomains
    //         .AsNoTracking()
    //         .Where(d =>
    //             d.SslStatus == SslStatus.Active &&
    //             d.SslCertificateExpiry.HasValue &&
    //             d.SslCertificateExpiry.Value <= expiryDate)
    //         .ToListAsync(cancellationToken);
    // }

    public async Task<IReadOnlyList<TenantDomain>> GetPendingVerificationDomainsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.TenantDomains
            .AsNoTracking()
            .Where(d =>
                d.VerificationStatus == DomainVerificationStatus.Pending &&
                d.VerificationExpiresAt.HasValue &&
                d.VerificationExpiresAt.Value > DateTime.UtcNow)
            .OrderBy(d => d.LastVerificationCheck ?? d.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}