# B2Connect CQRS Implementation Plan (Wolverine Edition)
## Skalierung für mehrere Millionen Produkte pro Tenant

**Version:** 2.0 - Wolverine Optimiert  
**Datum:** 26. Dezember 2025  
**Szenario:** Multi-Millionen Produkte/Tenant  
**Framework:** Wolverine (Message Bus + CQRS Mediator Pattern)

---

## 📊 Skalierungs-Anforderungen

### Annahmen
- **5-10 Millionen Produkte** pro großem Tenant
- **10.000+ Tenants**
- **10.000+ Requests/Sekunde** Peak Load
- **99.9% Verfügbarkeit** erforderlich
- **Datenbank-Unabhängigkeit** (PostgreSQL, aber auch andere)

### Probleme mit Current Architecture
```
❌ Synchrone Read/Write Models
   → Queries blockieren bei großen Datasets
   
❌ Single Database
   → Alle Queries konkurrieren um Ressourcen
   
❌ No Denormalization
   → JOIN-Queries auf Millionen Produkte ineffizient
   
❌ Real-time Constraints
   → Keine Event Sourcing für Audit Trail
   
❌ Limited Search Capabilities
   → SQL LIKE queries nicht skalierbar
```

---

## 🏗️ CQRS Architektur für Millionen-Scale

### 1. Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                         CLIENT LAYER                         │
├─────────────────────────────────────────────────────────────┤
│ Frontend / Admin Panel / Mobile Apps                         │
└──────────┬──────────────────┬──────────────────┬─────────────┘
           │                  │                  │
     ┌─────▼──────┐    ┌──────▼────────┐  ┌─────▼──────┐
     │  WRITE     │    │  READ         │  │  SEARCH    │
     │  COMMANDS  │    │  QUERIES      │  │  QUERIES   │
     └─────┬──────┘    └──────┬────────┘  └─────┬──────┘
           │                  │                  │
     ┌─────▼──────────────────▼──────────────────▼──────┐
     │           API GATEWAY / ROUTER                    │
     │  (Route Commands to Write, Queries to Read Store) │
     └─────┬──────────────────┬──────────────────┬──────┘
           │                  │                  │
    ┌──────▼────────┐  ┌──────▼────────┐  ┌────▼────────┐
    │  WRITE MODEL  │  │  READ MODEL   │  │ SEARCH INDEX│
    │               │  │               │  │             │
    │ Command Store │  │ Denormalized  │  │ ElasticSearch│
    │ (PostgreSQL)  │  │ Data (Redis,  │  │ / OpenSearch│
    │               │  │ PostgreSQL)   │  │             │
    └──────┬────────┘  └──────────────┘  └────────────┘
           │
    ┌──────▼──────────────────────┐
    │  EVENT STREAM (Log)          │
    │  (PostgreSQL / EventStore)   │
    └──────┬──────────────────────┘
           │
      ┌────▼─────┬──────────┬──────────┐
      │           │          │          │
    ┌─▼──┐   ┌──▼──┐    ┌──▼──┐   ┌──▼──┐
    │ M1 │   │ M2  │    │ M3  │   │ ... │
    └────┘   └─────┘    └─────┘   └─────┘
    
    Materialized Views / Read Projections
```

---

## 🎯 Phase 1: Foundation (Weeks 1-3)

### 1.1 Infrastructure Setup - Wolverine Configuration

#### Project Structure
```
📁 backend/services/CatalogService/
├── 📁 Commands/
│   ├── CreateProductCommand.cs
│   ├── UpdateProductCommand.cs
│   ├── DeleteProductCommand.cs
│   └── BulkImportProductsCommand.cs
├── 📁 Queries/
│   ├── GetProductByIdQuery.cs
│   ├── GetProductsPagedQuery.cs
│   ├── SearchProductsQuery.cs
│   └── GetCatalogStatsQuery.cs
├── 📁 Handlers/
│   ├── Commands/
│   │   ├── CreateProductCommandHandler.cs
│   │   ├── UpdateProductCommandHandler.cs
│   │   └── BulkImportProductsCommandHandler.cs
│   ├── Queries/
│   │   ├── GetProductByIdQueryHandler.cs
│   │   ├── GetProductsPagedQueryHandler.cs
│   │   └── SearchProductsQueryHandler.cs
│   └── Events/
│       ├── ProductCreatedEventHandler.cs
│       └── ProductUpdatedEventHandler.cs
├── 📁 Events/
│   ├── ProductCreatedEvent.cs
│   ├── ProductUpdatedEvent.cs
│   └── DomainEvents.cs
└── 📁 Behaviors/
    ├── ValidationBehavior.cs
    ├── TransactionBehavior.cs
    └── LoggingBehavior.cs
