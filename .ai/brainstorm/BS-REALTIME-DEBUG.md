---
docid: BS-REALTIME-DEBUG
title: Realtime Debug Functionality Brainstorm
owner: @SARAH
status: Brainstorm
created: 2026-01-10
---

# Brainstorm: Realtime Debug Functionality

**DocID**: `BS-REALTIME-DEBUG`  
**Created**: 10. Januar 2026  
**Status**: Brainstorm → Requirements → ADR Pipeline

---

## 🎯 Concept Overview

**Realtime Debug = Telemetry + User Feedback + DevTools Monitoring**

A unified debugging experience that correlates:
- Backend traces (OpenTelemetry/Aspire)
- Frontend user actions
- Browser performance metrics
- User-reported issues

---

## 🏗️ Architecture Components

### 1. **Telemetry Layer** (Backend)
| Component | Technology | Purpose |
|-----------|------------|---------|
| Distributed Tracing | OpenTelemetry + Aspire | Request flow across services |
| Metrics | Prometheus/OTLP | Performance KPIs |
| Logs | Structured logging | Contextual debugging |
| Correlation | TraceId/SpanId | Link frontend ↔ backend |

### 2. **User Feedback Layer** (Frontend)
| Component | Technology | Purpose |
|-----------|------------|---------|
| Feedback Widget | Vue component | User-triggered reports |
| Screenshot Capture | html2canvas | Visual context |
| Session Recording | rrweb or LogRocket-style | Replay user journey |
| Error Boundary | Vue errorHandler | Auto-capture crashes |

### 3. **DevTools Monitoring** (Browser)
| Component | Technology | Purpose |
|-----------|------------|---------|
| Network Monitor | Performance API | API latency, failures |
| Console Capture | Console override | JS errors, warnings |
| Performance Metrics | Web Vitals | LCP, FID, CLS |
| Resource Timing | Resource Timing API | Asset load times |

---

## 🔄 Data Flow Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        User Browser                              │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐  │
│  │ Vue App      │  │ DevTools     │  │ Feedback Widget      │  │
│  │ + Error      │  │ Monitor      │  │ + Screenshot         │  │
│  │   Boundary   │  │ (Perf API)   │  │ + Session Replay     │  │
│  └──────┬───────┘  └──────┬───────┘  └──────────┬───────────┘  │
│         │                 │                      │              │
│         └─────────────────┼──────────────────────┘              │
│                           ▼                                      │
│              ┌────────────────────────┐                         │
│              │ Debug Context Collector │                         │
│              │ (correlationId, userId) │                         │
│              └───────────┬────────────┘                         │
└──────────────────────────┼──────────────────────────────────────┘
                           │ WebSocket / HTTP
                           ▼
┌──────────────────────────────────────────────────────────────────┐
│                     Backend Services                              │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐   │
│  │ Debug API    │  │ OpenTelemetry│  │ Aspire Dashboard     │   │
│  │ Endpoint     │◄─┤ Collector    │  │ (existing)           │   │
│  └──────┬───────┘  └──────────────┘  └──────────────────────┘   │
│         │                                                        │
│         ▼                                                        │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │              Realtime Debug Dashboard                     │   │
│  │  • Correlated traces + user actions                      │   │
│  │  • Session replay with backend context                   │   │
│  │  • AI-assisted issue detection                           │   │
│  └──────────────────────────────────────────────────────────┘   │
└──────────────────────────────────────────────────────────────────┘
```

---

## 💡 Key Features

### P0 - Core Features
1. **Correlation Header Injection** - Pass `X-Correlation-Id` from frontend to all API calls
2. **Error Context Capture** - Stack trace + component tree + user actions
3. **Basic Feedback Widget** - "Report Issue" with screenshot

### P1 - Enhanced Debugging
4. **Session Recording** - Record user interactions (opt-in)
5. **Network Waterfall** - Show API calls with backend trace links
6. **Performance Dashboard** - Web Vitals + backend latency correlation

### P2 - AI-Assisted
7. **Anomaly Detection** - Flag unusual patterns automatically
8. **Root Cause Suggestions** - AI analysis of correlated data
9. **Predictive Alerts** - Warn before user-visible issues

---

## 🛠️ Technology Choices

### Frontend Debug Context
```typescript
// Debug context collector
interface DebugContext {
  correlationId: string;
  sessionId: string;
  userId?: string;
  timestamp: number;
  userAgent: string;
  viewport: { width: number; height: number };
  route: string;
  actions: UserAction[];  // Last N user actions
  errors: CapturedError[];
  performance: WebVitalsData;
  networkCalls: NetworkCall[];
}

