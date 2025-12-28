# B2Connect AI Coding Agent Instructions (Refactored)

**Version**: 2.0 (Optimized)  
**Last Updated**: 28. Dezember 2025  
**Purpose**: Primary instructions + delegation to role-specific agents  
**Architecture**: DDD Microservices with Wolverine + CLI

---

## 🚀 Quick Start

**Who are you?** Select your role below and jump to your agent:

| Role | Agent File | Quick Link |
|------|-----------|-----------|
| 💻 Backend Developer | `backend-developer.agent.md` | [→ Go](./agents/backend-developer.agent.md) |
| 🎨 Frontend Developer | `frontend-developer.agent.md` | [→ Go](./agents/frontend-developer.agent.md) |
| 🧪 QA Engineer | `qa-engineer.agent.md` | [→ Go](./agents/qa-engineer.agent.md) |
| 🔐 Security Engineer | `security-engineer.agent.md` | [→ Go](./agents/security-engineer.agent.md) |
| ⚙️ DevOps Engineer | `devops-engineer.agent.md` | [→ Go](./agents/devops-engineer.agent.md) |
| 👔 Tech Lead | `tech-lead.agent.md` | [→ Go](./agents/tech-lead.agent.md) |
| 📋 Product Owner | `product-owner.agent.md` | [→ Go](./agents/product-owner.agent.md) |
| ⚖️ Legal/Compliance | `legal-compliance.agent.md` | [→ Go](./agents/legal-compliance.agent.md) |

**Specialist Roles:**
| Role | Agent File |
|------|-----------|
| 🤖 AI Specialist | `ai-specialist.agent.md` |
| 🎨 UI Expert | `ui-expert.agent.md` |
| 👁️ UX Expert | `ux-expert.agent.md` |
| 🛒 Backend Store | `backend-store.agent.md` |
| 🎨 Frontend Store | `frontend-store.agent.md` |
| 🔧 Backend Admin | `backend-admin.agent.md` |
| 📊 Frontend Admin | `frontend-admin.agent.md` |

**Testing Specialists:**
| Role | Agent File |
|------|-----------|
| 🧪 QA Frontend | `qa-frontend.agent.md` |
| ⚡ QA Performance | `qa-performance.agent.md` |
| 🔍 QA Pentesting | `qa-pentesting.agent.md` |

**Stakeholders:**
| Role | Agent File |
|------|-----------|
| 📊 Stakeholder BI | `stakeholder-bi.agent.md` |
| 📧 Stakeholder CRM | `stakeholder-crm.agent.md` |
| 🔗 Stakeholder ERP | `stakeholder-erp.agent.md` |
| 📦 Stakeholder PIM | `stakeholder-pim.agent.md` |
| 🤝 Stakeholder Reseller | `stakeholder-reseller.agent.md` |

**Support:**
| Role | Agent File |
|------|-----------|
| 🎟️ Support Triage | `support-triage.agent.md` |

---

## 🏗️ Core Architecture (Always Valid)

### Wolverine HTTP Handler Pattern (NOT MediatR!)

```csharp
// ✅ CORRECT: Plain POCO command
public class CreateProductCommand
{
    public string Sku { get; set; }
    public string Name { get; set; }
}

// ✅ CORRECT: Service with public async methods
public class ProductService
{
    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        // Implementation
        return response;
    }
}

// ✅ CORRECT: Simple DI registration
builder.Services.AddScoped<ProductService>();
// Wolverine auto-discovers HTTP endpoint: POST /createproduct
```

**Reference**: See [backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs](../../backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs)

### Onion Architecture (Every Service)

```
Core Layer (Domain)          ← No framework dependencies
  ↓ (dependencies point inward)
Application Layer (CQRS)     ← DTOs, Handlers, Validators
  ↓
Infrastructure Layer         ← EF Core, Repositories, External Services
  ↓
Presentation Layer (API)     ← Controllers, Middleware
```

### Multi-Tenancy (Mandatory)

- **Every query** includes tenant ID filter
- **X-Tenant-ID** header extracted in middleware
- **Cross-tenant access** = security breach (test for it!)

### Audit Logging (Mandatory)

```csharp
public class BaseEntity
{
    public Guid TenantId { get; set; }      // Tenant isolation
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public bool IsDeleted { get; set; }     // Soft delete
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
```

---

## 🔧 Allowed Capabilities

✅ **GitHub CLI**: Create issues, PRs, manage branches  
✅ **Code Generation**: Create .NET, Vue.js, SQL code  
✅ **Testing**: Write unit tests, integration tests, E2E tests  
✅ **Documentation**: Create guides, architecture docs  
✅ **Git Management**: Commit, push, create branches  

