---
docid: REQ-008
title: Realtime Debug Functionality
owner: @ProductOwner
status: Draft
created: 2026-01-10
priority: P1
---

# REQ-008: Realtime Debug Functionality

**DocID**: `REQ-008`  
**Created**: 10. Januar 2026  
**Status**: Draft  
**Priority**: P1 - High Business Value

---

## Executive Summary

Implement a unified realtime debugging experience that correlates backend telemetry, frontend user actions, and browser performance metrics. This enables faster issue resolution, proactive problem detection, and improved developer productivity.

---

## Business Justification

| Factor | Description |
|--------|-------------|
| **Pain Point** | Debugging production issues requires manual correlation of logs, traces, and user reports |
| **Impact** | 2-4 hours average time to diagnose complex issues |
| **Target** | Reduce diagnosis time to <30 minutes with correlated data |
| **ROI** | Developer productivity gain + faster customer issue resolution |

---

## User Stories

### US-001: Developer Issue Diagnosis
**As a** developer  
**I want** to see correlated frontend actions with backend traces  
**So that** I can quickly identify the root cause of issues

**Acceptance Criteria:**
- [ ] AC-001.1: Click "View Trace" on any frontend error to see backend trace
- [ ] AC-001.2: See user actions (clicks, navigation) leading up to error
- [ ] AC-001.3: View request/response payload for failed API calls
- [ ] AC-001.4: Link to Aspire dashboard for detailed trace view

### US-002: User Feedback Collection
**As a** end user  
**I want** to report issues with context automatically captured  
**So that** support can help me faster

**Acceptance Criteria:**
- [ ] AC-002.1: "Report Issue" button available on all pages
- [ ] AC-002.2: Screenshot captured automatically
- [ ] AC-002.3: Last 30 seconds of user actions recorded
- [ ] AC-002.4: Browser/device info included
- [ ] AC-002.5: Correlation ID links to backend traces

### US-003: Realtime Error Monitoring
**As a** support engineer  
**I want** to see errors as they happen in production  
**So that** I can proactively assist users

**Acceptance Criteria:**
- [ ] AC-003.1: Live error feed with <5 second latency
- [ ] AC-003.2: Filter by tenant, error type, severity
- [ ] AC-003.3: Group similar errors automatically
- [ ] AC-003.4: Alert on error rate spikes

### US-004: Performance Correlation
**As a** performance engineer  
**I want** to correlate frontend Web Vitals with backend latency  
**So that** I can identify performance bottlenecks

**Acceptance Criteria:**
- [ ] AC-004.1: Display LCP, FID, CLS alongside backend response times
- [ ] AC-004.2: Show API waterfall with timing breakdown
- [ ] AC-004.3: Highlight slow endpoints (>500ms)
- [ ] AC-004.4: Compare performance across tenants

---

## Functional Requirements

### FR-001: Correlation ID Propagation
| ID | Requirement | Priority |
|----|-------------|----------|
| FR-001.1 | Frontend must inject `X-Correlation-Id` header on all API calls | P0 |
| FR-001.2 | Backend must propagate correlation ID through all service calls | P0 |
| FR-001.3 | Correlation ID must be included in all log entries | P0 |
| FR-001.4 | Response must include correlation ID for client-side tracking | P0 |

### FR-002: Frontend Error Capture
| ID | Requirement | Priority |
|----|-------------|----------|
| FR-002.1 | Capture Vue component errors with component stack | P0 |
| FR-002.2 | Capture unhandled promise rejections | P0 |
| FR-002.3 | Capture network errors with request details | P0 |
| FR-002.4 | Record user actions (click, input, navigation) | P1 |
| FR-002.5 | Capture screenshots on error (opt-in) | P1 |

### FR-003: Backend Debug Endpoint
| ID | Requirement | Priority |
|----|-------------|----------|
| FR-003.1 | POST `/api/debug/errors` - receive frontend errors | P0 |
| FR-003.2 | POST `/api/debug/feedback` - receive user feedback with screenshot | P1 |
| FR-003.3 | GET `/api/debug/traces/{correlationId}` - fetch trace data | P1 |
| FR-003.4 | WebSocket `/debug/live` - stream realtime events | P2 |

### FR-004: Debug Dashboard
| ID | Requirement | Priority |
|----|-------------|----------|
| FR-004.1 | Display active sessions with error status | P1 |
| FR-004.2 | Show error timeline with grouping | P1 |
| FR-004.3 | Session replay viewer (user actions) | P2 |
| FR-004.4 | Performance metrics visualization | P2 |

---

## Non-Functional Requirements

### NFR-001: Performance
| ID | Requirement | Target |
|----|-------------|--------|
| NFR-001.1 | Debug context overhead | <5ms per request |
| NFR-001.2 | Error capture to display latency | <5 seconds |
| NFR-001.3 | Session recording memory usage | <10MB per session |
| NFR-001.4 | Dashboard load time | <2 seconds |

### NFR-002: Privacy & Security
| ID | Requirement | Target |
|----|-------------|--------|
| NFR-002.1 | PII masking in recordings | Automatic for form inputs |
| NFR-002.2 | Debug data retention | 7 days default, configurable |
| NFR-002.3 | Access control | Admin/Developer roles only |
| NFR-002.4 | Data encryption | TLS in transit, encrypted at rest |

