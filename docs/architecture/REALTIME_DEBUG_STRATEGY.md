# Realtime Debugging Strategy - Quick Reference

**DocID**: `DEBUG-STRATEGY-REF`
**Last Updated**: January 10, 2026
**Status**: Active

## Overview

B2X implements a **hybrid realtime debugging strategy** that provides consistent debugging capabilities across development and production environments while maintaining security and performance.

## Environment Detection

The debug system automatically detects the environment:

```typescript
// Frontend automatically detects environment
const environment = await detectEnvironment()
// Returns: 'development' | 'production' | 'limited'
```

## Development Environment (Aspire)

### Features Available
- ✅ Full correlation tracking
- ✅ Realtime SignalR streams
- ✅ Rich error context
- ✅ User action recording
- ✅ Aspire Dashboard integration
- ✅ Debug data persistence

### Quick Start
```bash
# Start Aspire with debugging
cd src/backend/Infrastructure/Hosting/AppHost
dotnet run

# Open debug dashboard
# http://localhost:15500/debug (when implemented)
```

### Correlation Flow
```
Frontend Request → X-Correlation-Id Header → Backend Middleware
                     ↓
              OpenTelemetry Activity Enrichment
                     ↓
              SignalR Debug Hub (Realtime)
                     ↓
              PostgreSQL Debug Schema
                     ↓
              Aspire Dashboard Correlation
```

## Production Environment (Kubernetes)

### Features Available
- ✅ Secure correlation tracking
- ✅ Rate-limited error capture
- ✅ Filtered user actions (last 10)
- ✅ Opt-in debug mode
- ✅ Sidecar proxy correlation
- ✅ Ephemeral data storage

### Security Controls
- Rate limiting: 10 requests/minute
- Data retention: 24 hours max
- Opt-in only: User consent required
- Input limits: 1KB errors, 2KB feedback

### Quick Start
```typescript
// Enable debug mode (user opt-in)
localStorage.setItem('debug-enabled', 'true')

// System detects production environment
// Automatically applies security restrictions
```

## Unified Frontend API

### Debug Context Provider
```typescript
import { useDebugContext } from '@/composables/useDebugContext'

const { correlationId, startSession, recordAction, captureError } = useDebugContext()

// Start debug session
await startSession()

// Record user actions
recordAction({
  type: 'click',
  target: 'product-add-to-cart',
  timestamp: Date.now()
})

// Capture errors
captureError(error, {
  component: 'ProductCard',
  userActions: lastActions,
  browserInfo: navigator.userAgent
})
```

### Error Capture
```typescript
// Automatic error capture (Vue plugin)
app.config.errorHandler = (error, instance, info) => {
  captureError(error, {
    component: instance?.$?.type?.name,
    lifecycleHook: info,
    route: router.currentRoute.value.path
  })
}
```

## Backend Integration

### Development Controller
```csharp
[ApiController]
[Route("api/debug")]
public class DebugController : ControllerBase
{
    [HttpPost("errors")]
    public async Task<IActionResult> CaptureError([FromBody] DebugError error)

    [HttpPost("feedback")]
    public async Task<IActionResult> SubmitFeedback([FromBody] DebugFeedback feedback)

    [HttpGet("sessions")]
    public async Task<IActionResult> GetActiveSessions()
}
```

### Production Controller
```csharp
[ApiController]
[Route("api/debug")]
[EnableRateLimiting("DebugEndpoints")]
public class ProductionDebugController : ControllerBase
{
    [HttpPost("errors")]
    [RequestSizeLimit(1024)] // 1KB limit
    public async Task<IActionResult> CaptureError([FromBody] FilteredDebugError error)
}
```

## Correlation ID Format

```
Format: {tenantId}-{sessionId}-{requestId}
Example: acme-a1b2c3d4-550e8400-e29b-41d4-a716-446655440000

Headers:
- X-Correlation-Id: Primary correlation identifier
- X-Debug-Session: Additional session context
```

## Database Schema

### Development Schema
```sql
CREATE SCHEMA debug;

CREATE TABLE debug.sessions (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL,
    correlation_id VARCHAR(100) UNIQUE NOT NULL,
    user_id UUID,
    user_agent TEXT,
    viewport JSONB,
    environment VARCHAR(20) DEFAULT 'development'
);

CREATE TABLE debug.errors (
    id UUID PRIMARY KEY,
    session_id UUID REFERENCES debug.sessions(id),
    correlation_id VARCHAR(100) NOT NULL,
    severity VARCHAR(20) NOT NULL,
    message TEXT NOT NULL,
    stack_trace TEXT,
    user_actions JSONB,
    trace_id VARCHAR(32)
);
```

### Production Schema
- Same structure but with shorter retention
- Automatic cleanup after 24 hours
- Filtered data only (no full stack traces)

## SignalR Integration (Development Only)

### Hub Interface
```csharp
public interface IDebugHub
{
    Task OnErrorCaptured(DebugError error);
    Task OnSessionStarted(DebugSession session);
    Task OnPerformanceAlert(PerformanceAlert alert);
    Task OnFeedbackSubmitted(DebugFeedback feedback);
}
```

