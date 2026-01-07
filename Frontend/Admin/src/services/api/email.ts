import { apiClient } from '../client';
import type {
  EmailTemplate,
  EmailLayout,
  EmailSendingAccount,
  EmailTemplateForm,
  EmailTemplatePreview,
  EmailTemplateTestSend,
  EmailTemplateFilters,
  EmailAnalytics,
  EmailLog,
  EmailAttachment,
} from '@/types/email';

export const emailApi = {
  // Templates
  async getTemplates(filters?: EmailTemplateFilters): Promise<EmailTemplate[]> {
    const params = new URLSearchParams();
    if (filters?.status) params.append('status', filters.status);
    if (filters?.locale) params.append('locale', filters.locale);
    if (filters?.search) params.append('search', filters.search);

    const response = await apiClient.get(`/api/admin/email/templates?${params}`);
    return response.data;
  },

  async getTemplate(id: string): Promise<EmailTemplate> {
    const response = await apiClient.get(`/api/admin/email/templates/${id}`);
    return response.data;
  },

  async createTemplate(template: EmailTemplateForm): Promise<EmailTemplate> {
    const response = await apiClient.post('/api/admin/email/templates', template);
    return response.data;
  },

  async updateTemplate(
    templateKey: string,
    locale: string,
    template: EmailTemplateForm
  ): Promise<EmailTemplate> {
    const response = await apiClient.put(
      `/api/admin/email/templates/${templateKey}/${locale}`,
      template
    );
    return response.data;
  },

  async deleteTemplate(id: string): Promise<void> {
    await apiClient.delete(`/api/admin/email/templates/${id}`);
  },

  async duplicateTemplate(id: string): Promise<EmailTemplate> {
    const response = await apiClient.post(`/api/admin/email/templates/${id}/duplicate`);
    return response.data;
  },

  async publishTemplate(id: string): Promise<EmailTemplate> {
    const response = await apiClient.post(`/api/admin/email/templates/${id}/publish`);
    return response.data;
  },

  async archiveTemplate(id: string): Promise<EmailTemplate> {
    const response = await apiClient.post(`/api/email/templates/${id}/archive`);
    return response.data;
  },

  // Layouts
  async getLayouts(): Promise<EmailLayout[]> {
    const response = await apiClient.get('/api/admin/email/layouts');
    return response.data;
  },

  async getLayout(id: string): Promise<EmailLayout> {
    const response = await apiClient.get(`/api/admin/email/layouts/${id}`);
    return response.data;
  },

  async createLayout(
    layout: Omit<EmailLayout, 'id' | 'createdAt' | 'updatedAt'>
  ): Promise<EmailLayout> {
    const response = await apiClient.post('/api/admin/email/layouts', layout);
    return response.data;
  },

  async updateLayout(id: string, layout: Partial<EmailLayout>): Promise<EmailLayout> {
    const response = await apiClient.put(`/api/admin/email/layouts/${id}`, layout);
    return response.data;
  },

  async deleteLayout(id: string): Promise<void> {
    await apiClient.delete(`/api/admin/email/layouts/${id}`);
  },

  // Accounts
  async getAccounts(): Promise<EmailSendingAccount[]> {
    const response = await apiClient.get('/api/admin/email/accounts');
    return response.data;
  },

  async getAccount(id: string): Promise<EmailSendingAccount> {
    const response = await apiClient.get(`/api/admin/email/accounts/${id}`);
    return response.data;
  },

  async createAccount(
    account: Omit<EmailSendingAccount, 'id' | 'createdAt' | 'updatedAt' | 'monthlyUsed'>
  ): Promise<EmailSendingAccount> {
    const response = await apiClient.post('/api/admin/email/accounts', account);
    return response.data;
  },

  async updateAccount(
    id: string,
    account: Partial<EmailSendingAccount>
  ): Promise<EmailSendingAccount> {
    const response = await apiClient.put(`/api/admin/email/accounts/${id}`, account);
    return response.data;
  },

  async deleteAccount(id: string): Promise<void> {
    await apiClient.delete(`/api/admin/email/accounts/${id}`);
  },

  async testAccount(id: string): Promise<{ success: boolean; message: string }> {
    const response = await apiClient.post(`/api/admin/email/accounts/${id}/test`);
    return response.data;
  },

  // Preview & Testing
  async previewTemplate(
    templateKey: string,
    locale: string,
    sampleData?: Record<string, unknown>
  ): Promise<EmailTemplatePreview> {
    const response = await apiClient.post('/api/admin/email/preview', {
      templateKey,
      locale,
      sampleData,
    });
    return response.data;
  },

  async sendTestEmail(
    testSend: EmailTemplateTestSend
  ): Promise<{ success: boolean; messageId?: string }> {
    const response = await apiClient.post('/api/admin/email/test-send', testSend);
    return response.data;
  },

  // Analytics
  async getAnalytics(startDate?: string, endDate?: string): Promise<EmailAnalytics> {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate);
    if (endDate) params.append('endDate', endDate);

    const response = await apiClient.get(`/api/admin/email/analytics?${params}`);
    return response.data;
  },

  async getLogs(templateKey?: string, status?: string, limit = 50): Promise<EmailLog[]> {
    const params = new URLSearchParams();
    if (templateKey) params.append('templateKey', templateKey);
    if (status) params.append('status', status);
    params.append('limit', limit.toString());

    const response = await apiClient.get(`/api/admin/email/logs?${params}`);
    return response.data;
  },

  // Attachments
  async getAttachments(): Promise<EmailAttachment[]> {
    const response = await apiClient.get('/api/admin/email/attachments');
    return response.data;
  },

  async uploadAttachment(file: File, displayName?: string): Promise<EmailAttachment> {
    const formData = new FormData();
    formData.append('file', file);
    if (displayName) {
      formData.append('displayName', displayName);
    }

    const response = await apiClient.post('/api/admin/email/attachments', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },

  async deleteAttachment(id: string): Promise<void> {
    await apiClient.delete(`/api/admin/email/attachments/${id}`);
  },

  // Template Variables
  async getLiquidVariables(): Promise<
    Array<{ name: string; description: string; category: string; example: string; type: string }>
  > {
    const response = await apiClient.get('/api/admin/email/variables');
    return response.data;
  },
};
