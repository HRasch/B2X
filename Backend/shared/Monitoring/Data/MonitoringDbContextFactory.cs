// -----------------------------------------------------------------------------
// MonitoringDbContextFactory.cs
// Design-time factory for EF Core migrations
// -----------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace B2Connect.Shared.Monitoring.Data;

/// <summary>
/// Design-time factory for creating MonitoringDbContext instances during EF Core migrations.
/// This allows migrations to be generated without requiring the AppHost as startup project.
/// </summary>
/// <remarks>
/// Usage:
/// <code>
/// dotnet ef migrations add MigrationName --project backend/shared/Monitoring/B2Connect.Shared.Monitoring.csproj
/// dotnet ef database update --project backend/shared/Monitoring/B2Connect.Shared.Monitoring.csproj
/// </code>
/// </remarks>
public class MonitoringDbContextFactory : IDesignTimeDbContextFactory<MonitoringDbContext>
{
    /// <summary>
    /// Creates a new instance of MonitoringDbContext for design-time operations.
    /// </summary>
    /// <param name="args">Command line arguments (not used).</param>
    /// <returns>A configured MonitoringDbContext instance.</returns>
    public MonitoringDbContext CreateDbContext(string[] args)
    {
        // Build configuration from appsettings.json if available
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<MonitoringDbContext>();

        // Try to get connection string from configuration, fallback to default for migrations
        var connectionString = configuration.GetConnectionString("MonitoringDb")
            ?? "Host=localhost;Port=5432;Database=B2Connect_Monitoring;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "monitoring");
        });

        // Create a stub tenant context for design-time (returns empty tenant)
        return new MonitoringDbContext(optionsBuilder.Options, new DesignTimeTenantContext());
    }

    /// <summary>
    /// Stub tenant context for design-time operations.
    /// Returns empty GUIDs since tenant filtering is not needed during migrations.
    /// </summary>
    private sealed class DesignTimeTenantContext : Infrastructure.Context.ITenantContext
    {
        public Guid TenantId => Guid.Empty;
        public string TenantName => "DesignTime";
    }
}
