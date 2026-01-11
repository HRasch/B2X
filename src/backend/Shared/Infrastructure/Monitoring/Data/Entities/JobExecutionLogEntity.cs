using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2X.Core.Interfaces;
using B2X.Types.Domain;
using Microsoft.Extensions.Logging;

namespace B2X.Shared.Monitoring.Data.Entities;

/// <summary>
/// Entity representing a job execution log entry in the database.
/// </summary>
[Table("job_execution_logs")]
public class JobExecutionLogEntity : IAuditableEntity, ITenantEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the log entry.
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the job identifier this log belongs to.
    /// </summary>
    [Required]
    [Column("job_id")]
    public Guid JobId { get; set; }

    /// <summary>
    /// Gets or sets the tenant identifier.
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Gets or sets the log level.
    /// </summary>
    [Required]
    [Column("level")]
    public LogLevel Level { get; set; }

    /// <summary>
    /// Gets or sets the log message.
    /// </summary>
    [Required]
    [Column("message")]
    [StringLength(4000)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp of the log entry.
    /// </summary>
    [Required]
    [Column("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the stack trace if this is an error log.
    /// </summary>
    [Column("stack_trace")]
    [StringLength(8000)]
    public string? StackTrace { get; set; }

    /// <summary>
    /// Gets or sets additional context data as JSON.
    /// </summary>
    [Column("context", TypeName = "jsonb")]
    public string? Context { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the entity was last updated.
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public string CreatedBy { get; set; } = string.Empty;

    [Required]
    [Column("modified_at")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modified_by")]
    public string? ModifiedBy { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [Column("deleted_by")]
    public string? DeletedBy { get; set; }

    [Required]
    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Navigation property for the associated job.
    /// </summary>
    [ForeignKey(nameof(JobId))]
    public SchedulerJobEntity? Job { get; set; }
}
