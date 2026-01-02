using B2Connect.Tenancy.Models;
using B2Connect.Tenancy.Repositories;
using B2Connect.Tenancy.Services;
using B2Connect.Types.Domain;
using Microsoft.Extensions.Logging;
using Wolverine;
using Wolverine.Attributes;

namespace B2Connect.Tenancy.Handlers.Tenants;

/// <summary>
/// Wolverine handlers for tenant management operations.
/// </summary>
public static class TenantHandlers
{
    /// <summary>
    /// Handles creating a new tenant with automatic subdomain.
    /// </summary>
    [WolverineHandler]
    public static async Task<CreateTenantResponse> Handle(
        CreateTenantCommand command,
        ITenantRepository tenantRepository,
        ITenantDomainRepository domainRepository,
        IDomainLookupService domainLookupService,
        ILogger<CreateTenantCommand> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new tenant: {Name} with slug: {Slug}", command.Name, command.Slug);

        // Normalize slug
        var slug = command.Slug.Trim().ToLowerInvariant();

        // Validate slug uniqueness
        if (await tenantRepository.SlugExistsAsync(slug, null, cancellationToken))
        {
            throw new InvalidOperationException($"Slug '{slug}' is already in use.");
        }

        // Create tenant
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = command.Name.Trim(),
            Slug = slug,
            LogoUrl = command.LogoUrl,
            Status = TenantStatus.Active,
            Metadata = command.Metadata ?? []
        };

        await tenantRepository.CreateAsync(tenant, cancellationToken);
        logger.LogInformation("Tenant created: {TenantId}", tenant.Id);

        // Create automatic subdomain (primary)
        var subdomainName = $"{slug}.b2connect.de";

        // Check if subdomain is available
        if (await domainRepository.DomainExistsAsync(subdomainName, null, cancellationToken))
        {
            throw new InvalidOperationException($"Subdomain '{subdomainName}' is already in use.");
        }

        var primaryDomain = new TenantDomain
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            DomainName = subdomainName,
            Type = DomainType.Subdomain,
            IsPrimary = true,
            VerificationStatus = DomainVerificationStatus.Verified, // Subdomains are auto-verified
            VerifiedAt = DateTime.UtcNow,
            SslStatus = SslStatus.Active // Covered by wildcard cert
        };

        await domainRepository.CreateAsync(primaryDomain, cancellationToken);
        logger.LogInformation("Primary subdomain created: {Domain} for tenant: {TenantId}",
            subdomainName, tenant.Id);

        // Invalidate domain cache
        await domainLookupService.InvalidateCacheAsync(subdomainName, cancellationToken);

        return new CreateTenantResponse(tenant.Id, slug, subdomainName);
    }

    /// <summary>
    /// Handles updating an existing tenant.
    /// </summary>
    [WolverineHandler]
    public static async Task<TenantDetailDto> Handle(
        UpdateTenantCommand command,
        ITenantRepository tenantRepository,
        ITenantDomainRepository domainRepository,
        ILogger<UpdateTenantCommand> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating tenant: {TenantId}", command.TenantId);

        var tenant = await tenantRepository.GetByIdAsync(command.TenantId, cancellationToken)
            ?? throw new KeyNotFoundException($"Tenant {command.TenantId} not found.");

        // Apply updates
        if (!string.IsNullOrWhiteSpace(command.Name))
        {
            tenant.Name = command.Name.Trim();
        }

        if (command.LogoUrl != null)
        {
            tenant.LogoUrl = command.LogoUrl;
        }

        if (command.Status.HasValue)
        {
            tenant.Status = command.Status.Value;

            if (command.Status == TenantStatus.Suspended)
            {
                tenant.SuspendedAt = DateTime.UtcNow;
                tenant.SuspensionReason = command.SuspensionReason;
            }
            else if (command.Status == TenantStatus.Active)
            {
                tenant.SuspendedAt = null;
                tenant.SuspensionReason = null;
            }
        }

        if (command.Metadata != null)
        {
            tenant.Metadata = command.Metadata;
        }

        await tenantRepository.UpdateAsync(tenant, cancellationToken);
        logger.LogInformation("Tenant updated: {TenantId}", command.TenantId);

        // Get domains for response
        var domains = await domainRepository.GetByTenantIdAsync(tenant.Id, cancellationToken);

        return MapToDetailDto(tenant, domains);
    }

    /// <summary>
    /// Handles archiving a tenant.
    /// </summary>
    [WolverineHandler]
    public static async Task<bool> Handle(
        ArchiveTenantCommand command,
        ITenantRepository tenantRepository,
        ILogger<ArchiveTenantCommand> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Archiving tenant: {TenantId}", command.TenantId);

        var tenant = await tenantRepository.GetByIdAsync(command.TenantId, cancellationToken)
            ?? throw new KeyNotFoundException($"Tenant {command.TenantId} not found.");

        tenant.Status = TenantStatus.Archived;
        await tenantRepository.UpdateAsync(tenant, cancellationToken);

        logger.LogInformation("Tenant archived: {TenantId}", command.TenantId);
        return true;
    }

    /// <summary>
    /// Handles getting a tenant by ID.
    /// </summary>
    [WolverineHandler]
    public static async Task<TenantDetailDto?> Handle(
        GetTenantQuery query,
        ITenantRepository tenantRepository,
        ITenantDomainRepository domainRepository,
        CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetByIdAsync(query.TenantId, cancellationToken);
        if (tenant == null)
        {
            return null;
        }

        var domains = await domainRepository.GetByTenantIdAsync(tenant.Id, cancellationToken);
        return MapToDetailDto(tenant, domains);
    }

    /// <summary>
    /// Handles getting paginated list of tenants.
    /// </summary>
    [WolverineHandler]
    public static async Task<PagedResultDto<TenantListItemDto>> Handle(
        GetTenantsQuery query,
        ITenantRepository tenantRepository,
        ITenantDomainRepository domainRepository,
        CancellationToken cancellationToken)
    {
        var (tenants, totalCount) = await tenantRepository.GetPagedAsync(
            query.PageNumber,
            query.PageSize,
            query.Status,
            query.SearchTerm,
            cancellationToken);

        var items = new List<TenantListItemDto>();
        foreach (var tenant in tenants)
        {
            var domains = await domainRepository.GetByTenantIdAsync(tenant.Id, cancellationToken);
            var primaryDomain = domains.FirstOrDefault(d => d.IsPrimary);

            items.Add(new TenantListItemDto(
                tenant.Id,
                tenant.Name,
                tenant.Slug,
                tenant.Status,
                primaryDomain?.DomainName,
                domains.Count,
                tenant.CreatedAt));
        }

        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        return new PagedResultDto<TenantListItemDto>(
            items,
            totalCount,
            query.PageNumber,
            query.PageSize,
            totalPages);
    }

    #region Private Helpers

    private static TenantDetailDto MapToDetailDto(Tenant tenant, IReadOnlyList<TenantDomain> domains)
    {
        return new TenantDetailDto(
            tenant.Id,
            tenant.Name,
            tenant.Slug,
            tenant.Status,
            tenant.LogoUrl,
            tenant.SuspensionReason,
            tenant.SuspendedAt,
            tenant.Metadata,
            domains.Select(d => new TenantDomainDto(
                d.Id,
                d.DomainName,
                d.Type,
                d.IsPrimary,
                d.VerificationStatus,
                d.SslStatus,
                d.IsActive,
                d.CreatedAt)).ToList(),
            tenant.CreatedAt,
            tenant.UpdatedAt);
    }

    #endregion
}
