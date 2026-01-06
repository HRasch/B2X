# Wolverine Integration Implementation

## Overview
Wolverine messaging has been successfully integrated into the following backend services:
- Catalog Service
- Identity Service  
- CMS Service
- Localization Service

## Architecture

### Message Types

#### Events (Domain Events)
Events are published when something significant happens in the system and other services may need to react to it.

**Catalog Events:**
- `ProductCreatedEvent`
- `ProductUpdatedEvent`
- `ProductDeletedEvent`
- `ProductStockUpdatedEvent`
- `ProductPriceChangedEvent`

**Identity Events:**
- `UserRegisteredEvent`
- `UserLoggedInEvent`
- `PasswordResetEvent`
- `EmailVerifiedEvent`
- `UserRoleChangedEvent`

**CMS Events:**
- `PageCreatedEvent`
- `PageUpdatedEvent`
- `PagePublishedEvent`
- `PageUnpublishedEvent`
- `PageDeletedEvent`

#### Commands
Commands represent requests to perform an action.

- `IndexProductCommand`
- `RemoveProductFromIndexCommand`
- `ReindexAllProductsCommand`
- `SendEmailCommand`
- `GenerateInvoiceCommand`

## Configuration

### Local Queue (Development)
By default, services use local queues for development:

```json
{
  "Messaging": {
    "UseRabbitMq": false
  }
}
```

### RabbitMQ (Production)
For production, enable RabbitMQ:

```json
{
  "Messaging": {
    "UseRabbitMq": true
  },
  "RabbitMq": {
    "Uri": "amqp://username:password@rabbitmq-host:5672"
  }
}
```

## Service Implementation

### Catalog Service
Location: `/backend/BoundedContexts/Store/Catalog`

**Event Handlers:**
- `ProductEventHandlers.cs` - Handles product lifecycle events

**Configuration:**
- Program.cs updated with Wolverine setup
- Event handlers auto-discovered via assembly scanning

### Identity Service
Location: `/backend/BoundedContexts/Shared/Identity`

**Event Handlers:**
- `UserEventHandlers.cs` - Handles user lifecycle events

### CMS Service
Location: `/backend/BoundedContexts/Store/CMS`

**Event Handlers:**
- `PageEventHandlers.cs` - Handles CMS page events

### Localization Service
Location: `/backend/BoundedContexts/Store/Localization`

**Configuration:**
- Wolverine messaging enabled for future localization events

## Publishing Events

To publish an event from a service:

```csharp
using Wolverine;

public class ProductService
{
    private readonly IMessageBus _messageBus;
    
    public ProductService(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }
    
    public async Task CreateProduct(ProductDto product)
    {
        // Create product logic...
        
        // Publish event
        await _messageBus.PublishAsync(new ProductCreatedEvent(
            tenantId: product.TenantId,
            productId: product.Id,
            productName: product.Name,
            sku: product.Sku,
            price: product.Price,
            createdAt: DateTimeOffset.UtcNow
        ));
    }
}
```

## Event Handlers

Handlers are automatically discovered by Wolverine through assembly scanning:

```csharp
public class ProductCreatedEventHandler
{
    private readonly ILogger<ProductCreatedEventHandler> _logger;

    public ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(ProductCreatedEvent @event)
    {
        _logger.LogInformation("Product created: {ProductId}", @event.ProductId);
        // Handle the event...
    }
}
```

## Next Steps

### Immediate TODOs
1. **Implement actual handler logic** - Currently handlers only log events
2. **Add Service Integration:**
   - Search index updates in product handlers
   - Email sending in identity handlers
   - Cache invalidation in CMS handlers
3. **Add Command Handlers** for cross-service operations
4. **Configure RabbitMQ** for production environments
5. **Add Retry Policies** and error handling
6. **Add Integration Tests** for message flows

### Future Enhancements
1. **Event Sourcing** - Store events for audit and replay
2. **Saga Support** - For complex multi-service workflows
3. **Dead Letter Queues** - For failed message handling
4. **Message Versioning** - For backward compatibility
5. **Monitoring** - Message flow observability

## Testing

### Unit Testing Handlers
```csharp
[Fact]
public async Task ProductCreatedEventHandler_LogsEvent()
{
    // Arrange
    var logger = new Mock<ILogger<ProductCreatedEventHandler>>();
    var handler = new ProductCreatedEventHandler(logger.Object);
    var @event = new ProductCreatedEvent(/*...*/);
    
    // Act
    await handler.Handle(@event);
    
    // Assert
    logger.Verify(/* verify logging */);
}
```

### Integration Testing
Use Wolverine's test support for end-to-end message flow testing.

## Documentation References
- [Wolverine Documentation](https://wolverine.netlify.app/)
- [Event-Driven Architecture Guide](../docs/architecture/event-driven.md)
- [Messaging Best Practices](../docs/guides/messaging.md)
