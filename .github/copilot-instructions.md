# AI Coding Agent Instructions for B2Connect

**Last Updated**: 27. Dezember 2025 | **Architecture**: DDD Microservices with Aspire

## 🏗️ Architecture Foundation

B2Connect is a **Domain-Driven Design (DDD) multitenant SaaS platform** with three architectural layers:

### Bounded Contexts
```
backend/BoundedContexts/
├── Store/                 # Public storefront (read-only, cacheable)
│   ├── API/              # Store Gateway (Port 8000)
│   ├── Catalog/          # Products, Categories
│   ├── CMS/              # Content Management
│   ├── Theming/          # UI Themes & Layouts
│   ├── Localization/     # i18n Translations
│   └── Search/           # Elasticsearch integration
├── Admin/                # Admin operations (full CRUD)
│   └── API/              # Admin Gateway (Port 8080)
└── Shared/               # Cross-context services
    ├── Identity/         # Authentication (JWT)
    └── Tenancy/          # Multi-tenant isolation
```

### Onion Architecture (Each Service)
Every service follows: **Core (Domain) → Application (CQRS) → Infrastructure (Data) → Presentation (API)**
- **Core**: Entities, ValueObjects, Aggregates, Repositories (interfaces)
- **Application**: DTOs, CQRS Handlers, Validators (FluentValidation), Mappers
- **Infrastructure**: EF Core DbContext, Repositories (implementations), External services
- **Presentation**: Controllers/Endpoints, Middleware, Dependency Injection

**Critical Rule**: Dependencies point **inward only**. Core has **zero** framework dependencies.

## 🔧 Developer Workflows

### Building & Running
```bash
# Backend with Aspire (recommended - one command starts everything)
cd backend/Orchestration
dotnet run                    # Starts: Auth, Tenant, Catalog, CMS, Localization, Search, Gateways

# Manual service startup (if needed)
dotnet run --project backend/BoundedContexts/Store/API/B2Connect.Store.csproj
dotnet run --project backend/BoundedContexts/Admin/API/B2Connect.Admin.csproj

# Frontend
cd frontend-store  # or frontend-admin
npm install && npm run dev   # Port 5173 (store) or 5174 (admin)
```

### Testing
```bash
# Run all backend tests
dotnet test backend/B2Connect.slnx -v minimal

# Run specific bounded context tests
dotnet test backend/BoundedContexts/Store/Catalog/tests/B2Connect.Catalog.Tests.csproj

# Frontend tests (Vitest)
cd frontend-store && npm run test

# E2E tests (Playwright)
cd frontend-admin && npm run test:e2e
```

### Key Tasks (VS Code)
- **build-backend**: `dotnet build B2Connect.slnx`
- **test-backend**: `dotnet test B2Connect.slnx -v minimal`
- **backend-start**: Aspire orchestration
- **dev-frontend**: Hot reload development server

## 📋 Project-Specific Patterns

### CQRS with Handlers (Application Layer)
Each feature lives in a handler folder with Command/Query + Handler + Validator:
```csharp
// backend/BoundedContexts/Store/Catalog/src/Application/Products/CreateProduct/
public record CreateProductCommand(string Sku, string Name, decimal Price) : IRequest<ProductDto>;
public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = Product.Create(request.Sku, request.Name, request.Price);
        await _repository.AddAsync(product, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<ProductDto>(product);
    }
}
```

### Multi-Tenancy & Context Propagation
Tenant ID flows via `X-Tenant-ID` header through all services. **Always include in queries**:
```csharp
var product = await _repository.GetBySkuAsync(tenantId, sku);
// Core.Entities.Product must accept tenantId in constructor
```

### Validation Pattern
Use FluentValidation in separate classes:
```csharp
public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
```

### Repository Interface (Core Layer)
```csharp
namespace B2Connect.Catalog.Core.Interfaces;
public interface IProductRepository
{
    Task<Product?> GetBySkuAsync(Guid tenantId, string sku, CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
}
// Implementation in Infrastructure layer only
```

### Entity with Domain Events
```csharp
public class Product : AggregateRoot
{
    public Product(Guid tenantId, string sku, string name, decimal price)
    {
        TenantId = tenantId;
        Sku = sku;
        Name = name;
        Price = price;
        RaiseEvent(new ProductCreatedEvent(Id, tenantId, sku));
    }
}
```

