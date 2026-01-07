using B2X.CMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace B2X.CMS.Infrastructure.Context;

public class CmsDbContextFactory : IDesignTimeDbContextFactory<CmsDbContext>
{
    public CmsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CmsDbContext>();

        // Use a default connection string for migrations
        // In production, this will be configured via dependency injection
        optionsBuilder.UseNpgsql("Host=localhost;Database=B2X_cms;Username=postgres;Password=password",
            npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history", "cms");
            });

        return new CmsDbContext(optionsBuilder.Options);
    }
}
