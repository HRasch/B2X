namespace B2Connect.Shared.User.Models;

/// <summary>
/// Interface für Entitäten, die tenant-spezifische Erweiterungen unterstützen
/// Ermöglicht flexible Anpassung ohne Schema-Änderungen
/// </summary>
public interface IExtensibleEntity
{
    /// <summary>
    /// Tenant-ID für Isolation
    /// </summary>
    Guid TenantId { get; set; }

    /// <summary>
    /// JSON-Feld für tenant-spezifische Custom Properties
    /// Format: { "fieldName": "value", "erp_customer_id": "123456" }
    /// </summary>
    string? CustomProperties { get; set; }

    /// <summary>
    /// Versionierung für Audit Trail
    /// </summary>
    int Version { get; set; }
}

/// <summary>
/// Service zur Verwaltung von Custom Properties einer Entity
/// </summary>
public interface IEntityExtensionService
{
    /// <summary>
    /// Get custom property value
    /// </summary>
    T? GetCustomProperty<T>(IExtensibleEntity entity, string propertyName);

    /// <summary>
    /// Set custom property value (updates CustomProperties JSON)
    /// </summary>
    void SetCustomProperty<T>(IExtensibleEntity entity, string propertyName, T? value);

    /// <summary>
    /// Get all custom properties as dictionary
    /// </summary>
    Dictionary<string, object?> GetAllCustomProperties(IExtensibleEntity entity);

    /// <summary>
    /// Clear all custom properties
    /// </summary>
    void ClearCustomProperties(IExtensibleEntity entity);

    /// <summary>
    /// Validate custom property against tenant's extension schema
    /// </summary>
    Task<bool> ValidateCustomPropertyAsync(Guid tenantId, string entityType,
        string propertyName, object? value);
}
