# Monitoring MCP Server

MCP server for service monitoring, telemetry collection, and log analysis in the B2Connect project.

## Features

- **Application Metrics Collection**: Real-time performance metrics from running services
- **System Performance Monitoring**: CPU, memory, disk, and network monitoring
- **Error Tracking**: Application error and exception analysis
- **Log Analysis**: Pattern detection and anomaly identification in logs
- **Health Check Validation**: Service availability and endpoint validation
- **Alert Configuration**: Configurable monitoring alerts and thresholds
- **Performance Profiling**: Application bottleneck identification

## Integration

This MCP server integrates with:
- .NET Aspire Dashboard (telemetry collection)
- OpenTelemetry (metrics and tracing)
- Application health check endpoints
- Log files and structured logging

## Available Tools

### collect_application_metrics
Collect real-time application performance metrics from running services.

```typescript
{
  "serviceName": "optional-service-name" // monitors all if not specified
}
```

### monitor_system_performance
Monitor system performance metrics (CPU, memory, disk, network).

```typescript
{
  "hostName": "localhost" // defaults to localhost
}
```

### track_errors
Track and analyze application errors and exceptions.

```typescript
{
  "serviceName": "service-name",
  "timeRange": "1h" // 1h, 24h, 7d, etc.
}
```

### analyze_logs
Analyze application logs for patterns and anomalies.

```typescript
{
  "filePath": "/path/to/log/file.log",
  "pattern": "optional-search-pattern"
}
```

### validate_health_checks
Validate health check endpoints for services.

```typescript
{
  "serviceName": "optional-service-name" // checks all if not specified
}
```

### configure_alerts
Configure monitoring alerts and thresholds.

```typescript
{
  "serviceName": "service-name",
  "metric": "cpu_usage|memory_usage|error_rate|response_time",
  "threshold": "80%|500ms|5"
}
```

### profile_performance
Profile application performance and identify bottlenecks.

```typescript
{
  "serviceName": "service-name",
  "duration": "30s" // 30s, 5m, etc.
}
```

## Setup

1. Install dependencies:
```bash
npm install
```

2. Build the server:
```bash
npm run build
```

3. Start the server:
```bash
npm start
```

## MCP Configuration

Add to your `.vscode/mcp.json`:

```json
{
  "mcpServers": {
    "monitoring-mcp": {
      "command": "node",
      "args": [
        "scripts/mcp-console-logger.js",
        "monitoring-mcp",
        "node",
        "tools/MonitoringMCP/dist/index.js"
      ],
      "env": {
        "NODE_ENV": "production",
        "LOG_TO_CONSOLE": "true"
      },
      "disabled": false
    }
  }
}
```

## Usage Examples

### Monitor Service Health
```bash
# Check all services
monitoring-mcp/validate_health_checks serviceName="all"

# Check specific service
monitoring-mcp/validate_health_checks serviceName="identity"
```

### Analyze Application Logs
```bash
monitoring-mcp/analyze_logs filePath="logs/application.log" pattern="error"
```

### Track Errors
```bash
monitoring-mcp/track_errors serviceName="api" timeRange="24h"
```

### Configure Alerts
```bash
monitoring-mcp/configure_alerts serviceName="database" metric="connection_pool_usage" threshold="90%"
```

## Development

- `npm run dev`: Start in development mode with auto-reload
- `npm run lint`: Run ESLint
- `npm run format`: Format code with Prettier
- `npm test`: Run tests

## Architecture

The Monitoring MCP server provides a standardized interface for:
- Telemetry data collection from .NET Aspire
- Health check endpoint monitoring
- Log file analysis and pattern detection
- System performance metrics
- Alert configuration and management

It serves as a bridge between development tools and the B2Connect monitoring infrastructure.