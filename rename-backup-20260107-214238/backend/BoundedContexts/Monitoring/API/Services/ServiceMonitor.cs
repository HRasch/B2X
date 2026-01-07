using B2X.Shared.Monitoring.Abstractions;
using B2X.Shared.Monitoring.Data;
using B2X.Shared.Monitoring.Data.Entities;
using B2X.Shared.Monitoring.Models;
using B2X.Shared.Monitoring;
using Microsoft.EntityFrameworkCore;

namespace B2X.Shared.Monitoring.Services;

/// <summary>
/// Service for monitoring connected services (ERP, PIM, CRM, etc.).
/// Implements Phase 2: Connected services monitoring, error logging.
/// </summary>
public class ServiceMonitor : IServiceMonitor
{
    private readonly MonitoringDbContext _dbContext;
    private readonly ILogger<ServiceMonitor> _logger;

    public ServiceMonitor(MonitoringDbContext dbContext, ILogger<ServiceMonitor> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task RegisterServiceAsync(ConnectedService service, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Registering service {ServiceName} ({ServiceId}) for monitoring", service.Name, service.Id);

        // Persist to database
        var entity = new ConnectedServiceEntity
        {
            Id = service.Id,
            TenantId = Guid.Parse(service.TenantId),
            Name = service.Name,
            Type = service.Type,
            Endpoint = service.Endpoint,
            Status = service.Status,
            LastChecked = service.LastChecked,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "system",
            IsDeleted = false
        };

        _dbContext.ConnectedServices.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Service {ServiceName} registered successfully", service.Name);
    }

    /// <inheritdoc />
    public async Task UnregisterServiceAsync(Guid serviceId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Unregistering service {ServiceId} from monitoring", serviceId);

        var entity = await _dbContext.ConnectedServices
            .FirstOrDefaultAsync(s => s.Id == serviceId, cancellationToken);

        if (entity != null)
        {
            _dbContext.ConnectedServices.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Service {ServiceId} unregistered successfully", serviceId);
    }

    /// <inheritdoc />
    public async Task UpdateServiceStatusAsync(
        Guid serviceId,
        ServiceStatus status,
        double latencyMs = 0,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating status for service {ServiceId} to {Status}", serviceId, status);

        var entity = await _dbContext.ConnectedServices
            .FirstOrDefaultAsync(s => s.Id == serviceId, cancellationToken);

        if (entity != null)
        {
            entity.Status = status;
            entity.LastChecked = DateTime.UtcNow;
            entity.AverageLatencyMs = latencyMs;

            if (status == ServiceStatus.Healthy)
            {
                entity.LastSuccessful = DateTime.UtcNow;
            }

            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = "system";

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Service {ServiceId} status updated to {Status}", serviceId, status);
    }

    /// <inheritdoc />
    public async Task RecordResourceMetricsAsync(
        Guid serviceId,
        ResourceMetrics metrics,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Recording resource metrics for service {ServiceId}", serviceId);

        // Get tenant context from the service
        var service = await _dbContext.ConnectedServices
            .FirstOrDefaultAsync(s => s.Id == serviceId, cancellationToken);

        if (service == null)
        {
            _logger.LogWarning("Service {ServiceId} not found for metrics recording", serviceId);
            return;
        }

        var entity = new ResourceMetricsEntity
        {
            Id = Guid.NewGuid(),
            ServiceId = serviceId,
            TenantId = service.TenantId,
            CpuPercent = metrics.CpuPercent,
            CpuAverage = metrics.CpuAverage,
            CpuPeak = metrics.CpuPeak,
            MemoryPercent = metrics.MemoryPercent,
            MemoryUsedBytes = metrics.MemoryUsedBytes,
            MemoryTotalBytes = metrics.MemoryTotalBytes,
            MemoryPeakBytes = metrics.MemoryPeakBytes,
            ThreadCount = metrics.ThreadCount,
            ConnectionPoolActive = metrics.ConnectionPoolActive,
            ConnectionPoolTotal = metrics.ConnectionPoolTotal,
            QueueDepth = metrics.QueueDepth,
            Timestamp = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "system",
            IsDeleted = false
        };

        _dbContext.ResourceMetrics.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Check for alerts
        await CheckResourceAlertsAsync(serviceId, service.TenantId, metrics, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ConnectedService?> GetServiceStatusAsync(Guid serviceId, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.ConnectedServices
            .FirstOrDefaultAsync(s => s.Id == serviceId, cancellationToken);

        return entity != null ? MapToModel(entity) : null;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ConnectedService>> GetServicesAsync(
        string tenantId,
        ServiceType? type = null,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(tenantId, out var tenantGuid))
        {
            return Array.Empty<ConnectedService>();
        }

        var query = _dbContext.ConnectedServices
            .Where(s => s.TenantId == tenantGuid);

        if (type.HasValue)
        {
            query = query.Where(s => s.Type == type.Value);
        }

        var entities = await query.ToListAsync(cancellationToken);
        return entities.Select(MapToModel);
    }

    /// <inheritdoc />
    public async Task<ServiceTestResult> TestServiceConnectivityAsync(
        Guid serviceId,
        CancellationToken cancellationToken = default)
    {
        var service = await GetServiceStatusAsync(serviceId, cancellationToken);
        if (service == null)
        {
            return new ServiceTestResult
            {
                IsSuccessful = false,
                ErrorMessage = "Service not found"
            };
        }

        _logger.LogInformation("Testing connectivity to service {ServiceName}", service.Name);

        var startTime = DateTime.UtcNow;
        try
        {
            var result = await PerformConnectivityTestAsync(service, cancellationToken);
            var latency = (DateTime.UtcNow - startTime).TotalMilliseconds;

            await RecordConnectionTestResultAsync(serviceId, service.TenantId, result, latency, cancellationToken);

            var status = result.IsSuccessful ? ServiceStatus.Healthy : ServiceStatus.Unhealthy;
            await UpdateServiceStatusAsync(serviceId, status, latency, cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Connectivity test failed for service {ServiceName}", service.Name);

            var latency = (DateTime.UtcNow - startTime).TotalMilliseconds;
            var result = new ServiceTestResult
            {
                IsSuccessful = false,
                ErrorMessage = ex.Message,
                Data = new Dictionary<string, object> { ["ExceptionType"] = ex.GetType().Name }
            };

            await RecordConnectionTestResultAsync(serviceId, service.TenantId, result, latency, cancellationToken);
            await UpdateServiceStatusAsync(serviceId, ServiceStatus.Unhealthy, latency, cancellationToken);

            return result;
        }
    }

    private async Task<ServiceTestResult> PerformConnectivityTestAsync(
        ConnectedService service,
        CancellationToken cancellationToken)
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        try
        {
            var response = await client.GetAsync(service.Endpoint, cancellationToken);
            return new ServiceTestResult
            {
                IsSuccessful = response.IsSuccessStatusCode,
                LatencyMs = 0,
                ErrorMessage = response.IsSuccessStatusCode ? null : $"HTTP {response.StatusCode}",
                Data = new Dictionary<string, object>
                {
                    ["StatusCode"] = (int)response.StatusCode,
                    ["ResponseTime"] = response.Headers.Date?.ToString() ?? "Unknown"
                }
            };
        }
        catch (HttpRequestException ex)
        {
            return new ServiceTestResult
            {
                IsSuccessful = false,
                ErrorMessage = ex.Message,
                Data = new Dictionary<string, object> { ["ExceptionType"] = ex.GetType().Name }
            };
        }
    }

    private async Task RecordConnectionTestResultAsync(
        Guid serviceId,
        string tenantId,
        ServiceTestResult result,
        double latencyMs,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(tenantId, out var tenantGuid))
        {
            return;
        }

        var entity = new ConnectionTestResultEntity
        {
            Id = Guid.NewGuid(),
            ServiceId = serviceId,
            TenantId = tenantGuid,
            ExecutedAt = DateTime.UtcNow,
            Success = result.IsSuccessful,
            DnsResolutionMs = 0,
            TlsHandshakeMs = 0,
            TotalConnectionMs = latencyMs,
            SpeedRating = CalculateSpeedRating(latencyMs),
            ErrorMessage = result.ErrorMessage,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "system",
            IsDeleted = false
        };

        _dbContext.ConnectionTestResults.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task CheckResourceAlertsAsync(
        Guid serviceId,
        Guid tenantId,
        ResourceMetrics metrics,
        CancellationToken cancellationToken)
    {
        var alerts = new List<(ResourceAlertType type, double threshold, double actual, string message)>();

        if (metrics.CpuPercent > 80)
        {
            alerts.Add((ResourceAlertType.CpuUsage, 80, metrics.CpuPercent, $"CPU usage at {metrics.CpuPercent:F2}%"));
        }

        if (metrics.MemoryPercent > 85)
        {
            alerts.Add((ResourceAlertType.MemoryUsage, 85, metrics.MemoryPercent, $"Memory usage at {metrics.MemoryPercent:F2}%"));
        }

        if (metrics.QueueDepth > 1000)
        {
            alerts.Add((ResourceAlertType.ThreadPool, 1000, metrics.QueueDepth, $"Queue depth at {metrics.QueueDepth}"));
        }

        foreach (var alert in alerts)
        {
            var entity = new ResourceAlertEntity
            {
                Id = Guid.NewGuid(),
                ServiceId = serviceId,
                TenantId = tenantId,
                AlertType = alert.type,
                ThresholdValue = alert.threshold,
                ActualValue = alert.actual,
                TriggeredAt = DateTime.UtcNow,
                Message = alert.message,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "system",
                IsDeleted = false
            };

            _dbContext.ResourceAlerts.Add(entity);
        }

        if (alerts.Count > 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static string CalculateSpeedRating(double latencyMs)
    {
        return latencyMs switch
        {
            < 100 => "Excellent",
            < 300 => "Good",
            < 1000 => "Fair",
            _ => "Poor"
        };
    }

    private static ConnectedService MapToModel(ConnectedServiceEntity entity)
    {
        return new ConnectedService
        {
            Id = entity.Id,
            TenantId = entity.TenantId.ToString(),
            Name = entity.Name,
            Type = entity.Type,
            Endpoint = entity.Endpoint,
            Status = entity.Status,
            LastChecked = entity.LastChecked,
            AverageLatencyMs = entity.AverageLatencyMs,
            UptimePercent = entity.UptimePercent
        };
    }
}