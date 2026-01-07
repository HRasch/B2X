using B2Connect.AppHost.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace B2Connect.AppHost.Controllers;

/// <summary>
/// API endpoints for test data seeding operations.
/// Provides frontend access to seeding functionality.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SeedingController : ControllerBase
{
    private readonly ITestDataOrchestrator _orchestrator;
    private readonly ILogger<SeedingController> _logger;

    public SeedingController(
        ITestDataOrchestrator orchestrator,
        ILogger<SeedingController> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    /// <summary>
    /// Seeds all test data in the correct order.
    /// </summary>
    [HttpPost("seed-all")]
    public async Task<IActionResult> SeedAll([FromQuery] bool force = false)
    {
        try
        {
            _logger.LogInformation("Starting full test data seeding (force: {Force})", force);

            await _orchestrator.SeedAllAsync(HttpContext.RequestAborted);

            var status = await _orchestrator.GetStatusAsync(HttpContext.RequestAborted);

            return Ok(new
            {
                success = true,
                operation = "seed-all",
                force,
                status
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during full test data seeding");
            return StatusCode(500, new
            {
                success = false,
                operation = "seed-all",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Seeds only core services (Auth, Tenant, Localization).
    /// </summary>
    [HttpPost("seed-core")]
    public async Task<IActionResult> SeedCore()
    {
        try
        {
            _logger.LogInformation("Starting core services test data seeding");

            await _orchestrator.SeedCoreServicesAsync(HttpContext.RequestAborted);

            var status = await _orchestrator.GetStatusAsync(HttpContext.RequestAborted);

            return Ok(new
            {
                success = true,
                operation = "seed-core",
                status
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during core services seeding");
            return StatusCode(500, new
            {
                success = false,
                operation = "seed-core",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Seeds catalog-related data (products, categories, etc.).
    /// </summary>
    [HttpPost("seed-catalog")]
    public async Task<IActionResult> SeedCatalog()
    {
        try
        {
            _logger.LogInformation("Starting catalog test data seeding");

            await _orchestrator.SeedCatalogAsync(HttpContext.RequestAborted);

            var status = await _orchestrator.GetStatusAsync(HttpContext.RequestAborted);

            return Ok(new
            {
                success = true,
                operation = "seed-catalog",
                status
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during catalog seeding");
            return StatusCode(500, new
            {
                success = false,
                operation = "seed-catalog",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Seeds CMS-related data (pages, templates, etc.).
    /// </summary>
    [HttpPost("seed-cms")]
    public async Task<IActionResult> SeedCms()
    {
        try
        {
            _logger.LogInformation("Starting CMS test data seeding");

            await _orchestrator.SeedCmsAsync(HttpContext.RequestAborted);

            var status = await _orchestrator.GetStatusAsync(HttpContext.RequestAborted);

            return Ok(new
            {
                success = true,
                operation = "seed-cms",
                status
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during CMS seeding");
            return StatusCode(500, new
            {
                success = false,
                operation = "seed-cms",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Clears all test data and resets to initial state.
    /// </summary>
    [HttpPost("reset")]
    public async Task<IActionResult> ResetAll()
    {
        try
        {
            _logger.LogInformation("Starting test data reset");

            await _orchestrator.ResetAllAsync(HttpContext.RequestAborted);

            var status = await _orchestrator.GetStatusAsync(HttpContext.RequestAborted);

            return Ok(new
            {
                success = true,
                operation = "reset",
                status
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during test data reset");
            return StatusCode(500, new
            {
                success = false,
                operation = "reset",
                error = ex.Message
            });
        }
    }

    /// <summary>
    /// Gets current seeding status information.
    /// </summary>
    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        try
        {
            var status = await _orchestrator.GetStatusAsync(HttpContext.RequestAborted);

            return Ok(new
            {
                success = true,
                status
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving seeding status");
            return StatusCode(500, new
            {
                success = false,
                error = ex.Message
            });
        }
    }
}