import { afterEach, vi } from 'vitest';
import { ref } from 'vue';

// Mock Nuxt composables globally
vi.stubGlobal('useCookie', (_name: string) => {
  const cookieValue = ref<string | null>(null);
  return {
    value: cookieValue,
  };
});

// Make i18n available globally for tests
vi.mock('vue-i18n', async () => {
  const actual = await vi.importActual('vue-i18n');
  return {
    ...actual,
    useI18n: () => ({
      t: (key: string) => {
        // Return actual translations for common keys used in tests
        const translations: Record<string, string> = {
          'customerTypeSelection.title': 'How are you registering?',
          'customerTypeSelection.subtitle': 'Choose the account type that best fits your needs',
          'customerTypeSelection.private.title': 'Private Customer',
          'customerTypeSelection.private.description': 'Individual shopper',
          'customerTypeSelection.business.title': 'Business Customer',
          'customerTypeSelection.business.description': 'Company or organization',
          'customerTypeSelection.actions.continue': 'Continue',
          'customerTypeSelection.login.link': 'Sign in here',
          'customerTypeSelection.private.ariaLabel': 'Register as a private customer',
          'customerTypeSelection.business.ariaLabel': 'Register as a business customer',
          'validation.emailRequired': 'E-Mail-Adresse ist erforderlich',
          'validation.emailInvalid': 'E-Mail-Adresse ist ungültig',
          'validation.phoneInvalid': 'Telefonnummer ist ungültig',
          'validation.firstNameRequired': 'Vorname ist erforderlich',
          'validation.lastNameRequired': 'Nachname ist erforderlich',
          'validation.addressRequired': 'Adresse ist erforderlich',
          'validation.cityRequired': 'Stadt ist erforderlich',
          'validation.postalCodeRequired': 'Postleitzahl ist erforderlich',
          'navigation.previous': '← Previous',
          'navigation.next': 'Next →',
          'navigation.home': 'Home',
          'navigation.products': 'Products',
          'widgets.previous': 'Previous',
          'widgets.next': 'Next',
          'product.sku': 'SKU:',
          'product.priceOverview': 'Price Overview',
          'product.priceIncludesVat':
            'All prices include VAT in accordance with PAngV (Price Indication Ordinance)',
          'product.addToCart': 'Add to Cart',
          'product.share': 'Share:',
          'product.specifications': 'Specifications',
          'product.customerReviews': 'Customer Reviews',
          'product.verified': '✓ Verified',
          'products.title': 'B2X Store',
          'products.subtitle': 'Find the best products for your business',
          'products.search.label': 'Search products',
          'products.sort.label': 'Sort by',
          'products.sort.nameAsc': 'Name (A-Z)',
          'products.sort.priceAsc': 'Price (Low to High)',
          'products.sort.priceDesc': 'Price (High to Low)',
          'products.sort.ratingDesc': 'Rating (High to Low)',
          'products.filters.title': 'Filters',
          'products.filters.category': 'Category',
          'products.filters.priceRange': 'Price Range',
          'products.filters.priceRangeComingSoon': '€0 - €5000 (coming soon)',
          'products.filters.inStockOnly': 'In Stock Only',
          'products.results.foundFor': 'Found for:',
          'products.loading': 'Loading products...',
          'products.noProducts.title': 'No products found',
          'products.noProducts.message': 'Try adjusting your filters or search query',
          'products.noProducts.clearFilters': 'Clear Filters',
          'cart.title': 'Shopping Cart',
          'cart.empty.message': 'Your cart is empty',
          'cart.empty.continueShopping': 'Continue Shopping',
          'cart.summary.title': 'Order Summary',
          'cart.summary.subtotal': 'Subtotal',
          'cart.summary.tax': 'Tax ({rate}%)',
          'cart.summary.shipping': 'Shipping:',
          'cart.summary.total': 'Total Price (incl. VAT):',
          'cart.shipping.title': 'Shipping',
          'cart.shipping.destination': 'Destination Country',
          'cart.shipping.selectCountry': 'Please select...',
          'cart.shipping.countries.de': 'Germany',
          'cart.shipping.countries.at': 'Austria',
          'cart.shipping.countries.be': 'Belgium',
          'cart.shipping.countries.fr': 'France',
          'cart.shipping.countries.nl': 'Netherlands',
          'cart.shipping.countries.ch': 'Switzerland',
          'cart.shipping.countries.gb': 'Great Britain',
          'cart.actions.checkout': 'Go to Checkout',
          'cart.actions.continueShopping': 'Continue Shopping',
          'dashboard.title': 'Dashboard',
          'dashboard.sections.statistics.title': 'Statistics',
          'dashboard.sections.statistics.description':
            'Your dashboard statistics will appear here.',
          'dashboard.sections.activity.title': 'Recent Activity',
          'dashboard.sections.activity.description': 'Recent activities will be displayed here.',
          'dashboard.sections.actions.title': 'Quick Actions',
          'dashboard.sections.actions.manageTenants': 'Manage Tenants',
          'dashboard.sections.actions.settings': 'Account Settings',
          'home.title': 'Welcome to B2X',
          'home.subtitle': 'A modern multitenant SaaS platform for seamless business connectivity',
          'home.getStarted': 'Get Started',
          'home.dashboard.title': 'Your Dashboard',
          'home.dashboard.message': 'You are logged in. Navigate to your dashboard to get started.',
          'home.dashboard.button': 'Go to Dashboard',
          'home.features.multitenant.title': 'Multitenant Architecture',
          'home.features.multitenant.description':
            'Secure data isolation for multiple organizations on a single platform.',
          'home.features.microservices.title': 'Microservices',
          'home.features.microservices.description':
            'Scalable microservices designed for high performance and reliability.',
          'home.features.realtime.title': 'Real-time Updates',
          'home.features.realtime.description':
            'Event-driven architecture with Wolverine for instant data synchronization.',
          'notFound.title': 'Page Not Found',
          'notFound.message': 'The page you are looking for does not exist.',
          'notFound.goHome': 'Go Back Home',
          'product.skuLabel': 'SKU:',
          'store.title': 'B2X Shop',
          'store.loading': 'Loading products...',
          'store.error': 'Error loading products:',
          'store.retry': 'Try Again',
          'store.noProducts.title': 'No products found.',
          'store.noProducts.suggestion': 'Try a different search term or use fewer filters.',
          'store.paginationInfo': 'Page {current} of {total}',
          'tenants.title': 'Tenants Management',
          'tenants.actions.new': '+ New Tenant',
          'tenants.actions.edit': 'Edit',
          'tenants.actions.delete': 'Delete',
          'tenants.loading': 'Loading tenants...',
          'tenants.noTenants': 'No tenants found. Create your first tenant to get started.',
          'tenants.form.title': 'Create New Tenant',
          'tenants.form.labels.name': 'Name',
          'tenants.form.labels.slug': 'Slug',
          'tenants.form.labels.description': 'Description',
          'tenants.form.actions.create': 'Create',
          'tenants.form.actions.cancel': 'Cancel',
          'legal.checkout.header.title': 'Checkout',
          'legal.checkout.header.breadcrumb.shop': 'Shop',
          'legal.checkout.header.breadcrumb.cart': 'Cart',
          'legal.checkout.header.breadcrumb.checkout': 'Checkout',
          'legal.checkout.orderSummary.title': 'Order Summary',
          'legal.checkout.orderSummary.netto': 'Net Amount',
          'legal.checkout.orderSummary.vat': 'VAT (19%)',
          'legal.checkout.orderSummary.shipping': 'Shipping',
          'legal.checkout.orderSummary.total': 'Total',
          'legal.checkout.orderSummary.trustBadges.ssl': 'SSL Encrypted',
          'legal.checkout.orderSummary.trustBadges.returns': '14-Day Returns',
          'legal.checkout.orderSummary.trustBadges.insured': 'Insured Shipping',
          'legal.checkout.steps.shippingAddress': 'Shipping Address',
          'legal.checkout.form.description': 'Please enter your shipping address',
          'legal.checkout.form.labels.firstName': 'First Name',
          'legal.checkout.form.labels.lastName': 'Last Name',
          'legal.checkout.form.labels.streetAddress': 'Street Address',
          'legal.checkout.form.labels.postalCode': 'Postal Code',
          'legal.checkout.form.labels.city': 'City',
          'legal.checkout.form.labels.country': 'Country',
          'legal.checkout.form.countries.germany': 'Germany',
          'legal.checkout.form.countries.austria': 'Austria',
          'legal.checkout.form.countries.belgium': 'Belgium',
          'legal.checkout.form.countries.france': 'France',
          'legal.checkout.form.countries.netherlands': 'Netherlands',
          'legal.checkout.buttons.backToCart': 'Back to Cart',
          'legal.checkout.buttons.continueToShipping': 'Continue to Shipping',
        };
        return translations[key] || key; // Return translation or key as fallback
      },
      locale: 'en',
    }),
  };
});

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
vi.mock('./components/widgets/HeroBanner.vue', () => ({
  default: {
    name: 'HeroBanner',
    template: '<div class="hero-banner">Hero Banner Mock</div>',
  },
}));

vi.mock('./components/widgets/ProductGrid.vue', () => ({
  default: {
    name: 'ProductGrid',
    template: '<div class="product-grid">Product Grid Mock</div>',
  },
}));

vi.mock('./components/widgets/Testimonials.vue', () => ({
  default: {
    name: 'Testimonials',
    template: '<div class="testimonials">Testimonials Mock</div>',
  },
}));

vi.mock('./components/widgets/NewsletterSignup.vue', () => ({
  default: {
    name: 'NewsletterSignup',
    template: '<div class="newsletter-signup">Newsletter Signup Mock</div>',
  },
}));

vi.mock('./components/cms/WidgetNotFound.vue', () => ({
  default: {
    name: 'WidgetNotFound',
    template: '<div class="widget-not-found">Widget not found</div>',
  },
}));
