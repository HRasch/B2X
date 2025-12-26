using B2Connect.CatalogService.Services;
using B2Connect.CatalogService.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace B2Connect.CatalogService.Extensions;

/// <summary>
/// Extension methods for PIM Sync Service registration
/// </summary>
public static class PimSyncExtensions
{
    /// <summary>
    /// Register PIM Sync Service and Worker
    /// </summary>
    public static IServiceCollection AddPimSync(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register sync service
        services.AddScoped<IPimSyncService, PimSyncService>();

        // Register background worker if enabled
        var isSyncEnabled = configuration.GetValue("PimSync:Enabled", true);
        if (isSyncEnabled)
        {
            services.AddHostedService<PimSyncWorker>();
        }

        return services;
    }
}
