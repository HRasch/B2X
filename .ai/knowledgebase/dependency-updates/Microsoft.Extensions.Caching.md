---
docid: KB-099
title: Microsoft.Extensions.Caching
owner: @Backend
status: Active
created: 2026-01-08
---

# Microsoft.Extensions.Caching

**Overview**: Provides caching abstractions for .NET applications, including in-memory and distributed caching implementations.

**Source**: [dotnet/extensions](https://github.com/dotnet/extensions)  
**NuGet Packages**: 
- `Microsoft.Extensions.Caching.Memory` (In-memory caching)
- `Microsoft.Extensions.Caching.Distributed` (Distributed caching abstractions)
- `Microsoft.Extensions.Caching.SqlServer` (SQL Server distributed cache)
- `Microsoft.Extensions.Caching.StackExchangeRedis` (Redis distributed cache)

**Documentation**: [Caching in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/caching)  
**Current Version**: 9.0.0 (January 2026)

## Key Components

### In-Memory Caching
- **IMemoryCache**: Core interface for in-memory caching
- **MemoryCache**: Default implementation using ConcurrentDictionary
- **MemoryCacheEntryOptions**: Configuration for cache entries (expiration, priority, callbacks)
- **ICacheEntry**: Represents individual cache entries

### Distributed Caching
- **IDistributedCache**: Abstraction for distributed cache implementations
- **DistributedCacheEntryOptions**: Configuration for distributed cache entries
- **Extension Methods**: Convenience methods for string serialization/deserialization

## Basic Usage

### In-Memory Caching

#### Setup
```csharp
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddMemoryCache();
using IHost host = builder.Build();

IMemoryCache cache = host.Services.GetRequiredService<IMemoryCache>();
```

#### Basic Operations
```csharp
// Set a value
cache.Set("key", "value");

// Get a value
if (cache.TryGetValue("key", out string? value))
{
    Console.WriteLine(value);
}

// Get or create (synchronous)
string result = cache.GetOrCreate("key", entry =>
{
    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
    return "computed value";
});

// Get or create (asynchronous)
string asyncResult = await cache.GetOrCreateAsync("key", async entry =>
{
    entry.SlidingExpiration = TimeSpan.FromMinutes(10);
    return await ComputeValueAsync();
});
```

#### Cache Entry Options
```csharp
MemoryCacheEntryOptions options = new()
{
    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
    SlidingExpiration = TimeSpan.FromMinutes(30),
    Priority = CacheItemPriority.Normal,
    Size = 1024 // Size for memory limits
};

options.RegisterPostEvictionCallback((key, value, reason, state) =>
{
    Console.WriteLine($"{key} was evicted: {reason}");
});

cache.Set("key", "value", options);
```

### Distributed Caching

#### Setup with Redis
```csharp
using Microsoft.Extensions.Caching.StackExchangeRedis;

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "myapp:";
});
```

#### Setup with SQL Server
```csharp
using Microsoft.Extensions.Caching.SqlServer;

builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = "Server=.;Database=CacheDb;Trusted_Connection=True;";
    options.SchemaName = "dbo";
    options.TableName = "CacheTable";
});
```

#### Basic Operations
```csharp
IDistributedCache distributedCache = serviceProvider.GetRequiredService<IDistributedCache>();

// Set a value
await distributedCache.SetStringAsync("key", "value", new DistributedCacheEntryOptions
{
    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
});

// Get a value
string? value = await distributedCache.GetStringAsync("key");

// Remove a value
await distributedCache.RemoveAsync("key");

// Refresh sliding expiration
await distributedCache.RefreshAsync("key");
```

## Integration in B2X Project

The B2X project uses Microsoft.Extensions.Caching for:

- **API Response Caching**: Cache frequently accessed data from external APIs
- **Computed Results**: Cache expensive calculations and database queries
- **Session Data**: Store user session information
- **Configuration**: Cache application configuration data
- **Multitenant Data**: Tenant-specific caching with isolation
- **ERP Integration**: Cache ERP responses to reduce external API calls

### Recommended Patterns

#### Multitenant Caching Service
```csharp
public class TenantAwareCacheService
{
    private readonly IDistributedCache _cache;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<TenantAwareCacheService> _logger;

    public TenantAwareCacheService(
        IDistributedCache cache,
        ITenantContext tenantContext,
        ILogger<TenantAwareCacheService> logger)
    {
        _cache = cache;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        var tenantKey = GetTenantKey(key);
        var cached = await _cache.GetStringAsync(tenantKey);

        if (cached != null)
        {
            _logger.LogDebug("Cache hit for tenant {TenantId}, key {Key}",
                _tenantContext.TenantId, key);
            return JsonSerializer.Deserialize<T>(cached);
        }

        return null;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var tenantKey = GetTenantKey(key);
        var options = new DistributedCacheEntryOptions();

        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration.Value;
        }
        else
        {
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
        }

        var serialized = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(tenantKey, serialized, options);

        _logger.LogDebug("Cache set for tenant {TenantId}, key {Key}",
            _tenantContext.TenantId, key);
    }

    public async Task RemoveAsync(string key)
    {
        var tenantKey = GetTenantKey(key);
        await _cache.RemoveAsync(tenantKey);

        _logger.LogDebug("Cache removed for tenant {TenantId}, key {Key}",
            _tenantContext.TenantId, key);
    }

    private string GetTenantKey(string key)
    {
        return $"tenant:{_tenantContext.TenantId}:{key}";
    }
}
```

#### Wolverine CQRS Integration
```csharp
public class CachedQueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    private readonly IQueryHandler<TQuery, TResult> _innerHandler;
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachedQueryHandler<TQuery, TResult>> _logger;

    public CachedQueryHandler(
        IQueryHandler<TQuery, TResult> innerHandler,
        IDistributedCache cache,
        ILogger<CachedQueryHandler<TQuery, TResult>> logger)
    {
        _innerHandler = innerHandler;
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResult> Handle(TQuery query)
    {
        var cacheKey = GetCacheKey(query);

        // Try cache first
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
        {
            _logger.LogDebug("Query cache hit for {QueryType}", typeof(TQuery).Name);
            return JsonSerializer.Deserialize<TResult>(cached)!;
        }

        // Execute query
        var result = await _innerHandler.Handle(query);

        // Cache result
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = GetCacheDuration(query)
        };

        await _cache.SetStringAsync(cacheKey,
            JsonSerializer.Serialize(result), options);

        _logger.LogDebug("Query cache miss for {QueryType}, result cached",
            typeof(TQuery).Name);

        return result;
    }

    private string GetCacheKey(TQuery query)
    {
        // Generate deterministic cache key from query
        var queryJson = JsonSerializer.Serialize(query);
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(queryJson));
        return $"query:{typeof(TQuery).Name}:{Convert.ToBase64String(hash)}";
    }

    private TimeSpan GetCacheDuration(TQuery query)
    {
        // Customize cache duration based on query type
        return query switch
        {
            ICacheableQuery cacheable => cacheable.CacheDuration,
            _ => TimeSpan.FromMinutes(5)
        };
    }
}

public interface ICacheableQuery
{
    TimeSpan CacheDuration { get; }
}
```

#### ERP Response Caching
```csharp
public class ErpResponseCache
{
    private readonly IDistributedCache _cache;
    private readonly IErpClient _erpClient;
    private readonly ILogger<ErpResponseCache> _logger;

    public async Task<T> GetOrFetchAsync<T>(
        string endpoint,
        Dictionary<string, string> parameters,
        TimeSpan? cacheDuration = null)
    {
        var cacheKey = GenerateCacheKey(endpoint, parameters);

        // Try cache
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
        {
            _logger.LogInformation("ERP cache hit for {Endpoint}", endpoint);
            return JsonSerializer.Deserialize<T>(cached)!;
        }

        // Fetch from ERP
        _logger.LogInformation("ERP cache miss for {Endpoint}, fetching from ERP", endpoint);
        var result = await _erpClient.GetAsync<T>(endpoint, parameters);

        // Cache result
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheDuration ?? TimeSpan.FromMinutes(15)
        };

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), options);

        return result;
    }

    public async Task InvalidateAsync(string endpoint, Dictionary<string, string> parameters = null)
    {
        var cacheKey = GenerateCacheKey(endpoint, parameters);
        await _cache.RemoveAsync(cacheKey);
        _logger.LogInformation("ERP cache invalidated for {Endpoint}", endpoint);
    }

    private string GenerateCacheKey(string endpoint, Dictionary<string, string> parameters)
    {
        var keyBuilder = new StringBuilder($"erp:{endpoint}");
        if (parameters != null && parameters.Count > 0)
        {
            var sortedParams = parameters.OrderBy(p => p.Key);
            foreach (var param in sortedParams)
            {
                keyBuilder.Append($":{param.Key}={param.Value}");
            }
        }
        return keyBuilder.ToString();
    }
}
```

#### Cache Invalidation Patterns
```csharp
public class CacheInvalidationService
{
    private readonly IDistributedCache _cache;
    private readonly IMediator _mediator;
    private readonly ILogger<CacheInvalidationService> _logger;

    public CacheInvalidationService(
        IDistributedCache cache,
        IMediator mediator,
        ILogger<CacheInvalidationService> logger)
    {
        _cache = cache;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task InvalidateProductCacheAsync(int productId)
    {
        var keys = new[]
        {
            $"product:{productId}",
            $"product:details:{productId}",
            $"products:category:*", // Pattern invalidation
            "products:featured"
        };

        foreach (var key in keys)
        {
            if (key.Contains("*"))
            {
                // Handle pattern invalidation (would need custom implementation)
                await InvalidatePatternAsync(key);
            }
            else
            {
                await _cache.RemoveAsync(key);
            }
        }

        // Publish cache invalidation event
        await _mediator.Publish(new ProductCacheInvalidated(productId));

        _logger.LogInformation("Product cache invalidated for product {ProductId}", productId);
    }

    private async Task InvalidatePatternAsync(string pattern)
    {
        // Note: IDistributedCache doesn't support pattern matching
        // This would require a custom cache implementation or
        // maintaining a separate index of cache keys

        _logger.LogWarning("Pattern invalidation not implemented for distributed cache: {Pattern}", pattern);
    }
}

public record ProductCacheInvalidated(int ProductId);
```
```csharp
public class CachedProductService
{
    private readonly IMemoryCache _cache;
    private readonly IProductRepository _repository;
    private readonly ILogger<CachedProductService> _logger;

    public CachedProductService(
        IMemoryCache cache,
        IProductRepository repository,
        ILogger<CachedProductService> logger)
    {
        _cache = cache;
        _repository = repository;
        _logger = logger;
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        string cacheKey = $"product:{id}";

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            entry.SlidingExpiration = TimeSpan.FromMinutes(10);

            _logger.LogInformation("Fetching product {Id} from database", id);
            return await _repository.GetProductAsync(id);
        });
    }

    public async Task UpdateProductAsync(Product product)
    {
        await _repository.UpdateProductAsync(product);

        // Invalidate cache
        _cache.Remove($"product:{product.Id}");

        _logger.LogInformation("Product {Id} cache invalidated", product.Id);
    }
}
```

#### Background Cache Refresh
```csharp
public class CacheRefreshService : BackgroundService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheRefreshService> _logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Refresh expensive data in background
                var data = await FetchExpensiveDataAsync();
                await _cache.SetStringAsync("expensive-data",
                    JsonSerializer.Serialize(data),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                    });

                _logger.LogInformation("Cache refreshed with latest data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh cache");
            }

            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }
}
```

## Cache Strategies

### Cache-Aside Pattern
```csharp
public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory)
{
    if (_cache.TryGetValue(key, out T? cached))
    {
        return cached;
    }

    T result = await factory();
    _cache.Set(key, result, GetCacheOptions());
    return result;
}
```

### Cache-Through Pattern
```csharp
public async Task<T> GetAsync<T>(string key)
{
    return await _cache.GetOrCreateAsync(key, async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        return await _dataSource.GetAsync<T>(key);
    });
}
```

### Write-Through Pattern
```csharp
public async Task SetAsync<T>(string key, T value)
{
    await _dataSource.SetAsync(key, value);
    await _cache.SetStringAsync(key, JsonSerializer.Serialize(value));
}
```

## Performance Considerations

### Memory Limits
```csharp
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024 * 1024 * 100; // 100 MB
    options.CompactionPercentage = 0.1; // 10% compaction
});
```

### Cache Key Strategies
```csharp
public static class CacheKeys
{
    public static string Product(int id) => $"product:{id}";
    public static string ProductsByCategory(string category) => $"products:category:{category}";
    public static string UserPreferences(string userId) => $"user:{userId}:preferences";
}
```

### Cache Metrics
```csharp
public class CacheMetricsService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheMetricsService> _logger;

    public void LogCacheStats()
    {
        if (_cache is MemoryCache memoryCache)
        {
            var stats = memoryCache.GetCurrentStatistics();
            _logger.LogInformation(
                "Cache Stats - Entries: {Count}, Size: {Size} bytes",
                stats?.TotalEntries ?? 0,
                stats?.CurrentSize ?? 0);
        }
    }
}
```

## Testing Considerations

### Unit Testing with Cache
```csharp
public class ProductServiceTests
{
    [Fact]
    public async Task GetProductAsync_CachesResult()
    {
        // Arrange
        var mockCache = new Mock<IMemoryCache>();
        var mockRepository = new Mock<IProductRepository>();

        object? cachedValue = null;
        mockCache.Setup(c => c.TryGetValue("product:1", out cachedValue))
                .Returns(false)
                .Callback((object key, out object? value) => value = null);

        mockCache.Setup(c => c.Set(It.IsAny<object>(), It.IsAny<object>(), It.IsAny<MemoryCacheEntryOptions>()))
                .Verifiable();

        var service = new CachedProductService(mockCache.Object, mockRepository.Object, Mock.Of<ILogger<CachedProductService>>());

        // Act
        await service.GetProductAsync(1);

        // Assert
        mockCache.Verify(c => c.Set(It.IsAny<object>(), It.IsAny<object>(), It.IsAny<MemoryCacheEntryOptions>()), Times.Once);
    }
}
```

### Integration Testing
```csharp
public class CacheIntegrationTests : IClassFixture<RedisContainerFixture>
{
    private readonly RedisContainerFixture _fixture;

    public CacheIntegrationTests(RedisContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DistributedCache_PersistsAcrossInstances()
    {
        // Test distributed cache behavior
        var services1 = CreateServices(_fixture.ConnectionString);
        var services2 = CreateServices(_fixture.ConnectionString);

        var cache1 = services1.GetRequiredService<IDistributedCache>();
        var cache2 = services2.GetRequiredService<IDistributedCache>();

        await cache1.SetStringAsync("shared", "value");

        var result = await cache2.GetStringAsync("shared");
        Assert.Equal("value", result);
    }
}
```

## Best Practices

1. **Use Appropriate Cache Types**: In-memory for single server, distributed for multi-server
2. **Set Reasonable Expirations**: Balance performance vs data freshness
3. **Handle Cache Failures**: Design for cache unavailability
4. **Monitor Cache Performance**: Track hit rates and memory usage
5. **Use Structured Keys**: Consistent naming conventions
6. **Avoid Cache Stampede**: Use locks for expensive operations
7. **Test Cache Behavior**: Include cache testing in CI/CD

## Version History

- **8.0.0**: Initial release with .NET 8 support
- **8.1.0**: Enhanced performance and memory management
- **8.2.0**: Added cache compaction features
- **9.0.0**: .NET 9 compatibility, improved async support

## References

- [Microsoft.Extensions.Caching.Memory NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Memory)
- [Microsoft.Extensions.Caching.Distributed NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Distributed)
- [GitHub Repository](https://github.com/dotnet/extensions)
- [Caching in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory)

---

**Next Review**: April 2026