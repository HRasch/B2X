using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using B2Connect.Email.Interfaces;
using B2Connect.Email.Models;
using Microsoft.Extensions.Logging;

namespace B2Connect.Email.Services.Providers;

/// <summary>
/// Amazon SES email provider implementation
/// Uses AWS IAM authentication and SES API
/// </summary>
public class SesProvider : IEmailProvider
{
    private readonly IAmazonSimpleEmailService _sesClient;
    private readonly ILogger<SesProvider> _logger;
    private readonly string _region;

    public string ProviderName => "Amazon SES";

    public SesProvider(EmailProviderConfig config, ILogger<SesProvider> logger)
    {
        _region = config.AwsRegion ?? "us-east-1"; // Default region
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Create SES client with AWS SDK (IAM authentication handled automatically)
        var region = RegionEndpoint.GetBySystemName(_region);
        _sesClient = new AmazonSimpleEmailServiceClient(region);
    }

    /// <inheritdoc/>
    public async Task<EmailProviderResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        try
        {
            var request = CreateSendEmailRequest(message);

            _logger.LogInformation("Sending email via SES to {Recipient} in region {Region}",
                message.To.FirstOrDefault(), _region);

            var response = await _sesClient.SendEmailAsync(request, cancellationToken);

            _logger.LogInformation("Email sent successfully via SES. MessageId: {MessageId}", response.MessageId);

            return new EmailProviderResult
            {
                Success = true,
                ExternalMessageId = response.MessageId,
                ErrorMessage = null,
                IsRetryable = false
            };
        }
        catch (MessageRejectedException ex)
        {
            _logger.LogWarning(ex, "SES rejected message: {Message}");
            return new EmailProviderResult
            {
                Success = false,
                ExternalMessageId = null,
                ErrorMessage = $"Message rejected: {ex.Message}",
                IsRetryable = false // Permanent rejection
            };
        }
        catch (MailFromDomainNotVerifiedException ex)
        {
            _logger.LogError(ex, "SES domain not verified: {Domain}");
            return new EmailProviderResult
            {
                Success = false,
                ExternalMessageId = null,
                ErrorMessage = $"Domain not verified: {ex.Message}",
                IsRetryable = false // Configuration issue
            };
        }
        catch (AmazonSimpleEmailServiceException ex) when ((int)ex.StatusCode >= 500)
        {
            _logger.LogWarning(ex, "SES service error: {StatusCode} - {Message}");
            return new EmailProviderResult
            {
                Success = false,
                ExternalMessageId = null,
                ErrorMessage = $"SES service error: {ex.Message}",
                IsRetryable = true // Server errors are retryable
            };
        }
        catch (AmazonSimpleEmailServiceException ex) when ((int)ex.StatusCode == 429)
        {
            _logger.LogWarning(ex, "SES rate limit exceeded");
            return new EmailProviderResult
            {
                Success = false,
                ExternalMessageId = null,
                ErrorMessage = "Rate limit exceeded",
                IsRetryable = true // Throttling is retryable
            };
        }
        catch (AmazonSimpleEmailServiceException ex)
        {
            _logger.LogError(ex, "SES client error: {StatusCode} - {Message}");
            return new EmailProviderResult
            {
                Success = false,
                ExternalMessageId = null,
                ErrorMessage = $"SES error: {ex.Message}",
                IsRetryable = false // Client errors are not retryable
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending email via SES");
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
            // Check SES service status by attempting to get send quota
            var request = new GetSendQuotaRequest();
            await _sesClient.GetSendQuotaAsync(request, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private SendEmailRequest CreateSendEmailRequest(EmailMessage message)
    {
        var request = new SendEmailRequest
        {
            Source = message.From,
            Destination = new Destination
            {
                ToAddresses = message.To,
                CcAddresses = message.Cc,
                BccAddresses = message.Bcc
            },
            Message = new Amazon.SimpleEmail.Model.Message
            {
                Subject = new Content(message.Subject),
                Body = CreateBodyContent(message)
            }
        };

        return request;
    }

    private Body CreateBodyContent(EmailMessage message)
    {
        var body = new Body();

        if (message.IsHtml)
        {
            body.Html = new Content
            {
                Charset = "UTF-8",
                Data = message.Body
            };
        }
        else
        {
            body.Text = new Content
            {
                Charset = "UTF-8",
                Data = message.Body
            };
        }

        return body;
    }
}