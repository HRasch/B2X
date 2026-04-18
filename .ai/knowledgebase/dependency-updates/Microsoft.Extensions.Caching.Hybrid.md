---
docid: KB-109
title: Microsoft.Extensions.Caching.Hybrid
owner: @Backend
status: Active
created: 2026-01-11
---

# Microsoft.Extensions.Caching.Hybrid

**Version:** 10.1.0  
**Package:** [Microsoft.Extensions.Caching.Hybrid](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Hybrid)  
**Documentation:** [HybridCache library in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/hybrid)

## Overview

Microsoft.Extensions.Caching.Hybrid provides a high-performance hybrid caching implementation that combines in-memory (L1) and distributed (L2) caching with advanced features like cache-stampede protection, configurable serialization, and tag-based invalidation. The HybridCache service simplifies caching patterns that previously required manual coordination between IMemoryCache and IDistributedCache.

Built on top of IDistributedCache, it works with all existing cache backends (Redis, SQL Server, CosmosDB, etc.) and provides performance enhancements through support for the newer IBufferDistributedCache API. The library is designed for high-throughput applications requiring efficient caching with minimal configuration overhead.

## Key Features

- **Hybrid Caching**: Combines fast in-memory L1 cache with distributed L2 cache
- **Cache-Stampede Protection**: Prevents multiple concurrent requests for the same data
- **Simple API**: GetOrCreateAsync pattern with automatic cache management
- **Configurable Serialization**: Support for custom serializers (JSON, protobuf, etc.)
- **Tag-Based Invalidation**: Group cache entries and invalidate by tags
- **Performance Optimizations**: Buffer-based distributed cache support, object reuse
- **Native AOT Support**: Compatible with ahead-of-time compilation
- **Thread Safety**: Automatic handling of concurrent access patterns

## Core Components

### HybridCache Service
- **Primary Interface**: Main API for hybrid caching operations
- **GetOrCreateAsync**: Core method for cache retrieval with factory fallback
- **SetAsync**: Direct cache storage without retrieval
- **RemoveAsync**: Key-based cache invalidation
- **RemoveByTagAsync**: Tag-based cache invalidation

### Cache Options
- **HybridCacheOptions**: Global configuration (payload limits, key length, defaults)
- **HybridCacheEntryOptions**: Per-entry configuration (expiration, tags)

### Serialization System
- **Type-Specific Serializers**: Custom serializers for specific types
- **Serializer Factories**: General-purpose serializers for multiple types
- **Default JSON Serialization**: System.Text.Json for most scenarios

## Basic Usage

### Service Registration

```csharp
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.StackExchangeRedis;

var builder = WebApplication.CreateBuilder(args);

// Register distributed cache (Redis example)
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Register hybrid cache with default options
builder.Services.AddHybridCache();

// Or with custom options
builder.Services.AddHybridCache(options =>
{
    options.MaximumPayloadBytes = 1024 * 1024; // 1MB limit
    options.MaximumKeyLength = 1024;
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(5),
        LocalCacheExpiration = TimeSpan.FromMinutes(1)
    };
});

var app = builder.Build();
```

### Basic GetOrCreateAsync Usage

```csharp
using Microsoft.Extensions.Caching.Hybrid;

public class ProductService
{
    private readonly HybridCache _cache;

    public ProductService(HybridCache cache)
    {
        _cache = cache;
    }

    public async Task<Product> GetProductAsync(string productId)
    {
        return await _cache.GetOrCreateAsync(
            $"product:{productId}", // Cache key
            async cancellationToken =>
            {
                // Factory method - called only on cache miss
                return await _database.GetProductAsync(productId);
            });
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        return await _cache.GetOrCreateAsync(
            $"products:category:{category}",
            async cancellationToken =>
            {
                return await _database.GetProductsByCategoryAsync(category);
            });
    }
}
```

### Advanced GetOrCreateAsync with Options

```csharp
public async Task<Product> GetProductWithOptionsAsync(string productId)
{
    var tags = new[] { "products", $"category:{productId.Split('-')[0]}" };
    var options = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromHours(1),
        LocalCacheExpiration = TimeSpan.FromMinutes(10)
    };

    return await _cache.GetOrCreateAsync(
        $"product:{productId}",
        async cancellationToken => await _database.GetProductAsync(productId),
        options,
        tags);
}
```