```

#### Dependencies Already Available
```xml
<!-- Wolverine ist bereits installiert -->
<PackageReference Include="Wolverine" Version="*" />
<PackageReference Include="Wolverine.Http" Version="*" />

<!-- Zusätzliche Pakete -->
<PackageReference Include="Dapper" Version="*" />
<PackageReference Include="AutoMapper" Version="*" />
```

#### Wolverine Service Registration
```csharp
// Program.cs - Katalog Service
var builder = WebApplication.CreateBuilder(args);

// Add Wolverine with CQRS pattern support
builder.Services.AddWolverine(opts =>
{
    // Handler discovery - automatisch alle ICommandHandler<> und IEventHandler<> finden
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
    
    // Message storage für Reliability
    opts.UseInMemoryTransport();  // Development
    
    // Configure failure handling
    opts.Handlers.OnException<ValidationException>()
        .Discard();  // Validation errors don't retry
    
    opts.Handlers.OnException<TransientException>()
        .Retry.MaximumAttempts(3)
        .WithDelayInSeconds(1, 2, 5);  // Exponential backoff
    
    opts.Handlers.OnException<Exception>()
        .MoveToDeadLetterQueue();  // Permanent failures to DLQ
});

// Service registration
builder.Services.AddScoped<CatalogDbContext>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();
app.UseWolverine();
```

---

## 🎯 Phase 2: Write Model - Commands (Weeks 2-4)

### 2.1 Command Architecture mit Wolverine

#### Command Base Classes
```csharp
// backend/shared/cqrs/Commands.cs
using Wolverine;

/// <summary>
/// Base command marker interface
/// Wolverine automatically discovers and routes ICommand handlers
/// </summary>
public interface ICommand { }

/// <summary>
/// Command with response
/// Wolverine supports Request/Reply pattern: awaitable commands
/// </summary>
public interface ICommand<out TResponse> : ICommand { }

public abstract class Command : ICommand
{
    public Guid TenantId { get; set; }
    public string? CorrelationId { get; set; } = Guid.NewGuid().ToString();
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; }
}

public class CommandResult
{
    public Guid Id { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public IEnumerable<string>? Errors { get; set; }
    
    public static CommandResult Ok(Guid id) => 
        new() { Id = id, Success = true };
    
    public static CommandResult Fail(string error) =>
        new() { Success = false, ErrorMessage = error };
}
```

#### Example: CreateProductCommand
```csharp
// Commands/CreateProductCommand.cs
public class CreateProductCommand : ICommand<CommandResult>
{
    public Guid TenantId { get; set; }
    public required string Sku { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }
    public string[]? CategoryIds { get; set; }
    public Dictionary<string, string>? LocalizedData { get; set; }
}

// Validators/CreateProductCommandValidator.cs
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IProductRepository _repository;
    
    public CreateProductCommandValidator(IProductRepository repository)
    {
        _repository = repository;
        
        RuleFor(c => c.Sku)
            .NotEmpty()
            .MaximumLength(50)
            .MustAsync(async (sku, tenantId, ct) => 
                !await _repository.ExistsAsync(sku, ct))
            .WithMessage("SKU already exists");
            
        RuleFor(c => c.Price)
            .GreaterThan(0)
            .PrecisionScale(10, 2);
    }
}
```

#### Wolverine Command Handler Pattern
```csharp
// Handlers/Commands/CreateProductCommandHandler.cs
using Wolverine;

