using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2X.Core.Interfaces;
using B2X.Shared.Monitoring.Enums;
using B2X.Types.Domain;

namespace B2X.Shared.Monitoring.Data.Entities;

/// <summary>
/// Entity representing a communication error in the database.
/// </summary>
[Table("communication_errors")]
public class CommunicationErrorEntity : IAuditableEntity, ITenantEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the communication error.
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the service identifier this error belongs to.
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
    /// Gets or sets the timestamp when the error occurred.
    /// </summary>
    [Required]
    [Column("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the HTTP method used in the request.
    /// </summary>
    [Column("http_method")]
    [StringLength(10)]
    public string? HttpMethod { get; set; }

    /// <summary>
    /// Gets or sets the request URL.
    /// </summary>
    [Column("request_url")]
    [StringLength(2000)]
    public string? RequestUrl { get; set; }

    /// <summary>
    /// Gets or sets the HTTP status code.
    /// </summary>
    [Column("status_code")]
    public int? StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the error type.
    /// </summary>
    [Required]
    [Column("error_type")]
    public CommunicationErrorType ErrorType { get; set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    [Required]
    [Column("message")]
    [StringLength(2000)]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the request headers as JSON.
    /// </summary>
    [Column("request_headers", TypeName = "jsonb")]
    public string? RequestHeaders { get; set; }

    /// <summary>
    /// Gets or sets the response headers as JSON.
    /// </summary>
    [Column("response_headers", TypeName = "jsonb")]
    public string? ResponseHeaders { get; set; }

    /// <summary>
    /// Gets or sets the request body (truncated for security).
    /// </summary>
    [Column("request_body")]
    [StringLength(4000)]
    public string? RequestBody { get; set; }

    /// <summary>
    /// Gets or sets the response body (truncated for security).
    /// </summary>
    [Column("response_body")]
    [StringLength(4000)]
    public string? ResponseBody { get; set; }

    /// <summary>
    /// Gets or sets the DNS resolution time in milliseconds.
    /// </summary>
    [Column("dns_resolution_ms")]
    public double? DnsResolutionMs { get; set; }

    /// <summary>
    /// Gets or sets the TCP connection time in milliseconds.
    /// </summary>
    [Column("tcp_connection_ms")]
    public double? TcpConnectionMs { get; set; }

    /// <summary>
    /// Gets or sets the TLS handshake time in milliseconds.
    /// </summary>
    [Column("tls_handshake_ms")]
    public double? TlsHandshakeMs { get; set; }

    /// <summary>
    /// Gets or sets the time to first byte in milliseconds.
    /// </summary>
    [Column("time_to_first_byte_ms")]
    public double? TimeToFirstByteMs { get; set; }

    /// <summary>
    /// Gets or sets the total response time in milliseconds.
    /// </summary>
    [Column("total_response_time_ms")]
    public double? TotalResponseTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the number of retry attempts made.
    /// </summary>
    [Column("retry_count")]
    public int RetryCount { get; set; }

    /// <summary>
    /// Gets or sets the correlated job identifier if this error is related to a specific job.
    /// </summary>
    [Column("correlated_job_id")]
    public Guid? CorrelatedJobId { get; set; }

    /// <summary>
    /// Gets or sets audit fields per IAuditableEntity.
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

    /// <summary>
    /// Navigation property for the correlated job.
    /// </summary>
    [ForeignKey(nameof(CorrelatedJobId))]
    public SchedulerJobEntity? CorrelatedJob { get; set; }
}

/// <summary>
/// Represents the type of communication error.
/// </summary>
public enum CommunicationErrorType
{
    /// <summary>
    /// DNS resolution failed.
    /// </summary>
    DnsResolutionFailed,

    /// <summary>
    /// TCP connection failed.
    /// </summary>
    ConnectionFailed,

    /// <summary>
    /// TLS/SSL handshake failed.
    /// </summary>
    TlsHandshakeFailed,

    /// <summary>
    /// HTTP timeout occurred.
    /// </summary>
    Timeout,

    /// <summary>
    /// HTTP error status code received.
    /// </summary>
    HttpError,

    /// <summary>
    /// Network error occurred.
    /// </summary>
    NetworkError,

    /// <summary>
    /// Authentication failed.
    /// </summary>
    AuthenticationFailed,

    /// <summary>
    /// Authorization failed.
    /// </summary>
    AuthorizationFailed,

    /// <summary>
    /// Request was malformed.
    /// </summary>
    BadRequest,

    /// <summary>
    /// Server returned an internal error.
    /// </summary>
    ServerError,

    /// <summary>
    /// Unknown error type.
    /// </summary>
    Unknown
}
