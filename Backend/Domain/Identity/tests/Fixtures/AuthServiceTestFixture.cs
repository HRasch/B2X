using B2Connect.AuthService.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace B2Connect.Identity.Tests.Fixtures;

/// <summary>
/// Base fixture for AuthService tests with in-memory database and Identity setup
/// </summary>
public class AuthServiceTestFixture : IAsyncLifetime
{
    private readonly ServiceCollection _services = new();
    public ServiceProvider ServiceProvider { get; private set; } = null!;
    public AuthDbContext DbContext { get; private set; } = null!;
    public UserManager<AppUser> UserManager { get; private set; } = null!;
    public IAuthService AuthService { get; private set; } = null!;
    public IConfiguration Configuration { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        // Configure in-memory database
        _services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseInMemoryDatabase("AuthTestDb_" + Guid.NewGuid());
        });

        // Configure Identity
        _services
            .AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

        // Mock configuration for JWT
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["Jwt:Secret"]).Returns("test-secret-key-minimum-32-characters-required!!!!");
        configMock.Setup(x => x["Jwt:Issuer"]).Returns("B2Connect.Test");
        configMock.Setup(x => x["Jwt:Audience"]).Returns("B2Connect.Admin.Test");
        _services.AddSingleton(configMock.Object);

        // Add logging
        _services.AddLogging(builder => builder.AddConsole());

        // Register AuthService
        _services.AddScoped<IAuthService>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var userManager = sp.GetRequiredService<UserManager<AppUser>>();
            var logger = sp.GetRequiredService<ILogger<B2Connect.AuthService.Data.AuthService>>();
            return new B2Connect.AuthService.Data.AuthService(userManager, config, logger);
        });

        // Build service provider
        ServiceProvider = _services.BuildServiceProvider();
        DbContext = ServiceProvider.GetRequiredService<AuthDbContext>();
        UserManager = ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        AuthService = ServiceProvider.GetRequiredService<IAuthService>();
        Configuration = ServiceProvider.GetRequiredService<IConfiguration>();

        // Create database
        await DbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
        ServiceProvider.Dispose();
    }

    /// <summary>
    /// Creates a test user with email and password
    /// </summary>
    public async Task<AppUser> CreateTestUserAsync(
        string email = "test@example.com",
        string password = "TestPassword123",
        string firstName = "Test",
        string lastName = "User")
    {
        var user = new AppUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            TenantId = Guid.NewGuid().ToString(),
            IsActive = true
        };

        var result = await UserManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to create test user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        return user;
    }

    /// <summary>
    /// Creates a test admin user
    /// </summary>
    public async Task<AppUser> CreateAdminUserAsync(string email = "admin@example.com")
    {
        var user = await CreateTestUserAsync(email, "AdminPassword123");
        var role = new AppRole { Name = "Admin", Description = "Administrator role" };

        if (!await DbContext.Roles.AnyAsync(r => r.Name == "Admin"))
        {
            await DbContext.Roles.AddAsync(role);
            await DbContext.SaveChangesAsync();
        }

        await UserManager.AddToRoleAsync(user, "Admin");
        return user;
    }
}

/// <summary>
/// Base test class for AuthService tests
/// </summary>
public abstract class AuthServiceTestBase : IAsyncLifetime
{
    protected AuthServiceTestFixture Fixture { get; private set; } = null!;

    protected async Task InitializeFixtureAsync()
    {
        Fixture = new AuthServiceTestFixture();
        await Fixture.InitializeAsync();
    }

    protected async Task DisposeFixtureAsync()
    {
        await Fixture.DisposeAsync();
    }

    public async Task InitializeAsync() => await InitializeFixtureAsync();
    public async Task DisposeAsync() => await DisposeFixtureAsync();
}
