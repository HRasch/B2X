using B2X.SmartDataIntegration.Core;
using B2X.SmartDataIntegration.Infrastructure.Data;
using B2X.SmartDataIntegration.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace B2X.SmartDataIntegration;

/// <summary>
/// Extension methods for registering Smart Data Integration services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Smart Data Integration services to the dependency injection container
    /// </summary>
    public static IServiceCollection AddSmartDataIntegration(
        this IServiceCollection services,
        string connectionString)
    {
        // Register DbContext
        services.AddDbContext<SmartDataIntegrationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Register core services
        services.AddScoped<ISmartDataIntegrationService, SmartDataIntegrationService>();
        services.AddScoped<IMappingRepository, SmartDataIntegrationRepository>();
        services.AddScoped<IMappingEngine, BasicMappingEngine>();
        services.AddScoped<IMappingValidator, BasicMappingValidator>();

        return services;
    }

    /// <summary>
    /// Adds Smart Data Integration services with custom DbContext configuration
    /// </summary>
    public static IServiceCollection AddSmartDataIntegration(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> dbContextOptions)
    {
        // Register DbContext with custom options
        services.AddDbContext<SmartDataIntegrationDbContext>(dbContextOptions);

        // Register core services
        services.AddScoped<ISmartDataIntegrationService, SmartDataIntegrationService>();
        services.AddScoped<IMappingRepository, SmartDataIntegrationRepository>();
        services.AddScoped<IMappingEngine, BasicMappingEngine>();
        services.AddScoped<IMappingValidator, BasicMappingValidator>();

        return services;
    }
}
