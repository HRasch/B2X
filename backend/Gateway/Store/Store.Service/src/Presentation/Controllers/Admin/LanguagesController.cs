using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2Connect.Store.Application.Store.Services;
using B2Connect.Store.Core.Common.Entities;

namespace B2Connect.Store.Presentation.Controllers.Admin;

[ApiController]
[Route("api/languages")]
[Authorize(Roles = "Admin")]
public class LanguagesController : ControllerBase
{
    private readonly ILanguageService _languageService;
    private readonly ILogger<LanguagesController> _logger;

    public LanguagesController(ILanguageService languageService, ILogger<LanguagesController> logger)
    {
        _languageService = languageService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ICollection<Language>>> GetLanguages(CancellationToken cancellationToken)
    {
        var languages = await _languageService.GetAllActiveLanguagesAsync(cancellationToken);
        return Ok(languages);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Language>> GetLanguage(Guid id, CancellationToken cancellationToken)
    {
        var language = await _languageService.GetLanguageByIdAsync(id, cancellationToken);
        if (language == null)
            return NotFound($"Language with ID '{id}' not found");

        return Ok(language);
    }

    [HttpGet("code/{code}")]
    [AllowAnonymous]
    public async Task<ActionResult<Language>> GetLanguageByCode(string code, CancellationToken cancellationToken)
    {
        var language = await _languageService.GetLanguageByCodeAsync(code, cancellationToken);
        if (language == null)
            return NotFound($"Language with code '{code}' not found");

        return Ok(language);
    }

    [HttpPost]
    public async Task<ActionResult<Language>> CreateLanguage([FromBody] Language language, CancellationToken cancellationToken)
    {
        var createdLanguage = await _languageService.CreateLanguageAsync(language, cancellationToken);
        return CreatedAtAction(nameof(GetLanguage), new { id = createdLanguage.Id }, createdLanguage);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Language>> UpdateLanguage(Guid id, [FromBody] Language language, CancellationToken cancellationToken)
    {
        if (id != language.Id)
            return BadRequest("ID mismatch");

        var updated = await _languageService.UpdateLanguageAsync(language, cancellationToken);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLanguage(Guid id, CancellationToken cancellationToken)
    {
        await _languageService.DeleteLanguageAsync(id, cancellationToken);
        return NoContent();
    }
}


