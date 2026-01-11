using B2X.Catalog.Application.Commands;
using B2X.Catalog.Models;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

#pragma warning disable S6960 // This controller has multiple responsibilities and could be split into 2 smaller controllers

namespace B2X.Store.Controllers;

/// <summary>
/// Request DTOs for product operations
/// </summary>
public record CreateProductRequest(
    string Sku,
    string Name,
    string? Description,
    decimal Price,
    decimal? DiscountPrice,
    int StockQuantity,
    bool IsActive,
    List<Guid> CategoryIds,
    string? BrandName,
    List<string> Tags,
    string? Barcode
);

public record ProductUpdateRequest(
    string? Sku,
    string? Name,
    string? Description,
    decimal? Price,
    decimal? DiscountPrice,
    int? StockQuantity,
    bool? IsActive,
    List<Guid>? CategoryIds,
    string? BrandName,
    List<string>? Tags,
    string? Barcode
);

/// <summary>
/// Products API controller using CQRS pattern with Wolverine
/// Handles product CRUD operations via message bus
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMessageBus _bus;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IMessageBus bus,
        ILogger<ProductsController> logger)
    {
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets a product by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProduct(Guid id, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        var query = new GetProductByIdQuery(id, tenantId);
        var result = await _bus.InvokeAsync<Product?>(query);

        if (result == null)
        {
            return NotFound(new { Message = "Product not found" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Gets products by tenant with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = new GetProductsByTenantQuery(tenantId, page, pageSize);

        var result = await _bus.InvokeAsync<IEnumerable<Product>>(query);
        return Ok(result);
    }

    /// <summary>
    /// Gets products by category
    /// </summary>
    [HttpGet("category/{categoryId:guid}")]
    public async Task<IActionResult> GetProductsByCategory(
        Guid categoryId,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = new GetProductsByCategoryQuery(categoryId, tenantId, page, pageSize);

        var result = await _bus.InvokeAsync<IEnumerable<Product>>(query);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductRequest request,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        var command = new CreateProductCommand(
            tenantId,
            request.Sku,
            request.Name,
            request.Description,
            request.Price,
            request.DiscountPrice,
            request.StockQuantity,
            request.IsActive,
            request.CategoryIds,
            request.BrandName,
            request.Tags,
            request.Barcode
        );
        var result = await _bus.InvokeAsync<Product>(command);

        return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct(
        Guid id,
        [FromBody] ProductUpdateRequest request,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        var command = new UpdateProductCommand(
            id,
            tenantId,
            request.Sku,
            request.Name,
            request.Description,
            request.Price,
            request.DiscountPrice,
            request.StockQuantity,
            request.IsActive,
            request.CategoryIds,
            request.BrandName,
            request.Tags,
            request.Barcode
        );
        var result = await _bus.InvokeAsync<Product>(command);

        return Ok(result);
    }

    /// <summary>
    /// Deletes a product
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        var command = new DeleteProductCommand(id, tenantId);
        await _bus.InvokeAsync(command);

        return NoContent();
    }
}
