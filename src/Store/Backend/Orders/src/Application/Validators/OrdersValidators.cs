using B2X.Orders.Application.Commands;
using FluentValidation;

namespace B2X.Orders.Application.Validators;

/// <summary>
/// Validators for order commands
/// </summary>
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("Customer email is required")
            .EmailAddress().WithMessage("Customer email must be a valid email address");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must contain at least one item");

        RuleForEach(x => x.Items).ChildRules(items =>
        {
            items.RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product ID is required");

            items.RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name is required");

            items.RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0");

            items.RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Unit price must be greater than or equal to 0");
        });

        RuleFor(x => x.ShippingAddress)
            .NotNull().WithMessage("Shipping address is required");

        When(x => x.ShippingAddress != null, () =>
        {
            RuleFor(x => x.ShippingAddress!.FirstName)
                .NotEmpty().WithMessage("Shipping first name is required");

            RuleFor(x => x.ShippingAddress!.LastName)
                .NotEmpty().WithMessage("Shipping last name is required");

            RuleFor(x => x.ShippingAddress!.Street)
                .NotEmpty().WithMessage("Shipping street is required");

            RuleFor(x => x.ShippingAddress!.City)
                .NotEmpty().WithMessage("Shipping city is required");

            RuleFor(x => x.ShippingAddress!.PostalCode)
                .NotEmpty().WithMessage("Shipping postal code is required");

            RuleFor(x => x.ShippingAddress!.Country)
                .NotEmpty().WithMessage("Shipping country is required");
        });

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be 3 characters");

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("Payment method is required");
    }
}

public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    private static readonly string[] ValidStatuses = ["pending", "confirmed", "processing", "shipped", "delivered", "cancelled"];

    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Order ID is required");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(status => ValidStatuses.Contains(status))
            .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}");
    }
}

public class UpdateOrderPaymentStatusCommandValidator : AbstractValidator<UpdateOrderPaymentStatusCommand>
{
    private static readonly string[] ValidPaymentStatuses = ["pending", "paid", "failed", "refunded"];

    public UpdateOrderPaymentStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Order ID is required");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.PaymentStatus)
            .NotEmpty().WithMessage("Payment status is required")
            .Must(status => ValidPaymentStatuses.Contains(status))
            .WithMessage($"Payment status must be one of: {string.Join(", ", ValidPaymentStatuses)}");
    }
}

public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Order ID is required");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Cancellation reason is required")
            .MaximumLength(500).WithMessage("Cancellation reason must not exceed 500 characters");
    }
}

public class AddOrderItemCommandValidator : AbstractValidator<AddOrderItemCommand>
{
    public AddOrderItemCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.Item)
            .NotNull().WithMessage("Order item is required");

        When(x => x.Item != null, () =>
        {
            RuleFor(x => x.Item!.ProductId)
                .NotEmpty().WithMessage("Product ID is required");

            RuleFor(x => x.Item!.ProductName)
                .NotEmpty().WithMessage("Product name is required");

            RuleFor(x => x.Item!.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0");

            RuleFor(x => x.Item!.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Unit price must be greater than or equal to 0");
        });
    }
}

public class UpdateOrderItemCommandValidator : AbstractValidator<UpdateOrderItemCommand>
{
    public UpdateOrderItemCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required");

        RuleFor(x => x.OrderItemId)
            .NotEmpty().WithMessage("Order item ID is required");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Unit price must be greater than or equal to 0");
    }
}

public class RemoveOrderItemCommandValidator : AbstractValidator<RemoveOrderItemCommand>
{
    public RemoveOrderItemCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required");

        RuleFor(x => x.OrderItemId)
            .NotEmpty().WithMessage("Order item ID is required");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required");
    }
}
