using B2Connect.Shared.Core.Authorization;

namespace B2Connect.Shared.Infrastructure.Authorization;

/// <summary>
/// Authorization provider based on store/tenant settings.
/// Checks tenant-level configuration like IsPublicStore.
/// Uses a tenant settings accessor to get cached tenant configuration.
/// </summary>
public class StoreSettingsAuthorizeProvider : IAuthorizeProvider
{
    private readonly ITenantSettingsAccessor _tenantSettings;

    /// <summary>
    /// Initializes a new instance of StoreSettingsAuthorizeProvider.
    /// </summary>
    /// <param name="tenantSettings">The tenant settings accessor.</param>
    public StoreSettingsAuthorizeProvider(ITenantSettingsAccessor tenantSettings)
    {
        _tenantSettings = tenantSettings;
    }

    /// <inheritdoc />
    public int Priority => 50; // Higher priority than role-based (evaluated first)

    /// <inheritdoc />
    public bool IsAllowed(string permission)
    {
        var settings = _tenantSettings.GetSettings();
        if (settings == null)
        {
            return false;
        }

        return permission switch
        {
            // Public stores allow anonymous browsing
            Permissions.Store.Access => true, // Always allow store access (auth is separate)
            Permissions.Store.BrowseAnonymous => settings.IsPublicStore,
            Permissions.Store.ViewPrices => settings.IsPublicStore, // TODO: Add PriceSettings
            Permissions.Store.GuestCheckout => settings.IsPublicStore, // TODO: Add checkout settings
            _ => false
        };
    }

    /// <inheritdoc />
    public bool IsForbidden(string permission)
    {
        var settings = _tenantSettings.GetSettings();
        if (settings == null)
        {
            // No tenant context means no store access
            return permission == Permissions.Store.Access;
        }

        return permission switch
        {
            // Closed shops forbid anonymous browsing
            Permissions.Store.BrowseAnonymous => !settings.IsPublicStore,
            Permissions.Store.GuestCheckout => !settings.IsPublicStore,
            _ => false
        };
    }
}

/// <summary>
/// Provides access to tenant settings for authorization checks.
/// Implementation should cache settings for the request scope.
/// </summary>
public interface ITenantSettingsAccessor
{
    /// <summary>
    /// Gets the tenant settings for the current request.
    /// </summary>
    TenantSettings? GetSettings();
}

/// <summary>
/// Represents tenant settings relevant for authorization.
/// </summary>
public class TenantSettings
{
    /// <summary>
    /// Gets or sets whether the store is publicly accessible (B2C).
    /// If false, authentication is required (B2B closed shop).
    /// </summary>
    public bool IsPublicStore { get; init; } = true;

    /// <summary>
    /// Gets or sets whether prices are displayed to anonymous users.
    /// </summary>
    public bool ShowPricesToAnonymous { get; init; } = true;

    /// <summary>
    /// Gets or sets whether guest checkout is allowed.
    /// </summary>
    public bool AllowGuestCheckout { get; init; } = true;
}
