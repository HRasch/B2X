using B2X.Tenancy.Models;
using B2X.Types.Domain;

namespace B2X.Tenancy.Repositories;

/// <summary>
/// Repository interface for Tenant aggregate operations.
/// </summary>
public interface ITenantRepository
{
    /// <summary>
    /// Gets a tenant by ID.
    /// </summary>
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a tenant by slug.
    /// </summary>
    Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tenants with optional filtering.
    /// </summary>
    Task<IReadOnlyList<Tenant>> GetAllAsync(
        TenantStatus? status = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tenants with pagination.
    /// </summary>
    Task<(IReadOnlyList<Tenant> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        TenantStatus? status = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new tenant.
    /// </summary>
    Task<Tenant> CreateAsync(Tenant tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing tenant.
    /// </summary>
    Task<Tenant> UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a slug is already in use.
    /// </summary>
    Task<bool> SlugExistsAsync(string slug, Guid? excludeTenantId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of tenants.
    /// </summary>
    Task<int> CountAsync(TenantStatus? status = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for TenantDomain operations.
/// </summary>
public interface ITenantDomainRepository
{
    /// <summary>
    /// Gets a domain by ID.
    /// </summary>
    Task<TenantDomain?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a domain by domain name (case-insensitive).
    /// </summary>
    Task<TenantDomain?> GetByDomainNameAsync(string domainName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all domains for a tenant.
    /// </summary>
    Task<IReadOnlyList<TenantDomain>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the primary domain for a tenant.
    /// </summary>
    Task<TenantDomain?> GetPrimaryDomainAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves a tenant ID from a domain name.
    /// Returns null if domain is not found or not active.
    /// </summary>
    Task<Guid?> ResolveTenantIdAsync(string domainName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new domain for a tenant.
    /// </summary>
    Task<TenantDomain> CreateAsync(TenantDomain domain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing domain.
    /// </summary>
    Task<TenantDomain> UpdateAsync(TenantDomain domain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a domain.
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a domain name is already in use.
    /// </summary>
    Task<bool> DomainExistsAsync(string domainName, Guid? excludeDomainId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a domain as primary (unsets others for the tenant).
    /// </summary>
    Task SetPrimaryAsync(Guid tenantId, Guid domainId, CancellationToken cancellationToken = default);

    // SSL methods will be implemented in Phase 3
    // /// <summary>
    // /// Gets domains with expiring SSL certificates.
    // /// </summary>
    // Task<IReadOnlyList<TenantDomain>> GetDomainsWithExpiringSslAsync(
    //     int daysUntilExpiry,
    //     CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets domains pending verification that haven't expired.
    /// </summary>
    Task<IReadOnlyList<TenantDomain>> GetPendingVerificationDomainsAsync(
        CancellationToken cancellationToken = default);
}
