import apiClient from './api';

export enum EmailStatus {
  Queued = 0,
  Processing = 1,
  Sent = 2,
  Failed = 3,
  Cancelled = 4,
  Scheduled = 5,
}

export interface EmailMessage {
  id: string;
  tenantId: string;
  toEmail: string;
  subject: string;
  body: string;
  status: EmailStatus;
  retryCount: number;
  maxRetries: number;
  nextRetryAt?: string;
  sentAt?: string;
  failedAt?: string;
  lastError?: string;
  messageId?: string;
  scheduledFor?: string;
  createdAt: string;
  updatedAt: string;
}

export interface EmailStatistics {
  totalSent: number;
  totalDelivered: number;
  totalOpened: number;
  totalClicked: number;
  totalBounced: number;
  totalFailed: number;
  openRate: number;
  clickRate: number;
  bounceRate: number;
  deliveryRate: number;
}

export interface EmailEvent {
  id: string;
  emailId: string;
  eventType: number;
  metadata?: string;
  occurredAt: string;
}

export interface EmailHealthStatus {
  totalEmailsLast24Hours: number;
  failedEmailsLast24Hours: number;
  bouncedEmailsLast24Hours: number;
  lastEmailSent?: string;
  isHealthy: boolean;
}

export interface EmailSendResult {
  success: boolean;
  messageId?: string;
  errorMessage?: string;
  status: EmailSendStatus;
  sentAt: string;
}

export enum EmailSendStatus {
  Sent = 0,
  Failed = 1,
  Queued = 2,
  Bounced = 3,
}

export interface SmtpSettings {
  host: string;
  port: number;
  username: string;
  password: string;
  enableSsl: boolean;
  timeout: number;
  fromEmail: string;
  fromName: string;
  domain: string;
}

export interface SmtpTestResult {
  success: boolean;
  message: string;
  responseTime: number;
  testedAt: string;
}

export const emailMonitoringService = {
  async getHealthStatus(tenantId?: string): Promise<{ data: EmailHealthStatus }> {
    const headers = tenantId ? { 'X-Tenant-ID': tenantId } : {};
    const response = await apiClient.get('/admin/email/health', { headers });
    return response;
  },

  async getStatistics(
    fromDate: string,
    toDate: string,
    tenantId?: string
  ): Promise<{ data: EmailStatistics }> {
    const headers = tenantId ? { 'X-Tenant-ID': tenantId } : {};
    const response = await apiClient.get('/admin/email/analytics/summary', {
      headers,
      params: { fromDate, toDate },
    });
    return response;
  },

  async getRecentEvents(limit: number = 50, tenantId?: string): Promise<{ data: EmailEvent[] }> {
    const headers = tenantId ? { 'X-Tenant-ID': tenantId } : {};
    const response = await apiClient.get('/admin/email/events/recent', {
      headers,
      params: { limit },
    });
    return response;
  },

  async getErrorEvents(
    fromDate: string,
    toDate: string,
    tenantId?: string
  ): Promise<{ data: EmailEvent[] }> {
    const headers = tenantId ? { 'X-Tenant-ID': tenantId } : {};
    const response = await apiClient.get('/admin/email/events/errors', {
      headers,
      params: { fromDate, toDate },
    });
    return response;
  },

  async getEventsByType(
    fromDate: string,
    toDate: string,
    tenantId?: string
  ): Promise<{ data: Record<string, number> }> {
    const headers = tenantId ? { 'X-Tenant-ID': tenantId } : {};
    const response = await apiClient.get('/admin/email/analytics/by-type', {
      headers,
      params: { fromDate, toDate },
    });
    return response;
  },

  async sendTestEmail(
    toEmail: string,
    subject: string,
    body: string,
    tenantId?: string
  ): Promise<{ data: EmailSendResult }> {
    const headers = tenantId ? { 'X-Tenant-ID': tenantId } : {};
    const response = await apiClient.post(
      '/admin/email/test',
      {
        toEmail,
        subject,
        body,
      },
      { headers }
    );
    return response;
  },

  async getSmtpConfiguration(tenantId?: string): Promise<{ data: SmtpSettings }> {
    const headers = tenantId ? { 'X-Tenant-ID': tenantId } : {};
    const response = await apiClient.get('/admin/email/smtp/config', { headers });
    return response;
  },

  async updateSmtpConfiguration(
    settings: SmtpSettings,
    tenantId?: string
  ): Promise<{ data: void }> {
    const headers = tenantId ? { 'X-Tenant-ID': tenantId } : {};
    const response = await apiClient.put('/admin/email/smtp/config', settings, { headers });
    return response;
  },

  async getEmailMessages(
    params?: {
      status?: EmailStatus;
      search?: string;
      skip?: number;
      take?: number;
    },
    tenantId?: string
  ): Promise<{ data: EmailMessage[]; total?: number }> {
    const headers = tenantId ? { 'X-Tenant-ID': tenantId } : {};
    const queryParams = new URLSearchParams();

    if (params?.status) queryParams.append('status', params.status.toString());
    if (params?.search) queryParams.append('search', params.search);
    if (params?.skip !== undefined) queryParams.append('skip', params.skip.toString());
    if (params?.take !== undefined) queryParams.append('take', params.take.toString());

    const response = await apiClient.get(`/admin/email/messages?${queryParams}`, { headers });
    return response;
  },

  async getEmailMessage(id: string, tenantId?: string): Promise<{ data: EmailMessage }> {
    const headers = tenantId ? { 'X-Tenant-ID': tenantId } : {};
    const response = await apiClient.get(`/admin/email/messages/${id}`, { headers });
    return response;
  },

  async retryEmailMessage(id: string, tenantId?: string): Promise<{ data: void }> {
    const headers = tenantId ? { 'X-Tenant-ID': tenantId } : {};
    const response = await apiClient.post(`/admin/email/messages/${id}/retry`, {}, { headers });
    return response;
  },

  async cancelEmailMessage(id: string, tenantId?: string): Promise<{ data: void }> {
    const headers = tenantId ? { 'X-Tenant-ID': tenantId } : {};
    const response = await apiClient.post(`/admin/email/messages/${id}/cancel`, {}, { headers });
    return response;
  },
};
