---
docid: UNKNOWN-041
title: REQ 001 Monitoring Scheduler Jobs
owner: @DocMaintainer
status: Archived
created: 2026-01-08
---

﻿# Monitoring for Scheduler Jobs: Requirements Analysis

**DocID**: `REQ-001`  
**Title**: Health Monitoring and Job Status Visualization for ERP/PIM/CRM Scheduler Jobs  
**Owner**: @ProductOwner  
**Status**: Approved  
**Created**: 2026-01-02  
**Last Updated**: 2026-01-02  
**Consolidated By**: @SARAH  

## Overview
This document captures the requirements for implementing comprehensive health monitoring, error logging, and status/progress visualization for scheduler jobs handling ERP/PIM/CRM integrations in B2X. The system must provide both backend monitoring capabilities and frontend visualization in the admin interface.

## Business Context
Scheduler jobs are critical for maintaining data synchronization across ERP (enventa Trade), PIM, and CRM systems. Failures can lead to data inconsistencies, operational disruptions, and compliance issues. Management needs high-level visibility, while operational teams require detailed job monitoring.

## User Stories

### Management-Level Health Monitoring
- **As a system administrator**, I want to see a high-level health dashboard so that I can quickly assess overall system status without technical details.
- **As a manager**, I want to receive alerts for critical job failures so that I can escalate issues promptly.
- **As an operations lead**, I want trend charts for job performance so that I can identify patterns and plan capacity.

### Connected Services Health & Connectivity
- **As a system administrator**, I want to see the availability status (health) of all connected external services (ERP, PIM, CRM) so that I can identify connectivity issues immediately.
- **As a support engineer**, I want to view error logs for each connected service so that I can diagnose integration failures.
- **As an operations lead**, I want to execute on-demand connection tests for any service so that I can verify connectivity before critical operations.
- **As a manager**, I want to see connection speed/latency reports so that I can identify performance bottlenecks in integrations.
- **As a system administrator**, I want to monitor CPU and memory consumption of connected services so that I can identify resource bottlenecks and plan capacity.
- **As an operations lead**, I want to receive alerts when service resource usage exceeds thresholds so that I can take action before outages occur.
- **As a support engineer**, I want detailed communication error logs (network failures, timeouts, protocol errors) so that I can diagnose connectivity issues at the network level.
- **As a system administrator**, I want to see comprehensive error details including request/response payloads, headers, and timing so that I can provide accurate information to external service providers.

### Scheduler Job Status/Progress Visualization
- **As an admin user**, I want to view real-time status of all active scheduler jobs so that I can monitor progress and intervene if needed.
- **As a support engineer**, I want detailed logs and error information for failed jobs so that I can troubleshoot issues efficiently.
- **As a tenant admin**, I want job status filtered by my tenant so that I see only relevant information.

### CLI-Based Status Monitoring
- **As a DevOps engineer**, I want to check system health and job status from the command line so that I can monitor systems via SSH or automation scripts.
- **As an operations lead**, I want CLI commands to list active jobs, view service status, and check error logs so that I can quickly diagnose issues without browser access.
- **As a system administrator**, I want CLI output in JSON/table format so that I can integrate monitoring into existing scripts and pipelines.
- **As a support engineer**, I want to trigger connection tests and view results from CLI so that I can troubleshoot connectivity from any environment.

## Functional Requirements

### Health Monitoring (Backend)
1. **Periodic Health Checks**
   - System must perform health checks every 30 seconds for critical components
   - Check database connectivity, ERP API availability, queue depths
   - Expose health status via ASP.NET Core Health Checks endpoint

2. **Error Monitoring & Logging**
   - Capture all job exceptions with structured logging (Serilog + Elasticsearch)
   - Classify errors (transient/permanent, ERP-specific/general)
   - Implement retry logic with exponential backoff for transient failures
   - Alert on error thresholds (>5 failures in 10 minutes)

3. **Status Monitoring**
   - Track job lifecycle: queued → running → completed/failed
   - Store job metadata: start time, duration, affected records, tenant ID
   - Provide real-time status updates via APIs

4. **Connected Services Health Monitoring**
   - Track availability status of all external services:
     - ERP (enventa Trade) - API connectivity, session status
     - PIM systems - API health, sync status
     - CRM systems - Connection status, authentication
     - Database connections - PostgreSQL, Elasticsearch
   - Store connection error logs with timestamps and error details
   - Historical availability tracking (uptime percentages)
   - **Resource Consumption Monitoring**:
     - CPU usage (current %, average, peak)
     - Memory consumption (used, available, peak)
     - Connection pool utilization
     - Thread/worker count (for Actor-based services)
     - Queue depth and processing rate
   - Resource usage alerts with configurable thresholds
   - Historical resource trends (hourly/daily/weekly)

6. **Communication Error Logging**
   - Capture and categorize all communication errors:
     - **Network Errors**: DNS failures, connection refused, network unreachable
     - **Timeout Errors**: Connection timeout, read timeout, write timeout
     - **Protocol Errors**: SSL/TLS handshake failures, certificate errors, HTTP errors
     - **Authentication Errors**: Invalid credentials, token expiry, permission denied
     - **Data Errors**: Serialization failures, schema mismatches, encoding issues
   - Store detailed error context:
     - Timestamp with millisecond precision
     - Service endpoint URL (sanitized)
     - HTTP method and status code
     - Request headers (sensitive values masked)
     - Request payload summary (truncated, PII masked)
     - Response headers and body (truncated)
     - Network timing breakdown (DNS, connect, TLS, first byte, total)
     - Retry attempt count and history
     - Correlation ID for request tracing
   - Error aggregation and pattern detection:
     - Group similar errors by type and endpoint
     - Track error frequency and trends
     - Identify recurring patterns (e.g., daily timeout spikes)

5. **On-Demand Connection Testing**
   - Provide API endpoint to trigger connection test for any service
   - Measure and report:
     - Connection latency (ms)
     - Response time (ms)
     - Throughput (requests/second capability)
     - SSL/TLS handshake time
     - DNS resolution time
   - Store test results for historical comparison
   - Generate detailed test report with pass/fail status

7. **CLI Status Monitoring**
   - Extend existing B2X CLI (`b2c`) with monitoring commands:
     - System health overview
     - Job status listing and filtering
     - Connected services status
     - Error log viewing
     - Connection testing
     - Resource metrics
   - Output formats:
     - Table format (human-readable, default)
     - JSON format (for scripting/automation)
     - Watch mode (auto-refresh)
   - Authentication via API key or token
   - Exit codes for scripting (0=success, 1=warning, 2=error)

