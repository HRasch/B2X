using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace B2Connect.Admin.Presentation.Filters;

/// <summary>
/// Model State Validierungsfilter
/// Validiert automatisch das Model State vor der Controller-Aktion
/// 
/// Erspart wiederholte ModelState.IsValid Checks in jedem Controller
/// </summary>
public class ValidateModelStateFilter : IActionFilter
{
    private readonly ILogger<ValidateModelStateFilter> _logger;

    public ValidateModelStateFilter(ILogger<ValidateModelStateFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Überspringe Filter für GET/DELETE Requests (ohne Body)
        if (context.HttpContext.Request.Method == HttpMethods.Get ||
            context.HttpContext.Request.Method == HttpMethods.Delete)
        {
            return;
        }

        // Validiere Model State
        if (!context.ModelState.IsValid)
        {
            _logger.LogWarning("Model validation failed for action {Action}",
                context.ActionDescriptor.DisplayName);

            // Sammle alle Validierungsfehler
            var errors = context.ModelState
                .Where(ms => ms.Value?.Errors.Count > 0)
                .SelectMany(ms => ms.Value!.Errors.Select(e => new
                {
                    field = ms.Key,
                    message = e.ErrorMessage
                }))
                .ToList();

            var errorResponse = new
            {
                success = false,
                error = "Validation failed",
                errorCode = "VALIDATION_ERROR",
                errors = errors,
                timestamp = DateTime.UtcNow
            };

            context.Result = new BadRequestObjectResult(errorResponse);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Kein Code nötig nach der Action
    }
}
