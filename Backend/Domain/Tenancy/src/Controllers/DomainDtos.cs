namespace B2Connect.Tenancy.Controllers;

/// <summary>
/// Request DTO for adding a domain to a tenant.
/// </summary>
public record AddDomainRequest(
    string DomainName,
    bool SetAsPrimary = false);
