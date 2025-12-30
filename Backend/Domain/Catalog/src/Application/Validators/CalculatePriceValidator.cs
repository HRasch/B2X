namespace B2Connect.Catalog.Application.Validators;

using B2Connect.Catalog.Application.Handlers;
using FluentValidation;

public class CalculatePriceValidator : AbstractValidator<CalculatePriceCommand>
{
    public CalculatePriceValidator()
    {
        RuleFor(x => x.ProductPrice)
            .GreaterThan(0)
            .WithMessage("Product price must be greater than 0");

        RuleFor(x => x.DestinationCountry)
            .NotEmpty()
            .WithMessage("Destination country is required")
            .Length(2)
            .WithMessage("Country code must be 2 characters (e.g., DE, AT, FR)");

        RuleFor(x => x.ShippingCost)
            .GreaterThanOrEqualTo(0)
            .When(x => x.ShippingCost.HasValue)
            .WithMessage("Shipping cost cannot be negative");

        RuleFor(x => x.CurrencyCode)
            .NotEmpty()
            .WithMessage("Currency code is required")
            .Length(3)
            .WithMessage("Currency code must be 3 characters (e.g., EUR)");
    }
}
