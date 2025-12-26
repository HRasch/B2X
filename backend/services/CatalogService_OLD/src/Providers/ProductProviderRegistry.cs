using B2Connect.CatalogService.CQRS;
using B2Connect.CatalogService.Models;
using B2Connect.CatalogService.Services;
using Microsoft.Extensions.Logging;

namespace B2Connect.CatalogService.Providers;

/// <summary>
/// Product Provider Registry
/// Manages multiple product providers and enables dynamic provider selection
/// Supports fallback chain: If primary provider fails, try secondary, etc.
/// </summary>
public interface IProductProviderRegistry
{
    /// <summary>
    /// Register a product provider
    /// </summary>
    void RegisterProvider(IProductProvider provider, int priority = 0);

    /// <summary>
    /// Get provider by name
    /// </summary>
    IProductProvider? GetProvider(string providerName);

    /// <summary>
    /// Get all registered providers
    /// </summary>
    IEnumerable<IProductProvider> GetAllProviders();

    /// <summary>
    /// Get primary provider (highest priority, enabled)
    /// </summary>
    Task<IProductProvider?> GetPrimaryProviderAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get providers in priority order (highest first)
    /// </summary>
    Task<IEnumerable<IProductProvider>> GetProvidersInPriorityOrderAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get metadata for all providers
    /// </summary>
    Task<Dictionary<string, ProviderMetadata>> GetAllMetadataAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Default implementation of Product Provider Registry
/// </summary>
public class ProductProviderRegistry : IProductProviderRegistry
{
    private readonly Dictionary<string, (IProductProvider provider, int priority)> _providers = new();
    private readonly ILogger<ProductProviderRegistry> _logger;

