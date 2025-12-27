using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using B2Connect.Shared.User.Interfaces;
using B2Connect.Shared.User.Infrastructure.Data;
using B2Connect.Shared.User.Infrastructure.Repositories;

namespace B2Connect.Shared.User.Infrastructure;

/// <summary>
/// Dependency injection extensions for User domain infrastructure
/// </summary>
public static class UserInfrastructureExtensions
{
    /// <summary>
    /// Register User domain repositories and DbContext
    /// </summary>
    public static IServiceCollection AddUserInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        var connectionString = configuration.GetConnectionString("UserDb")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No connection string configured for UserDb");

        services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(connectionString,
                npgsqlOptions => npgsqlOptions
                    .MigrationsHistoryTable("__ef_migrations_history", "user")
                    .CommandTimeout(30))
            .UseSnakeCaseNamingConvention()
            .EnableSensitiveDataLogging()); // Only in development!

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();

        return services;
    }

    /// <summary>
    /// Initialize database schema (run migrations)
    /// </summary>
    public static async Task EnsureUserDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<UserDbContext>>();
            logger.LogError(ex, "Failed to migrate User database");
            throw;
        }
    }
}
