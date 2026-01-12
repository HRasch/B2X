using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2X.Core.Interfaces;
using B2X.Types.Domain;

namespace B2X.Shared.Monitoring.Data.Entities;

/// <summary>
/// Represents a debug session initiated by a user
/// </summary>
[Table("sessions", Schema = "debug")]
public class DebugSessionEntity : ITenantEntity, IAuditableEntity
{
    /// <summary>
    /// Unique identifier for the debug session
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
    /// Correlation ID linking this session to backend traces
    /// </summary>
    [Column("correlation_id")]
    [StringLength(100)]
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// User identifier if authenticated
    /// </summary>
    [Column("user_id")]
    public Guid? UserId { get; set; }

    /// <summary>
    /// User agent string from browser
    /// </summary>
    [Column("user_agent")]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Browser viewport dimensions (JSON)
    /// </summary>
    [Column("viewport", TypeName = "jsonb")]
    public string? Viewport { get; set; }

    /// <summary>
    /// Environment where session occurred (development/production)
    /// </summary>
    [Column("environment")]
    [StringLength(20)]
    public string Environment { get; set; } = "development";

    /// <summary>
    /// Session start timestamp
    /// </summary>
    [Column("started_at")]
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last activity timestamp
    /// </summary>
    [Column("last_activity_at")]
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Session end timestamp (null for active sessions)
    /// </summary>
    [Column("end_time")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Additional metadata (JSON)
    /// </summary>
    [Column("metadata", TypeName = "jsonb")]
    public string? Metadata { get; set; }

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

    // Navigation properties
    public ICollection<DebugErrorEntity> Errors { get; set; } = new List<DebugErrorEntity>();
    public ICollection<DebugFeedbackEntity> Feedback { get; set; } = new List<DebugFeedbackEntity>();
    public ICollection<DebugActionEntity> Actions { get; set; } = new List<DebugActionEntity>();
}
