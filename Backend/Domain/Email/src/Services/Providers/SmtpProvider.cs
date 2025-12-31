using B2Connect.Email.Interfaces;
using B2Connect.Email.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace B2Connect.Email.Services.Providers;

/// <summary>
/// SMTP email provider implementation
/// Uses SMTP protocol with TLS authentication
/// </summary>
public class SmtpProvider : IEmailProvider
{
    private readonly SmtpConfig _config;
    private readonly ILogger<SmtpProvider> _logger;

    public string ProviderName => "SMTP";

    public SmtpProvider(EmailProviderConfig config, ILogger<SmtpProvider> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _config = new SmtpConfig
        {
            Host = config.SmtpHost ?? throw new ArgumentNullException(nameof(config.SmtpHost)),
            Port = config.SmtpPort ?? 587,
            Username = config.Username ?? throw new ArgumentNullException(nameof(config.Username)),
            Password = config.Password ?? throw new ArgumentNullException(nameof(config.Password)),
            UseSsl = config.UseSsl ?? true,
            RequireTls = true // Always require TLS for security
        };
    }

    /// <inheritdoc/>
    public async Task<EmailProviderResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        try
        {
            using var client = new SmtpClient();

            // Set up protocol logger for debugging (only in development)
#if DEBUG
            // Protocol logging removed - ProtocolLogger is read-only in newer MailKit versions
#endif

            _logger.LogInformation("Connecting to SMTP server {Host}:{Port}", _config.Host, _config.Port);

            // Connect with TLS
            await client.ConnectAsync(_config.Host, _config.Port, _config.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None, cancellationToken);

            // Authenticate
            if (!string.IsNullOrEmpty(_config.Username))
            {
                _logger.LogDebug("Authenticating with SMTP server");
                await client.AuthenticateAsync(_config.Username, _config.Password, cancellationToken);
            }

            // Create MIME message
            var mimeMessage = CreateMimeMessage(message);

            _logger.LogInformation("Sending email via SMTP to {Recipient}", message.To.FirstOrDefault());

            // Send message
            var messageId = await client.SendAsync(mimeMessage, cancellationToken);

            _logger.LogInformation("Email sent successfully via SMTP. MessageId: {MessageId}", messageId);

            await client.DisconnectAsync(true, cancellationToken);

            return new EmailProviderResult
            {
                Success = true,
                ExternalMessageId = messageId,
                ErrorMessage = null,
                IsRetryable = false
            };
        }
        catch (MailKit.Security.AuthenticationException ex)
        {
            _logger.LogError(ex, "SMTP authentication failed");
            return new EmailProviderResult
            {
                Success = false,
                ExternalMessageId = null,
                ErrorMessage = $"Authentication failed: {ex.Message}",
                IsRetryable = false // Auth failures are permanent
            };
        }
        catch (MailKit.Net.Smtp.SmtpProtocolException ex)
        {
            _logger.LogWarning(ex, "SMTP protocol error: {Message}");

            // Protocol errors are generally retryable unless they indicate permanent issues
            var isRetryable = !ex.Message.Contains("authentication", StringComparison.OrdinalIgnoreCase) &&
                            !ex.Message.Contains("mailbox", StringComparison.OrdinalIgnoreCase);

            return new EmailProviderResult
            {
                Success = false,
                ExternalMessageId = null,
                ErrorMessage = $"SMTP protocol error: {ex.Message}",
                IsRetryable = isRetryable
            };
        }
        catch (MailKit.Net.Smtp.SmtpCommandException ex)
        {
            _logger.LogWarning(ex, "SMTP command error: {StatusCode} - {Message}");

            // Temporary server errors are retryable
            var isRetryable = (int)ex.StatusCode >= 400 && (int)ex.StatusCode < 500;

            return new EmailProviderResult
            {
                Success = false,
                ExternalMessageId = null,
                ErrorMessage = $"SMTP command error: {ex.Message}",
                IsRetryable = isRetryable
            };
        }
        catch (System.Net.Sockets.SocketException ex)
        {
            _logger.LogError(ex, "SMTP network error");
            return new EmailProviderResult
            {
                Success = false,
                ExternalMessageId = null,
                ErrorMessage = $"Network error: {ex.Message}",
                IsRetryable = true // Network issues are retryable
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending email via SMTP");
            return new EmailProviderResult
            {
                Success = false,
                ExternalMessageId = null,
                ErrorMessage = $"Unexpected error: {ex.Message}",
                IsRetryable = true // Unknown errors might be transient
            };
        }
    }

    /// <inheritdoc/>
    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = new SmtpClient();

            // Try to connect without authentication for health check
            await client.ConnectAsync(_config.Host, _config.Port,
                _config.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
                cancellationToken);

            await client.DisconnectAsync(true, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private MimeMessage CreateMimeMessage(EmailMessage message)
    {
        var mimeMessage = new MimeMessage();

        // Set sender
        mimeMessage.From.Add(new MailboxAddress("", message.From));

        // Set recipients
        foreach (var to in message.To)
        {
            mimeMessage.To.Add(new MailboxAddress("", to));
        }

        if (message.Cc != null)
        {
            foreach (var cc in message.Cc)
            {
                mimeMessage.Cc.Add(new MailboxAddress("", cc));
            }
        }

        if (message.Bcc != null)
        {
            foreach (var bcc in message.Bcc)
            {
                mimeMessage.Bcc.Add(new MailboxAddress("", bcc));
            }
        }

        // Set subject
        mimeMessage.Subject = message.Subject;

        // Set body
        var bodyBuilder = new BodyBuilder();

        if (message.IsHtml)
        {
            bodyBuilder.HtmlBody = message.Body;
        }
        else
        {
            bodyBuilder.TextBody = message.Body;
        }

        mimeMessage.Body = bodyBuilder.ToMessageBody();

        return mimeMessage;
    }
}

/// <summary>
/// SMTP configuration
/// </summary>
public class SmtpConfig
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool UseSsl { get; set; } = true;
    public bool RequireTls { get; set; } = true;
}