# Architecture Decision Record: Domain Services Stateless Design

**Date:** December 31, 2025  
**Author:** @Architect  
**Status:** Proposed  
**Scope:** Domain Services Architecture  

---

## Decision

**All Domain Services must be stateless** and configured per tenant through dependency injection. Services maintain no internal state between requests and rely on external dependencies (repositories, providers) for data persistence.

---

## Context

### Current State
B2Connect implements multiple domain services (Email, Catalog, CMS, Identity, Localization, Search, etc.) following Domain-Driven Design patterns. These services handle business logic and coordinate with infrastructure layers.

### Problem
- **State management complexity:** Stateful services complicate testing, scaling, and deployment
- **Tenant isolation:** Multi-tenant applications require careful state separation
- **Scalability issues:** Stateful services create bottlenecks and limit horizontal scaling
- **Testing difficulty:** Internal state makes unit testing complex and unreliable

### Goals
1. Enable horizontal scaling of domain services
2. Simplify testing through stateless design
3. Ensure proper tenant isolation
4. Reduce complexity in service lifecycle management

---

## Decision

### Stateless Domain Service Pattern

All domain services must follow this pattern:

```csharp
public class EmailService : IEmailService
{
    private readonly IEmailRepository _repository;
    private readonly IEmailProvider _provider;
    private readonly ILogger<EmailService> _logger;
    private readonly ITenantContext _tenantContext; // For tenant-specific config

    public EmailService(
        IEmailRepository repository,
        IEmailProvider provider,
        ILogger<EmailService> logger,
        ITenantContext tenantContext) // Injected tenant context
    {
        _repository = repository;
        _provider = provider;
        _logger = logger;
        _tenantContext = tenantContext;
    }

    // All methods are stateless - no instance variables for state
    public async Task<EmailSendResult> SendEmailAsync(SendEmailCommand command)
    {
        // Business logic here - all state comes from parameters or external deps
        var email = await _repository.GetByIdAsync(command.EmailId);
        // ... business logic ...
    }
}
```

### Key Principles

#### 1. No Instance State
- **❌ Don't:** Store state in instance variables
- **✅ Do:** Pass all required data as method parameters
- **✅ Do:** Use external dependencies for persistence

#### 2. Tenant Configuration via DI
- **❌ Don't:** Hardcode tenant-specific configuration
- **✅ Do:** Inject `ITenantContext` for tenant-specific settings
- **✅ Do:** Configure services per tenant in DI container

#### 3. External Dependencies Only
- **✅ Repositories:** For data access (database, cache, external APIs)
- **✅ Providers:** For external service integration (email, payment, etc.)
- **✅ Loggers:** For observability
- **✅ Validators:** For input validation

#### 4. Immutable Operations
- **✅ Do:** Make operations idempotent where possible
- **✅ Do:** Use value objects for complex parameters
- **✅ Do:** Return result objects instead of modifying input

---

## Implementation Guidelines

### Service Registration
```csharp
// In Program.cs or DI configuration
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITenantContext, TenantContext>();
```

### Tenant-Specific Configuration
```csharp
public interface ITenantContext
{
    string TenantId { get; }
    EmailConfiguration GetEmailConfiguration();
    // Other tenant-specific settings
}
```

### Testing Pattern
```csharp
[Fact]
public async Task SendEmailAsync_ValidCommand_SendsEmail()
{
    // Arrange - all dependencies mocked
    var repository = new Mock<IEmailRepository>();
    var provider = new Mock<IEmailProvider>();
    var tenantContext = new Mock<ITenantContext>();
    
    var service = new EmailService(repository.Object, provider.Object, null, tenantContext.Object);
    
    // Act
    var result = await service.SendEmailAsync(command);
    
    // Assert - no state to verify, only behavior
}
```

---

## Consequences

### Positive
- **Scalability:** Services can be scaled horizontally without state synchronization
- **Testability:** Easy to mock dependencies and test in isolation
- **Reliability:** No state corruption or memory leaks
- **Maintainability:** Clear separation of concerns

### Negative
- **Performance:** May require more database/cache calls (mitigated by caching)
- **Complexity:** Business logic must be carefully designed for statelessness
- **Coordination:** Complex workflows may need saga patterns or event sourcing

### Risks
- **Data consistency:** Race conditions possible without proper transaction handling
- **Performance overhead:** Frequent external calls if not cached properly
- **Complex business logic:** Some domains may require careful state management

---

## Alternatives Considered

### Stateful Services
- **Pros:** Can maintain complex state internally, potentially better performance
- **Cons:** Difficult to scale, test, and deploy
- **Decision:** Rejected due to scalability and testing concerns

### Actor Model (Akka.NET, Orleans)
- **Pros:** Manages state internally while providing scalability
- **Cons:** Adds complexity, learning curve, and infrastructure requirements
- **Decision:** Deferred for future consideration if stateless pattern proves insufficient

---

## Compliance

This ADR aligns with:
- ADR-001: Event-driven Architecture (stateless services enable better event handling)
- ADR_SERVICE_BOUNDARIES: Clear service boundaries with stateless design
- DDD principles: Domain services should be stateless coordinators

---

## Status: Proposed

**Next Steps:**
1. Review existing domain services for stateful patterns
2. Update service implementations to be stateless
3. Add tenant context injection where needed
4. Update testing patterns
5. Document migration path for any stateful services