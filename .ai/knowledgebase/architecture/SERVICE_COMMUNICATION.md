---
docid: KB-069
title: SERVICE_COMMUNICATION
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿# Service Communication Guide

**Last Updated:** December 30, 2025  
**Maintained By:** @Architect, @TechLead  
**Knowledge Base:** Architecture

---

## Overview

Guide to how services in B2X communicate with each other, including sync/async patterns, event contracts, and best practices.

---

## Communication Patterns

### Pattern 1: Synchronous HTTP/REST (Internal)

**When to Use:**
- Request-response needed immediately
- Simple queries (not complex business rules)
- API Gateway to service calls
- Admin operations

**Example: API Gateway to Catalog Service**

```
Client Request
    │
    ▼
API Gateway (Store)
    │ HTTP GET /api/v1/products
    ▼
Catalog Service
    │ Product search, return results
    ▼
API Gateway Response
    │
    ▼
Client Response (JSON)
```

**Code Example:**

```csharp
// Service A: API Gateway
[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly HttpClient _catalogClient;
    
    [HttpGet("{id}")]
    public async Task<ProductDto> GetProduct(int id)
    {
        var response = await _catalogClient.GetAsync(
            $"/internal/products/{id}");
        
        return await response.Content
            .ReadAsAsync<ProductDto>();
    }
}

// Service B: Catalog Service (Internal endpoint)
[ApiController]
[Route("internal/products")]
public class InternalProductsController : ControllerBase
{
    private readonly ProductRepository _repository;
    
    [HttpGet("{id}")]
    public async Task<ProductDto> GetProduct(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        return ProductMapper.ToDto(product);
    }
}
```

**Pros:**
- Simple, familiar pattern
- Immediate response
- Easy to debug

**Cons:**
- Tight coupling if overused
- Cascading failures (A fails → B fails)
- Blocking calls reduce scalability

**Guidelines:**
- Use only for queries or admin operations
- Internal services only (not public APIs)
- Add timeouts & circuit breakers
- Log requests for debugging

---

### Pattern 2: Asynchronous Events (Recommended)

**When to Use:**
- One service needs to notify others of state change
- Multiple services need to react
- Can handle temporary delays (seconds)
- Audit trail required

**Example: Product Inventory Update Notification**

```
Catalog Service
    │ Update product stock
    ▼
Event Published
    │ ProductStockUpdatedEvent
    ▼
Message Broker (RabbitMQ)
    │
    ├──> Search Service (Reindex product)
    ├──> Email Service (Notify customers)
    └──> Analytics Service (Track inventory changes)
```

**Code Example:**

```csharp
// Service A: Catalog Service (Publisher)
public class UpdateProductStockHandler
{
    private readonly IMessageBus _bus;
    private readonly ProductRepository _repository;
    
    public async Task Handle(UpdateProductStockCommand command)
    {
        // Update database
        var product = await _repository.GetByIdAsync(command.ProductId);
        product.Stock = command.NewStock;
        await _repository.SaveAsync(product);
        
        // Publish event
        var evt = new ProductStockUpdatedEvent
        {
            ProductId = product.Id,
            PreviousStock = product.Stock,
            NewStock = command.NewStock,
            Timestamp = DateTime.UtcNow
        };
        
        await _bus.PublishAsync(evt);
    }
}

// Service B: Search Service (Subscriber)
public class ProductStockIndexHandler
{
    private readonly ElasticsearchService _search;
    
    [WolverineHandler]
    public async Task Handle(ProductStockUpdatedEvent evt)
    {
        // Update search index
        await _search.UpdateAsync(new
        {
            id = evt.ProductId,
            stock = evt.NewStock,
            updated_at = evt.Timestamp
        });
    }
}

// Service C: Email Service (Subscriber)
public class ProductRestockNotificationHandler
{
    private readonly EmailService _email;
    
    [WolverineHandler]
    public async Task Handle(ProductStockUpdatedEvent evt)
    {
        // Only notify if product was out of stock
        if (evt.PreviousStock == 0 && evt.NewStock > 0)
        {
            await _email.SendRestockNotificationAsync(evt.ProductId);
        }
    }
}
```

