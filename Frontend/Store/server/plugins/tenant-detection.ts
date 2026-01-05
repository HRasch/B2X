// server/plugins/tenant-detection.ts
export default defineNitroPlugin((nitroApp) => {
  nitroApp.hooks.hook('request', (event) => {
    const host = getHeader(event, 'host') || ''
    const url = getRequestURL(event)

    // Extract tenant from subdomain or path
    // Example: tenant1.store.b2connect.com or /tenant/tenant1/
    let tenantId = 'default'

    // Check for subdomain (tenant.store.b2connect.com)
    const subdomainMatch = host.match(/^([a-zA-Z0-9-]+)\.store\.b2connect\.com$/)
    if (subdomainMatch) {
      tenantId = subdomainMatch[1]
    } else {
      // Check for path-based tenant (/tenant/tenant1/)
      const pathMatch = url.pathname.match(/^\/tenant\/([a-zA-Z0-9-]+)(\/|$)/)
      if (pathMatch) {
        tenantId = pathMatch[1]
      }
    }

    // Store tenant in event context for use in pages/composables
    event.context.tenantId = tenantId
  })
})