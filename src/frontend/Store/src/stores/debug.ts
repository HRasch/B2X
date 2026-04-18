/**
 * Debug Store
 *
 * Centralized state management for debug features using Pinia.
 * Provides reactive state for debug sessions, actions, errors, and settings.
 */

import { defineStore } from 'pinia';
import { ref, computed, reactive } from 'vue';

export interface DebugSession {
  id: string;
  startTime: Date;
  endTime?: Date;
  environment: string;
  userAgent: string;
  viewport: {
    width: number;
    height: number;
  };
  tenantId?: string;
  userId?: string;
  correlationId: string;
}

export interface DebugAction {
  id: string;
  sessionId: string;
  timestamp: Date;
  type: 'click' | 'navigation' | 'form-submit' | 'api-call' | 'custom';
  element?: string;
  selector?: string;
  url?: string;
  data?: unknown;
  metadata?: Record<string, unknown>;
}

export interface DebugError {
  id: string;
  sessionId: string;
  timestamp: Date;
  type: 'javascript' | 'network' | 'console' | 'unhandled';
  message: string;
  stack?: string;
  url?: string;
  line?: number;
  column?: number;
  userAgent: string;
  metadata?: Record<string, unknown>;
}

export interface DebugFeedback {
  id: string;
  sessionId: string;
  timestamp: Date;
  type: 'bug-report' | 'feature-request' | 'general-feedback';
  title: string;
  description: string;
  rating?: number;
  includeScreenshot: boolean;
  screenshot?: string;
  userAgent: string;
  url: string;
  metadata?: Record<string, unknown>;
}

export interface DebugSettings {
  autoRecordClicks: boolean;
  autoRecordErrors: boolean;
  autoRecordNavigation: boolean;
  autoRecordApiCalls: boolean;
  maxActionsPerSession: number;
  maxErrorsPerSession: number;
  enableSignalR: boolean;
  enableScreenshots: boolean;
  enableConsoleLogging: boolean;
}

