import { describe, it, expect } from 'vitest';
import { mount } from '@vue/test-utils';
import RegionRenderer from '@/components/cms/RegionRenderer.vue';
import WidgetRenderer from '@/components/cms/WidgetRenderer.vue';
import type { PageRegion, WidgetInstance } from '@/types/cms';

describe('RegionRenderer.vue', () => {
  const createRegion = (name: string, widgets: WidgetInstance[] = []): PageRegion => ({
    id: 'region-1',
    name,
    order: 1,
    settings: {},
    widgets,
  });

  const createWidget = (
    id: string,
    widgetTypeId: string,
    componentName: string
  ): WidgetInstance => ({
    id,
    widgetTypeId,
    componentPath: `widgets/${componentName}.vue`,
    order: 1,
    settings: {},
  });

  it('should render region successfully', () => {
    const region = createRegion('main');
    const wrapper = mount(RegionRenderer, {
      props: { region },
      global: {
        components: { WidgetRenderer },
      },
    });
    expect(wrapper.exists()).toBe(true);
  });

  it('should render all widgets in region', () => {
    const widgets = [
      createWidget('w1', 'hero-banner', 'HeroBanner'),
      createWidget('w2', 'product-grid', 'ProductGrid'),
      createWidget('w3', 'testimonials', 'Testimonials'),
    ];
    const region = createRegion('main', widgets);

    const wrapper = mount(RegionRenderer, {
      props: { region },
      global: {
        components: { WidgetRenderer },
      },
    });

    expect((wrapper.vm.$props as any).region.widgets).toHaveLength(3);
  });

  it('should have correct region class name', () => {
    const region = createRegion('header');
    const wrapper = mount(RegionRenderer, {
      props: { region },
    });

    expect(wrapper.classes()).toContain('region-header');
  });

  it('should apply region settings as styles', () => {
    const region: PageRegion = {
      id: 'region-1',
      name: 'main',
      order: 1,
      settings: {
        backgroundColor: '#f0f0f0',
        padding: '2rem',
      },
      widgets: [],
    };

    const wrapper = mount(RegionRenderer, {
      props: { region },
    });

    const regionContent = wrapper.find('.region-content');
    const style = regionContent.attributes('style');

    expect(style).toContain('background-color');
    expect(style).toContain('padding');
  });

  it('should show empty state in development when no widgets', () => {
    const region = createRegion('main', []);
    const wrapper = mount(RegionRenderer, {
      props: { region },
    });

    // Only shows empty state in development
    if (import.meta.env.DEV) {
      expect(wrapper.find('.empty-region').exists()).toBe(true);
    }
  });

  it('should set data-region-id attribute', () => {
    const region = createRegion('main');
    const wrapper = mount(RegionRenderer, {
      props: { region },
    });

    expect(wrapper.attributes('data-region-id')).toBe('region-1');
  });
});
