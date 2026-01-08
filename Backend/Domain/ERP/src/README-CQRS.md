# ERP CQRS Integration with Wolverine

## Overview

This implementation provides a **hybrid architecture** that combines:

1. **Actor Pattern** in the ERP Connector (.NET 4.8) - handles thread-safety for enventa ERP
2. **CQRS with Wolverine** in the main B2X backend (.NET 10) - provides command/query separation and event-driven architecture

## Architecture

```
┌─────────────────────────────────────────────────────────────────────────┐
│                        B2X Backend (.NET 10)                      │
│  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────────┐  │
│  │   CQRS Layer    │ → │   Wolverine     │ → │   ErpService         │  │
│  │ (Commands/      │    │   Message Bus  │    │   (HTTP Client)      │  │
│  │  Queries)       │    │                 │    │                     │  │
│  └─────────────────┘    └─────────────────┘    └─────────────────────┘  │
│                                         │                               │
│                              HTTP (localhost:5080)                     │
│                                         ▼                               │
│  ┌─────────────────────────────────────────────────────────────────┐   │
│  │                ERP Connector (.NET 4.8)                        │   │
│  │  ┌─────────────────┐    ┌─────────────────┐    ┌─────────────┐   │   │
│  │  │ EnventaErpService│ → │ EnventaActorPool │ → │ enventa ERP │   │   │
│  │  │ (Business Logic) │    │ (Thread-safe)    │    │             │   │   │
│  │  └─────────────────┘    └─────────────────┘    └─────────────┘   │   │
│  └─────────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────────┘
```

## Key Components

### Commands & Queries
- **Commands**: Modify state (SyncArticlesCommand, BatchWriteArticlesCommand)
- **Queries**: Read data (GetArticleQuery, QueryArticlesQuery)
- **Events**: Published after operations (ArticlesSyncedEvent, ArticleAccessedEvent)

### Handlers
- **Command Handlers**: Execute business logic via ErpService, publish events
- **Query Handlers**: Retrieve data via ErpService, publish access events
- **Event Handlers**: Handle side effects (logging, caching, search indexing)

## Benefits

### ✅ Preserves Proven Actor Pattern
- Thread-safety for enventa ERP remains intact
- Single-threaded access per tenant/business-unit
- Cancellation support for long-running operations

### ✅ Adds CQRS Separation
- Clear separation between read/write operations
- Commands for state changes, queries for data retrieval
- Event-driven side effects

### ✅ Event-Driven Architecture
- Automatic event publishing after operations
- Decoupled side effects (search indexing, cache invalidation)
- Analytics and monitoring capabilities

### ✅ Maintains Existing Code
- ErpService continues to work unchanged
- HTTP communication layer unchanged
- Backward compatibility maintained

## Usage Examples

### Basic Query
```csharp
// Query a single article
var query = new GetArticleQuery("tenant1", "ART001");
var article = await _bus.InvokeAsync<ArticleDto?>(query);
```

### Command with Response
```csharp
// Sync articles and get response
var command = new SyncArticlesCommand("tenant1", syncRequest);
var response = await _bus.InvokeAsync<DeltaSyncResponse<ArticleDto>>(command);
```

### Fire-and-Forget Command
```csharp
// Send command without waiting for response
var command = new SyncArticlesCommand("tenant1", syncRequest);
await _bus.SendAsync(command);
```

## Configuration

### Service Registration
```csharp
services.AddErpIntegration(options =>
{
    options.BaseUrl = "http://localhost:5080";
});

services.AddErpWolverineIntegration(opts =>
{
    // Wolverine configuration
    opts.Policies.AutoApplyTransactions();
});
```

### Handler Discovery
Handlers are automatically discovered by Wolverine using naming conventions:
- `Handle(Command/Query/Event)` methods in static classes
- Dependency injection for required services

## Event Flow

1. **Command Executed** → Handler calls ErpService → Operation completes
2. **Event Published** → Wolverine routes to event handlers
3. **Side Effects** → Search indexing, cache invalidation, analytics

## Testing

The CQRS layer can be tested independently:

```csharp
[Fact]
public async Task SyncArticlesCommand_PublishesEvent()
{
    var command = new SyncArticlesCommand("tenant1", request);
    var response = await _bus.InvokeAsync<DeltaSyncResponse<ArticleDto>>(command);

    // Verify event was published
    var publishedEvents = await _bus.WaitForMessageOfType<ArticlesSyncedEvent>();
    Assert.NotNull(publishedEvents);
}
```

## Migration Path

### Phase 1: Add CQRS Layer (Current)
- Add commands, queries, events
- Add handlers that delegate to existing ErpService
- Add event publishing
- Test alongside existing code

### Phase 2: Optimize (Future)
- Add caching to query handlers
- Add saga patterns for complex operations
- Add durable messaging for reliability
- Add event sourcing if needed

### Phase 3: Replace Direct Usage (Future)
- Update controllers to use CQRS instead of direct ErpService calls
- Remove direct ErpService dependencies
- Full CQRS architecture

## Why Not Akka.NET?

While Akka.NET is powerful, it would be **overkill** for this use case:

- **Complexity**: Steep learning curve and configuration overhead
- **Scope**: Actor pattern already solved by custom implementation
- **Integration**: Wolverine already adopted in B2X (ADR-001)
- **Maintenance**: Custom actor pattern is simpler and purpose-built

## Summary

This hybrid approach provides the **best of both worlds**:

- **Thread-safety** via proven actor pattern in ERP connector
- **CQRS separation** via Wolverine in main backend
- **Event-driven** side effects and monitoring
- **Maintainable** and incrementally adoptable

The actor pattern handles the critical enventa thread-safety requirements, while Wolverine adds the architectural benefits of CQRS and event-driven design. 🎯