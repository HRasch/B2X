# 🏗️ Wolverine Architecture Analysis & Error Prevention Guide

**Document Purpose:** Root cause analysis of the MediatR vs Wolverine architecture mistake and how to prevent it in future AI-assisted development.

**Created:** 27. Dezember 2025  
**Status:** Complete Analysis - Ready to Update Copilot Instructions

---

## Part 1: Root Cause Analysis

### The Mistake (What Happened)

**Scenario:** Asked to implement Story 8 backend handler

**First Attempt (WRONG):**
```csharp
// ❌ MediatR Pattern (not correct for this project)
public class CheckRegistrationTypeCommand : IRequest<CheckRegistrationTypeResponse> { }
public class CheckRegistrationTypeHandler : IRequestHandler<CheckRegistrationTypeCommand, CheckRegistrationTypeResponse> { }

// In Program.cs:
builder.Services.AddMediatR();
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Result: Build succeeded BUT architecturally wrong
```

**Error Detection:** User feedback → "Wieso hast du MediatR genutzt? Du sollst Wolverine nehmen"

**Correction (CORRECT):**
```csharp
// ✅ Wolverine Pattern (correct for this project)
public class CheckRegistrationTypeService
{
    public async Task<CheckRegistrationTypeResponse> CheckType(
        CheckRegistrationTypeCommand request,
        CancellationToken cancellationToken) { }
}

// In Program.cs:
builder.Services.AddScoped<CheckRegistrationTypeService>();
// MapWolverineEndpoints() already exists and auto-discovers handlers

// Result: Build succeeded AND architecturally correct
```

---

### Why This Happened: 3-Layer Root Cause

#### **Layer 1: Knowledge Gap (Primary)**
**What I Did:** Used global CQRS knowledge (MediatR is most popular)  
**What I Should Have Done:** Check project-specific pattern first

**Missing Information:**
- ❌ No explicit statement "This project uses Wolverine, NOT MediatR"
- ❌ No pattern comparison in instructions
- ❌ No architecture decision record (ADR)
- ❌ No reference to existing Wolverine handlers in codebase

#### **Layer 2: Insufficient Context (Secondary)**
**Current Copilot Instructions Say:**
> "Use Wolverine messaging (event bus) for eventual consistency"  
> "CQRS with Handlers (Application Layer) - each feature lives in a handler folder with Command/Query + Handler + Validator"

**Problem:** The example shows MediatR patterns, not Wolverine patterns!

```csharp
// WRONG EXAMPLE IN INSTRUCTIONS:
public record CreateProductCommand(string Sku, string Name, decimal Price) : IRequest<ProductDto>;
public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    // ...
}
```

This is **MediatR syntax**, not Wolverine!

#### **Layer 3: No Validation Checklist (Tertiary)**
**Missing:** Pre-implementation checklist to catch wrong patterns

---

## Part 2: Wolverine vs MediatR Comparison

### Architecture Comparison Table

| Aspect | MediatR | Wolverine |
|--------|---------|-----------|
| **Pattern** | Command Bus | Service-Based HTTP + Messaging |
| **Handler Interface** | `IRequest<T>`, `IRequestHandler<T, R>` | Plain POCO Service methods |
| **Routing** | Explicit attributes `[HttpPost]` | Method name convention |
| **HTTP Endpoint** | Traditional `[ApiController]` class | Service method name |
| **DI Config** | `AddMediatR()` + `AddControllers()` | Just `AddScoped<Service>()` |
| **Use Case** | In-process command bus | Microservices, distributed systems |
| **Response Type** | `IActionResult` or DTO | DTO directly |
| **Project Usage** | NOT used in B2X | ✅ Used in B2X |

### Code Pattern Comparison

