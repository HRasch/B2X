# CQRS Pattern mit Wolverine Message Handlers

**Status**: ✅ Implemented  
**Date**: 27. Dezember 2025  
**Pattern**: Command Query Responsibility Segregation (CQRS)

---

## 🎯 Konzept

**CQRS trennt die Verantwortlichkeiten nach Operationstyp:**
- **Commands**: Ändern den State (Create, Update, Delete) ✍️
- **Queries**: Lesen den State (Get, List, Search) 📖
- **Handlers**: Verarbeiten die Business-Logik 🔧

**Benefit**: Saubere Separation of Concerns

---

## 🏗️ Architektur

```
┌─────────────────────────────────────────────────────────────┐
│ HTTP Request (POST /api/products)                           │
└──────────────────┬──────────────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────────────┐
│ Controller (HTTP Layer)                                     │
│ ───────────────────────────────                             │
│ • Tenant-ID extrahieren (ValidateTenantAttribute)          │
│ • Request validieren (ValidateModelStateFilter)             │
│ • CreateProductCommand erstellen                            │
│ • Response formatieren                                      │
└──────────────────┬──────────────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────────────┐
│ Wolverine Message Bus (Dispatch)                            │
│ ───────────────────────────────                             │
│ await _messageBus.InvokeAsync<T>(command, ct)              │
└──────────────────┬──────────────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────────────┐
│ Handler (Business Logic)                                    │
│ ───────────────────────────────                             │
│ • Validierung (Duplikate, Constraints)                      │
│ • Repository-Zugriffe                                       │
│ • Domain Events                                             │
│ • Exception Handling                                        │
└──────────────────┬──────────────────────────────────────────┘
                   │
                   ▼
┌─────────────────────────────────────────────────────────────┐
│ Response                                                    │
│ ───────────────────────────────                             │
│ return CreatedResponse(nameof(GetProduct), ..., product);   │
└─────────────────────────────────────────────────────────────┘
```

---

## 📝 Implementierungsbeispiel

### 1. Commands & Queries definieren

**Datei**: `Application/Commands/Products/ProductCommands.cs`

```csharp
using Wolverine;

namespace B2X.Admin.Application.Commands.Products;

/// Command für Create-Operation
public record CreateProductCommand(
    Guid TenantId,
    string Name,
    string Sku,
    decimal Price,
    string? Description = null,
    Guid? CategoryId = null,
    Guid? BrandId = null) : IRequest<ProductResult>;

/// Command für Update-Operation
public record UpdateProductCommand(
    Guid TenantId,
    Guid ProductId,
    string Name,
    string Sku,
    decimal Price,
    string? Description = null,
    Guid? CategoryId = null,
    Guid? BrandId = null) : IRequest<ProductResult>;

/// Command für Delete-Operation
public record DeleteProductCommand(
    Guid TenantId,
    Guid ProductId) : IRequest<bool>;

/// Query für Get-Operation
public record GetProductQuery(
    Guid TenantId,
    Guid ProductId) : IRequest<ProductResult?>;

/// Query für GetAll-Operation
public record GetAllProductsQuery(
    Guid TenantId) : IRequest<IEnumerable<ProductResult>>;

/// Result DTO
public record ProductResult(
    Guid Id,
    Guid TenantId,
    string Name,
    string Sku,
    decimal Price,
    string? Description = null,
    Guid? CategoryId = null,
    Guid? BrandId = null,
    DateTime CreatedAt = default);
```

### 2. Handlers implementieren

**Datei**: `Application/Handlers/Products/ProductHandlers.cs`

