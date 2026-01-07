# 🚀 Wolverine Pattern Reference

**Audience**: Backend developers  
**Purpose**: Authoritative guide for Wolverine CQRS patterns in B2X  
**Critical**: NOT MediatR - Wolverine services are plain POCO classes

---

## Quick Start: HTTP Endpoint

```csharp
// ✅ CORRECT: Plain POCO command + Service method
public class CreateProductCommand
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

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

// DI: builder.Services.AddScoped<ProductService>();
```

---

## ❌ ANTIPATTERN: MediatR (DO NOT USE)

```csharp
// ❌ WRONG for B2X
public record CreateProductCommand(string Name, decimal Price) 
    : IRequest<CreateProductResponse>;

public class CreateProductHandler 
    : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    public async Task<CreateProductResponse> Handle(
        CreateProductCommand request, 
        CancellationToken ct)
    {
        // Handler logic
    }
}

// DI: builder.Services.AddMediatR();  // ❌ NEVER do this
```

**Why NOT**: B2X uses **Wolverine**, not MediatR. Different CQRS model.

---

## Pattern 1: HTTP Endpoint Handler

**When**: POST/GET/PUT/DELETE with request/response

**Structure**:
1. Command class (POCO, no interface)
2. Service class (async method)
3. Response class
4. Register in DI as AddScoped<>

**Example**:

```csharp
// Domain Models
public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Commands
public class CreateProductCommand
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class UpdateProductCommand
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Responses
public class CreateProductResponse
{
    public Guid Id { get; set; }
    public string Message { get; set; }
}

// Service
public class ProductService
{
    private readonly IProductRepository _repo;
    private readonly IEventBus _eventBus;
    
    public ProductService(IProductRepository repo, IEventBus eventBus)
    {
        _repo = repo;
        _eventBus = eventBus;
    }
    
    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand cmd,
        CancellationToken ct)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(cmd.Name))
            throw new ValidationException("Name is required");
        
        // Create aggregate
        var product = new Product 
        { 
            Id = Guid.NewGuid(),
            Name = cmd.Name,
            Price = cmd.Price
        };
        
        // Persist
        await _repo.AddAsync(product, ct);
        
        // Publish event (Wolverine will handle it)
        await _eventBus.PublishAsync(new ProductCreatedEvent
        {
            ProductId = product.Id,
            Name = product.Name
        }, ct);
        
        return new CreateProductResponse 
        { 
            Id = product.Id,
            Message = "Product created successfully"
        };
    }
    
    public async Task<CreateProductResponse> UpdateProduct(
        UpdateProductCommand cmd,
        CancellationToken ct)
    {
        var product = await _repo.GetByIdAsync(cmd.Id, ct);
        if (product == null)
            throw new NotFoundException("Product not found");
        
        product.Name = cmd.Name;
        product.Price = cmd.Price;
        
        await _repo.UpdateAsync(product, ct);
        
        await _eventBus.PublishAsync(new ProductUpdatedEvent
        {
            ProductId = product.Id,
            Name = product.Name
        }, ct);
        
        return new CreateProductResponse { Id = product.Id };
    }
}

// DI Registration (Program.cs)
builder.Services.AddScoped<ProductService>();
```

---

## Pattern 2: Event Handler (Subscriber)

**When**: Reacting to domain events published by other services

**Structure**:
1. Event class (POCO)
2. Handler method in a handler class
3. Wolverine automatically subscribes

**Example**:

```csharp
// Events (published by ProductService)
public class ProductCreatedEvent
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
}

public class ProductUpdatedEvent
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
}

// Handler class (Wolverine subscribes automatically)
public class ProductEventHandlers
{
    private readonly ISearchService _search;
    private readonly INotificationService _notifier;
    
    public ProductEventHandlers(
        ISearchService search,
        INotificationService notifier)
    {
        _search = search;
        _notifier = notifier;
    }
    
    // Wolverine invokes this automatically when ProductCreatedEvent is published
    public async Task Handle(ProductCreatedEvent @event)
    {
        // Index in Elasticsearch
        await _search.IndexProductAsync(@event.ProductId, @event.Name);
        
        // Notify admin
        await _notifier.NotifyAdminAsync($"New product: {event.Name}");
    }
    
    // Wolverine invokes this automatically when ProductUpdatedEvent is published
    public async Task Handle(ProductUpdatedEvent @event)
    {
        // Re-index in Elasticsearch
        await _search.ReindexProductAsync(@event.ProductId, @event.Name);
    }
}

// DI Registration
builder.Services.AddScoped<ProductEventHandlers>();
```

---

## Pattern 3: Validation & Error Handling

