using System;
using System.Collections.Generic;

namespace B2X.CMS.Core.Domain.Pages;

/// <summary>
/// Metadata for template overrides with validation and versioning (ADR-030)
/// </summary>
public class TemplateOverrideMetadata
{
    /// <summary>
    /// Unique identifier for this override version
    /// </summary>
    public Guid OverrideId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Version number for override history tracking
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Validation status of the template override
    /// </summary>
    public TemplateValidationStatus ValidationStatus { get; set; } = TemplateValidationStatus.Pending;

    /// <summary>
    /// Whether template has been validated
    /// </summary>
    public bool IsValidated { get; set; }

    /// <summary>
    /// Detailed validation results
    /// </summary>
    public List<string> ValidationResults { get; set; } = new();

    /// <summary>
    /// Optional preview rendering errors
    /// </summary>
    public List<string> PreviewErrors { get; set; } = new();

    /// <summary>
    /// AI-powered suggestions for improvements (confidence scores included)
    /// </summary>
    public Dictionary<string, decimal> AiSuggestions { get; set; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Whether this override is currently live/published
    /// </summary>
    public bool IsLive { get; set; }

    /// <summary>
    /// When this override was last published
    /// </summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// User ID who created this override
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// User ID who last modified this override
    /// </summary>
    public string? LastModifiedBy { get; set; }

    /// <summary>
    /// Date/time this override was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date/time this override was last modified
    /// </summary>
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Change summary for audit trail
    /// </summary>
    public string? ChangeDescription { get; set; }

    /// <summary>
    /// Security considerations for this override
    /// </summary>
    public Dictionary<string, object> SecurityMetadata { get; set; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Performance metrics for rendered template
    /// </summary>
    public Dictionary<string, double> PerformanceMetrics { get; set; } = new(StringComparer.Ordinal);
}

/// <summary>
/// Template validation status enum
/// </summary>
public enum TemplateValidationStatus
{
    /// <summary>
    /// Validation hasn't been run
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Validation passed successfully
    /// </summary>
    Valid = 1,

    /// <summary>
    /// Validation found warnings but template is usable
    /// </summary>
    ValidWithWarnings = 2,

    /// <summary>
    /// Validation found errors that prevent usage
    /// </summary>
    Invalid = 3,

    /// <summary>
    /// Validation encountered an error during processing
    /// </summary>
    Error = 4
}

/// <summary>
/// Validation issue severity levels
/// </summary>
public enum ValidationType
{
    /// <summary>
    /// Syntax validation (markup correctness)
    /// </summary>
    Syntax = 0,

    /// <summary>
    /// Security validation (XSS, injection, etc.)
    /// </summary>
    Security = 1,

    /// <summary>
    /// Performance validation (large assets, unoptimized code)
    /// </summary>
    Performance = 2,

    /// <summary>
    /// Best practices (accessibility, SEO, maintainability)
    /// </summary>
    BestPractices = 3,

    /// <summary>
    /// Accessibility compliance (WCAG)
    /// </summary>
    Accessibility = 4
}

/// <summary>
/// Result of template validation
/// </summary>
public class TemplateValidationResult
{
    /// <summary>
    /// Overall validation status
    /// </summary>
    public TemplateValidationStatus OverallStatus { get; set; }

    /// <summary>
    /// Individual validation issues
    /// </summary>
    public List<ValidationIssue> ValidationResults { get; set; } = new();

    /// <summary>
    /// AI-generated suggestions for improvements
    /// </summary>
    public List<AiSuggestion> AiSuggestions { get; set; } = new();

    /// <summary>
    /// Validation execution time (ms)
    /// </summary>
    public long ExecutionTimeMs { get; set; }

    /// <summary>
    /// Whether validation was successful
    /// </summary>
    public bool IsValid => OverallStatus is TemplateValidationStatus.Valid or TemplateValidationStatus.ValidWithWarnings;
}

/// <summary>
/// Individual validation issue
/// </summary>
public class ValidationIssue
{
    /// <summary>
    /// Issue type/category
    /// </summary>
    public ValidationType Type { get; set; }

    /// <summary>
    /// Severity level (error, warning, info)
    /// </summary>
    public IssueSeverity Severity { get; set; }

    /// <summary>
    /// Human-readable message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Line number where issue occurs (if applicable)
    /// </summary>
    public int? LineNumber { get; set; }

    /// <summary>
    /// Column number where issue occurs (if applicable)
    /// </summary>
    public int? ColumnNumber { get; set; }

    /// <summary>
    /// Suggested fix or remediation
    /// </summary>
    public string? Suggestion { get; set; }
}

/// <summary>
/// Issue severity level
/// </summary>
public enum IssueSeverity
{
    Info = 0,
    Warning = 1,
    Error = 2,
    Critical = 3
}

/// <summary>
/// AI-generated suggestion for template improvement
/// </summary>
public class AiSuggestion
{
    /// <summary>
    /// Category of suggestion (performance, security, ux, etc.)
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Suggestion text
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Confidence level (0-1)
    /// </summary>
    public decimal Confidence { get; set; }

    /// <summary>
    /// Priority level (low, medium, high)
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Whether user has accepted this suggestion
    /// </summary>
    public bool? Accepted { get; set; }

    /// <summary>
    /// Reasoning for the suggestion
    /// </summary>
    public string? Reasoning { get; set; }
}
