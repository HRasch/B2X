import { api } from './api';
import { useAuthStore } from '../stores/auth';

export interface StoreVisibilityResponse {
  isPublicStore: boolean;
  requiresAuthentication: boolean;
  tenantId: string;
  tenantName?: string;
}

/**
 * Cached store visibility state to avoid repeated API calls.
 */
let cachedVisibility: StoreVisibilityResponse | null = null;
let cacheTimestamp = 0;
const CACHE_TTL_MS = 5 * 60 * 1000; // 5 minutes cache

/**
 * Service for checking store visibility/access mode.
 * Determines if a store is public (B2C) or requires authentication (B2B closed shop).
 */
export const storeVisibilityService = {
  /**
   * Fetches the store visibility settings for the current tenant.
   * Uses caching to avoid repeated API calls.
   *
   * @param forceRefresh - If true, bypasses cache and fetches fresh data
   * @returns Store visibility configuration
   */
  async getVisibility(forceRefresh = false): Promise<StoreVisibilityResponse> {
    const now = Date.now();

    // Return cached value if still valid
    if (!forceRefresh && cachedVisibility && now - cacheTimestamp < CACHE_TTL_MS) {
      return cachedVisibility;
    }

    try {
      // Get tenant ID from auth store or use default for development
      const authStore = useAuthStore();
      let tenantId = authStore.tenantId;

      if (!tenantId) {
        // For development/testing, use a default tenant ID
        tenantId = '12345678-1234-1234-1234-123456789abc';
      }

      const response = await api.get<StoreVisibilityResponse>('/api/tenant/visibility', {
        headers: {
          'X-Tenant-ID': tenantId,
        },
      });

      cachedVisibility = response.data;
      cacheTimestamp = now;

      return cachedVisibility;
    } catch (error) {
      console.error('Failed to fetch store visibility:', error);

      // Default to public store on error to avoid blocking users
      return {
        isPublicStore: true,
        requiresAuthentication: false,
        tenantId: '',
        tenantName: undefined,
      };
    }
  },

  /**
   * Checks if the current store requires authentication.
   * Convenience method for route guards.
   *
   * @returns True if authentication is required (closed shop)
   */
  async requiresAuthentication(): Promise<boolean> {
    const visibility = await this.getVisibility();
    return visibility.requiresAuthentication;
  },

  /**
   * Clears the visibility cache.
   * Useful when tenant settings might have changed.
   */
  clearCache(): void {
    cachedVisibility = null;
    cacheTimestamp = 0;
  },
};

export default storeVisibilityService;
