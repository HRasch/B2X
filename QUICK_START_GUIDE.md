# B2Connect Quick Start Guide

**Project Status:** Multi-tenant SaaS E-Commerce Platform  
**Architecture:** DDD Microservices + Vue.js 3 + Wolverine  
**Last Updated:** 28. Dezember 2025

---

## 🚀 First Steps (Choose Your Role)

### 👨‍💻 I'm a Backend Developer
**Start here:** [BACKEND_DEVELOPER.md](docs/by-role/BACKEND_DEVELOPER.md)
```
Week 1: Learn Wolverine patterns (NOT MediatR!)
  → copilot-instructions.md (FULL document)
  → WOLVERINE_HTTP_ENDPOINTS.md
  → CQRS_WOLVERINE_PATTERN.md
  
Week 2: Learn onion architecture
  → ONION_ARCHITECTURE.md
  → DDD_BOUNDED_CONTEXTS.md
  
Week 3: Implement P0 compliance features
  → P0.6 E-Commerce Legal
  → P0.9 E-Rechnung
  → AUDIT_LOGGING_IMPLEMENTATION.md
```

### 🎨 I'm a Frontend Developer
**Start here:** [FRONTEND_DEVELOPER.md](docs/by-role/FRONTEND_DEVELOPER.md)
```
Week 1: Vue.js 3 + Tailwind CSS
  → FRONTEND_FEATURE_INTEGRATION_GUIDE.md
  → TAILWIND_BEST_PRACTICES.md (in copilot-instructions.md)
  
Week 2: Accessibility (CRITICAL - DEADLINE 28. JUNI 2025!)
  → P0.8_BARRIEREFREIHEIT_BITV_TESTS.md
  → WCAG 2.1 Level AA compliance
  
Week 3: E-Commerce UI
  → P0.6_ECOMMERCE_LEGAL_TESTS.md
  → Price display, returns, terms & conditions
```

### 🔐 I'm a Security Engineer
**Start here:** [SECURITY_ENGINEER.md](docs/by-role/SECURITY_ENGINEER.md)
```
Week 1: Security foundation
  → APPLICATION_SPECIFICATIONS.md (Security section)
  → PENTESTER_REVIEW.md
  → Encryption patterns
  
Week 2: Compliance implementation
  → EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md (P0.1-P0.5)
  → P0.7_AI_ACT_TESTS.md
  
Week 3: Implementation
  → AUDIT_LOGGING_IMPLEMENTATION.md
  → AES-256 encryption service
```

### ⚙️ I'm a DevOps Engineer
**Start here:** [DEVOPS_ENGINEER.md](docs/by-role/DEVOPS_ENGINEER.md)
```
Week 1: Aspire orchestration
  → architecture/ASPIRE_GUIDE.md
  → PORT_BLOCKING_SOLUTION.md
  → ASPIRE_DASHBOARD_TROUBLESHOOTING.md
  
Week 2: Compliance infrastructure
  → EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md (P0.3-P0.5)
  → Network segmentation
  → Incident response setup
  
Week 3: CI/CD & Monitoring
  → GITHUB_WORKFLOWS.md
```

### 🧪 I'm a QA Engineer
**Start here:** [QA_ENGINEER.md](docs/by-role/QA_ENGINEER.md)
```
52 Compliance Tests Total:
  - P0.6: E-Commerce Legal (15 tests)
  - P0.7: AI Act (15 tests)
  - P0.8: Accessibility/BITV (12 tests)
  - P0.9: E-Rechnung (10 tests)

Quick Commands:
  dotnet test --filter "Category=Compliance"
  npm run test
  npm run test:e2e
```

### 👔 I'm a Tech Lead / Architect
**Start here:** [TECH_LEAD.md](docs/by-role/TECH_LEAD.md)
```
Review all P0 documentation + architecture.
Architecture decisions:
  - Wolverine (not MediatR)
  - DDD with bounded contexts
  - Onion architecture (4 layers)
  - PostgreSQL per service
  - Redis for caching
  - Elasticsearch for search
```

