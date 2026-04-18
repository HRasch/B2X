namespace B2X.Admin.Application.Commands.Categories;

/// <summary>
/// Command to create a new category
/// </summary>
public record CreateCategoryCommand(
    Guid TenantId,
    string Name,
    string? Description,
    string Slug,
    Guid? ParentId,
    string? ImageUrl,
    string? Icon,
    int DisplayOrder,
    string? MetaTitle,
    string? MetaDescription,
    bool IsActive,
    bool IsVisible);

/// <summary>
/// Command to update an existing category
/// </summary>
public record UpdateCategoryCommand(
    Guid Id,
    Guid TenantId,
    string Name,
    string? Description,
    string Slug,
    Guid? ParentId,
    string? ImageUrl,
    string? Icon,
    int DisplayOrder,
    string? MetaTitle,
    string? MetaDescription,
    bool IsActive,
    bool IsVisible);

/// <summary>
/// Command to delete a category
/// </summary>
public record DeleteCategoryCommand(Guid Id, Guid TenantId);

/// <summary>
/// Query to get categories with pagination and filtering
/// </summary>
public record GetCategoriesQuery(Guid TenantId, string? SearchTerm = null, Guid? ParentId = null, bool? IsActive = null, int Page = 1, int PageSize = 20);

/// <summary>
/// Query to get a category by ID
/// </summary>
public record GetCategoryByIdQuery(Guid Id, Guid TenantId);

/// <summary>
/// Query to get a category by slug
/// </summary>
public record GetCategoryBySlugQuery(string Slug, Guid TenantId);

/// <summary>
/// Query to get root categories
/// </summary>
public record GetRootCategoriesQuery(Guid TenantId);

/// <summary>
/// Query to get child categories
/// </summary>
public record GetChildCategoriesQuery(Guid ParentId, Guid TenantId);

/// <summary>
/// Query to get category hierarchy/tree
/// </summary>
public record GetCategoryTreeQuery(Guid TenantId);

/// <summary>
/// Query to get active categories
/// </summary>
public record GetActiveCategoriesQuery(Guid TenantId);

/// <summary>
/// Query to search categories with pagination
/// </summary>
public record SearchCategoriesQuery(string SearchTerm, Guid TenantId, int Page = 1, int PageSize = 20);

/// <summary>
/// Query to get all categories with pagination
/// </summary>
public record GetAllCategoriesQuery(Guid TenantId, int Page = 1, int PageSize = 20);