### Frontend Visualization (Admin Interface)
1. **Management Dashboard**
   - Health status cards with color-coded indicators (green/yellow/red)
   - Summary metrics: success rates, active jobs count, recent failures
   - Trend charts for the last 7 days
   - Quick action buttons (view details, restart jobs)

2. **Connected Services Panel**
   - Service availability cards for each integration:
     - Service name and type (ERP/PIM/CRM/Database)
     - Current status (Online/Degraded/Offline)
     - Last successful connection timestamp
     - Average response time (last 24h)
     - Uptime percentage (last 7 days)
     - **Resource consumption indicators**:
       - CPU gauge (current % with color coding)
       - Memory gauge (used/total with color coding)
       - Mini sparkline for recent usage trend
   - Resource usage detail panel:
     - Real-time CPU/memory charts
     - Historical trends (1h, 24h, 7d views)
     - Threshold configuration
     - Alert history for resource events
   - Error log viewer per service:
     - Filterable by date range, severity, error type
     - Error message with stack trace (expandable)
     - Correlation to affected jobs
   - **Communication Error Details Panel**:
     - Error type classification badge (Network/Timeout/Protocol/Auth/Data)
     - Request/response inspector:
       - HTTP method, URL, status code
       - Headers viewer (collapsible, sensitive masked)
       - Payload viewer (JSON/XML formatted, truncated for large payloads)
     - Timing waterfall visualization:
       - DNS lookup time
       - TCP connection time
       - TLS handshake time
       - Time to first byte
       - Content download time
     - Retry history timeline
     - Copy to clipboard / Export for support tickets
     - "Report to Provider" template generator
   - Error pattern dashboard:
     - Error frequency heatmap by hour/day
     - Top error types pie chart
     - Error trend line chart
   - Connection test button per service:
     - "Test Connection" action button
     - Real-time test execution with progress indicator
     - Results panel showing:
       - Overall status (Pass/Fail)
       - Latency breakdown (DNS, SSL, Connect, Response)
       - Speed rating (Fast/Normal/Slow) with thresholds
       - Recommendations for slow connections
     - Export test report (PDF/JSON)
   - Historical latency chart per service

2. **Job Status Table**
   - List all jobs with columns: ID, Type, Status, Progress, Start Time, ETA
   - Filter by status, type, tenant, time range
   - Sort by any column
   - Pagination for large datasets

3. **Job Detail View**
   - Progress bar for running jobs
   - Step-by-step execution timeline
   - Error logs with expandable details
   - Retry/restart actions for failed jobs
   - Download logs option

### Technical Requirements
- **Performance**: Monitoring must not impact job performance (<5% overhead)
- **Scalability**: Support 1000+ concurrent jobs across multiple tenants
- **Security**: Role-based access, no sensitive data in logs, encrypted storage
- **Accessibility**: WCAG 2.1 AA compliance for all UI components
- **Integration**: Compatible with existing Aspire dashboard and Wolverine patterns

## Non-Functional Requirements
- **Availability**: 99.9% uptime for monitoring systems
- **Response Time**: <2 seconds for health checks, <5 seconds for job status queries
- **Data Retention**: Job logs retained for 90 days, health metrics for 1 year
- **Alert Response**: Critical alerts within 1 minute of detection

## Acceptance Criteria
- [ ] Management dashboard loads in <3 seconds with current health status
- [ ] Job status table updates in real-time for active jobs
- [ ] Failed jobs trigger email alerts to configured recipients
- [ ] All UI components are keyboard accessible and screen reader compatible
- [ ] System handles 1000 concurrent jobs without performance degradation
- [ ] Logs are searchable and filterable by tenant, job type, and error type

## Dependencies
- ASP.NET Core Health Checks
- Serilog with Elasticsearch sink
- Vue.js 3 with Pinia for state management
- Quartz.NET or Wolverine for job scheduling
- OpenTelemetry for distributed tracing

## Risks & Mitigations
- **Performance Impact**: Implement sampling for high-volume logging
- **Data Privacy**: Mask sensitive information in logs and UI
- **Scalability**: Use distributed caching for health metrics
- **ERP Integration Complexity**: Involve @Enventa for provider-specific monitoring

## Open Questions
- Should health monitoring be centralized or per bounded context?
- What alerting channels are preferred (email, Slack, SMS)?
- How to handle multi-tenant job isolation in UI?

## Agent Analysis Sections

### @Architect Analysis
**Completed**: 2026-01-02

#### System Design Recommendation
**Centralized Monitoring Service** with per-bounded-context health probes.

```
┌─────────────────────────────────────────────────────────────────┐
│                    Admin BFF (B2X.Admin)                  │
│  ┌─────────────────┐  ┌─────────────────┐  ┌────────────────┐  │
│  │ Health API      │  │ Job Status API  │  │ Metrics API    │  │
│  └────────┬────────┘  └────────┬────────┘  └───────┬────────┘  │
└───────────┼─────────────────────┼──────────────────┼────────────┘
            │                     │                  │
            ▼                     ▼                  ▼
┌─────────────────────────────────────────────────────────────────┐
│                  Monitoring Service (Shared)                    │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐  │
│  │ Health       │  │ Job State    │  │ Metrics Aggregator   │  │
│  │ Aggregator   │  │ Manager      │  │ (OpenTelemetry)      │  │
│  └──────────────┘  └──────────────┘  └──────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
            │                     │                  │
     ┌──────┴──────┬──────────────┴───────┬─────────┴────────┐
     ▼             ▼                      ▼                  ▼
┌─────────┐  ┌─────────┐  ┌────────────────────┐  ┌──────────────┐
│ Catalog │  │ Search  │  │ ERP Provider       │  │ PIM/CRM      │
│ Domain  │  │ Domain  │  │ (enventa Actor)    │  │ Provider     │
└─────────┘  └─────────┘  └────────────────────┘  └──────────────┘
```

#### Service Boundaries
1. **Monitoring.Shared** - Core monitoring abstractions and interfaces
2. **Monitoring.Infrastructure** - Persistence, Elasticsearch integration
3. **Admin.Monitoring** - API endpoints for admin frontend
4. **Per-Domain Health Probes** - Lightweight probes in each bounded context

#### Key Decisions
- **Centralized vs. Distributed**: Hybrid approach - centralized aggregation with distributed probes
- **Real-time Updates**: SignalR hub for live job status push to frontend
- **Data Storage**: PostgreSQL for job state, Elasticsearch for logs/metrics
- **Scalability**: Horizontal scaling via Kubernetes, job queues via Wolverine

#### ADR Required
Will create `ADR-024-scheduler-job-monitoring.md` after consolidation.

