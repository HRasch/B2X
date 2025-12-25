# Database Configuration Guide - B2Connect Layout Service

## Overview

Das Layout Service unterstützt **drei verschiedene Datenbankoptionen**:

1. **PostgreSQL** (Empfohlen für Production)
2. **SQL Server Express** (Lokal auf Windows)
3. **InMemory Database** (Für Unit Tests)

---

## Configuration

### appsettings.json Structure

```json
{
  "DatabaseConfig": {
    "Provider": "PostgreSQL|SqlServer|InMemory",
    "UseInMemory": false,
    "ConnectionStrings": {
      "PostgreSQL": "Server=localhost;Port=5432;Database=b2connect_layout;...",
      "SqlServer": "Server=(local)\\SQLEXPRESS;Database=B2Connect_Layout;...",
      "InMemory": "Data Source=:memory:"
    }
  }
}
```

---

## Setup für verschiedene Umgebungen

### 1. PostgreSQL (Development/Production)

#### Prerequisites

```bash
# PostgreSQL installieren (macOS)
brew install postgresql@15

# PostgreSQL starten
brew services start postgresql@15

# Standard User erstellen
createuser postgres
createdb b2connect_layout -O postgres
```

#### Configuration (appsettings.json)

```json
{
  "DatabaseConfig": {
    "Provider": "PostgreSQL",
    "UseInMemory": false,
    "ConnectionStrings": {
      "PostgreSQL": "Server=localhost;Port=5432;Database=b2connect_layout;User Id=postgres;Password=postgres;"
    }
  }
}
```

#### Migrations und Startup

```bash
cd backend/services/LayoutService

# Migration erstellen
dotnet ef migrations add InitialCreate

# Migration anwenden
dotnet ef database update

# Anwendung starten
dotnet run
```

---

### 2. SQL Server Express (Windows Development)

#### Prerequisites

```powershell
# SQL Server Express installieren
# Download from: https://www.microsoft.com/en-us/sql-server/sql-server-express

# SQL Server Management Studio (SSMS) überprüfen
# oder sqlcmd Tool verwenden
sqlcmd -S (local)\SQLEXPRESS -U sa
```

#### Configuration (appsettings.json)

```json
{
  "DatabaseConfig": {
    "Provider": "SqlServer",
    "UseInMemory": false,
    "ConnectionStrings": {
      "SqlServer": "Server=(local)\\SQLEXPRESS;Database=B2Connect_Layout;Trusted_Connection=true;Encrypt=false;"
    }
  }
}
```

#### Migrations und Startup

```powershell
cd backend/services/LayoutService

# Connection testen
dotnet user-secrets set "DatabaseConfig:ConnectionStrings:SqlServer" "Server=(local)\SQLEXPRESS;Database=B2Connect_Layout;Trusted_Connection=true;Encrypt=false;"

# Migration erstellen
dotnet ef migrations add InitialCreate -c LayoutDbContext

# Migration anwenden
dotnet ef database update

# Anwendung starten
dotnet run
```

---

### 3. InMemory Database (Unit Tests)

#### Automatische Konfiguration

InMemory Database wird **automatisch verwendet** für:
- Unit Tests (Test-Projekte)
- Environment = "Test"
- `UseInMemory = true` in appsettings.json

#### Test Configuration (appsettings.Test.json)

```json
{
  "DatabaseConfig": {
    "Provider": "InMemory",
    "UseInMemory": true
  }
}
```

#### Test Lifecycle

```csharp
public class LayoutServiceIntegrationTests : IAsyncLifetime
{
    private readonly LayoutDbContext _dbContext;

    public LayoutServiceIntegrationTests()
    {
        // Setup: InMemory Database für jeden Test
        var options = new DbContextOptionsBuilder<LayoutDbContext>()
            .UseInMemoryDatabase(databaseName: $"LayoutTestDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new LayoutDbContext(options);
    }

    public async Task InitializeAsync()
    {
        // Vor dem Test: Datenbank erstellen
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        // Nach dem Test: Aufräumen
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.DisposeAsync();
    }

    [Fact]
    public async Task MyTest()
    {
        // Test mit frischer InMemory Database
    }
}
```

---

## DatabaseExtensions Konfiguration

Das `DatabaseExtensions` Klasse verwaltet die Datenbankauswahl:

```csharp
// Program.cs
services.AddLayoutDatabase(configuration);
```

### Automatische Logik

```csharp
if (useInMemory || environment == "Test")
{
    // Verwende InMemory
    services.AddDbContext<LayoutDbContext>(options =>
        options.UseInMemoryDatabase("LayoutServiceTest")
            .EnableSensitiveDataLogging(true));
}
else if (provider == "SqlServer")
{
    // Verwende SQL Server
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.MigrateAssembly("B2Connect.LayoutService");
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
    });
}
else
{
    // Verwende PostgreSQL (Standard)
    options.UseNpgsql(connectionString, pgOptions =>
    {
        pgOptions.MigrationsAssembly("B2Connect.LayoutService");
        pgOptions.EnableRetryOnFailure(maxRetryCount: 3);
    });
}
```

---

## Entity Framework Migrations

### Migration erstellen (PostgreSQL)

```bash
cd backend/services/LayoutService

# Migration mit sprechender Beschreibung
dotnet ef migrations add InitialCreate \
  --project B2Connect.LayoutService.csproj \
  --startup-project B2Connect.LayoutService.csproj

# Generierte Migration überprüfen
ls Migrations/
```

### Migration erstellen (SQL Server)

```powershell
cd backend\services\LayoutService

# Migration erstellen
dotnet ef migrations add InitialCreate `
  --project B2Connect.LayoutService.csproj `
  --startup-project B2Connect.LayoutService.csproj
```

