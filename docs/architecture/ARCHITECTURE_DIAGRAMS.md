# B2X Architecture Diagram

**Last Updated**: 28. Dezember 2025

---

## System Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                    FRONTEND LAYER                               │
├──────────────────────────────┬──────────────────────────────────┤
│   Frontend Store (5173)       │    Frontend Admin (5174)         │
│   Vue.js + TypeScript         │    Vue.js + TypeScript           │
└──────────────────────────────┴──────────────────────────────────┘
            ▼                                    ▼
┌─────────────────────────────────────────────────────────────────┐
│                    HTTP LAYER (Wolverine)                       │
│                  All Services are HTTP-First                    │
├──────────────────────────────────────────────────────────────────┤
│  POST /checkregistrationtype   POST /login      GET /products   │
│  POST /createproduct           POST /register   GET /search     │
│  POST /publishpage             GET /health      POST /migrate   │
└──────────────────────────────────────────────────────────────────┘
             ▼                    ▼                   ▼
┌──────────────────┬──────────────────┬──────────────────┬────────────┐
│ MICROSERVICES    │                  │                  │            │
├──────────────────┴──────────────────┴──────────────────┴────────────┤
│                                                                      │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌──────────┐  │
│  │  Identity   │  │  Tenancy    │  │  Catalog    │  │   CMS    │  │
│  │  (7002)     │  │  (7003)     │  │  (7005)     │  │  (7006)  │  │
│  │ Wolverine   │  │ Wolverine   │  │ Wolverine   │  │Wolverine │  │
│  └─────────────┘  └─────────────┘  └─────────────┘  └──────────┘  │
│                                                                      │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐                │
│  │ Theming     │  │Localization │  │   Search    │                │
│  │  (7008)     │  │  (7004)     │  │  (9300)     │                │
│  │ Wolverine   │  │ Wolverine   │  │ Wolverine   │                │
│  └─────────────┘  └─────────────┘  └─────────────┘                │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘
             ▼
┌──────────────────────────────────────────────────────────────────────┐
│              EVENT BUS (Wolverine Message Bus)                       │
├──────────────────────────────────────────────────────────────────────┤
│  • ProductCreatedEvent     • UserRegisteredEvent                     │
│  • PagePublishedEvent      • TenantCreatedEvent                      │
│  • PriceChangedEvent       • PermissionChangedEvent                  │
│  • SearchIndexUpdatedEvent                                           │
└──────────────────────────────────────────────────────────────────────┘
             ▲          ▲           ▲          ▲
             │          │           │          │
         Publish    Subscribe   Publish    Subscribe
             │          │           │          │
┌────────────┴──────────┴───────────┴──────────┴────────────────────────┐
│                        PERSISTENCE LAYER                              │
├────────────────────────────────────────────────────────────────────────┤
│                                                                        │
│  PostgreSQL (5432)              Redis (6379)                         │
│  ├─ B2X_identity          ├─ Sessions                         │
│  ├─ B2X_tenancy           ├─ Cache                            │
│  ├─ B2X_catalog           └─ Temporary Data                   │
│  ├─ B2X_cms                                                    │
│  └─ B2X_search            Elasticsearch (9200)                │
│                                  └─ Full-text Search Index          │
│                                                                        │
└────────────────────────────────────────────────────────────────────────┘
             ▲
             │
