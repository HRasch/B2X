using B2Connect.Shared.User.Models.Enums;

namespace B2Connect.Shared.User.Models;

/// <summary>
/// Customer address (billing/shipping)
/// </summary>
public class Address
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public AddressType AddressType { get; set; } = AddressType.Shipping;
    public required string StreetAddress { get; set; }
    public string? StreetAddress2 { get; set; }
    public required string City { get; set; }
    public required string PostalCode { get; set; }
    public required string Country { get; set; }
    public string? State { get; set; }
    public required string RecipientName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // Navigation property
    public virtual User? User { get; set; }

    public string GetFormattedAddress()
    {
        var parts = new List<string>
        {
            StreetAddress,
            StreetAddress2,
            $"{PostalCode} {City}",
            State,
            Country
        };

        return string.Join(", ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
    }
}
