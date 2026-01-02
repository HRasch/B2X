using B2Connect.Tenancy.Models;
using B2Connect.Types.Domain;

namespace B2Connect.Tenancy.Handlers.Tenants;

#region DTOs

/// <summary>
/// DTO for tenant list items.
/// </summary>
public record TenantListItemDto(
    Guid Id,
    string Name,
    string Slug,
    TenantStatus Status,
    string? PrimaryDomain,
    int DomainCount,
    DateTime CreatedAt);

/// <summary>
/// DTO for tenant details.
/// </summary>
public record TenantDetailDto(
    Guid Id,
    string Name,
    string Slug,
    TenantStatus Status,
    string? LogoUrl,
    string? SuspensionReason,
    DateTime? SuspendedAt,
    Dictionary<string, string> Metadata,
    IReadOnlyList<TenantDomainDto> Domains,
    DateTime CreatedAt,
    DateTime UpdatedAt);

/// <summary>
/// DTO for tenant domain.
/// </summary>
public record TenantDomainDto(
    Guid Id,
    string DomainName,
    DomainType Type,
    bool IsPrimary,
    DomainVerificationStatus VerificationStatus,
    SslStatus SslStatus,
    bool IsActive,
    DateTime CreatedAt);

/// <summary>
/// Paginated result wrapper.
/// </summary>
public record PagedResultDto<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages);

#endregion

#region Commands

/// <summary>
/// Command to create a new tenant.
/// </summary>
public record CreateTenantCommand(
    string Name,
    string Slug,
    string? LogoUrl = null,
    Dictionary<string, string>? Metadata = null);

/// <summary>
/// Response from creating a tenant.
/// </summary>
public record CreateTenantResponse(
    Guid TenantId,
    string Slug,
    string PrimaryDomain);

/// <summary>
/// Command to update a tenant.
/// </summary>
public record UpdateTenantCommand(
    Guid TenantId,
    string? Name = null,
    string? LogoUrl = null,
    TenantStatus? Status = null,
    string? SuspensionReason = null,
    Dictionary<string, string>? Metadata = null);

/// <summary>
/// Command to archive (soft-delete) a tenant.
/// </summary>
public record ArchiveTenantCommand(Guid TenantId);

#endregion

#region Queries

/// <summary>
/// Query to get a tenant by ID.
/// </summary>
public record GetTenantQuery(Guid TenantId);

/// <summary>
/// Query to get all tenants with filtering and pagination.
/// </summary>
public record GetTenantsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    TenantStatus? Status = null,
    string? SearchTerm = null);

#endregion
