# B2Connect - Design Decisions

**Owner**: @software-architect  
**Last Updated**: 29. Dezember 2025  
**Status**: Active - COMPLETE  

⚠️ **GOVERNANCE NOTICE**: Only @software-architect can modify this document. All architectural changes require an ADR (Architecture Decision Record) created BEFORE implementation starts. No architectural changes allowed without documented decision and @tech-lead approval.

---

## Overview

This document tracks significant architectural decisions made for B2Connect. For formal records, see [ADR/ folder](./ADR/).

---

## 1. DDD Microservices Architecture

### Context
B2Connect needs independent scaling of different business capabilities:
- Product catalog (high read, low write, large data volume potential)
- Checkout (high write during peak, must be scalable)
- Inventory (synchronized with ERP, real-time critical)
- Admin analytics (complex queries, shouldn't affect customer experience)

Monolith approach would cause:
- Catalog reads blocked by checkout writes
- Inventory sync delays checkout processing
- Analytics queries slowing customer-facing APIs
- Difficult to scale independently

### Options Considered

**Option 1: Monolith** (all features in one .NET app)
```
Pros:
+ Simpler deployment
+ No distributed transaction complexity
+ Easier to debug locally
+ Lower operational overhead

Cons:
- Catalog reads compete with checkout writes
- Scaling requires scaling entire app (waste)
- Inventory sync delays checkout
- Can't deploy Catalog fixes without touching Checkout
- Database becomes bottleneck
```

**Option 2: Microservices with Shared Database**
```
Pros:
+ Loose coupling (separate code)
+ Can scale independently
+ Easier deployments

Cons:
- Tightly coupled at DB layer
- Schema changes affect all services
- No schema flexibility per service
- Distributed transactions still needed
- Cache invalidation nightmare
```

**Option 3: Microservices with Separate Databases** (CHOSEN)
```
Pros:
+ Completely independent scaling
+ Each service owns schema
+ True loose coupling (code + data)
+ Enables polyglot persistence (future)
+ Independent deployment without coordination
+ Fault isolation (one service down ≠ all down)

Cons:
- Distributed transactions (eventual consistency required)
- More complex observability
- More services to operate
- Need event streaming (Wolverine)
```

### Decision
**Microservices with separate databases per bounded context**

### Rationale

1. **Scale Independence**: Catalog can be read-heavy (1 server), Checkout write-heavy (4 servers), no conflict
2. **Deployment Agility**: Each team ships independently (Catalog bug fix doesn't need Checkout review)
3. **Fault Isolation**: Inventory service down doesn't block customer checkout
4. **Technology Fit**: .NET+PostgreSQL excellent for this pattern
5. **Team Structure**: 4-6 engineers can work on 5-6 services independently

### Consequences

**Positive**:
- ✅ Inventory can sync with ERP without affecting checkout latency
- ✅ Product catalog reads scale separately from writes
- ✅ Analytics service can run expensive reports without impacting store
- ✅ Each service optimized for its workload
- ✅ Deployment anxiety reduced (can roll back single service)
- ✅ Team velocity increases (parallel development)

**Negative**:
- ❌ Eventual consistency required (orders confirm before inventory deducts)
- ❌ Cross-service debugging harder (need tracing)
- ❌ More infrastructure to manage (5 services × 3 envs = 15 deployments)
- ❌ Distributed transaction complexity (compensating transactions)
- ❌ Dogfooding required (team must monitor/operate services)

### Related ADR
See [ADR-002: Onion Architecture](../../.ai/decisions/INDEX.md)

---

## 2. Wolverine Messaging, NOT MediatR

### Context
Need in-process command/query handling with async support. Two mature options:

### Options Considered

**Option 1: MediatR** (widely used .NET pattern)
```
Pros:
+ Very popular, large community
+ Lots of tutorials/blog posts
+ Works great for single monolith
+ Good test support

Cons:
- In-process only (no distributed messaging)
- No built-in HTTP endpoint mapping
- Difficult to add async messaging later
- No service discovery
- CQRS pattern forces separate handlers
```

**Option 2: Wolverine** (newer, designed for distributed systems)
```
Pros:
+ HTTP endpoint auto-discovery from handlers
+ Event-driven messaging built-in
+ Distributed saga support
+ Transactional outbox pattern
+ Async/await first-class citizen
+ Better for microservices

Cons:
- Smaller community
- Less online content
- Team learning curve
```

### Decision
**Wolverine for all HTTP handlers and messaging**

### Rationale

1. **HTTP Endpoint Discovery**: Wolverine auto-maps `Service.PublicAsyncMethod()` to `POST /publicasyncmethod` - no routing boilerplate needed
2. **Distributed Messaging**: Can add event-driven workflows without major refactoring
3. **Transactional Outbox**: Built-in support for "save to DB + publish event" atomicity
4. **Future-Proof**: Easier path to saga orchestration, choreography patterns
5. **Better DDD Support**: Event aggregates work naturally with Wolverine

### Consequences

**Positive**:
- ✅ New handlers require zero routing code
- ✅ Can add async event streaming later without major refactoring
- ✅ Built-in idempotency support
- ✅ Transactional messaging (no orphaned events)
- ✅ Better observability (Wolverine logs message flow)

**Negative**:
- ❌ Team needs to learn Wolverine (vs widespread MediatR knowledge)
- ❌ Smaller ecosystem (fewer extensions)
- ❌ Fewer StackOverflow answers
- ❌ No "MediatR decorator" community patterns (yet)

### Key Pattern
```csharp
// ✅ Wolverine Service (no IRequest, no IRequestHandler)
public class CreateProductService {
    public async Task<CreateProductResponse> CreateAsync(
        CreateProductCommand request,
        CancellationToken ct) 
    {
        // Business logic
        // Wolverine auto-maps to: POST /createasync
    }
}

// ❌ MediatR (what we DON'T do)
public record CreateProductCommand : IRequest<Response>;
public class Handler : IRequestHandler<CreateProductCommand, Response> { }
```

### Related ADR
See [ADR-001: Wolverine over MediatR](../../.ai/decisions/INDEX.md)

---

## 3. Onion Architecture (Per Service)

### Context
Each microservice needs clean separation of concerns:
- Domain logic shouldn't depend on frameworks
- Data access shouldn't leak into business logic
- Tests should run without database
- Easy to swap implementations (EF Core → Dapper later)

### Options Considered

**Option 1: Layered Architecture**
```
Presentation → Business Logic → Data Access → Database
(tight coupling, business logic depends on data)
```

**Option 2: Domain-Driven Design (No layers)**
```
Just "aggregates" and "repositories" (vague)
(confusing without structure)
```

**Option 3: Onion Architecture** (CHOSEN)
```
        ┌─────────────────────┐
        │  Presentation       │
        │  (API Routes)       │
        ├─────────────────────┤
        │  Infrastructure     │
        │  (EF, Repos)        │
        ├─────────────────────┤
        │  Application        │
        │  (Handlers, DTOs)   │
        ├─────────────────────┤
        │  Core/Domain        │
        │  (Entities, Logic)  │
        └─────────────────────┘

Dependencies point INWARD only
```

### Decision
**Onion Architecture with 4 layers per service**

### Rationale

1. **Testability**: Core layer (domain logic) has zero framework dependencies → unit tests run in milliseconds
2. **Flexibility**: Swap EF Core for Dapper without touching domain logic
3. **Clarity**: Clear dependency direction (outer → inner, never reverse)
4. **DDD-Aligned**: Core layer owns business rules, Application/Infrastructure serve it
5. **Maintainability**: New developers understand structure immediately

### Consequences

**Positive**:
- ✅ Unit tests run in <1ms (no DB, no container startup)
- ✅ Easy to test all business logic paths
- ✅ Can swap implementations (PostgreSQL → MongoDB) - Infrastructure only
- ✅ Clear where code belongs
- ✅ Dependency direction prevents spaghetti code

**Negative**:
- ❌ More boilerplate (DTOs, Mappers, Repositories)
- ❌ Requires discipline (easy to break layering)
- ❌ More files to navigate
- ❌ Slower initial feature development

### Layer Responsibilities

| Layer | Owns | Examples |
|-------|------|----------|
| **Core** | Entities, Value Objects, Interfaces, Domain Events | Product, Price, Order, IProductRepository (interface) |
| **Application** | Use Cases, Handlers, Validators, DTOs | CreateProductHandler, CreateProductValidator, CreateProductDto |
| **Infrastructure** | DB, Repos, External Services, Caching | ProductRepository, EF Core DbContext, EmailService |
| **Presentation** | API Routes, Middleware, Error Handling | Program.cs, Middleware, Error responses |

### Related ADR
See [ADR-002: Onion Architecture](../../.ai/decisions/INDEX.md)

---

## 4. PostgreSQL with Per-Service Databases

### Context
Microservices with separate databases introduces complexity. Need to decide:
- One shared PostgreSQL with multiple databases per service
- Separate PostgreSQL instances per service
- Different database vendors per service

### Options Considered

**Option 1: One PostgreSQL, One Database**
```
Pros:
+ Simplest operations
+ Easy backups
Cons:
- Shared server limits (CPU, connections)
- Can't scale database independently
- Single point of failure
```

**Option 2: One PostgreSQL, Multiple Databases**
```
Pros:
+ Shared infrastructure
+ Some isolation (schema-level)
+ Easy cross-service queries (if needed)
Cons:
- Still shared server (CPU, RAM bottleneck)
- Can't scale identity differently from catalog
- Blurs service boundaries
```

**Option 3: Per-Service PostgreSQL Instances** (CHOSEN)
```
Pros:
+ True independence
+ Can scale each separately
+ Failure isolation
+ Clear ownership
Cons:
- More infrastructure
- More backups to manage
- More monitoring needed
```

### Decision
**PostgreSQL 16 with per-service database (in same PG cluster for now)**

### Rationale

1. **Isolation**: If Catalog database grows to 100 GB, doesn't affect Checkout
2. **Scalability**: Identity service can use 1 GB, Catalog can use 50 GB
3. **Clear Ownership**: Each service owns schema, no cross-service dependencies
4. **Future Flexibility**: Could move to separate clusters/clouds later
5. **Team Autonomy**: No coordination needed for schema changes

### Consequences

**Positive**:
- ✅ Identity service can scale independently
- ✅ Catalog schema can optimize for product queries without affecting Order service
- ✅ No "shared database" schema conflicts
- ✅ Clear data ownership
- ✅ Easier disaster recovery (restore single service database)

**Negative**:
- ❌ Can't do cross-service joins (must be async via services)
- ❌ More backup complexity (5 databases × 3 envs = 15 backups)
- ❌ Cross-service consistency requires eventual consistency
- ❌ No referential integrity across services (application-layer enforced)

### Implementation Detail
```sql
-- One PostgreSQL server (5432), multiple databases

postgres://localhost/identity      -- Identity service
postgres://localhost/catalog       -- Catalog service
postgres://localhost/cms           -- CMS service
postgres://localhost/order         -- Order service
postgres://localhost/tenancy       -- Tenancy service
postgres://localhost/localization  -- Localization service
postgres://localhost/search        -- Search service (indices)
```

### Related ADR
See [ADR-004: PostgreSQL MultiTenancy](../../.ai/decisions/INDEX.md)

---

## 5. Aspire for Local Orchestration

### Context
Need way for 6 developers to run 5 microservices locally during development without:
- Docker complexity
- Container resource overhead
- Port conflicts
- Manual startup sequence

### Options Considered

**Option 1: Docker Compose**
```
Pros:
+ Prod-like environment
+ Works on all OS
Cons:
- Slow startup (containers)
- Resource hungry (4GB+ RAM)
- Port conflicts (each dev manages)
- Debugging difficult (attach to container)
```

**Option 2: Manual Service Startup**
```
Pros:
+ Debugger attached immediately
Cons:
- Manual startup sequence (5 clicks × 5 services)
- Easy to forget a service
- No dependency management
- Chaos
```

**Option 3: .NET Aspire** (CHOSEN)
```
Pros:
+ Native .NET integration
+ Automatic service discovery
+ Built-in dashboard
+ Dependency ordering
+ Environment variable management
Cons:
- Windows/macOS port issues (fixable)
- Newer (less documentation)
```

### Decision
**.NET Aspire for local development orchestration**

### Rationale

1. **Developer Experience**: One command (`dotnet run` in AppHost) starts all 5 services
2. **Service Discovery**: Services automatically find each other (no hardcoded URLs)
3. **Dashboard**: Single view of all service health, logs, metrics
4. **Dependency Ordering**: Identity starts before Catalog (dependencies enforced)
5. **Debugger Ready**: Can attach debugger to any service immediately

### Consequences

**Positive**:
- ✅ New developer runs single command, all services up
- ✅ Service discovery means no config file edits
- ✅ Dashboard shows which service is slow/broken
- ✅ Can toggle services on/off for focused development
- ✅ Aspire extensions for external services (Redis, PostgreSQL)

**Negative**:
- ❌ macOS/Linux port issues (DCP controller holds ports - fixable with script)
- ❌ Not production-like (Kubernetes very different)
- ❌ Requires .NET 8.2+
- ❌ Dashboard not auth-protected (local-only, acceptable)

### Production Deployment
Aspire is local-only. Production uses:
- Kubernetes (or Cloud Run) for orchestration
- LoadBalancer for routing
- Same microservices architecture

### Related ADR
See [ADR-003: Aspire Orchestration](../../.ai/decisions/INDEX.md)

---

## 6. Multi-Tenancy: X-Tenant-ID Header + Global Filter

### Context
B2Connect serves 100+ shops, each with separate data. Need to prevent:
- Shop A seeing Shop B's customers
- Cross-tenant data leaks
- Authorization bypass

### Options Considered

**Option 1: Separate Service per Tenant**
```
Pros:
+ Complete isolation
Cons:
- 100+ services (operations nightmare)
- Cost prohibitive (100 databases)
- No shared resources
```

**Option 2: Separate Database per Tenant**
```
Pros:
+ Complete isolation
+ Easy backups per tenant
Cons:
- 100+ databases to manage
- Operational complexity
- Hard to run cross-tenant reports
```

**Option 3: Row-Level Security (PostgreSQL built-in)**
```
Pros:
+ Database enforces isolation
+ Hard to accidentally expose
Cons:
- Database becomes complex
- Harder to test locally
- Performance impact
```

**Option 4: TenantId in Every Query (CHOSEN)**
```
Pros:
+ Simple to implement
+ Easy to test (mock tenantId)
+ Performance (indexed queries)
Cons:
- Requires discipline (every query needs WHERE TenantId=X)
- Easy to forget (code review catches)
```

### Decision
**X-Tenant-ID header + TenantId on every entity + Global EF Core filter**

### Rationale

1. **Simplicity**: TenantId is just another column
2. **Performance**: Index on TenantId, query planner optimal
3. **Testability**: Can mock tenantId in tests, no special setup
4. **Visibility**: Easy to see which tenant data belongs to
5. **Auditability**: TenantId in audit logs shows who accessed what

### Implementation Pattern
```csharp
// 1. Every entity has TenantId
public class Product {
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }  // ← MANDATORY
    public string Name { get; set; }
}

// 2. Request header extracts TenantId
var tenantId = httpContext.Request.Headers["X-Tenant-ID"];

// 3. Service filters by TenantId
var products = await _context.Products
    .Where(p => p.TenantId == tenantId)  // ← MANDATORY
    .ToListAsync();

// 4. EF Core global filter (defense-in-depth)
modelBuilder.Entity<Product>()
    .HasQueryFilter(p => p.TenantId == _tenantId);
```

### Consequences

**Positive**:
- ✅ Can't accidentally leak Shop A data to Shop B (global filter prevents)
- ✅ Easy to audit data access (TenantId in all logs)
- ✅ Cross-shop reports easy (filter TenantId = null for aggregate)
- ✅ Testing straightforward (mock tenantId)
- ✅ Horizontal scaling simple (one service handles multiple tenants)

**Negative**:
- ❌ Requires discipline (code review must verify WHERE clause)
- ❌ No database-level enforcement (if discipline breaks, leak happens)
- ❌ Global filter adds query complexity
- ❌ Bulk operations require special handling

### Security Review
See [copilot-instructions-security.md §Multi-Tenancy](../../.github/copilot-instructions-security.md)

---

## 7. Event-Driven Order Processing (Wolverine)

### Context
When customer places order, must:
- Save order to database
- Publish "OrderCreated" event
- Trigger async processes (send email, deduct inventory, notify warehouse)

If synchronous, checkout takes too long. If events lost, customer doesn't get confirmation.

### Options Considered

**Option 1: Synchronous Processing**
```
Customer clicks → Update DB → Send Email → Deduct Inventory → Return (slow!)
User waits 3+ seconds → Terrible UX
```

**Option 2: Fire-and-Forget Events**
```
Customer clicks → Update DB → Publish Event (hope it worked) → Return
Events lost if app crashes → Customer no confirmation
```

**Option 3: Transactional Outbox** (CHOSEN)
```
Customer clicks → Update DB + Save "EmailEvent" row (atomic)
            → Return immediately
Async process → Poll "EmailEvent" row → Send → Delete row
If process crashes → Resumes from where it left off
```

### Decision
**Transactional Outbox pattern with Wolverine**

### Rationale

1. **Atomicity**: Order saved + event published in same transaction (no "order without event")
2. **Durability**: Events survive app crash (stored in DB until processed)
3. **Idempotency**: Can retry event processing without duplicates
4. **Fast Checkout**: Return immediately after DB commit, events process async
5. **Observability**: Event store is audit trail of what happened

### Implementation Pattern
```csharp
// 1. Save order + event in same transaction
using var tx = await _context.Database.BeginTransactionAsync();
{
    var order = new Order { /* ... */ };
    _context.Orders.Add(order);
    
    var evt = new OrderCreatedEvent { OrderId = order.Id };
    _context.OutboxEvents.Add(evt);  // ← Same DB transaction
    
    await _context.SaveChangesAsync();
}

// 2. Wolverine polls OutboxEvents, processes them
// 3. Async handler sends email, deducts inventory
// 4. Marks event as processed (delete from outbox)
```

### Consequences

**Positive**:
- ✅ Order confirmed immediately (checkout returns quickly)
- ✅ Email can fail → Retry tomorrow (no customer impact)
- ✅ Inventory deduction can delay (order already confirmed)
- ✅ Audit trail (every event logged)
- ✅ Can replay events (reprocess all "OrderCreated" for debugging)

**Negative**:
- ❌ Eventually consistent (inventory deducted later, not instantly)
- ❌ Event processing adds operational complexity
- ❌ Outbox table needs cleanup (old processed events)
- ❌ Debugging distributed flow harder

### Related Patterns
- **Saga Pattern**: Multi-step distributed transactions (Wolverine supports)
- **Event Sourcing**: Store all events, rebuild state from them (possible, not currently used)

---

## 8. Encryption at Rest for PII

### Context
GDPR requires protecting personal data. Email, phone, name, address must be encrypted.

### Options Considered

**Option 1: No Encryption**
```
Cons:
- Illegal (GDPR violation)
- Fines: 2-4% annual revenue
- Liability if breached
```

**Option 2: Encrypt at Application Layer** (CHOSEN)
```
Pros:
+ Keys in KeyVault (separate from database)
+ Can rotate keys
+ Encrypts before sending to DB
+ Database admin can't read PII
Cons:
- Application complexity
- Must manage keys
```

**Option 3: Encrypt at Database Layer**
```
Pros:
+ Transparent to application
Cons:
- Keys in database (admin can access)
- Less secure than KeyVault
- Hard to rotate
```

### Decision
**AES-256-GCM encryption at application layer with keys in Azure KeyVault**

### Rationale

1. **Security**: Keys physically separate from data (KeyVault vs Database)
2. **Compliance**: GDPR-compliant encryption
3. **Auditability**: Can see when keys accessed (KeyVault logs)
4. **Rotation**: Annual key rotation possible
5. **Breach Mitigation**: If database stolen, PII still encrypted

### Implementation
```csharp
public interface IEncryptionService {
    Task<string> EncryptAsync(string plaintext, Guid keyId);
    Task<string> DecryptAsync(string ciphertext);
}

// Entity defines PII fields
public class Customer {
    public string Email { get; set; }      // Encrypted
    public string Phone { get; set; }      // Encrypted
    public string FirstName { get; set; }  // Encrypted
    public string LastName { get; set; }   // Encrypted
    public string Address { get; set; }    // Encrypted
}

// Service encrypts before saving
var customer = new Customer {
    Email = await _encryptionService.EncryptAsync(email),
    Phone = await _encryptionService.EncryptAsync(phone),
};
```

### Consequences

**Positive**:
- ✅ Customer data protected even if database breached
- ✅ GDPR-compliant
- ✅ Can decrypt customer on request (right to access)
- ✅ Can delete after decryption (right to forget)

**Negative**:
- ❌ Application must decrypt for each query (slower)
- ❌ Can't search encrypted fields (Email lookup requires decryption)
- ❌ Key management overhead
- ❌ Can't use database full-text search on PII

### PII Fields to Encrypt
- Email address
- Phone number
- First name / Last name
- Postal address
- Date of birth
- VAT ID (for B2B)
- Bank account (if stored)

---

## 9. DDD Bounded Contexts (Per Service)

### Context
Each microservice represents a business domain (Catalog, Order, Identity). Need clear boundaries so teams don't step on each other.

### Design Decision
**Each Wolverine service = One DDD bounded context**

### Rationale
- **Clear Ownership**: Order service owns Order concept
- **Clear Dependencies**: Catalog service → DDD repository for products (not direct DB)
- **Domain Language**: Each context has its own ubiquitous language
- **Schema Isolation**: Each context owns its schema, no foreign keys across services

### Consequences
- ✅ Clear team boundaries
- ✅ Easy to understand what each service does
- ✅ No cross-context dependencies
- ❌ Must use public API (service calls) for cross-context queries

---

## 10. Velocity-Based Development (No Timeboxes)

### Context
Traditional timeboxed sprints (5-day cycles, fixed ceremonies, SLA-based responses) don't work for:
- Agent-driven async development (no "10 AM standup")
- Velocity-based iteration (complete when feature done, not Friday)
- Severity-driven incidents (CRITICAL needs fast response, MEDIUM can wait)
- Priority-based code reviews (P1 reviewed ASAP, P3 reviewed when capacity)

### Decision
**Velocity-based development: complete iterations at ±20% baseline velocity**

### Rationale
1. **Async-First**: No required meeting times (Slack-based)
2. **Continuous Flow**: Complete feature → ship immediately (don't wait for Friday)
3. **Severity-Driven**: CRITICAL incidents get response <1h, P3 reviews <1 week
4. **Flexibility**: If team velocity is 45 points, iteration completes at 36-54 points

### Consequences
- ✅ No "fake work" to fill timeboxes
- ✅ Deploy immediately when ready (no Tue-Thu windows)
- ✅ Faster feedback (code review in hours, not days)
- ✅ Better incident response (severity-driven, not time-based)
- ❌ Must track story points carefully
- ❌ Retrospectives at iteration-end (not Friday)
- ❌ Team discipline required (don't overcommit)

### Metrics
- Baseline velocity: 45 points/iteration
- Acceptable range: 36-54 points (±20%)
- If 3 iterations <36: Reduce commitment
- If 3 iterations >54: Increase capacity or split work

---

## Summary Table

| Decision | Alternative | Why Chosen |
|----------|-------------|-----------|
| **Microservices** | Monolith | Scale independently |
| **Wolverine** | MediatR | HTTP discovery + events |
| **Onion** | Layered | Testability + clarity |
| **Per-Service DB** | Shared DB | True independence |
| **Aspire** | Docker | Native .NET developer UX |
| **TenantId Filter** | Row-Level Security | Simplicity + performance |
| **Event Outbox** | Fire & Forget | Durability + atomicity |
| **App-Layer Encryption** | DB Encryption | Keys in KeyVault |
| **DDD Contexts** | No structure | Clear boundaries |
| **Velocity-Based** | Timeboxed | Async + flexibility |

---

**Last Updated**: 29. Dezember 2025  
**Next Review**: 2026-03-29 (quarterly)  
**Owner**: @software-architect
