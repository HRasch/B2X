# CQRS Refactoring - Alle Controllers

**Status**: 📋 Plan & Implementation für alle Admin API Controllers  
**Date**: 27. Dezember 2025

---

## 📊 Controller Status Übersicht

| Controller | Status | Methoden | Typ | Aktion |
|------------|--------|----------|-----|--------|
| **ProductsController** | ✅ Complete | 13 | GET/POST/PUT/DELETE | CQRS Ready |
| **CategoriesController** | ⏳ Pending | 8 | GET/POST/PUT/DELETE | Needs CQRS |
| **BrandsController** | ⏳ Pending | 6 | GET/POST/PUT/DELETE | Needs CQRS |
| **UsersController** | 🔄 Mixed | 5 | GET/POST/PUT/DELETE | Proxy (Keep As-Is) |

---

## 🎯 CategoriesController - Refactoring Plan

### Current Methods (8)

```csharp
// GET Endpoints (6)
GetCategory(id)              ← SERVICE CALL
GetCategoryBySlug(slug)      ← SERVICE CALL
GetRootCategories()          ← SERVICE CALL
GetChildCategories(parentId) ← SERVICE CALL
GetHierarchy()               ← SERVICE CALL
GetActiveCategories()        ← SERVICE CALL

// POST/PUT/DELETE Endpoints (2)
CreateCategory(dto)          ← SERVICE CALL
UpdateCategory(id, dto)      ← SERVICE CALL
DeleteCategory(id)           ← SERVICE CALL
```

### CQRS Implementation Plan

#### 1. Commands (3)
```csharp
CreateCategoryCommand(TenantId, Name, Slug, Description?, ParentId?)
UpdateCategoryCommand(TenantId, CategoryId, Name, Slug, Description?, ParentId?)
DeleteCategoryCommand(TenantId, CategoryId)
```

#### 2. Queries (6)
```csharp
GetCategoryQuery(TenantId, CategoryId)
GetCategoryBySlugQuery(TenantId, Slug)
GetRootCategoriesQuery(TenantId)
GetChildCategoriesQuery(TenantId, ParentId)
GetCategoryHierarchyQuery(TenantId)
GetActiveCategoriesQuery(TenantId)
```

#### 3. Handlers (9)
- CreateCategoryHandler
- UpdateCategoryHandler
- DeleteCategoryHandler
- GetCategoryHandler
- GetCategoryBySlugHandler
- GetRootCategoriesHandler
- GetChildCategoriesHandler
- GetCategoryHierarchyHandler
- GetActiveCategoriesHandler

#### 4. Request/Response DTOs
```csharp
CreateCategoryRequest(Name, Slug, Description?, ParentId?)
UpdateCategoryRequest(Name, Slug, Description?, ParentId?)
CategoryResult(Id, TenantId, Name, Slug, Description?, ParentId?, CreatedAt)
```

---

## 🎯 BrandsController - Refactoring Plan

### Current Methods (6)

```csharp
// GET Endpoints (4)
GetBrand(id)              ← SERVICE CALL
GetBrandBySlug(slug)      ← SERVICE CALL
GetActiveBrands()         ← SERVICE CALL
GetBrandsPaged()          ← SERVICE CALL

// POST/PUT/DELETE Endpoints (2)
CreateBrand(dto)          ← SERVICE CALL
UpdateBrand(id, dto)      ← SERVICE CALL
DeleteBrand(id)           ← SERVICE CALL
```

### CQRS Implementation Plan

#### 1. Commands (3)
```csharp
CreateBrandCommand(TenantId, Name, Slug, Logo?, Description?)
UpdateBrandCommand(TenantId, BrandId, Name, Slug, Logo?, Description?)
DeleteBrandCommand(TenantId, BrandId)
```

#### 2. Queries (4)
```csharp
GetBrandQuery(TenantId, BrandId)
GetBrandBySlugQuery(TenantId, Slug)
GetActiveBrandsQuery(TenantId)
GetBrandsPagedQuery(TenantId, PageNumber, PageSize)
```

#### 3. Handlers (7)
- CreateBrandHandler
- UpdateBrandHandler
- DeleteBrandHandler
- GetBrandHandler
- GetBrandBySlugHandler
- GetActiveBrandsHandler
- GetBrandsPagedHandler

#### 4. Request/Response DTOs
```csharp
CreateBrandRequest(Name, Slug, Logo?, Description?)
UpdateBrandRequest(Name, Slug, Logo?, Description?)
BrandResult(Id, TenantId, Name, Slug, Logo?, Description?, CreatedAt)
```

---

## 🔄 UsersController - Special Case

**Status**: ✅ Keep As-Is (BFF Pattern)

