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
        // Skip tenant validation for public endpoints (Login, Register, Health, etc.)
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        if (IsPublicEndpoint(path))
        {
            await _next(context);
            return;
        }

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

    /// <summary>
    /// Check if the request is for a public endpoint that doesn't require X-Tenant-ID
    /// </summary>
    private static bool IsPublicEndpoint(string path)
    {
        var publicPaths = new[]
        {
            "/api/auth/login",
            "/api/auth/register",
            "/api/auth/refresh",
            "/api/auth/passkeys/registration/start",
            "/api/auth/passkeys/registration/complete",
            "/api/auth/passkeys/authentication/start",
            "/api/auth/passkeys/authentication/complete",
            "/health",
            "/healthz",
            "/live",
            "/ready",
            "/swagger",
            "/.well-known/",
            "/metrics"
        };

        return publicPaths.Any(p => path.StartsWith(p));
    }
}
