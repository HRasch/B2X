using Wolverine;

namespace B2Connect.Admin.Application.Commands.Products;

/// <summary>
/// Create Product Command - CQRS Pattern
/// Wird vom ProductsController empfangen und via Wolverine dispatched
/// 
/// Flow:
/// Controller empfängt HTTP POST
/// → Erstellt CreateProductCommand
/// → Dispatched via IMessageBus
/// → CreateProductHandler verarbeitet (TenantId wird automatisch injiziert)
/// → Response zurück an Controller
/// 
/// NOTE: TenantId wird automatisch via ITenantContext injiziert
/// (keine manuelle Übergabe notwendig mehr)
/// </summary>
public record CreateProductCommand(
    string Name,
    string Sku,
    decimal Price,
    string? Description = null,
    Guid? CategoryId = null,
    Guid? BrandId = null) : IRequest<ProductResult>;

public record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string Sku,
    decimal Price,
    string? Description = null,
    Guid? CategoryId = null,
    Guid? BrandId = null) : IRequest<ProductResult>;

public record GetProductQuery(Guid ProductId) : IRequest<ProductResult?>;

public record GetProductBySkuQuery(string Sku) : IRequest<ProductResult?>;

public record GetAllProductsQuery() : IRequest<IEnumerable<ProductResult>>;

public record GetProductsPagedQuery(int PageNumber, int PageSize) : IRequest<(IEnumerable<ProductResult> Items, int Total)>;

public record DeleteProductCommand(Guid ProductId) : IRequest<bool>;

// ─────────────────────────────────────────────────────────────────────────────
// Zusätzliche Queries für ProductsController
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Query für Get Product by Slug (SEO-freundliche URL)
/// </summary>
public record GetProductBySlugQuery(string Slug) : IRequest<ProductResult?>;

/// <summary>
/// Query für Get Products by Category
/// </summary>
public record GetProductsByCategoryQuery(Guid CategoryId) : IRequest<IEnumerable<ProductResult>>;

/// <summary>
/// Query für Get Products by Brand
/// </summary>
public record GetProductsByBrandQuery(Guid BrandId) : IRequest<IEnumerable<ProductResult>>;

/// <summary>
/// Query für Get Featured Products
/// </summary>
public record GetFeaturedProductsQuery(int Take = 10) : IRequest<IEnumerable<ProductResult>>;

/// <summary>
/// Query für Get New Products (nach CreatedAt sortiert)
/// </summary>
public record GetNewProductsQuery(int Take = 10) : IRequest<IEnumerable<ProductResult>>;

/// <summary>
/// Query für Search Products (Volltextsuche)
/// </summary>
public record SearchProductsQuery(
    string SearchTerm,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<(IEnumerable<ProductResult> Items, int Total)>;

/// <summary>
/// Result DTO - was vom Handler zurückkommt
/// </summary>
public record ProductResult(
    Guid Id,
    Guid TenantId,
    string Name,
    string Sku,
    decimal Price,
    string? Description = null,
    Guid? CategoryId = null,
    Guid? BrandId = null,
    DateTime CreatedAt = default);
