namespace B2X.Shared.Messaging.Events;

/// <summary>
/// Event raised when a user registers
/// </summary>
public record UserRegisteredEvent(
    Guid TenantId,
    Guid UserId,
    string Email,
    string UserName,
    DateTimeOffset RegisteredAt);

/// <summary>
/// Event raised when a user successfully logs in
/// </summary>
public record UserLoggedInEvent(
    Guid TenantId,
    Guid UserId,
    string Email,
    DateTimeOffset LoggedInAt,
    string IpAddress);

/// <summary>
/// Event raised when a user's password is reset
/// </summary>
public record PasswordResetEvent(
    Guid TenantId,
    Guid UserId,
    string Email,
    DateTimeOffset ResetAt);

/// <summary>
/// Event raised when a user's email is verified
/// </summary>
public record EmailVerifiedEvent(
    Guid TenantId,
    Guid UserId,
    string Email,
    DateTimeOffset VerifiedAt);

/// <summary>
/// Event raised when a user's role is changed
/// </summary>
public record UserRoleChangedEvent(
    Guid TenantId,
    Guid UserId,
    string OldRole,
    string NewRole,
    DateTimeOffset ChangedAt);
