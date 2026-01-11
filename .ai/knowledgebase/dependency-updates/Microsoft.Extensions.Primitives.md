---
docid: KB-113
title: Microsoft.Extensions.Primitives
owner: @TechLead
status: Active
created: 2026-01-11
---

# Microsoft.Extensions.Primitives

**Version:** 10.0.1  
**Package:** Microsoft.Extensions.Primitives  
**NuGet:** https://www.nuget.org/packages/Microsoft.Extensions.Primitives/

## Overview

Microsoft.Extensions.Primitives provides fundamental types and abstractions used throughout the .NET Extensions ecosystem. This library contains isolated primitive types that enable efficient string manipulation and change notification patterns used by configuration, file providers, HTTP handling, and other framework extensions.

## Key Interfaces and Types

### IChangeToken
The core interface for change notifications:

```csharp
public interface IChangeToken
{
    bool HasChanged { get; }
    bool ActiveChangeCallbacks { get; }
    IDisposable RegisterChangeCallback(Action<object> callback, object state);
}
```

### StringValues
Efficient representation of zero, one, or many strings:

```csharp
public struct StringValues : IEnumerable<string>, IEquatable<StringValues>
{
    // Constructors
    public StringValues(string value);
    public StringValues(string[] values);

    // Properties
    public int Count { get; }
    public string this[int index] { get; }

    // Methods
    public static bool IsNullOrEmpty(StringValues value);
    public override string ToString();
}
```

### StringSegment
Optimized substring representation without allocation:

```csharp
public struct StringSegment : IEquatable<StringSegment>
{
    public StringSegment(string buffer);
    public StringSegment(string buffer, int offset, int length);

    public string Buffer { get; }
    public int Offset { get; }
    public int Length { get; }
    public bool IsNullOrEmpty { get; }
    public char this[int index] { get; }
}
```

## Implementations

### CancellationChangeToken
Change token based on CancellationToken:

```csharp
var cts = new CancellationTokenSource();
var changeToken = new CancellationChangeToken(cts.Token);

// Trigger change
cts.Cancel();
```

### CompositeChangeToken
Combines multiple change tokens:

```csharp
var token1 = fileProvider.Watch("*.json");
var token2 = configuration.GetReloadToken();
var compositeToken = new CompositeChangeToken(new[] { token1, token2 });
```

## B2X Integration Patterns

### Configuration Change Monitoring

```csharp
public class ConfigurationMonitor
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationMonitor> _logger;

    public ConfigurationMonitor(
        IConfiguration configuration,
        ILogger<ConfigurationMonitor> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Monitor configuration changes
        ChangeToken.OnChange(
            () => _configuration.GetReloadToken(),
            () => OnConfigurationChanged());
    }

    private void OnConfigurationChanged()
    {
        _logger.LogInformation("Configuration reloaded");
        // Invalidate caches, update services, etc.
    }
}
```

### Multitenant Configuration with Change Tokens

```csharp
public class TenantConfigurationProvider
{
    private readonly ConcurrentDictionary<string, IChangeToken> _changeTokens = new();

    public IConfiguration GetTenantConfiguration(string tenantId)
    {
        var config = LoadTenantConfiguration(tenantId);

        // Create change token for tenant-specific files
        var changeToken = new CancellationChangeTokenSource().Token;
        _changeTokens[tenantId] = changeToken;

        return config;
    }

    public IChangeToken GetTenantChangeToken(string tenantId)
    {
        return _changeTokens.GetOrAdd(tenantId, _ =>
            new CancellationChangeTokenSource().Token);
    }
}
```

### HTTP Header Processing with StringValues

```csharp
public class HeaderProcessor
{
    public void ProcessHeaders(HttpContext context)
    {
        // Headers can have multiple values
        StringValues acceptHeaders = context.Request.Headers.Accept;

        foreach (string acceptHeader in acceptHeaders)
        {
            // Process each accept header
            ProcessAcceptHeader(acceptHeader);
        }

        // Set multiple response headers efficiently
        context.Response.Headers.CacheControl = new StringValues(new[]
        {
            "no-cache",
            "no-store",
            "must-revalidate"
        });
    }
}
```

