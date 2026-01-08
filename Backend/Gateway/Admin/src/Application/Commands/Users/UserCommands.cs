namespace B2X.Admin.Application.Commands.Users;

/// <summary>
/// User Commands & Queries - CQRS Pattern with Wolverine
///
/// Flow:
/// Controller empfängt HTTP Request
/// → Erstellt Command/Query
/// → Dispatched via IMessageBus
/// → Handler verarbeitet (inkl. HTTP-Call zum Identity Service)
/// → Response zurück an Controller
///
/// NOTE: TenantId wird automatisch via ITenantContextAccessor im Handler injiziert
/// (keine manuelle Übergabe notwendig - Tenant kommt aus Middleware)
/// </summary>
///
// ─────────────────────────────────────────────────────────────────────────────
// Queries
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Query to get all users for the current tenant (from context)
/// </summary>
public record GetUsersQuery();

/// <summary>
/// Query to get a specific user by ID
/// </summary>
public record GetUserQuery(Guid UserId);

// ─────────────────────────────────────────────────────────────────────────────
// Commands
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Command to create a new user
/// </summary>
public record CreateUserCommand(
    Guid CreatedByUserId,
    string Email,
    string? FirstName = null,
    string? LastName = null,
    string? Password = null,
    IEnumerable<string>? Roles = null);

/// <summary>
/// Command to update an existing user
/// </summary>
public record UpdateUserCommand(
    Guid UserId,
    Guid UpdatedByUserId,
    string? Email = null,
    string? FirstName = null,
    string? LastName = null,
    bool? IsActive = null,
    IEnumerable<string>? Roles = null);

/// <summary>
/// Command to delete a user
/// </summary>
public record DeleteUserCommand(
    Guid UserId,
    Guid DeletedByUserId);

// ─────────────────────────────────────────────────────────────────────────────
// Results
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Result DTO for user operations
/// </summary>
public record UserResult(
    Guid Id,
    string Email,
    string? FirstName,
    string? LastName,
    bool IsActive,
    DateTime CreatedAt,
    IEnumerable<string> Roles);

/// <summary>
/// Result DTO for user list
/// </summary>
public record UsersListResult(IEnumerable<UserResult> Users);
