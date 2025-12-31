# Wolverine HTTP Endpoints Pattern

## Übersicht

B2Connect Domain-Services verwenden **Wolverine HTTP Endpoints** statt klassischen ASP.NET Core Controllern. Dies ermöglicht eine konsistente CQRS-Architektur und nahtlose Integration mit Wolverine Messaging.

## Warum Wolverine HTTP statt Controller?

? **Konsistenz** - Commands/Queries werden einheitlich über Wolverine behandelt  
? **CQRS-First** - Natürliche Trennung von Read/Write Operations  
? **Weniger Boilerplate** - Keine Controller-Klassen nötig  
? **Auto-Discovery** - Endpoints werden automatisch registriert  
? **Mediator Pattern** - Direkte Integration mit `IMessageBus`  

## Konfiguration

### 1. Program.cs Setup

```csharp
using Wolverine;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

// Wolverine mit HTTP Endpoints aktivieren
builder.Host.UseWolverine(opts =>
{
    opts.ServiceName = "CatalogService";
    
    // HTTP Endpoints aktivieren
    opts.Http.EnableEndpoints = true;
    
    opts.Discovery.DisableConventionalDiscovery();
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly);
});

// ? NICHT: builder.Services.AddControllers();

var app = builder.Build();

// ? Wolverine Endpoints registrieren (ersetzt MapControllers)
app.MapWolverineEndpoints();

app.Run();
```

### 2. Endpoint Definition

```csharp
using Wolverine.Http;

namespace B2Connect.CatalogService.Endpoints;

public static class ProductEndpoints
{
    /// <summary>
    /// GET /api/products/sku/{sku}
    /// </summary>
    [WolverineGet("/api/products/sku/{sku}")]
    public static async Task<IResult> GetProductBySku(
        string sku,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IProductService productService,
        CancellationToken ct)
    {
        var product = await productService.GetBySkuAsync(tenantId, sku, ct);
        
        if (product == null)
        {
            return Results.NotFound(new { Message = "Product not found" });
        }

        return Results.Ok(product);
    }

    /// <summary>
    /// POST /api/products
    /// </summary>
    [WolverinePost("/api/products")]
    public static async Task<IResult> CreateProduct(
        CreateProductCommand command,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IMessageBus messageBus,
        CancellationToken ct)
    {
        var commandWithTenant = command with { TenantId = tenantId };
        var result = await messageBus.InvokeAsync<ProductDto>(commandWithTenant, ct);
        
        return Results.Created($"/api/products/{result.Sku}", result);
    }
}
```

## HTTP Verb Attributes

| Attribut | HTTP Methode | Verwendung |
|----------|--------------|------------|
| `[WolverineGet]` | GET | Daten abrufen |
| `[WolverinePost]` | POST | Ressourcen erstellen |
| `[WolverinePut]` | PUT | Ressourcen aktualisieren |
| `[WolverineDelete]` | DELETE | Ressourcen löschen |
| `[WolverinePatch]` | PATCH | Partielle Updates |

## Parameter Binding

### Route Parameter

```csharp
[WolverineGet("/api/products/{id}")]
public static Task<IResult> Get(Guid id, IProductService service)
{
    // id wird automatisch aus Route gebunden
}
```

### Query Parameter

```csharp
[WolverineGet("/api/products")]
public static Task<IResult> List(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20)
{
    // ?page=2&pageSize=50
}
```

### Request Body

```csharp
[WolverinePost("/api/products")]
public static Task<IResult> Create(CreateProductCommand command)
{
    // Automatisches JSON Deserialization
}
```

### Headers

```csharp
[WolverineGet("/api/products")]
public static Task<IResult> List(
    [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
    [FromHeader(Name = "Authorization")] string? token)
{
    // Header werden extrahiert
}
```

### Dependency Injection

```csharp
[WolverineGet("/api/products/{sku}")]
public static Task<IResult> Get(
    string sku,
    IProductService productService,  // ? Auto-injected
    ILogger<ProductEndpoints> logger, // ? Auto-injected
    CancellationToken ct)             // ? Auto-injected
{
    // Services werden automatisch injiziert
}
```

## CQRS Pattern mit Wolverine

### Query Endpoint

```csharp
[WolverineGet("/api/products/{sku}")]
public static async Task<IResult> GetProduct(
    string sku,
    Guid tenantId,
    IMessageBus messageBus,
    CancellationToken ct)
{
    var query = new GetProductQuery(tenantId, sku);
    var product = await messageBus.InvokeAsync<ProductDto>(query, ct);
    
    return product != null 
        ? Results.Ok(product) 
        : Results.NotFound();
}

// Query Handler (separates file)
public record GetProductQuery(Guid TenantId, string Sku);

public class GetProductQueryHandler
{
    private readonly IProductRepository _repo;

    public GetProductQueryHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<ProductDto?> Handle(GetProductQuery query, CancellationToken ct)
    {
        var product = await _repo.GetBySkuAsync(query.TenantId, query.Sku, ct);
        return product != null ? MapToDto(product) : null;
    }
}
```

