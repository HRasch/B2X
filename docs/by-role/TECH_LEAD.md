# 👔 Tech Lead / Architect - Documentation Guide

**Role:** Tech Lead / Architect | **P0 Components:** ALL (Oversight)  
**Time to Read:** ~6 hours | **Priority:** 🔴 CRITICAL

---

## 🎯 Your Mission

Als Tech Lead / Architect bist du verantwortlich für:
- **Architecture Decisions** (DDD, Microservices, Wolverine)
- **Code Quality** (Reviews, Standards, Patterns)
- **Technical Roadmap** (Prioritization, Dependencies)
- **Team Guidance** (Mentoring, Unblocking)
- **Cross-Team Coordination** (Security, DevOps, Legal)
- **Go/No-Go Technical Decisions**

---

## 📚 Required Reading (ALL P0)

### Week 1: Core Architecture

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 1 | **Copilot Instructions (FULL!)** | [copilot-instructions.md](../../.github/copilot-instructions.md) | 60 min |
| 2 | **Application Specifications** | [APPLICATION_SPECIFICATIONS.md](../APPLICATION_SPECIFICATIONS.md) | 45 min |
| 3 | **DDD Bounded Contexts** | [architecture/DDD_BOUNDED_CONTEXTS.md](../architecture/DDD_BOUNDED_CONTEXTS.md) | 30 min |
| 4 | **Onion Architecture** | [ONION_ARCHITECTURE.md](../ONION_ARCHITECTURE.md) | 30 min |
| 5 | **Wolverine HTTP Endpoints** | [api/WOLVERINE_HTTP_ENDPOINTS.md](../api/WOLVERINE_HTTP_ENDPOINTS.md) | 30 min |

### Week 2: Infrastructure & Security

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 6 | **Aspire Guide** | [architecture/ASPIRE_GUIDE.md](../architecture/ASPIRE_GUIDE.md) | 45 min |
| 7 | **Shared Authentication** | [architecture/SHARED_AUTHENTICATION.md](../architecture/SHARED_AUTHENTICATION.md) | 30 min |
| 8 | **Gateway Separation** | [api/GATEWAY_SEPARATION.md](../api/GATEWAY_SEPARATION.md) | 20 min |
| 9 | **Testing Framework** | [TESTING_FRAMEWORK_GUIDE.md](../TESTING_FRAMEWORK_GUIDE.md) | 30 min |

### Week 3: Compliance Deep Dive (ALL!)

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 10 | **EU Compliance Roadmap** | [compliance/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md](../compliance/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md) | 90 min |
| 11 | **P0.6 E-Commerce Tests** | [compliance/P0.6_ECOMMERCE_LEGAL_TESTS.md](../compliance/P0.6_ECOMMERCE_LEGAL_TESTS.md) | 20 min |
| 12 | **P0.7 AI Act Tests** | [compliance/P0.7_AI_ACT_TESTS.md](../compliance/P0.7_AI_ACT_TESTS.md) | 20 min |
| 13 | **P0.8 BITV Tests** | [compliance/P0.8_BARRIEREFREIHEIT_BITV_TESTS.md](../compliance/P0.8_BARRIEREFREIHEIT_BITV_TESTS.md) | 15 min |
| 14 | **P0.9 E-Rechnung Tests** | [compliance/P0.9_ERECHNUNG_TESTS.md](../compliance/P0.9_ERECHNUNG_TESTS.md) | 15 min |
| 15 | **Compliance Testing** | [compliance/COMPLIANCE_TESTING_EXAMPLES.md](../compliance/COMPLIANCE_TESTING_EXAMPLES.md) | 20 min |

