import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount } from '@vue/test-utils';
import { createTestingPinia } from '@pinia/testing';
import CategoriesPage from '~/pages/categories.vue';

// Mock Nuxt composables
vi.mock('#app', () => ({
  useRoute: () => ({
    params: { slug: '' }
  }),
  useRouter: () => ({}),
  navigateTo: vi.fn(),
  definePageMeta: vi.fn(),
}));

describe('CategoriesPage', () => {
  let wrapper: any;

  beforeEach(() => {
    wrapper = mount(CategoriesPage, {
      global: {
        plugins: [createTestingPinia()],
        stubs: ['NuxtLink'],
      },
    });
  });

  it('renders the categories page', () => {
    expect(wrapper.exists()).toBe(true);
    expect(wrapper.find('.categories-page').exists()).toBe(true);
  });

  it('displays loading state initially', () => {
    expect(wrapper.find('.loading-spinner').exists()).toBe(true);
  });

  it('shows page title', () => {
    expect(wrapper.text()).toContain('Categories');
  });
});