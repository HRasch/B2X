
using B2X.Catalog.Application.Validators;
using B2X.Catalog.Core.Interfaces;
using B2X.Shared.Core.Handlers;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace B2X.Catalog.Application.Handlers;
/// <summary>
/// Wolverine Service Handler for price calculations with VAT transparency
/// Pattern: Service class + public async methods = automatic HTTP endpoints
/// Issue #30: B2C Price Transparency (PAngV)
///
/// Wolverine auto-discovers this as:
///   POST /api/catalog/calculateprice
///   POST /api/catalog/getpricebreakdown
/// </summary>
public class PriceCalculationService : ValidatedBase
{
    private readonly ITaxRateService _taxService;
    private readonly CalculatePriceValidator _validator;

    public PriceCalculationService(
        ITaxRateService taxService,
        ILogger<PriceCalculationService> logger,
        CalculatePriceValidator validator)
        : base(logger)
    {
        _taxService = taxService;
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
        Logger.LogInformation(
            "Price calculation: {Price}€ for {Country}",
            request.ProductPrice, request.DestinationCountry ?? "(none)");

        try
        {
            // Validate input
            var validationError = await ValidateRequestAsync(
                request,
                _validator,
                cancellationToken,
                errorMessage => CreateValidationErrorResponse(errorMessage)).ConfigureAwait(false);

            if (validationError != null)
            {
                return validationError;
            }

            // Calculate price with VAT
            var destination = request.DestinationCountry ?? "DE";
            var vatRate = await _taxService.GetVatRateAsync(destination, cancellationToken).ConfigureAwait(false);

            var breakdown = CalculatePriceBreakdown(request, vatRate, destination);

            return new PriceBreakdownResponse
            {
                Success = true,
                Breakdown = breakdown
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error calculating price for {Country}", request.DestinationCountry ?? "(none)");
            return CreateCalculationErrorResponse();
        }
    }

    private PriceBreakdownResponse CreateValidationErrorResponse(string errorMessage)
    {
        Logger.LogWarning("Price validation failed: {Errors}", errorMessage);

        return new PriceBreakdownResponse
        {
            Success = false,
            Error = "VALIDATION_ERROR",
            Message = errorMessage
        };
    }

    private PriceBreakdown CalculatePriceBreakdown(CalculatePriceCommand request, decimal vatRate, string destination)
    {
        var vatAmount = request.ProductPrice * (vatRate / 100);
        var totalWithVat = request.ProductPrice + vatAmount;
        var shippingCost = request.ShippingCost ?? 0;
        var finalTotal = totalWithVat + shippingCost;

        Logger.LogInformation(
            "Price calculated successfully: {ProductPrice}€ + {VatAmount}€ VAT = {Total}€",
            request.ProductPrice, vatAmount, totalWithVat);

        return new PriceBreakdown(
            ProductPrice: request.ProductPrice,
            VatRate: vatRate,
            VatAmount: Math.Round(vatAmount, 2),
            TotalWithVat: Math.Round(totalWithVat, 2),
            CurrencyCode: request.CurrencyCode ?? "EUR",
            ShippingCost: shippingCost,
            FinalTotal: Math.Round(finalTotal, 2),
            DestinationCountry: destination
        );
    }

    private static PriceBreakdownResponse CreateCalculationErrorResponse()
    {
        return new PriceBreakdownResponse
        {
            Success = false,
            Error = "CALCULATION_ERROR",
            Message = "An error occurred while calculating price. Please try again."
        };
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
