using Microsoft.Extensions.Caching.Memory;

namespace B2X.ReverseProxy.Services;

/// <summary>
/// Resolves tenant information from domain names using:
/// 1. Subdomain pattern matching (tenant1.b2xgate.com)
/// 2. Custom domain lookup from database/cache
/// </summary>
public class TenantDomainResolver : ITenantDomainResolver
{
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TenantDomainResolver> _logger;

    private readonly string _baseDomain;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public TenantDomainResolver(
        IMemoryCache cache,
        IConfiguration configuration,
        ILogger<TenantDomainResolver> logger)
    {
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
        _baseDomain = configuration.GetValue<string>("Multitenancy:BaseDomain") ?? "b2xgate.com";
    }

    public async Task<TenantInfo?> ResolveAsync(string domain, CancellationToken cancellationToken = default)
    {
        // Check cache first
        var cacheKey = $"tenant:domain:{domain.ToLowerInvariant()}";

        if (_cache.TryGetValue<TenantInfo>(cacheKey, out var cachedTenant))
        {
            return cachedTenant;
        }

        // Try subdomain pattern first
        var tenant = TryResolveFromSubdomain(domain);

        // If not a subdomain, try custom domain lookup
        tenant ??= await ResolveFromCustomDomainAsync(domain, cancellationToken);

        if (tenant is not null)
        {
            // Cache the result
            _cache.Set(cacheKey, tenant, _cacheExpiration);
        }

        return tenant;
    }

    /// <summary>
    /// Tries to resolve tenant from subdomain pattern.
    /// Example: tenant1.b2xgate.com → tenant1
    /// </summary>
    private TenantInfo? TryResolveFromSubdomain(string domain)
    {
        // Check if domain matches pattern: {slug}.{baseDomain}
        if (!domain.EndsWith($".{_baseDomain}", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var slug = domain[..^(_baseDomain.Length + 1)];

        // Validate slug (no dots, valid characters)
        if (string.IsNullOrWhiteSpace(slug) || slug.Contains('.'))
        {
            return null;
        }

        _logger.LogDebug("Resolved tenant slug '{Slug}' from subdomain pattern", slug);

        // For subdomain-based tenants, we create a deterministic ID from slug
        // In production, this would still lookup the actual tenant from DB
        return new TenantInfo(
            TenantId: CreateDeterministicGuid(slug),
            Slug: slug.ToLowerInvariant(),
            DisplayName: slug,
            Status: TenantStatus.Active);
    }

    /// <summary>
    /// Resolves tenant from custom domain mapping in database.
    /// </summary>
    private async Task<TenantInfo?> ResolveFromCustomDomainAsync(
        string domain,
        CancellationToken cancellationToken)
    {
        // TODO: Implement actual database lookup via Management API or direct DB access
        // For now, check configuration for custom domain mappings

        var customDomains = _configuration
            .GetSection("Multitenancy:CustomDomains")
            .Get<Dictionary<string, CustomDomainMapping>>();

        if (customDomains?.TryGetValue(domain.ToLowerInvariant(), out var mapping) == true)
        {
            _logger.LogDebug(
                "Resolved tenant '{TenantSlug}' from custom domain mapping for {Domain}",
                mapping.TenantSlug,
                domain);

            return new TenantInfo(
                TenantId: mapping.TenantId,
                Slug: mapping.TenantSlug,
                DisplayName: mapping.DisplayName,
                Status: TenantStatus.Active);
        }

        // In production: Query Management API or Database
        // var response = await _httpClient.GetAsync($"/api/tenants/by-domain/{domain}");
        // ...

        _logger.LogDebug("No tenant mapping found for custom domain: {Domain}", domain);
        return null;
    }

    public void InvalidateCache(string domain)
    {
        var cacheKey = $"tenant:domain:{domain.ToLowerInvariant()}";
        _cache.Remove(cacheKey);
        _logger.LogInformation("Invalidated cache for domain: {Domain}", domain);
    }

    public void InvalidateAllCache()
    {
        // IMemoryCache doesn't support clearing all entries
        // In production, use IDistributedCache with Redis and pattern-based deletion
        _logger.LogWarning("InvalidateAllCache called - requires IDistributedCache implementation");
    }

    /// <summary>
    /// Creates a deterministic GUID from a string.
    /// Used for development/testing when actual tenant ID is not available.
    /// </summary>
    private static Guid CreateDeterministicGuid(string input)
    {
        var hash = System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(input.ToLowerInvariant()));
        return new Guid(hash.AsSpan()[..16]);
    }
}

/// <summary>
/// Custom domain mapping configuration.
/// </summary>
public record CustomDomainMapping(
    Guid TenantId,
    string TenantSlug,
    string DisplayName);
