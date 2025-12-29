# 📋 Audit Logging Implementation Guide

## Overview

This document explains how audit logging is implemented in B2Connect for P0.4 (Audit Logging).

## Components

### 1. **IAuditableEntity Interface** (`B2Connect.Shared.Core/Interfaces/`)

Marks entities that require automatic audit tracking.

```csharp
public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    string CreatedBy { get; set; }
    DateTime? ModifiedAt { get; set; }
    string? ModifiedBy { get; set; }
    DateTime? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
    bool IsDeleted { get; set; }
}
```

### 2. **AuditableEntity Base Class** (`B2Connect.Shared.Core/Entities/`)

Base class implementing the audit interface with sensible defaults.

```csharp
public abstract class AuditableEntity : IAuditableEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = "System";
    // ... rest of fields
}
```

### 3. **AuditInterceptor** (`B2Connect.Shared.Data/Interceptors/`)

EF Core interceptor that automatically:
- Sets `CreatedAt` and `CreatedBy` on new entities
- Sets `ModifiedAt` and `ModifiedBy` on updates
- Implements soft deletes (sets `IsDeleted`, `DeletedAt`, `DeletedBy` instead of hard deleting)
- Logs all changes via Serilog

### 4. **AuditLogService** (`B2Connect.Shared.Data/Logging/`)

Manual audit logging service for tracking specific actions:
- Records entity changes with detailed change information
- Maintains audit trail for compliance
- Ready to be replaced with Event Sourcing for higher audit requirements

## Usage

### Making an Entity Auditable

**Before:**
```csharp
public class User
{
    public string Id { get; set; }
    public string Email { get; set; }
}
```

**After:**
```csharp
public class User : AuditableEntity
{
    public string Email { get; set; }
}
```

### Configuring the Database Context

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Add the audit interceptor
        optionsBuilder.AddInterceptors(
            new AuditInterceptor(LoggerFactory.CreateLogger<AuditInterceptor>(), userId)
        );
    }
}
```

### Querying Non-Deleted Entities

```csharp
// Automatically excludes soft-deleted records
var activeUsers = context.Users.Where(u => !u.IsDeleted).ToList();

// Or create a helper extension
public static IQueryable<T> Active<T>(this IQueryable<T> query) 
    where T : IAuditableEntity
{
    return query.Where(e => !e.IsDeleted);
}

// Usage:
var activeUsers = context.Users.Active().ToList();
```

### Manual Audit Logging

```csharp
private readonly IAuditLogService _auditLog;

public async Task ProcessOrderAsync(Order order, string userId)
{
    // ... do work ...
    
    // Log the action
    await _auditLog.LogActionAsync(
        "Order",
        order.Id,
        "Processed",
        "Status: Pending -> Completed",
        userId
    );
}

// Later, retrieve audit history
var history = await _auditLog.GetAuditLogsAsync("Order", orderId);
```

## Security Benefits

✅ **Compliance**: Full audit trail for regulatory requirements (GDPR, HIPAA, etc.)
✅ **Soft Deletes**: Never lose data, maintain referential integrity
✅ **User Accountability**: Know who created/modified/deleted what and when
✅ **Change Tracking**: Detailed history of all modifications
✅ **Immutable Log**: Timestamped, cannot be modified retroactively

## Production Considerations

### Current State (Development)
- In-memory audit logs
- EF Core automatic audit fields
- Suitable for MVP and development

### Recommended for Production
1. **Dedicated Audit Table**: Store all changes in a separate, write-optimized table
2. **Event Sourcing**: Use Wolverine or similar for event-driven audit trail
3. **Secure Log Storage**: Use immutable logs (AWS QLDB, Azure Immutable Blob Storage)
4. **Retention Policy**: Archive old logs after X years per compliance requirements

### Migration Path
```sql
-- Future: Dedicated audit log table
CREATE TABLE AuditLogs (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    EntityType NVARCHAR(256) NOT NULL,
    EntityId NVARCHAR(256) NOT NULL,
    Action NVARCHAR(50) NOT NULL,
    ChangedValues NVARCHAR(MAX),
    UserId NVARCHAR(256),
    Timestamp DATETIME2 NOT NULL,
    INDEX idx_entity (EntityType, EntityId)
);
```

## Next Steps

- [ ] Implement dedicated audit log table in production
- [ ] Add Event Sourcing with Wolverine for critical workflows
- [ ] Add audit log API endpoints for compliance/audit reviews
- [ ] Configure retention policies
- [ ] Set up audit log archival process
