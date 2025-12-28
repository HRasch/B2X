# AI Coding Agent Instructions for B2Connect

**Last Updated**: 29. Dezember 2025 | **Architecture**: DDD Microservices with Wolverine + CLI  
**Optimization**: Patterns and anti-patterns moved to role-specific guides

---

## 🎯 START HERE - Choose Your Role

> **First time?** Select your role for targeted guidance:
> 
> | Role | Time | File | Includes |
> |------|------|------|----------|
> | 💻 Backend Developer | 15 min | [copilot-instructions-backend.md](./copilot-instructions-backend.md) | Wolverine, Async/Await (50), Performance (25), EF Core (25), Architecture (25), .NET10 (25), Anti-Patterns (50) |
> | 🎨 Frontend Developer | 15 min | [copilot-instructions-frontend.md](./copilot-instructions-frontend.md) | Vue.js (44), Tailwind (20), Vite (8), Anti-Patterns (44) |
> | ⚙️ DevOps Engineer | 10 min | [copilot-instructions-devops.md](./copilot-instructions-devops.md) | Aspire, Docker, Infrastructure |
> | 🧪 QA Engineer | 10 min | [copilot-instructions-qa.md](./copilot-instructions-qa.md) | Testing Strategy, 52 Compliance Tests |
> | 🔐 Security Engineer | 15 min | [copilot-instructions-security.md](./copilot-instructions-security.md) | Security Patterns (25), Encryption, Audit Logging, Anti-Patterns (25) |
> | **Quick Start** | **5 min** | **[Quick-Start Guide](./copilot-instructions-quickstart.md)** | Essential commands & conventions |

---

## 🔧 Allowed Capabilities

- ✅ **GitHub CLI Management**: Authorized to manage project on GitHub (issues, PRs, branches, boards)

---

## 🏗️ Core Architecture (Essential Overview Only)

### DDD Microservices with Wolverine

B2Connect is a **multi-tenant SaaS platform** using:
- **Wolverine** (async messaging, HTTP endpoints) - NOT MediatR
- **Domain-Driven Design** with Onion Architecture
- **PostgreSQL 16** (database per service)
- **Aspire** for service orchestration

### Service Port Map (Critical!)

| Service | Port | Type |
|---------|------|------|
| Identity | 7002 | Wolverine |
| Tenancy | 7003 | Wolverine |
| Localization | 7004 | Wolverine |
| Catalog | 7005 | Wolverine |
| CMS | 7006 | Wolverine |
| Theming | 7008 | Wolverine |
| Search | 9300 | Wolverine |
| Aspire Dashboard | 15500 | Orchestration |
| Frontend Store | 5173 | Vue.js |
| Frontend Admin | 5174 | Vue.js |

### Onion Architecture (Each Service)

```
Core (Domain)
  ↓ dependencies point inward only
Application (CQRS, Handlers, Validators)
  ↓
Infrastructure (EF Core, Repositories)
  ↓
Presentation (API Endpoints)
```

**Critical Rule**: Core has ZERO framework dependencies.

---

## 🔀 Git Workflow & Commit Conventions

### Commit Message Format

```
<type>(<scope>): <subject> (#<issue-id>)

<body>
<footer>

Types: fix, feat, docs, style, refactor, perf, test, chore
Scope: service affected (e.g., auth, catalog)
Subject: Imperative, lowercase, max 50 chars
Issue-ID: GitHub issue (e.g., #30)
```

### Examples

✅ **Good:**
```
feat(catalog): add product filtering by category (#30)

Implement category-based product filtering for store API.
Adds new QueryProductsByCategory handler in Catalog service.

Closes #30
```

✅ **Multiple commits per issue** (recommended):
```
Commit 1: feat(price-calc): add VAT calculation service (#20)
Commit 2: test(price-calc): add VAT calculation tests (#20)
Commit 3: docs(price-calc): document VAT logic (#20)

Each independently valuable, all reference same issue.
```

❌ **Avoid:**
```
Updated stuff  # Vague, no context
fixed things   # Not descriptive
```

### Valid Footer Keywords (GitHub Automation)

- `Closes #N` - Close issue on merge
- `Fixes #N` - Same as Closes
- `Resolves #N` - Same as Closes
- `Related #N` - Link without closing
- `Depends on #N` - Explicit dependency

---

## 🔒 Security-First Checklist (Before Code)

**BEFORE generating ANY code, verify:**

