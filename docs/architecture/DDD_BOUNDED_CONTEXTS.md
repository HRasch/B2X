# B2X DDD Bounded Contexts & Onion Architecture

**Last Reviewed:** 2026-01-10 — maintained by `@Architect`

## Current Structure (Unified Backend)

The backend architecture has been reorganized following **Domain-Driven Design (DDD)** principles and **Bounded Contexts**:

```
src/backend/
├── Admin/              # 🔐 Admin Operations Context
│   ├── API/            # Admin Gateway
│   └── Tests/
├── Store/              # 🛍️ Public Storefront Context
│   ├── API/            # Store Gateway
│   ├── Catalog/        # Product Catalog
│   ├── CMS/            # Content Management
│   ├── Theming/        # Design & Layouts
│   ├── Localization/   # i18n
│   ├── Search/         # Elasticsearch
│   └── Tests/
├── Management/         # ⚙️ Management Operations Context
│   ├── API/            # Management Gateway
│   └── Tests/
├── Infrastructure/     # 🔧 Infrastructure Layer
│   ├── Hosting/        # Aspire AppHost
│   ├── ServiceDefaults/ # Shared Service Defaults
│   └── Tests/
├── Services/           # 🔄 Cross-Context Services
│   ├── ERP/            # ERP Connectors
│   ├── Identity/       # Authentication
│   ├── Tenancy/        # Multi-Tenancy
│   └── Tests/
├── Shared/             # 📦 Shared Libraries (Kernel)
│   ├── Core/           # Domain Kernel
│   ├── Infrastructure/ # Shared Infrastructure
│   ├── Messaging/      # Wolverine Messaging
│   ├── Search/         # Elasticsearch
│   └── Tests/
└── Tests/              # 🧪 Integration Tests
```

## Bounded Contexts Explained

### 1. Store Context (Public Storefront)

**Responsibility**: Public, read-only APIs for the online shop

**Services**:
- **Catalog**: Products, categories, brands, attributes
- **CMS**: Pages, components, content
- **Theming**: Designs, layouts, templates, themes
- **Localization**: Multi-language support, translations
- **Search**: Full-text search (Elasticsearch)

**Characteristics**:
- ✅ Read-Only (no write operations)
- ✅ Publicly accessible
- ✅ High performance (caching)
- ✅ Scalable

**Frontend**: `frontend-store` (Port 5173)

---

### 2. Admin Context (Admin Operations)

**Responsibility**: CRUD operations, administration, configuration

**Services**:
- **Admin API**: Central admin operations
  - Product management (CRUD)
  - Content management (CRUD)
  - User management
  - Configuration

**Characteristics**:
- ✅ Full CRUD
- ✅ JWT Authentication
- ✅ Role-Based Authorization
- ✅ Audit Logging

**Frontend**: `frontend-admin` (Port 5174)

---

### 3. Management Context (Management Operations)

**Responsibility**: Tenant management, system configuration, monitoring

**Services**:
- **Management API**: Tenant and system management
  - Tenant configuration
  - System settings
  - Monitoring and health checks

**Characteristics**:
- ✅ Administrative operations
- ✅ Multi-tenant aware
- ✅ System-level configuration
- ✅ Monitoring integration

**Frontend**: `frontend-management` (Port 5175)

---

### 4. Shared Context (Cross-Context Services)

**Responsibility**: Services used by multiple contexts

**Services**:
- **Identity**: Authentication, user management
- **Tenancy**: Multi-tenant support, tenant isolation
- **ERP**: ERP system connectors

**Characteristics**:
- ✅ Cross-context
- ✅ Reusable
- ✅ No business logic

---

### 5. Infrastructure Context (Technical Infrastructure)

**Responsibility**: Hosting, orchestration, shared infrastructure

**Services**:
- **AppHost**: .NET Aspire orchestration
- **ServiceDefaults**: Shared service configuration
- **Hosting**: Deployment and hosting concerns

**Characteristics**:
- ✅ Technical infrastructure
- ✅ Deployment concerns
- ✅ Shared configuration

## Onion Architecture (within each Service)

Each service follows the **Onion Architecture** with 4 layers:

