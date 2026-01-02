using B2Connect.CatalogService.Models;
using B2Connect.CatalogService.Services;
using B2Connect.CatalogService.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace B2Connect.CatalogService.Controllers;

/// <summary>
/// Products API Controller
/// Endpoints for CRUD operations and search
/// </summary>
[ApiController]
[Route("api/v1/products")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IProductQueryHandler _queryHandler;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        IProductQueryHandler queryHandler,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _queryHandler = queryHandler;
        _logger = logger;
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        Guid id,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var product = await _queryHandler.GetByIdAsync(tenantId, id, cancellationToken);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    /// <summary>
    /// Get paged products
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _queryHandler.GetPagedAsync(tenantId, pageNumber, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Search products
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromQuery] string q,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest("Search term is required");
        }

        var result = await _queryHandler.SearchAsync(tenantId, q, pageNumber, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create product
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            return BadRequest("Request body is required");
        }

        var product = await _productService.CreateAsync(tenantId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>
    /// Update product
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _productService.UpdateAsync(tenantId, id, request, cancellationToken);
            return Ok(product);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Delete product
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var deleted = await _productService.DeleteAsync(tenantId, id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
