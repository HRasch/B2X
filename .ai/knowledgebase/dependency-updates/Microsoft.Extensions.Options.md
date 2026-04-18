---
docid: KB-103
title: Microsoft.Extensions.Options
owner: @TechLead
status: Active
created: 2026-01-10
---

# Microsoft.Extensions.Options

**Version:** 10.0.1  
**Package:** [Microsoft.Extensions.Options](https://www.nuget.org/packages/Microsoft.Extensions.Options)  
**Documentation:** [Options pattern in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/options)

## Overview

Microsoft.Extensions.Options provides a strongly typed way of specifying and accessing settings using dependency injection. It acts as a bridge between configuration, DI, and higher-level libraries, enabling developers to get a strongly-typed view of their configuration.

The options pattern uses classes to provide strongly typed access to groups of related settings, following the Interface Segregation Principle and Separation of Concerns.

## Key Features

- **Strongly Typed Configuration**: Bind configuration sections to POCO classes
- **Dependency Injection Integration**: Register options with DI container
- **Options Validation**: Built-in validation with DataAnnotations and custom validators
- **Named Options**: Support for multiple option instances with different names
- **Change Monitoring**: IOptionsMonitor for reacting to configuration changes
- **Source Generation**: High-performance validation code generation
- **Post-Configuration**: Override or modify options after initial configuration

## Core Interfaces

### IOptions<TOptions>
- Provides access to configured TOptions instances
- Singleton lifetime - values don't change during app lifetime
- Best for options that don't need to change after startup

### IOptionsSnapshot<TOptions>
- Scoped lifetime - new instance per request
- Supports reading updated configuration values
- Useful for per-request configuration changes

### IOptionsMonitor<TOptions>
- Singleton lifetime with change notification support
- Tracks configuration changes and notifies subscribers
- Supports named options and selective invalidation

## Basic Usage

### Simple Configuration Binding

```csharp
// appsettings.json
{
  "Database": {
    "ConnectionString": "Server=localhost;Database=B2X",
    "MaxPoolSize": 100,
    "EnableRetry": true
  }
}

// Options class
public class DatabaseOptions
{
    public string ConnectionString { get; set; }
    public int MaxPoolSize { get; set; } = 100;
    public bool EnableRetry { get; set; } = true;
}

// Registration
builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection("Database"));

// Usage
public class DatabaseService
{
    private readonly DatabaseOptions _options;

    public DatabaseService(IOptions<DatabaseOptions> options)
    {
        _options = options.Value;
    }
}
```

## B2X Integration Patterns

### Multitenant Configuration

```csharp
// B2X multitenant options
public class TenantOptions
{
    public string TenantId { get; set; }
    public string ConnectionString { get; set; }
    public string ElasticsearchIndex { get; set; }
    public bool EnableCaching { get; set; }
    public TimeSpan CacheTimeout { get; set; }
}

// Named options for different tenants
builder.Services.Configure<TenantOptions>("tenant-1",
    builder.Configuration.GetSection("Tenants:Tenant1"));
builder.Services.Configure<TenantOptions>("tenant-2", 
    builder.Configuration.GetSection("Tenants:Tenant2"));

// Usage with tenant context
public class TenantService
{
    private readonly IOptionsMonitor<TenantOptions> _tenantOptions;

    public TenantService(IOptionsMonitor<TenantOptions> tenantOptions)
    {
        _tenantOptions = tenantOptions;
    }

    public TenantOptions GetTenantOptions(string tenantId)
    {
        return _tenantOptions.Get(tenantId);
    }
}
```

### Wolverine CQRS Integration

```csharp
// Wolverine handler options
public class WolverineOptions
{
    public TimeSpan MessageTimeout { get; set; } = TimeSpan.FromMinutes(5);
    public int MaxRetries { get; set; } = 3;
    public bool EnableSagaTimeouts { get; set; } = true;
}

// Configure Wolverine options
builder.Services.Configure<WolverineOptions>(
    builder.Configuration.GetSection("Wolverine"));

// Wolverine message handler
public class ProcessOrderHandler
{
    private readonly WolverineOptions _options;

    public ProcessOrderHandler(IOptions<WolverineOptions> options)
    {
        _options = options.Value;
    }

    public async Task Handle(OrderPlaced message, IMessageContext context)
    {
        // Use options in message processing
        var cts = new CancellationTokenSource(_options.MessageTimeout);
        // ... processing logic
    }
}
```

### PostgreSQL Connection Options

```csharp
public class PostgresOptions
{
    public string Host { get; set; }
    public int Port { get; set; } = 5432;
    public string Database { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int MaxPoolSize { get; set; } = 20;
    public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(30);
}

// Configure with environment-specific overrides
builder.Services.Configure<PostgresOptions>(options =>
{
    builder.Configuration.GetSection("Database").Bind(options);
    
    // Environment-specific defaults
    if (builder.Environment.IsDevelopment())
    {
        options.MaxPoolSize = 10;
    }
});
```

## Options Validation

### DataAnnotations Validation

```csharp
using System.ComponentModel.DataAnnotations;

public class EmailOptions
{
    public const string SectionName = "Email";

    [Required]
    [EmailAddress]
    public string SmtpServer { get; set; }

    [Range(1, 65535)]
    public int Port { get; set; } = 587;

    [Required]
    [EmailAddress]
    public string FromAddress { get; set; }

    [Required]
    public string ApiKey { get; set; }
}

// Registration with validation
builder.Services.AddOptions<EmailOptions>()
    .Bind(builder.Configuration.GetSection(EmailOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();
```

### Custom Validation with IValidateOptions

```csharp
public class CustomValidation : IValidateOptions<EmailOptions>
{
    public ValidateOptionsResult Validate(string name, EmailOptions options)
    {
        var errors = new List<string>();

        if (options.Port == 25 && string.IsNullOrEmpty(options.ApiKey))
        {
            errors.Add("API key is required when using port 25");
        }

        if (options.SmtpServer.Contains("localhost") && 
            builder.Environment.IsProduction())
        {
            errors.Add("Cannot use localhost SMTP in production");
        }

        return errors.Any() 
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}

// Register custom validator
builder.Services.AddSingleton<IValidateOptions<EmailOptions>, CustomValidation>();
```

### Source-Generated Validation (C# 11+)

```csharp
[OptionsValidator]
public partial class EmailOptionsValidator : IValidateOptions<EmailOptions>
{
    // Implementation generated at compile-time
    // No manual validation code needed!
}

// Usage
builder.Services.AddSingleton<IValidateOptions<EmailOptions>, EmailOptionsValidator>();
```

## Change Monitoring

### IOptionsMonitor for Dynamic Configuration

```csharp
public class ConfigurationMonitor
{
    private readonly IOptionsMonitor<DatabaseOptions> _monitor;
    private readonly ILogger<ConfigurationMonitor> _logger;

    public ConfigurationMonitor(
        IOptionsMonitor<DatabaseOptions> monitor,
        ILogger<ConfigurationMonitor> logger)
    {
        _monitor = monitor;
        _logger = logger;

        // Subscribe to changes
        _monitor.OnChange(OnDatabaseOptionsChanged);
    }

    private void OnDatabaseOptionsChanged(DatabaseOptions options, string name)
    {
        _logger.LogInformation(
            "Database options changed for {Name}: MaxPoolSize={MaxPoolSize}", 
            name, options.MaxPoolSize);
    }

    public DatabaseOptions GetCurrentOptions() => _monitor.CurrentValue;
}
```

## Advanced Patterns

### Options Factory with Dependencies

```csharp
public class DatabaseOptionsFactory : IConfigureOptions<DatabaseOptions>
{
    private readonly IConfiguration _config;
    private readonly IHostEnvironment _env;

    public DatabaseOptionsFactory(IConfiguration config, IHostEnvironment env)
    {
        _config = config;
        _env = env;
    }

    public void Configure(DatabaseOptions options)
    {
        _config.GetSection("Database").Bind(options);
        
        // Environment-specific configuration
        if (_env.IsDevelopment())
        {
            options.ConnectionString = 
                options.ConnectionString.Replace("prod-db", "dev-db");
        }
    }
}

// Registration
builder.Services.ConfigureOptions<DatabaseOptionsFactory>();
```

### Named Options with Post-Configuration

```csharp
// Configure multiple instances
builder.Services.Configure<CacheOptions>("Memory", options =>
{
    options.SizeLimit = 1024;
    options.DefaultExpiration = TimeSpan.FromMinutes(5);
});

builder.Services.Configure<CacheOptions>("Redis", options =>
{
    options.ConnectionString = "redis:6379";
    options.DefaultExpiration = TimeSpan.FromHours(1);
});

// Post-configure all instances
builder.Services.PostConfigureAll<CacheOptions>(options =>
{
    options.CompressionEnabled = true;
});
```

## B2X-Specific Usage Examples

### ERP Connector Options

```csharp
public class ErpConnectorOptions
{
    public string BaseUrl { get; set; }
    public string ApiKey { get; set; }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxRetries { get; set; } = 3;
    public bool EnableLogging { get; set; } = true;
}

// Configure for different ERP systems
builder.Services.Configure<ErpConnectorOptions>("enventa",
    builder.Configuration.GetSection("ErpConnectors:Enventa"));
builder.Services.Configure<ErpConnectorOptions>("sap",
    builder.Configuration.GetSection("ErpConnectors:SAP"));
```

### Catalog Import Options

```csharp
public class CatalogImportOptions
{
    public int BatchSize { get; set; } = 1000;
    public TimeSpan ImportTimeout { get; set; } = TimeSpan.FromHours(2);
    public bool EnableValidation { get; set; } = true;
    public string TempDirectory { get; set; }
    public long MaxFileSizeBytes { get; set; } = 100 * 1024 * 1024; // 100MB
}

// With validation
builder.Services.AddOptions<CatalogImportOptions>()
    .Bind(builder.Configuration.GetSection("CatalogImport"))
    .Validate(options => options.BatchSize > 0, "BatchSize must be positive")
    .Validate(options => options.MaxFileSizeBytes > 0, "MaxFileSizeBytes must be positive")
    .ValidateOnStart();
```

## Best Practices

1. **Use IOptions<TOptions>** for simple, unchanging configuration
2. **Use IOptionsSnapshot<TOptions>** when options need to change per request
3. **Use IOptionsMonitor<TOptions>** for singleton services that need change notifications
4. **Validate options** at startup to catch configuration errors early
5. **Use named options** for multiple configurations of the same type
6. **Consider source-generated validation** for better performance
7. **Use post-configuration** sparingly and document when used

## Migration from .NET Core to .NET 10

- **Source Generation**: New OptionsValidator attribute for compile-time validation
- **ValidateOnStart**: New method to validate options during application startup
- **Enhanced Monitoring**: Improved change notification performance
- **Better DI Integration**: Simplified registration patterns

## Related Libraries

- **Microsoft.Extensions.Configuration**: For configuration sources
- **Microsoft.Extensions.Logging**: For logging within options validation
- **Microsoft.Extensions.Caching**: Often configured via options
- **Microsoft.Extensions.Http**: Uses options for HttpClient configuration

## Troubleshooting

### Common Issues

1. **Options not binding**: Check configuration section names match
2. **Validation failures**: Use ValidateOnStart() to catch at startup
3. **Change notifications not working**: Ensure file-based configuration providers
4. **Named options not resolving**: Verify exact name matching

### Debug Configuration

```csharp
// Log current options values
public class DebugService
{
    public DebugService(IOptionsMonitor<DatabaseOptions> monitor)
    {
        monitor.OnChange((options, name) =>
        {
            Console.WriteLine($"Options changed: {name}");
            Console.WriteLine($"Connection: {options.ConnectionString}");
        });
    }
}
```

This library provides the foundation for strongly-typed configuration in .NET applications, enabling clean separation of concerns and robust validation patterns essential for enterprise applications like B2X.