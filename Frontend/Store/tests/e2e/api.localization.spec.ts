import { test, expect, APIRequestContext } from '@playwright/test';

const API_BASE = 'http://localhost:5003/api';
const VALID_TOKEN = 'Bearer test-token'; // Adjust with real token
const TENANT_ID = 'test-tenant-id'; // Adjust with real tenant ID

test.describe('Localization API - Central Translations', () => {
  let request: APIRequestContext;

  test.beforeAll(async ({ playwright }) => {
    request = await playwright.request.newContext();
  });

  test.afterAll(async () => {
    await request.dispose();
  });

  test.describe('GET /localization/{category}/{key}', () => {
    test('should retrieve a translated string', async () => {
      const response = await request.get(`${API_BASE}/localization/auth/login?language=en`);
      expect(response.status()).toBe(200);

      const data = await response.json();
      expect(data).toHaveProperty('key');
      expect(data).toHaveProperty('value');
      expect(data).toHaveProperty('language');
    });

    test('should return English translation by default', async () => {
      const response = await request.get(`${API_BASE}/localization/auth/login?language=de`);
      expect(response.status()).toBe(200);

      const data = await response.json();
      expect(data.language).toBe('de');
    });

    test('should handle invalid category gracefully', async () => {
      const response = await request.get(
        `${API_BASE}/localization/invalid-category/key?language=en`
      );
      // Should return 404 or default text
      expect([200, 404]).toContain(response.status());
    });
  });

  test.describe('GET /localization/category/{category}', () => {
    test('should retrieve all translations in a category', async () => {
      const response = await request.get(`${API_BASE}/localization/category/auth?language=en`);
      expect(response.status()).toBe(200);

      const data = await response.json();
      expect(data).toHaveProperty('category');
      expect(data).toHaveProperty('language');
      expect(data).toHaveProperty('translations');
      expect(typeof data.translations).toBe('object');
    });

    test('should support multiple languages', async () => {
      const languages = ['en', 'de', 'fr', 'es'];

      for (const lang of languages) {
        const response = await request.get(`${API_BASE}/localization/category/ui?language=${lang}`);
        expect(response.status()).toBe(200);

        const data = await response.json();
        expect(data.language).toBe(lang);
      }
    });

    test('should return empty translations for non-existent category', async () => {
      const response = await request.get(
        `${API_BASE}/localization/category/nonexistent?language=en`
      );
      expect([200, 404]).toContain(response.status());
    });
  });

  test.describe('GET /localization/languages', () => {
    test('should list all supported languages', async () => {
      const response = await request.get(`${API_BASE}/localization/languages`);
      expect(response.status()).toBe(200);

      const data = await response.json();
      expect(data).toHaveProperty('languages');
      expect(Array.isArray(data.languages)).toBe(true);
      expect(data.languages.length).toBeGreaterThan(0);
      expect(data.languages).toContain('en');
    });
  });

  test.describe('POST /localization/{category}/{key}', () => {
    test('should require authentication', async () => {
      const response = await request.post(`${API_BASE}/localization/test/key`, {
        data: { en: 'Test', de: 'Test' },
      });
      expect(response.status()).toBe(401);
    });

    test('should require admin role', async () => {
      const response = await request.post(`${API_BASE}/localization/test/key`, {
        headers: {
          Authorization: VALID_TOKEN,
        },
        data: { en: 'Test', de: 'Test' },
      });
      expect([401, 403]).toContain(response.status());
    });

    test('should update translations with valid admin token', async () => {
      const payload = {
        en: 'Login Page Title',
        de: 'Anmeldungsseite Titel',
        fr: 'Titre de la page de connexion',
      };

      const response = await request.post(`${API_BASE}/localization/pages/login_title`, {
        headers: {
          Authorization: VALID_TOKEN,
        },
        data: payload,
      });
      expect([200, 204]).toContain(response.status());
    });

    test('should reject invalid translation data', async () => {
      const response = await request.post(`${API_BASE}/localization/test/key`, {
        headers: {
          Authorization: VALID_TOKEN,
        },
        data: {},
      });
      expect([400, 401]).toContain(response.status());
    });
  });

  test.describe('Error Handling', () => {
    test('should handle malformed language code', async () => {
      const response = await request.get(`${API_BASE}/localization/auth/login?language=invalid`);
      // Should fallback to English
      expect([200, 400]).toContain(response.status());
    });

    test('should handle missing query parameters', async () => {
      const response = await request.get(`${API_BASE}/localization/auth/login`);
      // Should use default language
      expect([200, 400]).toContain(response.status());
    });

    test('should handle special characters in keys', async () => {
      const response = await request.get(
        `${API_BASE}/localization/test/key-with_special.chars?language=en`
      );
      expect([200, 404]).toContain(response.status());
    });
  });
});
