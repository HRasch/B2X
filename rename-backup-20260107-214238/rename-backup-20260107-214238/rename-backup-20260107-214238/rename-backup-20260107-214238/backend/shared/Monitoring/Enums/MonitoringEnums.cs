namespace B2X.Shared.Monitoring;

/// <summary>
/// Represents the status of a scheduler job.
/// </summary>
public enum JobStatus
{
    /// <summary>
    /// The job is queued and waiting to be processed.
    /// </summary>
    Queued,

    /// <summary>
    /// The job is currently running.
    /// </summary>
    Running,

    /// <summary>
    /// The job completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// The job failed with an error.
    /// </summary>
    Failed,

    /// <summary>
    /// The job was cancelled.
    /// </summary>
    Cancelled,

    /// <summary>
    /// The job timed out.
    /// </summary>
    Timeout
}

/// <summary>
/// Represents the type of a connected service.
/// </summary>
public enum ServiceType
{
    /// <summary>
    /// Enterprise Resource Planning system.
    /// </summary>
    ERP,

    /// <summary>
    /// Product Information Management system.
    /// </summary>
    PIM,

    /// <summary>
    /// Customer Relationship Management system.
    /// </summary>
    CRM,

    /// <summary>
    /// Database service.
    /// </summary>
    Database,

    /// <summary>
    /// External API service.
    /// </summary>
    Api,

    /// <summary>
    /// Message queue service.
    /// </summary>
    MessageQueue,

    /// <summary>
    /// Search service.
    /// </summary>
    Search,

    /// <summary>
    /// Other service type.
    /// </summary>
    Other
}

/// <summary>
/// Represents the status of a connected service.
/// </summary>
public enum ServiceStatus
{
    /// <summary>
    /// The service is healthy and operational.
    /// </summary>
    Healthy,

    /// <summary>
    /// The service is degraded but still operational.
    /// </summary>
    Degraded,

    /// <summary>
    /// The service is unhealthy and not operational.
    /// </summary>
    Unhealthy,

    /// <summary>
    /// The service status is unknown.
    /// </summary>
    Unknown,

    /// <summary>
    /// The service is not reachable.
    /// </summary>
    Unreachable
}

/// <summary>
/// Represents the type of resource alert.
/// </summary>
public enum ResourceAlertType
{
    /// <summary>
    /// CPU usage alert.
    /// </summary>
    CpuUsage,

    /// <summary>
    /// Memory usage alert.
    /// </summary>
    MemoryUsage,

    /// <summary>
    /// Disk usage alert.
    /// </summary>
    DiskUsage,

    /// <summary>
    /// Connection pool alert.
    /// </summary>
    ConnectionPool,

    /// <summary>
    /// Thread pool alert.
    /// </summary>
    ThreadPool,

    /// <summary>
    /// Network latency alert.
    /// </summary>
    NetworkLatency
}
