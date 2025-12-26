using System.Net.Http.Json;
using System.Text.Json.Serialization;
using B2Connect.CatalogService.CQRS;
using B2Connect.CatalogService.CQRS.Queries;
using B2Connect.CatalogService.Models;
using B2Connect.CatalogService.Services;

namespace B2Connect.CatalogService.Providers;

/// <summary>
/// NexPIM Product Provider
/// Integrates with nexPIM (Product Information Management System)
/// Uses nexPIM GraphQL or REST API to fetch product data
/// 
/// Configuration:
/// {
///   "Name": "nexpim",
///   "Enabled": true,
///   "Priority": 10,
///   "BaseUrl": "https://nexpim.example.com/api",
///   "ApiKey": "YOUR_API_KEY",
///   "Settings": {
///     "DataSourceId": "b2connect-catalog",
///     "LanguageField": "language",
///     "UseGraphQL": "false"
///   }
/// }
/// </summary>
public class NexPIMProductProvider : IProductProvider
{
    private readonly HttpClient _httpClient;
    private readonly ProviderConfig _config;
    private readonly ILogger<NexPIMProductProvider> _logger;

    public string ProviderName => "nexpim";

    public NexPIMProductProvider(
        HttpClient httpClient,
        ProviderConfig config,
        ILogger<NexPIMProductProvider> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (!string.IsNullOrEmpty(_config.BaseUrl))
        {
            _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        }

        if (!string.IsNullOrEmpty(_config.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");
        }
    }

    public async Task<bool> IsEnabledAsync(CancellationToken cancellationToken)
    {
        return _config.Enabled && await VerifyConnectivityAsync(cancellationToken);
    }

    public async Task<ProductDto?> GetProductByIdAsync(
        Guid tenantId,
        Guid productId,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching product {ProductId} from nexPIM", productId);

            // NexPIM REST API endpoint
            var url = $"/products/{productId}";
            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Product {ProductId} not found in nexPIM", productId);
                return null;
            }

            var nexPimProduct = await response.Content.ReadAsAsync<NexPIMProductDto>(cancellationToken);
            return MapToProductDto(nexPimProduct, tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product {ProductId} from nexPIM", productId);
            throw;
        }
    }

    public async Task<PagedResult<ProductDto>> GetProductsPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        string? searchTerm = null,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? sortBy = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Fetching paged products from nexPIM (page={Page}, size={PageSize})",
                page, pageSize);

            // Build query parameters
            var queryParams = new Dictionary<string, string>
            {
                { "offset", ((page - 1) * pageSize).ToString() },
                { "limit", pageSize.ToString() }
            };

            if (!string.IsNullOrWhiteSpace(searchTerm))
                queryParams["search"] = searchTerm;

            if (!string.IsNullOrWhiteSpace(category))
                queryParams["category"] = category;

            if (minPrice.HasValue)
                queryParams["minPrice"] = minPrice.Value.ToString();

            if (maxPrice.HasValue)
                queryParams["maxPrice"] = maxPrice.Value.ToString();

            if (!string.IsNullOrWhiteSpace(sortBy))
                queryParams["sortBy"] = sortBy;

            // Build URL with query parameters
            var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
            var url = $"/products?{queryString}";

            using var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<NexPIMListResponse>(cancellationToken);

            var products = result.items
                ?.Select(item => MapToProductDto(item, tenantId))
                .ToList() ?? new List<ProductDto>();

            return new PagedResult<ProductDto>
            {
                Items = products,
                Page = page,
                PageSize = pageSize,
                TotalCount = result.total ?? 0,
                TotalPages = ((result.total ?? 0) + pageSize - 1) / pageSize,
                HasNextPage = page < (((result.total ?? 0) + pageSize - 1) / pageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paged products from nexPIM");
            throw;
        }
    }

    public async Task<PagedResult<ProductDto>> SearchProductsAsync(
        Guid tenantId,
        string searchTerm,
        int page,
        int pageSize,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? language = null,
        CancellationToken cancellationToken = default)
    {
        return await GetProductsPagedAsync(
            tenantId, page, pageSize,
            searchTerm, category, minPrice, maxPrice, null,
            cancellationToken);
    }

    public async Task<int> GetProductCountAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        try
        {
            var url = "/products/count";
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<dynamic>(cancellationToken);
            return int.TryParse(result?.count?.ToString(), out var count) ? count : 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product count from nexPIM");
            return 0;
        }
    }

    public async Task<bool> VerifyConnectivityAsync(CancellationToken cancellationToken)
    {
        try
        {
            var url = "/health";
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "nexPIM connectivity check failed");
            return false;
        }
    }

    public async Task<ProviderMetadata> GetMetadataAsync(CancellationToken cancellationToken)
    {
        try
        {
            var url = "/info";
            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return GetDefaultMetadata();

            var info = await response.Content.ReadAsAsync<dynamic>(cancellationToken);

            return new ProviderMetadata
            {
                Name = "nexPIM",
                Version = info?.version?.ToString() ?? "Unknown",
                IsConnected = true,
                LastSyncTime = DateTime.UtcNow,
                Capabilities = new ProviderCapabilities
                {
                    SupportsFullTextSearch = true,
                    SupportsPriceFiltering = true,
                    SupportsCategoryFiltering = true,
                    SupportsSorting = true,
                    SupportsPagination = true,
                    SupportsMultiLanguage = true,
                    SupportsSync = true
                }
            };
        }
        catch
        {
            return GetDefaultMetadata();
        }
    }

    private ProviderMetadata GetDefaultMetadata()
    {
        return new ProviderMetadata
        {
            Name = "nexPIM",
            Version = "Unknown",
            IsConnected = false,
            Capabilities = new ProviderCapabilities
            {
                SupportsFullTextSearch = true,
                SupportsPriceFiltering = true,
                SupportsCategoryFiltering = true,
                SupportsSorting = true,
                SupportsPagination = true,
                SupportsMultiLanguage = true,
                SupportsSync = true
            }
        };
    }

    private static ProductDto MapToProductDto(NexPIMProductDto nexPimProduct, Guid tenantId)
    {
        return new ProductDto
        {
            Id = Guid.TryParse(nexPimProduct.id, out var id) ? id : Guid.NewGuid(),
            TenantId = tenantId,
            Sku = nexPimProduct.sku ?? "NEXPIM-" + nexPimProduct.id,
            Name = nexPimProduct.name ?? "Unknown",
            Price = nexPimProduct.price ?? 0,
            Description = nexPimProduct.description ?? string.Empty,
            Category = nexPimProduct.category ?? "Uncategorized",
            StockQuantity = nexPimProduct.stock ?? 0,
            IsAvailable = (nexPimProduct.stock ?? 0) > 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}

/// <summary>
/// NexPIM Product DTO
/// </summary>
public class NexPIMProductDto
{
    [JsonPropertyName("id")]
    public string? id { get; set; }

    [JsonPropertyName("name")]
    public string? name { get; set; }

    [JsonPropertyName("sku")]
    public string? sku { get; set; }

    [JsonPropertyName("description")]
    public string? description { get; set; }

    [JsonPropertyName("price")]
    public decimal? price { get; set; }

    [JsonPropertyName("category")]
    public string? category { get; set; }

    [JsonPropertyName("stock")]
    public int? stock { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? imageUrl { get; set; }
}

/// <summary>
/// NexPIM List Response
/// </summary>
public class NexPIMListResponse
{
    [JsonPropertyName("items")]
    public List<NexPIMProductDto>? items { get; set; }

    [JsonPropertyName("total")]
    public int? total { get; set; }
}
