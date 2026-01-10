---
docid: KB-101
title: Microsoft.Extensions.Configuration
owner: @TechLead
status: Active
created: 2026-01-10
---

# Microsoft.Extensions.Configuration

**Version:** 10.0.1  
**NuGet:** [Microsoft.Extensions.Configuration](https://www.nuget.org/packages/Microsoft.Extensions.Configuration)  
**Source:** [dotnet/runtime](https://github.com/dotnet/runtime)  
**Documentation:** [Configuration in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration)

## Overview

Microsoft.Extensions.Configuration provides the core configuration abstraction for .NET applications. It enables reading configuration data from various sources (JSON files, environment variables, command-line arguments, etc.) and binding them to strongly-typed objects.

## Key Features

- **Unified Configuration API**: Single `IConfiguration` interface for all configuration sources
- **Multiple Providers**: JSON, XML, INI files, environment variables, command-line arguments, in-memory collections
- **Hierarchical Configuration**: Support for nested configuration structures with `:` delimiter
- **Strongly-Typed Binding**: Automatic binding to .NET objects using `ConfigurationBinder`
- **Reload Support**: Hot-reloading of configuration files when they change
- **Provider Ordering**: Last provider wins for duplicate keys

## Core Interfaces

```csharp
public interface IConfiguration
{
    string? this[string key] { get; set; }
    IEnumerable<IConfigurationSection> GetChildren();
    IConfigurationSection GetSection(string key);
    T? GetValue<T>(string key);
    T GetRequiredValue<T>(string key);
}

public interface IConfigurationBuilder
{
    IConfigurationBuilder Add(IConfigurationSource source);
    IConfigurationRoot Build();
}
```

## Configuration Providers

### JSON Configuration Provider

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Add JSON files with reload support
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// Example appsettings.json
/*
{
    "ConnectionStrings": {
        "DefaultConnection": "Server=localhost;Database=B2X;Trusted_Connection=True"
    },
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "B2X": {
        "TenantId": "default",
        "Features": {
            "Multitenancy": true,
            "Caching": false
        }
    }
}
*/
```

### Environment Variables Provider

```csharp
// Add environment variables with optional prefix
builder.Configuration.AddEnvironmentVariables(prefix: "B2X_");

// Environment variables are accessed with double underscores for hierarchy
// B2X_ConnectionStrings__DefaultConnection maps to ConnectionStrings:DefaultConnection
// B2X_B2X__TenantId maps to B2X:TenantId
```

### Command-Line Arguments Provider

```csharp
// Add command-line arguments (highest priority)
builder.Configuration.AddCommandLine(args);

// Usage: dotnet run --ConnectionStrings:DefaultConnection="Server=prod;Database=B2X"
```

### In-Memory Provider

```csharp
// Add in-memory configuration for testing or defaults
builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["DefaultConnection"] = "Server=localhost;Database=B2X",
    ["B2X:TenantId"] = "default"
});
```

## Configuration Binding

### Strongly-Typed Options

```csharp
// Define options classes
public class B2XOptions
{
    public string TenantId { get; set; } = "default";
    public bool Multitenancy { get; set; }
    public ConnectionStrings ConnectionStrings { get; set; } = new();
}

public class ConnectionStrings
{
    public string DefaultConnection { get; set; } = "";
}

// Bind configuration section to options
builder.Services.Configure<B2XOptions>(builder.Configuration.GetSection("B2X"));

// Inject and use options
public class TenantService
{
    private readonly B2XOptions _options;

    public TenantService(IOptions<B2XOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> GetTenantConnectionString()
    {
        return _options.ConnectionStrings.DefaultConnection;
    }
}
```

### Direct Value Access

```csharp
// Access configuration values directly
public class ConfigurationService
{
    private readonly IConfiguration _config;

    public ConfigurationService(IConfiguration config)
    {
        _config = config;
    }

    public string GetTenantId()
    {
        // Returns "default" if not found
        return _config["B2X:TenantId"] ?? "default";
    }

    public bool IsMultitenancyEnabled()
    {
        // Strongly-typed value retrieval
        return _config.GetValue<bool>("B2X:Features:Multitenancy");
    }

