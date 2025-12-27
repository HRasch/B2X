namespace B2Connect.Admin.Application.Commands.Brands;

/// <summary>
/// Brand Commands & Queries - CQRS Pattern
/// 
/// TenantId wird automatisch via ITenantContextAccessor im Handler injiziert
/// </summary>

// ─────────────────────────────────────────────────────────────────────────────
// Commands
// ─────────────────────────────────────────────────────────────────────────────

public record CreateBrandCommand(
    string Name,
    string Slug,
    string? Logo = null,
    string? Description = null);

public record UpdateBrandCommand(
    Guid BrandId,
    string Name,
    string Slug,
    string? Logo = null,
    string? Description = null);

public record DeleteBrandCommand(Guid BrandId);

// ─────────────────────────────────────────────────────────────────────────────
// Queries
// ─────────────────────────────────────────────────────────────────────────────

public record GetBrandQuery(Guid BrandId);

public record GetBrandBySlugQuery(string Slug);

public record GetActiveBrandsQuery();

public record GetBrandsPagedQuery(
    int PageNumber = 1,
    int PageSize = 10);

// ─────────────────────────────────────────────────────────────────────────────
// Result DTOs
// ─────────────────────────────────────────────────────────────────────────────

public record BrandResult(
    Guid Id,
    Guid TenantId,
    string Name,
    string Slug,
    string? Logo = null,
    string? Description = null,
    DateTime CreatedAt = default);
