using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace B2Connect.LayoutService.Data;

/// <summary>
/// Extension methods for configuring Database and Entity Framework Core
/// Supports PostgreSQL, SQL Server Express, and InMemory databases
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Configures the Layout Service database based on configuration
    /// </summary>
    public static IServiceCollection AddLayoutDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseConfig = configuration.GetSection("DatabaseConfig");
        var provider = databaseConfig.GetValue<string>("Provider") ?? "PostgreSQL";
        var useInMemory = databaseConfig.GetValue<bool>("UseInMemory");

        // For tests, always use InMemory unless explicitly configured
        if (useInMemory || (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test"))
        {
            services.AddDbContext<LayoutDbContext>(options =>
                options.UseInMemoryDatabase("LayoutServiceTest")
                    .EnableSensitiveDataLogging(true));

            return services;
        }

        var connectionStrings = databaseConfig.GetSection("ConnectionStrings");

        // Configure database provider
        services.AddDbContext<LayoutDbContext>(options =>
        {
            switch (provider.ToLowerInvariant())
            {
                case "sqlserver":
                case "sql server":
                    var sqlServerConnString = connectionStrings.GetValue<string>("SqlServer")
                        ?? throw new InvalidOperationException("SQL Server connection string not configured");
                    options.UseSqlServer(sqlServerConnString, sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly("B2Connect.LayoutService");
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelaySeconds: 5,
                            errorNumbersToAdd: null);
                    });
                    break;

                case "postgresql":
                case "postgres":
                default:
                    var pgConnString = connectionStrings.GetValue<string>("PostgreSQL")
                        ?? throw new InvalidOperationException("PostgreSQL connection string not configured");
                    options.UseNpgsql(pgConnString, pgOptions =>
                    {
                        pgOptions.MigrationsAssembly("B2Connect.LayoutService");
                        pgOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelaySeconds: 5,
                            errorNumbersToAdd: null);
                    });
                    break;
            }

            // Enable sensitive data logging for development
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging(true);
            }
        });

        // Add Health Checks
        AddHealthChecks(services, configuration, provider);

        return services;
    }

    /// <summary>
    /// Adds health checks for the configured database provider
    /// </summary>
    private static void AddHealthChecks(
        IServiceCollection services,
        IConfiguration configuration,
        string provider)
    {
        var healthChecks = services.AddHealthChecks();

        var connectionStrings = configuration.GetSection("DatabaseConfig:ConnectionStrings");

        switch (provider.ToLowerInvariant())
        {
            case "sqlserver":
            case "sql server":
                var sqlConnString = connectionStrings.GetValue<string>("SqlServer");
                if (!string.IsNullOrEmpty(sqlConnString))
                {
                    healthChecks.AddSqlServer(sqlConnString, name: "sql-server-check");
                }
                break;

            case "postgresql":
            case "postgres":
            default:
                var pgConnString = connectionStrings.GetValue<string>("PostgreSQL");
                if (!string.IsNullOrEmpty(pgConnString))
                {
                    healthChecks.AddNpgSql(pgConnString, name: "postgresql-check");
                }
                break;
        }
    }

    /// <summary>
    /// Ensures the database is created and applies pending migrations
    /// </summary>
    public static async Task EnsureDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LayoutDbContext>();

            try
            {
                // Check if using InMemory database
                if (context.Database.IsInMemory())
                {
                    // For InMemory, just ensure created
                    await context.Database.EnsureCreatedAsync();
                }
                else
                {
                    // For real databases, apply migrations
                    await context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't fail startup for database issues
                Console.WriteLine($"Database migration/creation failed: {ex.Message}");
                throw;
            }
        }
    }

    /// <summary>
    /// Gets the current database provider name for logging/debugging
    /// </summary>
    public static string GetDatabaseProviderName(this LayoutDbContext context)
    {
        return context.Database.ProviderName switch
        {
            "Microsoft.EntityFrameworkCore.InMemory" => "InMemory",
            "Microsoft.EntityFrameworkCore.Npgsql" => "PostgreSQL",
            "Microsoft.EntityFrameworkCore.SqlServer" => "SQL Server",
            _ => "Unknown"
        };
    }
}
