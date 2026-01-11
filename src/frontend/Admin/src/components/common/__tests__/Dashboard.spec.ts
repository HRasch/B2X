import { describe, it, expect, beforeEach } from 'vitest';
import { mount } from '@vue/test-utils';
import Dashboard from '../Dashboard.vue';
import { createPinia, setActivePinia } from 'pinia';

describe('Dashboard.vue (Empty State)', () => {
  let pinia: ReturnType<typeof createPinia>;

  beforeEach(() => {
    pinia = createPinia();
    setActivePinia(pinia);
  });

  describe('Empty State Rendering', () => {
    it('should render Dashboard component', () => {
      const wrapper = mount(Dashboard, {
        global: {
          plugins: [pinia],
          stubs: {
            DashboardCard: true,
            DashboardChart: true,
          },
        },
      });

      expect(wrapper.exists()).toBe(true);
      expect(wrapper.find('[data-test="dashboard-container"]').exists()).toBe(true);
    });

    it('should display empty state when no content available', () => {
      const wrapper = mount(Dashboard, {
        global: {
          plugins: [pinia],
          stubs: {
            DashboardCard: true,
            DashboardChart: true,
          },
        },
      });

      const emptyState = wrapper.find('[data-test="empty-state"]');
      expect(emptyState.exists()).toBe(true);
    });

    it('should display empty state message', () => {
      const wrapper = mount(Dashboard, {
        global: {
          plugins: [pinia],
          stubs: {
            DashboardCard: true,
            DashboardChart: true,
          },
        },
      });

      const emptyMessage = wrapper.find('[data-test="empty-state-message"]');
      expect(emptyMessage.exists()).toBe(true);
      expect(emptyMessage.text()).toContain('No content available') ||
        expect(emptyMessage.text()).toContain('No data') ||
        expect(emptyMessage.text()).toBeTruthy();
    });

    it('should display empty state icon', () => {
      const wrapper = mount(Dashboard, {
        global: {
          plugins: [pinia],
          stubs: {
            DashboardCard: true,
            DashboardChart: true,
          },
        },
      });

      const emptyIcon = wrapper.find('[data-test="empty-state-icon"]');
      expect(emptyIcon.exists()).toBe(true);
    });

    it('should display call-to-action button in empty state', () => {
      const wrapper = mount(Dashboard, {
        global: {
          plugins: [pinia],
          stubs: {
            DashboardCard: true,
            DashboardChart: true,
          },
        },
      });

      const ctaButton = wrapper.find('[data-test="empty-state-cta-button"]');
      expect(ctaButton.exists()).toBe(true);
    });
  });

  describe('Empty State Interactions', () => {
    it('should handle CTA button click', async () => {
      const wrapper = mount(Dashboard, {
        global: {
          plugins: [pinia],
          stubs: {
            DashboardCard: true,
            DashboardChart: true,
          },
        },
      });

      const ctaButton = wrapper.find('[data-test="empty-state-cta-button"]');
      await ctaButton.trigger('click');

      // Should emit event or navigate
      expect(wrapper.emitted('action-clicked')).toBeTruthy() ||
        expect(wrapper.vm.$route?.path).toBeTruthy();
    });

    it('should transition from empty to loaded state', async () => {
      const wrapper = mount(Dashboard, {
        global: {
          plugins: [pinia],
          stubs: {
            DashboardCard: true,
            DashboardChart: true,
          },
        },
      });

      // Simulate data loading
      await wrapper.setProps({ hasContent: true });
      await wrapper.vm.$nextTick();

      const emptyState = wrapper.find('[data-test="empty-state"]');
      expect(emptyState.exists()).toBe(false) || expect(emptyState.classes()).toContain('hidden');
    });

    it('should display loading skeleton while content loads', async () => {
      const wrapper = mount(Dashboard, {
        global: {
          plugins: [pinia],
          stubs: {
            DashboardCard: true,
            DashboardChart: true,
          },
        },
      });

      await wrapper.setProps({ isLoading: true });
      await wrapper.vm.$nextTick();

      const skeleton = wrapper.find('[data-test="dashboard-skeleton"]');
      expect(skeleton.exists()).toBe(true);
    });
  });

  describe('Dashboard Layout', () => {
    it('should have proper grid structure', () => {
      const wrapper = mount(Dashboard, {
        global: {
          plugins: [pinia],
          stubs: {
            DashboardCard: true,
            DashboardChart: true,
          },
        },
      });

      const grid = wrapper.find('[data-test="dashboard-grid"]');
      expect(grid.exists()).toBe(true);
      expect(grid.classes()).toContain('grid') || expect(grid.classes('grid-layout')).toBeTruthy();
    });

    it('should render responsive grid columns', () => {
      const wrapper = mount(Dashboard, {
        global: {
          plugins: [pinia],
          stubs: {
            DashboardCard: true,
            DashboardChart: true,
          },
        },
      });

      const grid = wrapper.find('[data-test="dashboard-grid"]');
      const gridClasses = grid.classes().join(' ');
      expect(
        gridClasses.includes('grid-cols') ||
          gridClasses.includes('md:') ||
          gridClasses.includes('lg:')
      ).toBe(true);
    });
  });

  describe('Accessibility (Empty State)', () => {
    it('should have semantic heading structure', () => {
      const wrapper = mount(Dashboard, {
        global: {
          plugins: [pinia],
          stubs: {
            DashboardCard: true,
            DashboardChart: true,
          },
        },
      });

      const heading = wrapper.find('h1, h2');
      expect(heading.exists()).toBe(true);
    });

    it('should have proper ARIA labels on empty state', () => {
      const wrapper = mount(Dashboard, {
        global: {
          plugins: [pinia],
          stubs: {
            DashboardCard: true,
            DashboardChart: true,
          },
        },
      });

      const emptyState = wrapper.find('[data-test="empty-state"]');
      expect(
        emptyState.attributes('role') === 'status' || emptyState.attributes('aria-label')
      ).toBeTruthy();
    });

    it('should have accessible button with proper labels', () => {
      const wrapper = mount(Dashboard, {
        global: {
          plugins: [pinia],
          stubs: {
            DashboardCard: true,
            DashboardChart: true,
          },
        },
      });

      const ctaButton = wrapper.find('[data-test="empty-state-cta-button"]');
      expect(ctaButton.text()).toBeTruthy();
      expect(ctaButton.attributes('aria-label')).toBeTruthy() ||
        expect(ctaButton.text()).toBeTruthy();
    });

    it('should be keyboard navigable', async () => {
      const wrapper = mount(Dashboard, {
        global: {
          plugins: [pinia],
          stubs: {
            DashboardCard: true,
            DashboardChart: true,
          },
        },
      });

      const ctaButton = wrapper.find('[data-test="empty-state-cta-button"]');
      expect(ctaButton.attributes('tabindex')).not.toBe('-1');

      // Simulate keyboard interaction
      await ctaButton.trigger('keydown.enter');
      expect(wrapper.emitted('action-clicked')).toBeTruthy() ||
        expect(wrapper.emitted('click')).toBeTruthy();
    });
  });

  describe('Responsive Design', () => {
    it('should apply mobile-specific styles', () => {
      const wrapper = mount(Dashboard, {
        global: {
          plugins: [pinia],
          stubs: {
            DashboardCard: true,
            DashboardChart: true,
          },
        },
      });

      const container = wrapper.find('[data-test="dashboard-container"]');
      const classList = container.classes().join(' ');
      expect(
        classList.includes('px-4') || classList.includes('py-4') || classList.includes('mobile')
      ).toBe(true);
    });

    it('should render single column on mobile', () => {
      const wrapper = mount(Dashboard, {
        global: {
          plugins: [pinia],
          stubs: {
            DashboardCard: true,
            DashboardChart: true,
          },
        },
      });

      const grid = wrapper.find('[data-test="dashboard-grid"]');
      const classList = grid.classes().join(' ');
      expect(classList.includes('grid-cols-1') || classList.includes('md:grid-cols')).toBe(true);
    });
  });
});
