namespace B2X.Admin.Application.Commands.Categories;

/// <summary>
/// Category Commands & Queries - CQRS Pattern
///
/// TenantId wird automatisch via ITenantContextAccessor im Handler injiziert
/// </summary>

// ─────────────────────────────────────────────────────────────────────────────
// Commands
// ─────────────────────────────────────────────────────────────────────────────

public record CreateCategoryCommand(
    string Name,
    string Slug,
    string? Description = null,
    Guid? ParentId = null);

public record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    string Slug,
    string? Description = null,
    Guid? ParentId = null);

public record DeleteCategoryCommand(Guid CategoryId);

// ─────────────────────────────────────────────────────────────────────────────
// Queries
// ─────────────────────────────────────────────────────────────────────────────

public record GetCategoryQuery(Guid CategoryId);

public record GetCategoryBySlugQuery(string Slug);

public record GetRootCategoriesQuery();

public record GetChildCategoriesQuery(Guid ParentId);

public record GetCategoryHierarchyQuery();

public record GetActiveCategoriesQuery();

// ─────────────────────────────────────────────────────────────────────────────
// Result DTOs
// ─────────────────────────────────────────────────────────────────────────────

public record CategoryResult(
    Guid Id,
    Guid TenantId,
    string Name,
    string Slug,
    string? Description = null,
    Guid? ParentId = null,
    DateTime CreatedAt = default);
