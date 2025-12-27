using System.Text.Json;
using System.Text.RegularExpressions;

namespace B2Connect.Shared.User.Services;

/// <summary>
/// Service zur Verwaltung von tenant-spezifischen Entity Extensions
/// Handlert JSON-Serialisierung, Validierung und Audit Logging
/// </summary>
public class EntityExtensionService : IEntityExtensionService
{
    private readonly IExtensionSchemaRepository _schemaRepository;
    private readonly IExtensionAuditRepository _auditRepository;
    private readonly ILogger<EntityExtensionService> _logger;

    public EntityExtensionService(
        IExtensionSchemaRepository schemaRepository,
        IExtensionAuditRepository auditRepository,
        ILogger<EntityExtensionService> logger)
    {
        _schemaRepository = schemaRepository;
        _auditRepository = auditRepository;
        _logger = logger;
    }

    public T? GetCustomProperty<T>(IExtensibleEntity entity, string propertyName)
    {
        if (string.IsNullOrEmpty(entity.CustomProperties))
            return default;

        try
        {
            var properties = JsonDocument.Parse(entity.CustomProperties);

            if (!properties.RootElement.TryGetProperty(propertyName, out var element))
                return default;

            return element.Deserialize<T>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize custom property '{PropertyName}'", propertyName);
            return default;
        }
    }

    public void SetCustomProperty<T>(IExtensibleEntity entity, string propertyName, T? value)
    {
        var properties = string.IsNullOrEmpty(entity.CustomProperties)
            ? new Dictionary<string, object?>()
            : JsonSerializer.Deserialize<Dictionary<string, object?>>(entity.CustomProperties)
              ?? new Dictionary<string, object?>();

        properties[propertyName] = value;
        entity.CustomProperties = JsonSerializer.Serialize(properties, new JsonSerializerOptions
        {
            WriteIndented = false
        });

        // Increment version für Optimistic Concurrency
        entity.Version++;

        _logger.LogInformation(
            "Custom property '{PropertyName}' set for entity {EntityId}",
            propertyName, entity.TenantId);
    }

    public Dictionary<string, object?> GetAllCustomProperties(IExtensibleEntity entity)
    {
        if (string.IsNullOrEmpty(entity.CustomProperties))
            return new Dictionary<string, object?>();

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object?>>(entity.CustomProperties)
                ?? new Dictionary<string, object?>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize all custom properties");
            return new Dictionary<string, object?>();
        }
    }

    public void ClearCustomProperties(IExtensibleEntity entity)
    {
        entity.CustomProperties = null;
        entity.Version++;
    }

    public async Task<bool> ValidateCustomPropertyAsync(Guid tenantId, string entityType,
        string propertyName, object? value)
    {
        // Hole das Schema für dieses Feld
        var schema = await _schemaRepository.GetSchemaAsync(tenantId, entityType, propertyName);

        if (schema == null)
        {
            _logger.LogWarning(
                "No schema found for custom property '{PropertyName}' on '{EntityType}' for tenant {TenantId}",
                propertyName, entityType, tenantId);
            return false;
        }

        if (!schema.IsActive)
        {
            _logger.LogWarning(
                "Custom property '{PropertyName}' is not active for tenant {TenantId}",
                propertyName, tenantId);
            return false;
        }

        // Null-Wert Validierung
        if (value == null)
        {
            if (schema.IsRequired)
            {
                _logger.LogWarning(
                    "Required custom property '{PropertyName}' cannot be null",
                    propertyName);
                return false;
            }
            return true;
        }

        // Datentyp-Validierung
        if (!ValidateDataType(schema.DataType, value))
        {
            _logger.LogWarning(
                "Custom property '{PropertyName}' has invalid data type. Expected {ExpectedType}",
                propertyName, schema.DataType);
            return false;
        }

        // String-Length Validierung
        if (schema.DataType == "string" && schema.MaxLength.HasValue)
        {
            var stringValue = value.ToString() ?? "";
            if (stringValue.Length > schema.MaxLength.Value)
            {
                _logger.LogWarning(
                    "Custom property '{PropertyName}' exceeds max length {MaxLength}",
                    propertyName, schema.MaxLength);
                return false;
            }
        }

        // Regex-Pattern Validierung
        if (!string.IsNullOrEmpty(schema.ValidationPattern))
        {
            var stringValue = value.ToString() ?? "";
            if (!Regex.IsMatch(stringValue, schema.ValidationPattern))
            {
                _logger.LogWarning(
                    "Custom property '{PropertyName}' does not match validation pattern",
                    propertyName);
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Audit eine Custom Property Change
    /// </summary>
    public async Task LogCustomPropertyChangeAsync(
        Guid entityId,
        Guid tenantId,
        string entityTypeName,
        string fieldName,
        object? oldValue,
        object? newValue,
        Guid? changedBy = null,
        string? reason = null)
    {
        var auditLog = new EntityExtensionAuditLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EntityId = entityId,
            EntityTypeName = entityTypeName,
            FieldName = fieldName,
            OldValue = oldValue?.ToString(),
            NewValue = newValue?.ToString(),
            ChangedBy = changedBy,
            Reason = reason,
            ChangedAt = DateTime.UtcNow
        };

        await _auditRepository.AddAsync(auditLog);
        await _auditRepository.SaveChangesAsync();

        _logger.LogInformation(
            "Custom property '{FieldName}' changed for {EntityType} {EntityId} by {UserId}",
            fieldName, entityTypeName, entityId, changedBy ?? Guid.Empty);
    }

    private static bool ValidateDataType(string dataType, object value)
    {
        return dataType.ToLowerInvariant() switch
        {
            "string" => value is string,
            "int" or "integer" => value is int or long,
            "decimal" => value is decimal or double or int,
            "datetime" => value is DateTime,
            "bool" or "boolean" => value is bool,
            "guid" => value is Guid or string,
            _ => false
        };
    }
}

/// <summary>
/// Repository für Extension Schemas
/// </summary>
public interface IExtensionSchemaRepository
{
    Task<EntityExtensionSchema?> GetSchemaAsync(
        Guid tenantId,
        string entityTypeName,
        string fieldName);

    Task<IEnumerable<EntityExtensionSchema>> GetSchemasForEntityAsync(
        Guid tenantId,
        string entityTypeName);

    Task<IEnumerable<EntityExtensionSchema>> GetSchemasForTenantAsync(Guid tenantId);

    Task AddAsync(EntityExtensionSchema schema);
    Task UpdateAsync(EntityExtensionSchema schema);
    Task DeleteAsync(Guid schemaId);
    Task SaveChangesAsync();
}

/// <summary>
/// Repository für Extension Audit Logs
/// </summary>
public interface IExtensionAuditRepository
{
    Task<IEnumerable<EntityExtensionAuditLog>> GetAuditLogsForEntityAsync(
        Guid entityId,
        string entityTypeName);

    Task<IEnumerable<EntityExtensionAuditLog>> GetAuditLogsForTenantAsync(Guid tenantId);

    Task AddAsync(EntityExtensionAuditLog auditLog);
    Task SaveChangesAsync();
}
