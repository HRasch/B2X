# 📚 DDD Bounded Contexts Reference

**Audience**: All backend developers  
**Purpose**: Where services live, how contexts communicate, architectural constraints  
**Critical**: Clear boundaries prevent cross-cutting concerns and data inconsistency

---

## Context Map (8 Bounded Contexts)

```
┌─────────────────────────────────────────────────────────┐
│                    B2X Platform                    │
└─────────────────────────────────────────────────────────┘
            │
    ┌───────┴───────┐
    │               │
┌───▼──┐        ┌──▼────┐
│Store │        │ Admin  │
│(Read)│        │(CRUD)  │
└──┬───┘        └─────┬──┘
   │                  │
   │    ┌─────────────┤
   │    │             │
   └────┼─────┬───────┴──────┐
        │     │              │
    ┌───▼──┐  │    ┌────────▼─┐
    │Shared│◄─┴────┤ Domain   │
    │(Core)│       │ Services │
    └──────┘       └──────────┘

Shared: Identity, Tenancy (cross-context)
Store: Read-only APIs (Catalog, CMS, Search, Theming)
Admin: Write operations (CRUD, config)
Domain: Business logic (Catalog, CMS, Search, etc.)
```

---

## 1. Store Context (Public Storefront)

### Responsibility
Public, read-only APIs for online shop. Optimized for performance & scale.

### Services

#### 📦 Catalog Service
```
Location: backend/Domain/Catalog/
Port: 6001 (via gateway 6000)
Database: PostgreSQL (shared)
Cache: Redis

Entities:
- Product (aggregate root)
- Category (aggregate root)
- Brand
- Attribute
- SKU (value object)
- Price (value object)

Key Operations:
✅ GetProducts (paginated, filtered)
✅ GetProductDetail (with reviews)
✅ SearchProducts (full-text via Elasticsearch)
✅ ❌ CreateProduct (NO - use Admin context)

Events Published:
→ ProductCreatedEvent (from Admin)
→ ProductUpdatedEvent (from Admin)
→ PriceChangedEvent (from Admin)

Events Subscribed:
← ProductCreatedEvent (for Search indexing)
← ProductUpdatedEvent (for Search re-indexing)
```

#### 📝 CMS Service
```
Location: backend/Domain/CMS/
Port: 6001 (via gateway 6000)
Database: PostgreSQL

Entities:
- Page (aggregate root)
- Component (value object)
- Section (value object)
- Navigation
- Menu

Key Operations:
✅ GetPage (by slug)
✅ GetPages (with hierarchy)
✅ ❌ CreatePage (NO - use Admin context)

Events Subscribed:
← PagePublishedEvent (from Admin)
← PageUpdatedEvent (from Admin)
```

#### 🎨 Theming Service
```
Location: backend/Domain/Theming/
Port: 6001 (via gateway 6000)
Database: PostgreSQL

Entities:
- Theme (aggregate root)
- Layout
- Component
- Template

Key Operations:
✅ GetActiveTheme
✅ GetThemeAssets
✅ ❌ CreateTheme (NO - use Admin context)

Architectural Constraint:
→ Performance: Theme assets must be cached (Redis TTL: 1h)
→ Consistency: Theme changes don't require Search reindex
```

#### 🌍 Localization Service
```
Location: backend/Domain/Localization/
Port: 6001 (via gateway 6000)
Database: PostgreSQL

Entities:
- Locale (aggregate root)
- Translation (value object)
- LanguagePack

Key Operations:
✅ GetTranslations (by locale, namespace)
✅ GetSupportedLocales
✅ ❌ UpdateTranslations (NO - use Admin context)

Architectural Constraint:
→ Performance: Translations must be in-memory cache
→ Consistency: Changes propagate via event (ContentTranslatedEvent)
```

#### 🔍 Search Service (Elasticsearch)
```
Location: backend/Domain/Search/
Port: 6001 (via gateway 6000)
Index: Elasticsearch 8.x

Entities:
- ProductIndex (aggregate root)
- CategoryIndex
- ContentIndex

Key Operations:
✅ SearchProducts (full-text)
✅ SearchContent (pages, blogs)
✅ GetFacets (for filtering)
✅ ❌ Index (NO - automated via events)

Event Subscribers (Auto-indexed):
← ProductCreatedEvent
← ProductUpdatedEvent
← PriceChangedEvent
← ProductDeletedEvent
← PagePublishedEvent
← PageUpdatedEvent

Architectural Constraint:
→ Consistency: Search lags 1-5 seconds behind writes (eventual consistency)
→ Performance: Bulk indexing every 10 minutes
→ Scalability: Index sharded by language + region
```

### Store Context Constraints

