import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount } from '@vue/test-utils';
import { createI18n } from 'vue-i18n';
import en from '../../../src/locales/en.json';
import de from '../../../src/locales/de.json';
import fr from '../../../src/locales/fr.json';
import es from '../../../src/locales/es.json';
import itLocale from '../../../src/locales/it.json';
import pt from '../../../src/locales/pt.json';
import nl from '../../../src/locales/nl.json';
import pl from '../../../src/locales/pl.json';
import Testimonials from '@/components/widgets/Testimonials.vue';

describe('Testimonials.vue', () => {
  let i18n: ReturnType<typeof createI18n>;

  beforeEach(() => {
    vi.useFakeTimers();

    // Create i18n instance for tests
    i18n = createI18n({
      legacy: false,
      locale: 'en',
      fallbackLocale: 'en',
      globalInjection: true,
      messages: {
        en,
        de,
        fr,
        es,
        it: itLocale,
        pt,
        nl,
        pl,
      },
    });
  });

  const createTestimonials = () => [
    {
      text: 'Great product!',
      author: 'John Doe',
      title: 'Verified Buyer',
    },
    {
      text: 'Excellent service',
      author: 'Jane Smith',
      title: 'Verified Buyer',
    },
    {
      text: 'Highly recommended',
      author: 'Bob Johnson',
      title: 'Verified Buyer',
    },
  ];

  it('should render testimonials carousel', () => {
    const wrapper = mount(Testimonials, {
      props: {
        settings: {
          title: 'What Customers Say',
          testimonials: createTestimonials(),
          autoplay: false,
        },
        widgetId: 'testimonials-1',
      },
      global: {
        plugins: [i18n],
      },
    });

    expect(wrapper.text()).toContain('What Customers Say');
  });

  it('should display first testimonial by default', () => {
    const testimonials = createTestimonials();
    const wrapper = mount(Testimonials, {
      props: {
        settings: {
          testimonials,
          autoplay: false,
        },
        widgetId: 'testimonials-1',
      },
      global: {
        plugins: [i18n],
      },
    });

    expect(wrapper.text()).toContain('Great product!');
    expect(wrapper.text()).toContain('John Doe');
  });

  it('should navigate to next testimonial', async () => {
    const testimonials = createTestimonials();
    const wrapper = mount(Testimonials, {
      props: {
        settings: {
          testimonials,
          autoplay: false,
        },
        widgetId: 'testimonials-1',
      },
      global: {
        plugins: [i18n],
      },
    });

    const nextButton = wrapper.findAll('button').find(b => b.text().includes('Next'));
    await nextButton?.trigger('click');

    expect(wrapper.text()).toContain('Excellent service');
    expect(wrapper.text()).toContain('Jane Smith');
  });

  it('should navigate to previous testimonial', async () => {
    const testimonials = createTestimonials();
    const wrapper = mount(Testimonials, {
      props: {
        settings: {
          testimonials,
          autoplay: false,
        },
        widgetId: 'testimonials-1',
      },
      global: {
        plugins: [i18n],
      },
    });

    const nextButton = wrapper.findAll('button').find(b => b.text().includes('Next'));
    const prevButton = wrapper.findAll('button').find(b => b.text().includes('Previous'));

    await nextButton?.trigger('click');
    await prevButton?.trigger('click');

    expect(wrapper.text()).toContain('Great product!');
  });

  it('should cycle through testimonials', async () => {
    const testimonials = createTestimonials();
    const wrapper = mount(Testimonials, {
      props: {
        settings: {
          testimonials,
          autoplay: false,
        },
        widgetId: 'testimonials-1',
      },
      global: {
        plugins: [i18n],
      },
    });

    const nextButton = wrapper.findAll('button').find(b => b.text().includes('Next'));

    // Go to second
    await nextButton?.trigger('click');
    expect(wrapper.text()).toContain('Jane Smith');

    // Go to third
    await nextButton?.trigger('click');
    expect(wrapper.text()).toContain('Bob Johnson');

    // Cycle back to first
    await nextButton?.trigger('click');
    expect(wrapper.text()).toContain('John Doe');
  });

  it('should show testimonial counter', () => {
    const testimonials = createTestimonials();
    const wrapper = mount(Testimonials, {
      props: {
        settings: {
          testimonials,
          autoplay: false,
        },
        widgetId: 'testimonials-1',
      },
      global: {
        plugins: [i18n],
      },
    });

    expect(wrapper.text()).toContain('1 / 3');
  });

  it('should hide navigation with single testimonial', () => {
    const wrapper = mount(Testimonials, {
      props: {
        settings: {
          testimonials: [createTestimonials()[0]],
          autoplay: false,
        },
        widgetId: 'testimonials-1',
      },
      global: {
        plugins: [i18n],
      },
    });

    const buttons = wrapper.findAll('button');
    expect(buttons.length).toBe(0);
  });
});
