using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace B2X.ServiceDefaults;

/// <summary>
/// Middleware that detects and logs requests that exceed a configurable duration threshold.
/// Useful for identifying performance bottlenecks and slow endpoints.
/// </summary>
public class LongRunningRequestMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LongRunningRequestMiddleware> _logger;
    private readonly TimeSpan _warningThreshold;
    private readonly TimeSpan _criticalThreshold;

    public LongRunningRequestMiddleware(
        RequestDelegate next,
        ILogger<LongRunningRequestMiddleware> logger,
        TimeSpan? warningThreshold = null,
        TimeSpan? criticalThreshold = null)
    {
        _next = next;
        _logger = logger;
        _warningThreshold = warningThreshold ?? TimeSpan.FromSeconds(3);
        _criticalThreshold = criticalThreshold ?? TimeSpan.FromSeconds(10);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path.Value;
        var requestMethod = context.Request.Method;

        try
        {
            await _next(context).ConfigureAwait(false);
        }
        finally
        {
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;

            // Add timing to response headers for debugging (only in development)
            if (!context.Response.HasStarted)
            {
                context.Response.Headers["X-Response-Time-Ms"] = elapsed.TotalMilliseconds.ToString("F2");
            }

            // Log based on duration thresholds
            if (elapsed >= _criticalThreshold)
            {
                _logger.LogError(
                    "CRITICAL: Long-running request detected. " +
                    "Method: {Method}, Path: {Path}, Duration: {Duration}ms, " +
                    "StatusCode: {StatusCode}, TraceId: {TraceId}",
                    requestMethod,
                    requestPath,
                    elapsed.TotalMilliseconds,
                    context.Response.StatusCode,
                    Activity.Current?.TraceId.ToString() ?? "N/A");
            }
            else if (elapsed >= _warningThreshold)
            {
                _logger.LogWarning(
                    "WARNING: Slow request detected. " +
                    "Method: {Method}, Path: {Path}, Duration: {Duration}ms, " +
                    "StatusCode: {StatusCode}, TraceId: {TraceId}",
                    requestMethod,
                    requestPath,
                    elapsed.TotalMilliseconds,
                    context.Response.StatusCode,
                    Activity.Current?.TraceId.ToString() ?? "N/A");
            }
            else if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(
                    "Request completed. Method: {Method}, Path: {Path}, Duration: {Duration}ms",
                    requestMethod,
                    requestPath,
                    elapsed.TotalMilliseconds);
            }
        }
    }
}

/// <summary>
/// Configuration options for the long-running request middleware.
/// </summary>
public class LongRunningRequestOptions
{
    /// <summary>
    /// Duration after which a request is logged as a warning. Default: 3 seconds.
    /// </summary>
    public TimeSpan WarningThreshold { get; set; } = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Duration after which a request is logged as critical/error. Default: 10 seconds.
    /// </summary>
    public TimeSpan CriticalThreshold { get; set; } = TimeSpan.FromSeconds(10);
}
