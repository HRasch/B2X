# Wolverine HTTP Endpoints

This directory contains HTTP endpoints using Wolverine's HTTP feature instead of traditional ASP.NET Core controllers.

## Structure

- **`ProductEndpoints.cs`** - Query endpoints (GET)
- **`ProductCommandEndpoints.cs`** - Command endpoints (POST, PUT, DELETE)

## Why Wolverine HTTP?

- ? **CQRS-First**: Natural separation of Commands and Queries
- ? **Mediator Pattern**: Direct integration with `IMessageBus`
- ? **Less Boilerplate**: No controller classes needed
- ? **Auto-Discovery**: Endpoints automatically registered

## Example

```csharp
[WolverineGet("/api/products/sku/{sku}")]
public static async Task<IResult> GetProductBySku(
    string sku,
    [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
    IProductService productService,
    CancellationToken ct)
{
    var product = await productService.GetBySkuAsync(tenantId, sku, ct);
    return product != null ? Results.Ok(product) : Results.NotFound();
}
```

## Key Features

- **Static Methods**: Endpoints are static methods (no class instance)
- **DI**: Services automatically injected as parameters
- **Attributes**: `[WolverineGet]`, `[WolverinePost]`, `[WolverinePut]`, `[WolverineDelete]`
- **Routing**: URL pattern in attribute

## See Also

- [Wolverine HTTP Documentation](../../docs/WOLVERINE_HTTP_ENDPOINTS.md)
- [Wolverine Official Docs](https://wolverine.netlify.app/guide/http/)
