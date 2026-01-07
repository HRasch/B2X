namespace B2Connect.Shared.Core.Authorization;

/// <summary>
/// Default implementation of IPermissionManager that aggregates multiple authorization providers.
/// Uses the eGate pattern: permission is granted if allowed by any provider AND not forbidden by any provider.
/// </summary>
public class PermissionManager : IPermissionManager
{
    private readonly IAuthorizeProvider[] _providers;

    /// <summary>
    /// Initializes a new instance of the PermissionManager.
    /// </summary>
    /// <param name="providers">The authorization providers to aggregate.</param>
    public PermissionManager(IEnumerable<IAuthorizeProvider> providers)
    {
        // Order by priority (lower values first)
        _providers = providers.OrderBy(p => p.Priority).ToArray();
    }

    /// <inheritdoc />
    public bool HasPermission(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
        {
            return false;
        }

        // Check if any provider forbids this permission (forbidden takes precedence)
        if (_providers.Any(p => p.IsForbidden(permission)))
        {
            return false;
        }

        // Check if at least one provider allows this permission
        return _providers.Any(p => p.IsAllowed(permission));
    }

    /// <inheritdoc />
    public bool HasAllPermissions(params string[] permissions)
    {
        return permissions.All(HasPermission);
    }

    /// <inheritdoc />
    public bool HasAnyPermission(params string[] permissions)
    {
        return permissions.Any(HasPermission);
    }
}
