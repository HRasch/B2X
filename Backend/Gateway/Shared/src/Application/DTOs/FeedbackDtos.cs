using System.ComponentModel.DataAnnotations;
using B2Connect.Domain.Support.Models;
using B2Connect.Domain.Support.Services;

namespace B2Connect.Gateway.Shared.Application.DTOs;

/// <summary>
/// Request DTO for creating feedback
/// </summary>
public record CreateFeedbackRequest
{
    [Required]
    [EnumDataType(typeof(FeedbackCategory))]
    public FeedbackCategory Category { get; init; }

    [Required]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters")]
    public string Description { get; init; } = string.Empty;

    public IReadOnlyList<AttachmentDto> Attachments { get; init; } = Array.Empty<AttachmentDto>();

    [Required]
    public CollectedContext Context { get; init; } = new();

    /// <summary>
    /// Client identifier for ban management (IP address or session ID)
    /// </summary>
    public string? ClientIdentifier { get; init; }
}

/// <summary>
/// Attachment DTO for file uploads
/// </summary>
public record AttachmentDto
{
    [Required]
    [StringLength(255, ErrorMessage = "Filename too long")]
    public string FileName { get; init; } = string.Empty;

    [Required]
    [RegularExpression(@"^(image|text|application)/.+", ErrorMessage = "Invalid content type")]
    public string ContentType { get; init; } = string.Empty;

    [Required]
    public byte[] Content { get; init; } = Array.Empty<byte>();

    public long Size => Content.Length;
}

/// <summary>
/// Response DTO for feedback validation
/// </summary>
public record ValidateFeedbackResponse
{
    public bool IsValid { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public IReadOnlyList<string> Reasons { get; init; } = Array.Empty<string>();
    public string Severity { get; init; } = string.Empty;
}

/// <summary>
/// Response DTO for feedback creation
/// </summary>
public record CreateFeedbackResponse
{
    public Guid CorrelationId { get; init; }
    public string IssueUrl { get; init; } = string.Empty;
    public string Status { get; init; } = "created";
    public string Message { get; init; } = "Vielen Dank für Ihr Feedback!";
}

/// <summary>
/// Response DTO for feedback status
/// </summary>
public record FeedbackStatusResponse
{
    public Guid CorrelationId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? GitHubIssueUrl { get; init; }
    public string? Resolution { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// Validation rules for feedback requests
/// </summary>
public static class FeedbackValidationRules
{
    public const int MaxDescriptionLength = 1000;
    public const int MinDescriptionLength = 10;
    public const int MaxAttachments = 3;
    public const long MaxAttachmentSize = 5 * 1024 * 1024; // 5MB
    public const int RateLimitRequestsPerHour = 5;
    public const int RateLimitRequestsPerDay = 10;

    public static readonly string[] AllowedContentTypes = {
        "image/jpeg", "image/png", "image/gif",
        "text/plain", "text/csv",
        "application/json", "application/xml"
    };

    /// <summary>
    /// Validates if content type is allowed
    /// </summary>
    public static bool IsContentTypeAllowed(string contentType)
    {
        return AllowedContentTypes.Contains(contentType.ToLower());
    }

    /// <summary>
    /// Validates attachment size
    /// </summary>
    public static bool IsAttachmentSizeValid(long size)
    {
        return size > 0 && size <= MaxAttachmentSize;
    }

    /// <summary>
    /// Validates number of attachments
    /// </summary>
    public static bool IsAttachmentCountValid(int count)
    {
        return count >= 0 && count <= MaxAttachments;
    }
}</content>
<parameter name = "filePath" >/ Users / holger / Documents / Projekte / B2Connect / backend / Gateway / Shared / src / Application / DTOs / FeedbackDtos.cs