    public IEnumerable<IConfigurationSection> GetAllSections()
    {
        return _config.GetChildren();
    }
}
```

## B2X Integration Patterns

### Multitenant Configuration

```csharp
// B2X multitenant configuration setup
public static class B2XConfigurationExtensions
{
    public static IConfigurationBuilder AddB2XConfiguration(
        this IConfigurationBuilder builder,
        string tenantId)
    {
        return builder
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{tenantId}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables(prefix: $"B2X_{tenantId}_")
            .AddEnvironmentVariables() // Global environment variables
            .AddCommandLine(Environment.GetCommandLineArgs());
    }
}

// Usage in Program.cs
builder.Configuration.AddB2XConfiguration("tenant123");
```

### Database Configuration with PostgreSQL

```csharp
// PostgreSQL connection configuration
public class DatabaseOptions
{
    public string Provider { get; set; } = "PostgreSQL";
    public string ConnectionString { get; set; } = "";
    public int MaxPoolSize { get; set; } = 100;
    public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(30);
}

// Register database configuration
builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection("Database"));

// Configure Entity Framework with PostgreSQL
builder.Services.AddDbContext<B2XDbContext>((serviceProvider, options) =>
{
    var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

    options.UseNpgsql(dbOptions.ConnectionString, npgsqlOptions =>
    {
        npgsqlOptions.MaxPoolSize(dbOptions.MaxPoolSize);
        npgsqlOptions.CommandTimeout(dbOptions.CommandTimeout);
    });
});
```

### Elasticsearch Configuration

```csharp
// Elasticsearch configuration
public class ElasticsearchOptions
{
    public string[] Urls { get; set; } = Array.Empty<string>();
    public string DefaultIndex { get; set; } = "b2x";
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxRetries { get; set; } = 3;
}

// Register Elasticsearch configuration
builder.Services.Configure<ElasticsearchOptions>(
    builder.Configuration.GetSection("Elasticsearch"));

// Configure Elasticsearch client
builder.Services.AddSingleton<IElasticClient>(serviceProvider =>
{
    var esOptions = serviceProvider.GetRequiredService<IOptions<ElasticsearchOptions>>().Value;

    var settings = new ConnectionSettings(new Uri(esOptions.Urls.First()))
        .DefaultIndex(esOptions.DefaultIndex)
        .RequestTimeout(esOptions.RequestTimeout)
        .MaximumRetries(esOptions.MaxRetries);

    return new ElasticClient(settings);
});
```

### Wolverine CQRS Configuration

```csharp
// Wolverine configuration
public class WolverineOptions
{
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public int HandlerExecutionTimeoutSeconds { get; set; } = 60;
    public bool IncludeAssemblyPatterns { get; set; } = true;
}

// Register Wolverine configuration
builder.Services.Configure<WolverineOptions>(
    builder.Configuration.GetSection("Wolverine"));

// Configure Wolverine with options
builder.Services.AddWolverine(opts =>
{
    var wolverineOptions = builder.Configuration.GetSection("Wolverine").Get<WolverineOptions>();

    opts.Handlers.GlobalTimeout = wolverineOptions.DefaultTimeout;
    opts.Handlers.ExecutionTimeout = TimeSpan.FromSeconds(wolverineOptions.HandlerExecutionTimeoutSeconds);

    if (wolverineOptions.IncludeAssemblyPatterns)
    {
        opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
    }
});
```

## Advanced Patterns

### Configuration Validation

```csharp
// Configuration validation
public class B2XOptionsValidator : IValidateOptions<B2XOptions>
{
    public ValidateOptionsResult Validate(string name, B2XOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrEmpty(options.TenantId))
        {
            failures.Add("TenantId is required");
        }

        if (options.ConnectionStrings == null ||
            string.IsNullOrEmpty(options.ConnectionStrings.DefaultConnection))
        {
            failures.Add("DefaultConnection is required");
        }

        return failures.Any()
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}

// Register validator
builder.Services.AddSingleton<IValidateOptions<B2XOptions>, B2XOptionsValidator>();
```

### Configuration Reloading

```csharp
// Handle configuration changes
public class ConfigurationReloader
{
    private readonly IConfiguration _config;

    public ConfigurationReloader(IConfiguration config)
    {
        _config = config;

        // Subscribe to configuration changes
        ChangeToken.OnChange(
            () => _config.GetReloadToken(),
            () => OnConfigurationChanged());
    }

