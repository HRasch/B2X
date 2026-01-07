using B2Connect.Email.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace B2Connect.Email;

/// <summary>
/// Design-time factory for EmailDbContext to enable EF Core migrations
/// </summary>
public class EmailDbContextFactory : IDesignTimeDbContextFactory<EmailDbContext>
{
    public EmailDbContext CreateDbContext(string[] args)
    {
        // Simple design-time configuration for migrations
        var optionsBuilder = new DbContextOptionsBuilder<EmailDbContext>();

        // Use PostgreSQL with a default connection string for migrations
        optionsBuilder.UseNpgsql("Host=localhost;Database=b2connect_email;Username=postgres;Password=password");

        return new EmailDbContext(optionsBuilder.Options);
    }
}
