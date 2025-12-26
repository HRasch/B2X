# Wolverine-Spezifische CQRS Implementation
## Praktische Patterns und Beispiele

**Datum:** 26. Dezember 2025  
**Framework:** Wolverine (mit Message Bus & CQRS Support)  

---

## 🎯 Wolverine vs MediatR für CQRS

### Warum Wolverine besser für diese Lösung ist:

| Feature | MediatR | Wolverine |
|---------|---------|-----------|
| **Message Bus** | ❌ Zusätzliches Package | ✅ Integriert |
| **Mediator Pattern** | ✅ Core Feature | ✅ Built-in |
| **Async Events** | ❌ Über Zusätze | ✅ Native |
| **Transactional Outbox** | ❌ Manuell | ✅ Automatisch |
| **Dead Letter Queue** | ❌ Nein | ✅ Ja |
| **Retry Policies** | ❌ Nein | ✅ Konfigurierbar |
| **Handler Discovery** | Manuell | ✅ Automatisch |
| **Cloud Support** | ❌ Nein | ✅ AWS/Azure/GCP |
| **Performance** | Gut | ⭐⭐⭐ Besser |

---

## 📋 Praktische Implementierungs-Patterns

### Pattern 1: Synchronous Command (Immediate Response)

```csharp
// Command Definition
public class CreateProductCommand : ICommand<CreateProductResult>
{
    public Guid TenantId { get; set; }
    public string Sku { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class CreateProductResult
{
    public Guid ProductId { get; set; }
    public bool Success { get; set; }
}

// Handler
public class CreateProductCommandHandler : 
    ICommandHandler<CreateProductCommand>
{
    private readonly CatalogDbContext _context;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public async Task<CreateProductResult> Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            // Create product
            var product = Product.Create(command.Sku, command.Name, command.Price);
            product.TenantId = command.TenantId;

            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            // Publish event asynchronously (Wolverine handles this)
            await _messageBus.PublishAsync(
                new ProductCreatedEvent 
                { 
                    ProductId = product.Id,
                    TenantId = command.TenantId,
                    Sku = command.Sku,
                    Name = command.Name
                },
                cancellation: cancellationToken);

            return new CreateProductResult 
            { 
                ProductId = product.Id, 
                Success = true 
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return new CreateProductResult { Success = false };
        }
    }
}

// Usage in Controller
[HttpPost("products")]
public async Task<IActionResult> CreateProduct(
    [FromBody] CreateProductCommand command,
    [FromServices] IMessageBus messageBus,
    CancellationToken cancellationToken)
{
    // Wolverine handles invocation automatically
    var result = await messageBus.InvokeAsync(command, cancellationToken);
    
    return result.Success 
        ? CreatedAtAction(nameof(GetProduct), new { id = result.ProductId }, result)
        : BadRequest("Failed to create product");
}
```

### Pattern 2: Asynchronous Event Handling

```csharp
// Domain Event
public class ProductCreatedEvent
{
    public Guid ProductId { get; set; }
    public Guid TenantId { get; set; }
    public string Sku { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// Multiple handlers can subscribe to same event
public class ProductCreatedReadModelHandler : 
    IEventHandler<ProductCreatedEvent>
{
    private readonly CatalogReadDbContext _readContext;
    private readonly ILogger<ProductCreatedReadModelHandler> _logger;

    public async Task Handle(
        ProductCreatedEvent @event,
        CancellationToken cancellationToken)
    {
        // Update read model with denormalized data
        var readModel = new ProductReadModel
        {
            Id = @event.ProductId,
            TenantId = @event.TenantId,
            Sku = @event.Sku,
            Name = @event.Name,
            Price = @event.Price,
            CreatedAt = @event.CreatedAt,
            SearchText = $"{@event.Sku} {@event.Name}"
        };

        _readContext.ProductsReadModel.Add(readModel);
        await _readContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Read model created for {Sku}", @event.Sku);
    }
}

public class ProductCreatedNotificationHandler : 
    IEventHandler<ProductCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<ProductCreatedNotificationHandler> _logger;

    public async Task Handle(
        ProductCreatedEvent @event,
        CancellationToken cancellationToken)
    {
        // Send notification to admins
        await _emailService.SendAdminNotificationAsync(
            $"New product created: {}", @event.Name,
            cancellationToken);
        
        _logger.LogInformation("Notification sent for {Sku}", @event.Sku);
    }
}
```

