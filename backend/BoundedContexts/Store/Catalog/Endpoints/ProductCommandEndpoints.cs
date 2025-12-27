using Wolverine;
using Wolverine.Http;

namespace B2Connect.CatalogService.Endpoints;

/// <summary>
/// Command endpoints for Product management (Create, Update, Delete)
/// Uses Wolverine's mediator pattern with commands
/// </summary>
public static class ProductCommandEndpoints
{
    /// <summary>
    /// POST /api/products
    /// Creates a new product
    /// </summary>
    [WolverinePost("/api/products")]
    public static async Task<IResult> CreateProduct(
        CreateProductCommand command,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IMessageBus messageBus,
        CancellationToken ct)
    {
        // Add tenant ID to command
        var commandWithTenant = command with { TenantId = tenantId };
        
        // Publish command via Wolverine
        var result = await messageBus.InvokeAsync<ProductDto>(commandWithTenant, ct);
        
        return Results.Created($"/api/products/{result.Sku}", result);
    }

    /// <summary>
    /// PUT /api/products/{sku}
    /// Updates an existing product
    /// </summary>
    [WolverinePut("/api/products/sku/{sku}")]
    public static async Task<IResult> UpdateProduct(
        string sku,
        UpdateProductCommand command,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IMessageBus messageBus,
        CancellationToken ct)
    {
        var commandWithTenant = command with { TenantId = tenantId, Sku = sku };
        
        var result = await messageBus.InvokeAsync<ProductDto>(commandWithTenant, ct);
        
        if (result == null)
        {
            return Results.NotFound(new { Message = $"Product with SKU '{sku}' not found" });
        }

        return Results.Ok(result);
    }

    /// <summary>
    /// DELETE /api/products/{sku}
    /// Deletes a product (soft delete)
    /// </summary>
    [WolverineDelete("/api/products/sku/{sku}")]
    public static async Task<IResult> DeleteProduct(
        string sku,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        IMessageBus messageBus,
        CancellationToken ct)
    {
        var command = new DeleteProductCommand(tenantId, sku);
        
        await messageBus.InvokeAsync(command, ct);
        
        return Results.NoContent();
    }
}

// Commands
public record CreateProductCommand(
    Guid TenantId,
    string Sku,
    string Name,
    decimal Price,
    string? Description = null
);

public record UpdateProductCommand(
    Guid TenantId,
    string Sku,
    string? Name = null,
    decimal? Price = null,
    string? Description = null
);

public record DeleteProductCommand(Guid TenantId, string Sku);

// DTO
public record ProductDto(
    Guid Id,
    Guid TenantId,
    string Sku,
    string Name,
    decimal Price,
    string? Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
