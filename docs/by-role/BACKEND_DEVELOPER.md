# 💻 Backend Developer - Documentation Guide

**Role:** Backend Developer | **P0 Components:** P0.1, P0.6, P0.7, P0.9  
**Time to Read:** ~5 hours | **Priority:** 🔴 CRITICAL

---

## 🎯 Your Mission

Als Backend Developer bist du verantwortlich für:
- **Audit Logging Implementation** (EF Core Interceptors) - P0.1
- **E-Commerce Backend** (VAT, Invoices, Returns) - P0.6
- **AI Act Backend** (Decision Logging, Bias Testing) - P0.7
- **E-Rechnung Backend** (ZUGFeRD, UBL Generation) - P0.9
- **Wolverine HTTP Handlers** (NICHT MediatR!)
- **CQRS Pattern Implementation**

---

## 📚 Required Reading (P0)

### Week 1: Core Patterns (CRITICAL!)

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 1 | **Copilot Instructions (FULL!)** | [copilot-instructions.md](../../.github/copilot-instructions.md) | 60 min |
| 2 | **Wolverine HTTP Endpoints** | [api/WOLVERINE_HTTP_ENDPOINTS.md](../api/WOLVERINE_HTTP_ENDPOINTS.md) | 45 min |
| 3 | **CQRS Wolverine Pattern** | [api/CQRS_WOLVERINE_PATTERN.md](../api/CQRS_WOLVERINE_PATTERN.md) | 30 min |
| 4 | **Onion Architecture** | [ONION_ARCHITECTURE.md](../ONION_ARCHITECTURE.md) | 30 min |
| 5 | **DDD Bounded Contexts** | [architecture/DDD_BOUNDED_CONTEXTS.md](../architecture/DDD_BOUNDED_CONTEXTS.md) | 30 min |

### Week 2: API & Testing

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 6 | **Admin API Guide** | [ADMIN_API_IMPLEMENTATION_GUIDE.md](../ADMIN_API_IMPLEMENTATION_GUIDE.md) | 30 min |
| 7 | **Gateway Separation** | [api/GATEWAY_SEPARATION.md](../api/GATEWAY_SEPARATION.md) | 20 min |
| 8 | **Testing Framework** | [TESTING_FRAMEWORK_GUIDE.md](../TESTING_FRAMEWORK_GUIDE.md) | 30 min |
| 9 | **Testing Guide** | [guides/TESTING_GUIDE.md](../guides/TESTING_GUIDE.md) | 20 min |

### Week 3: Compliance Features

| # | Document | Path | Est. Time |
|---|----------|------|-----------|
| 10 | **EU Compliance Roadmap** | [compliance/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md](../compliance/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md) | 60 min |
| 11 | **E-Commerce Tests (P0.6)** | [compliance/P0.6_ECOMMERCE_LEGAL_TESTS.md](../compliance/P0.6_ECOMMERCE_LEGAL_TESTS.md) | 30 min |
| 12 | **AI Act Tests (P0.7)** | [compliance/P0.7_AI_ACT_TESTS.md](../compliance/P0.7_AI_ACT_TESTS.md) | 30 min |
| 13 | **E-Rechnung Tests (P0.9)** | [compliance/P0.9_ERECHNUNG_TESTS.md](../compliance/P0.9_ERECHNUNG_TESTS.md) | 30 min |
| 14 | **Audit Logging** | [AUDIT_LOGGING_IMPLEMENTATION.md](../AUDIT_LOGGING_IMPLEMENTATION.md) | 20 min |

---

## ⚠️ CRITICAL: Wolverine Pattern (NOT MediatR!)

### ✅ CORRECT Pattern (Wolverine)

```csharp
// Step 1: Plain POCO Command (NO IRequest!)
public class CreateProductCommand
{
    public string Sku { get; set; }
    public string Name { get; set; }
}

// Step 2: Service Handler (NO IRequestHandler!)
public class ProductService
{
    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        // Business logic
        return new CreateProductResponse { Id = productId };
    }
}

// Step 3: Simple DI Registration
builder.Services.AddScoped<ProductService>();
```

### ❌ WRONG Pattern (MediatR - DO NOT USE!)

```csharp
// WRONG: IRequest interface
public record CreateProductCommand(string Sku) : IRequest<ProductDto>;

// WRONG: IRequestHandler
public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto> { }

// WRONG: AddMediatR
builder.Services.AddMediatR();
```

**Reference:** [CheckRegistrationTypeService.cs](../../backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs)

---

## 🔧 Your P0 Components

