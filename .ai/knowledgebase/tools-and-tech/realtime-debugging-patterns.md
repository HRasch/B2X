---
docid: KB-069
title: Realtime Debugging Patterns
owner: @DocMaintainer
status: Active
created: 2026-01-10
---

# Realtime Debugging Patterns in Vue/Nuxt Applications

**DocID**: `KB-069`  
**Last Updated**: 10. Januar 2026  
**Owner**: @DocMaintainer

## Overview

This article documents patterns and best practices for implementing comprehensive realtime debugging systems in Vue 3/Nuxt 3 applications with .NET backend integration. Based on the Phase 2 realtime debug strategy implementation for B2X platform.

## Core Architecture

### Frontend Components

**Composition API Composable** (`useDebugContext.ts`):
```typescript
// Centralized debug context with SignalR integration
export function useDebugContext() {
  const session = ref<DebugSession | null>(null)
  const isRecording = computed(() => session.value?.isActive ?? false)
  
  // SignalR event listeners
  const setupSignalREventListeners = () => {
    connection.on('SessionStarted', handleSessionStarted)
    connection.on('ActionRecorded', handleActionRecorded)
    connection.on('ErrorCaptured', handleErrorCaptured)
  }
  
  return {
    session: readonly(session),
    isRecording,
    startSession,
    stopSession,
    recordAction,
    recordError,
    submitFeedback
  }
}
```

**Pinia Store** (`debug.ts`):
```typescript
// Reactive state management for debug data
export const useDebugStore = defineStore('debug', () => {
  const session = ref<DebugSession | null>(null)
  const actions = ref<DebugAction[]>([])
  const errors = ref<DebugError[]>([])
  const feedbacks = ref<DebugFeedback[]>([])
  
  const startSession = async () => {
    session.value = await debugApi.createSession()
    actions.value = []
    errors.value = []
  }
  
  return {
    session: readonly(session),
    actions: readonly(actions),
    errors: readonly(errors),
    feedbacks: readonly(feedbacks),
    startSession,
    stopSession,
    recordAction,
    recordError,
    submitFeedback
  }
})
```

### Backend SignalR Infrastructure

**SignalR Hub** (`DebugHub.cs`):
```csharp
public class DebugHub : Hub
{
    public async Task JoinSessionGroup(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
    }
    
    public async Task LeaveSessionGroup(string sessionId)
    {
        await Groups.RemoveToGroupAsync(Context.ConnectionId, sessionId);
    }
}
```

**Event Broadcaster** (`DebugEventBroadcaster.cs`):
```csharp
public class DebugEventBroadcaster : IDebugEventBroadcaster
{
    private readonly IHubContext<DebugHub> _hubContext;
    
    public async Task BroadcastSessionStartedAsync(DebugSession session)
    {
        await _hubContext.Clients
            .Group(session.Id)
            .SendAsync("SessionStarted", session);
    }
    
    public async Task BroadcastActionRecordedAsync(DebugAction action)
    {
        await _hubContext.Clients
            .Group(action.SessionId)
            .SendAsync("ActionRecorded", action);
    }
}
```

## Nuxt 3 Integration Patterns

### SSR-Safe Component Integration

**ClientOnly Wrapper Pattern**:
```vue
<template>
  <div class="app-container">
    <!-- SSR-safe content -->
    <SomeComponent />
    
    <!-- Client-only debug components -->
    <ClientOnly>
      <DebugTrigger v-if="isDebugEnabled" />
      <DebugFeedbackWidget v-model="showFeedback" />
    </ClientOnly>
  </div>
</template>
```

**Plugin Registration**:
```typescript
// nuxt.config.ts
export default defineNuxtConfig({
  plugins: [
    '~/plugins/debug-init.js',    // Debug mode initialization
    '~/plugins/debug-guard.js'    // Route protection
  ]
})
```

### SignalR Connection Management

**Connection Lifecycle**:
```typescript
class DebugApiService {
  private connection: HubConnection | null = null
  
  async initializeSignalR(): Promise<void> {
    this.connection = new HubConnectionBuilder()
      .withUrl('/debug-hub', {
        accessTokenFactory: () => getAuthToken(),
        headers: { 'X-Tenant-Id': getTenantId() }
      })
      .withAutomaticReconnect()
      .build()
      
    await this.connection.start()
    this.setupSignalREventHandlers()
  }
  
  private setupSignalREventHandlers(): void {
    this.connection!.on('SessionStarted', this.handleSessionStarted.bind(this))
    this.connection!.on('ActionRecorded', this.handleActionRecorded.bind(this))
    this.connection!.on('ErrorCaptured', this.handleErrorCaptured.bind(this))
  }
}
```

