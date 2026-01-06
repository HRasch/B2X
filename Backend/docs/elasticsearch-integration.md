# Elasticsearch Integration für B2Connect Shop Platform

**Version**: 1.0  
**Status**: ✅ Complete  
**Last Updated**: 25. Dezember 2025

---

## Executive Summary

Elasticsearch ist das Herzstück der Such- und Katalog-Funktionalität in B2Connect. Das System ermöglicht:
- ⚡ **Sub-Millisekunden-Suche** über Millionen von Produkten
- 🔄 **Echtzeit-Index-Updates** beim Hinzufügen/Ändern von Produkten im Admin
- 🎯 **Facettierte Suche** mit komplexen Filtern
- 💡 **Autocomplete & Suggestions** für bessere UX
- 📊 **Aggregationen** für Analytics und Reporting

---

## Architecture Overview

### System-Komponenten

```
┌─────────────────────────────────────────────────────────────┐
│                     Admin Frontend                            │
│            (Produkt hinzufügen/bearbeiten)                   │
└───────────────────────┬─────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────────┐
│                  Admin API Gateway                            │
│              (Authentication & Validation)                    │
└───────────────────────┬─────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────────┐
│              Catalog Service (Port 5006)                     │
│       (PostgreSQL CRUD + Domain Events)                       │
└───────────┬──────────────────────────────────┬───────────────┘
            │                                  │
            ▼                                  ▼
    ┌───────────────┐              ┌────────────────────┐
    │ PostgreSQL DB │              │  Event Publisher   │
    └───────────────┘              │    (RabbitMQ)      │
                                   └────────┬───────────┘
                                           │
                                           ▼
                                   ┌───────────────────┐
                                   │  RabbitMQ Topics  │
                                   │ product.created   │
                                   │ product.updated   │
                                   │ product.deleted   │
                                   └────────┬──────────┘
                                           │
                                           ▼
                            ┌──────────────────────────────┐
                            │  Search Index Service        │
                            │  (Event Consumers)           │
                            └──────────┬───────────────────┘
                                       │
                                       ▼
                            ┌──────────────────────────────┐
                            │   Elasticsearch Cluster      │
                            │  (b2connect-products Index)  │
                            └──────────┬───────────────────┘
                                       │
                                       ▼
                            ┌──────────────────────────────┐
                            │   StoreFront & APIs          │
                            │  (Search & Filtering)        │
                            └──────────────────────────────┘
```

---

## Elasticsearch Configuration

### Cluster-Konfiguration

**Production Setup:**
```yaml
# elasticsearch.yml
cluster.name: b2connect-production
node.name: node-1
node.roles: [master, data, ingest]

# Networking
network.host: 0.0.0.0
http.port: 9200
transport.port: 9300

# Performance
thread_pool.search.queue_size: 10000
thread_pool.write.queue_size: 5000

# Security
xpack.security.enabled: true
xpack.security.authc:
  - type: native
  - type: ldap

# Monitoring
xpack.monitoring.enabled: true
xpack.watcher.enabled: true
```

### Index-Konfiguration

**Settings:**
```json
{
  "settings": {
    "number_of_shards": 3,
    "number_of_replicas": 2,
    "refresh_interval": "5s",
    "max_result_window": 50000,
    "analysis": {
      "analyzer": {
        "default": {
          "type": "standard"
        },
        "autocomplete": {
          "type": "custom",
          "tokenizer": "standard",
          "filter": ["lowercase"]
        }
      }
    }
  }
}
```

---

## Product Indexing Process

### 1. Admin erstellt/bearbeitet Produkt

**Workflow:**
```
Admin Action (Create/Update/Delete)
    ↓
Admin API validates Input
    ↓
Catalog Service speichert in PostgreSQL
    ↓
Domain Event wird erzeugt
    ↓
Event wird zu RabbitMQ publiziert
    ↓
Search Index Service konsumiert Event
    ↓
Elasticsearch wird aktualisiert
    ↓
Redis Cache wird invalidiert
    ↓
StoreFront zeigt aktualisierte Daten
```

### 2. Event Publishing (Domain-Driven Design)

**ProductCreated Event:**
```csharp
public class ProductCreatedEvent : DomainEvent
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public string Sku { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    
    public override void Publish(IMessageBus messageBus)
    {
        messageBus.PublishAsync(
            routingKey: "product.created",
            message: this
        );
    }
}
```

