/**
 * Debug API Service
 *
 * Handles communication with backend debug endpoints for:
 * - Session management
 * - Action logging
 * - Error reporting
 * - Feedback submission
 * - SignalR streaming
 */

import { HubConnectionBuilder, LogLevel, HubConnection } from '@microsoft/signalr';
import type { DebugSession, DebugAction, DebugError, DebugFeedback } from '@/stores/debug';
import { DEBUG_CONFIG } from '@/config/debug';

class DebugApiService {
  private baseUrl: string;
  private tenantId?: string;
  private correlationId?: string;
  private signalRConnection?: HubConnection;
  private isSignalRConnected = false;

  constructor() {
    this.baseUrl = this.getBaseUrl();
    this.tenantId = this.getTenantId();
    this.initializeSignalR();
  }

  private getBaseUrl(): string {
    // Determine API base URL based on environment
    if (process.env.NODE_ENV === 'production') {
      return '/api/debug';
    }
    return 'http://localhost:8000/api/debug'; // Store API
  }

  private getTenantId(): string | undefined {
    // Extract tenant ID from URL or localStorage
    const urlMatch = window.location.pathname.match(/^\/t\/([^/]+)/);
    if (urlMatch) {
      return urlMatch[1];
    }

    return localStorage.getItem('tenant-id') || undefined;
  }

  private async initializeSignalR(): Promise<void> {
    if (!DEBUG_CONFIG.features.enableSignalR) {
      return;
    }

    try {
      this.signalRConnection = new HubConnectionBuilder()
        .withUrl(DEBUG_CONFIG.signalR.hubUrl, {
          headers: {
            'X-Tenant-ID': this.tenantId || '',
            'X-Correlation-ID': this.correlationId || '',
          },
        })
        .withAutomaticReconnect()
        .configureLogging(
          DEBUG_CONFIG.signalR.enableLogging ? LogLevel.Information : LogLevel.Error
        )
        .build();

      // Set up event handlers
      this.setupSignalREventHandlers();

      // Start connection
      await this.signalRConnection.start();
      this.isSignalRConnected = true;

      console.log('[DebugAPI] SignalR connected');
    } catch (error) {
      console.error('[DebugAPI] SignalR connection failed:', error);
      this.isSignalRConnected = false;
    }
  }

  private setupSignalREventHandlers(): void {
    if (!this.signalRConnection) return;

    // Handle session events
    this.signalRConnection.on('SessionStarted', data => {
      console.log('[DebugAPI] Session started:', data);
      // Emit custom event for Vue components
      window.dispatchEvent(new CustomEvent('debug:sessionStarted', { detail: data }));
    });

    // Handle error events
    this.signalRConnection.on('ErrorCaptured', data => {
      console.log('[DebugAPI] Error captured:', data);
      window.dispatchEvent(new CustomEvent('debug:errorCaptured', { detail: data }));
    });

    // Handle feedback events
    this.signalRConnection.on('FeedbackSubmitted', data => {
      console.log('[DebugAPI] Feedback submitted:', data);
      window.dispatchEvent(new CustomEvent('debug:feedbackSubmitted', { detail: data }));
    });

    // Handle action events
    this.signalRConnection.on('ActionRecorded', data => {
      console.log('[DebugAPI] Action recorded:', data);
      window.dispatchEvent(new CustomEvent('debug:actionRecorded', { detail: data }));
    });

    // Handle performance alerts
    this.signalRConnection.on('PerformanceAlert', data => {
      console.warn('[DebugAPI] Performance alert:', data);
      window.dispatchEvent(new CustomEvent('debug:performanceAlert', { detail: data }));
    });
  }

  public async disconnectSignalR(): Promise<void> {
    if (this.signalRConnection && this.isSignalRConnected) {
      await this.signalRConnection.stop();
      this.isSignalRConnected = false;
      console.log('[DebugAPI] SignalR disconnected');
    }
  }

  public getSignalRConnectionStatus(): boolean {
    return this.isSignalRConnected;
  }

