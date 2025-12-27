namespace B2Connect.Shared.Core.Entities;

/// <summary>
/// Base entity class with multi-tenant support and audit tracking.
/// All domain entities should inherit from this class.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Entity identifier
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Tenant ID for multi-tenant isolation
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Timestamp when entity was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// User ID who created the entity
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// Timestamp when entity was last modified
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// User ID who last modified the entity
    /// </summary>
    public Guid? ModifiedBy { get; set; }

    /// <summary>
    /// Timestamp when entity was deleted (soft delete)
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// User ID who deleted the entity
    /// </summary>
    public Guid? DeletedBy { get; set; }

    /// <summary>
    /// Flag indicating if entity is soft-deleted
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Optimistic concurrency control
    /// </summary>
    public byte[]? RowVersion { get; set; }
}
