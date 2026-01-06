using B2Connect.CMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace B2Connect.CMS.Infrastructure.Context;

public class CmsDbContextFactory : IDesignTimeDbContextFactory<CmsDbContext>
{
    public CmsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CmsDbContext>();

        // Use a default connection string for migrations
        // In production, this will be configured via dependency injection
        optionsBuilder.UseNpgsql("Host=localhost;Database=b2connect_cms;Username=postgres;Password=password",
            npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history", "cms");
            });

        return new CmsDbContext(optionsBuilder.Options);
    }
}
