using B2Connect.CatalogService.Models;

namespace B2Connect.CatalogService.Infrastructure;

/// <summary>
/// Interface for VIES (VAT Information Exchange System) API client
/// VIES is the EU-provided VAT validation service
/// 
/// Reference: https://ec.europa.eu/taxation_customs/vies/
/// SOAP Endpoint: https://ec.europa.eu/taxation_customs/vies/services/checkVatService
/// </summary>
public interface IViesApiClient
{
    /// <summary>
    /// Validate a VAT ID against VIES database
    /// </summary>
    /// <param name="countryCode">2-letter ISO country code (e.g., "DE")</param>
    /// <param name="vatNumber">VAT number without country prefix</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with company details if valid</returns>
    Task<VatValidationResult> ValidateVatIdAsync(
        string countryCode,
        string vatNumber,
        CancellationToken cancellationToken = default);
}
