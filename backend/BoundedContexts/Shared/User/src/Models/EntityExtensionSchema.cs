namespace B2Connect.Shared.User.Models;

/// <summary>
/// Definiert die erlaubten Custom Properties pro Entity und Tenant
/// z.B. "User" hat für Tenant "ACME Corp" Custom Field "erp_customer_id"
/// </summary>
public class EntityExtensionSchema
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }

    /// <summary>
    /// Name der Entity, die erweitert wird (z.B. "User", "Order", "Product")
    /// </summary>
    public required string EntityTypeName { get; set; }

    /// <summary>
    /// Name des Custom Fields (z.B. "erp_customer_id", "warehouse_code")
    /// </summary>
    public required string FieldName { get; set; }

    /// <summary>
    /// Datentyp: string, int, decimal, datetime, bool
    /// </summary>
    public required string DataType { get; set; }

    /// <summary>
    /// Menschenlesbarer Label für UI
    /// </summary>
    public required string DisplayName { get; set; }

    /// <summary>
    /// Ausführliche Beschreibung für Admins
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Ist das Feld erforderlich?
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// Maximale Länge (für strings)
    /// </summary>
    public int? MaxLength { get; set; }

    /// <summary>
    /// Regex-Pattern zur Validierung (optional)
    /// z.B. "^\d{6}$" für 6-stellige Kundennummern
    /// </summary>
    public string? ValidationPattern { get; set; }

    /// <summary>
    /// Default-Wert
    /// </summary>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Ist das Feld sichtbar für normale Benutzer?
    /// (oder nur für Admins)
    /// </summary>
    public bool IsVisibleToUsers { get; set; } = true;

    /// <summary>
    /// Ist das Feld editierbar?
    /// </summary>
    public bool IsEditable { get; set; } = true;

    /// <summary>
    /// Integrationstyp (z.B. "enventa_trade_erp", "sap", "custom")
    /// </summary>
    public string? IntegrationSource { get; set; }

    /// <summary>
    /// Zuordnung zum externen System
    /// z.B. "ERP.Customer.CustomerNumber"
    /// </summary>
    public string? MappingPath { get; set; }

    /// <summary>
    /// Ist das Feld aktiv/enabled?
    /// </summary>
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Audit Trail für Custom Property Changes
/// </summary>
public class EntityExtensionAuditLog
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EntityId { get; set; }
    public required string EntityTypeName { get; set; }
    public required string FieldName { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public Guid? ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string? Reason { get; set; }
}
