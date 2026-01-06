// Simple client-side error logger for Admin frontend
import axios from 'axios';

type LogLevel = 'error' | 'warn' | 'info';

interface ClientLog {
  tenantId?: string;
  userId?: string;
  level: LogLevel;
  message: string;
  stack?: string;
  route?: string;
  component?: string;
  meta?: Record<string, unknown>;
  timestamp: string;
}

const BATCH_SIZE = 10;
const FLUSH_INTERVAL_MS = 5000;
// Use existing backend endpoint that accepts frontend error batches
const ENDPOINT = '/api/logs/errors';

class ErrorLogger {
  private queue: ClientLog[] = [];
  private timer: number | null = null;

  start() {
    if (this.timer) return;
    this.timer = window.setInterval(() => this.flush(), FLUSH_INTERVAL_MS);
  }

  stop() {
    if (this.timer) {
      clearInterval(this.timer);
      this.timer = null;
    }
  }

  log(level: LogLevel, message: string, data?: Partial<ClientLog>) {
    const entry: ClientLog = {
      level,
      message,
      timestamp: new Date().toISOString(),
      ...data,
    };
    this.queue.push(entry);
    if (this.queue.length >= BATCH_SIZE) this.flush();
  }

  error(err: unknown, data?: Partial<ClientLog>) {
    const message = err instanceof Error ? err.message : String(err);
    const stack = err instanceof Error ? err.stack : undefined;
    this.log('error', message, { ...data, stack });
  }

  async flush() {
    if (this.queue.length === 0) return;
    const payload = this.queue.splice(0, BATCH_SIZE);

    // Map to backend shape { errors: [ ... ] }
    const body = {
      errors: payload.map(p => ({
        id: p.tenantId ? `${p.tenantId}_${p.timestamp}` : p.timestamp,
        timestamp: p.timestamp,
        severity: p.level,
        message: p.message,
        stack: p.stack,
        componentName: p.component,
        routePath: p.route,
        userId: p.userId,
        tenantId: p.tenantId,
        userAgent: navigator.userAgent,
        url: window.location.href,
        fingerprint: p.meta?.fingerprint as string | undefined,
        Context: p.meta,
      })),
    };

    try {
      const tenantHeader = payload[0]?.tenantId || localStorage.getItem('tenantId') || '';
      await axios.post(ENDPOINT, body, { headers: { 'X-Tenant-ID': tenantHeader } });
    } catch (e) {
      // on failure, requeue and simple backoff
      this.queue.unshift(...payload);
    }
  }

  installVueHandler(
    app: any,
    ctxProvider: () => { tenantId?: string; userId?: string; route?: string }
  ) {
    app.config.errorHandler = (err: Error, vm: any, info: string) => {
      const ctx = ctxProvider();
      this.error(err, {
        component: vm?.$options?.name || info,
        route: ctx.route,
        tenantId: ctx.tenantId,
        userId: ctx.userId,
      });
    };
  }

  installGlobalHandlers(ctxProvider: () => { tenantId?: string; userId?: string; route?: string }) {
    window.addEventListener('error', ev => {
      const ctx = ctxProvider();
      const err = ev.error ?? ev.message;
      this.error(err, {
        route: ctx.route,
        tenantId: ctx.tenantId,
        userId: ctx.userId,
      });
    });
    window.addEventListener('unhandledrejection', ev => {
      const ctx = ctxProvider();
      this.error(ev.reason, { route: ctx.route, tenantId: ctx.tenantId, userId: ctx.userId });
    });
  }
}

export const errorLogger = new ErrorLogger();

export default errorLogger;
