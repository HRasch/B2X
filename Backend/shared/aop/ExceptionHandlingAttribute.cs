using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace B2Connect.Shared.AOP;

/// <summary>
/// AOP filter for centralized exception handling
/// Catches unhandled exceptions and returns standardized error responses
/// Logs exceptions for diagnostics
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ExceptionHandlingAttribute : ExceptionFilterAttribute
{
    private readonly ILogger<ExceptionHandlingAttribute> _logger;

    public ExceptionHandlingAttribute(ILogger<ExceptionHandlingAttribute> logger)
    {
        _logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Unhandled exception in {ActionName}",
            context.ActionDescriptor.DisplayName);

        var response = new
        {
            status = "Error",
            message = context.Exception.Message,
            errorType = context.Exception.GetType().Name,
            timestamp = DateTime.UtcNow
        };

        context.Result = new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.ExceptionHandled = true;
    }
}