## Privacy & Security Controls

### Data Sanitization

**Automatic Data Masking**:
```typescript
const sanitizeData = (data: any): any => {
  const sensitiveKeys = ['password', 'token', 'secret', 'key']
  
  if (typeof data === 'object' && data !== null) {
    const sanitized = { ...data }
    sensitiveKeys.forEach(key => {
      if (key in sanitized) {
        sanitized[key] = '[REDACTED]'
      }
    })
    return sanitized
  }
  
  return data
}
```

**URL Pattern Exclusion**:
```typescript
const shouldRecordAction = (url: string): boolean => {
  const excludePatterns = [
    /\/api\/auth/,
    /password/i,
    /\/admin\/sensitive/
  ]
  
  return !excludePatterns.some(pattern => pattern.test(url))
}
```

## Performance Optimization

### Event Batching

**Batch Action Recording**:
```typescript
class DebugActionBatcher {
  private actions: DebugAction[] = []
  private batchTimeout: NodeJS.Timeout | null = null
  
  recordAction(action: DebugAction): void {
    this.actions.push(action)
    
    if (this.actions.length >= 10) {
      this.flushBatch()
    } else if (!this.batchTimeout) {
      this.batchTimeout = setTimeout(() => this.flushBatch(), 5000)
    }
  }
  
  private async flushBatch(): Promise<void> {
    if (this.actions.length === 0) return
    
    const batch = [...this.actions]
    this.actions = []
    
    if (this.batchTimeout) {
      clearTimeout(this.batchTimeout)
      this.batchTimeout = null
    }
    
    await debugApi.recordActionsBatch(batch)
  }
}
```

### Memory Management

**Session Limits**:
```typescript
const DEBUG_CONFIG = {
  limits: {
    maxActionsPerSession: 1000,
    maxErrorsPerSession: 100,
    maxFeedbacksPerSession: 50,
    sessionTimeoutMinutes: 60
  }
}
```

## Error Handling Patterns

### Graceful Degradation

**SignalR Fallback**:
```typescript
const recordActionWithFallback = async (action: DebugAction): Promise<void> => {
  try {
    // Try SignalR first
    if (signalRConnected.value) {
      await signalRBroadcast(action)
    } else {
      // Fallback to HTTP API
      await httpApi.recordAction(action)
    }
  } catch (error) {
    // Silent failure - don't break user experience
    console.warn('Debug recording failed:', error)
  }
}
```

### Connection Recovery

**Automatic Reconnection**:
```typescript
const setupConnectionMonitoring = () => {
  connection.onreconnecting(() => {
    debugStore.setConnectionStatus('reconnecting')
  })
  
  connection.onreconnected(() => {
    debugStore.setConnectionStatus('connected')
    // Resync missed events
    resyncDebugState()
  })
  
  connection.onclose(() => {
    debugStore.setConnectionStatus('disconnected')
    // Attempt reconnection after delay
    setTimeout(() => initializeSignalR(), 5000)
  })
}
```

## Testing Strategies

### Component Integration Testing

**Mock SignalR for Testing**:
```typescript
// vitest.setup.ts
import { vi } from 'vitest'

vi.mock('@microsoft/signalr', () => ({
  HubConnectionBuilder: vi.fn(() => ({
    withUrl: vi.fn().mockReturnThis(),
    withAutomaticReconnect: vi.fn().mockReturnThis(),
    build: vi.fn(() => ({
      start: vi.fn().mockResolvedValue(undefined),
      stop: vi.fn().mockResolvedValue(undefined),
      on: vi.fn(),
      off: vi.fn(),
      invoke: vi.fn().mockResolvedValue(undefined)
    }))
  }))
}))
```

### End-to-End Validation

