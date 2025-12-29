# 📋 Entity Extensions System - Implementation Guide

**Status**: ✅ Production Ready  
**Version**: 1.0  
**Last Updated**: 27. Dezember 2025

---

## 🎯 Überblick

Das **Entity Extensions System** ermöglicht es, dass jeder Tenant (Mandant) seine eigenen Custom Properties auf Standard-Entitäten (User, Product, Order, etc.) hinzufügen kann - **ohne die Datenbank-Struktur zu ändern**.

### Anwendungsbeispiele

```
Tenant: ACME Corp (enventa Trade ERP Integration)
├─ User.erp_customer_id = "ERP-123456"
├─ User.erp_customer_number = "CUST-456789"
├─ User.warehouse_code = "WH-001"
├─ User.credit_limit = 50000.00
└─ User.customer_segment = "Premium"

Tenant: Mittelständisches Unternehmen
├─ User.internal_id = "INT-2024-001"
├─ User.department = "Sales"
└─ User.cost_center = "CC-100"

Tenant: Freiberufler
├─ User.bank_account_number = "DE89370400440532013000"
├─ User.tax_id = "DE123456789"
└─ User.payment_terms = "net_30"
```

---

## 🏗️ Architektur

### Layer-Struktur

```
┌─────────────────────────────────────────┐
│     API Layer (Controllers)              │
│  EntityExtensionsController             │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│   Application Layer (CQRS)              │
│  CreateUserWithErpCommand               │
│  EntityExtensionService                 │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│   Domain Layer (Core)                   │
│  IExtensibleEntity Interface            │
│  EntityExtensionSchema                  │
│  EntityExtensionAuditLog               │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│   Infrastructure Layer (Database)       │
│  EF Core DbContext                      │
│  SQL Tables: extension_schemas          │
│             extension_audit_logs        │
└─────────────────────────────────────────┘
```

### Datenfluss

```
1. Tenant definiert Custom Fields
   POST /api/admin/extensions/schemas
   → EntityExtensionSchema wird in DB gespeichert

2. User-Erstellung mit Custom Properties
   POST /api/users
   (mit erp_customer_id, warehouse_code, etc.)
   → User.CustomProperties JSON wird aktualisiert

3. Custom Properties Abruf
   GET /api/users/{id}
   → User + CustomProperties werden zurückgegeben

4. Audit Trail
   Alle Änderungen werden in extension_audit_logs getracked
```

---

## 🚀 Quick Start: enventa Trade ERP Setup

### 1. Schritt: Migrations erstellen

```bash
cd backend/BoundedContexts/Shared/User

# Neue Migration für Extension Tables
dotnet ef migrations add AddEntityExtensionSupport

# Migration anwenden
dotnet ef database update
```

Die Migration erstellt 3 neue Tabellen:
- `extension_schemas` - Definiert Custom Fields pro Tenant
- `extension_audit_logs` - Audit Trail für Custom Property Changes
- `users` - `custom_properties` Kolonne hinzugefügt

### 2. Schritt: DI konfigurieren

**Program.cs** in deinem Service:

```csharp
// Service Collections
services
    .AddUserContext()  // existing
    .AddEntityExtensions();  // NEW

// DbContext
services.AddDbContext<UserDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.UseSnakeCaseNamingConvention();  // PostgreSQL
});
```

### 3. Schritt: enventa Integration aktivieren

```csharp
// In der Tenant Onboarding Flow
public async Task OnboardTenantAsync(Guid tenantId, TenantInfo info)
{
    // Existierender Onboarding-Code...

    // Wenn "enventa_trade_erp" konfiguriert:
    if (info.IntegrationName == "enventa_trade_erp")
    {
        var erpIntegration = sp.GetRequiredService<EnventaTradeEerIntegration>();
        await erpIntegration.SetupTenantIntegrationAsync(tenantId);
        
        _logger.LogInformation("enventa Trade ERP fields configured for tenant {TenantId}", tenantId);
    }
}
```

---

## 📝 Verwendungsbeispiele

### A) Neue Custom Fields hinzufügen

