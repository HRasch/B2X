# 🚀 Wolverine Quick Reference - For B2X Developers

**Purpose:** Quick lookup for correct Wolverine patterns in B2X  
**For:** New features, Story implementations, API endpoint creation

---

## Pattern at a Glance

### HTTP Endpoints (CORRECT)
```csharp
// ✅ Wolverine Service
public class MyService
{
    public async Task<MyResponse> MyMethod(MyCommand cmd, CancellationToken ct)
    {
        return new MyResponse { ... };
    }
}

// DI: builder.Services.AddScoped<MyService>();
// Route: POST /mymethod (auto-generated from method name)
```

### ❌ DO NOT USE (MediatR - Wrong for B2X)
```csharp
// ❌ Wrong Pattern
public record MyCommand(string Prop) : IRequest<MyResponse>;
public class MyHandler : IRequestHandler<MyCommand, MyResponse> { }
// DI: builder.Services.AddMediatR();
```

---

## Checklist Before Coding

```
[ ] Plain POCO command (no IRequest)?
[ ] Service class with async methods?
[ ] No [ApiController] or [HttpPost]?
[ ] Registered as AddScoped<Service>()?
[ ] No AddMediatR() in DI?
```

---

## Common Patterns

### 1️⃣ HTTP Endpoint Handler

```csharp
// Command (POCO)
public class CreateProductCommand
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Service (Handler)
public class ProductService
{
    private readonly IProductRepository _repo;
    
    public ProductService(IProductRepository repo) => _repo = repo;
    
    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand cmd,
        CancellationToken ct)
    {
        var product = new Product(cmd.Name, cmd.Price);
        await _repo.AddAsync(product, ct);
        return new CreateProductResponse { Id = product.Id };
    }
}

// DI
builder.Services.AddScoped<ProductService>();
```

### 2️⃣ Event Handler (Subscriber)

```csharp
// Event (POCO)
public class ProductCreatedEvent
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
}

// Handler Class
public class ProductEventHandlers
{
    private readonly ISearchService _search;
    
    public ProductEventHandlers(ISearchService search) => _search = search;
    
    // Wolverine calls this automatically
    public async Task Handle(ProductCreatedEvent @event)
    {
        await _search.IndexProductAsync(@event.ProductId);
    }
}

// DI
builder.Services.AddScoped<ProductEventHandlers>();

// Usage (in service)
await _messageBus.PublishAsync(new ProductCreatedEvent 
{ 
    ProductId = id, 
    Name = product.Name 
});
```

### 3️⃣ Multi-Handler (Multiple Events)

```csharp
public class OrderEventHandlers
{
    private readonly IEmailService _email;
    private readonly INotificationService _notify;
    
    // Handle different events in same class
    public async Task Handle(OrderCreatedEvent @event)
    {
        await _email.SendOrderConfirmationAsync(@event.OrderId);
    }
    
    public async Task Handle(OrderShippedEvent @event)
    {
        await _notify.SendShippingNotificationAsync(@event.OrderId);
    }
    
    public async Task Handle(OrderDeliveredEvent @event)
    {
        await _notify.SendDeliveryConfirmationAsync(@event.OrderId);
    }
}
```

---

## Naming Conventions

| Element | Pattern | Example |
|---------|---------|---------|
| Service | `{Feature}Service` | `ProductService`, `AuthService` |
| Method | PascalCase | `CreateProduct`, `SendEmail` |
| HTTP Route | Lowercase from method | `CreateProduct` → `/createproduct` |
| Command | `{Action}{Entity}Command` | `CreateProductCommand` |
| Response | `{Action}{Entity}Response` | `CreateProductResponse` |
| Event | `{Entity}{Past Action}Event` | `ProductCreatedEvent` |
| Handler Class | `{Entity}EventHandlers` | `ProductEventHandlers` |
| Handle Method | `Handle(EventType)` | `Handle(ProductCreatedEvent)` |

---

## Project Structure

```
backend/Domain/
├── Identity/
│   └── src/
│       └── Handlers/              # All handlers here
│           ├── Services/
│           │   ├── AuthService.cs
│           │   └── RegistrationService.cs
│           ├── Events/
│           │   ├── UserEventHandlers.cs
│           │   ├── UserRegisteredEvent.cs
│           │   └── UserLoggedInEvent.cs
│           ├── Commands/
│           │   ├── CreateUserCommand.cs
│           │   ├── LoginCommand.cs
│           │   └── ResetPasswordCommand.cs
│           └── Responses/
│               ├── CreateUserResponse.cs
│               ├── LoginResponse.cs
│               └── ResetPasswordResponse.cs
```

---

## Comparison Table

