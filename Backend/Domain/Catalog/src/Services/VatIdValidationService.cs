using System.Text.Json;
using B2Connect.CatalogService.Infrastructure;
using B2Connect.CatalogService.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace B2Connect.CatalogService.Services;

/// <summary>
/// B2B VAT ID validation service with caching and reverse charge logic
/// Issue #31: B2B VAT-ID Validation (AStV Reverse Charge)
/// 
/// Features:
/// - VIES API integration for EU VAT ID validation
/// - 365-day caching of valid VAT IDs (24h for invalid)
/// - Reverse charge determination (0% VAT for valid EU VAT-IDs)
/// - Error handling and safe defaults
/// </summary>
public class VatIdValidationService : IVatIdValidationService
{
    private readonly IViesApiClient _viesClient;
    private readonly IDistributedCache _cache;
    private readonly ILogger<VatIdValidationService> _logger;

    /// <summary>
    /// All 27 EU member states (2025)
    /// </summary>
    private static readonly HashSet<string> EuCountries = new(StringComparer.OrdinalIgnoreCase)
    {
        "AT", "BE", "BG", "HR", "CY", "CZ", "DK", "EE",
        "FI", "FR", "DE", "GR", "HU", "IE", "IT", "LV",
        "LT", "LU", "MT", "NL", "PL", "PT", "RO", "SK",
        "SI", "ES", "SE"
    };

    public VatIdValidationService(
        IViesApiClient viesClient,
        IDistributedCache cache,
        ILogger<VatIdValidationService> logger)
    {
        _viesClient = viesClient ?? throw new ArgumentNullException(nameof(viesClient));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validate VAT ID with 365-day caching
    /// </summary>
    public async Task<VatValidationResult> ValidateVatIdAsync(
        string countryCode,
        string vatNumber,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentException("Country code is required", nameof(countryCode));

        if (string.IsNullOrWhiteSpace(vatNumber))
            throw new ArgumentException("VAT number is required", nameof(vatNumber));

        var cacheKey = $"vat:{countryCode}:{vatNumber}";

        // Check cache first
        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cached))
        {
            _logger.LogInformation("VAT validation cache hit: {CountryCode}{VatNumber}", countryCode, vatNumber);
            return JsonSerializer.Deserialize<VatValidationResult>(cached) 
                ?? throw new InvalidOperationException("Failed to deserialize cached VAT validation");
        }

        _logger.LogInformation("VAT validation cache miss, calling VIES API: {CountryCode}{VatNumber}", 
            countryCode, vatNumber);

        // Call VIES API
        var result = await _viesClient.ValidateVatIdAsync(countryCode, vatNumber, cancellationToken);

        // Cache the result
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(result.IsValid ? 365 : 1)
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), cacheOptions, cancellationToken);

        return result;
    }

    /// <summary>
    /// Determine if reverse charge applies
    /// Conditions for reverse charge:
    /// 1. VAT ID must be valid
    /// 2. Buyer and seller must be in different EU countries
    /// 3. Both must be EU countries
    /// </summary>
    public bool ShouldApplyReverseCharge(
        VatValidationResult validation,
        string buyerCountry,
        string sellerCountry)
    {
        if (string.IsNullOrWhiteSpace(buyerCountry))
            throw new ArgumentException("Buyer country is required", nameof(buyerCountry));

        if (string.IsNullOrWhiteSpace(sellerCountry))
            throw new ArgumentException("Seller country is required", nameof(sellerCountry));

        if (validation == null)
            throw new ArgumentNullException(nameof(validation));

        // Condition 1: VAT ID must be valid
        if (!validation.IsValid)
        {
            _logger.LogInformation("Reverse charge NOT applied: Invalid VAT ID {VatId}", validation.VatId);
            return false;
        }

        // Condition 2: Countries must be different
        if (buyerCountry.Equals(sellerCountry, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Reverse charge NOT applied: Same country ({Country})", buyerCountry);
            return false;
        }

        // Condition 3: Both must be EU countries
        if (!IsEuCountry(buyerCountry) || !IsEuCountry(sellerCountry))
        {
            _logger.LogInformation("Reverse charge NOT applied: Non-EU country. Buyer={Buyer}, Seller={Seller}",
                buyerCountry, sellerCountry);
            return false;
        }

        _logger.LogInformation("Reverse charge APPLIED: Valid VAT ID {VatId}, Buyer={Buyer}, Seller={Seller}",
            validation.VatId, buyerCountry, sellerCountry);
        return true;
    }

    /// <summary>
    /// Check if a country is an EU member state
    /// </summary>
    private static bool IsEuCountry(string? countryCode)
    {
        return !string.IsNullOrWhiteSpace(countryCode) && EuCountries.Contains(countryCode);
    }
}