### Pattern 3: Query Handler (Synchronous Request/Reply)

```csharp
// Query
public class GetProductQuery : IQuery<ProductDto>
{
    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }
}

// Handler - Wolverine automatically invokes this synchronously
public class GetProductQueryHandler : 
    IQueryHandler<GetProductQuery, ProductDto>
{
    private readonly CatalogReadDbContext _readContext;
    private readonly IDistributedCache _cache;
    private readonly ILogger<GetProductQueryHandler> _logger;

    public async Task<ProductDto> Handle(
        GetProductQuery query,
        CancellationToken cancellationToken)
    {
        // Try cache
        var cacheKey = $"product:{query.TenantId}:{query.ProductId}";
        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        
        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<ProductDto>(cached)!;
        }

        // Query denormalized read model
        var product = await _readContext.ProductsReadModel
            .AsNoTracking()
            .Where(p => p.Id == query.ProductId && p.TenantId == query.TenantId)
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null)
            throw new NotFoundException($"Product {query.ProductId} not found");

        var dto = MapToDto(product);

        // Cache for 1 hour
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(dto),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            },
            cancellationToken);

        return dto;
    }

    private ProductDto MapToDto(ProductReadModel model) => 
        new()
        {
            Id = model.Id,
            Sku = model.Sku,
            Name = model.Name,
            Price = model.Price,
            // ... other fields
        };
}

// Controller - Wolverine routes to handler automatically
[HttpGet("products/{id}")]
public async Task<IActionResult> GetProduct(
    Guid id,
    [FromServices] IMessageBus messageBus,
    CancellationToken cancellationToken)
{
    var query = new GetProductQuery 
    { 
        TenantId = GetTenantIdFromContext(),
        ProductId = id 
    };

    // Wolverine finds and invokes GetProductQueryHandler synchronously
    var product = await messageBus.InvokeAsync(query, cancellationToken);
    
    return Ok(product);
}
```

### Pattern 4: Scheduled Commands

```csharp
// Scheduled command - executes at specific time
public class RebuildCatalogIndexCommand : ICommand
{
    public Guid TenantId { get; set; }
}

public class RebuildCatalogIndexHandler : 
    ICommandHandler<RebuildCatalogIndexCommand>
{
    private readonly ISearchService _searchService;
    private readonly ILogger<RebuildCatalogIndexHandler> _logger;

    public async Task Handle(
        RebuildCatalogIndexCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Rebuilding search index for tenant {TenantId}", 
            command.TenantId);

        await _searchService.RebuildIndexAsync(command.TenantId, cancellationToken);
        
        _logger.LogInformation("Search index rebuild completed");
    }
}

// In startup service or controller - schedule the command
public class CatalogService
{
    private readonly IMessageBus _messageBus;

    public async Task ScheduleIndexRebuildAsync(Guid tenantId)
    {
        // Schedule to run tomorrow at 2 AM
        var scheduledTime = DateTime.UtcNow.AddDays(1)
            .Date
            .AddHours(2);

        await _messageBus.ScheduleAsync(
            new RebuildCatalogIndexCommand { TenantId = tenantId },
            scheduledTime);
    }

    public async Task ScheduleRecurringCleanupAsync()
    {
        // Schedule recurring cleanup every day at 3 AM
        await _messageBus.ScheduleRecurringAsync(
            new CleanupExpiredSessionsCommand(),
            "daily-cleanup",
            schedule: "0 3 * * *");  // Cron format
    }
}
```

### Pattern 5: Error Handling & Dead Letter Queue