  private async makeRequest<T>(
    endpoint: string,
    method: 'GET' | 'POST' | 'PUT' | 'DELETE' = 'GET',
    data?: unknown
  ): Promise<T> {
    const url = `${this.baseUrl}${endpoint}`;
    const headers: HeadersInit = {
      'Content-Type': 'application/json',
    };

    // Add tenant header if available
    if (this.tenantId) {
      headers['X-Tenant-ID'] = this.tenantId;
    }

    // Add correlation ID if available
    if (this.correlationId) {
      headers['X-Correlation-ID'] = this.correlationId;
    }

    const config: RequestInit = {
      method,
      headers,
    };

    if (data && method !== 'GET') {
      config.body = JSON.stringify(data);
    }

    try {
      const response = await fetch(url, config);

      if (!response.ok) {
        throw new Error(`HTTP ${response.status}: ${response.statusText}`);
      }

      return await response.json();
    } catch (error) {
      console.error(`Debug API request failed: ${method} ${url}`, error);
      throw error;
    }
  }

  // Session Management
  async createSession(session: DebugSession): Promise<{ sessionId: string }> {
    this.correlationId = session.correlationId;
    return this.makeRequest('/sessions', 'POST', session);
  }

  async updateSession(sessionId: string, updates: Partial<DebugSession>): Promise<void> {
    return this.makeRequest(`/sessions/${sessionId}`, 'PUT', updates);
  }

  async endSession(sessionId: string): Promise<void> {
    return this.makeRequest(`/sessions/${sessionId}/end`, 'POST');
  }

  async getSession(sessionId: string): Promise<DebugSession> {
    return this.makeRequest(`/sessions/${sessionId}`);
  }

  async getActiveSessions(): Promise<DebugSession[]> {
    return this.makeRequest('/sessions/active');
  }

  // Action Logging
  async logAction(action: DebugAction): Promise<void> {
    return this.makeRequest('/actions', 'POST', action);
  }

  async logActions(actions: DebugAction[]): Promise<void> {
    return this.makeRequest('/actions/batch', 'POST', actions);
  }

  async getSessionActions(sessionId: string): Promise<DebugAction[]> {
    return this.makeRequest(`/sessions/${sessionId}/actions`);
  }

  // Error Reporting
  async reportError(error: DebugError): Promise<void> {
    return this.makeRequest('/errors', 'POST', error);
  }

  async reportErrors(errors: DebugError[]): Promise<void> {
    return this.makeRequest('/errors/batch', 'POST', errors);
  }

  async getSessionErrors(sessionId: string): Promise<DebugError[]> {
    return this.makeRequest(`/sessions/${sessionId}/errors`);
  }

  // Feedback Submission
  async submitFeedback(feedback: DebugFeedback): Promise<{ feedbackId: string }> {
    return this.makeRequest('/feedback', 'POST', feedback);
  }

  async getSessionFeedback(sessionId: string): Promise<DebugFeedback[]> {
    return this.makeRequest(`/sessions/${sessionId}/feedback`);
  }

  // SignalR Connection (for future implementation)
  async getSignalRConnectionInfo(): Promise<{
    url: string;
    accessToken?: string;
  }> {
    return this.makeRequest('/signalr/info');
  }

  // Analytics and Reporting
  async getSessionStats(sessionId: string): Promise<{
    duration: number;
    actionsCount: number;
    errorsCount: number;
    feedbacksCount: number;
  }> {
    return this.makeRequest(`/sessions/${sessionId}/stats`);
  }

  async getDebugSummary(timeRange: { start: Date; end: Date }): Promise<{
    totalSessions: number;
    totalActions: number;
    totalErrors: number;
    totalFeedback: number;
    topErrors: Array<{ message: string; count: number }>;
  }> {
    const params = new URLSearchParams({
      start: timeRange.start.toISOString(),
      end: timeRange.end.toISOString(),
    });

    return this.makeRequest(`/summary?${params}`);
  }

  // Utility Methods
  setTenantId(tenantId: string): void {
    this.tenantId = tenantId;
  }

  setCorrelationId(correlationId: string): void {
    this.correlationId = correlationId;
  }

  // Health Check
  async healthCheck(): Promise<{ status: 'healthy' | 'unhealthy'; version: string }> {
    try {
      return await this.makeRequest('/health');
    } catch {
      return { status: 'unhealthy', version: 'unknown' };
    }
  }

  // Batch Operations for Performance
  async sendBatchData(data: {
    session?: DebugSession;
    actions?: DebugAction[];
    errors?: DebugError[];
    feedbacks?: DebugFeedback[];
  }): Promise<void> {
    return this.makeRequest('/batch', 'POST', data);
  }
}

// Create singleton instance
export const debugApi = new DebugApiService();

// Export types for convenience
export type { DebugSession, DebugAction, DebugError, DebugFeedback };

export default debugApi;
