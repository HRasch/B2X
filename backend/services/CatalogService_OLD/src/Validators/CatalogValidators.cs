using FluentValidation;
using B2Connect.CatalogService.Models;
using B2Connect.Types.Localization;

namespace B2Connect.CatalogService.Validators;

/// <summary>
/// Validator for CreateProductRequest
/// Enforces business rules for product creation
/// Supports async validation (SKU uniqueness check)
/// </summary>
public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU is required")
            .Length(3, 50).WithMessage("SKU must be between 3 and 50 characters")
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("SKU must contain only uppercase letters, numbers, and hyphens");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .Length(2, 200).WithMessage("Product name must be between 2 and 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Product description is required")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0")
            .Must(x => decimal.Round(x, 2) == x)
                .WithMessage("Price can have maximum 2 decimal places");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative");

        RuleFor(x => x.Tags)
            .NotNull().WithMessage("Tags collection is required")
            .Must(x => x.Length >= 1).WithMessage("At least one tag is required")
            .Must(x => x.All(t => !string.IsNullOrWhiteSpace(t)))
                .WithMessage("Tags cannot be empty");

        RuleFor(x => x.LocalizedNames)
            .NotNull().WithMessage("Localized names are required")
            .Must(x => x.Count > 0).WithMessage("At least one localized name is required")
            .Must(x => x.ContainsKey("en"))
                .WithMessage("English (en) localized name is required");

        RuleForEach(x => x.LocalizedNames)
            .Must(x => x.Value?.Translations?.Count > 0 && x.Value.Translations.Values.All(v => !string.IsNullOrWhiteSpace(v)))
                .WithMessage("Localized name content cannot be empty");
    }
}

/// <summary>
/// Validator for UpdateProductRequest
/// Similar to Create but allows partial updates
/// </summary>
public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Sku)
            .Length(3, 50)
                .When(x => !string.IsNullOrEmpty(x.Sku))
                .WithMessage("SKU must be between 3 and 50 characters");

        RuleFor(x => x.Name)
            .Length(2, 200)
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage("Product name must be between 2 and 200 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0)
                .When(x => x.Price.HasValue)
                .WithMessage("Price must be greater than 0");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0)
                .When(x => x.StockQuantity.HasValue)
                .WithMessage("Stock quantity cannot be negative");
    }
}

/// <summary>
/// Validator for CreateCategoryRequest
/// </summary>
public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required")
            .Length(2, 100).WithMessage("Category name must be between 2 and 100 characters");

        RuleFor(x => x.LocalizedNames)
            .NotNull().WithMessage("Localized names are required")
            .Must(x => x.Count > 0).WithMessage("At least one localized name is required")
            .Must(x => x.ContainsKey("en")).WithMessage("English (en) localized name is required");
    }
}

/// <summary>
/// Validator for CreateBrandRequest
/// </summary>
public class CreateBrandRequestValidator : AbstractValidator<CreateBrandRequest>
{
    public CreateBrandRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Brand name is required")
            .Length(2, 100).WithMessage("Brand name must be between 2 and 100 characters");

        RuleFor(x => x.LocalizedNames)
            .NotNull().WithMessage("Localized names are required")
            .Must(x => x.Count > 0).WithMessage("At least one localized name is required")
            .Must(x => x.ContainsKey("en")).WithMessage("English (en) localized name is required");
    }
}
