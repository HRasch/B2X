import { client } from '@/services/client';
import type { PageDefinition, WidgetDefinition } from '@/types/cms';

export const cmsApi = {
  /**
   * Fetch page definition from backend
   * Returns page structure with all regions and widgets
   */
  async getPageDefinition(pagePath: string): Promise<PageDefinition> {
    return client.get(`/api/cms/pages/by-path`, {
      params: { path: pagePath }
    });
  },

  /**
   * Get list of available widgets (for admin page builder)
   */
  async getAvailableWidgets(pageType?: string): Promise<WidgetDefinition[]> {
    return client.get(`/api/cms/widgets`, {
      params: { pageType }
    });
  },

  /**
   * Get specific widget definition
   */
  async getWidgetDefinition(widgetId: string): Promise<WidgetDefinition> {
    return client.get(`/api/cms/widgets/${widgetId}`);
  },

  /**
   * Get widgets by category (for admin interface)
   */
  async getWidgetsByCategory(category: string): Promise<WidgetDefinition[]> {
    return client.get(`/api/cms/widgets/category/${category}`);
  }
};
