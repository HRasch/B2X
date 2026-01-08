using B2X.Catalog.Models;
using B2X.Catalog.Services;

namespace B2X.Catalog.Handlers;

/// <summary>
/// Wolverine HTTP Handler for Price Calculation
///
/// Wolverine automatically generates HTTP endpoints from public async methods:
/// - Method name → HTTP route (POST /calculateprice)
/// - First parameter → request body
/// - Injected dependencies → resolved from DI container
/// - CancellationToken → automatic
///
/// Reference: Issue #30 (B2C Price Transparency)
/// </summary>
public class PriceCalculationHandler
{
    private readonly IPriceCalculationService _priceService;
    private readonly ILogger<PriceCalculationHandler> _logger;

    public PriceCalculationHandler(
        IPriceCalculationService priceService,
        ILogger<PriceCalculationHandler> logger)
    {
        _priceService = priceService;
        _logger = logger;
    }

    /// <summary>
    /// Calculate final price with VAT breakdown
    ///
    /// Wolverine Endpoint: POST /calculateprice
    ///
    /// Request:
    /// {
    ///   "basePrice": 99.99,
    ///   "destinationCountry": "DE",
    ///   "discountPercentage": 10.0
    /// }
    ///
    /// Response: PriceBreakdownDto
    /// {
    ///   "productPrice": 99.99,
    ///   "vatRate": 0.19,
    ///   "vatAmount": 19.00,
    ///   "priceIncludingVat": 118.99,
    ///   "discountPercentage": 10.0,
    ///   "discountAmount": 11.90,
    ///   "finalPrice": 107.09,
    ///   "currencyCode": "EUR",
    ///   "isB2bReverseCharge": false,
    ///   "countryCode": "DE",
    ///   "requiresAgeVerification": false
    /// }
    /// </summary>
    public async Task<PriceBreakdownDto> CalculatePrice(
        CalculatePriceRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Calculating price for {BasePrice} in {Country} with {Discount}% discount",
            request.BasePrice,
            request.DestinationCountry,
            request.DiscountPercentage ?? 0);

        try
        {
            var result = await _priceService.CalculatePriceAsync(
                request.BasePrice,
                request.DestinationCountry,
                request.DiscountPercentage,
                cancellationToken).ConfigureAwait(false);

            _logger.LogInformation(
                "Price calculation successful: {ProductPrice} → {FinalPrice} EUR",
                result.ProductPrice,
                result.FinalPrice);

            return result;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Price calculation validation error");
            throw new ArgumentException("Invalid price calculation parameters", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during price calculation");
            throw new InvalidOperationException("Price calculation failed", ex);
        }
    }
}

/// <summary>
/// Request model for price calculation
/// Wolverine automatically binds this from request body
/// </summary>
public class CalculatePriceRequest
{
    /// <summary>Base product price (before VAT)</summary>
    public decimal BasePrice { get; set; }

    /// <summary>Destination country (ISO 3166 alpha-2, e.g., "DE", "AT", "FR")</summary>
    public string DestinationCountry { get; set; } = string.Empty;

    /// <summary>Optional discount percentage (0-100)</summary>
    public decimal? DiscountPercentage { get; set; }
}
