using B2Connect.LocalizationService;

namespace B2Connect.Gateway.Shared.Middleware;

/// <summary>
/// Middleware for detecting and setting the current tenant from HTTP requests
/// Uses JWT token or header to determine tenant context
/// </summary>
public class TenantContextMiddleware
{
    private readonly RequestDelegate _next;

    public TenantContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the middleware to detect and set tenant context
    /// Priority: 1. JWT token claim, 2. X-Tenant-ID header, 3. Clear context
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            Guid? tenantId = null;

            // Priority 1: Extract from JWT token claim
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var tenantClaim = context.User.FindFirst("tenant_id")?.Value ??
                                 context.User.FindFirst("tenantid")?.Value ??
                                 context.User.FindFirst("TenantId")?.Value;

                if (!string.IsNullOrEmpty(tenantClaim) && Guid.TryParse(tenantClaim, out var parsedTenantId))
                {
                    tenantId = parsedTenantId;
                }
            }

            // Priority 2: Extract from X-Tenant-ID header (for service-to-service calls)
            if (tenantId == null)
            {
                var tenantHeader = context.Request.Headers["X-Tenant-ID"].FirstOrDefault() ??
                                  context.Request.Headers["x-tenant-id"].FirstOrDefault();

                if (!string.IsNullOrEmpty(tenantHeader) && Guid.TryParse(tenantHeader, out var parsedTenantId))
                {
                    tenantId = parsedTenantId;
                }
            }

            // Set the tenant context for this request
            TenantContext.CurrentTenantId = tenantId;

            // Store in HttpContext for access in services
            context.Items["TenantId"] = tenantId;

            await _next(context);
        }
        finally
        {
            // Always clear the context after request processing
            TenantContext.Clear();
        }
    }
}</content>
<parameter name = "filePath" >/ Users / holger / Documents / Projekte / B2Connect / backend / Gateway / Shared / src / Middleware / TenantContextMiddleware.cs