---
docid: ADR-054
title: Realtime Debug Strategy for Development and Production
owner: @Architect
status: Proposed
created: 2026-01-10
decision-makers: "@Architect, @TechLead, @DevOps, @Security"
---

# ADR-054: Realtime Debug Strategy for Development and Production

**Status**: Proposed
**Date**: 10. Januar 2026
**Decision Makers**: @Architect, @TechLead, @DevOps, @Security

---

## Context

The B2X platform requires a unified realtime debugging strategy that works across both development and production environments. Current capabilities are limited to Aspire Dashboard in development, but production environments (Kubernetes) lack equivalent debugging tools.

Key requirements:
- **Development**: Extend existing Aspire-based debugging with correlation-first architecture
- **Production**: Provide debugging capabilities without compromising security or performance
- **Unified Experience**: Consistent debugging workflow across environments
- **Security**: Tenant-isolated, no data leakage, GDPR compliant
- **Performance**: Minimal overhead in production

---

## Decision

We will implement a **Hybrid Debug Architecture** that provides:

1. **Development Environment**: Enhanced Aspire-based debugging with correlation tracking
2. **Production Environment**: Secure, performance-optimized debugging via Kubernetes-native tools
3. **Unified Frontend**: Environment-agnostic debug dashboard
4. **Progressive Enhancement**: Core debugging works without full infrastructure

### Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                    Unified Debug Frontend                        │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │ Debug Dashboard (Vue 3) - Environment Agnostic             │ │
│  │ • Auto-detects environment (dev/prod)                      │ │
│  │ • Adapts UI based on available capabilities                │ │
│  │ • Progressive enhancement for limited environments         │ │
│  └────────────────────────────────────────────────────────────┘ │
└─────────────────────────┬───────────────────────────────────────┘
                          │ WebSocket/HTTP
                          ▼
┌─────────────────────────────────────────────────────────────────┐
│                 Environment-Specific Backend                    │
│                                                                 │
│  ┌─────────────────────┐  ┌──────────────────────────────────┐ │
│  │ Development         │  │ Production                       │ │
│  │ (Aspire + SignalR)  │  │ (K8s + Sidecar)                 │ │
│  │                     │  │                                  │ │
│  │ • Full correlation  │  │ • Secure correlation            │ │
│  │ • Realtime streams  │  │ • Filtered streams               │ │
│  │ • Rich context      │  │ • Minimal context               │ │
│  │ • Debug storage     │  │ • Ephemeral only                │ │
│  └─────────────────────┘  └──────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

---

## Development Environment Strategy

### Enhanced Aspire-Based Debugging

**Architecture**:
```
Frontend (Vue 3) ←→ Correlation Middleware ←→ Debug Controller
    ↓                        ↓                        ↓
Debug Context      SignalR Debug Hub      PostgreSQL Debug Schema
Provider           (Realtime Streams)     (sessions, errors, feedback)
    ↓                        ↓                        ↓
Action Recorder    OpenTelemetry         Aspire Dashboard
(Stores user       Custom Exporter       (Trace Correlation)
actions)           (Debug Events)
```

**Components**:

1. **Correlation Middleware** (Backend)
   - Extracts/Generates `X-Correlation-Id` header
   - Format: `{tenantId}-{sessionId}-{requestId}`
   - Enriches OpenTelemetry Activity with correlation data
   - Adds correlation to LogContext for structured logging

2. **Debug Controller** (Backend)
   ```csharp
   [ApiController]
   [Route("api/debug")]
   public class DebugController : ControllerBase
   {
       [HttpPost("errors")] public async Task<IActionResult> CaptureError([FromBody] DebugError error)
       [HttpPost("feedback")] public async Task<IActionResult> SubmitFeedback([FromBody] DebugFeedback feedback)
       [HttpGet("sessions")] public async Task<IActionResult> GetActiveSessions()
       [HttpGet("traces/{correlationId}")] public async Task<IActionResult> GetTraceDetails(string correlationId)
   }
   ```

