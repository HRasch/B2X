using B2X.Catalog.Core.Entities;

namespace B2X.Catalog.Application.BackgroundJobs;

/// <summary>
/// Service for managing catalog import background jobs
/// </summary>
public interface ICatalogImportJobService
{
    /// <summary>
    /// Queues a catalog import job for background processing
    /// </summary>
    Task<CatalogImportJob> QueueImportJobAsync(
        Guid tenantId,
        Stream catalogStream,
        string format,
        string? customSchemaPath = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes all pending catalog import jobs
    /// </summary>
    Task ProcessPendingJobsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the status of a catalog import job
    /// </summary>
    Task<CatalogImportJobStatus?> GetJobStatusAsync(Guid jobId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a catalog import job
/// </summary>
public class CatalogImportJob
{
    public Guid JobId { get; set; }
    public Guid TenantId { get; set; }
    public string Format { get; set; } = string.Empty;
    public string? CustomSchemaPath { get; set; }
    public DateTime QueuedAt { get; set; }
    public CatalogImportJobStatus Status { get; set; }
}

/// <summary>
/// Status of a catalog import job
/// </summary>
public enum CatalogImportJobStatus
{
    Queued = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4
}
