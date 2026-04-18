---
docid: KB-106
title: Microsoft.Extensions.DependencyInjection
owner: @Backend
status: Active
created: 2026-01-11
---

# Microsoft.Extensions.DependencyInjection

**Version:** 10.0.1  
**Package:** [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection)  
**Documentation:** [.NET Dependency Injection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)

## Overview

Microsoft.Extensions.DependencyInjection provides the core dependency injection (DI) container for .NET applications. It implements the Inversion of Control (IoC) pattern, enabling loose coupling between classes and their dependencies. The DI container manages object creation, lifetime, and disposal, promoting testable and maintainable code architecture.

The library provides a service collection (`IServiceCollection`) for registering services and a service provider (`IServiceProvider`) for resolving dependencies. It's the foundation for modern .NET application development and integrates seamlessly with other Microsoft.Extensions libraries.

## Key Features

- **Service Registration**: Flexible registration methods for different service lifetimes
- **Constructor Injection**: Automatic dependency resolution through constructor parameters
- **Service Lifetimes**: Transient, scoped, and singleton lifetime management
- **Scope Validation**: Development-time validation to prevent lifetime mismatches
- **Keyed Services**: Named service registrations for multiple implementations (.NET 8+)
- **Framework Integration**: Built-in support for framework-provided services
- **Disposable Management**: Automatic disposal of services implementing IDisposable

## Core Interfaces and Classes

### IServiceCollection
- Collection of `ServiceDescriptor` objects representing service registrations
- Methods: `AddTransient()`, `AddScoped()`, `AddSingleton()`, `BuildServiceProvider()`
- Extensible through extension methods for registering groups of related services

### IServiceProvider
- Root interface for resolving services from the container
- Methods: `GetService<T>()`, `GetRequiredService<T>()`, `GetServices<T>()`
- Thread-safe for concurrent access

### IServiceScope / IServiceScopeFactory
- `IServiceScope`: Represents a scoped lifetime container
- `IServiceScopeFactory`: Factory for creating service scopes
- Used for managing scoped service lifetimes within operations

### ServiceDescriptor
- Describes a service registration with type, implementation, and lifetime
- Properties: `ServiceType`, `ImplementationType`, `ImplementationFactory`, `Lifetime`

## Service Lifetimes

### Transient
Services created each time they're requested. Best for lightweight, stateless services.

```csharp
services.AddTransient<IMessageWriter, MessageWriter>();
```

### Scoped
Services created once per scope (typically per request in web apps). Shared within a scope but isolated between scopes.

```csharp
services.AddScoped<IOrderRepository, OrderRepository>();
```

### Singleton
Services created once for the application lifetime. Shared across all requests and scopes.

```csharp
services.AddSingleton<ICacheService, CacheService>();
```

## Basic Usage

### Service Registration

```csharp
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// Register services with different lifetimes
services.AddTransient<IMessageWriter, MessageWriter>();
services.AddScoped<IOrderRepository, OrderRepository>();
services.AddSingleton<ICacheService, CacheService>();

// Build the service provider
var serviceProvider = services.BuildServiceProvider();
```

### Constructor Injection

```csharp
public class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly ICacheService _cache;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository repository,
        ICacheService cache,
        ILogger<OrderService> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Order> GetOrderAsync(int id)
    {
        _logger.LogInformation("Retrieving order {Id}", id);
        
        var order = await _cache.GetAsync<Order>($"order:{id}");
        if (order == null)
        {
            order = await _repository.GetByIdAsync(id);
            await _cache.SetAsync($"order:{id}", order);
        }
        
        return order;
    }
}
```

### Service Resolution

```csharp
// Resolve services
var orderService = serviceProvider.GetRequiredService<OrderService>();
var messageWriter = serviceProvider.GetService<IMessageWriter>();

// Resolve multiple implementations
var writers = serviceProvider.GetServices<IMessageWriter>();
```

## B2X Integration

The B2X project extensively uses Microsoft.Extensions.DependencyInjection for service registration and dependency management across all layers.

### Program.cs Service Registration

```csharp
// src/backend/Infrastructure/Hosting/AppHost/Program.cs
using Microsoft.Extensions.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

// Register application services
builder.Services.AddB2XServices();

// Register domain services
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();

// Register infrastructure services
builder.Services.AddScoped<IOrderRepository, PostgreSqlOrderRepository>();
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// Register cross-cutting concerns
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IMessageBus, RabbitMqMessageBus>();

var host = builder.Build();
```

### CQRS Handler Registration

