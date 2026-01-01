# Entity Framework Core Interceptors - Multi-Tenancy Implementation

## Übersicht
Entity Framework Core Interceptors ermöglichen die automatische Modifikation von Datenbankoperationen zur Laufzeit. Diese Implementierung zeigt, wie Interceptors für automatische Tenant-Filterung und Tenant-Zuweisung in einer Multi-Tenant-Anwendung verwendet werden.

## Architektur

### TenantCommandInterceptor
Der `TenantCommandInterceptor` erbt von `SaveChangesInterceptor` und modifiziert automatisch alle Save-Operationen:

```csharp
public class TenantCommandInterceptor : SaveChangesInterceptor
{
    private readonly ITenantContext _tenantContext;

    public TenantCommandInterceptor(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        // Automatische Tenant-ID Zuweisung für neue Entities
        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry.Entity is ITenantEntity tenantEntity &&
                entry.State == EntityState.Added)
            {
                tenantEntity.TenantId = _tenantContext.TenantId;
            }
        }
        return base.SavingChanges(eventData, result);
    }
}
```

### TenantQueryInterceptor
Der `TenantQueryInterceptor` erbt von `DbCommandInterceptor` und modifiziert automatisch alle SELECT-Abfragen:

```csharp
public class TenantQueryInterceptor : DbCommandInterceptor
{
    private readonly ITenantContext _tenantContext;

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        // Automatische WHERE tenant_id = ? Bedingung hinzufügen
        if (IsTenantQuery(command.CommandText))
        {
            command.CommandText = AddTenantFilter(command.CommandText);
            // Parameter für Tenant-ID hinzufügen
        }
        return base.ReaderExecuting(command, result);
    }
}
```

### TenantDbContext Base Class
Abstrakte Basisklasse für alle DbContexts, die Tenant-Filterung benötigen:

```csharp
public abstract class TenantDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    protected TenantDbContext(
        DbContextOptions options,
        ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(
            new TenantCommandInterceptor(_tenantContext),
            new TenantQueryInterceptor(_tenantContext));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Automatische Tenant-Filter für alle ITenantEntity Queries
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(CreateTenantFilter(entityType.ClrType));
            }
        }
    }

    private LambdaExpression CreateTenantFilter(Type entityType)
    {
        // Erstellt WHERE e.TenantId == tenantContext.TenantId Filter
        var parameter = Expression.Parameter(entityType, "e");
        var tenantProperty = Expression.Property(parameter, nameof(ITenantEntity.TenantId));
        var tenantValue = Expression.Property(
            Expression.Constant(_tenantContext), nameof(ITenantContext.TenantId));

        var filter = Expression.Equal(tenantProperty, tenantValue);
        return Expression.Lambda(filter, parameter);
    }
}
```

## Implementierung in Services

### Beispiel: LocalizationDbContext
```csharp
public class LocalizationDbContext : TenantDbContext
{
    public LocalizationDbContext(
        DbContextOptions<LocalizationDbContext> options,
        ITenantContext tenantContext)
        : base(options, tenantContext)
    {
    }

    public DbSet<LocalizedString> LocalizedStrings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Wichtig: Base aufrufen!

        modelBuilder.Entity<LocalizedString>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Value).IsRequired();
            entity.Property(e => e.Language).IsRequired().HasMaxLength(10);
            entity.Property(e => e.TenantId).IsRequired();
        });
    }
}
```

### ITenantEntity Interface
Marker-Interface für Entities, die Tenant-Filterung benötigen:

```csharp
public interface ITenantEntity
{
    Guid TenantId { get; set; }
}
```

### Entity Implementierung
```csharp
public class LocalizedString : ITenantEntity
{
    public Guid Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
    public string Language { get; set; }
    public Guid TenantId { get; set; } // Automatisch gesetzt durch Interceptor
}
```

## Vorteile dieser Architektur

### Automatische Tenant-Isolation
- **Zero-Code Tenant Filtering**: Kein manueller Code in Repositories/Services nötig
- **Compile-Time Safety**: ITenantEntity Interface erzwingt TenantId Property
- **Runtime Safety**: Interceptors validieren Tenant-Kontext vor jeder Operation

### Performance-Optimierung
- **Query-Level Filtering**: WHERE-Bedingungen werden auf Datenbankebene angewendet
- **Index-Unterstützung**: TenantId kann indiziert werden für optimale Performance
- **Minimale Overhead**: Interceptors werden nur bei Bedarf ausgeführt

### Wartbarkeit
- **DRY Principle**: Tenant-Logik zentralisiert in Interceptors
- **Testbarkeit**: Interceptors können unabhängig getestet werden
- **Erweiterbarkeit**: Neue Tenant-bezogene Logik einfach hinzufügen

## Best Practices