**Structure**:
1. Validation happens in service method
2. Throw domain exceptions
3. Wolverine/middleware handles response mapping

**Example**:

```csharp
public class CreateProductCommand
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class ProductService
{
    private readonly IProductRepository _repo;
    
    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand cmd,
        CancellationToken ct)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(cmd.Name))
            throw new ValidationException("Name is required");
        
        if (cmd.Price <= 0)
            throw new ValidationException("Price must be greater than 0");
        
        var existing = await _repo.GetByNameAsync(cmd.Name, ct);
        if (existing != null)
            throw new DuplicateException("Product already exists");
        
        // Create and persist
        var product = new Product 
        { 
            Id = Guid.NewGuid(),
            Name = cmd.Name,
            Price = cmd.Price
        };
        
        await _repo.AddAsync(product, ct);
        
        return new CreateProductResponse { Id = product.Id };
    }
}
```

---

## Pattern 4: Cross-Context Communication

**When**: Service A needs to call Service B (different bounded context)

**Structure**:
1. Define event or query
2. Publish/send via Wolverine
3. Other context subscribes

**Example**:

```csharp
// Store Context - Catalog Service
public class CatalogService
{
    private readonly IProductRepository _repo;
    private readonly IEventBus _eventBus;
    
    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand cmd,
        CancellationToken ct)
    {
        var product = new Product { ... };
        await _repo.AddAsync(product, ct);
        
        // Publish event - other contexts listen
        await _eventBus.PublishAsync(new ProductCreatedEvent
        {
            ProductId = product.Id,
            Name = product.Name
        }, ct);
        
        return new CreateProductResponse { Id = product.Id };
    }
}

// Admin Context - listening for product events
public class AdminSearchHandlers
{
    private readonly ISearchService _search;
    
    public AdminSearchHandlers(ISearchService search) => _search = search;
    
    // Automatically invoked when ProductCreatedEvent published
    public async Task Handle(ProductCreatedEvent @event)
    {
        // Admin also indexes for admin search
        await _search.IndexProductForAdminAsync(@event.ProductId);
    }
}
```

---

## Checklist: Is Your Endpoint Correct?

- [ ] Command is a plain POCO (no interfaces)?
- [ ] Service method is async?
- [ ] Response is a plain POCO?
- [ ] Service registered as `AddScoped<>`?
- [ ] No `[ApiController]` or `[HttpPost]` attributes?
- [ ] No `AddMediatR()` in DI?
- [ ] Events published via `IEventBus`?
- [ ] Event handlers are public async Task Handle(Event)?
- [ ] Validation throws exceptions?

---

## Anti-patterns to Avoid

| ❌ Don't | ✅ Do Instead |
|----------|--------------|
| `IRequest<T>` interface | Plain POCO command |
| `IRequestHandler<>` | Service method with async Task |
| `AddMediatR()` | Let Wolverine handle routing |
| `[ApiController]` | Use Wolverine routing |
| `[HttpPost]` | Use service method name |
| Multiple return types | Single response class |
| Static service | Injected via DI |

---

## Common Mistakes

### ❌ Mistake 1: Exposing Repository Directly
```csharp
// ❌ WRONG
public class ProductController
{
    public ProductController(IProductRepository repo) { }
    // Endpoint logic in controller
}

// ✅ CORRECT
public class ProductService
{
    public ProductService(IProductRepository repo) { }
    // Endpoint logic in service
}
// Register service, not repository
builder.Services.AddScoped<ProductService>();
```

### ❌ Mistake 2: Using MediatR Patterns
```csharp
// ❌ WRONG
public record CreateProductCommand() : IRequest<CreateProductResponse>;
public class Handler : IRequestHandler<CreateProductCommand, CreateProductResponse> { }

// ✅ CORRECT
public class CreateProductCommand { }
public class ProductService 
{ 
    public async Task<CreateProductResponse> CreateProduct(
        CreateProductCommand cmd, CancellationToken ct) { }
}
```

### ❌ Mistake 3: Forgetting CancellationToken
```csharp
// ❌ WRONG
public async Task<Response> MyMethod(MyCommand cmd)

// ✅ CORRECT
public async Task<Response> MyMethod(MyCommand cmd, CancellationToken ct)
```

---

## Reference

**Wolverine Docs**: https://wolverine.netlify.app  
**B2X Bounded Contexts**: See DDD_BOUNDED_CONTEXTS_REFERENCE.md  
**Error Handling**: See ERROR_HANDLING_PATTERNS.md  
**Feature Implementation**: See FEATURE_IMPLEMENTATION_PATTERNS.md

---

*Updated: 30. Dezember 2025*  
*Source: docs/architecture/WOLVERINE_QUICK_REFERENCE.md + team patterns*
