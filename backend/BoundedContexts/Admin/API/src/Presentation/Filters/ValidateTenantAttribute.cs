using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace B2Connect.Admin.Presentation.Filters;

/// <summary>
/// Attribute Filter für Tenant-ID Validierung
/// Extrahiert und validiert X-Tenant-ID Header aus allen Requests
/// 
/// Verwendung:
/// [ValidateTenant]
/// public async Task<ActionResult> GetUsers() { }
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ValidateTenantAttribute : Attribute, IAsyncActionFilter
{
    private readonly ILogger<ValidateTenantAttribute> _logger;

    public ValidateTenantAttribute(ILogger<ValidateTenantAttribute> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Extrahiere X-Tenant-ID Header
        var tenantIdHeader = context.HttpContext.Request.Headers["X-Tenant-ID"].ToString();

        // Validiere Tenant-ID Format
        if (string.IsNullOrWhiteSpace(tenantIdHeader))
        {
            _logger.LogWarning("Request missing X-Tenant-ID header from {RemoteIp}",
                context.HttpContext.Connection.RemoteIpAddress);

            context.Result = new UnauthorizedObjectResult(new
            {
                success = false,
                error = "Missing X-Tenant-ID header",
                timestamp = DateTime.UtcNow
            });
            return;
        }

        if (!Guid.TryParse(tenantIdHeader, out var tenantId) || tenantId == Guid.Empty)
        {
            _logger.LogWarning("Request with invalid X-Tenant-ID: {TenantId} from {RemoteIp}",
                tenantIdHeader, context.HttpContext.Connection.RemoteIpAddress);

            context.Result = new UnauthorizedObjectResult(new
            {
                success = false,
                error = "Invalid X-Tenant-ID format (must be GUID)",
                timestamp = DateTime.UtcNow
            });
            return;
        }

        // Speichere Tenant-ID im HttpContext für spätere Nutzung
        context.HttpContext.Items["TenantId"] = tenantId;
        context.HttpContext.Items["TenantIdString"] = tenantIdHeader;

        _logger.LogInformation("Request validated for TenantId: {TenantId}", tenantId);

        // Fahre mit nächstem Filter fort
        await next();
    }
}