export const useDebugStore = defineStore('debug', () => {
  // State
  const isEnabled = ref(false);
  const isInitialized = ref(false);
  const session = ref<DebugSession | null>(null);
  const actions = ref<DebugAction[]>([]);
  const errors = ref<DebugError[]>([]);
  const feedbacks = ref<DebugFeedback[]>([]);
  const settings = reactive<DebugSettings>({
    autoRecordClicks: true,
    autoRecordErrors: true,
    autoRecordNavigation: true,
    autoRecordApiCalls: false,
    maxActionsPerSession: 1000,
    maxErrorsPerSession: 100,
    enableSignalR: true,
    enableScreenshots: true,
    enableConsoleLogging: true,
  });

  // Getters
  const isRecording = computed(() => session.value !== null);
  const sessionDuration = computed(() => {
    if (!session.value) return 0;
    const endTime = session.value.endTime || new Date();
    return endTime.getTime() - session.value.startTime.getTime();
  });

  const actionsCount = computed(() => actions.value.length);
  const errorsCount = computed(() => errors.value.length);
  const feedbacksCount = computed(() => feedbacks.value.length);

  const recentActions = computed(() => actions.value.slice(-10).reverse());

  const recentErrors = computed(() => errors.value.slice(-5).reverse());

  // Actions
  function initialize() {
    if (isInitialized.value) return;

    // Load settings from localStorage
    loadSettings();

    // Set up event listeners
    setupEventListeners();

    // Mark as initialized
    isInitialized.value = true;

    console.log('🔧 Debug store initialized');
  }

  function enable() {
    isEnabled.value = true;
    localStorage.setItem('debug-enabled', 'true');
    console.log('🔧 Debug mode enabled');
  }

  function disable() {
    isEnabled.value = false;
    stopSession();
    localStorage.removeItem('debug-enabled');
    console.log('🔧 Debug mode disabled');
  }

  function startSession(overrides: Partial<DebugSession> = {}) {
    if (session.value) {
      console.warn('Session already active');
      return;
    }

    const newSession: DebugSession = {
      id: generateId(),
      startTime: new Date(),
      environment: process.env.NODE_ENV || 'development',
      userAgent: navigator.userAgent,
      viewport: {
        width: window.innerWidth,
        height: window.innerHeight,
      },
      correlationId: generateId(),
      ...overrides,
    };

    session.value = newSession;
    actions.value = [];
    errors.value = [];

    console.log('🎬 Debug session started:', newSession.id);

    // Broadcast session start
    if (settings.enableSignalR) {
      // TODO: Implement SignalR broadcasting
    }
  }

  function stopSession() {
    if (!session.value) return;

    session.value.endTime = new Date();
    console.log('⏹️ Debug session stopped:', session.value.id);

    // Broadcast session end
    if (settings.enableSignalR) {
      // TODO: Implement SignalR broadcasting
    }

    session.value = null;
  }

  function recordAction(action: Omit<DebugAction, 'id' | 'sessionId' | 'timestamp'>) {
    if (!session.value) return;

    const debugAction: DebugAction = {
      id: generateId(),
      sessionId: session.value.id,
      timestamp: new Date(),
      ...action,
    };

    actions.value.push(debugAction);

    // Limit actions per session
    if (actions.value.length > settings.maxActionsPerSession) {
      actions.value = actions.value.slice(-settings.maxActionsPerSession);
    }

    if (settings.enableConsoleLogging) {
      console.log('📝 Action recorded:', debugAction.type, debugAction.element || debugAction.url);
    }
  }

  function recordError(error: Omit<DebugError, 'id' | 'sessionId' | 'timestamp' | 'userAgent'>) {
    if (!session.value) return;

    const debugError: DebugError = {
      id: generateId(),
      sessionId: session.value.id,
      timestamp: new Date(),
      userAgent: navigator.userAgent,
      ...error,
    };

    errors.value.push(debugError);

    // Limit errors per session
    if (errors.value.length > settings.maxErrorsPerSession) {
      errors.value = errors.value.slice(-settings.maxErrorsPerSession);
    }

    console.error('🚨 Error recorded:', debugError.message);

    // Broadcast error
    if (settings.enableSignalR) {
      // TODO: Implement SignalR broadcasting
    }
  }

  function submitFeedback(
    feedback: Omit<DebugFeedback, 'id' | 'sessionId' | 'timestamp' | 'userAgent' | 'url'>
  ) {
    const debugFeedback: DebugFeedback = {
      id: generateId(),
      sessionId: session.value?.id || 'no-session',
      timestamp: new Date(),
      userAgent: navigator.userAgent,
      url: window.location.href,
      ...feedback,
    };

    feedbacks.value.push(debugFeedback);

    console.log('💬 Feedback submitted:', debugFeedback.title);

    // Broadcast feedback
    if (settings.enableSignalR) {
      // TODO: Implement SignalR broadcasting
    }

    return debugFeedback;
  }

  function updateSettings(newSettings: Partial<DebugSettings>) {
    Object.assign(settings, newSettings);
    saveSettings();
  }

  function clearData() {
    actions.value = [];
    errors.value = [];
    feedbacks.value = [];
    console.log('🧹 Debug data cleared');
  }

  // Private methods
  function loadSettings() {
    try {
      const saved = localStorage.getItem('debug-settings');
      if (saved) {
        const parsed = JSON.parse(saved);
        Object.assign(settings, parsed);
      }
    } catch (error) {
      console.warn('Failed to load debug settings:', error);
    }
  }

  function saveSettings() {
    try {
      localStorage.setItem('debug-settings', JSON.stringify(settings));
    } catch (error) {
      console.warn('Failed to save debug settings:', error);
    }
  }

  function setupEventListeners() {
    // Auto-record clicks
    if (settings.autoRecordClicks) {
      document.addEventListener('click', handleClick, true);
    }

    // Auto-record navigation
    if (settings.autoRecordNavigation) {
      window.addEventListener('popstate', handleNavigation);
      // Override pushState and replaceState
      const originalPushState = history.pushState;
      const originalReplaceState = history.replaceState;

      history.pushState = function (...args) {
        originalPushState.apply(this, args);
        handleNavigation();
      };

      history.replaceState = function (...args) {
        originalReplaceState.apply(this, args);
        handleNavigation();
      };
    }

    // Auto-record errors
    if (settings.autoRecordErrors) {
      window.addEventListener('error', handleError);
      window.addEventListener('unhandledrejection', handleUnhandledRejection);
    }
  }

  function handleClick(event: MouseEvent) {
    if (!isRecording.value) return;

    const target = event.target as HTMLElement;
    const action: Omit<DebugAction, 'id' | 'sessionId' | 'timestamp'> = {
      type: 'click',
      element: target.tagName.toLowerCase(),
      selector: getElementSelector(target),
      metadata: {
        x: event.clientX,
        y: event.clientY,
        altKey: event.altKey,
        ctrlKey: event.ctrlKey,
        shiftKey: event.shiftKey,
      },
    };

    recordAction(action);
  }

  function handleNavigation() {
    if (!isRecording.value) return;

    const action: Omit<DebugAction, 'id' | 'sessionId' | 'timestamp'> = {
      type: 'navigation',
      url: window.location.href,
      metadata: {
        referrer: document.referrer,
        title: document.title,
      },
    };

    recordAction(action);
  }

  function handleError(event: ErrorEvent) {
    if (!isRecording.value) return;

    const error: Omit<DebugError, 'id' | 'sessionId' | 'timestamp' | 'userAgent'> = {
      type: 'javascript',
      message: event.message,
      stack: event.error?.stack,
      url: event.filename,
      line: event.lineno,
      column: event.colno,
    };

    recordError(error);
  }

  function handleUnhandledRejection(event: PromiseRejectionEvent) {
    if (!isRecording.value) return;

    const error: Omit<DebugError, 'id' | 'sessionId' | 'timestamp' | 'userAgent'> = {
      type: 'unhandled',
      message: event.reason?.message || String(event.reason),
      stack: event.reason?.stack,
    };

    recordError(error);
  }

  function getElementSelector(element: HTMLElement): string {
    if (element.id) {
      return `#${element.id}`;
    }

    if (element.className) {
      return `.${element.className.split(' ').join('.')}`;
    }

    // Generate CSS selector
    let selector = element.tagName.toLowerCase();
    if (element.parentElement) {
      let sibling = element.previousElementSibling;
      let nth = 1;
      while (sibling) {
        if (sibling.tagName === element.tagName) {
          nth++;
        }
        sibling = sibling.previousElementSibling;
      }
      if (nth > 1) {
        selector += `:nth-of-type(${nth})`;
      }
    }

    return selector;
  }

  function generateId(): string {
    return Date.now().toString(36) + Math.random().toString(36).substr(2);
  }

  return {
    // State
    isEnabled,
    isInitialized,
    session,
    actions,
    errors,
    feedbacks,
    settings,

    // Getters
    isRecording,
    sessionDuration,
    actionsCount,
    errorsCount,
    feedbacksCount,
    recentActions,
    recentErrors,

    // Actions
    initialize,
    enable,
    disable,
    startSession,
    stopSession,
    recordAction,
    recordError,
    submitFeedback,
    updateSettings,
    clearData,
  };
});
