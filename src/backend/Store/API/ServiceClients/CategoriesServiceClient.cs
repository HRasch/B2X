using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace B2X.Store.ServiceClients;

/// <summary>
/// Client for communication with the Categories service
/// Uses Aspire Service Discovery to resolve "http://categories-service" automatically
/// </summary>
public interface ICategoriesServiceClient
{
    Task<CategoryDto?> GetCategoryByIdAsync(Guid id, Guid tenantId, CancellationToken ct = default);
    Task<CategoryDto?> GetCategoryBySlugAsync(string slug, Guid tenantId, CancellationToken ct = default);
    Task<PagedResult<CategoryDto>> GetCategoriesAsync(Guid tenantId, Guid? parentId = null, int page = 1, int pageSize = 20, CancellationToken ct = default);
    Task<List<CategoryDto>> GetCategoryTreeAsync(Guid tenantId, CancellationToken ct = default);
}

public class CategoriesServiceClient : ICategoriesServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CategoriesServiceClient> _logger;

    public CategoriesServiceClient(HttpClient httpClient, ILogger<CategoriesServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id, Guid tenantId, CancellationToken ct = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());
            var response = await _httpClient.GetAsync($"/api/categories/{id}", ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CategoryDto>(cancellationToken: ct).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get category {Id} from Categories service", id);
            return null;
        }
    }

    public async Task<CategoryDto?> GetCategoryBySlugAsync(string slug, Guid tenantId, CancellationToken ct = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());
            var response = await _httpClient.GetAsync($"/api/categories/slug/{slug}", ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CategoryDto>(cancellationToken: ct).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get category by slug {Slug} from Categories service", slug);
            return null;
        }
    }

    public async Task<PagedResult<CategoryDto>> GetCategoriesAsync(Guid tenantId, Guid? parentId = null, int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());
            var url = $"/api/categories?page={page}&pageSize={pageSize}";
            if (parentId.HasValue)
            {
                url += $"&parentId={parentId}";
            }
            var response = await _httpClient.GetAsync(url, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PagedResult<CategoryDto>>(cancellationToken: ct).ConfigureAwait(false) ?? new PagedResult<CategoryDto> { Items = new List<CategoryDto>() };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get categories from Categories service");
            return new PagedResult<CategoryDto> { Items = new List<CategoryDto>() };
        }
    }

    public async Task<List<CategoryDto>> GetCategoryTreeAsync(Guid tenantId, CancellationToken ct = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());
            var response = await _httpClient.GetAsync("/api/categories/tree", ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<CategoryDto>>(cancellationToken: ct).ConfigureAwait(false) ?? new List<CategoryDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to get category tree from Categories service");
            return new List<CategoryDto>();
        }
    }
}
