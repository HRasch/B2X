namespace B2X.Core.Interfaces;

/// <summary>
/// Interface for entities that support audit logging (CreatedAt, ModifiedAt, soft deletes)
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// When the record was created
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// User ID of who created the record
    /// </summary>
    string CreatedBy { get; set; }

    /// <summary>
    /// When the record was last modified
    /// </summary>
    DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// User ID of who last modified the record
    /// </summary>
    string? ModifiedBy { get; set; }

    /// <summary>
    /// When the record was soft-deleted
    /// </summary>
    DateTime? DeletedAt { get; set; }

    /// <summary>
    /// User ID of who soft-deleted the record
    /// </summary>
    string? DeletedBy { get; set; }

    /// <summary>
    /// Whether the record is soft-deleted
    /// </summary>
    bool IsDeleted { get; set; }
}
