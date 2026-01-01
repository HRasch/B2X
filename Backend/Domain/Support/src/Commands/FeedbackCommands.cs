using B2Connect.Domain.Support.Models;
using B2Connect.Domain.Support.Services;

namespace B2Connect.Domain.Support.Commands;

/// <summary>
/// Command to validate feedback before processing
/// </summary>
public record ValidateFeedbackCommand : ICommand<ValidationResult>
{
    public FeedbackCategory Category { get; init; }
    public string Description { get; init; } = string.Empty;
    public CollectedContext Context { get; init; } = new();
    public IReadOnlyList<Attachment> Attachments { get; init; } = Array.Empty<Attachment>();
    public Guid CorrelationId { get; init; }
}

/// <summary>
/// Result of feedback creation
/// </summary>
public record FeedbackResult
{
    public Guid CorrelationId { get; init; }
    public string IssueUrl { get; init; } = string.Empty;
    public string Status { get; init; } = "created";
}

/// <summary>
/// Command to update feedback status
/// </summary>
public record UpdateFeedbackStatusCommand : ICommand<Unit>
{
    public Guid CorrelationId { get; init; }
    public FeedbackStatus Status { get; init; }
    public string? Resolution { get; init; }
}

/// <summary>
/// Command to process feedback asynchronously (for background processing)
/// </summary>
public record ProcessFeedbackCommand : ICommand<Unit>
{
    public Guid FeedbackId { get; init; }
}

/// <summary>
/// Marker interface for commands
/// </summary>
public interface ICommand<T> { }

/// <summary>
/// Unit type for commands that don't return data
/// </summary>
public class Unit
{
    public static readonly Unit Value = new();
    private Unit() { }
}</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/backend/Domain/Support/src/Commands/FeedbackCommands.cs