# 📊 Architecture & Pattern Documentation

## Wolverine Framework (Correct Pattern for B2Connect)

### 📚 Documentation Files

| Document | Purpose | Audience |
|----------|---------|----------|
| **[WOLVERINE_QUICK_REFERENCE.md](WOLVERINE_QUICK_REFERENCE.md)** | Quick lookup for Wolverine patterns | Developers implementing features |
| **[WOLVERINE_ARCHITECTURE_ANALYSIS.md](WOLVERINE_ARCHITECTURE_ANALYSIS.md)** | Root cause analysis of MediatR mistake + error prevention | Architects, Lead developers |
| **[.github/copilot-instructions.md](.github/copilot-instructions.md)** | AI coding guidelines (UPDATED with Wolverine patterns) | AI assistants, Developers |

---

## Why This Matters

### The Problem That Happened
- Initial Story 8 implementation used **MediatR** (wrong pattern)
- B2Connect uses **Wolverine** (correct pattern)
- Copilot instructions had insufficient guidance
- No validation checklist to catch early

### The Solution
1. ✅ Converted implementation to Wolverine
2. ✅ Updated copilot instructions with explicit patterns
3. ✅ Created quick reference for developers
4. ✅ Documented root cause analysis for future prevention

---

## How to Use

### For New Feature Implementation

**Step 1:** Read [WOLVERINE_QUICK_REFERENCE.md](WOLVERINE_QUICK_REFERENCE.md)
- Find pattern that matches your use case
- Copy code template
- Adjust for your feature

**Step 2:** Use validation checklist
- [ ] Plain POCO command?
- [ ] Service class with async methods?
- [ ] No IRequest/IRequestHandler?
- [ ] Registered as AddScoped<Service>()?
- [ ] No AddMediatR()?

**Step 3:** Reference working examples
- [CheckRegistrationTypeService.cs](backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs) - HTTP endpoint
- [UserEventHandlers.cs](backend/Domain/Identity/src/Handlers/Events/UserEventHandlers.cs) - Event handlers

---

## Key Patterns

### ✅ CORRECT: Wolverine Service

```csharp
// 1. Plain POCO command
public class CreateProductCommand
{
    public string Name { get; set; }
}

// 2. Service with public async method
public class ProductService
{
    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand cmd,
        CancellationToken ct) { }
}

// 3. DI registration
builder.Services.AddScoped<ProductService>();
```

### ❌ WRONG: MediatR Pattern (DO NOT USE)

```csharp
// ❌ Wrong
public class CreateProductCommand : IRequest<Response> { }
public class Handler : IRequestHandler<CreateProductCommand, Response> { }
builder.Services.AddMediatR();
[ApiController, HttpPost]
public class ProductController { }
```

---

## Quick Lookup

**I need to...**

| Task | Pattern | File |
|------|---------|------|
| Create HTTP endpoint | Service with async method | Quick Reference Section 1 |
| Publish domain event | `await _bus.PublishAsync()` | Quick Reference Section 2 |
| Handle events | `Handle(EventType)` method | Quick Reference Section 2 |
| Validate input | FluentValidation in service | Quick Reference FAQ |
| Return error | Response DTO with Success flag | Quick Reference FAQ |
| Inject dependency | Constructor injection | Quick Reference FAQ |

---

## Error Prevention Checklist

Before committing handler code:

### Architecture Review
- [ ] No IRequest/IRequestHandler/ICommand interfaces
- [ ] Service class (not Controller)
- [ ] All methods are public async
- [ ] Dependencies injected via constructor

### Naming Convention
- [ ] Service method name matches expected HTTP route
- [ ] Event handlers use Handle(EventType) pattern
- [ ] Files in /Handlers/ folder
- [ ] Proper namespace structure

### DI Configuration
- [ ] Service registered with AddScoped
- [ ] No MediatR/Controllers references
- [ ] All dependencies resolvable
- [ ] MapWolverineEndpoints() present

### Code Quality
- [ ] CancellationToken parameter present
- [ ] Proper error handling
- [ ] Input validation
- [ ] Logging for debugging

---

## References

### Working Examples in Project

**Story 8 - Check Registration Type (HTTP Endpoint)**
- File: `backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs`
- Pattern: ✅ Wolverine service handler
- Features: ERP lookup, duplicate detection, response building

**Event Handlers (Domain Events)**
- File: `backend/Domain/Identity/src/Handlers/Events/UserEventHandlers.cs`
- Pattern: ✅ Multiple event handlers in one class
- Events: UserRegisteredEvent, UserLoggedInEvent, PasswordResetEvent, etc.

---

## Next Steps

1. **Developers:** Bookmark [WOLVERINE_QUICK_REFERENCE.md](WOLVERINE_QUICK_REFERENCE.md) for daily use
2. **Architects:** Review [WOLVERINE_ARCHITECTURE_ANALYSIS.md](WOLVERINE_ARCHITECTURE_ANALYSIS.md) for detailed comparison
3. **AI Assistants:** Follow updated [.github/copilot-instructions.md](.github/copilot-instructions.md) for code generation
4. **Teams:** Use validation checklist in code reviews

---

**Status:** ✅ Documentation Complete  
**Last Updated:** 27. Dezember 2025  
**Maintained by:** Architecture Team

