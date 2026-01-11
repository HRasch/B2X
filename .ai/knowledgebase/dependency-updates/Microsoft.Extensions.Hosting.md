---
docid: KB-105
title: Microsoft.Extensions.Hosting
owner: @Backend
status: Active
created: 2026-01-11
---

# Microsoft.Extensions.Hosting

**Version:** 10.0.1  
**Package:** [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting)  
**Documentation:** [.NET Generic Host](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host)

## Overview

Microsoft.Extensions.Hosting provides the .NET Generic Host, a cross-platform framework for building modern applications with dependency injection, configuration, logging, and lifetime management. It serves as the foundation for console applications, worker services, and background processing applications.

The Generic Host encapsulates application resources and provides centralized control over startup, configuration, and graceful shutdown. It's the recommended approach for building long-running .NET applications outside of web contexts.

## Key Features

- **Application Lifetime Management**: Centralized control over app startup and shutdown
- **Dependency Injection Integration**: Built-in DI container with service registration
- **Configuration Management**: Hierarchical configuration from multiple sources
- **Logging Infrastructure**: Structured logging with multiple providers
- **Hosted Services**: Background services with lifecycle management
- **Graceful Shutdown**: Proper cleanup and resource disposal
- **Environment Awareness**: Development, staging, production environment support

## Core Interfaces and Classes

### IHost
- Represents the application host
- Provides access to services via `Services` property
- Methods: `StartAsync()`, `StopAsync()`, `RunAsync()`

### HostApplicationBuilder
- Modern builder for creating hosts (recommended for .NET 6+)
- Fluent API for configuring services, configuration, and logging
- Replaces the older `HostBuilder`

### IHostApplicationLifetime
- Manages application lifetime events
- Properties: `ApplicationStarted`, `ApplicationStopping`, `ApplicationStopped`
- Method: `StopApplication()` for programmatic shutdown

### IHostedService
- Interface for services that run in the background
- Methods: `StartAsync()`, `StopAsync()`
- Used for long-running operations and background tasks

### BackgroundService
- Base class for hosted services
- Implements `IHostedService` with `ExecuteAsync()` method
- Handles cancellation tokens automatically

## Basic Usage

### Creating a Host with HostApplicationBuilder

```csharp
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Add services
builder.Services.AddSingleton<IMyService, MyService>();

// Add hosted service
builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();
await host.RunAsync();
```

### Background Service Implementation

```csharp
using Microsoft.Extensions.Hosting;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
```

### Application Lifetime Events

```csharp
using Microsoft.Extensions.Hosting;

public class LifetimeEventsHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly ILogger<LifetimeEventsHostedService> _logger;

    public LifetimeEventsHostedService(
        IHostApplicationLifetime appLifetime,
        ILogger<LifetimeEventsHostedService> logger)
    {
        _appLifetime = appLifetime;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _appLifetime.ApplicationStarted.Register(OnStarted);
        _appLifetime.ApplicationStopping.Register(OnStopping);
        _appLifetime.ApplicationStopped.Register(OnStopped);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("LifetimeEventsHostedService is stopping.");
        return Task.CompletedTask;
    }

    private void OnStarted()
    {
        _logger.LogInformation("Application has started.");
    }

    private void OnStopping()
    {
        _logger.LogInformation("Application is stopping.");
    }

    private void OnStopped()
    {
        _logger.LogInformation("Application has stopped.");
    }
}
```

## B2X Integration

The B2X project uses Microsoft.Extensions.Hosting extensively for background services and worker applications:

### Store Background Services

```csharp
// src/backend/Store/Services/Background/OrderProcessingService.cs
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace B2X.Store.Services.Background;

public class OrderProcessingService : BackgroundService
{
    private readonly ILogger<OrderProcessingService> _logger;
    private readonly IOrderQueue _orderQueue;
    private readonly IOrderProcessor _orderProcessor;

    public OrderProcessingService(
        ILogger<OrderProcessingService> logger,
        IOrderQueue orderQueue,
        IOrderProcessor orderProcessor)
    {
        _logger = logger;
        _orderQueue = orderQueue;
        _orderProcessor = orderProcessor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Order Processing Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var order = await _orderQueue.DequeueAsync(stoppingToken);
                if (order != null)
                {
                    await _orderProcessor.ProcessAsync(order, stoppingToken);
                }
                else
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order");
                await Task.Delay(5000, stoppingToken); // Back off on error
            }
        }
    }
}
```

### Admin Monitoring Service

```csharp
// src/backend/Admin/Services/Monitoring/SystemHealthMonitor.cs
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace B2X.Admin.Services.Monitoring;

public class SystemHealthMonitor : BackgroundService
{
    private readonly HealthCheckService _healthCheckService;
    private readonly ILogger<SystemHealthMonitor> _logger;
    private readonly IHostApplicationLifetime _appLifetime;

    public SystemHealthMonitor(
        HealthCheckService healthCheckService,
        ILogger<SystemHealthMonitor> logger,
        IHostApplicationLifetime appLifetime)
    {
        _healthCheckService = healthCheckService;
        _logger = logger;
        _appLifetime = appLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var healthReport = await _healthCheckService.CheckHealthAsync(stoppingToken);
            
            if (healthReport.Status != HealthStatus.Healthy)
            {
                _logger.LogWarning("System health degraded: {Status}", healthReport.Status);
                
                // If critical, initiate graceful shutdown
                if (healthReport.Status == HealthStatus.Unhealthy)
                {
                    _logger.LogCritical("Critical health issues detected, shutting down");
                    _appLifetime.StopApplication();
                    break;
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
```

