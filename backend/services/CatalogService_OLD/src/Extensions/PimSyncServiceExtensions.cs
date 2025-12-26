using B2Connect.CatalogService.Services;

namespace B2Connect.CatalogService.Extensions;

/// <summary>
/// Extension methods for PIM Sync Service with Progress Tracking
/// </summary>
public static class PimSyncServiceExtensions
{
    /// <summary>
    /// Sync products with progress tracking
    /// </summary>
    public static async Task<SyncResult> SyncProductsWithProgressAsync(
        this IPimSyncService syncService,
        ISyncProgressService progressService,
        string? providerName = null,
        CancellationToken cancellationToken = default)
    {
        // Create a new sync run
        var syncRunId = progressService.CreateSyncRun(providerName);

        try
        {
            // Execute the sync (standard way)
            var result = await syncService.SyncProductsAsync(providerName, cancellationToken);

            // Update progress based on result
            if (result.Success)
            {
                progressService.SetTotalProducts(syncRunId, result.ProductsSynced);
                progressService.UpdateProgress(syncRunId, result.ProductsSynced, result.ProductsSynced, 0);
                progressService.MarkCompleted(syncRunId);
            }
            else
            {
                progressService.MarkFailed(syncRunId, result.Error ?? "Unknown error", result.Errors.ToArray());
            }

            return result;
        }
        catch (Exception ex)
        {
            progressService.MarkFailed(syncRunId, ex.Message, ex.ToString());
            throw;
        }
    }
}
