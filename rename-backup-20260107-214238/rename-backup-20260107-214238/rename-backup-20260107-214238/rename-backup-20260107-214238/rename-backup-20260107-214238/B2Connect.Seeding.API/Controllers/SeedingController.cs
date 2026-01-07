using Microsoft.AspNetCore.Mvc;

namespace B2Connect.Seeding.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedingController : ControllerBase
{
    private readonly ILogger<SeedingController> _logger;

    public SeedingController(ILogger<SeedingController> logger)
    {
        _logger = logger;
    }

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        try
        {
            // Mock status response for frontend testing
            var status = new
            {
                status = new
                {
                    isSeeded = false,
                    lastSeededAt = (DateTime?)null,
                    seededWith = (string?)null,
                    tenantCount = 0,
                    userCount = 0,
                    productCount = 0
                }
            };

            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting seeding status");
            return StatusCode(500, new { error = "Failed to get seeding status", details = ex.Message });
        }
    }

    [HttpPost("seed-all")]
    public async Task<IActionResult> SeedAll()
    {
        try
        {
            _logger.LogInformation("Starting full seeding operation (mock)");
            await Task.Delay(2000); // Simulate work

            var result = new
            {
                success = true,
                message = "Full seeding completed successfully (mock)",
                duration = "2.1 seconds",
                operations = new[]
                {
                    new { operation = "seed-core", status = "completed", duration = "0.5s" },
                    new { operation = "seed-catalog", status = "completed", duration = "1.2s" },
                    new { operation = "seed-cms", status = "completed", duration = "0.4s" }
                }
            };

            _logger.LogInformation("Full seeding operation completed (mock)");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during full seeding operation");
            return StatusCode(500, new { error = "Seeding operation failed", details = ex.Message });
        }
    }

    [HttpPost("seed-core")]
    public async Task<IActionResult> SeedCore()
    {
        try
        {
            _logger.LogInformation("Starting core data seeding (mock)");
            await Task.Delay(500);

            var result = new
            {
                success = true,
                message = "Core data seeding completed (mock)",
                duration = "0.5 seconds"
            };

            _logger.LogInformation("Core data seeding completed (mock)");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during core data seeding");
            return StatusCode(500, new { error = "Core seeding failed", details = ex.Message });
        }
    }

    [HttpPost("seed-catalog")]
    public async Task<IActionResult> SeedCatalog()
    {
        try
        {
            _logger.LogInformation("Starting catalog seeding (mock)");
            await Task.Delay(1200);

            var result = new
            {
                success = true,
                message = "Catalog seeding completed (mock)",
                duration = "1.2 seconds"
            };

            _logger.LogInformation("Catalog seeding completed (mock)");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during catalog seeding");
            return StatusCode(500, new { error = "Catalog seeding failed", details = ex.Message });
        }
    }

    [HttpPost("seed-cms")]
    public async Task<IActionResult> SeedCms()
    {
        try
        {
            _logger.LogInformation("Starting CMS seeding (mock)");
            await Task.Delay(400);

            var result = new
            {
                success = true,
                message = "CMS seeding completed (mock)",
                duration = "0.4 seconds"
            };

            _logger.LogInformation("CMS seeding completed (mock)");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during CMS seeding");
            return StatusCode(500, new { error = "CMS seeding failed", details = ex.Message });
        }
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        try
        {
            _logger.LogInformation("Starting database reset (mock)");
            await Task.Delay(1000);

            var result = new
            {
                success = true,
                message = "Database reset completed (mock)",
                duration = "1.0 seconds"
            };

            _logger.LogInformation("Database reset completed (mock)");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during database reset");
            return StatusCode(500, new { error = "Reset failed", details = ex.Message });
        }
    }
}