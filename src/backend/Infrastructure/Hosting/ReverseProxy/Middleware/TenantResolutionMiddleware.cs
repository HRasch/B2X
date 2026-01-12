using B2X.ReverseProxy.Services;

namespace B2X.ReverseProxy.Middleware;

/// <summary>
/// Middleware that resolves the tenant from the incoming request's host header
/// and adds the X-Tenant-ID header for downstream services.
/// </summary>
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITenantDomainResolver _domainResolver;
    private readonly ILogger<TenantResolutionMiddleware> _logger;

    public const string TenantIdHeader = "X-Tenant-ID";
    public const string TenantSlugHeader = "X-Tenant-Slug";

    public TenantResolutionMiddleware(
        RequestDelegate next,
        ITenantDomainResolver domainResolver,
        ILogger<TenantResolutionMiddleware> logger)
    {
        _next = next;
        _domainResolver = domainResolver;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var host = context.Request.Host.Host;

        // Skip tenant resolution for health checks
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        // Skip tenant resolution for localhost/development hosts
        if (IsLocalhostHost(host))
        {
            await _next(context);
            return;
        }

        // If tenant header already exists, preserve it
        if (context.Request.Headers.ContainsKey(TenantIdHeader))
        {
            await _next(context);
            return;
        }

        try
        {
            var tenantInfo = await _domainResolver.ResolveAsync(host, context.RequestAborted);

            if (tenantInfo is null || tenantInfo.Status == TenantStatus.Inactive)
            {
                var reason = tenantInfo is null ? "not found" : "inactive";
                _logger.LogWarning("Tenant {Reason} for host: {Host}", reason, host);
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Tenant not found",
                    message = $"No tenant is configured for domain: {host}"
                });
                return;
            }

            // Add tenant headers for downstream services
            context.Request.Headers[TenantIdHeader] = tenantInfo.TenantId.ToString();
            context.Request.Headers[TenantSlugHeader] = tenantInfo.Slug;

            _logger.LogDebug(
                "Resolved tenant {TenantSlug} (ID: {TenantId}) for host {Host}",
                tenantInfo.Slug,
                tenantInfo.TenantId,
                host);

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving tenant for host: {Host}", host);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Internal server error",
                message = "An error occurred while processing the request"
            });
        }
    }

    private static bool IsLocalhostHost(string host)
    {
        return host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
               host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase) ||
               host.StartsWith("localhost:", StringComparison.OrdinalIgnoreCase) ||
               host.StartsWith("127.0.0.1:", StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Extension methods for TenantResolutionMiddleware.
/// </summary>
public static class TenantResolutionMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantResolutionMiddleware>();
    }
}
