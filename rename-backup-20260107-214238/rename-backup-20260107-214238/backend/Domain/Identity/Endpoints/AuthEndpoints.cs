using System.Security.Cryptography;
using B2X.AuthService.Data;
using B2X.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Wolverine.Http;

namespace B2X.Identity.Endpoints;

/// <summary>
/// Authentication endpoints using Wolverine HTTP with secure cookie-based authentication
/// </summary>
public static class AuthEndpoints
{
    /// <summary>
    /// POST /api/auth/login
    /// Authenticates a user and returns JWT tokens in httpOnly cookies + CSRF token
    /// </summary>
    [WolverinePost("/api/auth/login")]
    [AllowAnonymous]
    public static async Task<IResult> Login(
        LoginCommand command,
        IAuthService authService,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var request = new LoginRequest
        {
            Email = command.Email,
            Password = command.Password
        };

        var result = await authService.LoginAsync(request, ct).ConfigureAwait(false);

        return result.Match(
            onSuccess: (response, msg) =>
            {
                // Set httpOnly cookies for tokens
                SetAuthCookies(httpContext, response.AccessToken, response.RefreshToken);

                // Generate and set CSRF token
                var csrfToken = GenerateCsrfToken();
                SetCsrfCookie(httpContext, csrfToken);

                // Return user info without tokens (they're in cookies)
                var loginResponse = new LoginResponse
                {
                    User = response.User,
                    ExpiresIn = response.ExpiresIn,
                    Requires2FA = response.Requires2FA,
                    TempUserId = response.TempUserId,
                    TwoFactorEnabled = response.TwoFactorEnabled,
                    CsrfToken = csrfToken
                };

                return Results.Ok(loginResponse);
            },
            onFailure: (code, msg) =>
            {
                var statusCode = code.GetStatusCode();
                return Results.StatusCode(statusCode);
            }
        );
    }

    /// <summary>
    /// POST /api/auth/refresh
    /// Refreshes JWT tokens using refresh token from httpOnly cookie
    /// </summary>
    [WolverinePost("/api/auth/refresh")]
    [AllowAnonymous]
    public static async Task<IResult> RefreshToken(
        IAuthService authService,
        HttpContext httpContext,
        CancellationToken ct)
    {
        // Get refresh token from httpOnly cookie
        var refreshToken = httpContext.Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Results.Unauthorized();
        }

        var result = await authService.RefreshTokenAsync(refreshToken, ct).ConfigureAwait(false);

        return result.Match(
            onSuccess: (response, msg) =>
            {
                // Set new httpOnly cookies for refreshed tokens
                SetAuthCookies(httpContext, response.AccessToken, response.RefreshToken);

                // Generate new CSRF token
                var csrfToken = GenerateCsrfToken();
                SetCsrfCookie(httpContext, csrfToken);

                var refreshResponse = new RefreshResponse
                {
                    User = response.User,
                    ExpiresIn = response.ExpiresIn,
                    CsrfToken = csrfToken
                };

                return Results.Ok(refreshResponse);
            },
            onFailure: (code, msg) =>
            {
                // Clear invalid cookies
                ClearAuthCookies(httpContext);
                ClearCsrfCookie(httpContext);

                var statusCode = code.GetStatusCode();
                return Results.StatusCode(statusCode);
            }
        );
    }

    /// <summary>
    /// POST /api/auth/logout
    /// Logs out user and clears authentication cookies
    /// </summary>
    [WolverinePost("/api/auth/logout")]
    public static IResult Logout(HttpContext httpContext)
    {
        ClearAuthCookies(httpContext);
        ClearCsrfCookie(httpContext);
        return Results.NoContent();
    }

    /// <summary>
    /// GET /api/auth/csrf-token
    /// Returns current CSRF token for forms
    /// </summary>
    [WolverineGet("/api/auth/csrf-token")]
    public static IResult GetCsrfToken(HttpContext httpContext)
    {
        var csrfToken = httpContext.Request.Cookies["csrfToken"];
        if (string.IsNullOrEmpty(csrfToken))
        {
            // Generate new CSRF token if none exists
            csrfToken = GenerateCsrfToken();
            SetCsrfCookie(httpContext, csrfToken);
        }

        return Results.Ok(new { csrfToken });
    }

    /// <summary>
    /// POST /api/auth/validate
    /// Validates current authentication status
    /// </summary>
    [WolverinePost("/api/auth/validate")]
    public static IResult ValidateToken(HttpContext httpContext)
    {
        var accessToken = httpContext.Request.Cookies["accessToken"];
        if (string.IsNullOrEmpty(accessToken))
        {
            return Results.Unauthorized();
        }

        // Token validation is handled by JWT middleware
        // If we reach here, the token is valid
        return Results.Ok(new { valid = true });
    }

    #region Cookie Management

    private static void SetAuthCookies(HttpContext httpContext, string accessToken, string refreshToken)
    {
        // Access token cookie (short-lived, httpOnly)
        httpContext.Response.Cookies.Append("accessToken", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = !httpContext.Request.Host.Host.Contains("localhost"), // Secure in production
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1),
            Path = "/"
        });

        // Refresh token cookie (long-lived, httpOnly)
        httpContext.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = !httpContext.Request.Host.Host.Contains("localhost"), // Secure in production
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/"
        });
    }

    private static void SetCsrfCookie(HttpContext httpContext, string csrfToken)
    {
        // CSRF token cookie (accessible to JavaScript, but secure)
        httpContext.Response.Cookies.Append("csrfToken", csrfToken, new CookieOptions
        {
            HttpOnly = false, // Must be accessible to JavaScript for forms
            Secure = !httpContext.Request.Host.Host.Contains("localhost"), // Secure in production
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1), // Same as access token
            Path = "/"
        });
    }

    private static void ClearAuthCookies(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete("accessToken", new CookieOptions { Path = "/" });
        httpContext.Response.Cookies.Delete("refreshToken", new CookieOptions { Path = "/" });
    }

    private static void ClearCsrfCookie(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete("csrfToken", new CookieOptions { Path = "/" });
    }

    private static string GenerateCsrfToken()
    {
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }

    #endregion
}

// Commands
public record LoginCommand(string Email, string Password);

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string TenantId = "default");

public record RefreshTokenCommand(string RefreshToken);

public record LogoutCommand(string UserId);

public record ValidateTokenCommand(string Token);

// Response DTOs for cookie-based authentication
public class LoginResponse
{
    public UserInfo? User { get; set; }
    public int ExpiresIn { get; set; }
    public bool Requires2FA { get; set; }
    public string? TempUserId { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public string CsrfToken { get; set; } = string.Empty;
}

public class RefreshResponse
{
    public UserInfo? User { get; set; }
    public int ExpiresIn { get; set; }
    public string CsrfToken { get; set; } = string.Empty;
}