---

### @Backend Analysis
**Completed**: 2026-01-02

#### API Design

**Health Endpoints** (Admin BFF):
```
GET  /api/admin/health/summary          → Overall system health
GET  /api/admin/health/components       → Per-component health status
GET  /api/admin/health/history          → Health history (last 24h)
```

**Connected Services Endpoints** (Admin BFF):
```
GET  /api/admin/services                → List all connected services with status
GET  /api/admin/services/{id}           → Service details with config
GET  /api/admin/services/{id}/status    → Current availability status
GET  /api/admin/services/{id}/errors    → Error log for service (paginated)
POST /api/admin/services/{id}/test      → Execute connection test
GET  /api/admin/services/{id}/test/{testId} → Get test results
GET  /api/admin/services/{id}/latency   → Historical latency data
GET  /api/admin/services/{id}/resources → Current resource metrics (CPU/memory)
GET  /api/admin/services/{id}/resources/history → Historical resource data
PUT  /api/admin/services/{id}/resources/thresholds → Configure alert thresholds
GET  /api/admin/services/{id}/resources/alerts → Resource alert history
GET  /api/admin/services/{id}/communication-errors → Communication error log (paginated, filtered)
GET  /api/admin/services/{id}/communication-errors/{errorId} → Detailed error with request/response
GET  /api/admin/services/{id}/communication-errors/patterns → Error pattern analysis
GET  /api/admin/services/{id}/communication-errors/export → Export errors for support ticket
```

**Job Status Endpoints** (Admin BFF):
```
GET  /api/admin/jobs                    → List jobs (paginated, filtered)
GET  /api/admin/jobs/{id}               → Job details with logs
GET  /api/admin/jobs/{id}/logs          → Detailed execution logs
POST /api/admin/jobs/{id}/retry         → Retry failed job
POST /api/admin/jobs/{id}/cancel        → Cancel running job
```

**Real-time Hub** (SignalR):
```
Hub: /hubs/job-status
  → JobStatusChanged(jobId, status, progress)
  → HealthStatusChanged(component, status)
  → AlertTriggered(alert)
  → ResourceMetricsUpdated(serviceId, metrics)   // Real-time CPU/memory updates
  → ResourceAlertTriggered(serviceId, alert)     // Threshold exceeded
```

#### Data Models

```csharp
public record SchedulerJob
{
    public Guid Id { get; init; }
    public string JobType { get; init; }           // ERP_SYNC, PIM_IMPORT, CRM_EXPORT
    public string TenantId { get; init; }
    public JobStatus Status { get; init; }         // Queued, Running, Completed, Failed, Cancelled
    public int ProgressPercent { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string? ErrorMessage { get; init; }
    public Dictionary<string, object> Metadata { get; init; }
}

public record JobExecutionLog
{
    public Guid Id { get; init; }
    public Guid JobId { get; init; }
    public LogLevel Level { get; init; }
    public string Message { get; init; }
    public DateTime Timestamp { get; init; }
    public string? StackTrace { get; init; }
    public Dictionary<string, object> Context { get; init; }
}

public enum JobStatus { Queued, Running, Completed, Failed, Cancelled }
```

**Connected Service Models**:
```csharp
public record ConnectedService
{
    public Guid Id { get; init; }
    public string Name { get; init; }              // "enventa Trade ERP", "PIM System"
    public ServiceType Type { get; init; }         // ERP, PIM, CRM, Database
    public string Endpoint { get; init; }          // Connection URL/endpoint
    public ServiceStatus Status { get; init; }     // Online, Degraded, Offline
    public DateTime LastChecked { get; init; }
    public DateTime? LastSuccessful { get; init; }
    public double AverageLatencyMs { get; init; }
    public double UptimePercent { get; init; }     // Last 7 days
    public ResourceMetrics Resources { get; init; } // CPU/Memory metrics
}

public record ResourceMetrics
{
    public double CpuPercent { get; init; }        // Current CPU usage %
    public double CpuAverage { get; init; }        // Average CPU (last hour)
    public double CpuPeak { get; init; }           // Peak CPU (last 24h)
    public long MemoryUsedBytes { get; init; }     // Current memory used
    public long MemoryTotalBytes { get; init; }    // Total available memory
    public double MemoryPercent { get; init; }     // Memory usage %
    public long MemoryPeakBytes { get; init; }     // Peak memory (last 24h)
    public int ConnectionPoolActive { get; init; } // Active connections
    public int ConnectionPoolTotal { get; init; }  // Total pool size
    public int ThreadCount { get; init; }          // Active threads/workers
    public int QueueDepth { get; init; }           // Pending items in queue
    public DateTime Timestamp { get; init; }
}

public record ResourceAlert
{
    public Guid Id { get; init; }
    public Guid ServiceId { get; init; }
    public ResourceAlertType AlertType { get; init; }  // CpuHigh, MemoryHigh, PoolExhausted
    public double ThresholdValue { get; init; }
    public double ActualValue { get; init; }
    public DateTime TriggeredAt { get; init; }
    public DateTime? ResolvedAt { get; init; }
    public string Message { get; init; }
}

public enum ResourceAlertType 
{ 
    CpuHigh,           // CPU > threshold
    CpuSustained,      // CPU high for extended period
    MemoryHigh,        // Memory > threshold
    MemoryCritical,    // Memory > 95%
    PoolExhausted,     // Connection pool at capacity
    QueueBacklog       // Queue depth exceeds threshold
}

public record ServiceError
{
    public Guid Id { get; init; }
    public Guid ServiceId { get; init; }
    public DateTime Timestamp { get; init; }
    public string ErrorCode { get; init; }
    public string Message { get; init; }
    public string? StackTrace { get; init; }
    public ErrorSeverity Severity { get; init; }   // Warning, Error, Critical
    public Guid? CorrelatedJobId { get; init; }    // Link to affected job
}

public record ConnectionTestResult
{
    public Guid Id { get; init; }
    public Guid ServiceId { get; init; }
    public DateTime ExecutedAt { get; init; }
    public bool Success { get; init; }
    public ConnectionTestMetrics Metrics { get; init; }
    public string? ErrorMessage { get; init; }
    public string SpeedRating { get; init; }       // Fast, Normal, Slow
    public List<string> Recommendations { get; init; }
}

public record ConnectionTestMetrics
{
    public double DnsResolutionMs { get; init; }
    public double TlsHandshakeMs { get; init; }
    public double ConnectionMs { get; init; }
    public double ResponseMs { get; init; }
    public double TotalLatencyMs { get; init; }
    public double ThroughputKbps { get; init; }
}

public enum ServiceType { ERP, PIM, CRM, Database, MessageQueue, Cache }
public enum ServiceStatus { Online, Degraded, Offline, Unknown }
public enum ErrorSeverity { Warning, Error, Critical }
```

