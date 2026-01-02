using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using B2Connect.Admin.Presentation.Filters;
using B2Connect.Admin.Application.Commands.Users;
using Wolverine;

namespace B2Connect.Admin.Presentation.Controllers;

/// <summary>
/// Admin Users Controller - HTTP Layer Only (CQRS Pattern)
///
/// 🏗️ Architecture:
/// HTTP Request
///   ↓
/// Controller (HTTP Concerns only!)
///   - Header validation
///   - Create Command/Query
///   - Format Response
///   ↓
/// Wolverine Message Bus (Message Dispatch)
///   ↓
/// Handler (Business Logic)
///   - HTTP calls to Identity Service
///   - Validation
///   - Exception Handling
///   ↓
/// Response Back
///
/// 🎯 Benefits: Separation of Concerns!
/// - Controller: HTTP Concerns
/// - Handler: Business Logic (incl. Identity Service communication)
/// - Filter: Cross-Cutting Concerns
///
/// Filters Applied:
/// - ValidateTenantAttribute: Validates X-Tenant-ID header
/// - ApiExceptionHandlingFilter: Centralizes error handling
/// - ValidateModelStateFilter: Validates request models
/// - ApiLoggingFilter: Logs all requests and responses
/// </summary>
[ApiController]
[Route("api/admin/users")]
[Authorize]
[ValidateTenant]
public class UsersController : ApiControllerBase
{
    private readonly IMessageBus _messageBus;

    public UsersController(
        ILogger<UsersController> logger,
        IMessageBus messageBus) : base(logger)
    {
        _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
    }

    /// <summary>
    /// Get all users for the tenant
    /// HTTP: GET /api/admin/users
    /// CQRS: GetUsersQuery dispatched to Wolverine
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UsersListResult>> GetUsers(CancellationToken ct)
    {
        _logger.LogInformation("Fetching users for tenant: {TenantId}", GetTenantId());

        // Dispatch Query via Wolverine Message Bus → Handler
        // TenantId is automatically resolved from ITenantContextAccessor in Handler
        var query = new GetUsersQuery();
        var result = await _messageBus.InvokeAsync<UsersListResult?>(query, ct);

        if (result == null)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Identity Service unavailable");
        }

        return OkResponse(result, "Users retrieved successfully");
    }

    /// <summary>
    /// Get a specific user by ID
    /// HTTP: GET /api/admin/users/{userId}
    /// CQRS: GetUserQuery dispatched to Wolverine
    /// </summary>
    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResult>> GetUser(Guid userId, CancellationToken ct)
    {
        _logger.LogInformation("Fetching user {UserId} for tenant {TenantId}", userId, GetTenantId());

        // Dispatch Query via Wolverine Message Bus → Handler
        // TenantId is automatically resolved from ITenantContextAccessor in Handler
        var query = new GetUserQuery(userId);
        var user = await _messageBus.InvokeAsync<UserResult?>(query, ct);

        if (user == null)
        {
            return NotFoundResponse($"User {userId} not found");
        }

        return OkResponse(user, "User retrieved successfully");
    }

    /// <summary>
    /// Create a new user
    /// HTTP: POST /api/admin/users
    /// CQRS: CreateUserCommand dispatched to Wolverine
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResult>> CreateUser([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var currentUserId = GetUserId();
        _logger.LogInformation("User {UserId} creating new user for tenant {TenantId}", currentUserId, GetTenantId());

        // Dispatch Command via Wolverine Message Bus → Handler
        // TenantId is automatically resolved from ITenantContextAccessor in Handler
        var command = new CreateUserCommand(
            currentUserId,
            request.Email,
            request.FirstName,
            request.LastName,
            request.Password,
            request.Roles);

        var user = await _messageBus.InvokeAsync<UserResult?>(command, ct);

        if (user == null)
        {
            return BadRequest("Failed to create user");
        }

        return CreatedResponse(nameof(GetUser), new { userId = user.Id }, user);
    }

    /// <summary>
    /// Update an existing user
    /// HTTP: PUT /api/admin/users/{userId}
    /// CQRS: UpdateUserCommand dispatched to Wolverine
    /// </summary>
    [HttpPut("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResult>> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        var currentUserId = GetUserId();
        _logger.LogInformation("User {UserId} updating user {TargetUserId} for tenant {TenantId}",
            currentUserId, userId, GetTenantId());

        // Dispatch Command via Wolverine Message Bus → Handler
        // TenantId is automatically resolved from ITenantContextAccessor in Handler
        var command = new UpdateUserCommand(
            userId,
            currentUserId,
            request.Email,
            request.FirstName,
            request.LastName,
            request.IsActive,
            request.Roles);

        var user = await _messageBus.InvokeAsync<UserResult?>(command, ct);

        if (user == null)
        {
            return NotFoundResponse($"User {userId} not found");
        }

        return OkResponse(user, "User updated successfully");
    }

    /// <summary>
    /// Delete a user
    /// HTTP: DELETE /api/admin/users/{userId}
    /// CQRS: DeleteUserCommand dispatched to Wolverine
    /// </summary>
    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUser(Guid userId, CancellationToken ct)
    {
        var currentUserId = GetUserId();
        _logger.LogInformation("User {UserId} deleting user {TargetUserId} for tenant {TenantId}",
            currentUserId, userId, GetTenantId());

        // Dispatch Command via Wolverine Message Bus → Handler
        // TenantId is automatically resolved from ITenantContextAccessor in Handler
        var command = new DeleteUserCommand(userId, currentUserId);
        var deleted = await _messageBus.InvokeAsync<bool>(command, ct);

        if (!deleted)
        {
            return NotFoundResponse($"User {userId} not found");
        }

        return NoContent();
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Request DTOs
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Request DTO for creating a user
/// </summary>
public record CreateUserRequest(
    string Email,
    string? FirstName = null,
    string? LastName = null,
    string? Password = null,
    IEnumerable<string>? Roles = null);

/// <summary>
/// Request DTO for updating a user
/// </summary>
public record UpdateUserRequest(
    string? Email = null,
    string? FirstName = null,
    string? LastName = null,
    bool? IsActive = null,
    IEnumerable<string>? Roles = null);
