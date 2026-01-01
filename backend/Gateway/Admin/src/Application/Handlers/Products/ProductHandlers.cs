using Wolverine;
using B2Connect.Admin.Application.Commands.Products;
using B2Connect.Admin.Application.Handlers;
using B2Connect.Admin.Core.Interfaces;
using B2Connect.Middleware;
using B2Connect.Shared.Core;

namespace B2Connect.Admin.Application.Handlers.Products;

/// <summary>
/// Helper method for converting Product entities to ProductResult DTOs
/// </summary>
internal static class ProductMapper
{
    public static ProductResult ToResult(B2Connect.Admin.Core.Entities.Product product) =>
        new ProductResult(
            product.Id,
            product.TenantId ?? Guid.Empty,
            product.Name,
            product.Sku,
            product.Price,
            product.Description,
            product.CategoryId,
            product.BrandId,
            product.CreatedAt);
}

/// <summary>
/// Wolverine Message Handler für Product Commands
/// Enthält die komplette Business-Logik für Produkt-Operationen
/// 
/// TenantId wird automatisch via ITenantContextAccessor injiziert
/// </summary>
public class CreateProductHandler : ICommandHandler<CreateProductCommand, ProductResult>
{
    private readonly IProductRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;
    private readonly ILogger<CreateProductHandler> _logger;

    public CreateProductHandler(
        IProductRepository repository,
        ITenantContextAccessor tenantContext,
        ILogger<CreateProductHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ProductResult> Handle(CreateProductCommand command, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();

        _logger.LogInformation(
            "Creating product '{Name}' (SKU: {Sku}) for tenant {TenantId}",
            command.Name, command.Sku, tenantId);

        // Validierung
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ArgumentException("Product name is required", nameof(command.Name));

        if (command.Price <= 0)
            throw new ArgumentException("Product price must be greater than 0", nameof(command.Price));

        // Business Logic
        var product = new B2Connect.Admin.Core.Entities.Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
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

        return ProductMapper.ToResult(product);
    }
}

public class UpdateProductHandler : ICommandHandler<UpdateProductCommand, ProductResult>
{
    private readonly IProductRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;
    private readonly ILogger<UpdateProductHandler> _logger;

    public UpdateProductHandler(
        IProductRepository repository,
        ITenantContextAccessor tenantContext,
        ILogger<UpdateProductHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ProductResult> Handle(UpdateProductCommand command, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();

        _logger.LogInformation(
            "Updating product {ProductId} for tenant {TenantId}",
            command.ProductId, tenantId);

        var product = await _repository.GetByIdAsync(tenantId, command.ProductId, ct);
        if (product == null)
            throw new KeyNotFoundException($"Product {command.ProductId} not found");

        // Update fields - direct string assignment (Hybrid Localization Pattern)
        product.Name = command.Name;
        product.Sku = command.Sku;
        product.Price = command.Price;
        product.Description = command.Description;
        product.CategoryId = command.CategoryId;
        product.BrandId = command.BrandId;
        product.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(product, ct);

        _logger.LogInformation("Product {ProductId} updated successfully", product.Id);

        return ProductMapper.ToResult(product);
    }
}

public class GetProductHandler : IQueryHandler<GetProductQuery, ProductResult?>
{
    private readonly IProductRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;

    public GetProductHandler(IProductRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<ProductResult?> Handle(GetProductQuery query, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();
        var product = await _repository.GetByIdAsync(tenantId, query.ProductId, ct);

        if (product == null)
            return null;

        return ProductMapper.ToResult(product);
    }
}

public class GetProductBySkuHandler : IQueryHandler<GetProductBySkuQuery, ProductResult?>
{
    private readonly IProductRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;

    public GetProductBySkuHandler(IProductRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<ProductResult?> Handle(GetProductBySkuQuery query, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();
        var product = await _repository.GetBySkuAsync(tenantId, query.Sku, ct);

        if (product == null)
            return null;

        return ProductMapper.ToResult(product);
    }
}

public class GetAllProductsHandler : IQueryHandler<GetAllProductsQuery, IEnumerable<ProductResult>>
{
    private readonly IProductRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;

    public GetAllProductsHandler(IProductRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<IEnumerable<ProductResult>> Handle(GetAllProductsQuery query, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();
        var products = await _repository.GetAllAsync(tenantId, ct);

        return products.Select(ProductMapper.ToResult);
    }
}

public class DeleteProductHandler : ICommandHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;
    private readonly ILogger<DeleteProductHandler> _logger;

