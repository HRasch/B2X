using B2Connect.Email.Models;
using FluentValidation;

namespace B2Connect.Email.Handlers;

/// <summary>
/// Wolverine Command: Send Email
/// 
/// Handles sending a single email message.
/// 
/// Wolverine Pattern:
/// - Command is a POCO (Plain Old C# Object)
/// - Handler has [WolverineHandler] attribute
/// - Handler method is async and returns the response object
/// </summary>
public class SendEmailCommand
{
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
    /// HTML email body
    /// </summary>
    public string HtmlBody { get; set; } = string.Empty;

    /// <summary>
    /// Plain text email body (optional)
    /// </summary>
    public string? PlainTextBody { get; set; }

    /// <summary>
    /// Email type/template name
    /// </summary>
    public string EmailType { get; set; } = string.Empty;

    /// <summary>
    /// Sender email address
    /// </summary>
    public string? SenderEmail { get; set; }

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
    /// Schedule email to be sent at a specific time
    /// </summary>
    public DateTime? ScheduleFor { get; set; }

    /// <summary>
    /// Optional metadata for tracking
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Response from SendEmailCommand
/// </summary>
public class SendEmailResponse
{
    /// <summary>
    /// Whether the email was successfully queued/sent
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Message ID for tracking
    /// </summary>
    public string? MessageId { get; set; }

    /// <summary>
    /// Error message if sending failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Email status
    /// </summary>
    public EmailStatus Status { get; set; } = EmailStatus.Pending;
}

/// <summary>
/// Validator for SendEmailCommand
/// </summary>
public class SendEmailCommandValidator : AbstractValidator<SendEmailCommand>
{
    public SendEmailCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required");

        RuleFor(x => x.RecipientEmail)
            .NotEmpty()
            .WithMessage("RecipientEmail is required")
            .EmailAddress()
            .WithMessage("RecipientEmail must be a valid email address");

        RuleFor(x => x.Subject)
            .NotEmpty()
            .WithMessage("Subject is required")
            .MaximumLength(200)
            .WithMessage("Subject must not exceed 200 characters");

        RuleFor(x => x.HtmlBody)
            .NotEmpty()
            .WithMessage("HtmlBody is required")
            .MinimumLength(10)
            .WithMessage("HtmlBody must be at least 10 characters");

        RuleFor(x => x.EmailType)
            .NotEmpty()
            .WithMessage("EmailType is required")
            .Matches(@"^[a-zA-Z0-9_-]+$")
            .WithMessage("EmailType must contain only alphanumeric characters, hyphens, and underscores");

        When(x => !string.IsNullOrWhiteSpace(x.SenderEmail), () =>
        {
            RuleFor(x => x.SenderEmail)
                .EmailAddress()
                .WithMessage("SenderEmail must be a valid email address");
        });

        When(x => !string.IsNullOrWhiteSpace(x.ReplyToEmail), () =>
        {
            RuleFor(x => x.ReplyToEmail)
                .EmailAddress()
                .WithMessage("ReplyToEmail must be a valid email address");
        });

        When(x => x.ScheduleFor.HasValue, () =>
        {
            RuleFor(x => x.ScheduleFor)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("ScheduleFor must be in the future");
        });
    }
}
