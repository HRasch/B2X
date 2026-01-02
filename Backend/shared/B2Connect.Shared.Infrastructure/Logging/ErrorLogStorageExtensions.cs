using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace B2Connect.Shared.Infrastructure.Logging;

/// <summary>
/// Extension methods for registering error log storage services.
/// </summary>
public static class ErrorLogStorageExtensions
{
    /// <summary>
    /// Adds PostgreSQL-based error log storage to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">PostgreSQL connection string.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddPostgreSqlErrorLogStorage(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<ErrorLogDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure(3)));

        services.AddScoped<IErrorLogStorage, PostgreSqlErrorLogStorage>();

        return services;
    }

    /// <summary>
    /// Adds PostgreSQL-based error log storage with a custom configuration action.
    /// </summary>
    public static IServiceCollection AddPostgreSqlErrorLogStorage(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction)
    {
        services.AddDbContext<ErrorLogDbContext>(optionsAction);
        services.AddScoped<IErrorLogStorage, PostgreSqlErrorLogStorage>();

        return services;
    }

    /// <summary>
    /// Adds in-memory error log storage for testing purposes.
    /// </summary>
    public static IServiceCollection AddInMemoryErrorLogStorage(this IServiceCollection services)
    {
        services.AddDbContext<ErrorLogDbContext>(options =>
            options.UseInMemoryDatabase("ErrorLogs"));

        services.AddScoped<IErrorLogStorage, PostgreSqlErrorLogStorage>();

        return services;
    }

    /// <summary>
    /// Ensures the error log database schema is created.
    /// Call this during application startup.
    /// </summary>
    public static async Task EnsureErrorLogSchemaAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ErrorLogDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
}