### Week 4: Supporting Documentation

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 16 | **All Other Guides** | [guides/*](../guides/) | 60 min |
| 17 | **Role Documentation Map** | [ROLE_BASED_DOCUMENTATION_MAP.md](../ROLE_BASED_DOCUMENTATION_MAP.md) | 20 min |

---

## 🏗️ Architecture Overview

### System Context

```
┌─────────────────────────────────────────────────────────────────┐
│ B2Connect SaaS Platform                                         │
│ Multi-Tenant, EU-Only, 100+ Shops, 1000+ Users/Shop             │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────┐    ┌─────────────────┐                     │
│  │ Frontend Store  │    │ Frontend Admin  │                     │
│  │ Vue.js (5173)   │    │ Vue.js (5174)   │                     │
│  └────────┬────────┘    └────────┬────────┘                     │
│           │                      │                               │
│           ▼                      ▼                               │
│  ┌─────────────────────────────────────────┐                    │
│  │ API Gateway (YARP)                      │                    │
│  │ - Rate Limiting                         │                    │
│  │ - JWT Validation                        │                    │
│  │ - Tenant Routing                        │                    │
│  └─────────────────────────────────────────┘                    │
│           │                                                      │
│           ▼                                                      │
│  ┌─────────────────────────────────────────┐                    │
│  │ Microservices (Wolverine HTTP)          │                    │
│  │                                          │                    │
│  │ ┌──────────┐ ┌──────────┐ ┌──────────┐  │                    │
│  │ │ Identity │ │ Catalog  │ │   CMS    │  │                    │
│  │ │  (7002)  │ │  (7005)  │ │  (7006)  │  │                    │
│  │ └──────────┘ └──────────┘ └──────────┘  │                    │
│  │                                          │                    │
│  │ ┌──────────┐ ┌──────────┐ ┌──────────┐  │                    │
│  │ │ Tenancy  │ │ Theming  │ │  Search  │  │                    │
│  │ │  (7003)  │ │  (7008)  │ │  (9300)  │  │                    │
│  │ └──────────┘ └──────────┘ └──────────┘  │                    │
│  │                                          │                    │
│  │ ┌──────────┐                             │                    │
│  │ │ Local-   │                             │                    │
│  │ │ ization  │                             │                    │
│  │ │  (7004)  │                             │                    │
│  │ └──────────┘                             │                    │
│  └─────────────────────────────────────────┘                    │
│           │                                                      │
│           ▼                                                      │
│  ┌─────────────────────────────────────────┐                    │
│  │ Data Layer                              │                    │
│  │ ┌──────────┐ ┌──────────┐ ┌──────────┐  │                    │
│  │ │PostgreSQL│ │  Redis   │ │Elastic-  │  │                    │
│  │ │  (5432)  │ │  (6379)  │ │ search   │  │                    │
│  │ └──────────┘ └──────────┘ │  (9200)  │  │                    │
│  │                           └──────────┘  │                    │
│  └─────────────────────────────────────────┘                    │
│                                                                 │
│  Orchestration: Aspire (Dashboard: 15500)                       │
└─────────────────────────────────────────────────────────────────┘
```

### Onion Architecture (per Service)

```
┌─────────────────────────────────────────────────────────┐
│ Presentation (API Layer)                               │
│   - Program.cs (Entry point)                           │
│   - Wolverine Endpoints (auto-discovered)              │
│   - Middleware (Auth, Tenant, Logging)                 │
├─────────────────────────────────────────────────────────┤
│ Infrastructure Layer                                    │
│   - EF Core DbContext                                  │
│   - Repository Implementations                         │
│   - External Services (ERP, Email, etc.)               │
│   - Caching (Redis)                                    │
├─────────────────────────────────────────────────────────┤
│ Application Layer                                       │
│   - DTOs (Data Transfer Objects)                       │
│   - Handlers (Wolverine Services - NOT MediatR!)       │
│   - Validators (FluentValidation)                      │
│   - Mappers (AutoMapper)                               │
├─────────────────────────────────────────────────────────┤
│ Domain Layer (Core) - INNERMOST                        │
│   - Entities (Product, User, Order)                    │
│   - Value Objects (Price, SKU, Email)                  │
│   - Interfaces (IProductRepository)                    │
│   - Domain Events (ProductCreatedEvent)                │
│   - NO framework dependencies!                         │
└─────────────────────────────────────────────────────────┘

Dependencies: Outer → Inner (never reverse!)
```

---

## 📊 P0 Component Dependency Graph

```
                    ┌─────────────┐
                    │   P0.5      │
                    │ Key Mgmt    │
                    └──────┬──────┘
                           │
         ┌─────────────────┼─────────────────┐
         │                 │                 │
         ▼                 ▼                 ▼
   ┌─────────────┐  ┌─────────────┐  ┌─────────────┐
   │   P0.2      │  │   P0.1      │  │   P0.3      │
   │ Encryption  │  │ Audit Log   │  │ Incident    │
   │             │  │             │  │ Response    │
   └──────┬──────┘  └──────┬──────┘  └──────┬──────┘
          │                │                │
          └────────────────┼────────────────┘
                           │
                           ▼
                    ┌─────────────┐
                    │   P0.4      │
                    │ Network     │
                    │ Segment     │
                    └──────┬──────┘
                           │
    ┌──────────────────────┼──────────────────────┐
    │                      │                      │
    ▼                      ▼                      ▼
┌─────────┐         ┌─────────────┐        ┌─────────────┐
│  P0.6   │         │   P0.7      │        │   P0.9      │
│E-Commerce│         │  AI Act     │        │ E-Rechnung  │
└─────────┘         └─────────────┘        └─────────────┘
    │
    ▼
┌─────────┐
│  P0.8   │
│  BITV   │
└─────────┘

Legend:
─────► = Depends on
```

### Implementation Order (Recommended)

1. **Week 1-2:** P0.5 Key Management (foundation for encryption)
2. **Week 2-3:** P0.2 Encryption (needs keys)
3. **Week 3-4:** P0.1 Audit Logging (needs encryption)
4. **Week 4-5:** P0.3 Incident Response (needs audit logs)
5. **Week 5-6:** P0.4 Network Segmentation (parallel)
6. **Week 6-7:** P0.6 E-Commerce (business features)
7. **Week 7-8:** P0.9 E-Rechnung (business features)
8. **Week 8-9:** P0.7 AI Act (HIGH-RISK systems)
9. **Week 9-10:** P0.8 BITV (accessibility - CRITICAL DEADLINE!)

---

## 🔧 Architecture Decision Records (ADRs)

### ADR-001: Wolverine over MediatR

**Status:** Accepted  
**Context:** Need CQRS pattern for handlers  
**Decision:** Use Wolverine, not MediatR  
**Consequences:**
- ✅ Better distributed system support
- ✅ Built-in HTTP endpoint discovery
- ✅ Event-driven messaging
- ⚠️ Less community documentation
- ⚠️ Team learning curve

**Reference:** [WOLVERINE_ARCHITECTURE_ANALYSIS.md](../../WOLVERINE_ARCHITECTURE_ANALYSIS.md)

### ADR-002: Onion Architecture

**Status:** Accepted  
**Context:** Need clean separation of concerns  
**Decision:** Onion Architecture with 4 layers  
**Consequences:**
- ✅ Domain layer has no dependencies
- ✅ Easy to test business logic
- ✅ Clear dependency direction
- ⚠️ More boilerplate code
- ⚠️ Stricter developer discipline needed

### ADR-003: Aspire for Orchestration

**Status:** Accepted  
**Context:** Need local development orchestration  
**Decision:** .NET Aspire over Docker Compose  
**Consequences:**
- ✅ Native .NET integration
- ✅ Built-in dashboard
- ✅ Service discovery
- ⚠️ Less flexible than K8s
- ⚠️ Windows/macOS port issues (documented)

### ADR-004: PostgreSQL as Primary Database

**Status:** Accepted  
**Context:** Need multi-tenant database  
**Decision:** PostgreSQL 16 with tenant isolation  
**Consequences:**
- ✅ Excellent JSON support
- ✅ Strong encryption features
- ✅ Cost-effective at scale
- ⚠️ Team more familiar with SQL Server

---

## 📋 Code Review Checklist (Tech Lead)

### Architecture Review
- [ ] Onion Architecture respected (dependencies inward)
- [ ] Wolverine pattern used (NOT MediatR)
- [ ] Domain entities have no framework dependencies
- [ ] Repository interface in Core, implementation in Infrastructure
- [ ] DTOs used for API boundaries
- [ ] No circular dependencies

### Security Review
- [ ] No hardcoded secrets
- [ ] All PII encrypted
- [ ] Tenant isolation in all queries
- [ ] Audit logging for data changes
- [ ] Input validation with FluentValidation
- [ ] Proper error handling (no stack traces in responses)

### Quality Review
- [ ] Tests written (80%+ coverage target)
- [ ] Code compiles without warnings
- [ ] XML documentation for public APIs
- [ ] No TODO comments in main branch
- [ ] Async/await used consistently
- [ ] CancellationToken passed through

### Compliance Review
- [ ] Relevant P0 component addressed
- [ ] Acceptance criteria met
- [ ] Legal review completed (if required)
- [ ] Accessibility checked (if UI)

---

## 📞 Escalation Path

| Issue | Contact | SLA |
|-------|---------|-----|
| Architecture Question | Tech Lead (self) | - |
| Security Concern | Security Engineer | < 1h |
| Compliance Question | Legal Officer | < 24h |
| Resource Conflict | Product Owner | < 24h |
| Infrastructure Issue | DevOps Engineer | < 2h |
| Go/No-Go Decision | C-Level | 48h |

---

## ✅ Definition of Done (Tech Lead)

Before approving any phase gate:

- [ ] All architecture requirements met
- [ ] All security requirements met
- [ ] All compliance tests passing
- [ ] Code coverage >= 80%
- [ ] No critical/high bugs open
- [ ] Performance acceptable (< 200ms P95)
- [ ] Documentation complete
- [ ] Team trained on new patterns
- [ ] Rollback plan documented

---

## 🎯 Quick Decision Matrix

| Question | Answer |
|----------|--------|
| MediatR or Wolverine? | **Wolverine** |
| InMemory or PostgreSQL for dev? | **InMemory** (fast), PostgreSQL (production-like) |
| Soft delete or hard delete? | **Soft delete** (compliance) |
| Sync or async service calls? | **Async** (Wolverine events) |
| REST or GraphQL? | **REST** (Wolverine HTTP) |
| Monolith or Microservices? | **Microservices** (DDD bounded contexts) |
| Azure or AWS? | **Either** (cloud-agnostic with Aspire) |
| Vue 2 or Vue 3? | **Vue 3** (Composition API) |

---

**Next:** Start with [copilot-instructions.md](../../.github/copilot-instructions.md) (FULL document!)
