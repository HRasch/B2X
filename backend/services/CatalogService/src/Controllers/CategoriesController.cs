using Microsoft.AspNetCore.Mvc;
using B2Connect.CatalogService.Services;

namespace B2Connect.CatalogService.Controllers;

/// <summary>
/// API Controller for Category operations
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
    [ProduceResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProduceResponseType(StatusCodes.Status404NotFound)]
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
    [ProduceResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProduceResponseType(StatusCodes.Status404NotFound)]
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
    [ProduceResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetRootCategories()
    {
        var categories = await _service.GetRootCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Gets child categories of a parent
    /// </summary>
    [HttpGet("{parentId}/children")]
    [ProduceResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetChildCategories(Guid parentId)
    {
        var categories = await _service.GetChildCategoriesAsync(parentId);
        return Ok(categories);
    }

    /// <summary>
    /// Gets the complete category hierarchy
    /// </summary>
    [HttpGet("hierarchy")]
    [ProduceResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetHierarchy()
    {
        var categories = await _service.GetCategoryHierarchyAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Gets all active categories
    /// </summary>
    [HttpGet]
    [ProduceResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetActiveCategories()
    {
        var categories = await _service.GetActiveCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Creates a new category
    /// </summary>
    [HttpPost]
    [ProduceResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProduceResponseType(StatusCodes.Status400BadRequest)]
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
    [ProduceResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProduceResponseType(StatusCodes.Status404NotFound)]
    [ProduceResponseType(StatusCodes.Status400BadRequest)]
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
    [ProduceResponseType(StatusCodes.Status204NoContent)]
    [ProduceResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteCategory(Guid id)
    {
        var success = await _service.DeleteCategoryAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
