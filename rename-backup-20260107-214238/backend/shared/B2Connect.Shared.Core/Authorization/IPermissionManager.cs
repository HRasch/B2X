namespace B2X.Shared.Core.Authorization;

/// <summary>
/// Manages permission checks by aggregating multiple authorization providers.
/// </summary>
public interface IPermissionManager
{
    /// <summary>
    /// Checks if the current context has the specified permission.
    /// A permission is granted if:
    /// - At least one provider allows it (IsAllowed returns true)
    /// - AND no provider forbids it (IsForbidden returns false for all)
    /// </summary>
    /// <param name="permission">The permission system name to check.</param>
    /// <returns>True if the permission is granted.</returns>
    bool HasPermission(string permission);

    /// <summary>
    /// Checks multiple permissions at once.
    /// </summary>
    /// <param name="permissions">The permissions to check.</param>
    /// <returns>True if all permissions are granted.</returns>
    bool HasAllPermissions(params string[] permissions);

    /// <summary>
    /// Checks if any of the specified permissions are granted.
    /// </summary>
    /// <param name="permissions">The permissions to check.</param>
    /// <returns>True if at least one permission is granted.</returns>
    bool HasAnyPermission(params string[] permissions);
}
