# B2Connect AI Agent - Quick Start (READ THIS FIRST!)

**Last Updated**: 28. Dezember 2025 | **Focus**: Essential knowledge for immediate productivity  
**For comprehensive details**: See [copilot-instructions.md](./copilot-instructions.md)

---

## 🎯 The 5-Minute Foundation

### Architecture = Wolverine Microservices + DDD + Aspire

```
Each service: 
  Core (Domain) → Application (Handlers) → Infrastructure (EF Core) → API (Wolverine)
  
Key Pattern: Wolverine, NOT MediatR
  ✅ Plain POCO commands + Service.PublicAsyncMethod() 
  ❌ No IRequest, IRequestHandler, [HttpPost] attributes
  
Communication:
  → Synchronous: HTTP calls (rare)
  → Asynchronous: Wolverine events (preferred)
```

### Services (All Wolverine-based)

| Service | Port | Purpose |
|---------|------|---------|
| **Identity** | 7002 | Auth, JWT |
| **Catalog** | 7005 | Products |
| **CMS** | 7006 | Pages |
| **Tenancy** | 7003 | Multi-tenant |
| **Localization** | 7004 | i18n |
| **Theming** | 7008 | UI themes |
| **Search** | 9300 | Elasticsearch |
| **Aspire Dashboard** | 15500 | Monitoring |

---

## 🚀 Critical Commands (Copy-Paste)

```bash
# BUILD (ALWAYS FIRST!)
dotnet build B2Connect.slnx
dotnet test B2Connect.slnx -v minimal

# START DEVELOPMENT
cd backend/Orchestration && dotnet run
# Then open: http://localhost:15500 (Aspire Dashboard)

# KILL STUCK SERVICES (macOS/Linux)
./scripts/kill-all-services.sh

# FRONTEND (in separate terminal)
cd Frontend/Store && npm run dev    # Port 5173
cd Frontend/Admin && npm run dev    # Port 5174
```

---

## ⚠️ CRITICAL: 3 Rules You WILL Break

### Rule 1: Wolverine Service Pattern

**✅ DO THIS** (Service class + public async method):
```csharp
public class CheckRegistrationTypeService {
    public async Task<Response> CheckType(
        CheckRegistrationTypeCommand request,
        CancellationToken ct) { }
}
```

**❌ NEVER THIS** (MediatR pattern):
```csharp
// NO: public record CheckRegistrationTypeCommand : IRequest<Response>
// NO: public class Handler : IRequestHandler<...>
// NO: builder.Services.AddMediatR()
```

### Rule 2: Tenant Isolation

**✅ Every query filters by tenantId:**
```csharp
await _context.Products
    .Where(p => p.TenantId == tenantId)  // MANDATORY
    .ToListAsync();
```

**❌ Never without tenant filter** = Data breach

### Rule 3: Build Early & Often

**✅ Workflow:**
1. Create files
2. `dotnet build B2Connect.slnx` ← IMMEDIATELY
3. Fix any errors
4. Write tests
5. Run tests

**❌ Wrong:**
1. Create 500 lines of code
2. Assume it works
3. Discover 38 test failures later

---

## 📋 Before You Write Code

### Checklist (2 minutes)

- [ ] Architecture file: Is this a new Wolverine service or adding to existing?
- [ ] Repository: Do I need Core/Application/Infrastructure layers?
- [ ] Tenant Context: Does this access data? If yes, filter by `TenantId`
- [ ] Validation: FluentValidation? Yes, always.
- [ ] Encryption: Is this PII (email, phone, name, address)? If yes, AES-256 encrypt
- [ ] Audit Logging: Will this modify data? If yes, log before/after values
- [ ] Tests: Can I write a test in 5 minutes? If not, architecture is unclear

---

## 🔧 File Structure You'll Create

```
backend/Domain/[ServiceName]/
├── src/
│   ├── Core/                    # No framework dependencies
│   │   ├── Entities/            # Product, Order, etc.
│   │   ├── Interfaces/          # IProductRepository (contract)
│   │   └── Events/              # ProductCreatedEvent
│   ├── Application/
│   │   ├── Handlers/            # Service.PublicAsyncMethod()
│   │   ├── DTOs/
│   │   └── Validators/          # FluentValidation
│   ├── Infrastructure/
│   │   ├── Data/                # DbContext
│   │   └── Repositories/        # IProductRepository impl
│   └── Program.cs               # Entry point + DI
└── tests/
    └── [Service]Tests.csproj
```

---

## 🧪 Test Pattern (Copy-Paste Template)

```csharp
public class CreateProductHandlerTests : IAsyncLifetime {
    [Fact]
    public async Task Handle_ValidCommand_CreatesProduct() {
        // Arrange
        var cmd = new CreateProductCommand("SKU", "Name", 99.99m);
        
        // Act
        var result = await _handler.CreateAsync(_tenantId, cmd, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
    }
}
```

---

## 📚 When You're Stuck (5-Min Lookup)

| Question | See |
|----------|-----|
| "How do I create a new Wolverine endpoint?" | `backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs` |
| "How do I use FluentValidation?" | `docs/guides/TESTING_GUIDE.md` |
| "Where's the audit logging code?" | `docs/AUDIT_LOGGING_IMPLEMENTATION.md` |
| "Multi-tenancy example?" | `docs/DDD_BOUNDED_CONTEXTS.md` |
| "Encryption at rest?" | `.github/copilot-instructions.md` §Security |

---

## 🚨 Common Mistakes (Learned the Hard Way)

| Mistake | Cost | Prevention |
|---------|------|-----------|
| Forgot to build before pushing | 38 test failures | `dotnet build` first, always |
| Queried without TenantId filter | Data breach | Check EVERY `_context.X.Where()` |
| Used default `null` instead of explicit value | Locale failures | Make defaults meaningful + document |
| Added decimal field without locale awareness | Tests failed in German | Use `decimal` carefully |
| MediatR instead of Wolverine | Complete refactor | Copy Service.cs pattern |
| Hardcoded secrets | Security issue | Use `appsettings.json` + `IConfiguration` |

---

## 📞 Next Steps

1. **Read this (2 min)** ← You are here
2. **See architecture** → [DDD_BOUNDED_CONTEXTS.md](../../docs/architecture/DDD_BOUNDED_CONTEXTS.md) (5 min)
3. **Copy test pattern** → [TESTING_GUIDE.md](../../docs/guides/TESTING_GUIDE.md) (5 min)
4. **Detailed reference** → [copilot-instructions.md](./copilot-instructions.md) (as needed)

---

**Question?** First check the file references above. Then check the comprehensive guide. Then ask.