### Migration anwenden

```bash
# Auf Produktionsumgebung automatisch im Startup:
await serviceProvider.EnsureDatabaseAsync();

# Oder manuell:
dotnet ef database update
```

### Migration entfernen

```bash
# Letzte Migration entfernen
dotnet ef migrations remove
```

---

## TDD Test Patterns

### Unit Test (Mit Mocks)

```csharp
[Fact]
public async Task CreatePage_WithValidData_ShouldReturnPage()
{
    // Arrange
    var mockRepo = new Mock<ILayoutRepository>();
    var service = new LayoutService(mockRepo.Object);

    // Act
    var result = await service.CreatePageAsync(tenantId, request);

    // Assert
    Assert.NotNull(result);
    mockRepo.Verify(r => r.CreatePageAsync(tenantId, It.IsAny<CmsPage>()), Times.Once);
}
```

### Integration Test (Mit InMemory DB)

```csharp
[Fact]
public async Task CreatePage_WithValidData_ShouldPersistInDatabase()
{
    // Arrange: InMemory DB automatisch
    var tenantId = Guid.NewGuid();
    var page = new CmsPage { ... };

    // Act: Echte Persistierung
    var result = await _repository.CreatePageAsync(tenantId, page);

    // Assert: Aus DB lesen
    var persisted = await _dbContext.Pages.FirstOrDefaultAsync(p => p.Id == page.Id);
    Assert.NotNull(persisted);
}
```

---

## Umgebungsvariablen

### Development

```bash
export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_URLS=http://localhost:5008
export ConnectionStrings__PostgreSQL="Server=localhost;..."
```

### Testing

```bash
export ASPNETCORE_ENVIRONMENT=Test
# InMemory wird automatisch verwendet
```

### Production

```bash
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__PostgreSQL="<production-connection-string>"
# oder
export ConnectionStrings__SqlServer="<production-connection-string>"
```

---

## Datenbankschema

### Migriert automatisch folgende Tabellen

1. **Pages** (CmsPage)
   - Indexe: TenantId, Slug, Visibility
   - Relationships: 1:N zu Sections

2. **Sections** (CmsSection)
   - Indexe: PageId
   - Relationships: 1:N zu Components

3. **Components** (CmsComponent)
   - Indexe: SectionId
   - JSONB Spalten: Variables, DataBinding, Styling

4. **ComponentDefinitions**
   - JSONB Spalten: Props, Slots, PresetVariants
   - Seeded mit 5 Standardkomponenten

---

## Troubleshooting

### PostgreSQL Connection Fehler

```
Error: Unable to connect to PostgreSQL server
```

**Lösung:**
```bash
# PostgreSQL überprüfen
brew services list

# Falls nicht läuft:
brew services start postgresql@15

# Connection testen:
psql -h localhost -U postgres -d postgres
```

### SQL Server Connection Fehler

```
Error: Unable to connect to SQL Server Express
```

**Lösung:**
```powershell
# SQL Server Status überprüfen
Get-Service MSSQL*

# Instance Info:
sqlcmd -S (local)\SQLEXPRESS -U sa -P <password>
```

### InMemory Test Fehler

```
Error: The specified schema doesn't exist
```

**Lösung:**
```csharp
// appsettings.Test.json überprüfen
{
  "DatabaseConfig": {
    "UseInMemory": true  // MUSS true sein!
  }
}
```

---

## Performance Tipps

### PostgreSQL Optimierung

```sql
-- Indexe für häufige Queries
CREATE INDEX idx_pages_tenant_visibility 
ON Pages(TenantId, Visibility);

-- JSONB Index für Komponenten
CREATE INDEX idx_components_variables 
ON Components USING GIN(Variables);
```

### SQL Server Optimierung

```sql
-- Columnstore Index für Analytiken
CREATE NONCLUSTERED COLUMNSTORE INDEX 
    IX_Components_Columnstore 
ON Components (ComponentType, IsVisible, CreatedAt);
```

### InMemory Tipps

- Jeder Test bekommt neue InMemory DB Instanz
- Keine gleichzeitigen Tests auf gleicher DB
- Perfekt für Unit & Integration Tests
- ~1000x schneller als echte DB

---

## Checkliste für Datenbanksetup

- [ ] Datenbankprovider wählen (PostgreSQL/SqlServer/InMemory)
- [ ] Connection String in appsettings.json konfigurieren
- [ ] appsettings.{Environment}.json erstellen
- [ ] DatabaseExtensions in Program.cs registrieren
- [ ] Initiale Migration erstellen: `dotnet ef migrations add InitialCreate`
- [ ] Migration anwenden: `dotnet ef database update`
- [ ] Tests ausführen: `dotnet test`
- [ ] Anwendung starten: `dotnet run`
- [ ] Health Check überprüfen: `http://localhost:5008/health`

---

## Migrationen verwalten

### Neue Migration erstellen

```bash
dotnet ef migrations add AddNewFeature \
  --output-dir Migrations \
  --context LayoutDbContext
```

### Migration Script generieren (für Deployment)

```bash
dotnet ef migrations script -o migrations.sql
# oder mit von/bis
dotnet ef migrations script Migration1 Migration2
```

### Alle Migrationen auflisten

```bash
dotnet ef migrations list
```

---

**Summary**: 
- ✅ PostgreSQL (Production-ready)
- ✅ SQL Server Express (Windows-lokal)
- ✅ InMemory (Schnelle Tests)
- ✅ Automatische Migration auf Startup
- ✅ Health Checks für beide echten DBs
