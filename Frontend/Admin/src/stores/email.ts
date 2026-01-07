import { defineStore } from 'pinia';
import { ref } from 'vue';
import type {
  EmailTemplate,
  EmailLayout,
  EmailSendingAccount,
  EmailTemplateForm,
  EmailTemplatePreview,
  EmailTemplateTestSend,
  EmailTemplateFilters,
  EmailAnalytics,
  EmailApiError,
} from '@/types/email';
import { emailApi } from '@/services/api/email';

export const useEmailStore = defineStore('email', () => {
  // State
  const templates = ref<EmailTemplate[]>([]);
  const layouts = ref<EmailLayout[]>([]);
  const accounts = ref<EmailSendingAccount[]>([]);
  const analytics = ref<EmailAnalytics | null>(null);
  const loading = ref(false);
  const error = ref<EmailApiError | null>(null);

  // Actions
  const fetchTemplates = async (filters?: EmailTemplateFilters) => {
    loading.value = true;
    error.value = null;
    try {
      const params = new URLSearchParams();
      if (filters?.status) params.append('status', filters.status);
      if (filters?.locale) params.append('locale', filters.locale);
      if (filters?.search) params.append('search', filters.search);

      const response = await emailApi.getTemplates(filters);
      templates.value = response.data;
    } catch (err: unknown) {
      const errorObj = err as { response?: { data?: unknown } };
      error.value = errorObj.response?.data || {
        code: 'FETCH_ERROR',
        message: 'Failed to fetch templates',
      };
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const fetchTemplate = async (id: string): Promise<EmailTemplate> => {
    loading.value = true;
    error.value = null;
    try {
      const response = await emailApi.getTemplate(id);
      return response.data;
    } catch (err: unknown) {
      const errorObj = err as { response?: { data?: unknown } };
      error.value = errorObj.response?.data || {
        code: 'FETCH_ERROR',
        message: 'Failed to fetch template',
      };
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const saveTemplate = async (
    form: EmailTemplateForm,
    isEditing: boolean
  ): Promise<EmailTemplate> => {
    loading.value = true;
    error.value = null;
    try {
      const url = isEditing
        ? `/api/email/templates/${form.templateKey}/${form.locale}`
        : '/api/email/templates';
      const method = isEditing ? 'put' : 'post';

      const response = isEditing
        ? await emailApi.updateTemplate(form.templateKey, form.locale, form)
        : await emailApi.createTemplate(form);
      return response.data;
    } catch (err: any) {
      error.value = err.response?.data || {
        code: 'SAVE_ERROR',
        message: 'Failed to save template',
      };
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const deleteTemplate = async (id: string) => {
    loading.value = true;
    error.value = null;
    try {
      await emailApi.deleteTemplate(id);
      templates.value = templates.value.filter(t => t.id !== id);
    } catch (err: any) {
      error.value = err.response?.data || {
        code: 'DELETE_ERROR',
        message: 'Failed to delete template',
      };
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const duplicateTemplate = async (id: string): Promise<EmailTemplate> => {
    loading.value = true;
    error.value = null;
    try {
      const response = await emailApi.duplicateTemplate(id);
      templates.value.push(response.data);
      return response.data;
    } catch (err: any) {
      error.value = err.response?.data || {
        code: 'DUPLICATE_ERROR',
        message: 'Failed to duplicate template',
      };
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const publishTemplate = async (id: string) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await emailApi.publishTemplate(id);
      const index = templates.value.findIndex(t => t.id === id);
      if (index !== -1) {
        templates.value[index] = response.data;
      }
      return response.data;
    } catch (err: any) {
      error.value = err.response?.data || {
        code: 'PUBLISH_ERROR',
        message: 'Failed to publish template',
      };
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const fetchLayouts = async (): Promise<EmailLayout[]> => {
    loading.value = true;
    error.value = null;
    try {
      const response = await emailApi.getLayouts();
      layouts.value = response.data;
      return response.data;
    } catch (err: any) {
      error.value = err.response?.data || {
        code: 'FETCH_ERROR',
        message: 'Failed to fetch layouts',
      };
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const saveLayout = async (
    layout: Partial<EmailLayout>,
    isEditing: boolean
  ): Promise<EmailLayout> => {
    loading.value = true;
    error.value = null;
    try {
      const url = isEditing ? `/api/email/layouts/${layout.id}` : '/api/email/layouts';
      const method = isEditing ? 'put' : 'post';

      const response = isEditing
        ? await emailApi.updateLayout(layout.id!, layout)
        : await emailApi.createLayout(
            layout as Omit<EmailLayout, 'id' | 'createdAt' | 'updatedAt'>
          );
      if (isEditing) {
        const index = layouts.value.findIndex(l => l.id === layout.id);
        if (index !== -1) {
          layouts.value[index] = response.data;
        }
      } else {
        layouts.value.push(response.data);
      }
      return response.data;
    } catch (err: any) {
      error.value = err.response?.data || { code: 'SAVE_ERROR', message: 'Failed to save layout' };
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const deleteLayout = async (id: string) => {
    loading.value = true;
    error.value = null;
    try {
      await emailApi.deleteLayout(id);
      layouts.value = layouts.value.filter(l => l.id !== id);
    } catch (err: any) {
      error.value = err.response?.data || {
        code: 'DELETE_ERROR',
        message: 'Failed to delete layout',
      };
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const fetchAccounts = async () => {
    loading.value = true;
    error.value = null;
    try {
      const response = await emailApi.getAccounts();
      accounts.value = response.data;
    } catch (err: any) {
      error.value = err.response?.data || {
        code: 'FETCH_ERROR',
        message: 'Failed to fetch accounts',
      };
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const saveAccount = async (
    account: Partial<EmailSendingAccount>,
    isEditing: boolean
  ): Promise<EmailSendingAccount> => {
    loading.value = true;
    error.value = null;
    try {
      const url = isEditing ? `/api/email/accounts/${account.id}` : '/api/email/accounts';
      const method = isEditing ? 'put' : 'post';

      const response = isEditing
        ? await emailApi.updateAccount(account.id!, account)
        : await emailApi.createAccount(
            account as Omit<EmailSendingAccount, 'id' | 'createdAt' | 'updatedAt' | 'monthlyUsed'>
          );
      if (isEditing) {
        const index = accounts.value.findIndex(a => a.id === account.id);
        if (index !== -1) {
          accounts.value[index] = response.data;
        }
      } else {
        accounts.value.push(response.data);
      }
      return response.data;
    } catch (err: any) {
      error.value = err.response?.data || { code: 'SAVE_ERROR', message: 'Failed to save account' };
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const deleteAccount = async (id: string) => {
    loading.value = true;
    error.value = null;
    try {
      await emailApi.deleteAccount(id);
      accounts.value = accounts.value.filter(a => a.id !== id);
    } catch (err: any) {
      error.value = err.response?.data || {
        code: 'DELETE_ERROR',
        message: 'Failed to delete account',
      };
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const previewTemplate = async (
    templateKey: string,
    locale: string,
    sampleData?: Record<string, any>
  ): Promise<EmailTemplatePreview> => {
    loading.value = true;
    error.value = null;
    try {
      const response = await emailApi.previewTemplate(templateKey, locale, sampleData);
      return response.data;
    } catch (err: any) {
      error.value = err.response?.data || {
        code: 'PREVIEW_ERROR',
        message: 'Failed to preview template',
      };
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const sendTestEmail = async (
    templateKey: string,
    locale: string,
    toAddress: string,
    sampleData?: Record<string, any>
  ) => {
    loading.value = true;
    error.value = null;
    try {
      await emailApi.sendTestEmail({
        templateKey,
        locale,
        toAddress,
        sampleData,
      });
    } catch (err: any) {
      error.value = err.response?.data || {
        code: 'TEST_SEND_ERROR',
        message: 'Failed to send test email',
      };
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const fetchAnalytics = async (startDate?: string, endDate?: string) => {
    loading.value = true;
    error.value = null;
    try {
      const params = new URLSearchParams();
      if (startDate) params.append('startDate', startDate);
      if (endDate) params.append('endDate', endDate);

      const response = await emailApi.getAnalytics(params);
      analytics.value = response.data;
    } catch (err: any) {
      error.value = err.response?.data || {
        code: 'FETCH_ERROR',
        message: 'Failed to fetch analytics',
      };
      throw error.value;
    } finally {
      loading.value = false;
    }
  };

  const clearError = () => {
    error.value = null;
  };

  return {
    // State
    templates,
    layouts,
    accounts,
    analytics,
    loading,
    error,

    // Actions
    fetchTemplates,
    fetchTemplate,
    saveTemplate,
    deleteTemplate,
    duplicateTemplate,
    publishTemplate,
    fetchLayouts,
    saveLayout,
    deleteLayout,
    fetchAccounts,
    saveAccount,
    deleteAccount,
    previewTemplate,
    sendTestEmail,
    fetchAnalytics,
    clearError,
  };
});
