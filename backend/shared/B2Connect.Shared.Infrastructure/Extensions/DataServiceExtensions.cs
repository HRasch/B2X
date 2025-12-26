namespace B2Connect.Shared.Data.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// DbContext Extension Methods - nur für generische Patterns
/// Validator-Registrierung gehört in service-spezifische Program.cs
/// </summary>
public static class DataServiceExtensions
{
    /// <summary>
    /// Konfiguriert DbContext mit PostgreSQL
    /// </summary>
    public static IServiceCollection AddPostgresContext<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionName = "postgres")
        where TContext : DbContext
    {
        var connectionString = configuration.GetConnectionString(connectionName)
            ?? "Host=localhost;Database=b2connect;Username=postgres;Password=postgres";

        services.AddDbContext<TContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        return services;
    }

    /// <summary>
    /// Konfiguriert DbContext mit SQL Server
    /// </summary>
    public static IServiceCollection AddSqlServerContext<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionName = "sqlserver")
        where TContext : DbContext
    {
        var connectionString = configuration.GetConnectionString(connectionName);

        services.AddDbContext<TContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        return services;
    }

    /// <summary>
    /// Konfiguriert DbContext mit In-Memory für Tests
    /// </summary>
    public static IServiceCollection AddInMemoryContext<TContext>(
        this IServiceCollection services,
        string databaseName = "InMemory")
        where TContext : DbContext
    {
        services.AddDbContext<TContext>(options =>
        {
            options.UseInMemoryDatabase(databaseName);
        });

        return services;
    }
}
