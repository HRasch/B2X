# Database Schema: Price Calculation & VAT Validation

**Version**: 1.0  
**Last Updated**: 29. Dezember 2025  
**Database**: PostgreSQL 16  
**Related Issues**: #30 (B2C Price Transparency), #31 (B2B VAT-ID Validation)

---

## Schema Overview

### Price Calculation
- **No database persistence** - purely calculated on-demand
- Deterministic: Same input → same output
- No caching required (calculation < 1ms)

### VAT-ID Validation Cache
- **Persistent table**: `vat_id_validations`
- **Purpose**: Cache VIES API results to reduce external API calls
- **TTL Strategy**:
  - Valid IDs: 365 days (annual re-validation recommended by EU)
  - Invalid IDs: 24 hours (enables retry if API recovers)

---

## Table Definition: `vat_id_validations`

### SQL Schema

```sql
CREATE TABLE vat_id_validations (
    -- Primary Key
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    
    -- Multi-Tenancy
    tenant_id UUID NOT NULL,
    
    -- VAT-ID Components
    country_code VARCHAR(2) NOT NULL,
    vat_number VARCHAR(17) NOT NULL,
    
    -- Validation Result
    is_valid BOOLEAN NOT NULL,
    company_name VARCHAR(255),
    company_address TEXT,
    
    -- Timestamps
    validated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    
    -- Soft Delete
    is_deleted BOOLEAN NOT NULL DEFAULT false,
    deleted_at TIMESTAMP WITH TIME ZONE,
    
    -- Indexes
    CONSTRAINT unique_tenant_vat_number UNIQUE (tenant_id, vat_number) WHERE NOT is_deleted,
    CONSTRAINT valid_vat_number_length CHECK (length(vat_number) >= 1 AND length(vat_number) <= 17),
    CONSTRAINT valid_country_code_length CHECK (length(country_code) = 2)
);

-- Indexes for performance
CREATE INDEX idx_vat_validations_tenant_id 
    ON vat_id_validations(tenant_id) 
    WHERE NOT is_deleted;

CREATE INDEX idx_vat_validations_expires_at 
    ON vat_id_validations(expires_at) 
    WHERE NOT is_deleted;

CREATE INDEX idx_vat_validations_country_vat 
    ON vat_id_validations(country_code, vat_number) 
    WHERE NOT is_deleted;

CREATE INDEX idx_vat_validations_tenant_country 
    ON vat_id_validations(tenant_id, country_code) 
    WHERE NOT is_deleted;
```

### Entity Framework Core Model

```csharp
using Microsoft.EntityFrameworkCore;

namespace B2X.Catalog.Models;

public class VatIdValidationCache
{
    // Primary Key
    [Key]
    public Guid Id { get; set; }

    // Multi-Tenancy
    [Required]
    public Guid TenantId { get; set; }

    // VAT-ID
    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string CountryCode { get; set; } = string.Empty;

    [Required]
    [StringLength(17, MinimumLength = 1)]
    public string VatNumber { get; set; } = string.Empty;

    // Result
    [Required]
    public bool IsValid { get; set; }

    [StringLength(255)]
    public string? CompanyName { get; set; }

    public string? CompanyAddress { get; set; }

    // Timestamps
    [Required]
    public DateTime ValidatedAt { get; set; }

    [Required]
    public DateTime ExpiresAt { get; set; }

    // Soft Delete
    [Required]
    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedAt { get; set; }
}
```

### DbContext Configuration