### Direct Cache Storage

```csharp
public async Task UpdateProductAsync(Product product)
{
    // Update database first
    await _database.UpdateProductAsync(product);

    // Update cache directly
    await _cache.SetAsync(
        $"product:{product.Id}",
        product,
        new HybridCacheEntryOptions { Expiration = TimeSpan.FromHours(1) },
        new[] { "products", $"category:{product.Category}" });
}
```

### Cache Invalidation

```csharp
public async Task DeleteProductAsync(string productId)
{
    // Remove from database
    await _database.DeleteProductAsync(productId);

    // Remove from cache
    await _cache.RemoveAsync($"product:{productId}");
}

public async Task ClearCategoryCacheAsync(string category)
{
    // Remove all products in a category using tag
    await _cache.RemoveByTagAsync($"category:{category}");
}

public async Task ClearAllProductCacheAsync()
{
    // Clear all product-related cache using wildcard tag
    await _cache.RemoveByTagAsync("products");
}
```

## B2X Integration

The B2X platform extensively uses Microsoft.Extensions.Caching.Hybrid for its multi-tenant e-commerce caching needs, providing efficient data access across distributed services while maintaining cache consistency and performance.

### Store API Product Caching

```csharp
// src/backend/Store/API/Services/CachedProductService.cs
using Microsoft.Extensions.Caching.Hybrid;

public class CachedProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly HybridCache _cache;
    private readonly ITenantContext _tenantContext;

    public CachedProductService(
        IProductRepository repository,
        HybridCache cache,
        ITenantContext tenantContext)
    {
        _repository = repository;
        _cache = cache;
        _tenantContext = tenantContext;
    }

    public async Task<Product> GetProductAsync(string productId)
    {
        var tenantId = _tenantContext.Tenant.Id;
        var cacheKey = $"tenant:{tenantId}:product:{productId}";

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async cancellationToken =>
            {
                var product = await _repository.GetProductAsync(productId);
                return product with { TenantId = tenantId }; // Ensure tenant isolation
            },
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(30),
                LocalCacheExpiration = TimeSpan.FromMinutes(5)
            },
            new[] { $"tenant:{tenantId}", "products" });
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string categoryId, int page = 1, int pageSize = 20)
    {
        var tenantId = _tenantContext.Tenant.Id;
        var cacheKey = $"tenant:{tenantId}:products:category:{categoryId}:page:{page}:size:{pageSize}";

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async cancellationToken =>
            {
                return await _repository.GetProductsByCategoryAsync(categoryId, page, pageSize);
            },
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(15),
                LocalCacheExpiration = TimeSpan.FromMinutes(2)
            },
            new[] { $"tenant:{tenantId}", $"category:{categoryId}", "product-lists" });
    }
}
```

### Catalog Service with Stampede Protection

```csharp
// src/backend/Store/Services/CatalogService.cs
using Microsoft.Extensions.Caching.Hybrid;

public class CatalogService
{
    private readonly ICatalogRepository _repository;
    private readonly HybridCache _cache;
    private readonly ILogger<CatalogService> _logger;

    public CatalogService(
        ICatalogRepository repository,
        HybridCache cache,
        ILogger<CatalogService> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Catalog> GetCatalogAsync(string tenantId, string catalogId)
    {
        var cacheKey = $"catalog:{tenantId}:{catalogId}";

        var catalog = await _cache.GetOrCreateAsync(
            cacheKey,
            async cancellationToken =>
            {
                _logger.LogInformation("Loading catalog {CatalogId} for tenant {TenantId} from database",
                    catalogId, tenantId);

                // This factory method is protected against stampede
                // Only one concurrent request will execute this code
                var catalogData = await _repository.GetCatalogAsync(catalogId);

                // Expensive computation - only happens once per cache miss
                var processedCatalog = await ProcessCatalogDataAsync(catalogData, cancellationToken);

                return processedCatalog;
            },
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromHours(2),
                LocalCacheExpiration = TimeSpan.FromMinutes(30)
            },
            new[] { $"tenant:{tenantId}", "catalogs" });

        return catalog;
    }

    private async Task<Catalog> ProcessCatalogDataAsync(CatalogData data, CancellationToken cancellationToken)
    {
        // Simulate expensive processing that should only happen once
        await Task.Delay(100, cancellationToken); // Database queries, calculations, etc.

        return new Catalog
        {
            Id = data.Id,
            Name = data.Name,
            Categories = data.Categories.Select(c => new Category
            {
                Id = c.Id,
                Name = c.Name,
                ProductCount = c.ProductIds.Count
            }).ToList()
        };
    }
}
```

