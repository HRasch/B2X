using B2Connect.Shared.Core.Authorization;
using B2Connect.Shared.Tenancy.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using B2Connect.Shared.Infrastructure.Authorization;

namespace B2Connect.Gateway.Store.Authorization;

/// <summary>
/// Implementation of IUserPermissionAccessor for the Store Gateway.
/// Maps eGate "account" concept to B2Connect user permissions.
/// </summary>
public class UserPermissionAccessor : IUserPermissionAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITenantContext _tenantContext;

    /// <summary>
    /// Initializes a new instance of UserPermissionAccessor.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="tenantContext">The tenant context.</param>
    public UserPermissionAccessor(IHttpContextAccessor httpContextAccessor, ITenantContext tenantContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _tenantContext = tenantContext;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string>? GetUserPermissions()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        // Get user permissions from claims or user profile
        // This maps to eGate "account" permissions (B2Connect user-level permissions)
        var permissions = user.FindAll("permissions")
            .Select(c => c.Value)
            .ToList();

        // Add default user permissions if authenticated
        if (permissions.Count == 0)
        {
            permissions.AddRange(new[]
            {
                Permissions.Store.Access,
                Permissions.Store.ViewPrices,
                Permissions.Store.AddToCart,
                Permissions.Account.ManageAddresses,
                Permissions.Order.ViewOrders,
                Permissions.Order.PlaceOrder
            });
        }

        return permissions.AsReadOnly();
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string>? GetForbiddenPermissions()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        // Get explicitly forbidden permissions from claims
        // This could be used for account-level restrictions
        var forbidden = user.FindAll("forbidden_permissions")
            .Select(c => c.Value)
            .ToList();

        return forbidden.Count > 0 ? forbidden.AsReadOnly() : null;
    }
}