/// <summary>
/// Wolverine automatically discovers this handler because:
/// 1. Class name ends with "Handler"
/// 2. Implements ICommandHandler<CreateProductCommand>
/// 3. Handle() method signature matches convention
/// 
/// Wolverine will invoke this when CreateProductCommand is published/sent
/// </summary>
public class CreateProductCommandHandler : 
    ICommandHandler<CreateProductCommand>
{
    private readonly CatalogDbContext _context;
    private readonly IProductRepository _repository;
    private readonly IMessageBus _messageBus;
    private readonly IValidator<CreateProductCommand> _validator;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        CatalogDbContext context,
        IProductRepository repository,
        IMessageBus messageBus,
        IValidator<CreateProductCommand> validator,
        ILogger<CreateProductCommandHandler> logger)
    {
        _context = context;
        _repository = repository;
        _messageBus = messageBus;
        _validator = validator;
        _logger = logger;
    }

    /// <summary>
    /// Wolverine invokes this method when command is sent
    /// Exceptions are automatically handled by configured error policies
    /// </summary>
    public async Task<CommandResult> Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate command
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                _logger.LogWarning(
                    "Validation failed for CreateProductCommand. Errors: {Errors}",
                    string.Join("; ", errors));
                
                return CommandResult.Fail(string.Join("; ", errors));
            }

            // 2. Create aggregate
            var product = Product.Create(
                sku: command.Sku,
                name: command.Name,
                price: command.Price,
                tenantId: command.TenantId
            );

            // 3. Add domain event
            product.AddDomainEvent(new ProductCreatedEvent
            {
                ProductId = product.Id,
                Sku = command.Sku,
                Name = command.Name,
                Price = command.Price,
                TenantId = command.TenantId,
                UserId = command.UserId,
                Timestamp = DateTime.UtcNow
            });

            // 4. Save to database
            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            // 5. Publish domain events via Wolverine message bus
            // Wolverine will handle transactional outbox automatically
            foreach (var evt in product.DomainEvents)
            {
                await _messageBus.PublishAsync(evt, cancellation: cancellationToken);
            }

            _logger.LogInformation(
                "Product created: {Sku} (ID: {ProductId}, Tenant: {TenantId})",
                command.Sku, product.Id, command.TenantId);

            return CommandResult.Ok(product.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product {Sku}", command.Sku);
            throw;  // Let Wolverine's error handling take over
        }
    }
}
```

### 2.2 Wolverine Middleware Behaviors

#### Validation Middleware
```csharp
// Middleware/ValidationMiddleware.cs
using Wolverine;
using Wolverine.Middleware;

/// <summary>
/// Wolverine middleware for automatic command validation
/// Runs before any command handler
/// </summary>
public class ValidationMiddleware
{
    private readonly ILogger<ValidationMiddleware> _logger;

    public ValidationMiddleware(ILogger<ValidationMiddleware> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Before() - runs before the handler
    /// Wolverine inspects method signature and injects what's needed
    /// </summary>
    public async Task Before(
        ICommand command,
        IServiceProvider services,
        IInvokeChain chain)
    {
        // Get validator for this command type
        var validatorType = typeof(IValidator<>)
            .MakeGenericType(command.GetType());
        var validator = services.GetService(validatorType);

        if (validator != null)
        {
            var method = validatorType
                .GetMethod("ValidateAsync");
            
            var result = (dynamic)await method!.InvokeAsync(
                validator, 
                new object[] { command, CancellationToken.None });

            if (!result.IsValid)
            {
                var errors = ((IEnumerable<dynamic>)result.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                _logger.LogWarning(
                    "Command validation failed: {Errors}",
                    string.Join("; ", errors));
                
                throw new ValidationException(errors);
            }
        }

        // Continue to handler
        await chain.InvokeAsync();
    }

    /// <summary>
    /// After() - runs after the handler completes
    /// </summary>
    public void After(
        ICommand command,
        ILogger<ValidationMiddleware> logger)
    {
        logger.LogDebug("Command {CommandType} processed successfully", 
            command.GetType().Name);
    }

    /// <summary>
    /// Handles exceptions - Wolverine calls this if handler throws
    /// </summary>
    public async Task OnException(
        Exception ex,
        ILogger<ValidationMiddleware> logger)
    {
        logger.LogError(ex, "Exception in command handler");
        await Task.CompletedTask;
    }
}
```

#### Registration in Program.cs
```csharp
// Program.cs
builder.Services.AddWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
    
    // Apply middleware to all handlers
    opts.Policies.Add<ValidationMiddleware>();
    opts.Policies.Add<LoggingMiddleware>();
    
    // Or apply to specific handler types
    opts.HandlerGraph
        .Where(h => h.MessageType.Namespace!.Contains("Commands"))
        .ForEach(h => h.Middleware.Add(new ValidationMiddleware(_logger)));
});
```

---

## 📖 Phase 3: Read Model & Query Handlers (Weeks 3-5)

### 3.1 Wolverine Query Pattern

#### Query Base Classes
```csharp
// backend/shared/cqrs/Queries.cs
using Wolverine;

