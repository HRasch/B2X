import axios from 'axios';

export interface AnalyticsEvent {
  eventType: string;
  userId?: string;
  sessionId: string;
  timestamp: Date;
  properties: Record<string, unknown>;
}

class AnalyticsService {
  private sessionId: string;
  private baseUrl = '/api/analytics'; // Adjust based on your API

  constructor() {
    this.sessionId = this.getOrCreateSessionId();
  }

  private getOrCreateSessionId(): string {
    let sessionId = localStorage.getItem('analytics_session_id');
    if (!sessionId) {
      sessionId = `session_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
      localStorage.setItem('analytics_session_id', sessionId);
    }
    return sessionId;
  }

  async trackEvent(eventType: string, properties: Record<string, unknown> = {}) {
    const event: AnalyticsEvent = {
      eventType,
      sessionId: this.sessionId,
      timestamp: new Date(),
      properties,
    };

    try {
      await axios.post(`${this.baseUrl}/track`, event);
    } catch (error) {
      console.error('Analytics tracking failed:', error);
    }
  }

  async getABTestVariant(testId: string): Promise<string> {
    try {
      const response = await axios.get(`${this.baseUrl}/ab-test/${testId}`);
      return response.data.variant;
    } catch (error) {
      console.error('A/B test variant fetch failed:', error);
      return 'A'; // Default variant
    }
  }

  async getConversionFunnel(funnelId: string) {
    try {
      const response = await axios.get(`${this.baseUrl}/funnel/${funnelId}`);
      return response.data;
    } catch (error) {
      console.error('Funnel data fetch failed:', error);
      return null;
    }
  }
}

export const analyticsService = new AnalyticsService();
