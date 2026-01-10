using System.Net.Http.Json;
using B2X.Variants.Handlers;
using Microsoft.Extensions.Logging;

namespace B2X.Store.ServiceClients;

/// <summary>
/// Client for communication with the Variants service
/// Uses Aspire Service Discovery to resolve "http://variants-service" automatically
/// </summary>
public interface IVariantsServiceClient
{
    Task<B2X.Variants.Models.VariantDto?> GetVariantBySkuAsync(string sku, Guid tenantId, CancellationToken ct = default);
    Task<B2X.Variants.Models.VariantDto?> GetVariantByIdAsync(Guid id, Guid tenantId, CancellationToken ct = default);
    Task<PagedResult<B2X.Variants.Models.VariantDto>> GetVariantsByProductAsync(Guid productId, Guid tenantId, int page = 1, int pageSize = 20, CancellationToken ct = default);
    Task<PagedResult<B2X.Variants.Models.VariantDto>> SearchVariantsAsync(string query, Guid tenantId, int page = 1, int pageSize = 20, CancellationToken ct = default);
}

public class VariantsServiceClient : IVariantsServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<VariantsServiceClient> _logger;

    public VariantsServiceClient(HttpClient httpClient, ILogger<VariantsServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<B2X.Variants.Models.VariantDto?> GetVariantBySkuAsync(string sku, Guid tenantId, CancellationToken ct = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());
            var response = await _httpClient.GetAsync($"/api/variants/sku/{sku}", ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Variants.Models.VariantDto>(cancellationToken: ct).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get variant {Sku} from Variants service", sku);
            return null;
        }
    }

    public async Task<B2X.Variants.Models.VariantDto?> GetVariantByIdAsync(Guid id, Guid tenantId, CancellationToken ct = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());
            var response = await _httpClient.GetAsync($"/api/variants/{id}", ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Variants.Models.VariantDto>(cancellationToken: ct).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get variant {Id} from Variants service", id);
            return null;
        }
    }

    public async Task<PagedResult<B2X.Variants.Models.VariantDto>> GetVariantsByProductAsync(Guid productId, Guid tenantId, int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());
            var response = await _httpClient.GetAsync($"/api/variants/product/{productId}?page={page}&pageSize={pageSize}", ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PagedResult<Variants.Models.VariantDto>>(cancellationToken: ct).ConfigureAwait(false) ?? new PagedResult<B2X.Variants.Models.VariantDto> { Items = new List<B2X.Variants.Models.VariantDto>() };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get variants for product {ProductId} from Variants service", productId);
            return new PagedResult<B2X.Variants.Models.VariantDto> { Items = new List<B2X.Variants.Models.VariantDto>() };
        }
    }

    public async Task<PagedResult<B2X.Variants.Models.VariantDto>> SearchVariantsAsync(string query, Guid tenantId, int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());
            var response = await _httpClient.GetAsync($"/api/variants/search?q={Uri.EscapeDataString(query)}&page={page}&pageSize={pageSize}", ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PagedResult<Variants.Models.VariantDto>>(cancellationToken: ct).ConfigureAwait(false) ?? new PagedResult<B2X.Variants.Models.VariantDto> { Items = new List<B2X.Variants.Models.VariantDto>() };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to search variants with query '{Query}' from Variants service", query);
            return new PagedResult<B2X.Variants.Models.VariantDto> { Items = new List<B2X.Variants.Models.VariantDto>() };
        }
    }
}
