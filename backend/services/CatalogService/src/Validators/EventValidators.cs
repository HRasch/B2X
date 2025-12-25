using FluentValidation;
using B2Connect.CatalogService.Events;
using B2Connect.Types;

namespace B2Connect.CatalogService.Validators;

/// <summary>
/// Validator for ProductCreatedEvent
/// Ensures product events have all required data and valid values
/// </summary>
public class ProductCreatedEventValidator : AbstractValidator<ProductCreatedEvent>
{
    public ProductCreatedEventValidator()
    {
        // Base event properties
        RuleFor(e => e.EventId)
            .NotEmpty()
            .WithMessage("Event ID must not be empty");

        RuleFor(e => e.Timestamp)
            .NotEqual(default(DateTime))
            .WithMessage("Timestamp must be set")
            .LessThanOrEqualTo(DateTime.UtcNow.AddSeconds(1))
            .WithMessage("Timestamp cannot be in the future");

        RuleFor(e => e.AggregateId)
            .NotEqual(Guid.Empty)
            .WithMessage("ProductId (AggregateId) must not be empty");

        RuleFor(e => e.AggregateType)
            .NotEmpty()
            .Equal("Product")
            .WithMessage("AggregateType must be 'Product'");

        RuleFor(e => e.EventType)
            .NotEmpty()
            .Equal("product.created")
            .WithMessage("EventType must be 'product.created'");

        // Product-specific properties
        RuleFor(e => e.ProductId)
            .NotEqual(Guid.Empty)
            .WithMessage("ProductId must not be empty");

        RuleFor(e => e.Sku)
            .NotEmpty()
            .WithMessage("SKU must not be empty")
            .Length(3, 50)
            .WithMessage("SKU must be between 3 and 50 characters")
            .Matches(@"^[A-Z0-9\-]+$")
            .WithMessage("SKU must contain only uppercase letters, numbers, and hyphens");

        RuleFor(e => e.Name)
            .NotEmpty()
            .WithMessage("Product name must not be empty")
            .MaximumLength(255)
            .WithMessage("Product name must not exceed 255 characters");

        RuleFor(e => e.Description)
            .MaximumLength(2000)
            .WithMessage("Description must not exceed 2000 characters");

        RuleFor(e => e.Category)
            .NotEmpty()
            .WithMessage("Category must not be empty")
            .MaximumLength(100)
            .WithMessage("Category must not exceed 100 characters");

        RuleFor(e => e.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0")
            .Must(p => decimal.Round(p, 2) == p)
            .WithMessage("Price must have at most 2 decimal places");

        RuleFor(e => e.B2bPrice)
            .Must(p => p == null || p > 0)
            .WithMessage("B2B Price must be null or greater than 0")
            .Must(p => p == null || decimal.Round(p.GetValueOrDefault(), 2) == p.GetValueOrDefault())
            .WithMessage("B2B Price must have at most 2 decimal places")
            .When(e => e.B2bPrice.HasValue);

        RuleFor(e => e.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock quantity must not be negative");

        RuleFor(e => e.Tags)
            .NotNull()
            .WithMessage("Tags array must not be null")
            .Must(tags => tags.Length <= 20)
            .WithMessage("Maximum 20 tags allowed")
            .ForEach(tag => tag
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Each tag must not exceed 50 characters"));

        RuleFor(e => e.ImageUrls)
            .NotNull()
            .WithMessage("ImageUrls array must not be null")
            .Must(urls => urls.Length <= 10)
            .WithMessage("Maximum 10 images allowed")
            .ForEach(url => url
                .NotEmpty()
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("Each image URL must be a valid absolute URI"));

        RuleFor(e => e.Attributes)
            .NotNull()
            .WithMessage("Product attributes must not be null");

        RuleFor(e => e.TenantId)
            .NotEqual(Guid.Empty)
            .WithMessage("TenantId must not be empty");
    }
}

/// <summary>
/// Validator for ProductUpdatedEvent
/// Ensures updated product data is valid
/// </summary>
public class ProductUpdatedEventValidator : AbstractValidator<ProductUpdatedEvent>
{
    public ProductUpdatedEventValidator()
    {
        // Base event properties
        RuleFor(e => e.EventId)
            .NotEmpty()
            .WithMessage("Event ID must not be empty");

        RuleFor(e => e.Timestamp)
            .NotEqual(default(DateTime))
            .LessThanOrEqualTo(DateTime.UtcNow.AddSeconds(1))
            .WithMessage("Timestamp cannot be in the future");

        RuleFor(e => e.AggregateId)
            .NotEqual(Guid.Empty)
            .WithMessage("ProductId (AggregateId) must not be empty");

        RuleFor(e => e.AggregateType)
            .NotEmpty()
            .Equal("Product")
            .WithMessage("AggregateType must be 'Product'");

        RuleFor(e => e.EventType)
            .NotEmpty()
            .Equal("product.updated")
            .WithMessage("EventType must be 'product.updated'");

        // Product-specific properties
        RuleFor(e => e.ProductId)
            .NotEqual(Guid.Empty)
            .WithMessage("ProductId must not be empty");

        RuleFor(e => e.Changes)
            .NotNull()
            .WithMessage("Changes dictionary must not be null")
            .Must(c => c.Count > 0)
            .WithMessage("At least one change must be provided");

        RuleFor(e => e.TenantId)
            .NotEqual(Guid.Empty)
            .WithMessage("TenantId must not be empty");
    }
}

/// <summary>
/// Validator for ProductDeletedEvent
/// </summary>
public class ProductDeletedEventValidator : AbstractValidator<ProductDeletedEvent>
{
    public ProductDeletedEventValidator()
    {
        // Base event properties
        RuleFor(e => e.EventId)
            .NotEmpty()
            .WithMessage("Event ID must not be empty");

        RuleFor(e => e.Timestamp)
            .NotEqual(default(DateTime))
            .LessThanOrEqualTo(DateTime.UtcNow.AddSeconds(1))
            .WithMessage("Timestamp cannot be in the future");

        RuleFor(e => e.AggregateId)
            .NotEqual(Guid.Empty)
            .WithMessage("ProductId (AggregateId) must not be empty");

        RuleFor(e => e.AggregateType)
            .NotEmpty()
            .Equal("Product")
            .WithMessage("AggregateType must be 'Product'");

        RuleFor(e => e.EventType)
            .NotEmpty()
            .Equal("product.deleted")
            .WithMessage("EventType must be 'product.deleted'");

        // Product-specific properties
        RuleFor(e => e.ProductId)
            .NotEqual(Guid.Empty)
            .WithMessage("ProductId must not be empty");

        RuleFor(e => e.TenantId)
            .NotEqual(Guid.Empty)
            .WithMessage("TenantId must not be empty");
    }
}

/// <summary>
/// Validator for ProductsBulkImportedEvent
/// </summary>
public class ProductsBulkImportedEventValidator : AbstractValidator<ProductsBulkImportedEvent>
{
    public ProductsBulkImportedEventValidator()
    {
        // Base event properties
        RuleFor(e => e.EventId)
            .NotEmpty()
            .WithMessage("Event ID must not be empty");

        RuleFor(e => e.Timestamp)
            .NotEqual(default(DateTime))
            .LessThanOrEqualTo(DateTime.UtcNow.AddSeconds(1))
            .WithMessage("Timestamp cannot be in the future");

        RuleFor(e => e.AggregateType)
            .NotEmpty()
            .Equal("Product")
            .WithMessage("AggregateType must be 'Product'");

        RuleFor(e => e.EventType)
            .NotEmpty()
            .Equal("products.bulk-imported")
            .WithMessage("EventType must be 'products.bulk-imported'");

        // Bulk-specific properties
        RuleFor(e => e.ProductIds)
            .NotNull()
            .WithMessage("ProductIds array must not be null")
            .Must(ids => ids.Length > 0)
            .WithMessage("At least one product ID must be provided")
            .ForEach(id => id
                .NotEqual(Guid.Empty)
                .WithMessage("Product IDs must not be empty"));

        RuleFor(e => e.TotalCount)
            .GreaterThan(0)
            .WithMessage("Total count must be greater than 0")
            .Equal(e => e.ProductIds.Length)
            .WithMessage("TotalCount must match the number of ProductIds");

        RuleFor(e => e.TenantId)
            .NotEqual(Guid.Empty)
            .WithMessage("TenantId must not be empty");
    }
}
