using System.Xml.Linq;
using B2X.Catalog.Models;
using Microsoft.Extensions.Logging;

namespace B2X.Catalog.Infrastructure;

/// <summary>
/// VIES (VAT Information Exchange System) API client
/// Handles SOAP/XML communication with EU VAT validation service
/// Includes retry logic, timeout handling, and error recovery
///
/// Issue #31: B2B VAT-ID Validation (AStV Reverse Charge)
/// </summary>
public class ViesApiClient : IViesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ViesApiClient> _logger;

    /// <summary>
    /// VIES SOAP endpoint
    /// </summary>
    private const string ViesEndpoint = "https://ec.europa.eu/taxation_customs/vies/services/checkVatService";

    /// <summary>
    /// Maximum retries before giving up (default: 3)
    /// </summary>
    private const int MaxRetries = 3;

    /// <summary>
    /// HTTP timeout for VIES API calls (default: 10 seconds)
    /// </summary>
    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(10);

    public ViesApiClient(HttpClient httpClient, ILogger<ViesApiClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpClient.Timeout = RequestTimeout;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validate VAT ID against VIES with automatic retries
    /// </summary>
    public async Task<VatValidationResult> ValidateVatIdAsync(
        string countryCode,
        string vatNumber,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
        {
            throw new ArgumentException("Country code is required", nameof(countryCode));
        }

        if (string.IsNullOrWhiteSpace(vatNumber))
        {
            throw new ArgumentException("VAT number is required", nameof(vatNumber));
        }

        _logger.LogInformation("Starting VAT validation: {CountryCode}{VatNumber}", countryCode, vatNumber);

        // Retry logic with exponential backoff
        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            try
            {
                var result = await CallViesApiAsync(countryCode, vatNumber, cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("VAT validation successful: {CountryCode}{VatNumber} -> Valid={IsValid}",
                    countryCode, vatNumber, result.IsValid);
                return result;
            }
            catch (HttpRequestException ex) when (attempt < MaxRetries - 1)
            {
                // Exponential backoff: 2^attempt seconds (1s, 2s, 4s)
                var delaySeconds = Math.Pow(2, attempt);
                _logger.LogWarning(ex, "VIES API call failed (attempt {Attempt}/{MaxRetries}). Retrying in {DelaySeconds}s",
                    attempt + 1, MaxRetries, delaySeconds);

                await Task.Delay((int)(delaySeconds * 1000), cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("VIES API call cancelled");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during VIES validation: {CountryCode}{VatNumber}",
                    countryCode, vatNumber);
            }
        }

        // All retries exhausted - return safe default (invalid, retry in 24h)
        _logger.LogError("VIES API failed after {MaxRetries} retries for {CountryCode}{VatNumber}",
            MaxRetries, countryCode, vatNumber);

        return new VatValidationResult
        {
            IsValid = false,
            VatId = $"{countryCode}{vatNumber}",
            ValidatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(24)  // Retry after 24h
        };
    }

    /// <summary>
    /// Execute single VIES API call
    /// </summary>
    private async Task<VatValidationResult> CallViesApiAsync(
        string countryCode,
        string vatNumber,
        CancellationToken cancellationToken)
    {
        var soapRequest = GenerateSoapRequest(countryCode, vatNumber);

        using var content = new StringContent(
            soapRequest,
            System.Text.Encoding.UTF8,
            "application/soap+xml");

        var response = await _httpClient.PostAsync(ViesEndpoint, content, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return ParseViesResponse(responseBody, countryCode, vatNumber);
    }

    /// <summary>
    /// Generate SOAP request for VIES checkVat operation
    /// </summary>
    private static string GenerateSoapRequest(string countryCode, string vatNumber)
    {
        return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tns=""urn:ec.europa.eu:taxud:vies:services:checkVat:types_v2_1"">
    <soap:Body>
        <tns:checkVat>
            <tns:vatNumber>{System.Net.WebUtility.HtmlEncode(vatNumber)}</tns:vatNumber>
            <tns:countryCode>{System.Net.WebUtility.HtmlEncode(countryCode)}</tns:countryCode>
        </tns:checkVat>
    </soap:Body>
</soap:Envelope>";
    }

    /// <summary>
    /// Parse VIES SOAP response
    /// </summary>
    private static VatValidationResult ParseViesResponse(
        string xmlResponse,
        string countryCode,
        string vatNumber)
    {
        try
        {
            var doc = XDocument.Parse(xmlResponse);
            var ns = XNamespace.Get("urn:ec.europa.eu:taxud:vies:services:checkVat:types_v2_1");

            var validElement = doc.Root?.Descendants(ns + "valid").FirstOrDefault();
            var nameElement = doc.Root?.Descendants(ns + "name").FirstOrDefault();
            var addressElement = doc.Root?.Descendants(ns + "address").FirstOrDefault();

            var isValid = validElement?.Value.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false;
            var companyName = nameElement?.Value;
            var companyAddress = addressElement?.Value;

            // Cache duration: 365 days for valid, 24h for invalid (for retry)
            var expiresAt = isValid ? DateTime.UtcNow.AddDays(365) : DateTime.UtcNow.AddHours(24);

            return new VatValidationResult
            {
                IsValid = isValid,
                VatId = $"{countryCode}{vatNumber}",
                CompanyName = isValid ? companyName : null,
                CompanyAddress = isValid ? companyAddress : null,
                ValidatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to parse VIES response", ex);
        }
    }
}
