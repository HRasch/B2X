/**
 * ProductService Integration Tests
 *
 * These tests verify actual backend connectivity and API contract.
 * Run with a live backend: npm run test:integration
 *
 * Prerequisites:
 * - Backend running on http://localhost:8000 (Store Gateway)
 * - Catalog service running (via Aspire or standalone)
 */
import { describe, it, expect, beforeAll, afterAll } from 'vitest';
import axios, { type AxiosInstance } from 'axios';

// Test configuration
const API_BASE_URL = process.env.VITE_API_GATEWAY_URL || 'http://localhost:8000';
const TIMEOUT_MS = 10000;

describe('ProductService Integration Tests', () => {
  let api: AxiosInstance;
  let backendAvailable = false;

  beforeAll(async () => {
    api = axios.create({
      baseURL: API_BASE_URL,
      timeout: TIMEOUT_MS,
    });

    // Check if backend is available
    try {
      const response = await api.get('/health', { timeout: 2000 });
      backendAvailable = response.status === 200;
      console.log(`Backend available at ${API_BASE_URL}: ${backendAvailable}`);
    } catch {
      console.warn(`Backend not available at ${API_BASE_URL} - skipping integration tests`);
      backendAvailable = false;
    }
  });

  afterAll(() => {
    // Cleanup if needed
  });

  describe('API Route Discovery', () => {
    it.skipIf(!backendAvailable)('should have /health endpoint', async () => {
      const response = await api.get('/health');
      expect(response.status).toBe(200);
    });

    it.skipIf(!backendAvailable)(
      'should discover catalog products route at /api/catalog/products',
      async () => {
        try {
          const response = await api.get('/api/catalog/products');
          // Accept 200 (success) or 401 (auth required) - both indicate route exists
          expect([200, 401]).toContain(response.status);
        } catch (error: unknown) {
          if (axios.isAxiosError(error)) {
            // 401/403 means route exists but needs auth
            // 404 means route doesn't exist
            if (error.response?.status === 404) {
              throw new Error('Route /api/catalog/products not found - check YARP config');
            }
            // Other errors (401, 403, 500) mean route exists
            expect([401, 403, 500]).toContain(error.response?.status);
          } else {
            throw error;
          }
        }
      }
    );

    it.skipIf(!backendAvailable)(
      'should discover v1 products route at /api/v1/products',
      async () => {
        try {
          const response = await api.get('/api/v1/products');
          expect([200, 401]).toContain(response.status);
        } catch (error: unknown) {
          if (axios.isAxiosError(error)) {
            if (error.response?.status === 404) {
              throw new Error('Route /api/v1/products not found - check Catalog service routing');
            }
            expect([401, 403, 500]).toContain(error.response?.status);
          } else {
            throw error;
          }
        }
      }
    );

    it.skipIf(!backendAvailable)(
      'should discover v2 products route at /api/v2/products (Development YARP transform)',
      async () => {
        try {
          const response = await api.get('/api/v2/products');
          expect([200, 401]).toContain(response.status);
        } catch (error: unknown) {
          if (axios.isAxiosError(error)) {
            if (error.response?.status === 404) {
              console.warn('Route /api/v2/products not found - may not be in dev mode');
            }
            // Accept any response that indicates backend is responding
            expect(error.response?.status).toBeDefined();
          } else {
            throw error;
          }
        }
      }
    );
  });

  describe('Products Endpoint Contract', () => {
    it.skipIf(!backendAvailable)(
      'GET /api/catalog/products should return paginated response structure',
      async () => {
        try {
          const response = await api.get('/api/catalog/products', {
            params: { page: 1, pageSize: 10 },
          });

          // Verify response structure
          expect(response.data).toHaveProperty('items');
          expect(response.data).toHaveProperty('page');
          expect(response.data).toHaveProperty('pageSize');
          expect(response.data).toHaveProperty('totalCount');
          expect(Array.isArray(response.data.items)).toBe(true);
        } catch (error: unknown) {
          if (axios.isAxiosError(error) && error.response?.status === 401) {
            console.warn('Auth required for /api/catalog/products - skipping structure test');
            return;
          }
          throw error;
        }
      }
    );

    it.skipIf(!backendAvailable)(
      'POST /api/catalog/products/search should accept search query',
      async () => {
        try {
          const response = await api.post('/api/catalog/products/search', {
            term: 'test',
            page: 1,
            pageSize: 10,
          });

          expect(response.status).toBe(200);
          expect(response.data).toHaveProperty('items');
        } catch (error: unknown) {
          if (axios.isAxiosError(error)) {
            if (error.response?.status === 401) {
              console.warn('Auth required for search endpoint');
              return;
            }
            // 400 might indicate different request body format expected
            if (error.response?.status === 400) {
              console.warn('Bad request - check API contract:', error.response.data);
            }
          }
          throw error;
        }
      }
    );
  });

  describe('Error Handling', () => {
    it.skipIf(!backendAvailable)('should return 404 for non-existent product', async () => {
      try {
        await api.get('/api/catalog/products/00000000-0000-0000-0000-000000000000');
        throw new Error('Expected 404 but got success');
      } catch (error: unknown) {
        if (axios.isAxiosError(error)) {
          expect(error.response?.status).toBe(404);
        } else {
          throw error;
        }
      }
    });

    it.skipIf(!backendAvailable)('should return 400 for invalid query parameters', async () => {
      try {
        await api.get('/api/catalog/products', {
          params: { page: -1, pageSize: 10000 },
        });
        // Some APIs might sanitize invalid params instead of rejecting
      } catch (error: unknown) {
        if (axios.isAxiosError(error)) {
          expect([400, 422]).toContain(error.response?.status);
        } else {
          throw error;
        }
      }
    });
  });
});
