using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace B2Connect.Shared.Infrastructure.Middleware;

/// <summary>
/// Middleware to add security headers to all HTTP responses
/// Helps prevent XSS, Clickjacking, MIME type sniffing, etc.
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;

    public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // X-Content-Type-Options: Prevent MIME type sniffing
        // Tells browser not to MIME-sniff, trust Content-Type header
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";

        // X-Frame-Options: Prevent Clickjacking attacks
        // DENY = Cannot be displayed in a frame
        context.Response.Headers["X-Frame-Options"] = "DENY";

        // X-XSS-Protection: Legacy XSS protection (browser-level)
        // 1; mode=block = Enable XSS protection and block page if attack detected
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

        // Referrer-Policy: Control how much referrer information is shared
        // strict-origin-when-cross-origin = Send full referrer for same-origin, 
        // only origin for cross-origin
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // Content-Security-Policy: Prevent XSS, Clickjacking, etc.
        // default-src 'self' = Only allow resources from same origin
        const string csp = "default-src 'self'; " +
                  "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                  "style-src 'self' 'unsafe-inline'; " +
                  "img-src 'self' data: https:; " +
                  "font-src 'self' data:; " +
                  "connect-src 'self' https:; " +
                  "frame-ancestors 'none'; " +
                  "base-uri 'self'; " +
                  "form-action 'self'";

        context.Response.Headers["Content-Security-Policy"] = csp;

        // Permissions-Policy: Restrict browser feature access
        // Deny access to: geolocation, microphone, camera, payment
        context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=(), payment=()";

        _logger.LogDebug("Security headers applied to response");

        await _next(context);
    }
}

/// <summary>
/// Extension methods for security headers middleware
/// </summary>
public static class SecurityHeadersExtensions
{
    /// <summary>
    /// Add security headers middleware to the application
    /// Should be called early in the middleware pipeline
    /// </summary>
    public static IApplicationBuilder UseSecurityHeaders(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
