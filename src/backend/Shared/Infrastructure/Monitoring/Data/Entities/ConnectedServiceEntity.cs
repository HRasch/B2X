using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2X.Core.Interfaces;
using B2X.Shared.Monitoring;
using B2X.Types.Domain;

namespace B2X.Shared.Monitoring.Data.Entities;

/// <summary>
/// Entity representing a connected service in the database.
/// </summary>
[Table("connected_services")]
public class ConnectedServiceEntity : IAuditableEntity, ITenantEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the service.
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the service.
    /// </summary>
    [Required]
    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tenant identifier.
    /// </summary>
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Gets or sets the type of the service.
    /// </summary>
    [Required]
    [Column("type")]
    public ServiceType Type { get; set; }

    /// <summary>
    /// Gets or sets the endpoint URL of the service.
    /// </summary>
    [Required]
    [Column("endpoint")]
    [StringLength(500)]
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status of the service.
    /// </summary>
    [Required]
    [Column("status")]
    public ServiceStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the service was last checked.
    /// </summary>
    [Required]
    [Column("last_checked")]
    public DateTime LastChecked { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the service was last successfully contacted.
    /// </summary>
    [Column("last_successful")]
    public DateTime? LastSuccessful { get; set; }

    /// <summary>
    /// Gets or sets the average latency in milliseconds.
    /// </summary>
    [Column("average_latency_ms")]
    public double AverageLatencyMs { get; set; }

    /// <summary>
    /// Gets or sets the uptime percentage over the last 7 days.
    /// </summary>
    [Column("uptime_percent")]
    public double UptimePercent { get; set; }

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
    /// Navigation property for resource metrics.
    /// </summary>
    public ICollection<ResourceMetricsEntity> ResourceMetrics { get; set; } = new List<ResourceMetricsEntity>();

    /// <summary>
    /// Navigation property for service errors.
    /// </summary>
    public ICollection<ServiceErrorEntity> ServiceErrors { get; set; } = new List<ServiceErrorEntity>();

    /// <summary>
    /// Navigation property for connection test results.
    /// </summary>
    public ICollection<ConnectionTestResultEntity> ConnectionTestResults { get; set; } = new List<ConnectionTestResultEntity>();

    /// <summary>
    /// Navigation property for communication errors.
    /// </summary>
    public ICollection<CommunicationErrorEntity> CommunicationErrors { get; set; } = new List<CommunicationErrorEntity>();
}
