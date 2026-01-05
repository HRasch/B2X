using B2Connect.Email.Models;

namespace B2Connect.Email.Interfaces;

/// <summary>
/// Service für Email-Template Verwaltung
/// </summary>
public interface IEmailTemplateService
{
    /// <summary>
    /// Holt alle Email-Templates für einen Tenant
    /// </summary>
    Task<IEnumerable<EmailTemplate>> GetTemplatesAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Holt ein spezifisches Email-Template
    /// </summary>
    Task<EmailTemplate?> GetTemplateAsync(
        Guid tenantId,
        Guid templateId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Holt ein Template anhand des Keys
    /// </summary>
    Task<EmailTemplate?> GetTemplateByKeyAsync(
        Guid tenantId,
        string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Erstellt ein neues Email-Template
    /// </summary>
    Task<EmailTemplate> CreateTemplateAsync(
        Guid tenantId,
        CreateEmailTemplateDto dto,
        string? createdBy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Aktualisiert ein bestehendes Email-Template
    /// </summary>
    Task<EmailTemplate> UpdateTemplateAsync(
        Guid tenantId,
        Guid templateId,
        UpdateEmailTemplateDto dto,
        string? updatedBy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Löscht ein Email-Template
    /// </summary>
    Task DeleteTemplateAsync(
        Guid tenantId,
        Guid templateId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Aktiviert/Deaktiviert ein Template
    /// </summary>
    Task<EmailTemplate> ToggleTemplateStatusAsync(
        Guid tenantId,
        Guid templateId,
        bool isActive,
        string? updatedBy = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Testet ein Template mit Beispiel-Daten
    /// </summary>
    Task<EmailMessage> TestTemplateAsync(
        Guid tenantId,
        Guid templateId,
        string testEmail,
        Dictionary<string, object>? testVariables = null,
        CancellationToken cancellationToken = default);
}