### Multitenant Cache Management

```csharp
// src/backend/Shared/Infrastructure/Caching/MultitenantHybridCache.cs
using Microsoft.Extensions.Caching.Hybrid;

public class MultitenantHybridCache
{
    private readonly HybridCache _cache;
    private readonly ITenantContext _tenantContext;

    public MultitenantHybridCache(HybridCache cache, ITenantContext tenantContext)
    {
        _cache = cache;
        _tenantContext = tenantContext;
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        HybridCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default)
    {
        var tenantKey = GetTenantKey(key);
        var tenantTags = GetTenantTags(tags);

        return await _cache.GetOrCreateAsync(
            tenantKey,
            factory,
            options,
            tenantTags,
            cancellationToken);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        HybridCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default)
    {
        var tenantKey = GetTenantKey(key);
        var tenantTags = GetTenantTags(tags);

        await _cache.SetAsync(tenantKey, value, options, tenantTags, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var tenantKey = GetTenantKey(key);
        await _cache.RemoveAsync(tenantKey, cancellationToken);
    }

    public async Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        var tenantTag = GetTenantTag(tag);
        await _cache.RemoveByTagAsync(tenantTag, cancellationToken);
    }

    private string GetTenantKey(string key)
    {
        var tenantId = _tenantContext.Tenant.Id;
        return $"tenant:{tenantId}:{key}";
    }

    private IEnumerable<string> GetTenantTags(IEnumerable<string>? tags)
    {
        var tenantId = _tenantContext.Tenant.Id;
        var tenantTags = new List<string> { $"tenant:{tenantId}" };

        if (tags != null)
        {
            tenantTags.AddRange(tags.Select(tag => $"tenant:{tenantId}:{tag}"));
        }

        return tenantTags;
    }

    private string GetTenantTag(string tag)
    {
        var tenantId = _tenantContext.Tenant.Id;
        return $"tenant:{tenantId}:{tag}";
    }
}
```

### Order Processing with Cache Invalidation

```csharp
// src/backend/Store/Services/OrderProcessingService.cs
using Microsoft.Extensions.Caching.Hybrid;

public class OrderProcessingService
{
    private readonly IOrderRepository _orderRepository;
    private readonly MultitenantHybridCache _cache;
    private readonly ILogger<OrderProcessingService> _logger;

    public OrderProcessingService(
        IOrderRepository orderRepository,
        MultitenantHybridCache cache,
        ILogger<OrderProcessingService> logger)
    {
        _orderRepository = orderRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        var order = await _orderRepository.CreateOrderAsync(request);

        // Invalidate related caches
        await InvalidateOrderCachesAsync(order);

        _logger.LogInformation("Created order {OrderId} for customer {CustomerId}",
            order.Id, order.CustomerId);

        return order;
    }

    public async Task<Order> UpdateOrderAsync(string orderId, UpdateOrderRequest request)
    {
        var order = await _orderRepository.UpdateOrderAsync(orderId, request);

        // Cache the updated order
        await _cache.SetAsync(
            $"order:{orderId}",
            order,
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30) },
            new[] { "orders", $"customer:{order.CustomerId}" });

        // Invalidate related caches
        await InvalidateOrderCachesAsync(order);

        return order;
    }

    private async Task InvalidateOrderCachesAsync(Order order)
    {
        // Invalidate customer's order list
        await _cache.RemoveByTagAsync($"customer:{order.CustomerId}");

        // Invalidate product stock caches for ordered items
        foreach (var item in order.OrderItems)
        {
            await _cache.RemoveAsync($"product:{item.ProductId}:stock");
        }
    }
}
```

### ERP Data Caching with Custom Serialization

