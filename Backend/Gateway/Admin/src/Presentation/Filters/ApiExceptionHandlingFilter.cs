using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace B2Connect.Admin.Presentation.Filters;

/// <summary>
/// Global Exception Handling Filter
/// Zentralisiert die Fehlerbehandlung für alle Controller
///
/// Behandelt:
/// - ArgumentNullException → 400 Bad Request
/// - InvalidOperationException → 409 Conflict
/// - KeyNotFoundException → 404 Not Found
/// - UnauthorizedAccessException → 403 Forbidden
/// - Alle sonstigen Exceptions → 500 Internal Server Error
/// </summary>
public class ApiExceptionHandlingFilter : IAsyncExceptionFilter
{
    private readonly ILogger<ApiExceptionHandlingFilter> _logger;
    private readonly IWebHostEnvironment _environment;

    public ApiExceptionHandlingFilter(ILogger<ApiExceptionHandlingFilter> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public Task OnExceptionAsync(ExceptionContext context)
    {
        var exception = context.Exception;
        var traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

        // Log Exception
        _logger.LogError(exception,
            "Unhandled exception occurred. TraceId: {TraceId}, Path: {Path}",
            traceId, context.HttpContext.Request.Path);

        // Bestimme HTTP Status Code und Fehlermeldung basierend auf Exception Type
        var (statusCode, message, errorCode) = exception switch
        {
            ArgumentNullException or ArgumentException =>
                (StatusCodes.Status400BadRequest, exception.Message, "VALIDATION_ERROR"),

            KeyNotFoundException =>
                (StatusCodes.Status404NotFound, "Resource not found", "NOT_FOUND"),

            InvalidOperationException =>
                (StatusCodes.Status409Conflict, exception.Message, "CONFLICT"),

            UnauthorizedAccessException =>
                (StatusCodes.Status403Forbidden, "Unauthorized access", "FORBIDDEN"),

            TimeoutException =>
                (StatusCodes.Status504GatewayTimeout, "Operation timeout", "TIMEOUT"),

            HttpRequestException =>
                (StatusCodes.Status502BadGateway, "External service error", "SERVICE_ERROR"),

            _ => (StatusCodes.Status500InternalServerError,
                _environment.IsDevelopment() ? exception.Message : "An error occurred",
                "INTERNAL_ERROR")
        };

        // Konstruiere standardisierte Fehlerantwort
        var errorResponse = new
        {
            success = false,
            error = message,
            errorCode = errorCode,
            traceId = traceId,
            timestamp = DateTime.UtcNow,
            // Stack Trace nur in Development
            stackTrace = _environment.IsDevelopment() ? exception.StackTrace : null
        };

        context.Result = new ObjectResult(errorResponse)
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;

        return Task.CompletedTask;
    }
}