```csharp
// src/backend/Store/Application/DependencyInjection.cs
using Microsoft.Extensions.DependencyInjection;

namespace B2X.Store.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register command handlers
        services.AddTransient<ICommandHandler<CreateOrderCommand>, CreateOrderCommandHandler>();
        services.AddTransient<ICommandHandler<UpdateOrderCommand>, UpdateOrderCommandHandler>();
        
        // Register query handlers
        services.AddTransient<IQueryHandler<GetOrderQuery, OrderDto>, GetOrderQueryHandler>();
        services.AddTransient<IQueryHandler<GetOrdersQuery, IEnumerable<OrderDto>>, GetOrdersQueryHandler>();
        
        // Register validators
        services.AddTransient<IValidator<CreateOrderCommand>, CreateOrderCommandValidator>();
        
        return services;
    }
}
```

### Wolverine Integration

```csharp
// src/backend/Infrastructure/Messaging/WolverineConfiguration.cs
using Microsoft.Extensions.DependencyInjection;
using Wolverine;

namespace B2X.Infrastructure.Messaging;

public static class WolverineConfiguration
{
    public static IServiceCollection AddWolverineMessaging(this IServiceCollection services)
    {
        services.AddWolverine(opts =>
        {
            // Wolverine will automatically register message handlers
            // from the IServiceCollection
            opts.Discovery.IncludeAssembly(typeof(CreateOrderCommand).Assembly);
        });
        
        return services;
    }
}
```

### Multitenant Service Registration

```csharp
// src/backend/Shared/Infrastructure/Multitenancy/TenantServiceCollectionExtensions.cs
using Microsoft.Extensions.DependencyInjection;

namespace B2X.Shared.Infrastructure.Multitenancy;

public static class TenantServiceCollectionExtensions
{
    public static IServiceCollection AddTenantScoped<TService, TImplementation>(
        this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        // Register as scoped but with tenant isolation
        return services.AddScoped<TService, TImplementation>();
    }
    
    public static IServiceCollection AddTenantSingleton<TService, TImplementation>(
        this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        // Register as singleton per tenant
        return services.AddSingleton<TService, TImplementation>();
    }
}
```

### Background Service Dependencies

```csharp
// src/backend/Store/Services/Background/OrderProcessingService.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace B2X.Store.Services.Background;

public class OrderProcessingService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrderProcessingService> _logger;

    public OrderProcessingService(
        IServiceProvider serviceProvider,
        ILogger<OrderProcessingService> logger)
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
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
                
                try
                {
                    var pendingOrders = await orderService.GetPendingOrdersAsync();
                    foreach (var order in pendingOrders)
                    {
                        await ProcessOrderAsync(order, messageBus);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing orders");
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
    
    private async Task ProcessOrderAsync(Order order, IMessageBus messageBus)
    {
        // Process order logic
        await messageBus.PublishAsync(new OrderProcessedEvent(order.Id));
    }
}
```

## Advanced Patterns

### Keyed Services (.NET 8+)

```csharp
using Microsoft.Extensions.DependencyInjection;

// Register multiple implementations with keys
services.AddKeyedSingleton<IMessageWriter, ConsoleMessageWriter>("console");
services.AddKeyedSingleton<IMessageWriter, FileMessageWriter>("file");
services.AddKeyedSingleton<IMessageWriter, DatabaseMessageWriter>("database");

// Resolve specific implementation
public class NotificationService
{
    public NotificationService(
        [FromKeyedServices("console")] IMessageWriter consoleWriter,
        [FromKeyedServices("file")] IMessageWriter fileWriter)
    {
        // Use specific writers
    }
}
```

### Factory Pattern

```csharp
services.AddSingleton<IMessageWriterFactory>(sp => 
    new MessageWriterFactory(sp.GetRequiredService<ILoggerFactory>()));

public class MessageWriterFactory
{
    private readonly ILoggerFactory _loggerFactory;
    
    public MessageWriterFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }
    
    public IMessageWriter Create(string type)
    {
        return type switch
        {
            "console" => new ConsoleMessageWriter(_loggerFactory.CreateLogger<ConsoleMessageWriter>()),
            "file" => new FileMessageWriter(_loggerFactory.CreateLogger<FileMessageWriter>()),
            _ => throw new ArgumentException($"Unknown writer type: {type}")
        };
    }
}
```

### Service Decoration

