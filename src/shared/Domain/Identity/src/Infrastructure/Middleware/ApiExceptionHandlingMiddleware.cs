using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;

namespace B2X.Identity.Infrastructure.Middleware;

/// <summary>
/// Global exception handling middleware for API resilience
/// Converts exceptions to appropriate HTTP responses with retry guidance
/// </summary>
public class ApiExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionHandlingMiddleware> _logger;

    public ApiExceptionHandlingMiddleware(RequestDelegate next, ILogger<ApiExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in request pipeline");
            await HandleExceptionAsync(context, ex).ConfigureAwait(false);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errorCode) = exception switch
        {
            BrokenCircuitException => (
                StatusCodes.Status503ServiceUnavailable,
                "External service is temporarily unavailable. Please retry after some time.",
                "SERVICE_CIRCUIT_BROKEN"
            ),
            TimeoutException => (
                StatusCodes.Status504GatewayTimeout,
                "Request timeout. The external service is taking too long to respond.",
                "REQUEST_TIMEOUT"
            ),
            HttpRequestException hre => (
                StatusCodes.Status502BadGateway,
                $"External service error: {hre.Message}",
                "EXTERNAL_SERVICE_ERROR"
            ),
            ArgumentException ae => (
                StatusCodes.Status400BadRequest,
                ae.Message,
                "INVALID_INPUT"
            ),
            InvalidOperationException ioe => (
                StatusCodes.Status409Conflict,
                ioe.Message,
                "INVALID_OPERATION"
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred. Please try again later.",
                "INTERNAL_ERROR"
            )
        };

        context.Response.StatusCode = statusCode;

        var response = new
        {
            error = new
            {
                code = errorCode,
                message = message,
                timestamp = DateTime.UtcNow.ToString("O")
            }
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}

/// <summary>
/// Extension method to add exception handling middleware to pipeline
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseApiExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ApiExceptionHandlingMiddleware>();
    }
}