┌────────────┴─────────────────────────────────────────────────────────┐
│                    CLI LAYER (Operations)                             │
├──────────────────────────────────────────────────────────────────────┤
│  B2X auth create-user                                          │
│  B2X tenant create                                             │
│  B2X product import-csv                                        │
│  B2X migrate --service Identity                                │
│  B2X seed --service Catalog                                    │
│  B2X status --all                                              │
└──────────────────────────────────────────────────────────────────────┘
```

---

## Request Flow (Frontend to Service)

```
┌──────────────────────────────────────────────────────────────────┐
│                  Frontend Application                            │
│  (Vue.js Component - RegistrationCheck.vue)                     │
└──────────────────────────────────────────────────────────────────┘
                              │
                              │ 1. User enters email & business type
                              ▼
                     ┌─────────────────┐
                     │ Submit Form     │
                     └─────────────────┘
                              │
                              │ 2. Call registrationService.ts
                              ▼
                     ┌──────────────────────────────┐
                     │ HTTP POST Request            │
                     │ URL: localhost:7002/check... │
                     │ Header: X-Tenant-ID          │
                     │ Body: { email, businessType }│
                     └──────────────────────────────┘
                              │
                              │ 3. Network
                              ▼
                     ┌──────────────────────────────┐
                     │ Identity Microservice        │
                     │ (Wolverine Service)          │
                     │ Port 7002                    │
                     └──────────────────────────────┘
                              │
                              │ 4. Auto-discovered Handler
                              │    CheckRegistrationTypeService
                              │    .CheckType()
                              ▼
                     ┌──────────────────────────────┐
                     │ Business Logic               │
                     │ 1. Check ERP System          │
                     │ 2. Check Duplicates          │
                     │ 3. Determine Type            │
                     └──────────────────────────────┘
                              │
                              │ 5. Query Databases
                              ▼
         ┌────────────────────┬────────────────────┐
         │ PostgreSQL         │ Elasticsearch      │
         │ Check existing     │ Check duplicates   │
         │ Check ERP data     │                    │
         └────────────────────┴────────────────────┘
                              │
                              │ 6. Response
                              ▼
                     ┌──────────────────────────────┐
                     │ JSON Response                │
                     │ {                            │
                     │   success: true,             │
                     │   registrationType: "...",   │
                     │   erpData: { ... }           │
                     │ }                            │
                     └──────────────────────────────┘
                              │
                              │ 7. Network
                              ▼
                     ┌──────────────────────────────┐
                     │ Frontend receives Response   │
                     └──────────────────────────────┘
                              │
                              │ 8. Update Component State
                              ▼
                     ┌──────────────────────────────┐
                     │ Display Results              │
                     │ • Registration type badge    │
                     │ • ERP data table (if found)  │
                     │ • Confidence score           │
                     └──────────────────────────────┘
```

---

## Service-to-Service Communication (Events)

```
┌─────────────────────────────────────────────────────────────────┐
│           Service A (e.g., Catalog)                             │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │  ProductService.CreateProduct()                          │  │
│  │  {                                                       │  │
│  │    var product = new Product(...);                       │  │
│  │    await _repository.AddAsync(product);                  │  │
│  │    await _messageBus.PublishAsync(new                    │  │
│  │      ProductCreatedEvent(productId, tenantId, sku)       │  │
│  │    );  // ← Publish Event                               │  │
│  │  }                                                       │  │
│  └──────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                              │
                              │ Event Published
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                    EVENT BUS (Wolverine)                        │
│                  ProductCreatedEvent                            │
└─────────────────────────────────────────────────────────────────┘
                  │          │          │
         ┌────────┘          │          └────────┐
         │                   │                   │
         ▼                   ▼                   ▼
┌───────────────────┐ ┌──────────────┐ ┌──────────────────┐
│   Search Service  │ │ Cache Service│ │ Notification Svc │
│                   │ │              │ │                  │
│ SearchEventHandler│ │CacheHandler  │ │NotificationHandler
│.Handle(event)    │ │.Handle(event)│ │.Handle(event)    │
│                   │ │              │ │                  │
│ Index in          │ │ Invalidate   │ │ Send Email/SMS   │
│ Elasticsearch     │ │ cache        │ │ to user          │
└───────────────────┘ └──────────────┘ └──────────────────┘
```

---

## CLI Command Architecture

```
┌─────────────────────────────────────────────────────┐
│  User runs: B2X auth create-user email@x.com  │
└─────────────────────────────────────────────────────┘
                        ▼
        ┌──────────────────────────────┐
        │ Program.cs                   │
        │ Routes command to group       │
        │ AuthCommands.CreateUserCmd   │
        └──────────────────────────────┘
                        ▼
        ┌──────────────────────────────┐
        │ CreateUserCommand            │
        │ ├─ Parse arguments           │
        │ ├─ Validate input            │
        │ └─ Execute                   │
        └──────────────────────────────┘
                        ▼
        ┌──────────────────────────────┐
        │ CliHttpClient                │
        │ POST http://localhost:7002/  │
        │   auth/create-user           │
        │ { email, password, tenantId }│
        └──────────────────────────────┘
                        ▼
        ┌──────────────────────────────┐
        │ Identity Microservice        │
        │ CreateUserService.CreateUser│
        └──────────────────────────────┘
                        ▼
        ┌──────────────────────────────┐
        │ ConsoleOutputService         │
        │ [green]✓ User created[/]     │
        │ User ID: {userId}            │
        └──────────────────────────────┘
