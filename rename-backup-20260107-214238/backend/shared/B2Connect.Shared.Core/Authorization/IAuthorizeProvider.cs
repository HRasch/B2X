namespace B2X.Shared.Core.Authorization;

/// <summary>
/// Interface for authorization providers that determine permissions.
/// Multiple providers can be registered and aggregated by the PermissionManager.
/// Inspired by eGate's unified permission configuration pattern.
/// </summary>
public interface IAuthorizeProvider
{
    /// <summary>
    /// Gets the priority of this provider. Lower values are evaluated first.
    /// </summary>
    int Priority => 100;

    /// <summary>
    /// Checks if this provider explicitly allows the given permission.
    /// </summary>
    /// <param name="permission">The permission system name to check.</param>
    /// <returns>True if this provider explicitly allows the permission.</returns>
    bool IsAllowed(string permission);

    /// <summary>
    /// Checks if this provider explicitly forbids the given permission.
    /// Forbidden permissions take precedence over allowed permissions.
    /// </summary>
    /// <param name="permission">The permission system name to check.</param>
    /// <returns>True if this provider explicitly forbids the permission.</returns>
    bool IsForbidden(string permission);
}
