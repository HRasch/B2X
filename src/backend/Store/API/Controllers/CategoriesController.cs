using B2X.Categories.Handlers;
using B2X.Store.ServiceClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2X.Store.Controllers;

/// <summary>
/// Read-only Categories API for Store Frontend
/// Provides access to product categories for navigation and browsing
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "StoreAccess")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoriesServiceClient _categoriesService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(
        ICategoriesServiceClient categoriesService,
        ILogger<CategoriesController> logger)
    {
        _categoriesService = categoriesService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a category by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(B2X.Categories.Models.CategoryDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCategoryById(Guid id, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        var category = await _categoriesService.GetCategoryByIdAsync(id, tenantId);
        if (category == null)
        {
            return NotFound(new { Message = "Category not found" });
        }

        return Ok(category);
    }

    /// <summary>
    /// Gets a category by slug
    /// </summary>
    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(B2X.Categories.Models.CategoryDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCategoryBySlug(string slug, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        var category = await _categoriesService.GetCategoryBySlugAsync(slug, tenantId);
        if (category == null)
        {
            return NotFound(new { Message = "Category not found" });
        }

        return Ok(category);
    }

    /// <summary>
    /// Gets categories with optional parent filter
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<B2X.Categories.Models.CategoryDto>), 200)]
    public async Task<IActionResult> GetCategories(
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromQuery] Guid? parentId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _categoriesService.GetCategoriesAsync(tenantId, parentId, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Gets the complete category tree
    /// </summary>
    [HttpGet("tree")]
    [ProducesResponseType(typeof(List<B2X.Categories.Models.CategoryDto>), 200)]
    public async Task<IActionResult> GetCategoryTree([FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        var tree = await _categoriesService.GetCategoryTreeAsync(tenantId);
        return Ok(tree);
    }
}