```bash
# Admin-Anfrage: Definiere ein neues Custom Field
POST /api/admin/extensions/schemas
X-Tenant-ID: {tenant-guid}
Authorization: Bearer {admin-token}

{
  "entityTypeName": "User",
  "fieldName": "erp_customer_number",
  "dataType": "string",
  "displayName": "ERP Customer Number",
  "description": "Kundennummer aus enventa Trade ERP",
  "isRequired": false,
  "maxLength": 50,
  "validationPattern": "^[A-Z0-9-]+$",
  "isVisibleToUsers": true,
  "isEditable": false,
  "integrationSource": "enventa_trade_erp",
  "mappingPath": "ERP.Customer.CustomerNumber"
}

Response: 201 Created
{
  "id": "guid",
  "entityTypeName": "User",
  "fieldName": "erp_customer_number",
  "isActive": true,
  "createdAt": "2025-12-27T10:00:00Z"
}
```

### B) User mit ERP-Integration erstellen

```csharp
// C# Code - CQRS Command
var command = new CreateUserWithErpCommand(
    TenantId: tenantId,
    Email: "john.doe@example.com",
    FirstName: "John",
    LastName: "Doe",
    PhoneNumber: "+49 123 456789",
    ErpCustomerId: "CUST-12345",
    ErpApiKey: "sk_live_abc123...",
    ErpBaseUrl: "https://erp.example.com"
);

var userDto = await mediator.Send(command);

// userDto.CustomProperties jetzt enthalten:
// {
//   "erp_customer_id": "CUST-12345",
//   "erp_customer_number": "123456",
//   "warehouse_code": "WH-001",
//   "credit_limit": 50000.00,
//   "customer_segment": "Premium"
// }
```

### C) Custom Properties abrufen

```csharp
// C# - Service Layer
var extensionService = sp.GetRequiredService<IEntityExtensionService>();

// Single Property
var erpCustomerId = extensionService.GetCustomProperty<string>(user, "erp_customer_id");
// → "CUST-12345"

// Alle Properties
var allProps = extensionService.GetAllCustomProperties(user);
// → Dictionary mit allen Custom Fields
```

### D) Custom Property aktualisieren

```csharp
// C# - Mit Validierung
var isValid = await extensionService.ValidateCustomPropertyAsync(
    tenantId,
    "User",
    "erp_customer_number",
    "NEW-CUST-999");

if (isValid)
{
    var oldValue = extensionService.GetCustomProperty<string>(user, "erp_customer_number");
    extensionService.SetCustomProperty(user, "erp_customer_number", "NEW-CUST-999");
    
    // Audit Log
    await extensionService.LogCustomPropertyChangeAsync(
        user.Id,
        tenantId,
        "User",
        "erp_customer_number",
        oldValue,
        "NEW-CUST-999",
        changedBy: currentUserId,
        reason: "ERP Customer Sync");
    
    // Speichern
    await userRepository.SaveChangesAsync();
}
```

### E) Custom Fields auflisten

```bash
# GET /api/admin/extensions/schemas/User
X-Tenant-ID: {tenant-guid}
Authorization: Bearer {admin-token}

Response: 200 OK
[
  {
    "id": "guid-1",
    "entityTypeName": "User",
    "fieldName": "erp_customer_number",
    "dataType": "string",
    "displayName": "ERP Customer Number",
    "isRequired": false,
    "maxLength": 50,
    "validationPattern": "^[A-Z0-9-]+$",
    "isVisibleToUsers": true,
    "isEditable": false,
    "integrationSource": "enventa_trade_erp",
    "mappingPath": "ERP.Customer.CustomerNumber",
    "isActive": true,
    "createdAt": "2025-12-27T10:00:00Z"
  },
  ...
]
```

---

## 🔒 Sicherheit & Validierung

### Input Validation

```csharp
// Alle Custom Property Werte werden validiert:

1. Datentyp-Check
   ValidationPattern: "^[A-Z0-9-]+$"
   → "CUST-123456" ✅
   → "cust-123456" ❌ (lowercase nicht erlaubt)

2. Length Check
   MaxLength: 50
   → "CUST-123456" ✅ (13 Zeichen)
   → "VERY_LONG_CUSTOMER_NUMBER_THAT_EXCEEDS_MAXIMUM" ❌ (>50)

3. Required Check
   IsRequired: true
   → null ❌
   → "CUST-123456" ✅

4. Custom Regex
   ValidationPattern: "^WH-[0-9]{3}$"
   → "WH-001" ✅
   → "WH-01" ❌
   → "WAREHOUSE-001" ❌
```

### Tenant Isolation

