using B2Connect.Email.Interfaces;
using B2Connect.Email.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace B2Connect.Email.Services.Providers;

/// <summary>
/// SendGrid email provider implementation
/// Uses SendGrid API with API key authentication
/// </summary>
public class SendGridProvider : IEmailProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SendGridProvider> _logger;
    private readonly string _apiKey;

    public string ProviderName => "SendGrid";

    public SendGridProvider(EmailProviderConfig config, ILogger<SendGridProvider> logger)
        : this(config, logger, null)
    {
    }

    public SendGridProvider(EmailProviderConfig config, ILogger<SendGridProvider> logger, HttpMessageHandler? handler)
    {
        _apiKey = config.ApiKey ?? throw new ArgumentNullException(nameof(config.ApiKey));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _httpClient = handler != null ? new HttpClient(handler) : new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api.sendgrid.com/v3/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <inheritdoc/>
    public async Task<EmailProviderResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        try
        {
            var sendGridMessage = CreateSendGridMessage(message);
            var json = JsonSerializer.Serialize(sendGridMessage);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending email via SendGrid to {Recipient}", message.To.FirstOrDefault());

            var response = await _httpClient.PostAsync("mail/send", content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var responseData = JsonSerializer.Deserialize<SendGridResponse>(responseContent);

                _logger.LogInformation("Email sent successfully via SendGrid. MessageId: {MessageId}", responseData?.MessageId);

                return new EmailProviderResult
                {
                    Success = true,
                    ExternalMessageId = responseData?.MessageId,
                    ErrorMessage = null,
                    IsRetryable = false
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("SendGrid API error. Status: {StatusCode}, Error: {Error}", response.StatusCode, errorContent);

                // Check if error is retryable
                var isRetryable = (int)response.StatusCode >= 500 || (int)response.StatusCode == 429;

                return new EmailProviderResult
                {
                    Success = false,
                    ExternalMessageId = null,
                    ErrorMessage = $"SendGrid API error: {response.StatusCode} - {errorContent}",
                    IsRetryable = isRetryable
                };
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error sending email via SendGrid");
            return new EmailProviderResult
            {
                Success = false,
                ExternalMessageId = null,
                ErrorMessage = $"Network error: {ex.Message}",
                IsRetryable = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending email via SendGrid");
            return new EmailProviderResult
            {
                Success = false,
                ExternalMessageId = null,
                ErrorMessage = $"Unexpected error: {ex.Message}",
                IsRetryable = false
            };
        }
    }

    /// <inheritdoc/>
    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Simple health check - try to get user profile
            var response = await _httpClient.GetAsync("user/profile", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private SendGridMessage CreateSendGridMessage(EmailMessage message)
    {
        return new SendGridMessage
        {
            Personalizations = new[]
            {
                new SendGridPersonalization
                {
                    To = message.To.Select(email => new SendGridEmail { Email = email }).ToArray(),
                    Cc = message.Cc?.Select(email => new SendGridEmail { Email = email }).ToArray(),
                    Bcc = message.Bcc?.Select(email => new SendGridEmail { Email = email }).ToArray(),
                    Subject = message.Subject
                }
            },
            From = new SendGridEmail { Email = message.From },
            Content = new[]
            {
                new SendGridContent
                {
                    Type = message.IsHtml ? "text/html" : "text/plain",
                    Value = message.Body
                }
            }
        };
    }
}

/// <summary>
/// SendGrid API message format
/// </summary>
internal class SendGridMessage
{
    public SendGridPersonalization[] Personalizations { get; set; } = Array.Empty<SendGridPersonalization>();
    public SendGridEmail From { get; set; } = new();
    public SendGridContent[] Content { get; set; } = Array.Empty<SendGridContent>();
}

internal class SendGridPersonalization
{
    public SendGridEmail[] To { get; set; } = Array.Empty<SendGridEmail>();
    public SendGridEmail[] Cc { get; set; } = Array.Empty<SendGridEmail>();
    public SendGridEmail[] Bcc { get; set; } = Array.Empty<SendGridEmail>();
    public string Subject { get; set; } = string.Empty;
}

internal class SendGridEmail
{
    public string Email { get; set; } = string.Empty;
}

internal class SendGridContent
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

internal class SendGridResponse
{
    public string MessageId { get; set; } = string.Empty;
}