**Publishing Code:**
```csharp
public async Task<CreateProductResponse> CreateProductAsync(
    CreateProductRequest request, 
    Guid tenantId)
{
    // 1. Validierung
    ValidateProductData(request);
    
    // 2. Speichere in DB
    var product = new Product
    {
        Id = Guid.NewGuid(),
        Name = request.Name,
        Sku = request.Sku,
        Price = request.Price,
        Category = request.Category,
        TenantId = tenantId,
        CreatedAt = DateTime.UtcNow
    };
    
    await _productRepository.AddAsync(product);
    await _unitOfWork.SaveChangesAsync();
    
    // 3. Publishe Event
    var productCreatedEvent = new ProductCreatedEvent
    {
        ProductId = product.Id,
        Name = product.Name,
        Sku = product.Sku,
        Price = product.Price,
        Category = product.Category,
        AggregateId = product.Id,
        Timestamp = DateTime.UtcNow,
        Version = 1
    };
    
    await _eventBus.PublishAsync(productCreatedEvent);
    
    return new CreateProductResponse { ProductId = product.Id };
}
```

### 3. Search Index Service (Event Consumer)

**Listener Implementation:**
```csharp
[RabbitMQListener(queue: "search-index.product-created")]
public class ProductCreatedEventHandler
{
    private readonly IElasticsearchClient _elasticsearch;
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductCreatedEventHandler> _logger;

    public async Task HandleAsync(ProductCreatedEvent @event)
    {
        try
        {
            _logger.LogInformation(
                "Processing product created event: {ProductId}", 
                @event.ProductId);

            // 1. Hole vollständige Produktdaten
            var product = await _repository.GetByIdAsync(
                @event.ProductId, 
                includeDependencies: true);

            if (product == null)
            {
                _logger.LogWarning("Product not found: {ProductId}", 
                    @event.ProductId);
                return;
            }

            // 2. Mapped zu Index-Format
            var indexDocument = MapToIndexDocument(product);

            // 3. Indexiere in Elasticsearch
            var response = await _elasticsearch.IndexAsync(
                new IndexRequest<ProductIndexDocument>
                {
                    Index = "b2connect-products",
                    Id = product.Id.ToString(),
                    Document = indexDocument,
                    Refresh = Refresh.True  // Sofort verfügbar
                });

            if (!response.IsValid)
            {
                throw new Exception(
                    $"Failed to index product: {response.ServerError}");
            }

            _logger.LogInformation(
                "Product indexed successfully: {ProductId}", 
                product.Id);

            // 4. Invalidiere Cache
            await _cacheService.RemoveAsync(
                $"product:{product.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error handling product created event");
            
            // Sende zu Dead Letter Queue für Retry
            throw;
        }
    }

    private ProductIndexDocument MapToIndexDocument(Product product)
    {
        return new ProductIndexDocument
        {
            Id = product.Id,
            Sku = product.Sku,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            CategoryName = GetCategoryName(product.CategoryId),
            Price = product.Price,
            B2bPrice = product.B2bPrice,
            Stock = product.StockLevel,
            InStock = product.StockLevel > 0,
            Rating = product.AverageRating,
            Reviews = product.ReviewCount,
            Attributes = product.Attributes,
            Images = product.ImageUrls,
            Tags = product.Tags?.ToList() ?? new List<string>(),
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            TenantId = product.TenantId.ToString(),
            Suggestions = GenerateSuggestions(product)
        };
    }
}
```

---

## StoreFront Search Implementation

### Search API Endpoints

#### 1. Full Text Search
```
GET /catalog/products/search?q=wireless&pageSize=20

Request Handler:
```csharp
[HttpGet("search")]
public async Task<IActionResult> SearchProducts(
    [FromQuery] string q,
    [FromQuery] int pageSize = 10,
    [FromQuery] int pageNumber = 1)
{
    var searchRequest = new SearchRequest("b2connect-products")
    {
        Query = new MultiMatchQuery
        {
            Query = q,
            Fields = new[] { "name^3", "description^1.5", "tags" },
            Operator = Operator.Or,
            Fuzziness = Fuzziness.Auto
        },
        Size = pageSize,
        From = (pageNumber - 1) * pageSize,
        Sort = new List<ISort> 
        {
            new FieldSort { Field = "_score", Order = SortOrder.Descending }
        }
    };

    var response = await _elasticsearchClient.SearchAsync(
        searchRequest);

    return Ok(MapSearchResponse(response));
}
```

**Response Format:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "prod-123",
        "name": "Wireless Headphones",
        "price": 99.99,
        "rating": 4.5,
        "thumbnail": "img.jpg"
      }
    ],
    "totalCount": 45,
    "pageSize": 20,
    "pageNumber": 1,
    "facets": {
      "brands": [
        { "name": "TechBrand", "count": 12 }
      ]
    }
  }
}
```

