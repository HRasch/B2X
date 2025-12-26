using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace B2Connect.AuthService.Data;

// Identity User & Role
public class AppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? TenantId { get; set; }
    public bool IsTwoFactorRequired { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AppRole : IdentityRole
{
    public string? Description { get; set; }
}

// Database Context
public class AuthDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Seed Admin Role
        var adminRole = new AppRole
        {
            Id = "admin-role",
            Name = "Admin",
            NormalizedName = "ADMIN",
            Description = "Administrator role with full access"
        };

        builder.Entity<AppRole>().HasData(adminRole);

        // Seed Admin User
        var hasher = new PasswordHasher<AppUser>();
        var adminUser = new AppUser
        {
            Id = "admin-001",
            Email = "admin@example.com",
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            UserName = "admin@example.com",
            NormalizedUserName = "ADMIN@EXAMPLE.COM",
            FirstName = "Admin",
            LastName = "User",
            TenantId = "default",
            IsActive = true,
            EmailConfirmed = true,
            IsTwoFactorRequired = false,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        adminUser.PasswordHash = hasher.HashPassword(adminUser, "password");
        builder.Entity<AppUser>().HasData(adminUser);

        // Assign Admin Role to Admin User
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            }
        );
    }
}

// Services
public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
    Task<AppUser?> GetUserByIdAsync(string userId);
    Task<AuthResponse> EnableTwoFactorAsync(string userId);
    Task<bool> VerifyTwoFactorCodeAsync(string userId, string code);
}

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(UserManager<AppUser> userManager, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("User account is inactive");
        }

        // Check if 2FA is required
        if (user.IsTwoFactorRequired && !request.Skip2FA)
        {
            return new AuthResponse
            {
                Requires2FA = true,
                TempUserId = user.Id,
                AccessToken = string.Empty,
                RefreshToken = string.Empty,
                ExpiresIn = 0
            };
        }

        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        _logger.LogInformation("User {Email} logged in successfully", user.Email);

        return new AuthResponse
        {
            User = MapToUserInfo(user),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        // For demo: just generate a new token
        // In production: validate the refresh token against stored tokens
        var claimsPrincipal = ValidateExpiredToken(refreshToken);

        if (claimsPrincipal == null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.FindByIdAsync(userId!);

        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("User not found or inactive");
        }

        var newAccessToken = GenerateAccessToken(user);
        var newRefreshToken = GenerateRefreshToken();

        return new AuthResponse
        {
            User = MapToUserInfo(user),
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = 3600
        };
    }

    public async Task<AppUser?> GetUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<AuthResponse> EnableTwoFactorAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        user.IsTwoFactorRequired = true;
        await _userManager.UpdateAsync(user);

        return new AuthResponse
        {
            User = MapToUserInfo(user),
            AccessToken = string.Empty,
            RefreshToken = string.Empty,
            ExpiresIn = 0,
            TwoFactorEnabled = true
        };
    }

    public async Task<bool> VerifyTwoFactorCodeAsync(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        // For demo: accept "123456"
        return code == "123456";
    }

    private string GenerateAccessToken(AppUser user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Secret"] ?? "super-secret-key-for-development-only-change-in-production"));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new("TenantId", user.TenantId ?? "default")
        };

        // Add roles as claims
        var roles = _userManager.GetRolesAsync(user).Result;
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "B2Connect",
            audience: _configuration["Jwt:Audience"] ?? "B2Connect.Admin",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? ValidateExpiredToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(
                _configuration["Jwt:Secret"] ?? "super-secret-key-for-development-only-change-in-production");

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"] ?? "B2Connect",
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"] ?? "B2Connect.Admin",
                ValidateLifetime = false // Allow expired tokens for refresh
            }, out SecurityToken validatedToken);

            return principal;
        }
        catch
        {
            return null;
        }
    }

    private UserInfo MapToUserInfo(AppUser user)
    {
        var roles = _userManager.GetRolesAsync(user).Result.ToArray();
        return new UserInfo
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            TenantId = user.TenantId ?? "default",
            Roles = roles,
            Permissions = GetPermissionsForRoles(roles)
        };
    }

    private string[] GetPermissionsForRoles(string[] roles)
    {
        if (roles.Contains("Admin"))
        {
            return new[] { "*" }; // All permissions
        }

        return Array.Empty<string>();
    }
}

// DTOs
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
    public bool Skip2FA { get; set; }
}

public class AuthResponse
{
    public UserInfo? User { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public bool Requires2FA { get; set; }
    public string? TempUserId { get; set; }
    public bool TwoFactorEnabled { get; set; }
}

public class UserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string[] Roles { get; set; } = Array.Empty<string>();
    public string[] Permissions { get; set; } = Array.Empty<string>();
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class VerifyTwoFactorRequest
{
    public string Code { get; set; } = string.Empty;
}
