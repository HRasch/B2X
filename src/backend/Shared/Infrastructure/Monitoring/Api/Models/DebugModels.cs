using System.ComponentModel.DataAnnotations;
using B2X.Core.Interfaces;
using B2X.Types.Domain;

namespace B2X.Shared.Monitoring.Api.Models;

/// <summary>
/// Request model for starting a debug session
/// </summary>
public class StartDebugSessionRequest
{
    /// <summary>
    /// User agent string from browser
    /// </summary>
    [Required]
    public string UserAgent { get; set; } = string.Empty;

    /// <summary>
    /// Browser viewport dimensions (JSON)
    /// </summary>
    public string? Viewport { get; set; }

    /// <summary>
    /// Environment where session occurs
    /// </summary>
    public string Environment { get; set; } = "development";

    /// <summary>
    /// Additional metadata (JSON)
    /// </summary>
    public string? Metadata { get; set; }
}

/// <summary>
/// Request model for capturing an error
/// </summary>
public class CaptureErrorRequest
{
    /// <summary>
    /// Debug session ID
    /// </summary>
    [Required]
    public Guid SessionId { get; set; }

    /// <summary>
    /// Error level
    /// </summary>
    public string Level { get; set; } = "error";

    /// <summary>
    /// Error message
    /// </summary>
    [Required]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Error stack trace
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Source file where error occurred
    /// </summary>
    public string? SourceFile { get; set; }

    /// <summary>
    /// Line number
    /// </summary>
    public int? LineNumber { get; set; }

    /// <summary>
    /// Column number
    /// </summary>
    public int? ColumnNumber { get; set; }

    /// <summary>
    /// Browser URL
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// User agent
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Viewport dimensions (JSON)
    /// </summary>
    public string? Viewport { get; set; }

    /// <summary>
    /// Additional context (JSON)
    /// </summary>
    public string? Context { get; set; }
}

/// <summary>
/// Request model for submitting feedback
/// </summary>
public class SubmitFeedbackRequest
{
    /// <summary>
    /// Debug session ID
    /// </summary>
    [Required]
    public Guid SessionId { get; set; }

    /// <summary>
    /// Feedback type
    /// </summary>
    public string Type { get; set; } = "general";

    /// <summary>
    /// Feedback title
    /// </summary>
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Feedback description
    /// </summary>
    [Required]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// User satisfaction rating (1-5)
    /// </summary>
    [Range(1, 5)]
    public int? Rating { get; set; }

    /// <summary>
    /// Browser URL
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// User agent
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Viewport dimensions (JSON)
    /// </summary>
    public string? Viewport { get; set; }

    /// <summary>
    /// Screenshot (base64)
    /// </summary>
    public string? Screenshot { get; set; }

    /// <summary>
    /// Additional metadata (JSON)
    /// </summary>
    public string? Metadata { get; set; }
}

/// <summary>
/// Request model for capturing user action
/// </summary>
public class CaptureActionRequest
{
    /// <summary>
    /// Debug session ID
    /// </summary>
    [Required]
    public Guid SessionId { get; set; }

    /// <summary>
    /// Action type
    /// </summary>
    [Required]
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// Action description
    /// </summary>
    [Required]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Target element selector
    /// </summary>
    public string? TargetSelector { get; set; }

    /// <summary>
    /// Browser URL
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Mouse coordinates (JSON)
    /// </summary>
    public string? Coordinates { get; set; }

    /// <summary>
    /// Form data (JSON)
    /// </summary>
    public string? FormData { get; set; }

    /// <summary>
    /// Additional metadata (JSON)
    /// </summary>
    public string? Metadata { get; set; }
}