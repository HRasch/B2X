# B2Connect Architecture Restructuring Plan

## Overview
This document outlines the reorganization of the B2Connect project structure to align with Domain-Driven Design (DDD), Microservice, and Clean Architecture principles.

---

## Current State Analysis

### Services
- **Authentication Service** (auth-service)
- **Tenant Service** (tenant-service)
- **Catalog Service** (CatalogService)
- **Theme Service** (ThemeService)
- **Layout Service** (LayoutService)
- **Localization Service** (LocalizationService)
- **API Gateway** (api-gateway)
- **AppHost** (Aspire orchestration)
- **ServiceDefaults** (shared defaults)

### Shared Libraries (Cross-cutting concerns)
- **Types** - Shared type definitions
- **Utils** - Utility functions
- **Middleware** - Cross-service middleware
- **Core** - Core abstractions
- **Data** - Data access patterns
- **Search** - Search/Elasticsearch integration
- **Messaging** - Message bus/event handling
- **Extensions** - Extension methods
- **AOP** - Aspect-oriented programming
- **Validators** - Validation rules

---

## Proposed DDD/Clean Architecture Structure

### Principles Applied

1. **Bounded Contexts** - Each microservice represents a bounded context
2. **Domain-Driven Design Layers**:
   - **Domain** - Core business logic, entities, aggregates, value objects
   - **Application** - Use cases, commands, queries, handlers (CQRS)
   - **Infrastructure** - Data access, external services, messaging
   - **Presentation** - APIs, controllers (for monolithic parts)

3. **Clean Architecture** - Dependencies point inward toward domain
4. **Separation of Concerns** - Clear boundaries between layers
5. **CQRS Pattern** - Command Query Responsibility Segregation (where applicable)

---

## Target Directory Structure