```csharp
using Wolverine;
using B2X.Admin.Application.Commands.Products;
using B2X.Admin.Core.Interfaces;

namespace B2X.Admin.Application.Handlers.Products;

/// Wolverine Handler für Create Product
public class CreateProductHandler : ICommandHandler<CreateProductCommand, ProductResult>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<CreateProductHandler> _logger;

    public CreateProductHandler(IProductRepository repository, ILogger<CreateProductHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ProductResult> Handle(CreateProductCommand command, CancellationToken ct)
    {
        _logger.LogInformation(
            "Creating product '{Name}' (SKU: {Sku}) for tenant {TenantId}",
            command.Name, command.Sku, command.TenantId);

        // Validierung
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ArgumentException("Product name is required");

        if (command.Price <= 0)
            throw new ArgumentException("Product price must be greater than 0");

        // Business Logic
        var product = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = command.TenantId,
            Name = command.Name,
            Sku = command.Sku,
            Price = command.Price,
            Description = command.Description,
            CategoryId = command.CategoryId,
            BrandId = command.BrandId,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(product, ct);

        _logger.LogInformation("Product {ProductId} created successfully", product.Id);

        return new ProductResult(
            product.Id,
            product.TenantId,
            product.Name,
            product.Sku,
            product.Price,
            product.Description,
            product.CategoryId,
            product.BrandId,
            product.CreatedAt);
    }
}

/// Wolverine Handler für Get Product Query
public class GetProductHandler : IQueryHandler<GetProductQuery, ProductResult?>
{
    private readonly IProductRepository _repository;

    public GetProductHandler(IProductRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<ProductResult?> Handle(GetProductQuery query, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(query.TenantId, query.ProductId, ct);

        if (product == null)
            return null;

        return new ProductResult(
            product.Id,
            product.TenantId,
            product.Name,
            product.Sku,
            product.Price,
            product.Description,
            product.CategoryId,
            product.BrandId,
            product.CreatedAt);
    }
}
```

### 3. Controller dispatched via Wolverine

**Datei**: `Presentation/Controllers/ProductsController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[ValidateTenant]  // ← Tenant-Validierung
public class ProductsController : ApiControllerBase
{
    private readonly IMessageBus _messageBus;

    public ProductsController(IMessageBus messageBus, ILogger<ProductsController> logger)
        : base(logger)
    {
        _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
    }

    /// HTTP POST → Command → Handler → Response
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductResult>> CreateProduct(
        [FromBody] CreateProductRequest request,
        CancellationToken ct)
    {
        var tenantId = GetTenantId();
        var userId = GetUserId();

        // 1️⃣ Create Command
        var command = new CreateProductCommand(
            tenantId,
            request.Name,
            request.Sku,
            request.Price,
            request.Description,
            request.CategoryId,
            request.BrandId);

        // 2️⃣ Dispatch via Wolverine → Handler
        var product = await _messageBus.InvokeAsync<ProductResult>(command, ct);

        // 3️⃣ Return Response
        return CreatedResponse(nameof(GetProduct), new { id = product.Id }, product);
    }

    /// HTTP GET → Query → Handler → Response
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResult>> GetProduct(Guid id, CancellationToken ct)
    {
        var tenantId = GetTenantId();

        // 1️⃣ Create Query
        var query = new GetProductQuery(tenantId, id);

        // 2️⃣ Dispatch via Wolverine → Handler
        var product = await _messageBus.InvokeAsync<ProductResult?>(query, ct);

        // 3️⃣ Return Response
        if (product == null)
            return NotFoundResponse($"Product {id} not found");

        return OkResponse(product);
    }
}
```

---

## 🔑 Wichtige Konzepte

### Command vs Query

| Aspekt | Command | Query |
|--------|---------|-------|
| **Aktion** | Ändert State | Liest State |
| **Return** | Result DTO | Data DTO |
| **Caching** | Nein (invalidiert Cache) | Ja (cachebar) |
| **Seiteneffekte** | Erlaubt (Events) | Keine! |
| **Beispiele** | Create, Update, Delete | Get, List, Search |
| **Interface** | `ICommandHandler<TCommand, TResult>` | `IQueryHandler<TQuery, TResult>` |

### Handler-Typen

#### Command Handler
```csharp
public class CreateProductHandler : ICommandHandler<CreateProductCommand, ProductResult>
{
    public async Task<ProductResult> Handle(CreateProductCommand command, CancellationToken ct)
    {
        // Validierung
        // Repository-Zugriff
        // Return Result
    }
}
```

#### Query Handler
```csharp
public class GetProductHandler : IQueryHandler<GetProductQuery, ProductResult?>
{
    public async Task<ProductResult?> Handle(GetProductQuery query, CancellationToken ct)
    {
        // Nur Lese-Operationen!
        // Keine Seiteneffekte
        // Return Data oder null
    }
}
```

---

## 📦 Wolverine Integration

### 1. NuGet Package installieren
```bash
dotnet add package Wolverine
```

### 2. In Program.cs registrieren
```csharp
builder.Host.UseWolverine();

// Oder mit Konfiguration:
builder.Host.UseWolverine(opts =>
{
    opts.Handlers.DiscoverHandlersFromAssembly(typeof(Program).Assembly);
});
```

### 3. Handler-Discovery
Wolverine findet automatisch alle Handler-Klassen, die `ICommandHandler<>` oder `IQueryHandler<>` implementieren.

