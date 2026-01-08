using B2X.Email.Interfaces;
using B2X.Email.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace B2X.Email.Handlers;

/// <summary>
/// Background Service für die Verarbeitung der Email-Queue
/// </summary>
public class ProcessEmailQueueJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProcessEmailQueueJob> _logger;
    private Timer? _timer;

    public ProcessEmailQueueJob(
        IServiceProvider serviceProvider,
        ILogger<ProcessEmailQueueJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email Queue Processor started");

        // Start the timer to run every 30 seconds
        _timer = new Timer(async _ => await ProcessQueueAsync(stoppingToken).ConfigureAwait(false),
                          null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

        // Keep the service running
        await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(false);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Email Queue Processor stopping");

        _timer?.Change(Timeout.Infinite, 0);

        await base.StopAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task ProcessQueueAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var queueService = scope.ServiceProvider.GetRequiredService<IEmailQueueService>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        try
        {
            var pendingEmails = await queueService.GetPendingEmailsAsync(batchSize: 10, cancellationToken).ConfigureAwait(false);

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

                    var result = await emailService.SendEmailAsync(email, cancellationToken).ConfigureAwait(false);

                    if (result.Success)
                    {
                        await queueService.MarkEmailAsSentAsync(email.Id, result.MessageId ?? string.Empty, cancellationToken).ConfigureAwait(false);
                        _logger.LogInformation("Email {EmailId} sent successfully", email.Id);
                    }
                    else
                    {
                        var errorMessage = result.ErrorMessage ?? "Unknown error";
                        await queueService.MarkEmailAsFailedAsync(email.Id, errorMessage, cancellationToken).ConfigureAwait(false);
                        _logger.LogWarning("Email {EmailId} failed to send: {Error}", email.Id, errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing email {EmailId}", email.Id);
                    await queueService.MarkEmailAsFailedAsync(email.Id, ex.Message, cancellationToken).ConfigureAwait(false);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in email queue processing - will retry on next cycle");
            // Don't rethrow - let the service continue running
        }
    }
}
