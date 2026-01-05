using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using B2Connect.CMS.Core.Domain.Pages;

namespace B2Connect.CMS.Application.Pages;

/// <summary>
/// Repository for managing template overrides (ADR-030)
/// Handles hierarchical template resolution and version history
/// </summary>
public interface ITemplateRepository
{
    /// <summary>
    /// Get a template override for a specific tenant
    /// </summary>
    Task<PageDefinition?> GetTenantOverrideAsync(
        string tenantId,
        string templateKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get base template (system/default)
    /// </summary>
    Task<PageDefinition?> GetBaseTemplateAsync(
        string templateKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the full resolved template (tenant override + base)
    /// </summary>
    Task<PageDefinition?> GetResolvedTemplateAsync(
        string tenantId,
        string templateKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Create or update a template override
    /// </summary>
    Task<PageDefinition> SaveOverrideAsync(
        PageDefinition templateOverride,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publish a template override (make it live)
    /// </summary>
    Task PublishOverrideAsync(
        string tenantId,
        string templateKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get override version history
    /// </summary>
    Task<List<PageDefinition>> GetOverrideHistoryAsync(
        string tenantId,
        string templateKey,
        int limit = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revert to previous override version
    /// </summary>
    Task RevertToVersionAsync(
        string tenantId,
        string templateKey,
        int version,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a template override and revert to base template
    /// </summary>
    Task DeleteOverrideAsync(
        string tenantId,
        string templateKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all active overrides for a tenant
    /// </summary>
    Task<List<PageDefinition>> GetTenantOverridesAsync(
        string tenantId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Template validation service for syntax, security, and best practices checks (ADR-030)
/// </summary>
public interface ITemplateValidationService
{
    /// <summary>
    /// Validate a complete template override
    /// </summary>
    Task<TemplateValidationResult> ValidateTemplateAsync(
        PageDefinition template,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate raw template content
    /// </summary>
    Task<TemplateValidationResult> ValidateTemplateContentAsync(
        string tenantId,
        string templateKey,
        string content,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get AI-powered suggestions for template improvement
    /// </summary>
    Task<List<AiSuggestion>> GetSuggestionsAsync(
        PageDefinition template,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Record user acceptance/rejection of AI suggestion for learning
    /// </summary>
    Task RecordSuggestionFeedbackAsync(
        Guid suggestionId,
        bool accepted,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Service for hierarchical template resolution with caching (ADR-030)
/// </summary>
public interface ITemplateResolutionService
{
    /// <summary>
    /// Resolve template with tenant overrides applied
    /// </summary>
    Task<string> ResolveTemplateAsync(
        string tenantId,
        string templateKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get resolved template with metadata
    /// </summary>
    Task<PageDefinition> ResolveTemplateWithMetadataAsync(
        string tenantId,
        string templateKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidate template cache (call after update)
    /// </summary>
    Task InvalidateCacheAsync(
        string tenantId,
        string templateKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Merge tenant override sections into base template
    /// </summary>
    Task<string> MergeTemplatesAsync(
        PageDefinition baseTemplate,
        PageDefinition templateOverride,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Default implementation of template resolution service with hierarchical support
/// </summary>
public class HierarchicalTemplateResolver : ITemplateResolutionService
{
    private readonly ITemplateRepository _repository;
    private readonly ITemplateValidationService _validationService;
    private readonly ILogger<HierarchicalTemplateResolver> _logger;
    private readonly Dictionary<string, (string Content, DateTime Expires)> _cache = new();
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(15);

    public HierarchicalTemplateResolver(
        ITemplateRepository repository,
        ITemplateValidationService validationService,
        ILogger<HierarchicalTemplateResolver> logger)
    {
        _repository = repository;
        _validationService = validationService;
        _logger = logger;
    }

    public async Task<string> ResolveTemplateAsync(
        string tenantId,
        string templateKey,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{tenantId}:{templateKey}";

        // Check cache
        if (_cache.TryGetValue(cacheKey, out var cached) && cached.Expires > DateTime.UtcNow)
        {
            _logger.LogDebug("Cache hit for template {TemplateKey}", templateKey);
            return cached.Content;
        }

        // Resolve hierarchically
        var baseTemplate = await _repository.GetBaseTemplateAsync(templateKey, cancellationToken);
        if (baseTemplate == null)
        {
            throw new InvalidOperationException($"Base template '{templateKey}' not found");
        }

        var tenantOverride = await _repository.GetTenantOverrideAsync(tenantId, templateKey, cancellationToken);

        string resolved = tenantOverride != null
            ? await MergeTemplatesAsync(baseTemplate, tenantOverride, cancellationToken)
            : baseTemplate.TemplateLayout;

        // Cache result
        _cache[cacheKey] = (resolved, DateTime.UtcNow.Add(_cacheExpiration));

        _logger.LogInformation("Resolved template {TemplateKey} for tenant {TenantId}",
            templateKey, tenantId);

        return resolved;
    }

    public async Task<PageDefinition> ResolveTemplateWithMetadataAsync(
        string tenantId,
        string templateKey,
        CancellationToken cancellationToken = default)
    {
        var baseTemplate = await _repository.GetBaseTemplateAsync(templateKey, cancellationToken);
        if (baseTemplate == null)
        {
            throw new InvalidOperationException($"Base template '{templateKey}' not found");
        }

        var tenantOverride = await _repository.GetTenantOverrideAsync(tenantId, templateKey, cancellationToken);

        if (tenantOverride == null)
        {
            return baseTemplate;
        }

        // Return override with base template metadata merged
        if (tenantOverride.GlobalSettings != null && baseTemplate.GlobalSettings != null)
        {
            var merged = new Dictionary<string, object>(baseTemplate.GlobalSettings);
            foreach (var item in tenantOverride.GlobalSettings)
            {
                merged[item.Key] = item.Value;
            }
            tenantOverride.GlobalSettings = merged;
        }

        return tenantOverride;
    }

    public async Task InvalidateCacheAsync(
        string tenantId,
        string templateKey,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{tenantId}:{templateKey}";
        _cache.Remove(cacheKey);
        _logger.LogInformation("Invalidated cache for {CacheKey}", cacheKey);
        await Task.CompletedTask;
    }

    public async Task<string> MergeTemplatesAsync(
        PageDefinition baseTemplate,
        PageDefinition templateOverride,
        CancellationToken cancellationToken = default)
    {
        // Simple merge: replace specified sections, keep others from base
        var merged = baseTemplate.TemplateLayout;

        foreach (var section in templateOverride.OverrideSections)
        {
            // Simple placeholder-based replacement
            // In production, use a proper template engine (Liquid, Jinja, etc.)
            merged = merged.Replace($"{{{{ {section.Key} }}}}", section.Value);
        }

        _logger.LogDebug("Merged templates with {SectionCount} override sections",
            templateOverride.OverrideSections.Count);

        return await Task.FromResult(merged);
    }
}