
using B2X.Catalog.Application.Handlers;
using B2X.Catalog.Core.Interfaces;
using Wolverine.Http;

namespace B2X.Catalog.Endpoints;
/// <summary>
/// Wolverine HTTP Endpoints for Price Calculation
/// Issue #30: B2C Price Transparency (PAngV)
///
/// Endpoints are auto-discovered by Wolverine:
///   POST /api/calculateprice
///   POST /api/getpricebreakdown
/// </summary>
public class PriceEndpoints
{
    private readonly PriceCalculationService _priceService;
    private readonly ITaxRateService _taxService;

    public PriceEndpoints(PriceCalculationService priceService, ITaxRateService taxService)
    {
        _priceService = priceService;
        _taxService = taxService;
    }

    /// <summary>
    /// Calculate price with VAT breakdown
    /// POST /api/calculateprice
    /// </summary>
    [WolverinePost("/api/calculateprice")]
    public Task<PriceBreakdownResponse> CalculatePrice(
        CalculatePriceCommand request,
        CancellationToken cancellationToken)
    {
        return _priceService.CalculatePrice(request, cancellationToken);
    }

    /// <summary>
    /// Get price breakdown details
    /// POST /api/getpricebreakdown
    /// </summary>
    [WolverinePost("/api/getpricebreakdown")]
    public Task<PriceBreakdownResponse> GetPriceBreakdown(
        GetPriceBreakdownQuery query,
        CancellationToken cancellationToken)
    {
        return _priceService.GetPriceBreakdown(query, cancellationToken);
    }

    /// <summary>
    /// Get all active tax rates for frontend
    /// GET /api/taxrates
    /// </summary>
    [WolverineGet("/api/taxrates")]
    public Task<IEnumerable<TaxRateDto>> GetTaxRates(CancellationToken cancellationToken)
    {
        return _taxService.GetAllRatesAsync(cancellationToken);
    }

    /// <summary>
    /// Get VAT rate for specific country
    /// GET /api/taxrates/{countryCode}
    /// </summary>
    [WolverineGet("/api/taxrates/{countryCode}")]
    public Task<decimal> GetVatRate(string countryCode, CancellationToken cancellationToken)
    {
        return _taxService.GetVatRateAsync(countryCode, cancellationToken);
    }
}