❌ **Cannot**: Delete repositories, modify billing, access production secrets

---

## 🎯 Critical Rules (Non-Negotiable)

### 1. Build Early, Build Often
```bash
dotnet build B2Connect.slnx  # After EVERY file creation
```
If build fails → **STOP** → Fix → Build again → Then tests

### 2. Wolverine ONLY (NOT MediatR)
```csharp
// ❌ WRONG: IRequest interface
public record Cmd : IRequest<Response>;

// ✅ CORRECT: Plain POCO
public record Cmd(string Name);
```

### 3. FluentValidation for All Commands
```csharp
public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(50);
    }
}
```

### 4. Tenant Isolation (Every Query!)
```csharp
// ❌ WRONG: No tenant filter
var product = await _repo.GetBySkuAsync(sku);

// ✅ CORRECT: Tenant filter mandatory
var product = await _repo.GetBySkuAsync(tenantId, sku);
```

### 5. Soft Deletes (Never Hard Delete)
```csharp
// ❌ WRONG: Hard delete
await _context.Products.Where(x => x.Id == id).ExecuteDeleteAsync();

// ✅ CORRECT: Soft delete
product.IsDeleted = true;
product.DeletedAt = DateTime.UtcNow;
product.DeletedBy = currentUserId;
await _context.SaveChangesAsync();
```

### 6. Encryption for PII
```csharp
// Encrypted fields: Email, Phone, FirstName, LastName, Address, DOB
public class User
{
    public string EmailEncrypted { get; set; }  // Must be encrypted
    public string PhoneEncrypted { get; set; }  // Must be encrypted
}
```

### 7. No Hardcoded Secrets
```bash
# ❌ WRONG: Hardcoded
var secret = "super_secret_key";

# ✅ CORRECT: From environment/vault
var secret = Environment.GetEnvironmentVariable("JWT_SECRET");
```

### 8. CancellationToken Everywhere
```csharp
// ❌ WRONG: No cancellation token
public async Task<User> GetAsync(Guid id) { }

// ✅ CORRECT: CancellationToken parameter
public async Task<User> GetAsync(Guid id, CancellationToken ct) { }
```

### 9. Async/Await (Never .Wait() or .Result)
```csharp
// ❌ WRONG: Causes deadlock
var user = userTask.Result;
userTask.Wait();

// ✅ CORRECT: Use await
var user = await userTask;
```

### 10. 80%+ Test Coverage
```bash
dotnet test B2Connect.slnx  # All tests must pass
# Coverage target: >= 80%
```

---

## 🔐 Security Checklist (Before EVERY PR)

- [ ] No hardcoded secrets (API keys, passwords, tokens)
- [ ] All inputs validated (FluentValidation)
- [ ] PII encrypted (Email, Phone, Address, DOB, FirstName, LastName)
- [ ] Tenant isolation enforced (cross-tenant access impossible)
- [ ] Audit logging for data changes
- [ ] Soft deletes used (never hard delete)
- [ ] CancellationToken passed through
- [ ] HTTPS/TLS enforced
- [ ] JWT validation on protected endpoints
- [ ] No PII in error messages or logs

---

## 📋 Git Workflow

### Commit Message Format
```
<type>(<scope>): <subject> (#<issue-id>)

<body>

<footer>

Types: fix, feat, docs, style, refactor, perf, test, chore
Example: feat(catalog): add product filtering (#30)
```

### Branch Strategy
```
feature/<issue-number>-<description>  # Development
  ↓
master                                 # Merge here when ready
  ↓
Deploy to production
```

### Before Merging to Master
- ✅ All tests passing locally (`dotnet test B2Connect.slnx`)
- ✅ Code review approved
- ✅ No compiler warnings
- ✅ Security checklist completed
- ✅ At least one other developer reviewed

---

## 🚀 Running Projects

### Backend (All Services)
```bash
dotnet run --project backend/Orchestration/B2Connect.Orchestration.csproj
# Dashboard: http://localhost:15500
```

### Frontend (Store)
```bash
cd frontend-store && npm run dev
# Store: http://localhost:5173
```

### Frontend (Admin)
```bash
cd frontend-admin && npm run dev
# Admin: http://localhost:5174
```

### Kill Stuck Services
```bash
./scripts/kill-all-services.sh
```

---

## 📚 Key Documentation

