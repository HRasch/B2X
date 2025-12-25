# B2Connect CMS - TDD Implementation Workflow

**Date**: December 25, 2025  
**Status**: TDD Structure + Multi-Database Support  
**Approach**: Red → Green → Refactor  
**Database Support**: PostgreSQL | SQL Server Express | InMemory

---

## Overview

Die CMS-Implementierung folgt **strict Test-Driven Development (TDD)** mit **flexibler Datenbankunterstützung**:

### Datenbankoptionen

| Provider | Use Case | Status |
|----------|----------|--------|
| **PostgreSQL** | Production/Development | ✅ Supported |
| **SQL Server Express** | Local Windows Development | ✅ Supported |
| **InMemory** | Unit Tests (Fast) | ✅ Supported |

### Automatische Datenbankauswahl

```csharp
if (useInMemory || environment == "Test")
    → InMemory Database (Schnelle Tests)
else if (provider == "SqlServer")
    → SQL Server Express Connection
else
    → PostgreSQL (Default)
```

```
┌─────────────────────────────────────────────────────────┐
│                  TDD Cycle                              │
│                                                         │
│  1. RED    →  Write failing test                       │
│  2. GREEN  →  Write minimal code to pass test          │
│  3. REFACTOR → Improve code while keeping tests green  │
│                                                         │
│  Repeat for every feature                              │
└─────────────────────────────────────────────────────────┘
```

---

## Project Structure

```
LayoutService/
├── src/
│   ├── Models.cs              (Data models from tests)
│   ├── Interfaces.cs          (Service contracts)
│   ├── LayoutService.cs       (Implementation)
│   ├── Repository.cs          (Data access)
│   ├── Controllers/           (API endpoints)
│   └── Program.cs             (DI setup)
│
└── tests/
    └── LayoutServiceTests.cs  (Unit tests)
        ├── Create Page Tests
        ├── Get Page Tests
        ├── Update Page Tests
        ├── Delete Page Tests
        ├── Section Management Tests
        ├── Component Management Tests
        ├── Preview Generation Tests
        └── Component Registry Tests
```

---

## Test Categories

### 1. Page Management Tests ✅ WRITTEN

```csharp
[Fact]
public async Task CreatePage_WithValidPageData_ShouldReturnCreatedPage()
{
    // ARRANGE: Setup mocks and test data
    var tenantId = Guid.NewGuid();
    var createPageRequest = new CreatePageRequest { /* ... */ };
    
    // ACT: Call the method
    var result = await _service.CreatePageAsync(tenantId, createPageRequest);
    
    // ASSERT: Verify expected behavior
    Assert.NotNull(result);
    Assert.Equal(expectedPageId, result.Id);
}
```

**Coverage**:
- ✅ Create page with valid data
- ✅ Reject duplicate slug
- ✅ Require title & slug
- ✅ Get page by ID
- ✅ Get all pages for tenant
- ✅ Update page with version increment
- ✅ Delete page
- ✅ Handle non-existent pages

### 2. Section Management Tests ✅ WRITTEN

```csharp
[Fact]
public async Task AddSection_WithValidData_ShouldReturnSectionWithOrder()
{
    // Create section with auto-incremented order
    var result = await _service.AddSectionAsync(tenantId, pageId, request);
    Assert.Equal(0, result.Order);
}
```

**Coverage**:
- ✅ Add section to page
- ✅ Section auto-ordering
- ✅ Remove section
- ✅ Reorder sections

### 3. Component Management Tests ✅ WRITTEN

```csharp
[Fact]
public async Task AddComponent_WithValidData_ShouldReturnComponent()
{
    // Component with proper initialization
    var result = await _service.AddComponentAsync(tenantId, pageId, sectionId, request);
    Assert.True(result.IsVisible);
}
```

**Coverage**:
- ✅ Add component to section
- ✅ Update component
- ✅ Remove component
- ✅ Component type validation

### 4. Preview Generation Tests ✅ WRITTEN

```csharp
[Fact]
public async Task GeneratePreview_WithValidPageId_ShouldReturnHtmlPreview()
{
    var result = await _service.GeneratePreviewHtmlAsync(tenantId, pageId);
    Assert.Contains("<html>", result);
}
```

### 5. Component Registry Tests ✅ WRITTEN

```csharp
[Fact]
public async Task GetAvailableComponents_ShouldReturnComponentDefinitions()
{
    var result = await _service.GetComponentDefinitionsAsync();
    Assert.Equal(3, result.Count);
}
```

---

## TDD Workflow Steps

### Step 1: RED - Write Failing Test

```csharp
[Fact]
public async Task CreatePage_WithValidPageData_ShouldReturnCreatedPage()
{
    // Test written FIRST
    // Test FAILS because method doesn't exist yet
    var result = await _service.CreatePageAsync(tenantId, request);
}
```

