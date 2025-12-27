using Wolverine;
using B2Connect.Admin.Application.Commands.Products;
using B2Connect.Admin.Core.Interfaces;
using B2Connect.Shared.Tenancy.Infrastructure.Context;

namespace B2Connect.Admin.Application.Handlers.Products;

/// <summary>
/// Wolverine Message Handler für Product Commands
/// Enthält die komplette Business-Logik für Produkt-Operationen
/// 
/// Wolverine leitet automatisch:
/// - Commands/Queries vom Controller zum Handler
/// - Dependency Injection
/// - Exception Handling
/// - Logging (optional)
/// </summary>
public class CreateProductHandler : ICommandHandler<CreateProductCommand, ProductResult>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<CreateProductHandler> _logger;

    public CreateProductHandler(IProductRepository repository, ILogger<CreateProductHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ProductResult> Handle(CreateProductCommand command, CancellationToken ct)
    {
        _logger.LogInformation(
            "Creating product '{Name}' (SKU: {Sku}) for tenant {TenantId}",
            command.Name, command.Sku, command.TenantId);

        // Validierung
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ArgumentException("Product name is required", nameof(command.Name));

        if (command.Price <= 0)
            throw new ArgumentException("Product price must be greater than 0", nameof(command.Price));

        // Business Logic
        var product = new B2Connect.Admin.Core.Entities.Product
        {
            Id = Guid.NewGuid(),
            TenantId = command.TenantId,
            Name = command.Name,
            Sku = command.Sku,
            Price = command.Price,
            Description = command.Description,
            CategoryId = command.CategoryId,
            BrandId = command.BrandId,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(product, ct);

        _logger.LogInformation("Product {ProductId} created successfully", product.Id);

        return new ProductResult(
            product.Id,
            product.TenantId,
            product.Name,
            product.Sku,
            product.Price,
            product.Description,
            product.CategoryId,
            product.BrandId,
            product.CreatedAt);
    }
}

public class UpdateProductHandler : ICommandHandler<UpdateProductCommand, ProductResult>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<UpdateProductHandler> _logger;

    public UpdateProductHandler(IProductRepository repository, ILogger<UpdateProductHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ProductResult> Handle(UpdateProductCommand command, CancellationToken ct)
    {
        _logger.LogInformation(
            "Updating product {ProductId} for tenant {TenantId}",
            command.ProductId, command.TenantId);

        var product = await _repository.GetByIdAsync(command.TenantId, command.ProductId, ct);
        if (product == null)
            throw new KeyNotFoundException($"Product {command.ProductId} not found");

        // Update fields
        product.Name = command.Name;
        product.Sku = command.Sku;
        product.Price = command.Price;
        product.Description = command.Description;
        product.CategoryId = command.CategoryId;
        product.BrandId = command.BrandId;

        await _repository.UpdateAsync(product, ct);

        _logger.LogInformation("Product {ProductId} updated successfully", product.Id);

        return new ProductResult(
            product.Id,
            product.TenantId,
            product.Name,
            product.Sku,
            product.Price,
            product.Description,
            product.CategoryId,
            product.BrandId,
            product.CreatedAt);
    }
}

public class GetProductHandler : IQueryHandler<GetProductQuery, ProductResult?>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<GetProductHandler> _logger;

    public GetProductHandler(IProductRepository repository, ILogger<GetProductHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ProductResult?> Handle(GetProductQuery query, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(query.TenantId, query.ProductId, ct);

        if (product == null)
            return null;

        return new ProductResult(
            product.Id,
            product.TenantId,
            product.Name,
            product.Sku,
            product.Price,
            product.Description,
            product.CategoryId,
            product.BrandId,
            product.CreatedAt);
    }
}

public class GetProductBySkuHandler : IQueryHandler<GetProductBySkuQuery, ProductResult?>
{
    private readonly IProductRepository _repository;

    public GetProductBySkuHandler(IProductRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<ProductResult?> Handle(GetProductBySkuQuery query, CancellationToken ct)
    {
        var product = await _repository.GetBySkuAsync(query.TenantId, query.Sku, ct);

        if (product == null)
            return null;

        return new ProductResult(
            product.Id,
            product.TenantId,
            product.Name,
            product.Sku,
            product.Price,
            product.Description,
            product.CategoryId,
            product.BrandId,
            product.CreatedAt);
    }
}

public class GetAllProductsHandler : IQueryHandler<GetAllProductsQuery, IEnumerable<ProductResult>>
{
    private readonly IProductRepository _repository;

    public GetAllProductsHandler(IProductRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<ProductResult>> Handle(GetAllProductsQuery query, CancellationToken ct)
    {
        var products = await _repository.GetAllAsync(query.TenantId, ct);

        return products.Select(p => new ProductResult(
            p.Id,
            p.TenantId,
            p.Name,
            p.Sku,
            p.Price,
            p.Description,
            p.CategoryId,
            p.BrandId,
            p.CreatedAt));
    }
}

public class DeleteProductHandler : ICommandHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<DeleteProductHandler> _logger;

    public DeleteProductHandler(IProductRepository repository, ILogger<DeleteProductHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(DeleteProductCommand command, CancellationToken ct)
    {
        _logger.LogInformation(
            "Deleting product {ProductId} from tenant {TenantId}",
            command.ProductId, command.TenantId);

        var product = await _repository.GetByIdAsync(command.TenantId, command.ProductId, ct);
        if (product == null)
            return false;

        await _repository.DeleteAsync(command.TenantId, command.ProductId, ct);

        _logger.LogInformation("Product {ProductId} deleted successfully", command.ProductId);
        return true;
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für GetProductBySlug
// ─────────────────────────────────────────────────────────────────────────────
public class GetProductBySlugHandler : IQueryHandler<GetProductBySlugQuery, ProductResult?>
{
    private readonly IProductRepository _repository;

    public GetProductBySlugHandler(IProductRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<ProductResult?> Handle(GetProductBySlugQuery query, CancellationToken ct)
    {
        var product = await _repository.GetBySlugAsync(query.TenantId, query.Slug, ct);

        if (product == null)
            return null;

        return new ProductResult(
            product.Id,
            product.TenantId,
            product.Name,
            product.Sku,
            product.Price,
            product.Description,
            product.CategoryId,
            product.BrandId,
            product.CreatedAt);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für GetProductsByCategory
// ─────────────────────────────────────────────────────────────────────────────
public class GetProductsByCategoryHandler : IQueryHandler<GetProductsByCategoryQuery, IEnumerable<ProductResult>>
{
    private readonly IProductRepository _repository;

    public GetProductsByCategoryHandler(IProductRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<ProductResult>> Handle(GetProductsByCategoryQuery query, CancellationToken ct)
    {
        var products = await _repository.GetByCategoryAsync(query.TenantId, query.CategoryId, ct);

        return products.Select(p => new ProductResult(
            p.Id, p.TenantId, p.Name, p.Sku, p.Price,
            p.Description, p.CategoryId, p.BrandId, p.CreatedAt));
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für GetProductsByBrand
// ─────────────────────────────────────────────────────────────────────────────
public class GetProductsByBrandHandler : IQueryHandler<GetProductsByBrandQuery, IEnumerable<ProductResult>>
{
    private readonly IProductRepository _repository;

    public GetProductsByBrandHandler(IProductRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<ProductResult>> Handle(GetProductsByBrandQuery query, CancellationToken ct)
    {
        var products = await _repository.GetByBrandAsync(query.TenantId, query.BrandId, ct);

        return products.Select(p => new ProductResult(
            p.Id, p.TenantId, p.Name, p.Sku, p.Price,
            p.Description, p.CategoryId, p.BrandId, p.CreatedAt));
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für GetFeaturedProducts
// ─────────────────────────────────────────────────────────────────────────────
public class GetFeaturedProductsHandler : IQueryHandler<GetFeaturedProductsQuery, IEnumerable<ProductResult>>
{
    private readonly IProductRepository _repository;

    public GetFeaturedProductsHandler(IProductRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<ProductResult>> Handle(GetFeaturedProductsQuery query, CancellationToken ct)
    {
        var products = await _repository.GetFeaturedAsync(query.TenantId, query.Take, ct);

        return products.Select(p => new ProductResult(
            p.Id, p.TenantId, p.Name, p.Sku, p.Price,
            p.Description, p.CategoryId, p.BrandId, p.CreatedAt));
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für GetNewProducts
// ─────────────────────────────────────────────────────────────────────────────
public class GetNewProductsHandler : IQueryHandler<GetNewProductsQuery, IEnumerable<ProductResult>>
{
    private readonly IProductRepository _repository;

    public GetNewProductsHandler(IProductRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<ProductResult>> Handle(GetNewProductsQuery query, CancellationToken ct)
    {
        var products = await _repository.GetNewestAsync(query.TenantId, query.Take, ct);

        return products.Select(p => new ProductResult(
            p.Id, p.TenantId, p.Name, p.Sku, p.Price,
            p.Description, p.CategoryId, p.BrandId, p.CreatedAt));
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für SearchProducts
// ─────────────────────────────────────────────────────────────────────────────
public class SearchProductsHandler : IQueryHandler<SearchProductsQuery, (IEnumerable<ProductResult>, int)>
{
    private readonly IProductRepository _repository;

    public SearchProductsHandler(IProductRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<(IEnumerable<ProductResult>, int)> Handle(SearchProductsQuery query, CancellationToken ct)
    {
        var (products, total) = await _repository.SearchAsync(
            query.TenantId,
            query.SearchTerm,
            query.PageNumber,
            query.PageSize,
            ct);

        var results = products.Select(p => new ProductResult(
            p.Id, p.TenantId, p.Name, p.Sku, p.Price,
            p.Description, p.CategoryId, p.BrandId, p.CreatedAt));

        return (results, total);
    }
}
