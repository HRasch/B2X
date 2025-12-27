using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace B2Connect.Shared.User.Infrastructure.Data;

/// <summary>
/// EF Core Design-time factory for UserDbContext (used by dotnet-ef CLI)
/// </summary>
public class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();

        // Use in-memory database for migrations/scaffolding
        // Or use environment variable for connection string
        var connectionString = Environment.GetEnvironmentVariable("USER_DB_CONNECTION_STRING")
            ?? "Server=localhost;Port=5432;Database=b2connect_user;User Id=postgres;Password=postgres;";

        optionsBuilder.UseNpgsql(connectionString);

        return new UserDbContext(optionsBuilder.Options);
    }
}