/// <summary>
/// Base query interface
/// Wolverine treats queries as synchronous request/reply pattern
/// Handler returns TResponse directly (not wrapped in Command Result)
/// </summary>
public interface IQuery<out TResponse> { }

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
}
```

#### Example: GetProductsPagedQuery
```csharp
// Queries/GetProductsPagedQuery.cs
public class GetProductsPagedQuery : IQuery<PagedResult<ProductReadModel>>
{
    public Guid TenantId { get; set; }
    public string? SearchTerm { get; set; }
    public string[]? CategoryNames { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "name"; // name, price, newest, rating
}
```

### 3.2 Denormalized Read Model Schema

```sql
-- Optimized for millions of products with denormalized data
CREATE TABLE IF NOT EXISTS products_read_model (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL,
    sku VARCHAR(50) NOT NULL,
    name VARCHAR(255) NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    discounted_price DECIMAL(10,2),
    
    -- Denormalized fields (no joins!)
    category_names TEXT[] NOT NULL DEFAULT '{}',
    brand_name VARCHAR(100),
    stock_quantity INT NOT NULL DEFAULT 0,
    is_available BOOLEAN NOT NULL DEFAULT false,
    
    -- Search optimization
    search_text TEXT,
    tags TEXT[] NOT NULL DEFAULT '{}',
    
    -- Metadata
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    rating INT DEFAULT 0,
    review_count INT DEFAULT 0,
    version INT NOT NULL DEFAULT 1
);

-- CRITICAL INDEXES for 10+ million products
CREATE INDEX idx_products_read_tenant ON products_read_model(tenant_id);
CREATE INDEX idx_products_read_tenant_sku ON products_read_model(tenant_id, sku);
CREATE INDEX idx_products_read_tenant_available 
    ON products_read_model(tenant_id) 
    WHERE is_available = true;
CREATE INDEX idx_products_read_created 
    ON products_read_model(tenant_id, created_at DESC);
CREATE INDEX idx_products_read_search 
    ON products_read_model USING GIN(search_text);
CREATE INDEX idx_products_read_category_price 
    ON products_read_model(tenant_id, category_names, price)
    WHERE is_available = true;
```

### 3.3 Wolverine Query Handlers

```csharp
// Handlers/Queries/GetProductsPagedQueryHandler.cs
using Wolverine;

/// <summary>
/// Wolverine recognizes this as Query handler because:
/// 1. Implements IQueryHandler<GetProductsPagedQuery, PagedResult<ProductReadModel>>
/// 2. Handle() method is discovered automatically
/// 
/// Queries are SYNCHRONOUS - awaited by caller, bypasses message bus
/// This is perfect for read operations that need immediate response
/// </summary>
public class GetProductsPagedQueryHandler : 
    IQueryHandler<GetProductsPagedQuery, PagedResult<ProductReadModel>>
{
    private readonly CatalogReadDbContext _readContext;
    private readonly IDistributedCache _cache;
    private readonly ILogger<GetProductsPagedQueryHandler> _logger;

    public GetProductsPagedQueryHandler(
        CatalogReadDbContext readContext,
        IDistributedCache cache,
        ILogger<GetProductsPagedQueryHandler> logger)
    {
        _readContext = readContext;
        _cache = cache;
        _logger = logger;
    }

    public async Task<PagedResult<ProductReadModel>> Handle(
        GetProductsPagedQuery query,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            // Check cache first for common queries
            var cacheKey = GenerateCacheKey(query);
            var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
            
            if (!string.IsNullOrEmpty(cached))
            {
                sw.Stop();
                _logger.LogInformation(
                    "Query from cache in {ElapsedMs}ms",
                    sw.ElapsedMilliseconds);
                
                return JsonSerializer.Deserialize<PagedResult<ProductReadModel>>(cached)!;
            }

            // Build query
            var dbQuery = _readContext.ProductsReadModel
                .AsNoTracking()
                .Where(p => p.TenantId == query.TenantId);

            // Apply filters
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                // PostgreSQL full-text search
                dbQuery = dbQuery.Where(p => 
                    EF.Functions.Match(p.SearchText, query.SearchTerm));
            }

            if (query.CategoryNames?.Any() == true)
            {
                dbQuery = dbQuery.Where(p => 
                    query.CategoryNames.Any(c => p.CategoryNames.Contains(c)));
            }

            if (query.MinPrice.HasValue)
            {
                dbQuery = dbQuery.Where(p => p.Price >= query.MinPrice);
            }

            if (query.MaxPrice.HasValue)
            {
                dbQuery = dbQuery.Where(p => p.Price <= query.MaxPrice);
            }

            // Count before pagination
            var totalCount = await dbQuery.CountAsync(cancellationToken);

            // Apply sorting & pagination
            var items = await dbQuery
                .OrderBy(p => query.SortBy)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            var result = new PagedResult<ProductReadModel>
            {
                Items = items,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };

            // Cache for 5 minutes (moderate cache key variations)
            if (query.PageNumber == 1)  // Cache first page more aggressively
            {
                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(result),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    },
                    cancellationToken);
            }

            sw.Stop();
            _logger.LogInformation(
                "Query executed in {ElapsedMs}ms. Tenant: {TenantId}, " +
                "Total: {Total}, Returned: {Count}",
                sw.ElapsedMilliseconds, query.TenantId, totalCount, items.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing GetProductsPagedQuery");
            throw;
        }
    }

    private static string GenerateCacheKey(GetProductsPagedQuery query)
    {
        var hash = Convert.ToBase64String(
            System.Security.Cryptography.SHA256.HashData(
                Encoding.UTF8.GetBytes(
                    $"{query.TenantId}|{query.SearchTerm}|" +
                    $"{string.Join(",", query.CategoryNames ?? [])}|" +
                    $"{query.MinPrice}|{query.MaxPrice}|" +
                    $"{query.SortBy}|{query.PageNumber}|{query.PageSize}")));
        
        return $"products:query:{hash}";
    }
}
```

### 3.4 Event Handlers - Read Model Updates

```csharp
// Handlers/Events/ProductCreatedEventHandler.cs
using Wolverine;

