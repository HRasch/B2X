using B2Connect.AuthService.Data;
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

        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Login failed: {Message}", ex.Message);
            return Unauthorized(new { error = new { message = ex.Message } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error");
            return StatusCode(500, new { error = new { message = "Internal server error" } });
        }
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(request.RefreshToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = new { message = ex.Message } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Refresh error");
            return StatusCode(500, new { error = new { message = "Internal server error" } });
        }
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

        try
        {
            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                return Unauthorized(new { error = new { message = "User not found or inactive" } });
            }

            var roles = new List<string>();
            return Ok(new UserInfo
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                TenantId = user.TenantId ?? "default",
                Roles = roles.ToArray(),
                Permissions = new[] { "*" }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get current user error");
            return StatusCode(500, new { error = new { message = "Internal server error" } });
        }
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

        try
        {
            var response = await _authService.EnableTwoFactorAsync(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Enable 2FA error");
            return StatusCode(500, new { error = new { message = "Internal server error" } });
        }
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
