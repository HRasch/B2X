namespace B2Connect.CatalogService.Models;

/// <summary>
/// Result of VAT ID validation against VIES API
/// Issue #31: B2B VAT-ID Validation (AStV Reverse Charge)
/// </summary>
public record VatValidationResult
{
    /// <summary>
    /// Whether the VAT ID is valid according to VIES API
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// The validated VAT ID (format: 2-letter country code + alphanumeric number)
    /// </summary>
    public string VatId { get; init; } = string.Empty;

    /// <summary>
    /// Company name from VIES (if valid)
    /// </summary>
    public string? CompanyName { get; init; }

    /// <summary>
    /// Company address from VIES (if valid)
    /// </summary>
    public string? CompanyAddress { get; init; }

    /// <summary>
    /// When this validation was performed
    /// </summary>
    public DateTime ValidatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// When this cached validation expires (365 days for valid IDs, 24h for errors)
    /// </summary>
    public DateTime ExpiresAt { get; init; }
}