```csharp
services.AddTransient<IMessageWriter>(sp =>
{
    var inner = new ConsoleMessageWriter(sp.GetRequiredService<ILogger<ConsoleMessageWriter>>());
    return new LoggingMessageWriterDecorator(inner, sp.GetRequiredService<ILogger<LoggingMessageWriterDecorator>>());
});

public class LoggingMessageWriterDecorator : IMessageWriter
{
    private readonly IMessageWriter _inner;
    private readonly ILogger<LoggingMessageWriterDecorator> _logger;
    
    public LoggingMessageWriterDecorator(IMessageWriter inner, ILogger<LoggingMessageWriterDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
    }
    
    public void Write(string message)
    {
        _logger.LogInformation("Writing message: {Message}", message);
        _inner.Write(message);
        _logger.LogInformation("Message written successfully");
    }
}
```

### Conditional Registration

```csharp
services.AddTransient<IMessageWriter>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var useDatabase = config.GetValue<bool>("UseDatabaseLogging");
    
    return useDatabase 
        ? sp.GetRequiredService<DatabaseMessageWriter>()
        : sp.GetRequiredService<ConsoleMessageWriter>();
});
```

## Testing

### Unit Testing with DI

```csharp
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class OrderServiceTests
{
    [Fact]
    public async Task GetOrderAsync_ReturnsOrderFromCache_WhenExists()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Register mocks
        var cacheMock = new Mock<ICacheService>();
        cacheMock.Setup(x => x.GetAsync<Order>("order:1"))
                .ReturnsAsync(new Order { Id = 1, Total = 100 });
        
        services.AddSingleton(cacheMock.Object);
        services.AddScoped<IOrderRepository, MockOrderRepository>();
        services.AddScoped<OrderService>();
        
        var serviceProvider = services.BuildServiceProvider();
        var orderService = serviceProvider.GetRequiredService<OrderService>();
        
        // Act
        var order = await orderService.GetOrderAsync(1);
        
        // Assert
        Assert.Equal(1, order.Id);
        Assert.Equal(100, order.Total);
    }
}
```

### Integration Testing

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class OrderProcessingIntegrationTests : IAsyncLifetime
{
    private IHost _host;
    
    public async Task InitializeAsync()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddScoped<IOrderService, OrderService>();
                services.AddScoped<IOrderRepository, TestOrderRepository>();
                services.AddSingleton<ICacheService, MemoryCacheService>();
            })
            .Build();
            
        await _host.StartAsync();
    }
    
    [Fact]
    public async Task ProcessOrder_UpdatesOrderStatus()
    {
        var orderService = _host.Services.GetRequiredService<IOrderService>();
        
        // Test logic
        var order = await orderService.CreateOrderAsync(new CreateOrderRequest());
        Assert.Equal(OrderStatus.Created, order.Status);
        
        await orderService.ProcessOrderAsync(order.Id);
        
        var updatedOrder = await orderService.GetOrderAsync(order.Id);
        Assert.Equal(OrderStatus.Processed, updatedOrder.Status);
    }
    
    public async Task DisposeAsync()
    {
        await _host.StopAsync();
        _host.Dispose();
    }
}
```

## Configuration

### Service Registration Options

```csharp
services.AddOptions<OrderProcessingOptions>()
    .Configure(options =>
    {
        options.MaxRetries = 3;
        options.Timeout = TimeSpan.FromSeconds(30);
    })
    .Validate(options => options.MaxRetries > 0, "MaxRetries must be positive")
    .ValidateOnStart();
```

### Environment-Specific Registration

```csharp
var builder = Host.CreateApplicationBuilder(args);

if (builder.Environment.IsDevelopment())
{
    services.AddSingleton<IMessageWriter, DebugMessageWriter>();
}
else
{
    services.AddSingleton<IMessageWriter, ProductionMessageWriter>();
}
```

## Scope Validation

In development environment, the DI container validates service lifetimes:

```csharp
// This will throw an exception in development
services.AddSingleton<SingletonService>();
services.AddScoped<ScopedService>();

public class SingletonService
{
    public SingletonService(ScopedService scoped) // ❌ Invalid
    {
        // Singleton cannot depend on scoped service
    }
}
```

## Related Libraries

- **Microsoft.Extensions.DependencyInjection.Abstractions**: Core DI interfaces
- **Microsoft.Extensions.Hosting**: Host builder integration
- **Microsoft.Extensions.Options**: Strongly-typed options
- **Microsoft.Extensions.Logging**: Logging services
- **Microsoft.Extensions.Configuration**: Configuration services

This library provides the foundation for B2X's modular architecture, enabling clean separation of concerns, testability, and maintainable code through dependency injection patterns.