using Microsoft.Extensions.DependencyInjection;

namespace B2Connect.Shared.User;

/// <summary>
/// Service Collection Extension für Entity Extensions
/// </summary>
public static class EntityExtensionServiceCollectionExtensions
{
    /// <summary>
    /// Registriere alle Entity Extension Services
    /// </summary>
    public static IServiceCollection AddEntityExtensions(this IServiceCollection services)
    {
        // Service für Custom Properties Management
        services.AddScoped<IEntityExtensionService, EntityExtensionService>();

        // Integration für enventa Trade ERP
        services.AddScoped<EnventaTradeEerIntegration>();

        // HttpClient für ERP API Calls
        services.AddHttpClient<EnventaTradeEerIntegration>()
            .ConfigureHttpClient((provider, client) =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

        return services;
    }
}