#### MediatR Pattern (WRONG for B2X):
```csharp
// Step 1: Define command with IRequest
public record CheckTypeCommand(string Email) : IRequest<CheckTypeResponse>;

// Step 2: Create handler with IRequestHandler
public class CheckTypeHandler : IRequestHandler<CheckTypeCommand, CheckTypeResponse>
{
    public async Task<CheckTypeResponse> Handle(CheckTypeCommand request, CancellationToken ct)
    {
        return new CheckTypeResponse { /* ... */ };
    }
}

// Step 3: DI Configuration
builder.Services.AddMediatR();
builder.Services.AddControllers();

// Step 4: HTTP Endpoint
public class RegistrationController : ControllerBase
{
    private readonly IMediator _mediator;
    
    [HttpPost("check-type")]
    public async Task<IActionResult> CheckType([FromBody] CheckTypeCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return Ok(result);
    }
}

// Usage: POST /api/registration/check-type
```

#### Wolverine Pattern (CORRECT for B2X):
```csharp
// Step 1: Define command as plain POCO
public class CheckRegistrationTypeCommand
{
    public string Email { get; set; }
    // ... properties
}

// Step 2: Create service (NO IRequest/IRequestHandler)
public class CheckRegistrationTypeService
{
    // Convention: Method name → HTTP route
    // CheckType() → POST /checkregistrationtype
    public async Task<CheckRegistrationTypeResponse> CheckType(
        CheckRegistrationTypeCommand request,
        CancellationToken cancellationToken)
    {
        return new CheckRegistrationTypeResponse { /* ... */ };
    }
}

// Step 3: DI Configuration (simple!)
builder.Services.AddScoped<CheckRegistrationTypeService>();

// Step 4: No explicit controller needed!
// Wolverine auto-discovers and creates HTTP endpoint

// Usage: POST /api/registration/checktype
// (auto-generated from service namespace + method name)
```

---

## Part 3: Wolverine HTTP Endpoint Generation

### How Wolverine Auto-Discovers Handlers

**Mechanism:** Reflection-based automatic discovery

**Service Location Determines Route:**
```
Service Class: CheckRegistrationTypeService
Method Name: CheckType(CheckRegistrationTypeCommand, CancellationToken)
Namespace: B2X.Identity.Handlers

Generated Route: POST /checkregistrationtype
(HTTP verb inferred from method signature: async Task<T> = POST)

Full URL: http://localhost:7002/checkregistrationtype
```

### Wolverine Method Signature Conventions

```csharp
// ✅ CORRECT Wolverine Signatures:

// Simple async method
public async Task<Response> CheckType(CheckTypeCommand request, CancellationToken ct)

// With dependencies (auto-injected)
public async Task<Response> CheckType(
    CheckTypeCommand request,
    IService dependency,
    CancellationToken ct)

// Returning void (fire-and-forget)
public async Task HandleEvent(MyEvent @event)

// Returning value directly (no Task wrapper for simple sync operations)
public Result GetInfo(Query query)

// ❌ WRONG Signatures:

// MediatR syntax (IRequest interface)
public class CheckTypeCommand : IRequest<Response> { }

// Traditional controller attribute
public class Handler : IRequestHandler<Command, Response> { }

// Explicit route attributes
[HttpPost("/api/check")]
public async Task<IActionResult> CheckType() { }
```

### Wolverine Handler Discovery

**File:** [CheckRegistrationTypeService.cs](backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs)

```csharp
// Lines 44-48
public async Task<CheckRegistrationTypeResponse> CheckType(
    CheckRegistrationTypeCommand request,
    CancellationToken cancellationToken)
{
    // Wolverine automatically:
    // 1. Discovers this method
    // 2. Maps to HTTP POST endpoint
    // 3. Deserializes CheckRegistrationTypeCommand from request body
    // 4. Injects CancellationToken
    // 5. Serializes CheckRegistrationTypeResponse to response body
}
```

**Automatic HTTP Mapping:**
```
Method: CheckType(CheckRegistrationTypeCommand)
↓
HTTP Verb: POST (async Task<T> returns value)
HTTP Method: checktype (from method name, lowercase)
HTTP Endpoint: /checkregistrationtype
Full URL: POST /checkregistrationtype

Request Body: { "email": "...", "businessType": "..." }
Response Body: { "success": true, "registrationType": "...", ... }
```

---

## Part 4: Event Handling Pattern in Wolverine

