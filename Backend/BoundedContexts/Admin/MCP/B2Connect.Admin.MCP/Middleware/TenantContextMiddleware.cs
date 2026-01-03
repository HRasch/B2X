using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace B2Connect.Admin.MCP.Middleware;

/// <summary>
/// Middleware to extract and validate tenant context from JWT claims
/// </summary>
public class TenantContextMiddleware
{
    private readonly RequestDelegate _next;

    public TenantContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, TenantContext tenantContext)
    {
        // Extract tenant ID from JWT claims
        var tenantIdClaim = context.User.FindFirst("tenant_id")?.Value;

        if (string.IsNullOrEmpty(tenantIdClaim))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Missing tenant context" });
            return;
        }

        // Validate tenant ID format (UUID)
        if (!Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid tenant ID format" });
            return;
        }

        // Set tenant context
        tenantContext.TenantId = tenantId.ToString();
        tenantContext.UserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        tenantContext.UserRole = context.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

        // Add tenant ID to response headers for debugging
        context.Response.Headers["X-Tenant-ID"] = tenantContext.TenantId;

        await _next(context);
    }
}

/// <summary>
/// Tenant context service for dependency injection
/// </summary>
public class TenantContext
{
    public string TenantId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;

    public bool IsTenantAdmin => UserRole.Contains("tenant-admin");
    public bool IsSystemAdmin => UserRole.Contains("system-admin");
}