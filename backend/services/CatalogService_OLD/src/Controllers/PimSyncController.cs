using B2Connect.CatalogService.Services;
using Microsoft.AspNetCore.Mvc;

namespace B2Connect.CatalogService.Controllers;

/// <summary>
/// PIM Synchronization API Controller
/// Provides endpoints for manual PIM data synchronization
/// Manages sync status and health checks
/// </summary>
[ApiController]
[Route("api/v2/[controller]")]
[Produces("application/json")]
public class PimSyncController : ControllerBase
{
    private readonly IPimSyncService _syncService;
    private readonly ILogger<PimSyncController> _logger;

    public PimSyncController(
        IPimSyncService syncService,
        ILogger<PimSyncController> logger)
    {
        _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Trigger manual PIM data synchronization
    /// POST /api/v2/pimsync/sync?provider=pimcore
    /// 
    /// Fetches products from specified PIM provider and indexes them in ElasticSearch
    /// This operation is blocking and can take several minutes for large catalogs
    /// </summary>
    [HttpPost("sync")]
    [ProducesResponseType(typeof(SyncResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TriggerSync(
        [FromQuery] string? provider = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (_syncService.IsSyncInProgress)
            {
                _logger.LogWarning("Sync requested while sync is already in progress");
                return Conflict(new ErrorResponse
                {
                    Error = "Sync is already in progress",
                    Message = "Another sync operation is currently running. Please wait for it to complete."
                });
            }

            _logger.LogInformation(
                "Manual sync triggered{ProviderFilter}",
                !string.IsNullOrEmpty(provider) ? $" for provider '{provider}'" : "");

            var result = await _syncService.SyncProductsAsync(provider, cancellationToken);

            return Ok(new SyncResultDto
            {
                Success = result.Success,
                ProductsSynced = result.ProductsSynced,
                DurationMs = result.DurationMs,
                Error = result.Error,
                ErrorCount = result.Errors.Count,
                Errors = result.Errors
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error triggering manual sync");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorResponse
                {
                    Error = "Sync failed",
                    Message = ex.Message
                });
        }
    }

    /// <summary>
    /// Get current PIM sync status
    /// GET /api/v2/pimsync/status
    /// 
    /// Returns information about the last sync operation and current sync state
    /// </summary>
    [HttpGet("status")]
    [ProducesResponseType(typeof(SyncStatusDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSyncStatus(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Sync status requested");

            var status = await _syncService.GetSyncStatusAsync(cancellationToken);

            return Ok(new SyncStatusDto
            {
                IsSyncInProgress = _syncService.IsSyncInProgress,
                LastSyncTime = status.LastSyncTime,
                IsLastSyncSuccessful = status.IsSuccessful,
                LastProductsSynced = status.ProductsSynced,
                LastErrorCount = status.ErrorCount,
                LastDurationMs = status.DurationMs,
                LastErrorMessage = status.ErrorMessage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sync status");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorResponse
                {
                    Error = "Failed to get sync status",
                    Message = ex.Message
                });
        }
    }

    /// <summary>
    /// Get detailed sync health information
    /// GET /api/v2/pimsync/health
    /// 
    /// Returns comprehensive sync health metrics and recommendations
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(typeof(SyncHealthDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSyncHealth(CancellationToken cancellationToken)
    {
        try
        {
            var status = await _syncService.GetSyncStatusAsync(cancellationToken);

            var health = new SyncHealthDto
            {
                IsHealthy = true,
                Status = "OK",
                IsSyncInProgress = _syncService.IsSyncInProgress,
                LastSyncTime = status.LastSyncTime,
                TimeSinceLastSync = status.LastSyncTime.HasValue
                    ? DateTime.UtcNow - status.LastSyncTime.Value
                    : null,
                IsLastSyncSuccessful = status.IsSuccessful,
                Recommendations = new List<string>()
            };

            // Health checks
            if (!status.LastSyncTime.HasValue)
            {
                health.IsHealthy = false;
                health.Status = "No sync data";
                health.Recommendations.Add("No sync has been performed yet. Trigger a sync manually.");
            }
            else if ((DateTime.UtcNow - status.LastSyncTime.Value).TotalHours > 24)
            {
                health.IsHealthy = false;
                health.Status = "Data may be stale";
                health.Recommendations.Add("Last sync was more than 24 hours ago. Check sync worker configuration.");
            }
            else if (!status.IsSuccessful)
            {
                health.IsHealthy = false;
                health.Status = "Last sync failed";
                health.Recommendations.Add($"Last sync failed: {status.ErrorMessage}");
            }

            if (_syncService.IsSyncInProgress)
            {
                health.Recommendations.Add("Sync is currently in progress. This is normal.");
            }

            if (status.ErrorCount > 0 && status.IsSuccessful)
            {
                health.Recommendations.Add($"Last sync had {status.ErrorCount} errors. Review logs for details.");
            }

            return Ok(health);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sync health");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ErrorResponse
                {
                    Error = "Failed to get sync health",
                    Message = ex.Message
                });
        }
    }
}

/// <summary>
/// Sync result response DTO
/// </summary>
public class SyncResultDto
{
    /// <summary>
    /// Whether sync was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Number of products synced
    /// </summary>
    public int ProductsSynced { get; set; }

    /// <summary>
    /// Sync duration in milliseconds
    /// </summary>
    public long DurationMs { get; set; }

    /// <summary>
    /// Error message if sync failed
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Number of errors that occurred
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// Detailed error messages
    /// </summary>
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// Sync status response DTO
/// </summary>
public class SyncStatusDto
{
    /// <summary>
    /// Is a sync currently in progress?
    /// </summary>
    public bool IsSyncInProgress { get; set; }

    /// <summary>
    /// Timestamp of last sync
    /// </summary>
    public DateTime? LastSyncTime { get; set; }

    /// <summary>
    /// Was the last sync successful?
    /// </summary>
    public bool IsLastSyncSuccessful { get; set; }

    /// <summary>
    /// Number of products synced in last operation
    /// </summary>
    public int LastProductsSynced { get; set; }

    /// <summary>
    /// Number of errors in last sync
    /// </summary>
    public int LastErrorCount { get; set; }

    /// <summary>
    /// Duration of last sync in milliseconds
    /// </summary>
    public long LastDurationMs { get; set; }

    /// <summary>
    /// Error message from last sync (if failed)
    /// </summary>
    public string? LastErrorMessage { get; set; }
}

/// <summary>
/// Sync health response DTO
/// </summary>
public class SyncHealthDto
{
    /// <summary>
    /// Is the sync system healthy?
    /// </summary>
    public bool IsHealthy { get; set; }

    /// <summary>
    /// Current health status
    /// </summary>
    public string Status { get; set; } = "OK";

    /// <summary>
    /// Is a sync currently in progress?
    /// </summary>
    public bool IsSyncInProgress { get; set; }

    /// <summary>
    /// Timestamp of last sync
    /// </summary>
    public DateTime? LastSyncTime { get; set; }

    /// <summary>
    /// Time elapsed since last sync
    /// </summary>
    public TimeSpan? TimeSinceLastSync { get; set; }

    /// <summary>
    /// Was the last sync successful?
    /// </summary>
    public bool IsLastSyncSuccessful { get; set; }

    /// <summary>
    /// Recommendations for actions
    /// </summary>
    public List<string> Recommendations { get; set; } = new();
}

/// <summary>
/// Error response DTO
/// </summary>
public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
