using B2X.Catalog.Models;
using B2X.Catalog.Services;
using B2X.Shared.Core.Handlers;
using FluentValidation;

namespace B2X.Catalog.Handlers;

/// <summary>
/// Wolverine Service Handler for GetShippingMethodsRequest
/// PAngV Compliance: Shipping costs displayed before checkout
/// </summary>
public class ShippingCostHandler : ValidatedHandlerBase
{
    private readonly ShippingCostService _shippingCostService;
    private readonly GetShippingMethodsRequestValidator _validator;

    public ShippingCostHandler(
        ShippingCostService shippingCostService,
        ILogger<ShippingCostHandler> logger,
        GetShippingMethodsRequestValidator validator)
        : base(logger)
    {
        _shippingCostService = shippingCostService;
        _validator = validator;
    }

    /// <summary>
    /// Wolverine Service Handler for POST /api/cart/shipping-methods
    /// </summary>
    public async Task<GetShippingMethodsResponse> GetShippingMethods(
        GetShippingMethodsRequest request,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("GetShippingMethods requested for country: {Country}", request.DestinationCountry);

        try
        {
            // Validate input
            var validationError = await ValidateRequestAsync(
                request,
                _validator,
                cancellationToken,
                errorMessage => new GetShippingMethodsResponse
                {
                    Success = false,
                    Message = errorMessage,
                });

            if (validationError != null)
            {
                Logger.LogWarning("Validation failed for shipping methods request");
                return validationError;
            }

            // Get shipping methods
            var response = await _shippingCostService.GetShippingMethodsAsync(
                request.DestinationCountry,
                request.TotalWeight,
                request.OrderTotal,
                cancellationToken).ConfigureAwait(false);

            Logger.LogInformation("GetShippingMethods completed: {Count} methods returned",
                response.Methods.Count);

            return response;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in GetShippingMethods for country: {Country}",
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
            .Matches("^[A-Z]{2,3}$").WithMessage("Country code must contain only letters");

        RuleFor(x => x.TotalWeight)
            .GreaterThan(0).When(x => x.TotalWeight.HasValue)
            .WithMessage("Total weight must be greater than 0");

        RuleFor(x => x.OrderTotal)
            .GreaterThanOrEqualTo(0).When(x => x.OrderTotal.HasValue)
            .WithMessage("Order total cannot be negative");
    }
}
