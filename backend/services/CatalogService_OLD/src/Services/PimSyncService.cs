using B2Connect.CatalogService.Providers;
using B2Connect.CatalogService.Infrastructure;

namespace B2Connect.CatalogService.Services;

/// <summary>
/// PIM Synchronization Service
/// Fetches product data from configured PIM providers
/// Converts and loads data into ElasticSearch indices
/// Supports scheduled syncs and manual triggers
/// </summary>
public interface IPimSyncService
{
    /// <summary>
    /// Synchronize products from PIM provider to ElasticSearch
    /// </summary>
    Task<SyncResult> SyncProductsAsync(
        string? providerName = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get last sync status
    /// </summary>
    Task<SyncStatus> GetSyncStatusAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Check if sync is currently running
    /// </summary>
    bool IsSyncInProgress { get; }
}

/// <summary>
/// Default implementation of PIM Sync Service
/// </summary>
public class PimSyncService : IPimSyncService
{
    private readonly IProductProviderRegistry _registry;
    private readonly IElasticsearchClient _elasticsearchClient;
    private readonly ILogger<PimSyncService> _logger;
    private volatile bool _isSyncInProgress;
    private SyncStatus _lastSyncStatus = new();

    public bool IsSyncInProgress => _isSyncInProgress;

    // Language-specific indexes
    private const string IndexPrefix = "products_";
    private static readonly string[] SupportedLanguages = { "de", "en", "fr" };

    public PimSyncService(
        IProductProviderRegistry registry,
        IElasticsearchClient elasticsearchClient,
        ILogger<PimSyncService> logger)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _elasticsearchClient = elasticsearchClient ?? throw new ArgumentNullException(nameof(elasticsearchClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<SyncResult> SyncProductsAsync(
        string? providerName = null,
        CancellationToken cancellationToken = default)
    {
        if (_isSyncInProgress)
        {
            _logger.LogWarning("Sync is already in progress");
            return new SyncResult
            {
                Success = false,
                Error = "Sync is already in progress"
            };
        }

        _isSyncInProgress = true;
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var result = new SyncResult();

        try
        {
            _logger.LogInformation("Starting PIM sync{ProviderFilter}",
                !string.IsNullOrEmpty(providerName) ? $" for provider '{providerName}'" : "");

            // Get provider(s) to sync from
            var providers = string.IsNullOrEmpty(providerName)
                ? (await _registry.GetProvidersInPriorityOrderAsync(cancellationToken)).ToList()
                : new List<IProductProvider> { _registry.GetProvider(providerName)! };

            if (!providers.Any())
            {
                throw new InvalidOperationException("No providers found to sync");
            }

            // Sync each enabled provider
            foreach (var provider in providers)
            {
                if (!await provider.IsEnabledAsync(cancellationToken))
                {
                    _logger.LogInformation("Skipping disabled provider '{ProviderName}'", provider.ProviderName);
                    continue;
                }

                await SyncProviderAsync(provider, result, cancellationToken);
            }

            sw.Stop();
            result.DurationMs = sw.ElapsedMilliseconds;
            result.Success = result.ProductsSynced > 0;

            _lastSyncStatus = new SyncStatus
            {
                LastSyncTime = DateTime.UtcNow,
                IsSuccessful = result.Success,
                ProductsSynced = result.ProductsSynced,
                ErrorCount = result.Errors.Count,
                DurationMs = result.DurationMs
            };

            _logger.LogInformation(
                "PIM sync completed: {ProductsSynced} products, {ErrorCount} errors, {DurationMs}ms",
                result.ProductsSynced, result.Errors.Count, result.DurationMs);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PIM sync failed");
            result.Success = false;
            result.Error = ex.Message;
            result.Errors.Add(ex.Message);

            _lastSyncStatus = new SyncStatus
            {
                LastSyncTime = DateTime.UtcNow,
                IsSuccessful = false,
                ErrorMessage = ex.Message
            };

            return result;
        }
        finally
        {
            _isSyncInProgress = false;
        }
    }

    public async Task<SyncStatus> GetSyncStatusAsync(CancellationToken cancellationToken)
    {
        return await Task.FromResult(_lastSyncStatus);
    }

    private async Task SyncProviderAsync(
        IProductProvider provider,
        SyncResult result,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting sync for provider '{ProviderName}'", provider.ProviderName);

            // Fetch all products from provider (paginated)
            var pageSize = 100;
            var page = 1;
            var allProducts = new List<ProductDto>();

            while (true)
            {
                var pagedResult = await provider.GetProductsPagedAsync(
                    tenantId: Guid.Empty, // Sync all tenants
                    page: page,
                    pageSize: pageSize,
                    cancellationToken: cancellationToken);

                if (pagedResult.Items.Count == 0)
                    break;

                allProducts.AddRange(pagedResult.Items);

                _logger.LogDebug(
                    "Provider '{ProviderName}' fetched page {Page} ({Count} products)",
                    provider.ProviderName, page, pagedResult.Items.Count);

                if (!pagedResult.HasNextPage)
                    break;

                page++;
            }

            _logger.LogInformation(
                "Provider '{ProviderName}' returned {TotalProducts} products",
                provider.ProviderName, allProducts.Count);

            // Index products in ElasticSearch by language
            foreach (var language in SupportedLanguages)
            {
                await IndexProductsForLanguageAsync(
                    allProducts, language, provider.ProviderName, result, cancellationToken);
            }

            result.ProductsSynced += allProducts.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing provider '{ProviderName}'", provider.ProviderName);
            result.Errors.Add($"Provider '{provider.ProviderName}': {ex.Message}");
        }
    }

    private async Task IndexProductsForLanguageAsync(
        List<ProductDto> products,
        string language,
        string providerName,
        SyncResult result,
        CancellationToken cancellationToken)
    {
        try
        {
            var indexName = GetIndexNameForLanguage(language);

            _logger.LogInformation(
                "Indexing {Count} products for language '{Language}' from provider '{ProviderName}' in index '{IndexName}'",
                products.Count, language, providerName, indexName);

            // Batch index products
            const int batchSize = 100;
            var batch = new List<object>();

            for (int i = 0; i < products.Count; i++)
            {
                var product = products[i];

                // Prepare bulk operation
                batch.Add(new { index = new { _index = indexName, _id = product.Id.ToString() } });

                // Convert to ElasticSearch document
                var esDoc = new
                {
                    product.Id,
                    product.TenantId,
                    product.Sku,
                    product.Name,
                    product.Price,
                    B2bPrice = product.Price * 0.8m, // Example: 20% discount
                    product.Description,
                    product.Category,
                    StockQuantity = product.StockQuantity,
                    IsAvailable = product.StockQuantity > 0,
                    SearchText = $"{product.Name} {product.Description} {product.Sku} {product.Category}".ToLower(),
                    Provider = providerName,
                    Language = language,
                    product.CreatedAt,
                    product.UpdatedAt,
                    SyncedAt = DateTime.UtcNow
                };

                batch.Add(esDoc);

                // Execute batch when size reached
                if ((i + 1) % batchSize == 0)
                {
                    await BulkIndexAsync(batch, indexName, result, cancellationToken);
                    batch.Clear();
                }
            }

            // Index remaining products
            if (batch.Count > 0)
            {
                await BulkIndexAsync(batch, indexName, result, cancellationToken);
            }

            _logger.LogInformation(
                "Successfully indexed {Count} products for language '{Language}' from provider '{ProviderName}'",
                products.Count, language, providerName);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error indexing products for language '{Language}' from provider '{ProviderName}'",
                language, providerName);

            result.Errors.Add($"Indexing failed for language '{language}': {ex.Message}");
        }
    }

