using B2Connect.Shared.Core.Authorization;

namespace B2Connect.Shared.Infrastructure.Authorization;

/// <summary>
/// Authorization provider based on individual user permissions.
/// Checks user-specific permissions that may override tenant or role defaults.
/// Maps to eGate "account" concept (B2Connect "user").
/// </summary>
public class AccountAuthorizeProvider : IAuthorizeProvider
{
    private readonly IUserPermissionAccessor _userPermissions;

    /// <summary>
    /// Initializes a new instance of AccountAuthorizeProvider.
    /// </summary>
    /// <param name="userPermissions">The user permission accessor.</param>
    public AccountAuthorizeProvider(IUserPermissionAccessor userPermissions)
    {
        _userPermissions = userPermissions;
    }

    /// <inheritdoc />
    public int Priority => 30; // Higher priority than StoreSettingsAuthorizeProvider (20)

    /// <inheritdoc />
    public bool IsAllowed(string permission)
    {
        var userPermissions = _userPermissions.GetUserPermissions();
        return userPermissions?.Contains(permission) ?? false;
    }

    /// <inheritdoc />
    public bool IsForbidden(string permission)
    {
        var forbiddenPermissions = _userPermissions.GetForbiddenPermissions();
        return forbiddenPermissions?.Contains(permission) ?? false;
    }
}

/// <summary>
/// Interface for accessing user-specific permissions.
/// </summary>
public interface IUserPermissionAccessor
{
    /// <summary>
    /// Gets the permissions explicitly granted to the current user.
    /// </summary>
    /// <returns>A collection of permission strings, or null if no user context.</returns>
    IReadOnlyCollection<string>? GetUserPermissions();

    /// <summary>
    /// Gets the permissions explicitly forbidden for the current user.
    /// </summary>
    /// <returns>A collection of permission strings, or null if no user context.</returns>
    IReadOnlyCollection<string>? GetForbiddenPermissions();
}