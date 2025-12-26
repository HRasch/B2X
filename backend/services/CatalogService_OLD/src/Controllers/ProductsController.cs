using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using B2Connect.CatalogService.Services;
using B2Connect.CatalogService.Models;

namespace B2Connect.CatalogService.Controllers;

/// <summary>
/// API Controller for Product operations
/// Public GET endpoints for store, admin CRUD endpoints require Admin role
/// Uses AOP filters for logging, validation, and exception handling
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService service, ILogger<ProductsController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets a product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        var product = await _service.GetProductAsync(id);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    /// <summary>
    /// Gets a product by SKU
    /// </summary>
    [HttpGet("sku/{sku}")]
    public async Task<ActionResult<ProductDto>> GetProductBySky(string sku)
    {
        var product = await _service.GetProductBySkuAsync(sku);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    /// <summary>
    /// Gets a product by slug
    /// </summary>
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ProductDto>> GetProductBySlug(string slug)
    {
        var product = await _service.GetProductBySlugAsync(slug);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    /// <summary>
    /// Gets all products
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {
        var products = await _service.GetAllProductsAsync();
        return Ok(products);
    }

    /// <summary>
    /// Gets products with pagination
    /// </summary>
    [HttpGet("paged")]
    public async Task<ActionResult> GetProductsPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (items, total) = await _service.GetProductsPagedAsync(pageNumber, pageSize);
        return Ok(new { items, total, pageNumber, pageSize });
    }

    /// <summary>
    /// Gets products by category
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(Guid categoryId)
    {
        var products = await _service.GetProductsByCategoryAsync(categoryId);
        return Ok(products);
    }

    /// <summary>
    /// Gets products by brand
    /// </summary>
    [HttpGet("brand/{brandId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByBrand(Guid brandId)
    {
        var products = await _service.GetProductsByBrandAsync(brandId);
        return Ok(products);
    }

    /// <summary>
    /// Gets featured products
    /// </summary>
    [HttpGet("featured")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetFeaturedProducts([FromQuery] int take = 10)
    {
        var products = await _service.GetFeaturedProductsAsync(take);
        return Ok(products);
    }

    /// <summary>
    /// Gets new products
    /// </summary>
    [HttpGet("new")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetNewProducts([FromQuery] int take = 10)
    {
        var products = await _service.GetNewProductsAsync(take);
        return Ok(products);
    }

    /// <summary>
    /// Searches products by term
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Search term is required");

        var products = await _service.SearchProductsAsync(q);
        return Ok(products);
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
    {
        try
        {
            var product = await _service.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, UpdateProductDto dto)
    {
        try
        {
            var product = await _service.UpdateProductAsync(id, dto);
            return Ok(product);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a product
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        var success = await _service.DeleteProductAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
