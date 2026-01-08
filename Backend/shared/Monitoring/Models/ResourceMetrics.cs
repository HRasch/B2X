namespace B2X.Shared.Monitoring.Models;

/// <summary>
/// Represents resource metrics for a connected service.
/// </summary>
public record ResourceMetrics
{
    /// <summary>
    /// Gets the current CPU usage percentage.
    /// </summary>
    public double CpuPercent { get; init; }

    /// <summary>
    /// Gets the average CPU usage over the last period.
    /// </summary>
    public double CpuAverage { get; init; }

    /// <summary>
    /// Gets the peak CPU usage over the last period.
    /// </summary>
    public double CpuPeak { get; init; }

    /// <summary>
    /// Gets the memory used in bytes.
    /// </summary>
    public long MemoryUsedBytes { get; init; }

    /// <summary>
    /// Gets the total memory available in bytes.
    /// </summary>
    public long MemoryTotalBytes { get; init; }

    /// <summary>
    /// Gets the memory usage percentage.
    /// </summary>
    public double MemoryPercent => MemoryTotalBytes > 0
        ? (double)MemoryUsedBytes / MemoryTotalBytes * 100
        : 0;

    /// <summary>
    /// Gets the peak memory usage in bytes.
    /// </summary>
    public long MemoryPeakBytes { get; init; }

    /// <summary>
    /// Gets the number of active connections in the connection pool.
    /// </summary>
    public int ConnectionPoolActive { get; init; }

    /// <summary>
    /// Gets the total number of connections in the connection pool.
    /// </summary>
    public int ConnectionPoolTotal { get; init; }

    /// <summary>
    /// Gets the number of active threads.
    /// </summary>
    public int ThreadCount { get; init; }

    /// <summary>
    /// Gets the current queue depth.
    /// </summary>
    public int QueueDepth { get; init; }

    /// <summary>
    /// Gets the timestamp when these metrics were recorded.
    /// </summary>
    public DateTime Timestamp { get; init; }

    /// <summary>
    /// Gets the connection pool usage percentage.
    /// </summary>
    public double ConnectionPoolPercent => ConnectionPoolTotal > 0
        ? (double)ConnectionPoolActive / ConnectionPoolTotal * 100
        : 0;
}
