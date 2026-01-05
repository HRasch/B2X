using B2Connect.Shared.Core.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace B2Connect.Shared.Infrastructure.Authorization;

/// <summary>
/// Extension methods for registering authorization services.
/// </summary>
public static class AuthorizationServiceExtensions
{
    /// <summary>
    /// Adds the unified authorization system to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddUnifiedAuthorization(this IServiceCollection services)
    {
        // Register the permission manager
        services.AddScoped<IPermissionManager, PermissionManager>();

        // Register built-in authorization providers
        services.AddScoped<IAuthorizeProvider, StoreSettingsAuthorizeProvider>();
        services.AddScoped<IAuthorizeProvider, AccountAuthorizeProvider>();
        services.AddScoped<IAuthorizeProvider, RoleAuthorizeProvider>();

        return services;
    }

    /// <summary>
    /// Adds a custom authorization provider to the service collection.
    /// </summary>
    /// <typeparam name="TProvider">The authorization provider type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAuthorizeProvider<TProvider>(this IServiceCollection services)
        where TProvider : class, IAuthorizeProvider
    {
        services.AddScoped<IAuthorizeProvider, TProvider>();
        return services;
    }
}
