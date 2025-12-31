using B2Connect.Email.Models;
using B2Connect.Email.Interfaces;
using Microsoft.Extensions.Logging;

namespace B2Connect.Email.Handlers;

/// <summary>
/// Wolverine Handler for SendEmailCommand
/// Handles the command to send an email message
/// </summary>
public class SendEmailCommandHandler
{
    private readonly IEmailService _emailService;
    private readonly IEmailRepository _repository;
    private readonly ILogger<SendEmailCommandHandler> _logger;

    public SendEmailCommandHandler(
        IEmailService emailService,
        IEmailRepository repository,
        ILogger<SendEmailCommandHandler> logger)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handle SendEmailCommand
    /// </summary>
    public async Task<SendEmailResponse> Handle(SendEmailCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        try
        {
            // Create email message from command
            var message = new EmailMessage
            {
                TenantId = command.TenantId,
                RecipientEmail = command.RecipientEmail,
                RecipientName = command.RecipientName,
                Subject = command.Subject,
                HtmlBody = command.HtmlBody,
                PlainTextBody = command.PlainTextBody,
                EmailType = command.EmailType,
                SenderEmail = command.SenderEmail ?? "noreply@b2connect.local",
                SenderName = command.SenderName,
                CcRecipients = command.CcRecipients,
                BccRecipients = command.BccRecipients,
                ReplyToEmail = command.ReplyToEmail,
                ScheduledFor = command.ScheduleFor,
                Metadata = command.Metadata ?? new Dictionary<string, string>(),
                Status = command.ScheduleFor.HasValue ? EmailStatus.Pending : EmailStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            // Persist the message
            await _repository.AddAsync(message, cancellationToken);
            _logger.LogInformation(
                "Email message created. MessageId: {MessageId}, TenantId: {TenantId}, EmailType: {EmailType}",
                message.Id, message.TenantId, message.EmailType);

            // If not scheduled, send immediately
            if (!command.ScheduleFor.HasValue)
            {
                var sent = await _emailService.SendEmailAsync(message, cancellationToken);
                return new SendEmailResponse
                {
                    Success = sent,
                    MessageId = message.Id,
                    Status = message.Status,
                    ErrorMessage = sent ? null : message.LastError
                };
            }

            // If scheduled, return pending status
            return new SendEmailResponse
            {
                Success = true,
                MessageId = message.Id,
                Status = EmailStatus.Pending
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling SendEmailCommand for tenant {TenantId}", command.TenantId);
            return new SendEmailResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
