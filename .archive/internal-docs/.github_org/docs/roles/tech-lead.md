# 👔 Tech Lead Quick Start

**Role Focus:** Architecture decisions, code review, standards, cross-team coordination  
**Time to Productivity:** 1 week  
**Critical Components:** ALL (oversight role)

---

## ⚡ Week 1: Architecture Overview

### Day 1: Core Architecture
```
B2Connect Architecture:
  - DDD (Domain-Driven Design) with bounded contexts
  - Microservices: Identity, Catalog, CMS, Theming, Localization, Search
  - Onion Architecture per service (Domain → Application → Infrastructure → API)
  - Wolverine for HTTP endpoints (NOT MediatR!)
  - Aspire orchestration (local development)
```

**Critical Rule:** Wolverine, not MediatR
```csharp
// ✅ CORRECT: Wolverine Service
public class CheckRegistrationTypeService {
    public async Task<CheckRegistrationTypeResponse> CheckType(
        CheckRegistrationTypeCommand request,
        CancellationToken cancellationToken) { }
}

// ❌ WRONG: MediatR (DO NOT USE)
public record CheckRegistrationTypeCommand : IRequest<CheckRegistrationTypeResponse>;
public class Handler : IRequestHandler<CheckRegistrationTypeCommand, ...> { }
```

### Day 2: Service Architecture
```
Each Microservice:
├── Core/                 # Domain layer (no dependencies!)
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Interfaces/
│   └── Events/
├── Application/          # CQRS & business logic
│   ├── Handlers/
│   ├── Validators/
│   └── Mappers/
├── Infrastructure/       # EF Core, external services
│   ├── Repositories/
│   └── Data/
└── Presentation/         # API layer
    └── Controllers/
```

### Day 3: Code Quality Standards
```
Before Every Commit:
  ✅ No hardcoded secrets
  ✅ Wolverine pattern (not MediatR)
  ✅ Tests written (80%+ coverage)
  ✅ No circular dependencies
  ✅ Tenant ID in all queries
  ✅ Audit logging for data changes
  ✅ FluentValidation for inputs
  ✅ Async/await consistently
  ✅ CancellationToken passed through
```

### Day 4: Code Review Checklist
```
Architecture:
  □ Onion Architecture respected
  □ Domain layer has zero framework dependencies
  □ Repository interfaces in Core, implementations in Infrastructure
  □ Clear separation of concerns

Security:
  □ No hardcoded secrets (check: password, secret, key)
  □ PII fields encrypted (Email, Phone, Address, SSN)
  □ Tenant isolation in all queries
  □ Input validation with FluentValidation
  □ Audit logging for CRUD operations

Quality:
  □ Tests written (xUnit)
  □ Coverage > 80%
  □ Code compiles without warnings
  □ XML documentation for public APIs
  □ No TODO comments in main branch
```

### Day 5: Architecture Decisions
```
Key Decisions (ADRs):

ADR-001: Wolverine over MediatR
  ✅ Better for distributed systems
  ✅ Built-in HTTP endpoint discovery
  ✅ Event-driven messaging
  ⚠️ Less community support

ADR-002: Onion Architecture
  ✅ Domain-driven
  ✅ Testable core logic
  ✅ Clear dependency direction
  ⚠️ More boilerplate

ADR-003: Aspire for Orchestration
  ✅ Native .NET integration
  ✅ Service discovery
  ✅ Dashboard (localhost:15500)
  ⚠️ Less flexible than Kubernetes
```

---

## ⚡ Quick Commands

```bash
# Build & test
dotnet build B2Connect.slnx              # Build solution
dotnet test B2Connect.slnx -v minimal    # Run all tests
dotnet test --filter "Category=Compliance"  # Compliance tests

# Code review
dotnet analyze                            # Static analysis
dotnet format --verify-no-changes        # Check formatting

# Start services
cd AppHost && dotnet run   # Start Aspire (localhost:15500)
./scripts/kill-all-services.sh           # Kill stuck processes
./scripts/check-ports.sh                 # Verify ports available

# Specific service tests
dotnet test backend/Domain/Identity/tests/B2Connect.Identity.Tests.csproj
dotnet test backend/Domain/Catalog/tests/B2Connect.Catalog.Tests.csproj
```

