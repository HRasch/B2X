namespace B2X.ReverseProxy.Services;

/// <summary>
/// Service for resolving tenant information from domain names.
/// </summary>
public interface ITenantDomainResolver
{
    /// <summary>
    /// Resolves tenant information from a domain name.
    /// </summary>
    /// <param name="domain">The domain name (e.g., "tenant1.b2xgate.com" or "custom-domain.de")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tenant information if found, null otherwise</returns>
    Task<TenantInfo?> ResolveAsync(string domain, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates the cache for a specific domain.
    /// Called when tenant domain mappings are updated via Management API.
    /// </summary>
    /// <param name="domain">The domain to invalidate</param>
    void InvalidateCache(string domain);

    /// <summary>
    /// Invalidates all cached domain mappings.
    /// </summary>
    void InvalidateAllCache();
}

/// <summary>
/// Resolved tenant information.
/// </summary>
public record TenantInfo(
    Guid TenantId,
    string Slug,
    string DisplayName,
    TenantStatus Status);

/// <summary>
/// Tenant status enumeration.
/// </summary>
public enum TenantStatus
{
    Active,
    Suspended,
    Trial,
    Inactive
}
