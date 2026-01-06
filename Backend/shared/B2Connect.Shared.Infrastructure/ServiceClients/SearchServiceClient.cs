using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace B2Connect.Shared.Infrastructure.ServiceClients;

/// <summary>
/// Client for communication with the Search service
/// </summary>
public interface ISearchServiceClient
{
    Task<SearchResponseDto> SearchProductsAsync(SearchRequestDto request, Guid tenantId, CancellationToken ct = default);
}

public class SearchServiceClient : ISearchServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SearchServiceClient> _logger;

    public SearchServiceClient(HttpClient httpClient, ILogger<SearchServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<SearchResponseDto> SearchProductsAsync(SearchRequestDto request, Guid tenantId, CancellationToken ct = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId.ToString());
            var response = await _httpClient.PostAsJsonAsync("/api/search/products", request, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SearchResponseDto>(cancellationToken: ct)
                   ?? new SearchResponseDto(Array.Empty<ProductDocument>(), 0, 0, 0);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to search products with query {Query}", request.Query);
            return new SearchResponseDto(Array.Empty<ProductDocument>(), 0, 0, 0);
        }
    }
}

// DTOs for Search Service communication
public record SearchRequestDto(
    string Query,
    int Page = 1,
    int PageSize = 20,
    string? Sector = null,
    string? Locale = null
);

public record SearchResponseDto(
    IEnumerable<ProductDocument> Products,
    int Total,
    int Page,
    int PageSize
);

public record ProductDocument(
    string Id,
    string Title,
    string Description,
    string Sector,
    decimal Price,
    bool Available,
    string Locale = "en"
);
