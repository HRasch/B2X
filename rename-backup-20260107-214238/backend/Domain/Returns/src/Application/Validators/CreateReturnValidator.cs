using FluentValidation;
using B2X.Returns.Application.Commands;

namespace B2X.Returns.Application.Validators;

/// <summary>
/// Validator for CreateReturnCommand (14-day VVVG withdrawal request).
/// 
/// Key Rules:
/// - Return must be within 14 days from delivery (not order) date
/// - Customer can only return from their own orders
/// - Items count must match order
/// - Reason must be provided
/// </summary>
public class CreateReturnValidator : AbstractValidator<CreateReturnCommand>
{
    public CreateReturnValidator()
    {
        // Order & Customer validation
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        // Return details
        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Return reason is required")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters");

        RuleFor(x => x.ItemsCount)
            .GreaterThan(0)
            .WithMessage("At least 1 item must be returned")
            .LessThanOrEqualTo(1000)
            .WithMessage("Items count cannot exceed 1000");

        // CRITICAL: 14-day deadline validation
        RuleFor(x => x.DeliveryDate)
            .NotEmpty()
            .WithMessage("Delivery date is required")
            .LessThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Delivery date cannot be in the future");

        // Custom validator: Must be within 14 days from delivery date
        RuleFor(x => x.DeliveryDate)
            .Custom((deliveryDate, context) =>
            {
                var deadline = deliveryDate.AddDays(14);
                var daysRemaining = (deadline.Date - DateTime.UtcNow.Date).TotalDays;

                if (daysRemaining < 0)
                {
                    context.AddFailure(
                        "DeliveryDate",
                        $"Return period expired on {deadline:yyyy-MM-dd}. " +
                        $"VVVG requires returns within 14 days of delivery date."
                    );
                }
            });

        // Refund amount validation
        RuleFor(x => x.RefundAmount)
            .GreaterThan(0)
            .WithMessage("Refund amount must be greater than 0")
            .LessThanOrEqualTo(99999.99m)
            .WithMessage("Refund amount exceeds maximum allowed");

        RuleFor(x => x.OriginalOrderAmount)
            .GreaterThan(0)
            .WithMessage("Original order amount must be greater than 0")
            .GreaterThanOrEqualTo(x => x.RefundAmount)
            .WithMessage("Refund amount cannot exceed original order amount");

        // Shipping deduction validation
        RuleFor(x => x.ShippingDeduction)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Shipping deduction cannot be negative")
            .LessThanOrEqualTo(x => x.RefundAmount)
            .WithMessage("Shipping deduction cannot exceed refund amount");

        // Tenant validation
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID is required (multi-tenant isolation)");

        RuleFor(x => x.CreatedBy)
            .NotEmpty()
            .WithMessage("Creator ID is required (audit trail)");
    }
}