## 🔀 Inter-Service Communication

**Asynchronous**: Use Wolverine messaging (event bus) for eventual consistency
```csharp
// Publishing domain events
await _messageBus.PublishAsync(new ProductCreatedEvent(productId, tenantId, sku));

// Subscribing to events in Infrastructure
[WolverineHandler]
public async Task Handle(ProductCreatedEvent @event)
{
    // Update search index, cache, or trigger other services
}
```

**Synchronous**: Only Store → Admin gateway allowed via HTTP. Never direct service-to-service calls.

## 🗄️ Database & Persistence

- **ORM**: Entity Framework Core with async/await
- **Database Per Service**: Each bounded context owns its PostgreSQL 16 database
- **Migrations**: `dotnet ef migrations add <MigrationName>`
- **DbContext Location**: `Infrastructure/Data/[ServiceName]DbContext.cs`
- **Naming Convention**: EFCore.NamingConventions (snake_case in database)

### Common DbContext Pattern
```csharp
public class CatalogDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
```

## 🌍 Frontend Architecture

**Technology**: Vue.js 3 + TypeScript + Vite  
**Two separate apps**:
- **frontend-store** (Port 5173): Public storefront
- **frontend-admin** (Port 5174): Admin panel

### Vue 3 Composition API Pattern
```vue
<script setup lang="ts">
import { ref, computed } from 'vue'
import { useProductStore } from '@/stores/productStore'

const products = ref<Product[]>([])
const selectedTenant = ref<string>('')

const filteredProducts = computed(() => 
  products.value.filter(p => p.tenantId === selectedTenant.value)
)

const loadProducts = async () => {
  products.value = await fetchProducts(selectedTenant.value)
}
</script>
```

### API Client Pattern
Centralize all HTTP calls in service layer, never in components:
```typescript
// src/services/catalogService.ts
export const catalogService = {
  async getProducts(tenantId: string): Promise<Product[]> {
    return api.get(`/api/store/catalog/products`, {
      headers: { 'X-Tenant-ID': tenantId }
    })
  }
}
```

## 🎯 Critical Conventions

### Naming
- **C# Classes/Methods**: PascalCase (`CreateProductHandler`, `GetBySkuAsync`)
- **C# Fields**: camelCase with underscore (`_repository`, `_logger`)
- **Vue Components**: PascalCase (`ProductCard.vue`, `TenantSelector.vue`)
- **Files match class names** (`Product.cs`, `ProductRepository.cs`)
- **Interfaces**: Prefix `I` (`IProductRepository`, `ITenantService`)
- **Extensions**: Suffix `Extensions` (`StringExtensions.cs`)

### Project Structure Rules
1. **One public class per file**
2. **Service names include context**: `B2Connect.Catalog.Application.csproj` not just `Application.csproj`
3. **Test projects mirror source**: `src/` → `tests/` structure identical
4. **Shared code in `shared/`**: Database utilities, extensions, middleware only

### Dependency Injection (ASP.NET Core)
```csharp
// Program.cs - always use extension methods
services
    .AddCatalogCore()
    .AddCatalogApplication()
    .AddCatalogInfrastructure(configuration);

// Extension method pattern (ServiceCollectionExtensions.cs)
public static IServiceCollection AddCatalogCore(this IServiceCollection services) =>
    services.AddScoped<IProductRepository, ProductRepository>();
```

## 🧪 Testing Essentials

### Test File Organization
- **Unit tests**: Mirror source structure, `*Tests.cs` suffix
- **Test fixtures**: Reusable setup in `Fixtures/` folder
- **Integration tests**: Use TestContainers for PostgreSQL/Redis

### Test Pattern
```csharp
public class CreateProductHandlerTests : IAsyncLifetime
{
    private readonly CreateProductHandler _handler;
    private readonly Mock<IProductRepository> _mockRepo;
    
    public CreateProductHandlerTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _handler = new CreateProductHandler(_mockRepo.Object);
    }
    
    [Fact]
    public async Task Handle_ValidCommand_CreatesProduct()
    {
        // Arrange
        var command = new CreateProductCommand("SKU001", "Product", 99.99m);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        _mockRepo.Verify(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()));
    }
}
```

