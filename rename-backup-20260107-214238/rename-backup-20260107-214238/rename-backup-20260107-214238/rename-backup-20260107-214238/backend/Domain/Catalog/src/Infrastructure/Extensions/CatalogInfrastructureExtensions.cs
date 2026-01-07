using B2Connect.Catalog.Core.Services;
using B2Connect.Catalog.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace B2Connect.Catalog.Infrastructure.Extensions
{
    public static class CatalogInfrastructureExtensions
    {
        public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services)
        {
            // Register a minimal rule manager. Replace with DB-backed implementation later.
            services.AddSingleton<ICatalogShareRuleManager, InMemoryCatalogShareRuleManager>();

            return services;
        }
    }
}
