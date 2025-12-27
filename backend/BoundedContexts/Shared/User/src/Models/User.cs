namespace B2Connect.Shared.User.Models;

/// <summary>
/// Customer user entity (store customer)
/// Shared kernel - used by Store, Admin, and other contexts
/// 
/// Unterstützt tenant-spezifische Erweiterungen via CustomProperties
/// Beispiel: Kundennummer aus enventa Trade ERP
/// </summary>
public class User : IExtensibleEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Extension properties (IExtensibleEntity)
    /// <summary>
    /// JSON-Feld für tenant-spezifische Custom Fields
    /// Beispiel:
    /// {
    ///   "erp_customer_id": "CUST-123456",
    ///   "erp_customer_number": "123456",
    ///   "warehouse_code": "WH-001",
    ///   "customer_segment": "premium",
    ///   "internal_notes": "VIP Customer"
    /// }
    /// </summary>
    public string? CustomProperties { get; set; }

    /// <summary>
    /// Versionierung für Optimistic Concurrency Control
    /// </summary>
    public int Version { get; set; } = 1;

    // Navigation properties
    public virtual Profile? Profile { get; set; }
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public string GetFullName() => $"{FirstName} {LastName}";
}
