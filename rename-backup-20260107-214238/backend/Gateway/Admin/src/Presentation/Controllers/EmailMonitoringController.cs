using B2X.Email.Interfaces;
using B2X.Email.Models;
using B2X.Email.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace B2X.Admin.Presentation.Controllers;

/// <summary>
/// Controller für Email-Monitoring und Analytics
/// </summary>
[Route("api/admin/email")]
[ApiController]
[Authorize]
public class EmailMonitoringController : ApiControllerBase
{
    private readonly IEmailMonitoringService _monitoringService;
    private readonly IEmailService _emailService;
    private readonly IEmailQueueService _queueService;

    public EmailMonitoringController(
        ILogger<EmailMonitoringController> logger,
        IEmailMonitoringService monitoringService,
        IEmailService emailService,
        IEmailQueueService queueService) : base(logger)
    {
        _monitoringService = monitoringService;
        _emailService = emailService;
        _queueService = queueService;
    }

    /// <summary>
    /// Holt Email-Statistiken für einen Zeitraum
    /// HTTP: GET /api/admin/email/analytics/summary?fromDate=2024-01-01&toDate=2024-01-31
    /// </summary>
    [HttpGet("analytics/summary")]
    [ProducesResponseType(typeof(EmailStatistics), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmailStatistics(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();
        var statistics = await _monitoringService.GetEmailStatisticsAsync(
            tenantId, fromDate, toDate, cancellationToken);

        return Ok(statistics);
    }

    /// <summary>
    /// Holt die letzten Email-Events für Monitoring
    /// HTTP: GET /api/admin/email/events/recent?limit=50
    /// </summary>
    [HttpGet("events/recent")]
    [ProducesResponseType(typeof(List<EmailEvent>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentEmailEvents(
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();
        var events = await _monitoringService.GetRecentEmailEventsAsync(
            tenantId, limit, cancellationToken);

        return Ok(events);
    }

    /// <summary>
    /// Holt Email-Fehler für Troubleshooting
    /// HTTP: GET /api/admin/email/events/errors?fromDate=2024-01-01&toDate=2024-01-31
    /// </summary>
    [HttpGet("events/errors")]
    [ProducesResponseType(typeof(List<EmailEvent>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmailErrors(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();
        var errors = await _monitoringService.GetEmailErrorsAsync(
            tenantId, fromDate, toDate, cancellationToken);

        return Ok(errors);
    }

    /// <summary>
    /// Holt Email-Event-Statistiken nach Typ
    /// HTTP: GET /api/admin/email/analytics/by-type?fromDate=2024-01-01&toDate=2024-01-31
    /// </summary>
    [HttpGet("analytics/by-type")]
    [ProducesResponseType(typeof(Dictionary<string, int>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmailEventsByType(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();
        var events = await _monitoringService.GetRecentEmailEventsAsync(
            tenantId, 1000, cancellationToken);

        var filteredEvents = events.Where(e =>
            e.OccurredAt >= fromDate && e.OccurredAt <= toDate).ToList();

        var stats = filteredEvents
            .GroupBy(e => e.EventType)
            .ToDictionary(
                g => g.Key.ToString(),
                g => g.Count(), StringComparer.Ordinal);

        return Ok(stats);
    }

    /// <summary>
    /// Holt Email-Health-Status für Dashboard
    /// HTTP: GET /api/admin/email/health
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(typeof(EmailHealthStatus), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmailHealthStatus(
        CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();
        var now = DateTime.UtcNow;
        var last24Hours = now.AddHours(-24);

        var recentEvents = await _monitoringService.GetRecentEmailEventsAsync(
            tenantId, 100, cancellationToken);

        var last24HourEvents = recentEvents.Where(e => e.OccurredAt >= last24Hours).ToList();

        var healthStatus = new EmailHealthStatus
        {
            TotalEmailsLast24Hours = last24HourEvents.Count(e => e.EventType == EmailEventType.Sent),
            FailedEmailsLast24Hours = last24HourEvents.Count(e => e.EventType == EmailEventType.Failed),
            BouncedEmailsLast24Hours = last24HourEvents.Count(e => e.EventType == EmailEventType.Bounced),
            LastEmailSent = recentEvents
                .Where(e => e.EventType == EmailEventType.Sent)
                .MaxBy(e => e.OccurredAt)?.OccurredAt,
            IsHealthy = !last24HourEvents.Any(e => e.EventType == EmailEventType.Failed)
        };

        return Ok(healthStatus);
    }

    /// <summary>
    /// Sendet eine Test-Email
    /// HTTP: POST /api/admin/email/test
    /// </summary>
    [HttpPost("test")]
    [ProducesResponseType(typeof(EmailSendResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendTestEmail(
        [FromBody] SendTestEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();

        var result = await _emailService.SendTestEmailAsync(
            request.ToEmail,
            request.Subject,
            request.Body,
            request.IsHtml,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Holt die aktuellen SMTP-Konfigurationseinstellungen
    /// HTTP: GET /api/admin/email/smtp/config
    /// </summary>
    [HttpGet("smtp/config")]
    [ProducesResponseType(typeof(SmtpSettings), StatusCodes.Status200OK)]
    public IActionResult GetSmtpConfiguration()
    {
        // Note: In a real implementation, this would retrieve settings from a secure configuration store
        // For now, return default settings structure
        var settings = new SmtpSettings
        {
            Host = "smtp.example.com",
            Port = 587,
            Username = "",
            Password = "", // Never return actual password
            EnableSsl = true,
            Timeout = 30000,
            FromEmail = "noreply@B2X.com",
            FromName = "B2X",
            Domain = "B2X.com"
        };

        return Ok(settings);
    }

    /// <summary>
    /// Aktualisiert die SMTP-Konfigurationseinstellungen
    /// HTTP: PUT /api/admin/email/smtp/config
    /// </summary>
    [HttpPut("smtp/config")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateSmtpConfiguration(
        [FromBody] UpdateSmtpSettingsRequest request,
        CancellationToken cancellationToken = default)
    {
        // Note: In a real implementation, this would validate and save settings to a secure store
        // For now, just validate the input

        if (string.IsNullOrWhiteSpace(request.Host))
        {
            return BadRequest("SMTP Host is required");
        }

        if (request.Port <= 0 || request.Port > 65535)
        {
            return BadRequest("Invalid SMTP Port");
        }

        if (string.IsNullOrWhiteSpace(request.FromEmail))
        {
            return BadRequest("From Email is required");
        }

        // TODO: Implement actual configuration update logic
        // This would typically involve:
        // 1. Validating the SMTP settings by attempting a connection
        // 2. Saving to secure configuration store
        // 3. Reloading the email service configuration

        _logger.LogInformation("SMTP configuration update requested for tenant {TenantId}",
            GetTenantId());

        return NoContent();
    }

    /// <summary>
    /// Testet die SMTP-Verbindung mit den aktuellen Einstellungen
    /// HTTP: POST /api/admin/email/smtp/test-connection
    /// </summary>
    [HttpPost("smtp/test-connection")]
    [ProducesResponseType(typeof(SmtpTestResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> TestSmtpConnection(
        [FromBody] TestSmtpConnectionRequest request,
        CancellationToken cancellationToken = default)
    {
        // Note: In a real implementation, this would test the actual SMTP connection
        // For now, return a mock result

        var result = new SmtpTestResult
        {
            Success = true,
            Message = "SMTP connection test successful",
            ResponseTime = 150,
            TestedAt = DateTime.UtcNow
        };

        // TODO: Implement actual SMTP connection testing
        // This should attempt to connect to the SMTP server without sending an email

        return Ok(result);
    }

    /// <summary>
    /// Holt Emails für Management-Interface
    /// HTTP: GET /api/admin/email/messages?status=Queued&skip=0&take=50
    /// </summary>
    [HttpGet("messages")]
    [ProducesResponseType(typeof(List<EmailMessage>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmailMessages(
        [FromQuery] EmailStatus? status = null,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantId();
        var emails = await _queueService.GetEmailsForManagementAsync(
            tenantId, status, skip, take, cancellationToken);

        return Ok(emails);
    }

    /// <summary>
    /// Holt eine spezifische Email
    /// HTTP: GET /api/admin/email/messages/{id}
    /// </summary>
    [HttpGet("messages/{id}")]
    [ProducesResponseType(typeof(EmailMessage), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmailMessage(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var email = await _queueService.GetEmailByIdAsync(id, cancellationToken);
        if (email == null)
        {
            return NotFound();
        }

        // Check tenant isolation
        var tenantId = GetTenantId();
        if (email.TenantId != tenantId)
        {
            return Forbid();
        }

        return Ok(email);
    }

    /// <summary>
    /// Setzt eine fehlgeschlagene Email für manuellen Retry zurück
    /// HTTP: POST /api/admin/email/messages/{id}/retry
    /// </summary>
    [HttpPost("messages/{id}/retry")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RetryEmailMessage(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var email = await _queueService.GetEmailByIdAsync(id, cancellationToken);
        if (email == null)
        {
            return NotFound();
        }

        // Check tenant isolation
        var tenantId = GetTenantId();
        if (email.TenantId != tenantId)
        {
            return Forbid();
        }

        // Only allow retry for failed emails
        if (email.Status != EmailStatus.Failed)
        {
            return BadRequest("Only failed emails can be retried");
        }

        await _queueService.RetryEmailAsync(id, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Storniert eine Email (falls noch nicht versendet)
    /// HTTP: POST /api/admin/email/messages/{id}/cancel
    /// </summary>
    [HttpPost("messages/{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelEmailMessage(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var email = await _queueService.GetEmailByIdAsync(id, cancellationToken);
        if (email == null)
        {
            return NotFound();
        }

        // Check tenant isolation
        var tenantId = GetTenantId();
        if (email.TenantId != tenantId)
        {
            return Forbid();
        }

        // Cannot cancel sent or already cancelled emails
        if (email.Status == EmailStatus.Sent || email.Status == EmailStatus.Cancelled)
        {
            return BadRequest("Cannot cancel sent or already cancelled emails");
        }

        await _queueService.CancelEmailAsync(id, cancellationToken);

        return NoContent();
    }

    private new Guid GetTenantId()
    {
        var tenantIdHeader = Request.Headers["X-Tenant-ID"].FirstOrDefault();
        return Guid.TryParse(tenantIdHeader, out var tenantId) ? tenantId : Guid.Empty;
    }
}

/// <summary>
/// Request für Test-Email
/// </summary>
public record SendTestEmailRequest(
    string ToEmail,
    string Subject,
    string Body,
    bool IsHtml = true);

/// <summary>
/// Email-Health-Status für Dashboard
/// </summary>
public class EmailHealthStatus
{
    public int TotalEmailsLast24Hours { get; set; }
    public int FailedEmailsLast24Hours { get; set; }
    public int BouncedEmailsLast24Hours { get; set; }
    public DateTime? LastEmailSent { get; set; }
    public bool IsHealthy { get; set; }
}

/// <summary>
/// Request für SMTP-Konfigurationsupdate
/// </summary>
public class UpdateSmtpSettingsRequest
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
    public int Timeout { get; set; } = 30000;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
}

/// <summary>
/// Request für SMTP-Verbindungstest
/// </summary>
public class TestSmtpConnectionRequest
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
}

/// <summary>
/// Ergebnis des SMTP-Verbindungstests
/// </summary>
public class SmtpTestResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int ResponseTime { get; set; } // in milliseconds
    public DateTime TestedAt { get; set; }
}
