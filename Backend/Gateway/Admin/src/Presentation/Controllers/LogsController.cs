using System.Text.Json;
using B2Connect.Shared.Infrastructure.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2Connect.Admin.Presentation.Controllers;

/// <summary>
/// Controller for receiving and processing frontend error logs.
/// Enables automatic error tracking and debugging for production issues.
/// </summary>
[Route("api/logs")]
[ApiController]
public class LogsController : ApiControllerBase
{
    private readonly IErrorLogStorage? _errorLogStorage;

    public LogsController(
        ILogger<LogsController> logger,
        IErrorLogStorage? errorLogStorage = null) : base(logger)
    {
        _errorLogStorage = errorLogStorage;
    }

    /// <summary>
    /// Receives error logs from the frontend application.
    /// HTTP: POST /api/logs/errors
    /// </summary>
    [HttpPost("errors")]
    [AllowAnonymous] // Allow error logging even for unauthenticated users
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReceiveErrors([FromBody] ErrorLogBatch batch, CancellationToken cancellationToken)
    {
        if (batch?.Errors == null || batch.Errors.Count == 0)
        {
            return BadRequest(new { error = "No errors provided" });
        }

        var tenantId = Request.Headers["X-Tenant-ID"].FirstOrDefault() ?? "unknown";
        var clientIp = GetClientIp();
        var entries = new List<ErrorLogEntry>();

        foreach (var error in batch.Errors)
        {
            // Enrich with server-side context
            error.ServerTimestamp = DateTime.UtcNow;
            error.TenantId ??= tenantId;
            error.ClientIp = clientIp;

            // Create storage entry
            var entry = ErrorLogEntry.FromFrontendLog(
                error.Id,
                error.Timestamp,
                error.Severity,
                error.Message,
                error.Stack,
                error.ComponentName,
                error.RoutePath,
                error.RouteName,
                error.UserId,
                error.TenantId,
                error.UserAgent,
                error.Url,
                error.ClientIp,
                error.Fingerprint,
                error.Context);

            entries.Add(entry);

            // Log with structured data for easy querying
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["ErrorId"] = entry.Id,
                ["Fingerprint"] = error.Fingerprint ?? "unknown",
                ["UserId"] = error.UserId ?? "anonymous",
                ["TenantId"] = error.TenantId,
                ["Route"] = error.RoutePath ?? "unknown",
                ["Severity"] = error.Severity ?? "error",
            }))
            {
                var logLevel = error.Severity?.ToLowerInvariant() switch
                {
                    "fatal" => LogLevel.Critical,
                    "error" => LogLevel.Error,
                    "warning" => LogLevel.Warning,
                    "info" => LogLevel.Information,
                    _ => LogLevel.Error,
                };

                _logger.Log(logLevel,
                    "Frontend error: {Message} | Component: {Component} | URL: {Url}",
                    error.Message,
                    error.ComponentName ?? "unknown",
                    error.Url);

                // Log stack trace at debug level
                if (!string.IsNullOrEmpty(error.Stack))
                {
                    _logger.LogDebug("Stack trace: {Stack}", error.Stack);
                }

                // Log context if present
                if (error.Context != null)
                {
                    _logger.LogDebug("Error context: {Context}",
                        JsonSerializer.Serialize(error.Context));
                }
            }
        }

        // Store in database if storage is available
        if (_errorLogStorage != null)
        {
            try
            {
                await _errorLogStorage.StoreBatchAsync(entries, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store error logs in database");
                // Don't fail the request - logging to structured logs is still valuable
            }
        }

        return Ok(new { received = batch.Errors.Count });
    }

    /// <summary>
    /// Gets recent errors for the admin dashboard.
    /// HTTP: GET /api/logs/errors
    /// </summary>
    [HttpGet("errors")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetRecentErrors(
        [FromQuery] int count = 100,
        [FromQuery] string? tenantId = null,
        CancellationToken cancellationToken = default)
    {
        if (_errorLogStorage == null)
        {
            return StatusCode(503, new { error = "Error log storage not configured" });
        }

        var errors = await _errorLogStorage.GetRecentAsync(count, tenantId, cancellationToken);
        return Ok(errors);
    }

    /// <summary>
    /// Gets error statistics for monitoring dashboard.
    /// HTTP: GET /api/logs/errors/stats
    /// </summary>
    [HttpGet("errors/stats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetErrorStatistics(
        [FromQuery] string? tenantId = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        if (_errorLogStorage == null)
        {
            return StatusCode(503, new { error = "Error log storage not configured" });
        }

        var stats = await _errorLogStorage.GetStatisticsAsync(tenantId, from, to, cancellationToken);
        return Ok(stats);
    }

    /// <summary>
    /// Health check endpoint for error logging service.
    /// HTTP: GET /api/logs/health
    /// </summary>
    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            storageConfigured = _errorLogStorage != null
        });
    }

    private string GetClientIp()
    {
        // Check for forwarded IP (behind proxy/load balancer)
        var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}

/// <summary>
/// Batch of error logs from frontend
/// </summary>
public class ErrorLogBatch
{
    public List<FrontendErrorLog> Errors { get; set; } = new();
}

/// <summary>
/// Single error log entry from frontend
/// </summary>
public class FrontendErrorLog
{
    public string? Id { get; set; }
    public string? Timestamp { get; set; }
    public DateTime? ServerTimestamp { get; set; }
    public string? Severity { get; set; }
    public string? Message { get; set; }
    public string? Stack { get; set; }
    public string? ComponentName { get; set; }
    public string? RoutePath { get; set; }
    public string? RouteName { get; set; }
    public string? UserId { get; set; }
    public string? TenantId { get; set; }
    public string? UserAgent { get; set; }
    public string? Url { get; set; }
    public string? ClientIp { get; set; }
    public Dictionary<string, object>? Context { get; set; }
    public string? Fingerprint { get; set; }
}
