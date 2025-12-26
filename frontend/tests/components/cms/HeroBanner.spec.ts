import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import HeroBanner from '@/components/widgets/HeroBanner.vue'
import { createRouter, createMemoryHistory } from 'vue-router'

describe('HeroBanner.vue', () => {
  const createRouter_ = () => createRouter({
    history: createMemoryHistory(),
    routes: [{ path: '/', component: { template: '<div/>' } }]
  })

  it('should render hero banner with title', () => {
    const wrapper = mount(HeroBanner, {
      props: {
        settings: {
          title: 'Welcome',
          subtitle: 'Test',
          backgroundImage: '/image.jpg',
          height: 500,
          textColor: '#ffffff'
        },
        widgetId: 'hero-1'
      },
      global: {
        plugins: [createRouter_()]
      }
    })

    expect(wrapper.text()).toContain('Welcome')
    expect(wrapper.text()).toContain('Test')
  })

  it('should apply background image style', () => {
    const wrapper = mount(HeroBanner, {
      props: {
        settings: {
          title: 'Test',
          backgroundImage: '/test-image.jpg',
          height: 600
        },
        widgetId: 'hero-1'
      },
      global: {
        plugins: [createRouter_()]
      }
    })

    const element = wrapper.find('.hero-banner')
    const style = element.attributes('style')

    expect(style).toContain('/test-image.jpg')
    expect(style).toContain('600px')
  })

  it('should render CTA button when ctaLink is provided', () => {
    const wrapper = mount(HeroBanner, {
      props: {
        settings: {
          title: 'Test',
          ctaLink: '/products',
          ctaText: 'Shop Now'
        },
        widgetId: 'hero-1'
      },
      global: {
        plugins: [createRouter_()]
      }
    })

    const button = wrapper.find('button')
    expect(button.exists()).toBe(true)
    expect(button.text()).toContain('Shop Now')
  })

  it('should not render button without ctaLink', () => {
    const wrapper = mount(HeroBanner, {
      props: {
        settings: {
          title: 'Test'
        },
        widgetId: 'hero-1'
      },
      global: {
        plugins: [createRouter_()]
      }
    })

    const button = wrapper.find('button')
    expect(button.exists()).toBe(false)
  })

  it('should use default CTA text when not specified', () => {
    const wrapper = mount(HeroBanner, {
      props: {
        settings: {
          title: 'Test',
          ctaLink: '/products'
        },
        widgetId: 'hero-1'
      },
      global: {
        plugins: [createRouter_()]
      }
    })

    const button = wrapper.find('button')
    expect(button.text()).toContain('Shop Now')
  })
})
