# Monitoring MCP Usage Guide

**DocID**: `KB-061`  
**Title**: Monitoring MCP Usage Guide  
**Owner**: @DevOps  
**Status**: Active  
**Last Updated**: 6. Januar 2026

---

## Overview

The Monitoring MCP (Model Context Protocol) server provides comprehensive application and system monitoring capabilities for B2Connect. It enables real-time observability, performance tracking, error monitoring, and health validation across the entire application stack.

**Monitoring Scope**: Application metrics, system performance, error tracking, log analysis, health checks, alert configuration

---

## Core Features

### Application Metrics Collection
Collects and analyzes application-level metrics including response times, throughput, and resource utilization.

```bash
# Collect application metrics
monitoring-mcp/collect_application_metrics workspacePath="."

# Collect metrics for specific service
monitoring-mcp/collect_application_metrics workspacePath="." serviceName="B2Connect.API"
```

### System Performance Monitoring
Monitors system-level performance metrics including CPU, memory, disk, and network utilization.

```bash
# Monitor system performance
monitoring-mcp/monitor_system_performance workspacePath="."

# Monitor with custom thresholds
monitoring-mcp/monitor_system_performance workspacePath="." cpuThreshold="80" memoryThreshold="85"
```

### Error Tracking
Tracks and analyzes application errors, exceptions, and failure patterns.

```bash
# Track application errors
monitoring-mcp/track_errors workspacePath="."

# Track errors in specific time range
monitoring-mcp/track_errors workspacePath="." timeRange="24h" severity="ERROR"
```

### Log Analysis
Analyzes application logs for patterns, anomalies, and insights.

```bash
# Analyze application logs
monitoring-mcp/analyze_logs workspacePath="." logPath="logs/"

# Analyze specific log file
monitoring-mcp/analyze_logs workspacePath="." logPath="logs/app.log" pattern="ERROR"
```

### Health Checks Validation
Validates health check endpoints and service availability.

```bash
# Validate health checks
monitoring-mcp/validate_health_checks workspacePath="."

# Validate specific health check endpoint
monitoring-mcp/validate_health_checks workspacePath="." endpoint="/health"
```

### Alert Configuration
Configures and validates monitoring alerts and notifications.

```bash
# Configure alerts
monitoring-mcp/configure_alerts workspacePath="." configPath="monitoring/alerts.yml"

# Validate alert configuration
monitoring-mcp/configure_alerts workspacePath="." validateOnly=true
```

### Performance Profiling (Optional)
Advanced performance profiling and bottleneck analysis.

```bash
# Profile application performance
monitoring-mcp/profile_performance workspacePath="." duration="60s"

# Profile specific endpoint
monitoring-mcp/profile_performance workspacePath="." endpoint="/api/catalog" duration="30s"
```

### Resource Monitoring (Optional)
Detailed resource usage monitoring and capacity planning.

```bash
# Monitor resource usage
monitoring-mcp/monitor_resources workspacePath="." threshold="80"

# Monitor specific resources
monitoring-mcp/monitor_resources workspacePath="." resources="cpu,memory,disk"
```

---

## Integration with Development Workflow

### Pre-Deployment Monitoring Validation

```bash
#!/bin/bash
# Run before deployment

# 1. Health checks validation (MANDATORY)
monitoring-mcp/validate_health_checks workspacePath="."
if [ $? -ne 0 ]; then
  echo "❌ Health check validation failed"
  exit 1
fi

# 2. Application metrics collection
monitoring-mcp/collect_application_metrics workspacePath="."
if [ $? -ne 0 ]; then
  echo "❌ Application metrics collection failed"
  exit 1
fi

# 3. System performance monitoring
monitoring-mcp/monitor_system_performance workspacePath="."
if [ $? -ne 0 ]; then
  echo "❌ System performance issues detected"
  exit 1
fi

# 4. Alert configuration validation
monitoring-mcp/configure_alerts workspacePath="." configPath="monitoring/alerts.yml"
if [ $? -ne 0 ]; then
  echo "❌ Alert configuration issues found"
  exit 1
fi

echo "✅ Monitoring validation passed"
```

### CI/CD Integration

```yaml
# .github/workflows/monitoring-validation.yml
name: Monitoring Validation

on:
  pull_request:
    paths:
      - 'src/**'
      - 'monitoring/**'
      - 'appsettings*.json'

jobs:
  monitoring-check:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Run monitoring MCP validation
        run: |
          monitoring-mcp/validate_health_checks workspacePath="."
          monitoring-mcp/collect_application_metrics workspacePath="."
          monitoring-mcp/monitor_system_performance workspacePath="."
          monitoring-mcp/configure_alerts workspacePath="." configPath="monitoring/alerts.yml"
```

