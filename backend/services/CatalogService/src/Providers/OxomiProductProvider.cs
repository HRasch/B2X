using B2Connect.CatalogService.CQRS.Queries;
using B2Connect.CatalogService.Models;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace B2Connect.CatalogService.Providers;

/// <summary>
/// Oxomi Product Provider
/// Integrates with Oxomi (PIM/MDM Solution)
/// Uses Oxomi REST API to fetch product data
/// 
/// Configuration:
/// {
///   "Name": "oxomi",
///   "Enabled": true,
///   "Priority": 10,
///   "BaseUrl": "https://oxomi.example.com/api/v2",
///   "ApiKey": "YOUR_API_KEY",
///   "Settings": {
///     "EntityType": "Product",
///     "Environment": "production"
///   }
/// }
/// </summary>
public class OxomiProductProvider : IProductProvider
{
    private readonly HttpClient _httpClient;
    private readonly ProviderConfig _config;
    private readonly ILogger<OxomiProductProvider> _logger;

    public string ProviderName => "oxomi";

    public OxomiProductProvider(
        HttpClient httpClient,
        ProviderConfig config,
        ILogger<OxomiProductProvider> logger)
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
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _config.ApiKey);
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
            _logger.LogInformation("Fetching product {ProductId} from Oxomi", productId);

            // Oxomi API endpoint: /entities/{entityType}/{id}
            var url = $"/entities/Product/{productId}";
            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Product {ProductId} not found in Oxomi", productId);
                return null;
            }

            var oxomiProduct = await response.Content.ReadAsAsync<OxomiProductDto>(cancellationToken);
            return MapToProductDto(oxomiProduct, tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product {ProductId} from Oxomi", productId);
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
                "Fetching paged products from Oxomi (page={Page}, size={PageSize})",
                page, pageSize);

            // Build filter query for Oxomi
            var filters = new List<OxomiFilter>();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filters.Add(new OxomiFilter
                {
                    field = "name",
                    @operator = "contains",
                    value = searchTerm
                });
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                filters.Add(new OxomiFilter
                {
                    field = "category",
                    @operator = "equals",
                    value = category
                });
            }

            if (minPrice.HasValue)
            {
                filters.Add(new OxomiFilter
                {
                    field = "price",
                    @operator = "gte",
                    value = minPrice.Value.ToString()
                });
            }

            if (maxPrice.HasValue)
            {
                filters.Add(new OxomiFilter
                {
                    field = "price",
                    @operator = "lte",
                    value = maxPrice.Value.ToString()
                });
            }

            // Build request body
            var request = new OxomiQueryRequest
            {
                entityType = "Product",
                offset = (page - 1) * pageSize,
                limit = pageSize,
                filters = filters,
                sortBy = sortBy ?? "name"
            };

            using var response = await _httpClient.PostAsJsonAsync(
                "/entities/search",
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<OxomiSearchResponse>(cancellationToken);

            var products = result.results
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
            _logger.LogError(ex, "Error retrieving paged products from Oxomi");
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
            var request = new OxomiQueryRequest
            {
                entityType = "Product",
                limit = 1
            };

            using var response = await _httpClient.PostAsJsonAsync(
                "/entities/search",
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<OxomiSearchResponse>(cancellationToken);
            return result.total ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product count from Oxomi");
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
            _logger.LogError(ex, "Oxomi connectivity check failed");
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
                Name = "Oxomi",
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
            Name = "Oxomi",
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

    private static ProductDto MapToProductDto(OxomiProductDto oxomiProduct, Guid tenantId)
    {
        return new ProductDto
        {
            Id = Guid.TryParse(oxomiProduct.id, out var id) ? id : Guid.NewGuid(),
            TenantId = tenantId,
            Sku = oxomiProduct.sku ?? "OXOMI-" + oxomiProduct.id,
            Name = oxomiProduct.name ?? "Unknown",
            Price = oxomiProduct.price ?? 0,
            Description = oxomiProduct.description ?? string.Empty,
            Category = oxomiProduct.category ?? "Uncategorized",
            StockQuantity = oxomiProduct.stock ?? 0,
            IsAvailable = (oxomiProduct.stock ?? 0) > 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Oxomi Product DTO
/// </summary>
public class OxomiProductDto
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
/// Oxomi Search Request
/// </summary>
public class OxomiQueryRequest
{
    [JsonPropertyName("entityType")]
    public string entityType { get; set; } = "Product";

    [JsonPropertyName("filters")]
    public List<OxomiFilter>? filters { get; set; }

    [JsonPropertyName("offset")]
    public int offset { get; set; }

    [JsonPropertyName("limit")]
    public int limit { get; set; }

    [JsonPropertyName("sortBy")]
    public string sortBy { get; set; } = "name";
}

/// <summary>
/// Oxomi Filter
/// </summary>
public class OxomiFilter
{
    [JsonPropertyName("field")]
    public string field { get; set; } = string.Empty;

    [JsonPropertyName("operator")]
    public string @operator { get; set; } = "equals";

    [JsonPropertyName("value")]
    public string value { get; set; } = string.Empty;
}

/// <summary>
/// Oxomi Search Response
/// </summary>
public class OxomiSearchResponse
{
    [JsonPropertyName("results")]
    public List<OxomiProductDto>? results { get; set; }

    [JsonPropertyName("total")]
    public int? total { get; set; }
}
