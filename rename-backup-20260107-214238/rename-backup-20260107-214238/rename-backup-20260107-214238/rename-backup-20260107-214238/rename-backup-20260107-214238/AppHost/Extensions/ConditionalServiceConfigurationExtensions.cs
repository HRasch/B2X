using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using B2Connect.AppHost.Configuration;
using B2Connect.AppHost.Services;

namespace B2Connect.AppHost.Extensions;

/// <summary>
/// Extensions for conditional service configuration based on testing mode.
/// Services use these methods to register DbContext and infrastructure conditionally.
/// </summary>
public static class ConditionalServiceConfigurationExtensions
{
    /// <summary>
    /// Configures conditional PostgreSQL connection based on Testing:Mode configuration.
    /// 
    /// Usage in service project Program.cs:
    ///   var testingConfig = builder.Configuration.Get&lt;TestingConfiguration&gt;() ?? new();
    ///   if (testingConfig.IsPersistenTestMode())
    ///   {
    ///       services.AddDbContext&lt;MyDbContext&gt;(options =>
    ///           options.UseNpgsql(connectionString));
    ///   }
    ///   else
    ///   {
    ///       services.AddDbContext&lt;MyDbContext&gt;(options =>
    ///           options.UseInMemoryDatabase("MyServiceDb"));
    ///   }
    /// </summary>
    public static IServiceCollection ConfigureConditionalDbContext<TDbContext>(
        this IServiceCollection services,
        TestingConfiguration testingConfig,
        string connectionString,
        Action<IServiceProvider, DbContextOptionsBuilder> configurePostgres,
        Action<IServiceProvider, DbContextOptionsBuilder> configureInMemory)
        where TDbContext : DbContext
    {
        if (testingConfig.IsPersistenTestMode())
        {
            // Use PostgreSQL for persistent test environment
            services.AddDbContext<TDbContext>((sp, options) =>
            {
                configurePostgres(sp, options);
            });
        }
        else
        {
            // Use in-memory database for temporary test environment
            services.AddDbContext<TDbContext>((sp, options) =>
            {
                configureInMemory(sp, options);
            });
        }

        return services;
    }

    /// <summary>
    /// Configures conditional Redis connection based on Testing:Mode configuration.
    /// </summary>
    public static IServiceCollection ConfigureConditionalRedis(
        this IServiceCollection services,
        TestingConfiguration testingConfig,
        string redisConnectionString,
        Action<IDistributedCache> configureRedisCache,
        Action<IDistributedCache> configureMemoryCache)
    {
        if (testingConfig.IsPersistenTestMode())
        {
            // Use Redis for persistent test environment
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
            });

            services.AddSingleton<IDistributedCache>(sp =>
            {
                var cache = sp.GetRequiredService<IDistributedCache>();
                configureRedisCache(cache);
                return cache;
            });
        }
        else
        {
            // Use in-memory cache for temporary test environment
            services.AddDistributedMemoryCache();

            services.AddSingleton<IDistributedCache>(sp =>
            {
                var cache = sp.GetRequiredService<IDistributedCache>();
                configureMemoryCache(cache);
                return cache;
            });
        }

        return services;
    }

    /// <summary>
    /// Marks a service as configured for the specified testing mode.
    /// Useful for debugging and health check reporting.
    /// </summary>
    public static void LogTestingConfiguration(
        this ILogger logger,
        string serviceName,
        TestingConfiguration testingConfig)
    {
        logger.LogInformation(
            "Service '{ServiceName}' configured for {Mode} testing in {Environment} environment. " +
            "Seed on startup: {SeedOnStartup}, Default tenants: {DefaultTenants}",
            serviceName,
            testingConfig.Mode,
            testingConfig.Environment,
            testingConfig.SeedOnStartup,
            testingConfig.SeedData.DefaultTenantCount);
    }

    /// <summary>
    /// Registers the TestDataOrchestrator with HTTP clients for all services.
    /// Only registers when testing configuration requires seeding.
    /// </summary>
    public static IServiceCollection AddTestDataOrchestrator(
        this IServiceCollection services,
        TestingConfiguration testingConfig)
    {
        if (!testingConfig.ShouldSeedTestData())
        {
            return services; // Skip registration if no seeding needed
        }

        // Register HTTP clients for service communication
        services.AddHttpClient("auth-service", client =>
        {
            client.BaseAddress = new Uri("http://auth-service"); // Aspire service discovery
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        services.AddHttpClient("tenant-service", client =>
        {
            client.BaseAddress = new Uri("http://tenant-service"); // Aspire service discovery
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        services.AddHttpClient("localization-service", client =>
        {
            client.BaseAddress = new Uri("http://localization-service"); // Aspire service discovery
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        services.AddHttpClient("catalog-service", client =>
        {
            client.BaseAddress = new Uri("http://catalog-service"); // Aspire service discovery
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        services.AddHttpClient("cms-service", client =>
        {
            client.BaseAddress = new Uri("http://cms-service"); // Aspire service discovery
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        // Register the orchestrator
        services.AddTransient<ITestDataOrchestrator, TestDataOrchestrator>();

        // Register status tracker
        services.AddSingleton<SeedingStatusTracker>();

        return services;
    }
}
