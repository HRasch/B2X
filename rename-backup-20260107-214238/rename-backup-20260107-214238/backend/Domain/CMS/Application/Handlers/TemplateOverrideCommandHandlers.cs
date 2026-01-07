using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using B2X.CMS.Application.Pages;
using B2X.CMS.Core.Domain.Pages;
using Microsoft.Extensions.Logging;

namespace B2X.CMS.Application.Handlers;

/// <summary>
/// Wolverine command handlers for template override operations (ADR-030)
/// </summary>
public class CreateTemplateOverrideHandler
{
    private readonly ITemplateRepository _repository;
    private readonly ITemplateValidationService _validationService;
    private readonly ILogger<CreateTemplateOverrideHandler> _logger;

    public CreateTemplateOverrideHandler(
        ITemplateRepository repository,
        ITemplateValidationService validationService,
        ILogger<CreateTemplateOverrideHandler> logger)
    {
        _repository = repository;
        _validationService = validationService;
        _logger = logger;
    }

    public async Task<TemplateOverrideResult> Handle(CreateTemplateOverrideCommand command)
    {
        _logger.LogInformation(
            "Creating template override: TenantId={TenantId}, Key={Key}",
            command.TenantId, command.TemplateKey);

        // Validate content first
        var validation = await _validationService.ValidateTemplateContentAsync(
            command.TenantId,
            command.TemplateKey,
            command.TemplateContent).ConfigureAwait(false);

        if (!validation.IsValid)
        {
            return new TemplateOverrideResult(
                false,
                "Validation failed",
                null,
                validation);
        }

        // Create page definition for the override
        var pageDefinition = new PageDefinition(
            command.TenantId,
            "template",
            $"/templates/{command.TemplateKey}",
            command.TemplateKey,
            command.TemplateContent)
        {
            IsTemplateOverride = true,
            BaseTemplateKey = command.BaseTemplateKey,
            OverrideSections = command.OverrideSections
        };

        var saved = await _repository.SaveOverrideAsync(pageDefinition).ConfigureAwait(false);

        return new TemplateOverrideResult(
            true,
            "Created",
            saved.Id,
            validation);
    }
}

/// <summary>
/// Handler for updating template overrides
/// </summary>
public class UpdateTemplateOverrideHandler
{
    private readonly ITemplateRepository _repository;
    private readonly ITemplateValidationService _validationService;
    private readonly ILogger<UpdateTemplateOverrideHandler> _logger;

    public UpdateTemplateOverrideHandler(
        ITemplateRepository repository,
        ITemplateValidationService validationService,
        ILogger<UpdateTemplateOverrideHandler> logger)
    {
        _repository = repository;
        _validationService = validationService;
        _logger = logger;
    }

    public async Task<TemplateOverrideResult> Handle(UpdateTemplateOverrideCommand command)
    {
        _logger.LogInformation(
            "Updating template override: TenantId={TenantId}, Key={Key}",
            command.TenantId, command.TemplateKey);

        var validation = await _validationService.ValidateTemplateContentAsync(
            command.TenantId,
            command.TemplateKey,
            command.TemplateContent).ConfigureAwait(false);

        if (!validation.IsValid)
        {
            return new TemplateOverrideResult(false, "Validation failed", null, validation);
        }

        // Get existing override or create new
        var existing = await _repository.GetTenantOverrideAsync(command.TenantId, command.TemplateKey).ConfigureAwait(false);

        var pageDefinition = existing ?? new PageDefinition(
            command.TenantId,
            "template",
            $"/templates/{command.TemplateKey}",
            command.TemplateKey,
            command.TemplateContent);

        pageDefinition.TemplateLayout = command.TemplateContent;
        pageDefinition.OverrideSections = command.OverrideSections;
        pageDefinition.Version++;

        await _repository.SaveOverrideAsync(pageDefinition).ConfigureAwait(false);

        return new TemplateOverrideResult(true, "Updated", pageDefinition.Id, validation);
    }
}

