using B2Connect.Shared.Monitoring;
using B2Connect.Shared.Monitoring.Abstractions;
using B2Connect.Shared.Monitoring.Models;

namespace B2Connect.CLI.Services;

/// <summary>
/// HTTP client for communicating with the B2Connect Monitoring service.
/// </summary>
public sealed class MonitoringServiceClient : IDisposable
{
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;
    private bool _disposed;

    public MonitoringServiceClient()
    {
        var config = new ConfigurationService();
        _baseUrl = config.GetServiceUrl("monitoring") ?? "http://localhost:8090";
        _httpClient = new HttpClient { BaseAddress = new Uri(_baseUrl) };
    }

    /// <summary>
    /// Register a service for monitoring.
    /// </summary>
    public async Task<bool> RegisterServiceAsync(ConnectedService service)
    {
        try
        {
            var request = new RegisterServiceRequest
            {
                Name = service.Name,
                Type = service.Type,
                Endpoint = service.Endpoint,
                TenantId = service.TenantId
            };

            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("/api/monitoring/services", content);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get service status by ID.
    /// </summary>
    public async Task<ConnectedService?> GetServiceStatusAsync(Guid serviceId, string tenantId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/monitoring/services/{serviceId}?tenantId={tenantId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<ConnectedService>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Get all services for a tenant.
    /// </summary>
    public async Task<IEnumerable<ConnectedService>> GetServicesAsync(string tenantId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/monitoring/services?tenantId={tenantId}");

            if (!response.IsSuccessStatusCode)
            {
                return Array.Empty<ConnectedService>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var services = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ConnectedService>>(
                json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return services ?? Array.Empty<ConnectedService>();
        }
        catch
        {
            return Array.Empty<ConnectedService>();
        }
    }

    /// <summary>
    /// Test connectivity to a service.
    /// </summary>
    public async Task<ServiceTestResult> TestServiceConnectivityAsync(Guid serviceId, string tenantId)
    {
        try
        {
            var response = await _httpClient.PostAsync(
                $"/api/monitoring/services/{serviceId}/test?tenantId={tenantId}",
                null);

            if (!response.IsSuccessStatusCode)
            {
                return new ServiceTestResult
                {
                    IsSuccessful = false,
                    ErrorMessage = "Test failed",
                    LatencyMs = 0
                };
            }

            var json = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<ServiceTestResult>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new ServiceTestResult { IsSuccessful = false, ErrorMessage = "Failed to parse response" };
        }
        catch (Exception ex)
        {
            return new ServiceTestResult
            {
                IsSuccessful = false,
                ErrorMessage = ex.Message,
                LatencyMs = 0
            };
        }
    }

    /// <summary>
    /// Get resource alerts for monitoring.
    /// </summary>
    public async Task<IEnumerable<dynamic>> GetResourceAlertsAsync(Dictionary<string, object> filters)
    {
        try
        {
            var queryString = string.Join("&", filters.Select(f => $"{f.Key}={Uri.EscapeDataString(f.Value.ToString() ?? "")}"));
            var response = await _httpClient.GetAsync($"/api/monitoring/alerts?{queryString}");

            if (!response.IsSuccessStatusCode)
            {
                return Array.Empty<dynamic>();
            }

            var json = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<dynamic>>(json) ?? Array.Empty<dynamic>();
        }
        catch
        {
            return Array.Empty<dynamic>();
        }
    }

    /// <summary>
    /// Disposes of the HTTP client resources.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient.Dispose();
            _disposed = true;
        }
    }
}

/// <summary>
/// Request model for registering a service.
/// </summary>
public record RegisterServiceRequest
{
    public required string Name { get; init; }
    public required ServiceType Type { get; init; }
    public required string Endpoint { get; init; }
    public required string TenantId { get; init; }
}
