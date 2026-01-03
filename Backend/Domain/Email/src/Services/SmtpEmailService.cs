using B2Connect.Email.Interfaces;
using B2Connect.Email.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace B2Connect.Email.Services;

/// <summary>
/// Service für das Versenden von Emails über SMTP mit Monitoring
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly ILogger<SmtpEmailService> _logger;
    private readonly IEmailMonitoringService _monitoringService;
    private readonly SmtpSettings _smtpSettings;

    public SmtpEmailService(
        ILogger<SmtpEmailService> logger,
        IEmailMonitoringService monitoringService,
        IOptions<SmtpSettings> smtpSettings)
    {
        _logger = logger;
        _monitoringService = monitoringService;
        _smtpSettings = smtpSettings.Value;
    }

    public async Task<EmailSendResult> SendEmailAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        var result = new EmailSendResult();

        try
        {
            using var smtpClient = CreateSmtpClient();
            using var mailMessage = CreateMailMessage(message);

            _logger.LogInformation("Sending email to {To} with subject '{Subject}'",
                message.To, message.Subject);

            await smtpClient.SendMailAsync(mailMessage, cancellationToken);

            result.Success = true;
            result.Status = EmailSendStatus.Sent;
            result.MessageId = mailMessage.Headers["Message-ID"];

            _logger.LogInformation("Email sent successfully. MessageId: {MessageId}",
                result.MessageId);

            // Record successful send event
            await _monitoringService.RecordEmailEventAsync(
                message.Id,
                EmailEventType.Sent,
                $"MessageId: {result.MessageId}",
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", message.To);

            result.Success = false;
            result.Status = EmailSendStatus.Failed;
            result.ErrorMessage = ex.Message;

            // Record failure event
            await _monitoringService.RecordEmailEventAsync(
                message.Id,
                EmailEventType.Failed,
                $"Error: {ex.Message}",
                cancellationToken);
        }

        return result;
    }

    public async Task<EmailSendResult> SendTestEmailAsync(
        string toEmail,
        string subject,
        string body,
        bool isHtml = true,
        CancellationToken cancellationToken = default)
    {
        var testMessage = new EmailMessage
        {
            TenantId = Guid.Empty, // Test emails don't belong to a specific tenant
            To = toEmail,
            Subject = $"[TEST] {subject}",
            Body = body,
            IsHtml = isHtml,
            TemplateKey = "test"
        };

        var result = await SendEmailAsync(testMessage, cancellationToken);

        // Mark as test in monitoring
        if (_monitoringService != null)
        {
            await _monitoringService.RecordEmailEventAsync(
                testMessage.Id,
                result.Success ? EmailEventType.Sent : EmailEventType.Failed,
                "[TEST EMAIL]",
                cancellationToken);
        }

        return result;
    }

    private SmtpClient CreateSmtpClient()
    {
        return new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
        {
            Credentials = new System.Net.NetworkCredential(
                _smtpSettings.Username,
                _smtpSettings.Password),
            EnableSsl = _smtpSettings.EnableSsl,
            Timeout = _smtpSettings.Timeout
        };
    }

    private MailMessage CreateMailMessage(EmailMessage message)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName),
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = message.IsHtml
        };

        mailMessage.To.Add(message.To);

        if (!string.IsNullOrEmpty(message.Cc))
            mailMessage.CC.Add(message.Cc);

        if (!string.IsNullOrEmpty(message.Bcc))
            mailMessage.Bcc.Add(message.Bcc);

        // Set priority
        mailMessage.Priority = message.Priority switch
        {
            EmailPriority.Low => MailPriority.Low,
            EmailPriority.Normal => MailPriority.Normal,
            EmailPriority.High => MailPriority.High,
            EmailPriority.Urgent => MailPriority.High,
            _ => MailPriority.Normal
        };

        // Add attachments
        if (message.Attachments != null)
        {
            foreach (var attachment in message.Attachments)
            {
                var mailAttachment = new Attachment(
                    new MemoryStream(attachment.Content),
                    attachment.FileName,
                    attachment.ContentType);
                mailMessage.Attachments.Add(mailAttachment);
            }
        }

        // Add message ID for tracking
        mailMessage.Headers.Add("Message-ID", $"<{message.Id}@{_smtpSettings.Domain}>");

        return mailMessage;
    }
}

/// <summary>
/// SMTP-Konfiguration
/// </summary>
public class SmtpSettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
    public int Timeout { get; set; } = 30000; // 30 seconds
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public string Domain { get; set; } = "b2connect.local";
}