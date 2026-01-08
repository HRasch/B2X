using B2X.Shared.Monitoring;

namespace B2X.Shared.Monitoring.Models;

/// <summary>
/// Represents a scheduler job in the monitoring system.
/// </summary>
public record SchedulerJob
{
    /// <summary>
    /// Gets the unique identifier of the job.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the type of the job (e.g., ERP_SYNC, PIM_IMPORT, CRM_EXPORT).
    /// </summary>
    public string JobType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the tenant identifier for multi-tenant isolation.
    /// </summary>
    public string TenantId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the current status of the job.
    /// </summary>
    public JobStatus Status { get; init; }

    /// <summary>
    /// Gets the progress percentage (0-100).
    /// </summary>
    public int ProgressPercent { get; init; }

    /// <summary>
    /// Gets the timestamp when the job was created.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Gets the timestamp when the job started execution.
    /// </summary>
    public DateTime? StartedAt { get; init; }

    /// <summary>
    /// Gets the timestamp when the job completed.
    /// </summary>
    public DateTime? CompletedAt { get; init; }

    /// <summary>
    /// Gets the error message if the job failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets additional metadata associated with the job.
    /// </summary>
    public IReadOnlyDictionary<string, object> Metadata { get; init; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets the duration of the job execution.
    /// </summary>
    public TimeSpan? Duration => CompletedAt.HasValue && StartedAt.HasValue
        ? CompletedAt.Value - StartedAt.Value
        : null;
}
