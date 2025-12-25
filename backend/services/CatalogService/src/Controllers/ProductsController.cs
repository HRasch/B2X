using Microsoft.AspNetCore.Mvc;
using B2Connect.CatalogService.Services;

namespace B2Connect.CatalogService.Controllers;

/// <summary>
/// API Controller for Product operations
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
    [ProduceResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProduceResponseType(StatusCodes.Status404NotFound)]
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
    [ProduceResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProduceResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetProductBySku(string sku)
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
    [ProduceResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProduceResponseType(StatusCodes.Status404NotFound)]
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
    [ProduceResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {
        var products = await _service.GetAllProductsAsync();
        return Ok(products);
    }

    /// <summary>
    /// Gets products with pagination
    /// </summary>
    [HttpGet("paged")]
    [ProduceResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetProductsPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (items, total) = await _service.GetProductsPagedAsync(pageNumber, pageSize);
        return Ok(new { items, total, pageNumber, pageSize });
    }

    /// <summary>
    /// Gets products by category
    /// </summary>
    [HttpGet("category/{categoryId}")]
    [ProduceResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(Guid categoryId)
    {
        var products = await _service.GetProductsByCategoryAsync(categoryId);
        return Ok(products);
    }

    /// <summary>
    /// Gets products by brand
    /// </summary>
    [HttpGet("brand/{brandId}")]
    [ProduceResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByBrand(Guid brandId)
    {
        var products = await _service.GetProductsByBrandAsync(brandId);
        return Ok(products);
    }

    /// <summary>
    /// Gets featured products
    /// </summary>
    [HttpGet("featured")]
    [ProduceResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetFeaturedProducts([FromQuery] int take = 10)
    {
        var products = await _service.GetFeaturedProductsAsync(take);
        return Ok(products);
    }

    /// <summary>
    /// Gets new products
    /// </summary>
    [HttpGet("new")]
    [ProduceResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetNewProducts([FromQuery] int take = 10)
    {
        var products = await _service.GetNewProductsAsync(take);
        return Ok(products);
    }

    /// <summary>
    /// Searches products by term
    /// </summary>
    [HttpGet("search")]
    [ProduceResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
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
    [ProduceResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProduceResponseType(StatusCodes.Status400BadRequest)]
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
    [ProduceResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProduceResponseType(StatusCodes.Status404NotFound)]
    [ProduceResponseType(StatusCodes.Status400BadRequest)]
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
    [ProduceResponseType(StatusCodes.Status204NoContent)]
    [ProduceResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        var success = await _service.DeleteProductAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