    public DeleteProductHandler(
        IProductRepository repository,
        ITenantContextAccessor tenantContext,
        ILogger<DeleteProductHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(DeleteProductCommand command, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();

        _logger.LogInformation(
            "Deleting product {ProductId} from tenant {TenantId}",
            command.ProductId, tenantId);

        var product = await _repository.GetByIdAsync(tenantId, command.ProductId, ct);
        if (product == null)
            return false;

        await _repository.DeleteAsync(tenantId, command.ProductId, ct);

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
    private readonly ITenantContextAccessor _tenantContext;

    public GetProductBySlugHandler(IProductRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<ProductResult?> Handle(GetProductBySlugQuery query, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();
        var product = await _repository.GetBySlugAsync(tenantId, query.Slug, ct);

        if (product == null)
            return null;

        return ProductMapper.ToResult(product);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für GetProductsByCategory
// ─────────────────────────────────────────────────────────────────────────────
public class GetProductsByCategoryHandler : IQueryHandler<GetProductsByCategoryQuery, IEnumerable<ProductResult>>
{
    private readonly IProductRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;

    public GetProductsByCategoryHandler(IProductRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<IEnumerable<ProductResult>> Handle(GetProductsByCategoryQuery query, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();
        var products = await _repository.GetByCategoryAsync(tenantId, query.CategoryId, ct);

        return products.Select(ProductMapper.ToResult);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für GetProductsByBrand
// ─────────────────────────────────────────────────────────────────────────────
public class GetProductsByBrandHandler : IQueryHandler<GetProductsByBrandQuery, IEnumerable<ProductResult>>
{
    private readonly IProductRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;

    public GetProductsByBrandHandler(IProductRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<IEnumerable<ProductResult>> Handle(GetProductsByBrandQuery query, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();
        var products = await _repository.GetByBrandAsync(tenantId, query.BrandId, ct);

        return products.Select(ProductMapper.ToResult);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für GetFeaturedProducts
// ─────────────────────────────────────────────────────────────────────────────
public class GetFeaturedProductsHandler : IQueryHandler<GetFeaturedProductsQuery, IEnumerable<ProductResult>>
{
    private readonly IProductRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;

    public GetFeaturedProductsHandler(IProductRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<IEnumerable<ProductResult>> Handle(GetFeaturedProductsQuery query, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();
        var products = await _repository.GetFeaturedAsync(tenantId, query.Take, ct);

        return products.Select(ProductMapper.ToResult);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für GetNewProducts
// ─────────────────────────────────────────────────────────────────────────────
public class GetNewProductsHandler : IQueryHandler<GetNewProductsQuery, IEnumerable<ProductResult>>
{
    private readonly IProductRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;

    public GetNewProductsHandler(IProductRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<IEnumerable<ProductResult>> Handle(GetNewProductsQuery query, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();
        var products = await _repository.GetNewestAsync(tenantId, query.Take, ct);

        return products.Select(ProductMapper.ToResult);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für SearchProducts
// ─────────────────────────────────────────────────────────────────────────────
public class SearchProductsHandler : IQueryHandler<SearchProductsQuery, (IEnumerable<ProductResult>, int)>
{
    private readonly IProductRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;

    public SearchProductsHandler(IProductRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<(IEnumerable<ProductResult>, int)> Handle(SearchProductsQuery query, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();
        var (products, total) = await _repository.SearchAsync(
            tenantId,
            query.SearchTerm,
            query.PageNumber,
            query.PageSize,
            ct);

        var results = products.Select(ProductMapper.ToResult);

        return (results, total);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Query Handler für GetProductsPaged
// ─────────────────────────────────────────────────────────────────────────────
public class GetProductsPagedHandler : IQueryHandler<GetProductsPagedQuery, (IEnumerable<ProductResult>, int)>
{
    private readonly IProductRepository _repository;
    private readonly ITenantContextAccessor _tenantContext;

    public GetProductsPagedHandler(IProductRepository repository, ITenantContextAccessor tenantContext)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
    }

    public async Task<(IEnumerable<ProductResult>, int)> Handle(GetProductsPagedQuery query, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();
        var (products, total) = await _repository.GetPagedAsync(
            tenantId,
            query.PageNumber,
            query.PageSize,
            ct);

        var results = products.Select(ProductMapper.ToResult);

        return (results, total);
    }
}
