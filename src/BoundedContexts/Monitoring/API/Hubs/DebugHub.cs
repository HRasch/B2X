using B2X.Shared.Monitoring.Data;
using B2X.Shared.Monitoring.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace B2X.Monitoring.Hubs;

/// <summary>
/// SignalR hub for realtime debug communication.
/// Provides realtime streaming of debug events, errors, and user actions.
/// </summary>
public class DebugHub : Hub
{
    private readonly MonitoringDbContext _dbContext;
    private readonly ILogger<DebugHub> _logger;

    public DebugHub(MonitoringDbContext dbContext, ILogger<DebugHub> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects to the debug hub.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var correlationId = Context.GetHttpContext()?.Request.Headers["X-Correlation-Id"].ToString();
        var tenantId = Context.GetHttpContext()?.Request.Headers["X-Tenant-Id"].ToString();

        _logger.LogInformation("Debug client connected. ConnectionId: {ConnectionId}, CorrelationId: {CorrelationId}, TenantId: {TenantId}",
            Context.ConnectionId, correlationId, tenantId);

        // Add to tenant-specific group for targeted broadcasting
        if (!string.IsNullOrEmpty(tenantId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant-{tenantId}");
            _logger.LogInformation("Added connection {ConnectionId} to tenant group {TenantGroup}", Context.ConnectionId, $"tenant-{tenantId}");
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the debug hub.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var correlationId = Context.GetHttpContext()?.Request.Headers["X-Correlation-Id"].ToString();
        var tenantId = Context.GetHttpContext()?.Request.Headers["X-Tenant-Id"].ToString();

        _logger.LogInformation("Debug client disconnected. ConnectionId: {ConnectionId}, CorrelationId: {CorrelationId}, TenantId: {TenantId}",
            Context.ConnectionId, correlationId, tenantId);

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Join a specific debug session group for targeted updates.
    /// </summary>
    public async Task JoinSessionGroup(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"session-{sessionId}");
        _logger.LogInformation("Client {ConnectionId} joined session group {SessionGroup}", Context.ConnectionId, $"session-{sessionId}");
    }

    /// <summary>
    /// Leave a specific debug session group.
    /// </summary>
    public async Task LeaveSessionGroup(string sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"session-{sessionId}");
        _logger.LogInformation("Client {ConnectionId} left session group {SessionGroup}", Context.ConnectionId, $"session-{sessionId}");
    }

    /// <summary>
    /// Subscribe to error notifications for a specific tenant.
    /// </summary>
    public async Task SubscribeToErrors(string tenantId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"errors-{tenantId}");
        _logger.LogInformation("Client {ConnectionId} subscribed to error notifications for tenant {TenantId}", Context.ConnectionId, tenantId);
    }

    /// <summary>
    /// Unsubscribe from error notifications.
    /// </summary>
    public async Task UnsubscribeFromErrors(string tenantId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"errors-{tenantId}");
        _logger.LogInformation("Client {ConnectionId} unsubscribed from error notifications for tenant {TenantId}", Context.ConnectionId, tenantId);
    }

    /// <summary>
    /// Get active debug sessions for the current tenant.
    /// </summary>
    public async Task<List<DebugSessionSummary>> GetActiveSessions()
    {
        var tenantId = Context.GetHttpContext()?.Request.Headers["X-Tenant-Id"].ToString();

        if (string.IsNullOrEmpty(tenantId))
        {
            _logger.LogWarning("GetActiveSessions called without tenant context");
            return new List<DebugSessionSummary>();
        }

        var sessions = await _dbContext.DebugSessions
            .Where(s => s.TenantId == tenantId && s.EndTime == null)
            .OrderByDescending(s => s.StartTime)
            .Take(50) // Limit to prevent overload
            .Select(s => new DebugSessionSummary
            {
                Id = s.Id,
                CorrelationId = s.CorrelationId,
                UserId = s.UserId,
                StartTime = s.StartTime,
                UserAgent = s.UserAgent,
                Environment = s.Environment
            })
            .ToListAsync();

        _logger.LogInformation("Retrieved {Count} active debug sessions for tenant {TenantId}", sessions.Count, tenantId);
        return sessions;
    }

    /// <summary>
    /// Get recent errors for the current tenant.
    /// </summary>
    public async Task<List<DebugErrorSummary>> GetRecentErrors(int limit = 20)
    {
        var tenantId = Context.GetHttpContext()?.Request.Headers["X-Tenant-Id"].ToString();

        if (string.IsNullOrEmpty(tenantId))
        {
            _logger.LogWarning("GetRecentErrors called without tenant context");
            return new List<DebugErrorSummary>();
        }

        var errors = await _dbContext.DebugErrors
            .Where(e => e.Session.TenantId == tenantId)
            .OrderByDescending(e => e.CreatedAt)
            .Take(Math.Min(limit, 100)) // Cap at 100 to prevent overload
            .Select(e => new DebugErrorSummary
            {
                Id = e.Id,
                SessionId = e.SessionId,
                CorrelationId = e.CorrelationId,
                Severity = e.Severity,
                Message = e.Message,
                CreatedAt = e.CreatedAt,
                Component = e.Component
            })
            .ToListAsync();

        _logger.LogInformation("Retrieved {Count} recent errors for tenant {TenantId}", errors.Count, tenantId);
        return errors;
    }

    /// <summary>
    /// Get user actions for a specific session.
    /// </summary>
    public async Task<List<DebugActionSummary>> GetSessionActions(Guid sessionId, int limit = 50)
    {
        var tenantId = Context.GetHttpContext()?.Request.Headers["X-Tenant-Id"].ToString();

        if (string.IsNullOrEmpty(tenantId))
        {
            _logger.LogWarning("GetSessionActions called without tenant context");
            return new List<DebugActionSummary>();
        }

        var actions = await _dbContext.DebugActions
            .Where(a => a.SessionId == sessionId && a.Session.TenantId == tenantId)
            .OrderByDescending(a => a.CreatedAt)
            .Take(Math.Min(limit, 200)) // Cap at 200 to prevent overload
            .Select(a => new DebugActionSummary
            {
                Id = a.Id,
                SessionId = a.SessionId,
                ActionType = a.ActionType,
                Target = a.Target,
                Data = a.Data,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();

        _logger.LogInformation("Retrieved {Count} actions for session {SessionId}", actions.Count, sessionId);
        return actions;
    }
}

/// <summary>
/// Summary model for debug sessions sent over SignalR.
/// </summary>
public class DebugSessionSummary
{
    public Guid Id { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public DateTime StartTime { get; set; }
    public string? UserAgent { get; set; }
    public string Environment { get; set; } = "development";
}

/// <summary>
/// Summary model for debug errors sent over SignalR.
/// </summary>
public class DebugErrorSummary
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public string Severity { get; set; } = "error";
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? Component { get; set; }
}

/// <summary>
/// Summary model for debug actions sent over SignalR.
/// </summary>
public class DebugActionSummary
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public string? Data { get; set; }
    public DateTime CreatedAt { get; set; }
}