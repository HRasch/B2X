// HTTP communication contracts for massive data loads
// Compatible with both .NET Framework 4.8 and .NET 10
//
// Key techniques for high-volume data transfer:
// - Cursor-based pagination (not offset) for consistent results during sync
// - Chunked/streaming responses for memory efficiency
// - Delta sync with watermarks to minimize data transfer
// - Batch operations with configurable chunk sizes
// - Compression support (GZip/Deflate)
// - ETags for conditional requests

using System.ComponentModel.DataAnnotations;

namespace B2Connect.ERP.Abstractions.Http;

#region Pagination & Cursors

/// <summary>
/// Cursor-based pagination request for consistent results during sync.
/// More efficient than offset pagination for large datasets.
/// </summary>
public class CursorPageRequest
{
    /// <summary>
    /// Tenant identifier for multi-tenant scenarios.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Cursor from previous response. Null for first page.
    /// </summary>
    public string? Cursor { get; set; }

    /// <summary>
    /// Maximum items per page. Default: 1000, Max: 10000.
    /// </summary>
    [Range(1, 10000)]
    public int PageSize { get; set; } = 1000;

    /// <summary>
    /// Optional filters to apply.
    /// </summary>
    public List<QueryFilter>? Filters { get; set; }

    /// <summary>
    /// Optional sorting (single field for cursor consistency).
    /// </summary>
    public SortField? Sort { get; set; }

    /// <summary>
    /// Fields to include (projection). Null = all fields.
    /// </summary>
    public List<string>? Fields { get; set; }
}

/// <summary>
/// Cursor-based pagination response.
/// </summary>
/// <typeparam name="T">Item type.</typeparam>
public class CursorPageResponse<T>
{
    /// <summary>
    /// Items in this page.
    /// </summary>
    public List<T> Items { get; set; } = [];

    /// <summary>
    /// Cursor for next page. Null if no more pages.
    /// </summary>
    public string? NextCursor { get; set; }

    /// <summary>
    /// Whether more pages are available.
    /// </summary>
    public bool HasMore { get; set; }

    /// <summary>
    /// Total count (optional, expensive for large datasets).
    /// </summary>
    public long? TotalCount { get; set; }

    /// <summary>
    /// Server timestamp for this response.
    /// </summary>
    public DateTime ServerTimestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ETag for conditional requests.
    /// </summary>
    public string? ETag { get; set; }
}

#endregion

#region Delta Sync (Watermark-based)

/// <summary>
/// Delta sync request using watermark/timestamp for incremental updates.
/// </summary>
public class DeltaSyncRequest
{
    /// <summary>
    /// Tenant identifier.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Entity type to sync.
    /// </summary>
    [Required]
    public EntityType EntityType { get; set; }

    /// <summary>
    /// Watermark from previous sync. Records modified after this will be returned.
    /// </summary>
    public long? Watermark { get; set; }

    /// <summary>
    /// Alternative: UTC timestamp for last sync.
    /// </summary>
    public DateTime? SinceUtc { get; set; }

    /// <summary>
    /// Maximum items per batch.
    /// </summary>
    [Range(1, 50000)]
    public int BatchSize { get; set; } = 5000;

    /// <summary>
    /// Include deleted records (tombstones).
    /// </summary>
    public bool IncludeDeleted { get; set; } = true;

    /// <summary>
    /// Continuation token for resuming interrupted sync.
    /// </summary>
    public string? ContinuationToken { get; set; }
}

/// <summary>
/// Delta sync response with changed records.
/// </summary>
/// <typeparam name="T">Item type.</typeparam>
public class DeltaSyncResponse<T>
{
    /// <summary>
    /// Changed items (created, updated, deleted).
    /// </summary>
    public List<DeltaItem<T>> Changes { get; set; } = [];

    /// <summary>
    /// New watermark for next sync.
    /// </summary>
    public long NewWatermark { get; set; }

    /// <summary>
    /// Server timestamp of this response.
    /// </summary>
    public DateTime ServerTimestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Continuation token if more changes available.
    /// </summary>
    public string? ContinuationToken { get; set; }

    /// <summary>
    /// Whether more changes are available.
    /// </summary>
    public bool HasMore { get; set; }

    /// <summary>
    /// Statistics about this sync batch.
    /// </summary>
    public DeltaSyncStats Stats { get; set; } = new();
}

/// <summary>
/// Individual changed item with change type.
/// </summary>
/// <typeparam name="T">Item type.</typeparam>
public class DeltaItem<T>
{
    /// <summary>
    /// Type of change.
    /// </summary>
    public ChangeType ChangeType { get; set; }

    /// <summary>
    /// The changed item. Null for deletes (only Id provided).
    /// </summary>
    public T? Item { get; set; }

