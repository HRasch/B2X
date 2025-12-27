using B2Connect.Shared.Tenancy.Infrastructure.Context;

namespace B2Connect.Shared.Tenancy.Infrastructure.Middleware;

/// <summary>
/// Middleware that extracts X-Tenant-ID header and sets it in the ITenantContext.
/// Must be registered BEFORE any endpoint that needs tenant filtering.
/// 
/// Registration in Program.cs:
///   app.UseMiddleware<TenantContextMiddleware>();
/// </summary>
public class TenantContextMiddleware
{
    private readonly RequestDelegate _next;

    public TenantContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        // Extract X-Tenant-ID header
        var tenantIdHeader = context.Request.Headers["X-Tenant-ID"].FirstOrDefault();

        if (string.IsNullOrEmpty(tenantIdHeader))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                error = "Missing required header: X-Tenant-ID"
            });
            return;
        }

        // Parse tenant ID
        if (!Guid.TryParse(tenantIdHeader, out var tenantId) || tenantId == Guid.Empty)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                error = "Invalid X-Tenant-ID header. Must be a valid GUID."
            });
            return;
        }

        // Set tenant in context (injected as scoped, so available to handlers/DbContext)
        ((TenantContext)tenantContext).TenantId = tenantId;

        // Store in HttpContext.Items for filter access
        context.Items["TenantId"] = tenantId;

        await _next(context);
    }
}
