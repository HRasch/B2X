using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace B2Connect.Admin.Presentation.Filters;

/// <summary>
/// Request/Response Logging Filter
/// Loggt Informationen über jeden Request und Response
/// 
/// Logged:
/// - Request: Method, Path, Headers, Body (wenn möglich)
/// - Response: Status Code, Duration
/// - Performance Metriken
/// </summary>
public class ApiLoggingFilter : IAsyncActionFilter
{
    private readonly ILogger<ApiLoggingFilter> _logger;

    public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;
        var stopwatch = Stopwatch.StartNew();

        // Log Inbound Request
        var tenantId = context.HttpContext.Items.ContainsKey("TenantId")
            ? context.HttpContext.Items["TenantId"]?.ToString() ?? "unknown"
            : "unknown";

        _logger.LogInformation(
            "Inbound Request: {Method} {Path} | TenantId: {TenantId} | User: {User}",
            request.Method,
            request.Path.Value,
            tenantId,
            context.HttpContext.User?.Identity?.Name ?? "anonymous");

        // Führe Action aus
        var resultContext = await next();

        stopwatch.Stop();

        // Log Outbound Response
        var statusCode = resultContext.HttpContext.Response.StatusCode;
        var isSuccess = statusCode >= 200 && statusCode < 300;

        _logger.LogInformation(
            "Outbound Response: {Method} {Path} | Status: {StatusCode} | Duration: {ElapsedMilliseconds}ms | TenantId: {TenantId}",
            request.Method,
            request.Path.Value,
            statusCode,
            stopwatch.ElapsedMilliseconds,
            tenantId);

        // Log Warnings für langsame Requests (> 1000ms)
        if (stopwatch.ElapsedMilliseconds > 1000)
        {
            _logger.LogWarning(
                "Slow Request: {Method} {Path} took {ElapsedMilliseconds}ms",
                request.Method,
                request.Path.Value,
                stopwatch.ElapsedMilliseconds);
        }

        // Log Errors für fehlgeschlagene Requests (5xx)
        if (statusCode >= 500)
        {
            _logger.LogError(
                "Server Error: {Method} {Path} returned {StatusCode}",
                request.Method,
                request.Path.Value,
                statusCode);
        }
    }
}
