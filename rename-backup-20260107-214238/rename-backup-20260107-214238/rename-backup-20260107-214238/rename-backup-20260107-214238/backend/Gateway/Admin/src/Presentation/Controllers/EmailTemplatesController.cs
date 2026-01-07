using B2X.Admin.Presentation.Filters;
using B2X.Email.Interfaces;
using B2X.Email.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2X.Admin.Presentation.Controllers;

/// <summary>
/// Controller für Email-Template Verwaltung
/// </summary>
[Route("api/admin/email/templates")]
[ApiController]
[ValidateTenant]
[Authorize]
public class EmailTemplatesController : ApiControllerBase
{
    private readonly IEmailTemplateService _templateService;
    private readonly IEmailService _emailService;

    public EmailTemplatesController(
        ILogger<EmailTemplatesController> logger,
        IEmailTemplateService templateService,
        IEmailService emailService) : base(logger)
    {
        _templateService = templateService;
        _emailService = emailService;
    }

    /// <summary>
    /// Holt alle Email-Templates für den aktuellen Tenant
    /// HTTP: GET /api/admin/email/templates
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmailTemplate>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTemplates(
        CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();
        var templates = await _templateService.GetTemplatesAsync(tenantId, cancellationToken);

        return Ok(templates);
    }

    /// <summary>
    /// Holt ein spezifisches Email-Template
    /// HTTP: GET /api/admin/email/templates/{id}
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EmailTemplate), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTemplate(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();
        var template = await _templateService.GetTemplateAsync(tenantId, id, cancellationToken);

        if (template == null)
        {
            return NotFound($"Template with ID {id} not found");
        }

        return Ok(template);
    }

    /// <summary>
    /// Holt ein Template anhand des Keys
    /// HTTP: GET /api/admin/email/templates/key/{key}
    /// </summary>
    [HttpGet("key/{key}")]
    [ProducesResponseType(typeof(EmailTemplate), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTemplateByKey(
        string key,
        CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();
        var template = await _templateService.GetTemplateByKeyAsync(tenantId, key, cancellationToken);

        if (template == null)
        {
            return NotFound($"Template with key '{key}' not found");
        }

        return Ok(template);
    }

    /// <summary>
    /// Erstellt ein neues Email-Template
    /// HTTP: POST /api/admin/email/templates
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmailTemplate), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateTemplate(
        [FromBody] CreateEmailTemplateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId().ToString();
            var template = await _templateService.CreateTemplateAsync(tenantId, dto, userId, cancellationToken);

            return CreatedAtAction(nameof(GetTemplate), new { id = template.Id }, template);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Aktualisiert ein bestehendes Email-Template
    /// HTTP: PUT /api/admin/email/templates/{id}
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EmailTemplate), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTemplate(
        Guid id,
        [FromBody] UpdateEmailTemplateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId().ToString();
            var template = await _templateService.UpdateTemplateAsync(tenantId, id, dto, userId, cancellationToken);

            return Ok(template);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Löscht ein Email-Template
    /// HTTP: DELETE /api/admin/email/templates/{id}
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTemplate(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = GetTenantId();
            await _templateService.DeleteTemplateAsync(tenantId, id, cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Aktiviert/Deaktiviert ein Template
    /// HTTP: PATCH /api/admin/email/templates/{id}/status
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(EmailTemplate), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleTemplateStatus(
        Guid id,
        [FromBody] ToggleStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = GetTenantId();
            var userId = GetUserId().ToString();
            var template = await _templateService.ToggleTemplateStatusAsync(
                tenantId, id, request.IsActive, userId, cancellationToken);

            return Ok(template);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Testet ein Template mit Beispiel-Daten
    /// HTTP: POST /api/admin/email/templates/{id}/test
    /// </summary>
    [HttpPost("{id:guid}/test")]
    [ProducesResponseType(typeof(EmailMessage), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TestTemplate(
        Guid id,
        [FromBody] TestTemplateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.TestEmail))
        {
            return BadRequest("Test email address is required");
        }

        try
        {
            var tenantId = GetTenantId();
            var testMessage = await _templateService.TestTemplateAsync(
                tenantId, id, request.TestEmail, request.TestVariables, cancellationToken);

            // Send the test email
            var result = await _emailService.SendEmailAsync(testMessage, cancellationToken);

            return Ok(new
            {
                TestMessage = testMessage,
                SendResult = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

/// <summary>
/// Request DTO für Status-Änderung
/// </summary>
public class ToggleStatusRequest
{
    public bool IsActive { get; set; }
}

/// <summary>
/// Request DTO für Template-Test
/// </summary>
public class TestTemplateRequest
{
    public string TestEmail { get; set; } = string.Empty;
    public Dictionary<string, object>? TestVariables { get; set; }
}