#### 2. Advanced Filtering
```
GET /catalog/products/search?
  q=headphones&
  category=Electronics&
  minPrice=50&
  maxPrice=150&
  brand=TechBrand&
  inStock=true&
  sortBy=price

Query:
```json
{
  "query": {
    "bool": {
      "must": [
        { "multi_match": { "query": "headphones", "fields": ["name^3"] } }
      ],
      "filter": [
        { "term": { "category": "Electronics" } },
        { "range": { "price": { "gte": 50, "lte": 150 } } },
        { "term": { "attributes.brand": "TechBrand" } },
        { "term": { "inStock": true } }
      ]
    }
  },
  "sort": [{ "price": "asc" }],
  "aggs": {
    "brands": { "terms": { "field": "attributes.brand" } },
    "colors": { "terms": { "field": "attributes.color" } }
  }
}
```

#### 3. Autocomplete
```
GET /catalog/products/suggestions?q=wire

Query:
```json
{
  "query": {
    "match_phrase_prefix": {
      "suggestions": {
        "query": "wire"
      }
    }
  },
  "size": 5
}
```

---

## Performance Optimization

### Caching Strategy

**Multi-Layer Cache:**
```
┌─────────────────────┐
│   Browser Cache     │ (30 min)
└──────────┬──────────┘
           │
┌──────────▼──────────┐
│   Redis Cache       │ (5 min)
└──────────┬──────────┘
           │
┌──────────▼──────────┐
│   Elasticsearch     │ (Real-time)
└─────────────────────┘
```

**Cache Implementation:**
```csharp
public class CachedSearchService : ISearchService
{
    private readonly ISearchService _innerService;
    private readonly IDistributedCache _cache;

    public async Task<SearchResponse> SearchAsync(
        SearchQuery query)
    {
        var cacheKey = GenerateCacheKey(query);
        
        // Versuche aus Cache zu laden
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<SearchResponse>(cached);
        }

        // Nicht im Cache → Elasticsearch abfragen
        var result = await _innerService.SearchAsync(query);

        // Speichere in Cache (5 Minuten)
        await _cache.SetStringAsync(
            cacheKey, 
            JsonSerializer.Serialize(result),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

        return result;
    }

    private string GenerateCacheKey(SearchQuery query)
    {
        return $"search:{query.TenantId}:{query.Query}:{query.Filters}";
    }
}
```

### Index Optimization

**Force Merge (nach Bulk-Operationen):**
```
POST /b2connect-products/_forcemerge?max_num_segments=1
```

**Segment Management:**
```csharp
public class IndexMaintenanceService
{
    public async Task OptimizeIndexAsync()
    {
        // Force merge nach bulk imports
        var response = await _elasticsearch.ForceMergeAsync(
            new ForceMergeRequest("b2connect-products")
            {
                MaxNumberOfSegments = 1
            });
    }
}
```

---

## Monitoring & Alerting

### Key Metrics

**Real-time Monitoring:**
```
Dashboard Metrics:
├─ Index Health: Green/Yellow/Red
├─ Document Count: Gesamt indexierte Produkte
├─ Index Size: MB/GB
├─ Query Latency: ms (p50, p95, p99)
├─ Indexing Rate: docs/sec
├─ Cache Hit Rate: %
└─ Error Rate: %
```

**Prometheus Metrics:**
```
elasticsearch_indices_docs_total{index="b2connect-products"}
elasticsearch_indices_store_size_bytes{index="b2connect-products"}
elasticsearch_search_query_time_seconds
elasticsearch_indexing_time_seconds
elasticsearch_search_request_cache_hit_count
```

### Alert Rules

**Critical:**
- Index Health = Red
- Search Error Rate > 1%
- Query Latency > 2 seconds