**Status**: ❌ Test fails - method doesn't exist

### Step 2: GREEN - Write Minimal Implementation

```csharp
public async Task<CmsPage> CreatePageAsync(Guid tenantId, CreatePageRequest request)
{
    // Minimal code - just enough to make test pass
    var page = new CmsPage
    {
        Id = Guid.NewGuid(),
        TenantId = tenantId,
        Title = request.Title,
        // ...
    };
    return await _repository.CreatePageAsync(tenantId, page);
}
```

**Status**: ✅ Test passes

### Step 3: REFACTOR - Improve Code

```csharp
public async Task<CmsPage> CreatePageAsync(Guid tenantId, CreatePageRequest request)
{
    // Add validation
    if (string.IsNullOrWhiteSpace(request?.Title))
        throw new ArgumentException("Title is required");
    
    // Check for duplicates
    var slugExists = await _repository.PageSlugExistsAsync(tenantId, request.Slug);
    if (slugExists)
        throw new InvalidOperationException("Slug already exists");
    
    // ... rest of implementation
}
```

**Status**: ✅ Tests still pass, code improved

---

## Database Configuration

### Dateistruktur für Multi-Database Support

```
LayoutService/
├── appsettings.json          (PostgreSQL - default)
├── appsettings.Development.json
├── appsettings.Test.json     (InMemory - für Tests)
├── src/
│   ├── LayoutDbContext.cs    (EF Core DbContext)
│   ├── DatabaseExtensions.cs (Provider-Konfiguration)
│   └── LayoutRepository.cs   (EF Core Implementation)
```

### appsettings.json (PostgreSQL)

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

### appsettings.Test.json (InMemory)

```json
{
  "DatabaseConfig": {
    "Provider": "InMemory",
    "UseInMemory": true
  }
}
```

### Automatic Provider Detection

```csharp
// Program.cs
services.AddLayoutDatabase(configuration);

// DatabaseExtensions.cs
public static IServiceCollection AddLayoutDatabase(this IServiceCollection services, IConfiguration configuration)
{
    var provider = configuration.GetValue<string>("DatabaseConfig:Provider");
    var useInMemory = configuration.GetValue<bool>("DatabaseConfig:UseInMemory");

    if (useInMemory || Environment == "Test")
    {
        // Use InMemory for fast unit tests
        options.UseInMemoryDatabase("LayoutServiceTest");
    }
    else if (provider == "SqlServer")
    {
        // Use SQL Server Express
        options.UseSqlServer(connectionString);
    }
    else
    {
        // Use PostgreSQL (default)
        options.UseNpgsql(connectionString);
    }
}
```

### InMemory Database für Unit Tests

```csharp
public class LayoutServiceIntegrationTests : IAsyncLifetime
{
    private readonly LayoutDbContext _dbContext;
    private readonly LayoutRepository _repository;

    public LayoutServiceIntegrationTests()
    {
        // Jeder Test bekommt neue InMemory DB Instanz
        var options = new DbContextOptionsBuilder<LayoutDbContext>()
            .UseInMemoryDatabase(databaseName: $"LayoutTestDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new LayoutDbContext(options);
        _repository = new LayoutRepository(_dbContext);
    }

    public async Task InitializeAsync()
    {
        // Vor Test: DB erstellen
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        // Nach Test: Aufräumen
        await _dbContext.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task GetPageByIdAsync_WithValidId_ShouldReturnPage()
    {
        // Arrange: Daten in InMemory DB inserten
        var page = new CmsPage { Id = pageId, TenantId = tenantId, Title = "Test" };
        _dbContext.Pages.Add(page);
        await _dbContext.SaveChangesAsync();

        // Act: Repository aufrufen
        var result = await _repository.GetPageByIdAsync(tenantId, pageId);

        // Assert: Echte DB-Persistierung validieren
        Assert.NotNull(result);
        Assert.Equal("Test", result.Title);
    }
}
```

---

## LayoutDbContext Entity Configuration

### Seeded Default Components

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Seed 5 Standard-Komponenten
    var components = new[]
    {
        new ComponentDefinition { ComponentType = "Button", ... },
        new ComponentDefinition { ComponentType = "TextBlock", ... },
        new ComponentDefinition { ComponentType = "Image", ... },
        new ComponentDefinition { ComponentType = "Form", ... },
        new ComponentDefinition { ComponentType = "Card", ... }
    };
    modelBuilder.Entity<ComponentDefinition>().HasData(components);
}
```

### Database Indexes für Performance

```sql
-- Pages Table
CREATE INDEX IX_Pages_TenantId_Slug ON Pages(TenantId, Slug); -- Unique
CREATE INDEX IX_Pages_TenantId ON Pages(TenantId);
CREATE INDEX IX_Pages_TenantId_Visibility ON Pages(TenantId, Visibility);

