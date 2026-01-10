import { api } from './api';
import type { AxiosInstance } from 'axios';

/**
 * Localization API service for fetching and managing translations
 */
class LocalizationApi {
  private api: AxiosInstance;

  constructor(axiosInstance: AxiosInstance) {
    this.api = axiosInstance;
  }

  /**
   * Get a single translated string
   * @param category Translation category
   * @param key Translation key
   * @param language Language code (defaults to current language)
   */
  async getString(category: string, key: string, language: string = 'en'): Promise<string> {
    try {
      const response = await this.api.get<{
        key: string;
        value: string;
        language: string;
      }>(`/localization/${category}/${key}`, {
        params: { language },
      });
      return response.data.value;
    } catch (error) {
      console.error(`Failed to fetch localization string: ${category}.${key}`, error);
      return `[${category}.${key}]`;
    }
  }

  /**
   * Get all translations for a category
   * @param category Translation category
   * @param language Language code (defaults to 'en')
   */
  async getCategory(category: string, language: string = 'en'): Promise<Record<string, string>> {
    try {
      const response = await this.api.get<{
        category: string;
        language: string;
        translations: Record<string, string>;
      }>(`/localization/category/${category}`, {
        params: { language },
      });
      return response.data.translations;
    } catch (error) {
      console.error(`Failed to fetch localization category: ${category}`, error);
      return {};
    }
  }

  /**
   * Get all supported languages
   */
  async getSupportedLanguages(): Promise<string[]> {
    try {
      const response = await this.api.get<{ languages: string[] }>('/localization/languages');
      return response.data.languages;
    } catch (error) {
      console.error('Failed to fetch supported languages', error);
      return ['en'];
    }
  }

  /**
   * Set translations for a key (admin only)
   * @param category Translation category
   * @param key Translation key
   * @param translations Dictionary of language codes to values
   */
  async setTranslations(
    category: string,
    key: string,
    translations: Record<string, string>
  ): Promise<void> {
    try {
      await this.api.post(`/localization/${category}/${key}`, translations);
    } catch (error) {
      console.error(`Failed to set translations for ${category}.${key}`, error);
      throw error;
    }
  }

  /**
   * Prefetch translations for a category (useful for performance)
   * @param categories Categories to prefetch
   * @param language Language code
   */
  async prefetchCategories(
    categories: string[],
    language: string = 'en'
  ): Promise<Record<string, Record<string, string>>> {
    const results: Record<string, Record<string, string>> = {};

    try {
      await Promise.all(
        categories.map(async category => {
          results[category] = await this.getCategory(category, language);
        })
      );
    } catch (error) {
      console.error('Failed to prefetch categories', error);
    }

    return results;
  }
}

// Create singleton instance
const localizationApi = new LocalizationApi(api);

export default localizationApi;
