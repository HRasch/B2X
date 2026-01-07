namespace B2Connect.Catalog.Models;

/// <summary>
/// Response from VAT ID validation
/// Issue #31: B2B VAT-ID Validation (AStV Reverse Charge)
/// </summary>
public class ValidateVatIdResponse
{
    /// <summary>
    /// Whether the VAT ID is valid according to VIES
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Full VAT ID (country code + number)
    /// </summary>
    public string VatId { get; set; } = string.Empty;

    /// <summary>
    /// Company name from VIES (if valid)
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// Company address from VIES (if valid)
    /// </summary>
    public string? CompanyAddress { get; set; }

    /// <summary>
    /// Whether reverse charge applies (0% VAT for valid EU VAT-ID)
    /// Only true if:
    /// - VAT ID is valid
    /// - Buyer and seller are in different EU countries
    /// </summary>
    public bool ReverseChargeApplies { get; set; }

    /// <summary>
    /// User-friendly message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// When this validation result expires
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}