**Architecture**:
- [DDD Bounded Contexts](../../docs/architecture/DDD_BOUNDED_CONTEXTS.md)
- [Onion Architecture](../../docs/ONION_ARCHITECTURE.md)
- [Wolverine Patterns](../../docs/WOLVERINE_HTTP_ENDPOINTS.md)

**Compliance**:
- [EU SaaS Compliance Roadmap](../../docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md)
- [Application Specifications](../../docs/APPLICATION_SPECIFICATIONS.md)

**Guides**:
- [Testing Framework](../../docs/TESTING_FRAMEWORK_GUIDE.md)
- [Debugging Guide](../../docs/guides/DEBUGGING_GUIDE.md)

**Quick Starts** (by role):
- Backend: [BACKEND_DEVELOPER_QUICK_START.md](../../BACKEND_DEVELOPER_QUICK_START.md)
- Frontend: [FRONTEND_DEVELOPER_QUICK_START.md](../../FRONTEND_DEVELOPER_QUICK_START.md)
- QA: [QA_ENGINEER_QUICK_START.md](../../QA_ENGINEER_QUICK_START.md)
- Security: [SECURITY_ENGINEER_QUICK_START.md](../../SECURITY_ENGINEER_QUICK_START.md)
- DevOps: [DEVOPS_ENGINEER_QUICK_START.md](../../DEVOPS_ENGINEER_QUICK_START.md)
- Tech Lead: [TECH_LEAD_QUICK_START.md](../../TECH_LEAD_QUICK_START.md)

---

## 🔄 Session Retrospective (Latest Learnings)

### 1. Build Validation First
**Lesson**: Don't generate 500 lines → discover 38 test failures later

**Pattern**:
```bash
# Step 1: Generate code
# Step 2: dotnet build immediately
# Step 3: Fix compiler errors
# Step 4: Run tests
# Step 5: Verify audit logging
```

### 2. Locale-Aware Testing
**Lesson**: Germany uses `,` for decimals, test-machines vary

**Pattern**:
```csharp
var normalized = value.ToString().Replace(",", ".");  // Handle both formats
Assert.Equal(expected, normalized);
```

### 3. Explicit Default Values
**Lesson**: `null` is confusing → use meaningful defaults

```csharp
// ✅ CORRECT: Explicit default
public decimal FinalPrice { get; set; } = priceIncludingVat;

// ❌ WRONG: Implicit null
public decimal? FinalPrice { get; set; }
```

### 4. Wolverine NOT MediatR
**Lesson**: MediatR != Wolverine patterns used in B2Connect

```csharp
// ✅ CORRECT: Wolverine Service
public class ProductService
{
    public async Task<Response> CreateProduct(Command cmd, CancellationToken ct) { }
}

// ❌ WRONG: MediatR Handler
public class CreateProductHandler : IRequestHandler<Command, Response> { }
```

### 5. Compliance Always Integrated
**Lesson**: Security features NOT "nice-to-have" → built-in from day 1

```csharp
// ✅ Feature = Business Logic + Audit + Encryption + Authorization
public async Task<Result> CreateAsync(Guid tenantId, Command cmd, CancellationToken ct)
{
    // 1. Tenant validation
    // 2. Input validation
    // 3. Business logic
    // 4. Encryption
    // 5. Audit logging
    // 6. Error handling
}
```

---

## 📞 Need Help?

1. **Your role-specific agent**: Use the table at top of this file
2. **Architecture questions**: See Tech Lead agent
3. **Security concerns**: See Security Engineer agent
4. **Compliance**: See Legal/Compliance agent
5. **Testing**: See QA Engineer agent
6. **Not sure?**: Check [AGENTS_REGISTRY.md](./AGENTS_REGISTRY.md)

---

## ✅ Validation Checklist (Before Commit)

**Code Quality**:
- [ ] Compiles without warnings
- [ ] Tests passing (80%+ coverage)
- [ ] No hardcoded secrets
- [ ] No TODO comments
- [ ] Audit logging added

**Architecture**:
- [ ] Wolverine pattern (not MediatR)
- [ ] FluentValidation for commands
- [ ] Tenant isolation enforced
- [ ] Soft deletes (not hard deletes)
- [ ] CancellationToken throughout

**Security**:
- [ ] PII encrypted
- [ ] No cross-tenant leaks
- [ ] Input validated server-side
- [ ] Error messages don't expose internals

**Documentation**:
- [ ] Code commented (non-obvious logic)
- [ ] Commit message clear
- [ ] Issue number referenced (#issue-id)

---

**Last Updated**: 28. Dezember 2025  
**Version**: 2.0 (Agent-Delegating Model)  
**Status**: ✅ Ready for use