### NFR-003: Scalability
| ID | Requirement | Target |
|----|-------------|--------|
| NFR-003.1 | Concurrent sessions | 10,000+ |
| NFR-003.2 | Error ingestion rate | 1000 errors/second |
| NFR-003.3 | Storage per tenant | 1GB default allocation |

---

## Technical Constraints

### Existing Infrastructure (Must Integrate With)
Based on codebase analysis:

| Component | Location | Integration Point |
|-----------|----------|-------------------|
| OpenTelemetry Setup | `B2X.Tracing.Extensions` | Add custom debug exporter |
| Activity Source | `B2X.Tenancy.ActivitySource` | Extend for debug spans |
| Error Logging Service | `errorLoggingService.ts` | Enhance with correlation |
| API Client | `resilientApiClient.ts` | Add correlation header |
| Aspire Dashboard | `B2X.AppHost` | Link traces |

### Gaps to Address
| Gap | Solution |
|-----|----------|
| No correlation ID in frontend API calls | Update `resilientApiClient.ts` |
| No backend error receiver | Create debug controller |
| No WebSocket infrastructure | Add SignalR hub |
| No debug dashboard UI | Create Management frontend component |

---

## Implementation Phases

### Phase 1: Foundation (Sprint 1-2)
**Scope**: Correlation ID + Basic Error Capture

| Task | Owner | Estimate |
|------|-------|----------|
| Correlation ID middleware | @Backend | 2 days |
| Update frontend API client | @Frontend | 1 day |
| Debug error endpoint | @Backend | 2 days |
| Enhanced error capture | @Frontend | 2 days |
| Unit tests | @QA | 2 days |

**Exit Criteria**: 
- Frontend errors visible in backend logs with correlation ID
- Can trace request from frontend to backend

### Phase 2: Enhanced Capture (Sprint 3-4)
**Scope**: User Actions + Feedback Widget

| Task | Owner | Estimate |
|------|-------|----------|
| User action recorder | @Frontend | 3 days |
| Feedback widget UI | @Frontend + @UX | 3 days |
| Screenshot capture | @Frontend | 2 days |
| Feedback storage endpoint | @Backend | 2 days |
| PII masking implementation | @Security | 2 days |

**Exit Criteria**:
- Users can submit feedback with screenshot
- User actions recorded leading up to errors

### Phase 3: Dashboard (Sprint 5-7)
**Scope**: Debug Dashboard + Realtime Feed

| Task | Owner | Estimate |
|------|-------|----------|
| SignalR hub setup | @Backend | 2 days |
| Debug dashboard UI | @Frontend | 5 days |
| Session list component | @Frontend | 2 days |
| Error timeline component | @Frontend | 3 days |
| Aspire trace linking | @Backend | 2 days |
| Integration tests | @QA | 3 days |

**Exit Criteria**:
- Live error feed in Management dashboard
- Can click through to Aspire traces

### Phase 4: AI Enhancement (Sprint 8-9)
**Scope**: Anomaly Detection + Root Cause Analysis

| Task | Owner | Estimate |
|------|-------|----------|
| Anomaly detection rules | @Backend | 3 days |
| AI root cause suggestions | @Backend | 5 days |
| Alert configuration | @DevOps | 2 days |
| AI integration (optional) | @Architect | 3 days |

**Exit Criteria**:
- Automatic anomaly detection
- AI-suggested root causes

---

## Dependencies

| Dependency | Type | Status |
|------------|------|--------|
| OpenTelemetry infrastructure | Existing | ✅ Available |
| Aspire Dashboard | Existing | ✅ Available |
| SignalR | New | ⏳ To be added |
| Storage for debug data | New | ⏳ PostgreSQL table |
| html2canvas library | New | ⏳ npm package |
| rrweb (optional) | New | ⏳ npm package |

---

## Risks & Mitigations

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Performance overhead | High | Medium | Sampling, feature flags |
| PII exposure | High | Medium | Automatic masking, opt-in |
| Storage costs | Medium | Medium | Retention policies, compression |
| Browser compatibility | Low | Low | Feature detection, graceful fallback |

---

## Success Metrics

| Metric | Current | Target | Measurement |
|--------|---------|--------|-------------|
| Mean time to diagnosis | ~2 hours | <30 min | Support ticket resolution time |
| Error correlation rate | 0% | >95% | Errors with backend trace |
| Developer satisfaction | N/A | >4/5 | Survey |
| User feedback submissions | N/A | +50% | Feedback widget usage |

---

## Approval

| Role | Name | Status | Date |
|------|------|--------|------|
| Product Owner | @ProductOwner | ⏳ Pending | - |
| Tech Lead | @TechLead | ⏳ Pending | - |
| Architect | @Architect | ⏳ Pending | - |
| Security | @Security | ⏳ Pending | - |

---

## Related Documents

- [BS-REALTIME-DEBUG] Brainstorm Document
- [ADR-053] Realtime Debug Architecture (to be created)
- [KB-064] Chrome DevTools MCP Server
- [KB-061] Monitoring MCP Usage Guide