**Communication Error Models**:
```csharp
public record CommunicationError
{
    public Guid Id { get; init; }
    public Guid ServiceId { get; init; }
    public DateTime Timestamp { get; init; }
    public CommunicationErrorType ErrorType { get; init; }
    public string ErrorCode { get; init; }
    public string Message { get; init; }
    public string? StackTrace { get; init; }
    public ErrorSeverity Severity { get; init; }
    public Guid? CorrelatedJobId { get; init; }
    public CommunicationErrorDetails Details { get; init; }
}

public record CommunicationErrorDetails
{
    // Request Information
    public string HttpMethod { get; init; }
    public string EndpointUrl { get; init; }           // Sanitized (no secrets in query params)
    public int? HttpStatusCode { get; init; }
    public Dictionary<string, string> RequestHeaders { get; init; }  // Sensitive values masked
    public string? RequestPayloadSummary { get; init; } // Truncated, PII masked
    public long? RequestPayloadSizeBytes { get; init; }
    
    // Response Information
    public Dictionary<string, string>? ResponseHeaders { get; init; }
    public string? ResponseBodySummary { get; init; }   // Truncated
    public long? ResponsePayloadSizeBytes { get; init; }
    
    // Timing Information
    public NetworkTimingMetrics Timing { get; init; }
    
    // Retry Information
    public int RetryAttempt { get; init; }
    public int MaxRetries { get; init; }
    public List<RetryAttemptLog> RetryHistory { get; init; }
    
    // Tracing
    public string CorrelationId { get; init; }
    public string? TraceId { get; init; }
    public string? SpanId { get; init; }
}

public record NetworkTimingMetrics
{
    public double? DnsLookupMs { get; init; }
    public double? TcpConnectionMs { get; init; }
    public double? TlsHandshakeMs { get; init; }
    public double? TimeToFirstByteMs { get; init; }
    public double? ContentDownloadMs { get; init; }
    public double TotalTimeMs { get; init; }
}

public record RetryAttemptLog
{
    public int AttemptNumber { get; init; }
    public DateTime AttemptedAt { get; init; }
    public string ErrorMessage { get; init; }
    public double DurationMs { get; init; }
    public TimeSpan DelayBeforeRetry { get; init; }
}

```

#### CLI Command Reference

**Health & Status Commands**:
```bash
# System health overview
b2c monitor health                    # Overall system health summary
b2c monitor health --detailed         # Detailed health per component
b2c monitor health --watch            # Auto-refresh every 5 seconds
b2c monitor health --format json      # JSON output for scripting

# Connected services status
b2c monitor services                  # List all services with status
b2c monitor services --status degraded,offline  # Filter by status
b2c monitor services erp              # Show specific service details
b2c monitor services erp --resources  # Include CPU/memory metrics
b2c monitor services --watch          # Live updates

# Connection testing
b2c monitor test erp                  # Run connection test for ERP
b2c monitor test all                  # Test all services
b2c monitor test erp --verbose        # Detailed timing breakdown
b2c monitor test erp --format json    # JSON output
```

**Job Monitoring Commands**:
```bash
# Job listing
b2c jobs list                         # List recent jobs (last 24h)
b2c jobs list --status running        # Filter by status
b2c jobs list --type ERP_SYNC         # Filter by type
b2c jobs list --tenant tenant-123     # Filter by tenant
b2c jobs list --limit 50              # Limit results
b2c jobs list --watch                 # Live updates

# Job details
b2c jobs show <job-id>                # Job details with logs
b2c jobs logs <job-id>                # Full execution logs
b2c jobs logs <job-id> --tail 100     # Last 100 log entries
b2c jobs logs <job-id> --follow       # Stream logs in real-time

# Job actions
b2c jobs retry <job-id>               # Retry failed job
b2c jobs cancel <job-id>              # Cancel running job
```

**Error & Alert Commands**:
```bash
# Error logs
b2c monitor errors                    # Recent errors across all services
b2c monitor errors erp                # Errors for specific service
b2c monitor errors --type timeout     # Filter by error type
b2c monitor errors --severity critical # Filter by severity
b2c monitor errors --since 1h         # Errors in last hour
b2c monitor errors --export report.json # Export for support

# Alerts
b2c monitor alerts                    # Active alerts
b2c monitor alerts --history          # Alert history
b2c monitor alerts ack <alert-id>     # Acknowledge alert
```

**Resource Monitoring Commands**:
```bash
# Resource metrics
b2c monitor resources                 # Current CPU/memory all services
b2c monitor resources erp             # Resources for specific service
b2c monitor resources --watch         # Live resource monitoring
b2c monitor resources --top           # Top resource consumers
```

**Example CLI Output**:
```
$ b2c monitor health

╔══════════════════════════════════════════════════════════════╗
║  B2X System Health                     ✓ HEALTHY       ║
╠══════════════════════════════════════════════════════════════╣
║  Component          Status    Latency    Uptime    CPU  Mem  ║
╠══════════════════════════════════════════════════════════════╣
║  enventa ERP        ● Online    45ms     99.8%    45%  68%  ║
║  PIM System         ● Online   120ms     99.5%    22%  51%  ║
║  CRM Integration    ⚠ Degraded 850ms     94.1%    87%  92%  ║
║  PostgreSQL         ● Online     5ms    100.0%    15%  45%  ║
║  Elasticsearch      ● Online    12ms     99.9%    35%  62%  ║
╠══════════════════════════════════════════════════════════════╣
║  Active Jobs: 3     Failed (24h): 2     Alerts: 1 active    ║
╚══════════════════════════════════════════════════════════════╝
```

```
$ b2c jobs list --status running

╔════════════════════════════════════════════════════════════════════╗
║  Active Jobs                                                       ║
╠══════════╦═════════════╦══════════╦══════════╦═══════════╦════════╣
║  ID      ║ Type        ║ Status   ║ Progress ║ Started   ║ ETA    ║
╠══════════╬═════════════╬══════════╬══════════╬═══════════╬════════╣
║  #1234   ║ ERP_SYNC    ║ ● Running║ ████░ 72%║ 10:23 AM  ║ ~8 min ║
║  #1237   ║ PIM_IMPORT  ║ ● Running║ ██░░░ 35%║ 10:45 AM  ║ ~15 min║
║  #1238   ║ STOCK_SYNC  ║ ● Running║ █████ 95%║ 10:50 AM  ║ <1 min ║
╚══════════╩═════════════╩══════════╩══════════╩═══════════╩════════╝

3 jobs running. Use 'b2c jobs show <id>' for details.
```