/// <summary>
/// Wolverine Event Handler for async event processing
/// Automatically invoked when ProductCreatedEvent is published
/// Multiple handlers can subscribe to same event
/// </summary>
public class ProductCreatedEventHandler : 
    IEventHandler<ProductCreatedEvent>
{
    private readonly CatalogReadDbContext _readContext;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ProductCreatedEventHandler> _logger;

    public async Task Handle(
        ProductCreatedEvent @event,
        CancellationToken cancellationToken)
    {
        try
        {
            // Create denormalized read model
            var readModel = new ProductReadModel
            {
                Id = @event.ProductId,
                TenantId = @event.TenantId,
                Sku = @event.Sku,
                Name = @event.Name,
                Price = @event.Price,
                IsAvailable = true,
                CreatedAt = @event.Timestamp,
                UpdatedAt = @event.Timestamp,
                SearchText = $"{@event.Sku} {@event.Name}"
            };

            _readContext.ProductsReadModel.Add(readModel);
            await _readContext.SaveChangesAsync(cancellationToken);

            // Invalidate cache
            await InvalidateCacheAsync(
                @event.TenantId, 
                @event.ProductId, 
                cancellationToken);

            _logger.LogInformation(
                "Read model updated for Product {ProductId}",
                @event.ProductId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling ProductCreatedEvent");
            throw;  // Wolverine will retry or move to DLQ
        }
    }

    private async Task InvalidateCacheAsync(
        Guid tenantId,
        Guid productId,
        CancellationToken cancellationToken)
    {
        // Invalidate specific product cache
        var cacheKey = $"product:{tenantId}:{productId}";
        await _cache.RemoveAsync(cacheKey, cancellationToken);

        // Invalidate list caches (if using Redis for pattern support)
        // This is handled by cache key pattern matching in Redis
    }
}
```

---

## 🔍 Phase 4: Search & Aggregation (Weeks 4-6)

### 4.1 ElasticSearch Integration for Full-Text Search

#### Problem: Text Search mit Millionen Produkten
```
❌ PostgreSQL LIKE/ILIKE auf Millionen Produkten = 5-10 Sekunden
❌ Fuzzy search (typos) nicht möglich
❌ Faceted search langsam
```

#### Solution: ElasticSearch Index
```csharp
// Services/ElasticSearchService.cs
public class ElasticSearchService : ISearchService
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<ElasticSearchService> _logger;

    public async Task IndexProductAsync(ProductReadModel product)
    {
        var document = new ProductSearchDocument
        {
            Id = product.Id,
            TenantId = product.TenantId,
            Sku = product.Sku,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Categories = product.CategoryNames,
            Brand = product.BrandName,
            Tags = product.Tags,
            IsAvailable = product.IsAvailable,
            Rating = product.Rating
        };

        var response = await _elasticClient.IndexAsync(
            document,
            idx => idx.Index("products")
        );

        if (!response.IsValid)
        {
            _logger.LogError("Failed to index product: {Error}", 
                response.ServerError?.Error?.Reason);
        }
    }

    public async Task<SearchResults> SearchAsync(SearchQuery query)
    {
        var searchRequest = new SearchRequest("products")
        {
            Query = new BoolQuery
            {
                Must = new List<Query>
                {
                    new TermQuery { Field = "tenantId", Value = query.TenantId }
                },
                Should = new List<Query>
                {
                    // Multi-match with different field boosts
                    new MultiMatchQuery
                    {
                        Query = query.SearchTerm,
                        Fields = new[] { "name^3", "description", "tags" },
                        Fuzziness = Fuzziness.Auto // Handle typos
                    }
                }
            },
            Aggregations = new AggregationDictionary
            {
                // Faceted search: categories, brands, price ranges
                { "categories", new TermsAggregation("categories") { Field = "categories.keyword" } },
                { "brands", new TermsAggregation("brands") { Field = "brand.keyword" } },
                { "price_ranges", new RangeAggregation("price") 
                    { 
                        Ranges = new[]
                        {
                            new AggregationRange { To = 50 },
                            new AggregationRange { From = 50, To = 100 },
                            new AggregationRange { From = 100 }
                        }
                    }
                }
            },
            Size = query.PageSize,
            From = (query.PageNumber - 1) * query.PageSize
        };

        var response = await _elasticClient.SearchAsync<ProductSearchDocument>(
            _ => searchRequest
        );

        return new SearchResults
        {
            Products = response.Documents.Select(d => new ProductDto(d)).ToList(),
            Facets = response.Aggregations.ToDictionary(
                a => a.Key,
                a => a.Value as IAggregation
            ),
            Total = response.Total
        };
    }
}
```

#### ElasticSearch Mapping
```csharp
// Startup Configuration
services.AddElasticsearch(new Uri("http://localhost:9200"))
    .SetDefaultIndex("products");