    /// <summary>
    /// Item identifier (always present, even for deletes).
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Row version / ETag for optimistic concurrency.
    /// </summary>
    public byte[]? RowVersion { get; set; }

    /// <summary>
    /// Timestamp of this change.
    /// </summary>
    public DateTime ChangedAtUtc { get; set; }
}

/// <summary>
/// Type of change in delta sync.
/// </summary>
public enum ChangeType
{
    Created = 1,
    Updated = 2,
    Deleted = 3
}

/// <summary>
/// Statistics for delta sync operation.
/// </summary>
public class DeltaSyncStats
{
    public int CreatedCount { get; set; }
    public int UpdatedCount { get; set; }
    public int DeletedCount { get; set; }
    public int TotalChanges => CreatedCount + UpdatedCount + DeletedCount;
    public long ProcessingTimeMs { get; set; }
}

#endregion

#region Chunked/Streaming Transfer

/// <summary>
/// Request for streaming large datasets.
/// </summary>
public class StreamRequest
{
    public string? TenantId { get; set; }

    [Required]
    public EntityType EntityType { get; set; }

    /// <summary>
    /// Chunk size for streaming (records per chunk).
    /// </summary>
    [Range(100, 10000)]
    public int ChunkSize { get; set; } = 1000;

    /// <summary>
    /// Optional filter criteria.
    /// </summary>
    public QueryRequest? Filter { get; set; }

    /// <summary>
    /// Enable GZip compression for response.
    /// </summary>
    public bool UseCompression { get; set; } = true;

    /// <summary>
    /// Format: json, jsonl (JSON Lines), or csv.
    /// </summary>
    public StreamFormat Format { get; set; } = StreamFormat.JsonLines;
}

/// <summary>
/// Stream format for chunked transfer.
/// </summary>
public enum StreamFormat
{
    /// <summary>
    /// Standard JSON array (requires buffering).
    /// </summary>
    Json = 0,

    /// <summary>
    /// JSON Lines (newline-delimited JSON) - best for streaming.
    /// </summary>
    JsonLines = 1,

    /// <summary>
    /// CSV format for tabular data.
    /// </summary>
    Csv = 2
}

/// <summary>
/// Chunk header for streaming responses.
/// Sent before each chunk in the stream.
/// </summary>
public class ChunkHeader
{
    /// <summary>
    /// Chunk sequence number (1-based).
    /// </summary>
    public int ChunkNumber { get; set; }

    /// <summary>
    /// Number of items in this chunk.
    /// </summary>
    public int ItemCount { get; set; }

    /// <summary>
    /// Whether this is the last chunk.
    /// </summary>
    public bool IsLastChunk { get; set; }

    /// <summary>
    /// Total items processed so far.
    /// </summary>
    public long TotalProcessed { get; set; }

    /// <summary>
    /// Estimated total items (may be null if unknown).
    /// </summary>
    public long? EstimatedTotal { get; set; }

    /// <summary>
    /// Checksum for data integrity (optional).
    /// </summary>
    public string? Checksum { get; set; }
}

#endregion

#region Batch Operations

/// <summary>
/// Batch write request for bulk inserts/updates.
/// </summary>
/// <typeparam name="T">Item type.</typeparam>
public class BatchWriteRequest<T>
{
    public string? TenantId { get; set; }

    /// <summary>
    /// Items to write.
    /// </summary>
    [Required]
    public List<T> Items { get; set; } = [];

    /// <summary>
    /// Write mode: Insert, Update, or Upsert.
    /// </summary>
    public BatchWriteMode Mode { get; set; } = BatchWriteMode.Upsert;

    /// <summary>
    /// Continue processing remaining items if some fail.
    /// </summary>
    public bool ContinueOnError { get; set; } = true;

    /// <summary>
    /// Validate items before writing (slower but safer).
    /// </summary>
    public bool ValidateBeforeWrite { get; set; } = true;

    /// <summary>
    /// Transaction ID for distributed transactions.
    /// </summary>
    public string? TransactionId { get; set; }
}

/// <summary>
/// Batch write mode.
/// </summary>
public enum BatchWriteMode
{
    /// <summary>
    /// Insert new records only. Fails if exists.
    /// </summary>
    Insert = 1,

    /// <summary>
    /// Update existing records only. Fails if not exists.
    /// </summary>
    Update = 2,

    /// <summary>
    /// Insert or update based on existence.
    /// </summary>
    Upsert = 3
}

