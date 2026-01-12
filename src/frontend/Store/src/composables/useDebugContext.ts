import { reactive, onMounted, onUnmounted } from 'vue';

// Local interfaces for function parameters
export interface ActionInput {
  type: string;
  target?: string;
  coordinates?: { x: number; y: number };
  url?: string;
  data?: unknown;
  timestamp: number;
}

export interface FeedbackInput {
  type: 'bug-report' | 'feature-request' | 'general-feedback';
  title: string;
  description: string;
  rating?: number;
  includeScreenshot?: boolean;
  url: string;
  userAgent: string;
}

interface ErrorContext {
  file?: string;
  line?: number;
  column?: number;
  [key: string]: unknown;
}

// Debug context state
interface DebugState {
  session: DebugSession | null;
  isEnabled: boolean;
  isRecording: boolean;
  actions: DebugAction[];
  errors: DebugError[];
  feedbacks: DebugFeedback[];
  maxActions: number;
  environment: 'development' | 'production' | 'staging';
}

const debugState = reactive<DebugState>({
  session: null,
  isEnabled: false,
  isRecording: false,
  actions: [],
  errors: [],
  feedbacks: [],
  maxActions: 50,
  environment: 'development',
});

// Action recording queue
let actionQueue: ActionInput[] = [];
let actionFlushTimer: number | null = null;
const ACTION_FLUSH_INTERVAL = 5000; // 5 seconds

// API endpoints
const API_BASE = '/api/debug';

// Detect environment
function detectEnvironment(): 'development' | 'production' | 'staging' {
  if (typeof window === 'undefined') return 'development';

  const hostname = window.location.hostname;
  if (hostname.includes('localhost') || hostname.includes('127.0.0.1')) {
    return 'development';
  }
  if (hostname.includes('staging') || hostname.includes('dev')) {
    return 'staging';
  }
  return 'production';
}

// Generate correlation ID
function generateCorrelationId(): string {
  const tenantId = 'demo'; // In real app, get from auth context
  const sessionId = Math.random().toString(36).substring(2, 15);
  const requestId = Date.now().toString(36);
  return `${tenantId}-${sessionId}-${requestId}`;
}

// Get current viewport
function getViewport() {
  if (typeof window === 'undefined') return undefined;
  return {
    width: window.innerWidth,
    height: window.innerHeight,
  };
}

// Start debug session
async function startSession(): Promise<string | null> {
  try {
    const sessionData = {
      userAgent: navigator.userAgent,
      viewport: getViewport(),
      environment: debugState.environment,
      metadata: {
        url: window.location.href,
        referrer: document.referrer,
        timestamp: new Date().toISOString(),
      },
    };

    const response = await fetch(`${API_BASE}/session/start`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'X-Correlation-Id': generateCorrelationId(),
      },
      body: JSON.stringify(sessionData),
    });

    if (!response.ok) {
      console.warn('Failed to start debug session:', response.statusText);
      return null;
    }

    const result = await response.json();
    const sessionId = result as string;

    debugState.session = {
      id: sessionId,
      startTime: new Date(),
      environment: debugState.environment,
      userAgent: navigator.userAgent,
      viewport: getViewport() || { width: 0, height: 0 },
      correlationId: `corr_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
    };

    debugState.isRecording = true;
    console.log('Debug session started:', sessionId);

    return sessionId;
  } catch (error) {
    console.warn('Failed to start debug session:', error);
    return null;
  }
}

// Record user action
function recordAction(action: ActionInput): void {
  if (!debugState.isRecording || !debugState.session) return;

  // Convert composable action to store format
  const storeAction: DebugAction = {
    id: `action_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
    sessionId: debugState.session.id,
    timestamp: new Date(action.timestamp),
    type:
      action.type === 'click' ? 'click' : action.type === 'navigation' ? 'navigation' : 'custom',
    element: action.target,
    url: action.url,
    data: action.data,
    metadata: {
      coordinates: action.coordinates,
    },
  };

  // Add to local state
  debugState.actions.push(storeAction);

  // Keep only recent actions
  if (debugState.actions.length > debugState.maxActions) {
    debugState.actions = debugState.actions.slice(-debugState.maxActions);
  }

  // Add to queue for batch sending
  actionQueue.push(action);

  // Schedule flush if not already scheduled
  if (!actionFlushTimer) {
    actionFlushTimer = window.setTimeout(flushActions, ACTION_FLUSH_INTERVAL);
  }
}

