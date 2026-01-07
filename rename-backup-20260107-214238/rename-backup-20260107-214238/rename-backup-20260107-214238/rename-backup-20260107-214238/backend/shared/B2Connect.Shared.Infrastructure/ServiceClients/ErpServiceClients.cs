using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace B2Connect.Shared.Infrastructure.ServiceClients;

/// <summary>
/// Client for communication with the Customer service
/// </summary>
public interface ICustomerServiceClient
{
    Task<CustomerDto?> GetCustomerByErpIdAsync(string erpCustomerId, Guid tenantId, CancellationToken ct = default);
}

public class CustomerServiceClient : ICustomerServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CustomerServiceClient> _logger;

    public CustomerServiceClient(HttpClient httpClient, ILogger<CustomerServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CustomerDto?> GetCustomerByErpIdAsync(string erpCustomerId, Guid tenantId, CancellationToken ct = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());
            var response = await _httpClient.GetAsync($"/api/customers/erp/{erpCustomerId}", ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CustomerDto>(cancellationToken: ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get customer {ErpCustomerId} from Customer service", erpCustomerId);
            return null;
        }
    }
}

/// <summary>
/// Client for communication with the Usage service
/// </summary>
public interface IUsageServiceClient
{
    Task<UsageStatsDto> GetUsageStatsAsync(Guid tenantId, DateTime? from = null, DateTime? to = null, CancellationToken ct = default);
}

public class UsageServiceClient : IUsageServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UsageServiceClient> _logger;

    public UsageServiceClient(HttpClient httpClient, ILogger<UsageServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<UsageStatsDto> GetUsageStatsAsync(Guid tenantId, DateTime? from = null, DateTime? to = null, CancellationToken ct = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());

            var query = new List<string>();
            if (from.HasValue)
                query.Add($"from={from.Value:O}");
            if (to.HasValue)
                query.Add($"to={to.Value:O}");
            var queryString = query.Any() ? $"?{string.Join("&", query)}" : "";

            var response = await _httpClient.GetAsync($"/api/usage/stats{queryString}", ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UsageStatsDto>(cancellationToken: ct) ?? new UsageStatsDto();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get usage stats from Usage service");
            return new UsageStatsDto(); // Return empty stats on error
        }
    }
}

/// <summary>
/// Client for communication with the Access service
/// </summary>
public interface IAccessServiceClient
{
    Task UpdateUserAccessAsync(Guid tenantId, string userId, List<string> permissions, string updatedBy, CancellationToken ct = default);
}

public class AccessServiceClient : IAccessServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AccessServiceClient> _logger;

    public AccessServiceClient(HttpClient httpClient, ILogger<AccessServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task UpdateUserAccessAsync(Guid tenantId, string userId, List<string> permissions, string updatedBy, CancellationToken ct = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());

            var request = new
            {
                UserId = userId,
                Permissions = permissions,
                UpdatedBy = updatedBy
            };

            var response = await _httpClient.PutAsJsonAsync($"/api/access/users/{userId}", request, ct);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to update access for user {UserId}", userId);
            throw;
        }
    }
}

// DTOs for ERP service communication
public class CustomerDto
{
    public Guid Id { get; set; }
    public string? ErpCustomerId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public Address? Address { get; set; }
    public DateTime LastModified { get; set; }
}

public class Address
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
}

public class UsageStatsDto
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int ActiveCustomers { get; set; }
    public List<ProductStatsDto> TopProducts { get; set; } = new List<ProductStatsDto>();
}

public class ProductStatsDto
{
    public Guid ProductId { get; set; }
    public string? Name { get; set; }
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
}
