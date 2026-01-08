namespace B2X.Email.Models;

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
