using B2X.Core.Interfaces;

namespace B2X.Core.Entities;

/// <summary>
/// Base entity class with audit tracking (CreatedAt, ModifiedAt, soft deletes)
/// </summary>
public abstract class AuditableEntity : IAuditableEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string CreatedBy { get; set; } = "System";

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public bool IsDeleted { get; set; }
}