```csharp
// Program.cs - Configure error handling
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);

    // Configure failure handling per message type
    opts.Handlers.OnException<ValidationException>()
        .Discard();  // Validation errors don't retry

    opts.Handlers.OnException<TransientException>()
        .Retry
        .MaximumAttempts(3)
        .WithDelayInSeconds(1, 2, 5);  // 1s, 2s, 5s delays

    opts.Handlers.OnException<TimeoutException>()
        .Retry
        .MaximumAttempts(2)
        .WithDelayInSeconds(10);

    // Everything else goes to Dead Letter Queue after max attempts
    opts.Handlers.OnException<Exception>()
        .Retry
        .MaximumAttempts(5)
        .Then
        .MoveToDeadLetterQueue();

    // Configure Dead Letter Queue handling
    opts.DeadLetterQueue.IsEnabled = true;
});

// Separately handle Dead Letter Queue messages
public class DeadLetterQueueMonitor : IHostedService
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<DeadLetterQueueMonitor> _logger;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Monitor DLQ for failed messages
        // Implement recovery logic, alerts, etc.
        _logger.LogWarning("Dead Letter Queue monitoring started");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
```

### Pattern 6: Transactional Outbox (Guaranteed Delivery)

```csharp
// Wolverine automatically handles this, but here's how it works:

public class CreateProductCommandHandler : 
    ICommandHandler<CreateProductCommand>
{
    private readonly CatalogDbContext _context;
    private readonly IMessageBus _messageBus;

    public async Task Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Save product to database
        var product = Product.Create(command.Sku, command.Name, command.Price);
        _context.Products.Add(product);

        // 2. Save event to outbox in SAME transaction
        var @event = new ProductCreatedEvent { /* ... */ };
        
        // Wolverine's InvokeAsync() uses outbox automatically
        // If SaveChanges() succeeds, both product AND event are persisted
        // Then event is published asynchronously
        await _context.SaveChangesAsync(cancellationToken);

        // 3. Publish event asynchronously
        // Wolverine uses the persisted outbox message
        await _messageBus.PublishAsync(@event, cancellation: cancellationToken);

        // If this service crashes between steps 2 and 3:
        // - Product is safely in database
        // - Event is in outbox
        // - On restart, Wolverine reprocesses outbox
        // - Guaranteed delivery!
    }
}
```

### Pattern 7: Multi-Tenant Command Validation

```csharp
// Command with tenant isolation
public class UpdateProductCommand : ICommand<CommandResult>
{
    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class UpdateProductCommandValidator : 
    AbstractValidator<UpdateProductCommand>
{
    private readonly IProductRepository _repository;

    public UpdateProductCommandValidator(IProductRepository repository)
    {
        _repository = repository;

        RuleFor(c => c.ProductId)
            .NotEmpty()
            .MustAsync(async (productId, tenantId, cancellationToken) =>
            {
                // Validate product exists for THIS tenant
                var product = await _repository
                    .GetByIdAsync(tenantId, productId, cancellationToken);
                
                return product != null;  // Only true if tenant matches
            })
            .WithMessage("Product not found for tenant");

        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(c => c.Price)
            .GreaterThan(0);
    }
}

public class UpdateProductCommandHandler : 
    ICommandHandler<UpdateProductCommand>
{
    private readonly IProductRepository _repository;
    private readonly IMessageBus _messageBus;

    public async Task<CommandResult> Handle(
        UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        // Repository automatically filters by TenantId
        var product = await _repository.GetByIdAsync(
            command.TenantId, 
            command.ProductId, 
            cancellationToken);

        if (product == null)
            throw new NotFoundException("Product not found");

        // Update
        product.Name = command.Name;
        product.Price = command.Price;

        await _repository.UpdateAsync(product, cancellationToken);

        // Publish event
        await _messageBus.PublishAsync(
            new ProductUpdatedEvent 
            { 
                ProductId = product.Id,
                TenantId = command.TenantId,
                Name = command.Name,
                Price = command.Price
            },
            cancellation: cancellationToken);

        return CommandResult.Ok(product.Id);
    }
}
```

---

## 🧪 Testing Wolverine Handlers

### Unit Test Example