```csharp
// src/backend/ERP/Services/CachedErpService.cs
using Microsoft.Extensions.Caching.Hybrid;

public class CachedErpService
{
    private readonly IErpConnector _erpConnector;
    private readonly HybridCache _cache;

    public CachedErpService(IErpConnector erpConnector, HybridCache cache)
    {
        _erpConnector = erpConnector;
        _cache = cache;
    }

    public async Task<ErpProductData> GetProductDataAsync(string productId)
    {
        return await _cache.GetOrCreateAsync(
            $"erp:product:{productId}",
            async cancellationToken =>
            {
                // ERP calls can be slow and expensive
                var data = await _erpConnector.GetProductAsync(productId);

                // Transform to internal format
                return new ErpProductData
                {
                    ProductId = data.Id,
                    Name = data.Name,
                    Price = data.NetPrice,
                    Stock = data.AvailableStock,
                    LastUpdated = DateTime.UtcNow
                };
            },
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromHours(6), // ERP data changes less frequently
                LocalCacheExpiration = TimeSpan.FromMinutes(30)
            },
            new[] { "erp", "products" });
    }

    public async Task SyncProductStockAsync(string productId)
    {
        // Force refresh from ERP
        var freshData = await _erpConnector.GetProductAsync(productId);

        var productData = new ErpProductData
        {
            ProductId = freshData.Id,
            Name = freshData.Name,
            Price = freshData.NetPrice,
            Stock = freshData.AvailableStock,
            LastUpdated = DateTime.UtcNow
        };

        // Update cache with fresh data
        await _cache.SetAsync(
            $"erp:product:{productId}",
            productData,
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromHours(6) },
            new[] { "erp", "products" });
    }
}
```

### Performance Monitoring with Cache Metrics

```csharp
// src/backend/Shared/Infrastructure/Monitoring/CacheMetricsService.cs
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Diagnostics.Metrics;

public class CacheMetricsService
{
    private readonly HybridCache _cache;
    private readonly IMeterFactory _meterFactory;
    private readonly ILogger<CacheMetricsService> _logger;

    public CacheMetricsService(
        HybridCache cache,
        IMeterFactory meterFactory,
        ILogger<CacheMetricsService> logger)
    {
        _cache = cache;
        _meterFactory = meterFactory;
        _logger = logger;
    }

    public async Task<T> GetOrCreateWithMetricsAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        string operationName,
        CancellationToken cancellationToken = default)
    {
        var meter = _meterFactory.Create("B2X.Cache");
        var cacheHits = meter.CreateCounter<long>("cache_hits", description: "Number of cache hits");
        var cacheMisses = meter.CreateCounter<long>("cache_misses", description: "Number of cache misses");
        var cacheLatency = meter.CreateHistogram<double>("cache_operation_duration", "ms", "Duration of cache operations");

        using var timer = new System.Diagnostics.Stopwatch();
        timer.Start();

        try
        {
            var result = await _cache.GetOrCreateAsync(key, factory, cancellationToken: cancellationToken);

            timer.Stop();
            cacheLatency.Record(timer.ElapsedMilliseconds);

            // We can't easily determine if it was a hit or miss from the API
            // But we can log the operation
            _logger.LogDebug("Cache operation {Operation} for key {Key} took {Duration}ms",
                operationName, key, timer.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            timer.Stop();
            _logger.LogError(ex, "Cache operation {Operation} for key {Key} failed after {Duration}ms",
                operationName, key, timer.ElapsedMilliseconds);
            throw;
        }
    }
}
```

## Advanced Patterns

### Custom Serialization

```csharp
using Microsoft.Extensions.Caching.Hybrid;

// Protobuf serializer for high-performance scenarios
public class ProtobufSerializer<T> : IHybridCacheSerializer<T>
{
    public T Deserialize(ReadOnlySequence<byte> source)
    {
        using var stream = new MemoryStream(source.ToArray());
        return ProtoBuf.Serializer.Deserialize<T>(stream);
    }

    public void Serialize(T value, IBufferWriter<byte> target)
    {
        using var stream = new MemoryStream();
        ProtoBuf.Serializer.Serialize(stream, value);
        target.Write(stream.ToArray());
    }
}

// Register in DI
builder.Services.AddHybridCache()
    .AddSerializer<ErpProductData, ProtobufSerializer<ErpProductData>>();
```

### Cache Warming Strategy