### Host Configuration in B2X

```csharp
// src/backend/Infrastructure/Hosting/AppHost/Program.cs
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.AddConsole();
builder.Logging.AddApplicationInsights();

// Configure services
builder.Services.AddB2XServices();
builder.Services.AddHostedService<OrderProcessingService>();
builder.Services.AddHostedService<SystemHealthMonitor>();

// Configure configuration
builder.Configuration.AddJsonFile("appsettings.json");
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddAzureAppConfiguration();

var host = builder.Build();

// Custom startup logic
var startupService = host.Services.GetRequiredService<IStartupService>();
await startupService.InitializeAsync();

await host.RunAsync();
```

## Advanced Patterns

### Custom Host Lifetime

```csharp
using Microsoft.Extensions.Hosting;

public class CustomHostLifetime : IHostLifetime
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger<CustomHostLifetime> _logger;

    public CustomHostLifetime(
        IHostApplicationLifetime applicationLifetime,
        ILogger<CustomHostLifetime> logger)
    {
        _applicationLifetime = applicationLifetime;
        _logger = logger;
    }

    public Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Waiting for custom start signal...");
        // Wait for external signal (e.g., message queue, API call)
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Custom shutdown initiated");
        return Task.CompletedTask;
    }
}
```

### Scoped Services in Background Tasks

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class ScopedBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ScopedBackgroundService> _logger;

    public ScopedBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ScopedBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedService = scope.ServiceProvider.GetRequiredService<IScopedService>();
                await scopedService.DoWorkAsync(stoppingToken);
            }

            await Task.Delay(30000, stoppingToken);
        }
    }
}
```

### Host Shutdown Handling

```csharp
using Microsoft.Extensions.Hosting;

public class GracefulShutdownService : IHostedService
{
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly ILogger<GracefulShutdownService> _logger;

    public GracefulShutdownService(
        IHostApplicationLifetime appLifetime,
        ILogger<GracefulShutdownService> logger)
    {
        _appLifetime = appLifetime;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _appLifetime.ApplicationStopping.Register(() =>
        {
            _logger.LogInformation("Application is shutting down gracefully");
            // Perform cleanup operations
            // - Flush pending messages
            // - Close connections
            // - Save state
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("GracefulShutdownService stopped");
        return Task.CompletedTask;
    }
}
```

## Testing

### Testing Hosted Services

```csharp
using Microsoft.Extensions.Hosting;
using Xunit;

public class WorkerTests
{
    [Fact]
    public async Task ExecuteAsync_ProcessesWork_WhenNotCancelled()
    {
        // Arrange
        var logger = new LoggerFactory().CreateLogger<Worker>();
        var worker = new Worker(logger);
        
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(100); // Short delay to test cancellation

        // Act
        await worker.StartAsync(cts.Token);
        
        // Wait a bit
        await Task.Delay(50);
        
        // Assert - Verify work was processed
        // (Add assertions based on your worker's behavior)
    }
}
```

### Testing with Host Builder

```csharp
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

public class HostIntegrationTests
{
    [Fact]
    public async Task Host_StartsAndRunsHostedServices()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddSingleton<IMockService, MockService>();
        builder.Services.AddHostedService<TestHostedService>();
        
        using var host = builder.Build();
        
        // Act
        await host.StartAsync();
        
        // Assert
        var mockService = host.Services.GetRequiredService<IMockService>();
        // Verify service was called during startup
        
        await host.StopAsync();
    }
}
```

## Configuration

### Host Configuration Sources

```csharp
using Microsoft.Extensions.Configuration;

var builder = Host.CreateApplicationBuilder(args);

// Add configuration sources
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .AddAzureAppConfiguration(options =>
    {
        options.Connect(builder.Configuration["ConnectionStrings:AppConfig"]);
    });
```

### Environment-Specific Settings

```csharp
var builder = Host.CreateApplicationBuilder(args);

// Environment-aware configuration
if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddConsole();
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}
else
{
    builder.Logging.AddApplicationInsights();
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
}
```

## Related Libraries

- **Microsoft.Extensions.DependencyInjection**: Core DI container
- **Microsoft.Extensions.Configuration**: Configuration management
- **Microsoft.Extensions.Logging**: Logging infrastructure
- **Microsoft.Extensions.Options**: Strongly-typed options
- **Microsoft.Extensions.Diagnostics.HealthChecks**: Health monitoring

This library provides the foundation for B2X's background processing and service orchestration, enabling reliable, scalable, and maintainable worker applications with proper lifecycle management.