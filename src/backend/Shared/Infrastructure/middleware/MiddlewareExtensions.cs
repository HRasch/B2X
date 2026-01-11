using System;
using System.Diagnostics;
using B2X.Shared.Core.Authorization;
using B2X.Utils.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace B2X.Shared.Middleware;

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
/// Correlation middleware for debug tracking
/// Generates and propagates correlation IDs for request tracing
/// </summary>
public class CorrelationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationMiddleware> _logger;

    public CorrelationMiddleware(RequestDelegate next, ILogger<CorrelationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Extract or generate correlation ID
        var correlationId = ExtractOrGenerateCorrelationId(context);

        // Add to request headers for downstream services
        context.Request.Headers["X-Correlation-Id"] = correlationId;

        // Add to response headers for client visibility
        context.Response.Headers["X-Correlation-Id"] = correlationId;

        // Store in HttpContext for use by other middleware/components
        context.Items["CorrelationId"] = correlationId;

        // Enrich current activity with correlation data
        if (Activity.Current != null)
        {
            Activity.Current.SetTag("correlation.id", correlationId);
            Activity.Current.SetTag("correlation.tenant_id", context.Items["TenantId"]?.ToString());
        }

        // Log correlation context
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["TenantId"] = context.Items["TenantId"]?.ToString() ?? "unknown"
        }))
        {
            await _next(context);
        }
    }

    private static string ExtractOrGenerateCorrelationId(HttpContext context)
    {
        // Try to extract from request headers
        if (context.Request.Headers.TryGetValue("X-Correlation-Id", out var headerValue) &&
            !string.IsNullOrWhiteSpace(headerValue.ToString()))
        {
            return headerValue.ToString()!;
        }

        // Try to extract from traceparent header (W3C Trace Context)
        if (context.Request.Headers.TryGetValue("traceparent", out var traceparentValue) &&
            !string.IsNullOrWhiteSpace(traceparentValue.ToString()))
        {
            // Extract trace ID from traceparent header
            var parts = traceparentValue.ToString()!.Split('-');
            if (parts.Length >= 2)
            {
                return $"{context.Items["TenantId"]}-{GenerateSessionId()}-{parts[1]}";
            }
        }

        // Generate new correlation ID
        var tenantId = context.Items["TenantId"]?.ToString() ?? "unknown";
        var sessionId = GenerateSessionId();
        var requestId = GenerateRequestId();

        return $"{tenantId}-{sessionId}-{requestId}";
    }

    private static string GenerateSessionId()
    {
        // Generate a short session identifier
        return Guid.NewGuid().ToString("N").Substring(0, 8);
    }

    private static string GenerateRequestId()
    {
        // Use timestamp + random for uniqueness
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var random = Guid.NewGuid().ToString("N").Substring(0, 8);
        return $"{timestamp:x}-{random}";
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
    /// Adds correlation middleware for debug tracking.
    /// Must be called early in the pipeline, after tenant context but before other middleware.
    /// </summary>
    public static IApplicationBuilder UseCorrelation(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationMiddleware>();
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

/// <summary>
/// Extension methods for HttpContext to access correlation ID
/// </summary>
public static class HttpContextExtensions
{
    private const string CorrelationIdHeader = "X-Correlation-Id";
    private const string CorrelationIdItemKey = "CorrelationId";

    /// <summary>
    /// Gets the correlation ID from the current HttpContext
    /// </summary>
    /// <param name="context">The HttpContext</param>
    /// <returns>The correlation ID, or generates a new one if not present</returns>
    public static string GetCorrelationId(this HttpContext context)
    {
        // Try to get from request headers first
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var headerValue) &&
            !string.IsNullOrEmpty(headerValue.ToString()))
        {
            var correlationId = headerValue.ToString()!;
            context.Items[CorrelationIdItemKey] = correlationId;
            return correlationId;
        }

        // Try to get from context items (set by middleware)
        if (context.Items.TryGetValue(CorrelationIdItemKey, out var itemValue) &&
            itemValue is string existingId)
        {
            return existingId;
        }

        // Generate new correlation ID
        var newId = Guid.NewGuid().ToString();
        context.Items[CorrelationIdItemKey] = newId;
        return newId;
    }

    /// <summary>
    /// Sets the correlation ID in the HttpContext
    /// </summary>
    /// <param name="context">The HttpContext</param>
    /// <param name="correlationId">The correlation ID to set</param>
    public static void SetCorrelationId(this HttpContext context, string correlationId)
    {
        context.Items[CorrelationIdItemKey] = correlationId;
        context.Response.Headers[CorrelationIdHeader] = correlationId;
    }
}
