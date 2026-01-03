using B2Connect.Email.Models;

namespace B2Connect.Email.Interfaces;

/// <summary>
/// Service für das Versenden von Emails mit Monitoring
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sendet eine Email und zeichnet das Ergebnis auf
    /// </summary>
    Task<EmailSendResult> SendEmailAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sendet eine Test-Email an eine beliebige Adresse
    /// </summary>
    Task<EmailSendResult> SendTestEmailAsync(
        string toEmail,
        string subject,
        string body,
        bool isHtml = true,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Service für Email-Monitoring und Analytics
/// </summary>
public interface IEmailMonitoringService
{
    /// <summary>
    /// Zeichnet ein Email-Event auf (gesendet, zugestellt, geöffnet, geklickt, etc.)
    /// </summary>
    Task RecordEmailEventAsync(
        Guid emailId,
        EmailEventType eventType,
        string? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Holt Email-Statistiken für einen Zeitraum
    /// </summary>
    Task<EmailStatistics> GetEmailStatisticsAsync(
        Guid tenantId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Holt die letzten Email-Events für Monitoring
    /// </summary>
    Task<List<EmailEvent>> GetRecentEmailEventsAsync(
        Guid tenantId,
        int limit = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Holt Email-Fehler für Troubleshooting
    /// </summary>
    Task<List<EmailEvent>> GetEmailErrorsAsync(
        Guid tenantId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Service für Email-Queue-Verwaltung
/// </summary>
public interface IEmailQueueService
{
    /// <summary>
    /// Fügt eine Email zur Queue hinzu
    /// </summary>
    Task QueueEmailAsync(EmailMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Holt die nächsten Emails zur Verarbeitung
    /// </summary>
    Task<List<EmailMessage>> GetPendingEmailsAsync(int batchSize = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Markiert eine Email als erfolgreich versendet
    /// </summary>
    Task MarkEmailAsSentAsync(Guid emailId, string messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Markiert eine Email als fehlgeschlagen und plant Retry
    /// </summary>
    Task MarkEmailAsFailedAsync(Guid emailId, string errorMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Setzt eine Email für manuellen Retry zurück
    /// </summary>
    Task RetryEmailAsync(Guid emailId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Holt Emails für Management-Interface
    /// </summary>
    Task<List<EmailMessage>> GetEmailsForManagementAsync(
        Guid tenantId,
        EmailStatus? status = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Holt eine Email nach ID
    /// </summary>
    Task<EmailMessage?> GetEmailByIdAsync(Guid emailId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Storniert eine Email
    /// </summary>
    Task CancelEmailAsync(Guid emailId, CancellationToken cancellationToken = default);
}