### 1. Interface Design
- Verwende `ITenantEntity` als Marker-Interface
- Implementiere `TenantId` als erforderliche Property
- Verwende `Guid` für maximale Kompatibilität

### 2. Interceptor Registration
- Registriere Interceptors in `OnConfiguring`
- Stelle sicher, dass `ITenantContext` injected wird
- Verwende Scoped Lifetime für Tenant-Kontext

### 3. Query Filter Setup
- Rufe `base.OnModelCreating()` immer auf
- Verwende `HasQueryFilter` für automatische WHERE-Bedingungen
- Teste Queries mit verschiedenen Tenant-Kontexten

### 4. Testing
```csharp
[Fact]
public async Task Should_Automatically_Filter_By_Tenant()
{
    // Arrange
    var tenantId = Guid.NewGuid();
    _tenantContextMock.Setup(x => x.TenantId).Returns(tenantId);

    // Act
    var result = await _dbContext.LocalizedStrings.ToListAsync();

    // Assert
    // Alle Entities sollten automatisch nach tenantId gefiltert sein
}
```

## Häufige Fallstricke

### 1. Vergessene Base-Aufrufe
```csharp
// ❌ FALSCH
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // base.OnModelCreating() vergessen!
    // Tenant-Filter werden nicht angewendet
}

// ✅ RICHTIG
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder); // Wichtig!
    // Zusätzliche Konfiguration hier
}
```

### 2. Falsche Interceptor-Reihenfolge
```csharp
// ❌ FALSCH - Interceptors in falscher Reihenfolge
optionsBuilder.AddInterceptors(
    new TenantQueryInterceptor(_tenantContext), // Sollte zuerst kommen
    new TenantCommandInterceptor(_tenantContext));

// ✅ RICHTIG
optionsBuilder.AddInterceptors(
    new TenantCommandInterceptor(_tenantContext), // Save zuerst
    new TenantQueryInterceptor(_tenantContext));  // Query danach
```

### 3. Missing Tenant Context
```csharp
// Stelle sicher, dass ITenantContext verfügbar ist
services.AddScoped<ITenantContext, TenantContext>();
```

## Performance-Überlegungen

### Index-Strategien
```sql
-- Empfohlene Indizes für Tenant-Entities
CREATE INDEX IX_LocalizedStrings_TenantId ON LocalizedStrings (TenantId);
CREATE INDEX IX_LocalizedStrings_TenantId_Key ON LocalizedStrings (TenantId, Key);
```

### Query-Optimierung
- Interceptors fügen WHERE-Bedingungen auf SQL-Ebene hinzu
- Verwende `Include` sparsam bei Tenant-Queries
- Monitor Database Execution Plans

## Migration von manueller Tenant-Filterung

### Vorher (Manuell)
```csharp
public async Task<List<LocalizedString>> GetAllAsync()
{
    return await _dbContext.LocalizedStrings
        .Where(x => x.TenantId == _tenantContext.TenantId)
        .ToListAsync();
}
```

### Nachher (Automatisch)
```csharp
public async Task<List<LocalizedString>> GetAllAsync()
{
    return await _dbContext.LocalizedStrings.ToListAsync();
    // Tenant-Filter wird automatisch durch Interceptor angewendet
}
```

## Monitoring und Debugging

### SQL Query Logging
```csharp
optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging();
```

### Interceptor Debugging
```csharp
public class DebugTenantInterceptor : TenantCommandInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        _logger.LogInformation($"Saving {eventData.Context.ChangeTracker.Entries().Count()} entities");
        return base.SavingChanges(eventData, result);
    }
}
```

## Zusammenfassung

Entity Framework Core Interceptors bieten eine elegante Lösung für automatische Tenant-Filterung in Multi-Tenant-Anwendungen. Durch die Kombination von Command- und Query-Interceptors mit einem zentralen TenantDbContext wird Tenant-Isolation transparent und wartbar implementiert.

**Key Benefits:**
- ✅ Zero-Code Tenant Filtering
- ✅ Compile-Time Safety
- ✅ Performance-Optimized Queries
- ✅ Centralized Tenant Logic
- ✅ Easy Testing and Maintenance

**Empfohlene Implementierung:**
1. Erstelle `ITenantEntity` Interface
2. Implementiere `TenantCommandInterceptor` und `TenantQueryInterceptor`
3. Erstelle `TenantDbContext` Base-Class
4. Lass alle Tenant-DbContexts von `TenantDbContext` erben
5. Registriere Interceptors in DI-Container

---

**Quelle:** B2Connect Multi-Tenant Architecture Implementation  
**Aktualisiert:** Januar 2026  
**Version:** 1.0</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/.ai/knowledgebase/ef-core-interceptors-multitenancy.md