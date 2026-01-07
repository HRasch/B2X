using B2X.Email.Models;

namespace B2X.Email.Interfaces;

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
