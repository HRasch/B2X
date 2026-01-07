namespace B2X.Catalog.Models;

/// <summary>
/// Tax rate entity for storing VAT rates per country
/// Used for Issue #30: B2C Price Transparency (PAngV Compliance)
/// </summary>
public class TaxRate
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// ISO 3166-1 alpha-2 country code (DE, AT, FR, etc.)
    /// </summary>
    public required string CountryCode { get; set; }

    /// <summary>
    /// Standard VAT rate percentage (e.g., 19 for Germany, 20 for Austria)
    /// </summary>
    public decimal StandardVatRate { get; set; }

    /// <summary>
    /// Reduced VAT rate percentage (if applicable, e.g., 7% in Germany for books)
    /// </summary>
    public decimal? ReducedVatRate { get; set; }

    /// <summary>
    /// Date when this tax rate becomes effective
    /// </summary>
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Date when this tax rate is no longer effective (null = still active)
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// Is this rate currently active?
    /// </summary>
    public bool IsActive => DateTime.UtcNow >= EffectiveDate &&
                             (ExpiryDate == null || DateTime.UtcNow < ExpiryDate);

    /// <summary>
    /// Audit trail
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
