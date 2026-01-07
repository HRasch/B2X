using B2X.AuthService.Data;
using B2X.Types;
using Microsoft.AspNetCore.Authorization;
using Wolverine.Http;

namespace B2X.Identity.Endpoints;

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
        // TODO: Implement GetUserByIdAsync with proper Result handling
        return Results.NotFound(new { Message = "User not found" });
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
        // TODO: Implement GetUserByEmailAsync
        return Results.NotFound(new { Message = "User not found" });
    }

    private static readonly string[] error = new[] { "Not yet implemented" };

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
        // TODO: Implement UpdateUserAsync
        return Results.BadRequest(new { errors = error });
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
        var result = await authService.DeactivateUserAsync(userId, ct).ConfigureAwait(false);

        return result.Match(
            onSuccess: (success, msg) => Results.NoContent(),
            onFailure: (code, msg) =>
            {
                var statusCode = code.GetStatusCode();
                return Results.StatusCode(statusCode);
            }
        );
    }
}

// Commands
public record UpdateUserCommand(
    string? FirstName = null,
    string? LastName = null,
    string? Email = null
);
