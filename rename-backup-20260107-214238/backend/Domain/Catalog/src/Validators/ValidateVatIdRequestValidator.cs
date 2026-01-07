using B2X.Catalog.Models;
using FluentValidation;

namespace B2X.Catalog.Validators;

/// <summary>
/// Validation rules for VAT ID validation requests
/// Issue #31: B2B VAT-ID Validation (AStV Reverse Charge)
/// </summary>
public class ValidateVatIdRequestValidator : AbstractValidator<ValidateVatIdRequest>
{
    public ValidateVatIdRequestValidator()
    {
        // Country code: exactly 2 uppercase letters
        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .WithMessage("Country code is required")
            .Length(2)
            .WithMessage("Country code must be exactly 2 characters")
            .Matches("^[A-Z]{2}$")
            .WithMessage("Country code must be 2 uppercase letters (e.g., 'DE', 'AT', 'FR')");

        // VAT number: 1-17 alphanumeric characters (EU standard)
        RuleFor(x => x.VatNumber)
            .NotEmpty()
            .WithMessage("VAT number is required")
            .Length(1, 17)
            .WithMessage("VAT number must be 1-17 characters")
            .Matches("^[A-Z0-9]*$")
            .WithMessage("VAT number can only contain uppercase letters and digits");

        // Optional: Buyer country validation
        RuleFor(x => x.BuyerCountry)
            .Length(2)
            .When(x => !string.IsNullOrWhiteSpace(x.BuyerCountry))
            .WithMessage("Buyer country, if provided, must be exactly 2 characters")
            .Matches("^[A-Z]{2}$")
            .When(x => !string.IsNullOrWhiteSpace(x.BuyerCountry))
            .WithMessage("Buyer country must be 2 uppercase letters");

        // Optional: Seller country validation
        RuleFor(x => x.SellerCountry)
            .Length(2)
            .When(x => !string.IsNullOrWhiteSpace(x.SellerCountry))
            .WithMessage("Seller country, if provided, must be exactly 2 characters")
            .Matches("^[A-Z]{2}$")
            .When(x => !string.IsNullOrWhiteSpace(x.SellerCountry))
            .WithMessage("Seller country must be 2 uppercase letters");
    }
}
