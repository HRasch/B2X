using Quartz;
using B2Connect.CatalogService.Services;

namespace B2Connect.CatalogService.Jobs;

/// <summary>
/// Quartz Job for PIM synchronization
/// Executes on a configurable schedule
/// Tracks progress through ISyncProgressService
/// </summary>
public class PimSyncJob : IJob
{
    private readonly IPimSyncService _syncService;
    private readonly ISyncProgressService _progressService;
    private readonly ILogger<PimSyncJob> _logger;

    public PimSyncJob(
        IPimSyncService syncService,
        ISyncProgressService progressService,
        ILogger<PimSyncJob> logger)
    {
        _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
        _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var providerName = context.JobDetail.JobDataMap.GetString("ProviderName");
        var syncRunId = _progressService.CreateSyncRun(providerName);

        try
        {
            _logger.LogInformation("Executing PIM sync job (RunId: {SyncRunId}, Provider: {Provider})",
                syncRunId, providerName ?? "all");

            // Execute the sync
            var result = await _syncService.SyncProductsAsync(providerName, context.CancellationToken);

            if (result.Success)
            {
                _progressService.MarkCompleted(syncRunId);
                _logger.LogInformation(
                    "PIM sync job completed successfully (RunId: {SyncRunId}): {ProductsSynced} products",
                    syncRunId, result.ProductsSynced);
            }
            else
            {
                _progressService.MarkFailed(syncRunId, result.Error ?? "Unknown error",
                    result.Errors.ToArray());
                _logger.LogError(
                    "PIM sync job failed (RunId: {SyncRunId}): {Error}",
                    syncRunId, result.Error);
            }
        }
        catch (Exception ex)
        {
            _progressService.MarkFailed(syncRunId, ex.Message, ex.ToString());
            _logger.LogError(ex, "Error executing PIM sync job (RunId: {SyncRunId})", syncRunId);

            // Re-throw to let Quartz handle the failure
            throw;
        }
    }
}
