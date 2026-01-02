using B2Connect.Tenancy.Models;
using B2Connect.Tenancy.Repositories;
using B2Connect.Tenancy.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wolverine.Attributes;

namespace B2Connect.Tenancy.Handlers.Domains;

/// <summary>
/// Wolverine handlers for domain management operations.
/// </summary>
public static class DomainHandlers
{
    private const string DefaultBaseDomain = "b2connect.de";
    private const string DefaultProxyHost = "proxy.b2connect.de";

    /// <summary>
    /// Handles adding a new domain to a tenant.
    /// </summary>
    [WolverineHandler]
    public static async Task<AddDomainResponse> Handle(
        AddDomainCommand command,
        ITenantRepository tenantRepository,
        ITenantDomainRepository domainRepository,
        IDomainLookupService domainLookupService,
        IConfiguration configuration,
        ILogger<AddDomainCommand> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding domain {Domain} to tenant {TenantId}",
            command.DomainName, command.TenantId);

        // Verify tenant exists
        var tenant = await tenantRepository.GetByIdAsync(command.TenantId, cancellationToken)
            ?? throw new KeyNotFoundException($"Tenant {command.TenantId} not found.");

        // Normalize domain name
        var domainName = command.DomainName.Trim().ToLowerInvariant();

        // Check if domain already exists
        if (await domainRepository.DomainExistsAsync(domainName, null, cancellationToken))
        {
            throw new InvalidOperationException($"Domain '{domainName}' is already in use.");
        }

        // Determine domain type
        var baseDomain = configuration.GetValue("Tenancy:BaseDomain", DefaultBaseDomain);
        var isSubdomain = domainName.EndsWith($".{baseDomain}", StringComparison.OrdinalIgnoreCase);

        var domain = new TenantDomain
        {
            Id = Guid.NewGuid(),
            TenantId = command.TenantId,
            DomainName = domainName,
            Type = isSubdomain ? DomainType.Subdomain : DomainType.CustomDomain,
            IsPrimary = false // Will be set after creation if needed
        };

        // Subdomains are auto-verified; custom domains need verification
        if (isSubdomain)
        {
            domain.VerificationStatus = DomainVerificationStatus.Verified;
            domain.VerifiedAt = DateTime.UtcNow;
            domain.SslStatus = SslStatus.Active; // Covered by wildcard cert
        }
        else
        {
            domain.GenerateVerificationToken();
            domain.SslStatus = SslStatus.None;
        }

        await domainRepository.CreateAsync(domain, cancellationToken);
        logger.LogInformation("Domain {Domain} created with ID {DomainId}", domainName, domain.Id);

        // Set as primary if requested
        if (command.SetAsPrimary)
        {
            await domainRepository.SetPrimaryAsync(command.TenantId, domain.Id, cancellationToken);
            logger.LogInformation("Domain {Domain} set as primary for tenant {TenantId}",
                domainName, command.TenantId);
        }

        // Invalidate cache
        await domainLookupService.InvalidateCacheAsync(domainName, cancellationToken);

        // Generate DNS instructions for custom domains
        DnsInstructionsDto? dnsInstructions = null;
        if (domain.Type == DomainType.CustomDomain)
        {
            var proxyHost = configuration.GetValue("Tenancy:ProxyHost", DefaultProxyHost);
            dnsInstructions = new DnsInstructionsDto(
                VerificationRecordType: "TXT",
                VerificationRecordName: $"_b2connect.{domainName}",
                VerificationRecordValue: domain.VerificationToken!,
                CnameRecordName: domainName,
                CnameRecordValue: proxyHost,
                TokenExpiresAt: domain.VerificationExpiresAt);
        }