```
$ b2c monitor test erp --verbose

Testing connection to enventa Trade ERP...

  ✓ DNS Resolution         12ms
  ✓ TCP Connection         18ms  
  ✓ TLS Handshake          25ms
  ✓ HTTP Response          25ms
  ─────────────────────────────
  ✓ Total                  80ms   ⚡ Fast

  Throughput: 2.4 MB/s
  Status: PASSED

Exit code: 0
```

```
$ b2c monitor errors --since 1h --type timeout

╔════════════════════════════════════════════════════════════════════╗
║  Communication Errors (Last 1 hour)                    3 errors    ║
╠═══════════════╦═══════════╦═══════════════════════════════════════╣
║  Time         ║ Service   ║ Error                                 ║
╠═══════════════╬═══════════╬═══════════════════════════════════════╣
║  14:22:05     ║ CRM       ║ ConnectionTimeout: 30s exceeded       ║
║  14:15:30     ║ CRM       ║ ReadTimeout: No response after 30s    ║
║  13:58:12     ║ CRM       ║ ConnectionTimeout: 30s exceeded       ║
╚═══════════════╩═══════════╩═══════════════════════════════════════╝

Pattern: 3 timeout errors from CRM in 1 hour. Check service status.
Use 'b2c monitor errors <error-id>' for details.
```

**Exit Codes**:
| Code | Meaning | Use Case |
|------|---------|----------|
| 0 | Success / Healthy | All checks passed |
| 1 | Warning / Degraded | Some services degraded |
| 2 | Error / Unhealthy | Critical failures |
| 3 | Connection Error | Cannot reach API |

**Configuration**:
```bash
# Set API endpoint
b2c config set api-url https://admin.B2X.local

# Authenticate
b2c auth login                        # Interactive login
b2c auth token <api-key>              # Set API key for scripts

# Set default output format
b2c config set format json            # Always output JSON
```

public enum CommunicationErrorType
{
    // Network Errors
    DnsResolutionFailed,
    ConnectionRefused,
    NetworkUnreachable,
    HostUnreachable,
    
    // Timeout Errors
    ConnectionTimeout,
    ReadTimeout,
    WriteTimeout,
    RequestTimeout,
    
    // Protocol Errors
    SslHandshakeFailed,
    CertificateInvalid,
    CertificateExpired,
    HttpProtocolError,
    
    // Authentication Errors
    AuthenticationFailed,
    TokenExpired,
    PermissionDenied,
    InvalidCredentials,
    
    // Data Errors
    SerializationFailed,
    DeserializationFailed,
    SchemaMismatch,
    EncodingError,
    PayloadTooLarge,
    
    // Service Errors
    ServiceUnavailable,
    RateLimited,
    CircuitBreakerOpen,
    Unknown
}
```

#### Wolverine Integration
- Use Wolverine's built-in message tracking for job correlation
- Implement `IJobMonitor` interface for consistent status updates
- Leverage Wolverine's retry policies with monitoring hooks

#### Implementation Approach
1. Create `B2X.Shared.Monitoring` package
2. Implement job tracking middleware for Wolverine handlers
3. Add health check contributors per domain
4. SignalR hub for real-time updates
5. **Extend existing B2X CLI** (`backend/CLI/B2X.CLI/`)
   - Add `Commands/MonitorCommands/` for health, services, errors
   - Add `Commands/JobCommands/` for job management
   - Implement table and JSON output formatters
   - Add watch mode with terminal refresh

---

### @Frontend Analysis
**Completed**: 2026-01-02

#### Component Structure

```
frontend/Admin/src/
├── features/
│   └── monitoring/
│       ├── components/
│       │   ├── HealthDashboard.vue         # Main dashboard view
│       │   ├── HealthStatusCard.vue        # Individual health indicator
│       │   ├── JobStatusTable.vue          # Job list with filters
│       │   ├── JobDetailPanel.vue          # Slide-out detail view
│       │   ├── JobProgressBar.vue          # Visual progress indicator
│       │   ├── JobLogViewer.vue            # Log display with search
│       │   ├── TrendChart.vue              # 7-day performance chart
│       │   ├── AlertBanner.vue             # Critical alert notification
│       │   ├── ConnectedServicesPanel.vue  # Services overview
│       │   ├── ServiceStatusCard.vue       # Per-service health card
│       │   ├── ServiceErrorLog.vue         # Error log viewer
│       │   ├── ConnectionTestDialog.vue    # Test execution modal
│       │   ├── ConnectionTestResults.vue   # Test results display
│       │   └── LatencyChart.vue            # Historical latency graph
│       ├── composables/
│       │   ├── useJobMonitoring.ts         # Job status logic
│       │   ├── useHealthStatus.ts          # Health check logic
│       │   ├── useConnectedServices.ts     # Service status logic
│       │   ├── useConnectionTest.ts        # Connection test logic
│       │   └── useSignalR.ts               # Real-time connection
│       ├── stores/
│       │   ├── jobStore.ts                 # Pinia store for jobs
│       │   ├── healthStore.ts              # Pinia store for health
│       │   └── servicesStore.ts            # Pinia store for services
│       ├── types/
│       │   └── monitoring.types.ts         # TypeScript interfaces
│       └── api/
│           ├── monitoringApi.ts            # API client
│           └── servicesApi.ts              # Services API client
```

#### State Management (Pinia)

```typescript
// stores/jobStore.ts
export const useJobStore = defineStore('jobs', () => {
  const jobs = ref<SchedulerJob[]>([])
  const activeJob = ref<SchedulerJob | null>(null)
  const filters = ref<JobFilters>({ status: [], type: [], tenant: null })
  const isLoading = ref(false)

  // Real-time updates via SignalR
  const updateJobStatus = (jobId: string, status: JobStatus, progress: number) => {
    const job = jobs.value.find(j => j.id === jobId)
    if (job) {
      job.status = status
      job.progressPercent = progress
    }
  }

  return { jobs, activeJob, filters, isLoading, updateJobStatus }
})
```

#### Real-time Updates
- SignalR client with automatic reconnection
- Optimistic UI updates for job actions
- Debounced polling fallback if WebSocket fails

#### Key Features
- Virtual scrolling for large job lists (vue-virtual-scroller)
- Chart.js for trend visualization
- Keyboard shortcuts for power users
- Export to CSV/Excel

---

### @UI Analysis
**Completed**: 2026-01-02

#### Design System Components

| Component | Purpose | Accessibility |
|-----------|---------|---------------|
| `StatusBadge` | Job/health status indicator | Color + icon + text label |
| `ProgressBar` | Job completion progress | aria-valuenow, aria-valuemax |
| `DataTable` | Sortable, filterable job list | Keyboard navigation, screen reader |
| `SlidePanel` | Job detail overlay | Focus trap, Escape to close |
| `AlertToast` | Critical notifications | role="alert", auto-announce |
| `TrendChart` | Performance visualization | Alt text summary, data table fallback |

#### Color Scheme (Status Indicators)
```css
--status-healthy: #22c55e;    /* Green - operational */
--status-degraded: #f59e0b;   /* Amber - warning */
--status-unhealthy: #ef4444;  /* Red - critical */
--status-unknown: #6b7280;    /* Gray - no data */
```

#### Accessibility Requirements (WCAG 2.1 AA)
- **Color Contrast**: 4.5:1 minimum for text
- **Focus Indicators**: Visible focus rings on all interactive elements
- **Screen Reader**: Status changes announced via aria-live regions
- **Keyboard**: Full functionality without mouse
- **Motion**: Respect prefers-reduced-motion for animations

#### Responsive Design
- **Desktop (>1024px)**: Full dashboard with side panel
- **Tablet (768-1024px)**: Stacked cards, collapsible table
- **Mobile (<768px)**: Card-based list, bottom sheet for details

#### Wireframe Sketches
```
┌────────────────────────────────────────────────────────────┐
│ System Health                                    [⟳ 30s]   │
├────────────┬────────────┬────────────┬────────────────────┤
│ ● ERP Sync │ ● PIM      │ ● CRM      │ ● Database         │
│   Healthy  │   Healthy  │   Degraded │   Healthy          │
│   98.5%    │   99.2%    │   94.1%    │   100%             │
└────────────┴────────────┴────────────┴────────────────────┘

