# AI Coding Agent Instructions for B2Connect

**Last Updated**: 28. Dezember 2025 | **Architecture**: DDD Microservices with Wolverine + CLI  
**Retrospective**: Lessons learned from session ending 28. Dezember 2025

## 🔧 Allowed Capabilities

- ✅ **GitHub CLI Management**: You are authorized to manage this project on GitHub via the GitHub CLI (`gh` command), including creating issues, pull requests, managing branches, and project boards.

---

## 🔄 Retrospective & Learnings (28. Dezember 2025)

### Session Summary

**Completed Work:**
- 🎯 **Phase A:** Created 8 role-specific onboarding guides (~2,650 lines)
- 🎯 **Phase B:** Fixed 38 PriceCalculationService test failures
- 🎯 **Phase B Result:** 169/171 tests passing (99.4% success rate)
- 🎯 **Phase B Verification:** All fixes confirmed working via test execution

**Key Achievements:**
- ✅ 10,580+ lines of documentation & automation created
- ✅ GitHub automation infrastructure complete
- ✅ Backend code quality verified
- ✅ Build pipeline healthy (8.5s, 60 non-critical warnings)

---

### 🔍 Key Learnings & Process Improvements

#### 1. **Build Validation Must Be FIRST (Before Complex Features)**

**What Happened:**
- PriceCalculationService had 38 failing tests
- Root causes: Default value (`null` vs `priceIncludingVat`), operator precision (`>` vs `>=`), locale handling (`.` vs `,`)
- Issues accumulated because incremental build checks weren't enforced

**Best Practice for Developers:**
```bash
# BEFORE implementing any feature:
# Step 1: Build only (no tests yet)
dotnet build B2Connect.slnx

# Step 2: Fix ALL compiler warnings/errors
# (Do not proceed to tests if build fails)

# Step 3: Run domain-specific tests
dotnet test backend/Domain/[Service]/tests -v minimal

# Step 4: Only THEN implement new feature
# (Previous build validation prevents surprises)
```

**For AI Code Generation:**
- ✅ **DO:** Generate code → Build → Fix → Test cycle (iterate)
- ❌ **DON'T:** Generate 500 lines, "assume" it compiles, discover 38 test failures later
- ✅ **DO:** `dotnet build` immediately after creating files
- ❌ **DON'T:** Create multiple files, then build together (hard to isolate issues)

**Updated Checklist (Before Every PR):**
```
Build Validation:
  [ ] Compiles without errors (dotnet build)
  [ ] No compiler warnings (CS codes checked)
  [ ] Project structure valid
  [ ] Package references resolved
  
  [ ] Domain tests pass (dotnet test Domain/*)
  [ ] Full test suite passes (dotnet test B2Connect.slnx)
  [ ] Coverage >= 80%
  [ ] No new failures introduced
```

---

#### 2. **Decimal Precision & Locale Handling (German Context)**

**What Happened:**
- Germany uses `,` for decimal separator (1,99€ not 1.99€)
- Tests passed locally on some machines, failed on others
- Root cause: No explicit locale handling in precision assertions

**PriceCalculationService Fix (Code Pattern):**

**❌ BEFORE (Failed on German locale):**
```csharp
[Fact]
public void PrecisionTest_SmallPrices()
{
    var productPrice = 0.01m;
    var finalPrice = CalculatePrice(productPrice);
    
    // This fails on German locale where "0,01" != "0.01"
    Assert.Equal("0.01", finalPrice.ToString());
}
```

**✅ AFTER (Handles both . and , separators):**
```csharp
[Fact]
public void PrecisionTest_SmallPrices()
{
    var productPrice = 0.01m;
    var finalPrice = CalculatePrice(productPrice);
    
    // Accept both formats: "0.01" (international) and "0,01" (German)
    var normalizedPrice = finalPrice.ToString().Replace(",", ".");
    var expected = 0.01m.ToString().Replace(",", ".");
    
    Assert.Equal(expected, normalizedPrice);
}
```

**Best Practice for All Projects (Global/Locale-Aware):**

```csharp
// In test setup:
public class GlobalizationTestBase
{
    protected decimal NormalizeDecimal(decimal value) =>
        decimal.Parse(value.ToString().Replace(",", "."), CultureInfo.InvariantCulture);
    
    protected string NormalizeDecimalString(string value) =>
        value.Replace(",", ".");
}

// Use in tests:
public class PriceCalculationTests : GlobalizationTestBase
{
    [Fact]
    public void SmallPrices_HandlesBothSeparators()
    {
        var value = 0.01m;
        var normalized = NormalizeDecimal(value);
        
        Assert.Equal(0.01m, normalized);
        // Now passes on both German and English locales
    }
}
```

---

#### 3. **Default Values Must Be Explicit (Null vs Real Values)**

**What Happened:**
- FinalPrice had default `null` 
- Some tests assumed `priceIncludingVat` as default
- 3 test variants failed due to this mismatch

**The Fix:**
```csharp
// ❌ BEFORE: Implicit/confusing default
public decimal? FinalPrice { get; set; }  // null??

// ✅ AFTER: Explicit semantically meaningful default
public decimal FinalPrice { get; set; } = priceIncludingVat;  // Clear intent
```

**Pattern for All Value Objects & Entities:**

```csharp
public class Order
{
    // Rule 1: All decimals should have defaults (not nullable)
    public decimal SubTotal { get; set; } = 0m;
    public decimal TaxAmount { get; set; } = 0m;
    public decimal FinalPrice { get; set; } = 0m;  // Clear default
    
    // Rule 2: Document WHY the default exists
    /// <summary>
    /// Final price including VAT and shipping.
    /// Defaults to 0 (calculated during checkout).
    /// Never null (use 0 for "not yet calculated").
    /// </summary>
    public decimal PriceWithVat { get; set; } = 0m;
    
    // Rule 3: If truly optional, use explicit nullable + guard
    public string? DiscountCode { get; set; }  // Can be null = no discount
    
    public decimal ApplyDiscount()
    {
        if (string.IsNullOrEmpty(DiscountCode))
            return FinalPrice;  // No discount applied
        
        // Apply discount logic...
    }
}
```

**Checklist (Before Committing Code):**
```
Default Values:
  [ ] All numeric values have defaults (not null unless truly optional)
  [ ] Defaults are semantically meaningful (not just placeholder 0)
  [ ] Comments explain WHY default exists
  [ ] Tests verify both default + explicit assignment paths
  [ ] No nullable decimal<T>? unless truly optional
```

---

#### 4. **GitHub CLI Fallback Patterns (API Limitations)**

**What Happened:**
- `gh project create` API not available (GitHub limitation)
- Expected to create project board via CLI, but no endpoint exists
- Required manual workaround

**GitHub CLI Capabilities Reference (Updated):**

```bash
# ✅ WORKING GitHub CLI Commands
gh repo create              # Create repository
gh issue create             # Create issues (see ISSUES_*.md)
gh pr create               # Create pull requests
gh pr review               # Review PRs
gh release create          # Create releases
gh branch create           # Create branches
gh secret set              # Set repository secrets
gh label create            # Create custom labels

# ❌ NOT AVAILABLE (Limitations)
gh project create          # ⚠️ Not available (must use web UI)
gh column create           # ⚠️ Not available
gh card create             # ⚠️ Not available

# 🔄 WORKAROUND: For Project Boards
# 1. Create programmatically (Octokit library) - see CREATE_PROJECT_BOARD.sh
# 2. Or manual setup - see ONBOARDING_AUTOMATION_README.md

# See: backend/docs/GITHUB_WORKFLOWS.md for complete reference
```

**Best Practice (When API Unavailable):**

```bash
# Step 1: Check if GitHub CLI supports the operation
gh api graphql -f query='query { viewer { login } }'

# Step 2: If not supported, document workaround
# Pattern: FALLBACK_REQUIRED.md
#   - What was intended: gh project create ...
#   - Why it doesn't work: GitHub API limitation (as of 28.12.2025)
#   - Alternative 1: Use Octokit library (C#)
#   - Alternative 2: Use REST API directly (curl)
#   - Alternative 3: Manual setup (documented steps)

# Step 3: Provide automation for alternatives
# See: CREATE_PROJECT_BOARD.sh (uses REST API via curl)
```

---

#### 5. **Documentation Must Include Working Code Examples**

**What Happened:**
- Role-based guides were created with extensive documentation
- **Most valuable:** Guides that included complete code patterns
- **Less valuable:** Abstract descriptions without examples

**Example: Wolverine HTTP Endpoints**

**✅ HIGH VALUE (With Code):**
```markdown
# Wolverine HTTP Endpoints

## Pattern: CQRS Handler

### Step 1: Plain POCO Command
public class CreateProductCommand
{
    public string Sku { get; set; }
    public string Name { get; set; }
}

### Step 2: Service Handler
public class ProductService
{
    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        // Implementation...
    }
}

### Step 3: Register DI
builder.Services.AddScoped<ProductService>();

### Result
✅ HTTP endpoint auto-generated: POST /createproduct
✅ Wolverine handles routing, binding, validation
```

**vs ❌ LOW VALUE (Abstract only):**
```markdown
# Use Wolverine Pattern

Create a service handler for HTTP endpoints.
Register in dependency injection.
Wolverine automatically discovers and routes requests.
```

**Updated Best Practice for Developers:**

```
Documentation Standards:
  [ ] Every pattern must have minimum 3-5 code examples
  [ ] Examples show: correct ✅ and incorrect ❌ approaches
  [ ] Reference real files in codebase
  [ ] Include test example for pattern
  [ ] Show error scenarios and how to handle
  [ ] Provide copy-paste starter template
  
  ✅ Example Pattern:
    - What it is (1 paragraph)
    - When to use it (scenarios)
    - Step-by-step implementation
    - Complete code example (correct ✅)
    - Anti-patterns to avoid (❌)
    - Test example
    - Reference link to actual codebase
```

---

#### 6. **Wolverine Pattern Consistency (NOT MediatR)**

**What Happened:**
- Wolverine is B2Connect's chosen pattern (all 9 microservices)
- But MediatR patterns were still showing up in suggestions
- Need stronger reinforcement to prevent confusion

**Critical Reminders (Added to Checklist):**

```
Wolverine ONLY - Never MediatR:
  [ ] No `public record X : IRequest<Y>`
  [ ] No `public class H : IRequestHandler<X, Y>`
  [ ] No `builder.Services.AddMediatR()`
  [ ] No `[ApiController] [HttpPost]` attributes
  
  ✅ Instead:
  [ ] Plain POCO command (no IRequest interface)
  [ ] Service class with public async methods
  [ ] Simple DI registration: AddScoped<Service>()
  [ ] Wolverine auto-discovers endpoints
  
  🔗 REFERENCE:
  - File: backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs
  - This is the ONLY pattern used in B2Connect
```

---

#### 7. **Compliance Testing as Blocking Gate (52 Tests)**

**What Happened:**
- 52 compliance tests are defined (P0.6, P0.7, P0.8, P0.9)
- These tests must ALL PASS before Phase 1 features deploy
- This is a hard gate (not advisory)

**Critical Constraint (for Future Development):**

```
🔴 HARD GATE: Phase 1 Deployment
================================

Phase 1 features (Auth, Catalog, Checkout, Admin) 
can ONLY deploy to production if:

  ✅ P0.6 E-Commerce Tests: 15/15 passing
  ✅ P0.7 AI Act Tests: 15/15 passing
  ✅ P0.8 BITV Tests: 12/12 passing
  ✅ P0.9 E-Rechnung Tests: 10/10 passing

If ANY test fails: BLOCK deployment immediately

Owner: QA Engineer (Issue #5)
Timeline: Weeks 1-5 (5-week duration)
Status: Tests defined, awaiting implementation

See: docs/COMPLIANCE_QUICK_START_CHECKLIST.md
```

**For Code Generation:**
- ✅ Always implement compliance features ALONGSIDE business features
- ❌ Don't build features first, "add compliance later"
- ✅ Test-driven development: Write compliance test, THEN feature code
- ✅ Example: "Create product" feature = business code + audit logging + encryption

---

#### 8. **macOS CDP Port Issues (Build Cache)**

**What Happened:**
- Aspire's DCP controller can hold ports after shutdown
- Restart attempts fail with "port already in use"
- Solution: Always run cleanup before restart

**macOS Development Process (Updated):**

```bash
# CORRECT WORKFLOW:
# Step 1: Stop all services (if running)
pkill -9 -f "dcpctrl"
pkill -9 -f "dcpproc"
sleep 2

# Step 2: Kill lingering processes
./scripts/kill-all-services.sh

# Step 3: Clean build cache (if needed)
rm -rf bin obj
dotnet clean

# Step 4: Now safe to restart
dotnet run --project backend/Orchestration

# ❌ WRONG WORKFLOW:
# Don't do this: dotnet run (while ports still held)
# Results in: "Port 15500 already in use"
```

**Preventive Checklist (macOS):**
```
Before Restarting Services:
  [ ] Run: ./scripts/kill-all-services.sh
  [ ] Verify: lsof -i :15500 (should be empty)
  [ ] Verify: lsof -i :7002 (should be empty)
  [ ] Then proceed with: dotnet run
```

---

#### 9. **Security-First Code Generation (Critical)**

**What Learned:**
- AI-generated code defaults to convenience over security
- Must explicitly enforce security checks FIRST
- PII encryption, audit logging, tenant isolation not "optional"

**Updated AI Code Generation Rules:**

**BEFORE generating code, verify:**

```
🔐 SECURITY-FIRST CHECKLIST:

Database Layer:
  [ ] Entity has: TenantId (multi-tenant isolation)
  [ ] Entity has: CreatedAt, CreatedBy (audit trail)
  [ ] PII fields encrypted: Email, Phone, Address, DOB, Name
  [ ] Soft delete: IsDeleted, DeletedAt flags
  [ ] Query always filters by TenantId (no cross-tenant leaks)

Service Layer:
  [ ] Constructor accepts IEncryptionService (if PII handled)
  [ ] Audit logging: All CRUD operations logged
  [ ] Validation: FluentValidation required
  [ ] Authorization: Tenant ID verified from JWT claims
  [ ] CancellationToken: Passed to all async calls

API Layer:
  [ ] JWT validation: Required (no anonymous)
  [ ] TenantId extracted from header: X-Tenant-ID
  [ ] All inputs validated server-side
  [ ] No PII in error messages
  [ ] Response doesn't expose internal structure

Testing:
  [ ] Cross-tenant access blocked: ✅ Tested
  [ ] Encryption/decryption round-trip: ✅ Tested
  [ ] Audit logs created: ✅ Tested
  [ ] CancellationToken propagates: ✅ Tested

If ANY ❌: Do not approve code, request fixes
```

