# Backend-Struktur

## Überblick

Das B2Connect Projekt hat zwei separate Backends, die nach der **Onion-Architektur** strukturiert sind:

| Backend | Port | Frontend | Typ |
|---------|------|----------|-----|
| **backend-store** | 6000 | frontend-store (5173) | Öffentliche API (read-only) |
| **backend-admin** | 6100 | frontend-admin (5174) | Admin API (CRUD + JWT) |

## Ordnerstruktur

```
backend/services/
├── backend-store/
│   ├── src/
│   │   ├── Core/
│   │   │   ├── Entities/         # Domain entities
│   │   │   ├── Interfaces/       # Repository/Service contracts
│   │   │   └── Exceptions/       # Business exceptions
│   │   ├── Application/
│   │   │   ├── DTOs/             # Data Transfer Objects
│   │   │   ├── Services/         # Application services
│   │   │   ├── Handlers/         # Command/Query handlers
│   │   │   └── Validators/       # Input validation
│   │   ├── Infrastructure/
│   │   │   ├── Repositories/     # Repository implementations
│   │   │   ├── Data/             # DbContext, migrations
│   │   │   ├── External/         # External service clients
│   │   │   └── Persistence/      # Database initialization
│   │   └── Presentation/
│   │       ├── Controllers/      # API endpoints
│   │       ├── Middleware/       # Custom middleware
│   │       ├── Configuration/    # Service registration
│   │       ├── Program.cs        # Entry point
│   │       └── appsettings.json  # Configuration
│   └── B2Connect.Store.csproj
│
├── backend-admin/
│   ├── src/
│   │   ├── Core/                 # (gleiche wie Store)
│   │   ├── Application/          # (gleiche wie Store)
│   │   ├── Infrastructure/       # (gleiche wie Store)
│   │   └── Presentation/         # (mit JWT auth)
│   └── B2Connect.Admin.csproj
│
└── ... (andere Services)
```

## Onion Architecture Layers

### 1. **Core Layer** (Innerster Ring)
- 📍 `src/Core/`
- **Verantwortung:** Geschäftslogik
- **Inhalte:**
  - Domain Entities (Product, Category, etc.)
  - Business Rules
  - Repository Interfaces
  - Value Objects
- **Regeln:** Keine externen Abhängigkeiten!

### 2. **Application Layer**
- 📍 `src/Application/`
- **Verantwortung:** Use Cases
- **Inhalte:**
  - DTOs (Datenübertragung)
  - Application Services (Orchestration)
  - Command/Query Handlers
  - Input Validation
- **Abhängig von:** Core

### 3. **Infrastructure Layer**
- 📍 `src/Infrastructure/`
- **Verantwortung:** Technische Implementierungen
- **Inhalte:**
  - Repository Implementations
  - Database Context
  - External Service Clients
  - Caching Implementations
- **Abhängig von:** Core + Application

### 4. **Presentation Layer** (Äußerster Ring)
- 📍 `src/Presentation/`
- **Verantwortung:** API Schnittstelle
- **Inhalte:**
  - Controllers (Endpoints)
  - Request/Response Handling
  - Middleware
  - Dependency Injection Setup
- **Abhängig von:** Alle inneren Layer

## Abhängigkeitsfluss

```
Presentation Layer
    ↓
Application Layer
    ↓
Infrastructure Layer
    ↓
Core Layer (keine externe Abhängigkeiten)
```

**Regel:** Innere Layer sind unabhängig von äußeren!

## Layer-spezifische READMEs

Jeder Layer hat ein eigenes README mit detaillierten Richtlinien:

- [Store Core](backend/services/backend-store/src/Core/README.md)
- [Store Application](backend/services/backend-store/src/Application/README.md)
- [Store Infrastructure](backend/services/backend-store/src/Infrastructure/README.md)
- [Store Presentation](backend/services/backend-store/src/Presentation/README.md)

Analog für `backend-admin`.

## Beispiel: Product Management

### Core Layer - Entity
```csharp
// src/Core/Entities/Product.cs
public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0) 
            throw new InvalidOperationException("Price cannot be negative");
        Price = newPrice;
    }
}
```

### Core Layer - Interface
```csharp
// src/Core/Interfaces/IProductRepository.cs
public interface IProductRepository
{
    Task<Product> GetAsync(Guid id);
    Task<IEnumerable<Product>> ListAsync();
    Task SaveAsync(Product product);
}
```

### Application Layer - DTO & Service
```csharp
// src/Application/DTOs/ProductDto.cs
public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// src/Application/Services/ProductService.cs
public class ProductService
{
    private readonly IProductRepository _repository;
    
    public async Task<ProductDto> GetProductAsync(Guid id)
    {
        var product = await _repository.GetAsync(id);
        return MapToDto(product);
    }
}
```

### Infrastructure Layer - Repository
```csharp
// src/Infrastructure/Repositories/ProductRepository.cs
public class ProductRepository : IProductRepository
{
    private readonly StoreDbContext _context;
    
    public async Task<Product> GetAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }
}
```

### Presentation Layer - Controller
```csharp
// src/Presentation/Controllers/ProductsController.cs
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> Get(Guid id)
    {
        var product = await _service.GetProductAsync(id);
        return Ok(product);
    }
}
```

## Entwicklungs-Workflow

### Neue Features hinzufügen

1. **Definiere die Entity** in `Core/Entities/`
2. **Definiere das Interface** in `Core/Interfaces/`
3. **Erstelle das DTO** in `Application/DTOs/`
4. **Schreibe den Service** in `Application/Services/`
5. **Implementiere das Repository** in `Infrastructure/Repositories/`
6. **Erstelle den Controller** in `Presentation/Controllers/`

### Best Practices

✅ **DO:**
- Interfaces in Core definieren
- DTOs für externe API verwenden
- Business Rules in Entities
- Dependency Injection nutzen

❌ **DON'T:**
- Direkter DB-Zugriff in Services
- Circular Dependencies
- Business Logic in Controllers
- Static Dependencies

## Projekt-Konfiguration

### build-backend Task
```bash
dotnet build backend/B2Connect.slnx
```

### Einzelne Backends bauen
```bash
dotnet build backend/services/backend-store/B2Connect.Store.csproj
dotnet build backend/services/backend-admin/B2Connect.Admin.csproj
```

### Mit Orchestration starten
```bash
dotnet run --project backend/services/Orchestration/B2Connect.Orchestration.csproj
```

## Dokumentation

Für detaillierte Architektur-Information siehe:
- [ONION_ARCHITECTURE.md](../../docs/ONION_ARCHITECTURE.md)
- [GATEWAY_SEPARATION.md](../../docs/GATEWAY_SEPARATION.md)