    private void OnConfigurationChanged()
    {
        // Handle configuration reload (e.g., refresh caches, reconnect services)
        Console.WriteLine("Configuration reloaded");
    }
}
```

### Custom Configuration Provider

```csharp
// Custom configuration provider for B2X tenant data
public class TenantConfigurationProvider : ConfigurationProvider
{
    private readonly string _tenantId;

    public TenantConfigurationProvider(string tenantId)
    {
        _tenantId = tenantId;
    }

    public override void Load()
    {
        // Load tenant-specific configuration from database or API
        Data = new Dictionary<string, string?>
        {
            ["Tenant:Name"] = $"Tenant {_tenantId}",
            ["Tenant:Features:Multishop"] = "true",
            ["Tenant:Features:Punchout"] = "false"
        };
    }
}

public static class TenantConfigurationExtensions
{
    public static IConfigurationBuilder AddTenantConfiguration(
        this IConfigurationBuilder builder, string tenantId)
    {
        return builder.Add(new TenantConfigurationSource(tenantId));
    }
}

// Usage
builder.Configuration.AddTenantConfiguration("tenant123");
```

## Testing Configuration

```csharp
// Unit test configuration binding
public class ConfigurationTests
{
    [Fact]
    public void ShouldBindConfigurationToOptions()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["B2X:TenantId"] = "test-tenant",
                ["B2X:Multitenancy"] = "true"
            })
            .Build();

        var options = config.GetSection("B2X").Get<B2XOptions>();

        Assert.Equal("test-tenant", options.TenantId);
        Assert.True(options.Multitenancy);
    }

    [Fact]
    public void ShouldValidateConfiguration()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["B2X:TenantId"] = "" // Invalid: empty tenant ID
            })
            .Build();

        var validator = new B2XOptionsValidator();
        var result = validator.Validate("B2X", config.GetSection("B2X").Get<B2XOptions>());

        Assert.False(result.Succeeded);
        Assert.Contains("TenantId is required", result.Failures);
    }
}
```

## Performance Considerations

- **Configuration is read-only**: Build configuration once at startup
- **Use IOptionsSnapshot for scoped updates**: When configuration needs to change per request
- **Cache configuration values**: For frequently accessed values
- **Avoid deep nesting**: Keep configuration hierarchies shallow for better performance
- **Use reload tokens**: For efficient change detection

## Security Best Practices

- **Never store secrets in configuration files**: Use Azure Key Vault or environment variables
- **Validate configuration values**: Use IValidateOptions for security-critical settings
- **Use configuration prefixes**: Isolate tenant-specific configuration
- **Encrypt sensitive values**: When storing in databases or external systems

## Migration from .NET Core to .NET 10

```csharp
// .NET Core 3.1 style
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("appsettings.local.json", optional: true);
        });

// .NET 10 style with HostApplicationBuilder
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.local.json", optional: true);
```

## Related Libraries

- **Microsoft.Extensions.Configuration.Binder**: Strongly-typed configuration binding
- **Microsoft.Extensions.Configuration.Json**: JSON file provider
- **Microsoft.Extensions.Configuration.EnvironmentVariables**: Environment variables provider
- **Microsoft.Extensions.Configuration.CommandLine**: Command-line arguments provider
- **Microsoft.Extensions.Configuration.UserSecrets**: User secrets for development

## B2X Usage Examples

### Store Gateway Configuration

```csharp
// Store gateway configuration
public class StoreGatewayOptions
{
    public string BaseUrl { get; set; } = "https://api.b2x.com/store";
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxConcurrentRequests { get; set; } = 100;
}

// Configure store gateway
builder.Services.Configure<StoreGatewayOptions>(
    builder.Configuration.GetSection("StoreGateway"));

builder.Services.AddHttpClient("StoreGateway", (serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<StoreGatewayOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.Timeout = options.Timeout;
});
```

### Admin Gateway Configuration

```csharp
// Admin gateway configuration
public class AdminGatewayOptions
{
    public string BaseUrl { get; set; } = "https://api.b2x.com/admin";
    public string ApiKey { get; set; } = "";
    public bool EnableCaching { get; set; } = true;
}

// Configure admin gateway with API key from environment
builder.Services.Configure<AdminGatewayOptions>(
    builder.Configuration.GetSection("AdminGateway"));
```

This configuration enables B2X to manage complex, multi-tenant applications with proper separation of concerns, security, and performance optimizations.</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\.ai\knowledgebase\dependency-updates\Microsoft.Extensions.Configuration.md