┌────────────────────────────────────────────────────────────┐
│ Connected Services                               [⟳ Auto]  │
├────────────────────────────────────────────────────────────┤
│ ┌──────────────────────┐  ┌──────────────────────┐        │
│ │ enventa Trade ERP    │  │ PIM System           │        │
│ │ ● Online             │  │ ● Online             │        │
│ │ Latency: 45ms (Fast) │  │ Latency: 120ms (Norm)│        │
│ │ Uptime: 99.8%        │  │ Uptime: 99.5%        │        │
│ │ CPU: ██░░ 45%        │  │ CPU: █░░░ 22%        │        │
│ │ Mem: ███░ 68%        │  │ Mem: ██░░ 51%        │        │
│ │ Last OK: 2 min ago   │  │ Last OK: 1 min ago   │        │
│ │ [Test] [Logs] [▼]    │  │ [Test] [Logs] [▼]    │        │
│ └──────────────────────┘  └──────────────────────┘        │
│ ┌──────────────────────┐  ┌──────────────────────┐        │
│ │ CRM Integration      │  │ PostgreSQL           │        │
│ │ ● Degraded           │  │ ● Online             │        │
│ │ Latency: 850ms (Slow)│  │ Latency: 5ms (Fast)  │        │
│ │ Uptime: 94.1%        │  │ Uptime: 100%         │        │
│ │ CPU: ████ 87% ⚠      │  │ CPU: █░░░ 15%        │        │
│ │ Mem: ████ 92% ⚠      │  │ Mem: ██░░ 45%        │        │
│ │ Errors: 3 today      │  │ Last OK: just now    │        │
│ │ [Test] [Logs] [▼]    │  │ [Test] [Logs] [▼]    │        │
│ └──────────────────────┘  └──────────────────────┘        │
└────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────┐
│ Communication Error Details              [Copy] [Export]   │
├────────────────────────────────────────────────────────────┤
│ Error Type: ⚠ TIMEOUT          Severity: Error            │
│ Time: 2026-01-02 14:22:05.342  Correlation: abc-123-def   │
├────────────────────────────────────────────────────────────┤
│ REQUEST                                                    │
│ POST https://erp.example.com/api/v2/articles/sync         │
│ ┌────────────────────────────────────────────────────────┐│
│ │ Headers:                                               ││
│ │   Content-Type: application/json                       ││
│ │   Authorization: Bearer ****masked****                 ││
│ │   X-Tenant-Id: tenant-123                              ││
│ │ Body (truncated):                                      ││
│ │   { "articles": [...], "count": 1500 }                 ││
│ │   Size: 2.4 MB                                         ││
│ └────────────────────────────────────────────────────────┘│
├────────────────────────────────────────────────────────────┤
│ RESPONSE                                                   │
│ Status: — (No response - timeout)                         │
├────────────────────────────────────────────────────────────┤
│ TIMING WATERFALL                                          │
│ DNS:     │██░░░░░░░░│  12ms                               │
│ TCP:     │███░░░░░░░│  18ms                               │
│ TLS:     │████░░░░░░│  25ms                               │
│ Wait:    │██████████████████████████████│ 30,000ms ⚠      │
│ ─────────────────────────────────────────                 │
│ Total:   30,055ms (TIMEOUT after 30s)                     │
├────────────────────────────────────────────────────────────┤
│ RETRY HISTORY                                             │
│ #1 14:21:35 → Timeout after 30s → Wait 5s                │
│ #2 14:21:40 → Timeout after 30s → Wait 10s               │
│ #3 14:22:05 → Timeout after 30s → FAILED (max retries)   │
├────────────────────────────────────────────────────────────┤
│ [Generate Support Ticket Template]                        │
└────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────┐
│ Error Pattern Analysis - enventa ERP    [7d ▼] [Export]   │
├────────────────────────────────────────────────────────────┤
│ Error Frequency Heatmap (by hour)                         │
│      00 04 08 12 16 20                                    │
│ Mon  ░░░░░░██░░░░░░                                       │
│ Tue  ░░░░░░██░░░░░░                                       │
│ Wed  ░░░░░░██████░░  ← Peak: 14:00-16:00                 │
│ Thu  ░░░░░░██░░░░░░                                       │
│ Fri  ░░░░░░██░░░░░░                                       │
├────────────────────────────────────────────────────────────┤
│ Top Error Types           │ Trend (7 days)                │
│ ┌───────────────────────┐ │ ┌───────────────────────────┐│
│ │ Timeout     ████ 45%  │ │ │    ╭╮                     ││
│ │ Auth        ██░░ 25%  │ │ │ ╭──╯╰─╮  ╭──             ││
│ │ Network     █░░░ 18%  │ │ │─╯     ╰──╯               ││
│ │ Protocol    ░░░░ 12%  │ │ │ Mon Tue Wed Thu Fri      ││
│ └───────────────────────┘ │ └───────────────────────────┘│
├────────────────────────────────────────────────────────────┤
│ Pattern Detected: Timeout errors spike during ERP backup  │
│ window (14:00-16:00 daily). Consider scheduling sync jobs │
│ outside this window.                                      │
└────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────┐
│ Connection Test Results - enventa Trade ERP      [Export]  │
├────────────────────────────────────────────────────────────┤
│ Status: ✓ PASSED                    Speed: ⚡ Fast         │
├────────────────────────────────────────────────────────────┤
│ Latency Breakdown:                                         │
│   DNS Resolution:    │███░░░░░░░│  12ms                   │
│   TLS Handshake:     │████░░░░░░│  18ms                   │
│   Connection:        │██░░░░░░░░│   8ms                   │
│   Response:          │█████░░░░░│  25ms                   │
│   ─────────────────────────────────────                   │
│   Total:             │█████████░│  63ms                   │
├────────────────────────────────────────────────────────────┤
│ Throughput: 2.4 MB/s                                       │
│ Tested: 2026-01-02 14:35:22                               │
└────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────┐
│ Resource Monitor - enventa Trade ERP          [1h ▼] [⚙]  │
├────────────────────────────────────────────────────────────┤
│ CPU Usage                              Memory Usage        │
│ ┌─────────────────────────┐  ┌─────────────────────────┐  │
│ │     ╭──╮                │  │         ╭───╮          │  │
│ │  ╭──╯  ╰─╮   ╭──╮      │  │    ╭────╯   ╰──╮       │  │
│ │──╯       ╰───╯  ╰──────│  │────╯           ╰───────│  │
│ │ 0%              45% ▲  │  │ 0%              68% ▲  │  │
│ └─────────────────────────┘  └─────────────────────────┘  │
│ Current: 45% │ Avg: 38% │ Peak: 78%   Used: 2.7GB / 4GB   │
├────────────────────────────────────────────────────────────┤
│ Connection Pool: ████████░░ 8/10    Threads: 12           │
│ Queue Depth: 23 items               Processing: 45/min    │
├────────────────────────────────────────────────────────────┤
│ Thresholds: CPU > 80% ⚠  Memory > 90% ⚠  Pool > 90% ⚠    │
│ [Configure Thresholds]                                    │
└────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────┐
│ Service Error Log - CRM Integration    [Filter ▼] [Export] │
├──────────┬──────────┬───────────────────────────┬─────────┤
│ Time     │ Severity │ Message                   │ Job     │
├──────────┼──────────┼───────────────────────────┼─────────┤
│ 14:22:05 │ ⚠ Error  │ Connection timeout after  │ #1236   │
│          │          │ 30s - retry scheduled     │         │
├──────────┼──────────┼───────────────────────────┼─────────┤
│ 13:45:12 │ ⚠ Error  │ Authentication failed:    │ #1234   │
│          │          │ Token expired             │         │
├──────────┼──────────┼───────────────────────────┼─────────┤
│ 12:30:00 │ ⛔ Crit  │ Service unavailable -     │ -       │
│          │          │ all retries exhausted     │         │
└──────────┴──────────┴───────────────────────────┴─────────┘

