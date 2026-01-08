---
docid: KB-144
title: SCALABILITY_GUIDE
owner: @DocMaintainer
status: Active
created: 2026-01-08
---

﻿# Scalability & Performance Guide

**Last Updated:** December 30, 2025  
**Maintained By:** @Architect, @TechLead, @DevOps  
**Knowledge Base:** Operations & Architecture

---

## Overview

This guide covers scaling B2X from current capacity (1K-5K req/sec) to production-grade systems (10K+ req/sec) and preparing for growth.

---

## Current Capacity Analysis

### Baseline Metrics

| Metric | Current | With Optimization | Production Goal |
|--------|---------|-------------------|-----------------|
| **Requests/sec** | 1,000-5,000 | 5,000-10,000 | 10,000-50,000 |
| **Concurrent Users** | 10K-50K | 50K-100K | 100K+ |
| **Database Connections** | 100 | 200 | 500+ |
| **Memory per Instance** | 512 MB | 1 GB | 2 GB |
| **Latency (p99)** | 500ms | 200ms | <100ms |
| **Cache Hit Rate** | 60% | 80% | >85% |

### Bottleneck Analysis

| Component | Bottleneck | Impact | Mitigation |
|-----------|-----------|--------|-----------|
| **Database** | Connection pool | Medium | Increase pool size, read replicas |
| **API Gateway** | CPU on routing | Low | Add more instances |
| **Search (Elasticsearch)** | Index size | Medium | Sharding, time-based indices |
| **Cache (Redis)** | Memory usage | Medium | Eviction policy, persistence |
| **Message Queue (RabbitMQ)** | Queue depth | Low | Consumer scaling |

---

## Horizontal Scaling

### Stateless Service Design ✅ (Already Implemented)

```
Load Balancer
├── Instance 1 (Catalog Service)
├── Instance 2 (Catalog Service)
├── Instance 3 (Catalog Service)
└── Instance 4 (Catalog Service)
     │
     ▼
Shared Database (PostgreSQL)
Shared Cache (Redis)
Shared Queue (RabbitMQ)
```

**Current Setup:**
- All services are stateless (no session storage)
- Can spin up/down instances freely
- Load balanced across instances

**Scaling Steps:**

```yaml
# Kubernetes deployment
apiVersion: apps/v1
kind: Deployment
metadata:
  name: catalog-service
spec:
  replicas: 3  # Start with 3, scale to 10
  selector:
    matchLabels:
      app: catalog
  template:
    metadata:
      labels:
        app: catalog
    spec:
      containers:
      - name: catalog
        image: B2X/catalog:latest
        resources:
          requests:
            memory: "512Mi"
            cpu: "250m"
          limits:
            memory: "1Gi"
            cpu: "1000m"
        livenessProbe:
          httpGet:
            path: /health/live
            port: 5000
          initialDelaySeconds: 10
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 5000
          initialDelaySeconds: 5
          periodSeconds: 5
```

**Auto-Scaling:**

```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: catalog-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: catalog-service
  minReplicas: 3
  maxReplicas: 20
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
```

---

## Caching Strategy

### Current Caching (Redis)

```csharp
// Catalog Service - Product caching
public class CachedProductRepository : IProductRepository
{
    private readonly IRepository<Product> _db;
    private readonly IDistributedCache _cache;
    
    public async Task<Product> GetByIdAsync(int id)
    {
        var cacheKey = $"product:{id}";
        var cached = await _cache.GetStringAsync(cacheKey);
        
        if (cached != null)
            return JsonConvert.DeserializeObject<Product>(cached);
        
        var product = await _db.GetByIdAsync(id);
        
        // Cache for 1 hour
        await _cache.SetStringAsync(
            cacheKey,
            JsonConvert.SerializeObject(product),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            });
        
        return product;
    }
}
```

### Cache Strategies

**Strategy 1: Write-Through (Current)**
```
Request
  │
  ├─ Check Cache (Redis)
  │  └─ Hit: Return cached data
  │  └─ Miss: Query database
  │      ├─ Get data from DB
  │      └─ Write to cache
  └─ Return data
```

**Strategy 2: Write-Behind (For High-Traffic)**
```
Write Request
  │
  ├─ Write to Cache (immediate)
  ├─ Queue write to DB (async)
  └─ Return to client (fast!)
     │
     ▼ (background)
   Write to Database
```

**Strategy 3: Cache Invalidation (Event-Driven)**
```csharp
// When product updated, invalidate cache
public class ProductUpdatedHandler
{
    private readonly IDistributedCache _cache;
    
    [WolverineHandler]
    public async Task Handle(ProductUpdatedEvent evt)
    {
        // Invalidate cache entry
        await _cache.RemoveAsync($"product:{evt.ProductId}");
        
        // Also invalidate category cache
        await _cache.RemoveAsync($"category:{evt.CategoryId}:products");
    }
}
```

