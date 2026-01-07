// <copyright file="TenantController.cs" company="NissenVelten">
// Copyright (c) NissenVelten Software GmbH. All rights reserved.
// </copyright>

using B2X.Shared.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2X.Store.Controllers;

/// <summary>
/// Controller for tenant-related operations in the store frontend.
/// </summary>
[ApiController]
[Route("api/tenant")]
public class TenantController : ControllerBase
{
    private readonly ILogger<TenantController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITenantSettingsAccessor _tenantSettingsAccessor;

    public TenantController(
        ILogger<TenantController> logger,
        IHttpClientFactory httpClientFactory,
        ITenantSettingsAccessor tenantSettingsAccessor)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _tenantSettingsAccessor = tenantSettingsAccessor;
    }

    /// <summary>
    /// Gets the store visibility settings for the current tenant.
    /// This endpoint is always public to allow the frontend to determine
    /// whether authentication is required before showing content.
    /// </summary>
    /// <returns>Store visibility configuration</returns>
    [HttpGet("visibility")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(StoreVisibilityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetStoreVisibility(CancellationToken cancellationToken)
    {
        // Get tenant ID from header or context
        if (!Request.Headers.TryGetValue("X-Tenant-ID", out var tenantIdHeader) ||
            !Guid.TryParse(tenantIdHeader.ToString(), out var tenantId))
        {
            return BadRequest(new
            {
                success = false,
                message = "X-Tenant-ID header is required"
            });
        }

        try
        {
            // TEMPORARILY USE TenantSettingsAccessor instead of Tenancy service for testing
            _logger.LogInformation("TenantController.GetStoreVisibility - Calling TenantSettingsAccessor");
            var settings = _tenantSettingsAccessor.GetSettings();
            _logger.LogInformation("TenantController.GetStoreVisibility - Settings: IsPublicStore={IsPublicStore}", settings?.IsPublicStore ?? true);

            // TEMPORARY: Hardcode the response for testing
            return Ok(new StoreVisibilityResponse
            {
                IsPublicStore = false, // HARDCODED FOR TESTING
                RequiresAuthentication = true, // HARDCODED FOR TESTING
                TenantId = tenantId,
                TenantName = null // TODO: Get from tenancy service when available
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting store visibility for tenant {TenantId}", tenantId);

            // Default to public store on error
            return Ok(new StoreVisibilityResponse
            {
                IsPublicStore = true,
                RequiresAuthentication = false,
                TenantId = tenantId
            });
        }
    }

    /// <summary>
    /// Gets the tenant configuration including supported languages and settings.
    /// This endpoint is public to allow the frontend to configure itself based on tenant settings.
    /// </summary>
    /// <returns>Tenant configuration</returns>
    [HttpGet("config")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TenantConfigurationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTenantConfiguration(CancellationToken cancellationToken)
    {
        // Get tenant ID from header or context
        if (!Request.Headers.TryGetValue("X-Tenant-ID", out var tenantIdHeader) ||
            !Guid.TryParse(tenantIdHeader.ToString(), out var tenantId))
        {
            return BadRequest(new
            {
                success = false,
                message = "X-Tenant-ID header is required"
            });
        }

        try
        {
            // Get tenant settings including supported languages
            var settings = _tenantSettingsAccessor.GetSettings();

            return Ok(new TenantConfigurationResponse
            {
                TenantId = tenantId,
                IsPublicStore = settings?.IsPublicStore ?? true,
                ShowPricesToAnonymous = settings?.ShowPricesToAnonymous ?? true,
                AllowGuestCheckout = settings?.AllowGuestCheckout ?? true,
                SupportedLanguages = settings?.SupportedLanguages ?? ["de", "en"]
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tenant configuration for tenant {TenantId}", tenantId);

            // Default configuration on error
            return Ok(new TenantConfigurationResponse
            {
                TenantId = tenantId,
                IsPublicStore = true,
                ShowPricesToAnonymous = true,
                AllowGuestCheckout = true,
                SupportedLanguages = ["de", "en"]
            });
        }
    }
}

/// <summary>
/// Response DTO for tenant configuration.
/// </summary>
public record TenantConfigurationResponse
{
    /// <summary>
    /// The tenant ID.
    /// </summary>
    public Guid TenantId { get; init; }

    /// <summary>
    /// Whether the store is publicly accessible without authentication.
    /// </summary>
    public bool IsPublicStore { get; init; }

    /// <summary>
    /// Whether prices are displayed to anonymous users.
    /// </summary>
    public bool ShowPricesToAnonymous { get; init; }

    /// <summary>
    /// Whether guest checkout is allowed.
    /// </summary>
    public bool AllowGuestCheckout { get; init; }

    /// <summary>
    /// Supported languages for this tenant (ISO 639-1 codes).
    /// </summary>
    public IReadOnlyList<string> SupportedLanguages { get; init; } = ["de", "en"];
}

/// <summary>
/// Response DTO for store visibility settings.
/// </summary>
public record StoreVisibilityResponse
{
    /// <summary>
    /// Whether the store is publicly accessible without authentication.
    /// </summary>
    public bool IsPublicStore { get; init; }

    /// <summary>
    /// Whether authentication is required to access the store.
    /// Inverse of IsPublicStore for frontend convenience.
    /// </summary>
    public bool RequiresAuthentication { get; init; }

    /// <summary>
    /// The tenant ID.
    /// </summary>
    public Guid TenantId { get; init; }

    /// <summary>
    /// The tenant name (optional, for display purposes).
    /// </summary>
    public string? TenantName { get; init; }
}

/// <summary>
/// Internal DTO for tenant service response.
/// </summary>
internal record TenantResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsPublicStore { get; init; } = true;
}
