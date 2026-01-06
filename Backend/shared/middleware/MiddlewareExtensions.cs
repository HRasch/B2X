using System;
using B2Connect.Shared.Core.Authorization;
using B2Connect.Utils.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace B2Connect.Shared.Middleware;

/// <summary>
/// Tenant context middleware for multitenant applications
/// Extracts tenant ID from claims and request headers
/// </summary>
public class TenantContextMiddleware
{
    private readonly RequestDelegate _next;

    public TenantContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContextAccessor tenantContextAccessor, IServiceProvider serviceProvider)
    {
        var tenantId = context.User.GetTenantId();

        if (tenantId == Guid.Empty && context.Request.Headers.TryGetValue("X-Tenant-ID", out var headerValue) && Guid.TryParse(headerValue.ToString(), out var headerId))
        {
            tenantId = headerId;
        }

        if (tenantId != Guid.Empty)
        {
            tenantContextAccessor.SetTenantId(tenantId);
            context.Items["TenantId"] = tenantId;
        }

        await _next(context);
    }
}

/// <summary>
/// Interface for tenant context accessor
/// </summary>
public interface ITenantContextAccessor
{
    Guid GetTenantId();
    void SetTenantId(Guid tenantId);
}

/// <summary>
/// Default implementation of tenant context accessor using AsyncLocal
/// </summary>
public class TenantContextAccessor : ITenantContextAccessor
{
    private static readonly AsyncLocal<Guid> TenantIdLocal = new();

    public Guid GetTenantId() => TenantIdLocal.Value;
    public void SetTenantId(Guid tenantId) => TenantIdLocal.Value = tenantId;
}

/// <summary>
/// Exception handling middleware
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            message = "An error occurred processing your request",
            error = exception.Message
        };

        context.Response.StatusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}

/// <summary>
/// Service interface for checking tenant store access mode
/// </summary>
public interface ITenantStoreAccessService
{
    /// <summary>
    /// Checks if the tenant's store is publicly accessible.
    /// </summary>
    /// <param name="tenantId">The tenant ID to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if public store, false if closed shop requiring authentication</returns>
    Task<bool> IsPublicStoreAsync(Guid tenantId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Middleware that enforces store access based on tenant configuration.
/// For closed shops (IsPublicStore = false), requires authentication.
/// </summary>
public class StoreAccessMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<StoreAccessMiddleware> _logger;

    // Paths that are always public (authentication, health checks, etc.)
    private static readonly string[] AlwaysPublicPaths =
    [
        "/api/auth/",
        "/api/tenant/visibility",
        "/health",
        "/healthz",
        "/live",
        "/ready",
        "/.well-known/",
        "/swagger",
        "/metrics"
    ];

    public StoreAccessMiddleware(RequestDelegate next, ILogger<StoreAccessMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITenantContextAccessor tenantContextAccessor,
        IPermissionManager permissionManager)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        // Always allow public paths
        if (IsAlwaysPublicPath(path))
        {
            await _next(context);
            return;
        }

        var tenantId = tenantContextAccessor.GetTenantId();

        // If no tenant context, continue (let other middleware handle)
        if (tenantId == Guid.Empty)
        {
            await _next(context);
            return;
        }

        // Check if tenant allows anonymous browsing (public store)
        var allowsAnonymousBrowsing = permissionManager.HasPermission(Permissions.Store.BrowseAnonymous);

        if (allowsAnonymousBrowsing)
        {
            // Public store - allow all requests
            await _next(context);
            return;
        }

        // Closed shop - require authentication
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            _logger.LogInformation(
                "Unauthenticated access to closed shop denied for tenant {TenantId}, path: {Path}",
                tenantId, path);

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "This store requires authentication. Please log in to access.",
                code = "CLOSED_SHOP_AUTH_REQUIRED"
            });
            return;
        }

        // User is authenticated, allow access
        await _next(context);
    }

    private static bool IsAlwaysPublicPath(string path)
    {
        return AlwaysPublicPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>
/// Extension methods for registering middleware
/// </summary>
public static class MiddlewareExtensions
{
    public static IServiceCollection AddTenantContext(this IServiceCollection services)
    {
        services.AddScoped<ITenantContextAccessor, TenantContextAccessor>();
        return services;
    }

    public static IApplicationBuilder UseTenantContext(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TenantContextMiddleware>();
    }

    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    /// <summary>
    /// Adds store access middleware that enforces authentication for closed shops.
    /// Must be called after UseTenantContext and UseAuthentication.
    /// </summary>
    public static IApplicationBuilder UseStoreAccess(this IApplicationBuilder app)
    {
        return app.UseMiddleware<StoreAccessMiddleware>();
    }
}
