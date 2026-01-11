namespace B2X.Email.Models;

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