```
Database Layer:
  [ ] Entity has TenantId (multi-tenant isolation)
  [ ] Entity has CreatedAt, CreatedBy (audit trail)
  [ ] PII encrypted: Email, Phone, Address, DOB, Name
  [ ] Soft delete flags: IsDeleted, DeletedAt
  [ ] Query always filters by TenantId

Service Layer:
  [ ] IEncryptionService injected (if PII)
  [ ] Audit logging: All CRUD operations logged
  [ ] FluentValidation required
  [ ] CancellationToken passed throughout
  [ ] TenantId verified from JWT claims

API Layer:
  [ ] JWT validation required (no anonymous)
  [ ] X-Tenant-ID header extracted
  [ ] All inputs validated server-side
  [ ] No PII in error messages
  [ ] No stack traces in production

Testing:
  [ ] Cross-tenant access blocked
  [ ] Encryption/decryption round-trip tested
  [ ] Audit logs created
  [ ] CancellationToken propagates
```

---

## 🚀 Developer Workflows

### Build & Run

```bash
# Start all services with Aspire (recommended)
cd backend/Orchestration
dotnet run

# Dashboard: http://localhost:15500

# BEFORE restarting (CRITICAL macOS):
./scripts/kill-all-services.sh
./scripts/check-ports.sh
```

### Quick Commands

```bash
# Build
dotnet build B2Connect.slnx

# Test
dotnet test B2Connect.slnx -v minimal

# Identity service tests
dotnet test backend/Domain/Identity/tests -v minimal

# CLI
b2connect start
b2connect status
b2connect migrate --service Identity
```

### Cleanup (if ports stuck)

```bash
# macOS/Linux:
pkill -9 -f "dcpctrl"
pkill -9 -f "dcpproc"
./scripts/kill-all-services.sh

# Then restart
```

---

## 🎯 Wolverine Pattern (CRITICAL - NOT MediatR!)

### HTTP Endpoint Pattern

**Step 1: Plain POCO Command** (no `IRequest<T>`)
```csharp
public class CreateProductCommand
{
    public string Sku { get; set; }
    public string Name { get; set; }
}
```

**Step 2: Service Handler** (plain class, public async methods)
```csharp
public class ProductService
{
    private readonly IProductRepository _repo;

    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        // Business logic
        return new CreateProductResponse { Id = productId };
    }
}
```

**Step 3: Register DI** (simple!)
```csharp
builder.Services.AddScoped<ProductService>();
// Wolverine auto-discovers HTTP endpoint: POST /createproduct
```

**✅ Reference:** [backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs](../backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs)

### ❌ NEVER Use MediatR Pattern

```csharp
// ❌ WRONG: Don't use IRequest
public record CreateProductCommand : IRequest<ProductDto>;

// ❌ WRONG: Don't use IRequestHandler
public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto> { }

// ❌ WRONG: Don't add MediatR
builder.Services.AddMediatR();
```

---

## 📋 Detailed Patterns & Anti-Patterns (By Role)

