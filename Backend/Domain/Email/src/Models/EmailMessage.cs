namespace B2Connect.Email.Models;

/// <summary>
/// Supported email provider types
/// </summary>
public enum EmailProviderType
{
    /// <summary>
    /// SendGrid email service (API Key authentication)
    /// </summary>
    SendGrid,

    /// <summary>
    /// Mailgun email service (API Key authentication)
    /// </summary>
    Mailgun,

    /// <summary>
    /// Postmark email service (API Key authentication)
    /// </summary>
    Postmark,

    /// <summary>
    /// Amazon SES (AWS IAM authentication)
    /// </summary>
    AmazonSes,

    /// <summary>
    /// Microsoft Graph API (OAuth2 authentication)
    /// </summary>
    MicrosoftGraph,

    /// <summary>
    /// Gmail API (OAuth2 authentication)
    /// </summary>
    Gmail,

    /// <summary>
    /// SMTP server (Basic authentication)
    /// </summary>
    Smtp,

    /// <summary>
    /// Azure Communication Services (API Key + OAuth2)
    /// </summary>
    AzureCommunication
}

/// <summary>
/// Email message domain model.
/// Represents a single email that needs to be sent or has been sent.
/// </summary>
public class EmailMessage
{
    /// <summary>
    /// Unique identifier for the email message
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Tenant ID for multi-tenancy isolation
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Recipient email address
    /// </summary>
    public string RecipientEmail { get; set; } = string.Empty;

    /// <summary>
    /// Recipient name (optional)
    /// </summary>
    public string? RecipientName { get; set; }

    /// <summary>
    /// Email subject line
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Email body (HTML content)
    /// </summary>
    public string HtmlBody { get; set; } = string.Empty;

    /// <summary>
    /// Email body (plain text content)
    /// </summary>
    public string? PlainTextBody { get; set; }

    /// <summary>
    /// Email type/template name for tracking
    /// </summary>
    public string EmailType { get; set; } = string.Empty;

    /// <summary>
    /// Sender email address
    /// </summary>
    public string SenderEmail { get; set; } = "noreply@b2connect.local";

    /// <summary>
    /// Sender name
    /// </summary>
    public string? SenderName { get; set; }

    /// <summary>
    /// CC recipients (semicolon-separated)
    /// </summary>
    public string? CcRecipients { get; set; }

    /// <summary>
    /// BCC recipients (semicolon-separated)
    /// </summary>
    public string? BccRecipients { get; set; }

    /// <summary>
    /// Reply-to email address
    /// </summary>
    public string? ReplyToEmail { get; set; }

    /// <summary>
    /// Current status of the email
    /// </summary>
    public EmailStatus Status { get; set; } = EmailStatus.Pending;

    /// <summary>
    /// Number of send attempts
    /// </summary>
    public int SendAttempts { get; set; }

    /// <summary>
    /// Maximum number of send attempts before giving up
    /// </summary>
    public int MaxSendAttempts { get; set; } = 3;

    /// <summary>
    /// Last error message if send failed
    /// </summary>
    public string? LastError { get; set; }

    /// <summary>
    /// Timestamp when email was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when email was scheduled to send
    /// </summary>
    public DateTime? ScheduledFor { get; set; }

    /// <summary>
    /// Timestamp when email was successfully sent
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Timestamp of last send attempt
    /// </summary>
    public DateTime? LastAttemptAt { get; set; }

    /// <summary>
    /// External message ID from email provider (e.g., SendGrid, AWS SES)
    /// </summary>
    public string? ExternalMessageId { get; set; }

    /// <summary>
    /// Metadata for tracking and analytics
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }

    /// <summary>
    /// From address (convenience property)
    /// </summary>
    public string From => $"{SenderName} <{SenderEmail}>".Trim();

    /// <summary>
    /// To recipients as list (convenience property)
    /// </summary>
    public List<string> To => new List<string> { RecipientEmail };

    /// <summary>
    /// CC recipients as list (convenience property)
    /// </summary>
    public List<string>? Cc => string.IsNullOrEmpty(CcRecipients)
        ? null
        : CcRecipients.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();

    /// <summary>
    /// BCC recipients as list (convenience property)
    /// </summary>
    public List<string>? Bcc => string.IsNullOrEmpty(BccRecipients)
        ? null
        : BccRecipients.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();

    /// <summary>
    /// Email body content (convenience property)
    /// </summary>
    public string Body => !string.IsNullOrEmpty(HtmlBody) ? HtmlBody : PlainTextBody ?? string.Empty;

    /// <summary>
    /// Whether the email body is HTML (convenience property)
    /// </summary>
    public bool IsHtml => !string.IsNullOrEmpty(HtmlBody);
}

/// <summary>
/// Email message status enumeration
/// </summary>
public enum EmailStatus
{
    /// <summary>Email is pending and ready to be sent</summary>
    Pending = 0,

    /// <summary>Email is being sent</summary>
    Sending = 1,

    /// <summary>Email was successfully sent</summary>
    Sent = 2,

    /// <summary>Email sending failed permanently</summary>
    Failed = 3,

    /// <summary>Email is bounced/rejected</summary>
    Bounced = 4,

    /// <summary>Email was deferred/temporarily failed</summary>
    Deferred = 5,

    /// <summary>Email sending was cancelled</summary>
    Cancelled = 6
}
