# Elasticsearch Integration Implementation Guide

## Implementation Status

This document tracks the implementation of Elasticsearch integration for product search and catalog management.

### Completed Components

#### 1. Domain Events (✅ Complete)
- **File**: `backend/services/CatalogService/Events/ProductCreatedEvent.cs`
- **Classes**:
  - `ProductCreatedEvent` - Triggered when new product is added
  - `ProductUpdatedEvent` - Triggered when product is modified
  - `ProductDeletedEvent` - Triggered when product is removed
  - `ProductsBulkImportedEvent` - Triggered on bulk import
  - `ProductAttributesDto` - Product attribute model

**Key Features**:
- All events inherit from `DomainEvent` base class
- Tenant isolation support
- Full product data serialization
- Event metadata and tracking

#### 2. Elasticsearch Models (✅ Complete)
- **File**: `backend/services/SearchService/Models/ProductIndexDocument.cs`
- **Classes**:
  - `ProductIndexDocument` - Elasticsearch document mapping
  - `ProductSearchQueryRequest` - Search request DTO
  - `ProductSearchResponseDto` - Search results DTO
  - `ProductSearchResultItemDto` - Individual result item
  - `SearchSuggestionDto` - Autocomplete suggestions
  - `FacetResultDto` - Facet/filter options
  - `AggregationResultDto` - Aggregation results

**Key Features**:
- Full-featured product indexing
- Support for complex filtering
- Faceted navigation
- Aggregations for analytics

#### 3. Elasticsearch Index Configuration (✅ Complete)
- **File**: `backend/services/SearchService/Config/products-index-mapping.json`
- **Configuration**:
  - 3 shards, 1 replica for HA
  - Custom analyzers (text, autocomplete)
  - Complete field mappings
  - Performance tuning (refresh_interval: 1s)

**Key Features**:
- Edge n-gram analyzer for autocomplete
- Scaled floats for prices
- Keyword fields for exact matching
- Full-text search analyzers

#### 4. Search Index Service (✅ Complete)
- **File**: `backend/services/SearchService/Services/SearchIndexService.cs`
- **Purpose**: Background service consuming RabbitMQ events

**Key Features**:
- Automatic index initialization
- Event consumer pattern (async)
- Handles ProductCreated/Updated/Deleted/BulkImport events
- Index aliasing for zero-downtime updates
- Error handling with dead letter queue
- Automatic acknowledgment of processed events

**Event Processing Flow**:
1. Service starts and initializes index
2. Connects to RabbitMQ with automatic recovery
3. Listens on product-events exchange
4. Processes events based on routing key
5. Updates Elasticsearch index in real-time
6. Acknowledges message upon success

#### 5. Product Search API (✅ Complete)
- **File**: `backend/services/SearchService/Controllers/ProductSearchController.cs`
- **Endpoints**:
  - `POST /api/catalog/products/search` - Advanced search with filters
  - `GET /api/catalog/products/suggestions` - Autocomplete suggestions
  - `GET /api/catalog/products/{id}` - Get product details

**Key Features**:
- Multi-field full-text search
- Price range filtering
- Category, tag, brand filtering
- Color and size filtering
- Fuzzy matching
- Sorting (relevance, price, newest, popular)
- Redis caching (5-minute TTL)
- Performance monitoring
- Error handling

**Search Capabilities**:
```
Query: "blue leather jacket"
Filters: 
  - Category: "Clothing"
  - MinPrice: 50
  - MaxPrice: 200
  - Colors: ["blue"]
Sorting: "price_asc"
Result: 18 products matching criteria
```

#### 6. Event Publishing Service (✅ Complete)
- **File**: `backend/services/CatalogService/Services/RabbitMqEventPublisher.cs`
- **Purpose**: Publish domain events to RabbitMQ from Catalog Service

**Key Features**:
- Connection pooling and recovery
- Automatic retry with exponential backoff (3 attempts)
- Persistent message delivery
- Event metadata headers
- Message tracing support
- Comprehensive logging

**Integration Point**:
```csharp
// In CatalogService API endpoints
var @event = new ProductCreatedEvent(...);
await eventPublisher.PublishProductCreatedAsync(@event);
```

#### 7. Dependency Injection Configuration (✅ Complete)
- **File**: `backend/services/SearchService/Configuration/SearchServiceExtensions.cs`
- **Configurations**:
  - Elasticsearch client setup
  - RabbitMQ connection factory
  - Redis cache integration
  - Background service registration

