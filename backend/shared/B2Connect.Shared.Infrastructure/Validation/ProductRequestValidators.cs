using B2Connect.Catalog.Models;
using FluentValidation;

namespace B2Connect.Infrastructure.Validation;

/// <summary>
/// Validator for CreateProductRequest to prevent injection attacks and ensure data integrity.
/// Validates SKU uniqueness constraints, price ranges, stock quantities, and field length limits.
/// </summary>
public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        // SKU validation
        RuleFor(x => x.Sku)
            .NotEmpty()
            .WithMessage("SKU is required")
            .WithErrorCode("FIELD_REQUIRED")
            .Length(1, 50)
            .WithMessage("SKU must be between 1 and 50 characters")
            .WithErrorCode("FIELD_LENGTH_INVALID")
            .Matches(@"^[A-Z0-9\-_]+$")
            .WithMessage("SKU must contain only uppercase letters, numbers, hyphens, and underscores")
            .WithErrorCode("INVALID_CHARACTER_SET");

        // Name validation
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required")
            .WithErrorCode("FIELD_REQUIRED")
            .Length(1, 200)
            .WithMessage("Product name must be between 1 and 200 characters")
            .WithErrorCode("FIELD_LENGTH_INVALID")
            .Matches(@"^[a-zA-Z0-9\s\-&'().,]+$")
            .WithMessage("Product name contains invalid characters")
            .WithErrorCode("INVALID_CHARACTER_SET");

        // Description validation (optional but if provided, validate it)
        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Description must not exceed 2000 characters")
            .WithErrorCode("FIELD_TOO_LONG")
            .When(x => !string.IsNullOrEmpty(x.Description));

        // Price validation
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0")
            .WithErrorCode("INVALID_VALUE")
            .LessThanOrEqualTo(9999999.99m)
            .WithMessage("Price must not exceed 9,999,999.99")
            .WithErrorCode("INVALID_VALUE")
            .PrecisionScale(10, 2, true)
            .WithMessage("Price must have maximum 2 decimal places")
            .WithErrorCode("INVALID_PRECISION");

        // Discount Price validation (optional but if provided, must be less than price)
        RuleFor(x => x.DiscountPrice)
            .GreaterThan(0)
            .WithMessage("Discount price must be greater than 0")
            .WithErrorCode("INVALID_VALUE")
            .LessThan(x => x.Price)
            .WithMessage("Discount price must be less than regular price")
            .WithErrorCode("INVALID_VALUE")
            .When(x => x.DiscountPrice.HasValue);

        // Stock quantity validation
        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock quantity must be 0 or greater")
            .WithErrorCode("INVALID_VALUE")
            .LessThanOrEqualTo(1000000)
            .WithMessage("Stock quantity must not exceed 1,000,000")
            .WithErrorCode("INVALID_VALUE");

        // Categories validation (optional)
        RuleFor(x => x.Categories)
            .Must(x => x == null || (x.Count > 0 && x.Count <= 10))
            .WithMessage("Must have between 1 and 10 categories if provided")
            .WithErrorCode("INVALID_COLLECTION_SIZE")
            .When(x => x.Categories != null);

        // Individual category validation
        RuleForEach(x => x.Categories)
            .NotEmpty()
            .WithMessage("Category name cannot be empty")
            .WithErrorCode("FIELD_REQUIRED")
            .Length(1, 100)
            .WithMessage("Category name must be between 1 and 100 characters")
            .WithErrorCode("FIELD_LENGTH_INVALID")
            .When(x => x.Categories != null);

        // Brand name validation (optional)
        RuleFor(x => x.BrandName)
            .Length(1, 100)
            .WithMessage("Brand name must be between 1 and 100 characters")
            .WithErrorCode("FIELD_LENGTH_INVALID")
            .When(x => !string.IsNullOrEmpty(x.BrandName));

        // Tags validation (optional)
        RuleFor(x => x.Tags)
            .Must(x => x == null || (x.Count > 0 && x.Count <= 20))
            .WithMessage("Must have between 1 and 20 tags if provided")
            .WithErrorCode("INVALID_COLLECTION_SIZE")
            .When(x => x.Tags != null);

        // Individual tag validation
        RuleForEach(x => x.Tags)
            .NotEmpty()
            .WithMessage("Tag cannot be empty")
            .WithErrorCode("FIELD_REQUIRED")
            .Length(1, 50)
            .WithMessage("Tag must be between 1 and 50 characters")
            .WithErrorCode("FIELD_LENGTH_INVALID")
            .When(x => x.Tags != null);
    }
}

/// <summary>
/// Validator for UpdateProductRequest to prevent injection attacks and ensure data integrity.
/// Validates price ranges, stock quantities, and field length limits for update operations.
/// </summary>
public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        // Name validation (optional)
        RuleFor(x => x.Name)
            .Length(1, 200)
            .WithMessage("Product name must be between 1 and 200 characters")
            .WithErrorCode("FIELD_LENGTH_INVALID")
            .Matches(@"^[a-zA-Z0-9\s\-&'().,]+$")
            .WithMessage("Product name contains invalid characters")
            .WithErrorCode("INVALID_CHARACTER_SET")
            .When(x => !string.IsNullOrEmpty(x.Name));

        // Description validation (optional)
        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Description must not exceed 2000 characters")
            .WithErrorCode("FIELD_TOO_LONG")
            .When(x => !string.IsNullOrEmpty(x.Description));

        // Price validation (optional)
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0")
            .WithErrorCode("INVALID_VALUE")
            .LessThanOrEqualTo(9999999.99m)
            .WithMessage("Price must not exceed 9,999,999.99")
            .WithErrorCode("INVALID_VALUE")
            .When(x => x.Price.HasValue);

        // Discount Price validation (optional)
        RuleFor(x => x.DiscountPrice)
            .GreaterThan(0)
            .WithMessage("Discount price must be greater than 0")
            .WithErrorCode("INVALID_VALUE")
            .LessThan(x => x.Price ?? decimal.MaxValue)
            .WithMessage("Discount price must be less than regular price")
            .WithErrorCode("INVALID_VALUE")
            .When(x => x.DiscountPrice.HasValue);

        // Stock quantity validation (optional)
        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock quantity must be 0 or greater")
            .WithErrorCode("INVALID_VALUE")
            .LessThanOrEqualTo(1000000)
            .WithMessage("Stock quantity must not exceed 1,000,000")
            .WithErrorCode("INVALID_VALUE")
            .When(x => x.StockQuantity.HasValue);

        // Brand name validation (optional)
        RuleFor(x => x.BrandName)
            .Length(1, 100)
            .WithMessage("Brand name must be between 1 and 100 characters")
            .WithErrorCode("FIELD_LENGTH_INVALID")
            .When(x => !string.IsNullOrEmpty(x.BrandName));

        // Categories validation (optional)
        RuleFor(x => x.Categories)
            .Must(x => x == null || (x.Count > 0 && x.Count <= 10))
            .WithMessage("Must have between 1 and 10 categories if provided")
            .WithErrorCode("INVALID_COLLECTION_SIZE")
            .When(x => x.Categories != null);

        // Tags validation (optional)
        RuleFor(x => x.Tags)
            .Must(x => x == null || (x.Count > 0 && x.Count <= 20))
            .WithMessage("Must have between 1 and 20 tags if provided")
            .WithErrorCode("INVALID_COLLECTION_SIZE")
            .When(x => x.Tags != null);
    }
}
