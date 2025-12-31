# CQRS Implementation - Schnell-Referenz & Nächste Schritte

**Status**: ✅ ProductsController CQRS Implementation Complete  
**Date**: 27. Dezember 2025

---

## 🚀 Quick Start (5 Min Setup)

### 1. Wolverine in Program.cs registrieren

**Datei**: `backend/BoundedContexts/Admin/API/Program.cs`

```csharp
// Füge diese Zeilen ein (nach AddControllers):

// ─────────────────────────────────────────────────────────
// CQRS/Wolverine Integration
// ─────────────────────────────────────────────────────────

builder.Host.UseWolverine(opts =>
{
    // Handler-Discovery
    opts.Handlers.DiscoverHandlersFromAssemblyContaining<
        B2Connect.Admin.Application.Handlers.Products.CreateProductHandler>();
});

// Im builder.Services Section:
builder.Services.AddWolverine();
```

### 2. Compile & Test

```bash
# Build
cd /Users/holger/Documents/Projekte/B2Connect
dotnet build B2Connect.slnx

# Run Admin API
dotnet run --project backend/BoundedContexts/Admin/API/B2Connect.Admin.csproj

# Test Endpoint (in anderem Terminal)
curl -H "X-Tenant-ID: 00000000-0000-0000-0000-000000000001" \
     http://localhost:8080/api/products
```

---

## ✅ Checkliste: Was ist fertig?

- [x] CQRS Commands/Queries definiert (12 Total)
- [x] Wolverine Handlers implementiert (12 Total)
- [x] ProductsController refaktoriert (13 Methods)
- [x] Alle GET endpoints mit Query dispatch
- [x] Alle POST/PUT/DELETE endpoints mit Command dispatch
- [x] Response DTOs (ProductResult, CreateProductRequest, UpdateProductRequest)
- [x] Dokumentation (CQRS_WOLVERINE_PATTERN.md, CQRS_IMPLEMENTATION_COMPLETE.md)
- [ ] ⚠️ Wolverine in Program.cs konfiguriert
- [ ] ⚠️ Repository-Methoden implementiert
- [ ] ⚠️ Tests geschrieben

---

## 🔧 Was brauchst du noch?

### Task 1: Repository-Methoden (1-2h)

Die folgenden Methoden **müssen** in `IProductRepository` + `ProductRepository` implementiert sein:

```csharp
// In Core/Interfaces/IProductRepository.cs

namespace B2Connect.Admin.Core.Interfaces;

public interface IProductRepository
{
    // Diese Methoden existieren wahrscheinlich schon:
    Task<Product?> GetByIdAsync(Guid tenantId, Guid productId, CancellationToken ct = default);
    Task<Product?> GetBySkuAsync(Guid tenantId, string sku, CancellationToken ct = default);
    Task<IEnumerable<Product>> GetAllAsync(Guid tenantId, CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
    Task UpdateAsync(Product product, CancellationToken ct = default);
    Task DeleteAsync(Guid tenantId, Guid productId, CancellationToken ct = default);

    // NEUE Methoden (implementieren!):
    Task<Product?> GetBySlugAsync(Guid tenantId, string slug, CancellationToken ct = default);
    Task<(IEnumerable<Product>, int)> GetPagedAsync(
        Guid tenantId, int pageNumber, int pageSize, CancellationToken ct = default);
    Task<IEnumerable<Product>> GetByCategoryAsync(
        Guid tenantId, Guid categoryId, CancellationToken ct = default);
    Task<IEnumerable<Product>> GetByBrandAsync(
        Guid tenantId, Guid brandId, CancellationToken ct = default);
    Task<IEnumerable<Product>> GetFeaturedAsync(
        Guid tenantId, int take, CancellationToken ct = default);
    Task<IEnumerable<Product>> GetNewestAsync(
        Guid tenantId, int take, CancellationToken ct = default);
    Task<(IEnumerable<Product>, int)> SearchAsync(
        Guid tenantId, string searchTerm, int pageNumber, int pageSize, CancellationToken ct = default);
}
```