/// <summary>
/// Handler for publishing template overrides
/// </summary>
public class PublishTemplateOverrideHandler
{
    private readonly ITemplateRepository _repository;
    private readonly ILogger<PublishTemplateOverrideHandler> _logger;

    public PublishTemplateOverrideHandler(
        ITemplateRepository repository,
        ILogger<PublishTemplateOverrideHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TemplateOverrideResult> Handle(PublishTemplateOverrideCommand command)
    {
        _logger.LogInformation(
            "Publishing template override: TenantId={TenantId}, Key={Key}",
            command.TenantId, command.TemplateKey);

        await _repository.PublishOverrideAsync(command.TenantId, command.TemplateKey).ConfigureAwait(false);

        return new TemplateOverrideResult(true, "Published", null, null);
    }
}

/// <summary>
/// Handler for deleting template overrides
/// </summary>
public class DeleteTemplateOverrideHandler
{
    private readonly ITemplateRepository _repository;
    private readonly ILogger<DeleteTemplateOverrideHandler> _logger;

    public DeleteTemplateOverrideHandler(
        ITemplateRepository repository,
        ILogger<DeleteTemplateOverrideHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TemplateOverrideResult> Handle(DeleteTemplateOverrideCommand command)
    {
        _logger.LogInformation(
            "Deleting template override: TenantId={TenantId}, Key={Key}",
            command.TenantId, command.TemplateKey);

        await _repository.DeleteOverrideAsync(command.TenantId, command.TemplateKey).ConfigureAwait(false);

        return new TemplateOverrideResult(true, "Deleted", null, null);
    }
}

/// <summary>
/// Handler for validating template overrides
/// </summary>
public class ValidateTemplateOverrideHandler
{
    private readonly ITemplateRepository _repository;
    private readonly ITemplateValidationService _validationService;
    private readonly ILogger<ValidateTemplateOverrideHandler> _logger;

    public ValidateTemplateOverrideHandler(
        ITemplateRepository repository,
        ITemplateValidationService validationService,
        ILogger<ValidateTemplateOverrideHandler> logger)
    {
        _repository = repository;
        _validationService = validationService;
        _logger = logger;
    }

    public async Task<TemplateValidationResult> Handle(ValidateTemplateOverrideCommand command)
    {
        _logger.LogInformation(
            "Validating template override: TenantId={TenantId}, Key={Key}",
            command.TenantId, command.TemplateKey);

        var template = await _repository.GetTenantOverrideAsync(command.TenantId, command.TemplateKey).ConfigureAwait(false);

        if (template == null)
        {
            return new TemplateValidationResult
            {
                OverallStatus = TemplateValidationStatus.Invalid,
                ValidationResults = new List<ValidationIssue>
                {
                    new() { Type = ValidationType.Syntax, Severity = IssueSeverity.Error, Message = "Template not found" }
                }
            };
        }

        return await _validationService.ValidateTemplateAsync(template).ConfigureAwait(false);
    }
}

/// <summary>
/// Handler for validating template content directly
/// </summary>
public class ValidateTemplateContentHandler
{
    private readonly ITemplateValidationService _validationService;
    private readonly ILogger<ValidateTemplateContentHandler> _logger;

    public ValidateTemplateContentHandler(
        ITemplateValidationService validationService,
        ILogger<ValidateTemplateContentHandler> logger)
    {
        _validationService = validationService;
        _logger = logger;
    }

    public async Task<TemplateValidationResult> Handle(ValidateTemplateContentCommand command)
    {
        _logger.LogInformation(
            "Validating template content: TenantId={TenantId}, Key={Key}",
            command.TenantId, command.TemplateKey);

        return await _validationService.ValidateTemplateContentAsync(
            command.TenantId,
            command.TemplateKey,
            command.TemplateContent).ConfigureAwait(false);
    }
}

/// <summary>
/// Result of a template override operation
/// </summary>
public record TemplateOverrideResult(
    bool Success,
    string Message,
    string? Id,
    TemplateValidationResult? ValidationResult
);