### Redis Configuration for Production

```csharp
// Program.cs
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration.GetConnectionString("Redis");
    options.InstanceName = "B2X:";
});

services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var multiplexer = ConnectionMultiplexer.Connect(
        configuration.GetConnectionString("Redis"));
    
    // Enable offline mode for resilience
    multiplexer.InternalError += (connection, error) =>
    {
        _logger.LogError($"Redis error: {error.Message}");
    };
    
    return multiplexer;
});
```

**Cluster Setup (For 50K+ req/sec):**

```
Redis Cluster
├── Master 1 (Slots 0-5460)
├── Master 2 (Slots 5461-10922)
├── Master 3 (Slots 10923-16383)
├── Replica 1 (copies Master 1)
├── Replica 2 (copies Master 2)
└── Replica 3 (copies Master 3)
```

---

## Database Optimization

### Query Performance

```csharp
// BAD: N+1 query problem
public async Task<List<ProductDto>> GetProductsAsync()
{
    var products = await _context.Products.ToListAsync();
    
    foreach (var product in products)
    {
        product.Category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == product.CategoryId);
        // Query per product! 🔴
    }
}

// GOOD: Eager loading
public async Task<List<ProductDto>> GetProductsAsync()
{
    var products = await _context.Products
        .Include(p => p.Category)
        .Include(p => p.Supplier)
        .ToListAsync();
    // Single query! ✅
}
```

### Database Connection Pooling

```csharp
// Optimize connection pool size
public void ConfigureServices(IServiceCollection services)
{
    var connString = configuration.GetConnectionString("DefaultConnection");
    
    services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(connString, npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetialDelay: TimeSpan.FromSeconds(30),
                errorCodesToAdd: null);
                
            // Connection pool config
            npgsqlOptions.UseQueryCancellationToken();
        });
    });
}

// Connection pool sizing:
// Pool Size = (Core Count × 2) + Effective Spindle Count
// For 8-core machine: Pool Size = 16-25
// Default .NET: 100 (can be reduced)
```

### Read Replicas for Reporting

```csharp
// Create read-only context
services.AddDbContext<ProductCatalogReadContext>(options =>
{
    options.UseNpgsql(
        configuration.GetConnectionString("ProductCatalogRead")); // Read replica
});

// Use in reporting
public class ProductReportService
{
    private readonly ProductCatalogReadContext _readDb;
    
    public async Task<List<ProductReport>> GenerateReportAsync()
    {
        // Query off read replica
        return await _readDb.ProductReports
            .FromSqlRaw("SELECT * FROM mv_product_reports")
            .ToListAsync();
    }
}
```

### Index Strategy

```sql
-- Essential indexes for catalog
CREATE INDEX idx_products_category ON products(category_id);
CREATE INDEX idx_products_sku ON products(sku) WHERE active = true;
CREATE INDEX idx_products_created ON products(created_at DESC);

-- For search
CREATE INDEX idx_products_name_trgm ON products 
    USING GIN (name gin_trgm_ops); -- Text search

-- For inventory
CREATE INDEX idx_inventory_product_warehouse ON inventory(product_id, warehouse_id);

-- Monitor slow queries
SELECT query, calls, total_time, mean_time 
FROM pg_stat_statements 
WHERE mean_time > 100 
ORDER BY total_time DESC;
```

---

## Search Optimization

### Elasticsearch Configuration

```csharp
// Elasticsearch setup for product search
public class ElasticsearchConfiguration
{
    public static void AddElasticsearch(IServiceCollection services, 
        IConfiguration configuration)
    {
        var url = configuration["Elasticsearch:Url"];
        var settings = new ConnectionSettings(new Uri(url));
        
        settings
            .DefaultMappingFor<ProductDocument>(m => m
                .IndexName("products")
                .TypeName("_doc"))
            .BasicAuthentication("elastic", configuration["Elasticsearch:Password"])
            .EnableDebugMode()
            .OnRequestCompleted(details =>
            {
                // Log all requests
                Console.WriteLine($"ES: {details.HttpMethod} {details.Uri}");
            });
        
        var client = new ElasticClient(settings);
        services.AddSingleton<IElasticClient>(client);
    }
}
```

### Sharding Strategy

```
Product Index
├── products-2025.01 (January products)
├── products-2025.02 (February products)
├── products-2025.03 (March products)
└── products (alias for current)
```

**Index Lifecycle:**

