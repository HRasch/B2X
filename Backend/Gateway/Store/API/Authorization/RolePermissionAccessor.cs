using B2Connect.Shared.Core.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using B2Connect.Shared.Infrastructure.Authorization;

namespace B2Connect.Gateway.Store.Authorization;

/// <summary>
/// Implementation of IRolePermissionAccessor for the Store Gateway.
/// Maps eGate "user" concept to B2Connect userprofile with roles.
/// </summary>
public class RolePermissionAccessor : IRolePermissionAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of RolePermissionAccessor.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public RolePermissionAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string>? GetRolePermissions()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var permissions = new List<string>();

        // Get user roles from claims
        var roles = user.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        // Map roles to permissions
        foreach (var role in roles)
        {
            switch (role.ToLowerInvariant())
            {
                case "admin":
                case "administrator":
                    permissions.AddRange(new[]
                    {
                        Permissions.Store.Access,
                        Permissions.Store.BrowseAnonymous,
                        Permissions.Store.ViewPrices,
                        Permissions.Store.AddToCart,
                        Permissions.Store.GuestCheckout,
                        Permissions.Account.ManageAddresses,
                        Permissions.Account.ManageSubAccounts,
                        Permissions.Order.ViewOrders,
                        Permissions.Order.PlaceOrder,
                        Permissions.Order.ApproveOrders,
                        Permissions.Order.ViewPurchaseInfo,
                        Permissions.Cart.Approve
                    });
                    break;

                case "manager":
                case "storemanager":
                    permissions.AddRange(new[]
                    {
                        Permissions.Store.Access,
                        Permissions.Store.BrowseAnonymous,
                        Permissions.Store.ViewPrices,
                        Permissions.Store.AddToCart,
                        Permissions.Account.ManageAddresses,
                        Permissions.Order.ViewOrders,
                        Permissions.Order.PlaceOrder,
                        Permissions.Order.ViewPurchaseInfo
                    });
                    break;

                case "customer":
                case "user":
                default:
                    permissions.AddRange(new[]
                    {
                        Permissions.Store.Access,
                        Permissions.Store.ViewPrices,
                        Permissions.Store.AddToCart,
                        Permissions.Account.ManageAddresses,
                        Permissions.Order.ViewOrders,
                        Permissions.Order.PlaceOrder
                    });
                    break;
            }
        }

        return permissions.Distinct().ToList().AsReadOnly();
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string>? GetForbiddenRolePermissions()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var forbidden = new List<string>();

        // Get user roles from claims
        var roles = user.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        // Define role-based restrictions
        foreach (var role in roles)
        {
            switch (role.ToLowerInvariant())
            {
                case "guest":
                case "anonymous":
                    forbidden.AddRange(new[]
                    {
                        Permissions.Store.AddToCart,
                        Permissions.Account.ManageAddresses,
                        Permissions.Order.PlaceOrder,
                        Permissions.Order.ApproveOrders
                    });
                    break;

                case "restricted":
                    forbidden.AddRange(new[]
                    {
                        Permissions.Order.ApproveOrders,
                        Permissions.Cart.Approve,
                        Permissions.Account.ManageSubAccounts
                    });
                    break;
            }
        }

        return forbidden.Count > 0 ? forbidden.Distinct().ToList().AsReadOnly() : null;
    }
}