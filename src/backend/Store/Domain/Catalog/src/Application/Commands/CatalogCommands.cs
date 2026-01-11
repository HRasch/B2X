using Wolverine;

namespace B2X.Catalog.Application.Commands;

/// <summary>
/// Common DTOs
/// </summary>
public record Dimensions(decimal Length, decimal Width, decimal Height, string Unit);

/// <summary>
/// Commands for category operations
/// </summary>
public record CreateCategoryCommand(
    Guid TenantId,
    string Name,
    string? Description,
    string? Slug,
    Guid? ParentId,
    string? ImageUrl,
    string? Icon,
    int DisplayOrder,
    string? MetaTitle,
    string? MetaDescription,
    bool IsActive,
    bool IsVisible
);

public record UpdateCategoryCommand(
    Guid Id,
    Guid TenantId,
    string Name,
    string? Description,
    string? Slug,
    Guid? ParentId,
    string? ImageUrl,
    string? Icon,
    int DisplayOrder,
    string? MetaTitle,
    string? MetaDescription,
    bool IsActive,
    bool IsVisible
);

public record DeleteCategoryCommand(Guid Id, Guid TenantId);

/// <summary>
/// Commands for variant operations
/// </summary>
public record CreateVariantDto(
    Guid ProductId,
    string Sku,
    string Name,
    string? Description,
    Dictionary<string, string> Attributes,
    decimal Price,
    decimal? CompareAtPrice,
    int StockQuantity,
    bool TrackInventory,
    bool AllowBackorders,
    List<string> ImageUrls,
    string? PrimaryImageUrl,
    bool IsActive,
    int DisplayOrder,
    string? Barcode,
    decimal? Weight,
    Dimensions? Dimensions
);

public record UpdateVariantDto(
    string Sku,
    string Name,
    string? Description,
    Dictionary<string, string> Attributes,
    decimal Price,
    decimal? CompareAtPrice,
    int StockQuantity,
    bool TrackInventory,
    bool AllowBackorders,
    List<string> ImageUrls,
    string? PrimaryImageUrl,
    bool IsActive,
    int DisplayOrder,
    string? Barcode,
    decimal? Weight,
    Dimensions? Dimensions
);

public record CreateVariantCommand(CreateVariantDto Variant);
public record UpdateVariantCommand(Guid Id, UpdateVariantDto Variant, Guid TenantId);
public record DeleteVariantCommand(Guid Id, Guid TenantId);
public record UpdateVariantStockCommand(Guid Id, int NewStockQuantity, Guid TenantId);

/// <summary>
/// Commands for product operations
/// </summary>
public record CreateProductCommand(
    Guid TenantId,
    string Sku,
    string Name,
    string? Description,
    decimal Price,
    decimal? DiscountPrice,
    int StockQuantity,
    bool IsActive,
    List<Guid> CategoryIds,
    string? BrandName,
    List<string> Tags,
    string? Barcode
);

public record UpdateProductCommand(
    Guid Id,
    Guid TenantId,
    string? Sku,
    string? Name,
    string? Description,
    decimal? Price,
    decimal? DiscountPrice,
    int? StockQuantity,
    bool? IsActive,
    List<Guid>? CategoryIds,
    string? BrandName,
    List<string>? Tags,
    string? Barcode
);

public record DeleteProductCommand(Guid Id, Guid TenantId);

public record CategorizeProductCommand(Guid ProductId, Guid CategoryId, Guid TenantId);
public record RemoveProductFromCategoryCommand(Guid ProductId, Guid CategoryId, Guid TenantId);

/// <summary>
/// Queries for category operations
/// </summary>
public record GetCategoryByIdQuery(Guid Id, Guid TenantId);
public record GetCategoryBySlugQuery(string Slug, Guid TenantId);
public record GetCategoriesQuery(
    Guid TenantId,
    string? SearchTerm = null,
    Guid? ParentId = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 20);
public record GetCategoryTreeQuery(Guid TenantId);

/// <summary>
/// Queries for variant operations
/// </summary>
public record GetVariantByIdQuery(Guid Id, Guid TenantId);
public record GetVariantBySkuQuery(string Sku, Guid TenantId);
public record GetVariantsByProductQuery(Guid ProductId, Guid TenantId, int Page = 1, int PageSize = 50);
public record GetVariantsQuery(
    Guid TenantId,
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 20);
public record SearchVariantsQuery(string Query, Guid TenantId, int Page = 1, int PageSize = 20);

/// <summary>
/// Queries for product operations
/// </summary>
public record GetProductByIdQuery(Guid Id, Guid TenantId);
public record GetProductsByTenantQuery(Guid TenantId, int Page = 1, int PageSize = 50);
public record GetProductsByCategoryQuery(Guid CategoryId, Guid TenantId, int Page = 1, int PageSize = 50);
public record GetProductBySkuQuery(string Sku, Guid TenantId);