3. **SignalR Debug Hub** (Backend)
   ```csharp
   public class DebugHub : Hub<IDebugHub>
   {
       public async Task OnErrorCaptured(DebugError error)
       public async Task OnSessionStarted(DebugSession session)
       public async Task OnPerformanceAlert(PerformanceAlert alert)
       public async Task OnFeedbackSubmitted(DebugFeedback feedback)
   }
   ```

4. **Debug Context Provider** (Frontend)
   ```typescript
   // src/composables/useDebugContext.ts
   export const useDebugContext = () => {
     const correlationId = ref<string>()
     const sessionId = ref<string>()

     const startSession = () => { /* Generate IDs, send to backend */ }
     const recordAction = (action: UserAction) => { /* Store action sequence */ }
     const captureError = (error: Error, context: ErrorContext) => { /* Send to backend */ }

     return { correlationId, sessionId, startSession, recordAction, captureError }
   }
   ```

5. **Database Schema** (PostgreSQL)
   ```sql
   CREATE SCHEMA debug;

   CREATE TABLE debug.sessions (
       id UUID PRIMARY KEY,
       tenant_id UUID NOT NULL,
       correlation_id VARCHAR(100) UNIQUE NOT NULL,
       user_id UUID,
       user_agent TEXT,
       viewport JSONB,
       environment VARCHAR(20) DEFAULT 'development',
       started_at TIMESTAMPTZ DEFAULT NOW(),
       last_activity_at TIMESTAMPTZ DEFAULT NOW()
   );

   CREATE TABLE debug.errors (
       id UUID PRIMARY KEY,
       session_id UUID REFERENCES debug.sessions(id),
       correlation_id VARCHAR(100) NOT NULL,
       severity VARCHAR(20) NOT NULL,
       message TEXT NOT NULL,
       stack_trace TEXT,
       component_stack TEXT,
       user_actions JSONB,
       trace_id VARCHAR(32),
       created_at TIMESTAMPTZ DEFAULT NOW()
   );
   ```

**Debugging Workflow (Development)**:
1. User starts session → Correlation ID generated
2. Frontend sends `X-Correlation-Id` with all requests
3. Backend middleware enriches telemetry with correlation
4. Errors captured → Stored in debug schema + streamed via SignalR
5. Debug dashboard shows correlated traces in Aspire
6. User actions recorded for context

---

## Production Environment Strategy

### Kubernetes-Native Secure Debugging

**Architecture**:
```
Frontend ←→ API Gateway ←→ Service Mesh (Istio)
    ↓           ↓                    ↓
Debug Context  Rate Limited        Sidecar Proxy
Provider       Debug Endpoints     (Envoy Filters)
    ↓           ↓                    ↓
Action Buffer  Debug Service       Fluent Bit
(IndexedDB)    (Filtered Data)      (Log Correlation)
    ↓           ↓                    ↓
Secure Upload  PostgreSQL          Elasticsearch
(to Debug DB)  (Ephemeral)          (Filtered Logs)
```

**Components**:

1. **Sidecar Proxy Configuration** (Kubernetes)
   ```yaml
   # istio-sidecar-injection: enabled
   apiVersion: networking.istio.io/v1beta1
   kind: EnvoyFilter
   metadata:
     name: debug-correlation-filter
   spec:
     configPatches:
     - applyTo: HTTP_FILTER
       match:
         context: SIDECAR_INBOUND
         listener:
           filterChain:
             filter:
               name: envoy.filters.network.http_connection_manager
       patch:
         operation: INSERT_BEFORE
         value:
           name: envoy.filters.http.lua
           typed_config:
             "@type": type.googleapis.com/envoy.extensions.filters.http.lua.v3.Lua
             inlineCode: |
               function envoy_on_request(request_handle)
                 local correlation_id = request_handle:headers():get("x-correlation-id")
                 if correlation_id then
                   request_handle:headers():add("x-debug-session", correlation_id)
                 end
               end
   ```

2. **Debug Service** (Kubernetes Deployment)
   ```yaml
   apiVersion: apps/v1
   kind: Deployment
   metadata:
     name: debug-service
   spec:
     replicas: 1
     template:
       spec:
         containers:
         - name: debug-api
           image: b2x/debug-service:latest
           ports:
           - containerPort: 8080
           env:
           - name: DEBUG_MODE
             value: "production"
           - name: RATE_LIMIT_PER_MINUTE
             value: "10"
           resources:
             limits:
               cpu: 100m
               memory: 128Mi
   ```

