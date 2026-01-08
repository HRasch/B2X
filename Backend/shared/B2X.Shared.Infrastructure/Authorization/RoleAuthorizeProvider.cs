using B2X.Shared.Core.Authorization;

namespace B2X.Shared.Infrastructure.Authorization;

/// <summary>
/// Authorization provider based on user roles.
/// Checks role-based permissions that apply to groups of users.
/// Maps to eGate "user" concept (B2X "userprofile" with roles).
/// </summary>
public class RoleAuthorizeProvider : IAuthorizeProvider
{
    private readonly IRolePermissionAccessor _rolePermissions;

    /// <summary>
    /// Initializes a new instance of RoleAuthorizeProvider.
    /// </summary>
    /// <param name="rolePermissions">The role permission accessor.</param>
    public RoleAuthorizeProvider(IRolePermissionAccessor rolePermissions)
    {
        _rolePermissions = rolePermissions;
    }

    /// <inheritdoc />
    public int Priority => 10; // Lower priority than AccountAuthorizeProvider (30) and StoreSettingsAuthorizeProvider (20)

    /// <inheritdoc />
    public bool IsAllowed(string permission)
    {
        var rolePermissions = _rolePermissions.GetRolePermissions();
        return rolePermissions?.Contains(permission) ?? false;
    }

    /// <inheritdoc />
    public bool IsForbidden(string permission)
    {
        var forbiddenPermissions = _rolePermissions.GetForbiddenRolePermissions();
        return forbiddenPermissions?.Contains(permission) ?? false;
    }
}

/// <summary>
/// Interface for accessing role-based permissions.
/// </summary>
public interface IRolePermissionAccessor
{
    /// <summary>
    /// Gets the permissions granted by the current user's roles.
    /// </summary>
    /// <returns>A collection of permission strings, or null if no user context.</returns>
    IReadOnlyCollection<string>? GetRolePermissions();

    /// <summary>
    /// Gets the permissions forbidden by the current user's roles.
    /// </summary>
    /// <returns>A collection of permission strings, or null if no user context.</returns>
    IReadOnlyCollection<string>? GetForbiddenRolePermissions();
}