┌────────────────────────────────────────────────────────────┐
│ Active Jobs                          [Filter ▼] [Search]   │
├──────┬──────────┬────────┬──────────┬──────────┬──────────┤
│ ID   │ Type     │ Status │ Progress │ Started  │ Actions  │
├──────┼──────────┼────────┼──────────┼──────────┼──────────┤
│ 1234 │ ERP Sync │ ● Run  │ ████░ 72%│ 10:23 AM │ [⋮]      │
│ 1235 │ PIM Imp  │ ● Done │ █████100%│ 10:15 AM │ [⋮]      │
│ 1236 │ CRM Exp  │ ● Fail │ ██░░░ 45%│ 10:05 AM │ [⋮]      │
└──────┴──────────┴────────┴──────────┴──────────┴──────────┘
```

---

### @DevOps Analysis
**Completed**: 2026-01-02

#### Infrastructure Requirements

**Monitoring Stack**:
- **Metrics**: Prometheus + Grafana (existing Aspire integration)
- **Logs**: Elasticsearch + Kibana (centralized logging)
- **Tracing**: Jaeger via OpenTelemetry
- **Alerts**: Alertmanager → Email/Slack/PagerDuty

**Resource Estimates**:
| Component | CPU | Memory | Storage |
|-----------|-----|--------|---------|
| Monitoring Service | 0.5 core | 512MB | - |
| Elasticsearch | 2 cores | 4GB | 100GB |
| SignalR Hub | 0.25 core | 256MB | - |

#### Kubernetes Configuration
```yaml
# Health probe for scheduler jobs
livenessProbe:
  httpGet:
    path: /health/live
    port: 8080
  initialDelaySeconds: 10
  periodSeconds: 30

readinessProbe:
  httpGet:
    path: /health/ready
    port: 8080
  initialDelaySeconds: 5
  periodSeconds: 10
```

#### Alerting Rules
```yaml
# Prometheus alert for job failures
- alert: SchedulerJobFailureRate
  expr: rate(scheduler_job_failures_total[5m]) > 0.1
  for: 2m
  labels:
    severity: critical
  annotations:
    summary: "High scheduler job failure rate"
