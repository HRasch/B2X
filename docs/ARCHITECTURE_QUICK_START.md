# Architecture Quick Start for B2Connect

**Version:** 1.0 | **Last Updated:** 28. Dezember 2025

---

## 📐 Architecture Overview

B2Connect follows **Domain-Driven Design (DDD)** with **Bounded Contexts** and **Onion Architecture**:

```
┌─────────────────────────────────────┐
│ B2Connect DDD Microservices         │
├─────────────────────────────────────┤
│                                     │
│  BOUNDED CONTEXTS:                  │
│  ├─ Store (Public API)              │
│  ├─ Admin (Operations)              │
│  └─ Shared (Identity + Tenancy)     │
│                                     │
│  TECHNOLOGY:                        │
│  ├─ Framework: ASP.NET Core + Wolverine
│  ├─ Database: PostgreSQL            │
│  ├─ Cache: Redis                    │
│  ├─ Search: Elasticsearch           │
│  └─ Orchestration: .NET Aspire      │
│                                     │
└─────────────────────────────────────┘
```

## 🏗️ Onion Architecture (per Service)

Each bounded context (service) follows **4-layer onion architecture**:

```
Layer 1 (Outermost): PRESENTATION
  ├─ Controllers
  ├─ Middleware
  └─ Dependency Injection Configuration

Layer 2: INFRASTRUCTURE
  ├─ EF Core DbContext
  ├─ Repositories (implementations)
  ├─ External Services
  └─ Caching Layer

Layer 3: APPLICATION
  ├─ DTOs (Data Transfer Objects)
  ├─ Handlers (Wolverine Services)
  ├─ Validators (FluentValidation)
  └─ Mappers (AutoMapper)

Layer 4 (Innermost): CORE/DOMAIN
  ├─ Entities
  ├─ Value Objects
  ├─ Interfaces (IRepository contracts)
  └─ Domain Events
  
  ⚠️  RULE: Core has ZERO dependencies on outer layers!
```

## 🎯 Design Principles

### Dependency Inversion (DI)

```csharp
// ✅ CORRECT: Constructor injection
public class ProductService
{
    private readonly IProductRepository _repo;
    private readonly IEncryptionService _encryption;
    
    public ProductService(
        IProductRepository repo,
        IEncryptionService encryption)
    {
        _repo = repo;
        _encryption = encryption;
    }
}

// ❌ WRONG: Static dependencies (untestable)
public class ProductService
{
    private static IProductRepository Repo => ServiceLocator.Get<IProductRepository>();
}
```

### Single Responsibility Principle (SRP)

```csharp
// ✅ CORRECT: Each class has one reason to change
public class PriceCalculationService  // Responsibility: Calculate prices
{
    public decimal Calculate(Product product, string country) { }
}

public class AuditService  // Responsibility: Log actions
{
    public Task LogAsync(string action, object entity) { }
}

// ❌ WRONG: Multiple responsibilities (hard to test)
public class ProductServiceBigGodClass
{
    public Product CreateProduct() { }
    public void CalculateTax() { }
    public void SendEmail() { }
    public void LogToDatabase() { }  // Too many reasons to change!
}
```

### Loose Coupling

```csharp
// ✅ CORRECT: Depend on abstractions, not concrete types
public interface IProductRepository
{
    Task<Product> GetBySkuAsync(Guid tenantId, string sku);
}

public class ProductService
{
    public ProductService(IProductRepository repo) { }
}

// ❌ WRONG: Tight coupling to concrete type
public class ProductService
{
    private readonly SqlProductRepository _repo = new();  // Hard to test!
}
```

## 🔧 Technology Decisions

### Why Wolverine (not MediatR)?

| Aspect | Wolverine | MediatR |
|--------|-----------|---------|
| **Use Case** | Distributed microservices | In-process command bus |
| **Event Bus** | Built-in (Wolverine messaging) | External (RabbitMQ, etc.) |
| **HTTP Discovery** | Auto-discovers handlers → HTTP endpoints | Manual routing |
| **Scaling** | Better for service-to-service | Better for monolith |

