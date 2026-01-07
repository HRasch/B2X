namespace B2X.Email.Models;

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
