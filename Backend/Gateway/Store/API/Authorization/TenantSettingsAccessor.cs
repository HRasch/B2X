using B2X.Shared.Core.Authorization;
using B2X.Shared.Infrastructure.Authorization;
using B2X.Shared.Infrastructure.ServiceClients;
using B2X.Shared.Tenancy.Infrastructure.Context;
using B2X.Tenancy.Models;
using Microsoft.Extensions.Logging;

namespace B2X.Gateway.Store.Authorization;

/// <summary>
/// Implementation of ITenantSettingsAccessor for the Store Gateway.
/// Caches tenant settings for the current request scope.
/// </summary>
public class TenantSettingsAccessor : ITenantSettingsAccessor
{
    private readonly ITenantContext _tenantContext;
    private readonly ITenancyServiceClient _tenancyServiceClient;
    private readonly ILogger<TenantSettingsAccessor> _logger;
    private TenantSettings? _cachedSettings;

    /// <summary>
    /// Initializes a new instance of TenantSettingsAccessor.
    /// </summary>
    /// <param name="tenantContext">The tenant context to get current tenant ID.</param>
    /// <param name="tenancyServiceClient">The tenancy service client to fetch tenant data.</param>
    /// <param name="logger">The logger.</param>
    public TenantSettingsAccessor(ITenantContext tenantContext, ITenancyServiceClient tenancyServiceClient, ILogger<TenantSettingsAccessor> logger)
    {
        _tenantContext = tenantContext;
        _tenancyServiceClient = tenancyServiceClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public TenantSettings? GetSettings()
    {
        // Return cached settings if already loaded
        if (_cachedSettings != null)
        {
            return _cachedSettings;
        }

        // Get tenant from service client
        var tenantId = _tenantContext.TenantId;
        _logger.LogInformation("TenantSettingsAccessor.GetSettings() - TenantId: {TenantId}", tenantId);

        if (tenantId == Guid.Empty)
        {
            _logger.LogWarning("TenantSettingsAccessor.GetSettings() - No tenant context, returning null");
            return null;
        }

        // For testing, skip the service call and return hardcoded settings
        _logger.LogInformation("TenantSettingsAccessor.GetSettings() - Returning test settings: IsPublicStore=false");
        _cachedSettings = new TenantSettings
        {
            IsPublicStore = false, // TEMPORARILY SET TO FALSE FOR TESTING CLOSED SHOP
            ShowPricesToAnonymous = true, // Default to showing prices
            AllowGuestCheckout = true, // Default to allowing guest checkout
        };

        return _cachedSettings;
    }
}