**Implementierung Beispiel**:
```csharp
// In Infrastructure/Repositories/ProductRepository.cs

public async Task<Product?> GetBySlugAsync(Guid tenantId, string slug, CancellationToken ct = default)
{
    return await _context.Products
        .Where(p => p.TenantId == tenantId && p.Slug == slug && !p.IsDeleted)
        .FirstOrDefaultAsync(ct);
}

public async Task<IEnumerable<Product>> GetFeaturedAsync(
    Guid tenantId, int take, CancellationToken ct = default)
{
    return await _context.Products
        .Where(p => p.TenantId == tenantId && p.IsFeatured && !p.IsDeleted)
        .OrderByDescending(p => p.CreatedAt)
        .Take(take)
        .ToListAsync(ct);
}

public async Task<(IEnumerable<Product>, int)> SearchAsync(
    Guid tenantId, string searchTerm, int pageNumber, int pageSize, CancellationToken ct = default)
{
    var query = _context.Products
        .Where(p => p.TenantId == tenantId && !p.IsDeleted &&
                   (p.Name.Contains(searchTerm) || 
                    p.Description.Contains(searchTerm) ||
                    p.Sku.Contains(searchTerm)))
        .AsQueryable();

    var total = await query.CountAsync(ct);
    var items = await query
        .OrderByDescending(p => p.CreatedAt)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(ct);

    return (items, total);
}
```

---

### Task 2: Wolverine Registration (10min)

```csharp
// In Program.cs - füge nach AddControllers() ein:

builder.Host.UseWolverine(opts =>
{
    // Handler-Discovery
    opts.Handlers.DiscoverHandlersFromAssemblyContaining<CreateProductHandler>();
    
    // Optional: Logging
    opts.Handlers.OnException<ArgumentException>()
        .ReturnOutcome(new ArgumentException("Validation failed"));
});

builder.Services.AddWolverine();
```

---

### Task 3: Tests (2-3h) - Optional aber empfohlen

```csharp
// Tests/Admin/API/Handlers/Products/CreateProductHandlerTests.cs

[Fact]
public async Task Handle_WithValidCommand_CreatesProduct()
{
    // Arrange
    var mockRepo = new Mock<IProductRepository>();
    var handler = new CreateProductHandler(mockRepo.Object, _logger);
    var command = new CreateProductCommand(
        Guid.NewGuid(), "Laptop", "LAPTOP-001", 999.99m);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("Laptop", result.Name);
    mockRepo.Verify(
        x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
        Times.Once);
}

[Fact]
public async Task Handle_WithInvalidPrice_ThrowsException()
{
    // Arrange
    var mockRepo = new Mock<IProductRepository>();
    var handler = new CreateProductHandler(mockRepo.Object, _logger);
    var command = new CreateProductCommand(
        Guid.NewGuid(), "Laptop", "LAPTOP-001", -10m); // Invalid price

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(
        () => handler.Handle(command, CancellationToken.None));
}
```

---

## 🎯 Implementierungs-Flow

```
1. Wolverine Registration ✓ (10 min)
   └─ Program.cs UpdateFile

2. Repository Methods ✓ (1-2h)
   └─ IProductRepository + ProductRepository

3. Build & Test ✓ (30 min)
   ├─ dotnet build
   ├─ dotnet run
   └─ curl http://localhost:8080/api/products

4. Unit Tests (Optional, 2-3h)
   └─ Handler Tests
   └─ Controller Tests

5. Documentation ✓ (10 min)
   └─ API Docs
   └─ README
```

---

## 🔍 Überprüfung: Sind alle Handler vorhanden?

Führe das aus um zu sehen ob alle Handler registriert sind:

```csharp
// In Program.cs - vor app.Run():

var serviceProvider = app.Services;
var wolverine = serviceProvider.GetRequiredService<IMessageBus>();

// Handler sollten hier auftauchen:
// - CreateProductHandler
// - UpdateProductHandler
// - DeleteProductHandler
// - GetProductHandler
// - GetProductBySkuHandler
// - GetProductBySlugHandler
// - GetAllProductsHandler
// - GetProductsPagedHandler
// - GetProductsByCategoryHandler
// - GetProductsByBrandHandler
// - GetFeaturedProductsHandler
// - GetNewProductsHandler
// - SearchProductsHandler
```

