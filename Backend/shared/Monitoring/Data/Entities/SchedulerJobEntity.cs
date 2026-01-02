using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2Connect.Core.Interfaces;
using B2Connect.Types.Domain;
using B2Connect.Shared.Monitoring;

namespace B2Connect.Shared.Monitoring.Data.Entities;

/// <summary>
/// Entity representing a scheduler job in the database.
/// </summary>
[Table("scheduler_jobs")]
public class SchedulerJobEntity : IAuditableEntity, ITenantEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the job.
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the type of the job (ERP_SYNC, PIM_IMPORT, CRM_EXPORT).
    /// </summary>
    [Required]
    [Column("job_type")]
    [StringLength(100)]
    public string JobType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tenant identifier.
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Gets or sets the current status of the job.
    /// </summary>
    [Required]
    [Column("status")]
    public JobStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the progress percentage (0-100).
    /// </summary>
    [Column("progress_percent")]
    public int ProgressPercent { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the job was created.
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("created_by")]
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the job started execution.
    /// </summary>
    [Column("started_at")]
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the job completed.
    /// </summary>
    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the error message if the job failed.
    /// </summary>
    [Column("error_message")]
    [StringLength(2000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets additional metadata as JSON.
    /// </summary>
    [Column("metadata", TypeName = "jsonb")]
    public string? Metadata { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the entity was last updated.
    /// </summary>
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
    /// Navigation property for job execution logs.
    /// </summary>
    public ICollection<JobExecutionLogEntity> ExecutionLogs { get; set; } = new List<JobExecutionLogEntity>();
}