```csharp
using Microsoft.EntityFrameworkCore;

namespace B2X.Catalog.Data;

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<VatIdValidationCache>(entity =>
    {
        // Table
        entity.ToTable("vat_id_validations");

        // Primary Key
        entity.HasKey(e => e.Id);

        // Unique Constraint (with soft delete filter)
        entity.HasIndex(e => new { e.TenantId, e.VatNumber })
            .IsUnique()
            .HasFilter("NOT is_deleted")
            .HasName("unique_tenant_vat_number");

        // Indexes for Query Performance
        entity.HasIndex(e => e.TenantId)
            .HasFilter("NOT is_deleted")
            .HasName("idx_vat_validations_tenant_id");

        entity.HasIndex(e => e.ExpiresAt)
            .HasFilter("NOT is_deleted")
            .HasName("idx_vat_validations_expires_at");

        entity.HasIndex(e => new { e.CountryCode, e.VatNumber })
            .HasFilter("NOT is_deleted")
            .HasName("idx_vat_validations_country_vat");

        // Properties
        entity.Property(e => e.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        entity.Property(e => e.TenantId)
            .IsRequired();

        entity.Property(e => e.CountryCode)
            .HasMaxLength(2)
            .IsRequired();

        entity.Property(e => e.VatNumber)
            .HasMaxLength(17)
            .IsRequired();

        entity.Property(e => e.IsValid)
            .IsRequired();

        entity.Property(e => e.CompanyName)
            .HasMaxLength(255);

        entity.Property(e => e.ValidatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        entity.Property(e => e.ExpiresAt)
            .IsRequired();

        entity.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Global Query Filter (Soft Delete)
        entity.HasQueryFilter(e => !e.IsDeleted);
    });

    base.OnModelCreating(modelBuilder);
}
```

---

## Migration

### Create Initial Migration

```bash
dotnet ef migrations add InitialVatValidationCache \
  --project backend/Domain/Catalog/src/B2X.Catalog.API.csproj \
  --startup-project backend/Domain/Catalog/src/B2X.Catalog.API.csproj \
  --context ApplicationDbContext \
  --output-dir Data/Migrations
```

### Generated Migration (Example)

```csharp
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace B2X.Catalog.Data.Migrations
{
    public partial class InitialVatValidationCache : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vat_id_validations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, 
                        defaultValueSql: "gen_random_uuid()"),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    country_code = table.Column<string>(type: "character varying(2)", 
                        maxLength: 2, nullable: false),
                    vat_number = table.Column<string>(type: "character varying(17)", 
                        maxLength: 17, nullable: false),
                    is_valid = table.Column<bool>(type: "boolean", nullable: false),
                    company_name = table.Column<string>(type: "character varying(255)", 
                        maxLength: 255, nullable: true),
                    company_address = table.Column<string>(type: "text", nullable: true),
                    validated_at = table.Column<DateTime>(type: "timestamp with time zone", 
                        nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", 
                        nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, 
                        defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", 
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vat_id_validations", x => x.id);
                    table.UniqueConstraint("unique_tenant_vat_number", x => new { x.tenant_id, x.vat_number });
                    table.CheckConstraint("valid_vat_number_length", 
                        "length(vat_number) >= 1 AND length(vat_number) <= 17");
                    table.CheckConstraint("valid_country_code_length", 
                        "length(country_code) = 2");
                });

            migrationBuilder.CreateIndex(
                name: "idx_vat_validations_tenant_id",
                table: "vat_id_validations",
                column: "tenant_id",
                filter: "NOT is_deleted");

            migrationBuilder.CreateIndex(
                name: "idx_vat_validations_expires_at",
                table: "vat_id_validations",
                column: "expires_at",
                filter: "NOT is_deleted");

            migrationBuilder.CreateIndex(
                name: "idx_vat_validations_country_vat",
                table: "vat_id_validations",
                columns: new[] { "country_code", "vat_number" },
                filter: "NOT is_deleted");

            migrationBuilder.CreateIndex(
                name: "idx_vat_validations_tenant_country",
                table: "vat_id_validations",
                columns: new[] { "tenant_id", "country_code" },
                filter: "NOT is_deleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "vat_id_validations");
        }
    }
}
```

### Apply Migration

```bash
# Development
dotnet ef database update \
  --project backend/Domain/Catalog/src/B2X.Catalog.API.csproj \
  --startup-project backend/Domain/Catalog/src/B2X.Catalog.API.csproj

# Production (with confirmation)
dotnet ef database update \
  --project backend/Domain/Catalog/src/B2X.Catalog.API.csproj \
  --startup-project backend/Domain/Catalog/src/B2X.Catalog.API.csproj \
  --environment Production
```