**Reason**: UsersController ist ein **Facade/Gateway** zu Identity Service

```csharp
// Aktuelle Architektur:
HTTP Request (Admin Frontend)
    ↓
UsersController (BFF - Backend For Frontend)
    ↓
Identity Service (Separate Microservice)
    ↓
Response
```

**Decision**: Nicht zu CQRS refaktorieren, da es externe Service proxied

**Benefit**: Einfache Trennung von Concerns zwischen Services

---

## 📋 Implementation Roadmap

### Phase 1: Categories (2-3h)
- [ ] CategoryCommands.cs erstellen (3 Commands)
- [ ] CategoryQueries.cs erstellen (6 Queries) - ODER in Commands kombinieren
- [ ] CategoryHandlers.cs erstellen (9 Handlers)
- [ ] CategoriesController refaktorieren (8 Methoden)
- [ ] Tests (optional)

### Phase 2: Brands (2-3h)
- [ ] BrandCommands.cs erstellen (3 Commands)
- [ ] BrandQueries.cs erstellen (4 Queries)
- [ ] BrandHandlers.cs erstellen (7 Handlers)
- [ ] BrandsController refaktorieren (6 Methoden)
- [ ] Tests (optional)

### Phase 3: Verification (1h)
- [ ] Build & Compile
- [ ] Wolverine Registration überprüfen
- [ ] Endpoints testen
- [ ] Documentation aktualisieren

### Total Effort: ~5-7 hours

---

## 🛠️ Implementation Steps

### Step 1: Categories Commands & Queries

**File**: `backend/BoundedContexts/Admin/API/src/Application/Commands/Categories/CategoryCommands.cs`

```csharp
using Wolverine;

namespace B2Connect.Admin.Application.Commands.Categories;

public record CreateCategoryCommand(
    Guid TenantId,
    string Name,
    string Slug,
    string? Description = null,
    Guid? ParentId = null) : IRequest<CategoryResult>;

public record UpdateCategoryCommand(
    Guid TenantId,
    Guid CategoryId,
    string Name,
    string Slug,
    string? Description = null,
    Guid? ParentId = null) : IRequest<CategoryResult>;

public record DeleteCategoryCommand(Guid TenantId, Guid CategoryId) : IRequest<bool>;

// Queries
public record GetCategoryQuery(Guid TenantId, Guid CategoryId) : IRequest<CategoryResult?>;
public record GetCategoryBySlugQuery(Guid TenantId, string Slug) : IRequest<CategoryResult?>;
public record GetRootCategoriesQuery(Guid TenantId) : IRequest<IEnumerable<CategoryResult>>;
public record GetChildCategoriesQuery(Guid TenantId, Guid ParentId) : IRequest<IEnumerable<CategoryResult>>;
public record GetCategoryHierarchyQuery(Guid TenantId) : IRequest<IEnumerable<CategoryResult>>;
public record GetActiveCategoriesQuery(Guid TenantId) : IRequest<IEnumerable<CategoryResult>>;

public record CategoryResult(
    Guid Id,
    Guid TenantId,
    string Name,
    string Slug,
    string? Description = null,
    Guid? ParentId = null,
    DateTime CreatedAt = default);
```

### Step 2: Categories Handlers

**File**: `backend/BoundedContexts/Admin/API/src/Application/Handlers/Categories/CategoryHandlers.cs`

```csharp
using Wolverine;
using B2Connect.Admin.Application.Commands.Categories;
using B2Connect.Admin.Core.Interfaces;

namespace B2Connect.Admin.Application.Handlers.Categories;

public class CreateCategoryHandler : ICommandHandler<CreateCategoryCommand, CategoryResult>
{
    private readonly ICategoryRepository _repository;
    private readonly ILogger<CreateCategoryHandler> _logger;

    public CreateCategoryHandler(ICategoryRepository repository, ILogger<CreateCategoryHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CategoryResult> Handle(CreateCategoryCommand command, CancellationToken ct)
    {
        _logger.LogInformation("Creating category '{Name}' for tenant {TenantId}", 
            command.Name, command.TenantId);

        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ArgumentException("Category name is required");

        if (string.IsNullOrWhiteSpace(command.Slug))
            throw new ArgumentException("Category slug is required");

        var category = new Category
        {
            Id = Guid.NewGuid(),
            TenantId = command.TenantId,
            Name = command.Name,
            Slug = command.Slug,
            Description = command.Description,
            ParentId = command.ParentId,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(category, ct);

        return new CategoryResult(
            category.Id, category.TenantId, category.Name, category.Slug,
            category.Description, category.ParentId, category.CreatedAt);
    }
}

public class GetCategoryHandler : IQueryHandler<GetCategoryQuery, CategoryResult?>
{
    private readonly ICategoryRepository _repository;

    public GetCategoryHandler(ICategoryRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<CategoryResult?> Handle(GetCategoryQuery query, CancellationToken ct)
    {
        var category = await _repository.GetByIdAsync(query.TenantId, query.CategoryId, ct);
        if (category == null)
            return null;

        return new CategoryResult(
            category.Id, category.TenantId, category.Name, category.Slug,
            category.Description, category.ParentId, category.CreatedAt);
    }
}

// ... weitere Handler
```

