using Microsoft.AspNetCore.Mvc;
using B2Connect.CatalogService.Services;

namespace B2Connect.CatalogService.Controllers;

/// <summary>
/// API Controller for Brand operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _service;
    private readonly ILogger<BrandsController> _logger;

    public BrandsController(IBrandService service, ILogger<BrandsController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets a brand by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProduceResponseType(typeof(BrandDto), StatusCodes.Status200OK)]
    [ProduceResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BrandDto>> GetBrand(Guid id)
    {
        var brand = await _service.GetBrandAsync(id);
        if (brand == null)
            return NotFound();

        return Ok(brand);
    }

    /// <summary>
    /// Gets a brand by slug
    /// </summary>
    [HttpGet("slug/{slug}")]
    [ProduceResponseType(typeof(BrandDto), StatusCodes.Status200OK)]
    [ProduceResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BrandDto>> GetBrandBySlug(string slug)
    {
        var brand = await _service.GetBrandBySlugAsync(slug);
        if (brand == null)
            return NotFound();

        return Ok(brand);
    }

    /// <summary>
    /// Gets all active brands
    /// </summary>
    [HttpGet]
    [ProduceResponseType(typeof(IEnumerable<BrandDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BrandDto>>> GetActiveBrands()
    {
        var brands = await _service.GetActiveBrandsAsync();
        return Ok(brands);
    }

    /// <summary>
    /// Gets brands with pagination
    /// </summary>
    [HttpGet("paged")]
    [ProduceResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetBrandsPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (items, total) = await _service.GetBrandsPagedAsync(pageNumber, pageSize);
        return Ok(new { items, total, pageNumber, pageSize });
    }

    /// <summary>
    /// Creates a new brand
    /// </summary>
    [HttpPost]
    [ProduceResponseType(typeof(BrandDto), StatusCodes.Status201Created)]
    [ProduceResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BrandDto>> CreateBrand(CreateBrandDto dto)
    {
        try
        {
            var brand = await _service.CreateBrandAsync(dto);
            return CreatedAtAction(nameof(GetBrand), new { id = brand.Id }, brand);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating brand");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing brand
    /// </summary>
    [HttpPut("{id}")]
    [ProduceResponseType(typeof(BrandDto), StatusCodes.Status200OK)]
    [ProduceResponseType(StatusCodes.Status404NotFound)]
    [ProduceResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BrandDto>> UpdateBrand(Guid id, UpdateBrandDto dto)
    {
        try
        {
            var brand = await _service.UpdateBrandAsync(id, dto);
            return Ok(brand);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating brand");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a brand
    /// </summary>
    [HttpDelete("{id}")]
    [ProduceResponseType(StatusCodes.Status204NoContent)]
    [ProduceResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteBrand(Guid id)
    {
        var success = await _service.DeleteBrandAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
