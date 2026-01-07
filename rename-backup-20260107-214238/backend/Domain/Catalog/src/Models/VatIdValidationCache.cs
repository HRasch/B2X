namespace B2X.CatalogService.Models;

/// <summary>
/// Database entity for caching VAT ID validation results
/// Allows 365-day cache without repeated VIES API calls
/// Issue #31: B2B VAT-ID Validation (AStV Reverse Charge)
/// </summary>
public class VatIdValidationCache
{
    /// <summary>
    /// Primary key
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Tenant ID for multi-tenancy
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Country code from VAT ID (e.g., "DE" for Germany)
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// The VAT number part (without country code)
    /// </summary>
    public string VatNumber { get; set; } = string.Empty;

    /// <summary>
    /// Whether VIES confirmed this is a valid VAT ID
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Company name from VIES (if valid)
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// Company address from VIES (if valid)
    /// </summary>
    public string? CompanyAddress { get; set; }

    /// <summary>
    /// When the validation was performed
    /// </summary>
    public DateTime ValidatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this cache entry expires (365 days for valid, 24h for invalid)
    /// After expiry, next call will hit VIES API again
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Soft delete flag
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// When record was deleted
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Used to identify duplicates during validation
    /// </summary>
    public string GetCacheKey() => $"vat:{CountryCode}:{VatNumber}";
}
