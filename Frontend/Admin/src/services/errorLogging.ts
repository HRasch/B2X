/**
 * Production Error Logging Service
 *
 * Captures, logs, and reports errors in production for easy debugging.
 * Supports automatic error grouping, context enrichment, and optional
 * integration with external services (Sentry, LogRocket, etc.)
 */

import type { App, ComponentPublicInstance } from 'vue';
import type { Router } from 'vue-router';

// Error severity levels
export type ErrorSeverity = 'fatal' | 'error' | 'warning' | 'info';

// Structured error log entry
export interface ErrorLogEntry {
  id: string;
  timestamp: string;
  severity: ErrorSeverity;
  message: string;
  stack?: string;
  componentName?: string;
  routePath?: string;
  routeName?: string;
  userId?: string;
  tenantId?: string;
  userAgent: string;
  url: string;
  context?: Record<string, unknown>;
  fingerprint?: string; // For grouping similar errors
}

// Configuration options
export interface ErrorLoggingConfig {
  enabled: boolean;
  logToConsole: boolean;
  logToServer: boolean;
  serverEndpoint?: string;
  maxStoredErrors: number;
  sampleRate: number; // 0-1, percentage of errors to report
  ignoredErrors: RegExp[];
  beforeSend?: (error: ErrorLogEntry) => ErrorLogEntry | null;
}

const DEFAULT_CONFIG: ErrorLoggingConfig = {
  enabled: import.meta.env.PROD,
  logToConsole: !import.meta.env.PROD,
  logToServer: import.meta.env.PROD,
  serverEndpoint: '/api/logs/errors',
  maxStoredErrors: 100,
  sampleRate: 1.0,
  ignoredErrors: [
    /ResizeObserver loop/i,
    /Loading chunk .* failed/i,
    /Network Error/i,
    /timeout of .* exceeded/i,
  ],
};

class ErrorLoggingService {
  private config: ErrorLoggingConfig;
  private errorQueue: ErrorLogEntry[] = [];
  private router?: Router;
  private userId?: string;
  private tenantId?: string;
  private isProcessingQueue = false;

  constructor(config: Partial<ErrorLoggingConfig> = {}) {
    this.config = { ...DEFAULT_CONFIG, ...config };
    this.setupGlobalHandlers();
  }

  /**
   * Initialize with Vue app and router
   */
  init(app: App, router?: Router): void {
    this.router = router;

    // Vue error handler
    app.config.errorHandler = (err, instance, info) => {
      this.captureVueError(err, instance, info);
    };

    // Vue warning handler (dev only)
    if (!import.meta.env.PROD) {
      app.config.warnHandler = (msg, instance, trace) => {
        console.warn(`[Vue warn]: ${msg}`, { instance, trace });
      };
    }

    console.log('[ErrorLogging] Initialized', {
      enabled: this.config.enabled,
      logToServer: this.config.logToServer,
    });
  }

  /**
   * Set user context for error attribution
   */
  setUserContext(userId?: string, tenantId?: string): void {
    this.userId = userId;
    this.tenantId = tenantId;
  }

  /**
   * Clear user context on logout
   */
  clearUserContext(): void {
    this.userId = undefined;
    this.tenantId = undefined;
  }

  /**
   * Manually capture an error
   */
  captureError(
    error: Error | string,
    severity: ErrorSeverity = 'error',
    context?: Record<string, unknown>
  ): void {
    const errorObj = typeof error === 'string' ? new Error(error) : error;
    this.processError(errorObj, severity, context);
  }

  /**
   * Capture a message (non-error event)
   */
  captureMessage(
    message: string,
    severity: ErrorSeverity = 'info',
    context?: Record<string, unknown>
  ): void {
    this.processError(new Error(message), severity, {
      ...context,
      isMessage: true,
    });
  }

  /**
   * Capture API/network error with request context
   */
  captureNetworkError(
    error: Error,
    requestInfo: {
      url: string;
      method: string;
      status?: number;
      duration?: number;
    }
  ): void {
    this.processError(error, 'error', {
      type: 'network',
      ...requestInfo,
    });
  }

  /**
   * Get recent errors (for debugging UI)
   */
  getRecentErrors(): ErrorLogEntry[] {
    return [...this.errorQueue];
  }

  /**
   * Clear stored errors
   */
  clearErrors(): void {
    this.errorQueue = [];
  }

  // Private methods

  private setupGlobalHandlers(): void {
    // Unhandled promise rejections
    window.addEventListener('unhandledrejection', event => {
      const error = event.reason instanceof Error ? event.reason : new Error(String(event.reason));
      this.processError(error, 'error', { type: 'unhandledrejection' });
    });

    // Global errors
    window.addEventListener('error', event => {
      // Ignore script loading errors (usually ad blockers or extensions)
      if (event.filename && !event.filename.includes(window.location.origin)) {
        return;
      }
      this.processError(event.error || new Error(event.message), 'error', {
        type: 'global',
        filename: event.filename,
        lineno: event.lineno,
      });
    });
  }

  private captureVueError(
    err: unknown,
    instance: ComponentPublicInstance | null,
    info: string
  ): void {
    const error = err instanceof Error ? err : new Error(String(err));
    const componentName = instance?.$options?.name || instance?.$options?.__name || 'Anonymous';

    this.processError(error, 'error', {
      type: 'vue',
      componentName,
      lifecycleHook: info,
    });

    // Re-throw in development for better debugging
    if (!import.meta.env.PROD) {
      console.error(`[Vue Error in ${componentName}]:`, err);
    }
  }