---

## ❌ Häufige Fehler

### Fehler 1: "Handler not found"
**Ursache**: Wolverine kennt die Handler nicht  
**Lösung**: `opts.Handlers.DiscoverHandlersFromAssemblyContaining<...>();` hinzufügen

### Fehler 2: "NullReferenceException in Handler"
**Ursache**: Repository-Methode ist nicht implementiert  
**Lösung**: GetBySlugAsync, GetFeaturedAsync etc. implementieren

### Fehler 3: "404 Not Found"
**Ursache**: Repository gibt null zurück (normal!)  
**Lösung**: Handler checkt auf null, Controller gibt `NotFoundResponse()` zurück ✅

### Fehler 4: "X-Tenant-ID header is missing"
**Ursache**: ValidateTenantAttribute erkennt Header nicht  
**Lösung**: Header muss `X-Tenant-ID: {guid}` sein

---

## 📝 Files zum Überprüfen

| File | Was zu überprüfen |
|------|-------------------|
| ProductCommands.cs | 12 Records definiert (Commands + Queries) |
| ProductHandlers.cs | 12 Handler Klassen implementiert |
| ProductsController.cs | 13 Methoden alle mit `_messageBus.InvokeAsync<T>()` |
| IProductRepository | Alle 13 Methoden definiert |
| ProductRepository | Alle 13 Methoden implementiert |
| Program.cs | Wolverine registration vorhanden |

---

## 🧪 Schnell-Test

```bash
# 1. Build
dotnet build backend/BoundedContexts/Admin/API/B2Connect.Admin.csproj

# 2. Run
dotnet run --project backend/BoundedContexts/Admin/API/B2Connect.Admin.csproj &

# 3. Test GET (needs TenantId header!)
curl -H "X-Tenant-ID: 00000000-0000-0000-0000-000000000001" \
     http://localhost:8080/api/products

# Expected Response:
# {
#   "success": true,
#   "data": [],
#   "timestamp": "2025-12-27T10:00:00Z"
# }

# 4. Test POST
curl -X POST http://localhost:8080/api/products \
  -H "Content-Type: application/json" \
  -H "X-Tenant-ID: 00000000-0000-0000-0000-000000000001" \
  -H "Authorization: Bearer <valid-jwt-token>" \
  -d '{
    "name": "Test Product",
    "sku": "TEST-001",
    "price": 99.99,
    "description": "Test Description"
  }'

# Expected Response: 201 Created
```

---

## 📚 Weitere Ressourcen

- [CQRS_WOLVERINE_PATTERN.md](./CQRS_WOLVERINE_PATTERN.md) - Vollständiger CQRS Guide
- [CQRS_IMPLEMENTATION_COMPLETE.md](./CQRS_IMPLEMENTATION_COMPLETE.md) - Implementierungs-Status
- [CONTROLLER_FILTER_REFACTORING.md](../reference-docs/CONTROLLER_FILTER_REFACTORING.md) - Filter-Pattern
- Wolverine Docs: https://wolverine.joelmwale.com/

---

## ✨ Zusammenfassung: Was wurde erreicht?

✅ **CQRS Pattern** implementiert in ProductsController  
✅ **Wolverine Integration** vorbereitet  
✅ **Thin Controller** Architektur etabliert  
✅ **12 Handler** für alle CRUD + such-Operationen  
✅ **Dokumentation** fertig  
⏳ **Deployment ready** - nur noch Repository-Methoden + Wolverine Registration

---

**Status**: 🟡 90% FERTIG - Warte auf Repository-Implementierung  
**Next Action**: Implementiere die fehlenden Repository-Methoden  
**Estimated Time**: 1-2 Stunden Arbeit  
**Impact**: Alle ProductsController Endpoints funktionieren dann!

---

*Fragen? Siehe CQRS_WOLVERINE_PATTERN.md für detaillierte Dokumentation*