// Flush queued actions to server
async function flushActions(): Promise<void> {
  if (actionQueue.length === 0 || !debugState.session) return;

  const actionsToSend = [...actionQueue];
  actionQueue = [];
  actionFlushTimer = null;

  try {
    const promises = actionsToSend.map(action =>
      fetch(`${API_BASE}/action/capture`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'X-Correlation-Id': generateCorrelationId(),
        },
        body: JSON.stringify({
          sessionId: debugState.session!.id,
          actionType: action.type,
          description: `${action.type} on ${action.target}`,
          targetSelector: action.target,
          url: action.url || window.location.href,
          coordinates: action.coordinates,
          formData: action.data,
          metadata: {
            timestamp: action.timestamp,
            ...(action.data && typeof action.data === 'object' ? action.data : {}),
          },
        }),
      })
    );

    await Promise.allSettled(promises);
  } catch (error) {
    console.warn('Failed to flush debug actions:', error);
    // Re-queue failed actions
    actionQueue.unshift(...actionsToSend);
  }
}

// Capture error
async function captureError(error: Error, context?: ErrorContext): Promise<void> {
  if (!debugState.isRecording || !debugState.session) return;

  const debugError: DebugError = {
    id: `error_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
    sessionId: debugState.session.id,
    timestamp: new Date(),
    type: 'javascript',
    message: error.message,
    stack: error.stack,
    url: window.location.href,
    userAgent: navigator.userAgent,
    metadata: context,
  };

  // Add to local state
  debugState.errors.push(debugError);

  try {
    await fetch(`${API_BASE}/error/capture`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'X-Correlation-Id': generateCorrelationId(),
      },
      body: JSON.stringify({
        sessionId: debugState.session.id,
        level: 'error',
        message: debugError.message,
        stackTrace: debugError.stack,
        sourceFile: context?.file,
        lineNumber: context?.line,
        columnNumber: context?.column,
        url: debugError.url,
        userAgent: navigator.userAgent,
        viewport: getViewport(),
        context: context,
      }),
    });
  } catch (err) {
    console.warn('Failed to capture debug error:', err);
  }
}

// Submit feedback
async function submitFeedback(feedback: FeedbackInput): Promise<boolean> {
  if (!debugState.session) return false;

  try {
    // Take screenshot if requested
    let screenshot: string | undefined;
    if (feedback.includeScreenshot) {
      try {
        screenshot = await takeScreenshot();
      } catch (error) {
        console.warn('Failed to take screenshot:', error);
      }
    }

    const response = await fetch(`${API_BASE}/feedback/submit`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'X-Correlation-Id': debugState.session.correlationId,
      },
      body: JSON.stringify({
        sessionId: debugState.session.id,
        type: feedback.type,
        title: feedback.title,
        description: feedback.description,
        rating: feedback.rating,
        url: feedback.url || window.location.href,
        userAgent: navigator.userAgent,
        viewport: getViewport(),
        screenshot: screenshot,
        metadata: {
          ...feedback,
        },
      }),
    });

    return response.ok;
  } catch (error) {
    console.warn('Failed to submit feedback:', error);
    return false;
  }
}

// Take screenshot (simplified - would need html2canvas in real implementation)
async function takeScreenshot(): Promise<string> {
  // Placeholder - in real implementation, use html2canvas or similar
  return new Promise(resolve => {
    setTimeout(
      () =>
        resolve(
          'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg=='
        ),
      100
    );
  });
}

// Enable/disable debug mode
function setDebugEnabled(enabled: boolean): void {
  debugState.isEnabled = enabled;
  if (enabled) {
    console.log('Debug mode enabled');
  } else {
    console.log('Debug mode disabled');
    stopSession();
  }
}

// Stop debug session
function stopSession(): void {
  debugState.isRecording = false;
  debugState.session = null;
  debugState.actions = [];
  debugState.errors = [];

  if (actionFlushTimer) {
    clearTimeout(actionFlushTimer);
    actionFlushTimer = null;
  }

  // Flush any remaining actions
  flushActions();
}

// Auto-record common events
function setupAutoRecording(): void {
  if (typeof window === 'undefined') return;

  // Click events
  document.addEventListener(
    'click',
    event => {
      const target = event.target as HTMLElement;
      if (target) {
        recordAction({
          type: 'click',
          target:
            target.tagName +
            (target.id ? `#${target.id}` : '') +
            (target.className ? `.${target.className.split(' ')[0]}` : ''),
          timestamp: Date.now(),
          coordinates: { x: event.clientX, y: event.clientY },
          url: window.location.href,
        });
      }
    },
    true
  );

  // Navigation events
  let currentUrl = window.location.href;
  const observer = new MutationObserver(() => {
    if (window.location.href !== currentUrl) {
      recordAction({
        type: 'navigation',
        target: 'url-change',
        timestamp: Date.now(),
        data: { from: currentUrl, to: window.location.href },
        url: window.location.href,
      });
      currentUrl = window.location.href;
    }
  });
  observer.observe(document, { childList: true, subtree: true });

  // Global error handler
  window.addEventListener('error', event => {
    captureError(new Error(event.message), {
      filename: event.filename,
      lineno: event.lineno,
      colno: event.colno,
    });
  });

  // Unhandled promise rejections
  window.addEventListener('unhandledrejection', event => {
    captureError(new Error(`Unhandled promise rejection: ${event.reason}`), {
      reason: event.reason,
    });
  });
}