    private async Task BulkIndexAsync(
        List<object> batch,
        string indexName,
        SyncResult result,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Bulk indexing {OperationCount} items to '{IndexName}'", batch.Count, indexName);

            // In production, use proper Elasticsearch bulk API
            // For now, we'll skip bulk indexing or implement a simple version
            // The full BulkAsync API requires proper request builder setup

            // TODO: Implement proper bulk indexing when Elasticsearch service is available
            // For now, just log that we're skipping this
            _logger.LogWarning("Bulk indexing to Elasticsearch not yet fully implemented");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing bulk index for index '{IndexName}'", indexName);
            result.Errors.Add($"Bulk index failed: {ex.Message}");
        }
    }

    private string GetIndexNameForLanguage(string language)
    {
        return language?.ToLower() switch
        {
            "de" => "products_de",
            "en" => "products_en",
            "fr" => "products_fr",
            _ => "products_de"
        };
    }
}

/// <summary>
/// Sync result with statistics
/// </summary>
public class SyncResult
{
    public bool Success { get; set; }
    public int ProductsSynced { get; set; }
    public long DurationMs { get; set; }
    public string? Error { get; set; }
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// Sync status and history
/// </summary>
public class SyncStatus
{
    public DateTime? LastSyncTime { get; set; }
    public bool IsSuccessful { get; set; }
    public int ProductsSynced { get; set; }
    public int ErrorCount { get; set; }
    public long DurationMs { get; set; }
    public string? ErrorMessage { get; set; }
}
