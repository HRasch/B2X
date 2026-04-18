import { describe, it, expect } from 'vitest';
import { mount } from '@vue/test-utils';
import PageHeader from '@/components/layout/PageHeader.vue';
import CardContainer from '@/components/layout/CardContainer.vue';

describe('Layout components', () => {
  it('renders PageHeader title and subtitle and actions slot', () => {
    const wrapper = mount(PageHeader, {
      props: { title: 'Test Title', subtitle: 'A subtitle' },
      slots: { actions: '<button data-test="action">Action</button>' },
    });

    expect(wrapper.text()).toContain('Test Title');
    expect(wrapper.text()).toContain('A subtitle');
    expect(wrapper.find('[data-test="action"]').exists()).toBe(true);
  });

  it('renders CardContainer with header and body slot', () => {
    const wrapper = mount(CardContainer, {
      props: { title: 'Card' },
      slots: { default: '<div data-test="body">Body</div>' },
    });

    expect(wrapper.find('.card-container__title').text()).toBe('Card');
    expect(wrapper.find('[data-test="body"]').exists()).toBe(true);
  });
});
