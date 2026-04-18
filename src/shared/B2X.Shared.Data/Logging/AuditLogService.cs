using Microsoft.Extensions.Logging;

namespace B2X.Data.Logging;

/// <summary>
/// Interface for audit log repository/service
/// </summary>
public interface IAuditLogService
{
    Task LogActionAsync(string entityType, string entityId, string action, string? changedValues = null, string userId = "System");
    Task<IEnumerable<AuditLogEntry>> GetAuditLogsAsync(string entityType, string entityId);
}

/// <summary>
/// Audit log entry (for manual tracking)
/// </summary>
public class AuditLogEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Created, Modified, Deleted
    public string? ChangedValues { get; set; }
    public string UserId { get; set; } = "System";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Default audit log service (in-memory for development)
/// In production, use a dedicated audit log table or Event Sourcing
/// </summary>
public class AuditLogService : IAuditLogService
{
    private readonly ILogger<AuditLogService> _logger;
    private readonly List<AuditLogEntry> _auditLogs = new();

    public AuditLogService(ILogger<AuditLogService> logger)
    {
        _logger = logger;
    }

    public Task LogActionAsync(string entityType, string entityId, string action, string? changedValues = null, string userId = "System")
    {
        var entry = new AuditLogEntry
        {
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            ChangedValues = changedValues,
            UserId = userId,
            Timestamp = DateTime.UtcNow
        };

        _auditLogs.Add(entry);

        _logger.LogInformation(
            "📋 Audit log: {Action} on {EntityType}#{EntityId} by {UserId}",
            action, entityType, entityId, userId);

        return Task.CompletedTask;
    }

    public Task<IEnumerable<AuditLogEntry>> GetAuditLogsAsync(string entityType, string entityId)
    {
        var logs = _auditLogs
            .Where(l => l.EntityType == entityType && l.EntityId == entityId)
            .OrderByDescending(l => l.Timestamp)
            .ToList();

        return Task.FromResult<IEnumerable<AuditLogEntry>>(logs);
    }
}