### Command Endpoint

```csharp
[WolverinePost("/api/products")]
public static async Task<IResult> CreateProduct(
    CreateProductCommand command,
    Guid tenantId,
    IMessageBus messageBus,
    CancellationToken ct)
{
    var commandWithTenant = command with { TenantId = tenantId };
    var result = await messageBus.InvokeAsync<ProductDto>(commandWithTenant, ct);
    
    return Results.Created($"/api/products/{result.Sku}", result);
}

// Command Handler (separate file)
public record CreateProductCommand(Guid TenantId, string Sku, string Name, decimal Price);

public class CreateProductCommandHandler
{
    private readonly IProductRepository _repo;

    public CreateProductCommandHandler(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<ProductDto> Handle(CreateProductCommand command, CancellationToken ct)
    {
        var product = Product.Create(command.TenantId, command.Sku, command.Name, command.Price);
        await _repo.AddAsync(product, ct);
        return MapToDto(product);
    }
}
```

## Authorization

```csharp
using Microsoft.AspNetCore.Authorization;

[WolverineGet("/api/users/{userId}")]
[Authorize]  // ? Standard ASP.NET Core Authorization
public static Task<IResult> GetUser(string userId, IAuthService authService)
{
    // Nur authentifizierte User
}

[WolverineDelete("/api/users/{userId}")]
[Authorize(Roles = "Admin")]  // ? Rollen-basiert
public static Task<IResult> DeleteUser(string userId, IAuthService authService)
{
    // Nur Admins
}
```

## Validation

```csharp
using FluentValidation;

[WolverinePost("/api/products")]
public static async Task<IResult> CreateProduct(
    CreateProductCommand command,
    IValidator<CreateProductCommand> validator,
    IMessageBus messageBus)
{
    // Manuelle Validation
    var validationResult = await validator.ValidateAsync(command);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(
            validationResult.ToDictionary());
    }

    var result = await messageBus.InvokeAsync<ProductDto>(command);
    return Results.Created($"/api/products/{result.Sku}", result);
}

// Validator
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
```

## Error Handling

```csharp
[WolverineGet("/api/products/{sku}")]
public static async Task<IResult> GetProduct(
    string sku,
    Guid tenantId,
    IProductService productService,
    ILogger<ProductEndpoints> logger,
    CancellationToken ct)
{
    try
    {
        var product = await productService.GetBySkuAsync(tenantId, sku, ct);
        
        if (product == null)
        {
            return Results.NotFound(new { Message = "Product not found" });
        }

        return Results.Ok(product);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to get product {Sku}", sku);
        return Results.Problem("An error occurred while retrieving the product");
    }
}
```

## Projekt-Struktur

```
BoundedContexts/Store/Catalog/
??? Endpoints/
?   ??? ProductEndpoints.cs          # GET queries
?   ??? ProductCommandEndpoints.cs   # POST/PUT/DELETE commands
?   ??? CategoryEndpoints.cs
??? Handlers/
?   ??? GetProductQueryHandler.cs
?   ??? CreateProductCommandHandler.cs
?   ??? UpdateProductCommandHandler.cs
??? Commands/
?   ??? CreateProductCommand.cs
?   ??? UpdateProductCommand.cs
??? Queries/
?   ??? GetProductQuery.cs
?   ??? SearchProductsQuery.cs
??? Program.cs
```

## Migration von Controllern zu Wolverine

### Vorher (Controller)

```csharp
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("{sku}")]
    public async Task<ActionResult<ProductDto>> Get(string sku)
    {
        var product = await _productService.GetBySkuAsync(sku);
        if (product == null) return NotFound();
        return Ok(product);
    }
}
```

### Nachher (Wolverine Endpoint)

```csharp
public static class ProductEndpoints
{
    [WolverineGet("/api/products/{sku}")]
    public static async Task<IResult> Get(
        string sku,
        IProductService productService,
        CancellationToken ct)
    {
        var product = await productService.GetBySkuAsync(sku, ct);
        return product != null ? Results.Ok(product) : Results.NotFound();
    }
}
```

## Best Practices

? **Static Methods** - Endpoints als statische Methoden definieren  
? **CancellationToken** - Immer als letzter Parameter  
? **IResult Return Type** - Für flexible Responses  
? **Tenant Isolation** - Immer `X-Tenant-ID` Header prüfen  
? **Separate Handlers** - Commands/Queries in eigenen Handler-Klassen  
? **Logging** - ILogger per DI injizieren  
? **Validation** - FluentValidation für Input-Checks  

? **Keine Controller-Klassen** mehr  
? **Kein ControllerBase** erben  
? **Kein [ApiController]** Attribut  
? **Kein MapControllers()** in Program.cs  

## Weitere Informationen

- [Wolverine HTTP Documentation](https://wolverine.netlify.app/guide/http/)
- [Wolverine Mediator](https://wolverine.netlify.app/guide/messaging/message-bus.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
