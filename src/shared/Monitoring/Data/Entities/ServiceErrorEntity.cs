using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2X.Core.Interfaces;
using B2X.Types.Domain;

namespace B2X.Shared.Monitoring.Data.Entities;

/// <summary>
/// Entity representing a service error in the database.
/// </summary>
[Table("service_errors")]
public class ServiceErrorEntity : IAuditableEntity, ITenantEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the error.
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the service identifier this error belongs to.
    /// </summary>
    [Required]
    [Column("service_id")]
    public Guid ServiceId { get; set; }

    /// <summary>
    /// Gets or sets the tenant identifier.
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the error occurred.
    /// </summary>
    [Required]
    [Column("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    [Required]
    [Column("error_code")]
    [StringLength(100)]
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    [Required]
    [Column("message")]
    [StringLength(2000)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the stack trace if available.
    /// </summary>
    [Column("stack_trace")]
    [StringLength(8000)]
    public string? StackTrace { get; set; }

    /// <summary>
    /// Gets or sets the severity of the error.
    /// </summary>
    [Required]
    [Column("severity")]
    public ErrorSeverity Severity { get; set; }

    /// <summary>
    /// Gets or sets the correlated job identifier if this error is related to a specific job.
    /// </summary>
    [Column("correlated_job_id")]
    public Guid? CorrelatedJobId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the entity was last updated.
    /// </summary>
    [Required]
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

    [Required]
    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Navigation property for the associated service.
    /// </summary>
    [ForeignKey(nameof(ServiceId))]
    public ConnectedServiceEntity? Service { get; set; }

    /// <summary>
    /// Navigation property for the correlated job.
    /// </summary>
    [ForeignKey(nameof(CorrelatedJobId))]
    public SchedulerJobEntity? CorrelatedJob { get; set; }
}

/// <summary>
/// Represents the severity of an error.
/// </summary>
public enum ErrorSeverity
{
    /// <summary>
    /// Warning level - non-critical issue.
    /// </summary>
    Warning,

    /// <summary>
    /// Error level - functional issue.
    /// </summary>
    Error,

    /// <summary>
    /// Critical level - system-impacting issue.
    /// </summary>
    Critical
}
