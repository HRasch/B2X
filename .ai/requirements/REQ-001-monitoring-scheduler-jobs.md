# REQ-001: Monitoring for Scheduler Jobs

**DocID**: `REQ-001`  
**Status**: Approved | **Owner**: @ProductOwner  
**Full Details**: `.ai/archive/requirements-full/REQ-001-monitoring-scheduler-jobs.md`

## Summary
Health monitoring, error logging, and status visualization for ERP/PIM/CRM scheduler jobs.

## Key User Stories

### Management
- High-level health dashboard
- Critical failure alerts
- Performance trend charts

### Operations
- Real-time job status
- Detailed error logs
- Connection test capabilities
- CLI-based monitoring

### Admin
- Tenant-filtered job views
- Service connectivity status
- Resource usage monitoring

## Core Requirements

| Area | Requirement |
|------|-------------|
| Health Checks | Every 30s for critical components |
| Service Status | ERP, PIM, CRM connectivity display |
| Job Progress | Real-time progress bars, status indicators |
| Error Logging | Structured logs with request/response details |
| CLI Support | JSON/table output for automation |
| Alerts | Configurable thresholds, email/Slack notifications |

## Technical Approach
- Wolverine handlers for health checks
- OpenTelemetry for metrics
- Vue.js admin dashboard components
- PostgreSQL for job status persistence

## Acceptance Criteria
- [ ] Dashboard shows all service health status
- [ ] Job progress visible in real-time
- [ ] Errors logged with full context
- [ ] CLI commands functional
- [ ] Alerts trigger on threshold breach

## References
- [ADR-024 Scheduler Monitoring](.ai/decisions/ADR-024-scheduler-job-monitoring.md)
- [enventa ERP KB](.ai/knowledgebase/enventa-trade-erp.md)