### Frontend Connection
```typescript
import { HubConnectionBuilder } from '@microsoft/signalr'

const connection = new HubConnectionBuilder()
    .withUrl('/debugHub')
    .withAutomaticReconnect()
    .build()

connection.on('OnErrorCaptured', (error) => {
    console.log('Realtime error:', error)
})
```

## Kubernetes Configuration (Production)

### Sidecar Proxy
```yaml
apiVersion: networking.istio.io/v1beta1
kind: EnvoyFilter
metadata:
  name: debug-correlation-filter
spec:
  configPatches:
  - applyTo: HTTP_FILTER
    patch:
      operation: INSERT_BEFORE
      value:
        name: envoy.filters.http.lua
        # Lua script for correlation injection
```

### Debug Service Deployment
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: debug-service
spec:
  template:
    spec:
      containers:
      - name: debug-api
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

## Performance Budget

### Development
- Correlation middleware: <1ms overhead
- Error capture: <5ms
- SignalR connection: ~50KB persistent

**Total**: <2% request latency increase

### Production
- Sidecar proxy: <0.5ms overhead
- Rate limiting: <1ms
- Debug service: <5ms (when enabled)

**Total**: <0.1% request latency increase

## Security Checklist

### Development
- [ ] Tenant-isolated data access
- [ ] Authenticated debug dashboard access
- [ ] Input sanitization on all debug data
- [ ] Regular cleanup of debug data

### Production
- [ ] Rate limiting enabled (10 req/min)
- [ ] Opt-in debug mode only
- [ ] Data retention ≤24 hours
- [ ] TLS 1.3 required
- [ ] GDPR compliance (data minimization)

## Troubleshooting

### Development Issues
```bash
# Check Aspire dashboard
curl http://localhost:15500

# Verify SignalR connection
# Check browser network tab for WebSocket

# Debug correlation headers
curl -H "X-Correlation-Id: test-123" http://localhost:8000/api/products
```

### Production Issues
```bash
# Check debug service health
kubectl get pods -l app=debug-service

# Verify sidecar injection
kubectl get pods -o jsonpath='{.spec.containers[*].name}'

# Check rate limit metrics
kubectl logs -l app=debug-service | grep "rate-limit"
```

## Implementation Status

### ✅ Completed
- ADR-054: Realtime Debug Strategy
- Environment detection logic
- Basic correlation middleware
- Security framework

### 🚧 In Progress
- Debug dashboard UI
- SignalR hub implementation
- Production sidecar configuration

### 📋 Planned
- User feedback widget
- Performance monitoring integration
- Automated testing

## Staging Validation Process

### Overview
All debug features must be validated in staging before production deployment. This ensures functionality, security, and performance under production-like conditions.

### Staging Requirements
- **Infrastructure**: Kubernetes cluster matching production
- **Data**: Realistic tenant data (anonymized)
- **Traffic**: Production-like load simulation
- **Security**: Same policies as production

### Validation Phases

#### 1. User Acceptance Testing (UAT)
```gherkin
Scenario: Error capture validation
  Given user enables debug mode in staging
  When application errors occur
  Then errors are captured with correlation
  And appear in debug dashboard within 5 seconds
```

#### 2. Load Testing
```bash
# Test debug under production load
k6 run --vus 1000 --duration 30m debug-load-test.js

# Validate rate limiting
ab -n 10000 -c 100 https://staging.b2x.com/api/debug/errors
```

#### 3. Security Testing
- Penetration testing of debug endpoints
- Rate limiting validation
- Tenant isolation verification
- GDPR compliance audit

#### 4. Integration Testing
- Monitoring system integration
- Alert configuration validation
- Log correlation testing
- Backup/recovery procedures

### Go/No-Go Criteria
- ✅ Functionality: 100% feature completeness
- ✅ Performance: <0.1% overhead under load
- ✅ Security: Zero critical vulnerabilities
- ✅ Privacy: 100% GDPR compliance
- ✅ UAT: >95% pass rate

### Sign-Off Requirements
- [ ] Development team approval
- [ ] QA team validation
- [ ] Security team clearance
- [ ] Performance team approval
- [ ] Product team UAT sign-off
- [ ] Legal compliance review

### Rollback Validation
- Feature flags can disable debug instantly
- Debug service scales to zero
- Database cleanup removes debug data
- Frontend degrades gracefully

## Quick Commands

### Development
```bash
# Start full debugging environment
npm run dev  # Frontend
dotnet run   # Aspire AppHost

# Open dashboards
start http://localhost:15500  # Aspire
start http://localhost:3000/debug  # Debug UI
```

### Production
```bash
# Enable debug mode
# In browser console:
localStorage.setItem('debug-enabled', 'true')

# Check debug status
curl https://api.b2x.com/api/debug/status
```

## Support

- **Development Issues**: Check Aspire Dashboard logs
- **Production Issues**: Check Kubernetes pod logs
- **Security Concerns**: Contact @Security team
- **Performance Issues**: Monitor via debug dashboard

---

**See Also**:
- [ADR-054] Full Realtime Debug Strategy
- [ADR-053] Development Debug Architecture
- [KB-061] Monitoring MCP Usage Guide
- [KB-055] Security MCP Best Practices</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\docs\architecture\REALTIME_DEBUG_STRATEGY.md