import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import Testimonials from '@/components/widgets/Testimonials.vue'

describe('Testimonials.vue', () => {
  beforeEach(() => {
    vi.useFakeTimers()
  })

  const createTestimonials = () => [
    {
      text: 'Great product!',
      author: 'John Doe',
      title: 'Verified Buyer'
    },
    {
      text: 'Excellent service',
      author: 'Jane Smith',
      title: 'Verified Buyer'
    },
    {
      text: 'Highly recommended',
      author: 'Bob Johnson',
      title: 'Verified Buyer'
    }
  ]

  it('should render testimonials carousel', () => {
    const wrapper = mount(Testimonials, {
      props: {
        settings: {
          title: 'What Customers Say',
          testimonials: createTestimonials(),
          autoplay: false
        },
        widgetId: 'testimonials-1'
      }
    })

    expect(wrapper.text()).toContain('What Customers Say')
  })

  it('should display first testimonial by default', () => {
    const testimonials = createTestimonials()
    const wrapper = mount(Testimonials, {
      props: {
        settings: {
          testimonials,
          autoplay: false
        },
        widgetId: 'testimonials-1'
      }
    })

    expect(wrapper.text()).toContain('Great product!')
    expect(wrapper.text()).toContain('John Doe')
  })

  it('should navigate to next testimonial', async () => {
    const testimonials = createTestimonials()
    const wrapper = mount(Testimonials, {
      props: {
        settings: {
          testimonials,
          autoplay: false
        },
        widgetId: 'testimonials-1'
      }
    })

    const nextButton = wrapper.findAll('button').find(b => b.text().includes('Next'))
    await nextButton?.trigger('click')

    expect(wrapper.text()).toContain('Excellent service')
    expect(wrapper.text()).toContain('Jane Smith')
  })

  it('should navigate to previous testimonial', async () => {
    const testimonials = createTestimonials()
    const wrapper = mount(Testimonials, {
      props: {
        settings: {
          testimonials,
          autoplay: false
        },
        widgetId: 'testimonials-1'
      }
    })

    const nextButton = wrapper.findAll('button').find(b => b.text().includes('Next'))
    const prevButton = wrapper.findAll('button').find(b => b.text().includes('Previous'))

    await nextButton?.trigger('click')
    await prevButton?.trigger('click')

    expect(wrapper.text()).toContain('Great product!')
  })

  it('should cycle through testimonials', async () => {
    const testimonials = createTestimonials()
    const wrapper = mount(Testimonials, {
      props: {
        settings: {
          testimonials,
          autoplay: false
        },
        widgetId: 'testimonials-1'
      }
    })

    const nextButton = wrapper.findAll('button').find(b => b.text().includes('Next'))

    // Go to second
    await nextButton?.trigger('click')
    expect(wrapper.text()).toContain('Jane Smith')

    // Go to third
    await nextButton?.trigger('click')
    expect(wrapper.text()).toContain('Bob Johnson')

    // Cycle back to first
    await nextButton?.trigger('click')
    expect(wrapper.text()).toContain('John Doe')
  })

  it('should show testimonial counter', () => {
    const testimonials = createTestimonials()
    const wrapper = mount(Testimonials, {
      props: {
        settings: {
          testimonials,
          autoplay: false
        },
        widgetId: 'testimonials-1'
      }
    })

    expect(wrapper.text()).toContain('1 / 3')
  })

  it('should hide navigation with single testimonial', () => {
    const wrapper = mount(Testimonials, {
      props: {
        settings: {
          testimonials: [createTestimonials()[0]],
          autoplay: false
        },
        widgetId: 'testimonials-1'
      }
    })

    const buttons = wrapper.findAll('button')
    expect(buttons.length).toBe(0)
  })
})