        return new AddDomainResponse(
            domain.Id,
            domain.DomainName,
            domain.Type,
            domain.VerificationStatus,
            dnsInstructions);
    }

    /// <summary>
    /// Handles removing a domain from a tenant.
    /// </summary>
    [WolverineHandler]
    public static async Task<bool> Handle(
        RemoveDomainCommand command,
        ITenantDomainRepository domainRepository,
        IDomainLookupService domainLookupService,
        ILogger<RemoveDomainCommand> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing domain {DomainId} from tenant {TenantId}",
            command.DomainId, command.TenantId);

        var domain = await domainRepository.GetByIdAsync(command.DomainId, cancellationToken)
            ?? throw new KeyNotFoundException($"Domain {command.DomainId} not found.");

        // Verify domain belongs to tenant
        if (domain.TenantId != command.TenantId)
        {
            throw new UnauthorizedAccessException("Domain does not belong to the specified tenant.");
        }

        // Cannot remove the last domain
        var domains = await domainRepository.GetByTenantIdAsync(command.TenantId, cancellationToken);
        if (domains.Count <= 1)
        {
            throw new InvalidOperationException("Cannot remove the last domain. Tenant must have at least one domain.");
        }

        // If removing primary, set another as primary
        if (domain.IsPrimary)
        {
            var newPrimary = domains.FirstOrDefault(d => d.Id != command.DomainId);
            if (newPrimary != null)
            {
                await domainRepository.SetPrimaryAsync(command.TenantId, newPrimary.Id, cancellationToken);
            }
        }

        await domainRepository.DeleteAsync(command.DomainId, cancellationToken);
        logger.LogInformation("Domain {DomainId} removed", command.DomainId);

        // Invalidate cache
        await domainLookupService.InvalidateCacheAsync(domain.DomainName, cancellationToken);

        return true;
    }

    /// <summary>
    /// Handles verifying a domain's DNS configuration.
    /// </summary>
    [WolverineHandler]
    public static async Task<VerifyDomainResponse> Handle(
        VerifyDomainCommand command,
        ITenantDomainRepository domainRepository,
        IDnsVerificationService dnsVerificationService,
        IDomainLookupService domainLookupService,
        ILogger<VerifyDomainCommand> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Verifying domain {DomainId}", command.DomainId);

        var domain = await domainRepository.GetByIdAsync(command.DomainId, cancellationToken)
            ?? throw new KeyNotFoundException($"Domain {command.DomainId} not found.");

        // Check if already verified
        if (domain.VerificationStatus == DomainVerificationStatus.Verified)
        {
            return new VerifyDomainResponse(true, DomainVerificationStatus.Verified, "Domain is already verified.");
        }

        // Check if token expired
        if (domain.VerificationExpiresAt.HasValue && domain.VerificationExpiresAt.Value < DateTime.UtcNow)
        {
            // Generate new token
            domain.GenerateVerificationToken();
            await domainRepository.UpdateAsync(domain, cancellationToken);

            return new VerifyDomainResponse(
                false,
                DomainVerificationStatus.Failed,
                "Verification token expired. A new token has been generated. Please update your DNS record.");
        }

        // Verify DNS TXT record
        var verificationResult = await dnsVerificationService.VerifyDomainAsync(
            domain.DomainName,
            domain.VerificationToken!,
            cancellationToken);

        if (verificationResult.IsVerified)
        {
            domain.MarkAsVerified();
            domain.SslStatus = SslStatus.Provisioning; // Trigger SSL provisioning

            await domainRepository.UpdateAsync(domain, cancellationToken);
            logger.LogInformation("Domain {Domain} verified successfully", domain.DomainName);

            // Invalidate cache to reflect new status
            await domainLookupService.InvalidateCacheAsync(domain.DomainName, cancellationToken);

            return new VerifyDomainResponse(
                true,
                DomainVerificationStatus.Verified,
                "Domain verified successfully. SSL certificate is being provisioned.");
        }
        else
        {
            domain.MarkVerificationFailed();
            await domainRepository.UpdateAsync(domain, cancellationToken);

            logger.LogWarning("Domain {Domain} verification failed: {Reason}",
                domain.DomainName, verificationResult.FailureReason);

            return new VerifyDomainResponse(
                false,
                DomainVerificationStatus.Failed,
                verificationResult.FailureReason ?? "DNS verification failed. Please check your DNS configuration.");
        }
    }

    /// <summary>
    /// Handles setting a domain as primary.
    /// </summary>
    [WolverineHandler]
    public static async Task<bool> Handle(
        SetPrimaryDomainCommand command,
        ITenantDomainRepository domainRepository,
        ILogger<SetPrimaryDomainCommand> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting domain {DomainId} as primary for tenant {TenantId}",
            command.DomainId, command.TenantId);

        var domain = await domainRepository.GetByIdAsync(command.DomainId, cancellationToken)
            ?? throw new KeyNotFoundException($"Domain {command.DomainId} not found.");

        // Verify domain belongs to tenant
        if (domain.TenantId != command.TenantId)
        {
            throw new UnauthorizedAccessException("Domain does not belong to the specified tenant.");
        }

        // Domain must be verified to be primary
        if (domain.VerificationStatus != DomainVerificationStatus.Verified)
        {
            throw new InvalidOperationException("Only verified domains can be set as primary.");
        }

        await domainRepository.SetPrimaryAsync(command.TenantId, command.DomainId, cancellationToken);
        logger.LogInformation("Domain {DomainId} set as primary", command.DomainId);

        return true;
    }

    /// <summary>
    /// Handles getting all domains for a tenant.
    /// </summary>
    [WolverineHandler]
    public static async Task<IReadOnlyList<DomainDetailDto>> Handle(
        GetDomainsQuery query,
        ITenantDomainRepository domainRepository,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var domains = await domainRepository.GetByTenantIdAsync(query.TenantId, cancellationToken);
        var proxyHost = configuration.GetValue("Tenancy:ProxyHost", DefaultProxyHost);

        return domains.Select(d => MapToDetailDto(d, proxyHost)).ToList();
    }

    /// <summary>
    /// Handles getting a single domain by ID.
    /// </summary>
    [WolverineHandler]
    public static async Task<DomainDetailDto?> Handle(
        GetDomainQuery query,
        ITenantDomainRepository domainRepository,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var domain = await domainRepository.GetByIdAsync(query.DomainId, cancellationToken);
        if (domain == null)
        {
            return null;
        }

        var proxyHost = configuration.GetValue("Tenancy:ProxyHost", DefaultProxyHost);
        return MapToDetailDto(domain, proxyHost);
    }

    #region Private Helpers

    private static DomainDetailDto MapToDetailDto(TenantDomain domain, string proxyHost)
    {
        DnsInstructionsDto? dnsInstructions = null;

        // Only show DNS instructions for unverified custom domains
        if (domain.Type == DomainType.CustomDomain &&
            domain.VerificationStatus != DomainVerificationStatus.Verified &&
            !string.IsNullOrEmpty(domain.VerificationToken))
        {
            dnsInstructions = new DnsInstructionsDto(
                VerificationRecordType: "TXT",
                VerificationRecordName: $"_b2connect.{domain.DomainName}",
                VerificationRecordValue: domain.VerificationToken,
                CnameRecordName: domain.DomainName,
                CnameRecordValue: proxyHost,
                TokenExpiresAt: domain.VerificationExpiresAt);
        }

        return new DomainDetailDto(
            domain.Id,
            domain.TenantId,
            domain.DomainName,
            domain.Type,
            domain.IsPrimary,
            domain.VerificationStatus,
            domain.SslStatus,
            domain.IsActive,
            domain.CreatedAt,
            dnsInstructions);
    }

    #endregion
}
