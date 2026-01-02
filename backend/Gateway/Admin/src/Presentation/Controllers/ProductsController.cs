using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using B2Connect.Admin.Presentation.Filters;
using B2Connect.Admin.Application.Commands.Products;
using B2Connect.Admin.Application.DTOs;
using Wolverine;

namespace B2Connect.Admin.Presentation.Controllers;

/// <summary>
/// Products Controller - HTTP Layer Only (CQRS Pattern)
///
/// 🏗️ Architektur:
/// HTTP Request
///   ↓
/// Controller (HTTP Concerns nur!)
///   - Header validieren
///   - Command/Query erstellen (ohne TenantId!)
///   - Response formatieren
///   ↓
/// Wolverine Message Bus (Message Dispatch)
///   ↓
/// Handler (Business Logic)
///   - TenantId via ITenantContextAccessor
///   - Validierung
///   - Repository Zugriff
///   ↓
/// Response Back
///
/// NOTE: TenantId wird automatisch im Handler via ITenantContextAccessor injiziert!
///
/// Filters Applied:
/// - ValidateTenantAttribute: Validates X-Tenant-ID header
/// - ApiExceptionHandlingFilter: Centralizes error handling
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ValidateTenant]
public class ProductsController : ApiControllerBase
{
    private readonly IMessageBus _messageBus;

    public ProductsController(IMessageBus messageBus, ILogger<ProductsController> logger) : base(logger)
    {
        _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
    }

    /// <summary>
    /// Gets a product by ID
    /// HTTP: GET /api/products/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResult>> GetProduct(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Fetching product {ProductId}", id);

        var query = new GetProductQuery(id);
        var product = await _messageBus.InvokeAsync<ProductResult?>(query, ct);

        if (product == null)
        {
            return NotFoundResponse($"Product {id} not found");
        }

