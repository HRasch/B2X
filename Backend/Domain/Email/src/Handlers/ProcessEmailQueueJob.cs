using B2Connect.Email.Interfaces;
using B2Connect.Email.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace B2Connect.Email.Handlers;

/// <summary>
/// Background Service für die Verarbeitung der Email-Queue
/// </summary>
public class ProcessEmailQueueJob : BackgroundService
{
    private readonly IEmailQueueService _queueService;
    private readonly IEmailService _emailService;
    private readonly ILogger<ProcessEmailQueueJob> _logger;
    private Timer _timer;

    public ProcessEmailQueueJob(
        IEmailQueueService queueService,
        IEmailService emailService,
        ILogger<ProcessEmailQueueJob> logger)
    {
        _queueService = queueService;
        _emailService = emailService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email Queue Processor started");

        // Start the timer to run every 30 seconds
        _timer = new Timer(async _ => await ProcessQueueAsync(stoppingToken),
                          null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

        // Keep the service running
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Email Queue Processor stopping");

        _timer?.Change(Timeout.Infinite, 0);

        await base.StopAsync(cancellationToken);
    }

    private async Task ProcessQueueAsync(CancellationToken cancellationToken)
    {
        try
        {
            var pendingEmails = await _queueService.GetPendingEmailsAsync(batchSize: 10, cancellationToken);

            if (!pendingEmails.Any())
            {
                return;
            }

            _logger.LogInformation("Processing {Count} emails", pendingEmails.Count);

            foreach (var email in pendingEmails)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    _logger.LogInformation("Sending email {EmailId} to {To}", email.Id, email.To);

                    var result = await _emailService.SendEmailAsync(email, cancellationToken);

                    if (result.Success)
                    {
                        await _queueService.MarkEmailAsSentAsync(email.Id, result.MessageId ?? string.Empty, cancellationToken);
                        _logger.LogInformation("Email {EmailId} sent successfully", email.Id);
                    }
                    else
                    {
                        var errorMessage = result.ErrorMessage ?? "Unknown error";
                        await _queueService.MarkEmailAsFailedAsync(email.Id, errorMessage, cancellationToken);
                        _logger.LogWarning("Email {EmailId} failed to send: {Error}", email.Id, errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing email {EmailId}", email.Id);
                    await _queueService.MarkEmailAsFailedAsync(email.Id, ex.Message, cancellationToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in email queue processing");
            // Don't rethrow - let the service continue running
        }
    }
}