    public ProductProviderRegistry(ILogger<ProductProviderRegistry> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void RegisterProvider(IProductProvider provider, int priority = 0)
    {
        if (provider == null)
            throw new ArgumentNullException(nameof(provider));

        _providers[provider.ProviderName] = (provider, priority);
        _logger.LogInformation("Registered provider '{ProviderName}' with priority {Priority}",
            provider.ProviderName, priority);
    }

    public IProductProvider? GetProvider(string providerName)
    {
        if (string.IsNullOrWhiteSpace(providerName))
            return null;

        return _providers.TryGetValue(providerName, out var entry) ? entry.provider : null;
    }

    public IEnumerable<IProductProvider> GetAllProviders()
    {
        return _providers.Values.Select(entry => entry.provider);
    }

    public async Task<IProductProvider?> GetPrimaryProviderAsync(CancellationToken cancellationToken)
    {
        var providers = await GetProvidersInPriorityOrderAsync(cancellationToken);
        return providers.FirstOrDefault();
    }

    public async Task<IEnumerable<IProductProvider>> GetProvidersInPriorityOrderAsync(CancellationToken cancellationToken)
    {
        var enabledProviders = new List<(IProductProvider provider, int priority)>();

        foreach (var (provider, priority) in _providers.Values)
        {
            if (await provider.IsEnabledAsync(cancellationToken))
            {
                enabledProviders.Add((provider, priority));
            }
        }

        // Sort by priority (descending) then by name
        return enabledProviders
            .OrderByDescending(x => x.priority)
            .ThenBy(x => x.provider.ProviderName)
            .Select(x => x.provider);
    }

    public async Task<Dictionary<string, ProviderMetadata>> GetAllMetadataAsync(CancellationToken cancellationToken)
    {
        var metadata = new Dictionary<string, ProviderMetadata>();

        foreach (var (providerName, (provider, _)) in _providers)
        {
            try
            {
                var providerMetadata = await provider.GetMetadataAsync(cancellationToken);
                metadata[providerName] = providerMetadata;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get metadata for provider '{ProviderName}'", providerName);
                metadata[providerName] = new ProviderMetadata
                {
                    Name = providerName,
                    IsConnected = false
                };
            }
        }

        return metadata;
    }
}

/// <summary>
/// Product Provider Resolver
/// Resolves product data by trying providers in priority order
/// Implements fallback chain: Primary → Secondary → Tertiary, etc.
/// </summary>
public interface IProductProviderResolver
{
    /// <summary>
    /// Resolve product by trying all providers in priority order
    /// Returns first successful result or null
    /// </summary>
    Task<ProductDto?> ResolveProductByIdAsync(
        Guid tenantId,
        Guid productId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Resolve paged products using primary provider
    /// Falls back to other providers if primary fails
    /// </summary>
    Task<PagedResult<ProductDto>> ResolveProductsPagedAsync(
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
    /// Resolve search using primary provider with fallback
    /// </summary>
    Task<PagedResult<ProductDto>> ResolveSearchAsync(
        Guid tenantId,
        string searchTerm,
        int page,
        int pageSize,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? language = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Default implementation of Product Provider Resolver
/// </summary>
public class ProductProviderResolver : IProductProviderResolver
{
    private readonly IProductProviderRegistry _registry;
    private readonly ILogger<ProductProviderResolver> _logger;

    public ProductProviderResolver(
        IProductProviderRegistry registry,
        ILogger<ProductProviderResolver> logger)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ProductDto?> ResolveProductByIdAsync(
        Guid tenantId,
        Guid productId,
        CancellationToken cancellationToken)
    {
        var providers = await _registry.GetProvidersInPriorityOrderAsync(cancellationToken);

        foreach (var provider in providers)
        {
            try
            {
                _logger.LogDebug(
                    "Trying to resolve product {ProductId} from provider '{ProviderName}'",
                    productId, provider.ProviderName);

                var product = await provider.GetProductByIdAsync(tenantId, productId, cancellationToken);
                if (product != null)
                {
                    _logger.LogInformation(
                        "Product {ProductId} resolved from provider '{ProviderName}'",
                        productId, provider.ProviderName);
                    return product;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Provider '{ProviderName}' failed to resolve product {ProductId}. Trying next provider.",
                    provider.ProviderName, productId);
            }
        }

        _logger.LogWarning("No provider could resolve product {ProductId}", productId);
        return null;
    }

    public async Task<PagedResult<ProductDto>> ResolveProductsPagedAsync(
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
        var providers = await _registry.GetProvidersInPriorityOrderAsync(cancellationToken);

        foreach (var provider in providers)
        {
            try
            {
                _logger.LogDebug(
                    "Trying to resolve paged products from provider '{ProviderName}' (page={Page})",
                    provider.ProviderName, page);

                var result = await provider.GetProductsPagedAsync(
                    tenantId, page, pageSize,
                    searchTerm, category, minPrice, maxPrice, sortBy,
                    cancellationToken);

                _logger.LogInformation(
                    "Products resolved from provider '{ProviderName}' (got {Count} items)",
                    provider.ProviderName, result.Items.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Provider '{ProviderName}' failed to resolve paged products. Trying next provider.",
                    provider.ProviderName);
            }
        }

        _logger.LogWarning("No provider could resolve paged products");
        return new PagedResult<ProductDto>
        {
            Items = new List<ProductDto>(),
            Page = page,
            PageSize = pageSize,
            TotalCount = 0,
            TotalPages = 0,
            HasNextPage = false
        };
    }

    public async Task<PagedResult<ProductDto>> ResolveSearchAsync(
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
        var providers = await _registry.GetProvidersInPriorityOrderAsync(cancellationToken);

        foreach (var provider in providers)
        {
            try
            {
                _logger.LogDebug(
                    "Trying to search products from provider '{ProviderName}' (term='{SearchTerm}')",
                    provider.ProviderName, searchTerm);

                var result = await provider.SearchProductsAsync(
                    tenantId, searchTerm, page, pageSize,
                    category, minPrice, maxPrice, language,
                    cancellationToken);

                _logger.LogInformation(
                    "Search resolved from provider '{ProviderName}' (got {Count} items)",
                    provider.ProviderName, result.Items.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Provider '{ProviderName}' failed to search products. Trying next provider.",
                    provider.ProviderName);
            }
        }

        _logger.LogWarning("No provider could search products for term '{SearchTerm}'", searchTerm);
        return new PagedResult<ProductDto>
        {
            Items = new List<ProductDto>(),
            Page = page,
            PageSize = pageSize,
            TotalCount = 0,
            TotalPages = 0,
            HasNextPage = false
        };
    }
}
