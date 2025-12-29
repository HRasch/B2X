using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Wolverine.Http;
using B2Connect.AuthService.Data;
using B2Connect.Types;

namespace B2Connect.Identity.Endpoints;

/// <summary>
/// Authentication endpoints using Wolverine HTTP
/// </summary>
public static class AuthEndpoints
{
    /// <summary>
    /// POST /api/auth/login
    /// Authenticates a user and returns JWT token
    /// </summary>
    [WolverinePost("/api/auth/login")]
    [AllowAnonymous]
    public static async Task<IResult> Login(
        LoginCommand command,
        IAuthService authService,
        CancellationToken ct)
    {
        var request = new LoginRequest
        {
            Email = command.Email,
            Password = command.Password
        };

        var result = await authService.LoginAsync(request);

        return result.Match(
            onSuccess: (response, msg) => Results.Ok(response),
            onFailure: (code, msg) =>
            {
                var statusCode = code.GetStatusCode();
                return Results.StatusCode(statusCode);
            }
        );
    }

    /// <summary>
    /// POST /api/auth/register
    /// Registers a new user account
    /// </summary>
    [WolverinePost("/api/auth/register")]
    public static async Task<IResult> Register(
        RegisterCommand command,
        IAuthService authService,
        CancellationToken ct)
    {
        // TODO: Implement RegisterAsync with proper Result handling
        return Results.BadRequest(new { errors = new[] { "Not yet implemented" } });
    }

    /// <summary>
    /// POST /api/auth/refresh
    /// Refreshes an expired JWT token
    /// </summary>
    [WolverinePost("/api/auth/refresh")]
    [AllowAnonymous]
    public static async Task<IResult> RefreshToken(
        RefreshTokenCommand command,
        IAuthService authService,
        CancellationToken ct)
    {
        var result = await authService.RefreshTokenAsync(command.RefreshToken);

        return result.Match(
            onSuccess: (response, msg) => Results.Ok(response),
            onFailure: (code, msg) =>
            {
                var statusCode = code.GetStatusCode();
                return Results.StatusCode(statusCode);
            }
        );
    }

    /// <summary>
    /// POST /api/auth/logout
    /// Logs out a user and invalidates their tokens
    /// </summary>
    [WolverinePost("/api/auth/logout")]
    public static async Task<IResult> Logout(
        LogoutCommand command,
        IAuthService authService,
        CancellationToken ct)
    {
        // TODO: Implement LogoutAsync
        return Results.NoContent();
    }

    /// <summary>
    /// POST /api/auth/validate
    /// Validates a JWT token
    /// </summary>
    [WolverinePost("/api/auth/validate")]
    public static async Task<IResult> ValidateToken(
        ValidateTokenCommand command,
        IAuthService authService,
        CancellationToken ct)
    {
        // TODO: Implement ValidateTokenAsync
        return Results.Unauthorized();
    }
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