---

## Data Model Details

### Column Specifications

| Column | Type | Null | Default | Purpose |
|--------|------|------|---------|---------|
| `id` | UUID | NO | gen_random_uuid() | Unique identifier |
| `tenant_id` | UUID | NO | — | Multi-tenant isolation |
| `country_code` | VARCHAR(2) | NO | — | VAT-ID country (ISO 3166-1 alpha-2) |
| `vat_number` | VARCHAR(17) | NO | — | VAT number without country prefix |
| `is_valid` | BOOLEAN | NO | — | VIES validation result |
| `company_name` | VARCHAR(255) | YES | NULL | Company name from VIES |
| `company_address` | TEXT | YES | NULL | Company address from VIES |
| `validated_at` | TIMESTAMP | NO | NOW() | When VIES validation occurred |
| `expires_at` | TIMESTAMP | NO | — | When cache expires (365 or 24 hours later) |
| `is_deleted` | BOOLEAN | NO | false | Soft delete flag |
| `deleted_at` | TIMESTAMP | YES | NULL | When soft delete occurred |

### Indexes

| Name | Columns | Filter | Purpose |
|------|---------|--------|---------|
| `PK_vat_id_validations` | `id` | — | Primary key |
| `unique_tenant_vat_number` | `tenant_id`, `vat_number` | `NOT is_deleted` | Prevent duplicates per tenant |
| `idx_vat_validations_tenant_id` | `tenant_id` | `NOT is_deleted` | Queries by tenant |
| `idx_vat_validations_expires_at` | `expires_at` | `NOT is_deleted` | Cache expiry cleanup |
| `idx_vat_validations_country_vat` | `country_code`, `vat_number` | `NOT is_deleted` | Lookup by VAT-ID |
| `idx_vat_validations_tenant_country` | `tenant_id`, `country_code` | `NOT is_deleted` | Tenant + country queries |

### Constraints

- **NOT NULL**: `id`, `tenant_id`, `country_code`, `vat_number`, `is_valid`, `validated_at`, `expires_at`, `is_deleted`
- **UNIQUE**: `(tenant_id, vat_number)` WHERE `is_deleted = false`
- **CHECK**: `length(country_code) = 2`
- **CHECK**: `length(vat_number) >= 1 AND length(vat_number) <= 17`

---

## Query Examples

### Find Valid VAT-ID Cache Entry

```csharp
var cached = await context.VatIdValidationCaches
    .Where(v => v.TenantId == tenantId 
            && v.CountryCode == "AT" 
            && v.VatNumber == "U12345678"
            && v.ExpiresAt > DateTime.UtcNow)
    .FirstOrDefaultAsync();
```

### Cache a Valid VAT-ID (365-day TTL)

```csharp
var cacheEntry = new VatIdValidationCache
{
    Id = Guid.NewGuid(),
    TenantId = tenantId,
    CountryCode = "AT",
    VatNumber = "U12345678",
    IsValid = true,
    CompanyName = "Example GmbH",
    CompanyAddress = "Musterstraße 1, 1010 Wien",
    ValidatedAt = DateTime.UtcNow,
    ExpiresAt = DateTime.UtcNow.AddDays(365)  // 365-day TTL
};

context.VatIdValidationCaches.Add(cacheEntry);
await context.SaveChangesAsync();
```

### Cache an Invalid VAT-ID (24-hour TTL)

```csharp
var cacheEntry = new VatIdValidationCache
{
    Id = Guid.NewGuid(),
    TenantId = tenantId,
    CountryCode = "FR",
    VatNumber = "INVALID123",
    IsValid = false,
    CompanyName = null,
    CompanyAddress = null,
    ValidatedAt = DateTime.UtcNow,
    ExpiresAt = DateTime.UtcNow.AddHours(24)  // 24-hour TTL for retry
};

context.VatIdValidationCaches.Add(cacheEntry);
await context.SaveChangesAsync();
```

### Soft Delete Expired Entry

