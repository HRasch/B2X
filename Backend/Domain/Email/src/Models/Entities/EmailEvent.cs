namespace B2Connect.Email.Models;

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
