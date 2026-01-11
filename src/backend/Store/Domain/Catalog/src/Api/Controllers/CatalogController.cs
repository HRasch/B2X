using B2X.Catalog.Application.Commands;
using B2X.Catalog.Core.Interfaces;
using B2X.Catalog.Models;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace B2X.Catalog.Api.Controllers;

/// <summary>
/// Unified catalog API controller
/// Provides endpoints for Products, Categories, and Variants
/// </summary>
[ApiController]
[Route("api/v1/catalog")]
public class CatalogController : ControllerBase
{
    private readonly IMessageBus _bus;
    private readonly ICatalogRepository _repository;

    public CatalogController(IMessageBus bus, ICatalogRepository repository)
    {
        _bus = bus;
        _repository = repository;
    }

    #region Product Endpoints

    /// <summary>
    /// Get a product by ID
    /// </summary>
    [HttpGet("products/{id}")]
    public async Task<ActionResult<Product>> GetProduct(Guid id, [FromQuery] Guid tenantId)
    {
        var product = await _repository.GetProductByIdAsync(id, tenantId);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    /// <summary>
    /// Get products with pagination
    /// </summary>
    [HttpGet("products")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
        [FromQuery] Guid tenantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var products = await _repository.GetProductsByTenantAsync(tenantId, page, pageSize);
        return Ok(products);
    }

    /// <summary>
    /// Get products by category
    /// </summary>
    [HttpGet("categories/{categoryId}/products")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(
        Guid categoryId,
        [FromQuery] Guid tenantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var products = await _repository.GetProductsByCategoryAsync(categoryId, tenantId, page, pageSize);
        return Ok(products);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost("products")]
    public async Task<ActionResult<Product>> CreateProduct(CreateProductCommand command)
    {
        var product = await _bus.InvokeAsync<Product>(command);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id, tenantId = product.TenantId }, product);
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut("products/{id}")]
    public async Task<ActionResult<Product>> UpdateProduct(Guid id, UpdateProductCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        var product = await _bus.InvokeAsync<Product>(command);
        return Ok(product);
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    [HttpDelete("products/{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id, [FromQuery] Guid tenantId)
    {
        var command = new DeleteProductCommand(id, tenantId);
        await _bus.InvokeAsync(command);
        return NoContent();
    }

    /// <summary>
    /// Add product to category
    /// </summary>
    [HttpPost("products/{productId}/categories/{categoryId}")]
    public async Task<IActionResult> AddProductToCategory(Guid productId, Guid categoryId, [FromQuery] Guid tenantId)
    {
        var command = new CategorizeProductCommand(productId, categoryId, tenantId);
        await _bus.InvokeAsync(command);
        return NoContent();
    }

    /// <summary>
    /// Remove product from category
    /// </summary>
    [HttpDelete("products/{productId}/categories/{categoryId}")]
    public async Task<IActionResult> RemoveProductFromCategory(Guid productId, Guid categoryId, [FromQuery] Guid tenantId)
    {
        var command = new RemoveProductFromCategoryCommand(productId, categoryId, tenantId);
        await _bus.InvokeAsync(command);
        return NoContent();
    }

    #endregion

    #region Category Endpoints

    /// <summary>
    /// Get a category by ID
    /// </summary>
    [HttpGet("categories/{id}")]
    public async Task<ActionResult<Category>> GetCategory(Guid id, [FromQuery] Guid tenantId)
    {
        var category = await _repository.GetCategoryByIdAsync(id, tenantId);
        if (category == null)
            return NotFound();

        return Ok(category);
    }

    /// <summary>
    /// Get categories with pagination and filtering
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult> GetCategories(
        [FromQuery] Guid tenantId,
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? parentId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? sortBy = "DisplayOrder",
        [FromQuery] bool sortDescending = false)
    {
        var result = await _repository.GetCategoriesPagedAsync(
            tenantId, searchTerm, parentId, isActive, pageNumber, pageSize, sortBy, sortDescending);

        return Ok(result);
    }

    /// <summary>
    /// Get category hierarchy
    /// </summary>
    [HttpGet("categories/hierarchy")]
    public async Task<ActionResult<List<Category>>> GetCategoryHierarchy(
        [FromQuery] Guid tenantId,
        [FromQuery] Guid? rootCategoryId = null)
    {
        var categories = await _repository.GetCategoryHierarchyAsync(tenantId, rootCategoryId);
        return Ok(categories);
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost("categories")]
    public async Task<ActionResult<Category>> CreateCategory(CreateCategoryCommand command)
    {
        var category = await _bus.InvokeAsync<Category>(command);
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id, tenantId = category.TenantId }, category);
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    [HttpPut("categories/{id}")]
    public async Task<ActionResult<Category>> UpdateCategory(Guid id, UpdateCategoryCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        var category = await _bus.InvokeAsync<Category>(command);
        return Ok(category);
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id, [FromQuery] Guid tenantId)
    {
        var command = new DeleteCategoryCommand(id, tenantId);
        await _bus.InvokeAsync(command);
        return NoContent();
    }

    #endregion

    #region Variant Endpoints

    /// <summary>
    /// Get a variant by ID
    /// </summary>
    [HttpGet("variants/{id}")]
    public async Task<ActionResult<Variant>> GetVariant(Guid id)
    {
        var variant = await _repository.GetVariantByIdAsync(id);
        if (variant == null)
            return NotFound();

        return Ok(variant);
    }

    /// <summary>
    /// Get variants by product ID
    /// </summary>
    [HttpGet("products/{productId}/variants")]
    public async Task<ActionResult<IEnumerable<Variant>>> GetVariantsByProduct(Guid productId)
    {
        var variants = await _repository.GetVariantsByProductIdAsync(productId);
        return Ok(variants);
    }

    /// <summary>
    /// Get variants with pagination
    /// </summary>
    [HttpGet("variants")]
    public async Task<ActionResult> GetVariants(
        [FromQuery] Guid tenantId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var (items, totalCount) = await _repository.GetVariantsPagedAsync(tenantId, pageNumber, pageSize);
        return Ok(new { Items = items, TotalCount = totalCount, PageNumber = pageNumber, PageSize = pageSize });
    }

    /// <summary>
    /// Search variants
    /// </summary>
    [HttpGet("variants/search")]
    public async Task<ActionResult> SearchVariants(
        [FromQuery] Guid tenantId,
        [FromQuery] string query,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var (items, totalCount) = await _repository.SearchVariantsAsync(tenantId, query, pageNumber, pageSize);
        return Ok(new { Items = items, TotalCount = totalCount, PageNumber = pageNumber, PageSize = pageSize });
    }

    /// <summary>
    /// Create a new variant
    /// </summary>
    [HttpPost("variants")]
    public async Task<ActionResult<Variant>> CreateVariant(CreateVariantCommand command)
    {
        var variant = await _bus.InvokeAsync<Variant>(command);
        return CreatedAtAction(nameof(GetVariant), new { id = variant.Id }, variant);
    }

    /// <summary>
    /// Update an existing variant
    /// </summary>
    [HttpPut("variants/{id}")]
    public async Task<ActionResult<Variant>> UpdateVariant(Guid id, UpdateVariantCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        var variant = await _bus.InvokeAsync<Variant>(command);
        return Ok(variant);
    }

    /// <summary>
    /// Delete a variant
    /// </summary>
    [HttpDelete("variants/{id}")]
    public async Task<IActionResult> DeleteVariant(Guid id)
    {
        var command = new DeleteVariantCommand(id);
        await _bus.InvokeAsync(command);
        return NoContent();
    }

    /// <summary>
    /// Update variant stock
    /// </summary>
    [HttpPatch("variants/{id}/stock")]
    public async Task<IActionResult> UpdateVariantStock(Guid id, [FromBody] int newStockQuantity)
    {
        var command = new UpdateVariantStockCommand(id, newStockQuantity);
        await _bus.InvokeAsync(command);
        return NoContent();
    }

    #endregion
}
