using B2X.Catalog.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace B2X.Catalog.Handlers;

/// <summary>
/// Wolverine HTTP Handler for Terms Acceptance
/// Story: P0.6-US-005 - Mandatory Terms & Conditions Acceptance
///
/// Endpoint: POST /api/checkout/accept-terms
///
/// This handler:
/// 1. Receives terms acceptance from checkout page
/// 2. Validates all required terms are checked
/// 3. Logs acceptance for audit trail
/// 4. Returns confirmation to frontend
/// </summary>
public class TermsAcceptanceHandler
{
    private readonly TermsAcceptanceService _service;
    private readonly ILogger<TermsAcceptanceHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TermsAcceptanceHandler(
        TermsAcceptanceService service,
        ILogger<TermsAcceptanceHandler> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _service = service;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Wolverine Service Handler: POST /api/checkout/accept-terms
    /// Automatically discovered and registered as HTTP endpoint by Wolverine
    /// </summary>
    public Task<RecordTermsAcceptanceResponse> AcceptTerms(
        Guid tenantId,
        RecordTermsAcceptanceRequest request,
        CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgent = httpContext?.Request?.Headers["User-Agent"].ToString() ?? "unknown";

        _logger.LogInformation(
            "Terms acceptance request from {IpAddress} for tenant {TenantId}",
            ipAddress, tenantId);

        return _service.RecordAcceptanceAsync(
            tenantId,
            request,
            ipAddress,
            userAgent,
            cancellationToken);
    }
}
