using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace B2Connect.Catalog.API.Controllers;

[ApiController]
[Route("api/catalog")]
public class DevCatalogController : ControllerBase
{
    private readonly B2Connect.Catalog.Infrastructure.Data.CatalogDbContext _db;

    public DevCatalogController(B2Connect.Catalog.Infrastructure.Data.CatalogDbContext db)
    {
        _db = db;
    }

    [HttpGet("imports")]
    public async Task<IActionResult> GetImports([FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        if (tenantId == Guid.Empty) return BadRequest(new { error = "X-Tenant-ID header is required" });

        var imports = await _db.CatalogImports
            .Where(i => i.TenantId == tenantId || i.TenantId == B2Connect.Shared.Core.SeedConstants.DefaultTenantId)
            .OrderByDescending(i => i.ImportTimestamp)
            .Select(i => new
            {
                id = i.Id,
                supplierId = i.SupplierId,
                catalogId = i.CatalogId,
                productCount = i.ProductCount,
                importTimestamp = i.ImportTimestamp
            })
            .ToListAsync();

        return Ok(imports);
    }

    [HttpGet("imports/{id:guid}/products")]
    public async Task<IActionResult> GetImportProducts(Guid id, [FromHeader(Name = "X-Tenant-ID")] Guid tenantId)
    {
        if (tenantId == Guid.Empty) return BadRequest(new { error = "X-Tenant-ID header is required" });

        var products = await _db.CatalogProducts
            .Where(p => p.CatalogImportId == id)
            .OrderBy(p => p.CreatedAt)
            .Select(p => new
            {
                id = p.Id,
                supplierAid = p.SupplierAid,
                productData = p.ProductData
            })
            .ToListAsync();

        return Ok(products);
    }
}
