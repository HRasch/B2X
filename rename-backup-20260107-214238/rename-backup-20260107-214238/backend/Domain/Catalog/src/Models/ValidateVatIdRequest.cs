namespace B2X.Catalog.Models;

/// <summary>
/// Request to validate a B2B customer's VAT ID
/// Issue #31: B2B VAT-ID Validation (AStV Reverse Charge)
/// </summary>
public class ValidateVatIdRequest
{
    /// <summary>
    /// 2-letter ISO country code (e.g., "DE", "AT", "FR")
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// VAT number without country prefix (e.g., "123456789")
    /// </summary>
    public string VatNumber { get; set; } = string.Empty;

    /// <summary>
    /// Buyer's country (for reverse charge determination)
    /// </summary>
    public string? BuyerCountry { get; set; }

    /// <summary>
    /// Seller's country (for reverse charge determination)
    /// </summary>
    public string? SellerCountry { get; set; }
}
