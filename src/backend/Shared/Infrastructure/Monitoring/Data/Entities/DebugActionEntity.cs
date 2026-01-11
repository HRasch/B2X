using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2X.Core.Interfaces;
using B2X.Types.Domain;

namespace B2X.Shared.Monitoring.Data.Entities;

/// <summary>
/// Represents a user action captured during a debug session
/// </summary>
[Table("actions", Schema = "debug")]
public class DebugActionEntity : ITenantEntity, IAuditableEntity
{
    /// <summary>
    /// Unique identifier for the action
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
    /// Reference to the debug session this action belongs to
    /// </summary>
    [Column("session_id")]
    public Guid SessionId { get; set; }

    /// <summary>
    /// Correlation ID linking this action to backend traces
    /// </summary>
    [Column("correlation_id")]
    [StringLength(100)]
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// Action type (click, navigation, form-submit, etc.)
    /// </summary>
    [Column("action_type")]
    [StringLength(50)]
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// Action description
    /// </summary>
    [Column("description")]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Target element selector or identifier
    /// </summary>
    [Column("target_selector")]
    public string? TargetSelector { get; set; }

    /// <summary>
    /// Browser URL where action occurred
    /// </summary>
    [Column("url")]
    public string? Url { get; set; }

    /// <summary>
    /// Mouse coordinates (JSON)
    /// </summary>
    [Column("coordinates", TypeName = "jsonb")]
    public string? Coordinates { get; set; }

    /// <summary>
    /// Form data if action was form submission (JSON)
    /// </summary>
    [Column("form_data", TypeName = "jsonb")]
    public string? FormData { get; set; }

    /// <summary>
    /// Additional action metadata (JSON)
    /// </summary>
    [Column("metadata", TypeName = "jsonb")]
    public string? Metadata { get; set; }

    /// <summary>
    /// Timestamp when action occurred
    /// </summary>
    [Column("occurred_at")]
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Sequence number within the session
    /// </summary>
    [Column("sequence_number")]
    public int SequenceNumber { get; set; }

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