interface UserAction {
  type: 'click' | 'input' | 'navigation' | 'scroll';
  target: string;  // CSS selector or component name
  timestamp: number;
  metadata?: Record<string, unknown>;
}

interface CapturedError {
  message: string;
  stack?: string;
  componentStack?: string;
  timestamp: number;
  correlationId: string;
}
```

### Backend Correlation Middleware
```csharp
// Debug correlation middleware
public class DebugCorrelationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DebugCorrelationMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-Id"]
            .FirstOrDefault() ?? Activity.Current?.TraceId.ToString();
        
        var sessionId = context.Request.Headers["X-Session-Id"].FirstOrDefault();
        
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["SessionId"] = sessionId,
            ["UserAgent"] = context.Request.Headers.UserAgent.ToString()
        });
        
        // Add to response for frontend correlation
        context.Response.Headers["X-Correlation-Id"] = correlationId;
        
        await _next(context);
    }
}
```

---

## 🔌 Integration with Existing Infrastructure

### Aspire Dashboard Extension
- Add custom tab for "User Sessions"
- Link traces to frontend context
- Display user feedback alongside traces

### MCP Integration (Chrome DevTools MCP)
- Capture browser metrics during development
- Automated performance profiling
- Visual regression correlation

### Wolverine Integration
- Correlate message handlers with user actions
- Saga debugging with user context
- Event replay with frontend state

---

## 📊 Dashboard Mockup Concept

```
┌─────────────────────────────────────────────────────────────────┐
│ Realtime Debug Dashboard                           [Live] 🟢    │
├─────────────────────────────────────────────────────────────────┤
│ Sessions (12 active)  │  Errors (3 new)  │  Performance (P95)   │
├───────────────────────┼──────────────────┼──────────────────────┤
│                       │                  │                      │
│ 🔴 Session #A42F      │  TypeError at    │  API Latency: 245ms  │
│    Error at checkout  │  ProductCard.vue │  LCP: 1.8s          │
│    [View Replay]      │  [View Trace]    │  CLS: 0.02          │
│                       │                  │                      │
│ 🟡 Session #B73C      │  Network Error   │  ─────────────────   │
│    Slow API response  │  /api/orders     │  Slowest Endpoints:  │
│    [View Timeline]    │  [View Backend]  │  • /api/search 890ms │
│                       │                  │  • /api/cart 340ms   │
└───────────────────────┴──────────────────┴──────────────────────┘
```

---

## 🚀 Implementation Phases

### Phase 1: Foundation (2 weeks)
- [ ] Correlation ID middleware (backend)
- [ ] Debug context provider (frontend)
- [ ] Basic error capture
- [ ] Simple feedback widget

### Phase 2: Enhanced Capture (2 weeks)
- [ ] Session recording (rrweb integration)
- [ ] Network monitoring overlay
- [ ] Performance metrics collection
- [ ] Screenshot capture

### Phase 3: Dashboard (3 weeks)
- [ ] Realtime debug dashboard UI
- [ ] Aspire dashboard integration
- [ ] Session replay viewer
- [ ] Trace correlation view

### Phase 4: AI Enhancement (2 weeks)
- [ ] Anomaly detection rules
- [ ] AI-assisted root cause analysis
- [ ] Predictive alerts
- [ ] Automated issue classification

---

## 👥 Agent Involvement

| Agent | Responsibility |
|-------|---------------|
| @Architect | Overall architecture, integration patterns |
| @Backend | Correlation middleware, telemetry pipeline |
| @Frontend | Debug context, feedback widget, session recording |
| @DevOps | Dashboard deployment, metrics infrastructure |
| @Security | PII handling in debug data, opt-in consent |
| @UX | Feedback widget design, dashboard UX |

---

## ❓ Open Questions

1. **Privacy**: How to handle PII in session recordings?
2. **Storage**: How long to retain debug data?
3. **Opt-in**: Developer-only or user-accessible feedback?
4. **Performance**: Impact of recording on app performance?
5. **Integration**: Extend Aspire dashboard or build custom?

---

## 📎 Related Documents

- [ADR-053] Realtime Debug Architecture (to be created)
- [REQ-REALTIME-DEBUG] Requirements Specification (to be created)
- [KB-064] Chrome DevTools MCP Server
- [KB-061] Monitoring MCP Usage Guide

---

**Next Steps**:
1. ✅ Brainstorm saved
2. ⏳ Analyze existing integration points
3. ⏳ Create requirements document
4. ⏳ Create ADR for architecture decision
