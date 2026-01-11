using System.Collections.Concurrent;
using System.Text.Json;
using B2X.Catalog.Application.Adapters;
using B2X.Catalog.Application.Handlers;
using B2X.Catalog.Core.Entities;
using B2X.Catalog.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace B2X.Catalog.Application.BackgroundJobs;

/// <summary>
/// Implementation of catalog import job service
/// Manages queuing and processing of catalog import jobs
/// </summary>
public class CatalogImportJobService : ICatalogImportJobService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CatalogImportJobService> _logger;

    // In-memory job queue - in production, this should be persisted
    private readonly ConcurrentDictionary<Guid, CatalogImportJobData> _jobQueue = new();
    private readonly ConcurrentDictionary<Guid, CatalogImportJobStatus> _jobStatuses = new();

    public CatalogImportJobService(
        IServiceProvider serviceProvider,
        ILogger<CatalogImportJobService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<CatalogImportJob> QueueImportJobAsync(
        Guid tenantId,
        Stream catalogStream,
        string format,
        string? customSchemaPath = null,
        CancellationToken cancellationToken = default)
    {
        var jobId = Guid.NewGuid();

        // Store the catalog data temporarily (in production, store in blob storage)
        using var memoryStream = new MemoryStream();
        await catalogStream.CopyToAsync(memoryStream, cancellationToken);
        var catalogData = memoryStream.ToArray();

        var jobData = new CatalogImportJobData
        {
            JobId = jobId,
            TenantId = tenantId,
            Format = format,
            CustomSchemaPath = customSchemaPath,
            CatalogData = catalogData,
            QueuedAt = DateTime.UtcNow
        };

        _jobQueue[jobId] = jobData;
        _jobStatuses[jobId] = CatalogImportJobStatus.Queued;

        _logger.LogInformation("Queued catalog import job {JobId} for tenant {TenantId}, format: {Format}",
            jobId, tenantId, format);

        return new CatalogImportJob
        {
            JobId = jobId,
            TenantId = tenantId,
            Format = format,
            CustomSchemaPath = customSchemaPath,
            QueuedAt = jobData.QueuedAt,
            Status = CatalogImportJobStatus.Queued
        };
    }

    public async Task ProcessPendingJobsAsync(CancellationToken cancellationToken = default)
    {
        var pendingJobs = _jobQueue
            .Where(kvp => _jobStatuses.GetValueOrDefault(kvp.Key) == CatalogImportJobStatus.Queued)
            .ToList();

        foreach (var (jobId, jobData) in pendingJobs)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            await ProcessJobAsync(jobId, jobData, cancellationToken);
        }
    }

    public Task<CatalogImportJobStatus?> GetJobStatusAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        _jobStatuses.TryGetValue(jobId, out var status);
        return Task.FromResult(status == default ? null : (CatalogImportJobStatus?)status);
    }

    private async Task ProcessJobAsync(Guid jobId, CatalogImportJobData jobData, CancellationToken cancellationToken)
    {
        _jobStatuses[jobId] = CatalogImportJobStatus.Processing;

        try
        {
            _logger.LogInformation("Processing catalog import job {JobId}", jobId);

            using var scope = _serviceProvider.CreateScope();
            var catalogImportService = scope.ServiceProvider.GetRequiredService<CatalogImportService>();

            // Convert stored bytes back to stream
            using var stream = new MemoryStream(jobData.CatalogData);

            var result = await catalogImportService.ImportAsync(
                jobData.TenantId,
                stream,
                jobData.Format,
                jobData.CustomSchemaPath,
                cancellationToken);

            _jobStatuses[jobId] = result.Status == ImportStatus.Completed
                ? CatalogImportJobStatus.Completed
                : CatalogImportJobStatus.Failed;

            _logger.LogInformation("Completed catalog import job {JobId} with status {Status}",
                jobId, _jobStatuses[jobId]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process catalog import job {JobId}", jobId);
            _jobStatuses[jobId] = CatalogImportJobStatus.Failed;
        }
        finally
        {
            // Clean up completed jobs after some time
            _ = Task.Delay(TimeSpan.FromMinutes(5), cancellationToken)
                .ContinueWith(_ => _jobQueue.TryRemove(jobId, out CatalogImportJobData _), cancellationToken);
        }
    }
}

/// <summary>
/// Internal data structure for storing job information
/// </summary>
internal class CatalogImportJobData
{
    public Guid JobId { get; set; }
    public Guid TenantId { get; set; }
    public string Format { get; set; } = string.Empty;
    public string? CustomSchemaPath { get; set; }
    public byte[] CatalogData { get; set; } = Array.Empty<byte>();
    public DateTime QueuedAt { get; set; }
}
