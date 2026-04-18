namespace B2X.Admin.Application.Commands.Products;

/// <summary>
/// Product Commands & Queries - CQRS Pattern
///
/// Flow:
/// Controller empfängt HTTP Request
/// → Erstellt Command/Query (ohne TenantId!)
/// → Dispatched via IMessageBus
/// → Handler holt TenantId via ITenantContextAccessor
/// → Response zurück an Controller
///
/// NOTE: TenantId wird automatisch via ITenantContextAccessor im Handler injiziert
/// </summary>

// ─────────────────────────────────────────────────────────────────────────────
// Commands
// ─────────────────────────────────────────────────────────────────────────────

public record CreateProductCommand(
    string Name,
    string Sku,
    decimal Price,
    string? Description = null,
    Guid? CategoryId = null,
    Guid? BrandId = null);

public record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string Sku,
    decimal Price,
    string? Description = null,
    Guid? CategoryId = null,
    Guid? BrandId = null);

public record DeleteProductCommand(Guid ProductId);

/// <summary>
/// Bulk Import Products Command (ADR-025)
/// Für Massen-Import von Produkten aus ERP-Systemen
/// Verwendet EFCore.BulkExtensions für Performance
/// </summary>
public record BulkImportProductsCommand(
    IReadOnlyList<BulkImportProductItem> Products);

/// <summary>
/// Einzelnes Produkt für Bulk Import
/// </summary>
public record BulkImportProductItem(
    string Name,
    string Sku,
    decimal Price,
    string? Description = null,
    Guid? CategoryId = null,
    Guid? BrandId = null);

// ─────────────────────────────────────────────────────────────────────────────
// Queries
// ─────────────────────────────────────────────────────────────────────────────

public record GetProductQuery(Guid ProductId);

public record GetProductBySkuQuery(string Sku);

public record GetAllProductsQuery();

public record GetProductsPagedQuery(int PageNumber, int PageSize);

/// <summary>
/// Query für Get Product by Slug (SEO-freundliche URL)
/// </summary>
public record GetProductBySlugQuery(string Slug);

/// <summary>
/// Query für Get Products by Category
/// </summary>
public record GetProductsByCategoryQuery(Guid CategoryId);

/// <summary>
/// Query für Get Products by Brand
/// </summary>
public record GetProductsByBrandQuery(Guid BrandId);

/// <summary>
/// Query für Get Featured Products
/// </summary>
public record GetFeaturedProductsQuery(int Take = 10);

/// <summary>
/// Query für Get New Products (nach CreatedAt sortiert)
/// </summary>
public record GetNewProductsQuery(int Take = 10);

/// <summary>
/// Query für Search Products (Volltextsuche)
/// </summary>
public record SearchProductsQuery(
    string SearchTerm,
    int PageNumber = 1,
    int PageSize = 20);

/// <summary>
/// Query für Get All Products For Index (ADR-025)
/// Speziell für Search Reindexing optimiert mit Dapper
/// Gibt nur die für Search-Index nötigen Felder zurück
/// </summary>
public record GetAllProductsForIndexQuery();

// ─────────────────────────────────────────────────────────────────────────────
// Result DTOs
// ─────────────────────────────────────────────────────────────────────────────

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

/// <summary>
/// Result DTO für Search Index - optimiert für Dapper (ADR-025)
/// Enthält nur die für Search-Index nötigen Felder
/// Keine Navigation Properties, minimal Memory Footprint
/// </summary>
public record ProductIndexResult(
    Guid Id,
    Guid TenantId,
    string Name,
    string Sku,
    decimal Price,
    string? Description = null,
    string? CategoryName = null,
    string? BrandName = null,
    DateTime CreatedAt = default);

/// <summary>
/// Result DTO für Bulk Import Operationen
/// </summary>
public record BulkImportResult(
    int TotalProducts,
    int ImportedProducts,
    int FailedProducts,
    IReadOnlyList<string> Errors,
    Guid ImportId);
