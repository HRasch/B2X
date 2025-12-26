namespace B2Connect.CatalogService.Models;

/// <summary>
/// Represents the current progress of a PIM sync operation
/// </summary>
public class SyncProgressModel
{
    /// <summary>
    /// Unique identifier for this sync run
    /// </summary>
    public Guid SyncRunId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Current status of the sync
    /// </summary>
    public SyncProgressStatus Status { get; set; } = SyncProgressStatus.Queued;

    /// <summary>
    /// Provider being synced
    /// </summary>
    public string? ProviderName { get; set; }

    /// <summary>
    /// Total products to sync (once known)
    /// </summary>
    public int? TotalProducts { get; set; }

    /// <summary>
    /// Products processed so far
    /// </summary>
    public int ProductsProcessed { get; set; }

    /// <summary>
    /// Products indexed successfully
    /// </summary>
    public int ProductsIndexed { get; set; }

    /// <summary>
    /// Products failed during sync
    /// </summary>
    public int ProductsFailed { get; set; }

    /// <summary>
    /// Current language being processed
    /// </summary>
    public string? CurrentLanguage { get; set; }

    /// <summary>
    /// Start time of the sync
    /// </summary>
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// End time of the sync (null if still running)
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Current error message (if any)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Detailed errors
    /// </summary>
    public List<string> DetailedErrors { get; set; } = new();

    /// <summary>
    /// Percentage complete (0-100)
    /// </summary>
    public double ProgressPercentage
    {
        get
        {
            if (!TotalProducts.HasValue || TotalProducts == 0)
                return Status == SyncProgressStatus.Running ? 5 : 0;

            return Math.Min(100, (ProductsProcessed / (double)TotalProducts.Value) * 100);
        }
    }

    /// <summary>
    /// Estimated time remaining
    /// </summary>
    public TimeSpan? EstimatedTimeRemaining
    {
        get
        {
            if (Status != SyncProgressStatus.Running || !TotalProducts.HasValue || ProductsProcessed == 0)
                return null;

            var elapsed = DateTime.UtcNow - StartedAt;
            var productsPerSecond = ProductsProcessed / elapsed.TotalSeconds;

            if (productsPerSecond <= 0)
                return null;

            var remainingProducts = TotalProducts.Value - ProductsProcessed;
            var secondsRemaining = remainingProducts / productsPerSecond;

            return TimeSpan.FromSeconds(secondsRemaining);
        }
    }

    /// <summary>
    /// Duration of the sync operation
    /// </summary>
    public TimeSpan Duration => (CompletedAt ?? DateTime.UtcNow) - StartedAt;
}

/// <summary>
/// Status of a sync operation
/// </summary>
public enum SyncProgressStatus
{
    Queued,
    Running,
    Completed,
    Failed,
    Cancelled
}
