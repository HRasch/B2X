using B2Connect.CatalogService.CQRS;
using B2Connect.CatalogService.CQRS.Queries;
using B2Connect.CatalogService.Models;
using B2Connect.CatalogService.Services;

namespace B2Connect.CatalogService.Providers;

/// <summary>
/// Product Provider Interface
/// Abstraction for different product data sources (internal DB, PimCore, nexPIM, Oxomi, etc.)
/// Supports querying and searching products from different sources
/// </summary>
public interface IProductProvider
{
    /// <summary>
    /// Unique identifier for the provider (e.g., "internal", "pimcore", "nexpim", "oxomi")
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Indicates if this provider is currently enabled/configured
    /// </summary>
    Task<bool> IsEnabledAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get a single product by ID from this provider
    /// Returns null if not found
    /// </summary>
    Task<ProductDto?> GetProductByIdAsync(
        Guid tenantId,
        Guid productId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Get paginated products with filtering
    /// </summary>
    Task<PagedResult<ProductDto>> GetProductsPagedAsync(
        Guid tenantId,
        int page,
        int pageSize,
        string? searchTerm = null,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? sortBy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Search products full-text (for ElasticSearch compatible providers)
    /// Falls back to in-memory filtering if not supported
    /// </summary>
    Task<PagedResult<ProductDto>> SearchProductsAsync(
        Guid tenantId,
        string searchTerm,
        int page,
        int pageSize,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? language = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get product count for tenant
    /// </summary>
    Task<int> GetProductCountAsync(Guid tenantId, CancellationToken cancellationToken);

    /// <summary>
    /// Verify connection and authentication with provider
    /// Used for health checks
    /// </summary>
    Task<bool> VerifyConnectivityAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get provider metadata (version, capabilities, etc.)
    /// </summary>
    Task<ProviderMetadata> GetMetadataAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Metadata about the product provider
/// Used for diagnostics and capabilities discovery
/// </summary>
public class ProviderMetadata
{
    /// <summary>
    /// Provider name (e.g., "Internal CatalogService")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Provider version
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Connection status
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// Last sync time (if applicable)
    /// </summary>
    public DateTime? LastSyncTime { get; set; }

    /// <summary>
    /// List of supported features
    /// </summary>
    public ProviderCapabilities Capabilities { get; set; } = new();

    /// <summary>
    /// Additional metadata
    /// </summary>
    public Dictionary<string, object> CustomMetadata { get; set; } = new();
}

/// <summary>
/// Capabilities of a product provider
/// </summary>
public class ProviderCapabilities
{
    /// <summary>
    /// Supports full-text search with fuzzy matching
    /// </summary>
    public bool SupportsFullTextSearch { get; set; }

    /// <summary>
    /// Supports price range filtering
    /// </summary>
    public bool SupportsPriceFiltering { get; set; }

    /// <summary>
    /// Supports category filtering
    /// </summary>
    public bool SupportsCategoryFiltering { get; set; }

    /// <summary>
    /// Supports sorting
    /// </summary>
    public bool SupportsSorting { get; set; }

    /// <summary>
    /// Supports pagination
    /// </summary>
    public bool SupportsPagination { get; set; }

    /// <summary>
    /// Supports multi-language product data
    /// </summary>
    public bool SupportsMultiLanguage { get; set; }

    /// <summary>
    /// Can sync data back (write operations)
    /// </summary>
    public bool SupportsSync { get; set; }
}

/// <summary>
/// Provider Configuration
/// Used to configure and initialize providers
/// </summary>
public class ProviderConfig
{
    /// <summary>
    /// Provider name/type
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Is this provider enabled?
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Priority for provider selection (higher = preferred)
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Connection string or configuration
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// API Key or credentials
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Base URL for API providers
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Custom settings for specific providers
    /// </summary>
    public Dictionary<string, string> Settings { get; set; } = new();

    /// <summary>
    /// Timeout for provider operations (milliseconds)
    /// </summary>
    public int TimeoutMs { get; set; } = 30000;

    /// <summary>
    /// Cache results from this provider (seconds)
    /// </summary>
    public int CacheDurationSeconds { get; set; } = 300;
}
