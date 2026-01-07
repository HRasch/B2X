using B2X.Email.Models;

namespace B2X.Email.Interfaces;

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
