# 💻 Backend Developer Agent Context

**Version:** 1.0 | **Focus:** Feature Implementation with Wolverine CQRS | **Last Updated:** 28. Dezember 2025

---

## 🎯 Your Mission

Build business features with **integrated compliance**. Every feature you build includes:
- Wolverine HTTP handlers (NOT MediatR!)
- CQRS pattern (Commands + Queries)
- FluentValidation on all inputs
- Audit logging for all changes
- Tenant isolation in all queries
- 80%+ test coverage

**Key Components:**
- **P0.1 Integration:** Audit logging in all services
- **P0.2 Integration:** Encryption for PII/cost data
- **P0.6:** E-Commerce features (VAT, invoices, withdrawals)
- **P0.9:** E-Rechnung (ZUGFeRD invoice generation)

---

## 📚 ONLY READ THESE (Stop Reading Anything Else)

### Critical Entry Points
1. **[docs/by-role/BACKEND_DEVELOPER.md](../../docs/by-role/BACKEND_DEVELOPER.md)** - Your guide (START HERE)
2. **[.github/copilot-instructions.md](../../.github/copilot-instructions.md)** - Architecture & patterns (READ FULL DOCUMENT)
3. **[docs/api/WOLVERINE_HTTP_ENDPOINTS.md](../../docs/api/WOLVERINE_HTTP_ENDPOINTS.md)** - Wolverine-specific patterns

### Reference by Feature Type
- **HTTP Endpoints:** docs/api/WOLVERINE_HTTP_ENDPOINTS.md
- **CQRS Pattern:** docs/api/CQRS_WOLVERINE_PATTERN.md
- **Onion Architecture:** docs/ONION_ARCHITECTURE.md
- **DDD Bounded Contexts:** docs/architecture/DDD_BOUNDED_CONTEXTS.md
- **E-Commerce (P0.6):** docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md §P0.6 (lines 840-1200)
- **E-Rechnung (P0.9):** docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md §P0.9 (lines 1580-1750)
- **Testing:** docs/TESTING_FRAMEWORK_GUIDE.md
- **Code Examples:** backend/Domain/Identity/src/Handlers/ (reference implementations)

---

## ⚠️ CRITICAL: Wolverine Only (NOT MediatR!)

### ✅ CORRECT Wolverine Pattern

```csharp
// Step 1: Plain POCO Command (NO IRequest interface!)
public class CreateProductCommand
{
    public string Sku { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Step 2: Service Handler (NO IRequestHandler!)
public class ProductService
{
    private readonly IProductRepository _repo;
    private readonly IValidator<CreateProductCommand> _validator;
    private readonly IAuditService _audit;
    
    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        // Validate input
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return BadRequest(validation.Errors);
        
        // Business logic
        var product = new Product(request.Sku, request.Name, request.Price);
        
        // Persist (triggers audit logging via interceptor)
        await _repo.AddAsync(product, cancellationToken);
        
        return Ok(new CreateProductResponse { Id = product.Id });
    }
}

// Step 3: Register in DI (simple!)
builder.Services.AddScoped<ProductService>();

// Step 4: Wolverine auto-discovers HTTP endpoint
// POST /createproduct → Automatically created!
```

### ❌ WRONG MediatR Pattern (DO NOT USE!)

```csharp
// WRONG: IRequest interface
public record CreateProductCommand(string Sku) : IRequest<ProductDto>;

// WRONG: IRequestHandler
public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto> { }

// WRONG: AddMediatR
builder.Services.AddMediatR();
```

**Reference:** [backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs](../../backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs)

---

## 🏗️ Onion Architecture (Each Service Must Follow)

```
Service/
├── Core/                       # Domain Layer (Innermost)
│   ├── Entities/               # Product, User, Order
│   ├── ValueObjects/           # Price, SKU, Email
│   ├── Interfaces/             # IProductRepository
│   └── Events/                 # ProductCreatedEvent
│
├── Application/                # Application Layer
│   ├── DTOs/                   # ProductDto, ProductCreateDto
│   ├── Handlers/               # Wolverine Services (NOT MediatR!)
│   ├── Validators/             # FluentValidation
│   └── Mappers/                # AutoMapper profiles
│
├── Infrastructure/             # Infrastructure Layer
│   ├── Data/                   # EF Core DbContext
│   ├── Repositories/           # Repository implementations
│   └── External/               # External services
│
└── Program.cs                  # Presentation Layer entry
```

**Rule:** Dependencies ALWAYS point inward. Core has ZERO framework dependencies.

---

## ✅ Code Generation Checklist (Before Submitting Any PR)

