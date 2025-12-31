# B2Connect DDD Bounded Contexts & Onion Architecture

**Last Reviewed:** 2025-12-31 — maintained by `@Architect`

## Neue Struktur (ab Dezember 2025)

Die Backend-Architektur wurde nach **Domain-Driven Design (DDD)** Prinzipien und **Bounded Contexts** reorganisiert:

```
backend/
├── BoundedContexts/
│   ├── Store/              # 🛍️ Public Storefront Context
│   │   ├── API/            # Store Gateway (Port 6000)
│   │   ├── Catalog/        # Produktkatalog
│   │   ├── CMS/            # Content Management
│   │   ├── Theming/        # Design & Layouts
│   │   ├── Localization/   # i18n
│   │   └── Search/         # Elasticsearch
│   │
│   ├── Admin/              # 🔐 Admin Operations Context
│   │   └── API/            # Admin Gateway (Port 6100)
│   │
│   └── Shared/             # 🔄 Cross-Context Services
│       ├── Identity/       # Authentication
│       └── Tenancy/        # Multi-Tenancy
│
├── Orchestration/          # ⚙️ Aspire Orchestration
├── ServiceDefaults/        # ⚙️ Shared Service Defaults
│
├── shared/                 # 📦 Shared Libraries (Kernel)
│   ├── kernel/             # Domain Kernel
│   ├── B2Connect.Shared.Core/
│   ├── B2Connect.Shared.Infrastructure/
│   ├── B2Connect.Shared.Messaging/
│   └── B2Connect.Shared.Search/
│
└── Tests/                  # 🧪 Test Projects
```

## Bounded Contexts erklärt

### 1. Store Context (Public Storefront)

**Verantwortung**: Öffentliche, read-only APIs für den Online-Shop

**Services**:
- **Catalog**: Produkte, Kategorien, Marken, Attribute
- **CMS**: Seiten, Komponenten, Content
- **Theming**: Designs, Layouts, Templates, Themes
- **Localization**: Mehrsprachigkeit, Übersetzungen
- **Search**: Volltextsuche (Elasticsearch)

**Charakteristik**:
- ✅ Read-Only (keine Schreiboperationen)
- ✅ Öffentlich zugänglich
- ✅ Hohe Performance (Caching)
- ✅ Skalierbar

**Frontend**: `frontend-store` (Port 5173)

---

### 2. Admin Context (Admin Operations)

**Verantwortung**: CRUD-Operationen, Verwaltung, Konfiguration

**Services**:
- **Admin API**: Zentrale Admin-Operationen
  - Produkt-Management (CRUD)
  - Content-Management (CRUD)
  - User-Management
  - Konfiguration

**Charakteristik**:
- ✅ Full CRUD
- ✅ JWT Authentication
- ✅ Role-Based Authorization
- ✅ Audit Logging

**Frontend**: `frontend-admin` (Port 5174)

---

### 3. Shared Context (Cross-Context)

**Verantwortung**: Services, die von mehreren Contexts genutzt werden

**Services**:
- **Identity**: Authentifizierung, User-Verwaltung
- **Tenancy**: Multi-Tenant Support, Mandanten-Isolation

**Charakteristik**:
- ✅ Kontext-übergreifend
- ✅ Wiederverwendbar
- ✅ Keine Business-Logik

---

## Onion Architecture (innerhalb jedes Service)

Jeder Service folgt der **Onion Architecture** mit 4 Schichten:

```
Service/
├── Core/                   # 🎯 Domain Layer (Innerster Ring)
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
└── Presentation/           # 🌐 API Layer (Äußerster Ring)
    ├── Controllers/        # REST Endpoints
    ├── Middleware/         # Custom Middleware
    ├── Configuration/      # Dependency Injection
    └── Program.cs          # Entry Point
```

### Dependency Flow (Onion Principle)

```
Presentation → Infrastructure → Application → Core
   (API)          (Data)         (Logic)      (Domain)
   
Abhängigkeiten zeigen IMMER nach INNEN!
Core hat KEINE Abhängigkeiten zu äußeren Schichten.
```

---

## DDD Patterns verwendet

### Aggregate Roots
- `Product` (Catalog)
- `CmsPage` (CMS)
- `Theme` (Theming)

### Repositories
- Ein Repository pro Aggregate Root
- Nur Interfaces in Core, Implementierung in Infrastructure

### Domain Events
- `ProductCreatedEvent`
- `PriceChangedEvent`
- `PagePublishedEvent`

### Value Objects
- `Price` (Amount + Currency)
- `SKU` (eindeutiger Produktcode)
- `LocalizedContent` (Text + Language)

### CQRS Pattern
- **Commands**: Schreiboperationen (Admin Context)
- **Queries**: Leseoperationen (Store Context)
- Trennung verbessert Performance und Skalierbarkeit

---

## Kommunikation zwischen Contexts

### Synchron (HTTP)
- Store → Shared (Identity für Token-Validierung)
- Admin → Shared (Identity, Tenancy)

### Asynchron (Events)
- Admin Context publiziert Events → Store Context reagiert
- Beispiel: `ProductUpdatedEvent` → Elasticsearch Reindex

### Message Bus
- **Wolverine** für In-Process Messaging
- **RabbitMQ/Azure Service Bus** für Microservices (optional)

---

## Vorteile dieser Struktur

### ✅ Klare Verantwortlichkeiten
- Jeder Bounded Context hat eigene Zuständigkeit
- Keine vermischte Business-Logik

### ✅ Skalierbarkeit
- Store Context kann horizontal skaliert werden
- Admin Context benötigt weniger Instanzen

### ✅ Wartbarkeit
- Onion Architecture erzwingt saubere Abhängigkeiten
- Core bleibt frei von Framework-Code

### ✅ Testbarkeit
- Domain-Logik (Core) ist isoliert testbar
- Mocking von Infrastructure einfach

### ✅ Deployment-Flexibilität
- Contexts können unabhängig deployed werden
- Microservices-ready

---

## Migration Checklist (status)

- [x] BoundedContexts Ordner erstellt
- [x] Services nach Contexts verschoben
- [x] Solution-Datei aktualisiert
- [x] Tasks.json aktualisiert
- [x] Namespaces angepasst (majority migrated to `B2Connect.Store.*` namespaces)
- [x] Project References updated (gateways & services reference updated projects)
- [ ] Orchestration (Aspire) review & tune remaining
- [ ] Tests: verify & re-baseline failing tests
- [ ] Dokumentation vervollständigen (link targets, examples)

---

## Nächste Schritte

1. **Namespaces standardisieren**:
   - `B2Connect.Store.Catalog.*`
   - `B2Connect.Admin.API.*`
   - `B2Connect.Shared.Identity.*`

2. **Onion-Layers verfeinern**:
   - Core/Application/Infrastructure/Presentation in jedem Service

3. **Tests reorganisieren**:
   - `Tests/Store/Catalog.Tests/`
   - `Tests/Admin/API.Tests/`

4. **CI/CD anpassen**:
   - Build-Pipelines pro Context
   - Separate Deployments

---

## Ressourcen

- [Onion Architecture](../ONION_ARCHITECTURE.md)
- [CQRS Pattern](../../docs/features/CQRS_INTEGRATION_POINT1.md)
- [DDD Patterns](https://martinfowler.com/bliki/DomainDrivenDesign.html)
