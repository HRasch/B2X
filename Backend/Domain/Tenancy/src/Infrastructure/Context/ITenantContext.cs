namespace B2X.Shared.Tenancy.Infrastructure.Context;

/// <summary>
/// Provides access to the current tenant context in the request scope.
/// Injected into DbContext to automatically filter queries by tenant.
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// Gets the current tenant ID for this request.
    /// </summary>
    Guid TenantId { get; }
}