```
backend/
├── services/
│   ├── Gateway/                          (API Gateway - Entry point)
│   │   ├── B2Connect.Gateway.csproj
│   │   └── src/
│   │       ├── Endpoints/               (API routes)
│   │       ├── Middleware/              (Gateway middleware)
│   │       ├── Configuration/           (Routing config)
│   │       └── Program.cs
│   │
│   ├── Identity/                        (Auth - Bounded Context)
│   │   ├── B2Connect.Identity.API.csproj
│   │   ├── B2Connect.Identity.Core.csproj
│   │   ├── B2Connect.Identity.Application.csproj
│   │   ├── B2Connect.Identity.Infrastructure.csproj
│   │   ├── B2Connect.Identity.Tests.csproj
│   │   └── src/
│   │       ├── API/                     (Endpoints, Controllers)
│   │       ├── Core/                    (Domain: Entities, Value Objects, Aggregates)
│   │       ├── Application/             (UseCases, Commands, Queries, Handlers, DTOs)
│   │       └── Infrastructure/          (Database, External Services, Repositories)
│   │
│   ├── Catalog/                         (Catalog - Bounded Context)
│   │   ├── B2Connect.Catalog.API.csproj
│   │   ├── B2Connect.Catalog.Core.csproj
│   │   ├── B2Connect.Catalog.Application.csproj
│   │   ├── B2Connect.Catalog.Infrastructure.csproj
│   │   ├── B2Connect.Catalog.Tests.csproj
│   │   └── src/
│   │       ├── API/                     (Endpoints, Controllers)
│   │       ├── Core/                    (Domain: Product, Category, Aggregates)
│   │       ├── Application/             (UseCases, Commands, Queries)
│   │       └── Infrastructure/          (Database, Search, Repositories)
│   │
│   ├── Tenancy/                         (Tenant - Bounded Context)
│   │   ├── B2Connect.Tenancy.API.csproj
│   │   ├── B2Connect.Tenancy.Core.csproj
│   │   ├── B2Connect.Tenancy.Application.csproj
│   │   ├── B2Connect.Tenancy.Infrastructure.csproj
│   │   ├── B2Connect.Tenancy.Tests.csproj
│   │   └── src/
│   │       ├── API/
│   │       ├── Core/                    (Domain: Tenant, Subscription)
│   │       ├── Application/
│   │       └── Infrastructure/
│   │
│   ├── Theming/                         (Theme - Bounded Context)
│   │   ├── B2Connect.Theming.API.csproj
│   │   ├── B2Connect.Theming.Core.csproj
│   │   ├── B2Connect.Theming.Application.csproj
│   │   ├── B2Connect.Theming.Infrastructure.csproj
│   │   ├── B2Connect.Theming.Tests.csproj
│   │   └── src/
│   │       ├── API/
│   │       ├── Core/                    (Domain: Theme, Layout, Component)
│   │       ├── Application/
│   │       └── Infrastructure/
│   │
│   ├── Localization/                    (i18n - Bounded Context)
│   │   ├── B2Connect.Localization.API.csproj
│   │   ├── B2Connect.Localization.Core.csproj
│   │   ├── B2Connect.Localization.Application.csproj
│   │   ├── B2Connect.Localization.Infrastructure.csproj
│   │   ├── B2Connect.Localization.Tests.csproj
│   │   └── src/
│   │       ├── API/
│   │       ├── Core/                    (Domain: Language, Translation)
│   │       ├── Application/
│   │       └── Infrastructure/
│   │
│   ├── Orchestration/                   (Aspire Host)
│   │   ├── B2Connect.Orchestration.csproj
│   │   └── Program.cs
│   │
│   └── Defaults/                        (Shared Defaults)
│       ├── B2Connect.ServiceDefaults.csproj
│       └── Extensions/
│
├── shared/
│   ├── B2Connect.Shared.csproj          (Facade for all shared)
│   │
│   ├── Kernel/                          (Core abstractions - ALL layers depend on this)
│   │   ├── B2Connect.Shared.Kernel.csproj
│   │   └── src/
│   │       ├── Abstractions/            (Interfaces, Base Classes)
│   │       │   ├── IEntity.cs
│   │       │   ├── IRepository.cs
│   │       │   ├── IUnitOfWork.cs
│   │       │   ├── IHandler.cs
│   │       │   ├── ICommand.cs
│   │       │   ├── IQuery.cs
│   │       │   ├── IDomainEvent.cs
│   │       │   └── IService.cs
│   │       ├── Exceptions/              (Base exceptions)
│   │       ├── Types/                   (Common value types)
│   │       └── Extensions/              (Core extensions)
│   │
│   ├── Domain/                          (Domain events, specifications, domain services)
│   │   ├── B2Connect.Shared.Domain.csproj
│   │   └── src/
│   │       ├── Events/                  (Base domain events)
│   │       ├── Specifications/          (Reusable specs)
│   │       └── ValueObjects/            (Shared VOs: Id, Email, etc)
│   │
│   ├── Application/                     (CQRS, MediaTR handlers, pipelines)
│   │   ├── B2Connect.Shared.Application.csproj
│   │   └── src/
│   │       ├── Behaviors/               (Validation, Logging, Transaction)
│   │       ├── DTOs/                    (Common DTOs)
│   │       ├── Mappings/                (AutoMapper profiles)
│   │       └── Handlers/                (Base handlers)
│   │
│   ├── Infrastructure/                  (EF Core, Data Context, Repositories)
│   │   ├── B2Connect.Shared.Infrastructure.csproj
│   │   └── src/
│   │       ├── Persistence/             (DbContext, Migrations)
│   │       │   ├── Design/
│   │       │   ├── Migrations/
│   │       │   └── Configurations/
│   │       ├── Repositories/            (Base repository implementations)
│   │       ├── Data/                    (Seeders, factories)
│   │       └── UnitOfWork/
│   │
│   ├── Messaging/                       (Event bus, message handling)
│   │   ├── B2Connect.Shared.Messaging.csproj
│   │   └── src/
│   │       ├── Events/                  (Integration events)
│   │       ├── Handlers/                (Event handlers)
│   │       └── Bus/                     (Message bus abstraction)
│   │
│   ├── Search/                          (Elasticsearch integration)
│   │   ├── B2Connect.Shared.Search.csproj
│   │   └── src/
│   │       ├── Indexing/
│   │       ├── Queries/
│   │       └── Configurations/
│   │
│   ├── Validation/                      (Fluent Validation, Custom validators)
│   │   ├── B2Connect.Shared.Validation.csproj
│   │   └── src/
│   │       ├── Rules/
│   │       ├── Extensions/
│   │       └── Validators/
│   │
│   ├── Middleware/                      (Cross-cutting middleware)
│   │   ├── B2Connect.Shared.Middleware.csproj
│   │   └── src/
│   │       ├── Logging/
│   │       ├── ErrorHandling/
│   │       ├── Authentication/
│   │       ├── Tenant/
│   │       └── Localization/
│   │
│   └── Tools/                           (Utilities, Helpers)
│       ├── B2Connect.Shared.Tools.csproj
│       └── src/
│           ├── Extensions/
│           ├── Helpers/
│           ├── Filters/
│           └── Formatters/
│
└── tests/                               (Integration & E2E tests)
    ├── B2Connect.IntegrationTests.csproj
    ├── B2Connect.E2ETests.csproj
    └── Common/
        └── TestFixtures/
```