```csharp
// Create new index each month
public class ElasticsearchIndexRotationService
{
    private readonly IElasticClient _elastic;
    
    public async Task RotateIndicesAsync()
    {
        var newIndexName = $"products-{DateTime.UtcNow:yyyy.MM}";
        
        // Create new index with updated mappings
        await _elastic.Indices.CreateAsync(newIndexName, c => c
            .Settings(s => s
                .NumberOfShards(5)
                .NumberOfReplicas(2)
                .RefreshInterval("30s"))
            .Map<ProductDocument>(m => m
                .Properties(p => p
                    .Text(t => t.Name(d => d.Name))
                    .Keyword(k => k.Name(d => d.Sku))
                    .Number(n => n.Name(d => d.Price)))));
        
        // Reindex old index to new
        var reindex = await _elastic.ReindexOnServerAsync(r => r
            .Source(s => s.Index("products"))
            .Destination(d => d.Index(newIndexName)));
        
        // Update alias
        await _elastic.Indices.BulkAliasAsync(b => b
            .Remove(a => a.Alias("products").Index("products-*"))
            .Add(a => a.Alias("products").Index(newIndexName)));
    }
}
```

---

## Performance Monitoring

### Key Metrics Dashboard

```
Application Performance
├── Requests/sec: 2,450 (▲)
├── Error Rate: 0.02% (▼)
├── Avg Latency: 145ms (↔)
├── p99 Latency: 485ms (▼)
└── Cache Hit Rate: 78% (▲)

Database
├── Connection Pool: 25/100 (25% used)
├── Slow Queries: 3 (>100ms)
├── Deadlocks: 0
└── Disk I/O: 45% utilized

Cache
├── Hit Rate: 78%
├── Evictions: 12/hour
├── Memory: 2.1GB / 4GB
└── Operations/sec: 15,230

Search
├── Query Latency: p50=25ms, p99=145ms
├── Index Size: 8.2GB
├── Document Count: 2,450,000
└── Query/sec: 890
```

### Monitoring Code

```csharp
// Application Insights metrics
public class ProductServiceMetrics
{
    private readonly ILogger<ProductServiceMetrics> _logger;
    private readonly TelemetryClient _telemetry;
    
    public async Task<Product> GetProductAsync(int id)
    {
        using var operation = _telemetry.StartOperation<RequestTelemetry>("GetProduct");
        var sw = Stopwatch.StartNew();
        
        try
        {
            var product = await _repository.GetByIdAsync(id);
            
            _telemetry.TrackEvent("ProductRetrieved", 
                new Dictionary<string, string>
                {
                    {"ProductId", id.ToString()},
                    {"CacheHit", "true"}
                },
                new Dictionary<string, double>
                {
                    {"Duration", sw.ElapsedMilliseconds}
                });
            
            return product;
        }
        catch (Exception ex)
        {
            _telemetry.TrackException(ex);
            throw;
        }
    }
}
```

---

## Load Testing

### Test Plan

```
Load Profile:
├── Ramp-up: 0 → 5,000 req/sec over 5 minutes
├── Plateau: 5,000 req/sec for 30 minutes
├── Spike: 10,000 req/sec for 5 minutes
└── Cool-down: Gradual to 0

Target Metrics:
├── Error rate: < 0.1%
├── p99 latency: < 500ms
├── Database: < 80% CPU
└── Cache: > 75% hit rate
```

### K6 Load Test Script

```javascript
// k6 load test
import http from 'k6/http';
import { check } from 'k6';

export let options = {
  vus: 100,  // Virtual users
  duration: '30m',
  rampUpDuration: '5m',
  rampDownDuration: '5m',
  thresholds: {
    http_req_duration: ['p(95)<500', 'p(99)<1000'],
    http_req_failed: ['rate<0.01'],
  },
};

export default function() {
  // Random product ID (1-100000)
  const productId = Math.floor(Math.random() * 100000) + 1;
  
  let res = http.get(`http://api/v1/products/${productId}`);
  
  check(res, {
    'status is 200': (r) => r.status === 200,
    'response time < 500ms': (r) => r.timings.duration < 500,
  });
}
```

---

## Scaling Roadmap

### Phase 1: Baseline (Now)
- ✅ 1K-5K req/sec
- ✅ Single database instance
- ✅ Basic caching
- Timeline: Current

### Phase 2: Scale to 10K req/sec
- [ ] Database read replicas
- [ ] Elasticsearch clustering
- [ ] Redis cluster
- [ ] Horizontal scaling (5+ instances)
- Timeline: Month 3-6

### Phase 3: Scale to 50K+ req/sec
- [ ] Database sharding
- [ ] Multi-region deployment
- [ ] CDN for static assets
- [ ] Advanced caching (2-tier)
- Timeline: Month 6-12

---

## References

- [PostgreSQL Performance Tuning](https://wiki.postgresql.org/wiki/Performance_Optimization)
- [Redis Best Practices](https://redis.io/topics/cluster-tutorial)
- [Elasticsearch Scaling](https://www.elastic.co/guide/en/elasticsearch/reference/current/scalability.html)
- [K6 Load Testing](https://k6.io/docs/)

---

**Last Updated:** December 30, 2025  
**Review Date:** June 30, 2026  
**Maintained By:** @Architect, @TechLead, @DevOps
