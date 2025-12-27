using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace B2Connect.Shared.Infrastructure.ServiceClients;

/// <summary>
/// Client for communication with the Tenancy service
/// Uses Aspire Service Discovery to resolve "http://tenant-service" automatically
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

    public TenancyServiceClient(HttpClient httpClient, ILogger<TenancyServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<TenantDto?> GetTenantByIdAsync(Guid tenantId, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/tenants/{tenantId}", ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TenantDto>(cancellationToken: ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get tenant {TenantId} from Tenancy service", tenantId);
            return null;
        }
    }

    public async Task<TenantDto?> GetTenantByDomainAsync(string domain, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/tenants/by-domain/{domain}", ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TenantDto>(cancellationToken: ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get tenant by domain {Domain} from Tenancy service", domain);
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
