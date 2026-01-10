using B2X.Tenancy.Models;

namespace B2X.Tenancy.Handlers.Domains;

#region DTOs

/// <summary>
/// DTO for domain details with DNS instructions.
/// </summary>
public record DomainDetailDto(
    Guid Id,
    Guid TenantId,
    string DomainName,
    DomainType Type,
    bool IsPrimary,
    DomainVerificationStatus VerificationStatus,
    SslStatus SslStatus,
    bool IsActive,
    DateTime CreatedAt,
    DnsInstructionsDto? DnsInstructions);

/// <summary>
/// DNS configuration instructions for custom domain setup.
/// </summary>
public record DnsInstructionsDto(
    string VerificationRecordType,
    string VerificationRecordName,
    string VerificationRecordValue,
    string CnameRecordName,
    string CnameRecordValue,
    DateTime? TokenExpiresAt);

/// <summary>
/// Response from adding a domain.
/// </summary>
public record AddDomainResponse(
    Guid DomainId,
    string DomainName,
    DomainType Type,
    DomainVerificationStatus VerificationStatus,
    DnsInstructionsDto? DnsInstructions);

/// <summary>
/// Response from domain verification.
/// </summary>
public record VerifyDomainResponse(
    bool Success,
    DomainVerificationStatus Status,
    string? Message);

#endregion

#region Commands

/// <summary>
/// Command to add a domain to a tenant.
/// </summary>
public record AddDomainCommand(
    Guid TenantId,
    string DomainName,
    bool SetAsPrimary = false);

/// <summary>
/// Command to remove a domain from a tenant.
/// </summary>
public record RemoveDomainCommand(
    Guid TenantId,
    Guid DomainId);

/// <summary>
/// Command to verify a domain's DNS configuration.
/// </summary>
public record VerifyDomainCommand(Guid DomainId);

/// <summary>
/// Command to set a domain as primary.
/// </summary>
public record SetPrimaryDomainCommand(
    Guid TenantId,
    Guid DomainId);

#endregion

#region Queries

/// <summary>
/// Query to get all domains for a tenant.
/// </summary>
public record GetDomainsQuery(Guid TenantId);

/// <summary>
/// Query to get a domain by ID.
/// </summary>
public record GetDomainQuery(Guid DomainId);

#endregion
