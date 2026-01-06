using System.Security.Claims;
using Microsoft.AspNetCore.Builder;

namespace B2Connect.Utils.Extensions;

/// <summary>
/// Utility extension methods for ASP.NET Core
/// </summary>
public static class UtilityExtensions
{
    /// <summary>
    /// Placeholder for utility extension methods
    /// </summary>
    public static IApplicationBuilder UseUtilities(this IApplicationBuilder builder)
    {
        return builder;
    }

    /// <summary>
    /// Extract tenant ID from claims
    /// </summary>
    public static Guid GetTenantId(this ClaimsPrincipal user)
    {
        var tenantIdClaim = user?.FindFirst("tenant_id")?.Value;

        if (Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            return tenantId;
        }

        return Guid.Empty;
    }
}