### Message Handlers (Event Subscribers)

**File:** [UserEventHandlers.cs](backend/Domain/Identity/src/Handlers/Events/UserEventHandlers.cs)

```csharp
public class UserEventHandlers
{
    // ✅ Wolverine Event Handler Pattern
    public async Task Handle(UserRegisteredEvent @event)
    {
        // Automatically called when UserRegisteredEvent is published
        // Wolverine injects dependencies automatically
    }
    
    public async Task Handle(UserLoggedInEvent @event)
    {
        // Another event handler
    }
}

// Usage in service:
public class UserService
{
    private readonly IMessageBus _bus;
    
    public async Task RegisterUser(...)
    {
        // ... create user ...
        
        // Publish event
        await _bus.PublishAsync(new UserRegisteredEvent(...));
        
        // All Handle() methods automatically called by Wolverine
    }
}
```

**How It Works:**
1. **Define Event:** Plain POCO class
2. **Define Handler:** Public async method named `Handle(EventType @event)`
3. **Publish Event:** `await _bus.PublishAsync(new MyEvent())`
4. **Wolverine Magic:** Auto-discovers and executes all `Handle()` methods
5. **No IRequest/IRequestHandler:** Not needed!

---

## Part 5: Updated Copilot Instructions (FIXED)

### Current Instructions (INCORRECT)

```markdown
## 🔀 Inter-Service Communication

**Asynchronous**: Use Wolverine messaging (event bus)

### CQRS with Handlers (Application Layer)
Each feature lives in a handler folder with Command/Query + Handler + Validator:
```csharp
public record CreateProductCommand(string Sku, string Name, decimal Price) : IRequest<ProductDto>;
public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto> { }
```
```

**Problems:**
1. ❌ Example uses `IRequest<T>` (MediatR, not Wolverine)
2. ❌ Example uses `IRequestHandler<T, R>` (MediatR, not Wolverine)
3. ❌ No mention that these patterns are WRONG for this project
4. ❌ No validation checklist to prevent mistakes
5. ❌ No reference to actual Wolverine patterns in codebase

---

### Proposed FIXED Instructions

**Insert after "Dependency Injection" section in copilot-instructions.md:**

```markdown
## 🎯 CRITICAL: Wolverine HTTP Handlers (NOT MediatR!)

### ⚠️ Important Architecture Decision

**B2X uses Wolverine, NOT MediatR** for HTTP endpoints and event handling.

**Why this matters:** Wolverine is specifically designed for distributed microservices,
while MediatR is for in-process command bus patterns. B2X's architecture
requires Wolverine's event-driven messaging and HTTP endpoint auto-discovery.

---

### Wolverine Pattern for HTTP Endpoints

#### 1️⃣ Define Command as Plain POCO (NO IRequest interface)

```csharp
namespace B2X.Identity.Handlers;

public class CheckRegistrationTypeCommand
{
    public string Email { get; set; }
    public string BusinessType { get; set; }
}
```

**NOT LIKE THIS:**
```csharp
// ❌ WRONG: Don't use IRequest from MediatR
public record CreateProductCommand(string Sku, string Name) : IRequest<ProductDto>;
```

#### 2️⃣ Create Service Handler (Plain Class with Public Async Methods)

```csharp
namespace B2X.Identity.Handlers;

public class CheckRegistrationTypeService
{
    private readonly IErpCustomerService _erpService;
    private readonly ILogger<CheckRegistrationTypeService> _logger;

    public CheckRegistrationTypeService(
        IErpCustomerService erpService,
        ILogger<CheckRegistrationTypeService> logger)
    {
        _erpService = erpService;
        _logger = logger;
    }

    /// <summary>
    /// HTTP Endpoint: POST /checkregistrationtype
    /// Wolverine automatically maps method name to HTTP route
    /// </summary>
    public async Task<CheckRegistrationTypeResponse> CheckType(
        CheckRegistrationTypeCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing registration check for {Email}", request.Email);
        
        // Business logic here
        var response = new CheckRegistrationTypeResponse
        {
            Success = true,
            RegistrationType = RegistrationType.Bestandskunde
        };
        
        return response;
    }
}
```

**NOT LIKE THIS:**
```csharp
// ❌ WRONG: Don't implement IRequestHandler from MediatR
public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken ct) { }
}

