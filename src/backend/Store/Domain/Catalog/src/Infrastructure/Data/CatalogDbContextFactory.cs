using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace B2X.Catalog.Infrastructure.Data;

/// <summary>
/// Design-time factory for CatalogDbContext.
/// Used by EF Core tools for migrations.
/// </summary>
public class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CatalogDbContext>();

        // Use a default connection string for design-time operations
        // This is only used for generating migrations, not for runtime
        optionsBuilder.UseNpgsql(
            "Host=localhost;Database=B2X_catalog;Username=postgres;Password=postgres",
            npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "catalog"));

        return new CatalogDbContext(optionsBuilder.Options);
    }
}
