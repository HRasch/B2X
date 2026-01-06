namespace B2Connect.Shared.Messaging.Events;

/// <summary>
/// Event raised when a CMS page is created
/// </summary>
public record PageCreatedEvent(
    Guid TenantId,
    Guid PageId,
    string Title,
    string Slug,
    string Language,
    DateTimeOffset CreatedAt);

/// <summary>
/// Event raised when a CMS page is updated
/// </summary>
public record PageUpdatedEvent(
    Guid TenantId,
    Guid PageId,
    string Title,
    string Slug,
    DateTimeOffset UpdatedAt);

/// <summary>
/// Event raised when a CMS page is published
/// </summary>
public record PagePublishedEvent(
    Guid TenantId,
    Guid PageId,
    string Title,
    string Slug,
    DateTimeOffset PublishedAt);

/// <summary>
/// Event raised when a CMS page is unpublished
/// </summary>
public record PageUnpublishedEvent(
    Guid TenantId,
    Guid PageId,
    DateTimeOffset UnpublishedAt);

/// <summary>
/// Event raised when a CMS page is deleted
/// </summary>
public record PageDeletedEvent(
    Guid TenantId,
    Guid PageId,
    DateTimeOffset DeletedAt);
