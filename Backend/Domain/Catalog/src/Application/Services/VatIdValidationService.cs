using B2Connect.Catalog.Application.DTOs;
using B2Connect.Catalog.Application.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace B2Connect.Catalog.Application.Services;

/// <summary>
/// VAT-ID Validation Service for B2B Orders
/// 
/// Responsibilities:
/// - Validate EU VAT-IDs via VIES (VAT Information Exchange System)
/// - Cache validation results (365 days per NIS2 requirement)
/// - Determine if reverse charge applies (Seller doesn't charge VAT if buyer has valid VAT-ID in different EU country)
/// 
/// Regulatory Reference:
/// - AStV (Umsatzsteuer-Systemrichtlinie): Reverse Charge Rules
/// - NIS2: Cache retention (1 year minimum for audit trail)
/// </summary>
public interface IVatIdValidationService
{
    /// <summary>
    /// Validate a VAT-ID against VIES API
    /// </summary>
    Task<VatValidationResult> ValidateVatIdAsync(
        string country,
        string vatId,
        CancellationToken ct = default);

    /// <summary>
    /// Check if reverse charge applies (Buyer: different EU country with valid VAT-ID)
    /// </summary>
    bool ShouldApplyReverseCharge(
        VatValidationResult validation,
        string buyerCountry,
        string sellerCountry);
}

public class VatIdValidationService : IVatIdValidationService
{
    private readonly IViesClient _viesClient;
    private readonly IDistributedCache _cache;
    private readonly ILogger<VatIdValidationService> _logger;

    // Cache configuration
    private const int CACHE_DURATION_DAYS = 365;  // NIS2: Annual retention for audit trail

    public VatIdValidationService(
        IViesClient viesClient,
        IDistributedCache cache,
        ILogger<VatIdValidationService> logger)
    {
        _viesClient = viesClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<VatValidationResult> ValidateVatIdAsync(
        string country,
        string vatId,
        CancellationToken ct = default)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(country) || country.Length != 2)
        {
            _logger.LogWarning("Invalid country code: {Country}", country);
            return VatValidationResult.Invalid("Country code must be 2 characters");
        }

        if (string.IsNullOrWhiteSpace(vatId) || vatId.Length < 2 || vatId.Length > 12)
        {
            _logger.LogWarning("Invalid VAT number: {VatId}", vatId);
            return VatValidationResult.Invalid("VAT number must be 2-12 characters");

            if (!string.IsNullOrEmpty(cachedResult))
            {
                _logger.LogDebug("VAT-ID validation cache hit: {Country}-{VatId}", country, vatId);
                var result = JsonSerializer.Deserialize<VatValidationResult>(cachedResult);
                result!.WasFromCache = true;
                return result;
            }

            // Call VIES API
            _logger.LogInformation("Validating VAT-ID via VIES: {Country}-{VatId}", country, vatId);

            try
            {
                var viesResponse = await _viesClient.ValidateAsync(country, vatId, ct);

                var result = new VatValidationResult
                {
                    IsValid = viesResponse.IsValid,
                    CountryCode = viesResponse.CountryCode,
                    VatNumber = viesResponse.VatNumber,
                    CompanyName = viesResponse.CompanyName,
                    CompanyAddress = viesResponse.CompanyAddress,
                    ValidatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(CACHE_DURATION_DAYS),
                    WasFromCache = false
                };

                // Cache the result (365 days)
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(CACHE_DURATION_DAYS)
                };

                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(result),
                    cacheOptions,
                    ct
                );

                _logger.LogInformation(
                    "VAT-ID validated successfully: {Country}-{VatId}, Valid: {IsValid}",
                    country, vatId, result.IsValid
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "VIES API error validating {Country}-{VatId}", country, vatId);
                throw new VatIdValidationException($"Failed to validate VAT-ID: {ex.Message}", ex);
            }
        }

    public bool ShouldApplyReverseCharge(
        VatValidationResult validation,
        string buyerCountry,
        string sellerCountry)
    {
        if (!validation.IsValid)
        {
            _logger.LogDebug("Reverse charge: VAT-ID not valid");
            return false;
        }

        // Reverse charge applies if:
        // 1. Buyer has valid VAT-ID
        // 2. Buyer is in different EU country than seller
        // 3. Both countries are EU

        var isValid = validation.IsValid &&
                      buyerCountry != sellerCountry &&
                      IsEuCountry(buyerCountry) &&
                      IsEuCountry(sellerCountry);

        _logger.LogDebug(
            "Reverse charge check: Valid={Valid}, BuyerCountry={BuyerCountry}, SellerCountry={SellerCountry}, Result={ShouldApply}",
            validation.IsValid, buyerCountry, sellerCountry, isValid
        );

        return isValid;
    }

    private static bool IsEuCountry(string countryCode)
    {
        // EU/EEA member states (as of 2025)
        var euCountries = new[]
        {
            "AT", "BE", "BG", "HR", "CY", "CZ", "DK", "EE", "FI", "FR",
            "DE", "GR", "HU", "IE", "IT", "LV", "LT", "LU", "MT", "NL",
            "PL", "PT", "RO", "SK", "SI", "ES", "SE"
        };

        return euCountries.Contains(countryCode.ToUpper());
    }
}

/// <summary>
/// Exception for VAT-ID validation failures
/// </summary>
public class VatIdValidationException : Exception
{
    public VatIdValidationException(string message) : base(message) { }

    public VatIdValidationException(string message, Exception innerException)
        : base(message, innerException) { }
}
