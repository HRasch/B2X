using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace B2X.Shared.AOP;

/// <summary>
/// AOP filter for automatic model state validation
/// Prevents controllers from executing if ModelState is invalid
/// Returns standardized validation error response
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(ms => ms.Value?.Errors.Count > 0)
                .ToDictionary(
                    ms => ms.Key,
                    ms => ms.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            context.Result = new BadRequestObjectResult(new
            {
                status = "ValidationFailed",
                message = "One or more validation errors occurred",
                errors = errors,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
