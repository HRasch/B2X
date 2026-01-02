using Wolverine;
using B2Connect.Admin.Application.Commands.Products;
using B2Connect.Admin.Application.Handlers;
using B2Connect.Admin.Core.Interfaces;
using B2Connect.Admin.Infrastructure.Data;
using B2Connect.Middleware;
using B2Connect.Types.Localization;
using B2Connect.ERP.Infrastructure.DataAccess;
using Dapper;
using EFCore.BulkExtensions;

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
            product.Name?.Get("en") ?? string.Empty,
            product.Sku,
            product.Price,
            product.Description?.Get("en"),
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
        {
            throw new ArgumentException("Product name is required", nameof(command.Name));
        }

        if (command.Price <= 0)
        {
            throw new ArgumentException("Product price must be greater than 0", nameof(command.Price));
        }

        // Business Logic
        var product = new B2Connect.Admin.Core.Entities.Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = new LocalizedContent().Set("en", command.Name),
            Sku = command.Sku,
            Price = command.Price,
            Description = command.Description != null
                ? new LocalizedContent().Set("en", command.Description)
                : null,
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
        {
            throw new KeyNotFoundException($"Product {command.ProductId} not found");
        }

        // Update fields - convert string to LocalizedContent
        product.Name = new LocalizedContent().Set("en", command.Name);
        product.Sku = command.Sku;
        product.Price = command.Price;
        product.Description = command.Description != null
            ? new LocalizedContent().Set("en", command.Description)
            : null;
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
        {
            return null;
        }

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
        {
            return null;
        }

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
        {
            return false;
        }

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
        {
            return null;
        }

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

// ─────────────────────────────────────────────────────────────────────────────
// Bulk Import Handler (ADR-025) - EFCore.BulkExtensions
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Wolverine Handler für Bulk Product Import (ADR-025)
/// Verwendet EFCore.BulkExtensions für optimale Performance bei Massen-Importen
///
/// Performance: 10-50x schneller als individuelle Inserts
/// Use Case: ERP Catalog Sync, CSV Import, etc.
/// </summary>
public class BulkImportProductsHandler : ICommandHandler<BulkImportProductsCommand, BulkImportResult>
{
    private readonly CatalogDbContext _dbContext;
    private readonly ITenantContextAccessor _tenantContext;
    private readonly ILogger<BulkImportProductsHandler> _logger;

    public BulkImportProductsHandler(
        CatalogDbContext dbContext,
        ITenantContextAccessor tenantContext,
        ILogger<BulkImportProductsHandler> _logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        this._logger = _logger ?? throw new ArgumentNullException(nameof(_logger));
    }

    public async Task<BulkImportResult> Handle(BulkImportProductsCommand command, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();
        var importId = Guid.NewGuid();

        _logger.LogInformation(
            "Starting bulk import of {Count} products for tenant {TenantId} (ImportId: {ImportId})",
            command.Products.Count, tenantId, importId);

        var errors = new List<string>();
        var validProducts = new List<B2Connect.Admin.Core.Entities.Product>();

        // Phase 1: Validierung und Mapping
        foreach (var item in command.Products)
        {
            try
            {
                // Validierung
                if (string.IsNullOrWhiteSpace(item.Name))
                {
                    errors.Add($"Product SKU '{item.Sku}': Name is required");
                    continue;
                }

                if (item.Price <= 0)
                {
                    errors.Add($"Product SKU '{item.Sku}': Price must be greater than 0");
                    continue;
                }

                // Mapping zu Entity
                var product = new B2Connect.Admin.Core.Entities.Product
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = new LocalizedContent().Set("en", item.Name),
                    Sku = item.Sku,
                    Price = item.Price,
                    Description = item.Description != null
                        ? new LocalizedContent().Set("en", item.Description)
                        : null,
                    CategoryId = item.CategoryId,
                    BrandId = item.BrandId,
                    CreatedAt = DateTime.UtcNow
                };

                validProducts.Add(product);
            }
            catch (Exception ex)
            {
                errors.Add($"Product SKU '{item.Sku}': {ex.Message}");
            }
        }

        // Phase 2: Bulk Insert mit EFCore.BulkExtensions
        var importedCount = 0;
        if (validProducts.Any())
        {
            try
            {
                var bulkConfig = new BulkConfig
                {
                    BatchSize = 1000, // Performance-Optimierung
                    UseTempDB = true, // Reduziert Locking
                    CalculateStats = true
                };

                await _dbContext.BulkInsertAsync(validProducts, bulkConfig, cancellationToken: ct);
                importedCount = validProducts.Count;

                _logger.LogInformation(
                    "Successfully imported {Count} products via BulkInsert for tenant {TenantId}",
                    importedCount, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk insert failed for tenant {TenantId}", tenantId);
                errors.Add($"Bulk insert failed: {ex.Message}");
                importedCount = 0;
            }
        }

        var result = new BulkImportResult(
            TotalProducts: command.Products.Count,
            ImportedProducts: importedCount,
            FailedProducts: command.Products.Count - importedCount,
            Errors: errors,
            ImportId: importId);

        _logger.LogInformation(
            "Bulk import completed for tenant {TenantId} (ImportId: {ImportId}): {Imported}/{Total} products imported, {Failed} failed",
            tenantId, importId, result.ImportedProducts, result.TotalProducts, result.FailedProducts);

        return result;
    }
}

/// <summary>
/// Get All Products For Index Handler (ADR-025)
/// Optimiert für Search Reindexing mit Dapper
/// Verwendet direkte SQL-Queries ohne EF Core Overhead
/// </summary>
public class GetAllProductsForIndexHandler : IQueryHandler<GetAllProductsForIndexQuery, IEnumerable<ProductIndexResult>>
{
    private readonly IDapperConnectionFactory _connectionFactory;
    private readonly ITenantContextAccessor _tenantContext;
    private readonly ILogger<GetAllProductsForIndexHandler> _logger;

    public GetAllProductsForIndexHandler(
        IDapperConnectionFactory connectionFactory,
        ITenantContextAccessor tenantContext,
        ILogger<GetAllProductsForIndexHandler> logger)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _tenantContext = tenantContext ?? throw new ArgumentNullException(nameof(tenantContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<ProductIndexResult>> Handle(GetAllProductsForIndexQuery query, CancellationToken ct)
    {
        var tenantId = _tenantContext.GetTenantId();

        _logger.LogInformation("Starting product index retrieval for tenant {TenantId}", tenantId);

        using var connection = _connectionFactory.CreateConnection();

        // Optimierte Query für Search Index - nur relevante Felder
        // Join mit Categories und Brands für Namen (ohne Navigation Properties)
        const string sql = @"
            SELECT
                p.id,
                p.tenant_id,
                p.name,
                p.sku,
                p.price,
                p.description,
                c.name as category_name,
                b.name as brand_name,
                p.created_at
            FROM products p
            LEFT JOIN categories c ON p.category_id = c.id AND p.tenant_id = c.tenant_id
            LEFT JOIN brands b ON p.brand_id = b.id AND p.tenant_id = b.tenant_id
            WHERE p.tenant_id = @TenantId
            ORDER BY p.created_at DESC";

        var products = await connection.QueryAsync<ProductIndexResult>(
            sql,
            new { TenantId = tenantId },
            commandTimeout: 300); // 5 Minuten Timeout für große Datasets

        _logger.LogInformation(
            "Retrieved {Count} products for search indexing for tenant {TenantId}",
            products.Count(), tenantId);

        return products;
    }
}
