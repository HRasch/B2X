using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using B2X.Core.Interfaces;
using B2X.Types.Domain;

namespace B2X.Shared.Monitoring.Data.Entities;

/// <summary>
/// Entity representing a connection test result in the database.
/// </summary>
[Table("connection_test_results")]
public class ConnectionTestResultEntity : IAuditableEntity, ITenantEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the test result.
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the service identifier this test result belongs to.
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
    /// Gets or sets the timestamp when the test was executed.
    /// </summary>
    [Required]
    [Column("executed_at")]
    public DateTime ExecutedAt { get; set; }

    /// <summary>
    /// Gets or sets whether the connection test was successful.
    /// </summary>
    [Required]
    [Column("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the DNS resolution time in milliseconds.
    /// </summary>
    [Column("dns_resolution_ms")]
    public double DnsResolutionMs { get; set; }

    /// <summary>
    /// Gets or sets the TLS handshake time in milliseconds.
    /// </summary>
    [Column("tls_handshake_ms")]
    public double TlsHandshakeMs { get; set; }

    /// <summary>
    /// Gets or sets the total connection time in milliseconds.
    /// </summary>
    [Column("total_connection_ms")]
    public double TotalConnectionMs { get; set; }

    /// <summary>
    /// Gets or sets the speed rating of the connection.
    /// </summary>
    [Required]
    [Column("speed_rating")]
    [StringLength(20)]
    public string SpeedRating { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error message if the test failed.
    /// </summary>
    [Column("error_message")]
    [StringLength(1000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the recommendations as JSON.
    /// </summary>
    [Column("recommendations", TypeName = "jsonb")]
    public string? Recommendations { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the entity was created.
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
