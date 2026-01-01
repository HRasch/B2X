using B2Connect.Domain.Support.Models;

namespace B2Connect.Domain.Support.Queries;

/// <summary>
/// Query to get feedback status by correlation ID
/// </summary>
public record GetFeedbackStatusQuery : IQuery<FeedbackStatusResult>
{
    public Guid CorrelationId { get; init; }
}

/// <summary>
/// Result of feedback status query
/// </summary>
public record FeedbackStatusResult
{
    public Guid CorrelationId { get; init; }
    public FeedbackStatus Status { get; init; }
    public string? GitHubIssueUrl { get; init; }
    public string? Resolution { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// Query to get feedback by ID (admin only)
/// </summary>
public record GetFeedbackByIdQuery : IQuery<FeedbackDetailsResult>
{
    public Guid FeedbackId { get; init; }
}

/// <summary>
/// Detailed feedback result (admin only)
/// </summary>
public record FeedbackDetailsResult
{
    public Guid Id { get; init; }
    public FeedbackCategory Category { get; init; }
    public string Description { get; init; } = string.Empty;
    public AnonymizedContext Context { get; init; } = new();
    public IReadOnlyList<AttachmentInfo> Attachments { get; init; } = Array.Empty<AttachmentInfo>();
    public Guid CorrelationId { get; init; }
    public FeedbackStatus Status { get; init; }
    public string? GitHubIssueUrl { get; init; }
    public string? Resolution { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// Attachment information (without content for security)
/// </summary>
public record AttachmentInfo
{
    public Guid Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long Size { get; init; }
}

/// <summary>
/// Query to get feedback statistics (admin only)
/// </summary>
public record GetFeedbackStatisticsQuery : IQuery<FeedbackStatisticsResult>
{
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public FeedbackCategory? Category { get; init; }
}

/// <summary>
/// Feedback statistics result
/// </summary>
public record FeedbackStatisticsResult
{
    public int TotalFeedback { get; init; }
    public int ResolvedFeedback { get; init; }
    public double AverageResolutionTimeHours { get; init; }
    public Dictionary<FeedbackCategory, int> CategoryBreakdown { get; init; } = new();
    public Dictionary<FeedbackStatus, int> StatusBreakdown { get; init; } = new();
    public List<RecentFeedbackItem> RecentItems { get; init; } = new();
}

/// <summary>
/// Recent feedback item for statistics
/// </summary>
public record RecentFeedbackItem
{
    public Guid CorrelationId { get; init; }
    public FeedbackCategory Category { get; init; }
    public FeedbackStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Marker interface for queries
/// </summary>
public interface IQuery<T> { }</content>
<parameter name = "filePath" >/ Users / holger / Documents / Projekte / B2Connect / backend / Domain / Support / src / Queries / FeedbackQueries.cs