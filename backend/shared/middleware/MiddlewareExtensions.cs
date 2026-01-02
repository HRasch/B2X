using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using B2Connect.Utils.Extensions;

namespace B2Connect.Middleware;

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

    public async Task InvokeAsync(HttpContext context, ITenantContextAccessor tenantContextAccessor)
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
}
