using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2X.Store.Application.Store.Services;
using Microsoft.Extensions.Logging;
using B2X.Store.Core.Common.Entities;
using Microsoft.Extensions.Logging;

namespace B2X.Store.Presentation.Controllers.Admin;

[ApiController]
[Route("api/countries")]
[Authorize(Roles = "Admin")]
public class CountriesController : ControllerBase
{
    private readonly ICountryService _countryService;
    private readonly ILogger<CountriesController> _logger;

    public CountriesController(ICountryService countryService, ILogger<CountriesController> logger)
    {
        _countryService = countryService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ICollection<Country>>> GetCountries(CancellationToken cancellationToken)
    {
        var countries = await _countryService.GetAllActiveCountriesAsync(cancellationToken);
        return Ok(countries);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Country>> GetCountry(Guid id, CancellationToken cancellationToken)
    {
        var country = await _countryService.GetCountryByIdAsync(id, cancellationToken);
        if (country == null)
            return NotFound($"Country with ID '{id}' not found");

        return Ok(country);
    }

    [HttpGet("code/{code}")]
    [AllowAnonymous]
    public async Task<ActionResult<Country>> GetCountryByCode(string code, CancellationToken cancellationToken)
    {
        var country = await _countryService.GetCountryByCodeAsync(code, cancellationToken);
        if (country == null)
            return NotFound($"Country with code '{code}' not found");

        return Ok(country);
    }

    [HttpGet("region/{region}")]
    [AllowAnonymous]
    public async Task<ActionResult<ICollection<Country>>> GetCountriesByRegion(string region, CancellationToken cancellationToken)
    {
        var countries = await _countryService.GetCountriesByRegionAsync(region, cancellationToken);
        return Ok(countries);
    }

    [HttpGet("shipping")]
    [AllowAnonymous]
    public async Task<ActionResult<ICollection<Country>>> GetShippingCountries(CancellationToken cancellationToken)
    {
        var countries = await _countryService.GetShippingCountriesAsync(cancellationToken);
        return Ok(countries);
    }

    [HttpPost]
    public async Task<ActionResult<Country>> CreateCountry([FromBody] Country country, CancellationToken cancellationToken)
    {
        var createdCountry = await _countryService.CreateCountryAsync(country, cancellationToken);
        return CreatedAtAction(nameof(GetCountry), new { id = createdCountry.Id }, createdCountry);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Country>> UpdateCountry(Guid id, [FromBody] Country country, CancellationToken cancellationToken)
    {
        if (id != country.Id)
            return BadRequest("ID mismatch");

        var updated = await _countryService.UpdateCountryAsync(country, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCountry(Guid id, CancellationToken cancellationToken)
    {
        await _countryService.DeleteCountryAsync(id, cancellationToken);
        return NoContent();
    }
}