```csharp
// Nur der eigene Tenant kann seine Extension Schemas sehen/ändern

POST /api/admin/extensions/schemas
X-Tenant-ID: tenant-a-guid  // Only tenant-a can modify their schemas!

// ❌ NICHT MÖGLICH:
// Ein Admin von Tenant A kann die Extension Schemas von Tenant B nicht sehen
```

### Audit Trail

```sql
-- Alle Custom Property Changes sind auditiert
SELECT * FROM extension_audit_logs
WHERE tenant_id = '{tenant-guid}'
  AND entity_id = '{user-guid}'
  AND field_name = 'erp_customer_number'
ORDER BY changed_at DESC;

-- Beispiel-Output:
entity_id     | field_name           | old_value | new_value       | changed_at
--------------|----------------------|-----------|-----------------|------------------
user-123-guid | erp_customer_number  | NULL      | CUST-123456     | 2025-12-27 10:00:00
user-123-guid | erp_customer_number  | CUST-123  | CUST-123456     | 2025-12-27 11:00:00
```

---

## 📊 Datenbank-Schema

### extension_schemas Table

```sql
CREATE TABLE extension_schemas (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL,
    entity_type_name VARCHAR(100) NOT NULL,
    field_name VARCHAR(100) NOT NULL,
    data_type VARCHAR(50) NOT NULL,
    display_name VARCHAR(200) NOT NULL,
    description VARCHAR(1000),
    is_required BOOLEAN DEFAULT false,
    max_length INTEGER,
    validation_pattern VARCHAR(500),
    default_value VARCHAR(500),
    is_visible_to_users BOOLEAN DEFAULT true,
    is_editable BOOLEAN DEFAULT true,
    integration_source VARCHAR(100),
    mapping_path VARCHAR(500),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMPTZ NOT NULL,
    updated_at TIMESTAMPTZ NOT NULL,
    
    UNIQUE (tenant_id, entity_type_name, field_name),
    INDEX idx_tenant_id (tenant_id),
    INDEX idx_integration_source (tenant_id, integration_source),
    INDEX idx_active_schemas (tenant_id, is_active)
);
```

### extension_audit_logs Table

```sql
CREATE TABLE extension_audit_logs (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL,
    entity_id UUID NOT NULL,
    entity_type_name VARCHAR(100) NOT NULL,
    field_name VARCHAR(100) NOT NULL,
    old_value TEXT,
    new_value TEXT,
    changed_by UUID,
    changed_at TIMESTAMPTZ NOT NULL,
    reason VARCHAR(500),
    
    INDEX idx_entity_audit (tenant_id, entity_id, entity_type_name),
    INDEX idx_tenant_timeline (tenant_id, changed_at),
    INDEX idx_timestamp (changed_at)
);
```

### User Table (Updated)

```sql
ALTER TABLE users ADD COLUMN custom_properties TEXT;
ALTER TABLE users ADD COLUMN version INTEGER DEFAULT 1;

-- custom_properties speichert JSON:
-- {
--   "erp_customer_id": "CUST-12345",
--   "erp_customer_number": "123456",
--   "warehouse_code": "WH-001"
-- }
```

---

## 🧪 Unit Tests

```csharp
[TestClass]
public class EntityExtensionServiceTests
{
    [TestMethod]
    public void SetCustomProperty_WithValidValue_UpdatesJson()
    {
        // Arrange
        var user = new User { TenantId = tenantId };
        var service = new EntityExtensionService(...);

        // Act
        service.SetCustomProperty(user, "erp_customer_id", "CUST-123");

        // Assert
        Assert.AreEqual("{\"erp_customer_id\":\"CUST-123\"}", user.CustomProperties);
    }

    [TestMethod]
    public async Task ValidateCustomProperty_InvalidPattern_ReturnsFalse()
    {
        // Arrange
        var service = new EntityExtensionService(...);
        var value = "invalid-format";  // Nicht ^WH-[0-9]{3}$

        // Act
        var result = await service.ValidateCustomPropertyAsync(
            tenantId, "User", "warehouse_code", value);

        // Assert
        Assert.IsFalse(result);
    }
}
```

---

## 🔄 Integration Patterns

### Pattern 1: Synchrone Integration (enventa Trade ERP)

```csharp
// Bei User-Erstellung sofort ERP abfragen
var command = new CreateUserWithErpCommand(...);
var user = await mediator.Send(command);
// Custom Properties sind sofort verfügbar
```

### Pattern 2: Asynchrone Integration (Background Job)

