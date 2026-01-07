using B2Connect.Email.Infrastructure;
using B2Connect.Email.Interfaces;
using B2Connect.Email.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace B2Connect.Email.Services;

/// <summary>
/// Service für Email-Template Verwaltung
/// </summary>
public class EmailTemplateService : IEmailTemplateService
{
    private readonly EmailDbContext _dbContext;
    private readonly ILogger<EmailTemplateService> _logger;

    public EmailTemplateService(
        EmailDbContext dbContext,
        ILogger<EmailTemplateService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<EmailTemplate>> GetTemplatesAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailTemplates
            .Where(t => t.TenantId == tenantId)
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<EmailTemplate?> GetTemplateAsync(
        Guid tenantId,
        Guid templateId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailTemplates
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.Id == templateId, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<EmailTemplate?> GetTemplateByKeyAsync(
        Guid tenantId,
        string key,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailTemplates
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.Key == key && t.IsActive, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<EmailTemplate> CreateTemplateAsync(
        Guid tenantId,
        CreateEmailTemplateDto dto,
        string? createdBy = null,
        CancellationToken cancellationToken = default)
    {
        // Check if key already exists
        var existingTemplate = await _dbContext.EmailTemplates
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.Key == dto.Key, cancellationToken).ConfigureAwait(false);

        if (existingTemplate != null)
        {
            throw new InvalidOperationException($"Template with key '{dto.Key}' already exists for tenant {tenantId}");
        }

        var template = new EmailTemplate
        {
            TenantId = tenantId,
            Name = dto.Name,
            Key = dto.Key,
            Subject = dto.Subject,
            Body = dto.Body,
            IsHtml = dto.IsHtml,
            Variables = dto.Variables,
            Description = dto.Description,
            CreatedBy = createdBy,
            UpdatedBy = createdBy
        };

        _dbContext.EmailTemplates.Add(template);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Created email template {TemplateId} for tenant {TenantId}", template.Id, tenantId);

        return template;
    }

    /// <inheritdoc/>
    public async Task<EmailTemplate> UpdateTemplateAsync(
        Guid tenantId,
        Guid templateId,
        UpdateEmailTemplateDto dto,
        string? updatedBy = null,
        CancellationToken cancellationToken = default)
    {
        var template = await _dbContext.EmailTemplates
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.Id == templateId, cancellationToken).ConfigureAwait(false);

        if (template == null)
        {
            throw new KeyNotFoundException($"Template {templateId} not found for tenant {tenantId}");
        }

        template.Name = dto.Name;
        template.Subject = dto.Subject;
        template.Body = dto.Body;
        template.IsHtml = dto.IsHtml;
        template.Variables = dto.Variables;
        template.Description = dto.Description;
        template.IsActive = dto.IsActive;
        template.UpdatedAt = DateTime.UtcNow;
        template.UpdatedBy = updatedBy;

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Updated email template {TemplateId} for tenant {TenantId}", templateId, tenantId);

        return template;
    }

    /// <inheritdoc/>
    public async Task DeleteTemplateAsync(
        Guid tenantId,
        Guid templateId,
        CancellationToken cancellationToken = default)
    {
        var template = await _dbContext.EmailTemplates
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.Id == templateId, cancellationToken).ConfigureAwait(false);

        if (template == null)
        {
            throw new KeyNotFoundException($"Template {templateId} not found for tenant {tenantId}");
        }

        _dbContext.EmailTemplates.Remove(template);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Deleted email template {TemplateId} for tenant {TenantId}", templateId, tenantId);
    }

    /// <inheritdoc/>
    public async Task<EmailTemplate> ToggleTemplateStatusAsync(
        Guid tenantId,
        Guid templateId,
        bool isActive,
        string? updatedBy = null,
        CancellationToken cancellationToken = default)
    {
        var template = await _dbContext.EmailTemplates
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.Id == templateId, cancellationToken).ConfigureAwait(false);

        if (template == null)
        {
            throw new KeyNotFoundException($"Template {templateId} not found for tenant {tenantId}");
        }

        template.IsActive = isActive;
        template.UpdatedAt = DateTime.UtcNow;
        template.UpdatedBy = updatedBy;

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Toggled email template {TemplateId} status to {IsActive} for tenant {TenantId}",
            templateId, isActive, tenantId);

        return template;
    }

    /// <inheritdoc/>
    public async Task<EmailMessage> TestTemplateAsync(
        Guid tenantId,
        Guid templateId,
        string testEmail,
        Dictionary<string, object>? testVariables = null,
        CancellationToken cancellationToken = default)
    {
        var template = await _dbContext.EmailTemplates
            .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.Id == templateId, cancellationToken).ConfigureAwait(false);

        if (template == null)
        {
            throw new KeyNotFoundException($"Template {templateId} not found for tenant {tenantId}");
        }

        // Merge template variables with test variables
        var variables = new Dictionary<string, object>(template.Variables
            .ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value, StringComparer.Ordinal), StringComparer.Ordinal);

        if (testVariables != null)
        {
            foreach (var kvp in testVariables)
            {
                variables[kvp.Key] = kvp.Value;
            }
        }

        // Create test email message
        var emailMessage = new EmailMessage
        {
            TenantId = tenantId,
            To = testEmail,
            Subject = ReplaceVariables(template.Subject, variables),
            Body = ReplaceVariables(template.Body, variables),
            IsHtml = template.IsHtml,
            TemplateKey = template.Key,
            Variables = variables,
            Status = EmailStatus.Queued
        };

        return emailMessage;
    }

    private static string ReplaceVariables(string template, Dictionary<string, object> variables)
    {
        var result = template;
        foreach (var variable in variables)
        {
            result = result.Replace($"{{{variable.Key}}}", variable.Value?.ToString() ?? string.Empty);
        }
        return result;
    }
}
