import { api } from "./api";

export interface Product {
  id: string;
  name: string;
  price: number;
  b2bPrice: number;
  image: string;
  category: string;
  description: string;
  inStock: boolean;
  rating: number;
  sku?: string;
  brand?: string;
  tags?: string;
  material?: string;
  stockQuantity?: number;
  relevanceScore?: number;
}

export interface SearchResponse {
  items: Product[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  searchMetadata?: {
    queryExecutionTimeMs: number;
    hitCount: number;
    source: string;
  };
}

export interface SearchFilters {
  searchTerm?: string;
  category?: string;
  minPrice?: number;
  maxPrice?: number;
  language?: string;
  onlyAvailable?: boolean;
  sortBy?: string;
}

/**
 * ProductService - ElasticSearch-powered product discovery
 *
 * Features:
 * - Full-text search with fuzzy matching (typo tolerance)
 * - Multi-field search (Name, Description, Category, SKU, Brand)
 * - Price range filtering
 * - Category filtering
 * - Availability filtering
 * - Language-specific search (de, en, fr)
 * - Relevance-based ranking
 *
 * Uses: /api/v2/products/elasticsearch endpoint
 */
export class ProductService {
  /**
   * Search products using ElasticSearch
   *
   * @param filters Search filters and pagination
   * @param page Page number (1-based)
   * @param pageSize Items per page (default: 20, max: 100)
   * @returns Promise<SearchResponse> with products and metadata
   *
   * @example
   * const response = await ProductService.searchProducts({
   *   searchTerm: 'laptop',
   *   category: 'Elektronik',
   *   minPrice: 100,
   *   maxPrice: 5000,
   *   language: 'de'
   * }, 1, 20)
   */
  static async searchProducts(
    filters: SearchFilters,
    page: number = 1,
    pageSize: number = 20
  ): Promise<SearchResponse> {
    try {
      if (!filters.searchTerm || filters.searchTerm.trim() === "") {
        throw new Error("Search term is required");
      }

      // Validate pagination
      if (page < 1) page = 1;
      if (pageSize < 1 || pageSize > 100) pageSize = 20;

      // Build query parameters
      const params = new URLSearchParams({
        term: filters.searchTerm.trim(),
        page: page.toString(),
        pageSize: pageSize.toString(),
        language: filters.language || "de",
        onlyAvailable: (filters.onlyAvailable !== false).toString(),
        sortBy: filters.sortBy || "relevance",
      });

      // Add optional filters
      if (filters.category) {
        params.append("category", filters.category);
      }
      if (filters.minPrice !== undefined && filters.minPrice > 0) {
        params.append("minPrice", filters.minPrice.toString());
      }
      if (filters.maxPrice !== undefined && filters.maxPrice > 0) {
        params.append("maxPrice", filters.maxPrice.toString());
      }

      // Call ElasticSearch endpoint
      const response = await api.get<SearchResponse>(
        `/v2/products/elasticsearch?${params.toString()}`
      );

      return response.data;
    } catch (error) {
      console.error("Error searching products:", error);
      throw error;
    }
  }

  /**
   * Get single product by ID (from read model)
   * Faster than search for direct product access
   */
  static async getProductById(productId: string): Promise<Product> {
    try {
      const response = await api.get<Product>(`/v2/products/${productId}`);
      return response.data;
    } catch (error) {
      console.error(`Error fetching product ${productId}:`, error);
      throw error;
    }
  }

  /**
   * Get paginated products (from read model)
   * Use for browsing without search term
   */
  static async getProducts(
    page: number = 1,
    pageSize: number = 20,
    filters?: Partial<SearchFilters>
  ): Promise<SearchResponse> {
    try {
      if (page < 1) page = 1;
      if (pageSize < 1 || pageSize > 100) pageSize = 20;

      const params = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString(),
      });

      if (filters?.category) {
        params.append("category", filters.category);
      }
      if (filters?.minPrice !== undefined && filters.minPrice > 0) {
        params.append("minPrice", filters.minPrice.toString());
      }
      if (filters?.maxPrice !== undefined && filters.maxPrice > 0) {
        params.append("maxPrice", filters.maxPrice.toString());
      }

      const response = await api.get<SearchResponse>(
        `/v2/products?${params.toString()}`
      );

      return response.data;
    } catch (error) {
      console.error("Error fetching products:", error);
      throw error;
    }
  }

  /**
   * Get catalog statistics (aggregated data)
   */
  static async getCatalogStats() {
    try {
      const response = await api.get("/v2/products/stats");
      return response.data;
    } catch (error) {
      console.error("Error fetching catalog statistics:", error);
      throw error;
    }
  }
}
