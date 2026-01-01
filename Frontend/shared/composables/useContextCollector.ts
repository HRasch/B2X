import { ref, computed } from 'vue'

export interface BrowserContext {
  userAgent: string
  language: string
  platform: string
  screenResolution: string
  viewportSize: string
  timezone: string
}

export interface ApplicationContext {
  version: string
  environment: string
  buildNumber: string
  userRole?: string
  tenantId: string
}

export interface SessionContext {
  startTime: Date
  duration: number
  pageViews: number
  lastAction: string
}

export interface PerformanceContext {
  loadTime: number
  domReady: number
  firstPaint: number
  largestContentfulPaint: number
}

export interface UrlContext {
  current: string
  referrer: string
}

export interface ErrorContext {
  message: string
  stack: string
  timestamp: Date
}

export interface CollectedContext {
  browser: BrowserContext
  application: ApplicationContext
  session: SessionContext
  performance: PerformanceContext
  url: UrlContext
  errors: ErrorContext[]
}

/**
 * Composable for collecting anonymized context data for feedback
 */
export function useContextCollector() {
  const sessionStartTime = ref(new Date())
  const pageViews = ref(1)
  const lastAction = ref('page_load')
  const errors = ref<ErrorContext[]>([])

  // Track page views
  const trackPageView = (action: string = 'navigation') => {
    pageViews.value++
    lastAction.value = action
  }

  // Track errors
  const trackError = (error: Error) => {
    errors.value.push({
      message: error.message,
      stack: error.stack || 'No stack trace',
      timestamp: new Date()
    })

    // Keep only last 5 errors to avoid memory issues
    if (errors.value.length > 5) {
      errors.value = errors.value.slice(-5)
    }
  }

  // Collect browser context
  const collectBrowserContext = (): BrowserContext => {
    return {
      userAgent: navigator.userAgent,
      language: navigator.language,
      platform: navigator.platform,
      screenResolution: `${screen.width}x${screen.height}`,
      viewportSize: `${window.innerWidth}x${window.innerHeight}`,
      timezone: Intl.DateTimeFormat().resolvedOptions().timeZone
    }
  }

  // Collect application context
  const collectApplicationContext = (): ApplicationContext => {
    // Get version from environment or meta tag
    const version = import.meta.env.VITE_APP_VERSION || '1.0.0'
    const environment = import.meta.env.MODE || 'production'
    const buildNumber = import.meta.env.VITE_BUILD_NUMBER || 'dev'

    // Get tenant ID from localStorage or environment
    const tenantId = localStorage.getItem('tenantId') ||
                    import.meta.env.VITE_DEFAULT_TENANT_ID ||
                    'default'

    // Get user role if available (for admin context)
    const userRole = localStorage.getItem('userRole') || undefined

    return {
      version,
      environment,
      buildNumber,
      userRole,
      tenantId
    }
  }

  // Collect session context
  const collectSessionContext = (): SessionContext => {
    const duration = Date.now() - sessionStartTime.value.getTime()

    return {
      startTime: sessionStartTime.value,
      duration,
      pageViews: pageViews.value,
      lastAction: lastAction.value
    }
  }

  // Collect performance context
  const collectPerformanceContext = (): PerformanceContext => {
    const navigation = performance.getEntriesByType('navigation')[0] as PerformanceNavigationTiming
    const paint = performance.getEntriesByType('paint')

    const loadTime = navigation ? navigation.loadEventEnd - navigation.fetchStart : 0
    const domReady = navigation ? navigation.domContentLoadedEventEnd - navigation.fetchStart : 0

    const firstPaint = paint.find(entry => entry.name === 'first-paint')?.startTime || 0
    const largestContentfulPaint = performance.getEntriesByType('largest-contentful-paint')[0]?.startTime || 0

    return {
      loadTime: Math.round(loadTime),
      domReady: Math.round(domReady),
      firstPaint: Math.round(firstPaint),
      largestContentfulPaint: Math.round(largestContentfulPaint)
    }
  }

  // Collect URL context
  const collectUrlContext = (): UrlContext => {
    return {
      current: window.location.href,
      referrer: document.referrer
    }
  }

  // Collect all context data
  const collect = async (): Promise<CollectedContext> => {
    return {
      browser: collectBrowserContext(),
      application: collectApplicationContext(),
      session: collectSessionContext(),
      performance: collectPerformanceContext(),
      url: collectUrlContext(),
      errors: [...errors.value]
    }
  }

  // Collect context asynchronously (for background collection)
  const collectAsync = async () => {
    try {
      const context = await collect()
      // Could send to analytics or store locally
      console.log('Context collected:', context)
    } catch (error) {
      console.warn('Failed to collect context:', error)
    }
  }

  // Setup global error tracking
  const setupErrorTracking = () => {
    window.addEventListener('error', (event) => {
      trackError(new Error(`${event.message} at ${event.filename}:${event.lineno}:${event.colno}`))
    })

    window.addEventListener('unhandledrejection', (event) => {
      trackError(new Error(`Unhandled promise rejection: ${event.reason}`))
    })
  }

  // Initialize
  setupErrorTracking()

  return {
    collect,
    collectAsync,
    trackPageView,
    trackError
  }
}</content>
<parameter name="filePath">/Users/holger/Documents/Projekte/B2Connect/frontend/shared/composables/useContextCollector.ts