3. **Fluent Bit Configuration** (Log Correlation)
   ```yaml
   apiVersion: v1
   kind: ConfigMap
   metadata:
     name: fluent-bit-config
   data:
     fluent-bit.conf: |
       [INPUT]
           Name              tail
           Path              /var/log/containers/*debug*.log
           Parser            docker
           Tag               debug.*

       [FILTER]
           Name              grep
           Match             debug.*
           Regex             correlation_id [^"]*

       [OUTPUT]
           Name              elasticsearch
           Match             debug.*
           Host              elasticsearch.production.svc.cluster.local
           Index             debug-logs
           Type              debug
   ```

4. **Rate Limiting** (Production)
   ```csharp
   // Production DebugController with rate limiting
   [ApiController]
   [Route("api/debug")]
   [EnableRateLimiting("DebugEndpoints")]
   public class ProductionDebugController : ControllerBase
   {
       [HttpPost("errors")]
       [RequestSizeLimit(1024)] // 1KB limit
       public async Task<IActionResult> CaptureError([FromBody] FilteredDebugError error)

       [HttpPost("feedback")]
       [RequestSizeLimit(2048)] // 2KB limit
       public async Task<IActionResult> SubmitFeedback([FromBody] FilteredDebugFeedback feedback)
   }
   ```

5. **Frontend Production Mode**
   ```typescript
   // src/composables/useDebugContext.ts
   export const useDebugContext = () => {
     const isProduction = import.meta.env.PROD
     const debugEnabled = computed(() =>
       isProduction ? localStorage.getItem('debug-enabled') === 'true' : true
     )

     // Production: Buffer actions locally, upload on error
     const actionBuffer = ref<UserAction[]>([])
     const uploadActions = async () => {
       if (actionBuffer.value.length > 0) {
         await apiClient.post('/api/debug/actions', {
           actions: actionBuffer.value.slice(-10) // Last 10 only
         })
         actionBuffer.value = []
       }
     }

     return { debugEnabled, actionBuffer, uploadActions }
   }
   ```

**Debugging Workflow (Production)**:
1. User enables debug mode (opt-in only)
2. Correlation ID generated client-side
3. Actions buffered in IndexedDB
4. On error → Upload last 10 actions + error context
5. Rate-limited API prevents abuse
6. Sidecar proxy adds correlation to logs
7. Fluent Bit correlates logs with debug data
8. Debug dashboard shows filtered, correlated data

---

## Unified Frontend Strategy

### Environment-Agnostic Debug Dashboard

**Progressive Enhancement**:
```vue
<template>
  <div class="debug-dashboard">
    <!-- Core Features (Always Available) -->
    <DebugSessionManager />
    <ErrorCapture v-if="debugEnabled" />

    <!-- Development Features -->
    <RealtimeStreams v-if="environment === 'development'" />
    <AspireTraceViewer v-if="environment === 'development'" />

    <!-- Production Features -->
    <SecureFeedback v-if="environment === 'production'" />
    <RateLimitedActions v-if="environment === 'production'" />

    <!-- Limited Mode (Network Issues) -->
    <OfflineDebug v-if="!connected" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'

const environment = ref<'development' | 'production'>('development')
const connected = ref(true)
const debugEnabled = ref(false)

const detectEnvironment = async () => {
  try {
    // Try development endpoints first
    await fetch('/aspire/api/v1/resources')
    environment.value = 'development'
  } catch {
    // Fallback to production detection
    const response = await fetch('/api/debug/status')
    if (response.ok) {
      environment.value = 'production'
      const data = await response.json()
      debugEnabled.value = data.enabled
    }
  }
}

onMounted(detectEnvironment)
</script>
```

**Feature Matrix**:

