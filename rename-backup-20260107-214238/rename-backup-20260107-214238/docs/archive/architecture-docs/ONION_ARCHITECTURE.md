# Onion Architecture für B2X Backends

## Architektur-Überblick

Die Backends (Store und Admin) folgen der **Onion Architecture** (auch Hexagonal Architecture genannt), die eine klare Separation of Concerns und Unabhängigkeit von technischen Details gewährleistet.

```
┌─────────────────────────────────────────┐
│  Presentation Layer (API, Controllers)  │
│  - HTTP Requests/Responses              │
│  - Routing, Middleware                  │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│  Application Layer (Services, DTOs)     │
│  - Use Cases, Orchestration             │
│  - Business Logic Coordination           │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│  Infrastructure Layer (Repositories)    │
│  - Database Access, External Services   │
│  - Technical Details                    │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│  Core Layer (Entities, Interfaces)      │
│  - Domain Models, Business Rules        │
│  - Framework-Independent                │
└─────────────────────────────────────────┘
```

## Layer-Beschreibungen

### 1. Core Layer (Innerster Ring)
**Verantwortung:** Geschäftslogik und Domain Models

```
src/Core/
├── Entities/           # Domain entities
├── Interfaces/         # Repository & Service Contracts
├── Exceptions/         # Business exceptions
└── ValueObjects/       # Value objects
```

**Merkmale:**
- ✅ Keine externe Abhängigkeiten
- ✅ Geschäftsregeln sind zentral
- ✅ Framework-agnostisch
- ❌ Kein Database Code
- ❌ Kein HTTP Code

### 2. Application Layer
**Verantwortung:** Use Cases und Orchestration

```
src/Application/
├── DTOs/               # Data Transfer Objects
├── Services/           # Application Services
├── Handlers/           # Command/Query Handlers
├── Mappings/           # DTO Mappings
└── Validators/         # Input Validation
```

**Merkmale:**
- ✅ Abhängig von Core
- ✅ Orchestriert Business Logic
- ✅ Definiert Use Cases
- ❌ Kein direct database access
- ❌ Kein HTTP handling

### 3. Infrastructure Layer
**Verantwortung:** Datenbankzugriff und externe Services

```
src/Infrastructure/
├── Repositories/       # Repository Implementations
├── Data/               # DbContext, Migrations
├── External/           # External Service Clients
├── Persistence/        # Database Initialization
└── Services/           # Technical Services
```

**Merkmale:**
- ✅ Implementiert Core Interfaces
- ✅ Alle technischen Details
- ✅ Database & External Service Access
- ❌ Keine Business Logic
- ❌ Kein HTTP handling

### 4. Presentation Layer (Äußerster Ring)
**Verantwortung:** API-Schnittstelle

```
src/Presentation/
├── Controllers/        # API Controllers
├── Middleware/         # Custom Middleware
├── Configuration/      # Service Registration
├── Program.cs          # Entry Point
└── appsettings.json    # Configuration
```

**Merkmale:**
- ✅ HTTP Endpoints
- ✅ Abhängig von allen Layern
- ✅ Request/Response Handling
- ❌ Keine Business Logic
- ❌ Kein Database Code

## Dependency Flow

```
Presentation → Application → Core
    ↓              ↓          ↑
Infrastructure ──────────────┘
```

**Regel:** Innere Layer sind unabhängig von äußeren Layern.

## Beispiel: Product Management

### 1. Core Layer (Geschäftsregeln)
```csharp
namespace B2X.Store.Core.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    
    public bool IsAvailable => Price > 0;
    
    public void ChangePrice(decimal newPrice)
    {
        if (newPrice < 0) throw new InvalidProductException();
        Price = newPrice;
    }
}
```

### 2. Application Layer (Use Cases)
```csharp
namespace B2X.Store.Application.Services;

public interface IProductService
{
    Task<ProductDto> GetProductAsync(Guid id);
    Task<IEnumerable<ProductDto>> ListProductsAsync();
}

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    
    public async Task<ProductDto> GetProductAsync(Guid id)
    {
        var product = await _repository.GetAsync(id);
        return MapToDto(product);
    }
}
```

### 3. Infrastructure Layer (Datenbankzugriff)
```csharp
namespace B2X.Store.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly StoreDbContext _context;
    
    public async Task<Product> GetAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }
}
```

### 4. Presentation Layer (API)
```csharp
namespace B2X.Store.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        var product = await _service.GetProductAsync(id);
        return Ok(product);
    }
}
```

## Best Practices

### ✅ DO
- Definiere Interfaces in Core/Application
- Implementiere Interfaces in Infrastructure
- Verwende DTOs für externe Kommunikation
- Halte Business Rules in Entities
- Injiziere Abhängigkeiten

### ❌ DON'T
- Direkter Database Access in Services
- Static Dependencies
- Circular Dependencies
- Business Logic in Controllers
- Framework Code in Core

## Struktur für beide Backends

### backend-store (Port 6000)
```
backend-store/
├── src/
│   ├── Core/           (Domain entities)
│   ├── Application/    (Use cases)
│   ├── Infrastructure/ (Data access)
│   └── Presentation/   (API Gateway)
├── B2X.Store.csproj
└── Properties/
```

### backend-admin (Port 6100)
```
backend-admin/
├── src/
│   ├── Core/           (Domain entities)
│   ├── Application/    (Use cases)
│   ├── Infrastructure/ (Data access)
│   └── Presentation/   (API Gateway)
├── B2X.Admin.csproj
└── Properties/
```

## Weitere Information

Siehe README.md in jedem Layer für spezifische Richtlinien.
