namespace B2Connect.Catalog.Application.Interfaces;

/// <summary>
/// VIES (VAT Information Exchange System) API Client
/// Calls EU tax authority service to validate B2B VAT-IDs
/// See: https://ec.europa.eu/taxation_customs/vies/
/// </summary>
public interface IViesClient
{
    /// <summary>
    /// Validate a VAT-ID via VIES API
    /// </summary>
    /// <param name="countryCode">EU country code (e.g., "DE")</param>
    /// <param name="vatNumber">VAT number (e.g., "123456789")</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>VIES response with company data if valid</returns>
    /// <exception cref="ViesValidationException">When VIES API returns error</exception>
    Task<ViesApiResponse> ValidateAsync(
        string countryCode,
        string vatNumber,
        CancellationToken ct = default);
}

/// <summary>
/// Response from VIES API
/// </summary>
public class ViesApiResponse
{
    /// <summary>
    /// Whether the VAT-ID is valid according to VIES
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Country code part of VAT-ID
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// VAT number (without country prefix)
    /// </summary>
    public string? VatNumber { get; set; }

    /// <summary>
    /// Registered company name
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// Registered address
    /// </summary>
    public string? CompanyAddress { get; set; }

    /// <summary>
    /// When VIES data was last updated
    /// </summary>
    public DateTime? LastUpdated { get; set; }

    /// <summary>
    /// VIES response code
    /// Values: VALID, INVALID, NOT_FOUND, UNAVAILABLE, INVALID_REQUESTER
    /// </summary>
    public string? ResponseCode { get; set; }

    /// <summary>
    /// Error message if validation failed
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Exception when VIES API call fails
/// </summary>
public class ViesValidationException : Exception
{
    public string CountryCode { get; set; }
    public string VatNumber { get; set; }

    public ViesValidationException(
        string countryCode,
        string vatNumber,
        string message,
        Exception? innerException = null)
        : base(message, innerException)
    {
        CountryCode = countryCode;
        VatNumber = vatNumber;
    }
}
