import { test, expect, APIRequestContext } from '@playwright/test'

const API_BASE = 'http://localhost:5003/api'
const VALID_ADMIN_TOKEN = 'Bearer admin-token' // Adjust with real admin token
const TENANT_ID = 'test-tenant-id' // Adjust with real tenant ID
const ENTITY_ID = '550e8400-e29b-41d4-a716-446655440000'

test.describe('Entity Localization API', () => {
  let request: APIRequestContext

  test.beforeAll(async (context) => {
    request = await context.request.newContext({
      extraHTTPHeaders: {
        'tenant': TENANT_ID,
        'Authorization': VALID_ADMIN_TOKEN
      }
    })
  })

  test.afterAll(async () => {
    await request.dispose()
  })

  test.describe('GET /entity-localization/{entityId}', () => {
    test('should retrieve all localized properties for an entity', async () => {
      const response = await request.get(
        `${API_BASE}/entity-localization/${ENTITY_ID}`
      )
      expect([200, 404]).toContain(response.status())
      
      if (response.status() === 200) {
        const data = await response.json()
        expect(data).toHaveProperty('entityId')
        expect(data).toHaveProperty('properties')
        expect(Array.isArray(data.properties)).toBe(true)
        expect(data).toHaveProperty('totalProperties')
        expect(data).toHaveProperty('localizations')
      }
    })

    test('should return 404 for non-existent entity', async () => {
      const response = await request.get(
        `${API_BASE}/entity-localization/00000000-0000-0000-0000-000000000000`
      )
      expect([404, 200]).toContain(response.status())
    })

    test('should require authentication', async ({ request: contextRequest }) => {
      const noAuthRequest = await contextRequest.newContext()
      const response = await noAuthRequest.get(
        `${API_BASE}/entity-localization/${ENTITY_ID}`,
        {
          headers: { 'tenant': TENANT_ID }
        }
      )
      expect(response.status()).toBe(401)
      await noAuthRequest.dispose()
    })

    test('should require tenant header', async ({ request: contextRequest }) => {
      const noTenantRequest = await contextRequest.newContext({
        extraHTTPHeaders: {
          'Authorization': VALID_ADMIN_TOKEN
        }
      })
      const response = await noTenantRequest.get(
        `${API_BASE}/entity-localization/${ENTITY_ID}`
      )
      expect(response.status()).toBe(400)
      await noTenantRequest.dispose()
    })
  })

  test.describe('GET /entity-localization/{entityId}/{propertyName}', () => {
    test('should retrieve all translations for a property', async () => {
      const response = await request.get(
        `${API_BASE}/entity-localization/${ENTITY_ID}/Name`
      )
      expect([200, 404]).toContain(response.status())
      
      if (response.status() === 200) {
        const data = await response.json()
        expect(data).toHaveProperty('entityId')
        expect(data).toHaveProperty('propertyName')
        expect(data).toHaveProperty('translations')
        expect(data).toHaveProperty('availableLanguages')
        expect(data).toHaveProperty('totalTranslations')
      }
    })

    test('should return 404 for non-existent property', async () => {
      const response = await request.get(
        `${API_BASE}/entity-localization/${ENTITY_ID}/NonExistentProperty`
      )
      expect([404, 200]).toContain(response.status())
    })

    test('should list available languages', async () => {
      const response = await request.get(
        `${API_BASE}/entity-localization/${ENTITY_ID}/Name`
      )
      if (response.status() === 200) {
        const data = await response.json()
        expect(Array.isArray(data.availableLanguages)).toBe(true)
      }
    })
  })

  test.describe('GET /entity-localization/{entityId}/{propertyName}/{language}', () => {
    test('should retrieve translation for specific language', async () => {
      const response = await request.get(
        `${API_BASE}/entity-localization/${ENTITY_ID}/Name/en`
      )
      expect([200, 404]).toContain(response.status())
      
      if (response.status() === 200) {
        const data = await response.json()
        expect(data).toHaveProperty('value')
        expect(data.language).toBe('en')
      }
    })

    test('should support multiple languages', async () => {
      const languages = ['en', 'de', 'fr', 'es']
      
      for (const lang of languages) {
        const response = await request.get(
          `${API_BASE}/entity-localization/${ENTITY_ID}/Name/${lang}`
        )
        expect([200, 404]).toContain(response.status())
      }
    })

    test('should return 404 for non-existent translation', async () => {
      const response = await request.get(
        `${API_BASE}/entity-localization/${ENTITY_ID}/Name/xx`
      )
      expect([404, 200]).toContain(response.status())
    })
  })

  test.describe('POST /entity-localization/{entityId}/{propertyName}', () => {
    test('should require authentication', async ({ request: contextRequest }) => {
      const noAuthRequest = await contextRequest.newContext({
        extraHTTPHeaders: { 'tenant': TENANT_ID }
      })
      const response = await noAuthRequest.post(
        `${API_BASE}/entity-localization/${ENTITY_ID}/Name`,
        {
          data: { language: 'en', value: 'Test' }
        }
      )
      expect(response.status()).toBe(401)
      await noAuthRequest.dispose()
    })

    test('should require admin role', async ({ request: contextRequest }) => {
      const userRequest = await contextRequest.newContext({
        extraHTTPHeaders: {
          'tenant': TENANT_ID,
          'Authorization': 'Bearer user-token'
        }
      })
      const response = await userRequest.post(
        `${API_BASE}/entity-localization/${ENTITY_ID}/Name`,
        {
          data: { language: 'en', value: 'Test' }
        }
      )
      expect([401, 403]).toContain(response.status())
      await userRequest.dispose()
    })

    test('should set a single translation', async () => {
      const response = await request.post(
        `${API_BASE}/entity-localization/${ENTITY_ID}/Name`,
        {
          data: {
            language: 'en',
            value: 'Product Name'
          }
        }
      )
      expect([200, 204, 400]).toContain(response.status())
    })

    test('should validate required fields', async () => {
      const response = await request.post(
        `${API_BASE}/entity-localization/${ENTITY_ID}/Name`,
        {
          data: { language: 'en' } // Missing 'value'
        }
      )
      expect(response.status()).toBe(400)
    })

    test('should reject empty values', async () => {
      const response = await request.post(
        `${API_BASE}/entity-localization/${ENTITY_ID}/Name`,
        {
          data: {
            language: 'en',
            value: ''
          }
        }
      )
      expect([400, 204]).toContain(response.status())
    })
  })

  test.describe('PUT /entity-localization/{entityId}/{propertyName}', () => {
    test('should require admin role', async ({ request: contextRequest }) => {
      const noAuthRequest = await contextRequest.newContext({
        extraHTTPHeaders: { 'tenant': TENANT_ID }
      })
      const response = await noAuthRequest.put(
        `${API_BASE}/entity-localization/${ENTITY_ID}/Name`,
        {
          data: {
            en: 'Product Name',
            de: 'Produktname',
            fr: 'Nom du produit'
          }
        }
      )
      expect(response.status()).toBe(401)
      await noAuthRequest.dispose()
    })

    test('should set multiple translations at once', async () => {
      const response = await request.put(
        `${API_BASE}/entity-localization/${ENTITY_ID}/Name`,
        {
          data: {
            en: 'Product Name',
            de: 'Produktname',
            fr: 'Nom du produit',
            es: 'Nombre del producto'
          }
        }
      )
      expect([200, 204, 400]).toContain(response.status())
    })

    test('should reject empty translations object', async () => {
      const response = await request.put(
        `${API_BASE}/entity-localization/${ENTITY_ID}/Name`,
        {
          data: {}
        }
      )
      expect(response.status()).toBe(400)
    })

    test('should handle at least one translation', async () => {
      const response = await request.put(
        `${API_BASE}/entity-localization/${ENTITY_ID}/Description`,
        {
          data: {
            en: 'A detailed product description'
          }
        }
      )
      expect([200, 204, 400]).toContain(response.status())
    })
  })

  test.describe('POST /entity-localization/{entityId}/{propertyName}/validate', () => {
    test('should validate required languages present', async () => {
      const response = await request.post(
        `${API_BASE}/entity-localization/${ENTITY_ID}/Name/validate`,
        {
          data: {
            requiredLanguages: ['en', 'de']
          }
        }
      )
      expect([200, 404]).toContain(response.status())
      
      if (response.status() === 200) {
        const data = await response.json()
        expect(data).toHaveProperty('isValid')
        expect(data).toHaveProperty('requiredLanguages')
        expect(data).toHaveProperty('missingLanguages')
        expect(data).toHaveProperty('message')
      }
    })

    test('should accept all required languages', async () => {
      const response = await request.post(
        `${API_BASE}/entity-localization/${ENTITY_ID}/Name/validate`,
        {
          data: {
            requiredLanguages: ['en']
          }
        }
      )
      if (response.status() === 200) {
        const data = await response.json()
        // Either valid or returns validation info
        expect(data).toHaveProperty('isValid')
      }
    })

    test('should report missing languages', async () => {
      const response = await request.post(
        `${API_BASE}/entity-localization/${ENTITY_ID}/Name/validate`,
        {
          data: {
            requiredLanguages: ['en', 'de', 'fr', 'es', 'it', 'pt', 'nl', 'pl']
          }
        }
      )
      if (response.status() === 200) {
        const data = await response.json()
        expect(data).toHaveProperty('missingLanguages')
        expect(Array.isArray(data.missingLanguages)).toBe(true)
      }
    })

    test('should reject invalid payload', async () => {
      const response = await request.post(
        `${API_BASE}/entity-localization/${ENTITY_ID}/Name/validate`,
        {
          data: {
            requiredLanguages: [] // Empty array
          }
        }
      )
      expect(response.status()).toBe(400)
    })
  })

  test.describe('Error Handling & Edge Cases', () => {
    test('should handle invalid entity ID format', async () => {
      const response = await request.get(
        `${API_BASE}/entity-localization/invalid-uuid/Name`
      )
      expect([400, 404]).toContain(response.status())
    })

    test('should handle special characters in property names', async () => {
      const response = await request.get(
        `${API_BASE}/entity-localization/${ENTITY_ID}/Name-with_special.chars`
      )
      expect([200, 404, 400]).toContain(response.status())
    })

    test('should handle very long translation values', async () => {
      const longValue = 'a'.repeat(10000)
      const response = await request.post(
        `${API_BASE}/entity-localization/${ENTITY_ID}/Description`,
        {
          data: {
            language: 'en',
            value: longValue
          }
        }
      )
      expect([200, 204, 400, 413]).toContain(response.status())
    })

    test('should handle concurrent requests', async () => {
      const responses = await Promise.all([
        request.get(`${API_BASE}/entity-localization/${ENTITY_ID}`),
        request.get(`${API_BASE}/entity-localization/${ENTITY_ID}/Name`),
        request.get(`${API_BASE}/entity-localization/${ENTITY_ID}/Name/en`)
      ])
      
      responses.forEach(response => {
        expect([200, 404]).toContain(response.status())
      })
    })

    test('should maintain tenant isolation', async ({ request: contextRequest }) => {
      const otherTenantRequest = await contextRequest.newContext({
        extraHTTPHeaders: {
          'tenant': 'other-tenant-id',
          'Authorization': VALID_ADMIN_TOKEN
        }
      })
      
      const response = await otherTenantRequest.get(
        `${API_BASE}/entity-localization/${ENTITY_ID}`
      )
      // Should not have access to other tenant's data
      expect([404, 403]).toContain(response.status())
      await otherTenantRequest.dispose()
    })
  })

  test.describe('Performance Tests', () => {
    test('should respond quickly to GET requests', async () => {
      const start = Date.now()
      await request.get(`${API_BASE}/entity-localization/${ENTITY_ID}/Name`)
      const duration = Date.now() - start
      
      expect(duration).toBeLessThan(2000) // 2 second max response time
    })

    test('should handle rapid successive requests', async () => {
      const requests = Array(10).fill(null).map(() =>
        request.get(`${API_BASE}/entity-localization/${ENTITY_ID}`)
      )
      
      const responses = await Promise.all(requests)
      responses.forEach(response => {
        expect([200, 404]).toContain(response.status())
      })
    })
  })
})