// In AppHost startup:
var indexResponse = await elasticClient.Indices.CreateAsync("products", 
    c => c
        .Settings(s => s
            .NumberOfShards(10)
            .NumberOfReplicas(1)
            .RefreshInterval("30s") // Balance between search freshness and indexing performance
        )
        .Mappings(m => m
            .Properties<ProductSearchDocument>(p => p
                .Keyword(k => k.Name(f => f.TenantId))
                .Text(t => t
                    .Name(f => f.Name)
                    .Analyzer("standard")
                    .Fields(f => f.Keyword(k => k.Name("keyword")))
                )
                .Text(t => t
                    .Name(f => f.Description)
                    .Analyzer("standard")
                )
                .Keyword(k => k.Name(f => f.Categories))
                .Keyword(k => k.Name(f => f.Tags))
                .Numeric(n => n.Name(f => f.Price))
            )
        )
);
```

---

## 📈 Phase 5: Performance Optimization (Weeks 5-7)

### 5.1 Caching Strategy

```csharp
// Services/CachedQueryHandler.cs
public class GetProductByIdQuery : IRequest<ProductReadModel>
{
    public Guid TenantId { get; set; }
    public Guid ProductId { get; set; }
}

[Cached(DurationSeconds = 3600)]
public class GetProductByIdQueryHandler : 
    IRequestHandler<GetProductByIdQuery, ProductReadModel>
{
    private readonly CatalogReadDbContext _context;
    private readonly IDistributedCache _cache;

    public async Task<ProductReadModel> Handle(
        GetProductByIdQuery query,
        CancellationToken cancellationToken)
    {
        // Try cache first
        var cacheKey = $"product:{query.TenantId}:{query.ProductId}";
        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        
        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<ProductReadModel>(cached)!;
        }

        // Cache miss: query database
        var product = await _context.ProductsReadModel
            .AsNoTracking()
            .FirstOrDefaultAsync(
                p => p.Id == query.ProductId && p.TenantId == query.TenantId,
                cancellationToken);

        if (product != null)
        {
            // Cache for 1 hour
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(product),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                },
                cancellationToken);
        }

        return product!;
    }
}
```

### 5.2 Cache Invalidation on Events

```csharp
// EventHandlers/CacheInvalidationHandler.cs
public class ProductUpdatedEventHandler : 
    IEventHandler<ProductUpdatedEvent>
{
    private readonly IDistributedCache _cache;

    public async Task Handle(
        ProductUpdatedEvent @event,
        CancellationToken cancellationToken)
    {
        // Invalidate product cache
        var cacheKey = $"product:{@event.TenantId}:{@event.ProductId}";
        await _cache.RemoveAsync(cacheKey, cancellationToken);

        // Invalidate list caches (by tenant)
        var listPattern = $"products:list:{@event.TenantId}:*";
        // Note: Use Redis for pattern-based invalidation
    }
}
```

### 5.3 Read Model Batch Updates

```csharp
// Services/BatchReadModelUpdateService.cs
public class BatchReadModelUpdateService
{
    private readonly CatalogReadDbContext _context;
    private const int BatchSize = 1000;

    public async Task RebuildReadModelAsync(
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        var writeContext = new CatalogDbContext();
        
        // Clear old read model
        await _context.ProductsReadModel
            .Where(p => p.TenantId == tenantId)
            .ExecuteDeleteAsync(cancellationToken);

        // Rebuild in batches
        var products = writeContext.Products
            .Where(p => p.TenantId == tenantId)
            .AsNoTracking();

        var pageCount = 0;
        await foreach (var batch in products.Batch(BatchSize, cancellationToken))
        {
            var readModels = batch.Select(p => MapToReadModel(p)).ToList();
            await _context.AddRangeAsync(readModels, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            
            pageCount++;
            Console.WriteLine($"Rebuilt {pageCount * BatchSize} products...");
        }
    }

    private ProductReadModel MapToReadModel(Product p)
    {
        return new ProductReadModel
        {
            Id = p.Id,
            TenantId = p.TenantId,
            Sku = p.Sku,
            Name = p.Name,
            // ... mapped fields
        };
    }
}
```

---

## 🚀 Phase 6: API Controllers (Week 6)

### 6.1 Command Endpoints

```csharp
// Controllers/ProductsCommandController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsCommandController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsCommandController> _logger;

