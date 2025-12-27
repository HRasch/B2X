using Wolverine;

namespace B2Connect.Admin.Application.Commands.Categories;

/// <summary>
/// Category Create Command - CQRS Pattern
/// </summary>
public record CreateCategoryCommand(
    Guid TenantId,
    string Name,
    string Slug,
    string? Description = null,
    Guid? ParentId = null) : IRequest<CategoryResult>;

/// <summary>
/// Category Update Command - CQRS Pattern
/// </summary>
public record UpdateCategoryCommand(
    Guid TenantId,
    Guid CategoryId,
    string Name,
    string Slug,
    string? Description = null,
    Guid? ParentId = null) : IRequest<CategoryResult>;

/// <summary>
/// Category Delete Command - CQRS Pattern
/// </summary>
public record DeleteCategoryCommand(Guid TenantId, Guid CategoryId) : IRequest<bool>;

// ─────────────────────────────────────────────────────────────────────────────
// Queries
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Get Category by ID Query
/// </summary>
public record GetCategoryQuery(Guid TenantId, Guid CategoryId) : IRequest<CategoryResult?>;

/// <summary>
/// Get Category by Slug Query
/// </summary>
public record GetCategoryBySlugQuery(Guid TenantId, string Slug) : IRequest<CategoryResult?>;

/// <summary>
/// Get Root Categories Query
/// </summary>
public record GetRootCategoriesQuery(Guid TenantId) : IRequest<IEnumerable<CategoryResult>>;

/// <summary>
/// Get Child Categories Query
/// </summary>
public record GetChildCategoriesQuery(Guid TenantId, Guid ParentId) : IRequest<IEnumerable<CategoryResult>>;

/// <summary>
/// Get Category Hierarchy Query
/// </summary>
public record GetCategoryHierarchyQuery(Guid TenantId) : IRequest<IEnumerable<CategoryResult>>;

/// <summary>
/// Get Active Categories Query
/// </summary>
public record GetActiveCategoriesQuery(Guid TenantId) : IRequest<IEnumerable<CategoryResult>>;

/// <summary>
/// Category Result DTO
/// </summary>
public record CategoryResult(
    Guid Id,
    Guid TenantId,
    string Name,
    string Slug,
    string? Description = null,
    Guid? ParentId = null,
    DateTime CreatedAt = default);
