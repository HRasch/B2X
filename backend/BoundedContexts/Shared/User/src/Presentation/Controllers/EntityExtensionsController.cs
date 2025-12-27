using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace B2Connect.Shared.User.Presentation.Controllers;

/// <summary>
/// API für Entity Extensions Management
/// Admin-only Endpoints
/// </summary>
[ApiController]
[Route("api/admin/extensions")]
[Authorize(Roles = "Admin")]
public class EntityExtensionsController : ControllerBase
{
    private readonly IEntityExtensionService _extensionService;
    private readonly IExtensionSchemaRepository _schemaRepository;
    private readonly IExtensionAuditRepository _auditRepository;
    private readonly ILogger<EntityExtensionsController> _logger;

    public EntityExtensionsController(
        IEntityExtensionService extensionService,
        IExtensionSchemaRepository schemaRepository,
        IExtensionAuditRepository auditRepository,
        ILogger<EntityExtensionsController> logger)
    {
        _extensionService = extensionService;
        _schemaRepository = schemaRepository;
        _auditRepository = auditRepository;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/admin/extensions/schemas/{entityType}
    /// Hole alle Extension Schemas für einen Entity Type
    /// </summary>
    [HttpGet("schemas/{entityType}")]
    public async Task<ActionResult<IEnumerable<EntityExtensionSchemaDto>>> GetSchemas(
        string entityType,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        var schemas = await _schemaRepository.GetSchemasForEntityAsync(tenantId, entityType);
        var dtos = schemas.Select(s => MapToDto(s)).ToList();

        return Ok(dtos);
    }

    /// <summary>
    /// POST /api/admin/extensions/schemas
    /// Erstelle ein neues Extension Schema
    /// </summary>
    [HttpPost("schemas")]
    public async Task<ActionResult<EntityExtensionSchemaDto>> CreateSchema(
        [FromBody] CreateExtensionSchemaRequest request,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        var schema = new EntityExtensionSchema
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EntityTypeName = request.EntityTypeName,
            FieldName = request.FieldName,
            DataType = request.DataType,
            DisplayName = request.DisplayName,
            Description = request.Description,
            IsRequired = request.IsRequired,
            MaxLength = request.MaxLength,
            ValidationPattern = request.ValidationPattern,
            DefaultValue = request.DefaultValue,
            IsVisibleToUsers = request.IsVisibleToUsers,
            IsEditable = request.IsEditable,
            IntegrationSource = request.IntegrationSource,
            MappingPath = request.MappingPath,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _schemaRepository.AddAsync(schema);
        await _schemaRepository.SaveChangesAsync();

        _logger.LogInformation(
            "Created extension schema '{FieldName}' for {EntityType} in tenant {TenantId}",
            schema.FieldName, schema.EntityTypeName, tenantId);

        return CreatedAtAction(nameof(GetSchemas),
            new { entityType = schema.EntityTypeName },
            MapToDto(schema));
    }

    /// <summary>
    /// PUT /api/admin/extensions/schemas/{schemaId}
    /// Update ein Extension Schema
    /// </summary>
    [HttpPut("schemas/{schemaId}")]
    public async Task<ActionResult<EntityExtensionSchemaDto>> UpdateSchema(
        Guid schemaId,
        [FromBody] UpdateExtensionSchemaRequest request,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        var schema = await _schemaRepository.GetSchemaAsync(
            tenantId,
            request.EntityTypeName,
            request.FieldName);

        if (schema == null)
            return NotFound("Extension schema not found");

        schema.DisplayName = request.DisplayName;
        schema.Description = request.Description;
        schema.IsRequired = request.IsRequired;
        schema.MaxLength = request.MaxLength;
        schema.ValidationPattern = request.ValidationPattern;
        schema.DefaultValue = request.DefaultValue;
        schema.IsVisibleToUsers = request.IsVisibleToUsers;
        schema.IsEditable = request.IsEditable;
        schema.IsActive = request.IsActive;
        schema.UpdatedAt = DateTime.UtcNow;

        await _schemaRepository.UpdateAsync(schema);
        await _schemaRepository.SaveChangesAsync();

        _logger.LogInformation(
            "Updated extension schema '{FieldName}' in tenant {TenantId}",
            schema.FieldName, tenantId);

        return Ok(MapToDto(schema));
    }

    /// <summary>
    /// DELETE /api/admin/extensions/schemas/{schemaId}
    /// Lösche ein Extension Schema
    /// </summary>
    [HttpDelete("schemas/{schemaId}")]
    public async Task<IActionResult> DeleteSchema(
        Guid schemaId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        // Hinweis: Stelle sicher, dass keine Daten mit diesem Schema mehr existieren
        // Könnte alternative zu Hard-Delete sein: IsActive = false setzen

        await _schemaRepository.DeleteAsync(schemaId);
        await _schemaRepository.SaveChangesAsync();

        _logger.LogInformation("Deleted extension schema {SchemaId} from tenant {TenantId}",
            schemaId, tenantId);

        return NoContent();
    }

    /// <summary>
    /// GET /api/admin/extensions/audit-logs/{entityId}/{entityType}
    /// Hole Audit-Logs für Custom Property Changes
    /// </summary>
    [HttpGet("audit-logs/{entityId}/{entityType}")]
    public async Task<ActionResult<IEnumerable<EntityExtensionAuditLogDto>>> GetAuditLogs(
        Guid entityId,
        string entityType,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        var logs = await _auditRepository.GetAuditLogsForEntityAsync(entityId, entityType);
        var dtos = logs.Select(l => MapToDto(l)).ToList();

        return Ok(dtos);
    }

    // Private Mapping Methods
    private EntityExtensionSchemaDto MapToDto(EntityExtensionSchema schema)
    {
        return new EntityExtensionSchemaDto
        {
            Id = schema.Id,
            EntityTypeName = schema.EntityTypeName,
            FieldName = schema.FieldName,
            DataType = schema.DataType,
            DisplayName = schema.DisplayName,
            Description = schema.Description,
            IsRequired = schema.IsRequired,
            MaxLength = schema.MaxLength,
            ValidationPattern = schema.ValidationPattern,
            DefaultValue = schema.DefaultValue,
            IsVisibleToUsers = schema.IsVisibleToUsers,
            IsEditable = schema.IsEditable,
            IntegrationSource = schema.IntegrationSource,
            MappingPath = schema.MappingPath,
            IsActive = schema.IsActive,
            CreatedAt = schema.CreatedAt,
            UpdatedAt = schema.UpdatedAt
        };
    }

    private EntityExtensionAuditLogDto MapToDto(EntityExtensionAuditLog log)
    {
        return new EntityExtensionAuditLogDto
        {
            Id = log.Id,
            EntityId = log.EntityId,
            EntityTypeName = log.EntityTypeName,
            FieldName = log.FieldName,
            OldValue = log.OldValue,
            NewValue = log.NewValue,
            ChangedAt = log.ChangedAt,
            Reason = log.Reason
        };
    }
}

// ============ DTOs ============

public class EntityExtensionSchemaDto
{
    public Guid Id { get; set; }
    public string EntityTypeName { get; set; } = "";
    public string FieldName { get; set; } = "";
    public string DataType { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string? Description { get; set; }
    public bool IsRequired { get; set; }
    public int? MaxLength { get; set; }
    public string? ValidationPattern { get; set; }
    public string? DefaultValue { get; set; }
    public bool IsVisibleToUsers { get; set; }
    public bool IsEditable { get; set; }
    public string? IntegrationSource { get; set; }
    public string? MappingPath { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class EntityExtensionAuditLogDto
{
    public Guid Id { get; set; }
    public Guid EntityId { get; set; }
    public string EntityTypeName { get; set; } = "";
    public string FieldName { get; set; } = "";
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? Reason { get; set; }
}

// ============ Requests ============

public class CreateExtensionSchemaRequest
{
    public string EntityTypeName { get; set; } = "";
    public string FieldName { get; set; } = "";
    public string DataType { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string? Description { get; set; }
    public bool IsRequired { get; set; } = false;
    public int? MaxLength { get; set; }
    public string? ValidationPattern { get; set; }
    public string? DefaultValue { get; set; }
    public bool IsVisibleToUsers { get; set; } = true;
    public bool IsEditable { get; set; } = true;
    public string? IntegrationSource { get; set; }
    public string? MappingPath { get; set; }
}

public class UpdateExtensionSchemaRequest
{
    public string EntityTypeName { get; set; } = "";
    public string FieldName { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string? Description { get; set; }
    public bool IsRequired { get; set; }
    public int? MaxLength { get; set; }
    public string? ValidationPattern { get; set; }
    public string? DefaultValue { get; set; }
    public bool IsVisibleToUsers { get; set; }
    public bool IsEditable { get; set; }
    public bool IsActive { get; set; }
}
