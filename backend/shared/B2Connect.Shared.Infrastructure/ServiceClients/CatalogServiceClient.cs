using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace B2Connect.Shared.Infrastructure.ServiceClients;

/// <summary>
/// Client for communication with the Catalog service
/// Uses Aspire Service Discovery to resolve "http://catalog-service" automatically
/// </summary>
public interface ICatalogServiceClient
{
    Task<ProductDto?> GetProductBySkuAsync(string sku, Guid tenantId, CancellationToken ct = default);
    Task<IEnumerable<ProductDto>> SearchProductsAsync(string query, Guid tenantId, CancellationToken ct = default);
}

public class CatalogServiceClient : ICatalogServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CatalogServiceClient> _logger;

    public CatalogServiceClient(HttpClient httpClient, ILogger<CatalogServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ProductDto?> GetProductBySkuAsync(string sku, Guid tenantId, CancellationToken ct = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());
            var response = await _httpClient.GetAsync($"/api/products/sku/{sku}", ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken: ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get product {Sku} from Catalog service", sku);
            return null;
        }
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string query, Guid tenantId, CancellationToken ct = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());
            var response = await _httpClient.GetAsync($"/api/products/search?q={Uri.EscapeDataString(query)}", ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>(cancellationToken: ct)
                   ?? Enumerable.Empty<ProductDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to search products with query {Query}", query);
            return Enumerable.Empty<ProductDto>();
        }
    }
}

public record ProductDto(Guid Id, string Sku, string Name, decimal Price, Guid TenantId);
