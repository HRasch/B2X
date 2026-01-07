namespace B2X.Shared.Infrastructure.Logging;

/// <summary>
/// Abstraction for error log storage.
/// Allows swapping backends (PostgreSQL, Elasticsearch, etc.) without changing consumers.
/// </summary>
public interface IErrorLogStorage
{
    /// <summary>
    /// Stores a single error log entry.
    /// </summary>
    Task StoreAsync(ErrorLogEntry entry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores multiple error log entries in a batch.
    /// </summary>
    Task StoreBatchAsync(IEnumerable<ErrorLogEntry> entries, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves recent errors, optionally filtered by tenant.
    /// </summary>
    Task<IReadOnlyList<ErrorLogEntry>> GetRecentAsync(
        int count = 100,
        string? tenantId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves errors by fingerprint for grouping/deduplication analysis.
    /// </summary>
    Task<IReadOnlyList<ErrorLogEntry>> GetByFingerprintAsync(
        string fingerprint,
        int count = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets error statistics for a tenant within a time range.
    /// </summary>
    Task<ErrorStatistics> GetStatisticsAsync(
        string? tenantId = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Error log entry entity for storage.
/// </summary>
public class ErrorLogEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public DateTime? ClientTimestamp { get; set; }
    public string Severity { get; set; } = "error";
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? Fingerprint { get; set; }
    public string? ComponentName { get; set; }
    public string? RoutePath { get; set; }
    public string? RouteName { get; set; }
    public string? UserId { get; set; }
    public string? TenantId { get; set; }
    public string? UserAgent { get; set; }
    public string? Url { get; set; }
    public string? ClientIp { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Creates an ErrorLogEntry from a frontend error log DTO.
    /// </summary>
    public static ErrorLogEntry FromFrontendLog(
        string? id,
        string? timestamp,
        string? severity,
        string? message,
        string? stack,
        string? componentName,
        string? routePath,
        string? routeName,
        string? userId,
        string? tenantId,
        string? userAgent,
        string? url,
        string? clientIp,
        string? fingerprint,
        Dictionary<string, object>? context)
    {
        return new ErrorLogEntry
        {
            Id = string.IsNullOrEmpty(id) ? Guid.NewGuid() : Guid.TryParse(id, out var guid) ? guid : Guid.NewGuid(),
            ClientTimestamp = DateTime.TryParse(timestamp, out var ts) ? ts : null,
            Severity = severity ?? "error",
            Message = message ?? "Unknown error",
            StackTrace = stack,
            Fingerprint = fingerprint,
            ComponentName = componentName,
            RoutePath = routePath,
            RouteName = routeName,
            UserId = userId,
            TenantId = tenantId,
            UserAgent = userAgent,
            Url = url,
            ClientIp = clientIp,
            Metadata = context,
        };
    }
}

/// <summary>
/// Error statistics summary.
/// </summary>
public class ErrorStatistics
{
    public int TotalErrors { get; set; }
    public int UniqueFingerprints { get; set; }
    public int CriticalErrors { get; set; }
    public int WarningErrors { get; set; }
    public Dictionary<string, int> ErrorsByComponent { get; set; } = new();
    public Dictionary<string, int> ErrorsByRoute { get; set; } = new();
    public List<ErrorFrequency> TopErrors { get; set; } = new();
}

/// <summary>
/// Error frequency for top errors analysis.
/// </summary>
public class ErrorFrequency
{
    public string Fingerprint { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int Count { get; set; }
    public DateTime FirstSeen { get; set; }
    public DateTime LastSeen { get; set; }
}