```csharp
public class CacheWarmerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CacheWarmerService> _logger;

    public CacheWarmerService(IServiceProvider serviceProvider, ILogger<CacheWarmerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await WarmCriticalCachesAsync();
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache warming failed");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

    private async Task WarmCriticalCachesAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

        // Warm frequently accessed products
        var popularProductIds = await GetPopularProductIdsAsync();
        var warmingTasks = popularProductIds.Select(id =>
            productService.GetProductAsync(id)).ToArray();

        await Task.WhenAll(warmingTasks);
        _logger.LogInformation("Warmed cache for {Count} popular products", warmingTasks.Length);
    }

    private async Task<IEnumerable<string>> GetPopularProductIdsAsync()
    {
        // Get from analytics database or configuration
        return new[] { "prod-1", "prod-2", "prod-3" };
    }
}
```

### Conditional Caching Strategy

```csharp
public class ConditionalCacheService
{
    private readonly HybridCache _cache;

    public ConditionalCacheService(HybridCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetOrCreateIfAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        Func<T, bool> shouldCache,
        HybridCacheEntryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        // Try to get from cache first
        var cachedResult = await _cache.GetOrCreateAsync(
            key,
            async ct =>
            {
                var result = await factory(ct);
                return shouldCache(result) ? result : default;
            },
            options,
            cancellationToken: cancellationToken);

        // If factory returned default (shouldn't cache), call factory again
        if (EqualityComparer<T>.Default.Equals(cachedResult, default))
        {
            return await factory(cancellationToken);
        }

        return cachedResult;
    }
}

// Usage example
public async Task<Product> GetProductIfActiveAsync(string productId)
{
    return await _conditionalCache.GetOrCreateIfAsync(
        $"product:{productId}",
        cancellationToken => _repository.GetProductAsync(productId),
        product => product.IsActive, // Only cache active products
        new HybridCacheEntryOptions { Expiration = TimeSpan.FromHours(1) });
}
```

### Cache Dependency Management

```csharp
public class CacheDependencyManager
{
    private readonly HybridCache _cache;
    private readonly Dictionary<string, HashSet<string>> _dependencies;

    public CacheDependencyManager(HybridCache cache)
    {
        _cache = cache;
        _dependencies = new Dictionary<string, HashSet<string>>();
    }

    public async Task SetWithDependenciesAsync<T>(
        string key,
        T value,
        IEnumerable<string> dependencyKeys,
        HybridCacheEntryOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        // Store the value
        await _cache.SetAsync(key, value, options, cancellationToken: cancellationToken);

        // Track dependencies
        lock (_dependencies)
        {
            foreach (var depKey in dependencyKeys)
            {
                if (!_dependencies.ContainsKey(depKey))
                {
                    _dependencies[depKey] = new HashSet<string>();
                }
                _dependencies[depKey].Add(key);
            }
        }
    }

    public async Task InvalidateDependenciesAsync(string changedKey, CancellationToken cancellationToken = default)
    {
        HashSet<string> dependentKeys;
        lock (_dependencies)
        {
            if (!_dependencies.TryGetValue(changedKey, out dependentKeys))
            {
                return;
            }
        }

        // Remove all dependent cache entries
        var removeTasks = dependentKeys.Select(key => _cache.RemoveAsync(key, cancellationToken));
        await Task.WhenAll(removeTasks);
    }
}
```

## Testing

### Testing Cache Behavior

```csharp
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

public class ProductServiceTests
{
    [Fact]
    public async Task GetProductAsync_CachesResult()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddHybridCache();
        var provider = services.BuildServiceProvider();

        var cache = provider.GetRequiredService<HybridCache>();
        var repositoryMock = new Mock<IProductRepository>();
        var service = new ProductService(repositoryMock.Object, cache);

        var product = new Product { Id = "test-1", Name = "Test Product" };
        repositoryMock.Setup(r => r.GetProductAsync("test-1")).ReturnsAsync(product);

        // Act - First call should hit repository
        var result1 = await service.GetProductAsync("test-1");
        var result2 = await service.GetProductAsync("test-1"); // Should hit cache

        // Assert
        Assert.Equal(product, result1);
        Assert.Equal(product, result2);
        repositoryMock.Verify(r => r.GetProductAsync("test-1"), Times.Once); // Only called once
    }

    [Fact]
    public async Task GetProductAsync_HandlesCacheMiss()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddHybridCache();
        var provider = services.BuildServiceProvider();

        var cache = provider.GetRequiredService<HybridCache>();
        var repositoryMock = new Mock<IProductRepository>();
        var service = new ProductService(repositoryMock.Object, cache);

        repositoryMock.Setup(r => r.GetProductAsync("missing"))
            .ThrowsAsync(new ProductNotFoundException("Product not found"));

        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(() =>
            service.GetProductAsync("missing"));
    }
}
```

