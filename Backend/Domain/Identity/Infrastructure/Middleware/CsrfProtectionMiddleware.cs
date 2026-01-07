using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace B2Connect.Identity.Infrastructure.Middleware;

/// <summary>
/// CSRF Protection Middleware
/// Validates CSRF tokens for state-changing requests (POST, PUT, DELETE, PATCH)
/// </summary>
public class CsrfProtectionMiddleware
{
    private readonly RequestDelegate _next;

    public CsrfProtectionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip CSRF validation for:
        // - GET, HEAD, OPTIONS requests (safe methods)
        // - Authentication endpoints (login, refresh, logout)
        // - Health checks and public endpoints
        var method = context.Request.Method;
        var path = context.Request.Path.Value;

        if (IsSafeMethod(method) ||
            IsAuthEndpoint(path) ||
            IsPublicEndpoint(path))
        {
            await _next(context).ConfigureAwait(false);
            return;
        }

        // For state-changing requests, validate CSRF token
        var csrfTokenFromCookie = context.Request.Cookies["csrfToken"];
        var csrfTokenFromHeader = context.Request.Headers["X-CSRF-Token"].FirstOrDefault();

        if (string.IsNullOrEmpty(csrfTokenFromCookie) ||
            string.IsNullOrEmpty(csrfTokenFromHeader) ||
!string.Equals(csrfTokenFromCookie, csrfTokenFromHeader, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "CSRF token validation failed",
                message = "Invalid or missing CSRF token"
            }).ConfigureAwait(false);
            return;
        }

        await _next(context).ConfigureAwait(false);
    }

    private static bool IsSafeMethod(string method)
    {
        return method is "GET" or "HEAD" or "OPTIONS";
    }

    private static bool IsAuthEndpoint(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        return path.StartsWith("/api/auth/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPublicEndpoint(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        // Add other public endpoints that don't need CSRF protection
        return path.Equals("/", StringComparison.OrdinalIgnoreCase) ||
               path.Equals("/health", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWith("/api/public/", StringComparison.OrdinalIgnoreCase);
    }
}
