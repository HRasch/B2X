namespace B2Connect.Shared.Tenancy.Infrastructure.Context;

/// <summary>
/// Scoped service that holds the current tenant ID for the request.
/// Set by middleware from X-Tenant-ID header.
/// Injected into DbContext for automatic query filtering.
/// </summary>
public class TenantContext : ITenantContext
{
    /// <summary>
    /// Gets or sets the current tenant ID.
    /// </summary>
    public Guid TenantId { get; set; } = Guid.Empty;
}
