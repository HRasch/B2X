using Microsoft.AspNetCore.Mvc;
using B2Connect.Store.Application.Store.ReadServices;
using B2Connect.Store.Core.Common.Entities;

namespace B2Connect.Store.Presentation.Controllers.Public;

[ApiController]
[Route("api/public/languages")]
public class PublicLanguagesController : ControllerBase
{
    private readonly ILanguageReadService _languageReadService;
    private readonly ILogger<PublicLanguagesController> _logger;

    public PublicLanguagesController(ILanguageReadService languageReadService, ILogger<PublicLanguagesController> logger)
    {
        _languageReadService = languageReadService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<Language>>> GetLanguages(CancellationToken cancellationToken)
    {
        var languages = await _languageReadService.GetAllLanguagesAsync(cancellationToken);
        return Ok(languages);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Language>> GetLanguage(Guid id, CancellationToken cancellationToken)
    {
        var language = await _languageReadService.GetLanguageByIdAsync(id, cancellationToken);
        if (language == null)
            return NotFound($"Language with ID '{id}' not found");

        return Ok(language);
    }

    [HttpGet("code/{code}")]
    public async Task<ActionResult<Language>> GetLanguageByCode(string code, CancellationToken cancellationToken)
    {
        var language = await _languageReadService.GetLanguageByCodeAsync(code, cancellationToken);
        if (language == null)
            return NotFound($"Language with code '{code}' not found");

        return Ok(language);
    }

    [HttpGet("default")]
    public async Task<ActionResult<Language>> GetDefaultLanguage(CancellationToken cancellationToken)
    {
        var language = await _languageReadService.GetDefaultLanguageAsync(cancellationToken);
        if (language == null)
            return NotFound("Default language not found");

        return Ok(language);
    }

    [HttpGet("store/{storeId}")]
    public async Task<ActionResult<ICollection<Language>>> GetLanguagesByStore(Guid storeId, CancellationToken cancellationToken)
    {
        var languages = await _languageReadService.GetLanguagesByStoreAsync(storeId, cancellationToken);
        return Ok(languages);
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> GetLanguageCount(CancellationToken cancellationToken)
    {
        var count = await _languageReadService.GetLanguageCountAsync(cancellationToken);
        return Ok(count);
    }
}