// ❌ WRONG: Don't use [ApiController] and [HttpPost]
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand cmd) { }
}
```

#### 3️⃣ Register Service in Dependency Injection

```csharp
// Program.cs
var builder = WebApplicationBuilder.CreateBuilder(args);

// Add service handlers
builder.Services.AddScoped<CheckRegistrationTypeService>();
builder.Services.AddScoped<AnotherService>();

// MapWolverineEndpoints() already exists in base config
// It auto-discovers all services and creates HTTP endpoints
app.MapWolverineEndpoints();
```

**NOT LIKE THIS:**
```csharp
// ❌ WRONG: Don't add MediatR
builder.Services.AddMediatR();

// ❌ WRONG: Don't add traditional controllers
builder.Services.AddControllers();

// ❌ WRONG: Don't add validator assembly
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
```

#### 4️⃣ HTTP Usage (No Code Needed - Wolverine Handles It)

Wolverine automatically creates the HTTP endpoint:

```bash
# Request
POST /checkregistrationtype
Content-Type: application/json

{
  "email": "john@example.com",
  "businessType": "B2B"
}

# Response
HTTP/1.1 200 OK
Content-Type: application/json

{
  "success": true,
  "registrationType": "Bestandskunde",
  "erpCustomerId": "12345"
}
```

---

### Wolverine Pattern for Event Handling

#### 1️⃣ Define Event as Plain POCO

```csharp
namespace B2X.Identity.Events;

public class UserRegisteredEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
}
```

#### 2️⃣ Create Event Handler Service

```csharp
namespace B2X.Identity.Handlers.Events;

public class UserEventHandlers
{
    private readonly IEmailService _emailService;
    private readonly ILogger<UserEventHandlers> _logger;

    public UserEventHandlers(
        IEmailService emailService,
        ILogger<UserEventHandlers> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Wolverine automatically calls this when UserRegisteredEvent is published
    /// Method name "Handle" + event type is the convention
    /// </summary>
    public async Task Handle(UserRegisteredEvent @event)
    {
        _logger.LogInformation("User registered: {UserId}", @event.UserId);
        await _emailService.SendWelcomeEmailAsync(@event.Email);
    }

    public async Task Handle(UserLoggedInEvent @event)
    {
        // Another event handler
    }
}
```

#### 3️⃣ Register Event Handlers in DI

```csharp
// Program.cs
builder.Services.AddScoped<UserEventHandlers>();
```

#### 4️⃣ Publish Events in Services

```csharp
public class AuthService
{
    private readonly IMessageBus _messageBus;