### Architecture & Patterns
- [ ] **Wolverine pattern** - Service with public async methods (NOT MediatR)
- [ ] **Onion architecture** - Dependencies point inward
- [ ] **Repository interface** in Core, implementation in Infrastructure
- [ ] **DTOs** for API boundaries (not entities)
- [ ] **FluentValidation** for all Commands
- [ ] **No MediatR** - only Wolverine services

### Compliance & Security
- [ ] **Audit logging** - All CRUD operations logged via interceptor
- [ ] **Tenant isolation** - Every query has `WHERE TenantId == ...`
- [ ] **Encryption** - All PII/cost data encrypted (Email, Phone, Cost, Supplier)
- [ ] **Soft deletes** - IsDeleted flag, never hard delete
- [ ] **Validation** - Server-side validation on all inputs
- [ ] **Error handling** - No sensitive data in error messages

### Testing
- [ ] **Unit tests** written (xUnit)
- [ ] **Integration tests** for API endpoints
- [ ] **Mocked dependencies** (Moq)
- [ ] **Tenant isolation tested** (cross-tenant access must fail)
- [ ] **Test coverage >= 80%**
- [ ] **No TODO comments** in tests

### Database
- [ ] **EF Core migrations** created
- [ ] **Encryption configured** via Value Converters
- [ ] **Foreign keys** with cascade delete policies
- [ ] **Indexes** on frequently queried columns
- [ ] **Query optimization** - no N+1 queries

---

## 🔧 Required Patterns (Copy & Adapt)

### Handler with Compliance (Template)

```csharp
public class ProductService
{
    private readonly IProductRepository _repo;
    private readonly IValidator<CreateProductCommand> _validator;
    private readonly IAuditService _audit;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<ProductService> _logger;
    
    // Wolverine auto-discovers this method as HTTP endpoint
    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate input
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return BadRequest(validation.Errors);
            
            // 2. Verify tenant access
            var product = await _repo.GetBySkuAsync(_tenantContext.TenantId, request.Sku);
            if (product != null)
                return Conflict("Product with this SKU already exists");
            
            // 3. Business logic
            var newProduct = new Product(
                _tenantContext.TenantId,
                request.Sku,
                request.Name,
                request.Price
            );
            
            // 4. Persist (triggers audit logging via interceptor)
            await _repo.AddAsync(newProduct, cancellationToken);
            
            _logger.LogInformation("Product created: {Sku} for tenant {TenantId}",
                request.Sku, _tenantContext.TenantId);
            
            return Ok(new CreateProductResponse
            {
                Success = true,
                Id = newProduct.Id,
                Message = "Product created successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return InternalServerError("An error occurred");
        }
    }
}
```

### Entity with Audit Trail & Encryption (Template)

```csharp
public class Product : AggregateRoot
{
    public Guid TenantId { get; set; }              // Tenant isolation
    public string Sku { get; set; }
    public string Name { get; set; }
    public decimal PublicPrice { get; set; }
    
    // Encrypted (sensitive data)
    public string CostPriceEncrypted { get; set; }  // Internal cost
    public string SupplierNameEncrypted { get; set; } // Supplier
    
    // Audit trail
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime ModifiedAt { get; set; }
    public Guid? ModifiedBy { get; set; }
    
    // Soft delete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}
```

### Validator (Template)

```csharp
public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.Sku)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[A-Z0-9\-]+$", "SKU must be alphanumeric");
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .LessThanOrEqualTo(999999.99m);
    }
}
```

---

## 📊 Feature Development Workflow

1. **Define Entity** (Core layer)
   ```csharp
   public class Product : AggregateRoot { ... }
   ```

2. **Create Repository Interface** (Core layer)
   ```csharp
   public interface IProductRepository
   {
       Task<Product?> GetBySkuAsync(Guid tenantId, string sku);
       Task AddAsync(Product product);
   }
   ```

3. **Create DTOs** (Application layer)
   ```csharp
   public class CreateProductCommand { ... }
   public class CreateProductResponse { ... }
   ```

4. **Create Validator** (Application layer)
   ```csharp
   public class CreateProductValidator : AbstractValidator<CreateProductCommand> { ... }
   ```

5. **Create Handler Service** (Application layer)
   ```csharp
   public class ProductService
   {
       public async Task<CreateProductResponse> CreateProduct(...) { ... }
   }
   ```

6. **Create Repository Implementation** (Infrastructure layer)
   ```csharp
   public class ProductRepository : IProductRepository { ... }
   ```

7. **Configure DbContext** (Infrastructure layer)
   ```csharp
   modelBuilder.Entity<Product>()
       .HasKey(x => x.Id)
       .Property(x => x.CostPriceEncrypted)
       .HasConversion(v => _encryptionService.Encrypt(v), ...);
   ```

8. **Register in DI** (Program.cs)
   ```csharp
   builder.Services.AddScoped<IProductRepository, ProductRepository>();
   builder.Services.AddScoped<ProductService>();
   ```

