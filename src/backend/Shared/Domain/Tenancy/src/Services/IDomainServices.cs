namespace B2X.Tenancy.Services;

/// <summary>
/// Service for looking up tenant IDs from domain names with caching.
/// </summary>
public interface IDomainLookupService
{
    /// <summary>
    /// Resolves a tenant ID from a domain name.
    /// Uses multi-level caching for performance.
    /// </summary>
    /// <param name="domainName">The domain name to look up.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The tenant ID, or null if not found.</returns>
    Task<Guid?> ResolveTenantIdAsync(string domainName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates the cache for a specific domain.
    /// Called when domain is added, updated, or removed.
    /// </summary>
    Task InvalidateCacheAsync(string domainName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates all cached domain entries for a tenant.
    /// </summary>
    Task InvalidateTenantCacheAsync(Guid tenantId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for verifying domain ownership via DNS records.
/// </summary>
public interface IDnsVerificationService
{
    /// <summary>
    /// Verifies that a domain has the correct DNS TXT record.
    /// </summary>
    /// <param name="domainName">The domain to verify.</param>
    /// <param name="expectedToken">The expected verification token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Verification result.</returns>
    Task<DnsVerificationResult> VerifyDomainAsync(
        string domainName,
        string expectedToken,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of DNS verification.
/// </summary>
public record DnsVerificationResult(
    bool IsVerified,
    string? FailureReason = null,
    string? FoundValue = null);