### Tools
- **Unit/Integration**: xUnit, Moq, FluentAssertions, TestContainers
- **Components**: Vue Test Utils (frontend-store)
- **E2E**: Playwright (frontend-admin)
- **Run**: `dotnet test` or `npm run test`

## 🔐 Security & Tenancy

### Tenant Context
- Always extract `X-Tenant-ID` header in middleware
- Pass Guid tenantId through all domain queries
- **Never** query without tenant filter (data isolation critical)

### Authentication
- **JWT tokens** stored in appsettings (local dev)
- **Azure Key Vault** for production secrets
- Identity service handles token generation/refresh

## 📦 External Dependencies to Know

**Backend**:
- **Wolverine**: Async messaging & CQRS
- **FluentValidation**: Input validation
- **EF Core**: Data access (with NamingConventions)
- **AutoMapper**: DTO ↔ Entity mapping
- **Elasticsearch**: Full-text search (Search service)

**Frontend**:
- **Pinia**: State management
- **Axios**: HTTP client
- **Vue Router**: Client routing
- **Tailwind CSS**: Styling

## 📝 Code Review Checklist

Before committing:
- ✅ Onion architecture respected (Core has zero external deps)
- ✅ Validator created for each Command
- ✅ Tenant ID included in all queries
- ✅ Repository interface in Core, implementation in Infrastructure
- ✅ Tests written (aim for 80%+)
- ✅ No synchronous service-to-service calls
- ✅ Nullable reference types enabled (`#nullable enable`)
- ✅ Async/await used consistently (no `.Result` or `.Wait()`)

## � Security Checklist for Feature Implementation

When implementing ANY feature, verify these critical security requirements (P0 priorities from [APPLICATION_SPECIFICATIONS.md](../docs/APPLICATION_SPECIFICATIONS.md)):

### P0.1 - JWT & Secrets Management
- [ ] **No hardcoded secrets** in code, config files, or version control
- [ ] Secrets use **Azure Key Vault** (production) or `appsettings.Development.json` (local only)
- [ ] JWT secret minimum **32 characters** (use `openssl rand -base64 32`)
- [ ] Token expiration: **1 hour access token**, **7 days refresh token**
- [ ] Implement token refresh flow in handlers
- [ ] Use HS256 (symmetric) or RS256 (asymmetric) only
- **Code Pattern**:
```csharp
// CORRECT: Use configuration
var jwtSecret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("Missing JWT secret");

// WRONG: Never hardcode
var jwtSecret = "my-secret-123"; // ❌ SECURITY RISK

// Token generation
var token = new JwtSecurityToken(
    issuer: configuration["Jwt:Issuer"],
    audience: configuration["Jwt:Audience"],
    claims: claims,
    expires: DateTime.UtcNow.AddHours(1),
    signingCredentials: new SigningCredentials(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        SecurityAlgorithms.HmacSha256
    )
);
```

### P0.2 - CORS & HTTPS
- [ ] **No hardcoded localhost/domains** in production
- [ ] CORS origins configured per environment (dev, staging, prod)
- [ ] HTTPS enforced (no HTTP in production)
- [ ] HSTS header with `max-age: 31536000` (1 year minimum)
- [ ] Rate limiting: **1000 req/min per IP**, **100 req/min per user**
- **Code Pattern**:
```csharp
// Program.cs
var allowedOrigins = configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// HSTS
app.UseHsts(); // Only in production
app.UseHttpsRedirection();
```

### P0.3 - Encryption at Rest & in Transit
- [ ] **PII fields encrypted**: Email, Phone, Address, SSN, DOB, FirstName, LastName
- [ ] Use **AES-256** encryption for sensitive data
- [ ] **Never store credit cards** - use tokenization only
- [ ] **Field-level encryption** in database (not table-level)
- [ ] TLS 1.2+ for all HTTPS connections
- [ ] IV (Initialization Vector) randomized for each encryption
- **Code Pattern**:
```csharp
// Core/Entities/User.cs
public class User : AggregateRoot
{
    // Sensitive fields encrypted
    private string _encryptedEmail;
    public string Email
    {
        get => _encryptionService.Decrypt(_encryptedEmail);
        set => _encryptedEmail = _encryptionService.Encrypt(value);
    }
}

// Infrastructure/Services/EncryptionService.cs
public class EncryptionService : IEncryptionService
{
    public string Encrypt(string plainText)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(_encryptionKey);
            aes.GenerateIV(); // Random IV each time
            
            using (var encryptor = aes.CreateEncryptor())
            using (var ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length);
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (var sw = new StreamWriter(cs))
                        sw.Write(plainText);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}
```

