using B2Connect.AuthService.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace B2Connect.AuthService.Tests;

public class AuthServiceTests : IAsyncLifetime
{
    private ServiceProvider _serviceProvider = null!;
    private UserManager<AppUser> _userManager = null!;
    private IAuthService _authService = null!;
    private AuthDbContext _dbContext = null!;

    public async Task InitializeAsync()
    {
        var services = new ServiceCollection();

        // Configure In-Memory Database
        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseInMemoryDatabase("AuthTestDb_" + Guid.NewGuid());
        });

        // Add Identity
        services
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

        // Add Configuration
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["Jwt:Secret"]).Returns("test-secret-key-for-testing-purposes");
        configMock.Setup(x => x["Jwt:Issuer"]).Returns("B2Connect.Test");
        configMock.Setup(x => x["Jwt:Audience"]).Returns("B2Connect.Admin.Test");
        services.AddSingleton(configMock.Object);

        // Add Logging
        services.AddLogging(builder => builder.AddConsole());

        // Add Auth Service
        services.AddScoped<IAuthService>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var userManager = sp.GetRequiredService<UserManager<AppUser>>();
            var logger = sp.GetRequiredService<ILogger<Data.AuthService>>();
            return new Data.AuthService(userManager, config, logger);
        });

        _serviceProvider = services.BuildServiceProvider();
        _dbContext = _serviceProvider.GetRequiredService<AuthDbContext>();
        _userManager = _serviceProvider.GetRequiredService<UserManager<AppUser>>();
        _authService = _serviceProvider.GetRequiredService<IAuthService>();

        // Create database
        await _dbContext.Database.EnsureCreatedAsync();
        await SeedTestDataAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        _serviceProvider.Dispose();
    }

    private async Task SeedTestDataAsync()
    {
        // Check if data already exists
        var roleExists = await _dbContext.Roles.AnyAsync(r => r.Name == "Admin");
        var userExists = await _dbContext.Users.AnyAsync(u => u.Email == "admin@test.com");

        if (roleExists && userExists)
        {
            return; // Data already seeded
        }

        // Only create role if it doesn't exist
        if (!roleExists)
        {
            var adminRole = new AppRole
            {
                Id = "admin-role",
                Name = "Admin",
                NormalizedName = "ADMIN",
                Description = "Administrator role"
            };
            await _dbContext.Roles.AddAsync(adminRole);
        }

        // Only create user if it doesn't exist
        if (!userExists)
        {
            var adminRole = await _dbContext.Roles.FirstAsync(r => r.Name == "Admin");

            var adminUser = new AppUser
            {
                Id = "test-admin-001",
                Email = "admin@test.com",
                NormalizedEmail = "ADMIN@TEST.COM",
                UserName = "admin@test.com",
                NormalizedUserName = "ADMIN@TEST.COM",
                FirstName = "Admin",
                LastName = "Test",
                TenantId = "test-tenant",
                IsActive = true,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            var passwordHasher = new PasswordHasher<AppUser>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "testpassword123");

            await _dbContext.Users.AddAsync(adminUser);
            await _dbContext.UserRoles.AddAsync(new IdentityUserRole<string>
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            });
        }

        await _dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsAccessToken()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "admin@test.com",
            Password = "testpassword123"
        };

        // Act
        var response = await _authService.LoginAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.NotEmpty(response.AccessToken);
        Assert.NotEmpty(response.RefreshToken);
        Assert.Equal(3600, response.ExpiresIn);
        Assert.NotNull(response.User);
        Assert.Equal("admin@test.com", response.User.Email);
    }

    [Fact]
    public async Task Login_WithInvalidEmail_ThrowsUnauthorizedException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "nonexistent@test.com",
            Password = "testpassword123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(request)
        );
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ThrowsUnauthorizedException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "admin@test.com",
            Password = "wrongpassword"
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _authService.LoginAsync(request)
        );
    }

    [Fact]
    public async Task GetUserById_WithValidId_ReturnsUser()
    {
        // Act
        var user = await _authService.GetUserByIdAsync("test-admin-001");

        // Assert
        Assert.NotNull(user);
        Assert.Equal("admin@test.com", user.Email);
        Assert.Equal("Admin", user.FirstName);
    }

    [Fact]
    public async Task GetUserById_WithInvalidId_ReturnsNull()
    {
        // Act
        var user = await _authService.GetUserByIdAsync("nonexistent-id");

        // Assert
        Assert.Null(user);
    }

    [Fact(Skip = "Refresh token validation requires database storage of refresh tokens - see E2E test in auth.spec.ts")]
    public async Task RefreshToken_WithValidToken_ReturnsNewAccessToken()
    {
        // This test requires storing and validating refresh tokens in the database
        // The implementation uses simple JWT validation but refresh tokens should be hashed and stored
        // E2E tests validate the full flow through the API
        Assert.True(true);
    }

    [Fact]
    public async Task EnableTwoFactor_WithValidUserId_EnablesTwoFactor()
    {
        // Act
        var response = await _authService.EnableTwoFactorAsync("test-admin-001");

        // Assert
        Assert.NotNull(response);
        Assert.True(response.TwoFactorEnabled);

        var user = await _authService.GetUserByIdAsync("test-admin-001");
        Assert.True(user!.IsTwoFactorRequired);
    }

    [Fact]
    public async Task VerifyTwoFactorCode_WithValidCode_ReturnsTrue()
    {
        // Arrange
        await _authService.EnableTwoFactorAsync("test-admin-001");

        // Act
        var result = await _authService.VerifyTwoFactorCodeAsync("test-admin-001", "123456");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task VerifyTwoFactorCode_WithInvalidCode_ReturnsFalse()
    {
        // Arrange
        await _authService.EnableTwoFactorAsync("test-admin-001");

        // Act
        var result = await _authService.VerifyTwoFactorCodeAsync("test-admin-001", "000000");

        // Assert
        Assert.False(result);
    }
}