### Query String Parsing with StringSegment

```csharp
public class QueryParser
{
    public Dictionary<string, StringValues> ParseQueryString(string queryString)
    {
        var parameters = new Dictionary<string, StringValues>();

        if (string.IsNullOrEmpty(queryString))
            return parameters;

        var tokenizer = new StringTokenizer(queryString, new[] { '&' });

        foreach (var segment in tokenizer)
        {
            var pair = segment.Trim();
            var equalsIndex = pair.IndexOf('=');

            if (equalsIndex > 0)
            {
                var key = pair.Substring(0, equalsIndex);
                var value = pair.Substring(equalsIndex + 1);

                // Handle multiple values for same key
                if (parameters.TryGetValue(key, out var existing))
                {
                    parameters[key] = StringValues.Concat(existing, value);
                }
                else
                {
                    parameters[key] = new StringValues(value);
                }
            }
        }

        return parameters;
    }
}
```

### File Change Monitoring for Cache Invalidation

```csharp
public class CacheInvalidator
{
    private readonly IMemoryCache _cache;
    private readonly IFileProvider _fileProvider;
    private readonly ILogger<CacheInvalidator> _logger;

    public CacheInvalidator(
        IMemoryCache cache,
        IFileProvider fileProvider,
        ILogger<CacheInvalidator> logger)
    {
        _cache = cache;
        _fileProvider = fileProvider;
        _logger = logger;

        // Watch for changes to cached files
        var changeToken = _fileProvider.Watch("*.json");
        ChangeToken.OnChange(
            () => changeToken,
            () => InvalidateCache());
    }

    private void InvalidateCache()
    {
        _logger.LogInformation("Invalidating cache due to file changes");
        // Clear specific cache entries or entire cache
        _cache.Remove("configuration");
        _cache.Remove("templates");
    }
}
```

## Performance Optimizations

### StringValues for Efficient Header Handling

```csharp
// Instead of List<string> which allocates
private StringValues _cachedHeaders;

// Efficient concatenation
public void AddHeader(string name, string value)
{
    if (_cachedHeaders.Count == 0)
    {
        _cachedHeaders = new StringValues(value);
    }
    else
    {
        _cachedHeaders = StringValues.Concat(_cachedHeaders, value);
    }
}
```

### StringSegment for Zero-Allocation Parsing

```csharp
public class PathParser
{
    public (StringSegment directory, StringSegment fileName) ParsePath(string path)
    {
        var lastSeparator = path.LastIndexOf(Path.DirectorySeparatorChar);

        if (lastSeparator >= 0)
        {
            return (
                new StringSegment(path, 0, lastSeparator),
                new StringSegment(path, lastSeparator + 1, path.Length - lastSeparator - 1)
            );
        }

        return (StringSegment.Empty, new StringSegment(path));
    }
}
```

### Change Token Composition for Complex Dependencies

```csharp
public class MultiSourceChangeToken
{
    private readonly IFileProvider _fileProvider;
    private readonly IConfiguration _configuration;

    public IChangeToken CreateCompositeToken()
    {
        var fileToken = _fileProvider.Watch("**/*.json");
        var configToken = _configuration.GetReloadToken();

        return new CompositeChangeToken(new[] { fileToken, configToken });
    }
}
```

## Testing Patterns

### Mocking Change Tokens

```csharp
public class MockChangeToken : IChangeToken
{
    private readonly Action _callback;

    public MockChangeToken(Action callback = null)
    {
        _callback = callback;
    }

    public bool HasChanged => false;
    public bool ActiveChangeCallbacks => true;

    public IDisposable RegisterChangeCallback(Action<object> callback, object state)
    {
        return new CallbackRegistration(() => callback(state));
    }

    public void TriggerChange()
    {
        _callback?.Invoke();
    }

    private class CallbackRegistration : IDisposable
    {
        private readonly Action _action;

        public CallbackRegistration(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action();
        }
    }
}
```

### Testing StringValues Operations