### Testing Cache Invalidation

```csharp
public class CacheInvalidationTests
{
    [Fact]
    public async Task RemoveByTagAsync_InvalidatesTaggedEntries()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddHybridCache();
        var provider = services.BuildServiceProvider();

        var cache = provider.GetRequiredService<HybridCache>();

        // Set multiple entries with same tag
        await cache.SetAsync("product:1", "Product 1", tags: new[] { "products" });
        await cache.SetAsync("product:2", "Product 2", tags: new[] { "products" });
        await cache.SetAsync("category:1", "Category 1", tags: new[] { "categories" });

        // Act - Remove by tag
        await cache.RemoveByTagAsync("products");

        // Assert - Products should be gone, category should remain
        var product1 = await cache.GetAsync<string>("product:1");
        var product2 = await cache.GetAsync<string>("product:2");
        var category1 = await cache.GetAsync<string>("category:1");

        Assert.Null(product1);
        Assert.Null(product2);
        Assert.Equal("Category 1", category1);
    }
}
```

### Testing Stampede Protection

```csharp
public class StampedeProtectionTests
{
    [Fact]
    public async Task GetOrCreateAsync_PreventsStampede()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddHybridCache();
        var provider = services.BuildServiceProvider();

        var cache = provider.GetRequiredService<HybridCache>();
        var callCount = 0;

        // Factory that takes time and counts calls
        async Task<string> Factory(CancellationToken ct)
        {
            Interlocked.Increment(ref callCount);
            await Task.Delay(100, ct); // Simulate slow operation
            return "result";
        }

        // Act - Multiple concurrent requests
        var tasks = Enumerable.Range(0, 10).Select(_ =>
            cache.GetOrCreateAsync("test-key", Factory)).ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert - Factory called only once despite 10 concurrent requests
        Assert.Equal(1, callCount);
        Assert.All(results, result => Assert.Equal("result", result));
    }
}
```

## Configuration

### appsettings.json Configuration

```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  },
  "HybridCache": {
    "MaximumPayloadBytes": 1048576,
    "MaximumKeyLength": 1024,
    "DefaultEntryOptions": {
      "Expiration": "00:05:00",
      "LocalCacheExpiration": "00:01:00"
    }
  }
}
```

### Program.cs Setup

```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure Redis cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "B2X:"; // Prefix for multi-tenant isolation
});

// Configure hybrid cache
builder.Services.AddHybridCache(options =>
{
    // Bind from configuration
    builder.Configuration.GetSection("HybridCache").Bind(options);

    // Configure serialization
    options.AddSerializer<Product, JsonSerializer<Product>>();
});

// Add custom services
builder.Services.AddScoped<IProductService, CachedProductService>();
builder.Services.AddSingleton<MultitenantHybridCache>();

var app = builder.Build();
```

## Limitations

- Tag-based invalidation is logical only (doesn't remove from other server instances)
- Maximum key length of 1024 characters by default
- Maximum payload size of 1MB by default
- No built-in cache warming mechanisms
- Requires IDistributedCache for L2 cache (optional but recommended)
- Native AOT requires explicit serializer configuration

## Performance Considerations

- **Object Reuse**: Mark immutable types with `[ImmutableObject(true)]` for better performance
- **Buffer Optimization**: Use IBufferDistributedCache implementations when available
- **Key Design**: Use string interpolation directly in GetOrCreateAsync calls
- **Tag Management**: Limit number of tags per entry for better performance
- **Serialization**: Choose appropriate serializers based on data types and performance needs

## Related Libraries

- **Microsoft.Extensions.Caching.StackExchangeRedis**: Redis distributed cache implementation
- **Microsoft.Extensions.Caching.SqlServer**: SQL Server distributed cache
- **Microsoft.Extensions.Caching.Memory**: In-memory cache implementation
- **Microsoft.Extensions.Diagnostics**: Performance monitoring and metrics

This library provides B2X with enterprise-grade caching capabilities, enabling high-performance data access across the distributed multi-tenant architecture while maintaining cache consistency and preventing common caching pitfalls like stampedes and invalidation issues.