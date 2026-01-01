namespace B2Connect.Shared.Core;

/// <summary>
/// Marker interface for entities that belong to a tenant
/// </summary>
public interface ITenantEntity
{
    /// <summary>The tenant ID this entity belongs to</summary>
    Guid TenantId { get; set; }
}