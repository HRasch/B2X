namespace B2Connect.CatalogService.Services;

/// <summary>
/// Manages and tracks PIM sync progress across multiple sync runs
/// </summary>
public interface ISyncProgressService
{
    /// <summary>
    /// Create a new sync progress tracker
    /// </summary>
    Guid CreateSyncRun(string? providerName = null);

    /// <summary>
    /// Update the progress of an ongoing sync
    /// </summary>
    void UpdateProgress(Guid syncRunId, int processed, int indexed, int failed, string? currentLanguage = null);

    /// <summary>
    /// Mark total products for the sync run
    /// </summary>
    void SetTotalProducts(Guid syncRunId, int totalProducts);

    /// <summary>
    /// Mark sync as completed
    /// </summary>
    void MarkCompleted(Guid syncRunId);

    /// <summary>
    /// Mark sync as failed
    /// </summary>
    void MarkFailed(Guid syncRunId, string errorMessage, params string[] errors);

    /// <summary>
    /// Get current progress of a sync run
    /// </summary>
    SyncProgressModel? GetProgress(Guid syncRunId);

    /// <summary>
    /// Get all active sync runs
    /// </summary>
    List<SyncProgressModel> GetActiveSyncs();

    /// <summary>
    /// Get the most recent sync for a specific provider
    /// </summary>
    SyncProgressModel? GetLatestSync(string? providerName = null);

    /// <summary>
    /// Get history of syncs
    /// </summary>
    List<SyncProgressModel> GetSyncHistory(int maxResults = 20);
}

/// <summary>
/// Default in-memory implementation of sync progress service
/// Note: For production, consider using Redis or database
/// </summary>
public class SyncProgressService : ISyncProgressService
{
    private readonly Dictionary<Guid, SyncProgressModel> _activeSyncs = new();
    private readonly List<SyncProgressModel> _syncHistory = new();
    private readonly object _lock = new();
    private const int MaxHistorySize = 100;

    public Guid CreateSyncRun(string? providerName = null)
    {
        lock (_lock)
        {
            var syncRun = new SyncProgressModel
            {
                SyncRunId = Guid.NewGuid(),
                Status = SyncProgressStatus.Queued,
                ProviderName = providerName
            };

            _activeSyncs[syncRun.SyncRunId] = syncRun;
            return syncRun.SyncRunId;
        }
    }

    public void UpdateProgress(Guid syncRunId, int processed, int indexed, int failed, string? currentLanguage = null)
    {
        lock (_lock)
        {
            if (_activeSyncs.TryGetValue(syncRunId, out var progress))
            {
                progress.ProductsProcessed = processed;
                progress.ProductsIndexed = indexed;
                progress.ProductsFailed = failed;
                if (!string.IsNullOrEmpty(currentLanguage))
                    progress.CurrentLanguage = currentLanguage;

                if (progress.Status == SyncProgressStatus.Queued)
                    progress.Status = SyncProgressStatus.Running;
            }
        }
    }

    public void SetTotalProducts(Guid syncRunId, int totalProducts)
    {
        lock (_lock)
        {
            if (_activeSyncs.TryGetValue(syncRunId, out var progress))
            {
                progress.TotalProducts = totalProducts;
            }
        }
    }

    public void MarkCompleted(Guid syncRunId)
    {
        lock (_lock)
        {
            if (_activeSyncs.TryGetValue(syncRunId, out var progress))
            {
                progress.Status = SyncProgressStatus.Completed;
                progress.CompletedAt = DateTime.UtcNow;
                _activeSyncs.Remove(syncRunId);
                _syncHistory.Insert(0, progress);

                // Keep history size bounded
                while (_syncHistory.Count > MaxHistorySize)
                    _syncHistory.RemoveAt(_syncHistory.Count - 1);
            }
        }
    }

    public void MarkFailed(Guid syncRunId, string errorMessage, params string[] errors)
    {
        lock (_lock)
        {
            if (_activeSyncs.TryGetValue(syncRunId, out var progress))
            {
                progress.Status = SyncProgressStatus.Failed;
                progress.ErrorMessage = errorMessage;
                progress.CompletedAt = DateTime.UtcNow;
                progress.DetailedErrors.AddRange(errors);
                _activeSyncs.Remove(syncRunId);
                _syncHistory.Insert(0, progress);

                // Keep history size bounded
                while (_syncHistory.Count > MaxHistorySize)
                    _syncHistory.RemoveAt(_syncHistory.Count - 1);
            }
        }
    }

    public SyncProgressModel? GetProgress(Guid syncRunId)
    {
        lock (_lock)
        {
            return _activeSyncs.TryGetValue(syncRunId, out var progress) ? progress : null;
        }
    }

    public List<SyncProgressModel> GetActiveSyncs()
    {
        lock (_lock)
        {
            return _activeSyncs.Values.ToList();
        }
    }

    public SyncProgressModel? GetLatestSync(string? providerName = null)
    {
        lock (_lock)
        {
            return _syncHistory
                .Where(s => string.IsNullOrEmpty(providerName) || s.ProviderName == providerName)
                .FirstOrDefault();
        }
    }

    public List<SyncProgressModel> GetSyncHistory(int maxResults = 20)
    {
        lock (_lock)
        {
            return _syncHistory.Take(maxResults).ToList();
        }
    }
}
