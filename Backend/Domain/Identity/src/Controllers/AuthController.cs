using System.Security.Claims;
using B2X.AuthService.Data;
using B2X.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2X.AuthService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private static readonly string[] value = Array.Empty<string>();

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { error = new { message = "Email and password are required" } });
        }

        var result = await _authService.LoginAsync(request);

        return result.Match(
            onSuccess: (response, msg) => Ok(response),
            onFailure: (code, msg) =>
            {
                var statusCode = code.GetStatusCode();
                return StatusCode(statusCode, new { error = new { code, message = code.ToMessage() } });
            }
        );
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken);

        return result.Match(
            onSuccess: (response, msg) => Ok(response),
            onFailure: (code, msg) =>
            {
                var statusCode = code.GetStatusCode();
                return StatusCode(statusCode, new { error = new { code, message = code.ToMessage() } });
            }
        );
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { error = new { message = "User not found" } });
        }

        var result = await _authService.GetUserByIdAsync(userId);

        return result.Match(
            onSuccess: (user, msg) => Ok(new
            {
                data = new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    TenantId = user.TenantId ?? "default",
                    Roles = value,
                    Permissions = new[] { "*" }
                },
                message = msg
            }),
            onFailure: (code, msg) =>
            {
                var statusCode = code.GetStatusCode();
                return StatusCode(statusCode, new { error = new { code, message = code.ToMessage() } });
            }
        );
    }

    [HttpPost("2fa/enable")]
    [Authorize]
    public async Task<IActionResult> Enable2FA()
    {
        var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await _authService.EnableTwoFactorAsync(userId);

        return result.Match(
            onSuccess: (response, msg) => Ok(new { data = response, message = msg }),
            onFailure: (code, msg) =>
            {
                var statusCode = code.GetStatusCode();
                return StatusCode(statusCode, new { error = new { code, message = code.ToMessage() } });
            }
        );
    }

    [HttpPost("2fa/verify")]
    public async Task<IActionResult> Verify2FA([FromBody] VerifyTwoFactorRequest request)
    {
        if (string.IsNullOrEmpty(request.Code))
        {
            return BadRequest(new { error = new { message = "Code is required" } });
        }

        // TODO: Get userId from temp token or session
        // For now, return error
        return BadRequest(new { error = new { message = "2FA verification not yet implemented" } });
    }

    [HttpGet("users")]
    [Authorize]
    public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null)
    {
        // Log current user claims for debugging
        var userRoles = User.Claims.Where(c => string.Equals(c.Type, ClaimTypes.Role, StringComparison.Ordinal)).Select(c => c.Value).ToList();
        _logger.LogInformation("User roles from token: {Roles}", string.Join(", ", userRoles));

        // Check if user has Admin role (case-insensitive)
        var isAdmin = userRoles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase));
        if (!isAdmin)
        {
            _logger.LogWarning("User attempted to access users without Admin role. Roles: {Roles}", string.Join(", ", userRoles));
            return Forbid();
        }

        var result = await _authService.GetAllUsersAsync(page, pageSize, search);

        return result.Match(
            onSuccess: (users, msg) => Ok(new { data = users, message = msg }),
            onFailure: (code, msg) =>
            {
                var statusCode = code.GetStatusCode();
                return StatusCode(statusCode, new { error = new { code, message = code.ToMessage() } });
            }
        );
    }

    [HttpGet("users/{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(string id)
    {
        // Check if user has Admin role
        var userRoles = User.Claims.Where(c => string.Equals(c.Type, ClaimTypes.Role, StringComparison.Ordinal)).Select(c => c.Value).ToList();
        if (!userRoles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
        {
            return Forbid();
        }

        var result = await _authService.GetUserByIdAsync(id);

        return result.Match(
            onSuccess: (user, msg) => Ok(new { data = MapUserToDto(user), message = msg }),
            onFailure: (code, msg) =>
            {
                var statusCode = code.GetStatusCode();
                return StatusCode(statusCode, new { error = new { code, message = code.ToMessage() } });
            }
        );
    }

    [HttpPatch("users/{id}/status")]
    [Authorize]
    public async Task<IActionResult> ToggleUserStatus(string id, [FromBody] ToggleUserStatusRequest request)
    {
        // Check if user has Admin role
        var userRoles = User.Claims.Where(c => string.Equals(c.Type, ClaimTypes.Role, StringComparison.Ordinal)).Select(c => c.Value).ToList();
        if (!userRoles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
        {
            return Forbid();
        }

        var result = await _authService.ToggleUserStatusAsync(id, request.IsActive);

        return result.Match(
            onSuccess: (user, msg) => Ok(new { data = MapUserToDto(user), message = msg }),
            onFailure: (code, msg) =>
            {
                var statusCode = code.GetStatusCode();
                return StatusCode(statusCode, new { error = new { code, message = code.ToMessage() } });
            }
        );
    }

    private static object MapUserToDto(AppUser user)
    {
        return new
        {
            id = user.Id,
            email = user.Email,
            firstName = user.FirstName ?? string.Empty,
            lastName = user.LastName ?? string.Empty,
            tenantId = user.TenantId ?? "default",
            isActive = user.IsActive,
            isTwoFactorEnabled = user.IsTwoFactorRequired,
            roles = Array.Empty<string>()
        };
    }
}

public class ToggleUserStatusRequest
{
    public required bool IsActive { get; set; }
}
