using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using B2Connect.LocalizationService.Data;

namespace B2Connect.LocalizationService.Data;

/// <summary>
/// Factory for creating LocalizationDbContext instances at design time (migrations, etc.)
/// </summary>
public class LocalizationDbContextFactory : IDesignTimeDbContextFactory<LocalizationDbContext>
{
    /// <summary>
    /// Creates a new instance of LocalizationDbContext for design-time operations
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>A configured LocalizationDbContext instance</returns>
    public LocalizationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LocalizationDbContext>();

        // Use environment variable or default development connection string
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__LocalizationDb")
            ?? "Host=localhost;Database=b2connect_localization;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString);

        return new LocalizationDbContext(optionsBuilder.Options);
    }
}