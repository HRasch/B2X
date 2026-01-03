import { afterEach, vi } from 'vitest';

// Only cleanup if available (for Vue Test Utils)
afterEach(() => {
  // No cleanup needed for simple tests
  localStorage.clear();
});

// Mock localStorage
const localStorageMock = (() => {
  let store: Record<string, string> = {};

  return {
    getItem: (key: string) => store[key] || null,
    setItem: (key: string, value: string) => {
      store[key] = value.toString();
    },
    removeItem: (key: string) => {
      delete store[key];
    },
    clear: () => {
      store = {};
    },
  };
})();

Object.defineProperty(window, 'localStorage', {
  value: localStorageMock,
});

// Mock window.matchMedia
Object.defineProperty(window, 'matchMedia', {
  writable: true,
  value: vi.fn().mockImplementation(query => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: vi.fn(),
    removeListener: vi.fn(),
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn(),
  })),
});

// Suppress Vue warnings about async component failures in test environment
// These are expected when widget components cannot be loaded dynamically
const originalWarn = console.warn;
console.warn = function (...args: unknown[]) {
  // Filter out Vue's unhandled async component loader warnings
  const firstArg = args[0] as string | undefined;
  if (
    typeof firstArg === 'string' &&
    firstArg.includes('[Vue warn]: Unhandled error during execution of async component loader')
  ) {
    return;
  }
  // Allow other warnings through
  originalWarn.apply(console, args);
};

// Mock dynamic widget imports to suppress load errors in test environment
// The actual widget components are not needed for unit tests
vi.mock('./src/components/widgets/HeroBanner.vue', () => ({
  default: {
    name: 'HeroBanner',
    template: '<div class="hero-banner">Hero Banner Mock</div>',
  },
}));

vi.mock('./src/components/widgets/ProductGrid.vue', () => ({
  default: {
    name: 'ProductGrid',
    template: '<div class="product-grid">Product Grid Mock</div>',
  },
}));

vi.mock('./src/components/widgets/Testimonials.vue', () => ({
  default: {
    name: 'Testimonials',
    template: '<div class="testimonials">Testimonials Mock</div>',
  },
}));

vi.mock('./src/components/widgets/NewsletterSignup.vue', () => ({
  default: {
    name: 'NewsletterSignup',
    template: '<div class="newsletter-signup">Newsletter Signup Mock</div>',
  },
}));

vi.mock('./src/components/cms/WidgetNotFound.vue', () => ({
  default: {
    name: 'WidgetNotFound',
    template: '<div class="widget-not-found">Widget not found</div>',
  },
}));