    [HttpPost]
    [ProducesResponseType(typeof(CommandResult), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        command.TenantId = GetTenantIdFromContext();
        command.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = result.Id },
            result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        command.TenantId = GetTenantIdFromContext();
        command.Id = id;

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return NoContent();
    }
}
```

### 6.2 Query Endpoints

```csharp
// Controllers/ProductsQueryController.cs
[ApiController]
[Route("api/[controller]")]
public class ProductsQueryController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpGet("{id}")]
    [ResponseCache(Duration = 3600)]
    public async Task<ActionResult<ProductReadModel>> GetProductById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery
        {
            TenantId = GetTenantIdFromContext(),
            ProductId = id
        };

        var result = await _mediator.Send(query, cancellationToken);
        
        return result != null 
            ? Ok(result) 
            : NotFound();
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<ProductReadModel>>> SearchProducts(
        [FromQuery] string? searchTerm,
        [FromQuery] string[]? categories,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsPagedQuery
        {
            TenantId = GetTenantIdFromContext(),
            SearchTerm = searchTerm,
            CategoryNames = categories,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            PageNumber = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
```

---

## 🔄 Phase 7: Event Sourcing & Audit (Week 7)

### 7.1 Event Store

```csharp
// Models/DomainEvent.cs
public abstract class DomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public int Version { get; set; }
    public Guid TenantId { get; set; }
    public string? UserId { get; set; }
}

// Services/EventStore.cs
public class EventStore : IEventStore
{
    private readonly EventStoreDbContext _context;
    private readonly ILogger<EventStore> _logger;

    public async Task AppendEventAsync<T>(
        T @event,
        CancellationToken cancellationToken)
        where T : DomainEvent
    {
        var eventRecord = new EventRecord
        {
            EventId = @event.EventId,
            EventType = typeof(T).FullName,
            TenantId = @event.TenantId,
            Payload = JsonSerializer.Serialize(@event),
            OccurredAt = @event.OccurredAt,
            UserId = @event.UserId
        };

        _context.Events.Add(eventRecord);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Event {EventType} appended (Id: {EventId}, Tenant: {TenantId})",
            eventRecord.EventType, eventRecord.EventId, eventRecord.TenantId);
    }

    public async Task<IEnumerable<DomainEvent>> GetEventsAsync(
        Guid tenantId,
        Guid aggregateId,
        CancellationToken cancellationToken)
    {
        var records = await _context.Events
            .AsNoTracking()
            .Where(e => e.TenantId == tenantId && e.AggregateId == aggregateId)
            .OrderBy(e => e.Version)
            .ToListAsync(cancellationToken);

        return records.Select(r => 
            JsonSerializer.Deserialize(r.Payload, Type.GetType(r.EventType)!) 
                as DomainEvent)!;
    }
}
```

---

## 📊 Phase 8: Testing & Benchmarking (Weeks 7-8)

### 8.1 Integration Tests

```csharp
// Tests/CatalogServiceCqrsTests.cs
[TestClass]
public class ProductCqrsTests
{
    private IMediator _mediator;
    private CatalogReadDbContext _readContext;
    private Guid _tenantId = Guid.NewGuid();

    [TestInitialize]
    public async Task Setup()
    {
        var services = new ServiceCollection();
        // ... register services
        var provider = services.BuildServiceProvider();
        _mediator = provider.GetRequiredService<IMediator>();
        _readContext = provider.GetRequiredService<CatalogReadDbContext>();
    }

    [TestMethod]
    public async Task CreateProduct_UpdatesReadModel()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            TenantId = _tenantId,
            Sku = "TEST-001",
            Name = "Test Product",
            Price = 99.99m
        };

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.IsTrue(result.Success);

        // Verify read model is updated
        var readModel = await _readContext.ProductsReadModel
            .FirstOrDefaultAsync(p => p.Id == result.Id);
        
        Assert.IsNotNull(readModel);
        Assert.AreEqual("Test Product", readModel.Name);
    }

    [TestMethod]
    public async Task SearchProducts_WithMillionProducts()
    {
        // Arrange: Insert 1 million test products
        await InsertMillionProductsAsync(_tenantId);

        // Act
        var query = new GetProductsPagedQuery
        {
            TenantId = _tenantId,
            PageNumber = 1,
            PageSize = 20
        };

        var sw = Stopwatch.StartNew();
        var result = await _mediator.Send(query);
        sw.Stop();

        // Assert: Should complete in < 500ms
        Assert.IsTrue(sw.ElapsedMilliseconds < 500,
            $"Query took {sw.ElapsedMilliseconds}ms (expected < 500ms)");
    }

    private async Task InsertMillionProductsAsync(Guid tenantId)
    {
        // Bulk insert 1 million products using EF Bulk Insert
        var products = Enumerable.Range(1, 1_000_000)
            .Select(i => new Product 
            { 
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Sku = $"SKU-{i:D7}",
                Name = $"Product {i}",
                Price = (decimal)(10 + (i % 1000))
            })
            .ToList();

        await _context.BulkInsertAsync(products);
    }
}
```

### 8.2 Load Testing

```csharp
// k6 Load Test Script (k6.io)
import http from 'k6/http';
import { check } from 'k6';

