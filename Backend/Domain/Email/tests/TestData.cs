using B2Connect.Email.Models;

namespace B2Connect.Email.Tests.Services;

public static class TestData
{
    public static EmailMessage CreateValidEmailMessage()
    {
        return new EmailMessage
        {
            Id = Guid.NewGuid().ToString(),
            TenantId = "test-tenant-123",
            RecipientEmail = "test@example.com",
            RecipientName = "Test User",
            Subject = "Test Email Subject",
            HtmlBody = "<p>This is a test email.</p>",
            PlainTextBody = "This is a test email.",
            EmailType = "test",
            SenderEmail = "noreply@test.com",
            SenderName = "Test Sender",
            Status = EmailStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            SendAttempts = 0,
            MaxSendAttempts = 3
        };
    }

    public static EmailMessage CreateSentEmailMessage()
    {
        var message = CreateValidEmailMessage();
        message.Status = EmailStatus.Sent;
        message.SentAt = DateTime.UtcNow;
        message.SendAttempts = 1;
        return message;
    }

    public static EmailMessage CreateFailedEmailMessage()
    {
        var message = CreateValidEmailMessage();
        message.Status = EmailStatus.Failed;
        message.LastError = "Test failure";
        message.SendAttempts = 3;
        return message;
    }

    public static EmailMessage CreateDeferredEmailMessage()
    {
        var message = CreateValidEmailMessage();
        message.Status = EmailStatus.Deferred;
        message.LastError = "Temporary failure";
        message.SendAttempts = 1;
        return message;
    }
}