---

## Layered Architecture Within Each Service

### Example: Catalog Service Structure

```
services/Catalog/
├── src/
│   ├── API/                           (Presentation Layer)
│   │   ├── Endpoints/
│   │   │   ├── Products/
│   │   │   │   ├── CreateProduct.cs
│   │   │   │   ├── GetProduct.cs
│   │   │   │   └── ListProducts.cs
│   │   │   └── Categories/
│   │   ├── Controllers/               (Alternative: Traditional controllers)
│   │   └── Middleware/                (Service-specific middleware)
│   │
│   ├── Core/                          (Domain Layer - Pure Business Logic)
│   │   ├── Entities/
│   │   │   ├── Product.cs             (Aggregate Root)
│   │   │   ├── Category.cs
│   │   │   └── ProductVariant.cs
│   │   ├── ValueObjects/
│   │   │   ├── ProductId.cs
│   │   │   ├── Price.cs
│   │   │   └── Sku.cs
│   │   ├── Aggregates/
│   │   │   └── ProductAggregate.cs
│   │   ├── Repositories/              (Interfaces only)
│   │   │   ├── IProductRepository.cs
│   │   │   └── ICategoryRepository.cs
│   │   ├── Services/                  (Domain Services)
│   │   │   └── PricingDomainService.cs
│   │   ├── Events/                    (Domain Events)
│   │   │   ├── ProductCreatedEvent.cs
│   │   │   ├── PriceChangedEvent.cs
│   │   │   └── ProductDeletedEvent.cs
│   │   ├── Exceptions/                (Domain Exceptions)
│   │   │   ├── InvalidProductException.cs
│   │   │   └── InsufficientStockException.cs
│   │   └── Specifications/
│   │       ├── ActiveProductsSpecification.cs
│   │       └── DiscountedProductsSpec.cs
│   │
│   ├── Application/                   (Application Layer - Use Cases)
│   │   ├── Commands/                  (CUD Operations)
│   │   │   ├── CreateProduct/
│   │   │   │   ├── CreateProductCommand.cs
│   │   │   │   ├── CreateProductHandler.cs
│   │   │   │   └── CreateProductValidator.cs
│   │   │   ├── UpdateProduct/
│   │   │   └── DeleteProduct/
│   │   ├── Queries/                   (Read Operations)
│   │   │   ├── GetProduct/
│   │   │   │   ├── GetProductQuery.cs
│   │   │   │   ├── GetProductHandler.cs
│   │   │   │   └── ProductDto.cs
│   │   │   ├── SearchProducts/
│   │   │   └── ListCategories/
│   │   ├── DTOs/                      (Data Transfer Objects)
│   │   │   ├── ProductDto.cs
│   │   │   ├── CreateProductRequest.cs
│   │   │   └── ProductResponse.cs
│   │   ├── Mappings/                  (AutoMapper profiles)
│   │   │   └── ProductMappingProfile.cs
│   │   ├── Services/                  (Application Services)
│   │   │   └── ProductApplicationService.cs
│   │   └── Handlers/                  (Event handlers)
│   │       └── ProductCreatedEventHandler.cs
│   │
│   ├── Infrastructure/                (Infrastructure Layer)
│   │   ├── Persistence/
│   │   │   ├── CatalogDbContext.cs
│   │   │   ├── Repositories/
│   │   │   │   ├── ProductRepository.cs
│   │   │   │   ├── CategoryRepository.cs
│   │   │   │   └── BaseRepository.cs
│   │   │   ├── Configurations/        (EF Configurations)
│   │   │   │   ├── ProductConfiguration.cs
│   │   │   │   └── CategoryConfiguration.cs
│   │   │   └── Migrations/
│   │   ├── External/                  (External services)
│   │   │   └── InventoryServiceClient.cs
│   │   ├── Search/                    (Elasticsearch)
│   │   │   ├── ProductIndexer.cs
│   │   │   └── ProductSearchRepository.cs
│   │   └── DependencyInjection.cs      (Service registration)
│   │
│   └── Program.cs                     (Service startup)
│
└── B2Connect.Catalog.{Layer}.csproj   (Project files for each layer)
```

