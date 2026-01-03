namespace B2Connect.Email.Models;

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
    public Dictionary<string, object> Variables { get; set; } = new();
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

/// <summary>
/// Email-Anhang
/// </summary>
public class EmailAttachment
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
}

/// <summary>
/// Email-Priorität
/// </summary>
public enum EmailPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Urgent = 3
}

/// <summary>
/// Email-Status für Queue-Verarbeitung
/// </summary>
public enum EmailStatus
{
    Queued = 0,
    Processing = 1,
    Sent = 2,
    Failed = 3,
    Cancelled = 4,
    Scheduled = 5
}

/// <summary>
/// Ergebnis des Email-Versands
/// </summary>
public class EmailSendResult
{
    public bool Success { get; set; }
    public string? MessageId { get; set; }
    public string? ErrorMessage { get; set; }
    public EmailSendStatus Status { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Status des Email-Versands
/// </summary>
public enum EmailSendStatus
{
    Sent = 0,
    Failed = 1,
    Queued = 2,
    Bounced = 3
}

/// <summary>
/// Email-Event für Monitoring
/// </summary>
public class EmailEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EmailId { get; set; }
    public Guid TenantId { get; set; }
    public EmailEventType EventType { get; set; }
    public string? Metadata { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
}

/// <summary>
/// Typ des Email-Events
/// </summary>
public enum EmailEventType
{
    Sent = 0,
    Delivered = 1,
    Opened = 2,
    Clicked = 3,
    Bounced = 4,
    Complained = 5,
    Unsubscribed = 6,
    Failed = 7
}

/// <summary>
/// Email-Statistiken
/// </summary>
public class EmailStatistics
{
    public int TotalSent { get; set; }
    public int TotalDelivered { get; set; }
    public int TotalOpened { get; set; }
    public int TotalClicked { get; set; }
    public int TotalBounced { get; set; }
    public int TotalFailed { get; set; }
    public double OpenRate => TotalSent > 0 ? (double)TotalOpened / TotalSent * 100 : 0;
    public double ClickRate => TotalSent > 0 ? (double)TotalClicked / TotalSent * 100 : 0;
    public double BounceRate => TotalSent > 0 ? (double)TotalBounced / TotalSent * 100 : 0;
    public double DeliveryRate => TotalSent > 0 ? (double)TotalDelivered / TotalSent * 100 : 0;
}