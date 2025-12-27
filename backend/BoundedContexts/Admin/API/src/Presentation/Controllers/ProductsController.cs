using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using B2Connect.Admin.Presentation.Filters;
using B2Connect.Admin.Application.Commands.Products;
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
///   - Command/Query erstellen
///   - Response formatieren
///   ↓
/// Wolverine Message Bus (Message Dispatch)
///   ↓
/// Handler (Business Logic)
///   - Validierung
///   - Repository Zugriff
///   - Exception Handling
///   ↓
/// Response Back
///
/// 🎯 Benefit: Separation of Concerns!
/// - Controller: HTTP Concerns
/// - Handler: Business Logic
/// - Filter: Cross-Cutting Concerns
/// 
/// Filters Applied:
/// - ValidateTenantAttribute: Validates X-Tenant-ID header
/// - ApiExceptionHandlingFilter: Centralizes error handling
/// - ValidateModelStateFilter: Validates request models
/// - ApiLoggingFilter: Logs all requests and responses
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
    /// CQRS: GetProductQuery dispatched to Wolverine
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResult>> GetProduct(Guid id, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching product {ProductId} for tenant {TenantId}", id, tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new GetProductQuery(tenantId, id);
        var product = await _messageBus.InvokeAsync<ProductResult?>(query, ct);

        if (product == null)
            return NotFoundResponse($"Product {id} not found");

        return OkResponse(product);
    }

    /// <summary>
    /// Gets a product by SKU
    /// HTTP: GET /api/products/sku/{sku}
    /// CQRS: GetProductBySkuQuery dispatched to Wolverine
    /// </summary>
    [HttpGet("sku/{sku}")]
    public async Task<ActionResult<ProductResult>> GetProductBySky(string sku, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching product by SKU {Sku} for tenant {TenantId}", sku, tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new GetProductBySkuQuery(tenantId, sku);
        var product = await _messageBus.InvokeAsync<ProductResult?>(query, ct);

        if (product == null)
            return NotFoundResponse($"Product with SKU {sku} not found");

        return OkResponse(product);
    }

    /// <summary>
    /// Gets a product by slug
    /// HTTP: GET /api/products/slug/{slug}
    /// CQRS: GetProductBySlugQuery dispatched to Wolverine
    /// </summary>
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ProductResult>> GetProductBySlug(string slug, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching product by slug {Slug} for tenant {TenantId}", slug, tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new GetProductBySlugQuery(tenantId, slug);
        var product = await _messageBus.InvokeAsync<ProductResult?>(query, ct);

        if (product == null)
            return NotFoundResponse($"Product with slug '{slug}' not found");

        return OkResponse(product);
    }

    /// <summary>
    /// Gets all products
    /// HTTP: GET /api/products
    /// CQRS: GetAllProductsQuery dispatched to Wolverine
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResult>>> GetAllProducts(CancellationToken ct)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching all products for tenant {TenantId}", tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new GetAllProductsQuery(tenantId);
        var products = await _messageBus.InvokeAsync<IEnumerable<ProductResult>>(query, ct);

        return OkResponse(products);
    }

    /// <summary>
    /// Gets products with pagination
    /// HTTP: GET /api/products/paged?pageNumber=1&pageSize=10
    /// CQRS: GetProductsPagedQuery dispatched to Wolverine
    /// </summary>
    [HttpGet("paged")]
    public async Task<ActionResult> GetProductsPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching products (page {PageNumber}, size {PageSize}) for tenant {TenantId}",
            pageNumber, pageSize, tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new GetProductsPagedQuery(tenantId, pageNumber, pageSize);
        var (items, total) = await _messageBus.InvokeAsync<(IEnumerable<ProductResult> Items, int Total)>(query, ct);

        return OkResponse(new { items, total, pageNumber, pageSize });
    }

    /// <summary>
    /// Gets products by category
    /// HTTP: GET /api/products/category/{categoryId}
    /// CQRS: GetProductsByCategoryQuery dispatched to Wolverine
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductResult>>> GetProductsByCategory(Guid categoryId, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching products for category {CategoryId} in tenant {TenantId}",
            categoryId, tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new GetProductsByCategoryQuery(tenantId, categoryId);
        var products = await _messageBus.InvokeAsync<IEnumerable<ProductResult>>(query, ct);

        return OkResponse(products);
    }

    /// <summary>
    /// Gets products by brand
    /// HTTP: GET /api/products/brand/{brandId}
    /// CQRS: GetProductsByBrandQuery dispatched to Wolverine
    /// </summary>
    [HttpGet("brand/{brandId}")]
    public async Task<ActionResult<IEnumerable<ProductResult>>> GetProductsByBrand(Guid brandId, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching products for brand {BrandId} in tenant {TenantId}",
            brandId, tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new GetProductsByBrandQuery(tenantId, brandId);
        var products = await _messageBus.InvokeAsync<IEnumerable<ProductResult>>(query, ct);

        return OkResponse(products);
    }

    /// <summary>
    /// Gets featured products
    /// HTTP: GET /api/products/featured?take=10
    /// CQRS: GetFeaturedProductsQuery dispatched to Wolverine
    /// </summary>
    [HttpGet("featured")]
    public async Task<ActionResult<IEnumerable<ProductResult>>> GetFeaturedProducts([FromQuery] int take = 10, CancellationToken ct = default)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching {Count} featured products for tenant {TenantId}", take, tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new GetFeaturedProductsQuery(tenantId, take);
        var products = await _messageBus.InvokeAsync<IEnumerable<ProductResult>>(query, ct);

        return OkResponse(products);
    }

    /// <summary>
    /// Gets new products
    /// HTTP: GET /api/products/new?take=10
    /// CQRS: GetNewProductsQuery dispatched to Wolverine
    /// </summary>
    [HttpGet("new")]
    public async Task<ActionResult<IEnumerable<ProductResult>>> GetNewProducts([FromQuery] int take = 10, CancellationToken ct = default)
    {
        var tenantId = GetTenantId();
        _logger.LogInformation("Fetching {Count} new products for tenant {TenantId}", take, tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new GetNewProductsQuery(tenantId, take);
        var products = await _messageBus.InvokeAsync<IEnumerable<ProductResult>>(query, ct);

        return OkResponse(products);
    }

    /// <summary>
    /// Searches products by term
    /// HTTP: GET /api/products/search?q=searchTerm&pageNumber=1&pageSize=20
    /// CQRS: SearchProductsQuery dispatched to Wolverine
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult> SearchProducts([FromQuery] string q, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var tenantId = GetTenantId();

        if (string.IsNullOrWhiteSpace(q))
            return BadRequestResponse("Search term is required");

        _logger.LogInformation("Searching products with term '{SearchTerm}' for tenant {TenantId}", q, tenantId);

        // Dispatch Query via Wolverine Message Bus → Handler
        var query = new SearchProductsQuery(tenantId, q, pageNumber, pageSize);
        var (items, total) = await _messageBus.InvokeAsync<(IEnumerable<ProductResult>, int)>(query, ct);

        return OkResponse(new { items, total, pageNumber, pageSize, searchTerm = q });
    }

    /// <summary>
    /// Creates a new product
    /// HTTP: POST /api/products
    /// CQRS: CreateProductCommand dispatched to Wolverine
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductResult>> CreateProduct([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} creating product for tenant {TenantId}", userId, tenantId);

        // Dispatch Command via Wolverine Message Bus → Handler
        var command = new CreateProductCommand(
            tenantId,
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
    /// CQRS: UpdateProductCommand dispatched to Wolverine
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductResult>> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} updating product {ProductId} in tenant {TenantId}",
            userId, id, tenantId);

        // Dispatch Command via Wolverine Message Bus → Handler
        var command = new UpdateProductCommand(
            tenantId,
            id,
            request.Name,
            request.Sku,
            request.Price,
            request.Description,
            request.CategoryId,
            request.BrandId);

        var product = await _messageBus.InvokeAsync<ProductResult>(command, ct);

        return OkResponse(product, "Product updated successfully");
    }

    /// <summary>
    /// Deletes a product
    /// HTTP: DELETE /api/products/{id}
    /// CQRS: DeleteProductCommand dispatched to Wolverine
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteProduct(Guid id, CancellationToken ct)
    {
        var tenantId = GetTenantId();
        var userId = GetUserId();
        _logger.LogInformation("User {UserId} deleting product {ProductId} from tenant {TenantId}",
            userId, id, tenantId);

        // Dispatch Command via Wolverine Message Bus → Handler
        var command = new DeleteProductCommand(tenantId, id);
        var success = await _messageBus.InvokeAsync<bool>(command, ct);

        if (!success)
            return NotFoundResponse($"Product {id} not found");

        return NoContent();
    }
}