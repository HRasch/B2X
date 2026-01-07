using System.Text.Json;
using B2Connect.Tenancy.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace B2Connect.Tenancy.Services;

/// <summary>
/// Service for looking up tenant IDs from domain names with multi-level caching.
///
/// Cache Layers:
/// 1. L1: In-memory cache (IMemoryCache) - fastest, process-local
/// 2. L2: Distributed cache (Redis) - shared across instances
/// 3. Database: TenantDomain table - source of truth
/// </summary>
public class DomainLookupService : IDomainLookupService
{
    private readonly ITenantDomainRepository _domainRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<DomainLookupService> _logger;

    private const string CacheKeyPrefix = "tenant:domain:";

    // Cache durations
    private static readonly TimeSpan MemoryCacheDuration = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan DistributedCacheDuration = TimeSpan.FromMinutes(10);

    public DomainLookupService(
        ITenantDomainRepository domainRepository,
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        ILogger<DomainLookupService> logger)
    {
        _domainRepository = domainRepository;
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<Guid?> ResolveTenantIdAsync(string domainName, CancellationToken cancellationToken = default)
    {
        var normalizedDomain = NormalizeDomain(domainName);
        var cacheKey = $"{CacheKeyPrefix}{normalizedDomain}";

        // L1: Check in-memory cache
        if (_memoryCache.TryGetValue(cacheKey, out Guid? cachedTenantId))
        {
            _logger.LogDebug("Domain {Domain} resolved from memory cache: {TenantId}",
                normalizedDomain, cachedTenantId);
            return cachedTenantId;
        }

        // L2: Check distributed cache
        try
        {
            var distributedValue = await _distributedCache.GetStringAsync(cacheKey, cancellationToken).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(distributedValue))
            {
                var tenantId = Guid.Parse(distributedValue);

                // Populate L1 cache
                _memoryCache.Set(cacheKey, (Guid?)tenantId, MemoryCacheDuration);

                _logger.LogDebug("Domain {Domain} resolved from distributed cache: {TenantId}",
                    normalizedDomain, tenantId);
                return tenantId;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read from distributed cache for domain {Domain}", normalizedDomain);
            // Continue to database lookup
        }

        // L3: Database lookup
        var resolvedTenantId = await _domainRepository.ResolveTenantIdAsync(normalizedDomain, cancellationToken).ConfigureAwait(false);

        if (resolvedTenantId.HasValue)
        {
            // Populate caches
            await PopulateCachesAsync(cacheKey, resolvedTenantId.Value, cancellationToken).ConfigureAwait(false);
            _logger.LogDebug("Domain {Domain} resolved from database: {TenantId}",
                normalizedDomain, resolvedTenantId);
        }
        else
        {
            _logger.LogDebug("Domain {Domain} not found in database", normalizedDomain);

            // Cache negative result (short TTL) to prevent repeated DB lookups
            _memoryCache.Set(cacheKey, (Guid?)null, TimeSpan.FromSeconds(30));
        }

        return resolvedTenantId;
    }

    public async Task InvalidateCacheAsync(string domainName, CancellationToken cancellationToken = default)
    {
        var normalizedDomain = NormalizeDomain(domainName);
        var cacheKey = $"{CacheKeyPrefix}{normalizedDomain}";

        // Remove from L1
        _memoryCache.Remove(cacheKey);

        // Remove from L2
        try
        {
            await _distributedCache.RemoveAsync(cacheKey, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to remove domain {Domain} from distributed cache", normalizedDomain);
        }

        _logger.LogInformation("Cache invalidated for domain {Domain}", normalizedDomain);
    }

    public async Task InvalidateTenantCacheAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        // Get all domains for the tenant
        var domains = await _domainRepository.GetByTenantIdAsync(tenantId, cancellationToken).ConfigureAwait(false);

        foreach (var domain in domains)
        {
            await InvalidateCacheAsync(domain.DomainName, cancellationToken).ConfigureAwait(false);
        }

        _logger.LogInformation("Cache invalidated for all domains of tenant {TenantId}", tenantId);
    }

    private async Task PopulateCachesAsync(string cacheKey, Guid tenantId, CancellationToken cancellationToken)
    {
        // Populate L1
        _memoryCache.Set(cacheKey, (Guid?)tenantId, MemoryCacheDuration);

        // Populate L2
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = DistributedCacheDuration
            };

            await _distributedCache.SetStringAsync(cacheKey, tenantId.ToString(), options, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to populate distributed cache for key {CacheKey}", cacheKey);
        }
    }

    private static string NormalizeDomain(string domainName)
    {
        return domainName.Trim().ToLowerInvariant();
    }
}
