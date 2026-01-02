using B2Connect.Shared.Tenancy.Infrastructure.Context;
using B2Connect.Shared.Infrastructure.ServiceClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace B2Connect.Shared.Tenancy.Infrastructure.Middleware;

/// <summary>
/// Middleware that extracts X-Tenant-ID header and sets it in the ITenantContext.
/// Must be registered BEFORE any endpoint that needs tenant filtering.
///
/// Tenant Resolution Strategy (with Security Validation):
/// 1. JWT Tenant Claim (source of truth, if authenticated)
/// 2. X-Tenant-ID Header (validated against JWT if authenticated)
/// 3. Host-based lookup via TenancyServiceClient (validated, rate-limited)
/// 4. Development fallback (ONLY in Development environment)
///
/// Security Features:
/// - Tenant spoofing prevention (JWT validation)
/// - Environment-aware fallback (never in production)
/// - Host input validation (prevent injection)
/// - Generic error messages (prevent information disclosure)
/// - Audit logging for security events
///
/// Registration in Program.cs:
///   app.UseMiddleware<TenantContextMiddleware>();
/// </summary>
public partial class TenantContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TenantContextMiddleware> _logger;
    private readonly IHostEnvironment _environment;
    private static readonly Regex DomainValidationRegex = MyRegex();

    public TenantContextMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        ILogger<TenantContextMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ITenantContext tenantContext,
        ITenancyServiceClient tenancyClient)
    {
        // Skip tenant validation for public endpoints (Login, Register, Health, etc.)
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        if (IsPublicEndpoint(path))
        {
            await _next(context);
            return;
        }

        Guid? tenantId = null;
        Guid? jwtTenantId = null;
        Guid? headerTenantId = null;
        var isAuthenticated = context.User?.Identity?.IsAuthenticated ?? false;

        // 1. Extract tenant from JWT (source of truth for authenticated requests)
        if (isAuthenticated)
        {
            jwtTenantId = ExtractTenantFromJwt(context);
            if (jwtTenantId.HasValue)
            {
                _logger.LogDebug("Tenant ID extracted from JWT: {TenantId}", jwtTenantId);
            }
        }

        // 2. Extract tenant from X-Tenant-ID header
        var tenantIdHeader = context.Request.Headers["X-Tenant-ID"].FirstOrDefault();
        if (!string.IsNullOrEmpty(tenantIdHeader) && Guid.TryParse(tenantIdHeader, out var parsedHeaderTenantId))
        {
            headerTenantId = parsedHeaderTenantId;
            _logger.LogDebug("Tenant ID extracted from X-Tenant-ID header: {TenantId}", headerTenantId);
        }

        // 3. SECURITY: Validate JWT and Header match (prevent tenant spoofing)
        if (isAuthenticated && jwtTenantId.HasValue && headerTenantId.HasValue && jwtTenantId != headerTenantId)
        {
            _logger.LogWarning(
                "SECURITY ALERT: Tenant mismatch detected! JWT: {JwtTenant}, Header: {HeaderTenant}, User: {UserId}, IP: {IP}",
                jwtTenantId, headerTenantId, context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, context.Connection.RemoteIpAddress);

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                error = "Access denied. Please contact support if this persists."
            });
            return;
        }

        // 4. Use JWT tenant if available (highest trust)
        if (jwtTenantId.HasValue)
        {
            tenantId = jwtTenantId;
        }
        // 5. Use header tenant if no JWT (e.g., API key auth)
        else if (headerTenantId.HasValue)
        {
            tenantId = headerTenantId;

            // Validate tenant ownership for authenticated users
            if (isAuthenticated)
            {
                var userId = ExtractUserIdFromJwt(context);
                if (userId.HasValue)
                {
                    var hasAccess = await tenancyClient.ValidateTenantAccessAsync(tenantId.Value, userId.Value);
                    if (!hasAccess)
                    {
                        _logger.LogWarning(
                            "SECURITY ALERT: User {UserId} attempted to access unauthorized Tenant {TenantId}",
                            userId, tenantId);

                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            success = false,
                            error = "Access denied. Please contact support if this persists."
                        });
                        return;
                    }
                }
            }
        }

        // 6. Try host-based lookup (Production)
        if (!tenantId.HasValue)
        {
            var host = context.Request.Host.Host;

            // SECURITY: Validate host format (prevent injection)
            if (!IsValidDomainName(host))
            {
                _logger.LogWarning("SECURITY ALERT: Invalid host format detected: {Host}, IP: {IP}",
                    host, context.Connection.RemoteIpAddress);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    error = "Invalid request. Please contact support."
                });
                return;
            }

            _logger.LogDebug("Attempting host-based tenant lookup for: {Host}", host);

            try
            {
                var tenant = await tenancyClient.GetTenantByDomainAsync(host);
                if (tenant?.IsActive == true)
                {
                    tenantId = tenant.Id;
                    _logger.LogInformation("Tenant ID resolved from host {Host}: {TenantId}", host, tenantId);
                }
                else if (tenant?.IsActive == false)
                {
                    _logger.LogWarning("Inactive tenant accessed via host {Host}: {TenantId}", host, tenant.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to resolve tenant from host {Host}", host);
            }
        }

        // 7. Development fallback (ONLY in Development environment)
        if (!tenantId.HasValue)
        {
            var useFallback = _configuration.GetValue<bool>("Tenant:Development:UseFallback", false);

            // SECURITY: Prevent fallback in non-development environments
            if (useFallback && !_environment.IsDevelopment())
            {
                _logger.LogCritical(
                    "SECURITY ALERT: Development fallback is enabled in {Environment} environment! This is a critical security misconfiguration.",
                    _environment.EnvironmentName);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    error = "Service configuration error. Please contact support."
                });
                return;
            }

            if (useFallback && _environment.IsDevelopment())
            {
                var fallbackTenantIdStr = _configuration["Tenant:Development:FallbackTenantId"];
                if (!string.IsNullOrEmpty(fallbackTenantIdStr) && Guid.TryParse(fallbackTenantIdStr, out var fallbackTenantId))
                {
                    tenantId = fallbackTenantId;
                    _logger.LogWarning("Using Development fallback tenant ID: {TenantId}", tenantId);
                }
            }
        }

        // 8. No tenant ID could be resolved
        if (!tenantId.HasValue || tenantId.Value == Guid.Empty)
        {
            // Log detailed info for debugging (not sent to client)
            _logger.LogWarning(
                "Tenant resolution failed. Path: {Path}, Host: {Host}, HeaderProvided: {HeaderProvided}, Authenticated: {Authenticated}, IP: {IP}",
                path, context.Request.Host.Host, !string.IsNullOrEmpty(tenantIdHeader), isAuthenticated, context.Connection.RemoteIpAddress);

            // Generic error message (prevent information disclosure)
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                error = "Invalid request. Please contact support if this persists."
            });
            return;
        }

        // 9. Set tenant in context (available to handlers/DbContext)
        ((TenantContext)tenantContext).TenantId = tenantId.Value;
        context.Items["TenantId"] = tenantId.Value;

        _logger.LogDebug("Request processing with Tenant ID: {TenantId}", tenantId.Value);

        await _next(context);
    }

    /// <summary>
    /// Extract tenant ID from JWT claims (source of truth)
    /// </summary>
    private static Guid? ExtractTenantFromJwt(HttpContext context)
    {
        var tenantClaim = context.User?.FindFirst("tenant_id") ?? context.User?.FindFirst("TenantId");
        if (tenantClaim == null || !Guid.TryParse(tenantClaim.Value, out var tenantId))
        {
            return null;
        }

        return tenantId;
    }

    /// <summary>
    /// Extract user ID from JWT claims
    /// </summary>
    private static Guid? ExtractUserIdFromJwt(HttpContext context)
    {
        var userClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier) ?? context.User?.FindFirst("sub");
        if (userClaim == null || !Guid.TryParse(userClaim.Value, out var userId))
        {
            return null;
        }

        return userId;
    }

    /// <summary>
    /// Validate domain name format (RFC 1035 compliance, prevent injection attacks)
    /// </summary>
    private static bool IsValidDomainName(string domain)
    {
        if (string.IsNullOrWhiteSpace(domain) || domain.Length > 253)
        {
            return false;
        }

        return DomainValidationRegex.IsMatch(domain);
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

    [GeneratedRegex(@"^[a-z0-9]([a-z0-9\-]{0,61}[a-z0-9])?(\.[a-z0-9]([a-z0-9\-]{0,61}[a-z0-9])?)*$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex MyRegex();
}