### 📋 I'm a Product Owner
**Start here:** [PRODUCT_OWNER.md](docs/by-role/PRODUCT_OWNER.md)
```
Phase 0: Compliance Foundation (6 weeks, parallel)
  → P0.1-P0.9 components
  
Phase 1: MVP + Compliance (8 weeks)
  → Auth, Catalog, Orders, Admin Dashboard
  
Phase 2: Scale (10 weeks)
  → Database replication, Redis cluster, auto-scaling
  
Phase 3: Production (6 weeks)
  → Load testing, chaos engineering, compliance audit
```

### ⚖️ I'm a Legal/Compliance Officer
**Start here:** [LEGAL_COMPLIANCE.md](docs/by-role/LEGAL_COMPLIANCE.md)
```
Key Regulations:
  - NIS2 (Incident response < 24h)
  - GDPR (Data protection)
  - BITV 2.0 / BFSG (Accessibility - ACTIVE!)
  - AI Act (€30M fine risk)
  - E-Commerce Legal (PAngV, VVVG, TMG)
  - E-Rechnung (ZUGFeRD, UBL)

Test Files to Review:
  - P0.6_ECOMMERCE_LEGAL_TESTS.md
  - P0.7_AI_ACT_TESTS.md
  - P0.8_BARRIEREFREIHEIT_BITV_TESTS.md
  - P0.9_ERECHNUNG_TESTS.md
```

---

## ⚡ Quick Commands

### Backend Development
```bash
# Build everything
dotnet build B2Connect.slnx

# Run tests (all)
dotnet test B2Connect.slnx -v minimal

# Run specific service tests
dotnet test backend/Domain/Identity/tests -v minimal
dotnet test backend/Domain/Catalog/tests -v minimal

# Start Aspire orchestration (all services)
cd backend/Orchestration && dotnet run

# Or use VS Code task
# Press Cmd+Shift+B → Select "backend-start"
```

### Frontend Development
```bash
# Store Frontend (port 5173)
cd Frontend/Store
npm install
npm run dev

# Admin Frontend (port 5174)
cd Frontend/Admin
npm install
npm run dev

# Tests
npm run test
npm run test:e2e
```

### Accessibility Testing
```bash
# Check accessibility with Lighthouse
npx lighthouse http://localhost:5173 --only-categories=accessibility

# Check with axe DevTools CLI
npx @axe-core/cli http://localhost:5173

# Install NVDA (Windows) or use VoiceOver (macOS) for manual testing
# Press Cmd+F5 on macOS to enable VoiceOver
```

### Port Management
```bash
# Kill stuck services (macOS)
./scripts/kill-all-services.sh

# Check what's using ports
lsof -i :5173
lsof -i :5174
lsof -i :7002
lsof -i :8000

# Kill process on specific port
kill -9 $(lsof -t -i :5173)
```

---

## 🏗️ Project Structure (Essential Folders)

```
backend/
├── Domain/
│   ├── Identity/          ← Authentication service
│   ├── Catalog/           ← Product management
│   ├── CMS/               ← Content management
│   ├── Tenancy/           ← Multi-tenant isolation
│   └── [Other services]
├── BoundedContexts/
│   ├── Store/             ← Public API (read-only)
│   └── Admin/             ← Admin API (CRUD)
├── Orchestration/         ← Aspire service orchestration
└── shared/                ← Shared libraries

Frontend/
├── Store/                 ← Customer-facing (port 5173)
└── Admin/                 ← Admin panel (port 5174)

docs/
├── by-role/               ← Role-specific guidance
│   ├── BACKEND_DEVELOPER.md
│   ├── FRONTEND_DEVELOPER.md
│   ├── SECURITY_ENGINEER.md
│   ├── DEVOPS_ENGINEER.md
│   ├── QA_ENGINEER.md
│   ├── PRODUCT_OWNER.md
│   ├── LEGAL_COMPLIANCE.md
│   └── TECH_LEAD.md
├── architecture/          ← Architecture patterns
│   ├── DDD_BOUNDED_CONTEXTS.md
│   ├── ONION_ARCHITECTURE.md
│   └── ASPIRE_GUIDE.md
└── compliance/            ← Compliance test specs
    ├── P0.6_ECOMMERCE_LEGAL_TESTS.md
    ├── P0.7_AI_ACT_TESTS.md
    ├── P0.8_BARRIEREFREIHEIT_BITV_TESTS.md
    └── P0.9_ERECHNUNG_TESTS.md
```

---

