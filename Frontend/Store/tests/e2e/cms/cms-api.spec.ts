import { test, expect, APIRequestContext } from '@playwright/test';

// CMS API Types
interface CmsWidget {
  id: string;
  widgetTypeId: string;
  category: string;
  displayName: string;
  componentPath: string;
  isEnabled: boolean;
  defaultSettings: CmsWidgetSetting[];
  settings?: Record<string, unknown>;
}

interface CmsWidgetSetting {
  key: string;
  displayName: string;
  type: string;
}

interface CmsRegion {
  id: string;
  name: string;
  widgets: CmsWidget[];
}

// Helper to make API requests
const API_BASE_URL = 'http://localhost:5173/api';

test.describe('CMS API Integration', () => {
  let request: APIRequestContext;

  test.beforeAll(async ({ playwright }) => {
    request = await playwright.request.newContext();
  });

  test.afterAll(async () => {
    await request.dispose();
  });

  test('should fetch page definition for home page', async () => {
    const response = await request.get(`${API_BASE_URL}/cms/pages/by-path?path=/`);

    expect(response.status()).toBe(200);

    const data = await response.json();
    expect(data).toHaveProperty('pageTitle');
    expect(data).toHaveProperty('regions');
    expect(data.regions).toBeInstanceOf(Array);
  });

  test('should fetch available widgets', async () => {
    const response = await request.get(`${API_BASE_URL}/cms/widgets`);

    expect(response.status()).toBe(200);

    const widgets = await response.json();
    expect(Array.isArray(widgets)).toBe(true);

    // Should have default widgets
    if (widgets.length > 0) {
      const widget = widgets[0];
      expect(widget).toHaveProperty('id');
      expect(widget).toHaveProperty('displayName');
      expect(widget).toHaveProperty('componentPath');
    }
  });

  test('should fetch widgets for specific page type', async () => {
    const response = await request.get(`${API_BASE_URL}/cms/widgets?pageType=home`);

    expect(response.status()).toBe(200);

    const widgets = await response.json();
    expect(Array.isArray(widgets)).toBe(true);
  });

  test('should fetch widgets by category', async () => {
    const response = await request.get(`${API_BASE_URL}/cms/widgets/category/media`);

    expect(response.status()).toBe(200);

    const widgets = await response.json();
    expect(Array.isArray(widgets)).toBe(true);

    // All widgets should be in media category
    widgets.forEach((widget: CmsWidget) => {
      expect(widget.category).toBe('media');
    });
  });

  test('page definition should contain required properties', async () => {
    const response = await request.get(`${API_BASE_URL}/cms/pages/by-path?path=/`);
    const data = await response.json();

    // Check required properties
    expect(data).toHaveProperty('id');
    expect(data).toHaveProperty('tenantId');
    expect(data).toHaveProperty('pageType');
    expect(data).toHaveProperty('pagePath');
    expect(data).toHaveProperty('pageTitle');
    expect(data).toHaveProperty('templateLayout');
    expect(data).toHaveProperty('isPublished');
  });

  test('page definition regions should contain widgets', async () => {
    const response = await request.get(`${API_BASE_URL}/cms/pages/by-path?path=/`);
    const data = await response.json();

    expect(data.regions).toBeInstanceOf(Array);

    data.regions.forEach((region: CmsRegion) => {
      expect(region).toHaveProperty('id');
      expect(region).toHaveProperty('name');
      expect(region).toHaveProperty('widgets');
      expect(Array.isArray(region.widgets)).toBe(true);

      region.widgets.forEach((widget: CmsWidget) => {
        expect(widget).toHaveProperty('id');
        expect(widget).toHaveProperty('widgetTypeId');
        expect(widget).toHaveProperty('settings');
      });
    });
  });

  test('widget should have component path for dynamic loading', async () => {
    const response = await request.get(`${API_BASE_URL}/cms/pages/by-path?path=/`);
    const data = await response.json();

    if (data.regions.length > 0 && data.regions[0].widgets.length > 0) {
      const widget = data.regions[0].widgets[0];
      expect(widget).toHaveProperty('componentPath');
      expect(widget.componentPath).toMatch(/\.vue$/);
    }
  });
});

test.describe('CMS Widget Definitions', () => {
  let request: APIRequestContext;

  test.beforeAll(async ({ playwright }) => {
    request = await playwright.request.newContext();
  });

  test.afterAll(async () => {
    await request.dispose();
  });

  test('widget definition should contain settings metadata', async () => {
    const response = await request.get(`${API_BASE_URL}/cms/widgets`);
    const widgets = await response.json();

    if (widgets.length > 0) {
      const widget = widgets[0];

      expect(widget).toHaveProperty('defaultSettings');
      expect(Array.isArray(widget.defaultSettings)).toBe(true);

      widget.defaultSettings.forEach((setting: CmsWidgetSetting) => {
        expect(setting).toHaveProperty('key');
        expect(setting).toHaveProperty('displayName');
        expect(setting).toHaveProperty('type');
      });
    }
  });

  test('widget categories should be consistent', async () => {
    const response = await request.get(`${API_BASE_URL}/cms/widgets`);
    const widgets = await response.json();

    const categories = new Set(widgets.map((w: CmsWidget) => w.category));

    // Should have known categories
    const knownCategories = ['media', 'content', 'products', 'forms'];
    categories.forEach(cat => {
      expect(knownCategories).toContain(cat);
    });
  });

  test('disabled widgets should not be returned', async () => {
    const response = await request.get(`${API_BASE_URL}/cms/widgets`);
    const widgets = await response.json();

    widgets.forEach((widget: CmsWidget) => {
      expect(widget.isEnabled).toBe(true);
    });
  });
});

test.describe('CMS Page Types', () => {
  let request: APIRequestContext;

  test.beforeAll(async ({ playwright }) => {
    request = await playwright.request.newContext();
  });

  test.afterAll(async () => {
    await request.dispose();
  });

  test('should support different page types', async () => {
    const pageTypes = ['/', '/products', '/about', '/contact'];

    for (const path of pageTypes) {
      const response = await request.get(`${API_BASE_URL}/cms/pages/by-path?path=${path}`);

      if (response.status() === 200) {
        const data = await response.json();
        expect(data).toHaveProperty('pageType');
        expect(data).toHaveProperty('templateLayout');
      }
    }
  });

  test('product listing should have product grid widget', async () => {
    const response = await request.get(`${API_BASE_URL}/cms/pages/by-path?path=/products`);

    if (response.status() === 200) {
      const data = await response.json();

      const hasProductGrid = data.regions.some((region: CmsRegion) =>
        region.widgets.some((widget: CmsWidget) => widget.widgetTypeId === 'product-grid')
      );

      expect(hasProductGrid).toBe(true);
    }
  });
});
