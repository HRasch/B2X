using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace B2Connect.Shared.AOP;

/// <summary>
/// AOP filter for request/response logging
/// Logs all HTTP requests and responses with execution time
/// Useful for diagnostics and performance monitoring
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequestLoggingAttribute : ActionFilterAttribute
{
    private readonly ILogger<RequestLoggingAttribute> _logger;

    public RequestLoggingAttribute(ILogger<RequestLoggingAttribute> logger)
    {
        _logger = logger;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var httpContext = context.HttpContext;
        _logger.LogInformation(
            "→ {Method} {Path} | User: {User}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            httpContext.User?.Identity?.Name ?? "Anonymous"
        );
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        var httpContext = context.HttpContext;
        var statusCode = httpContext.Response.StatusCode;
        var statusText = GetStatusText(statusCode);

        _logger.LogInformation(
            "← {StatusCode} {StatusText} | {Path}",
            statusCode,
            statusText,
            httpContext.Request.Path
        );
    }

    private static string GetStatusText(int statusCode) =>
        statusCode switch
        {
            200 => "OK",
            201 => "Created",
            204 => "NoContent",
            400 => "BadRequest",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "NotFound",
            500 => "InternalServerError",
            _ => "Unknown"
        };
}