**Pros:**
- Loose coupling (services don't know about each other)
- Scalable (async processing)
- Resilient (queued if service down)
- Audit trail (event log)

**Cons:**
- Eventual consistency (not immediate)
- More complex error handling
- Requires saga pattern for rollbacks
- More infrastructure (message broker)

**Guidelines:**
- Prefer events for cross-service notifications
- Include relevant context in event (don't require service lookup)
- Handle idempotency (same event processed twice = same result)
- Implement dead letter queue handling
- Log all published/consumed events

---

### Pattern 3: Saga Pattern (Distributed Transactions)

**When to Use:**
- Multi-step process spanning services
- Need rollback capability
- Coordination required
- Example: Create order → Reserve inventory → Process payment

**Orchestration-Based (Central Coordinator)**

```csharp
// Order Service (Orchestrator)
public class CreateOrderSaga
{
    private readonly IMessageBus _bus;
    
    [WolverineHandler]
    public async Task Handle(CreateOrderCommand command, 
        Guid correlationId)
    {
        // Step 1: Reserve inventory
        await _bus.PublishAsync(
            new ReserveInventoryCommand 
            { 
                OrderId = command.Id,
                Items = command.Items
            },
            opts => opts.CorrelationId(correlationId));
        
        // Wait for response
        var reserved = await _bus.RequestAsync<InventoryReservedEvent>(
            timeout: TimeSpan.FromSeconds(10));
        
        if (!reserved.Success)
            throw new InventoryReservationException();
        
        // Step 2: Process payment
        await _bus.PublishAsync(
            new ProcessPaymentCommand 
            { 
                OrderId = command.Id,
                Amount = command.Total
            },
            opts => opts.CorrelationId(correlationId));
        
        var paid = await _bus.RequestAsync<PaymentProcessedEvent>();
        
        if (!paid.Success)
        {
            // Compensation: Release reserved inventory
            await _bus.PublishAsync(
                new ReleaseInventoryCommand { OrderId = command.Id });
            throw new PaymentFailedException();
        }
        
        // Step 3: Create order
        var order = new Order { /* ... */ };
        await _orderRepository.SaveAsync(order);
        
        await _bus.PublishAsync(new OrderCreatedEvent { /* ... */ });
    }
}
```

**Choreography-Based (Event Chain)**

```csharp
// Each service reacts to events and publishes new ones
// Order Service
public class CreateOrderHandler
{
    public async Task Handle(CreateOrderCommand cmd, 
        IMessageContext context)
    {
        var order = new Order { /* ... */ };
        await context.PublishAsync(new OrderCreatedEvent { OrderId = order.Id });
    }
}

// Inventory Service (listens to OrderCreatedEvent)
public class OrderCreatedInventoryHandler
{
    public async Task Handle(OrderCreatedEvent evt, 
        IMessageContext context)
    {
        var reserved = await _inventoryService.ReserveAsync(evt.Items);
        if (reserved)
            await context.PublishAsync(
                new InventoryReservedEvent { OrderId = evt.OrderId });
        else
            await context.PublishAsync(
                new InventoryReservationFailedEvent { OrderId = evt.OrderId });
    }
}

// Payment Service (listens to InventoryReservedEvent)
public class InventoryReservedPaymentHandler
{
    public async Task Handle(InventoryReservedEvent evt, 
        IMessageContext context)
    {
        var paid = await _paymentService.ProcessAsync();
        if (paid)
            await context.PublishAsync(
                new PaymentProcessedEvent { OrderId = evt.OrderId });
        else
            // Compensation: release inventory
            await context.PublishAsync(
                new ReleaseInventoryCommand { OrderId = evt.OrderId });
    }
}
```

**Pros:**
- Handles complex multi-service workflows
- Compensation transactions for rollback
- Maintains audit trail

**Cons:**
- More complex than simple events
- Orchestration creates coupling
- Error handling tricky
- Testing challenging

**Guidelines:**
- Use orchestration for important flows (orders, payments)
- Use choreography for independent events
- Always have compensation handlers
- Log all saga steps
- Monitor saga completion rates

---

## Event Contracts

### Event Structure

All events should follow this contract:

```csharp
public abstract class DomainEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public Guid AggregateId { get; set; } // Entity being modified
    public string AggregateType { get; set; } // e.g., "Product"
    public int Version { get; set; } // Event version for schema evolution
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public Guid CorrelationId { get; set; } // For tracing request flow
    public Guid CausationId { get; set; } // What caused this event
    public string Source { get; set; } // Service name
}

// Example: Concrete event
public class ProductCreatedEvent : DomainEvent
{
    public string Name { get; set; }
    public string Sku { get; set; }
    public decimal Price { get; set; }
    public int InitialStock { get; set; }
}
```

### Event Versioning

```csharp
// V1: Original event
public class ProductCreatedEventV1 : DomainEvent
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// V2: Added new field (backward compatible)
public class ProductCreatedEventV2 : DomainEvent
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string CategoryId { get; set; } // NEW
}

// Handler supports both versions
public class ProductIndexHandler
{
    [WolverineHandler]
    public async Task Handle(ProductCreatedEventV1 evt) => /* ... */
    
    [WolverineHandler]
    public async Task Handle(ProductCreatedEventV2 evt) => /* ... */
}
```

---

## Error Handling

### Retry Strategy

```csharp
// Wolverine auto-retry configuration
builder.Services.AddWolverine(opts =>
{
    opts.Policies.AutoRetry()
        .MaximumAttempts(3)
        .OnFailureGoToDeadLetterQueue();
});
```

### Dead Letter Queue

```csharp
// Handle failed messages
public class DeadLetterHandler
{
    private readonly ILogger<DeadLetterHandler> _logger;
    
    [WolverineHandler]
    public async Task Handle(DeadLetterEnvelope envelope)
    {
        _logger.LogError($"Message failed after retries: {envelope.MessageId}");
        
        // Alert ops team
        await _alerting.NotifyAsync(new Alert
        {
            Severity = "Critical",
            Message = "Message processing failed",
            Details = envelope
        });
        
        // Store for later analysis
        await _deadLetterRepository.StoreAsync(envelope);
    }
}
```

---

## Best Practices

### ✅ DO

1. **Include context in events**
   ```csharp
   // Good: Event has all info needed
   public class ProductCreatedEvent : DomainEvent
   {
       public string Name { get; set; }
       public string Description { get; set; }
       public decimal Price { get; set; }
   }
   ```

2. **Handle idempotency**
   ```csharp
   // Same event processed twice = same result
   public async Task Handle(ProductCreatedEvent evt)
   {
       var exists = await _search.ExistsAsync(evt.AggregateId);
       if (exists) return; // Already processed
       
       await _search.IndexAsync(evt);
   }
   ```

3. **Log everything**
   ```csharp
   _logger.LogInformation("Published {EventType} for {AggregateType} {AggregateId}",
       evt.GetType().Name, evt.AggregateType, evt.AggregateId);
   ```

4. **Use correlation IDs**
   ```csharp
   // Trace request across services
   var correlationId = context.CorrelationId;
   _logger.LogInformation("Event {EventId} [Correlation: {CorrelationId}]",
       evt.EventId, correlationId);
   ```

### ❌ DON'T

1. **Don't require service lookups**
   ```csharp
   // Bad: Handler must fetch additional data
   public async Task Handle(ProductCreatedEvent evt)
   {
       var product = await _catalogClient.GetProductAsync(evt.ProductId);
       var category = await _catalogClient.GetCategoryAsync(product.CategoryId);
       // ...
   }
   
   // Good: Event has everything needed
   public async Task Handle(ProductCreatedEvent evt)
   {
       await _search.IndexAsync(new SearchDoc
       {
           Id = evt.ProductId,
           Name = evt.Name,
           Category = evt.CategoryId
       });
   }
   ```

2. **Don't create circular dependencies**
   ```csharp
   // Bad: A → B → A
   // Service A publishes OrderCreated
   // Service B listens and publishes ProductStockUpdated
   // Service A listens and...
   
   // Good: Use sagas for complex flows
   ```

3. **Don't ignore failures**
   ```csharp
   // Bad: Silent failures
   try { await _search.IndexAsync(evt); }
   catch { } // Ignored!
   
   // Good: Handle explicitly
   try { await _search.IndexAsync(evt); }
   catch (Exception ex)
   {
       _logger.LogError(ex, "Failed to index product");
       throw; // Let Wolverine retry
   }
   ```

4. **Don't block event processing**
   ```csharp
   // Bad: Long-running operation blocks event thread
   public async Task Handle(ProductCreatedEvent evt)
   {
       await LongRunningReportGeneration(evt); // 5+ minutes!
   }
   
   // Good: Queue for background job
   public async Task Handle(ProductCreatedEvent evt)
   {
       await _backgroundJobQueue.EnqueueAsync(
           new GenerateReportJob { ProductId = evt.AggregateId });
   }
   ```

---

## Monitoring & Debugging

### Key Metrics to Track

1. **Message Throughput**
   - Events published per second
   - Events processed per second
   - Queue depth

2. **Latency**
   - Event processing time
   - End-to-end flow time
   - Percentiles (p50, p95, p99)

3. **Errors**
   - Failed message count
   - Retry count
   - Dead letter queue size

4. **Health**
   - Handler status
   - Message broker health
   - Service availability

### Debugging Distributed Events

```csharp
// Trace an event flow
var correlationId = Guid.NewGuid();

// Service A publishes
_logger.LogInformation("Publishing {Event} [Correlation: {CorrelationId}]",
    nameof(OrderCreatedEvent), correlationId);

// Service B receives
_logger.LogInformation("Processing {Event} [Correlation: {CorrelationId}]",
    nameof(OrderCreatedEvent), context.CorrelationId);

// Query logs by correlation ID to see full flow
```

---

## References

- [B2X Architecture Review](../../decisions/ARCHITECTURE_REVIEW_2025_12_30.md)
- [ADR-001: Event-Driven Architecture](../../decisions/ADR-001-event-driven-architecture.md)
- [Wolverine Documentation](https://wolverine.netlify.app/)
- [Service Communication Patterns](https://microservices.io/patterns/communication-style/messaging.html)

---

**Last Updated:** December 30, 2025  
**Review Date:** March 30, 2026