### 4. Message Bus in Controller injizieren
```csharp
public class ProductsController : ApiControllerBase
{
    private readonly IMessageBus _messageBus;

    public ProductsController(IMessageBus messageBus, ILogger<ProductsController> logger)
        : base(logger)
    {
        _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
    }

    // ... Action Methods
}
```

### 5. Commands/Queries dispatchen
```csharp
// Command dispatchen (mit Response)
var product = await _messageBus.InvokeAsync<ProductResult>(command, ct);

// Query dispatchen
var product = await _messageBus.InvokeAsync<ProductResult?>(query, ct);

// Command ohne Response
await _messageBus.PublishAsync(command, ct);
```

---

## 🎯 Best Practices

### ✅ Regel 1: Controller bleibt dünn
```csharp
// ✅ Richtig
public async Task<ActionResult> CreateProduct(CreateProductRequest request, CancellationToken ct)
{
    var command = new CreateProductCommand(...);
    var result = await _messageBus.InvokeAsync<ProductResult>(command, ct);
    return CreatedResponse(..., result);
}

// ❌ Falsch - Logik im Controller!
public async Task<ActionResult> CreateProduct(CreateProductRequest request, CancellationToken ct)
{
    var product = new Product { ... };
    await _repository.AddAsync(product, ct);
    return CreatedResponse(..., product);
}
```

### ✅ Regel 2: Commands sind Commands, keine DTOs
```csharp
// ✅ Richtig
public record CreateProductCommand(
    Guid TenantId,
    string Name,
    string Sku,
    decimal Price) : IRequest<ProductResult>;

// ❌ Falsch - Command = DTO
public class CreateProductCommand
{
    public CreateProductDto Dto { get; set; }
}
```

### ✅ Regel 3: Handler enthalten alle Business-Logik
```csharp
// ✅ Richtig
public class CreateProductHandler : ICommandHandler<CreateProductCommand, ProductResult>
{
    public async Task<ProductResult> Handle(CreateProductCommand command, CancellationToken ct)
    {
        // Validierung
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ArgumentException("Name required");

        // Duplikat-Check
        var existing = await _repository.GetBySkuAsync(command.TenantId, command.Sku, ct);
        if (existing != null)
            throw new InvalidOperationException("Product with this SKU already exists");

        // Create
        var product = new Product { ... };
        await _repository.AddAsync(product, ct);

        // Return DTO
        return new ProductResult(...);
    }
}
```

### ✅ Regel 4: Queries haben keine Seiteneffekte
```csharp
// ✅ Richtig
public class GetProductHandler : IQueryHandler<GetProductQuery, ProductResult?>
{
    public async Task<ProductResult?> Handle(GetProductQuery query, CancellationToken ct)
    {
        // Nur lesen!
        var product = await _repository.GetByIdAsync(query.TenantId, query.ProductId, ct);
        return product != null ? new ProductResult(...) : null;
    }
}

// ❌ Falsch - Seiteneffekt in Query!
public class GetProductHandler : IQueryHandler<GetProductQuery, ProductResult?>
{
    public async Task<ProductResult?> Handle(GetProductQuery query, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(query.TenantId, query.ProductId, ct);
        
        // Seiteneffekt! ❌
        product.ViewCount++;
        await _repository.UpdateAsync(product, ct);
        
        return new ProductResult(...);
    }
}
```

### ✅ Regel 5: Exception Handling im Handler
```csharp
// Handler wirft Exceptions → Filter handhabt
public class CreateProductHandler : ICommandHandler<CreateProductCommand, ProductResult>
{
    public async Task<ProductResult> Handle(CreateProductCommand command, CancellationToken ct)
    {
        if (command.Price <= 0)
            throw new ArgumentException("Price must be > 0");  // ← ApiExceptionHandlingFilter fängt auf
        
        // ...
    }
}

// Controller wirft nicht!
public async Task<ActionResult<ProductResult>> CreateProduct(CreateProductRequest request, CancellationToken ct)
{
    var command = new CreateProductCommand(...);
    var result = await _messageBus.InvokeAsync<ProductResult>(command, ct);
    // Exceptions vom Handler werden automatisch gehandhabt
    return CreatedResponse(..., result);
}
```

---

## 🧪 Testing

CQRS macht Tests **viel einfacher**!