**Configuration Methods**:
```csharp
// In Program.cs
services.AddSearchServices(configuration);
```

#### 8. Application Settings (✅ Complete)
- **File**: `backend/services/SearchService/appsettings.json`
- **Includes**:
  - Elasticsearch nodes and authentication
  - RabbitMQ broker settings
  - Redis connection configuration
  - Cache duration settings
  - Logging configuration
  - Performance targets

#### 9. Base Domain Event Class (✅ Complete)
- **File**: `backend/shared/types/DomainEvent.cs`
- **Purpose**: Abstract base for all domain events
- **Properties**:
  - EventId, Timestamp, EventType
  - AggregateId, AggregateType, Version
  - Metadata dictionary

---

## Integration Steps

### Step 1: Update Catalog Service Program.cs

Add RabbitMQ event publisher:

```csharp
var connectionFactory = new ConnectionFactory
{
    HostName = builder.Configuration["RabbitMQ:HostName"] ?? "localhost",
    Port = int.Parse(builder.Configuration["RabbitMQ:Port"] ?? "5672")
};

builder.Services.AddSingleton<IConnectionFactory>(connectionFactory);
builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
```

### Step 2: Update Product API Endpoints

In CatalogService product controller:

```csharp
[Inject]
private readonly IEventPublisher _eventPublisher;

[HttpPost("products")]
public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
{
    var product = new Product { /* ... */ };
    await _dbContext.Products.AddAsync(product);
    await _dbContext.SaveChangesAsync();

    // Publish event
    var @event = new ProductCreatedEvent(
        product.Id,
        product.Sku,
        product.Name,
        // ... other fields ...
        product.TenantId);
    
    await _eventPublisher.PublishProductCreatedAsync(@event);

    return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
}
```

### Step 3: Update Search Service Program.cs

```csharp
builder.Services.AddSearchServices(builder.Configuration);

var app = builder.Build();

// Start hosted services (SearchIndexService)
await app.RunAsync();
```

### Step 4: Update API Gateway

Add routes to Search Service:

```yaml
routes:
  - path: /api/catalog/products/search
    method: POST
    service: search-service
    
  - path: /api/catalog/products/suggestions
    method: GET
    service: search-service
```

### Step 5: Configure Aspire AppHost

```csharp
var elasticsearch = builder.AddElasticsearch("elasticsearch")
    .WithHealthCheck();

var rabbitmq = builder.AddRabbitMQ("rabbitmq");

var redis = builder.AddRedis("redis");

var searchService = builder.AddProject<Projects.SearchService>("search-service")
    .WithReference(elasticsearch)
    .WithReference(rabbitmq)
    .WithReference(redis);

var catalogService = builder.AddProject<Projects.CatalogService>("catalog-service")
    .WithReference(rabbitmq);
```

---

## Data Flow

### Creating a Product (Admin)

```
1. Admin submits product form
2. POST /api/admin/products (Catalog Service)
3. Product saved to PostgreSQL
4. ProductCreatedEvent published to RabbitMQ
5. Search Index Service receives event
6. ProductIndexDocument created
7. Elasticsearch index updated
8. Result cached in Redis
9. StoreFront search now includes product

Total Latency: ~2-3 seconds
```

### Updating a Product (Admin)

```
1. Admin modifies product details
2. PUT /api/admin/products/{id} (Catalog Service)
3. Product updated in PostgreSQL
4. ProductUpdatedEvent published
5. Search Index Service receives event
6. Elasticsearch document partially updated
7. Cache invalidated
8. StoreFront reflects changes

Total Latency: ~1-2 seconds
```

### Searching Products (StoreFront)

```
1. Customer enters search query
2. POST /api/catalog/products/search
3. Cache checked (5-minute TTL)
4. If miss: Elasticsearch query executed
5. Results scored by relevance
6. Filters applied (category, price, etc.)
7. Results cached
8. Response returned with ~100ms latency
```

---

## Testing

### Unit Tests

Create tests in `Tests/SearchService.Tests/`:

```csharp
[TestClass]
public class SearchIndexServiceTests
{
    [TestMethod]
    public async Task ProductCreatedEvent_IndexesProduct()
    {
        // Arrange
        var @event = new ProductCreatedEvent(...);
        
        // Act
        await _service.HandleProductCreatedAsync(@event);
        
        // Assert
        var result = await _elasticsearchClient.GetAsync(...);
        Assert.IsTrue(result.Found);
    }
}
```

### Integration Tests

