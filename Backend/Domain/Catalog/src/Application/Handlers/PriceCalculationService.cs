
using B2Connect.Catalog.Application.Validators;
using B2Connect.Catalog.Core.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace B2Connect.Catalog.Application.Handlers;
/// <summary>
/// Wolverine Service Handler for price calculations with VAT transparency
/// Pattern: Service class + public async methods = automatic HTTP endpoints
/// Issue #30: B2C Price Transparency (PAngV)
///
/// Wolverine auto-discovers this as:
///   POST /api/catalog/calculateprice
///   POST /api/catalog/getpricebreakdown
/// </summary>
public class PriceCalculationService
{
    private readonly ITaxRateService _taxService;
    private readonly ILogger<PriceCalculationService> _logger;
    private readonly CalculatePriceValidator _validator;

    public PriceCalculationService(
        ITaxRateService taxService,
        ILogger<PriceCalculationService> logger,
        CalculatePriceValidator validator)
    {
        _taxService = taxService;
        _logger = logger;
        _validator = validator;
    }

    /// <summary>
    /// Wolverine HTTP Handler: Calculate price with VAT breakdown
    /// POST /api/catalog/calculateprice
    /// </summary>
    public async Task<PriceBreakdownResponse> CalculatePrice(
        CalculatePriceCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Price calculation: {Price}€ for {Country}",
            request.ProductPrice, request.DestinationCountry ?? "(none)");

        try
        {
            // 1. Validate input
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Price validation failed: {Errors}",
                    string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

                return new PriceBreakdownResponse
                {
                    Success = false,
                    Error = "VALIDATION_ERROR",
                    Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }

            // 2. Get VAT rate for destination country (use fallback if null)
            var destination = request.DestinationCountry ?? "DE";
            var vatRate = await _taxService.GetVatRateAsync(destination, cancellationToken);

            // 3. Calculate VAT amount
            var vatAmount = request.ProductPrice * (vatRate / 100);
            var totalWithVat = request.ProductPrice + vatAmount;

            // 4. Build response with transparent breakdown
            var response = new PriceBreakdownResponse
            {
                Success = true,
                Breakdown = new PriceBreakdown(
                    ProductPrice: request.ProductPrice,
                    VatRate: vatRate,
                    VatAmount: Math.Round(vatAmount, 2),
                    TotalWithVat: Math.Round(totalWithVat, 2),
                    CurrencyCode: request.CurrencyCode ?? "EUR",
                    ShippingCost: request.ShippingCost ?? 0,
                    FinalTotal: Math.Round(totalWithVat + (request.ShippingCost ?? 0), 2),
                    DestinationCountry: destination
                )
            };

            _logger.LogInformation(
                "Price calculated successfully: {ProductPrice}€ + {VatAmount}€ VAT = {Total}€",
                request.ProductPrice, vatAmount, totalWithVat);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating price for {Country}", request.DestinationCountry ?? "(none)");
            return new PriceBreakdownResponse
            {
                Success = false,
                Error = "CALCULATION_ERROR",
                Message = "An error occurred while calculating price. Please try again."
            };
        }
    }

    /// <summary>
    /// Get price breakdown details (used by frontend for display)
    /// POST /api/catalog/getpricebreakdown
    /// </summary>
    public Task<PriceBreakdownResponse> GetPriceBreakdown(
        GetPriceBreakdownQuery query,
        CancellationToken cancellationToken)
    {
        return CalculatePrice(
            new CalculatePriceCommand(
                query.ProductPrice,
                query.DestinationCountry,
                query.ShippingCost,
                "EUR"),
            cancellationToken);
    }
}

// Command/Query DTOs
public record CalculatePriceCommand(
    decimal ProductPrice,
    string? DestinationCountry,
    decimal? ShippingCost = null,
    string? CurrencyCode = "EUR");

public record GetPriceBreakdownQuery(
    decimal ProductPrice,
    string? DestinationCountry,
    decimal? ShippingCost = null);

public record PriceBreakdown(
    decimal ProductPrice,
    decimal VatRate,
    decimal VatAmount,
    decimal TotalWithVat,
    string? CurrencyCode = "EUR",
    decimal ShippingCost = 0,
    decimal FinalTotal = 0,
    string DestinationCountry = "DE");

public record PriceBreakdownResponse(
    bool Success = false,
    PriceBreakdown? Breakdown = null,
    string? Error = null,
    string? Message = null);
