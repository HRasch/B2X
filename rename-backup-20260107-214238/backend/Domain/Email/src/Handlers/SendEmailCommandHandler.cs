using B2X.Email.Interfaces;
using B2X.Email.Models;
using B2X.Shared.Messaging.Commands;
using Microsoft.Extensions.Logging;
using Wolverine.Attributes;

namespace B2X.Email.Handlers;

/// <summary>
/// Wolverine Handler für SendEmailCommand
/// </summary>
public class SendEmailCommandHandler
{
    private readonly IEmailQueueService _queueService;
    private readonly ILogger<SendEmailCommandHandler> _logger;

    public SendEmailCommandHandler(
        IEmailQueueService queueService,
        ILogger<SendEmailCommandHandler> logger)
    {
        _queueService = queueService;
        _logger = logger;
    }

    [WolverineHandler]
    public async Task<EmailSendResult> Handle(
        SendEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing SendEmailCommand for {To}", command.To);

        var emailMessage = new EmailMessage
        {
            TenantId = command.TenantId,
            To = command.To,
            Subject = command.Subject,
            Body = command.Body,
            IsHtml = command.IsHtml
        };

        // Queue the email instead of sending immediately
        await _queueService.QueueEmailAsync(emailMessage, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Email {EmailId} queued for sending", emailMessage.Id);

        // Return success result since queuing was successful
        return new EmailSendResult
        {
            Success = true,
            Status = EmailSendStatus.Queued,
            SentAt = DateTime.UtcNow
        };
    }
}