### P0.1: Audit Logging (Week 3-4, with Security)
```
Effort: 40 hours (shared)
Your Tasks:
  ✅ EF Core SaveChangesInterceptor
  ✅ AuditLogEntry entity
  ✅ Tenant isolation in queries
  ✅ JSON serialization for before/after values

Acceptance:
  ✅ All CRUD operations logged
  ✅ Tests: 5+ test cases
  ✅ < 10ms overhead per operation
```

### P0.6: E-Commerce Backend (Week 5-6)
```
Effort: 60 hours
Your Tasks:
  ✅ VAT calculation service (B2B reverse charge)
  ✅ VIES VAT-ID validation API
  ✅ Invoice generation (PDF + storage)
  ✅ Return/withdrawal management
  ✅ 10-year invoice archival

Acceptance:
  ✅ 15 tests passing
  ✅ VAT calculation correct
  ✅ VIES integration working
```

### P0.7: AI Act Backend (Week 9-10, with Security)
```
Effort: 50 hours (shared)
Your Tasks:
  ✅ AiDecisionLog entity
  ✅ Decision logging service
  ✅ Performance monitoring job
  ✅ User explanation API

Acceptance:
  ✅ 15 tests passing
  ✅ Decision log queryable
  ✅ Explanation API working
```

### P0.9: E-Rechnung Backend (Week 7-8)
```
Effort: 40 hours
Your Tasks:
  ✅ ZUGFeRD 3.0 XML generation
  ✅ Hybrid PDF creation (embedded XML)
  ✅ UBL 2.3 alternative format
  ✅ Schema validation
  ✅ ERP webhook API

Acceptance:
  ✅ 10 tests passing
  ✅ ZUGFeRD schema validates (0 errors)
  ✅ SAP/NetSuite import tested
```

---

## ⚡ Quick Commands

```bash
# Build backend
dotnet build B2Connect.slnx

# Run all tests
dotnet test B2Connect.slnx -v minimal

# Run specific service tests
dotnet test backend/Domain/Identity/tests -v minimal
dotnet test backend/Domain/Catalog/tests -v minimal

# Start Aspire (all services)
cd AppHost && dotnet run

# Start single service
dotnet run --project backend/Domain/Identity/src/B2Connect.Identity.csproj

# Generate migration
dotnet ef migrations add <Name> --project backend/Domain/Identity/src
```

---

## 🏗️ Code Structure (Onion Architecture)

```
backend/Domain/[Service]/
├── src/
│   ├── Core/                    # Domain Layer (innermost)
│   │   ├── Entities/            # Product, User, Order
│   │   ├── ValueObjects/        # Price, SKU, Email
│   │   ├── Interfaces/          # IProductRepository
│   │   └── Events/              # ProductCreatedEvent
│   │
│   ├── Application/             # Application Layer
│   │   ├── DTOs/                # ProductDto
│   │   ├── Handlers/            # Wolverine services (!)
│   │   ├── Validators/          # FluentValidation
│   │   └── Mappers/             # AutoMapper profiles
│   │
│   ├── Infrastructure/          # Infrastructure Layer
│   │   ├── Data/                # EF Core DbContext
│   │   ├── Repositories/        # Repository implementations
│   │   └── External/            # External services
│   │
│   └── Program.cs               # Presentation Layer entry
│
└── tests/                       # Test project
    └── [Service].Tests.csproj
```

---

## 📊 Code Quality Checklist

Before every PR:

- [ ] **Wolverine pattern used** (NOT MediatR)
- [ ] **FluentValidation** for all commands
- [ ] **Tenant ID** in all queries
- [ ] **Audit logging** for data changes
- [ ] **Soft deletes** (IsDeleted flag)
- [ ] **Tests written** (80%+ coverage target)
- [ ] **No hardcoded secrets**
- [ ] **Async/await** used consistently
- [ ] **CancellationToken** passed through

---

## 📞 Escalation Path

| Issue | Contact | SLA |
|-------|---------|-----|
| Architecture Question | Tech Lead | < 4h |
| Security Concern | Security Engineer | < 1h |
| Database Issue | DBA/DevOps | < 2h |
| Test Failure | QA Engineer | < 4h |

---

## ✅ Definition of Done (Backend)

Before marking any task as complete:

- [ ] Code compiles without warnings
- [ ] All tests passing
- [ ] Code review approved
- [ ] Documentation updated
- [ ] No TODO comments left
- [ ] Logging added for debugging
- [ ] Error handling complete
- [ ] Performance acceptable (< 200ms)

---

**Next:** Start with [copilot-instructions.md](../../.github/copilot-instructions.md) (FULL document!)