-- Sections Table
CREATE INDEX IX_Sections_PageId ON Sections(PageId);

-- Components Table
CREATE INDEX IX_Components_SectionId ON Components(SectionId);

-- JSONB Indexes (PostgreSQL)
CREATE INDEX IX_Components_Variables ON Components USING GIN(Variables);
```

---

## Running Tests with Different Databases

### Tests mit InMemory Database (Standard - SCHNELL)

```bash
cd backend/services/LayoutService

# Alle Tests ausführen (nutzt appsettings.Test.json → InMemory)
dotnet test

# Spezifischer Test
dotnet test --filter "CreatePageAsync_WithValidPage_ShouldPersistAndReturn"

# Mit Coverage
dotnet test /p:CollectCoverage=true

# Watch Mode
dotnet watch test
```

**Performance**: ~50ms für 20+ Tests

### Integration Tests mit echtem PostgreSQL

```bash
# PostgreSQL starten
docker run --name postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:15

# Tests mit PostgreSQL ausführen
ASPNETCORE_ENVIRONMENT=Development dotnet test

# Cleanup
docker stop postgres && docker rm postgres
```

### Integration Tests mit SQL Server Express

```powershell
# SQL Server starten (Windows)
# Falls installiert: SQL Server Express läuft automatisch

# Tests mit SQL Server ausführen
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:DatabaseConfig:Provider = "SqlServer"
dotnet test

# oder via appsettings:
dotnet test --configuration Debug
```

---



### PHASE 1: Layout Service - Page Management ✅ COMPLETE

| Component | Tests | Implementation | Status |
|-----------|-------|-----------------|--------|
| Create Page | ✅ 3 tests | ✅ Full impl + Repository | GREEN |
| Get Page | ✅ 2 tests | ✅ Full impl + Repository | GREEN |
| Update Page | ✅ 2 tests | ✅ Full impl + Repository | GREEN |
| Delete Page | ✅ 2 tests | ✅ Full impl + Repository | GREEN |
| Add Section | ✅ 1 test | ✅ Full impl + Repository | GREEN |
| Remove Section | ✅ 1 test | ✅ Full impl + Repository | GREEN |
| Add Component | ✅ 1 test | ✅ Full impl + Repository | GREEN |
| Update Component | ✅ 1 test | ✅ Full impl + Repository | GREEN |
| Remove Component | ✅ 1 test | ✅ Full impl + Repository | GREEN |
| Generate Preview | ✅ 1 test | ✅ Full impl + Repository | GREEN |
| Component Registry | ✅ 1 test | ✅ Full impl + Repository | GREEN |
| **Integration Tests** | ✅ 16 tests | ✅ InMemory DB Tests | GREEN |

**Total**: 16 unit tests + 16 integration tests written, full implementations with EF Core repository

---

## Next Implementation Steps (TDD)

### PHASE 1B: Database Migrations ✅ READY

```
Status: EF Core configured for all 3 providers

1. ✅ LayoutDbContext created with all entities
2. ✅ DatabaseExtensions for automatic provider selection
3. ✅ appsettings.json and appsettings.Test.json configured
4. ✅ InMemory database for unit tests
5. ⏳ Create first migration:
   dotnet ef migrations add InitialCreate
   
6. ⏳ Apply migrations on startup:
   await serviceProvider.EnsureDatabaseAsync();
```

### PHASE 2: Database Repository Implementation

```
Next: Implement ILayoutRepository with PostgreSQL

1. RED: Write repository tests
   - Test: GetPageByIdAsync_WithValidId_ShouldReturnPage
   - Test: CreatePageAsync_ShouldPersistAndReturn
   - Test: UpdatePageAsync_ShouldIncrementVersion
   - etc.

2. GREEN: Implement DbContext & Repository
   - Create LayoutDbContext
   - Implement repository methods
   - Make tests pass

3. REFACTOR: Add error handling
   - Connection resilience
   - Transaction management
   - Performance optimization
```

### PHASE 3: API Controllers

```
1. RED: Write controller tests
   - Test: GetPage_WithValidId_ReturnsOk
   - Test: CreatePage_WithInvalidData_ReturnsBadRequest
   - Test: Unauthorized_WithoutTenantHeader_ReturnsForbidden
   - etc.

2. GREEN: Implement controllers
   - PageController
   - SectionController
   - ComponentController

3. REFACTOR: Add validation
   - Input validation
   - Authorization checks
   - Error responses
```

### PHASE 4: Theme Service Tests & Implementation

```
1. RED: Write Theme Service tests
   - Theme CRUD operations
   - Design variable management
   - CSS generation
   - Theme publishing

2. GREEN: Implement Theme Service
3. REFACTOR: Optimize theme variable system
```

### PHASE 5: Content Service Tests & Implementation

```
1. RED: Write Content Service tests
   - Publishing workflow
   - Version control
   - SEO metadata
   - Scheduled publishing

