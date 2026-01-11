using B2X.Services.Search.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace B2X.Services.Search.Controllers.Admin;

[ApiController]
[Route("v2/admin/seed")]
public class SeedController : ControllerBase
{
    private readonly ICatalogIndexer _indexer;
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _env;

    public SeedController(ICatalogIndexer indexer, IConfiguration config, IWebHostEnvironment env)
    {
        _indexer = indexer;
        _config = config;
        _env = env;
    }

    [HttpPost]
    public async Task<IActionResult> Seed([FromQuery] bool force = false, [FromHeader(Name = "X-Seed-Api-Key")] string? apiKey = null)
    {
        // Allow when in Development
        if (_env.IsDevelopment())
        {
            await _indexer.SeedAsync(force);
            return Ok(new { seeded = true, force });
        }

        // Allow when API key matches configured key
        var configured = _config["Catalog:SeedApiKey"];
        if (!string.IsNullOrWhiteSpace(configured) && apiKey == configured)
        {
            await _indexer.SeedAsync(force);
            return Ok(new { seeded = true, force, method = "apikey" });
        }

        // Allow when authenticated user has Admin role or admin claim
        var user = HttpContext.User;
        if (user?.Identity != null && user.Identity.IsAuthenticated)
        {
            if (user.IsInRole("Admin") || user.HasClaim(c => c.Type == "role" && c.Value == "admin") || user.HasClaim(c => c.Type == "roles" && c.Value == "Admin"))
            {
                await _indexer.SeedAsync(force);
                return Ok(new { seeded = true, force, method = "principal" });
            }
        }

        return Forbid();
    }
}