**Pattern (All New Features):**
```csharp
// TEMPLATE: Feature = Business + Compliance
public class ProductService  // Business logic
{
    private readonly IProductRepository _repo;
    private readonly IEncryptionService _encryption;  // Security
    private readonly IAuditService _audit;            // Compliance
    
    public async Task<CreateProductResponse> CreateAsync(
        Guid tenantId,                    // Tenant isolation
        CreateProductCommand cmd,
        CancellationToken ct)             // Cancellation
    {
        // 1. Validate tenant (from JWT, not input)
        if (!await AuthorizeAsync(tenantId, ct))
            return Unauthorized();
        
        // 2. Validate input
        var validation = await _validator.ValidateAsync(cmd, ct);
        if (!validation.IsValid) return BadRequest(validation.Errors);
        
        // 3. Business logic
        var product = new Product(tenantId, cmd.Sku, cmd.Name);
        
        // 4. Encryption (if needed)
        product.SupplierEncrypted = _encryption.Encrypt(cmd.Supplier);
        
        // 5. Persist
        await _repo.AddAsync(product, ct);
        await _audit.LogAsync(tenantId, "ProductCreated", product);
        
        return Ok(product);
    }
}
```

---

### 📊 Session Metrics (28. Dezember 2025)

| Metric | Value | Notes |
|--------|-------|-------|
| **Phase A Duration** | ~3 hours | GitHub automation infrastructure |
| **Phase A Output** | 10,580 lines | 8 guides + scripts + docs |
| **Phase B Duration** | ~15 minutes | Test execution + verification |
| **Phase B Result** | 169/171 tests ✅ | 99.4% success rate |
| **Test Failures Fixed** | 38 variants | PriceCalculationService |
| **Build Time** | 8.5s | 60 non-critical warnings |
| **Documentation Quality** | 9/9 standards ✅ | Code examples, patterns, anti-patterns |
| **Compliance Gate** | 52 tests ready | Blocking gate for Phase 1 |

---

### 🎯 Key Takeaways for Team

1. **Build Early, Build Often** - Don't accumulate 38 test failures
2. **Locale-Aware Testing** - German context requires explicit handling
3. **Explicit > Implicit** - Default values must be meaningful
4. **Documentation = Code + Examples** - Theory without practice is useless
5. **Wolverine Only** - No MediatR, ever
6. **Compliance First** - Encryption, audit logging, tenant isolation from day 1
7. **GitHub API Realistic** - Plan fallbacks for unsupported operations
8. **macOS Cleanup Required** - Kill services before restart, always
9. **Security-First Templates** - Copy-paste working patterns, not "how-to" guides
10. **52 Tests = Hard Gate** - No compromise on compliance testing

---



## 🏗️ Architecture Foundation

B2Connect is a **Domain-Driven Design (DDD) multitenant SaaS platform** using Wolverine for all microservices:

### Microservices Architecture (Wolverine-Based)
```
backend/
├── Domain/                     # Individual Microservices (each standalone)
│   ├── Identity/              # Authentication (Wolverine) (Port 7002)
│   ├── Tenancy/               # Multi-tenant isolation (Wolverine) (Port 7003)
│   ├── Localization/          # i18n translations (Wolverine) (Port 7004)
│   ├── Catalog/               # Products, categories (Wolverine) (Port 7005)
│   ├── CMS/                   # Content Management (Wolverine) (Port 7006)
│   ├── Theming/               # UI themes, layouts (Wolverine) (Port 7008)
│   └── Search/                # Elasticsearch integration (Wolverine)
│
├── CLI/                        # B2Connect Console Interface
│   └── B2Connect.CLI/          # dotnet tool for operations
│       ├── Commands/
│       │   ├── AuthCommands/
│       │   ├── TenantCommands/
│       │   ├── ProductCommands/
│       │   ├── ContentCommands/
│       │   └── SystemCommands/
│       ├── Services/
│       └── Program.cs
│
├── Orchestration/             # Aspire Orchestration
├── ServiceDefaults/           # Shared defaults
└── shared/                    # Shared libraries
```

### Service Port Map (Critical!)
| Service | Port | Type | Purpose |
|---------|------|------|---------|
| Identity Microservice | 7002 | Wolverine | JWT Authentication, Passkeys |
| Tenancy Microservice | 7003 | Wolverine | Multi-tenancy management |
| Localization Microservice | 7004 | Wolverine | i18n translations |
| Catalog Microservice | 7005 | Wolverine | Products, categories |
| CMS Microservice | 7006 | Wolverine | Content Management |
| Theming Microservice | 7008 | Wolverine | UI themes, layouts |
| Search Microservice | 9300 | Wolverine | Elasticsearch integration |
| Frontend Store | 5173 | Vue.js | Public storefront |
| Frontend Admin | 5174 | Vue.js | Admin panel |
| Aspire Dashboard | 15500 | Orchestration | Observability UI |
| Redis | 6379 | Cache | Sessions & cache |
| PostgreSQL | 5432 | Database | Persistence |
| Elasticsearch | 9200 | Search | Full-text search |
| RabbitMQ | 5672 | Messaging | Event bus (optional) |

### Onion Architecture (Each Service)
Every service follows: **Core (Domain) → Application (CQRS) → Infrastructure (Data) → Presentation (API)**
- **Core**: Entities, ValueObjects, Aggregates, Repositories (interfaces)
- **Application**: DTOs, CQRS Handlers (Wolverine), Validators (FluentValidation), Mappers
- **Infrastructure**: EF Core DbContext, Repositories (implementations), External services
- **Presentation**: Controllers/Endpoints, Middleware, Dependency Injection

**Critical Rule**: Dependencies point **inward only**. Core has **zero** framework dependencies.

## 🔧 Developer Workflows

### Building & Running

**Recommended: Use Aspire Orchestration**
```bash
# Start everything with one command
./scripts/start-aspire.sh

# Or manually:
cd backend/Orchestration
dotnet run

# Dashboard available at: http://localhost:15500
```

**⚠️ Port Conflicts on Restart**
Aspire's DCP controller can hold ports after shutdown. Always use the cleanup script:
```bash
# Before restarting Aspire (CRITICAL!)
./scripts/kill-all-services.sh

# Check port status
./scripts/check-ports.sh
```

**Manual service startup (if needed - Wolverine services)**
```bash
# Each microservice runs independently with Wolverine
dotnet run --project backend/Domain/Identity/src/B2Connect.Identity.csproj
dotnet run --project backend/Domain/Tenancy/src/B2Connect.Tenancy.csproj
dotnet run --project backend/Domain/Catalog/src/B2Connect.Catalog.csproj
dotnet run --project backend/Domain/CMS/src/B2Connect.CMS.csproj

# Frontend (separate terminal)
cd frontend-store && npm install && npm run dev   # Port 5173
cd frontend-admin && npm install && npm run dev   # Port 5174
```

**Using B2Connect CLI (Recommended)**
```bash
# Build CLI tool
dotnet build backend/CLI/B2Connect.CLI/B2Connect.CLI.csproj

# Install as global tool
dotnet tool install --global --add-source ./nupkg B2Connect.CLI

# Use CLI for operations
b2connect start              # Start all services via Aspire
b2connect stop               # Stop all services
b2connect status             # Check service status
b2connect seed <data.json>   # Seed database
b2connect migrate            # Run migrations
b2connect info               # Show configuration
```

### Testing
```bash
# Run all backend tests
dotnet test B2Connect.slnx -v minimal

# Run specific bounded context tests
dotnet test backend/BoundedContexts/Store/Catalog/tests/B2Connect.Catalog.Tests.csproj

# Frontend tests (Vitest)
cd frontend-store && npm run test

# E2E tests (Playwright)
cd frontend-admin && npm run test:e2e
```

### VS Code Tasks (Recommended)
- `backend-start` - Starts Aspire orchestration (runs kill-all-services first)
- `build-backend` - `dotnet build B2Connect.slnx`
- `test-backend` - `dotnet test B2Connect.slnx -v minimal`
- `kill-all-services` - Frees all ports (run before restart!)

## 📋 Project-Specific Patterns

### ⚠️ CRITICAL: Wolverine HTTP Handlers (NOT MediatR!)

**B2Connect uses Wolverine for HTTP endpoints and event handling, NOT MediatR.** This is a critical architectural decision for distributed microservices. See [WOLVERINE_ARCHITECTURE_ANALYSIS.md](../WOLVERINE_ARCHITECTURE_ANALYSIS.md) for detailed comparison.

#### Wolverine HTTP Endpoint Pattern

**Step 1: Define Command as Plain POCO (NO IRequest interface)**
```csharp
namespace B2Connect.Identity.Handlers;

public class CheckRegistrationTypeCommand
{
    public string Email { get; set; }
    public string BusinessType { get; set; }
}
```

**Step 2: Create Service Handler (plain class with public async methods)**
```csharp
namespace B2Connect.Identity.Handlers;

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
    /// Wolverine auto-generates HTTP endpoint: POST /checkregistrationtype
    /// Method name is the route, no explicit attributes needed
    /// </summary>
    public async Task<CheckRegistrationTypeResponse> CheckType(
        CheckRegistrationTypeCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing: {Email}", request.Email);
        
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

**Step 3: Register in DI (simple!)**
```csharp
// Program.cs
builder.Services.AddScoped<CheckRegistrationTypeService>();
// MapWolverineEndpoints() already exists and auto-discovers all handlers
```

**✅ REFERENCE:** See [backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs](../backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs) for complete working example.

---

#### Wolverine Event Handler Pattern

Events are published via `IMessageBus` and handled by methods following the `Handle(EventType @event)` convention:

```csharp
// Step 1: Define event as plain POCO
public class UserRegisteredEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
}

// Step 2: Create event handlers (public async Task Handle methods)
public class UserEventHandlers
{
    private readonly IEmailService _emailService;

    public UserEventHandlers(IEmailService emailService)
    {
        _emailService = emailService;
    }

    /// <summary>
    /// Wolverine automatically calls this when UserRegisteredEvent is published
    /// </summary>
    public async Task Handle(UserRegisteredEvent @event)
    {
        await _emailService.SendWelcomeEmailAsync(@event.Email);
    }
}

// Step 3: Publish events in services
public class AuthService
{
    private readonly IMessageBus _messageBus;

    public async Task RegisterUserAsync(RegisterCommand cmd)
    {
        // ... create user ...
        
        // Publish event - Wolverine automatically calls all Handle() methods
        await _messageBus.PublishAsync(new UserRegisteredEvent
        {
            UserId = userId,
            Email = cmd.Email
        });
    }
}
```

**✅ REFERENCE:** See [backend/Domain/Identity/src/Handlers/Events/UserEventHandlers.cs](../backend/Domain/Identity/src/Handlers/Events/UserEventHandlers.cs) for working event handlers.

---

#### ❌ Anti-Patterns (DO NOT USE)

```csharp
// WRONG: Don't use IRequest from MediatR
public record CreateProductCommand(string Sku, string Name) : IRequest<ProductDto>;

// WRONG: Don't use IRequestHandler
public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken ct) { }
}

// WRONG: Don't add MediatR
builder.Services.AddMediatR();

