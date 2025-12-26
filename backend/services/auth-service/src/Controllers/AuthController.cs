using B2Connect.AuthService.Data;
using B2Connect.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2Connect.AuthService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

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
            onSuccess: (response, msg) => Ok(new { data = response, message = msg }),
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
            onSuccess: (response, msg) => Ok(new { data = response, message = msg }),
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
            ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

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
                    Roles = new string[] { },
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
            ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

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
}