```csharp
[Test]
public void StringValues_Concatenation_WorksCorrectly()
{
    // Arrange
    var values1 = new StringValues(new[] { "a", "b" });
    var values2 = new StringValues("c");

    // Act
    var result = StringValues.Concat(values1, values2);

    // Assert
    Assert.Equal(3, result.Count);
    Assert.Equal("a", result[0]);
    Assert.Equal("b", result[1]);
    Assert.Equal("c", result[2]);
}
```

## Common Patterns

### Configuration Reload with Callbacks

```csharp
services.Configure<AppSettings>(configuration)
    .AddOptions()
    .Configure(options =>
    {
        ChangeToken.OnChange(
            () => configuration.GetReloadToken(),
            () => options.Reload());
    });
```

### HTTP Request Processing

```csharp
public class RequestProcessor
{
    public async Task ProcessRequest(HttpContext context)
    {
        // Efficient header access
        StringValues authorization = context.Request.Headers.Authorization;

        if (!StringValues.IsNullOrEmpty(authorization))
        {
            // Process authorization header(s)
            foreach (var authHeader in authorization)
            {
                await ValidateAuthorization(authHeader);
            }
        }

        // Query parameter access
        StringValues filters = context.Request.Query["filter"];

        var queryFilters = filters.ToArray();
        // Apply filters...
    }
}
```

### File Path Manipulation

```csharp
public class PathUtilities
{
    public static StringSegment GetFileName(StringSegment path)
    {
        var lastSeparator = path.LastIndexOfAny(new[] { '/', '\\' });

        if (lastSeparator >= 0)
        {
            return path.Subsegment(lastSeparator + 1);
        }

        return path;
    }

    public static StringSegment GetDirectoryName(StringSegment path)
    {
        var lastSeparator = path.LastIndexOfAny(new[] { '/', '\\' });

        if (lastSeparator >= 0)
        {
            return path.Subsegment(0, lastSeparator);
        }

        return StringSegment.Empty;
    }
}
```

## Migration from Custom Types

### Before (Custom Change Notification)
```csharp
public class CustomChangeNotifier
{
    public event Action Changed;

    public void NotifyChange()
    {
        Changed?.Invoke();
    }
}
```

### After (IChangeToken)
```csharp
public class ChangeNotifier : IChangeToken
{
    private readonly CancellationTokenSource _cts = new();

    public bool HasChanged => _cts.IsCancellationRequested;
    public bool ActiveChangeCallbacks => true;

    public IDisposable RegisterChangeCallback(Action<object> callback, object state)
    {
        return _cts.Token.Register(() => callback(state));
    }

    public void NotifyChange()
    {
        _cts.Cancel();
    }
}
```

## Best Practices

1. **Use StringValues for headers and query parameters** to handle multiple values efficiently
2. **Prefer StringSegment for parsing** to avoid string allocations
3. **Compose change tokens** for complex dependency scenarios
4. **Use CancellationChangeToken** for simple cancellation-based notifications
5. **Register callbacks with proper cleanup** to prevent memory leaks
6. **Test change token behavior** in unit tests with mock implementations

## Performance Considerations

- **StringValues reduces allocations** when dealing with single/multiple string values
- **StringSegment avoids substring allocations** during parsing operations
- **Change tokens enable efficient monitoring** without polling
- **Composite tokens allow batching** of multiple change sources

## Security Considerations

- **Validate StringSegment bounds** before accessing characters
- **Sanitize input** when using StringValues from untrusted sources
- **Use change tokens appropriately** to avoid information disclosure through timing

## Related Libraries

- **Microsoft.Extensions.Configuration**: Uses IChangeToken for reload notifications
- **Microsoft.Extensions.FileProviders**: Uses IChangeToken for file watching
- **Microsoft.Extensions.Caching**: Uses IChangeToken for cache invalidation
- **Microsoft.AspNetCore.Http**: Uses StringValues for headers and query strings

## Version History

- **10.0.1**: Latest stable version with .NET 10 support
- **8.0.0**: .NET 8 support with performance improvements
- **6.0.0**: .NET 6 support with StringSegment enhancements

This library provides the fundamental building blocks that enable efficient, reactive programming patterns throughout the B2X application, particularly in configuration management, HTTP processing, and change notification systems.