### Unit Test für Handler
```csharp
[Fact]
public async Task CreateProductHandler_WithValidInput_ReturnsProductResult()
{
    // Arrange
    var mockRepository = new Mock<IProductRepository>();
    var mockLogger = new Mock<ILogger<CreateProductHandler>>();
    var handler = new CreateProductHandler(mockRepository.Object, mockLogger.Object);

    var command = new CreateProductCommand(
        Guid.NewGuid(),  // TenantId
        "Test Product",
        "SKU001",
        99.99m);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("Test Product", result.Name);
    Assert.Equal("SKU001", result.Sku);
    Assert.Equal(99.99m, result.Price);

    // Verify repository was called
    mockRepository.Verify(
        x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
        Times.Once);
}
```

### Integration Test für Controller
```csharp
[Fact]
public async Task CreateProduct_WithValidRequest_ReturnsCreated()
{
    // Arrange
    var request = new CreateProductRequest(
        "Test Product",
        "SKU001",
        99.99m);

    // Act
    var response = await _httpClient.PostAsJsonAsync("/api/products", request);

    // Assert
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    var result = await response.Content.ReadAsAsync<ProductResult>();
    Assert.NotNull(result);
    Assert.Equal("Test Product", result.Name);
}
```

---

## 📊 Vorher vs. Nachher

### Vorher (Service Injection)
```csharp
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    
    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult> CreateProduct(CreateProductDto dto)
    {
        try
        {
            var product = await _service.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, "Error creating product");
        }
    }
}
```

**Probleme:**
- ❌ Try-catch in jedem Action
- ❌ Mixed concerns (HTTP + Business Logic)
- ❌ Schwer zu testen
- ❌ Code-Duplizierung

### Nachher (CQRS + Wolverine)
```csharp
public class ProductsController : ApiControllerBase
{
    private readonly IMessageBus _messageBus;
    
    public ProductsController(IMessageBus messageBus, ILogger<ProductsController> logger)
        : base(logger)
    {
        _messageBus = messageBus;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductResult>> CreateProduct(
        [FromBody] CreateProductRequest request,
        CancellationToken ct)
    {
        var tenantId = GetTenantId();
        var command = new CreateProductCommand(
            tenantId,
            request.Name,
            request.Sku,
            request.Price);

        var product = await _messageBus.InvokeAsync<ProductResult>(command, ct);
        return CreatedResponse(nameof(GetProduct), new { id = product.Id }, product);
    }
}
```

**Vorteile:**
- ✅ Keine Try-catch (Filter handled)
- ✅ Saubere Separation of Concerns
- ✅ Leicht zu testen
- ✅ Wiederverwendbar (mehrere Controller können den gleichen Handler nutzen)
- ✅ Event Sourcing ready

---

## 🔗 Verwandte Patterns

### Domain Events
Handler können Domain Events publishen für asynchrone Verarbeitung:

```csharp
public class CreateProductHandler : ICommandHandler<CreateProductCommand, ProductResult>
{
    public async Task<ProductResult> Handle(CreateProductCommand command, CancellationToken ct)
    {
        var product = new Product { ... };
        await _repository.AddAsync(product, ct);

        // Domain Event für asynchrone Verarbeitung
        product.AddDomainEvent(new ProductCreatedEvent(
            product.Id,
            product.TenantId,
            product.Name));

        // Product mit Events speichern
        await _repository.SaveAsync(product, ct);

        return new ProductResult(...);
    }
}
```

### Mediator Pattern (Alternative zu CQRS)
Wenn direkt Mediat or statt Wolverine genutzt wird:

```csharp
using MediatR;

var request = new CreateProductCommand(...);
var result = await _mediator.Send(request, ct);
```

---

## 📈 Skalierung

### Asynchrone Verarbeitung
```csharp
// Statt await - Fire and Forget
await _messageBus.PublishAsync(
    new SendEmailNotificationCommand(productId, customerId));
```

### Verteilte Systeme
Wolverine kann mit verschiedenen Transport-Mechanismen konfiguriert werden:
- In-Process (Standard)
- RabbitMQ
- Azure Service Bus
- AWS SQS

---

## ✅ Implementierungs-Checklist

- [x] Commands und Queries definiert
- [x] Handler implementiert
- [x] Controller refaktoriert
- [x] Wolverine integriert
- [x] Requests/Response DTOs
- [x] Exception Handling via Filter
- [x] Logging in Handlerin
- [ ] Domain Events implementieren
- [ ] Tests schreiben
- [ ] Dokumentation erweitern

---

**Status**: ✅ Ready for Production  
**Nächste Phase**: Domain Events & Event Sourcing  
**Last Updated**: 27. Dezember 2025