### Application Integration

```csharp
// Startup.cs - Health Checks Configuration
public void ConfigureServices(IServiceCollection services)
{
    services.AddHealthChecks()
        .AddCheck<DatabaseHealthCheck>("database")
        .AddCheck<ExternalServiceHealthCheck>("external-services")
        .AddCheck<MemoryHealthCheck>("memory");

    // Monitoring MCP integration
    services.AddSingleton<IMonitoringService, MonitoringService>();
}
```

```typescript
// src/services/monitoring.ts
import { MonitoringService } from '@/types/monitoring'

export class MonitoringService {
  async collectMetrics(): Promise<MetricsData> {
    // Integration with monitoring MCP
    const response = await fetch('/api/monitoring/metrics')
    return response.json()
  }

  async trackError(error: Error, context: ErrorContext): Promise<void> {
    // Error tracking integration
    await fetch('/api/monitoring/errors', {
      method: 'POST',
      body: JSON.stringify({ error, context })
    })
  }
}
```

---

## Key Metrics and Thresholds

### Application Metrics

| Metric | Threshold | Action |
|--------|-----------|--------|
| Response Time (P95) | <500ms | Warning >1s |
| Error Rate | <1% | Critical >5% |
| Throughput | Based on capacity | Monitor trends |
| Memory Usage | <80% | Critical >90% |
| CPU Usage | <70% | Critical >85% |

### System Metrics

| Component | Warning | Critical |
|-----------|---------|----------|
| CPU Usage | >70% | >85% |
| Memory Usage | >80% | >90% |
| Disk Usage | >85% | >95% |
| Network I/O | >70% | >85% |

### Error Categories

- **Critical**: System down, data loss, security breaches
- **Error**: Application errors, failed operations
- **Warning**: Performance degradation, configuration issues
- **Info**: Normal operational messages

---

## Alert Configuration

### Alert Rules Structure

```yaml
# monitoring/alerts.yml
alerts:
  - name: "High Error Rate"
    condition: "error_rate > 0.05"
    severity: "critical"
    channels: ["slack", "email"]
    cooldown: "5m"

  - name: "High Response Time"
    condition: "response_time_p95 > 2000"
    severity: "warning"
    channels: ["slack"]
    cooldown: "10m"

  - name: "Low Disk Space"
    condition: "disk_usage > 0.9"
    severity: "error"
    channels: ["email"]
    cooldown: "1h"
```

### Alert Channels

- **Slack**: Real-time notifications for critical issues
- **Email**: Detailed reports and non-urgent alerts
- **PagerDuty**: Critical system alerts requiring immediate response
- **Webhook**: Integration with external monitoring systems

---

## Best Practices

### Monitoring Strategy

1. **Define SLIs/SLOs**: Service Level Indicators and Objectives
2. **Implement the Four Golden Signals**: Latency, Traffic, Errors, Saturation
3. **Use appropriate granularity**: Don't over-monitor or under-monitor
4. **Establish baselines**: Know normal vs abnormal behavior
5. **Automate responses**: Use alerts to trigger automated remediation

### Health Check Implementation

```csharp
// Health check example
public class DatabaseHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Database connectivity check
            await _dbContext.Database.CanConnectAsync(cancellationToken);

            // Additional checks (optional)
            var userCount = await _dbContext.Users.CountAsync(cancellationToken);

            return HealthCheckResult.Healthy(
                $"Database healthy. User count: {userCount}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Database check failed",
                ex);
        }
    }
}
```

### Log Aggregation

```csharp
// Structured logging example
_logger.LogInformation(
    "User login attempt",
    new
    {
        UserId = userId,
        IpAddress = ipAddress,
        UserAgent = userAgent,
        Timestamp = DateTime.UtcNow,
        Success = true
    });
```

### Performance Monitoring

```csharp
// Performance monitoring example
public async Task<IActionResult> GetProducts()
{
    using var activity = _activitySource.StartActivity("GetProducts");

    activity?.SetTag("operation", "get_products");
    activity?.SetTag("user.id", User.Identity.Name);

    try
    {
        var products = await _productService.GetAllAsync();
        activity?.SetTag("products.count", products.Count());

        return Ok(products);
    }
    catch (Exception ex)
    {
        activity?.SetTag("error", true);
        activity?.SetTag("error.message", ex.Message);

        throw;
    }
}
```

