using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace B2Connect.Shared.Infrastructure.ServiceClients;

/// <summary>
/// Client for communication with the Tenancy service
/// Uses Aspire Service Discovery to resolve "http://tenant-service" automatically
///
/// Features:
/// - In-memory caching (5 min for valid tenants, 1 min for not-found)
/// - Retry policy with exponential backoff (3 attempts)
/// - Comprehensive error handling and logging
/// </summary>
public interface ITenancyServiceClient
{
    Task<TenantDto?> GetTenantByIdAsync(Guid tenantId, CancellationToken ct = default);
    Task<TenantDto?> GetTenantByDomainAsync(string domain, CancellationToken ct = default);
    Task<bool> ValidateTenantAccessAsync(Guid tenantId, Guid userId, CancellationToken ct = default);
}

public class TenancyServiceClient : ITenancyServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TenancyServiceClient> _logger;
    private readonly IMemoryCache _cache;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    private static readonly TimeSpan ValidTenantCacheDuration = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan NotFoundCacheDuration = TimeSpan.FromMinutes(1);

    public TenancyServiceClient(
        HttpClient httpClient,
        ILogger<TenancyServiceClient> logger,
        IMemoryCache cache)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cache = cache;

        // Configure retry policy: 3 attempts with exponential backoff
        _retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode && r.StatusCode != System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        "Tenancy service call failed (attempt {RetryCount}/3). Retrying in {Delay}ms. Status: {StatusCode}",
                        retryCount, timespan.TotalMilliseconds, outcome.Result?.StatusCode);
                });
    }

    public async Task<TenantDto?> GetTenantByIdAsync(Guid tenantId, CancellationToken ct = default)
    {
        if (tenantId == Guid.Empty)
        {
            _logger.LogWarning("GetTenantByIdAsync called with empty GUID");
            return null;
        }

        var cacheKey = $"tenant:id:{tenantId}";

        // Try cache first
        if (_cache.TryGetValue<TenantDto?>(cacheKey, out var cachedTenant))
        {
            _logger.LogDebug("Tenant {TenantId} retrieved from cache", tenantId);
            return cachedTenant;
        }

        try
        {
            var response = await _retryPolicy.ExecuteAsync(() =>
                _httpClient.GetAsync($"/api/tenants/{tenantId}", ct));

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogDebug("Tenant {TenantId} not found", tenantId);
                _cache.Set(cacheKey, (TenantDto?)null, NotFoundCacheDuration);
                return null;
            }

            response.EnsureSuccessStatusCode();
            var tenant = await response.Content.ReadFromJsonAsync<TenantDto>(cancellationToken: ct);

            if (tenant != null)
            {
                if (!tenant.IsActive)
                {
                    _logger.LogWarning("Tenant {TenantId} exists but is inactive", tenantId);
                }

                _cache.Set(cacheKey, tenant, ValidTenantCacheDuration);
            }

            return tenant;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get tenant {TenantId} from Tenancy service after retries", tenantId);
            return null;
        }
    }

    public async Task<TenantDto?> GetTenantByDomainAsync(string domain, CancellationToken ct = default)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(domain))
        {
            _logger.LogWarning("GetTenantByDomainAsync called with empty domain");
            return null;
        }

        var normalizedDomain = domain.ToLowerInvariant();
        var cacheKey = $"tenant:domain:{normalizedDomain}";

        // Try cache first
        if (_cache.TryGetValue<TenantDto?>(cacheKey, out var cachedTenant))
        {
            _logger.LogDebug("Tenant for domain {Domain} retrieved from cache", normalizedDomain);
            return cachedTenant;
        }

        try
        {
            var response = await _retryPolicy.ExecuteAsync(() =>
                _httpClient.GetAsync($"/api/tenants/by-domain/{Uri.EscapeDataString(normalizedDomain)}", ct));

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogDebug("No tenant found for domain {Domain}", normalizedDomain);
                // Cache negative results to prevent DoS
                _cache.Set(cacheKey, (TenantDto?)null, NotFoundCacheDuration);
                return null;
            }

            response.EnsureSuccessStatusCode();
            var tenant = await response.Content.ReadFromJsonAsync<TenantDto>(cancellationToken: ct);

            if (tenant != null)
            {
                if (!tenant.IsActive)
                {
                    _logger.LogWarning("Tenant {TenantId} for domain {Domain} exists but is inactive",
                        tenant.Id, normalizedDomain);
                }

                // Cache valid tenants for longer
                _cache.Set(cacheKey, tenant, ValidTenantCacheDuration);

                // Also cache by tenant ID for faster lookups
                var idCacheKey = $"tenant:id:{tenant.Id}";
                _cache.Set(idCacheKey, tenant, ValidTenantCacheDuration);
            }

            return tenant;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get tenant by domain {Domain} from Tenancy service after retries", normalizedDomain);
            return null;
        }
    }

    public async Task<bool> ValidateTenantAccessAsync(Guid tenantId, Guid userId, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/tenants/{tenantId}/validate-access",
                new { UserId = userId }, ct);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to validate tenant access for {TenantId}, {UserId}", tenantId, userId);
            return false;
        }
    }
}

public record TenantDto(Guid Id, string Name, string Domain, bool IsActive);