export const options = {
    stages: [
        { duration: '2m', target: 100 },   // Ramp up to 100 users
        { duration: '5m', target: 1000 },  // Ramp up to 1000 users
        { duration: '2m', target: 0 },     // Ramp down to 0
    ],
};

export default function () {
    // Test search endpoint
    const response = http.get(
        'http://localhost:5000/api/products/search?searchTerm=test&page=1&pageSize=20'
    );

    check(response, {
        'search returns 200': (r) => r.status === 200,
        'search completes in < 500ms': (r) => r.timings.duration < 500,
    });
}
```

---

## 🛠️ Phase 9: Deployment & Monitoring (Week 8+)

### 9.1 Database Tuning

```sql
-- Analyze query plans
EXPLAIN ANALYZE
SELECT * FROM products_read_model 
WHERE tenant_id = $1 
AND search_text @@ to_tsquery('english', $2)
LIMIT 20;

-- Vacuum and analyze for stats
VACUUM ANALYZE products_read_model;

-- Monitor slow queries
CREATE EXTENSION IF NOT EXISTS pg_stat_statements;

-- Find slow queries
SELECT query, mean_exec_time, calls
FROM pg_stat_statements
WHERE query LIKE '%products_read_model%'
ORDER BY mean_exec_time DESC
LIMIT 10;
```

### 9.2 Monitoring & Alerts

```csharp
// Health Check
services.AddHealthChecks()
    .AddCheck<ReadModelHealthCheck>("read-model")
    .AddCheck<EventStoreHealthCheck>("event-store")
    .AddCheck<ElasticSearchHealthCheck>("elasticsearch");

// Metrics
services.AddMetrics()
    .AddPrometheusExporter();

// Example metric
var commandDuration = new Histogram(
    name: "cqrs_command_duration_seconds",
    help: "Duration of command execution",
    labelNames: new[] { "command_type" }
);
```

---

## 📋 Implementierungs-Checklist

### Foundation
- [ ] MediatR NuGet-Pakete installieren
- [ ] Command/Query Base Classes erstellen
- [ ] Pipeline Behaviors implementieren

### Write Model
- [ ] Command Classes definieren
- [ ] Command Validators erstellen
- [ ] Command Handlers schreiben
- [ ] Domain Events erweitern

### Read Model
- [ ] Denormalized Read Tables designen
- [ ] Event Handler Projections schreiben
- [ ] Database Indexes optimieren
- [ ] Query Handlers implementieren

### Search
- [ ] ElasticSearch Setup
- [ ] Indexing bei Events triggern
- [ ] Search API implementieren
- [ ] Faceted Search konfigurieren

### Optimization
- [ ] Redis Caching hinzufügen
- [ ] Cache Invalidation implementieren
- [ ] Query Performance tunen
- [ ] Load Tests durchführen

### Production
- [ ] Event Sourcing Setup
- [ ] Monitoring & Alerting
- [ ] Database Tuning
- [ ] Disaster Recovery

---

## 🎯 Expected Performance Metrics

| Scenario | Current | CQRS Optimized |
|----------|---------|----------------|
| **List Products** | 2-3s | 50-100ms |
| **Search (Fuzzy)** | ❌ Not possible | 200-500ms |
| **Single Product** | 100-200ms | <50ms (cached) |
| **Bulk Import** | 30s (1M) | 5-10s |
| **Max Concurrent Users** | 100 | 10,000+ |

---

## 📚 Referenzen

- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [Event Sourcing](https://martinfowler.com/eaaDev/EventSourcing.html)
- [PostgreSQL Performance](https://www.postgresql.org/docs/current/sql-explain.html)
- [ElasticSearch Best Practices](https://www.elastic.co/guide/en/elasticsearch/reference/current/index.html)

---

**Next Step:** Start mit Phase 1 (Infrastructure Setup) - Week 1
