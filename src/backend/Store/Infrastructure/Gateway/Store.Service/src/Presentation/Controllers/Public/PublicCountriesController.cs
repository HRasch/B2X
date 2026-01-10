using Microsoft.AspNetCore.Mvc;
using B2X.Store.Application.Store.ReadServices;
using Microsoft.Extensions.Logging;
using B2X.Store.Core.Common.Entities;
using Microsoft.Extensions.Logging;

namespace B2X.Store.Presentation.Controllers.Public;

[ApiController]
[Route("api/public/countries")]
public class PublicCountriesController : ControllerBase
{
    private readonly ICountryReadService _countryReadService;
    private readonly ILogger<PublicCountriesController> _logger;

    public PublicCountriesController(ICountryReadService countryReadService, ILogger<PublicCountriesController> logger)
    {
        _countryReadService = countryReadService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<Country>>> GetCountries(CancellationToken cancellationToken)
    {
        var countries = await _countryReadService.GetAllCountriesAsync(cancellationToken);
        return Ok(countries);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Country>> GetCountry(Guid id, CancellationToken cancellationToken)
    {
        var country = await _countryReadService.GetCountryByIdAsync(id, cancellationToken);
        if (country == null)
            return NotFound($"Country with ID '{id}' not found");

        return Ok(country);
    }

    [HttpGet("code/{code}")]
    public async Task<ActionResult<Country>> GetCountryByCode(string code, CancellationToken cancellationToken)
    {
        var country = await _countryReadService.GetCountryByCodeAsync(code, cancellationToken);
        if (country == null)
            return NotFound($"Country with code '{code}' not found");

        return Ok(country);
    }

    [HttpGet("region/{region}")]
    public async Task<ActionResult<ICollection<Country>>> GetCountriesByRegion(string region, CancellationToken cancellationToken)
    {
        var countries = await _countryReadService.GetCountriesByRegionAsync(region, cancellationToken);
        return Ok(countries);
    }

    [HttpGet("regions")]
    public async Task<ActionResult<ICollection<string>>> GetAvailableRegions(CancellationToken cancellationToken)
    {
        var regions = await _countryReadService.GetAvailableRegionsAsync(cancellationToken);
        return Ok(regions);
    }

    [HttpGet("shipping")]
    public async Task<ActionResult<ICollection<Country>>> GetShippingCountries(CancellationToken cancellationToken)
    {
        var countries = await _countryReadService.GetShippingCountriesAsync(cancellationToken);
        return Ok(countries);
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCountryCount(CancellationToken cancellationToken)
    {
        var count = await _countryReadService.GetCountryCountAsync(cancellationToken);
        return Ok(count);
    }
}