```csharp
var expired = await context.VatIdValidationCaches
    .Where(v => v.ExpiresAt < DateTime.UtcNow && !v.IsDeleted)
    .ToListAsync();

foreach (var entry in expired)
{
    entry.IsDeleted = true;
    entry.DeletedAt = DateTime.UtcNow;
}

await context.SaveChangesAsync();
```

### List All Validations for Tenant (Last 30 Days)

```csharp
var recentValidations = await context.VatIdValidationCaches
    .Where(v => v.TenantId == tenantId 
            && v.ValidatedAt > DateTime.UtcNow.AddDays(-30))
    .OrderByDescending(v => v.ValidatedAt)
    .ToListAsync();
```

---

## Performance Characteristics

### Typical Query Performance

| Query | Index Used | Time | Notes |
|-------|------------|------|-------|
| Find by tenant + country + vat | `idx_vat_validations_tenant_country` + `PK` | < 1ms | Most common query |
| List by tenant | `idx_vat_validations_tenant_id` | < 5ms | Few thousand rows |
| Cleanup expired | `idx_vat_validations_expires_at` | < 10ms | Scheduled daily |
| Insert new entry | PK + UNIQUE | < 2ms | Simple insert |

### Expected Table Size

| Timeframe | Entries | Size | Notes |
|-----------|---------|------|-------|
| Week 1 | ~1,000 | ~100 KB | New shop, low volume |
| Month 1 | ~10,000 | ~1 MB | Growing customer base |
| Month 6 | ~100,000 | ~10 MB | Mature shop, repeat customers |
| Year 1 | ~200,000 | ~20 MB | With 365-day retention + cleanup |

### Maintenance

**Daily Cleanup Task** (removes expired soft-deleted entries):
```csharp
public async Task CleanupExpiredCacheAsync(CancellationToken cancellationToken)
{
    var now = DateTime.UtcNow;
    
    // Soft delete expired entries
    await context.VatIdValidationCaches
        .Where(v => v.ExpiresAt < now && !v.IsDeleted)
        .ExecuteUpdateAsync(s => s
            .SetProperty(e => e.IsDeleted, true)
            .SetProperty(e => e.DeletedAt, now),
            cancellationToken);

    // Hard delete old soft-deleted entries (optional, 90-day retention)
    await context.VatIdValidationCaches
        .Where(v => v.IsDeleted && v.DeletedAt < now.AddDays(-90))
        .ExecuteDeleteAsync(cancellationToken);
}
```

**Schedule**: Every 24 hours (can run during low-traffic window)

---

## Related EF Core Configuration

### Global Query Filter (Soft Delete)

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Automatically exclude soft-deleted entries from queries
    modelBuilder.Entity<VatIdValidationCache>()
        .HasQueryFilter(e => !e.IsDeleted);
    
    // To include soft-deleted: context.VatIdValidationCaches.IgnoreQueryFilters()
}
```

### Audit Logging Interceptor

```csharp
public class AuditLoggingInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        var userId = GetCurrentUserId(); // From auth context
        var timestamp = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<VatIdValidationCache>()
                     .Where(e => e.State != EntityState.Unchanged))
        {
            // Log before/after values to audit trail
            var auditLog = new AuditLogEntry
            {
                EntityType = nameof(VatIdValidationCache),
                Action = entry.State.ToString(),
                UserId = userId,
                Timestamp = timestamp,
                BeforeValues = entry.OriginalValues.Clone().ToObject().ToJson(),
                AfterValues = entry.CurrentValues.ToObject().ToJson()
            };

            context.AuditLogs.Add(auditLog);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
```

---

**Related Documentation**:
- [API_ENDPOINTS_PRICE_AND_VAT.md](../architecture-docs/API_ENDPOINTS_PRICE_AND_VAT.md)
- [ARCHITECTURE_PRICE_AND_VAT_VALIDATION.md](../architecture-docs/ARCHITECTURE_PRICE_AND_VAT_VALIDATION.md)
- [DEPLOYMENT_PRICE_AND_VAT.md](DEPLOYMENT_PRICE_AND_VAT.md)
