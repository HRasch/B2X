using B2X.Types.Localization;

namespace B2X.Admin.Application.DTOs;

/// <summary>
/// Request DTO for creating a product
/// Used in POST /api/products
/// </summary>
public record CreateProductRequest(
    string Sku,
    string Name,
    string Description,
    decimal Price,
    decimal? B2bPrice,
    int StockQuantity,
    string[] Tags,
    Dictionary<string, LocalizedContent> LocalizedNames,
    Guid? CategoryId = null,
    Guid? BrandId = null);

/// <summary>
/// Request DTO for updating a product
/// Used in PUT /api/products/{id}
/// All properties are optional for partial updates
/// </summary>
public record UpdateProductRequest(
    string? Sku,
    string? Name,
    string? Description,
    decimal? Price,
    decimal? B2bPrice,
    int? StockQuantity,
    string[]? Tags,
    Dictionary<string, LocalizedContent>? LocalizedNames,
    Guid? CategoryId = null,
    Guid? BrandId = null);

/// <summary>
/// Request DTO for creating a category
/// Used in POST /api/categories
/// </summary>
public record CreateCategoryRequest(
    string Name,
    Dictionary<string, LocalizedContent> LocalizedNames);

/// <summary>
/// Request DTO for updating a category
/// Used in PUT /api/categories/{id}
/// </summary>
public record UpdateCategoryRequest(
    string? Name,
    Dictionary<string, LocalizedContent>? LocalizedNames);

/// <summary>
/// Request DTO for creating a brand
/// Used in POST /api/brands
/// </summary>
public record CreateBrandRequest(
    string Name,
    Dictionary<string, LocalizedContent> LocalizedNames);

/// <summary>
/// Request DTO for updating a brand
/// Used in PUT /api/brands/{id}
/// </summary>
public record UpdateBrandRequest(
    string? Name,
    Dictionary<string, LocalizedContent>? LocalizedNames);
