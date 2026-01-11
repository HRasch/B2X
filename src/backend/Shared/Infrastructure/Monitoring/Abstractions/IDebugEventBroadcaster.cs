using B2X.Shared.Monitoring.Data.Entities;

namespace B2X.Shared.Monitoring.Abstractions;

/// <summary>
/// Interface for broadcasting debug events via SignalR.
/// </summary>
public interface IDebugEventBroadcaster
{
    /// <summary>
    /// Broadcast when a new debug session starts.
    /// </summary>
    Task BroadcastSessionStartedAsync(DebugSessionEntity session);

    /// <summary>
    /// Broadcast when a debug error occurs.
    /// </summary>
    Task BroadcastErrorCapturedAsync(DebugErrorEntity error, string tenantId);

    /// <summary>
    /// Broadcast when user feedback is submitted.
    /// </summary>
    Task BroadcastFeedbackSubmittedAsync(DebugFeedbackEntity feedback, string tenantId);

    /// <summary>
    /// Broadcast when a user action is recorded.
    /// </summary>
    Task BroadcastActionRecordedAsync(DebugActionEntity action, string tenantId);

    /// <summary>
    /// Broadcast performance alerts.
    /// </summary>
    Task BroadcastPerformanceAlertAsync(string tenantId, string alertType, object data);

    /// <summary>
    /// Send a direct message to a specific connection.
    /// </summary>
    Task SendToConnectionAsync(string connectionId, string method, object data);

    /// <summary>
    /// Broadcast a custom event to a tenant group.
    /// </summary>
    Task BroadcastToTenantAsync(string tenantId, string eventName, object data);
}
