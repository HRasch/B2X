using B2Connect.Email.Interfaces;
using B2Connect.Email.Models;
using B2Connect.Email.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace B2Connect.Email.Services;

/// <summary>
/// Service für Email-Monitoring und Analytics
/// </summary>
public class EmailMonitoringService : IEmailMonitoringService
{
    private readonly EmailDbContext _dbContext;
    private readonly ILogger<EmailMonitoringService> _logger;

    public EmailMonitoringService(
        EmailDbContext dbContext,
        ILogger<EmailMonitoringService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task RecordEmailEventAsync(
        Guid emailId,
        EmailEventType eventType,
        string? metadata = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var emailEvent = new EmailEvent
            {
                EmailId = emailId,
                EventType = eventType,
                Metadata = metadata,
                OccurredAt = DateTime.UtcNow
            };

            _dbContext.EmailEvents.Add(emailEvent);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Recorded email event: {EventType} for email {EmailId}",
                eventType, emailId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record email event for email {EmailId}", emailId);
            throw;
        }
    }

    public async Task<EmailStatistics> GetEmailStatisticsAsync(
        Guid tenantId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        var events = await _dbContext.EmailEvents
            .Where(e => e.TenantId == tenantId &&
                       e.OccurredAt >= fromDate &&
                       e.OccurredAt <= toDate)
            .ToListAsync(cancellationToken);

        return new EmailStatistics
        {
            TotalSent = events.Count(e => e.EventType == EmailEventType.Sent),
            TotalDelivered = events.Count(e => e.EventType == EmailEventType.Delivered),
            TotalOpened = events.Count(e => e.EventType == EmailEventType.Opened),
            TotalClicked = events.Count(e => e.EventType == EmailEventType.Clicked),
            TotalBounced = events.Count(e => e.EventType == EmailEventType.Bounced),
            TotalFailed = events.Count(e => e.EventType == EmailEventType.Failed)
        };
    }

    public async Task<List<EmailEvent>> GetRecentEmailEventsAsync(
        Guid tenantId,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailEvents
            .Where(e => e.TenantId == tenantId)
            .OrderByDescending(e => e.OccurredAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<EmailEvent>> GetEmailErrorsAsync(
        Guid tenantId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailEvents
            .Where(e => e.TenantId == tenantId &&
                       e.OccurredAt >= fromDate &&
                       e.OccurredAt <= toDate &&
                       (e.EventType == EmailEventType.Failed ||
                        e.EventType == EmailEventType.Bounced ||
                        e.EventType == EmailEventType.Complained))
            .OrderByDescending(e => e.OccurredAt)
            .ToListAsync(cancellationToken);
    }
}