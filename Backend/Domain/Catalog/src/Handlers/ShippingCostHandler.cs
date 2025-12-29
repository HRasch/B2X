using B2Connect.Catalog.Models;
using B2Connect.Catalog.Services;
using FluentValidation;

namespace B2Connect.Catalog.Handlers;

/// <summary>
/// Wolverine Service Handler for GetShippingMethodsRequest
/// PAngV Compliance: Shipping costs displayed before checkout
/// </summary>
public class ShippingCostHandler
{
    private readonly ShippingCostService _shippingCostService;
    private readonly ILogger<ShippingCostHandler> _logger;
    private readonly GetShippingMethodsRequestValidator _validator;

    public ShippingCostHandler(
        ShippingCostService shippingCostService,
        ILogger<ShippingCostHandler> logger,
        GetShippingMethodsRequestValidator validator)
    {
        _shippingCostService = shippingCostService;
        _logger = logger;
        _validator = validator;
    }

    /// <summary>
    /// Wolverine Service Handler for POST /api/cart/shipping-methods
    /// </summary>
    public async Task<GetShippingMethodsResponse> GetShippingMethods(
        GetShippingMethodsRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetShippingMethods requested for country: {Country}", request.DestinationCountry);

        try
        {
            // Validate input
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for shipping methods request");
                return new GetShippingMethodsResponse
                {
                    Success = false,
                    Message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)),
                };
            }

            // Get shipping methods
            var response = await _shippingCostService.GetShippingMethodsAsync(
                request.DestinationCountry,
                request.TotalWeight,
                request.OrderTotal,
                cancellationToken);

            _logger.LogInformation("GetShippingMethods completed: {Count} methods returned",
                response.Methods.Count);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetShippingMethods for country: {Country}",
                request.DestinationCountry);

            return new GetShippingMethodsResponse
            {
                Success = false,
                Message = "An error occurred while fetching shipping methods",
            };
        }
    }
}

/// <summary>
/// Validator for GetShippingMethodsRequest
/// </summary>
public class GetShippingMethodsRequestValidator : AbstractValidator<GetShippingMethodsRequest>
{
    public GetShippingMethodsRequestValidator()
    {
        RuleFor(x => x.DestinationCountry)
            .NotEmpty().WithMessage("Destination country is required")
            .Length(2, 3).WithMessage("Country code must be 2-3 characters")
            .Matches(@"^[A-Z]{2,3}$").WithMessage("Country code must contain only letters");

        RuleFor(x => x.TotalWeight)
            .GreaterThan(0).When(x => x.TotalWeight.HasValue)
            .WithMessage("Total weight must be greater than 0");

        RuleFor(x => x.OrderTotal)
            .GreaterThanOrEqualTo(0).When(x => x.OrderTotal.HasValue)
            .WithMessage("Order total cannot be negative");
    }
}
