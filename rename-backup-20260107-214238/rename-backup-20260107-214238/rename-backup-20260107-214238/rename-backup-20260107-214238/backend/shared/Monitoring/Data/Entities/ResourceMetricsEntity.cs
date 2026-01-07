using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2X.Core.Interfaces;
using B2X.Types.Domain;

namespace B2X.Shared.Monitoring.Data.Entities;

/// <summary>
/// Entity representing resource metrics for a connected service.
/// </summary>
[Table("resource_metrics")]
public class ResourceMetricsEntity : IAuditableEntity, ITenantEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the metrics entry.
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the service identifier these metrics belong to.
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
    /// Gets or sets the CPU usage percentage.
    /// </summary>
    [Column("cpu_percent")]
    public double CpuPercent { get; set; }

    /// <summary>
    /// Gets or sets the average CPU usage over the last hour.
    /// </summary>
    [Column("cpu_average")]
    public double CpuAverage { get; set; }

    /// <summary>
    /// Gets or sets the peak CPU usage over the last 24 hours.
    /// </summary>
    [Column("cpu_peak")]
    public double CpuPeak { get; set; }

    /// <summary>
    /// Gets or sets the used memory in bytes.
    /// </summary>
    [Column("memory_used_bytes")]
    public long MemoryUsedBytes { get; set; }

    /// <summary>
    /// Gets or sets the total available memory in bytes.
    /// </summary>
    [Column("memory_total_bytes")]
    public long MemoryTotalBytes { get; set; }

    /// <summary>
    /// Gets or sets the memory usage percentage.
    /// </summary>
    [Column("memory_percent")]
    public double MemoryPercent { get; set; }

    /// <summary>
    /// Gets or sets the peak memory usage over the last 24 hours.
    /// </summary>
    [Column("memory_peak_bytes")]
    public long MemoryPeakBytes { get; set; }

    /// <summary>
    /// Gets or sets the number of active connections in the pool.
    /// </summary>
    [Column("connection_pool_active")]
    public int ConnectionPoolActive { get; set; }

    /// <summary>
    /// Gets or sets the total size of the connection pool.
    /// </summary>
    [Column("connection_pool_total")]
    public int ConnectionPoolTotal { get; set; }

    /// <summary>
    /// Gets or sets the number of active threads/workers.
    /// </summary>
    [Column("thread_count")]
    public int ThreadCount { get; set; }

    /// <summary>
    /// Gets or sets the depth of the pending items queue.
    /// </summary>
    [Column("queue_depth")]
    public int QueueDepth { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when these metrics were collected.
    /// </summary>
    [Required]
    [Column("timestamp")]
    public DateTime Timestamp { get; set; }

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