```
Service/
├── Core/                   # 🎯 Domain Layer (Innermost Ring)
│   ├── Entities/           # Domain Entities (Product, Category)
│   ├── ValueObjects/       # Value Objects (Price, SKU)
│   ├── Interfaces/         # Repository Contracts
│   ├── Exceptions/         # Domain Exceptions
│   └── Events/             # Domain Events
│
├── Application/            # 📋 Application Layer
│   ├── DTOs/               # Data Transfer Objects
│   ├── Handlers/           # CQRS Handlers (Commands/Queries)
│   ├── Validators/         # FluentValidation
│   └── Mappers/            # Entity ↔ DTO Mapping
│
├── Infrastructure/         # 🔧 Infrastructure Layer
│   ├── Repositories/       # EF Core Implementations
│   ├── Data/               # DbContext, Migrations
│   ├── External/           # External Services (Elasticsearch)
│   ├── Caching/            # Redis, Memory Cache
│   └── Messaging/          # Event Bus
│
└── Presentation/           # 🌐 API Layer (Outermost Ring)
    ├── Controllers/        # REST Endpoints
    ├── Middleware/         # Custom Middleware
    ├── Configuration/      # Dependency Injection
    └── Program.cs          # Entry Point
```

### Dependency Flow (Onion Principle)

```
Presentation → Infrastructure → Application → Core
   (API)          (Data)         (Logic)      (Domain)

Dependencies ALWAYS point INWARD!
Core has NO dependencies to outer layers.
```

---

## DDD Patterns Used

### Aggregate Roots
- `Product` (Catalog)
- `CmsPage` (CMS)
- `Theme` (Theming)

### Repositories
- One repository per Aggregate Root
- Only interfaces in Core, implementation in Infrastructure

### Domain Events
- `ProductCreatedEvent`
- `PriceChangedEvent`
- `PagePublishedEvent`

### Value Objects
- `Price` (Amount + Currency)
- `SKU` (unique product code)
- `LocalizedContent` (Text + Language)

### CQRS Pattern
- **Commands**: Write operations (Admin Context)
- **Queries**: Read operations (Store Context)
- Separation improves performance and scalability

---

## Communication between Contexts

### Synchronous (HTTP)
- Store → Shared (Identity for token validation)
- Admin → Shared (Identity, Tenancy)

### Asynchronous (Events)
- Admin Context publishes events → Store Context reacts
- Example: `ProductUpdatedEvent` → Elasticsearch reindex

### Message Bus
- **Wolverine** for in-process messaging
- **RabbitMQ/Azure Service Bus** for microservices (optional)

---

## Benefits of this Structure

### ✅ Clear Responsibilities
- Each Bounded Context has its own responsibility
- No mixed business logic

### ✅ Scalability
- Store Context can be horizontally scaled
- Admin Context needs fewer instances

### ✅ Maintainability
- Onion Architecture enforces clean dependencies
- Core remains free of framework code

### ✅ Testability
- Domain logic (Core) is isolated and testable
- Mocking of Infrastructure is simple

### ✅ Deployment Flexibility
- Contexts can be deployed independently
- Microservices-ready

---

## Migration Status (Completed)

- [x] Unified src/backend/ structure implemented
- [x] Services moved to appropriate contexts
- [x] Solution file updated
- [x] Tasks.json updated
- [x] Namespaces standardized (B2X.Store.*, B2X.Admin.*, etc.)
- [x] Project references updated
- [x] Orchestration (Aspire) configured
- [x] Tests reorganized and passing
- [x] Documentation updated

---

## Next Steps

1. **Performance Optimization**:
   - Implement caching strategies
   - Optimize database queries
   - Add monitoring and metrics

2. **Microservices Evolution**:
   - Separate deployments per context
   - API Gateway implementation
   - Service mesh consideration

3. **Testing Enhancement**:
   - Integration tests between contexts
   - Performance testing
   - Chaos engineering

4. **CI/CD Enhancement**:
   - Context-specific build pipelines
   - Automated testing per context
   - Independent deployments

---

## Resources

- [Onion Architecture](../archive/architecture-docs/ONION_ARCHITECTURE.md)
- [CQRS Pattern](../../docs/features/CQRS_INTEGRATION_POINT1.md)
- [DDD Patterns](https://martinfowler.com/bliki/DomainDrivenDesign.html)