---

## Troubleshooting

### Common Issues

#### Health Check Failures
```bash
# Diagnose health check issues
monitoring-mcp/validate_health_checks workspacePath="." verbose=true

# Check specific health check
curl -f http://localhost:5000/health || echo "Health check failed"
```

#### High Error Rates
```bash
# Analyze error patterns
monitoring-mcp/track_errors workspacePath="." timeRange="1h" groupBy="type"

# Check application logs
monitoring-mcp/analyze_logs workspacePath="." logPath="logs/app.log" pattern="ERROR"
```

#### Performance Degradation
```bash
# Profile performance bottlenecks
monitoring-mcp/profile_performance workspacePath="." duration="60s"

# Monitor system resources
monitoring-mcp/monitor_resources workspacePath="." resources="all" interval="10s"
```

### Alert Fatigue Prevention

1. **Set appropriate thresholds**: Avoid false positives
2. **Use cooldown periods**: Prevent alert storms
3. **Implement alert grouping**: Combine related alerts
4. **Regular alert review**: Remove obsolete alerts
5. **Escalation policies**: Different response times for different severities

---

## Configuration

### MCP Server Configuration

```json
// .vscode/mcp.json
{
  "mcpServers": {
    "monitoring-mcp": {
      "disabled": false,
      "config": {
        "metricsEndpoint": "/metrics",
        "healthEndpoint": "/health",
        "alertsConfig": "monitoring/alerts.yml",
        "logPath": "logs/",
        "collectionInterval": "30s",
        "retentionPeriod": "30d"
      }
    }
  }
}
```

### Application Configuration

```json
// appsettings.json
{
  "Monitoring": {
    "Enabled": true,
    "Metrics": {
      "Enabled": true,
      "Endpoint": "/metrics",
      "Interval": "30s"
    },
    "HealthChecks": {
      "Enabled": true,
      "Endpoint": "/health",
      "Timeout": "30s"
    },
    "Logging": {
      "Level": "Information",
      "IncludeMetrics": true
    }
  }
}
```

### Docker Monitoring

```yaml
# docker-compose.yml
version: '3.8'
services:
  app:
    image: b2connect/app
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s

  prometheus:
    image: prom/prometheus
    volumes:
      - ./monitoring/prometheus.yml:/etc/prometheus/prometheus.yml
    ports:
      - "9090:9090"

  grafana:
    image: grafana/grafana
    environment:
      - GF_SECURITY_ADMIN_PASSWORD=admin
    ports:
      - "3000:3000"
```

---

## Dashboard Integration

### Grafana Dashboards

```json
// monitoring/dashboards/overview.json
{
  "dashboard": {
    "title": "B2Connect Overview",
    "panels": [
      {
        "title": "Response Time",
        "type": "graph",
        "targets": [
          {
            "expr": "histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m]))",
            "legendFormat": "P95"
          }
        ]
      },
      {
        "title": "Error Rate",
        "type": "graph",
        "targets": [
          {
            "expr": "rate(http_requests_total{status=~\"5..\"}[5m]) / rate(http_requests_total[5m]) * 100",
            "legendFormat": "Error Rate %"
          }
        ]
      }
    ]
  }
}
```

### Custom Metrics

```csharp
// Custom metrics example
public class CustomMetrics
{
    private readonly Counter<long> _requestsCounter;
    private readonly Histogram<double> _requestDuration;

    public CustomMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("B2Connect");

        _requestsCounter = meter.CreateCounter<long>(
            "http_requests_total",
            description: "Total number of HTTP requests");

        _requestDuration = meter.CreateHistogram<double>(
            "http_request_duration_seconds",
            description: "HTTP request duration in seconds");
    }

    public void RecordRequest(string method, string path, int statusCode, double duration)
    {
        _requestsCounter.Add(1, new KeyValuePair<string, object>("method", method),
                               new KeyValuePair<string, object>("path", path),
                               new KeyValuePair<string, object>("status", statusCode));

        _requestDuration.Record(duration, new KeyValuePair<string, object>("method", method),
                                       new KeyValuePair<string, object>("path", path));
    }
}
```

---

## Related Documentation

- [KB-055] Security MCP Best Practices
- [KB-057] Database MCP Usage Guide
- [KB-058] Testing MCP Usage Guide
- [GL-008] Governance Policies

---

**Maintained by**: @DevOps  
**Last Review**: 6. Januar 2026  
**Next Review**: 6. Februar 2026</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/.ai/knowledgebase/tools-and-tech/monitoring-mcp-usage.md