| Aspect | Wolverine ✅ | MediatR ❌ |
|--------|-------------|----------|
| **Service Class** | Plain POCO | `IRequestHandler<T, R>` |
| **Command Interface** | None (plain class) | `IRequest<T>` |
| **Route Attributes** | None (auto-generated) | `[HttpPost]` `[ApiController]` |
| **DI Registration** | `AddScoped<Service>()` | `AddMediatR()` |
| **Method Name** | Becomes HTTP route | Explicit in attribute |
| **Event Handlers** | `Handle(EventType)` | Custom or MediatR behavior |
| **Use in B2X** | ✅ CORRECT | ❌ DO NOT USE |

---

## Error Prevention

### Common Mistakes

| Mistake | Detection | Fix |
|---------|-----------|-----|
| Used `IRequest<T>` | Code review, build check | Remove interface, use POCO |
| Used `IRequestHandler` | Code review | Delete class, create service |
| Added `[HttpPost]` attribute | Code review, build check | Delete attribute |
| Added `[ApiController]` | Code review | Delete attribute |
| Called `AddMediatR()` | Code review, Program.cs | Delete line, add `AddScoped<Service>()` |
| Registered as IMediator | Code review | Change to service registration |

### Pre-Commit Validation

```bash
# Before pushing code:
grep -r "IRequest<" . --include="*.cs"      # Should be 0
grep -r "IRequestHandler" . --include="*.cs" # Should be 0
grep -r "AddMediatR" . --include="*.cs"      # Should be 0
grep -r "\[HttpPost\]" . --include="*.cs"    # Should be 0
```

---

## FAQ

### Q: What if I need to validate input?
**A:** Use FluentValidation in service, not in separate handler
```csharp
public class ProductService
{
    private readonly IValidator<CreateProductCommand> _validator;
    
    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand cmd, CancellationToken ct)
    {
        var result = await _validator.ValidateAsync(cmd, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);
        // ... process ...
    }
}
```

### Q: How do I return errors?
**A:** Return response DTO with success flag or throw specific exception
```csharp
public class CreateProductResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public Guid? ProductId { get; set; }
}

// Or use Result<T> pattern
public async Task<Result<ProductDto>> CreateProduct(...)
{
    if (validation fails)
        return Result.Fail("Error message");
    return Result.Ok(productDto);
}
```

### Q: How do I inject dependencies?
**A:** Via constructor, Wolverine auto-injects from DI container
```csharp
public class ProductService
{
    private readonly IProductRepository _repo;
    private readonly ILogger<ProductService> _logger;
    
    // Dependencies automatically injected
    public ProductService(
        IProductRepository repo,
        ILogger<ProductService> logger)
    {
        _repo = repo;
        _logger = logger;
    }
}

// In DI:
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();
```

### Q: Can I return JSON responses?
**A:** Yes, return DTO directly - Wolverine serializes automatically
```csharp
public async Task<ProductDto> GetProduct(
    GetProductCommand cmd,
    CancellationToken ct)
{
    var product = await _repo.GetAsync(cmd.Id, ct);
    return new ProductDto
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price
    };
}

// Wolverine automatically returns:
// HTTP 200
// Content-Type: application/json
// Body: { "id": "...", "name": "...", "price": ... }
```

### Q: Where do I put the service?
**A:** In `/Handlers/` folder within the bounded context
```
backend/Domain/Identity/src/Handlers/
├── AuthService.cs
├── RegistrationService.cs
└── Events/
    └── UserEventHandlers.cs
```

### Q: How do handlers discover HTTP methods?
**A:** Method names in PascalCase become lowercase HTTP routes
```csharp
public class ProductService
{
    public async Task<T> GetProduct(...) { }      // GET /getproduct
    public async Task<T> CreateProduct(...) { }   // POST /createproduct
    public async Task<T> UpdateProduct(...) { }   // PUT /updateproduct
    public async Task DeleteProduct(...) { }      // DELETE /deleteproduct
}
```

---

## Real Examples in Project

### Working Example 1: HTTP Endpoint
**File:** `backend/Domain/Identity/src/Handlers/CheckRegistrationTypeService.cs`
- Plain service class ✅
- Public async method ✅
- POCO command ✅
- Returns DTO directly ✅

### Working Example 2: Event Handler
**File:** `backend/Domain/Identity/src/Handlers/Events/UserEventHandlers.cs`
- Multiple `Handle(EventType)` methods ✅
- Async event processing ✅
- Wolverine auto-discovery ✅

---

## Next Steps

1. **Reading**: Understand patterns in CheckRegistrationTypeService.cs
2. **Implementing**: Copy structure for new features
3. **Validating**: Use checklist before committing
4. **Testing**: Add unit tests for service methods

---

## Resources

- [WOLVERINE_ARCHITECTURE_ANALYSIS.md](WOLVERINE_ARCHITECTURE_ANALYSIS.md) - Deep dive analysis
- [.github/copilot-instructions.md](.github/copilot-instructions.md) - AI coding guidelines
- [Wolverine Official](https://wolverinefx.net/) - Official documentation

---

**Last Updated:** 27. Dezember 2025  
**Maintained By:** Architecture Team  
**Questions?** Check examples above or reference actual code in /backend/Domain/

