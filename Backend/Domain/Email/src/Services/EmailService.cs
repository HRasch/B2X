using B2Connect.Email.Models;
using B2Connect.Email.Interfaces;
using Microsoft.Extensions.Logging;

namespace B2Connect.Email.Services;

/// <summary>
/// Default email service implementation
/// Coordinates email sending across different providers with retry logic and persistence
/// </summary>
public class EmailService : IEmailService
{
    private readonly IEmailRepository _repository;
    private readonly IEmailProvider _provider;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IEmailRepository repository,
        IEmailProvider provider,
        ILogger<EmailService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<bool> SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        try
        {
            // Check if provider is available
            var isAvailable = await _provider.IsAvailableAsync(cancellationToken);
            if (!isAvailable)
            {
                _logger.LogWarning("Email provider {ProviderName} is not available", _provider.ProviderName);
                message.Status = EmailStatus.Deferred;
                await _repository.UpdateAsync(message, cancellationToken);
                return false;
            }

            // Mark as sending
            message.Status = EmailStatus.Sending;
            message.SendAttempts++;
            message.LastAttemptAt = DateTime.UtcNow;
            await _repository.UpdateAsync(message, cancellationToken);

            // Send through provider
            var result = await _provider.SendAsync(message, cancellationToken);

            if (result.Success)
            {
                message.Status = EmailStatus.Sent;
                message.SentAt = DateTime.UtcNow;
                message.ExternalMessageId = result.ExternalMessageId;
                await _repository.UpdateAsync(message, cancellationToken);

                _logger.LogInformation(
                    "Email sent successfully. MessageId: {MessageId}, TenantId: {TenantId}, Recipient: {Recipient}",
                    message.Id, message.TenantId, message.RecipientEmail);
                return true;
            }

            // Handle failure
            _logger.LogWarning(
                "Email sending failed. MessageId: {MessageId}, Attempt: {Attempt}/{MaxAttempts}, Error: {Error}",
                message.Id, message.SendAttempts, message.MaxSendAttempts, result.ErrorMessage);

            message.LastError = result.ErrorMessage;

            if (result.IsRetryable && message.SendAttempts < message.MaxSendAttempts)
            {
                message.Status = EmailStatus.Deferred;
            }
            else
            {
                message.Status = EmailStatus.Failed;
            }

            await _repository.UpdateAsync(message, cancellationToken);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending email. MessageId: {MessageId}", message.Id);
            message.Status = EmailStatus.Failed;
            message.LastError = ex.Message;
            await _repository.UpdateAsync(message, cancellationToken);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<EmailMessage?> GetEmailAsync(string messageId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(messageId))
            throw new ArgumentException("MessageId cannot be empty", nameof(messageId));

        return await _repository.GetByIdAsync(messageId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<EmailMessage>> GetPendingEmailsAsync(
        string tenantId, int limit = 100, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));

        return await _repository.GetPendingByTenantAsync(tenantId, limit, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> RetryFailedEmailsAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));

        var failedEmails = await _repository.GetFailedForRetryAsync(tenantId, cancellationToken: cancellationToken);
        var count = 0;

        foreach (var email in failedEmails)
        {
            var sent = await SendEmailAsync(email, cancellationToken);
            if (sent) count++;
        }

        return count;
    }

    /// <inheritdoc/>
    public async Task UpdateEmailStatusAsync(
        string messageId, EmailStatus status, string? errorMessage = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(messageId))
            throw new ArgumentException("MessageId cannot be empty", nameof(messageId));

        var message = await _repository.GetByIdAsync(messageId, cancellationToken);
        if (message == null)
        {
            _logger.LogWarning("Email not found for status update. MessageId: {MessageId}", messageId);
            return;
        }

        message.Status = status;
        if (!string.IsNullOrWhiteSpace(errorMessage))
            message.LastError = errorMessage;

        await _repository.UpdateAsync(message, cancellationToken);
        _logger.LogInformation("Email status updated. MessageId: {MessageId}, Status: {Status}", messageId, status);
    }

    /// <inheritdoc/>
    public async Task<bool> CancelEmailAsync(string messageId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(messageId))
            throw new ArgumentException("MessageId cannot be empty", nameof(messageId));

        var message = await _repository.GetByIdAsync(messageId, cancellationToken);
        if (message == null)
        {
            _logger.LogWarning("Email not found for cancellation. MessageId: {MessageId}", messageId);
            return false;
        }

        // Only pending or deferred emails can be cancelled
        if (message.Status != EmailStatus.Pending && message.Status != EmailStatus.Deferred)
        {
            _logger.LogWarning(
                "Cannot cancel email with status {Status}. MessageId: {MessageId}",
                message.Status, messageId);
            return false;
        }

        message.Status = EmailStatus.Cancelled;
        await _repository.UpdateAsync(message, cancellationToken);
        _logger.LogInformation("Email cancelled. MessageId: {MessageId}", messageId);
        return true;
    }
}