// Vue composable
function setupSignalREventListeners() {
  if (typeof window === 'undefined') return;

  // Listen for SignalR events from the debug API
  window.addEventListener('debug:sessionStarted', (event: Event) => {
    const customEvent = event as CustomEvent;
    console.log('[DebugContext] Remote session started:', customEvent.detail);
    // Update local state if needed
  });

  window.addEventListener('debug:errorCaptured', (event: Event) => {
    const customEvent = event as CustomEvent;
    console.log('[DebugContext] Remote error captured:', customEvent.detail);
    // Could show notification or update UI
  });

  window.addEventListener('debug:feedbackSubmitted', (event: Event) => {
    const customEvent = event as CustomEvent;
    console.log('[DebugContext] Remote feedback submitted:', customEvent.detail);
    // Could show notification
  });

  window.addEventListener('debug:actionRecorded', (event: Event) => {
    const customEvent = event as CustomEvent;
    console.log('[DebugContext] Remote action recorded:', customEvent.detail);
    // Could update action count or show activity
  });

  window.addEventListener('debug:performanceAlert', (event: Event) => {
    const customEvent = event as CustomEvent;
    console.warn('[DebugContext] Performance alert:', customEvent.detail);
    // Could show performance warning
  });
}

export function useDebugContext() {
  // Initialize environment detection
  onMounted(() => {
    debugState.environment = detectEnvironment();

    // Check if debug mode is enabled via localStorage or URL param
    const urlParams = new URLSearchParams(window.location.search);
    const debugParam = urlParams.get('debug');
    const debugStorage = localStorage.getItem('debug-enabled');

    if (debugParam === 'true' || debugStorage === 'true') {
      setDebugEnabled(true);
      setupAutoRecording();
    }

    // Set up SignalR event listeners
    setupSignalREventListeners();
  });

  onUnmounted(() => {
    stopSession();
  });

  return {
    // State
    session: debugState.session,
    isEnabled: debugState.isEnabled,
    isRecording: debugState.isRecording,
    actions: debugState.actions,
    errors: debugState.errors,
    feedbacks: debugState.feedbacks,
    environment: debugState.environment,

    // Methods
    startSession,
    recordAction,
    captureError,
    submitFeedback,
    setDebugEnabled,
    stopSession,
  };
}
