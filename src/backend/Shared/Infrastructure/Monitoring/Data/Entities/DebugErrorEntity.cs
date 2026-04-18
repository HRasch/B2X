using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2X.Core.Interfaces;
using B2X.Types.Domain;

namespace B2X.Shared.Monitoring.Data.Entities;

/// <summary>
/// Represents an error captured during a debug session
/// </summary>
[Table("errors", Schema = "debug")]
public class DebugErrorEntity : ITenantEntity, IAuditableEntity
{
    /// <summary>
    /// Unique identifier for the error
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
    /// Reference to the debug session this error belongs to
    /// </summary>
    [Column("session_id")]
    public Guid SessionId { get; set; }

    /// <summary>
    /// Correlation ID linking this error to backend traces
    /// </summary>
    [Column("correlation_id")]
    [StringLength(100)]
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// Error level (error, warning, info)
    /// </summary>
    [Column("level")]
    [StringLength(20)]
    public string Level { get; set; } = "error";

    /// <summary>
    /// Error message
    /// </summary>
    [Column("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Error stack trace
    /// </summary>
    [Column("stack_trace")]
    public string? StackTrace { get; set; }

    /// <summary>
    /// Source file where error occurred
    /// </summary>
    [Column("source_file")]
    public string? SourceFile { get; set; }

    /// <summary>
    /// Line number where error occurred
    /// </summary>
    [Column("line_number")]
    public int? LineNumber { get; set; }

    /// <summary>
    /// Column number where error occurred
    /// </summary>
    [Column("column_number")]
    public int? ColumnNumber { get; set; }

    /// <summary>
    /// Component/module where error occurred
    /// </summary>
    [Column("component")]
    [StringLength(100)]
    public string? Component { get; set; }

    /// <summary>
    /// Browser URL where error occurred
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
    /// Additional error context (JSON)
    /// </summary>
    [Column("context", TypeName = "jsonb")]
    public string? Context { get; set; }

    /// <summary>
    /// Timestamp when error occurred
    /// </summary>
    [Column("occurred_at")]
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether this error has been resolved
    /// </summary>
    [Column("is_resolved")]
    public bool IsResolved { get; set; }

    /// <summary>
    /// Resolution notes
    /// </summary>
    [Column("resolution_notes")]
    public string? ResolutionNotes { get; set; }

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