  private processError(
    error: Error,
    severity: ErrorSeverity,
    context?: Record<string, unknown>
  ): void {
    if (!this.config.enabled) return;

    // Check if error should be ignored
    if (this.shouldIgnoreError(error)) return;

    // Sample rate check
    if (Math.random() > this.config.sampleRate) return;

    const entry = this.createLogEntry(error, severity, context);

    // Apply beforeSend hook
    if (this.config.beforeSend) {
      const modified = this.config.beforeSend(entry);
      if (!modified) return; // Filter out if null returned
    }

    // Console logging
    if (this.config.logToConsole) {
      this.logToConsole(entry);
    }

    // Queue for server
    this.queueError(entry);

    // Send to server
    if (this.config.logToServer) {
      this.flushToServer();
    }
  }

  private shouldIgnoreError(error: Error): boolean {
    const message = error.message || '';
    return this.config.ignoredErrors.some(pattern => pattern.test(message));
  }

  private createLogEntry(
    error: Error,
    severity: ErrorSeverity,
    context?: Record<string, unknown>
  ): ErrorLogEntry {
    const route = this.router?.currentRoute.value;

    return {
      id: this.generateErrorId(),
      timestamp: new Date().toISOString(),
      severity,
      message: error.message,
      stack: this.sanitizeStack(error.stack),
      componentName: context?.componentName as string | undefined,
      routePath: route?.path,
      routeName: route?.name as string | undefined,
      userId: this.userId,
      tenantId: this.tenantId,
      userAgent: navigator.userAgent,
      url: window.location.href,
      context,
      fingerprint: this.generateFingerprint(error),
    };
  }

  private generateErrorId(): string {
    return `err_${Date.now()}_${Math.random().toString(36).substring(2, 9)}`;
  }

  private generateFingerprint(error: Error): string {
    // Create a fingerprint for grouping similar errors
    const parts = [
      error.name,
      error.message.replace(/\d+/g, 'N'), // Normalize numbers
      error.stack?.split('\n')[1]?.trim() || '', // First stack frame
    ];
    return btoa(parts.join('|')).substring(0, 32);
  }

  private sanitizeStack(stack?: string): string | undefined {
    if (!stack) return undefined;
    // Remove sensitive data from stack traces
    return stack
      .replace(/token=[^&\s]+/gi, 'token=[REDACTED]')
      .replace(/password=[^&\s]+/gi, 'password=[REDACTED]')
      .replace(/apikey=[^&\s]+/gi, 'apikey=[REDACTED]');
  }

  private logToConsole(entry: ErrorLogEntry): void {
    const style = {
      fatal: 'color: white; background: red; padding: 2px 6px;',
      error: 'color: red;',
      warning: 'color: orange;',
      info: 'color: blue;',
    };

    console.groupCollapsed(
      `%c[${entry.severity.toUpperCase()}]%c ${entry.message}`,
      style[entry.severity],
      ''
    );
    console.log('ID:', entry.id);
    console.log('Timestamp:', entry.timestamp);
    console.log('Route:', entry.routePath);
    console.log('User:', entry.userId || 'anonymous');
    if (entry.stack) console.log('Stack:', entry.stack);
    if (entry.context) console.log('Context:', entry.context);
    console.groupEnd();
  }

  private queueError(entry: ErrorLogEntry): void {
    this.errorQueue.push(entry);

    // Trim queue if too large
    if (this.errorQueue.length > this.config.maxStoredErrors) {
      this.errorQueue = this.errorQueue.slice(-this.config.maxStoredErrors);
    }

    // Persist to localStorage for crash recovery
    try {
      localStorage.setItem('b2connect_error_queue', JSON.stringify(this.errorQueue.slice(-20)));
    } catch {
      // localStorage might be full or unavailable
    }
  }

  private async flushToServer(): Promise<void> {
    if (this.isProcessingQueue || this.errorQueue.length === 0) return;
    if (!this.config.serverEndpoint) return;

    this.isProcessingQueue = true;

    try {
      const errors = this.errorQueue.splice(0, 10); // Send up to 10 at a time

      const response = await fetch(this.config.serverEndpoint, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'X-Tenant-ID': this.tenantId || '',
        },
        body: JSON.stringify({ errors }),
        keepalive: true, // Ensure delivery even on page unload
      });

      if (!response.ok) {
        // Re-queue on failure
        this.errorQueue.unshift(...errors);
      }
    } catch {
      // Silently fail - don't create error loops
    } finally {
      this.isProcessingQueue = false;

      // Continue if more errors queued
      if (this.errorQueue.length > 0) {
        setTimeout(() => this.flushToServer(), 1000);
      }
    }
  }
}

// Singleton instance
export const errorLogging = new ErrorLoggingService();

// Vue plugin
export const errorLoggingPlugin = {
  install(app: App, options?: { router?: Router }) {
    errorLogging.init(app, options?.router);

    // Provide to components
    app.provide('errorLogging', errorLogging);

    // Global property
    app.config.globalProperties.$errorLogging = errorLogging;
  },
};

// Composable for components
export function useErrorLogging() {
  return {
    captureError: errorLogging.captureError.bind(errorLogging),
    captureMessage: errorLogging.captureMessage.bind(errorLogging),
    captureNetworkError: errorLogging.captureNetworkError.bind(errorLogging),
    setUserContext: errorLogging.setUserContext.bind(errorLogging),
    clearUserContext: errorLogging.clearUserContext.bind(errorLogging),
    getRecentErrors: errorLogging.getRecentErrors.bind(errorLogging),
  };
}

export default errorLogging;