```csharp
[TestFixture]
public class CreateProductCommandHandlerTests
{
    private Mock<CatalogDbContext> _contextMock;
    private Mock<IMessageBus> _messageBusMock;
    private Mock<IValidator<CreateProductCommand>> _validatorMock;
    private CreateProductCommandHandler _handler;

    [SetUp]
    public void Setup()
    {
        _contextMock = new Mock<CatalogDbContext>();
        _messageBusMock = new Mock<IMessageBus>();
        _validatorMock = new Mock<IValidator<CreateProductCommand>>();

        var loggerMock = new Mock<ILogger<CreateProductCommandHandler>>();
        _handler = new CreateProductCommandHandler(
            _contextMock.Object,
            _repositoryMock.Object,
            _messageBusMock.Object,
            _validatorMock.Object,
            loggerMock.Object);
    }

    [Test]
    public async Task Handle_WithValidCommand_CreatesProductAndPublishesEvent()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            TenantId = Guid.NewGuid(),
            Sku = "TEST-001",
            Name = "Test Product",
            Price = 99.99m
        };

        _validatorMock
            .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());  // Valid

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);

        // Verify event was published
        _messageBusMock.Verify(
            b => b.PublishAsync(
                It.Is<ProductCreatedEvent>(e => e.Sku == "TEST-001"),
                It.IsAny<DeliveryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
```

### Integration Test with Wolverine Host

```csharp
[TestFixture]
public class CatalogServiceIntegrationTests : IAsyncLifetime
{
    private WolverineHost _host;

    public async Task InitializeAsync()
    {
        _host = await Host.CreateDefaultBuilder()
            .UseWolverine((context, opts) =>
            {
                // Use in-memory transport for tests
                opts.UseInMemoryTransport();

                // Auto-discover all handlers
                opts.Discovery.IncludeAssembly(typeof(Program).Assembly);

                // Register test services
                opts.Services.AddScoped<CatalogDbContext>();
                opts.Services.AddScoped<IProductRepository, InMemoryProductRepository>();
            })
            .StartAsync();
    }

    [Test]
    public async Task CreateProduct_UpdatesReadModel()
    {
        // Arrange
        var messageBus = _host.GetRuntime().Bus;
        var command = new CreateProductCommand
        {
            TenantId = Guid.NewGuid(),
            Sku = "SKU-123",
            Name = "Test Product",
            Price = 50.00m
        };

        // Act
        var result = await messageBus.InvokeAsync(command);

        // Assert
        Assert.That(result.Success, Is.True);

        // Verify read model was updated
        var readDbContext = _host.Services
            .GetRequiredService<CatalogReadDbContext>();
        
        var readModel = await readDbContext.ProductsReadModel
            .FirstOrDefaultAsync(p => p.Id == result.ProductId);

        Assert.That(readModel, Is.Not.Null);
        Assert.That(readModel.Name, Is.EqualTo("Test Product"));
    }

    public async Task DisposeAsync()
    {
        await _host.StopAsync();
        _host.Dispose();
    }
}
```

---

## 📊 Performance Comparison

### Query Performance mit CQRS + Wolverine

```csharp
// Before CQRS (Synchronous Read/Write)
SELECT p.*, c.*, b.*, i.* 
FROM Products p
LEFT JOIN Categories c ON p.CategoryId = c.Id
LEFT JOIN Brands b ON p.BrandId = b.Id
LEFT JOIN Inventory i ON p.Id = i.ProductId
WHERE p.TenantId = @TenantId
LIMIT 20

// ❌ Full scan on 10M rows, multiple joins
// ⏱️ 2-3 seconds

// After CQRS (Denormalized Read Model)
SELECT * FROM products_read_model 
WHERE tenant_id = @TenantId 
ORDER BY created_at DESC
LIMIT 20

// ✅ Index scan, no joins, aggregated data
// ⏱️ 50-100ms
```

---

## 🎯 Migration Pfad: Current → CQRS

### Phase 1: Parallel Implementation
1. Keep existing CatalogService working
2. Add Wolverine handlers alongside
3. Sync read model via events
4. 2-3 weeks

### Phase 2: Switchover
1. Redirect GET requests to read model
2. Redirect POST/PUT to command handlers
3. Monitor for consistency
4. 1 week

### Phase 3: Cleanup
1. Remove legacy synchronous code
2. Optimize denormalized schema
3. Fine-tune indexes
4. 1 week

---

## 📚 Key Links

- [Wolverine Documentation](https://wolverine.netlify.app/)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [Transactional Outbox](https://microservices.io/patterns/data/transactional-outbox.html)