---

## Project Organization Strategy

### Naming Convention
```
B2Connect.[BoundedContext].{Layer}.csproj

Examples:
- B2Connect.Catalog.API.csproj           (API endpoints)
- B2Connect.Catalog.Core.csproj          (Domain layer)
- B2Connect.Catalog.Application.csproj   (Application layer)
- B2Connect.Catalog.Infrastructure.csproj(Infrastructure layer)
- B2Connect.Catalog.Tests.csproj         (Unit/Integration tests)

Shared:
- B2Connect.Shared.Kernel.csproj         (Core abstractions)
- B2Connect.Shared.Domain.csproj         (Domain patterns)
- B2Connect.Shared.Application.csproj    (CQRS/Handlers)
- B2Connect.Shared.Infrastructure.csproj (Data access)
```

### Dependency Rules (Acyclic)

```
API (Endpoints/Controllers)
  ↓ (depends on)
Application (Commands, Queries, Handlers)
  ↓ (depends on)
Core (Domain, Entities, Business Logic)
  ↓ (depends on)
Infrastructure (Repositories, Data Access)
  ↓ (depends on)
Shared.Kernel (Core abstractions - no dependencies except .NET)

Horizontal Dependencies:
- Core projects DO depend on Shared.Kernel
- Application projects DO depend on Core + Shared.Application
- Infrastructure projects DO depend on Core + Shared.Infrastructure
- API projects DO depend on Application + Core
- Shared.* DO NOT depend on service-specific projects
```

---

## Refactoring Priorities

### Phase 1: Foundation (Weeks 1-2)
1. Create shared layer structure (Kernel, Domain, Application, Infrastructure)
2. Define core abstractions and interfaces
3. Set up CQRS framework

### Phase 2: Gateway & Identity (Weeks 2-3)
1. Restructure API Gateway
2. Refactor Authentication service with layered architecture

### Phase 3: Core Services (Weeks 3-5)
1. Refactor Catalog service
2. Refactor Tenancy service
3. Refactor Theming service

### Phase 4: Supporting Services (Weeks 5-6)
1. Refactor Localization service
2. Finalize Search & Messaging integration

### Phase 5: Testing & Documentation (Weeks 6-7)
1. Update test projects
2. Update solution files (.slnx)
3. Documentation

---

## Key DDD/Clean Architecture Principles

### 1. Bounded Contexts
- Each service owns its data
- No shared databases
- Integration through APIs/Events

### 2. Layering
- **Domain**: Pure business logic, no framework dependencies
- **Application**: Orchestration, no business logic
- **Infrastructure**: Technical implementations
- **API**: Request/Response mapping

### 3. CQRS Pattern
- Commands (CUD) go through handlers
- Queries are optimized reads
- Handlers manage transactions/events

### 4. Domain Events
- Triggered when aggregate state changes
- Used for cross-service communication
- Decoupled event publishing

### 5. Repositories
- Abstract data access
- One per aggregate root
- Implement Unit of Work pattern

### 6. Value Objects
- Immutable
- No identity
- Encapsulate validation logic

---

## Migration Checklist

- [ ] Create new directory structure
- [ ] Create project files for each layer
- [ ] Move existing code to appropriate layers
- [ ] Extract domain logic from controllers
- [ ] Implement repositories
- [ ] Set up CQRS handlers
- [ ] Refactor tests for new structure
- [ ] Update .slnx files
- [ ] Update CI/CD pipeline
- [ ] Document bounded contexts
- [ ] Add ADRs (Architecture Decision Records)

---

## Tools & Technologies
- **CQRS Framework**: MediatR
- **Validation**: FluentValidation
- **ORM**: Entity Framework Core
- **Mapping**: AutoMapper
- **Testing**: xUnit, Moq, TestContainers
- **Search**: Elasticsearch
- **Messaging**: RabbitMQ/Azure Service Bus
- **API**: Fast Endpoints or Vertical Slice Architecture

---

## References
- Domain-Driven Design (Eric Evans)
- Clean Architecture (Robert C. Martin)
- Microservices Patterns (Chris Richardson)
- CQRS Pattern (Microsoft Docs)
- Fast Endpoints Documentation