```csharp
// User wird erst erstellt, ERP Sync läuft asynchron
var user = await userRepository.CreateAsync(newUser);

// Publishing Domain Event
user.RaiseDomainEvent(new UserCreatedEvent(user.Id, user.Email));

// In Handler: ERP Sync asynchron
[WolverineHandler]
public async Task Handle(UserCreatedEvent @event)
{
    await _erpIntegration.SyncUserWithErpAsync(...);
}
```

### Pattern 3: Scheduled Sync (Cron Job)

```csharp
// Täglich alle User mit ERP synchronisieren
[ScheduleRule("0 2 * * *")]  // 2 AM täglich
public async Task SyncAllUsersWithErp(Guid tenantId)
{
    var users = await _userRepository.GetActiveUsersAsync(tenantId);
    
    foreach (var user in users)
    {
        var erpId = _extensionService.GetCustomProperty<string>(user, "erp_customer_id");
        if (!string.IsNullOrEmpty(erpId))
        {
            await _erpIntegration.SyncUserWithErpAsync(
                user, erpId, apiKey, baseUrl);
        }
    }
}
```

---

## ⚠️ Best Practices

### ✅ DO

```csharp
// 1. Immer validieren vor dem Speichern
var isValid = await extensionService.ValidateCustomPropertyAsync(...);
if (isValid)
{
    extensionService.SetCustomProperty(user, fieldName, value);
}

// 2. Audit Trail für wichtige Changes
await extensionService.LogCustomPropertyChangeAsync(
    user.Id, tenantId, "User", fieldName,
    oldValue, newValue, userId, "ERP Sync");

// 3. Schema definieren vor der Nutzung
// Extension Schemas sollten vom Admin über API konfiguriert werden,
// nicht hardcodiert im Code

// 4. Versionierung nutzen für Concurrency
// Version wird inkrementiert bei jedem Change
user.Version++
```

### ❌ DON'T

```csharp
// 1. Keine direkten JSON-Manipulationen
user.CustomProperties = "{\"fieldName\":\"value\"}";  // ❌

// 2. Keine unkontrollierten Custom Properties
service.SetCustomProperty(user, randomFieldName, userInput);  // ❌

// 3. Keine PII in Custom Properties speichern (wenn verschlüsselt nötig)
// Wenn sensible Daten: Separate Tabelle mit Verschlüsselung

// 4. Keine SQL-Injection über Custom Fields
// JSON wird parameterisiert, aber validieren Sie trotzdem Input
```

---

## 🚨 Fehlerbehandlung

```csharp
try
{
    var isValid = await extensionService.ValidateCustomPropertyAsync(
        tenantId, "User", "erp_customer_id", value);
    
    if (!isValid)
        return BadRequest(new { error = "Validation failed" });
    
    extensionService.SetCustomProperty(user, "erp_customer_id", value);
    await userRepository.SaveChangesAsync();
}
catch (DbUpdateConcurrencyException ex)
{
    // Optimistic Concurrency - Retry mit neuesten Daten
    return Conflict(new { error = "Entity was modified by another user" });
}
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to update custom property");
    return StatusCode(500, new { error = "Internal server error" });
}
```

---

## 📈 Performance-Considerations

| Szenario | Optimierung |
|----------|-----------|
| Custom Properties Abruf (häufig) | Redis Caching für User + Extensions |
| Validierung bei jedem Save | Extension Schemas in-memory cachen |
| Audit Log Queries (große Tenants) | Partitionierung nach Datum |
| JSON-Parsing | Lazy Loading (nur wenn abgerufen) |

---

## 🔗 Nächste Schritte

1. **Andere Entities erweitern**
   - Product mit Custom Properties
   - Order mit Custom Properties

2. **Weitere Integration-Templates**
   - SAP Integration
   - Microsoft Dynamics Integration
   - Custom-Provider API

3. **Frontend Admin-Panel**
   - Extension Schema Manager UI
   - Custom Property Editor
   - Audit Log Viewer

4. **Performance-Optimierungen**
   - Caching-Strategy
   - Batch-Operations
   - Async Processing

---

## 📚 Referenzen

- [ApplicationSpecifications.md](../../../docs/APPLICATION_SPECIFICATIONS.md) - Security P0
- [DDD BoundedContexts](../../../docs/architecture/DDD_BOUNDED_CONTEXTS.md) - Architecture
- [Copilot Instructions](../../../.github/copilot-instructions.md) - Development Guidelines
