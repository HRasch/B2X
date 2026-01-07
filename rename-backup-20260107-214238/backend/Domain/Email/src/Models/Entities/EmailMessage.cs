using System.ComponentModel.DataAnnotations.Schema;

namespace B2X.Email.Models;

/// <summary>
/// Email-Nachricht
/// </summary>
public class EmailMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public string To { get; set; } = string.Empty;
    public string? Cc { get; set; }
    public string? Bcc { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
    public string? TemplateKey { get; set; }
    public Dictionary<string, object> Variables { get; set; } = new(StringComparer.Ordinal);
    [NotMapped]
    public List<EmailAttachment>? Attachments { get; set; }
    public EmailPriority Priority { get; set; } = EmailPriority.Normal;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Queue and retry properties
    public EmailStatus Status { get; set; } = EmailStatus.Queued;
    public int RetryCount { get; set; } = 0;
    public int MaxRetries { get; set; } = 3;
    public DateTime? NextRetryAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public string? LastError { get; set; }
    public string? MessageId { get; set; }
    public DateTime? ScheduledFor { get; set; }
}