```

#### Deployment Considerations
- Canary deployments for monitoring service updates
- Blue-green for SignalR hub (maintain connections)
- Horizontal Pod Autoscaler for peak loads

---

### @Security Analysis
**Completed**: 2026-01-02

#### Security Controls

**Data Protection**:
- ✅ PII masking in logs (email, names, addresses)
- ✅ Encryption at rest (AES-256 for job metadata)
- ✅ TLS 1.3 for all API/SignalR communication
- ✅ Log retention policy (90 days, then archive/delete)

**Access Control**:
- ✅ Role-based access: Admin, Operator, Viewer
- ✅ Tenant isolation in job queries
- ✅ Audit logging for job actions (retry, cancel)
- ✅ Rate limiting on API endpoints

**Compliance**:
- ✅ GDPR: Right to erasure includes job logs with PII
- ✅ NIS2: Incident logging and reporting capability
- ✅ SOC 2: Audit trail for all administrative actions

#### Threat Mitigations
| Threat | Mitigation |
|--------|------------|
| Log injection | Sanitize all log inputs |
| Unauthorized job control | RBAC + audit logging |
| Data leakage via logs | PII masking, access controls |
| DoS via SignalR | Connection limits, rate limiting |

#### Security Review Checkpoint
- [ ] Penetration test for monitoring APIs
- [ ] Review log masking rules
- [ ] Validate tenant isolation

---

### @Enventa Analysis
**Completed**: 2026-01-02

#### ERP-Specific Monitoring Needs

**Actor Pattern Considerations**:
- Single-threaded Actor must report health without blocking
- Queue depth monitoring for Actor mailbox
- Timeout tracking for long-running ERP operations

**enventa Trade Specific Metrics**:
```
erp_actor_queue_depth          # Messages waiting in Actor mailbox
erp_actor_processing_time_ms   # Time per operation
erp_connection_status          # 0=disconnected, 1=connected
erp_bulk_operation_progress    # For large imports/exports
erp_session_count              # Active ORM sessions
erp_last_successful_sync       # Timestamp of last success
```

**Job Types for ERP Integration**:
| Job Type | Description | Typical Duration |
|----------|-------------|------------------|
| `ERP_ARTICLE_SYNC` | Product data sync | 5-30 min |
| `ERP_PRICE_UPDATE` | Price list import | 2-10 min |
| `ERP_STOCK_SYNC` | Inventory levels | 1-5 min |
| `ERP_ORDER_EXPORT` | Order to ERP | <1 min |
| `ERP_CUSTOMER_SYNC` | Customer data | 5-15 min |

**Error Categories**:
- `ERP_CONNECTION_FAILED` - Cannot reach enventa
- `ERP_SESSION_TIMEOUT` - ORM session expired
- `ERP_DATA_VALIDATION` - Invalid data from/to ERP
- `ERP_BULK_PARTIAL` - Partial success in bulk operation

**Progress Reporting**:
For bulk operations (e.g., 10,000 articles):
- Report progress every 100 items or 30 seconds
- Include: processed count, success count, error count, ETA

---

### @QA Analysis
**Completed**: 2026-01-02

#### Testing Strategy

**Unit Tests**:
- Health check aggregation logic
- Job state transitions
- Log masking functions
- API response formatting

**Integration Tests**:
- SignalR connection and message flow
- Database persistence for job state
- Elasticsearch log indexing
- Alerting trigger conditions

**E2E Tests** (Playwright):
- Dashboard loads with real-time data
- Job filtering and sorting
- Job detail panel interactions
- Retry/cancel job actions
- Accessibility compliance scan

#### Failure Simulation Scenarios

| Scenario | Test Method | Expected Behavior |
|----------|-------------|-------------------|
| ERP connection lost | Mock network failure | Health degrades, alert triggered |
| Job timeout | Inject slow operation | Job marked failed, retry available |
| Database unavailable | Stop PostgreSQL | Graceful degradation, cached data shown |
| High load | 1000 concurrent jobs | UI responsive, pagination works |
| SignalR disconnect | Kill WebSocket | Auto-reconnect, poll fallback |

#### Test Data Requirements
- Seed data: 100+ historical jobs across all types
- Mock ERP responses for various scenarios
- Multi-tenant test data (3+ tenants)

#### Coverage Targets
- Unit tests: 80% coverage
- Integration tests: Key paths covered
- E2E tests: Critical user journeys
- Accessibility: axe-core automated + manual audit

---

**Next Steps**:
1. ✅ Agents complete their analysis sections
2. ✅ @SARAH consolidates into final requirements
3. ⏳ @Architect creates ADR-024 for architectural decisions
4. ⏳ Implementation planning begins

## SARAH Consolidation Summary

### Scope Confirmation
This requirements document covers a **comprehensive monitoring solution** with four delivery channels:
1. **Backend APIs** - Health checks, job status, service monitoring, error logging
2. **Admin Frontend** - Management dashboard, job status table, service panels, error viewers
3. **CLI Tool** - Command-line monitoring for DevOps and automation
4. **Real-time Updates** - SignalR hub for live status push

### Key Technical Decisions (For ADR-024)
| Decision | Choice | Rationale |
|----------|--------|-----------|
| Monitoring Architecture | Hybrid (centralized aggregation + distributed probes) | Balance of visibility and performance |
| Real-time Protocol | SignalR | Native .NET support, Vue.js client available |
| Job State Storage | PostgreSQL | Transactional integrity, existing infrastructure |
| Log Storage | Elasticsearch | Full-text search, time-series queries |
| Metrics | OpenTelemetry + Prometheus | Industry standard, Aspire integration |
| CLI Framework | Extend existing B2X.CLI | Reuse authentication, configuration |

### Implementation Phases (Recommended)
| Phase | Scope | Priority | Estimate |
|-------|-------|----------|----------|
| **Phase 1** | Core health checks, job status API, basic dashboard | P0 | 2 sprints |
| **Phase 2** | Connected services monitoring, error logging | P0 | 2 sprints |
| **Phase 3** | CLI monitoring commands | P1 | 1 sprint |
| **Phase 4** | Communication error details, pattern analysis | P1 | 1 sprint |
| **Phase 5** | Resource monitoring (CPU/memory) | P2 | 1 sprint |

### Risk Assessment
| Risk | Impact | Mitigation |
|------|--------|------------|
| Performance overhead | Medium | Sampling for high-volume logs, async logging |
| ERP integration complexity | High | @Enventa involvement, Actor pattern constraints |
| Data privacy in logs | High | PII masking, GDPR compliance review |
| SignalR scalability | Medium | Connection limits, Redis backplane for scale |

### Open Questions Resolved
| Question | Decision |
|----------|----------|
| Centralized vs. per-context monitoring? | Hybrid - centralized aggregation |
| Alerting channels? | Email + Slack via Alertmanager |
| Multi-tenant isolation? | Tenant filter in API + RBAC |

### Acceptance Criteria Updates
- [ ] Management dashboard loads in <3 seconds with current health status
- [ ] Job status table updates in real-time for active jobs
- [ ] Failed jobs trigger email alerts to configured recipients
- [ ] All UI components are keyboard accessible and screen reader compatible
- [ ] System handles 1000 concurrent jobs without performance degradation
- [ ] Logs are searchable and filterable by tenant, job type, and error type
- [ ] **CLI commands return within 2 seconds for all status queries**
- [ ] **Communication errors include full timing breakdown**
- [ ] **Resource alerts trigger within 30 seconds of threshold breach**

**Status**: Approved - ADR Created

---

*Agents: @ProductOwner, @Architect, @Backend, @Frontend, @UI, @DevOps, @Security, @Enventa, @QA | Owner: @ProductOwner | Consolidated: @SARAH*