## 📊 Architecture Overview

### Microservices (Wolverine-Based)
```
Frontend Store (5173)      Frontend Admin (5174)
       ↓                          ↓
  Store Gateway (8000)     Admin Gateway (8080)
       ↓                          ↓
    ┌─────────────────────────────┘
    ↓
Microservices (Private Subnet):
  - Identity (7002)
  - Catalog (7005)
  - CMS (7006)
  - Tenancy (7003)
  - [etc.]
    ↓
Data Layer:
  - PostgreSQL (5432)
  - Redis (6379)
  - Elasticsearch (9200)
```

### Each Microservice: Onion Architecture
```
Presentation Layer    → Controllers/Endpoints (Wolverine)
Application Layer     → DTOs, Handlers (Wolverine services), Validators
Infrastructure Layer  → EF Core, Repositories, External services
Domain Layer (Core)   → Entities, Value Objects, Interfaces (NO framework deps)
```

### Key Pattern: Wolverine (NOT MediatR!)
```csharp
// ✅ CORRECT: Wolverine service
public class CheckRegistrationTypeService
{
    public async Task<CheckRegistrationTypeResponse> CheckType(
        CheckRegistrationTypeCommand request,
        CancellationToken cancellationToken)
    {
        // Business logic
    }
}

// ❌ WRONG: MediatR pattern
// Do NOT use IRequest, IRequestHandler, or MediatR!
```

---

## 🔐 Critical Security Patterns

### No Hardcoded Secrets
```csharp
// ❌ WRONG
var secret = "my-secret-123";

// ✅ CORRECT
var secret = Environment.GetEnvironmentVariable("Encryption__Key")
    ?? throw new InvalidOperationException("Secret not configured");
```

### Encrypt PII
```csharp
// User entity
public string EmailEncrypted { get; set; }        // ← Always encrypted
public string PhoneNumberEncrypted { get; set; }  // ← Always encrypted
public string FirstNameEncrypted { get; set; }    // ← Always encrypted
```

### Audit Logging
```csharp
// Every data change is logged
await _auditService.LogAsync(
    action: "ProductCreated",
    entityId: product.Id,
    oldValues: null,
    newValues: product
);
```

### Tenant Isolation
```csharp
// ALWAYS filter by tenant
var product = await _repository.GetBySkuAsync(
    tenantId: _currentTenant.Id,  // ← MANDATORY
    sku: "PROD-001"
);
```

---

## 🎯 Compliance Deadlines (CRITICAL!)

| Deadline | Component | Impact |
|----------|-----------|--------|
| **28. Juni 2025** | P0.8 BITV/Accessibility | €5,000-100,000 fines |
| **17. Okt 2025** | P0.3 NIS2 Incident Response | Service shutdown risk |
| **12. Mai 2026** | P0.7 AI Act | €30M fines max |
| **1. Jan 2026** | P0.9 E-Rechnung (B2G) | Contract loss |

**Action:** These are NOT optional. They must be implemented before production.

---

## 📞 Getting Help

### Quick Reference
- **Architecture questions:** See [TECH_LEAD.md](docs/by-role/TECH_LEAD.md)
- **Code examples:** Search `backend/Domain/Identity/` (best example project)
- **Testing:** See [TESTING_FRAMEWORK_GUIDE.md](docs/TESTING_FRAMEWORK_GUIDE.md)
- **Patterns:** See [copilot-instructions.md](../.github/copilot-instructions.md)

### Escalation
1. Read the relevant role documentation
2. Check [ROLE_BASED_DOCUMENTATION_MAP.md](docs/ROLE_BASED_DOCUMENTATION_MAP.md)
3. Search for similar examples in codebase
4. Ask Tech Lead for architecture decisions

---

## ✅ Next Steps

1. **Choose your role** above
2. **Read the role-specific documentation**
3. **Follow the "Week 1, Week 2, Week 3" roadmap**
4. **Run the quick commands** to verify your setup
5. **Start implementing** your first task

**Welcome to B2Connect! 🚀**

---

**Last Updated:** 28. Dezember 2025  
**Documentation Owner:** Architecture Team  
**Questions?** Check [ROLE_BASED_DOCUMENTATION_MAP.md](docs/ROLE_BASED_DOCUMENTATION_MAP.md)
