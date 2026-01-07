using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace B2X.LayoutService.Data;

/// <summary>
/// Extension methods for configuring Database and Entity Framework Core
/// Uses InMemory database for development
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Configures the Layout Service database (InMemory for development)
    /// </summary>
    public static IServiceCollection AddLayoutDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Use InMemory for development/testing
        services.AddDbContext<LayoutDbContext>(options =>
            options.UseInMemoryDatabase("LayoutService")
                .EnableSensitiveDataLogging(true));

        return services;
    }

    /// <summary>
    /// Ensures the database is created
    /// </summary>
    public static async Task EnsureDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<LayoutDbContext>();

            try
            {
                // For InMemory, just ensure created
                await context.Database.EnsureCreatedAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Log the error but don't fail startup
                Console.WriteLine($"Database creation failed: {ex.Message}");
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
