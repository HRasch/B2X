/**
 * Debug Router Guard Plugin
 *
 * This Nuxt plugin controls access to debug-related routes and ensures
 * proper debug mode initialization.
 */

export default defineNuxtPlugin(() => {
  // Add debug guard to router
  const router = useRouter()

  // Debug route guard
  const debugGuard = (to, from) => {
    // Check if route requires debug mode
    if (to.meta?.requiresDebug) {
      const isEnabled = typeof localStorage !== 'undefined' &&
                       localStorage.getItem('debug-enabled') === 'true'

      if (!isEnabled) {
        console.warn('🔒 Debug route access denied. Debug mode not enabled.')
        // Redirect to home or show error
        return '/'
      }
    }

    // Check if route requires active debug session
    if (to.meta?.requiresActiveSession) {
      // This would need access to the debug store
      // For now, just check if we're in debug mode
      const isEnabled = typeof localStorage !== 'undefined' &&
                       localStorage.getItem('debug-enabled') === 'true'

      if (!isEnabled) {
        console.warn('🔒 Debug route requires active session.')
        // Redirect to debug panel or show error
        return '/debug'
      }
    }

    // Initialize debug context if entering debug routes
    if (to.path.startsWith('/debug')) {
      // Debug initialization is handled by the debug-init plugin
      console.log('🔧 Entering debug route:', to.path)
    }
  }

  // Add the guard to router
  router.beforeEach(debugGuard)

  // Provide debug guard utilities
  return {
    provide: {
      debugGuard: {
        checkAccess: (route) => {
          const result = debugGuard(route, null)
          return result === undefined // undefined means access granted
        }
      }
    }
  }
})

/**
 * Debug Route Meta Configuration
 *
 * Use these meta properties in your route definitions:
 *
 * {
 *   path: '/debug',
 *   component: DebugPanel,
 *   meta: {
 *     requiresDebug: true,        // Requires debug mode to be enabled
 *     requiresActiveSession: true, // Requires active debug session
 *     debugOnly: true             // Only accessible in debug mode
 *   }
 * }
 */

export const debugRouteMeta = {
  requiresDebug: true,
  requiresActiveSession: false,
  debugOnly: true
}