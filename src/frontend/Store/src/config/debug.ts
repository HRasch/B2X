/**
 * Debug Configuration
 *
 * Centralized configuration for debug features including:
 * - Feature flags
 * - API endpoints
 * - UI settings
 * - Performance limits
 * - Environment-specific settings
 */

export const DEBUG_CONFIG = {
  // Feature Flags
  features: {
    enableUserActionRecording: true,
    enableErrorCapture: true,
    enableFeedbackWidget: true,
    enableDebugPanel: true,
    enableSignalR: true, // Enabled with backend SignalR implementation
    enableScreenshots: true,
    enableConsoleLogging: true,
    enableAnalytics: false,
    enableRemoteDebugging: false,
  },

  // API Configuration
  api: {
    baseUrl:
      process.env.NODE_ENV === 'production' ? '/api/debug' : 'http://localhost:8000/api/debug',
    timeout: 5000,
    retryAttempts: 3,
    retryDelay: 1000,
    batchSize: 50,
    flushInterval: 30000, // 30 seconds
  },

  // SignalR Configuration
  signalR: {
    hubUrl:
      process.env.NODE_ENV === 'production' ? '/debug-hub' : 'http://localhost:8000/debug-hub',
    reconnectAttempts: 5,
    reconnectInterval: 2000,
    keepAliveInterval: 15000,
    serverTimeout: 30000,
    handshakeTimeout: 15000,
    enableLogging: process.env.NODE_ENV === 'development',
  },

  // UI Configuration
  ui: {
    triggerButton: {
      position: 'bottom-right',
      size: 56,
      showIndicator: true,
      animationDuration: 300,
    },
    panel: {
      width: 320,
      maxHeight: '70vh',
      animationDuration: 300,
      zIndex: 1000,
    },
    feedbackWidget: {
      maxWidth: 500,
      maxHeight: '80vh',
      animationDuration: 300,
      zIndex: 1001,
    },
  },

  // Session Configuration
  session: {
    maxDuration: 8 * 60 * 60 * 1000, // 8 hours
    idleTimeout: 30 * 60 * 1000, // 30 minutes
    autoSaveInterval: 60000, // 1 minute
    maxConcurrentSessions: 1,
  },

  // Data Limits
  limits: {
    maxActionsPerSession: 1000,
    maxErrorsPerSession: 100,
    maxFeedbacksPerSession: 10,
    maxScreenshotSize: 2 * 1024 * 1024, // 2MB
    maxActionDataSize: 1024, // 1KB per action
    maxErrorDataSize: 2048, // 2KB per error
  },

  // Performance Settings
  performance: {
    throttleClicks: 100, // ms
    throttleScroll: 200, // ms
    throttleResize: 300, // ms
    debounceNavigation: 500, // ms
    maxCallStackDepth: 10,
  },

  // Privacy Settings
  privacy: {
    maskSensitiveData: true,
    excludeUrls: [/\/api\/auth/, /\/api\/payment/, /password/i, /token/i, /secret/i],
    allowedDomains: [window.location.hostname, 'localhost', '127.0.0.1'],
    requireConsent: false,
  },

  // Environment-specific Settings
  environments: {
    development: {
      enableConsoleLogging: true,
      enableAnalytics: false,
      autoEnable: false,
      verboseLogging: true,
    },
    staging: {
      enableConsoleLogging: true,
      enableAnalytics: true,
      autoEnable: false,
      verboseLogging: false,
    },
    production: {
      enableConsoleLogging: false,
      enableAnalytics: true,
      autoEnable: false,
      verboseLogging: false,
      privacy: {
        maskSensitiveData: true,
        requireConsent: true,
      },
    },
  },

  // Keyboard Shortcuts
  shortcuts: {
    toggleDebug: 'ctrl+shift+d',
    openPanel: 'ctrl+shift+p',
    takeScreenshot: 'ctrl+shift+s',
    clearData: 'ctrl+shift+c',
  },

  // Error Types
  errorTypes: {
    javascript: 'JavaScript Error',
    network: 'Network Error',
    console: 'Console Error',
    unhandled: 'Unhandled Promise Rejection',
    custom: 'Custom Error',
  },

  // Action Types
  actionTypes: {
    click: 'User Click',
    navigation: 'Page Navigation',
    formSubmit: 'Form Submission',
    apiCall: 'API Call',
    scroll: 'Page Scroll',
    resize: 'Window Resize',
    focus: 'Element Focus',
    blur: 'Element Blur',
    custom: 'Custom Action',
  },

  // Feedback Types
  feedbackTypes: {
    bugReport: 'Bug Report',
    featureRequest: 'Feature Request',
    generalFeedback: 'General Feedback',
    uiIssue: 'UI/UX Issue',
    performance: 'Performance Issue',
  },

  // Storage Keys
  storage: {
    enabled: 'debug-enabled',
    settings: 'debug-settings',
    session: 'debug-session',
    tenantId: 'debug-tenant-id',
    userId: 'debug-user-id',
  },

  // Analytics Configuration
  analytics: {
    trackPageViews: true,
    trackUserInteractions: true,
    trackErrors: true,
    trackPerformance: false,
    sampleRate: 1.0, // 100%
    batchSize: 20,
    flushInterval: 30000,
  },
} as const;

// Helper function to get environment-specific config
export function getEnvironmentConfig() {
  const env = process.env.NODE_ENV || 'development';
  return (
    DEBUG_CONFIG.environments[env as keyof typeof DEBUG_CONFIG.environments] ||
    DEBUG_CONFIG.environments.development
  );
}

// Helper function to check if feature is enabled
export function isFeatureEnabled(feature: keyof typeof DEBUG_CONFIG.features): boolean {
  const envConfig = getEnvironmentConfig();
  const envOverride = envConfig[feature as keyof typeof envConfig];

  if (typeof envOverride === 'boolean') {
    return envOverride;
  }

  return DEBUG_CONFIG.features[feature];
}

// Helper function to get config value with environment override
export function getConfigValue<T extends keyof typeof DEBUG_CONFIG>(
  section: T,
  key: keyof (typeof DEBUG_CONFIG)[T],
  defaultValue?: any
): any {
  const envConfig = getEnvironmentConfig();
  const envOverride = envConfig[key as keyof typeof envConfig];

  if (envOverride !== undefined) {
    return envOverride;
  }

  const sectionConfig = DEBUG_CONFIG[section];
  return (sectionConfig as any)[key] || defaultValue;
}

// Export merged config for current environment
export const CURRENT_CONFIG = {
  ...DEBUG_CONFIG,
  ...getEnvironmentConfig(),
};

export default DEBUG_CONFIG;
