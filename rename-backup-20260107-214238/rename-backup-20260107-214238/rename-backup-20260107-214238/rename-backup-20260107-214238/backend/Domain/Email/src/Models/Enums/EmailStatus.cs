namespace B2X.Email.Models;

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
