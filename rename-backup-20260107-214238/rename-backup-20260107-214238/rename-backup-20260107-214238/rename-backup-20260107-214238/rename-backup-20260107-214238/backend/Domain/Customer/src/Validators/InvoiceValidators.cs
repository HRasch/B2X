using B2Connect.Customer.Models;
using FluentValidation;

namespace B2Connect.Customer.Validators;

/// <summary>
/// Validator for GenerateInvoiceCommand
/// Issue #32: Invoice Modification for Reverse Charge
/// </summary>
public class GenerateInvoiceCommandValidator : AbstractValidator<GenerateInvoiceCommand>
{
    public GenerateInvoiceCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required");
    }
}

/// <summary>
/// Validator for ModifyInvoiceCommand
/// </summary>
public class ModifyInvoiceCommandValidator : AbstractValidator<ModifyInvoiceCommand>
{
    public ModifyInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceId)
            .NotEmpty().WithMessage("InvoiceId is required");

        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required");

        When(x => x.ApplyReverseCharge, () =>
        {
            RuleFor(x => x.BuyerVatId)
                .NotEmpty().WithMessage("BuyerVatId is required when applying reverse charge")
                .Matches("^[A-Z]{2}[A-Z0-9]+$").WithMessage("Invalid VAT ID format");

            RuleFor(x => x.BuyerCountry)
                .NotEmpty().WithMessage("BuyerCountry is required when applying reverse charge")
                .Length(2).WithMessage("BuyerCountry must be 2-letter ISO code");
        });
    }
}