**For B2Connect:** Wolverine is better because:
- ✅ Auto-discovers HTTP endpoints (no manual routing)
- ✅ Built-in event messaging (no external bus needed)
- ✅ Better for distributed microservices
- ✅ Simpler pattern (service methods → HTTP endpoints)

### Why PostgreSQL?

```
Evaluation Criteria:
├─ ✅ Multi-tenant support (row-level security)
├─ ✅ JSON support (JSONB for audit logs)
├─ ✅ Encryption features (pgcrypto)
├─ ✅ Strong ACID compliance
└─ ✅ Cost-effective at scale (open source)

Alternatives considered:
├─ ❌ SQL Server: Enterprise license cost
├─ ❌ MongoDB: Weak ACID (compliance risk)
└─ ❌ DynamoDB: Multi-tenant is harder
```

### Why Redis (not in-memory)?

```
Evaluation Criteria:
├─ ✅ Distributed caching (shared across instances)
├─ ✅ Session persistence
├─ ✅ Pub/Sub messaging
├─ ✅ No memory loss on app restart
└─ ✅ Cost-effective

Use Cases:
├─ Session storage (JWT tokens)
├─ Product cache (5-min TTL)
├─ Rate limiting counters
└─ Pub/Sub for events
```

## 📝 Common Patterns

### Repository Pattern

```csharp
// Core layer: Interface (no implementation details)
namespace B2Connect.Store.Catalog.Core.Interfaces;

public interface IProductRepository
{
    Task<Product> GetBySkuAsync(Guid tenantId, string sku, CancellationToken ct);
    Task AddAsync(Product product, CancellationToken ct);
    Task UpdateAsync(Product product, CancellationToken ct);
    Task DeleteAsync(Guid tenantId, Guid productId, CancellationToken ct);
}

// Infrastructure layer: Implementation (EF Core)
namespace B2Connect.Store.Catalog.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly CatalogDbContext _context;
    
    public async Task<Product> GetBySkuAsync(
        Guid tenantId, 
        string sku, 
        CancellationToken ct)
    {
        return await _context.Products
            .AsNoTracking()  // Read-only
            .Where(p => p.TenantId == tenantId && p.Sku == sku)
            .FirstOrDefaultAsync(ct);
    }
}

// DI Registration
services.AddScoped<IProductRepository, ProductRepository>();
```

### CQRS Handler Pattern (Wolverine)

```csharp
// Command (Plain POCO - NO IRequest interface!)
public class CreateProductCommand
{
    public string Sku { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Handler (Plain service - NO IRequestHandler!)
public class ProductService
{
    private readonly IProductRepository _repo;
    private readonly IValidator<CreateProductCommand> _validator;
    
    // ✅ Wolverine auto-discovers this method
    // ✅ Creates HTTP endpoint: POST /createproduct
    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand cmd,
        CancellationToken ct)
    {
        // Validate
        var validation = await _validator.ValidateAsync(cmd, ct);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);
        
        // Business logic
        var product = new Product(cmd.Sku, cmd.Name, cmd.Price);
        await _repo.AddAsync(product, ct);
        
        return new CreateProductResponse { Id = product.Id };
    }
}

// DI Registration (simple!)
services.AddScoped<ProductService>();
// That's it! Wolverine auto-discovers the HTTP endpoint.
```

### Validation Pattern

```csharp
// Core layer: Validator (no dependencies on Framework)
using FluentValidation;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU is required")
            .MaximumLength(50).WithMessage("SKU max 50 chars")
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("SKU must be alphanumeric");
        
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be > 0")
            .LessThanOrEqualTo(999999.99m).WithMessage("Price too high");
    }
}

// Application layer: Use in Handler
public class ProductService
{
    private readonly IValidator<CreateProductCommand> _validator;
    
    public async Task<Response> CreateProduct(
        CreateProductCommand cmd,
        CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(cmd, ct);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);
        
        // ... continue
    }
}

// DI Registration
services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
```

