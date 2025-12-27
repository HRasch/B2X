using Microsoft.AspNetCore.Identity;
using Wolverine.Http;

namespace B2Connect.AuthService.Endpoints;

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
    public static async Task<IResult> Login(
        LoginCommand command,
        IAuthService authService,
        CancellationToken ct)
    {
        var result = await authService.LoginAsync(command.Email, command.Password, ct);
        
        if (!result.Success)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(new
        {
            token = result.Token,
            refreshToken = result.RefreshToken,
            expiresAt = result.ExpiresAt,
            user = result.User
        });
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
        var result = await authService.RegisterAsync(
            command.Email, 
            command.Password, 
            command.FirstName, 
            command.LastName,
            command.TenantId,
            ct);
        
        if (!result.Success)
        {
            return Results.BadRequest(new { errors = result.Errors });
        }

        return Results.Created($"/api/users/{result.UserId}", new
        {
            userId = result.UserId,
            email = command.Email
        });
    }

    /// <summary>
    /// POST /api/auth/refresh
    /// Refreshes an expired JWT token
    /// </summary>
    [WolverinePost("/api/auth/refresh")]
    public static async Task<IResult> RefreshToken(
        RefreshTokenCommand command,
        IAuthService authService,
        CancellationToken ct)
    {
        var result = await authService.RefreshTokenAsync(command.RefreshToken, ct);
        
        if (!result.Success)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(new
        {
            token = result.Token,
            refreshToken = result.RefreshToken,
            expiresAt = result.ExpiresAt
        });
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
        await authService.LogoutAsync(command.UserId, ct);
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
        var isValid = await authService.ValidateTokenAsync(command.Token, ct);
        
        if (!isValid)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(new { valid = true });
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
