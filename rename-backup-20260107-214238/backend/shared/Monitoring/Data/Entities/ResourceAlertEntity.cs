using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2X.Core.Interfaces;
using B2X.Shared.Monitoring;
using B2X.Types.Domain;

namespace B2X.Shared.Monitoring.Data.Entities;

/// <summary>
/// Entity representing a resource alert for a connected service.
/// </summary>
[Table("resource_alerts")]
public class ResourceAlertEntity : IAuditableEntity, ITenantEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the alert.
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the service identifier this alert belongs to.
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
    /// Gets or sets the type of resource alert.
    /// </summary>
    [Required]
    [Column("alert_type")]
    public ResourceAlertType AlertType { get; set; }

    /// <summary>
    /// Gets or sets the threshold value that triggered the alert.
    /// </summary>
    [Column("threshold_value")]
    public double ThresholdValue { get; set; }

    /// <summary>
    /// Gets or sets the actual value that exceeded the threshold.
    /// </summary>
    [Column("actual_value")]
    public double ActualValue { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the alert was triggered.
    /// </summary>
    [Required]
    [Column("triggered_at")]
    public DateTime TriggeredAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the alert was resolved.
    /// </summary>
    [Column("resolved_at")]
    public DateTime? ResolvedAt { get; set; }

    /// <summary>
    /// Gets or sets the alert message.
    /// </summary>
    [Required]
    [Column("message")]
    [StringLength(1000)]
    public string Message { get; set; } = string.Empty;

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
}
