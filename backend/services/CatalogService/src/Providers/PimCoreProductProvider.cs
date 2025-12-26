using System.Net.Http.Json;
using System.Text.Json.Serialization;
using B2Connect.CatalogService.CQRS;
using B2Connect.CatalogService.CQRS.Queries;
using B2Connect.CatalogService.Models;
using B2Connect.CatalogService.Services;

namespace B2Connect.CatalogService.Providers;

/// <summary>
/// PimCore Product Provider
/// Integrates with PimCore Product Information Management System
/// Uses PimCore REST API to fetch product data
/// 
/// Configuration:
/// {
///   "Name": "pimcore",
///   "Enabled": true,
///   "Priority": 10,
///   "BaseUrl": "https://pimcore.example.com",
///   "ApiKey": "YOUR_API_KEY",
///   "Settings": {
///     "ObjectClassName": "Product",
///     "LanguageField": "language",
///     "PriceField": "price",
///     "AvailableField": "isAvailable"
///   }
/// }
/// </summary>
public class PimCoreProductProvider : IProductProvider
{
    private readonly HttpClient _httpClient;
    private readonly ProviderConfig _config;
    private readonly ILogger<PimCoreProductProvider> _logger;

    public string ProviderName => "pimcore";

    public PimCoreProductProvider(
        HttpClient httpClient,
        ProviderConfig config,
        ILogger<PimCoreProductProvider> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Configure HttpClient
        if (!string.IsNullOrEmpty(_config.BaseUrl))
        {
            _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        }

        // Add API Key if configured
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
            _logger.LogInformation("Fetching product {ProductId} from PimCore", productId);

            // PimCore API endpoint to fetch single object
            var url = $"/webservice/rest/object?id={productId}&apikey={_config.ApiKey}";

            using var response = await _httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Product {ProductId} not found in PimCore", productId);
                return null;
            }

            var pimCoreProduct = await response.Content.ReadAsAsync<PimCoreProductDto>(cancellationToken);
            return MapToProductDto(pimCoreProduct, tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product {ProductId} from PimCore", productId);
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
                "Fetching paged products from PimCore (page={Page}, size={PageSize})",
                page, pageSize);

            // Build filter query
            var filters = new List<string>
            {
                "classname = 'Product'"  // Filter by Product class
            };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filters.Add($"(name LIKE '%{searchTerm}%' OR description LIKE '%{searchTerm}%')");
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                filters.Add($"category = '{category}'");
            }

            if (minPrice.HasValue)
                filters.Add($"price >= {minPrice.Value}");

            if (maxPrice.HasValue)
                filters.Add($"price <= {maxPrice.Value}");

            var filterQuery = string.Join(" AND ", filters);
            var offset = (page - 1) * pageSize;

            // PimCore list API
            var url = $"/webservice/rest/list?classname=Product&filter={Uri.EscapeDataString(filterQuery)}&offset={offset}&limit={pageSize}";

            using var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<PimCoreListResponse>(cancellationToken);

            var products = result.objects
                ?.Select(obj => MapToProductDto(obj, tenantId))
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
            _logger.LogError(ex, "Error retrieving paged products from PimCore");
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
            var url = "/webservice/rest/list?classname=Product&count=true";
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<PimCoreListResponse>(cancellationToken);
            return result.total ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product count from PimCore");
            return 0;
        }
    }

    public async Task<bool> VerifyConnectivityAsync(CancellationToken cancellationToken)
    {
        try
        {
            var url = "/webservice/rest/server-info";
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PimCore connectivity check failed");
            return false;
        }
    }

    public async Task<ProviderMetadata> GetMetadataAsync(CancellationToken cancellationToken)
    {
        try
        {
            var url = "/webservice/rest/server-info";
            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return GetDefaultMetadata();

            var info = await response.Content.ReadAsAsync<dynamic>(cancellationToken);

            return new ProviderMetadata
            {
                Name = "PimCore",
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
            Name = "PimCore",
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

    private static ProductDto MapToProductDto(PimCoreProductDto pimCoreProduct, Guid tenantId)
    {
        return new ProductDto
        {
            Id = Guid.TryParse(pimCoreProduct.id, out var id) ? id : Guid.NewGuid(),
            TenantId = tenantId,
            Sku = pimCoreProduct.sku ?? "PIMCORE-" + pimCoreProduct.id,
            Name = pimCoreProduct.name ?? "Unknown",
            Price = pimCoreProduct.price ?? 0,
            Description = pimCoreProduct.description ?? string.Empty,
            Category = pimCoreProduct.category ?? "Uncategorized",
            StockQuantity = pimCoreProduct.stock ?? 0,
            IsAvailable = pimCoreProduct.isAvailable ?? false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}

/// <summary>
/// PimCore Product DTO (API Response Model)
/// </summary>
public class PimCoreProductDto
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

    [JsonPropertyName("isAvailable")]
    public bool? isAvailable { get; set; }

    [JsonPropertyName("image")]
    public string? image { get; set; }
}

/// <summary>
/// PimCore List Response
/// </summary>
public class PimCoreListResponse
{
    [JsonPropertyName("objects")]
    public List<PimCoreProductDto>? objects { get; set; }

    [JsonPropertyName("total")]
    public int? total { get; set; }
}