/// <summary>
/// Batch write response with detailed results.
/// </summary>
/// <typeparam name="T">Item type.</typeparam>
public class BatchWriteResponse<T>
{
    /// <summary>
    /// Overall success (all items processed without error).
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Total items in request.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Successfully processed items.
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Items that failed.
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// Inserted items count.
    /// </summary>
    public int InsertedCount { get; set; }

    /// <summary>
    /// Updated items count.
    /// </summary>
    public int UpdatedCount { get; set; }

    /// <summary>
    /// Skipped items count (e.g., no changes detected).
    /// </summary>
    public int SkippedCount { get; set; }

    /// <summary>
    /// Detailed errors per item.
    /// </summary>
    public List<BatchItemError> Errors { get; set; } = [];

    /// <summary>
    /// Processing time in milliseconds.
    /// </summary>
    public long ProcessingTimeMs { get; set; }

    /// <summary>
    /// Transaction ID if applicable.
    /// </summary>
    public string? TransactionId { get; set; }
}

/// <summary>
/// Error details for a single batch item.
/// </summary>
public class BatchItemError
{
    /// <summary>
    /// Index of the failed item in the request.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Item identifier if available.
    /// </summary>
    public string? ItemId { get; set; }

    /// <summary>
    /// Error code.
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Field that caused the error (for validation errors).
    /// </summary>
    public string? Field { get; set; }
}

#endregion

#region Health & Status

/// <summary>
/// Connector health check response.
/// </summary>
public class ConnectorHealthResponse
{
    /// <summary>
    /// Overall health status.
    /// </summary>
    public HealthStatus Status { get; set; }

    /// <summary>
    /// Connector version.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// .NET runtime version.
    /// </summary>
    public string RuntimeVersion { get; set; } = string.Empty;

    /// <summary>
    /// Connected tenants.
    /// </summary>
    public List<TenantStatus> Tenants { get; set; } = [];

    /// <summary>
    /// Server UTC time.
    /// </summary>
    public DateTime ServerTimeUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Uptime in seconds.
    /// </summary>
    public long UptimeSeconds { get; set; }
}

/// <summary>
/// Overall health status.
/// </summary>
public enum HealthStatus
{
    Healthy = 0,
    Degraded = 1,
    Unhealthy = 2
}

/// <summary>
/// Status of a single tenant connection.
/// </summary>
public class TenantStatus
{
    public string TenantId { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public DateTime? LastSuccessfulSync { get; set; }
    public string? LastError { get; set; }
    public long ArticleCount { get; set; }
    public long CustomerCount { get; set; }
    public long OrderCount { get; set; }
}

#endregion

#region HTTP Headers & Conventions

/// <summary>
/// Standard HTTP headers for ERP connector communication.
/// </summary>
public static class ErpHttpHeaders
{
    /// <summary>
    /// Tenant ID header.
    /// </summary>
    public const string TenantId = "X-Tenant-Id";

    /// <summary>
    /// Correlation ID for request tracing.
    /// </summary>
    public const string CorrelationId = "X-Correlation-Id";

    /// <summary>
    /// Request timestamp.
    /// </summary>
    public const string RequestTimestamp = "X-Request-Timestamp";

    /// <summary>
    /// Watermark for delta sync.
    /// </summary>
    public const string Watermark = "X-Watermark";

    /// <summary>
    /// Continuation token for pagination.
    /// </summary>
    public const string ContinuationToken = "X-Continuation-Token";

    /// <summary>
    /// Total count (when available).
    /// </summary>
    public const string TotalCount = "X-Total-Count";

    /// <summary>
    /// Chunk number for streaming.
    /// </summary>
    public const string ChunkNumber = "X-Chunk-Number";

    /// <summary>
    /// Whether more data is available.
    /// </summary>
    public const string HasMore = "X-Has-More";

    /// <summary>
    /// Content checksum for integrity.
    /// </summary>
    public const string ContentChecksum = "X-Content-Checksum";

    /// <summary>
    /// API version.
    /// </summary>
    public const string ApiVersion = "X-Api-Version";
}

/// <summary>
/// Standard API endpoints for ERP connector.
/// </summary>
public static class ErpApiEndpoints
{
    public const string Health = "/api/health";
    public const string Articles = "/api/articles";
    public const string ArticlesSync = "/api/articles/sync";
    public const string ArticlesStream = "/api/articles/stream";
    public const string ArticlesBatch = "/api/articles/batch";
    public const string Customers = "/api/customers";
    public const string CustomersSync = "/api/customers/sync";
    public const string CustomersStream = "/api/customers/stream";
    public const string CustomersBatch = "/api/customers/batch";
    public const string Orders = "/api/orders";
    public const string OrdersSync = "/api/orders/sync";
    public const string OrdersStream = "/api/orders/stream";
    public const string OrdersBatch = "/api/orders/batch";
}

#endregion