### Soft Delete Pattern

```csharp
// Core layer: Entity with soft delete
public class Product : AggregateRoot
{
    public Guid TenantId { get; set; }
    public string Sku { get; set; }
    public string Name { get; set; }
    
    // Soft delete fields
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}

// Application layer: Filter deleted items
public class ProductRepository : IProductRepository
{
    public async Task<Product> GetBySkuAsync(
        Guid tenantId, 
        string sku, 
        CancellationToken ct)
    {
        return await _context.Products
            .Where(p => !p.IsDeleted)  // Never return deleted items
            .Where(p => p.TenantId == tenantId && p.Sku == sku)
            .FirstOrDefaultAsync(ct);
    }
    
    public async Task DeleteAsync(
        Guid tenantId, 
        Guid productId, 
        CancellationToken ct)
    {
        var product = await _context.Products.FindAsync(productId, cancellationToken: ct);
        product.IsDeleted = true;
        product.DeletedAt = DateTime.UtcNow;
        product.DeletedBy = _currentUser.Id;
        await _context.SaveChangesAsync(ct);
    }
}

// Infrastructure layer: Apply soft delete filter globally
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);
    
    // Apply global filter: exclude deleted items from all queries
    builder.Entity<Product>()
        .HasQueryFilter(p => !p.IsDeleted);
}
```

### Multi-Tenancy Pattern

```csharp
// Core layer: Entity with tenant isolation
public class Product : AggregateRoot
{
    public Guid TenantId { get; set; }  // MUST have TenantId
    public string Sku { get; set; }
    public string Name { get; set; }
}

// Application layer: Always filter by TenantId
public class ProductRepository : IProductRepository
{
    private readonly ITenantContext _tenantContext;
    
    public async Task<Product> GetBySkuAsync(
        string sku, 
        CancellationToken ct)
    {
        var tenantId = _tenantContext.TenantId;  // Get from context
        
        return await _context.Products
            .Where(p => p.TenantId == tenantId)  // ⚠️  ALWAYS filter by tenant!
            .Where(p => p.Sku == sku)
            .FirstOrDefaultAsync(ct);
    }
}

// Middleware layer: Extract tenant from header
public class TenantMiddleware
{
    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        var tenantId = context.Request.Headers["X-Tenant-ID"].ToString();
        if (Guid.TryParse(tenantId, out var guid))
        {
            tenantContext.SetTenant(guid);
        }
        
        await _next(context);
    }
}
```

## 🚀 Getting Started (New Feature)

**Step-by-step for implementing a new feature:**

1. **Define Domain (Core Layer)**
   ```csharp
   // 1. Create entity in Core/Entities/
   public class Product : AggregateRoot { }
   
   // 2. Create interface in Core/Interfaces/
   public interface IProductRepository { }
   ```

2. **Create Application (Handler + Validator)**
   ```csharp
   // 3. Create command in Application/Commands/
   public class CreateProductCommand { }
   
   // 4. Create validator in Application/Validators/
   public class CreateProductValidator : AbstractValidator { }
   
   // 5. Create handler in Application/Handlers/
   public class ProductService { }
   ```

3. **Implement Infrastructure (Repository)**
   ```csharp
   // 6. Implement repository in Infrastructure/Repositories/
   public class ProductRepository : IProductRepository { }
   
   // 7. Add migration: dotnet ef migrations add AddProduct
   ```

4. **Register DI (Presentation)**
   ```csharp
   // 8. Register in Program.cs
   services.AddScoped<IProductRepository, ProductRepository>();
   services.AddScoped<ProductService>();
   services.AddValidatorsFromAssembly(typeof(CreateProductValidator).Assembly);
   ```

5. **Write Tests**
   ```csharp
   // 9. Create tests in tests/ProductServiceTests.cs
   [Fact]
   public async Task CreateProduct_ValidCommand_CreatesProduct() { }
   ```

---

**Document Owner:** Architecture Team  
**Last Updated:** 28. Dezember 2025
