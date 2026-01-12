import { mount } from '@vue/test-utils';
import { describe, it, expect, vi } from 'vitest';
import { createI18n } from 'vue-i18n';
import EmailTemplatesList from '@/views/email/EmailTemplatesList.vue';

// Mock the email store used inside the component
vi.mock('@/stores/email', () => {
  return {
    useEmailStore: () => ({
      fetchTemplates: vi.fn().mockResolvedValue([]),
      duplicateTemplate: vi.fn().mockResolvedValue(undefined),
      deleteTemplate: vi.fn().mockResolvedValue(undefined),
    }),
  };
});

const i18n = createI18n({
  legacy: false,
  locale: 'en',
  messages: {
    en: {
      email: {
        templates: {
          title: 'Email Templates',
          subtitle: 'Manage your email templates',
          create: 'Create',
          noTemplates: 'No templates found',
          search: 'Search templates',
          allLocales: 'All locales',
          english: 'English',
          german: 'German',
          french: 'French',
          active: 'Active',
          draft: 'Draft',
          archived: 'Archived',
          updated: 'Updated',
        },
      },
      ui: {
        previous: 'Previous',
        next: 'Next',
        edit: 'Edit',
        delete: 'Delete',
        view: 'View',
        filter: 'Filter',
      },
    },
  },
});

describe('EmailTemplatesList.vue', () => {
  it('renders header title and create button', async () => {
    const wrapper = mount(EmailTemplatesList, { global: { plugins: [i18n] } });
    // wait for mounted async calls
    await wrapper.vm.$nextTick();

    expect(wrapper.text()).toContain('Email Templates');
    expect(wrapper.find('button').text()).toContain('Create');
  });
});
