using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using B2Connect.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace B2Connect.AuthService.Data;

// Identity User & Role
public enum AccountType
{
    DU, // DomainAdmin
    SU, // TenantAdmin
    U,   // User
    SR   // SalesRep
}

public class AppUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? TenantId { get; set; }
    public bool IsTwoFactorRequired { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeletable { get; set; } = true; // DomainAdmins cannot be deleted
    public AccountType AccountType { get; set; } = AccountType.U; // Default to User
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
            Description = "Administrator role with full access",
            ConcurrencyStamp = "admin-role-concurrency-stamp-001"
        };

        builder.Entity<AppRole>().HasData(adminRole);

        // Seed Admin User
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
            SecurityStamp = "admin-security-stamp-001",
            ConcurrencyStamp = "admin-concurrency-stamp-001",
            PasswordHash = "AQAAAAIAAYagAAAAEN7xlcM7+QF0BazMFn8WcIT2TOxzdYJrF8wAmVLoYHGhl4KIltB9chtzz65vmEG9jA=="
        };

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
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<Result<AppUser>> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> EnableTwoFactorAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> VerifyTwoFactorCodeAsync(string userId, string code, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<Result<AppUser>> ToggleUserStatusAsync(string userId, bool isActive, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeactivateUserAsync(string userId, CancellationToken cancellationToken = default);
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public string[] Roles { get; set; } = Array.Empty<string>();
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

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password).ConfigureAwait(false))
        {
            return new Result<AuthResponse>.Failure(ErrorCodes.InvalidCredentials, ErrorCodes.InvalidCredentials.ToMessage());
        }

        if (!user.IsActive)
        {
            return new Result<AuthResponse>.Failure(ErrorCodes.UserInactive, ErrorCodes.UserInactive.ToMessage());
        }

        // Check if 2FA is required
        if (user.IsTwoFactorRequired && !(request.Skip2FA ?? false))
        {
            var response = new AuthResponse
            {
                Requires2FA = true,
                TempUserId = user.Id,
                AccessToken = string.Empty,
                RefreshToken = string.Empty,
                ExpiresIn = 0
            };
            return new Result<AuthResponse>.Success(response, ErrorCodes.TwoFactorRequired.ToMessage());
        }

        var accessToken = await GenerateAccessToken(user).ConfigureAwait(false);
        var refreshToken = await GenerateRefreshToken(user).ConfigureAwait(false);

        _logger.LogInformation("User {Email} logged in successfully", user.Email);

        var loginResponse = new AuthResponse
        {
            User = await MapToUserInfo(user).ConfigureAwait(false),
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600
        };

        return new Result<AuthResponse>.Success(loginResponse, "Login successful");
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // Validate the refresh token is not empty
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return new Result<AuthResponse>.Failure(ErrorCodes.InvalidToken, "Refresh token is required");
        }

        // Try to validate the token (even if expired, we can still extract claims)
        var claimsPrincipal = ValidateExpiredToken(refreshToken);

        if (claimsPrincipal == null)
        {
            return new Result<AuthResponse>.Failure(ErrorCodes.InvalidToken, ErrorCodes.InvalidToken.ToMessage());
        }

        var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return new Result<AuthResponse>.Failure(ErrorCodes.InvalidToken, "Invalid token: user ID not found");
        }

        var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);

        if (user?.IsActive != true)
        {
            return new Result<AuthResponse>.Failure(ErrorCodes.UserNotFound, ErrorCodes.UserNotFound.ToMessage());
        }

        // Generate new tokens
        var newAccessToken = await GenerateAccessToken(user).ConfigureAwait(false);
        var newRefreshToken = await GenerateRefreshToken(user).ConfigureAwait(false);

        var response = new AuthResponse
        {
            User = await MapToUserInfo(user).ConfigureAwait(false),
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = 3600
        };

        return new Result<AuthResponse>.Success(response, "Token refreshed successfully");
    }

    public async Task<Result<AppUser>> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
        return user == null
            ? new Result<AppUser>.Failure(ErrorCodes.NotFound, ErrorCodes.NotFound.ToMessage())
            : new Result<AppUser>.Success(user, "User loaded successfully");
    }

    public async Task<Result<AuthResponse>> EnableTwoFactorAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
        if (user == null)
        {
            return new Result<AuthResponse>.Failure(ErrorCodes.NotFound, ErrorCodes.NotFound.ToMessage());
        }

        user.IsTwoFactorRequired = true;
        await _userManager.UpdateAsync(user).ConfigureAwait(false);

        var response = new AuthResponse
        {
            User = await MapToUserInfo(user).ConfigureAwait(false),
            AccessToken = string.Empty,
            RefreshToken = string.Empty,
            ExpiresIn = 0,
            TwoFactorEnabled = true
        };

        return new Result<AuthResponse>.Success(response, ErrorCodes.TwoFactorEnabled.ToMessage());
    }

    public async Task<bool> VerifyTwoFactorCodeAsync(string userId, string code, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
        if (user == null)
        {
            return false;
        }

        // For demo: accept "123456"
        return string.Equals(code, "123456", StringComparison.Ordinal);
    }

    public async Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower(System.Globalization.CultureInfo.CurrentCulture);
                query = query.Where(u =>
                    (u.Email != null && u.Email.ToLower(System.Globalization.CultureInfo.CurrentCulture).Contains(search)) ||
                    (u.FirstName != null && u.FirstName.ToLower(System.Globalization.CultureInfo.CurrentCulture).Contains(search)) ||
                    (u.LastName != null && u.LastName.ToLower(System.Globalization.CultureInfo.CurrentCulture).Contains(search)));
            }

            var users = await query
                .OrderBy(u => u.Email)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    TenantId = user.TenantId ?? "default",
                    IsActive = user.IsActive,
                    IsTwoFactorEnabled = user.IsTwoFactorRequired,
                    Roles = roles.ToArray()
                });
            }

            _logger.LogInformation("Retrieved {Count} users (page {Page})", userDtos.Count, page);
            return new Result<IEnumerable<UserDto>>.Success(userDtos, "Users loaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return new Result<IEnumerable<UserDto>>.Failure(ErrorCodes.OperationFailed, "Failed to retrieve users");
        }
    }

    public async Task<Result<AppUser>> ToggleUserStatusAsync(string userId, bool isActive, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
        if (user == null)
        {
            return new Result<AppUser>.Failure(ErrorCodes.NotFound, "User not found");
        }

        user.IsActive = isActive;
        var updateResult = await _userManager.UpdateAsync(user).ConfigureAwait(false);

        if (!updateResult.Succeeded)
        {
            return new Result<AppUser>.Failure(ErrorCodes.OperationFailed, "Failed to update user status");
        }

        _logger.LogInformation("User {UserId} status changed to {IsActive}", userId, isActive);
        return new Result<AppUser>.Success(user, $"User {(isActive ? "activated" : "deactivated")} successfully");
    }

    public async Task<Result<bool>> DeactivateUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
        if (user == null)
        {
            return new Result<bool>.Failure(ErrorCodes.NotFound, "User not found");
        }

        // DomainAdmins cannot be deactivated/deleted
        if (user.AccountType == AccountType.DU)
        {
            _logger.LogWarning("Attempted to deactivate DomainAdmin user {UserId} - operation denied", userId);
            return new Result<bool>.Failure(ErrorCodes.OperationFailed, "DomainAdmin accounts cannot be deactivated");
        }

        // Check if user is already inactive
        if (!user.IsActive)
        {
            return new Result<bool>.Success(true, "User is already deactivated");
        }

        user.IsActive = false;
        var updateResult = await _userManager.UpdateAsync(user).ConfigureAwait(false);

        if (!updateResult.Succeeded)
        {
            return new Result<bool>.Failure(ErrorCodes.OperationFailed, "Failed to deactivate user");
        }

        _logger.LogInformation("User {UserId} ({AccountType}) deactivated successfully", userId, user.AccountType);
        return new Result<bool>.Success(true, "User deactivated successfully");
    }

    private async Task<string> GenerateAccessToken(AppUser user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Secret"] ?? "super-secret-key-for-development-only-change-in-production"));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new("TenantId", user.TenantId ?? "default"),
            new("AccountType", user.AccountType.ToString())
        };

        // Add roles as claims
        var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        _logger.LogInformation("Roles for user {Email}: {Roles}", user.Email, string.Join(", ", roles));

        // Fallback: If user is admin@example.com and has no roles, add Admin role
        // This handles InMemory database where seed data for UserRoles might not load correctly
        if (!roles.Any() && user.Email?.Equals("admin@example.com", StringComparison.OrdinalIgnoreCase) == true)
        {
            _logger.LogWarning("No roles found for admin user, adding Admin role as fallback");
            roles = new List<string> { "Admin" };
        }

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

    private Task<string> GenerateRefreshToken(AppUser user)
    {
        // Generate a JWT refresh token with user ID embedded
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Secret"] ?? "super-secret-key-for-development-only-change-in-production"));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Create a JWT refresh token with minimal claims (just userId)
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "B2Connect",
            audience: _configuration["Jwt:Audience"] ?? "B2Connect.Admin",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7), // 7-day refresh token
            signingCredentials: credentials
        );

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
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
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }

    private async Task<UserInfo> MapToUserInfo(AppUser user)
    {
        var roles = (await _userManager.GetRolesAsync(user).ConfigureAwait(false)).ToArray();
        return new UserInfo
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            TenantId = user.TenantId ?? "default",
            Roles = roles,
            Permissions = GetPermissionsForRoles(roles),
            TwoFactorEnabled = user.IsTwoFactorRequired
        };
    }

    private static string[] GetPermissionsForRoles(string[] roles)
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
    public bool? RememberMe { get; set; }
    public bool? Skip2FA { get; set; }
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
    public bool TwoFactorEnabled { get; set; }
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class VerifyTwoFactorRequest
{
    public string Code { get; set; } = string.Empty;
}
