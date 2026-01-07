using B2X.Catalog.Models;

namespace B2X.Catalog.Services;

/// <summary>
/// Service for B2B VAT ID validation and reverse charge determination
/// Issue #31: B2B VAT-ID Validation (AStV Reverse Charge)
/// </summary>
public interface IVatIdValidationService
{
    /// <summary>
    /// Validate a VAT ID (cached for 365 days)
    /// </summary>
    Task<VatValidationResult> ValidateVatIdAsync(
        string countryCode,
        string vatNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Determine if reverse charge applies to this transaction
    /// Reverse charge applies when:
    /// - VAT ID is valid
    /// - Buyer and seller are in different EU countries
    /// </summary>
    bool ShouldApplyReverseCharge(
        VatValidationResult validation,
        string buyerCountry,
        string sellerCountry);
}