    public AuthService(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task RegisterUserAsync(RegisterCommand cmd)
    {
        // ... create user ...
        var user = new User { Id = userId, Email = cmd.Email };
        await _repository.AddAsync(user);
        
        // Publish event
        await _messageBus.PublishAsync(new UserRegisteredEvent
        {
            UserId = userId,
            Email = cmd.Email
        });
        
        // Wolverine automatically calls UserEventHandlers.Handle(UserRegisteredEvent)
    }
}
```

---

### Validation Checklist: Before Implementing Handlers

Before you write ANY handler code, verify these requirements:

```
[ ] Wolverine Pattern Check
  [ ] Did I create plain POCO Command/Event class (no IRequest/IEvent interface)?
  [ ] Did I create Service class (not Handler with IRequestHandler)?
  [ ] Did I use public async Task<T> method names (not Handle)?
  [ ] Did I avoid [ApiController], [HttpPost], [Route] attributes?
  
[ ] DI Configuration Check
  [ ] Did I add AddScoped<MyService>()?
  [ ] Did I NOT add AddMediatR()?
  [ ] Did I NOT add AddControllers()?
  [ ] Is MapWolverineEndpoints() already in Program.cs?
  
[ ] Naming Convention Check
  [ ] Service method name matches HTTP endpoint (CheckType → /checkregistrationtype)?
  [ ] Event handlers named Handle(EventType @event)?
  [ ] No explicit route definitions?
  
[ ] Architecture Alignment Check
  [ ] Are dependencies passed through constructor (DI)?
  [ ] Is business logic in Service, not in handlers?
  [ ] Are events published for async operations?
  [ ] Is CancellationToken passed through for long operations?

[ ] Anti-Patterns Avoided
  [ ] No IRequest<T> interfaces
  [ ] No IRequestHandler<T, R> implementations
  [ ] No [ApiController] attributes
  [ ] No [HttpPost] decorators
  [ ] No AddMediatR() in DI
  [ ] No traditional controller routes
```

---

### Real Example: Story 8 Implementation

**Reference Implementation:** [backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs](backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs)

This is the **CORRECT pattern** for B2X. Use this as a template:

1. Service class in `/Handlers/` folder
2. Public async methods (not IRequestHandler)
3. POCO command classes
4. Event handlers in `/Handlers/Events/` subfolder
5. All following Wolverine conventions

**Examine this file before implementing similar features.**

---

### Resources

- **Wolverine Official:** https://wolverinefx.net/
- **GitHub Repo:** https://github.com/JasperFx/wolverine
- **B2X Pattern Ref:** See `UserEventHandlers.cs` in Identity service
- **DDD Foundation:** See `/backend/Domain/` structure

---

### Common Mistakes & Fixes

| Mistake | What I Did | What I Should Do | Fix |
|---------|-----------|-----------------|-----|
| Used `IRequest<T>` | `public class Cmd : IRequest<Res>` | Plain POCO | Remove `: IRequest<T>` |
| Used `IRequestHandler` | `public class H : IRequestHandler<C,R>` | Service class | Create public async method instead |
| Added explicit routes | `[HttpPost("/api/...")]` | No attributes | Delete attribute, use method name |
| Called handlers manually | `var h = new Handler(); await h.Handle(cmd)` | Publish via bus | `await _bus.PublishAsync(cmd)` |
| Registered as command | `AddMediatR()` | Register service | `AddScoped<MyService>()` |
| Traditional controller | `[ApiController] public class C : ControllerBase` | Service class | Delete class, create Service with async methods |

```

---

## Part 6: Complete Error Prevention System

### 1️⃣ Pre-Implementation Checklist

**When asked to implement a new handler/endpoint:**

```bash
STEP 1: Pattern Verification
- [ ] Is this project using Wolverine? YES (default in B2X)
- [ ] Should I use IRequest<T>? NO (that's MediatR)
- [ ] Should I create [ApiController]? NO (that's traditional ASP.NET Core)
- [ ] Should I add [HttpPost]? NO (method name IS the route)

STEP 2: Example Reference
- [ ] Check existing Wolverine service: CheckRegistrationTypeService.cs
- [ ] Check existing event handlers: UserEventHandlers.cs
- [ ] Copy structure, not MediatR patterns

STEP 3: Service Validation
- [ ] Is service a plain class? YES
- [ ] Are methods public async Task<T>? YES
- [ ] Is there CancellationToken parameter? YES
- [ ] Any IRequest interfaces? NO (should fail check)

STEP 4: DI Validation
- [ ] Is service registered as AddScoped? YES
- [ ] Is AddMediatR() present? NO (should fail)
- [ ] Is AddControllers() present? NO (should fail check)
- [ ] Is MapWolverineEndpoints() in middleware? YES

STEP 5: Naming Validation
- [ ] Method names follow convention? YES (MyMethod → /mymethod endpoint)
- [ ] Event handlers named Handle()? YES
- [ ] No explicit route definitions? YES (should pass)
```

### 2️⃣ Code Review Checklist

**Before committing Wolverine handler code:**

```
Architecture Review:
  [ ] No IRequest/IRequestHandler/ICommand interfaces
  [ ] Service class (not Controller)
  [ ] All methods are public async
  [ ] Dependencies injected via constructor
  [ ] Business logic separated from HTTP handling

Implementation Review:
  [ ] CancellationToken parameter present
  [ ] Proper error handling (try/catch or Result pattern)
  [ ] Input validation before processing
  [ ] Logging for debugging
  [ ] Response DTO properly structured

DI Configuration Review:
  [ ] Service registered with AddScoped
  [ ] No MediatR/Controllers references
  [ ] All dependencies resolvable
  [ ] Tests have proper mocking

Naming Convention Review:
  [ ] Service method name matches expected HTTP route
  [ ] Event handlers use Handle(EventType) pattern
  [ ] Folder structure follows /Handlers/ convention
  [ ] Namespace follows service pattern
```

### 3️⃣ Automated Validation (Grep-Based)

**Quick validation script to detect anti-patterns:**

```bash
#!/bin/bash
# Detect MediatR patterns in B2X

echo "🔍 Checking for MediatR anti-patterns in implementation..."

# Check for IRequest usage
if grep -r "IRequest<" backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs; then
    echo "❌ ERROR: Found IRequest<> interface (MediatR pattern)"
    exit 1
fi

# Check for IRequestHandler usage
if grep -r "IRequestHandler" backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs; then
    echo "❌ ERROR: Found IRequestHandler (MediatR pattern)"
    exit 1
fi

# Check for [ApiController]
if grep -r "\[ApiController\]" backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs; then
    echo "❌ ERROR: Found [ApiController] attribute (traditional ASP.NET)"
    exit 1
fi

# Check for [HttpPost]
if grep -r "\[HttpPost\]" backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs; then
    echo "❌ ERROR: Found [HttpPost] attribute"
    exit 1
fi

# Check for AddMediatR in Program.cs
if grep -r "AddMediatR" backend/Domain/Identity/src/Program.cs; then
    echo "❌ ERROR: Found AddMediatR in DI config"
    exit 1
fi

echo "✅ All Wolverine pattern checks passed!"
```

---

## Part 7: References

### Current Correct Implementation

**File:** [CheckRegistrationTypeService.cs](backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs)
- 195 lines
- Wolverine service handler
- Includes ERP service integration
- Includes duplicate detection
- Proper error handling
- Comprehensive logging

**File:** [UserEventHandlers.cs](backend/Domain/Identity/src/Handlers/Events/UserEventHandlers.cs)
- 5+ event handlers (Handle methods)
- Wolverine event subscriber pattern
- Async event processing
- Cross-service messaging

### Comparison for Reference

**MediatR Pattern (Wrong):**
```
File: RegistrationController.cs
Uses: [ApiController], [HttpPost], IMediator, await _mediator.Send()
Result: ❌ Deleted (was incorrect)
```

**Wolverine Pattern (Correct):**
```
File: CheckRegistrationTypeService.cs
Uses: Plain service class, public async methods, no IRequest
Result: ✅ Current implementation
```

---

## Summary

### The Key Insight

**The error happened because:**
1. MediatR is more widely known in .NET community
2. B2X uses less common Wolverine pattern
3. Copilot instructions didn't explicitly differentiate
4. No validation checklist to catch the mistake early
5. No reference to existing correct patterns in codebase

### How to Fix It

**Update [copilot-instructions.md](.github/copilot-instructions.md):**
1. ✅ Add "CRITICAL: Wolverine vs MediatR" section
2. ✅ Show pattern comparison side-by-side
3. ✅ Reference actual code examples from project
4. ✅ Add validation checklist
5. ✅ Document event handler pattern
6. ✅ Add common mistakes & fixes table

### Long-Term Prevention

1. **Architecture Decision Record (ADR):** Why Wolverine instead of MediatR?
2. **Code Examples:** Keep CheckRegistrationTypeService.cs as template
3. **Automated Checks:** Add pattern validation to CI/CD pipeline
4. **Training:** Document this in onboarding for new developers
5. **Consistency:** Apply same pattern to all new services

---

**Status:** ✅ Analysis Complete - Ready for Implementation  
**Next Step:** Update copilot-instructions.md with new Wolverine section