```csharp
[TestClass]
public class ProductSearchIntegrationTests
{
    [TestMethod]
    public async Task Search_ReturnsCorrectResults()
    {
        // Create test index and products
        // Execute search query
        // Verify results match criteria
    }
}
```

### E2E Tests

Using Playwright:

```typescript
test('Product search workflow', async ({ page }) => {
  // 1. Navigate to StoreFront
  await page.goto('/products');
  
  // 2. Search for product
  await page.fill('[data-testid="search-input"]', 'blue jacket');
  await page.click('[data-testid="search-button"]');
  
  // 3. Verify results appear
  const results = await page.locator('[data-testid="product-card"]');
  expect(results).toHaveCount(3);
  
  // 4. Apply filter
  await page.click('[data-testid="filter-category-clothing"]');
  
  // 5. Verify filtered results
  const filtered = await page.locator('[data-testid="product-card"]');
  expect(filtered).toHaveCount(2);
});
```

---

## Monitoring & Observability

### Prometheus Metrics

Add to SearchIndexService:

```csharp
private static readonly Counter EventsProcessed = Counter
    .Create("search_events_processed_total", "Total events processed")
    .LabelNames("event_type", "status")
    .Register();

private static readonly Histogram IndexLatency = Histogram
    .Create("search_index_latency_seconds", "Time to index documents")
    .LabelNames("operation")
    .Register();
```

### Health Checks

```csharp
services.AddHealthChecks()
    .AddElasticsearch(...)
    .AddRabbitMQ(...)
    .AddRedis(...);

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = WriteHealthCheckResponse
});
```

### Logging

Key log levels:
- `DEBUG`: Elasticsearch query details, RabbitMQ messages
- `INFO`: Events published/processed, service startup
- `WARNING`: Retry attempts, cache misses
- `ERROR`: Indexing failures, connection errors

---

## Troubleshooting

### High Index Latency

**Symptom**: Products appear in search after 5+ seconds

**Solutions**:
1. Increase number of shards in Elasticsearch
2. Enable write buffer batching
3. Reduce index refresh interval if appropriate

### Missing Products in Search

**Symptom**: New products not appearing in search results

**Solutions**:
1. Check RabbitMQ queue length
2. Verify Search Index Service is running
3. Check Elasticsearch index health
4. Review logs for indexing errors

### Search Performance Issues

**Symptom**: Search queries take >500ms

**Solutions**:
1. Check cache hit rate (should be >80%)
2. Add more Elasticsearch replicas for read scaling
3. Index larger result sets with pagination
4. Optimize search queries (reduce fuzzy matching)

---

## Performance Targets

| Operation | Target Latency | Actual |
|-----------|-----------------|--------|
| Product Index | 1-2 seconds | - |
| Simple Search | 50-100ms | - |
| Complex Search | 100-200ms | - |
| Autocomplete | 50ms | - |
| Cache Hit | <10ms | - |

---

## Next Steps

1. **Create Integration Tests**
   - Test event publishing
   - Test index updates
   - Test search queries

2. **Add Monitoring Dashboard**
   - Grafana dashboard for metrics
   - Real-time index health
   - Query performance tracking

3. **Implement Advanced Features**
   - Faceted navigation
   - Synonym management
   - Personalized search
   - Search analytics

4. **Performance Optimization**
   - Query optimization
   - Cache warming
   - Index optimization
   - Query result pre-filtering

---

## Files Created

```
backend/
├── services/
│   ├── CatalogService/
│   │   ├── Events/
│   │   │   └── ProductCreatedEvent.cs
│   │   └── Services/
│   │       └── RabbitMqEventPublisher.cs
│   ├── SearchService/
│   │   ├── Config/
│   │   │   └── products-index-mapping.json
│   │   ├── Controllers/
│   │   │   └── ProductSearchController.cs
│   │   ├── Models/
│   │   │   └── ProductIndexDocument.cs
│   │   ├── Services/
│   │   │   └── SearchIndexService.cs
│   │   ├── Configuration/
│   │   │   └── SearchServiceExtensions.cs
│   │   └── appsettings.json
├── shared/
│   └── types/
│       └── DomainEvent.cs
```

---

## References

- [Elasticsearch.Net Documentation](https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/index.html)
- [RabbitMQ .NET Client](https://www.rabbitmq.com/dotnet-api-guide.html)
- [StackExchange.Redis](https://stackexchange.github.io/StackExchange.Redis/)
- [ASP.NET Core Background Services](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.backgroundservice)
