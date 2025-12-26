using FluentValidation;
using B2Connect.CatalogService.CQRS.Commands;
using B2Connect.CatalogService.Repositories;

namespace B2Connect.CatalogService.CQRS.Validators;

/// <summary>
/// Validates CreateProductCommand before execution
/// Runs automatically via Wolverine before handler is invoked
/// </summary>
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;

        RuleFor(c => c.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID is required");

        RuleFor(c => c.Sku)
            .NotEmpty()
            .WithMessage("SKU is required")
            .MaximumLength(50)
            .WithMessage("SKU must not exceed 50 characters")
            .MustAsync(async (sku, tenantId, cancellationToken) =>
            {
                // Check if SKU already exists for this tenant
                var existingProduct = await _productRepository.GetBySkuAsync(sku, cancellationToken);
                return existingProduct == null;
            })
            .WithMessage("SKU '{PropertyValue}' already exists for this tenant");

        RuleFor(c => c.Name)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MaximumLength(255)
            .WithMessage("Product name must not exceed 255 characters");

        RuleFor(c => c.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0")
            .PrecisionScale(10, 2, ignoreTrailingZeros: true)
            .WithMessage("Price must have at most 2 decimal places");

        RuleFor(c => c.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock quantity cannot be negative");
    }
}

/// <summary>
/// Validates UpdateProductCommand
/// </summary>
public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductCommandValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;

        RuleFor(c => c.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID is required");

        RuleFor(c => c.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        When(c => c.Name != null, () =>
        {
            RuleFor(c => c.Name)
                .MaximumLength(255)
                .WithMessage("Product name must not exceed 255 characters");
        });

        When(c => c.Price.HasValue, () =>
        {
            RuleFor(c => c.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0")
                .PrecisionScale(10, 2)
                .WithMessage("Price must have at most 2 decimal places");
        });
    }
}

/// <summary>
/// Validates DeleteProductCommand
/// </summary>
public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(c => c.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID is required");

        RuleFor(c => c.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");
    }
}

/// <summary>
/// Validates BulkImportProductsCommand
/// </summary>
public class BulkImportProductsCommandValidator : AbstractValidator<BulkImportProductsCommand>
{
    public BulkImportProductsCommandValidator()
    {
        RuleFor(c => c.TenantId)
            .NotEmpty()
            .WithMessage("Tenant ID is required");

        RuleFor(c => c.Products)
            .NotEmpty()
            .WithMessage("No products provided for import")
            .Must(p => p.Length <= 10000)
            .WithMessage("Cannot import more than 10,000 products at once");

        RuleForEach(c => c.Products)
            .ChildRules(product =>
            {
                product.RuleFor(p => p.Sku)
                    .NotEmpty()
                    .MaximumLength(50);

                product.RuleFor(p => p.Name)
                    .NotEmpty()
                    .MaximumLength(255);

                product.RuleFor(p => p.Price)
                    .GreaterThan(0)
                    .PrecisionScale(10, 2);
            });
    }
}
