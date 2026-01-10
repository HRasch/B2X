using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2X.Core.Interfaces;
using B2X.Types.Domain;

namespace B2X.Shared.Monitoring.Data.Entities;

/// <summary>
/// Represents user feedback captured during a debug session
/// </summary>
[Table("feedback", Schema = "debug")]
public class DebugFeedbackEntity : ITenantEntity, IAuditableEntity
{
    /// <summary>
    /// Unique identifier for the feedback
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Tenant identifier for multi-tenant isolation
    /// </summary>
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Reference to the debug session this feedback belongs to
    /// </summary>
    [Column("session_id")]
    public Guid SessionId { get; set; }

    /// <summary>
    /// Correlation ID linking this feedback to backend traces
    /// </summary>
    [Column("correlation_id")]
    [StringLength(100)]
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// Feedback type (bug-report, feature-request, general)
    /// </summary>
    [Column("type")]
    [StringLength(30)]
    public string Type { get; set; } = "general";

    /// <summary>
    /// Feedback title/subject
    /// </summary>
    [Column("title")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed feedback description
    /// </summary>
    [Column("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// User satisfaction rating (1-5 scale)
    /// </summary>
    [Column("rating")]
    [Range(1, 5)]
    public int? Rating { get; set; }

    /// <summary>
    /// Browser URL where feedback was submitted
    /// </summary>
    [Column("url")]
    public string? Url { get; set; }

    /// <summary>
    /// User agent string
    /// </summary>
    [Column("user_agent")]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Browser viewport dimensions (JSON)
    /// </summary>
    [Column("viewport", TypeName = "jsonb")]
    public string? Viewport { get; set; }

    /// <summary>
    /// Screenshot data (base64 encoded)
    /// </summary>
    [Column("screenshot")]
    public string? Screenshot { get; set; }

    /// <summary>
    /// Additional metadata (JSON)
    /// </summary>
    [Column("metadata", TypeName = "jsonb")]
    public string? Metadata { get; set; }

    /// <summary>
    /// Timestamp when feedback was submitted
    /// </summary>
    [Column("submitted_at")]
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether this feedback has been reviewed
    /// </summary>
    [Column("is_reviewed")]
    public bool IsReviewed { get; set; }

    /// <summary>
    /// Review notes from development team
    /// </summary>
    [Column("review_notes")]
    public string? ReviewNotes { get; set; }

    /// <summary>
    /// Priority level (low, medium, high, critical)
    /// </summary>
    [Column("priority")]
    [StringLength(20)]
    public string Priority { get; set; } = "medium";

    // IAuditableEntity implementation
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public string CreatedBy { get; set; } = string.Empty;

    [Column("modified_at")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modified_by")]
    public string? ModifiedBy { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("deleted_by")]
    public string? DeletedBy { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    // Navigation property
    [ForeignKey("SessionId")]
    public DebugSessionEntity? Session { get; set; }
}