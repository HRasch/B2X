using B2X.Catalog.Core.Interfaces;
using B2X.Catalog.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace B2X.Catalog.Infrastructure;

/// <summary>
/// Service collection extensions for catalog infrastructure
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds catalog infrastructure services to the service collection
    /// </summary>
    public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services)
    {
        // Register repository
        services.AddScoped<ICatalogRepository, CatalogRepository>();

        // Add any other infrastructure services here
        // services.AddScoped<ICatalogCache, CatalogCache>();
        // services.AddScoped<ICatalogSearch, CatalogSearch>();

        return services;
    }
}
