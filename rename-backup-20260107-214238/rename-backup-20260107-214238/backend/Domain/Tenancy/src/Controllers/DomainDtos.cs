namespace B2X.Tenancy.Controllers;

/// <summary>
/// Request DTO for adding a domain to a tenant.
/// </summary>
public record AddDomainRequest(
    string DomainName,
    bool SetAsPrimary = false);