```

---

## Multi-Tenancy Request Path

```
Request
├─ Header: X-Tenant-ID: <guid-12345>
├─ Header: Authorization: Bearer <jwt-token>
└─ Body: { email: "user@example.com", ... }
           │
           ▼
    ┌─────────────────────────────────┐
    │ Middleware                      │
    │ Extract tenant from header      │
    │ Set HttpContext.TenantId        │
    └─────────────────────────────────┘
           │
           ▼
    ┌─────────────────────────────────┐
    │ Service Handler                 │
    │ var products =                  │
    │  _repo.GetByTenant(             │
    │    tenantId: context.TenantId  │
    │  )                              │
    └─────────────────────────────────┘
           │
           ▼
    ┌─────────────────────────────────┐
    │ Database Query                  │
    │ SELECT * FROM products          │
    │ WHERE tenant_id = '<guid-12345>'│
    └─────────────────────────────────┘
           │
           ▼
    ┌─────────────────────────────────┐
    │ Response                        │
    │ Only Tenant A's data returned   │
    │ Tenant B's data protected ✓     │
    └─────────────────────────────────┘
```

---

## Data Flow Example: Product Creation

```
Admin User (Tenant A)
        │
        │ POST /product/create
        │ { name: "Laptop", price: 999, tenantId: A }
        ▼
    Catalog Service (Port 7005)
    ├─ Validate input (FluentValidation)
    ├─ Create Product entity
    ├─ Save to PostgreSQL
    │  (INSERT INTO products WHERE tenant_id = A)
    ├─ Publish ProductCreatedEvent
    └─ Return response
           │
      ┌────┴────────┬──────────────┬─────────────┐
      │             │              │             │
      ▼             ▼              ▼             ▼
  Search Svc   Cache Svc    Notification Svc  Analytics Svc
  └─Index in   └─Invalidate └─Send email to   └─Track event
    Elastic      product       admin
    Search      cache

All handled automatically by Wolverine Event Bus!
No manual service-to-service calls needed.
```

---

## Deployment Topology

```
┌──────────────────────────────────────────────────────────┐
│              ASPIRE ORCHESTRATION                        │
│  (dotnet run --project AppHost/B2X.AppHost.csproj)            │
│                                                          │
│  Dashboard: http://localhost:15500                       │
│  ├─ Shows all running services                          │
│  ├─ Health status                                       │
│  ├─ Logs & Traces                                       │
│  └─ Metrics                                             │
└──────────────────────────────────────────────────────────┘
           │
    ┌──────┴──────┬───────────┬───────────┬───────────┐
    │             │           │           │           │
    ▼             ▼           ▼           ▼           ▼
 Identity     Catalog      CMS       Theming      Search
 Service      Service      Service    Service      Service
 (7002)       (7005)       (7006)     (7008)       (9300)
           │
    ┌──────┴──────┬───────────┐
    │             │           │
    ▼             ▼           ▼
 PostgreSQL    Redis      Elasticsearch
 (5432)        (6379)     (9200)

Or for production:
┌─────────────────────────────────────────────┐
│ Kubernetes Cluster (AWS EKS, Azure AKS)    │
├─────────────────────────────────────────────┤
│ Services as Docker containers               │
│ PostgreSQL managed database                 │
│ Redis managed cache                         │
│ Elasticsearch managed service               │
│ Load balancer in front                      │
└─────────────────────────────────────────────┘
```

---

## Technology Stack Summary

```
┌────────────────────────────────────────────────────┐
│           B2X Technology Stack               │
├────────────────────────────────────────────────────┤
│                                                    │
│  Backend:                                          │
│  ├─ .NET 10 / C#                                   │
│  ├─ Wolverine (CQRS, Event Bus, HTTP)            │
│  ├─ Entity Framework Core 8                       │
│  ├─ PostgreSQL 16                                 │
│  ├─ Redis (caching)                               │
│  ├─ Elasticsearch (search)                        │
│  ├─ FluentValidation                              │
│  └─ System.CommandLine (CLI)                      │
│                                                    │
│  Frontend:                                         │
│  ├─ Vue.js 3 (Composition API)                    │
│  ├─ TypeScript                                     │
│  ├─ Vite (bundler)                                │
│  ├─ Axios (HTTP client)                           │
│  ├─ Pinia (state management)                      │
│  ├─ Vue Router (routing)                          │
│  └─ Tailwind CSS                                  │
│                                                    │
│  Infrastructure:                                   │
│  ├─ .NET Aspire (orchestration)                   │
│  ├─ Docker (containerization)                     │
│  ├─ Kubernetes (production)                       │
│  ├─ GitHub Actions (CI/CD)                        │
│  └─ Azure Key Vault (secrets)                     │
│                                                    │
└────────────────────────────────────────────────────┘
```

---

**Last Updated**: 28. Dezember 2025  
**Status**: ✅ COMPLETE