| Topic | Backend | Frontend | DevOps | Security | QA |
|-------|---------|----------|--------|----------|-----|
| **Async/Await** (50 rules) | [✓](./copilot-instructions-backend.md#-asyncawait-best-practices) | - | - | - | - |
| **Performance** (25 rules) | [✓](./copilot-instructions-backend.md#-performance-best-practices) | - | - | - | - |
| **EF Core** (25 rules) | [✓](./copilot-instructions-backend.md#-entity-framework-core-best-practices) | - | - | - | - |
| **Architecture** (25 rules) | [✓](./copilot-instructions-backend.md#-architecture--design-best-practices) | - | - | - | - |
| **.NET10** (25 rules) | [✓](./copilot-instructions-backend.md#-net-10--c-14-specific-features) | - | - | - | - |
| **Code Quality** (50 rules) | [✓](./copilot-instructions-backend.md#-code-quality-anti-patterns) | - | - | - | - |
| **Vue.js** (44 rules) | - | [✓](./copilot-instructions-frontend.md#-vue3-best-practices) | - | - | - |
| **Tailwind** (20 rules) | - | [✓](./copilot-instructions-frontend.md#-tailwind-css-best-practices) | - | - | - |
| **Vite** (8 rules) | - | [✓](./copilot-instructions-frontend.md#-vite-best-practices) | - | - | - |
| **Security** (25 rules) | - | - | - | [✓](./copilot-instructions-security.md) | - |
| **Encryption** | - | - | - | [✓](./copilot-instructions-security.md) | - |
| **Compliance Testing** (52 tests) | - | - | - | - | [✓](./copilot-instructions-qa.md) |

---

## 💡 Key Learnings (Session 28. Dezember 2025)

### 1. Build Validation First

```bash
# BEFORE implementing features:
dotnet build B2Connect.slnx  # Fix compiler errors
dotnet test backend/Domain/[Service]/tests -v minimal  # Then test

# ❌ DON'T: Generate code → Build at end (38 failures accumulated)
# ✅ DO: Build immediately after creating files
```

### 2. Wolverine ONLY (Not MediatR)

- **Wolverine**: All 9 microservices, event-driven, distributed
- **MediatR**: In-process, single app only
- B2Connect requires Wolverine for messaging across services

### 3. No Hardcoded Secrets

```csharp
// ❌ WRONG
var jwtSecret = "my-secret-123";

// ✅ RIGHT
var jwtSecret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException();
```

### 4. Tenant Isolation Mandatory

```csharp
// ❌ WRONG: No tenant filter = security breach
var product = await _context.Products.FirstOrDefaultAsync(p => p.Sku == sku);

// ✅ RIGHT: Always filter by tenant
var product = await _context.Products
    .Where(p => p.TenantId == tenantId && p.Sku == sku)
    .FirstOrDefaultAsync();
```

### 5. Locale Handling (German Context)

Germany uses `,` for decimal separator:
- German: `1,99€` (1,99)
- International: `1.99€` (1.99)

Test both formats or normalize explicitly.

### 6. Central Package Management (CPM)

B2Connect uses CPM in `Directory.Packages.props`. New standalone projects:

```xml
<!-- Disable CPM for CLI tools -->
<PropertyGroup>
  <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
</PropertyGroup>
```

### 7. macOS DCP Port Issues

```bash
# Aspire's DCP controller holds ports after shutdown
# Always clean before restart:

./scripts/kill-all-services.sh
./scripts/check-ports.sh
dotnet run  # Safe to restart
```

### 8. Documentation with Code Examples

**High value**: Every pattern must have 3-5 code examples
**Low value**: Abstract descriptions without working examples

### 9. 52 Compliance Tests = Hard Gate

Phase 1 features can ONLY deploy if:
- ✅ P0.6 E-Commerce Tests: 15/15
- ✅ P0.7 AI Act Tests: 15/15
- ✅ P0.8 BITV Tests: 12/12
- ✅ P0.9 E-Rechnung Tests: 10/10

If ANY fails: Deployment BLOCKED.

---

## 📊 Retrospective Metrics (28. Dezember 2025)

| Metric | Value | Target |
|--------|-------|--------|
| Phase A Output | 10,580+ lines | Documentation ✅ |
| Test Passing Rate | 169/171 (99.4%) | > 95% ✅ |
| Build Time | 8.5s | < 10s ✅ |
| Code Coverage | > 80% | >= 80% ✅ |
| Compliance Tests | 52 defined | Ready ✅ |
| Wolverine Pattern | 100% | No MediatR ✅ |

---

## 🚀 Getting Started

1. **Your Role**: Read role-specific guide (15 min)
2. **Architecture**: Understand Wolverine, Onion, Tenancy
3. **Code Example**: Check `backend/Domain/Identity/` patterns
4. **Security**: Review security checklist BEFORE coding
5. **Build**: `dotnet build` immediately after file creation
6. **Test**: Run domain-specific tests
7. **Commit**: Follow git workflow with issue ID

---

## 🔗 Complete Documentation Index

- [Quick-Start Guide](./copilot-instructions-quickstart.md) - 5 min onboarding
- [Backend Developer](./copilot-instructions-backend.md) - Wolverine, Async, Performance, EF Core, Architecture, Code Quality
- [Frontend Developer](./copilot-instructions-frontend.md) - Vue.js, Tailwind, Vite, Anti-Patterns
- [DevOps Engineer](./copilot-instructions-devops.md) - Aspire, Infrastructure, Deployment
- [QA Engineer](./copilot-instructions-qa.md) - Testing, 52 Compliance Tests
- [Security Engineer](./copilot-instructions-security.md) - Encryption, Audit, Security Patterns
- [Tech Lead](./copilot-instructions-techpad.md) - Architecture Decisions
- [Project Index](./copilot-instructions-index.md) - All documentation

---

**Last updated**: 29. Dezember 2025 | **Token Optimization**: 3,578 → 1,200 lines | **Patterns moved to role-specific guides**: 200+ rules