| Feature | Development | Production | Limited Mode |
|---------|-------------|------------|--------------|
| Error Capture | ✅ Full | ✅ Filtered | ✅ Local Only |
| User Actions | ✅ All | ✅ Last 10 | ❌ |
| Realtime Streams | ✅ SignalR | ❌ | ❌ |
| Trace Correlation | ✅ Aspire | ✅ Sidecar | ❌ |
| Feedback Widget | ✅ Rich | ✅ Basic | ✅ |
| Session Recording | ✅ Full | ✅ Opt-in | ❌ |
| Performance Metrics | ✅ Live | ✅ Aggregated | ❌ |

---

## Security & Privacy Strategy

### Data Handling by Environment

| Data Type | Development | Production |
|-----------|-------------|------------|
| User Actions | Full capture | Last 10 actions only |
| Screenshots | Full capture | Opt-in, blurred PII |
| Error Stacks | Full capture | Filtered, no sensitive data |
| Session Metadata | Full capture | Minimal (user agent, viewport) |
| Correlation IDs | Persistent | Ephemeral (24h TTL) |

### Access Control

**Development**:
- Debug dashboard: Any authenticated user
- Debug API: Tenant-isolated
- SignalR: Same tenant only

**Production**:
- Debug mode: Opt-in via localStorage
- Debug API: Rate limited (10 req/min)
- Data retention: 24 hours max
- Admin access: Separate role required

### GDPR Compliance

**Data Minimization**:
- Production: Only capture necessary context
- User consent: Required for session recording
- Right to erasure: Cascade delete on user deletion
- Data portability: Export debug data on request

**Security Measures**:
- TLS 1.3 required for all debug endpoints
- API key authentication for debug service
- Input sanitization on all debug data
- Regular security audits of debug components

---

## Performance Strategy

### Development Performance Budget

| Component | Overhead | Mitigation |
|-----------|----------|------------|
| Correlation middleware | <1ms | Lightweight header processing |
| Error capture | <5ms | Async processing |
| Action recording | <2ms | Debounced, batched |
| SignalR connection | ~50KB | One connection per dashboard |
| Database writes | <10ms | Background processing |

**Total Overhead**: <2% of request latency

### Production Performance Budget

| Component | Overhead | Mitigation |
|-----------|----------|------------|
| Sidecar proxy | <0.5ms | Optimized Lua filter |
| Rate limiting | <1ms | In-memory counters |
| Debug service | <5ms | Minimal processing |
| Log correlation | <2ms | Async processing |
| Data filtering | <1ms | Pre-computed filters |

**Total Overhead**: <0.1% of request latency (when debug disabled)

---

## Implementation Roadmap

### Phase 1: Development Foundation (2 weeks)
- [ ] Implement correlation middleware
- [ ] Create debug database schema
- [ ] Build basic debug controller
- [ ] Add frontend correlation injection
- [ ] Enhanced error capture service

### Phase 2: Development Enhancement (2 weeks)
- [ ] SignalR debug hub
- [ ] User action recorder
- [ ] Feedback widget with screenshots
- [ ] Aspire trace correlation
- [ ] Debug dashboard UI

### Phase 3: Production Foundation (3 weeks)
- [ ] Kubernetes debug service deployment
- [ ] Sidecar proxy configuration
- [ ] Rate limiting implementation
- [ ] Production debug controller
- [ ] Fluent Bit log correlation

### Phase 4: Unified Frontend (2 weeks)
- [ ] Environment detection
- [ ] Progressive enhancement
- [ ] Production mode optimizations
- [ ] Offline debug capabilities
- [ ] Admin debug controls

### Phase 5: Security & Compliance (1 week)
- [ ] Security audit
- [ ] GDPR compliance review
- [ ] Performance optimization
- [ ] Documentation updates

### Phase 6: Staging Validation (2 weeks)
- [ ] Deploy debug components to staging environment
- [ ] User acceptance testing (UAT) with real user scenarios
- [ ] Load testing with debug features enabled
- [ ] Security penetration testing of debug endpoints
- [ ] Performance validation against production-like traffic
- [ ] Cross-browser compatibility testing
- [ ] Mobile device testing for debug features
- [ ] Integration testing with existing monitoring systems
- [ ] Data retention and cleanup validation
- [ ] User consent flow testing
- [ ] Admin dashboard validation
- [ ] Rollback procedure testing
- [ ] Documentation review and user training materials

