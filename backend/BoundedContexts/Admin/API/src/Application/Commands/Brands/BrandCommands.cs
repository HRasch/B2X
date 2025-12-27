using Wolverine;

namespace B2Connect.Admin.Application.Commands.Brands;

/// <summary>
/// Brand Create Command - CQRS Pattern
/// </summary>
public record CreateBrandCommand(
    Guid TenantId,
    string Name,
    string Slug,
    string? Logo = null,
    string? Description = null) : IRequest<BrandResult>;

/// <summary>
/// Brand Update Command - CQRS Pattern
/// </summary>
public record UpdateBrandCommand(
    Guid TenantId,
    Guid BrandId,
    string Name,
    string Slug,
    string? Logo = null,
    string? Description = null) : IRequest<BrandResult>;

/// <summary>
/// Brand Delete Command - CQRS Pattern
/// </summary>
public record DeleteBrandCommand(Guid TenantId, Guid BrandId) : IRequest<bool>;

// ─────────────────────────────────────────────────────────────────────────────
// Queries
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Get Brand by ID Query
/// </summary>
public record GetBrandQuery(Guid TenantId, Guid BrandId) : IRequest<BrandResult?>;

/// <summary>
/// Get Brand by Slug Query
/// </summary>
public record GetBrandBySlugQuery(Guid TenantId, string Slug) : IRequest<BrandResult?>;

/// <summary>
/// Get Active Brands Query
/// </summary>
public record GetActiveBrandsQuery(Guid TenantId) : IRequest<IEnumerable<BrandResult>>;

/// <summary>
/// Get Brands Paged Query
/// </summary>
public record GetBrandsPagedQuery(
    Guid TenantId,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<(IEnumerable<BrandResult> Items, int Total)>;

/// <summary>
/// Brand Result DTO
/// </summary>
public record BrandResult(
    Guid Id,
    Guid TenantId,
    string Name,
    string Slug,
    string? Logo = null,
    string? Description = null,
    DateTime CreatedAt = default);