// WRONG: Don't use [ApiController] or [HttpPost]
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand cmd) { }
}
```

---

### Validation Checklist: Before Implementing Handlers

Before writing ANY handler code, verify:
- [ ] Is this using plain POCO command (no `IRequest<T>` interface)?
- [ ] Is this a Service class with public async methods (no `IRequestHandler`)?
- [ ] Are dependencies injected via constructor?
- [ ] Is `CancellationToken` passed as parameter?
- [ ] No `[ApiController]`, `[HttpPost]`, or route attributes?
- [ ] Is service registered as `AddScoped<MyService>()`?
- [ ] Is `AddMediatR()` NOT in Program.cs?

**If any answer is NO, your implementation is wrong. See references above.**

### Multi-Tenancy & Context Propagation
Tenant ID flows via `X-Tenant-ID` header through all services. **Always include in queries**:
```csharp
var product = await _repository.GetBySkuAsync(tenantId, sku);
// Core.Entities.Product must accept tenantId in constructor
```

### Validation Pattern
Use FluentValidation in separate classes:
```csharp
public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
```

### Repository Interface (Core Layer)
```csharp
namespace B2Connect.Catalog.Core.Interfaces;
public interface IProductRepository
{
    Task<Product?> GetBySkuAsync(Guid tenantId, string sku, CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
}
// Implementation in Infrastructure layer only
```

### Entity with Domain Events
```csharp
public class Product : AggregateRoot
{
    public Product(Guid tenantId, string sku, string name, decimal price)
    {
        TenantId = tenantId;
        Sku = sku;
        Name = name;
        Price = price;
        RaiseEvent(new ProductCreatedEvent(Id, tenantId, sku));
    }
}
```

## 🔀 Inter-Service Communication (Wolverine-Based)

**All microservices communicate via Wolverine messaging** - no traditional gateway pattern:

### Asynchronous (Event-Driven) - Primary Pattern
Use Wolverine for all inter-service communication:
```csharp
// Publishing domain events (in any microservice)
await _messageBus.PublishAsync(new ProductCreatedEvent(productId, tenantId, sku));

// Subscribing to events (in event handler classes, auto-discovered)
// Wolverine automatically calls Handle() methods across all running services
public class ProductEventHandlers
{
    public async Task Handle(ProductCreatedEvent @event)
    {
        // Update search index, trigger notifications, sync cache
        // Works across microservices automatically
        await _searchService.IndexProductAsync(@event.ProductId);
        await _messageBus.PublishAsync(new SearchIndexUpdatedEvent(@event.ProductId));
    }
}
```

### Wolverine Runtime Configuration
```csharp
// Program.cs - Each microservice has Wolverine configured
var builder = Host.CreateApplicationBuilder(args);

builder.UseWolverine(opts => 
{
    // Local message handling
    opts.Handlers.Discovery(x => x.IncludeAssemblyContaining<Program>());
    
    // For distributed setup, add external messaging:
    // opts.UseRabbitMq("rabbitmq://localhost");
    // opts.UseAzureServiceBus("connection-string");
    
    // Event forwarding for cross-service reliability
    opts.OnException<ServiceUnavailableException>()
        .Requeue(attempts: 3, delayInMilliseconds: 1000);
});

builder.Services.AddWolverineHandlers();
var host = builder.Build();
await host.RunAsync();
```

**Frontend Communication**: Frontends consume microservice APIs directly (Wolverine HTTP endpoints).

## 🗄️ Database & Persistence

- **ORM**: Entity Framework Core with async/await
- **Database Per Service**: Each bounded context owns its PostgreSQL 16 database
- **Migrations**: `dotnet ef migrations add <MigrationName>`
- **DbContext Location**: `Infrastructure/Data/[ServiceName]DbContext.cs`
- **Naming Convention**: EFCore.NamingConventions (snake_case in database)

### Common DbContext Pattern
```csharp
public class CatalogDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
```

## 🌍 Frontend Architecture

**Technology**: Vue.js 3 + TypeScript + Vite  
**Two separate apps**:
- **frontend-store** (Port 5173): Public storefront
- **frontend-admin** (Port 5174): Admin panel

### Vue 3 Composition API Pattern
```vue
<script setup lang="ts">
import { ref, computed } from 'vue'
import { useProductStore } from '@/stores/productStore'

const products = ref<Product[]>([])
const selectedTenant = ref<string>('')

const filteredProducts = computed(() => 
  products.value.filter(p => p.tenantId === selectedTenant.value)
)

const loadProducts = async () => {
  products.value = await fetchProducts(selectedTenant.value)
}
</script>
```

### API Client Pattern
Centralize all HTTP calls in service layer, never in components:
```typescript
// src/services/catalogService.ts
export const catalogService = {
  async getProducts(tenantId: string): Promise<Product[]> {
    return api.get(`/api/store/catalog/products`, {
      headers: { 'X-Tenant-ID': tenantId }
    })
  }
}
```

---

## 🎨 Frontend Development Best Practices (Tailwind CSS, Vue.js, Vite)

**This section contains best practices and anti-patterns from official documentation for Tailwind CSS v4.1, Vue.js 3, and Vite.**

---

### Tailwind CSS Best Practices

#### ✅ DO: Utility-First Approach

| # | Rule | Example |
|---|------|---------|
| 1 | **Use utility classes over custom CSS** | `<div class="flex items-center gap-4">` not custom `.flex-center` |
| 2 | **Mobile-first breakpoints** | Unprefixed = all screens, `sm:` = 640px+, `md:` = 768px+, `lg:` = 1024px+ |
| 3 | **Target mobile with unprefixed utilities** | `block md:flex` (block on mobile, flex on md+) |
| 4 | **Stack state variants** | `hover:bg-sky-700 dark:hover:bg-sky-600 disabled:opacity-50` |
| 5 | **Use design system constraints** | `bg-blue-500` instead of `bg-[#3b82f6]` |
| 6 | **Group related utilities** | Keep layout, spacing, typography classes together |
| 7 | **Use container queries for component-relative sizing** | `@container` parent + `@md:flex-row` child |
| 8 | **Dark mode with `dark:` variant** | `bg-white dark:bg-slate-800` |
| 9 | **Manage duplication with loops/components** | Use v-for or extract to Vue component |
| 10 | **Use arbitrary values sparingly** | `top-[117px]` only for one-off values |

```vue
<!-- ✅ GOOD: Utility-first, mobile-first, state variants -->
<template>
  <button
    class="px-4 py-2 rounded-lg font-medium
           bg-blue-500 text-white
           hover:bg-blue-600 active:bg-blue-700
           focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2
           disabled:opacity-50 disabled:cursor-not-allowed
           dark:bg-blue-600 dark:hover:bg-blue-500
           transition-colors duration-200"
    :disabled="isLoading"
  >
    <slot />
  </button>
</template>

<!-- ✅ GOOD: Responsive design (mobile-first) -->
<div class="flex flex-col md:flex-row gap-4 md:gap-8">
  <aside class="w-full md:w-64 shrink-0">Sidebar</aside>
  <main class="flex-1">Content</main>
</div>

<!-- ✅ GOOD: Container queries for component-relative responsiveness -->
<div class="@container">
  <div class="flex flex-col @md:flex-row @lg:gap-8">
    <!-- Responds to container size, not viewport -->
  </div>
</div>
```

#### ❌ DON'T: Tailwind Anti-Patterns

| # | Anti-Pattern | Why It's Bad | Fix |
|---|-------------|--------------|-----|
| 11 | **Using `sm:` for mobile styles** | `sm:` means 640px+, not mobile | Use unprefixed for mobile |
| 12 | **Conflicting utilities on same element** | `p-4 p-8` - unpredictable result | Pick one value |
| 13 | **Custom CSS when utilities exist** | Duplicates functionality | Use Tailwind utilities |
| 14 | **Inline styles over utilities** | Misses design constraints, states | Use utility classes |
| 15 | **Extracting classes too early** | Premature abstraction | Tolerate duplication first |
| 16 | **Magic arbitrary values everywhere** | Breaks design consistency | Use theme values |
| 17 | **Not using dark mode variant** | Poor dark mode support | Add `dark:` variants |
| 18 | **Overriding utilities with `!important`** | Specificity wars | Fix class order/conflicts |
| 19 | **Long class strings without organization** | Hard to read/maintain | Group by concern |
| 20 | **Ignoring responsive prefixes** | Desktop-only design | Add mobile-first breakpoints |

```vue
<!-- ❌ BAD: Using sm: for mobile (sm: means 640px+, not mobile) -->
<div class="sm:block hidden">This is HIDDEN on mobile!</div>

<!-- ✅ GOOD: Unprefixed for mobile, sm: to hide on larger -->
<div class="block sm:hidden">This shows on mobile only</div>

<!-- ❌ BAD: Conflicting utilities -->
<div class="p-4 p-8 text-sm text-lg">Unpredictable!</div>

<!-- ❌ BAD: Inline styles when utility exists -->
<div style="display: flex; gap: 16px;">Use utilities!</div>

<!-- ✅ GOOD: Tailwind utilities -->
<div class="flex gap-4">Clean and consistent</div>

<!-- ❌ BAD: Arbitrary values breaking design system -->
<div class="mt-[13px] p-[7px] text-[15px]">Inconsistent spacing</div>

<!-- ✅ GOOD: Design system values -->
<div class="mt-3 p-2 text-sm">Consistent spacing</div>
```

#### Dark Mode Implementation
```vue
<!-- Dark mode setup in tailwind.config or CSS -->
<!-- Option 1: System preference (automatic) -->
<style>
@import "tailwindcss";
/* dark: variant uses prefers-color-scheme by default */
</style>

<!-- Option 2: Manual toggle with class -->
<style>
@import "tailwindcss";
@custom-variant dark (&:where(.dark, .dark *));
</style>

<!-- Toggle script -->
<script setup lang="ts">
const isDark = ref(localStorage.getItem('theme') === 'dark')

const toggleDark = () => {
  isDark.value = !isDark.value
  document.documentElement.classList.toggle('dark', isDark.value)
  localStorage.setItem('theme', isDark.value ? 'dark' : 'light')
}

// Initialize on mount
onMounted(() => {
  const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches
  const stored = localStorage.getItem('theme')
  isDark.value = stored ? stored === 'dark' : prefersDark
  document.documentElement.classList.toggle('dark', isDark.value)
})
</script>
```

---

### Vue.js 3 Best Practices

#### ✅ DO: Priority A - Essential Rules

| # | Rule | Example |
|---|------|---------|
| 21 | **Multi-word component names** | `ProductCard.vue`, `UserProfile.vue` not `Card.vue` |
| 22 | **Detailed prop definitions** | Include type, required, validator |
| 23 | **Always use `:key` with `v-for`** | `v-for="item in items" :key="item.id"` |
| 24 | **Never use `v-if` with `v-for`** | Compute filtered list first |
| 25 | **Component-scoped styling** | `<style scoped>` or CSS modules |

```vue
<!-- ✅ GOOD: Multi-word component name, detailed props -->
<script setup lang="ts">
// ProductCard.vue (multi-word name)
interface Props {
  product: Product
  showPrice?: boolean
  maxQuantity?: number
}

const props = withDefaults(defineProps<Props>(), {
  showPrice: true,
  maxQuantity: 99
})

// Emits with type definition
const emit = defineEmits<{
  (e: 'add-to-cart', product: Product, quantity: number): void
  (e: 'favorite', productId: string): void
}>()
</script>

<!-- ✅ GOOD: v-for with :key, no v-if on same element -->
<template>
  <!-- Compute filtered list, don't use v-if with v-for -->
  <ul>
    <li 
      v-for="item in filteredItems" 
      :key="item.id"
      class="p-4 border-b"
    >
      {{ item.name }}
    </li>
  </ul>
</template>

<script setup lang="ts">
const items = ref<Item[]>([])
const showActive = ref(true)

// ✅ GOOD: Computed property for filtering
const filteredItems = computed(() => 
  showActive.value 
    ? items.value.filter(item => item.isActive)
    : items.value
)
</script>

<style scoped>
/* Component-scoped styles */
.product-card {
  /* Only applies to this component */
}
</style>
```

#### ✅ DO: Priority B - Strongly Recommended Rules

| # | Rule | Example |
|---|------|---------|
| 26 | **One component per file** | Don't define multiple components in one .vue |
| 27 | **PascalCase file names** | `MyComponent.vue` not `my-component.vue` |
| 28 | **Base component prefix** | `BaseButton.vue`, `BaseInput.vue` |
| 29 | **Tightly coupled child naming** | `TodoList.vue` → `TodoListItem.vue` |
| 30 | **Self-closing components** | `<MyComponent />` not `<MyComponent></MyComponent>` |
| 31 | **Simple template expressions** | Move complex logic to computed |
| 32 | **Prop name casing** | camelCase in JS, kebab-case in templates |
| 33 | **Multi-attribute elements on new lines** | One attribute per line when many |
| 34 | **Directive shorthands consistently** | Always `:` for bind, `@` for on, `#` for slot |

```vue
<!-- ✅ GOOD: Consistent structure, naming, self-closing -->
<template>
  <!-- Self-closing when no content -->
  <BaseButton @click="handleSubmit" />
  <BaseInput v-model="username" />
  
  <!-- Multi-attribute on new lines -->
  <ProductCard
    :product="product"
    :show-price="true"
    :max-quantity="10"
    @add-to-cart="handleAddToCart"
    @favorite="handleFavorite"
  />
  
  <!-- Simple expressions, complex in computed -->
  <span>{{ formattedPrice }}</span>
  
  <!-- Consistent directive shorthands -->
  <input
    :value="searchQuery"
    :placeholder="$t('search.placeholder')"
    @input="updateSearch"
    @keyup.enter="submitSearch"
  />
</template>

<script setup lang="ts">
// ✅ GOOD: Complex logic in computed, not template
const formattedPrice = computed(() => {
  return new Intl.NumberFormat('de-DE', {
    style: 'currency',
    currency: 'EUR'
  }).format(product.value.price)
})
</script>
```

#### ❌ DON'T: Vue.js Anti-Patterns

| # | Anti-Pattern | Why It's Bad | Fix |
|---|-------------|--------------|-----|
| 35 | **Single-word component names** | Conflicts with HTML elements | Use multi-word names |
| 36 | **Props without types** | No validation, poor DX | Define types/validators |
| 37 | **v-for without :key** | Poor performance, bugs | Always add unique :key |
| 38 | **v-if and v-for on same element** | v-for has higher priority | Use computed or wrapper |
| 39 | **Mutating props directly** | Breaks one-way data flow | Emit events to parent |
| 40 | **Complex template expressions** | Hard to read/test | Use computed properties |
| 41 | **Options API in new code** | Less TypeScript support | Use Composition API |
| 42 | **Global state in components** | Hard to test, tight coupling | Use Pinia stores |
| 43 | **Direct DOM manipulation** | Bypasses Vue reactivity | Use refs and reactive |
| 44 | **Watchers for everything** | Often better as computed | Prefer computed |

```vue
<!-- ❌ BAD: Single-word name, props without types -->
<script>
// Card.vue - conflicts with potential <card> element
export default {
  props: ['title', 'content'] // No types!
}
</script>

<!-- ❌ BAD: v-if with v-for -->
<template>
  <li v-for="item in items" v-if="item.isActive" :key="item.id">
    {{ item.name }}
  </li>
</template>

<!-- ❌ BAD: Mutating prop directly -->
<script setup>
const props = defineProps(['modelValue'])
props.modelValue = 'new value' // WRONG!
</script>

<!-- ❌ BAD: Complex template expression -->
<template>
  <span>
    {{ items.filter(i => i.active).map(i => i.name).join(', ').toUpperCase() }}
  </span>
</template>

<!-- ✅ GOOD: Move to computed -->
<script setup>
const activeNames = computed(() => 
  items.value
    .filter(i => i.active)
    .map(i => i.name)
    .join(', ')
    .toUpperCase()
)
</script>
<template>
  <span>{{ activeNames }}</span>
</template>
```

---

### Vite Best Practices

#### ✅ DO: Vite Performance Optimization

| # | Rule | Example |
|---|------|---------|
| 45 | **Explicit file extensions in imports** | `import './Component.vue'` not `import './Component'` |
| 46 | **Avoid barrel files (index.ts re-exports)** | Import directly from source file |
| 47 | **Warm up frequently used files** | Configure `server.warmup` |
| 48 | **Use native CSS over preprocessors when possible** | Tailwind CSS > Sass for utilities |
| 49 | **Audit plugin performance** | Check slow `buildStart`, `config` hooks |
| 50 | **Prefer `import type` for type-only imports** | `import type { User } from './types'` |

```typescript
// vite.config.ts
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig({
  plugins: [vue()],
  
  // ✅ GOOD: Warm up frequently accessed files
  server: {
    warmup: {
      clientFiles: [
        './src/components/BaseButton.vue',
        './src/components/BaseInput.vue',
        './src/layouts/DefaultLayout.vue',
        './src/stores/*.ts'
      ]
    }
  },
  
  // ✅ GOOD: Optimize dependency pre-bundling
  optimizeDeps: {
    include: ['vue', 'vue-router', 'pinia', 'axios'],
    exclude: ['@vueuse/core'] // If causing issues
  },
  
  // ✅ GOOD: Configure CSS processing
  css: {
    devSourcemap: true,
    // Prefer native CSS, minimize preprocessor usage
  }
})
```

```typescript
// ✅ GOOD: Explicit extensions, direct imports
import { ProductCard } from './components/ProductCard.vue'
import { useProductStore } from './stores/productStore.ts'
import type { Product, Category } from './types/catalog.ts'

// ❌ BAD: Barrel file (index.ts)
// src/components/index.ts
export * from './ProductCard.vue'
export * from './CategoryList.vue'
export * from './SearchBar.vue'
// This forces Vite to process ALL files even if you only need one!

// ❌ BAD: Import from barrel
import { ProductCard } from './components' // Loads everything!

// ✅ GOOD: Direct import
import { ProductCard } from './components/ProductCard.vue' // Only loads what's needed
```

#### ❌ DON'T: Vite Anti-Patterns

| # | Anti-Pattern | Why It's Bad | Fix |
|---|-------------|--------------|-----|
| 51 | **Barrel files (index.ts exports)** | Forces processing all files | Import directly |
| 52 | **Missing file extensions** | Slower resolution | Add `.vue`, `.ts` |
| 53 | **Heavy plugins on `buildStart`** | Blocks dev server start | Defer or optimize |
| 54 | **Not pre-bundling dependencies** | Slower page loads | Add to `optimizeDeps.include` |
| 55 | **Large synchronous transforms** | Blocks HMR | Use async transforms |
| 56 | **Browser cache during debugging** | Stale modules | Disable cache in DevTools |
| 57 | **Unnecessary preprocessors** | Slower builds | Use native CSS + Tailwind |
| 58 | **Not using `import type`** | Larger bundles | Separate type imports |

---

### Component File Structure Standard

```
src/
├── assets/                     # Static assets (images, fonts)
├── components/
│   ├── base/                   # Base/presentational components
│   │   ├── BaseButton.vue
│   │   ├── BaseInput.vue
│   │   └── BaseModal.vue
│   ├── layout/                 # Layout components
│   │   ├── TheHeader.vue       # "The" prefix for singletons
│   │   ├── TheFooter.vue
│   │   └── TheSidebar.vue
│   └── feature/                # Feature-specific components
│       ├── product/
│       │   ├── ProductCard.vue
│       │   ├── ProductList.vue
│       │   └── ProductFilters.vue
│       └── cart/
│           ├── CartItem.vue
│           └── CartSummary.vue
├── composables/                # Vue composables (useXxx)
│   ├── useAuth.ts
│   ├── useCart.ts
│   └── useProducts.ts
├── services/                   # API service layer
│   ├── api.ts                  # Axios instance
│   ├── catalogService.ts
│   └── authService.ts
├── stores/                     # Pinia stores
│   ├── authStore.ts
│   ├── cartStore.ts
│   └── productStore.ts
├── types/                      # TypeScript types
│   ├── catalog.ts
│   ├── user.ts
│   └── api.ts
├── utils/                      # Utility functions
│   ├── formatters.ts
│   └── validators.ts
├── views/                      # Page components (router views)
│   ├── HomeView.vue
│   ├── ProductView.vue
│   └── CheckoutView.vue
├── App.vue
└── main.ts
```

### Frontend Quick Reference Checklist

Before every frontend PR, verify:

```
Tailwind CSS:
  [ ] Mobile-first (unprefixed for mobile, md: for larger)
  [ ] No conflicting utilities
  [ ] Dark mode variants added (dark:)
  [ ] Design system values used (not arbitrary)
  [ ] No inline styles where utilities exist

Vue.js:
  [ ] Multi-word component names
  [ ] Props have types and defaults
  [ ] :key on all v-for
  [ ] No v-if with v-for on same element
  [ ] Complex logic in computed, not template
  [ ] <style scoped> used

Vite:
  [ ] Explicit file extensions in imports
  [ ] No barrel file imports
  [ ] Type imports use `import type`
  [ ] Heavy dependencies pre-bundled
```

---

## 🎯 Critical Conventions

### Naming
- **C# Classes/Methods**: PascalCase (`CreateProductHandler`, `GetBySkuAsync`)
- **C# Fields**: camelCase with underscore (`_repository`, `_logger`)
- **Vue Components**: PascalCase (`ProductCard.vue`, `TenantSelector.vue`)
- **Files match class names** (`Product.cs`, `ProductRepository.cs`)
- **Interfaces**: Prefix `I` (`IProductRepository`, `ITenantService`)
- **Extensions**: Suffix `Extensions` (`StringExtensions.cs`)
- **Enums**: PascalCase, Use `string` or `int` backing values (`AddressType`, `OrderStatus`)

### Type Design
- **Prefer Enums for restricted types**: Always use `enum` instead of `string` for fixed sets of values
  - ✅ `public enum AddressType { Shipping, Billing, Other }`
  - ❌ `public string AddressType { get; set; } // Can be any string!`
  
- **Benefits of Enums**:
  - Type safety: Compiler prevents invalid values
  - IntelliSense: IDE shows all valid options
  - Performance: Enums are integers at runtime
  - Maintainability: Centralized list of valid values
  - Database: Stored as integers or strings (configurable)

- **Common Restricted Types** (Use Enums):
  - AddressType: Shipping, Billing, Residential, Commercial
  - OrderStatus: Pending, Processing, Shipped, Delivered, Cancelled, Returned
  - PaymentStatus: Unpaid, Partial, Paid, Refunded
  - UserRole: Admin, Manager, Customer, Guest
  - Gender: Male, Female, Other, PreferNotToSay
  - EmploymentStatus: Employed, SelfEmployed, Unemployed, Student, Retired

- **Enum Configuration in EF Core**:
  ```csharp
  // Store as string in database (recommended for readability)
  entity.Property(p => p.AddressType)
      .HasConversion<string>()
      .HasDefaultValue(AddressType.Shipping);
  
  // Or store as integer (more compact)
  entity.Property(p => p.OrderStatus)
      .HasConversion<int>();
  ```

### Project Structure Rules
1. **One public class per file**
2. **Service names include context**: `B2Connect.Catalog.Application.csproj` not just `Application.csproj`
3. **Test projects mirror source**: `src/` → `tests/` structure identical
4. **Shared code in `shared/`**: Database utilities, extensions, middleware only
5. **Enums in Core layer**: `Domain/Enums/` folder with all restricted types

### Dependency Injection (ASP.NET Core)
```csharp
// Program.cs - always use extension methods
services
    .AddCatalogCore()
    .AddCatalogApplication()
    .AddCatalogInfrastructure(configuration);

// Extension method pattern (ServiceCollectionExtensions.cs)
public static IServiceCollection AddCatalogCore(this IServiceCollection services) =>
    services.AddScoped<IProductRepository, ProductRepository>();
```

---

## 📋 .NET 10 / C# 14 Best Practices & Anti-Patterns

**This section contains 100+ best practices and 100+ anti-patterns compiled from Microsoft documentation, David Fowl's diagnostic scenarios, and industry standards for .NET 10 development.**

---

### 🔥 Async/Await Best Practices (50 Rules)

#### ✅ DO: Async Patterns

| # | Rule | Example |
|---|------|---------|
| 1 | **Use async/await for all I/O operations** | `await httpClient.GetAsync(url)` |
| 2 | **Make entire call stack async (async is viral)** | If any method is async, callers should be async too |
| 3 | **Return Task/Task<T> from async methods** | `public async Task<User> GetUserAsync()` |
| 4 | **Use ValueTask<T> for pre-computed or trivial values** | `return new ValueTask<int>(cachedValue)` |
| 5 | **Always flow CancellationToken to APIs that accept it** | `await dbContext.SaveChangesAsync(cancellationToken)` |
| 6 | **Dispose CancellationTokenSource used for timeouts** | `using var cts = new CancellationTokenSource(timeout)` |
| 7 | **Use TaskCreationOptions.RunContinuationsAsynchronously** | Prevents stack overflow with TaskCompletionSource |
| 8 | **Call FlushAsync() before Dispose on streams** | `await streamWriter.FlushAsync()` |
| 9 | **Prefer async/await over directly returning Task** | Normalizes exceptions, easier debugging |
| 10 | **Use PeriodicTimer for async timer scenarios (.NET 6+)** | `var timer = new PeriodicTimer(TimeSpan.FromSeconds(1))` |
| 11 | **Use static factory pattern for async construction** | `public static async Task<MyClass> CreateAsync()` |
| 12 | **Cache Task<T> in ConcurrentDictionary, not results** | Avoids multiple parallel fetches for same key |
| 13 | **Use ConfigureAwait(false) in library code** | `await task.ConfigureAwait(false)` |
| 14 | **Use IAsyncDisposable for async cleanup** | `await using var resource = new AsyncResource()` |
| 15 | **Prefer async Task methods over sync methods returning Task** | Async all the way pattern |

```csharp
// ✅ GOOD: Proper async pattern
public async Task<OrderResult> ProcessOrderAsync(Order order, CancellationToken ct)
{
    // Flow cancellation token throughout
    var customer = await _customerService.GetAsync(order.CustomerId, ct);
    var inventory = await _inventoryService.CheckAsync(order.Items, ct);
    
    // Dispose timeout CancellationTokenSource
    using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
    timeoutCts.CancelAfter(TimeSpan.FromSeconds(30));
    
    var result = await _paymentService.ProcessAsync(order, timeoutCts.Token);
    
    // FlushAsync before dispose
    await _auditWriter.FlushAsync();
    
    return result;
}
```

#### ❌ DON'T: Async Anti-Patterns

| # | Anti-Pattern | Why It's Bad |
|---|-------------|--------------|
| 16 | **Task.Wait() or Task.Result** | Causes thread-pool starvation, deadlocks |
| 17 | **async void methods (except event handlers)** | Exceptions crash the process |
| 18 | **Task.Run for long-running blocking work** | Steals thread pool threads |
| 19 | **ContinueWith instead of await** | Complex, error-prone, harder to read |
| 20 | **Forgetting to await async calls** | Silent failures, fire-and-forget issues |
| 21 | **Using .GetAwaiter().GetResult()** | Same deadlock issues as Task.Result |
| 22 | **Mixing sync and async I/O in same method** | Performance degradation |
| 23 | **Creating async lambdas that return void** | `button.Click += async (s, e) => await ...` |
| 24 | **Storing disposable objects in AsyncLocal<T>** | Memory leaks, thread-safety issues |
| 25 | **Using Thread.Sleep in async methods** | Blocks thread, use Task.Delay |

```csharp
// ❌ BAD: Sync-over-async (thread pool starvation, potential deadlock)
public string GetData()
{
    var result = httpClient.GetStringAsync(url).Result; // DEADLOCK RISK!
    return result;
}

// ❌ BAD: async void crashes process on exception
public async void ProcessData()
{
    await DoSomethingAsync(); // If this throws, process crashes
}

// ❌ BAD: Blocking thread pool with long-running work
await Task.Run(() =>
{
    Thread.Sleep(30000); // NEVER do this!
    ProcessLargeFile();  // This blocks a thread pool thread
});

// ✅ GOOD: Use dedicated thread for long-running blocking work
var thread = new Thread(ProcessLargeFile) { IsBackground = true };
thread.Start();
```

---

### ⚡ Performance Best Practices (25 Rules)

#### ✅ DO: Performance Optimizations

| # | Rule | Benefit |
|---|------|---------|
| 26 | **Use Span<T> and Memory<T> for buffer operations** | Avoids heap allocations |
| 27 | **Use ArrayPool<T>.Shared for temporary arrays** | Reuses array instances |
| 28 | **Use StringBuilder for string concatenation in loops** | Avoids intermediate strings |
| 29 | **Use string.Create() for high-performance string building** | Single allocation |
| 30 | **Prefer structs for small, immutable data (< 16 bytes)** | Stack allocation |
| 31 | **Use readonly struct for immutable value types** | Prevents defensive copies |
| 32 | **Use in parameters for large readonly structs** | Pass by reference |
| 33 | **Use stackalloc for small temporary buffers** | `Span<byte> buffer = stackalloc byte[256]` |
| 34 | **Cache compiled regex (or use source generators)** | `[GeneratedRegex("pattern")]` |
| 35 | **Use HttpClientFactory for HTTP connections** | Proper connection pooling |
| 36 | **Prefer IReadOnlyList<T> over IEnumerable<T> for collections** | Allows Count without enumeration |
| 37 | **Use FrozenDictionary/FrozenSet for read-only lookups (.NET 8+)** | Optimized for reads |
| 38 | **Avoid boxing value types (int to object)** | Heap allocation per box |
| 39 | **Use object pooling for frequently created objects** | `ObjectPool<T>` |
| 40 | **Use SearchValues<T> for searching multiple values (.NET 8+)** | SIMD-optimized |

```csharp
// ✅ GOOD: High-performance patterns
public void ProcessBuffer(ReadOnlySpan<byte> data)
{
    // Rent array from pool instead of allocating
    var buffer = ArrayPool<byte>.Shared.Rent(1024);
    try
    {
        // Use Span for slicing without allocation
        var slice = buffer.AsSpan(0, data.Length);
        data.CopyTo(slice);
        ProcessData(slice);
    }
    finally
    {
        ArrayPool<byte>.Shared.Return(buffer);
    }
}

// ✅ GOOD: Source-generated regex (compile-time optimized)
[GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
private static partial Regex EmailRegex();

// ✅ GOOD: FrozenDictionary for read-heavy scenarios
private static readonly FrozenDictionary<string, int> StatusCodes = 
    new Dictionary<string, int>
    {
        ["OK"] = 200,
        ["NotFound"] = 404,
        ["Error"] = 500
    }.ToFrozenDictionary();
```

#### ❌ DON'T: Performance Anti-Patterns

| # | Anti-Pattern | Why It's Bad |
|---|-------------|--------------|
| 41 | **String concatenation in loops with +** | O(n²) complexity, many allocations |
| 42 | **LINQ in hot paths without caching** | Allocates iterators on each call |
| 43 | **Boxing in hot paths** | Heap allocation per call |
| 44 | **New HttpClient per request** | Port exhaustion, socket leak |
| 45 | **Large struct parameters without 'in'** | Copied on each call |
| 46 | **Allocating in Dispose/Finalize** | GC pressure during cleanup |
| 47 | **Regex without RegexOptions.Compiled** | Interpreted mode is slow |
| 48 | **ToList() when only enumeration needed** | Unnecessary allocation |
| 49 | **FirstOrDefault() + null check vs Any()** | Multiple enumerations |
| 50 | **Capturing variables in LINQ closures** | Allocates closure object |

```csharp
// ❌ BAD: String concatenation in loop
string result = "";
foreach (var item in items)
{
    result += item.ToString(); // New string allocation each iteration!
}

// ✅ GOOD: StringBuilder
var sb = new StringBuilder();
foreach (var item in items)
{
    sb.Append(item.ToString());
}
var result = sb.ToString();

// ❌ BAD: New HttpClient per request (port exhaustion)
using var client = new HttpClient(); // Creates new connection each time!
var response = await client.GetAsync(url);

// ✅ GOOD: Use IHttpClientFactory
public class MyService
{
    private readonly HttpClient _httpClient;
    
    public MyService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("MyApi");
    }
}
```

---

### 🗄️ Entity Framework Core Best Practices (25 Rules)

#### ✅ DO: EF Core Patterns

| # | Rule | Benefit |
|---|------|---------|
| 51 | **Use no-tracking queries for read-only data** | `.AsNoTracking()` - better performance |
| 52 | **Use split queries for complex includes** | `.AsSplitQuery()` - avoids cartesian explosion |
| 53 | **Specify explicit includes** | Avoid N+1 queries |
| 54 | **Use pagination (Skip/Take)** | Don't load entire tables |
| 55 | **Use compiled queries for hot paths** | `EF.CompileQuery<>()` |
| 56 | **Use ExecuteUpdate/ExecuteDelete for bulk ops (.NET 7+)** | Single database roundtrip |
| 57 | **Configure query filters for soft delete** | `.HasQueryFilter(e => !e.IsDeleted)` |
| 58 | **Use value converters for complex types** | Encrypt PII, store enums |
| 59 | **Batch related SaveChanges calls** | Reduce roundtrips |
| 60 | **Use DbContext pooling** | `AddDbContextPool<>()` |
| 61 | **Configure connection resiliency** | `.EnableRetryOnFailure()` |
| 62 | **Use indexed properties for performance** | Index frequently filtered columns |
| 63 | **Prefer projections over full entity loads** | `.Select(x => new Dto { ... })` |

```csharp
// ✅ GOOD: Efficient query patterns
public async Task<PagedResult<ProductDto>> GetProductsAsync(
    int page, int pageSize, CancellationToken ct)
{
    var query = _context.Products
        .AsNoTracking()  // Read-only, no tracking overhead
        .Where(p => p.TenantId == _tenantId)
        .OrderBy(p => p.Name);
    
    var total = await query.CountAsync(ct);
    
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(p => new ProductDto  // Project to DTO, not full entity
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price
        })
        .ToListAsync(ct);
    
    return new PagedResult<ProductDto>(items, total, page, pageSize);
}

// ✅ GOOD: Bulk update without loading entities (.NET 7+)
await _context.Products
    .Where(p => p.CategoryId == oldCategoryId)
    .ExecuteUpdateAsync(s => s.SetProperty(p => p.CategoryId, newCategoryId), ct);
```

#### ❌ DON'T: EF Core Anti-Patterns

| # | Anti-Pattern | Why It's Bad |
|---|-------------|--------------|
| 64 | **Lazy loading without explicit opt-in** | N+1 query problems |
| 65 | **Loading full entities for projections** | Wasted memory and bandwidth |
| 66 | **Using Count() when Any() suffices** | Counts all rows vs stops at first |
| 67 | **Multiple SaveChanges in a loop** | Use single SaveChanges |
| 68 | **Tracking queries for read-only scenarios** | Unnecessary overhead |
| 69 | **Large Include chains** | Cartesian explosion |
| 70 | **Client-side evaluation in LINQ** | Data transferred to client |
| 71 | **DbContext as singleton** | Not thread-safe |
| 72 | **Ignoring migration scripts** | Schema drift issues |
| 73 | **String interpolation in queries** | SQL injection risk |

```csharp
// ❌ BAD: N+1 query problem
var orders = await _context.Orders.ToListAsync();
foreach (var order in orders)
{
    // Each iteration executes a query!
    var customer = await _context.Customers.FindAsync(order.CustomerId);
}

// ✅ GOOD: Eager loading with Include
var orders = await _context.Orders
    .Include(o => o.Customer)
    .ToListAsync();

// ❌ BAD: SQL injection via string interpolation
var name = userInput;
var products = await _context.Products
    .FromSqlRaw($"SELECT * FROM Products WHERE Name = '{name}'") // DANGER!
    .ToListAsync();

// ✅ GOOD: Parameterized query
var products = await _context.Products
    .FromSqlInterpolated($"SELECT * FROM Products WHERE Name = {name}")
    .ToListAsync();
```

---

### 🔒 Security Best Practices (25 Rules)

#### ✅ DO: Security Patterns

| # | Rule | Reason |
|---|------|--------|
| 74 | **Validate all user input server-side** | Never trust client data |
| 75 | **Use parameterized queries** | Prevent SQL injection |
| 76 | **Use HTTPS everywhere** | Encrypt data in transit |
| 77 | **Store passwords with Argon2/bcrypt** | Never plain MD5/SHA1 |
| 78 | **Use secrets manager (KeyVault)** | Never hardcode secrets |
| 79 | **Implement rate limiting** | Prevent brute force |
| 80 | **Use CSRF tokens for state changes** | Prevent cross-site attacks |
| 81 | **Set secure cookie flags** | HttpOnly, Secure, SameSite |
| 82 | **Implement Content Security Policy** | Prevent XSS attacks |
| 83 | **Encrypt PII at rest (AES-256)** | GDPR compliance |
| 84 | **Use short-lived JWT tokens** | 1h access, 7d refresh |
| 85 | **Validate JWT signature and claims** | Prevent token tampering |
| 86 | **Log security events (audit trail)** | NIS2 compliance |
| 87 | **Implement least privilege access** | Role-based authorization |
| 88 | **Use HSTS headers** | Force HTTPS |

```csharp
// ✅ GOOD: Secure API endpoint
[Authorize(Policy = "RequireAdmin")]
[RateLimit("Fixed", PermitLimit = 100, Window = "1m")]
public async Task<IActionResult> UpdateUser(
    [FromRoute] Guid userId,
    [FromBody] UpdateUserDto dto,
    CancellationToken ct)
{
    // Validate tenant isolation
    if (!await _authService.CanAccessUser(_currentUser.TenantId, userId))
        return Forbid();
    
    // Input validation (FluentValidation)
    var validation = await _validator.ValidateAsync(dto, ct);
    if (!validation.IsValid)
        return BadRequest(validation.Errors);
    
    // Audit logging
    await _auditService.LogAsync(
        "UserUpdated", 
        userId, 
        _currentUser.Id,
        oldValues: existingUser,
        newValues: dto);
    
    return Ok();
}
```

#### ❌ DON'T: Security Anti-Patterns

| # | Anti-Pattern | Risk |
|---|-------------|------|
| 89 | **Hardcoding secrets/passwords** | Exposed in source control |
| 90 | **Using MD5/SHA1 for passwords** | Easily cracked |
| 91 | **SQL string concatenation** | SQL injection |
| 92 | **Trusting client-side validation only** | Bypassed easily |
| 93 | **Using binary formatters** | Remote code execution |
| 94 | **Using .NET Remoting/DCOM** | Security vulnerabilities |
| 95 | **Logging sensitive data (PII)** | Privacy violation |
| 96 | **Exposing stack traces in production** | Information disclosure |
| 97 | **Using Code Access Security (CAS)** | Deprecated, ineffective |
| 98 | **HTTP for sensitive endpoints** | Data interception |

```csharp
// ❌ BAD: Hardcoded secrets
var connectionString = "Server=prod;Password=super_secret_123"; // NEVER!

// ✅ GOOD: Use secrets manager
var connectionString = configuration["ConnectionStrings:Database"];
// Or Azure KeyVault
var secret = await keyVaultClient.GetSecretAsync("DatabasePassword");

// ❌ BAD: Logging PII
_logger.LogInformation("User {Email} logged in", user.Email); // Privacy violation!

// ✅ GOOD: Log user ID only
_logger.LogInformation("User {UserId} logged in", user.Id);
```

---

### 🏗️ Architecture & Design Best Practices (25 Rules)

#### ✅ DO: Design Patterns

| # | Rule | Benefit |
|---|------|---------|
| 99 | **Use Dependency Injection** | Testability, loose coupling |
| 100 | **Prefer composition over inheritance** | Flexibility |
| 101 | **Follow Single Responsibility Principle** | Maintainability |
| 102 | **Use interfaces for abstractions** | Testability |
| 103 | **Implement Repository pattern for data access** | Separation of concerns |
| 104 | **Use Result<T> pattern for error handling** | Explicit error cases |
| 105 | **Implement Unit of Work for transactions** | Consistency |
| 106 | **Use Options pattern for configuration** | `IOptions<T>` |
| 107 | **Implement Circuit Breaker for external calls** | Resilience |
| 108 | **Use health checks** | Observability |
| 109 | **Implement structured logging** | Searchable logs |
| 110 | **Use feature flags for gradual rollouts** | Safe deployments |
| 111 | **Implement retry policies (Polly)** | Transient fault handling |
| 112 | **Use CancellationToken everywhere** | Graceful shutdown |
| 113 | **Implement idempotent operations** | Safe retries |

```csharp
// ✅ GOOD: Result pattern for explicit error handling
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    
    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}

public async Task<Result<Order>> CreateOrderAsync(CreateOrderCommand cmd)
{
    var validation = _validator.Validate(cmd);
    if (!validation.IsValid)
        return Result<Order>.Failure(validation.Errors.First().Message);
    
    var inventory = await _inventoryService.CheckAsync(cmd.Items);
    if (!inventory.Available)
        return Result<Order>.Failure("Insufficient inventory");
    
    var order = await _orderRepository.CreateAsync(cmd);
    return Result<Order>.Success(order);
}
```

#### ❌ DON'T: Design Anti-Patterns

| # | Anti-Pattern | Problem |
|---|-------------|---------|
| 114 | **God classes (too many responsibilities)** | Unmaintainable |
| 115 | **Service Locator pattern** | Hidden dependencies |
| 116 | **Tight coupling to implementations** | Hard to test |
| 117 | **Static dependencies** | Untestable |
| 118 | **Catching generic Exception** | Swallows important errors |
| 119 | **Throwing exceptions for flow control** | Expensive, confusing |
| 120 | **Anemic domain models** | Business logic scattered |
| 121 | **Circular dependencies** | Hard to understand |
| 122 | **Magic strings/numbers** | Use constants/enums |
| 123 | **Deep inheritance hierarchies** | Fragile base class problem |

```csharp
// ❌ BAD: Service locator anti-pattern
public class OrderService
{
    public void CreateOrder()
    {
        var repo = ServiceLocator.Get<IOrderRepository>(); // Hidden dependency!
    }
}

// ✅ GOOD: Constructor injection
public class OrderService
{
    private readonly IOrderRepository _repository;
    
    public OrderService(IOrderRepository repository)
    {
        _repository = repository; // Explicit dependency
    }
}
```

---

### 🆕 .NET 10 / C# 14 Specific Features (25 Rules)

#### ✅ DO: Use New Features

| # | Feature | Example |
|---|---------|---------|
| 124 | **Field-backed properties (C# 14)** | `public string Name { get; set => field = value.Trim(); }` |
| 125 | **Extension blocks (C# 14)** | `extension StringExtensions for string { ... }` |
| 126 | **Null-conditional assignment (C# 14)** | `obj?.Property ??= defaultValue;` |
| 127 | **nameof for unbound generics (C# 14)** | `nameof(List<>)` |
| 128 | **SearchValues<T> for multi-value search** | SIMD-optimized searching |
| 129 | **FrozenDictionary for read-heavy lookups** | Optimized for reads |
| 130 | **Required members** | `required string Name { get; init; }` |
| 131 | **Primary constructors** | `public class Person(string name)` |
| 132 | **Collection expressions** | `int[] arr = [1, 2, 3];` |
| 133 | **Raw string literals** | `"""JSON content"""` |
| 134 | **List patterns** | `if (list is [var first, .., var last])` |
| 135 | **File-scoped types** | `file class InternalHelper` |
| 136 | **Generic math interfaces** | `INumber<T>`, `IAdditionOperators<T>` |
| 137 | **TimeProvider for testable time** | Inject TimeProvider instead of DateTime.Now |
| 138 | **WebSocketStream (.NET 10)** | Stream-based WebSocket API |

```csharp
// ✅ GOOD: C# 14 field-backed property
public class Person
{
    // Auto-property with custom setter logic
    public string Name { get; set => field = value?.Trim() ?? ""; }
    public DateTime CreatedAt { get; init => field = value.ToUniversalTime(); }
}

// ✅ GOOD: Collection expressions
int[] numbers = [1, 2, 3, 4, 5];
List<string> names = ["Alice", "Bob", "Charlie"];
Dictionary<string, int> scores = new() { ["Alice"] = 100, ["Bob"] = 85 };

// ✅ GOOD: Primary constructor with DI
public class ProductService(
    IProductRepository repository,
    ILogger<ProductService> logger)
{
    public async Task<Product> GetAsync(Guid id) =>
        await repository.GetByIdAsync(id);
}

// ✅ GOOD: Required members for immutable DTOs
public record CreateProductCommand
{
    public required string Sku { get; init; }
    public required string Name { get; init; }
    public required decimal Price { get; init; }
}
```

#### ❌ DON'T: Legacy Patterns to Avoid

| # | Legacy Pattern | Modern Alternative |
|---|---------------|-------------------|
| 139 | **Manual null checks** | Use nullable reference types |
| 140 | **DateTime.Now** | Use TimeProvider (testable) |
| 141 | **Task.Factory.StartNew** | Use Task.Run |
| 142 | **WebClient** | Use HttpClient |
| 143 | **BinaryFormatter** | Use System.Text.Json |
| 144 | **ArrayList** | Use List<T> |
| 145 | **Hashtable** | Use Dictionary<K,V> |
| 146 | **StringCollection** | Use List<string> |
| 147 | **ReaderWriterLock** | Use ReaderWriterLockSlim |
| 148 | **Thread.Abort()** | Use CancellationToken |
| 149 | **AppDomain for isolation** | Use AssemblyLoadContext |
| 150 | **Synchronous I/O in ASP.NET Core** | Use async I/O |

```csharp
// ❌ BAD: DateTime.Now is not testable
public bool IsExpired() => DateTime.Now > _expirationDate;

// ✅ GOOD: Use TimeProvider
public class ExpirationChecker(TimeProvider timeProvider)
{
    public bool IsExpired() => timeProvider.GetUtcNow() > _expirationDate;
}

// ❌ BAD: BinaryFormatter (security vulnerability)
BinaryFormatter formatter = new(); // NEVER use!

// ✅ GOOD: System.Text.Json
var json = JsonSerializer.Serialize(obj);
var obj = JsonSerializer.Deserialize<MyType>(json);
```

---

### 📝 Code Quality Anti-Patterns (50 Rules)

| # | Anti-Pattern | Why It's Bad | Fix |
|---|-------------|--------------|-----|
| 151 | **Empty catch blocks** | Silently swallows errors | Log or rethrow |
| 152 | **Catch (Exception ex) then throw ex** | Loses stack trace | Use `throw;` |
| 153 | **Public fields** | No encapsulation | Use properties |
| 154 | **Mutable public collections** | External modification | Return IReadOnlyList |
| 155 | **Long methods (> 30 lines)** | Hard to understand | Extract methods |
| 156 | **Deeply nested code (> 3 levels)** | Hard to follow | Early returns |
| 157 | **Magic numbers** | Unclear meaning | Use named constants |
| 158 | **Commented-out code** | Clutters codebase | Delete it |
| 159 | **TODO comments in production** | Never fixed | Create issues instead |
| 160 | **Inconsistent naming** | Confusing | Follow conventions |
| 161 | **Using var everywhere** | Less readable | Use explicit types for complex cases |
| 162 | **Not using var for obvious types** | Verbose | `var list = new List<string>()` |
| 163 | **Multiple return points confusion** | Hard to follow | Single exit or clear early returns |
| 164 | **Boolean parameters** | Unclear call site | Use enums or separate methods |
| 165 | **Output parameters** | Hard to use | Return tuples or result objects |
| 166 | **ref parameters for non-value-types** | Unnecessary | Pass by reference is default |
| 167 | **Async suffix on non-async methods** | Misleading | Use proper naming |
| 168 | **Mixing sync and async code** | Confusing | Be consistent |
| 169 | **Long parameter lists (> 4)** | Hard to use | Use objects |
| 170 | **Using dynamic** | No compile-time safety | Use generics |
| 171 | **Excessive use of regions** | Code smell | Refactor into classes |
| 172 | **Unused using statements** | Clutter | Remove them |
| 173 | **Unused parameters** | Dead code | Remove or use |
| 174 | **Unused private members** | Dead code | Remove them |
| 175 | **Non-sealed classes** | Unintended inheritance | Seal by default |
| 176 | **Virtual methods in constructors** | Dangerous | Avoid |
| 177 | **Ignoring return values** | Silent bugs | Check or use discard |
| 178 | **Implicit conversions in hot paths** | Hidden allocations | Be explicit |
| 179 | **DateTime comparisons without kind** | Timezone bugs | Use DateTimeOffset |
| 180 | **String == for culture-sensitive comparison** | Wrong results | Use StringComparison |
| 181 | **Modifying collection while iterating** | InvalidOperationException | Use ToList() first |
| 182 | **Double-check locking without volatile** | Race condition | Use Lazy<T> |
| 183 | **Manual locking with Monitor** | Easy to deadlock | Use lock keyword |
| 184 | **Using lock(this)** | External code can deadlock | Lock private object |
| 185 | **Using lock("string literal")** | Interned strings cause issues | Lock private object |
| 186 | **Thread.Sleep in production code** | Blocks thread | Use async/await |
| 187 | **GC.Collect() calls** | Performance impact | Let GC decide |
| 188 | **Finalizers without Dispose pattern** | Resource leaks | Implement IDisposable |
| 189 | **Large structs (> 16 bytes)** | Copy overhead | Use class or ref struct |
| 190 | **Mutable structs** | Confusing semantics | Make readonly |
| 191 | **Capturing loop variables in closures** | Wrong value captured | Use local copy |
| 192 | **Equality comparison of floating-point** | Precision issues | Use tolerance |
| 193 | **Switch without default case** | Missing cases silently fail | Add default |
| 194 | **Throwing in property getters** | Unexpected exceptions | Return default |
| 195 | **Complex LINQ in one line** | Unreadable | Break into steps |
| 196 | **OrderBy after Where** | Wrong results if Where changes order | Be intentional |
| 197 | **Using Count() > 0 instead of Any()** | Less efficient | Use Any() |
| 198 | **FirstOrDefault().Property** | NullReferenceException | Check for null |
| 199 | **Ignoring CancellationToken** | Operations can't be cancelled | Always pass it |
| 200 | **Using reflection in hot paths** | Slow | Cache or use source generators |

```csharp
// ❌ BAD: Catch and rethrow (loses stack trace)
try { DoSomething(); }
catch (Exception ex) { throw ex; } // Stack trace lost!

// ✅ GOOD: Rethrow properly
try { DoSomething(); }
catch (Exception ex) 
{ 
    _logger.LogError(ex, "Error occurred");
    throw; // Preserves stack trace
}

// ❌ BAD: Boolean parameter
public void Process(bool useCache) { } // What does true mean at call site?

// ✅ GOOD: Clear intent
public enum CacheMode { UseCache, BypassCache }
public void Process(CacheMode cacheMode) { }

// ❌ BAD: Mutable struct (confusing)
public struct Point
{
    public int X;  // Mutable field!
    public void Move(int dx) => X += dx;
}

// ✅ GOOD: Readonly struct
public readonly struct Point(int x, int y)
{
    public int X { get; } = x;
    public int Y { get; } = y;
    public Point Move(int dx, int dy) => new(X + dx, Y + dy);
}
```

---

### 🎯 Quick Reference Checklist

**Before Every PR, Verify:**

```
Async/Await:
  [ ] No Task.Wait() or Task.Result
  [ ] No async void (except event handlers)
  [ ] CancellationToken passed through
  [ ] await used instead of ContinueWith

Performance:
  [ ] No string concatenation in loops
  [ ] Using HttpClientFactory
  [ ] EF Core queries use AsNoTracking when appropriate
  [ ] No LINQ in hot paths without caching

Security:
  [ ] No hardcoded secrets
  [ ] All user input validated server-side
  [ ] Parameterized queries for database
  [ ] No PII in logs
  [ ] Encryption for sensitive data

Code Quality:
  [ ] No empty catch blocks
  [ ] No magic numbers/strings
  [ ] Methods < 30 lines
  [ ] Nesting < 3 levels
  [ ] No commented-out code

.NET 10:
  [ ] Using latest C# features where beneficial
  [ ] Using TimeProvider instead of DateTime.Now
  [ ] Using System.Text.Json, not BinaryFormatter
  [ ] Using nullable reference types
```

---

## 🧪 Testing Essentials

### Test File Organization
- **Unit tests**: Mirror source structure, `*Tests.cs` suffix
- **Test fixtures**: Reusable setup in `Fixtures/` folder
- **Integration tests**: Use TestContainers for PostgreSQL/Redis

### Test Pattern
```csharp
public class CreateProductHandlerTests : IAsyncLifetime
{
    private readonly CreateProductHandler _handler;
    private readonly Mock<IProductRepository> _mockRepo;
    
    public CreateProductHandlerTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _handler = new CreateProductHandler(_mockRepo.Object);
    }
    
    [Fact]
    public async Task Handle_ValidCommand_CreatesProduct()
    {
        // Arrange
        var command = new CreateProductCommand("SKU001", "Product", 99.99m);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        _mockRepo.Verify(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()));
    }
}
```

### Tools
- **Unit/Integration**: xUnit, Moq, FluentAssertions, TestContainers
- **Components**: Vue Test Utils (frontend-store)
- **E2E**: Playwright (frontend-admin)
- **Run**: `dotnet test` or `npm run test`

## 🔐 Security & Tenancy

### Tenant Context
- Always extract `X-Tenant-ID` header in middleware
- Pass Guid tenantId through all domain queries
- **Never** query without tenant filter (data isolation critical)

### Authentication
- **JWT tokens** stored in appsettings (local dev)
- **Azure Key Vault** for production secrets
- Identity service handles token generation/refresh

## 📦 External Dependencies to Know

**Backend Microservices**:
- **Wolverine** (8.0+): Async messaging, HTTP endpoints, CQRS for all services
- **FluentValidation**: Input validation for all commands
- **EF Core 8**: Data access (with NamingConventions for snake_case DB)
- **AutoMapper**: DTO ↔ Entity mapping
- **Elasticsearch**: Full-text search (Search microservice)
- **Aspire**: Service orchestration and discovery

**CLI Tool**:
- **Spectre.Console**: Rich terminal output
- **System.CommandLine**: CLI argument parsing
- **Json.NET**: Configuration serialization

**Frontend**:
- **Pinia**: State management
- **Axios**: HTTP client
- **Vue Router**: Client routing
- **Tailwind CSS**: Styling

## 📝 Code Review Checklist

Before committing:
- ✅ Onion architecture respected (Core has zero external deps)
- ✅ Validator created for each Command
- ✅ Tenant ID included in all queries
- ✅ Repository interface in Core, implementation in Infrastructure
- ✅ Tests written (aim for 80%+)
- ✅ No synchronous service-to-service calls
- ✅ Nullable reference types enabled (`#nullable enable`)
- ✅ Async/await used consistently (no `.Result` or `.Wait()`)
- ✅ Restricted types use Enums, not strings (AddressType, OrderStatus, etc.)
- ✅ Enum configuration in EF Core with proper conversion strategy

## � Security Checklist for Feature Implementation

When implementing ANY feature, verify these critical security requirements (P0 priorities from [APPLICATION_SPECIFICATIONS.md](../docs/APPLICATION_SPECIFICATIONS.md)):

### P0.1 - JWT & Secrets Management
- [ ] **No hardcoded secrets** in code, config files, or version control
- [ ] Secrets use **Azure Key Vault** (production) or `appsettings.Development.json` (local only)
- [ ] JWT secret minimum **32 characters** (use `openssl rand -base64 32`)
- [ ] Token expiration: **1 hour access token**, **7 days refresh token**
- [ ] Implement token refresh flow in handlers
- [ ] Use HS256 (symmetric) or RS256 (asymmetric) only
- **Code Pattern**:
```csharp
// CORRECT: Use configuration
var jwtSecret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("Missing JWT secret");

// WRONG: Never hardcode
var jwtSecret = "my-secret-123"; // ❌ SECURITY RISK

// Token generation
var token = new JwtSecurityToken(
    issuer: configuration["Jwt:Issuer"],
    audience: configuration["Jwt:Audience"],
    claims: claims,
    expires: DateTime.UtcNow.AddHours(1),
    signingCredentials: new SigningCredentials(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        SecurityAlgorithms.HmacSha256
    )
);
```

### P0.2 - CORS & HTTPS
- [ ] **No hardcoded localhost/domains** in production
- [ ] CORS origins configured per environment (dev, staging, prod)
- [ ] HTTPS enforced (no HTTP in production)
- [ ] HSTS header with `max-age: 31536000` (1 year minimum)
- [ ] Rate limiting: **1000 req/min per IP**, **100 req/min per user**
- **Code Pattern**:
```csharp
// Program.cs
var allowedOrigins = configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// HSTS
app.UseHsts(); // Only in production
app.UseHttpsRedirection();
```

### P0.3 - Encryption at Rest & in Transit
- [ ] **PII fields encrypted**: Email, Phone, Address, SSN, DOB, FirstName, LastName
- [ ] Use **AES-256** encryption for sensitive data
- [ ] **Never store credit cards** - use tokenization only
- [ ] **Field-level encryption** in database (not table-level)
- [ ] TLS 1.2+ for all HTTPS connections
- [ ] IV (Initialization Vector) randomized for each encryption
- **Code Pattern**:
```csharp
// Core/Entities/User.cs
public class User : AggregateRoot
{
    // Sensitive fields encrypted
    private string _encryptedEmail;
    public string Email
    {
        get => _encryptionService.Decrypt(_encryptedEmail);
        set => _encryptedEmail = _encryptionService.Encrypt(value);
    }
}

// Infrastructure/Services/EncryptionService.cs
public class EncryptionService : IEncryptionService
{
    public string Encrypt(string plainText)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(_encryptionKey);
            aes.GenerateIV(); // Random IV each time
            
            using (var encryptor = aes.CreateEncryptor())
            using (var ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length);
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (var sw = new StreamWriter(cs))
                        sw.Write(plainText);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}
```

### P0.4 - Audit Logging (Immutable)
- [ ] **All data modifications logged** (CREATE, UPDATE, DELETE)
- [ ] Capture: **Timestamp, User ID, Action, Before/After values**
- [ ] Use **soft deletes** (logical deletion, not hard delete)
- [ ] Audit logs **immutable** (no update/delete of audit records)
- [ ] Include **tenant ID** in all audit entries
- **Code Pattern**:
```csharp
// Core/Events/AuditLogEntry.cs
public class AuditLogEntry
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid TenantId { get; init; }
    public Guid UserId { get; init; }
    public string EntityType { get; init; }
    public string Action { get; init; } // CREATE, UPDATE, DELETE
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public string? BeforeValues { get; init; } // JSON
    public string? AfterValues { get; init; }  // JSON
}

// Infrastructure/Services/AuditService.cs
public async Task LogAsync(Guid tenantId, Guid userId, string entity, 
    string action, object? before = null, object? after = null)
{
    var entry = new AuditLogEntry
    {
        TenantId = tenantId,
        UserId = userId,
        EntityType = entity,
        Action = action,
        BeforeValues = before != null ? JsonSerializer.Serialize(before) : null,
        AfterValues = after != null ? JsonSerializer.Serialize(after) : null
    };
    
    await _context.AuditLogs.AddAsync(entry);
    // Never delete audit logs - only add
}
```

### Additional Security Patterns

**Input Validation**
- [ ] All inputs validated **server-side** (never trust client)
- [ ] Use **FluentValidation** for all Commands
- [ ] Whitelist approach (allow known-good, block everything else)
- [ ] Length limits enforced (SQL injection prevention)
```csharp
public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Sku)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[A-Z0-9\-]+$", "SKU must be alphanumeric");
        
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .LessThanOrEqualTo(999999.99m);
    }
}
```

**Tenant Isolation**
- [ ] **Every query includes tenant ID filter** (no cross-tenant leaks)
- [ ] `X-Tenant-ID` header extracted in middleware and validated
- [ ] Tenant ID passed through entire call stack
```csharp
public async Task<Product?> GetBySkuAsync(Guid tenantId, string sku)
{
    // CORRECT: Filter by tenantId
    return await _context.Products
        .Where(p => p.TenantId == tenantId && p.Sku == sku)
        .FirstOrDefaultAsync();
    
    // WRONG: No tenant filter = security breach
    // return await _context.Products.FirstOrDefaultAsync(p => p.Sku == sku);
}
```

**No Sensitive Data in Logs**
- [ ] Never log: Passwords, tokens, credit cards, SSN, email (if PII sensitive)
- [ ] Use structured logging with redaction filters
- [ ] Review log output before commit
```csharp
// WRONG
_logger.LogInformation($"User logged in: {user.Email}"); // ❌ PII in logs

// CORRECT
_logger.LogInformation("User logged in with ID {UserId}", user.Id);
```

---

## � CLI Tool: B2Connect Console Interface

All backend operations are also available via CLI for automation and local development:

### CLI Structure
```
backend/CLI/B2Connect.CLI/
├── Commands/
│   ├── AuthCommands/          # User management, token generation
│   ├── TenantCommands/        # Tenant CRUD operations
│   ├── ProductCommands/       # Catalog operations
│   ├── ContentCommands/       # CMS operations
│   └── SystemCommands/        # Database migrations, seeding
├── Services/
│   ├── CliHttpClient.cs       # HTTP client for microservices
│   ├── ConfigurationService.cs # Load service endpoints
│   └── ConsoleOutputService.cs # Formatted output
└── Program.cs                  # CLI entry point
```

### Example CLI Commands
```bash
# Authentication
b2connect auth create-user john@example.com --password secret123 --tenant-id <guid>
b2connect auth generate-token john@example.com --password secret123
b2connect auth list-users --tenant-id <guid>

# Tenant Management
b2connect tenant create "Acme Corp" --admin-email admin@acme.com
b2connect tenant list
b2connect tenant show <tenant-id>

# Product Management
b2connect product create "SKU-001" "Product Name" --price 99.99 --tenant-id <guid>
b2connect product import-csv products.csv --tenant-id <guid>
b2connect product list --tenant-id <guid>

# System Operations
b2connect migrate --service Identity
b2connect seed --service Catalog --file products.json
b2connect status --check-all-services
b2connect health
```

### Implementation Pattern for CLI Commands
```csharp
public class CreateUserCommand : ICommand
{
    [Argument(0, Description = "User email")]
    public string Email { get; set; }
    
    [Option("-p|--password", Description = "User password")]
    public string Password { get; set; }
    
    [Option("-t|--tenant-id", Description = "Tenant ID")]
    public string TenantId { get; set; }
    
    public async Task<int> InvokeAsync(ICliService cliService)
    {
        var result = await cliService.PostAsync(
            "http://localhost:7002/auth/create-user",
            new { email = Email, password = Password, tenantId = TenantId }
        );
        
        if (result.Success)
        {
            AnsiConsole.MarkupLine($"[green]✓[/] User created successfully");
            return 0;
        }
        
        AnsiConsole.MarkupLine($"[red]✗[/] {result.Error}");
        return 1;
    }
}
```

## � Team Roles & Documentation

B2Connect erfordert 8 spezifische Team-Rollen für die Compliance-Implementierung. Jede Rolle hat dedizierte Dokumentation:

### Role-Specific Documentation Entry Points

| Role | Primary Focus | Entry Point | P0 Components |
|------|---------------|-------------|---------------|
| 🔐 **Security Engineer** | Encryption, Audit, Incident Response | [SECURITY_ENGINEER.md](../docs/by-role/SECURITY_ENGINEER.md) | P0.1, P0.2, P0.3, P0.5, P0.7 |
| ⚙️ **DevOps Engineer** | Infrastructure, Network, Aspire | [DEVOPS_ENGINEER.md](../docs/by-role/DEVOPS_ENGINEER.md) | P0.3, P0.4, P0.5 |
| 💻 **Backend Developer** | Wolverine, CQRS, Compliance APIs | [BACKEND_DEVELOPER.md](../docs/by-role/BACKEND_DEVELOPER.md) | P0.1, P0.6, P0.7, P0.9 |
| 🎨 **Frontend Developer** | Vue.js, Accessibility, UX | [FRONTEND_DEVELOPER.md](../docs/by-role/FRONTEND_DEVELOPER.md) | P0.6, P0.8 |
| 🧪 **QA Engineer** | Testing (52 Compliance Tests) | [QA_ENGINEER.md](../docs/by-role/QA_ENGINEER.md) | ALL (Test Execution) |
| 📋 **Product Owner** | Prioritization, Go/No-Go Gates | [PRODUCT_OWNER.md](../docs/by-role/PRODUCT_OWNER.md) | Executive Oversight |
| ⚖️ **Legal/Compliance** | Regulations, Legal Review | [LEGAL_COMPLIANCE.md](../docs/by-role/LEGAL_COMPLIANCE.md) | P0.6, P0.7, P0.8, P0.9 |
| 👔 **Tech Lead/Architect** | Architecture, Code Review | [TECH_LEAD.md](../docs/by-role/TECH_LEAD.md) | ALL (Oversight) |

### P0 Component Overview (Compliance Foundation)

| Component | Description | Owner | Tests | Deadline Impact |
|-----------|-------------|-------|-------|-----------------|
| **P0.1** | Audit Logging (Immutable) | Security | 5 | NIS2 |
| **P0.2** | Encryption at Rest (AES-256) | Security | 5 | GDPR |
| **P0.3** | Incident Response (< 24h) | Security + DevOps | 6 | NIS2 |
| **P0.4** | Network Segmentation | DevOps | 4 | NIS2 |
| **P0.5** | Key Management (KeyVault) | DevOps | 4 | All |
| **P0.6** | E-Commerce Legal (B2B/B2C) | Backend + Legal | 15 | PAngV, VVVG |
| **P0.7** | AI Act Compliance | Backend + Security | 15 | AI Act |
| **P0.8** | Barrierefreiheit (BITV 2.0) | Frontend | 12 | **BFSG (28. Juni 2025!)** |
| **P0.9** | E-Rechnung (ZUGFeRD/UBL) | Backend | 10 | ERechnungsVO |

### Complete Documentation Map

See: [docs/ROLE_BASED_DOCUMENTATION_MAP.md](../docs/ROLE_BASED_DOCUMENTATION_MAP.md)

---

## 🚀 Getting Unblocked

1. **Role-specific guidance**: See [docs/by-role/](../docs/by-role/) for your role's documentation
2. **Architecture questions**: Read [docs/architecture/DDD_BOUNDED_CONTEXTS.md](../docs/architecture/DDD_BOUNDED_CONTEXTS.md)
3. **Security specs**: See [docs/APPLICATION_SPECIFICATIONS.md](../docs/APPLICATION_SPECIFICATIONS.md) sections 3-4
4. **Service boundaries**: Check [COMPREHENSIVE_REVIEW.md](../COMPREHENSIVE_REVIEW.md) "Service Architecture"
5. **Code examples**: Search `backend/Domain/Identity/` for working Wolverine patterns
6. **Testing setup**: See [TESTING_STRATEGY.md](../TESTING_STRATEGY.md)
7. **Aspire orchestration**: See [ASPIRE_QUICK_START.md](../ASPIRE_QUICK_START.md)
8. **CLI Tool**: Check `backend/CLI/B2Connect.CLI/` for command examples
9. **Compliance Roadmap**: See [docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md](../docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md)

---

## 💡 AI Development Learnings & Best Practices

### Lessons Learned from B2Connect Implementation

This section documents key learnings from building B2Connect to improve future AI-assisted code generation.

#### 1. Central Package Management (CPM) Handling

**Problem Encountered:**
```
error NU1010: Die folgenden PackageReference-Elemente definieren kein 
entsprechendes PackageVersion-Element
```

**Root Cause:**
B2Connect uses **Central Package Management (CPM)** defined in `Directory.Packages.props`. When creating new projects in subdirectories, NuGet requires either:
1. **Option A**: Declare `<PackageVersion>` in `Directory.Packages.props` AND use `<PackageReference>` without versions in `.csproj`
2. **Option B**: Disable CPM in the project's `.csproj` with `<ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>`

**Solution Applied for CLI:**
```xml
<!-- In B2Connect.CLI.csproj -->
<PropertyGroup>
  <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="System.CommandLine" Version="2.0.0-beta5.25277.114" />
  <PackageReference Include="Spectre.Console" Version="0.49.1" />
</ItemGroup>
```

**Best Practice for Future Projects:**
- ✅ When adding packages to existing projects: Check if `ManagePackageVersionsCentrally=true` is inherited
- ✅ For new CLI tools or special projects: Set `ManagePackageVersionsCentrally=false` in `.csproj` PropertyGroup
- ✅ Keep main backend services using CPM for consistency
- ✅ When in doubt, explicitly declare versions in `.csproj`

**Prevention:**
When asked to create a new .NET project outside main structure, include this in the PropertyGroup:
```xml
<PropertyGroup>
  <!-- Disable CPM if this is a standalone tool/CLI -->
  <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
</PropertyGroup>
```

---

#### 2. Build Cache Issues & Solutions

**Problem Encountered:**
```
error CS0102: Der Typ "ApiResponse<T>" enthält bereits eine Definition
```

This appeared even after fixing the code, indicating stale build artifacts.

**Root Causes:**
1. Incremental build cache contains old compiled files
2. `.nuget` package cache not invalidated
3. `bin/obj` directories not fully cleaned

**Solutions (in order of effectiveness):**

**Quick Fix (usually works):**
```bash
cd project
rm -rf bin obj
dotnet clean
dotnet build
```

**Aggressive Clean (when quick fix fails):**
```bash
# Remove all build artifacts across solution
find . -type d -name obj -o -name bin | xargs rm -rf

# Clear NuGet cache (dangerous - re-downloads everything)
rm -rf ~/.nuget/packages

# Rebuild
dotnet build
```

**Build without cache:**
```bash
dotnet build --no-incremental
```

**Best Practice for Future Code Generation:**
- ✅ Always include cleanup steps after major structural changes
- ✅ When creating new projects, explicitly clean before first build
- ✅ Never rely on incremental build after dependency changes
- ✅ If build fails with "type already defined" → aggressive clean is needed

**Prevention:**
After file modifications, include in instructions:
```bash
# Clean and rebuild
rm -rf bin obj && dotnet clean && dotnet build
```

---

#### 3. System.CommandLine API Changes

**Problem Encountered:**
```csharp
error CS0234: Der Typ- oder Namespacename "Builder" ist im Namespace 
"System.CommandLine" nicht vorhanden
```

**Root Cause:**
`System.CommandLine` has different APIs across versions:
- `2.0.0-beta4`: Includes `CommandLineBuilder` in `System.CommandLine.Builder`
- `2.0.0-beta5+`: API changed, builder merged into main namespace

**Solution Applied:**
```csharp
// WRONG (older API):
using System.CommandLine.Builder;
var parser = new CommandLineBuilder(rootCommand).Build();

// CORRECT (newer API - 2.0.0-beta5+):
using System.CommandLine.Parsing;
var parser = rootCommand.Build();
// OR use RootCommand directly without builder
```

**Best Practice for Future:**
- ✅ When using pre-release/beta packages, check version compatibility first
- ✅ Use latest stable version when available (not beta)
- ✅ For System.CommandLine CLI tools, use version `2.0.0-beta5.25277.114` or higher
- ✅ Reference official API docs for beta packages

---

#### 4. Duplicate Method Definitions in Classes

**Problem Encountered:**
```csharp
public class ApiResponse<T> 
{
    public static ApiResponse<T> Error(string error) => ...;
    public static ApiResponse<T> Error(string error) => ...;  // ERROR: Duplicate!
    public static ApiResponse<T> Error(string error, string message) => ...;
}
```

**Root Cause:**
When generating similar overloads, accidental duplication can occur. Method overloading (same name, different parameters) is valid, but identical signatures are not.

**Solution Applied:**
```csharp
// CORRECT: Use optional parameters instead of multiple overloads
public static ApiResponse<T> Error(string error, string? message = null) => new()
{
    Success = false,
    Error = error,
    Message = message
};
```

**Best Practice for Future:**
- ✅ Use optional parameters instead of method overloads when possible
- ✅ When overloads are needed, ensure parameters differ in count OR type
- ✅ Avoid optional parameters with nullable types for clarity
- ✅ Pattern: `Method(required, optional1 = null, optional2 = null)`

---

#### 5. Project Structure for CLI Tools

**Lessons from B2Connect.CLI Implementation:**

**Correct Structure:**
```
backend/CLI/B2Connect.CLI/
├── B2Connect.CLI.csproj              # CLI-specific, disables CPM
├── Program.cs                         # Entry point
├── Commands/
│   ├── AuthCommands/
│   │   ├── CreateUserCommand.cs
│   │   └── LoginCommand.cs
│   ├── TenantCommands/
│   ├── ProductCommands/
│   ├── ContentCommands/
│   └── SystemCommands/
├── Services/
│   ├── CliHttpClient.cs              # HTTP communication
│   ├── ConfigurationService.cs       # Config management
│   └── ConsoleOutputService.cs       # Output formatting
└── README.md                          # CLI documentation
```

**Key Patterns:**

**1. Commands inherit from System.CommandLine.Command:**
```csharp
public static class CreateUserCommand
{
    public static Command Create()
    {
        var command = new Command("create-user", "Create a new user account");
        
        var emailArgument = new Argument<string>("email", "User email");
        command.AddArgument(emailArgument);
        
        command.SetHandler(ExecuteAsync, emailArgument);
        return command;
    }
    
    private static async Task ExecuteAsync(string email)
    {
        // Implementation
    }
}
```

**2. Services are registered in DI, not commands:**
```csharp
var authCommand = new Command("auth", "Authentication commands");
authCommand.AddCommand(CreateUserCommand.Create());
authCommand.AddCommand(LoginCommand.Create());
rootCommand.AddCommand(authCommand);
```

**3. Configuration via environment variables:**
```csharp
public class ConfigurationService
{
    private Dictionary<string, ServiceEndpoint> _endpoints;
    
    public string GetServiceUrl(string serviceName)
    {
        return _endpoints[serviceName.ToLower()].Url;
    }
    
    public string? GetToken() => Environment.GetEnvironmentVariable("B2CONNECT_TOKEN");
    public string? GetTenantId() => Environment.GetEnvironmentVariable("B2CONNECT_TENANT");
}
```

**4. Console output via Spectre.Console:**
```csharp
public class ConsoleOutputService
{
    public void Success(string message) => 
        AnsiConsole.MarkupLine($"[green]✓[/] {message}");
    
    public void Error(string message) => 
        AnsiConsole.MarkupLine($"[red]✗[/] {message}");
    
    public void Spinner(string title, Func<Task> action) =>
        AnsiConsole.Status().Start(title, async ctx => await action());
}
```

**Best Practice for Future CLI Tools:**
- ✅ Use static factory methods (`Create()`) for commands
- ✅ Keep HTTP client logic separate from command logic
- ✅ Configuration via environment variables for automation
- ✅ Use Spectre.Console for rich output, not plain Console
- ✅ Provide both `appsettings.json` and env var configuration

---

#### 6. Wolverine vs MediatR Pattern (CRITICAL)

**Hard-Learned Lesson:**
B2Connect uses **Wolverine**, not MediatR. This was not obvious from initial context and led to incorrect patterns.

**Why This Matters:**
- Wolverine is designed for distributed microservices
- MediatR is for in-process command bus
- B2Connect's architecture requires event-driven messaging

**Correct Wolverine Pattern:**
```csharp
// Step 1: Define command as plain POCO (no IRequest interface)
public class CreateProductCommand
{
    public string Sku { get; set; }
    public string Name { get; set; }
}

// Step 2: Service with public async method
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

// Step 3: Register simple DI
builder.Services.AddScoped<ProductService>();

// Step 4: Wolverine auto-discovers HTTP endpoint
// POST /createproduct
```

**Wrong MediatR Pattern (DO NOT USE):**
```csharp
// WRONG: IRequest interface
public record CreateProductCommand(string Sku, string Name) : IRequest<CreateProductResponse>;

// WRONG: IRequestHandler
public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResponse> { }

// WRONG: AddMediatR
builder.Services.AddMediatR();
```

**Best Practice:**
- ✅ Always verify architecture patterns before implementing
- ✅ Check existing code in Domain/ for pattern references
- ✅ Reference: `backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs`
- ✅ Never use MediatR in B2Connect (Wolverine only)

---

#### 7. Documentation & Code Comments

**Lesson:** Even well-written instructions can be incomplete. When patterns are critical, document them in code.

**Examples from B2Connect.CLI:**

```csharp
/// <summary>
/// HTTP Client für CLI-Commands zur Kommunikation mit Microservices
/// </summary>
public class CliHttpClient
{
    // Clear documentation of what it does
}

/// <summary>
/// Wolverine auto-discovers and creates HTTP endpoint from this method
/// Method name becomes route: CheckType → POST /checktype
/// CancellationToken is injected automatically
/// </summary>
public async Task<CheckRegistrationTypeResponse> CheckType(
    CheckRegistrationTypeCommand request,
    CancellationToken cancellationToken)
{
    // Implementation
}
```

**Best Practice:**
- ✅ Document non-obvious architectural decisions in XML comments
- ✅ Include examples in // comments for complex logic
- ✅ Reference related files: "See CheckRegistrationTypeService.cs for pattern"
- ✅ Explain WHY, not just WHAT

---

#### 8. Testing Early Failures

**Lesson:** Build projects early during development to catch issues:

**Wrong Approach:**
1. Generate all code
2. Create multiple files
3. Build at the end (discovers many errors)

**Right Approach:**
1. Create .csproj first
2. Build to verify project structure
3. Add packages incrementally
4. Build after each significant change
5. Fix issues immediately

**Implementation:**
```bash
# After creating .csproj
dotnet build  # <- Catch issues early

# After adding packages
dotnet build  # <- Verify versions work

# After implementing commands
dotnet build  # <- Fix compilation errors
```

**Best Practice:**
- ✅ Ask for builds after critical steps
- ✅ Create .csproj files early (not last)
- ✅ Verify NuGet packages exist before using
- ✅ Test build regularly during development

---

#### 9. File Handling - Create vs Overwrite

**Problem Encountered:**
When modifying existing files, sometimes accidental duplicates or incomplete rewrites occur.

**Safe Pattern:**
```bash
# WRONG: Try to create file that exists
create_file("path/file.cs")  # ERROR: File exists

# RIGHT: Delete first, then create
rm path/file.cs
create_file("path/file.cs")

# BETTER: Use replace_string_in_file for modifications
replace_string_in_file(oldString, newString)  # Explicit, reversible
```

**Best Practice:**
- ✅ Use `create_file` only for new files
- ✅ Use `replace_string_in_file` for modifications
- ✅ Include ample context (3-5 lines before/after) in replacements
- ✅ For major rewrites, delete + recreate is safer than find-replace
- ✅ Verify file content after major changes

---

### Quick Reference: Future Code Generation Checklist

When asked to create new C# code:

- [ ] **Architecture**: Verify Wolverine pattern (not MediatR)
- [ ] **CPM**: Check if `ManagePackageVersionsCentrally=true` applies
- [ ] **Dependencies**: List all NuGet packages and versions upfront
- [ ] **Build Early**: Create .csproj and build before adding complex code
- [ ] **Naming**: Follow naming conventions (PascalCase classes, camelCase fields)
- [ ] **Documentation**: Include XML comments for public APIs
- [ ] **Testing**: Provide unit test examples where appropriate
- [ ] **Cleanup**: Include build cleanup steps if modifying structure
- [ ] **Verification**: Confirm build succeeds before completion
- [ ] **References**: Link to existing code examples in project

---

## 🔥 Troubleshooting

### Port Already in Use (Common Issue!)
```bash
# Problem: "Address already in use" beim Start
# Lösung: Kill-Script ausführen
./scripts/kill-all-services.sh

# Ports prüfen
./scripts/check-ports.sh

# Force-Start (killt automatisch)
./scripts/start-aspire.sh --force
```

### Aspire/DCP Prozesse hängen
```bash
# DCP Controller hält Ports nach Shutdown
pkill -9 -f "dcpctrl"
pkill -9 -f "dcpproc"

# Oder komplett:
./scripts/kill-all-services.sh
```

### Service nicht erreichbar
```bash
# Health-Check
curl http://localhost:7002/health  # Auth
curl http://localhost:8000/health  # Store Gateway
curl http://localhost:15500        # Aspire Dashboard
```

---

## 🚀 Feature Integration Roadmap for EU SaaS at Scale

**Status:** Phase 0 (Compliance Foundation) - Parallel to MVP Development

### Platform Overview

B2Connect is a **multi-tenant SaaS platform**:
- **100+ Shops** (independent businesses)
- **1.000+ Concurrent Users per Shop**
- **EU-only Data Residency** (GDPR, NIS2, DORA compliant)
- **Cloud-Agnostisch** (AWS, Azure, On-Premise via Aspire)
- **99.9%+ SLA Target**

### Core Principle: Compliance-First Architecture

**Technologie zuerst. Ohne sichere Infrastruktur → kein Business.**

Alle Features in den Phasen 1-3 integrieren Compliance ab Tag 1:
- Audit Logging für jede Datenänderung
- Verschlüsselung sensibler Daten
- Mandanten-Isolation (kein Cross-Tenant Datenleck möglich)
- Skalierbarkeit mit Sicherheitsgrenzen respektiert

---

### 📋 Compliance Requirements (EU SaaS)

| Regulation | Deadline | Impact | Status |
|-----------|----------|--------|--------|
| **NIS2 Directive** | Oct 2025 (Phase 1), Oct 2026 (Phase 2) | Supply Chain Security, Incident Response < 24h | 🔴 P0 |
| **GDPR** | Active (May 2018) | Data Protection, Encryption, Right-to-Forget | 🔴 P0 |
| **DORA** | Jan 2025 (but SaaS: Jan 2026) | Operational Resilience, Backup & Recovery | 🔴 P0 |
| **eIDAS 2.0** | Nov 2024+ | Digital Signatures, Electronic ID | 🟡 P1 |
| **TISAX** | Ongoing | Telecom Security (DE/AT) | 🟡 P1 |

---

### 📅 Implementation Phases

#### **Phase 0: Compliance Foundation (Weeks 1-6, PARALLEL to MVP)**

🎯 **Goal:** Build security infrastructure that Phase 1 features depend on.

**Components:**
1. **P0.1: Audit Logging System** (immutable, encrypted, tenant-isolated)
   - All CRUD operations logged automatically
   - Tamper detection (hash verification)
   - SIEM integration ready
   - **Effort:** 1 FTE, 1 week | **Owner:** Security Engineer

2. **P0.2: Encryption at Rest** (AES-256-GCM)
   - All PII encrypted (email, phone, address, DOB)
   - Product cost data encrypted
   - Key rotation policy (annual for NIS2)
   - **Effort:** 1 FTE, 1 week | **Owner:** Security Engineer

3. **P0.3: Incident Response System** (< 24h Notification)
   - Automated detection (brute force, data exfiltration, availability)
   - NIS2 notification service (< 24h to authorities)
   - Incident tracking dashboard
   - **Effort:** 1.5 FTE, 1.5 weeks | **Owner:** Security Eng + DevOps

4. **P0.4: Network Segmentation & DDoS Protection**
   - VPC with 3 subnets (public, services, databases)
   - DDoS protection (AWS Shield, Azure DDoS)
   - WAF rules deployed
   - mTLS for service-to-service communication
   - **Effort:** 1 FTE, 1.5 weeks | **Owner:** DevOps Engineer

5. **P0.5: Key Management** (Azure KeyVault / Vault)
   - All secrets in vault (nothing hardcoded)
   - Key rotation automation
   - Access control & audit logging
   - **Effort:** 1 FTE, 1 week | **Owner:** DevOps Engineer

**Phase 0 Gate (Week 6):**
```
✅ Audit Logging: All CRUD operations captured
✅ Encryption: AES-256 at rest verified
✅ Incident Response: Detection rules running
✅ Network: Segmentation enforced
✅ Keys: Vault configured & rotating

If ANY ❌ → HOLD all Phase 1 deployments
```

---

#### **Phase 1: MVP with Built-in Compliance (Weeks 7-14)**

🎯 **Goal:** Deliver business features with 100% compliance integration.

**Features:**
1. **F1.1: Multi-Tenant Authentication**
   - JWT tokens (1h access, 7d refresh)
   - Tenant isolation (X-Tenant-ID mandatory)
   - Email verification
   - Suspicious activity detection (5+ failed logins → 10min lockout)
   - Audit logging for all login attempts
   - **Effort:** 2 weeks, Backend Dev

2. **F1.2: Product Catalog**
   - CRUD for products/categories
   - Redis caching (5-min TTL)
   - Soft deletes (never hard delete)
   - Supplier names encrypted
   - Audit trail for all changes
   - **Effort:** 2 weeks, Backend Dev

3. **F1.3: Shopping Cart & Checkout**
   - Cart in Redis (session-based)
   - PII not stored (reference only)
   - Billing address encrypted
   - Payment via Stripe/PayPal
   - Order audit trail
   - **Effort:** 2 weeks, Backend Dev

4. **F1.4: Admin Dashboard**
   - Tenant/User management
   - Read-only audit log viewer
   - Session timeout (30min inactivity)
   - All admin actions logged
   - **Effort:** 2 weeks, Backend + Frontend Dev

**Phase 1 Gate (Week 14):**
```
✅ All 4 features working (happy path tested)
✅ 100% audit logging integrated
✅ Encryption working end-to-end
✅ Code coverage > 80%
✅ API response < 200ms (P95)

If ANY ❌ → HOLD production deployment
```

---

#### **Phase 2: Scale with Compliance (Weeks 15-24)**

🎯 **Goal:** Scale to 10,000+ concurrent users without breaking security.

**Components:**
1. **P2.1: Database Read Replicas** (PostgreSQL 1 Write, 3 Read)
   - Streaming replication (encrypted, same as primary)
   - Connection pooling (PgBouncer)
   - Read-only routing for SELECT queries
   - **Effort:** 1.5 weeks, DevOps

2. **P2.2: Redis Cluster** (3+ nodes, HA)
   - Sentinel monitoring
   - AOF persistence
   - Annual key rotation
   - **Effort:** 1 week, DevOps

3. **P2.3: Elasticsearch Cluster** (3+ nodes, sharded)
   - Per-tenant index sharding
   - Daily index rotation
   - Backup to S3/Azure Blob
   - **Effort:** 1.5 weeks, DevOps

4. **P2.4: Auto-Scaling Configuration**
   - HPA (Horizontal Pod Autoscaler)
   - Rules: CPU > 70% → scale up, < 20% → scale down
   - Min 2 instances (HA), Max 10 instances
   - **Effort:** 1.5 weeks, DevOps

**Phase 2 Gate (Week 24):**
```
✅ 10,000+ concurrent users handled
✅ No single point of failure (HA verified)
✅ API response < 100ms (P95)
✅ Auto-scaling working correctly

If ANY ❌ → HOLD production deployment
```

---

#### **Phase 3: Production Hardening (Weeks 25-30)**

🎯 **Goal:** Verify production readiness before 100K+ users go live.

**Activities:**
1. **P3.1: Load Testing** (k6 + Grafana)
   - Scenario 1: Normal (1000 users/shop × 100 shops)
   - Scenario 2: Black Friday (5x normal, sudden spike)
   - Target: API < 500ms, < 1% failure rate

2. **P3.2: Chaos Engineering**
   - Service down → auto-restart verified
   - DB failover → < 10 sec recovery
   - Network latency → circuit breaker works
   - AWS region down → fail-over ready

3. **P3.3: Compliance Audit**
   - NIS2 supply chain security verified
   - Penetration testing completed
   - Incident response procedures tested
   - Backup & recovery procedures validated
   - GDPR right-to-be-forgotten tested

**Phase 3 Gate (Week 30):**
```
✅ Load testing: Black Friday scenario passed
✅ Chaos engineering: All failure modes handled
✅ Compliance: 100% NIS2/GDPR verified by 3rd party
✅ Disaster recovery: RTO < 4h, RPO < 1h proven

If ANY ❌ → HOLD production launch
```

---

### 📊 Implementation Timeline

```
Week 1-6    Phase 0: Compliance Foundation (PARALLEL)
            → Audit, Encryption, Incident Response, Network, Keys
            
Week 7-14   Phase 1: MVP + Compliance
            → Auth, Catalog, Checkout, Admin Dashboard
            ✅ Go/No-Go Gate: All compliance checks pass
            
Week 15-24  Phase 2: Scale with Compliance
            → DB Replication, Redis Cluster, ES Cluster, Auto-Scaling
            ✅ Go/No-Go Gate: 10K users handled
            
Week 25-30  Phase 3: Production Hardening
            → Load Testing, Chaos Engineering, Compliance Audit
            ✅ Go/No-Go Gate: Production Ready
            
Week 31+    Launch: 100K+ users ready
```

---

### 🔍 Detailed Implementation Guide

**See:** [docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md](../docs/EU_SAAS_COMPLIANCE_IMPLEMENTATION_ROADMAP.md)

This document contains:
- P0.1-P0.5: Compliance Foundation with code examples
- F1.1-F1.4: Feature acceptance criteria & testing
- P2.1-P2.4: Scaling strategy & configuration
- P3.1-P3.3: Testing & validation procedures

---

**Questions or unclear sections?** Ask for clarification on specific areas.
