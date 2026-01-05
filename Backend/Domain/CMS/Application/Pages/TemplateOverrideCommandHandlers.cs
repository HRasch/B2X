using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using B2Connect.CMS.Core.Domain.Pages;
using B2Connect.CMS.Core.Domain;
using B2Connect.Shared.Tenancy.Infrastructure.Context;
using B2Connect.CMS.Application.Handlers;
using Wolverine;

namespace B2Connect.CMS.Application.Pages;

/// <summary>
/// Handler for creating template overrides (ADR-030 Phase 1)
/// </summary>
public class CreateTemplateOverrideCommandHandler : ICommandHandler<CreateTemplateOverrideCommand, PageDefinition>
{
    private readonly ITemplateRepository _templateRepository;
    private readonly ITemplateValidationService _validationService;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<CreateTemplateOverrideCommandHandler> _logger;

    public CreateTemplateOverrideCommandHandler(
        ITemplateRepository templateRepository,
        ITemplateValidationService validationService,
        TenantContext tenantContext,
        ILogger<CreateTemplateOverrideCommandHandler> logger)
    {
        _templateRepository = templateRepository;
        _validationService = validationService;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<PageDefinition> Handle(
        CreateTemplateOverrideCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating template override: TenantId={TenantId}, TemplateKey={TemplateKey}",
            command.TenantId, command.TemplateKey);

        // Get base template
        var baseTemplate = await _templateRepository.GetBaseTemplateAsync(
            command.TemplateKey, cancellationToken);

        if (baseTemplate == null)
        {
            throw new InvalidOperationException(
                $"Base template '{command.TemplateKey}' not found");
        }

        // Create override
        var templateOverride = new PageDefinition
        {
            TenantId = command.TenantId,
            PageType = baseTemplate.PageType,
            PagePath = baseTemplate.PagePath,
            PageTitle = baseTemplate.PageTitle,
            TemplateLayout = command.TemplateContent,
            IsTemplateOverride = true,
            BaseTemplateKey = command.TemplateKey,
            OverrideSections = command.OverrideSections,
            OverrideMetadata = new TemplateOverrideMetadata
            {
                CreatedBy = command.CreatedBy,
                ValidationStatus = TemplateValidationStatus.Pending
            },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 1
        };

        // Save override
        templateOverride = await _templateRepository.SaveOverrideAsync(templateOverride, cancellationToken);

        // Validate asynchronously (don't block creation)
        _ = Task.Run(async () =>
        {
            var validationResult = await _validationService.ValidateTemplateAsync(
                templateOverride, cancellationToken);
            templateOverride.OverrideMetadata!.ValidationStatus = validationResult.OverallStatus;
            templateOverride.OverrideMetadata!.ValidationResults = validationResult.ValidationResults
                .Select(v => $"{v.Severity}: {v.Message}")
                .ToList();
            await _templateRepository.SaveOverrideAsync(templateOverride, cancellationToken);
        }, cancellationToken);

        _logger.LogInformation(
            "Template override created: TenantId={TenantId}, TemplateKey={TemplateKey}",
            command.TenantId, command.TemplateKey);

        return templateOverride;
    }
}

/// <summary>
/// Handler for publishing template overrides
/// </summary>
public class PublishTemplateOverrideCommandHandler : ICommandHandler<PublishTemplateOverrideCommand, Unit>
{
    private readonly ITemplateRepository _templateRepository;
    private readonly ITemplateResolutionService _resolutionService;
    private readonly ILogger<PublishTemplateOverrideCommandHandler> _logger;

    public PublishTemplateOverrideCommandHandler(
        ITemplateRepository templateRepository,
        ITemplateResolutionService resolutionService,
        ILogger<PublishTemplateOverrideCommandHandler> logger)
    {
        _templateRepository = templateRepository;
        _resolutionService = resolutionService;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        PublishTemplateOverrideCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Publishing template override: TenantId={TenantId}, TemplateKey={TemplateKey}",
            command.TenantId, command.TemplateKey);

        var templateOverride = await _templateRepository.GetTenantOverrideAsync(
            command.TenantId, command.TemplateKey, cancellationToken);

        if (templateOverride == null)
        {
            throw new InvalidOperationException(
                $"Template override '{command.TemplateKey}' not found for tenant {command.TenantId}");
        }

        if (templateOverride.OverrideMetadata?.ValidationStatus == TemplateValidationStatus.Invalid)
        {
            throw new InvalidOperationException(
                "Cannot publish invalid template. Please fix validation errors first.");
        }

        // Mark as live
        templateOverride.IsPublished = true;
        templateOverride.PublishedAt = DateTime.UtcNow;
        if (templateOverride.OverrideMetadata != null)
        {
            templateOverride.OverrideMetadata.IsLive = true;
            templateOverride.OverrideMetadata.PublishedAt = DateTime.UtcNow;
        }

        await _templateRepository.SaveOverrideAsync(templateOverride, cancellationToken);

        // Invalidate cache to force reload
        await _resolutionService.InvalidateCacheAsync(
            command.TenantId, command.TemplateKey, cancellationToken);

        _logger.LogInformation(
            "Template override published: TenantId={TenantId}, TemplateKey={TemplateKey}",
            command.TenantId, command.TemplateKey);

        return Unit.Value;
    }
}