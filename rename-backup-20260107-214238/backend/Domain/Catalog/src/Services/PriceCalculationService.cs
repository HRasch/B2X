using B2X.Catalog.Models;
using Microsoft.Extensions.Logging;

namespace B2X.Catalog.Services;

/// <summary>
/// Service for calculating product prices with VAT transparency
/// Issue #30: B2C Price Transparency (PAngV Compliance)
///
/// PAngV (Preisangaben-Verordnung) Compliance:
/// - All prices shown to customers MUST include VAT (Brutto)
/// - VAT breakdown must be transparent
/// - Shipping costs shown before checkout
/// </summary>
public interface IPriceCalculationService
{
    /// <summary>
    /// Calculate final price with VAT breakdown
    /// </summary>
    Task<PriceBreakdownDto> CalculatePriceAsync(
        decimal basePrice,
        string destinationCountry,
        decimal? discountPercentage = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get VAT rate for a specific country
    /// </summary>
    Task<decimal> GetVatRateAsync(
        string countryCode,
        CancellationToken cancellationToken = default);
}

public class PriceCalculationService : IPriceCalculationService
{
    private readonly ILogger<PriceCalculationService> _logger;

    // VAT rates per EU country (as of 2025)
    // Note: This should eventually come from database for easy updates
    private static readonly Dictionary<string, (decimal Standard, decimal? Reduced)> VatRates = new(StringComparer.Ordinal)
    {
        // EU Countries - Standard VAT rates
        { "AT", (20.00m, 10.00m) },    // Austria: 20% standard, 10% reduced
        { "BE", (21.00m, 6.00m) },     // Belgium: 21% standard, 6% reduced
        { "BG", (20.00m, 9.00m) },     // Bulgaria: 20% standard, 9% reduced
        { "HR", (25.00m, 5.00m) },     // Croatia: 25% standard, 5% reduced
        { "CY", (19.00m, 5.00m) },     // Cyprus: 19% standard, 5% reduced
        { "CZ", (21.00m, 15.00m) },    // Czech Republic: 21% standard, 15% reduced
        { "DK", (25.00m, null) },      // Denmark: 25% standard, no reduced
        { "DE", (19.00m, 7.00m) },     // Germany: 19% standard, 7% reduced (books, food)
        { "EE", (20.00m, 9.00m) },     // Estonia: 20% standard, 9% reduced
        { "ES", (21.00m, 10.00m) },    // Spain: 21% standard, 10% reduced
        { "FR", (20.00m, 5.50m) },     // France: 20% standard, 5.5% reduced
        { "GR", (24.00m, 13.00m) },    // Greece: 24% standard, 13% reduced
        { "HU", (27.00m, 5.00m) },     // Hungary: 27% standard, 5% reduced
        { "IE", (23.00m, 13.50m) },    // Ireland: 23% standard, 13.5% reduced
        { "IT", (22.00m, 10.00m) },    // Italy: 22% standard, 10% reduced
        { "LV", (21.00m, 12.00m) },    // Latvia: 21% standard, 12% reduced
        { "LT", (21.00m, 5.00m) },     // Lithuania: 21% standard, 5% reduced
        { "LU", (17.00m, 8.00m) },     // Luxembourg: 17% standard, 8% reduced
        { "MT", (18.00m, 7.00m) },     // Malta: 18% standard, 7% reduced
        { "NL", (21.00m, 9.00m) },     // Netherlands: 21% standard, 9% reduced
        { "PL", (23.00m, 8.00m) },     // Poland: 23% standard, 8% reduced
        { "PT", (23.00m, 13.00m) },    // Portugal: 23% standard, 13% reduced
        { "RO", (19.00m, 9.00m) },     // Romania: 19% standard, 9% reduced
        { "SK", (20.00m, 10.00m) },    // Slovakia: 20% standard, 10% reduced
        { "SI", (22.00m, 9.50m) },     // Slovenia: 22% standard, 9.5% reduced
        { "SE", (25.00m, 12.00m) },    // Sweden: 25% standard, 12% reduced
        { "CH", (8.10m, 2.50m) },      // Switzerland: 8.1% standard (not EU but relevant), 2.5% reduced
    };