        return OkResponse(product);
    }

    /// <summary>
    /// Gets a product by SKU
    /// HTTP: GET /api/products/sku/{sku}
    /// </summary>
    [HttpGet("sku/{sku}")]
    public async Task<ActionResult<ProductResult>> GetProductBySky(string sku, CancellationToken ct)
    {
        _logger.LogInformation("Fetching product by SKU {Sku}", sku);

        var query = new GetProductBySkuQuery(sku);
        var product = await _messageBus.InvokeAsync<ProductResult?>(query, ct);

        if (product == null)
        {
            return NotFoundResponse($"Product with SKU {sku} not found");
        }

        return OkResponse(product);
    }

    /// <summary>
    /// Gets a product by slug
    /// HTTP: GET /api/products/slug/{slug}
    /// </summary>
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ProductResult>> GetProductBySlug(string slug, CancellationToken ct)
    {
        _logger.LogInformation("Fetching product by slug {Slug}", slug);

        var query = new GetProductBySlugQuery(slug);
        var product = await _messageBus.InvokeAsync<ProductResult?>(query, ct);

        if (product == null)
        {
            return NotFoundResponse($"Product with slug '{slug}' not found");
        }

        return OkResponse(product);
    }

    /// <summary>
    /// Gets all products
    /// HTTP: GET /api/products
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResult>>> GetAllProducts(CancellationToken ct)
    {
        _logger.LogInformation("Fetching all products");

        var query = new GetAllProductsQuery();
        var products = await _messageBus.InvokeAsync<IEnumerable<ProductResult>>(query, ct);

        return OkResponse(products);
    }

    /// <summary>
    /// Gets products with pagination
    /// HTTP: GET /api/products/paged?pageNumber=1&pageSize=10
    /// </summary>
    [HttpGet("paged")]
    public async Task<ActionResult> GetProductsPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching products (page {PageNumber}, size {PageSize})", pageNumber, pageSize);

        var query = new GetProductsPagedQuery(pageNumber, pageSize);
        var (items, total) = await _messageBus.InvokeAsync<(IEnumerable<ProductResult> Items, int Total)>(query, ct);

        return OkResponseDynamic(new { items, total, pageNumber, pageSize });
    }

    /// <summary>
    /// Gets products by category
    /// HTTP: GET /api/products/category/{categoryId}
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductResult>>> GetProductsByCategory(Guid categoryId, CancellationToken ct)
    {
        _logger.LogInformation("Fetching products for category {CategoryId}", categoryId);

        var query = new GetProductsByCategoryQuery(categoryId);
        var products = await _messageBus.InvokeAsync<IEnumerable<ProductResult>>(query, ct);

        return OkResponse(products);
    }

    /// <summary>
    /// Gets products by brand
    /// HTTP: GET /api/products/brand/{brandId}
    /// </summary>
    [HttpGet("brand/{brandId}")]
    public async Task<ActionResult<IEnumerable<ProductResult>>> GetProductsByBrand(Guid brandId, CancellationToken ct)
    {
        _logger.LogInformation("Fetching products for brand {BrandId}", brandId);

        var query = new GetProductsByBrandQuery(brandId);
        var products = await _messageBus.InvokeAsync<IEnumerable<ProductResult>>(query, ct);

        return OkResponse(products);
    }

    /// <summary>
    /// Gets featured products
    /// HTTP: GET /api/products/featured?take=10
    /// </summary>
    [HttpGet("featured")]
    public async Task<ActionResult<IEnumerable<ProductResult>>> GetFeaturedProducts([FromQuery] int take = 10, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching {Count} featured products", take);

        var query = new GetFeaturedProductsQuery(take);
        var products = await _messageBus.InvokeAsync<IEnumerable<ProductResult>>(query, ct);

        return OkResponse(products);
    }

    /// <summary>
    /// Gets new products
    /// HTTP: GET /api/products/new?take=10
    /// </summary>
    [HttpGet("new")]
    public async Task<ActionResult<IEnumerable<ProductResult>>> GetNewProducts([FromQuery] int take = 10, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching {Count} new products", take);

        var query = new GetNewProductsQuery(take);
        var products = await _messageBus.InvokeAsync<IEnumerable<ProductResult>>(query, ct);

        return OkResponse(products);
    }

    /// <summary>
    /// Searches products by term
    /// HTTP: GET /api/products/search?q=searchTerm&pageNumber=1&pageSize=20
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult> SearchProducts([FromQuery] string q, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequestResponse("Search term is required");
        }

        _logger.LogInformation("Searching products with term '{SearchTerm}'", q);

        var query = new SearchProductsQuery(q, pageNumber, pageSize);
        var (items, total) = await _messageBus.InvokeAsync<(IEnumerable<ProductResult>, int)>(query, ct);

        return OkResponseDynamic(new { items, total, pageNumber, pageSize, searchTerm = q });
    }

    /// <summary>
    /// Creates a new product
    /// HTTP: POST /api/products
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductResult>> CreateProduct([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} creating product", userId);

        var command = new CreateProductCommand(
            request.Name,
            request.Sku,
            request.Price,
            request.Description,
            request.CategoryId,
            request.BrandId);

        var product = await _messageBus.InvokeAsync<ProductResult>(command, ct);

        return CreatedResponse(nameof(GetProduct), new { id = product.Id }, product);
    }

    /// <summary>
    /// Updates an existing product
    /// HTTP: PUT /api/products/{id}
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductResult>> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} updating product {ProductId}", userId, id);

        var command = new UpdateProductCommand(
            id,
            request.Name ?? string.Empty,
            request.Sku ?? string.Empty,
            request.Price ?? 0m,
            request.Description,
            request.CategoryId,
            request.BrandId);

        var product = await _messageBus.InvokeAsync<ProductResult>(command, ct);

        return OkResponse(product, "Product updated successfully");
    }

    /// <summary>
    /// Deletes a product
    /// HTTP: DELETE /api/products/{id}
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteProduct(Guid id, CancellationToken ct)
    {
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} deleting product {ProductId}", userId, id);

        var command = new DeleteProductCommand(id);
        var success = await _messageBus.InvokeAsync<bool>(command, ct);

        if (!success)
        {
            return NotFoundResponse($"Product {id} not found");
        }

        return NoContent();
    }
}