**Warning:**
- Query Latency > 1 second
- Indexing Lag > 30 seconds
- Failed Indexing Operations > 5

---

## Disaster Recovery

### Backup & Restore

**Daily Snapshots:**
```csharp
public class BackupService
{
    public async Task CreateDailySnapshotAsync()
    {
        var snapshotName = $"b2connect-products-{DateTime.UtcNow:yyyy-MM-dd}";
        
        await _elasticsearch.SnapshotAsync(
            new SnapshotRequest("backup-repository", snapshotName)
            {
                Indices = "b2connect-products",
                IncludeGlobalState = false
            });
    }

    public async Task RestoreFromSnapshotAsync(string snapshotName)
    {
        // Close index vor Restore
        await _elasticsearch.CloseIndexAsync("b2connect-products");
        
        // Restore
        await _elasticsearch.RestoreAsync(
            new RestoreRequest("backup-repository", snapshotName)
            {
                Indices = "b2connect-products"
            });
    }
}
```

### Index Rebuild

**Szenario: Index beschädigt oder out-of-sync**

```csharp
public class IndexRebuildService
{
    public async Task RebuildIndexAsync(string tenantId)
    {
        var newIndexName = $"b2connect-products-rebuild-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
        
        // 1. Erstelle neuen Index
        await _elasticsearch.CreateIndexAsync(newIndexName);
        
        // 2. Indexiere alle Produkte
        var products = await _repository.GetAllAsync(tenantId);
        var batch = products.Batch(100);
        
        foreach (var group in batch)
        {
            var bulkRequest = new BulkRequest("b2connect-products");
            foreach (var product in group)
            {
                bulkRequest.Operations.Add(
                    new BulkIndexOperation<ProductDocument>
                    {
                        Document = MapToIndexDocument(product),
                        Index = newIndexName
                    });
            }
            
            await _elasticsearch.BulkAsync(bulkRequest);
        }
        
        // 3. Verifiziere
        var oldCount = await CountDocuments("b2connect-products");
        var newCount = await CountDocuments(newIndexName);
        
        if (oldCount == newCount)
        {
            // 4. Switch Alias (Zero Downtime)
            await _elasticsearch.AliasAsync(a => a
                .Remove(r => r.Index("b2connect-products-v1").Alias("b2connect-products"))
                .Add(add => add.Index(newIndexName).Alias("b2connect-products")));
            
            // 5. Lösche alten Index
            await _elasticsearch.DeleteIndexAsync("b2connect-products-v1");
        }
    }
}
```

---

## Best Practices

### ✅ Do's
- ✅ Verwende Aliases für Zero-Downtime Updates
- ✅ Indexiere asynchron via Event Queue
- ✅ Implementiere Retry-Logik für fehlgeschlagene Events
- ✅ Monitore Query Performance kontinuierlich
- ✅ Nutze Bulk API für Batch-Operationen
- ✅ Implementiere Timeout-Schutz für lange Queries

### ❌ Don'ts
- ❌ Indexiere synchron im kritischen Pfad
- ❌ Verwende zu breite Analyzers
- ❌ Speichere große Binärdaten im Index
- ❌ Ignoriere Fehlerquoten
- ❌ Verwende unbegrenzte Queries

---

## Troubleshooting

### Problem: Produkt wird nach Update nicht sofort gefunden

**Ursache:** Index Refresh Interval zu lange

**Lösung:**
```json
PUT /b2connect-products/_settings
{
  "refresh_interval": "1s"  // Statt 5s
}
```

### Problem: Search queries sind langsam

**Debugging:**
```
GET /b2connect-products/_search?pretty
{
  "profile": true,
  "query": { ... }
}
```

**Optimierung:**
- Force merge statt zu viele Segments
- Erhöhe JVM Heap
- Optimiere Mapping (z.B. text statt keyword für freie Felder)

### Problem: Index ist beschädigt

**Recovery:**
```csharp
// Trigger Index Rebuild
await _indexRebuildService.RebuildIndexAsync(tenantId);
```

---

## Related Documentation

- [Shop Platform Specifications](./shop-platform-specs.md) - Übersicht aller Shop-Features
- [API Specifications](./api-specifications.md) - API Endpoint-Dokumentation
- [Architecture Guide](./architecture.md) - Gesamt-Systemarchitektur

