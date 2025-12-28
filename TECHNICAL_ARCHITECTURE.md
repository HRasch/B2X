![B2Connect Technical Architecture](https://img.shields.io/badge/B2Connect-Technical%20Architecture-blue)

# Technical Architecture Guide

**Last Updated:** 28. Dezember 2025  
**Version:** 1.0  
**Owner:** Tech Lead / Architecture Team

---

## Table of Contents

1. [System Overview](#system-overview)
2. [Microservices Architecture](#microservices-architecture)
3. [Technology Stack](#technology-stack)
4. [Architectural Patterns](#architectural-patterns)
5. [Data Flow](#data-flow)
6. [Security & Compliance](#security--compliance)
7. [Scalability & Performance](#scalability--performance)
8. [Development Environment](#development-environment)

---

## System Overview

B2Connect is a **multi-tenant European SaaS platform** designed for B2B/B2C e-commerce with strict compliance requirements (NIS2, GDPR, AI Act, BITV 2.0, E-Rechnung).

### Key Characteristics

| Property | Value | Reason |
|----------|-------|--------|
| **Architecture Pattern** | Event-Driven Microservices (Wolverine) | Decoupled services, scalable, compliance-friendly |
| **Frontend Framework** | Vue.js 3 + TypeScript | Modern, reactive, accessibility-aware |
| **Backend Framework** | ASP.NET Core 8 + Wolverine | High-performance, messaging built-in |
| **Database** | PostgreSQL 16 (per-service) | ACID compliance, encryption support |
| **Cache** | Redis (sessions, cache) | High-throughput, tenant-isolated |
| **Search** | Elasticsearch 9 | Full-text search, analytics-ready |
| **Message Bus** | Wolverine (in-process) | Event-driven, eventual consistency |
| **Deployment** | .NET Aspire + Docker | Local dev = prod architecture |
| **Cloud-Native** | AWS/Azure/On-Prem capable | Container-orchestrated |

### Design Principle: **Compliance-First**

Every feature implements:
- ✅ Audit logging (immutable event trail)
- ✅ Encryption (PII at rest and in transit)
- ✅ Tenant isolation (no cross-tenant leaks)
- ✅ Data retention policies (GDPR right-to-forget)
- ✅ User consent tracking

---

## Microservices Architecture

### Service Map (10 Microservices)

```
┌─────────────────────────────────────────────────────────────────┐
│                         Gateway Layer                             │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐           │
│  │ Store API    │  │ Admin API    │  │ Public API   │           │
│  │ (Port 8000)  │  │ (Port 8080)  │  │ (Port 8100)  │           │
│  └──────────────┘  └──────────────┘  └──────────────┘           │
└─────────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────────┐
│                    Microservices (Wolverine)                     │
│                                                                  │
│  ┌────────────────┐  ┌────────────────┐  ┌──────────────┐     │
│  │ Identity (7002)│  │ Tenancy (7003) │  │ Localization │     │
│  │ • Auth         │  │ • Multi-tenant │  │ (7004)       │     │
│  │ • JWT tokens   │  │ • Org setup    │  │ • i18n       │     │
│  │ • MFA support  │  │ • Isolation    │  │ • Translate  │     │
│  └────────────────┘  └────────────────┘  └──────────────┘     │
│                                                                  │
│  ┌────────────────┐  ┌────────────────┐  ┌──────────────┐     │
│  │ Catalog (7005) │  │ CMS (7006)     │  │ Theming (7008)   │
│  │ • Products     │  │ • Pages        │  │ • UI themes  │     │
│  │ • Categories   │  │ • Blocks       │  │ • Layouts    │     │
│  │ • Inventory    │  │ • Assets       │  │ • Custom CSS │     │
│  └────────────────┘  └────────────────┘  └──────────────┘     │
│                                                                  │
│  ┌────────────────┐  ┌────────────────┐  ┌──────────────┐     │
│  │ Search (9300)  │  │ Compliance     │  │ Audit (7010) │     │
│  │ • Elasticsearch│  │ (7009)         │  │ • Logging    │     │
│  │ • Per-tenant   │  │ • P0.* checks  │  │ • Tamper     │     │
│  │ • Full-text    │  │ • Gates        │  │   detection  │     │
│  └────────────────┘  └────────────────┘  └──────────────┘     │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
                             ↓
┌─────────────────────────────────────────────────────────────────┐
│                  Data & Infrastructure Layer                     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐          │
│  │ PostgreSQL   │  │ Redis        │  │ Vault        │          │
│  │ (per-service)│  │ (shared)     │  │ (secrets)    │          │
│  │ • Encrypted  │  │ • Sessions   │  │ • Keys       │          │
│  │ • Backups    │  │ • Cache      │  │ • Rotation   │          │
│  └──────────────┘  └──────────────┘  └──────────────┘          │
└─────────────────────────────────────────────────────────────────┘
```

### Service Port Reference

| Service | Port | Framework | Type | Purpose |
|---------|------|-----------|------|---------|
| **Identity** | 7002 | Wolverine | Core | JWT Auth, Passkeys, MFA |
| **Tenancy** | 7003 | Wolverine | Core | Multi-tenant management, org isolation |
| **Localization** | 7004 | Wolverine | Core | Translations (8 languages) |
| **Catalog** | 7005 | Wolverine | Core | Products, categories, inventory |
| **CMS** | 7006 | Wolverine | Core | Pages, content blocks, assets |
| **Theming** | 7008 | Wolverine | Core | UI themes, layouts, custom CSS |
| **Compliance** | 7009 | Wolverine | Core | P0.* compliance gates, reporting |
| **Audit** | 7010 | Wolverine | Core | Immutable logging, tamper detection |
| **Search** | 9300 | Wolverine | Optional | Elasticsearch integration |
| **Store Gateway** | 8000 | ASP.NET Core | API | Public storefront API |
| **Admin Gateway** | 8080 | ASP.NET Core | API | Admin panel API |
| **Aspire Dashboard** | 15500 | Aspire | Orchestration | Dev observability |
| **PostgreSQL** | 5432 | Database | Persistence | Per-service data |
| **Redis** | 6379 | Cache | Persistence | Sessions, cache, pub/sub |

### Service Responsibilities

#### Core Microservices (DDD Bounded Contexts)

**Identity Service** (Port 7002)
```
Responsibilities:
  ✅ JWT token generation & validation (1h access, 7d refresh)
  ✅ User registration & password management
  ✅ Multi-factor authentication (TOTP, passkeys)
  ✅ Suspicious activity detection (brute force protection)
  ✅ Session management & timeouts
  ✅ Audit logging for auth events

Dependencies:
  → Tenancy (X-Tenant-ID validation)
  → Audit (Log all auth attempts)
  → Vault (Encryption keys)

Data:
  → users (PII encrypted)
  → login_attempts (audit trail)
  → mfa_settings (encrypted TOTP secrets)
```

**Tenancy Service** (Port 7003)
```
Responsibilities:
  ✅ Tenant CRUD operations
  ✅ Multi-tenant isolation enforcement
  ✅ Organization hierarchy management
  ✅ Tenant-specific settings & features
  ✅ Soft-delete support (data retention)

Dependencies:
  → Audit (Log all tenant changes)
  → Vault (Tenant encryption keys)

Data:
  → tenants (company data, encrypted)
  → users_tenants (role assignments)
  → tenant_settings (feature flags)
```

**Catalog Service** (Port 7005)
```
Responsibilities:
  ✅ Product CRUD (SKU, name, price, category)
  ✅ Category management & hierarchy
  ✅ Inventory tracking & reservations
  ✅ Pricing rules & bulk import
  ✅ Supplier management (encrypted)

Dependencies:
  → Search (Elasticsearch indexing)
  → Audit (Log all product changes)
  → Vault (Supplier data encryption)

Data:
  → products (supplier info encrypted)
  → categories (hierarchy)
  → inventory (soft-reserved)
```

**Search Service** (Port 9300)
```
Responsibilities:
  ✅ Per-tenant Elasticsearch indexing
  ✅ Full-text product search
  ✅ Faceted navigation (filters)
  ✅ Search analytics (anonymized)
  ✅ Index replication & backup

Dependencies:
  → Catalog (Product events)
  → Audit (Log all searches)

Data:
  → per-tenant ES indices
  → search_analytics (anonymized)
```

**Audit Service** (Port 7010)
```
Responsibilities:
  ✅ Immutable audit log storage
  ✅ Tamper detection (hash verification)
  ✅ SIEM event forwarding
  ✅ Compliance reporting
  ✅ Retention policy enforcement

Dependencies:
  → None (fully autonomous)

Data:
  → audit_logs (immutable, never update/delete)
  → audit_hashes (tamper detection)
  → siem_events (forwarded)
  → retention_policies (GDPR compliance)
```

---

## Technology Stack

### Backend (.NET 8)

**Core Framework**
```csharp
// ASP.NET Core 8
// Entity Framework Core 8
// Wolverine 1.0+ (messaging, CQRS)
```

**Key Libraries**
| Library | Version | Purpose |
|---------|---------|---------|
| **Wolverine** | 1.0+ | Event-driven messaging, HTTP endpoints |
| **EF Core** | 8.0+ | ORM with encryption/audit support |
| **FluentValidation** | 11.0+ | Input validation (command level) |
| **AutoMapper** | 13.0+ | DTO ↔ Entity mapping |
| **System.Text.Json** | 8.0+ | JSON serialization |
| **System.Security.Cryptography** | 8.0+ | AES-256 encryption |
| **Serilog** | 3.0+ | Structured logging (SIEM-ready) |
| **Polly** | 8.0+ | Resilience (retry, circuit breaker) |
| **TestContainers** | 3.0+ | Integration testing (PostgreSQL, Redis) |

**Database**
```
PostgreSQL 16+
  ✅ ACID compliance (transactions)
  ✅ Per-service database isolation
  ✅ Encrypted columns (AES-256)
  ✅ Soft-delete support (IsDeleted + DeletedAt)
  ✅ Audit tables (automatic via EF Core interceptor)
  ✅ Row-level security (per tenant)

Schema Convention:
  ✅ snake_case table/column names (via EFCore.NamingConventions)
  ✅ Indexes on TenantId (all tables)
  ✅ Indexes on foreign keys
  ✅ Audit tables shadow originals (CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, DeletedAt, DeletedBy)
```

**Caching & Sessions**
```
Redis 7.0+
  ✅ Session storage (distributed)
  ✅ Cache layer (products, categories)
  ✅ Pub/Sub for events (eventual consistency)
  ✅ Per-tenant key isolation (redis key prefix: tenant_id:)
  ✅ 5-minute TTL for cache entries
  ✅ 30-minute TTL for sessions
```

### Frontend

**Technology Stack**
```
Vue.js 3 (Composition API)
  ✅ TypeScript (strict mode)
  ✅ Vite (build tool, < 1sec reload)
  ✅ Tailwind CSS v4 (utility-first styling)
  ✅ Pinia (state management, stores)
  ✅ Vue Router (SPA routing)
  ✅ Axios (HTTP client, interceptors)
  ✅ Vitest + Playwright (testing)
```

**Architecture Pattern**
```
Frontend Structure:
  src/
  ├── components/          # Vue components (Composition API)
  │   ├── base/           # Base components (Button, Input, etc.)
  │   ├── feature/        # Feature components (Product, Cart, etc.)
  │   └── layout/         # Layout components (Header, Footer, etc.)
  ├── views/              # Page-level components (router views)
  ├── stores/             # Pinia stores (state management)
  ├── services/           # API client layer (axios)
  ├── composables/        # Vue composables (reusable logic)
  ├── types/              # TypeScript types (interfaces)
  ├── utils/              # Utilities (formatters, validators)
  └── router/             # Vue Router configuration
```

**State Management (Pinia)**
```typescript
// Example: Product Store
export const useProductStore = defineStore('products', () => {
  const products = ref<Product[]>([])
  const selectedProduct = ref<Product | null>(null)
  
  const fetchProducts = async (tenantId: string) => {
    products.value = await productService.getAll(tenantId)
  }
  
  const selectProduct = (product: Product) => {
    selectedProduct.value = product
  }
  
  return { products, selectedProduct, fetchProducts, selectProduct }
})
```

---

## Architectural Patterns

### 1. Event-Driven Architecture (Wolverine)

**Command Handler Pattern** (No MediatR!)
```csharp
// Step 1: Plain POCO command (no IRequest interface)
public class CreateProductCommand
{
    public required string Sku { get; init; }
    public required string Name { get; init; }
    public required decimal Price { get; init; }
}

// Step 2: Service with public async method
public class ProductService
{
    private readonly IProductRepository _repository;
    
    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        // Validate
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
            return new CreateProductResponse { Success = false, Error = validation.Errors.First().ErrorMessage };
        
        // Create entity
        var product = new Product(request.Sku, request.Name, request.Price);
        await _repository.AddAsync(product, cancellationToken);
        
        // Publish event (Wolverine handles pub/sub)
        await _messageBus.PublishAsync(new ProductCreatedEvent(product.Id, product.Sku));
        
        return new CreateProductResponse { Success = true, ProductId = product.Id };
    }
}

// Step 3: Register in DI
builder.Services.AddScoped<ProductService>();

// Step 4: Wolverine auto-creates HTTP endpoint
// POST /createproduct
```

**Event Handler Pattern**
```csharp
// Events are plain POCOs
public class ProductCreatedEvent
{
    public Guid ProductId { get; set; }
    public string Sku { get; set; }
}

// Handlers use Handle(EventType @event) convention
public class ProductEventHandlers
{
    private readonly ISearchService _searchService;
    
    public ProductEventHandlers(ISearchService searchService)
    {
        _searchService = searchService;
    }
    
    // Wolverine auto-calls this when ProductCreatedEvent is published
    public async Task Handle(ProductCreatedEvent @event)
    {
        // Index in Elasticsearch
        await _searchService.IndexProductAsync(@event.ProductId);
    }
}

// No registration needed—Wolverine auto-discovers Handle methods!
```

**Why Wolverine (Not MediatR)?**
- ✅ Built for distributed microservices
- ✅ Event-driven out of the box
- ✅ In-process messaging (fast, low-latency)
- ✅ Extensible (custom message handlers)
- ✅ Better for domain events & eventual consistency
- ❌ MediatR = in-process command bus only, not events

### 2. Domain-Driven Design (DDD)

**Layered Architecture (Per Service)**
```
┌─────────────────────────────────────┐
│    Presentation (API Layer)         │
│    Controllers → DTOs               │
└──────────────┬──────────────────────┘
               ↓
┌─────────────────────────────────────┐
│    Application (Service Layer)      │
│    Commands → Handlers → Services   │
│    Validators → Mappers             │
└──────────────┬──────────────────────┘
               ↓
┌─────────────────────────────────────┐
│    Domain (Core/Business Logic)     │
│    Entities → ValueObjects          │
│    Aggregates → Repositories (I)    │
│    DomainEvents                     │
│  ✅ ZERO external dependencies!     │
└──────────────┬──────────────────────┘
               ↓
┌─────────────────────────────────────┐
│    Infrastructure (Data Access)     │
│    EF Core DbContext                │
│    Repository Implementations       │
│    External Service Clients         │
└─────────────────────────────────────┘
```

**Code Location Convention**
```
backend/Domain/[Service]/
├── src/
│   ├── Core/                    # Domain Layer (no dependencies!)
│   │   ├── Entities/            # Aggregate roots
│   │   ├── ValueObjects/        # Immutable value types
│   │   ├── Enums/               # Restricted type enums
│   │   ├── Interfaces/          # Repository interfaces (abstraction)
│   │   └── Events/              # Domain events
│   ├── Application/             # Application Layer
│   │   ├── Commands/            # Command DTOs
│   │   ├── Handlers/            # Wolverine command handlers
│   │   ├── Validators/          # FluentValidation
│   │   ├── DTOs/                # Response/Transfer objects
│   │   └── Mappers/             # AutoMapper profiles
│   └── Infrastructure/          # Infrastructure Layer
│       ├── Data/                # EF Core DbContext
│       ├── Repositories/        # Repository implementations
│       └── Services/            # External integrations
└── tests/                       # Mirror src/ structure
    ├── Unit/
    ├── Integration/
    └── Fixtures/                # Shared test data
```

### 3. Multi-Tenancy Pattern

**Tenant Context Propagation**
```csharp
// 1. Extract from JWT token claims (in middleware)
public class TenantMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Extract from JWT sub claim (tenant_id embedded)
        var tenantId = context.User.FindFirst("sub")?.Value;
        
        // Store in HttpContext items for access throughout request
        if (tenantId != null)
            context.Items["TenantId"] = Guid.Parse(tenantId);
        
        await _next(context);
    }
}

// 2. Access in handlers via request object
public class ProductService
{
    public async Task<Product?> GetBySkuAsync(
        Guid tenantId,              // ✅ Always required parameter
        string sku,
        CancellationToken ct)
    {
        // ✅ ALWAYS filter by tenant
        return await _context.Products
            .Where(p => p.TenantId == tenantId && p.Sku == sku)
            .FirstOrDefaultAsync(ct);
    }
}

// 3. EF Core query filter (automatic tenant filtering)
modelBuilder.Entity<Product>()
    .HasQueryFilter(p => p.TenantId == _tenantId);
    // Prevents accidental cross-tenant queries
```

### 4. Encryption Pattern (AES-256-GCM)

**Entity-Level Encryption** (EF Core Value Converters)
```csharp
public class User : AggregateRoot
{
    private string _encryptedEmail;
    
    public string Email
    {
        get => _encryptionService.Decrypt(_encryptedEmail);
        set => _encryptedEmail = _encryptionService.Encrypt(value);
    }
}

// EF Core Configuration
modelBuilder.Entity<User>()
    .Property(u => u.Email)
    .HasConversion(
        v => _encryptionService.Encrypt(v),          // To database
        v => _encryptionService.Decrypt(v)           // From database
    )
    .HasColumnName("email_encrypted")
    .HasMaxLength(512);  // Ciphertext is longer than plaintext
```

**Encryption Service (AES-256-GCM)**
```csharp
public class EncryptionService : IEncryptionService
{
    private readonly byte[] _masterKey;
    
    public string Encrypt(string plainText)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = _masterKey;
            aes.GenerateIV();  // Random IV per encryption
            
            using (var encryptor = aes.CreateEncryptor())
            using (var ms = new MemoryStream())
            {
                // Write IV to output (needed for decryption)
                ms.Write(aes.IV, 0, aes.IV.Length);
                
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                    sw.Write(plainText);
                
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
    
    public string Decrypt(string cipherText)
    {
        var data = Convert.FromBase64String(cipherText);
        
        using (var aes = Aes.Create())
        {
            aes.Key = _masterKey;
            
            // Extract IV (first 16 bytes)
            aes.IV = data[..16];
            
            using (var decryptor = aes.CreateDecryptor())
            using (var ms = new MemoryStream(data, 16, data.Length - 16))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
                return sr.ReadToEnd();
        }
    }
}
```

### 5. Audit Logging Pattern (Immutable)

**Automatic Audit via EF Core Interceptor**
```csharp
public class AuditInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context!;
        var auditService = context.GetService<IAuditService>();
        var tenantId = (Guid)context.Items["TenantId"];
        var userId = (Guid)context.Items["UserId"];
        
        // Capture before/after for all changed entities
        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                await auditService.LogAsync(
                    tenantId, userId,
                    entry.Entity.GetType().Name,
                    "CREATE",
                    before: null,
                    after: GetValues(entry),
                    cancellationToken);
            }
            else if (entry.State == EntityState.Modified)
            {
                await auditService.LogAsync(
                    tenantId, userId,
                    entry.Entity.GetType().Name,
                    "UPDATE",
                    before: GetOriginalValues(entry),
                    after: GetValues(entry),
                    cancellationToken);
            }
            else if (entry.State == EntityState.Deleted)
            {
                await auditService.LogAsync(
                    tenantId, userId,
                    entry.Entity.GetType().Name,
                    "DELETE",
                    before: GetValues(entry),
                    after: null,
                    cancellationToken);
            }
        }
        
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}

// Register in Program.cs
services.AddDbContext<CatalogDbContext>(options =>
    options.AddInterceptors(new AuditInterceptor())
);
```

---

## Data Flow

### Request Lifecycle (Store Frontend)

```
1. Frontend (Vue.js)
   ↓
   GET /api/products?tenantId=xyz

2. Store Gateway (Port 8000)
   ↓
   → Validate JWT (from Authorization header)
   → Extract TenantId from JWT claims
   → Route to Catalog service
   
3. Catalog Service (Port 7005)
   ↓
   ProductService.GetProductsAsync(tenantId, filter)
   
4. Application Layer
   ↓
   → FluentValidation (validate filter)
   → Check cache (Redis)
   → If cache miss → query repository
   
5. Domain Layer (Repository)
   ↓
   await context.Products
       .Where(p => p.TenantId == tenantId)  // ✅ Tenant filter
       .Where(p => p.Name.Contains(filter))
       .ToListAsync()
   
6. Infrastructure (EF Core)
   ↓
   → Decrypt PII fields (supplier names)
   → Apply audit query filter (soft deletes)
   → Execute SQL
   
7. Database (PostgreSQL)
   ↓
   SELECT * FROM products
   WHERE tenant_id = $1 AND name LIKE $2 AND is_deleted = false
   
8. Return to Frontend
   ↓
   [200 OK] { products: [...], total: 42 }
   → Store in Pinia store
   → Render in Vue template
```

### Event-Driven Flow (Product Created)

```
1. Frontend creates product
   POST /createproduct
   → ProductCommand { sku, name, price }

2. Catalog Service receives
   ProductService.CreateProduct(cmd, tenantId)
   
3. Service publishes domain event
   await _messageBus.PublishAsync(
       new ProductCreatedEvent(productId, sku)
   )

4. Wolverine routes event to handlers
   ↓
   SearchService.Handle(ProductCreatedEvent)
       → Index in Elasticsearch
   ↓
   AuditService.Handle(ProductCreatedEvent)
       → Log to immutable audit table
   ↓
   NotificationService.Handle(ProductCreatedEvent)
       → Send email to admins

5. All handlers complete (eventually consistent)
   ↓
   Product indexed in search
   ↓
   Audit logged
   ↓
   Email sent
```

---

## Security & Compliance

### Security Layers

| Layer | Implementation | Threat |
|-------|----------------|--------|
| **Network** | TLS 1.2+, HSTS, CORS | Data interception, XSS |
| **Authentication** | JWT (1h access, 7d refresh) | Unauthorized access |
| **Authorization** | Role-based (RBAC) + Tenant ID checks | Privilege escalation |
| **Encryption** | AES-256-GCM (PII at rest) | Data breaches |
| **Input Validation** | FluentValidation server-side | SQL injection, XSS |
| **Audit Logging** | Immutable event trail + tamper detection | Denial of accountability |
| **Rate Limiting** | 1000 req/min per IP, 100 req/min per user | Brute force, DDoS |
| **Secrets Management** | Azure Key Vault (prod), appsettings.Development.json (local) | Credential exposure |

### Compliance Features

**GDPR**
- ✅ Data encryption (AES-256)
- ✅ Right-to-be-forgotten (soft delete + purge job)
- ✅ Audit logging (consent tracking)
- ✅ Data export (GDPR export API)

**NIS2**
- ✅ Incident response (< 24h notification)
- ✅ Backup & recovery (daily automated)
- ✅ Network segmentation (VPC with subnets)
- ✅ Audit logging (immutable trail)

**AI Act**
- ✅ AI usage disclosure (for recommendation engines)
- ✅ Human override (bypass AI decisions)
- ✅ Audit trail (all AI-assisted decisions)
- ✅ Right to explanation (decision logging)

**BITV 2.0 (Accessibility)**
- ✅ WCAG 2.1 Level AA (keyboard nav, screen reader)
- ✅ Color contrast (4.5:1 minimum)
- ✅ Alt text for images
- ✅ Semantic HTML (Vue template structure)

**E-Rechnung**
- ✅ ZUGFeRD 3.0 XML generation
- ✅ Hybrid PDF (embedded XML)
- ✅ 10-year archival
- ✅ Signature validation

---

## Scalability & Performance

### Horizontal Scaling Strategy

**Phase 1 (MVP)**
- 1x Catalog Pod
- 1x Identity Pod
- 1x PostgreSQL (no replication)
- 1x Redis (no clustering)
- Target: 1,000 concurrent users/tenant

**Phase 2 (Scale)**
- 3x Catalog Pods (Kubernetes HPA)
- 2x Identity Pods
- 3x PostgreSQL Read Replicas + 1 Write Master
- 3x Redis Cluster
- Target: 10,000 concurrent users/tenant

**Phase 3 (Enterprise)**
- 10x Catalog Pods
- 5x Identity Pods
- 5x PostgreSQL Read Replicas + 1 Write Master
- 5x Redis Cluster + Sentinel
- 3x Elasticsearch nodes (sharded per tenant)
- Target: 100,000+ concurrent users/tenant

### Performance Targets

| Metric | Target | Implementation |
|--------|--------|-----------------|
| **API Response** | < 200ms (P95) | Redis cache, query optimization |
| **Search** | < 500ms (P95) | Elasticsearch with per-tenant indices |
| **Page Load** | < 3s (P95) | CDN, lazy loading, code splitting |
| **Build Time** | < 30s | Vite, incremental builds |
| **Test Execution** | < 5 min | Parallel test runs, test containers |

### Caching Strategy

**Multi-Level Caching**
```
Application Cache (Redis):
  ✅ Products: 5-min TTL
  ✅ Categories: 5-min TTL
  ✅ User settings: 30-min TTL
  ✅ Permissions: 1-hour TTL

Browser Cache:
  ✅ Static assets: 1 year (versioned)
  ✅ API responses: 30 seconds (via Cache-Control header)

Database Query Cache:
  ✅ EF Core 2nd-level cache (optional, Redis)
  ✅ Compiled queries for hot paths
```

---

## Development Environment

### Local Setup (Aspire)

**One-Command Startup**
```bash
# Start all services locally
dotnet run --project backend/Orchestration/B2Connect.Orchestration.csproj

# Everything starts:
# - All microservices (Wolverine)
# - PostgreSQL
# - Redis
# - Elasticsearch (optional)
# - Aspire Dashboard (http://localhost:15500)
```

**Environment Parity**
```
Local (dev)          = Same as production
├── Docker containers (per service)
├── PostgreSQL 16 (same schema)
├── Redis 7 (same config)
├── Wolverine messaging (same events)
└── Aspire orchestration = K8s-like setup
```

### Development Workflows

**Feature Development**
```bash
# 1. Create feature branch
git checkout -b feature/P0.6-withdrawal-right

# 2. Start Aspire (all services)
dotnet run --project backend/Orchestration/B2Connect.Orchestration.csproj

# 3. Develop in your IDE (hot reload enabled)
# Files auto-save → Vite/dotnet watch recompile

# 4. Test with sample data
dotnet run --project backend/CLI/B2Connect.CLI \
  seed --service Catalog --file test-data.json

# 5. Run compliance tests
dotnet test B2Connect.slnx -v minimal

# 6. Push to GitHub (GitHub Actions run full suite)
```

**Code Review Checklist** (Before Commit)
```
Architecture:
  [ ] Onion architecture respected (Core has no dependencies)
  [ ] Wolverine pattern used (not MediatR)
  [ ] DDD bounded contexts honored
  [ ] Multi-tenant isolation verified (TenantId in all queries)

Compliance:
  [ ] Audit logging added
  [ ] PII encryption verified
  [ ] No secrets hardcoded
  [ ] GDPR right-to-forget support

Code Quality:
  [ ] No sync-over-async (.Wait(), .Result)
  [ ] FluentValidation for all commands
  [ ] Tests cover happy path + error cases
  [ ] Code coverage > 80%

Documentation:
  [ ] XML comments for public APIs
  [ ] README updated (if new service)
  [ ] Migration steps documented (if DB change)
```

---

## Troubleshooting

### Common Issues

**Issue: "Port Already in Use"**
```bash
# Aspire/DCP holds ports after shutdown
pkill -9 -f "dcpctrl"
pkill -9 -f "dcpproc"
sleep 2

# Try starting again
dotnet run --project backend/Orchestration/...
```

**Issue: "Cannot Connect to PostgreSQL"**
```bash
# Check if service is healthy in Aspire Dashboard
# http://localhost:15500 → look for "postgres" container

# Verify in terminal
docker ps | grep postgres

# If missing, restart Aspire completely
./scripts/kill-all-services.sh
dotnet run --project backend/Orchestration/...
```

**Issue: "Compilation Error After Pull"**
```bash
# Clean build cache
rm -rf bin obj
dotnet clean
dotnet build B2Connect.slnx

# If still failing, restore packages
dotnet restore B2Connect.slnx
dotnet build B2Connect.slnx
```

---

## Additional Resources

- 📖 **[Wolverine Documentation](https://wolverine.netlify.app/)** - Event-driven patterns
- 📖 **[DDD Quick Start](./DDD_QUICK_START.md)** - Domain-driven design guide
- 📖 **[Backend Developer Guide](./docs/by-role/BACKEND_DEVELOPER.md)** - Development patterns
- 📖 **[Security Engineer Guide](./docs/by-role/SECURITY_ENGINEER.md)** - Compliance details
- 🔧 **[Aspire Dashboard](http://localhost:15500)** - Service observability
- 🧪 **[Testing Strategy](./TESTING_STRATEGY.md)** - Unit/integration/E2E tests

---

**Last Updated:** 28. Dezember 2025  
**Next Review:** 15. Januar 2026  
**Owner:** Architecture Team
