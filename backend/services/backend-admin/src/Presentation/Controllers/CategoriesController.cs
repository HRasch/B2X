using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using B2Connect.Admin.Application.Services;
using B2Connect.Admin.Core.Entities;

namespace B2Connect.Admin.Presentation.Controllers;

/// <summary>
/// API Controller for Category operations
/// Public GET endpoints for store, admin CRUD endpoints require Admin role
/// Uses AOP filters for logging, validation, and exception handling
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategoryService service, ILogger<CategoriesController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets a category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(Guid id)
    {
        var category = await _service.GetCategoryAsync(id);
        if (category == null)
            return NotFound();

        return Ok(category);
    }

    /// <summary>
    /// Gets a category by slug
    /// </summary>
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryBySlug(string slug)
    {
        var category = await _service.GetCategoryBySlugAsync(slug);
        if (category == null)
            return NotFound();

        return Ok(category);
    }

    /// <summary>
    /// Gets all root categories
    /// </summary>
    [HttpGet("root")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetRootCategories()
    {
        var categories = await _service.GetRootCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Gets child categories of a parent
    /// </summary>
    [HttpGet("{parentId}/children")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetChildCategories(Guid parentId)
    {
        var categories = await _service.GetChildCategoriesAsync(parentId);
        return Ok(categories);
    }

    /// <summary>
    /// Gets the complete category hierarchy
    /// </summary>
    [HttpGet("hierarchy")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetHierarchy()
    {
        var categories = await _service.GetCategoryHierarchyAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Gets all active categories
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetActiveCategories()
    {
        var categories = await _service.GetActiveCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Creates a new category
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto dto)
    {
        try
        {
            var category = await _service.CreateCategoryAsync(dto);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing category
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(Guid id, UpdateCategoryDto dto)
    {
        try
        {
            var category = await _service.UpdateCategoryAsync(id, dto);
            return Ok(category);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a category
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCategory(Guid id)
    {
        var success = await _service.DeleteCategoryAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
