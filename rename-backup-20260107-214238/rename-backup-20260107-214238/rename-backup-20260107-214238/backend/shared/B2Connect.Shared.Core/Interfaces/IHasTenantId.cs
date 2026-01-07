namespace B2X.Shared.Core.Interfaces;

/// <summary>
/// Marker interface for entities that have tenant-level isolation.
/// Enables automatic Global Query Filter in EF Core DbContext.
/// </summary>
public interface IHasTenantId
{
    /// <summary>
    /// Tenant ID for multi-tenant data isolation
    /// </summary>
    Guid TenantId { get; set; }
}
