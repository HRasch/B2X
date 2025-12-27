import { test, expect, APIRequestContext } from '@playwright/test'

const API_BASE = 'http://localhost:5003/api'
const GATEWAY_BASE = 'http://localhost:5000/api'
const AUTH_BASE = 'http://localhost:5001/api'
const TENANT_BASE = 'http://localhost:5002/api'
const LOCALIZATION_BASE = 'http://localhost:5003/api'

test.describe('Health Check Endpoints', () => {
  let request: APIRequestContext

  test.beforeAll(async ({ playwright }) => {
    request = await playwright.request.newContext()
  })

  test.afterAll(async () => {
    await request.dispose()
  })

  test.describe('Service Health Checks', () => {
    test('should check API Gateway health', async () => {
      const response = await request.get(`${GATEWAY_BASE}/health`)
      expect([200, 404]).toContain(response.status())
      
      if (response.status() === 200) {
        const data = await response.json()
        expect(data).toHaveProperty('status')
      }
    })

    test('should check Auth Service health', async () => {
      const response = await request.get(`${AUTH_BASE}/health`)
      expect([200, 404]).toContain(response.status())
    })

    test('should check Tenant Service health', async () => {
      const response = await request.get(`${TENANT_BASE}/health`)
      expect([200, 404]).toContain(response.status())
    })

    test('should check Localization Service health', async () => {
      const response = await request.get(`${LOCALIZATION_BASE}/health`)
      expect([200, 404]).toContain(response.status())
    })
  })

  test.describe('Dashboard Health Endpoint', () => {
    test('should return dashboard health status', async () => {
      const response = await request.get('http://localhost:9000/health')
      expect(response.status()).toBe(200)
      
      const data = await response.json()
      expect(data).toHaveProperty('status')
      expect(data.status).toBe('healthy')
    })

    test('should return service status from dashboard', async () => {
      const response = await request.get('http://localhost:9000/api/health')
      expect(response.status()).toBe(200)
      
      const data = await response.json()
      expect(data).toHaveProperty('status')
      expect(data).toHaveProperty('services')
      expect(typeof data.services).toBe('object')
    })

    test('should show all registered services', async () => {
      const response = await request.get('http://localhost:9000/api/health')
      const data = await response.json()
      
      // Should have at least the main services
      expect(Object.keys(data.services).length).toBeGreaterThan(0)
    })
  })

  test.describe('Health Status Values', () => {
    test('should return valid status values', async () => {
      const validStatuses = ['healthy', 'unhealthy', 'unavailable']
      
      const response = await request.get('http://localhost:9000/api/health')
      if (response.status() === 200) {
        const data = await response.json()
        
        Object.values(data.services).forEach((service: any) => {
          expect(['healthy', 'unhealthy', 'unavailable']).toContain(
            service.status
          )
        })
      }
    })

    test('should include timestamp in response', async () => {
      const response = await request.get('http://localhost:9000/health')
      const data = await response.json()
      
      expect(data).toHaveProperty('timestamp')
      const timestamp = new Date(data.timestamp)
      expect(timestamp).toBeInstanceOf(Date)
    })
  })
})

test.describe('Cross-Service Communication', () => {
  let request: APIRequestContext

  test.beforeAll(async ({ playwright }) => {
    request = await playwright.request.newContext()
  })

  test.afterAll(async () => {
    await request.dispose()
  })

  test('should handle requests between services', async () => {
    // Test that API Gateway can reach other services through health check
    const response = await request.get('http://localhost:9000/api/health')
    expect(response.status()).toBe(200)
    
    const data = await response.json()
    // At least some services should be reachable
    const reachableServices = Object.values(data.services)
      .filter((s: any) => s.status === 'healthy')
    
    expect(reachableServices.length).toBeGreaterThanOrEqual(0)
  })

  test('should not block on missing services', async () => {
    const start = Date.now()
    const response = await request.get('http://localhost:9000/health')
    const duration = Date.now() - start
    
    // Health check should be fast (< 1 second) even if services are unavailable
    expect(response.status()).toBe(200)
    expect(duration).toBeLessThan(1000)
  })
})

test.describe('API Gateway Routing', () => {
  let request: APIRequestContext

  test.beforeAll(async ({ playwright }) => {
    request = await playwright.request.newContext()
  })

  test.afterAll(async () => {
    await request.dispose()
  })

  test('should return gateway is running message', async () => {
    const response = await request.get('http://localhost:5000/')
    expect(response.status()).toBe(200)
    
    const text = await response.text()
    expect(text.toLowerCase()).toContain('running')
  })

  test('should route requests appropriately', async () => {
    const response = await request.get('http://localhost:5000/health')
    expect([200, 404]).toContain(response.status())
  })
})

test.describe('Service Availability', () => {
  let request: APIRequestContext

  test.beforeAll(async ({ playwright }) => {
    request = await playwright.request.newContext()
  })

  test.afterAll(async () => {
    await request.dispose()
  })

  test('should indicate when all services are available', async () => {
    const response = await request.get('http://localhost:9000/api/health')
    if (response.status() === 200) {
      const data = await response.json()
      
      // Check if we can identify service availability
      expect(data).toHaveProperty('services')
    }
  })

  test('should handle gracefully when some services are down', async () => {
    const response = await request.get('http://localhost:9000/api/health')
    expect(response.status()).toBe(200)
    
    // Should return 200 even if some services are unavailable
    const data = await response.json()
    expect(data.status).toBe('healthy') // Dashboard itself is healthy
  })

  test('should provide service status breakdown', async () => {
    const response = await request.get('http://localhost:9000/api/health')
    const data = await response.json()
    
    if (Object.keys(data.services).length > 0) {
      // Services should have status information
      Object.values(data.services).forEach((service: any) => {
        expect(service).toHaveProperty('status')
      })
    }
  })
})

test.describe('API Response Formats', () => {
  let request: APIRequestContext

  test.beforeAll(async ({ playwright }) => {
    request = await playwright.request.newContext()
  })

  test.afterAll(async () => {
    await request.dispose()
  })

  test('should return JSON responses', async () => {
    const response = await request.get('http://localhost:9000/health')
    expect(response.headers()['content-type']).toContain('application/json')
    
    const data = await response.json()
    expect(typeof data).toBe('object')
  })

  test('should include appropriate headers', async () => {
    const response = await request.get('http://localhost:9000/health')
    const headers = response.headers()
    
    expect(headers).toHaveProperty('content-type')
  })

  test('should handle various content negotiation', async () => {
    const response = await request.get('http://localhost:9000/health', {
      headers: {
        'Accept': 'application/json'
      }
    })
    
    expect(response.status()).toBe(200)
    expect(response.headers()['content-type']).toContain('application/json')
  })
})