---

## Staging Validation Process

### Overview
Before production deployment, all debug features must undergo comprehensive validation in a staging environment that mirrors production as closely as possible. This ensures debug functionality works correctly under real-world conditions while maintaining security and performance standards.

### Staging Environment Requirements

**Infrastructure Setup**:
- Kubernetes cluster matching production specifications
- Same network policies and security configurations
- Production-like data volumes and user traffic simulation
- Monitoring and logging stack identical to production
- Database with realistic tenant data (anonymized)

**Debug Feature Configuration**:
- All debug components deployed and enabled
- Rate limiting set to production levels
- Security policies matching production
- Data retention policies active
- GDPR compliance features enabled

### User Acceptance Testing (UAT)

**Test User Scenarios**:
1. **Error Reproduction**: Users intentionally trigger errors to validate capture
2. **Session Recording**: Users perform typical workflows with debug enabled
3. **Feedback Submission**: Users submit feedback with screenshots
4. **Performance Impact**: Users test application responsiveness with debug active
5. **Cross-Device Testing**: Validation on various devices and browsers

**UAT Test Cases**:
```gherkin
Feature: Debug Feature Validation

Scenario: Error capture and correlation
  Given user is in staging environment with debug enabled
  When an error occurs in the application
  Then error is captured with full context
  And correlation ID links to user session
  And error appears in debug dashboard within 5 seconds

Scenario: User consent and privacy
  Given user has not consented to debug tracking
  When debug features are available
  Then no user data is captured
  And clear consent dialog is presented
  And user can opt-out at any time

Scenario: Performance impact validation
  Given debug features are enabled
  When user performs normal operations
  Then application response time <500ms
  And no visible performance degradation
  And debug overhead <0.1% of total requests
```

### Load Testing

**Performance Validation**:
- Simulate production traffic patterns (1000+ concurrent users)
- Test debug features under load
- Validate rate limiting effectiveness
- Monitor resource usage (CPU, memory, network)
- Test database performance with debug data writes

**Load Test Scenarios**:
```bash
# Simulate production load with debug enabled
k6 run --vus 1000 --duration 30m load-test-debug.js

# Test rate limiting
ab -n 10000 -c 100 https://staging.b2x.com/api/debug/errors

# Validate correlation under load
./scripts/test-correlation-load.sh
```

### Security Testing

**Penetration Testing**:
- Attempt to bypass rate limiting
- Test for data leakage through debug endpoints
- Validate tenant isolation
- Check for injection vulnerabilities
- Test authentication bypass scenarios

**Security Test Checklist**:
- [ ] Debug endpoints reject unauthenticated requests
- [ ] Rate limiting prevents abuse (429 responses)
- [ ] Tenant data isolation maintained
- [ ] No sensitive data in debug logs
- [ ] Input sanitization prevents XSS
- [ ] HTTPS required for all debug traffic
- [ ] API keys properly secured

### Integration Testing

**System Integration**:
- Debug data flows to monitoring systems
- Alerts trigger correctly
- Log correlation works with existing infrastructure
- Database cleanup processes function
- Backup and recovery procedures validated

**Third-Party Integration**:
- External monitoring tools receive debug data
- SIEM systems capture security events
- Compliance reporting includes debug metrics
- Audit logs contain debug activities

### Validation Criteria

**Go/No-Go Decision Matrix**:

| Category | Criteria | Threshold | Status |
|----------|----------|-----------|--------|
| Functionality | All debug features work as designed | 100% | ⏳ |
| Performance | <0.1% overhead under load | <0.1% | ⏳ |
| Security | Zero critical vulnerabilities | 0 | ⏳ |
| Privacy | GDPR compliance validated | 100% | ⏳ |
| User Experience | UAT pass rate | >95% | ⏳ |
| Reliability | Error capture success rate | >99% | ⏳ |

**Sign-Off Requirements**:
- ✅ Development team technical review
- ✅ QA team functionality validation
- ✅ Security team penetration test approval
- ✅ Performance team load test approval
- ✅ Product team UAT approval
- ✅ Legal/Compliance GDPR review
- ✅ DevOps deployment readiness