**Debug System Health Check**:
```typescript
const validateDebugSystem = async (): Promise<boolean> => {
  try {
    // Check component presence
    const debugTrigger = document.querySelector('[data-testid="debug-trigger"]')
    if (!debugTrigger) return false
    
    // Check SignalR connection
    const connection = await debugApi.getConnectionStatus()
    if (connection.status !== 'connected') return false
    
    // Check session creation
    const session = await debugApi.createSession()
    if (!session.id) return false
    
    return true
  } catch (error) {
    console.error('Debug system validation failed:', error)
    return false
  }
}
```

## Production Deployment Considerations

### Feature Flags

**Environment-Based Configuration**:
```typescript
const DEBUG_CONFIG = {
  development: {
    enableSignalR: true,
    enableUserActionRecording: true,
    enableErrorCapture: true,
    enableFeedbackWidget: true,
    maxActionsPerSession: 1000
  },
  production: {
    enableSignalR: true,
    enableUserActionRecording: false, // Privacy concerns
    enableErrorCapture: true,
    enableFeedbackWidget: true,
    maxActionsPerSession: 100,
    requireConsent: true
  }
}
```

### Monitoring & Analytics

**Debug System Metrics**:
```typescript
const trackDebugMetrics = () => {
  // Session success rate
  analytics.track('debug_session_created', {
    success: true,
    duration: sessionDuration
  })
  
  // Error capture rate
  analytics.track('debug_error_captured', {
    count: errorsCaptured,
    userImpact: errorSeverity
  })
  
  // User engagement
  analytics.track('debug_feedback_submitted', {
    rating: feedbackRating,
    hasScreenshot: !!screenshot
  })
}
```

## Best Practices

### 1. SSR Compatibility
- Always wrap client-side debug components in `<ClientOnly>`
- Initialize SignalR connections after client-side hydration
- Avoid DOM manipulation in server-rendered code

### 2. Privacy First
- Implement automatic data sanitization
- Provide clear user consent mechanisms
- Respect Do Not Track preferences
- Exclude sensitive routes from recording

### 3. Performance Conscious
- Use event batching for high-frequency actions
- Implement session limits and cleanup
- Lazy-load debug components
- Monitor memory usage

### 4. Error Resilient
- Graceful degradation when services fail
- Automatic reconnection for SignalR
- Silent failure to avoid breaking user experience
- Comprehensive error logging for debugging

### 5. Testable Architecture
- Dependency injection for services
- Mock SignalR connections in tests
- Component integration validation
- End-to-end health checks

## Common Pitfalls

### SignalR Connection Issues
- **Problem**: Connection fails in SSR environment
- **Solution**: Initialize connections in `onMounted()` or client-only plugins

### Memory Leaks
- **Problem**: Unlimited action/error accumulation
- **Solution**: Implement session limits and periodic cleanup

### Privacy Violations
- **Problem**: Sensitive data captured in debug sessions
- **Solution**: Automatic sanitization and configurable exclusions

### Performance Impact
- **Problem**: Debug overhead affects user experience
- **Solution**: Feature flags, batching, and lazy loading

## Migration Path

### From Basic to Advanced Debugging

1. **Phase 1**: Basic error logging and user feedback
2. **Phase 2**: Action recording and realtime streaming (current implementation)
3. **Phase 3**: Performance monitoring and analytics integration
4. **Phase 4**: AI-powered issue analysis and recommendations

## Related Articles

- [KB-007: Vue.js 3](vue.md) - Vue 3 fundamentals
- [KB-008: Pinia State](pinia.md) - State management patterns
- [KB-054: Vue MCP Integration](tools-and-tech/vue-mcp-integration.md) - Vue tooling
- [KB-055: Security MCP Best Practices](tools-and-tech/security-mcp-best-practices.md) - Security patterns

## Implementation Checklist

- [ ] SSR-safe component integration with ClientOnly
- [ ] SignalR connection management with automatic reconnect
- [ ] Privacy controls and data sanitization
- [ ] Performance optimization with batching and limits
- [ ] Comprehensive error handling and graceful degradation
- [ ] Feature flags for different environments
- [ ] Integration testing and health checks
- [ ] Documentation and user consent mechanisms

---

**Status**: ✅ Complete realtime debug implementation documented  
**Next Review**: March 2026  
**Contributors**: Phase 2 Debug Implementation Team</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\.ai\knowledgebase\tools-and-tech\realtime-debugging-patterns.md