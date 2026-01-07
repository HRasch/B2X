using System.Threading;
using System.Threading.Tasks;
using B2X.Shared.Monitoring.Models;

namespace B2X.Shared.Monitoring.Abstractions;

/// <summary>
/// Interface for monitoring scheduler jobs.
/// </summary>
public interface IJobMonitor
{
    /// <summary>
    /// Starts monitoring a job.
    /// </summary>
    /// <param name="job">The job to monitor.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task StartMonitoringAsync(SchedulerJob job, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the progress of a job.
    /// </summary>
    /// <param name="jobId">The job identifier.</param>
    /// <param name="progressPercent">The progress percentage (0-100).</param>
    /// <param name="statusMessage">Optional status message.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task UpdateProgressAsync(Guid jobId, int progressPercent, string? statusMessage = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a job as completed successfully.
    /// </summary>
    /// <param name="jobId">The job identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task CompleteJobAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a job as failed.
    /// </summary>
    /// <param name="jobId">The job identifier.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="exception">Optional exception details.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task FailJobAsync(Guid jobId, string errorMessage, Exception? exception = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a job.
    /// </summary>
    /// <param name="jobId">The job identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task CancelJobAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current status of a job.
    /// </summary>
    /// <param name="jobId">The job identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The job status.</returns>
    Task<SchedulerJob?> GetJobStatusAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all jobs for a tenant.
    /// </summary>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="skip">Number of jobs to skip.</param>
    /// <param name="take">Number of jobs to take.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A collection of jobs.</returns>
    Task<IEnumerable<SchedulerJob>> GetJobsAsync(
        string tenantId,
        JobStatus? status = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default);
}
