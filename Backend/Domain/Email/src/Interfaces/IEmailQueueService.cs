using B2X.Email.Models;

namespace B2X.Email.Interfaces;

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