### Step 3: CategoriesController Refaktorieren

```csharp
[ApiController]
[Route("api/[controller]")]
[ValidateTenant]
public class CategoriesController : ApiControllerBase
{
    private readonly IMessageBus _messageBus;

    public CategoriesController(IMessageBus messageBus, ILogger<CategoriesController> logger) 
        : base(logger)
    {
        _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResult>> GetCategory(Guid id, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        var query = new GetCategoryQuery(tenantId, id);
        var category = await _messageBus.InvokeAsync<CategoryResult?>(query, ct);

        if (category == null)
            return NotFoundResponse($"Category {id} not found");

        return OkResponse(category);
    }

    // ... weitere Methoden
}
```

---

## 📁 File Structure (Nach Implementation)

```
backend/BoundedContexts/Admin/API/src/
├── Application/
│   ├── Commands/
│   │   ├── Products/
│   │   │   └── ProductCommands.cs ✅
│   │   ├── Categories/
│   │   │   └── CategoryCommands.cs ⏳
│   │   └── Brands/
│   │       └── BrandCommands.cs ⏳
│   └── Handlers/
│       ├── Products/
│       │   └── ProductHandlers.cs ✅
│       ├── Categories/
│       │   └── CategoryHandlers.cs ⏳
│       └── Brands/
│           └── BrandHandlers.cs ⏳
├── Presentation/
│   └── Controllers/
│       ├── ProductsController.cs ✅ (CQRS)
│       ├── CategoriesController.cs ⏳ (Service → CQRS)
│       ├── BrandsController.cs ⏳ (Service → CQRS)
│       ├── UsersController.cs ✅ (Keep Proxy)
│       └── ApiControllerBase.cs ✅
```

---

## ⚡ Quick Implementation Checklist

### Categories
- [ ] CategoryCommands.cs erstellen (3 commands + 6 queries + CategoryResult)
- [ ] CategoryHandlers.cs erstellen (9 handlers)
- [ ] CategoriesController refaktorieren (8 methods)
- [ ] ICategory Repository überprüfen (alle Methoden vorhanden?)
- [ ] Build & Test

### Brands
- [ ] BrandCommands.cs erstellen (3 commands + 4 queries + BrandResult)
- [ ] BrandHandlers.cs erstellen (7 handlers)
- [ ] BrandsController refaktorieren (6 methods)
- [ ] IBrand Repository überprüfen (alle Methoden vorhanden?)
- [ ] Build & Test

### Verification
- [ ] Alle Controllers CQRS-ready
- [ ] Alle Handler implementiert
- [ ] Wolverine Registration aktiv
- [ ] Dokumentation aktualisiert

---

## 📊 Final Metrics (nach Completion)

| Controller | Commands | Queries | Handlers | Methods | Status |
|------------|----------|---------|----------|---------|--------|
| Products | 3 | 10 | 12 | 13 | ✅ |
| Categories | 3 | 6 | 9 | 8 | ⏳ |
| Brands | 3 | 4 | 7 | 6 | ⏳ |
| **Total** | **9** | **20** | **28** | **27** | 🟡 |

---

## 🎓 Learning Pattern

Jeder Controller folgt dem gleichen Muster:

1. **Commands/Queries definieren** → Commands-File
2. **Handler implementieren** → Handlers-File
3. **Controller refaktorieren** → Service calls durch Bus dispatch
4. **Test & Verify** → Build & run

Dies ist **hochgradig wiederverwendbar** und **skalierbar** auf alle weiteren Entities!

---

## 📌 Nächste Aktion

**Option 1: Ich implementiere sofort**
- CategoryCommands + CategoryHandlers + CategoriesController refactored
- BrandCommands + BrandHandlers + BrandsController refactored
- Alles in 1-2h

**Option 2: Du implementierst selbst**
- Kopiere das Pattern von ProductsController
- Ersetze Product → Category/Brand
- Folge dem gleichen Muster

**Option 3: Hybrid**
- Ich erstelle die Commands & Handlers
- Du refaktorierst die Controller

---

**Welche Option möchtest du?** 🚀
