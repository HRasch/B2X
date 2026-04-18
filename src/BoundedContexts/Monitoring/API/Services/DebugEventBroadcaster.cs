using B2X.Monitoring.Hubs;
using B2X.Shared.Monitoring.Abstractions;
using B2X.Shared.Monitoring.Data.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace B2X.Monitoring.Services;

/// <summary>
/// Service for broadcasting debug events via SignalR to connected clients.
/// </summary>
public class DebugEventBroadcaster : IDebugEventBroadcaster
{
    private readonly IHubContext<DebugHub> _hubContext;
    private readonly ILogger<DebugEventBroadcaster> _logger;

    public DebugEventBroadcaster(
        IHubContext<DebugHub> hubContext,
        ILogger<DebugEventBroadcaster> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <summary>
    /// Broadcast when a new debug session starts.
    /// </summary>
    public async Task BroadcastSessionStartedAsync(DebugSessionEntity session)
    {
        try
        {
            var data = new
            {
                session.Id,
                session.CorrelationId,
                session.UserId,
                session.UserAgent,
                session.Viewport,
                session.Environment,
                session.StartedAt
            };

            // Broadcast to tenant group
            await _hubContext.Clients.Group($"tenant-{session.TenantId}").SendAsync("SessionStarted", data);

            _logger.LogInformation("Broadcasted session start: {SessionId}, CorrelationId: {CorrelationId}",
                session.Id, session.CorrelationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to broadcast session start for session {SessionId}", session.Id);
        }
    }

    /// <summary>
    /// Broadcast when a debug error occurs.
    /// </summary>
    public async Task BroadcastErrorCapturedAsync(DebugErrorEntity error, string tenantId)
    {
        try
        {
            var data = new
            {
                error.Id,
                error.SessionId,
                error.Message,
                error.StackTrace,
                error.Component,
                error.Url,
                Timestamp = error.CreatedAt,
                error.ColumnNumber,
                error.LineNumber,
                error.SourceFile,
                error.Level
            };

            // Broadcast to tenant group and error subscribers
            await _hubContext.Clients.Group($"tenant-{tenantId}").SendAsync("ErrorCaptured", data);
            await _hubContext.Clients.Group($"errors-{tenantId}").SendAsync("ErrorCaptured", data);

            _logger.LogInformation("Broadcasted error: {ErrorId}, Session: {SessionId}, Tenant: {TenantId}",
                error.Id, error.SessionId, tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to broadcast error capture for error {ErrorId}", error.Id);
        }
    }

    /// <summary>
    /// Broadcast when user feedback is submitted.
    /// </summary>
    public async Task BroadcastFeedbackSubmittedAsync(DebugFeedbackEntity feedback, string tenantId)
    {
        try
        {
            var data = new
            {
                feedback.Id,
                feedback.SessionId,
                feedback.Type,
                feedback.Title,
                feedback.Description,
                feedback.Rating,
                IncludeScreenshot = feedback.Screenshot != null,
                feedback.Url,
                feedback.UserAgent,
                feedback.Viewport,
                feedback.SubmittedAt
            };

            // Broadcast to tenant group
            await _hubContext.Clients.Group($"tenant-{tenantId}").SendAsync("FeedbackSubmitted", data);

            _logger.LogInformation("Broadcasted feedback: {FeedbackId}, Session: {SessionId}, Tenant: {TenantId}",
                feedback.Id, feedback.SessionId, tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to broadcast feedback submission for feedback {FeedbackId}", feedback.Id);
        }
    }

    /// <summary>
    /// Broadcast when a user action is recorded.
    /// </summary>
    public async Task BroadcastActionRecordedAsync(DebugActionEntity action, string tenantId)
    {
        try
        {
            var data = new
            {
                action.Id,
                action.SessionId,
                Type = action.ActionType,
                Target = action.TargetSelector,
                Timestamp = action.OccurredAt,
                Data = action.Metadata,
                action.Url,
                action.Coordinates
            };

            // Broadcast to tenant group and session group
            await _hubContext.Clients.Group($"tenant-{tenantId}").SendAsync("ActionRecorded", data);
            await _hubContext.Clients.Group($"session-{action.SessionId}").SendAsync("ActionRecorded", data);

            _logger.LogInformation("Broadcasted action: {ActionId}, Session: {SessionId}, Tenant: {TenantId}",
                action.Id, action.SessionId, tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to broadcast action recording for action {ActionId}", action.Id);
        }
    }

    /// <summary>
    /// Broadcast performance alerts.
    /// </summary>
    public async Task BroadcastPerformanceAlertAsync(string tenantId, string alertType, object data)
    {
        try
        {
            var alertData = new
            {
                AlertType = alertType,
                Data = data,
                Timestamp = DateTime.UtcNow
            };

            // Broadcast to tenant group
            await _hubContext.Clients.Group($"tenant-{tenantId}").SendAsync("PerformanceAlert", alertData);

            _logger.LogInformation("Broadcasted performance alert: {AlertType}, Tenant: {TenantId}", alertType, tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to broadcast performance alert for tenant {TenantId}", tenantId);
        }
    }

    /// <summary>
    /// Send a direct message to a specific connection.
    /// </summary>
    public async Task SendToConnectionAsync(string connectionId, string method, object data)
    {
        try
        {
            await _hubContext.Clients.Client(connectionId).SendAsync(method, data);

            _logger.LogInformation("Sent direct message to connection: {ConnectionId}, Method: {Method}", connectionId, method);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send direct message to connection {ConnectionId}", connectionId);
        }
    }

    /// <summary>
    /// Broadcast a custom event to a tenant group.
    /// </summary>
    public async Task BroadcastToTenantAsync(string tenantId, string eventName, object data)
    {
        try
        {
            var eventData = new
            {
                EventName = eventName,
                Data = data,
                Timestamp = DateTime.UtcNow
            };

            // Broadcast to tenant group
            await _hubContext.Clients.Group($"tenant-{tenantId}").SendAsync(eventName, eventData);

            _logger.LogInformation("Broadcasted custom event to tenant: {TenantId}, Event: {EventName}", tenantId, eventName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to broadcast custom event {EventName} to tenant {TenantId}", eventName, tenantId);
        }
    }
}