### P0.4 - Audit Logging (Immutable)
- [ ] **All data modifications logged** (CREATE, UPDATE, DELETE)
- [ ] Capture: **Timestamp, User ID, Action, Before/After values**
- [ ] Use **soft deletes** (logical deletion, not hard delete)
- [ ] Audit logs **immutable** (no update/delete of audit records)
- [ ] Include **tenant ID** in all audit entries
- **Code Pattern**:
```csharp
// Core/Events/AuditLogEntry.cs
public class AuditLogEntry
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid TenantId { get; init; }
    public Guid UserId { get; init; }
    public string EntityType { get; init; }
    public string Action { get; init; } // CREATE, UPDATE, DELETE
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public string? BeforeValues { get; init; } // JSON
    public string? AfterValues { get; init; }  // JSON
}

// Infrastructure/Services/AuditService.cs
public async Task LogAsync(Guid tenantId, Guid userId, string entity, 
    string action, object? before = null, object? after = null)
{
    var entry = new AuditLogEntry
    {
        TenantId = tenantId,
        UserId = userId,
        EntityType = entity,
        Action = action,
        BeforeValues = before != null ? JsonSerializer.Serialize(before) : null,
        AfterValues = after != null ? JsonSerializer.Serialize(after) : null
    };
    
    await _context.AuditLogs.AddAsync(entry);
    // Never delete audit logs - only add
}
```

### Additional Security Patterns

**Input Validation**
- [ ] All inputs validated **server-side** (never trust client)
- [ ] Use **FluentValidation** for all Commands
- [ ] Whitelist approach (allow known-good, block everything else)
- [ ] Length limits enforced (SQL injection prevention)
```csharp
public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Sku)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[A-Z0-9\-]+$", "SKU must be alphanumeric");
        
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .LessThanOrEqualTo(999999.99m);
    }
}
```

**Tenant Isolation**
- [ ] **Every query includes tenant ID filter** (no cross-tenant leaks)
- [ ] `X-Tenant-ID` header extracted in middleware and validated
- [ ] Tenant ID passed through entire call stack
```csharp
public async Task<Product?> GetBySkuAsync(Guid tenantId, string sku)
{
    // CORRECT: Filter by tenantId
    return await _context.Products
        .Where(p => p.TenantId == tenantId && p.Sku == sku)
        .FirstOrDefaultAsync();
    
    // WRONG: No tenant filter = security breach
    // return await _context.Products.FirstOrDefaultAsync(p => p.Sku == sku);
}
```

**No Sensitive Data in Logs**
- [ ] Never log: Passwords, tokens, credit cards, SSN, email (if PII sensitive)
- [ ] Use structured logging with redaction filters
- [ ] Review log output before commit
```csharp
// WRONG
_logger.LogInformation($"User logged in: {user.Email}"); // ❌ PII in logs

// CORRECT
_logger.LogInformation("User logged in with ID {UserId}", user.Id);
```

---

## 🚀 Getting Unblocked

1. **Architecture questions**: Read [docs/architecture/DDD_BOUNDED_CONTEXTS.md](../docs/architecture/DDD_BOUNDED_CONTEXTS.md)
2. **Security specs**: See [docs/APPLICATION_SPECIFICATIONS.md](../docs/APPLICATION_SPECIFICATIONS.md) sections 3-4
3. **Service boundaries**: Check [COMPREHENSIVE_REVIEW.md](../COMPREHENSIVE_REVIEW.md) "Service Architecture"
4. **Code examples**: Search `backend/BoundedContexts/Store/Catalog/` for working patterns
5. **Testing setup**: See [TESTING_STRATEGY.md](../TESTING_STRATEGY.md)
6. **Aspire orchestration**: See [ASPIRE_QUICK_START.md](../ASPIRE_QUICK_START.md)

---

**Questions or unclear sections?** Ask for clarification on specific areas.
