using Microsoft.AspNetCore.Mvc;
using B2Connect.CatalogService.Services;
using B2Connect.CatalogService.Models;

namespace B2Connect.CatalogService.Controllers;

/// <summary>
/// API endpoints for monitoring PIM sync progress
/// Provides real-time progress tracking and history
/// </summary>
[ApiController]
[Route("api/v2/pimsync")]
public class PimSyncProgressController : ControllerBase
{
    private readonly ISyncProgressService _progressService;
    private readonly IPimSyncService _syncService;
    private readonly ILogger<PimSyncProgressController> _logger;

    public PimSyncProgressController(
        ISyncProgressService progressService,
        IPimSyncService syncService,
        ILogger<PimSyncProgressController> logger)
    {
        _progressService = progressService ?? throw new ArgumentNullException(nameof(progressService));
        _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get progress of a specific sync run
    /// </summary>
    [HttpGet("progress/{syncRunId}")]
    public IActionResult GetProgress(Guid syncRunId)
    {
        var progress = _progressService.GetProgress(syncRunId);
        if (progress == null)
        {
            return NotFound(new { error = "Sync run not found", syncRunId });
        }

        return Ok(progress);
    }

    /// <summary>
    /// Get all currently running sync operations
    /// </summary>
    [HttpGet("progress/active")]
    public IActionResult GetActiveSyncs()
    {
        var activeSyncs = _progressService.GetActiveSyncs();
        return Ok(activeSyncs);
    }

    /// <summary>
    /// Get the latest sync for a specific provider
    /// </summary>
    [HttpGet("progress/latest")]
    public IActionResult GetLatestSync([FromQuery] string? provider = null)
    {
        var latestSync = _progressService.GetLatestSync(provider);
        if (latestSync == null)
        {
            return NotFound(new { error = "No sync history found", provider });
        }

        return Ok(latestSync);
    }

    /// <summary>
    /// Get sync history (completed and failed syncs)
    /// </summary>
    [HttpGet("progress/history")]
    public IActionResult GetSyncHistory([FromQuery] int maxResults = 20)
    {
        var history = _progressService.GetSyncHistory(Math.Min(maxResults, 100));
        return Ok(history);
    }

    /// <summary>
    /// Get comprehensive dashboard data
    /// Includes active syncs, latest sync, and summary statistics
    /// </summary>
    [HttpGet("dashboard")]
    public IActionResult GetDashboard()
    {
        var activeSyncs = _progressService.GetActiveSyncs();
        var latestSync = _progressService.GetLatestSync();
        var history = _progressService.GetSyncHistory(5);

        var successCount = history.Count(s => s.Status == SyncProgressStatus.Completed);
        var failureCount = history.Count(s => s.Status == SyncProgressStatus.Failed);
        var totalProducts = history.Where(s => s.Status == SyncProgressStatus.Completed)
            .Sum(s => s.ProductsIndexed);

        var dashboard = new SyncDashboardDto
        {
            ActiveSyncCount = activeSyncs.Count,
            ActiveSyncs = activeSyncs,
            LatestSync = latestSync,
            RecentHistory = history,
            Statistics = new SyncStatisticsDto
            {
                TotalSyncsCompleted = successCount,
                TotalSyncsFailed = failureCount,
                SuccessRate = (successCount + failureCount) > 0
                    ? (successCount / (double)(successCount + failureCount)) * 100
                    : 0,
                TotalProductsIndexed = totalProducts,
                AverageSyncDuration = history.Count > 0
                    ? TimeSpan.FromMilliseconds(history.Average(s => s.Duration.TotalMilliseconds))
                    : TimeSpan.Zero
            }
        };

        return Ok(dashboard);
    }
}

/// <summary>
/// Dashboard summary data
/// </summary>
public class SyncDashboardDto
{
    /// <summary>
    /// Number of currently active syncs
    /// </summary>
    public int ActiveSyncCount { get; set; }

    /// <summary>
    /// List of active sync operations
    /// </summary>
    public List<SyncProgressModel> ActiveSyncs { get; set; } = new();

    /// <summary>
    /// Most recent sync operation
    /// </summary>
    public SyncProgressModel? LatestSync { get; set; }

    /// <summary>
    /// Recent sync history (last 5)
    /// </summary>
    public List<SyncProgressModel> RecentHistory { get; set; } = new();

    /// <summary>
    /// Summary statistics
    /// </summary>
    public SyncStatisticsDto Statistics { get; set; } = new();
}

/// <summary>
/// Summary statistics for sync operations
/// </summary>
public class SyncStatisticsDto
{
    /// <summary>
    /// Total number of completed syncs
    /// </summary>
    public int TotalSyncsCompleted { get; set; }

    /// <summary>
    /// Total number of failed syncs
    /// </summary>
    public int TotalSyncsFailed { get; set; }

    /// <summary>
    /// Success rate percentage (0-100)
    /// </summary>
    public double SuccessRate { get; set; }

    /// <summary>
    /// Total products indexed across all syncs
    /// </summary>
    public int TotalProductsIndexed { get; set; }

    /// <summary>
    /// Average sync duration
    /// </summary>
    public TimeSpan AverageSyncDuration { get; set; }
}
