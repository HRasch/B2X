using B2Connect.CatalogService.Services;
using Microsoft.Extensions.Hosting;

namespace B2Connect.CatalogService.Workers;

/// <summary>
/// PIM Sync Background Worker
/// Runs scheduled synchronization of PIM data to ElasticSearch
/// Configurable sync interval via appsettings
/// </summary>
public class PimSyncWorker : BackgroundService
{
    private readonly IPimSyncService _syncService;
    private readonly ILogger<PimSyncWorker> _logger;
    private readonly int _syncIntervalSeconds;

    public PimSyncWorker(
        IPimSyncService syncService,
        IConfiguration configuration,
        ILogger<PimSyncWorker> logger)
    {
        _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Read sync interval from configuration (default: 1 hour)
        _syncIntervalSeconds = configuration.GetValue("PimSync:IntervalSeconds", 3600);

        if (_syncIntervalSeconds < 60)
        {
            _logger.LogWarning(
                "PimSync interval is very short ({Seconds}s). Minimum recommended is 60s",
                _syncIntervalSeconds);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PIM Sync Worker starting with interval {IntervalSeconds}s", _syncIntervalSeconds);

        // Wait a bit before first sync to allow app startup
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting scheduled PIM sync");

                var result = await _syncService.SyncProductsAsync(cancellationToken: stoppingToken);

                if (result.Success)
                {
                    _logger.LogInformation(
                        "Scheduled PIM sync completed successfully: {ProductsSynced} products in {DurationMs}ms",
                        result.ProductsSynced, result.DurationMs);
                }
                else
                {
                    _logger.LogError(
                        "Scheduled PIM sync failed: {Error}. Errors: {ErrorCount}",
                        result.Error, result.Errors.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during scheduled PIM sync");
            }

            // Wait for next sync interval
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(_syncIntervalSeconds), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("PIM Sync Worker is stopping");
                break;
            }
        }
    }
}