### Rollback Validation

**Rollback Testing**:
- Feature flags can disable debug features instantly
- Debug service can be scaled to zero
- Database cleanup removes all debug data
- Frontend gracefully degrades without debug features
- No production impact from debug system failures

### Documentation & Training

**User Documentation**:
- How to enable/disable debug mode
- Privacy implications and consent
- What data is captured and why
- How to submit feedback
- Troubleshooting common issues

**Admin Training**:
- Debug dashboard usage
- Monitoring debug system health
- Responding to debug alerts
- User data handling procedures
- Compliance reporting

### Post-Staging Activities

**Production Readiness Review**:
- Final security assessment
- Performance baseline establishment
- Monitoring dashboard configuration
- Incident response procedures
- Support team training completion

**Deployment Checklist**:
- [ ] All validation criteria met
- [ ] Sign-offs obtained from all teams
- [ ] Documentation published
- [ ] Training completed
- [ ] Rollback procedures documented
- [ ] Monitoring alerts configured
- [ ] Production deployment plan approved

## Monitoring & Alerting

### Development Alerts
- Debug database disk usage >80%
- SignalR connection failures
- Correlation ID collision rate >1%
- Error capture failure rate >5%

### Production Alerts
- Debug service rate limit hits
- Sidecar proxy errors
- Debug data queue backlog
- GDPR compliance violations

### Health Checks
```csharp
// Debug service health check
public class DebugServiceHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        // Check database connectivity
        // Check rate limit status
        // Check sidecar proxy health
        return HealthCheckResult.Healthy();
    }
}
```

---

## Rollback Strategy

### Development Rollback
- Feature flags for debug components
- Database schema migrations reversible
- SignalR hub can be disabled
- Frontend debug can be toggled off

### Production Rollback
- Debug service can be scaled to 0
- Sidecar proxy filters removable
- Rate limits adjustable to 0
- Feature flags in frontend

---

## Success Metrics

### Development Metrics
- Mean time to diagnosis: <5 minutes (target: <2 minutes)
- Debug session success rate: >95%
- Correlation accuracy: >99%
- User action capture rate: >98%

### Production Metrics
- Debug overhead: <0.1% latency increase
- Security incidents: 0
- GDPR compliance: 100%
- User opt-in rate: >10% (target)

### Staging Validation Metrics
- UAT pass rate: >95%
- Load test performance: <0.1% overhead under 1000 concurrent users
- Security test findings: 0 critical vulnerabilities
- Integration test success: 100%
- Rollback success rate: 100%
- Documentation completeness: 100%

---

## Consequences

### Positive
- ✅ Unified debugging experience across environments
- ✅ Faster issue resolution in both dev and prod
- ✅ Better user experience with proactive error handling
- ✅ Maintains security and performance in production
- ✅ Builds on existing OpenTelemetry investment

### Negative
- ⚠️ Additional infrastructure complexity
- ⚠️ Ongoing maintenance of debug components
- ⚠️ Storage costs for debug data (development)
- ⚠️ Learning curve for new debugging tools

### Neutral
- ↔️ Requires developer training on new workflows
- ↔️ May surface previously unknown issues (good for quality)
- ↔️ Additional monitoring and alerting complexity

---

## Review & Approval

| Role | Reviewer | Decision | Date |
|------|----------|----------|------|
| Architect | @Architect | ⏳ Pending | - |
| Tech Lead | @TechLead | ⏳ Pending | - |
| DevOps | @DevOps | ⏳ Pending | - |
| Security | @Security | ⏳ Pending | - |

---

## References

- [ADR-053] Realtime Debug Architecture (Development-focused)
- [ADR-003] Aspire Orchestration
- [KB-055] Security MCP Best Practices
- [KB-061] Monitoring MCP Usage Guide
- [KB-064] Chrome DevTools MCP Server
- [GL-008] Governance Policies (Security requirements)
- [REQ-008] Realtime Debug Requirements</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\.ai\decisions\ADR-054-realtime-debug-strategy.md