    public PriceCalculationService(ILogger<PriceCalculationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Calculate final price with VAT breakdown
    /// </summary>
    public async Task<PriceBreakdownDto> CalculatePriceAsync(
        decimal basePrice,
        string destinationCountry,
        decimal? discountPercentage = null,
        CancellationToken cancellationToken = default)
    {
        if (basePrice < 0)
        {
            throw new ArgumentException("Price cannot be negative", nameof(basePrice));
        }

        if (string.IsNullOrWhiteSpace(destinationCountry))
        {
            throw new ArgumentException("Country code required", nameof(destinationCountry));
        }

        if (discountPercentage is < 0 or > 100)
        {
            throw new ArgumentException("Discount must be between 0-100%", nameof(discountPercentage));
        }

        // Get VAT rate for destination country
        var vatRate = await GetVatRateAsync(
            destinationCountry.ToUpper(System.Globalization.CultureInfo.CurrentCulture),
            cancellationToken
        ).ConfigureAwait(false);

#pragma warning disable CA1848 // Use the LoggerMessage delegates
        _logger.LogDebug(
            "Calculating price - Base: {BasePrice}, Country: {Country}, VAT Rate: {VatRate}%",
            basePrice, destinationCountry, vatRate
        );
#pragma warning restore CA1848 // Use the LoggerMessage delegates

        // Calculate VAT amount
        var vatAmount = Math.Round(basePrice * vatRate / 100, 2);

        // Price including VAT
        var priceIncludingVat = Math.Round(basePrice + vatAmount, 2);

        // Apply discount if specified
        var (discountAmount, finalPrice, originalPrice) = CalculateDiscount(priceIncludingVat, discountPercentage);

        var result = new PriceBreakdownDto
        {
            ProductPrice = basePrice,
            VatRate = vatRate,
            VatAmount = vatAmount,
            PriceIncludingVat = priceIncludingVat,
            DiscountAmount = discountAmount,
            FinalPrice = finalPrice,
            OriginalPrice = originalPrice,
            CurrencyCode = "EUR",
            DestinationCountry = destinationCountry.ToUpper(System.Globalization.CultureInfo.CurrentCulture)
        };

        _logger.LogInformation(
            "Price calculated - Final: {FinalPrice} (incl. {VatAmount} VAT at {VatRate}%)",
            finalPrice ?? priceIncludingVat,
            vatAmount,
            vatRate
        );

        return await Task.FromResult(result).ConfigureAwait(false);
    }

    private (decimal? DiscountAmount, decimal? FinalPrice, decimal? OriginalPrice) CalculateDiscount(decimal priceIncludingVat, decimal? discountPercentage)
    {
        if (discountPercentage.HasValue && discountPercentage > 0)
        {
            var discountAmount = Math.Round(priceIncludingVat * discountPercentage.Value / 100, 2);
            var finalPrice = Math.Round(priceIncludingVat - discountAmount, 2);
            var originalPrice = priceIncludingVat;

            _logger.LogDebug(
                "Applied discount - Original: {Original}, Discount: {Discount}%, Amount: {Amount}, Final: {Final}",
                originalPrice, discountPercentage, discountAmount, finalPrice
            );

            return (discountAmount, finalPrice, originalPrice);
        }

        return (null, priceIncludingVat, null);
    }

    /// <summary>
    /// Get VAT rate for a specific country
    /// </summary>
    public async Task<decimal> GetVatRateAsync(
        string countryCode,
        CancellationToken cancellationToken = default)
    {
        var code = countryCode.ToUpper(System.Globalization.CultureInfo.CurrentCulture);

        if (!VatRates.TryGetValue(code, out var rates))
        {
            _logger.LogWarning(
                "VAT rate not found for country {Country}, defaulting to 19% (Germany)",
                code
            );
            // Default to German VAT rate for safety
            return 19.00m;
        }

        return await Task.FromResult(rates.Standard).ConfigureAwait(false);
    }
}
