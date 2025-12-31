using B2Connect.Domain.Search.Models;
using B2Connect.Domain.Search.Services;
using Microsoft.AspNetCore.Mvc;

namespace B2Connect.Services.Search.Controllers;

[ApiController]
[Route("api/v1/products")]
public class ProductSearchController : ControllerBase
{
    private readonly IElasticService _elastic;
    private readonly ITenantResolver _tenantResolver;

    public ProductSearchController(IElasticService elastic, ITenantResolver tenantResolver)
    {
        _elastic = elastic;
        _tenantResolver = tenantResolver;
    }

    [HttpGet]
    public async Task<IActionResult> Browse([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var tenantId = Request.Headers["X-Tenant-ID"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            var host = Request.Host.Host;
            tenantId = _tenantResolver.ResolveTenantIdFromHost(host) ?? tenantId;
        }
        var locale = Request.Headers["X-Locale"].FirstOrDefault() ?? Request.Headers["Accept-Language"].FirstOrDefault()?.Split(',')[0];
        var result = await _elastic.SearchAsync(new SearchRequestDto { Query = "*", Page = page, PageSize = pageSize, Locale = locale }, tenantId, locale);
        return Ok(result);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchRequestDto request)
    {
        var tenantId = Request.Headers["X-Tenant-ID"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            var host = Request.Host.Host;
            tenantId = _tenantResolver.ResolveTenantIdFromHost(host) ?? tenantId;
        }
        var locale = request.Locale ?? Request.Headers["X-Locale"].FirstOrDefault() ?? Request.Headers["Accept-Language"].FirstOrDefault()?.Split(',')[0];
        var result = await _elastic.SearchAsync(request, tenantId, locale);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var tenantId = Request.Headers["X-Tenant-ID"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            var host = Request.Host.Host;
            tenantId = _tenantResolver.ResolveTenantIdFromHost(host) ?? tenantId;
        }
        var locale = Request.Headers["X-Locale"].FirstOrDefault() ?? Request.Headers["Accept-Language"].FirstOrDefault()?.Split(',')[0];
        var doc = await _elastic.GetByIdAsync(id, tenantId, locale);
        if (doc == null) return NotFound(new { code = "NotFound", message = "Product not found" });
        return Ok(doc);
    }
}
