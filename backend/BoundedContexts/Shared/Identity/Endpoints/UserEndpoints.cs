using Microsoft.AspNetCore.Authorization;
using Wolverine.Http;

namespace B2Connect.AuthService.Endpoints;

/// <summary>
/// User management endpoints using Wolverine HTTP
/// </summary>
public static class UserEndpoints
{
    /// <summary>
    /// GET /api/users/{userId}
    /// Gets user details by ID
    /// </summary>
    [WolverineGet("/api/users/{userId}")]
    [Authorize]
    public static async Task<IResult> GetUser(
        string userId,
        IAuthService authService,
        CancellationToken ct)
    {
        var user = await authService.GetUserByIdAsync(userId, ct);
        
        if (user == null)
        {
            return Results.NotFound(new { Message = "User not found" });
        }

        return Results.Ok(user);
    }

    /// <summary>
    /// GET /api/users/email/{email}
    /// Gets user by email address
    /// </summary>
    [WolverineGet("/api/users/email/{email}")]
    [Authorize]
    public static async Task<IResult> GetUserByEmail(
        string email,
        IAuthService authService,
        CancellationToken ct)
    {
        var user = await authService.GetUserByEmailAsync(email, ct);
        
        if (user == null)
        {
            return Results.NotFound(new { Message = "User not found" });
        }

        return Results.Ok(user);
    }

    /// <summary>
    /// PUT /api/users/{userId}
    /// Updates user profile
    /// </summary>
    [WolverinePut("/api/users/{userId}")]
    [Authorize]
    public static async Task<IResult> UpdateUser(
        string userId,
        UpdateUserCommand command,
        IAuthService authService,
        CancellationToken ct)
    {
        var result = await authService.UpdateUserAsync(
            userId, 
            command.FirstName, 
            command.LastName, 
            command.Email,
            ct);
        
        if (!result.Success)
        {
            return Results.BadRequest(new { errors = result.Errors });
        }

        return Results.Ok(result.User);
    }

    /// <summary>
    /// DELETE /api/users/{userId}
    /// Deactivates a user account (soft delete)
    /// </summary>
    [WolverineDelete("/api/users/{userId}")]
    [Authorize(Roles = "Admin")]
    public static async Task<IResult> DeactivateUser(
        string userId,
        IAuthService authService,
        CancellationToken ct)
    {
        await authService.DeactivateUserAsync(userId, ct);
        return Results.NoContent();
    }
}

// Commands
public record UpdateUserCommand(
    string? FirstName = null,
    string? LastName = null,
    string? Email = null
);