---

## 📚 Critical Documentation

| Document | Purpose | Priority |
|----------|---------|----------|
| `copilot-instructions.md` | AI coding patterns (FULL READ) | 🔴 CRITICAL |
| `docs/DDD_BOUNDED_CONTEXTS.md` | Service architecture | 🔴 CRITICAL |
| `docs/ONION_ARCHITECTURE.md` | Layer structure | 🔴 CRITICAL |
| `docs/WOLVERINE_HTTP_ENDPOINTS.md` | Endpoint patterns | 🔴 CRITICAL |
| `docs/APPLICATION_SPECIFICATIONS.md` | Full requirements | 🟡 HIGH |
| `docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md` | All P0 components | 🟡 HIGH |

---

## 🔐 P0 Components (Your Oversight)

| Component | Owner | Status | Go/No-Go |
|-----------|-------|--------|----------|
| P0.1: Audit Logging | Security Eng | ⏳ | Required before Phase 1 |
| P0.2: Encryption | Security Eng | ⏳ | Required before Phase 1 |
| P0.3: Incident Response | DevOps | ⏳ | Required before Phase 1 |
| P0.4: Network | DevOps | ⏳ | Required before Phase 1 |
| P0.5: Keys | DevOps | ⏳ | Required before Phase 1 |
| P0.6: E-Commerce | Backend | ⏳ | Required for launch |
| P0.7: AI Act | Backend + Security | ⏳ | Required for launch |
| P0.8: BITV | Frontend | ⏳ DEADLINE 28. Juni! | Required before launch |
| P0.9: E-Rechnung | Backend | ⏳ | Required for launch |

**Go/No-Go Gate Before Phase 1:**
```
✅ Phase 0 completion checklist:
  □ All P0.1-P0.5 components implemented
  □ Security review passed
  □ Legal review passed
  □ No critical bugs open
  □ Code coverage > 80%

IF ANY ❌ → HOLD all Phase 1 deployments
```

---

## 🎯 Approval Responsibilities

As Tech Lead, you approve:

1. **Architecture Changes:**
   - New bounded contexts
   - Database schema changes
   - External service integrations
   - Infrastructure changes

2. **Code Quality:**
   - Team code review standards
   - Refactoring decisions
   - Technical debt prioritization

3. **Security:**
   - Encryption implementation
   - Audit logging design
   - Secret management
   - API security patterns

4. **Compliance:**
   - P0 component implementation
   - Legal compliance features
   - Accessibility requirements
   - Test coverage targets

5. **Go/No-Go Decisions:**
   - Phase 0 completion
   - Phase 1 launch readiness
   - Production deployment
   - Major releases

---

## 🚨 Common Review Issues

| Issue | Fix | Approval |
|-------|-----|----------|
| MediatR instead of Wolverine | Refactor to Wolverine | Reject PR |
| No tenant ID in query | Add tenant filter | Reject PR |
| Hardcoded secrets | Move to KeyVault | Reject PR |
| Missing tests | Write xUnit tests | Reject PR |
| No audit logging | Add SaveChangesInterceptor | Reject PR |
| Performance > 200ms | Optimize with caching | Review required |

---

## 📞 Escalation Path

| Issue | Contact | SLA |
|-------|---------|-----|
| Architecture question | Self (Tech Lead) | - |
| Security concern | Security Engineer | < 1h |
| Compliance blocker | Legal Officer | < 24h |
| Performance issue | DevOps | < 4h |
| Code quality | Team in standup | Next sprint |

---

**Key Reminders:**
- Wolverine, not MediatR!
- Domain layer: zero framework dependencies
- Every feature = unit test + integration test
- Tenant ID in ALL queries (non-negotiable)
- Audit logging for CRUD operations
- Phase 0 = blocking gate before Phase 1
- BITV deadline = 28. Juni 2025 (non-negotiable)