| Constraint | Reason | Impact |
|-----------|--------|--------|
| **Read-only** | Prevent data inconsistency | All writes go through Admin context |
| **Eventually consistent** | High performance | Search lags 1-5s behind |
| **Cached** | Reduce DB load | Cache TTL: 1h per service |
| **Horizontally scalable** | Support traffic spikes | Stateless design |

---

## 2. Admin Context (Admin Operations)

### Responsibility
Full CRUD operations, configuration, management. Smaller scale, higher security.

### Services

#### 🔧 Admin API
```
Location: backend/Gateway/Admin/
Port: 6100
Database: PostgreSQL (shared with Store)

Secured By: JWT + Role-Based Authorization

Endpoints:

# Product Management
POST   /api/admin/products              (CreateProduct)
PUT    /api/admin/products/{id}         (UpdateProduct)
DELETE /api/admin/products/{id}         (DeleteProduct)
GET    /api/admin/products              (ListProducts for admin)

# Content Management
POST   /api/admin/cms/pages             (CreatePage)
PUT    /api/admin/cms/pages/{id}        (UpdatePage)
DELETE /api/admin/cms/pages/{id}        (DeletePage)

# Theming
POST   /api/admin/themes                (CreateTheme)
PUT    /api/admin/themes/{id}           (UpdateTheme)

# User Management
POST   /api/admin/users                 (CreateUser)
PUT    /api/admin/users/{id}            (UpdateUser)
DELETE /api/admin/users/{id}            (DeleteUser)

# Configuration
PUT    /api/admin/config/{key}          (UpdateConfig)
GET    /api/admin/config                (GetAllConfig)

Events Published:
→ ProductCreatedEvent (triggers Store indexing)
→ ProductUpdatedEvent
→ PriceChangedEvent
→ PagePublishedEvent
→ PageUpdatedEvent
→ UserCreatedEvent
→ UserUpdatedEvent
→ UserDeletedEvent
```

### Admin Context Constraints

| Constraint | Reason | Impact |
|-----------|--------|--------|
| **JWT Auth** | Security | All requests require valid token |
| **RBAC** | Fine-grained control | Roles: Admin, Editor, Manager |
| **Audit logging** | Compliance | Every change logged with user + timestamp |
| **Rate limited** | Prevent abuse | 100 req/min per user |

---

## 3. Shared Context (Cross-Context Services)

### Services

#### 🔐 Identity Service
```
Location: backend/Domain/Identity/
Database: PostgreSQL

Entities:
- User (aggregate root)
- Role
- Permission
- Session

Key Operations:
✅ Authenticate (login)
✅ ValidateToken (JWT)
✅ GetUser (by ID or email)
✅ CreateUser (from Admin or self-registration)
✅ UpdateUser (profile update)

Used By:
← Store (token validation for caching headers)
← Admin (token validation for all requests)
← All services (user context in events)

Architectural Constraint:
→ Single source of truth for all users
→ Token validation must be <100ms (cached)
```

#### 👥 Tenancy Service
```
Location: backend/Domain/Tenancy/
Database: PostgreSQL

Entities:
- Tenant (aggregate root)
- TenantUser (relationship)
- TenantConfig

Key Operations:
✅ GetTenant (by ID or domain)
✅ GetTenantConfig
✅ ValidateUserInTenant
✅ ListTenantsForUser

Used By:
← Store (identify tenant from domain)
← Admin (tenant isolation)
← All services (scope data by tenant)

Architectural Constraint:
→ All queries must include TenantId
→ Data isolation: One tenant cannot access another's data
→ Performance: Tenant resolution <50ms
```

---

## 4. Domain Services (Cross-context Utilities)

Not full contexts, but important services used by multiple contexts:

### Shared Libraries
```
Location: backend/shared/
├── B2X.Shared.Core/            # Domain kernel
│   ├── Entities/                    # Base entity classes
│   ├── ValueObjects/                # Price, SKU, etc.
│   ├── Events/                      # DomainEvent base
│   └── Exceptions/                  # DomainException, etc.
│
├── B2X.Shared.Infrastructure/   # Cross-context infra
│   ├── Repositories/                # Generic repository base
│   ├── Caching/                     # Redis helpers
│   ├── Messaging/                   # Event bus wrapper
│   └── Data/                        # Shared migrations
│
├── B2X.Shared.Messaging/        # Wolverine integration
│   ├── EventBus/                    # Wolverine IMessageBus
│   ├── Handlers/                    # Base handler patterns
│   └── Configuration/               # Wolverine setup
│
└── B2X.Shared.Search/           # Elasticsearch
    ├── Client/                      # ES client wrapper
    ├── Indexing/                    # Index management
    └── Queries/                     # Search builders
```

---

## Communication Patterns