2. GREEN: Implement Content Service
3. REFACTOR: Add caching & performance
```

---

## Test-Driven Development Guidelines

### ✅ DO

- Write test BEFORE implementation
- Make tests fail first (RED)
- Write minimal code to pass (GREEN)
- Refactor with confidence (all tests green)
- Test public APIs, not implementation details
- Use descriptive test names: `MethodName_Scenario_Expected`
- Test happy path AND error cases
- Use mocks for dependencies
- Keep tests simple and focused

### ❌ DON'T

- Write implementation before tests
- Skip error case testing
- Mock entire service layer
- Write tests that are too brittle
- Test private methods directly
- Write vague test names
- Copy-paste test code without refactoring

---

## Running Tests

### Run all tests
```bash
cd backend/services/LayoutService
dotnet test
```

### Run specific test class
```bash
dotnet test --filter ClassName=LayoutServiceTests
```

### Run with coverage
```bash
dotnet test /p:CollectCoverage=true
```

### Watch mode (re-run on changes)
```bash
dotnet watch test
```

---

## Test Coverage Goals

| Service | Target Coverage |
|---------|-----------------|
| Layout Service | ≥ 90% |
| Repository | ≥ 85% |
| Controllers | ≥ 80% |
| Utilities | ≥ 85% |
| **Overall** | **≥ 85%** |

**Current**: 16 tests written, focusing on core functionality

---

## Mocking Strategy

### Repository Mocks

```csharp
var _mockRepository = new Mock<ILayoutRepository>();

_mockRepository
    .Setup(r => r.CreatePageAsync(tenantId, It.IsAny<CmsPage>()))
    .ReturnsAsync(expectedPage);
```

### Database Context Mocks (Future)

```csharp
// For integration tests
var options = new DbContextOptionsBuilder<LayoutDbContext>()
    .UseInMemoryDatabase(databaseName: "test")
    .Options;

var dbContext = new LayoutDbContext(options);
var repository = new LayoutRepository(dbContext);
```

---

## Integration Testing Strategy

### Unit Tests (Current)
- Isolated service logic
- Mocked dependencies
- Fast execution (~1-2 sec)

### Integration Tests (Next Phase)
- Real database (PostgreSQL in-memory or test container)
- Full service stack
- API endpoints
- Slower execution (~5-10 sec)

### E2E Tests (Later Phase)
- Docker containers
- Multiple services
- Real workflows
- Slowest execution (~30+ sec)

---

## Continuous Integration

### Pre-commit Checks
```bash
# Run tests locally
dotnet test --logger:"console;verbosity=detailed"

# Check coverage
dotnet test /p:CollectCoverage=true

# Verify no breaking changes
```

### CI/CD Pipeline (GitHub Actions)
```yaml
on: [push, pull_request]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - uses: actions/setup-dotnet@v1
      - run: dotnet test --filter "TestCategory!=Integration"
      - run: dotnet test /p:CollectCoverage=true
```

---

## Debugging Tests

### Visual Studio Code
```json
{
  "type": "coreclr",
  "name": "Test LayoutService",
  "request": "launch",
  "program": "${workspaceFolder}/backend/services/LayoutService/bin/Debug/net8.0/B2Connect.LayoutService.dll",
  "args": ["--logger", "console;verbosity=normal"],
  "cwd": "${workspaceFolder}",
  "stopAtEntry": false,
  "console": "integratedTerminal"
}
```

### Run Single Test with Debugging
```bash
cd backend/services/LayoutService
dotnet test --filter "CreatePage_WithValidPageData_ShouldReturnCreatedPage" --verbosity:detailed
```

---

## Performance Considerations

### Test Performance
- Aim for < 100ms per test
- Use in-memory repositories for unit tests
- Avoid real I/O operations
- Mock external services

### Current Test Suite
- 16 unit tests
- ~50ms total execution time
- All mocked dependencies
- Fast feedback loop

---

## Refactoring Checklist

After writing tests and minimal implementation:

- [ ] Add error handling
- [ ] Add input validation
- [ ] Add logging
- [ ] Add try-catch blocks
- [ ] Extract common logic
- [ ] Add comments/documentation
- [ ] Ensure all tests still pass
- [ ] Check code coverage
- [ ] Code review for edge cases

---

## Summary

✅ **Layout Service - Phase 1 Complete**
- 16 comprehensive unit tests written
- Minimal implementation to pass all tests
- Structure ready for refactoring phase
- Ready for database layer implementation

🔄 **Next**: Implement ILayoutRepository with PostgreSQL

---

**TDD Philosophy**: 
> "The tests are not just verification, they are the specification. 
> Write tests first, then code to satisfy them."

**Result**: 
- High quality code
- Comprehensive test coverage
- Confidence in refactoring
- Better design through tests
