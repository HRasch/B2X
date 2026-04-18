import { vi, beforeAll, afterAll } from 'vitest';
import { h, getCurrentInstance } from 'vue';
import { config } from '@vue/test-utils';
import PageHeader from '@/components/layout/PageHeader.vue';
import CardContainer from '@/components/layout/CardContainer.vue';
import FormSection from '@/components/layout/FormSection.vue';
import FormRow from '@/components/layout/FormRow.vue';
import FormGroup from '@/components/layout/FormGroup.vue';

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

// Mock sessionStorage
Object.defineProperty(window, 'sessionStorage', {
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

// Configure Vue Test Utils
config.global.stubs = {
  teleport: true,
  Transition: false,
};

// Provide simple global mocks for vue-i18n and router used in components
config.global.mocks = {
  $t: (key: string, values?: Record<string, unknown>) => {
    if (!key) return '';
    const mappings: Record<string, string> = {
      'dashboard.title': 'Dashboard',
      'dashboard.welcomeBack': 'Welcome Back',
      'email.templates.title': 'Email Templates',
      'email.templates.subtitle': 'Manage system email templates',
      'email.templates.create': 'Create',
      'email.templates.allLocales': 'All locales',
      'email.templates.english': 'English',
      'email.templates.german': 'German',
      'email.templates.french': 'French',
      'email.templates.search': 'Search templates...',
      'email.templates.noTemplates': 'No templates found',
      'ui.filter': 'Filter',
      'ui.previous': 'Previous',
      'ui.next': 'Next',
      'ui.view': 'View',
      'ui.edit': 'Edit',
      'ui.delete': 'Delete',
      'users.placeholders.search': 'Search users...',
    };

    if (mappings[key]) {
      if (values && values.name) return `${mappings[key]} ${values.name}`;
      return mappings[key];
    }

    if (values && (values.page || values.total)) {
      return `Page ${values.page} of ${values.total}`;
    }

    const parts = key.split('.');
    return parts[parts.length - 1].replace(/[-_]/g, ' ').replace(/(^|\s)\S/g, s => s.toUpperCase());
  },
  $d: (val: unknown) => val,
  $n: (val: unknown) => val,
  $router: {
    push: vi.fn(),
    replace: vi.fn(),
  },
  $route: {
    path: '/',
    params: {},
    query: {},
  },
};

// Register layout components globally so tests that mount views do not need to stub them
config.global.components = {
  PageHeader,
  CardContainer,
  FormSection,
  FormRow,
  FormGroup,
};

// Mock Monaco editor module used by CodeEditor component to avoid loading real Monaco in unit tests
vi.mock('@guolao/vue-monaco-editor', () => {
  const VueMonacoEditor = {
    name: 'VueMonacoEditor',
    props: ['modelValue', 'language', 'options', 'theme', 'height'],
    emits: ['update:modelValue', 'change'],
    setup(props, { slots }) {
      // Try to read parent's height prop (CodeEditor passes height to its own SFC styles)
      const inst = getCurrentInstance();
      const parentHeight = inst?.parent?.props?.height;
      const resolvedHeight = props.height || parentHeight || '400px';
      return () =>
        h(
          'div',
          { class: 'mock-monaco', style: { height: resolvedHeight } },
          slots.default && slots.default()
        );
    },
  };

  return {
    VueMonacoEditor,
    default: VueMonacoEditor,
    useMonaco: () => ({}),
  };
});

// Partially mock vue-router while preserving real exports like createRouter
vi.mock('vue-router', async importOriginal => {
  const actual = await importOriginal();
  const mockRouter = {
    push: vi.fn(),
    replace: vi.fn(),
  };

  const mockRoute = {
    path: '/',
    params: {},
    query: {},
  };

  return {
    ...actual,
    useRouter: () => mockRouter,
    useRoute: () => mockRoute,
    RouterLink: {
      name: 'RouterLink',
      props: ['to'],
      template: '<a><slot/></a>',
    },
    RouterView: {
      name: 'RouterView',
      template: '<div />',
    },
  };
});

// Mock window.scrollTo
Object.defineProperty(window, 'scrollTo', {
  value: vi.fn(),
});

// Suppress console warnings in tests
const originalError = console.error;
const originalWarn = console.warn;

beforeAll(() => {
  console.error = vi.fn((...args) => {
    if (
      typeof args[0] === 'string' &&
      (args[0].includes('Not implemented: HTMLFormElement.prototype.submit') ||
        args[0].includes('Not implemented: HTMLFormElement.prototype.reset') ||
        args[0].includes('Navigation'))
    ) {
      return;
    }
    originalError.call(console, ...args);
  });

  console.warn = vi.fn((...args) => {
    if (
      typeof args[0] === 'string' &&
      args[0].includes('Avoid app logic that relies on enumerating keys on a component instance')
    ) {
      return;
    }
    originalWarn.call(console, ...args);
  });
});

afterAll(() => {
  console.error = originalError;
  console.warn = originalWarn;
});
