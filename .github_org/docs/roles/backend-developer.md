# 💻 Backend Developer Quick Start

**Role Focus:** Wolverine handlers, CQRS, business logic, APIs  
**Time to Productivity:** 3 weeks  
**Critical Pattern:** NOT MediatR → Use Wolverine!

---

## ⚡ Week 1: Foundation

### Day 1-2: Core Patterns
```bash
# Read these in order:
1. .github/copilot-instructions.md (§Wolverine HTTP Handlers)
2. docs/WOLVERINE_HTTP_ENDPOINTS.md
3. docs/CQRS_WOLVERINE_PATTERN.md
4. docs/ONION_ARCHITECTURE.md
```

### Day 3-4: Project Structure
```bash
# Explore:
backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs
# ✅ Reference implementation (NOT MediatR!)

# Build and run:
dotnet build B2Connect.slnx
```

### Day 5: First Wolverine Service
```csharp
// Create file: backend/Domain/[Service]/src/Handlers/YourService.cs

public class YourService
{
    private readonly IYourRepository _repo;
    
    public YourService(IYourRepository repo) => _repo = repo;
    
    // Wolverine auto-generates HTTP endpoint from this method
    public async Task<YourResponse> YourHandler(
        YourCommand request,
        CancellationToken cancellationToken)
    {
        // Business logic here
        return new YourResponse { Success = true };
    }
}

// Register in Program.cs:
builder.Services.AddScoped<YourService>();
```

**Validation:**
```bash
dotnet build  # Should compile
# Endpoint: POST /yourhandler
```

---

## 📋 Week 2: Business Logic & Data Access

### Day 1-2: Repository Pattern
- Core: Define `IYourRepository` interface
- Infrastructure: Implement with EF Core
- Tests: Mock the interface

### Day 3-4: FluentValidation
```csharp
public class YourCommandValidator : AbstractValidator<YourCommand>
{
    public YourCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

// In handler:
var validation = await _validator.ValidateAsync(request, ct);
if (!validation.IsValid)
    throw new ValidationException(validation.Errors);
```

### Day 5: Audit Logging
```csharp
// All data changes are auto-logged via EF Core Interceptor
// Your job: ensure entity has these fields:
public DateTime CreatedAt { get; set; }
public Guid CreatedBy { get; set; }
public DateTime ModifiedAt { get; set; }
public Guid? ModifiedBy { get; set; }
public bool IsDeleted { get; set; }  // Soft delete
```

---

## 🔐 Week 3: Compliance & Testing

### Day 1-2: Tenant Isolation
```csharp
// EVERY query must filter by tenant
var items = await _repo.GetAsync(tenantId);  // ✅ CORRECT
var items = await _repo.GetAsync();           // ❌ SECURITY BREACH

// Verify in code review:
// - X-Tenant-ID header extracted
// - Validated against JWT claims
// - Passed to all repository methods
```

### Day 3-4: Write Tests
```bash
dotnet test backend/Domain/[Service]/tests/

# Minimum requirements:
- Happy path (success case)
- Error case (validation fail)
- Authorization (tenant isolation)
- Audit logging (change tracked)
```

### Day 5: Code Review Checklist
- [ ] Wolverine pattern used (NOT MediatR)
- [ ] FluentValidation for all commands
- [ ] Tenant ID in all queries
- [ ] Audit logging for data changes
- [ ] Soft deletes implemented
- [ ] Tests written (80%+ coverage)
- [ ] No hardcoded secrets
- [ ] CancellationToken passed through

---

## ⚡ Quick Commands

```bash
# Build
dotnet build B2Connect.slnx

# Test single service
dotnet test backend/Domain/Identity/tests/

# Test all
dotnet test B2Connect.slnx -v minimal

# Start local (InMemory database)
dotnet run --project AppHost/B2Connect.AppHost.csproj

# Database migration (if using PostgreSQL)
dotnet ef migrations add [MigrationName] --project backend/Domain/[Service]/src
dotnet ef database update --project backend/Domain/[Service]/src
```

---

## 📚 Critical Resources

| Topic | Link | Time |
|-------|------|------|
| Wolverine Patterns | `.github/copilot-instructions.md` | 30 min |
| CQRS Implementation | `docs/CQRS_WOLVERINE_PATTERN.md` | 20 min |
| Onion Architecture | `docs/ONION_ARCHITECTURE.md` | 20 min |
| Testing Framework | `docs/TESTING_FRAMEWORK_GUIDE.md` | 15 min |
| Security Requirements | `.github/copilot-instructions.md` §Security | 20 min |

---

## 🎯 First Task Assignment

**Create a new handler for [Feature]:**
1. Define command POCO (no IRequest!)
2. Create validator
3. Implement service handler
4. Write 3 tests (happy path, validation error, tenant isolation)
5. Submit for code review

**Estimated Time:** 3-4 hours  
**Success Criteria:** Tests passing + code review approved

---

## 📞 Getting Help

- **Architecture Question:** Ask Tech Lead
- **Wolverine Pattern Issue:** Reference CheckRegistrationTypeService.cs
- **Database Issue:** Check Infrastructure layer implementation
- **Security Question:** Ask Security Engineer

**Key Reminder:** If you see MediatR or [ApiController], you're in the wrong codebase section! 🚀
