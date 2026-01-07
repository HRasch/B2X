using B2Connect.Email.Infrastructure;
using B2Connect.Email.Interfaces;
using B2Connect.Email.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace B2Connect.Email.Services;

/// <summary>
/// Service für Email-Queue-Verwaltung
/// </summary>
public class EmailQueueService : IEmailQueueService
{
    private readonly EmailDbContext _dbContext;
    private readonly ILogger<EmailQueueService> _logger;

    public EmailQueueService(
        EmailDbContext dbContext,
        ILogger<EmailQueueService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task QueueEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        // Set scheduled time if not provided
        if (!message.ScheduledFor.HasValue)
        {
            message.ScheduledFor = DateTime.UtcNow;
        }

        // Set initial status
        message.Status = message.ScheduledFor > DateTime.UtcNow ? EmailStatus.Scheduled : EmailStatus.Queued;

        _dbContext.EmailMessages.Add(message);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Email {EmailId} queued for tenant {TenantId}", message.Id, message.TenantId);
    }

    /// <inheritdoc />
    public async Task<List<EmailMessage>> GetPendingEmailsAsync(int batchSize = 10, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var emails = await _dbContext.EmailMessages
            .Where(e => (e.Status == EmailStatus.Queued && e.ScheduledFor <= now) ||
                       (e.Status == EmailStatus.Scheduled && e.ScheduledFor <= now) ||
                       (e.Status == EmailStatus.Failed && e.NextRetryAt <= now && e.RetryCount < e.MaxRetries))
            .OrderByDescending(e => e.Priority)
            .ThenBy(e => e.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        // Mark as processing
        foreach (var email in emails)
        {
            email.Status = EmailStatus.Processing;
        }

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Retrieved {Count} emails for processing", emails.Count);

        return emails;
    }

    /// <inheritdoc />
    public async Task MarkEmailAsSentAsync(Guid emailId, string messageId, CancellationToken cancellationToken = default)
    {
        var email = await _dbContext.EmailMessages.FindAsync(new object[] { emailId }, cancellationToken).ConfigureAwait(false);
        if (email == null)
        {
            _logger.LogWarning("Email {EmailId} not found for marking as sent", emailId);
            return;
        }

        email.Status = EmailStatus.Sent;
        email.SentAt = DateTime.UtcNow;
        email.MessageId = messageId;

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Email {EmailId} marked as sent with message ID {MessageId}", emailId, messageId);
    }

    /// <inheritdoc />
    public async Task MarkEmailAsFailedAsync(Guid emailId, string errorMessage, CancellationToken cancellationToken = default)
    {
        var email = await _dbContext.EmailMessages.FindAsync(new object[] { emailId }, cancellationToken).ConfigureAwait(false);
        if (email == null)
        {
            _logger.LogWarning("Email {EmailId} not found for marking as failed", emailId);
            return;
        }

        email.RetryCount++;
        email.LastError = errorMessage;
        email.FailedAt = DateTime.UtcNow;

        if (email.RetryCount >= email.MaxRetries)
        {
            email.Status = EmailStatus.Failed;
            email.NextRetryAt = null;
            _logger.LogWarning("Email {EmailId} failed permanently after {RetryCount} retries", emailId, email.RetryCount);
        }
        else
        {
            // Exponential backoff: 5 minutes * 2^retryCount
            var delayMinutes = 5 * Math.Pow(2, email.RetryCount - 1);
            email.NextRetryAt = DateTime.UtcNow.AddMinutes(delayMinutes);
            email.Status = EmailStatus.Failed; // Will be retried later

            _logger.LogInformation("Email {EmailId} failed, scheduled for retry {RetryCount}/{MaxRetries} at {NextRetryAt}",
                emailId, email.RetryCount, email.MaxRetries, email.NextRetryAt);
        }

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task RetryEmailAsync(Guid emailId, CancellationToken cancellationToken = default)
    {
        var email = await _dbContext.EmailMessages.FindAsync(new object[] { emailId }, cancellationToken).ConfigureAwait(false);
        if (email == null)
        {
            _logger.LogWarning("Email {EmailId} not found for manual retry", emailId);
            return;
        }

        email.Status = EmailStatus.Queued;
        email.NextRetryAt = DateTime.UtcNow;
        email.LastError = null;

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Email {EmailId} manually queued for retry", emailId);
    }

    /// <inheritdoc />
    public async Task<List<EmailMessage>> GetEmailsForManagementAsync(
        Guid tenantId,
        EmailStatus? status = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.EmailMessages
            .Where(e => e.TenantId == tenantId);

        if (status.HasValue)
        {
            query = query.Where(e => e.Status == status.Value);
        }

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<EmailMessage?> GetEmailByIdAsync(Guid emailId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailMessages.FindAsync(new object[] { emailId }, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task CancelEmailAsync(Guid emailId, CancellationToken cancellationToken = default)
    {
        var email = await _dbContext.EmailMessages.FindAsync(new object[] { emailId }, cancellationToken).ConfigureAwait(false);
        if (email == null)
        {
            _logger.LogWarning("Email {EmailId} not found for cancellation", emailId);
            return;
        }

        if (email.Status == EmailStatus.Sent || email.Status == EmailStatus.Cancelled)
        {
            _logger.LogWarning("Cannot cancel email {EmailId} with status {Status}", emailId, email.Status);
            return;
        }

        email.Status = EmailStatus.Cancelled;

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Email {EmailId} cancelled", emailId);
    }
}