9. **Write Tests** (Test project)
   ```csharp
   [Fact]
   public async Task CreateProduct_ValidInput_ReturnSuccess() { ... }
   ```

---

## 🚫 Strict Rules (Violations = Rejection)

| Area | ✅ Correct | ❌ Wrong |
|------|-----------|---------|
| **Pattern** | Wolverine Service | MediatR Handler |
| **Query** | `.Where(x => x.TenantId == tenantId)` | No where clause |
| **Encryption** | `CostPriceEncrypted` + Value Converter | Plain `CostPrice` |
| **Validation** | `FluentValidation` on Command | No validation |
| **Delete** | `IsDeleted = true` (soft delete) | Hard delete |
| **Logging** | Via EF Core Interceptor | Manual logging |
| **Async** | `async Task<T>` with `await` | `sync` or `Task.Result` |
| **Exceptions** | Log + generic error response | Expose stack trace |
| **Tests** | Cross-tenant access blocked | Only happy path |

---

## 🔑 Key Files You'll Modify

### For New Features
- `backend/Domain/[Service]/src/Core/Entities/` - Add entity
- `backend/Domain/[Service]/src/Core/Interfaces/` - Add repository interface
- `backend/Domain/[Service]/src/Application/DTOs/` - Add DTOs
- `backend/Domain/[Service]/src/Application/Handlers/` - Add Wolverine service
- `backend/Domain/[Service]/src/Application/Validators/` - Add validators
- `backend/Domain/[Service]/src/Infrastructure/Data/` - Update DbContext
- `backend/Domain/[Service]/src/Infrastructure/Repositories/` - Add implementation
- `backend/Domain/[Service]/tests/` - Add unit + integration tests

### Configuration
- `backend/Domain/[Service]/src/Program.cs` - DI registration
- `backend/Domain/[Service]/src/Infrastructure/ServiceCollectionExtensions.cs` - Service setup

---

## 📚 Reference Implementations (Copy These Patterns!)

**Working Wolverine Examples:**
- [backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs](../../backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs) - Complete HTTP handler example
- [backend/Domain/Identity/src/Handlers/Events/UserEventHandlers.cs](../../backend/Domain/Identity/src/Handlers/Events/UserEventHandlers.cs) - Event handler patterns

These are the ONLY patterns to follow. Don't innovate - copy these!

---

## 🧪 Testing Template

```csharp
public class ProductServiceTests
{
    private readonly IProductRepository _repo = new Mock<IProductRepository>().Object;
    private readonly IValidator<CreateProductCommand> _validator = new CreateProductValidator();
    private readonly ProductService _service;
    
    public ProductServiceTests()
    {
        _service = new ProductService(_repo, _validator, ...);
    }
    
    [Fact]
    public async Task CreateProduct_ValidInput_ReturnsSuccess()
    {
        // Arrange
        var cmd = new CreateProductCommand { Sku = "TEST-001", ... };
        
        // Act
        var result = await _service.CreateProduct(cmd, CancellationToken.None);
        
        // Assert
        Assert.True(result.Success);
        Assert.NotEqual(Guid.Empty, result.Id);
    }
    
    [Fact]
    public async Task CreateProduct_WithoutTenantId_ReturnsForbidden()
    {
        // Arrange
        var cmd = new CreateProductCommand { TenantId = Guid.Empty, ... };
        
        // Act
        var result = await _service.CreateProduct(cmd, CancellationToken.None);
        
        // Assert
        Assert.False(result.Success);
    }
    
    [Fact]
    public async Task CreateProduct_TenantIsolation_DifferentTenantCannotAccess()
    {
        // Verify cross-tenant access is blocked
    }
}
```

---

## 🎯 Features You'll Build

### P0.6: E-Commerce (B2B & B2C)
- VAT calculation per country
- VIES VAT-ID validation (B2B)
- Reverse charge logic (B2B)
- 14-day withdrawal management (VVVG §357)
- Invoice generation & archival
- Refund processing (VVVG §359)

### P0.9: E-Rechnung
- ZUGFeRD 3.0 XML generation
- Hybrid PDF creation (embedded XML)
- UBL 2.3 alternative format
- Digital signature (XAdES)
- ERP webhook API

---

## 📞 When You Get Stuck

- **Wolverine pattern?** → Check CheckRegistrationTypeService.cs
- **Entity design?** → Check copilot-instructions.md §Entity Design
- **Test structure?** → Check TESTING_FRAMEWORK_GUIDE.md
- **E-Commerce logic?** → Check EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md §P0.6
- **E-Rechnung?** → Check EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md §P0.9

---

**Ready? Start with:** [docs/by-role/BACKEND_DEVELOPER.md](../../docs/by-role/BACKEND_DEVELOPER.md)
