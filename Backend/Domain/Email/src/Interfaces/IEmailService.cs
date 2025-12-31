using B2Connect.Email.Models;

namespace B2Connect.Email.Interfaces;

/// <summary>
/// Interface for email service implementations
/// Handles the core business logic for email operations
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send an email message
    /// </summary>
    Task<bool> SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get an email message by ID
    /// </summary>
    Task<EmailMessage?> GetEmailAsync(string messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get pending emails for a tenant
    /// </summary>
    Task<IEnumerable<EmailMessage>> GetPendingEmailsAsync(string tenantId, int limit = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retry sending failed emails
    /// </summary>
    Task<int> RetryFailedEmailsAsync(string tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update email status
    /// </summary>
    Task UpdateEmailStatusAsync(string messageId, EmailStatus status, string? errorMessage = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancel a scheduled email
    /// </summary>
    Task<bool> CancelEmailAsync(string messageId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for email provider factory
/// Creates provider instances based on configuration
/// </summary>
public interface IEmailProviderFactory
{
    /// <summary>
    /// Create an email provider instance
    /// </summary>
    IEmailProvider CreateProvider(EmailProviderConfig config);
}

/// <summary>
/// Interface for email provider (SMTP, SendGrid, AWS SES, etc.)
/// Handles actual email transmission
/// </summary>
public interface IEmailProvider
{
    /// <summary>
    /// Send an email through the provider
    /// </summary>
    Task<EmailProviderResult> SendAsync(EmailMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get provider name
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Check if provider is configured and available
    /// </summary>
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Result from email provider
/// </summary>
public class EmailProviderResult
{
    /// <summary>
    /// Whether sending was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// External message ID from provider
    /// </summary>
    public string? ExternalMessageId { get; set; }

    /// <summary>
    /// Error message if sending failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Whether the error is retryable
    /// </summary>
    public bool IsRetryable { get; set; }
}

/// <summary>
/// Interface for email repository
/// Handles data access for email messages
/// </summary>
public interface IEmailRepository
{
    /// <summary>
    /// Add a new email message
    /// </summary>
    Task AddAsync(EmailMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing email message
    /// </summary>
    Task UpdateAsync(EmailMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get email by ID
    /// </summary>
    Task<EmailMessage?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get pending emails for a tenant
    /// </summary>
    Task<IEnumerable<EmailMessage>> GetPendingByTenantAsync(string tenantId, int limit = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get failed emails for retry
    /// </summary>
    Task<IEnumerable<EmailMessage>> GetFailedForRetryAsync(string tenantId, int maxAttempts = 3, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete an email message
    /// </summary>
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