### Synchronous (HTTP)
```
Store Context:
┌─────────────┐
│   Store     │  token validation
│  Catalog    │──────────────────────→ Shared
│   Service   │  ✓ token valid         Identity
└─────────────┘                        Service
                 ← "user data"

Admin Context:
┌─────────────┐
│   Admin     │  token validation
│    API      │──────────────────────→ Shared
└─────────────┘                        Identity
                 ← "user + roles"
```

### Asynchronous (Events via Wolverine)
```
Admin Context:
┌──────────────┐
│ Admin API    │ publishes ProductCreatedEvent
│ (ProductCmd) │────────────────────────────→ Wolverine Event Bus
└──────────────┘
                                          ↓
                                   ┌──────────────┐
                                   │ Store Search │
                                   │   Service    │
                                   │ (Subscriber) │
                                   │ [Re-indexes] │
                                   └──────────────┘

Events Flow (One-way, Asynchronous):
ProductCreatedEvent ──→ [Search re-index]
ProductUpdatedEvent ──→ [Search re-index]
PriceChangedEvent   ──→ [Update cache, Search re-index]
PagePublishedEvent  ──→ [Search index]
```

---

## Service Placement Matrix

Use this to decide where a new service goes:

```
                       Write Operations?
                         Yes      No
                        ----    ------
    Read-only          Admin   Store
    (Scale for         (CRUD)  (API)
     millions)
    
    Cross-context      Shared  Shared
    (Identity,         (Write) (Read)
     Tenancy)
```

### Examples

**Q: New "Reviews" feature?**
```
Write operations: ✓ Yes (users write reviews)
Read-only: ✗ No (Store must show reviews)
→ Decision: Split service
   - AdminReviewService: Create, Delete (Admin context)
   - StoreReviewService: Read (Store context)
   - Events: ReviewCreatedEvent (Admin → Store indexing)
```

**Q: New "Recommendations" feature?**
```
Write operations: ✗ No (AI-generated, async job)
Read-only: ✓ Yes (only reads product data)
→ Decision: Store context
   - RecommendationService: Read products, compute scores
   - Eventual consistency: Cache for 1h
```

**Q: New "User Preferences" feature?**
```
Write operations: ✓ Yes (users update preferences)
Read-only: ✗ No
Cross-context: ✓ Yes (Admin + Store both need it)
→ Decision: Shared context
   - Shared.UserPreferences: CRUD for all
   - Used by: Store (for personalization), Admin (for management)
```

---

## Architectural Constraints

### Performance
- **Store read operations**: <200ms P99
- **Admin CRUD**: <500ms P99
- **Identity validation**: <100ms P99
- **Search queries**: <100ms P99

### Consistency
- **Immediate consistency**: Within same service
- **Eventual consistency**: Across contexts (1-5 seconds via events)
- **Data isolation**: Tenant data never crosses tenant boundary

### Scalability
- **Store**: Horizontal (stateless), 10+ instances
- **Admin**: Vertical (smaller scale), 2-4 instances
- **Shared**: Horizontal (caching critical), 4-8 instances

### Security
- **Admin**: JWT + RBAC
- **Store**: Optional JWT for personalization, public read
- **Shared**: No direct exposure, accessed only via other services

---

## Deployment Independence

Each context can be deployed independently:

```yaml
Store Context:
  - backend/Domain/ (Catalog, CMS, Theming, Localization, Search)
  - Port: 6000 (gateway), 6001 (services)
  - Deploy: Any time (read-only)

Admin Context:
  - backend/Gateway/Admin/
  - Port: 6100
  - Deploy: Any time (backward compatible)

Cross-Context Services:
  - backend/Domain/Identity/ & Tenancy/
  - Database schema changes: Coordinate with all contexts
  - Breaking changes: Require full deployment
```

---

## Adding a New Service Checklist

- [ ] Identify bounded context (Store, Admin, Shared)
- [ ] Define aggregate roots (main entities)
- [ ] Identify value objects (immutable data)
- [ ] Plan domain events
- [ ] Choose sync/async communication
- [ ] Map to Port number
- [ ] Add to Domain/ structure
- [ ] Update csproj solution file
- [ ] Add to Aspire orchestration
- [ ] Update .github/tasks.json
- [ ] Document context boundaries

---

## References

- [Wolverine Pattern Reference](WOLVERINE_PATTERN_REFERENCE.md)
- [ERROR_HANDLING_PATTERNS](../INDEX.md)
- [FEATURE_IMPLEMENTATION_PATTERNS](../patterns/FEATURE_IMPLEMENTATION_PATTERNS.md)
- [Aspire Orchestration Reference](../../../docs/architecture/INDEX.md)

---

*Updated: 30. Dezember 2025*  
*Source: docs/architecture/DDD_BOUNDED_CONTEXTS.md + team architecture decisions*
