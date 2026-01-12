using B2X.Catalog.Application.Commands;
using B2X.Catalog.Models;
using B2X.Types.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

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
    private readonly IMessageBus _messageBus;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(
        IMessageBus messageBus,
        ILogger<CategoriesController> logger)
    {
        _messageBus = messageBus;
        _logger = logger;
    }

    /// <summary>
    /// Gets a category by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CategoryDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCategoryById(Guid id, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        try
        {
            var query = new GetCategoryByIdQuery(id, tenantId);
            var category = await _messageBus.InvokeAsync<CategoryDto>(query);

            if (category == null)
            {
                return NotFound(new { Message = "Category not found" });
            }

            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category {Id}", id);
            return NotFound(new { Message = "Category not found" });
        }
    }

    /// <summary>
    /// Gets a category by slug
    /// </summary>
    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(CategoryDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCategoryBySlug(string slug, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        try
        {
            var query = new GetCategoryBySlugQuery(slug, tenantId);
            var category = await _messageBus.InvokeAsync<CategoryDto>(query);

            if (category == null)
            {
                return NotFound(new { Message = "Category not found" });
            }

            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category by slug {Slug}", slug);
            return NotFound(new { Message = "Category not found" });
        }
    }

    /// <summary>
    /// Gets categories with optional parent filter
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CategoryDto>), 200)]
    public async Task<IActionResult> GetCategories(
        [FromHeader(Name = "X-Tenant-ID")] Guid tenantId,
        [FromQuery] Guid? parentId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new GetCategoriesQuery(tenantId, null, parentId, null, page, pageSize);
            var result = await _messageBus.InvokeAsync<PagedResult<CategoryDto>>(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories");
            return BadRequest(new { Message = "Failed to retrieve categories" });
        }
    }

    /// <summary>
    /// Gets the complete category tree
    /// </summary>
    [HttpGet("tree")]
    [ProducesResponseType(typeof(List<CategoryDto>), 200)]
    public async Task<IActionResult> GetCategoryTree([FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        try
        {
            var query = new GetCategoryTreeQuery(tenantId);
            var tree = await _messageBus.InvokeAsync<IEnumerable<CategoryDto>>(query);
            return Ok(tree);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category tree");
            return BadRequest(new { Message = "Failed to retrieve category tree" });
        }
    }
}
