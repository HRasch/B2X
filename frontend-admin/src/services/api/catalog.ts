/**
 * Catalog API Service
 * Endpoints für Produkte, Kategorien und Marken
 */

import { apiClient } from "../client";
import type {
  Product,
  Category,
  Brand,
  CreateProductRequest,
  UpdateProductRequest,
  CreateCategoryRequest,
  UpdateCategoryRequest,
  CreateBrandRequest,
  UpdateBrandRequest,
  ProductFilters,
  CategoryFilters,
  BrandFilters,
  PaginatedResponse,
} from "@/types/catalog";

export const catalogApi = {
  // =========================================================================
  // Products
  // =========================================================================

  /**
   * Fetch all products with optional filters
   */
  getProducts(filters?: ProductFilters) {
    return apiClient.get<PaginatedResponse<Product>>("/api/v1/products", {
      params: filters,
    });
  },

  /**
   * Get a single product by ID
   */
  getProduct(id: string) {
    return apiClient.get<Product>(`/api/v1/products/${id}`);
  },

  /**
   * Create a new product
   */
  createProduct(data: CreateProductRequest) {
    return apiClient.post<Product>("/api/v1/products", data);
  },

  /**
   * Update an existing product
   */
  updateProduct(id: string, data: UpdateProductRequest) {
    return apiClient.put<Product>(`/api/v1/products/${id}`, data);
  },

  /**
   * Delete a product
   */
  deleteProduct(id: string) {
    return apiClient.delete<void>(`/api/v1/products/${id}`);
  },

  /**
   * Bulk import products from CSV
   */
  bulkImportProducts(file: File) {
    const formData = new FormData();
    formData.append("file", file);
    return apiClient.post<{ imported: number; errors: any[] }>(
      "/api/v1/products/bulk-import",
      formData,
      { headers: { "Content-Type": "multipart/form-data" } }
    );
  },

  // =========================================================================
  // Categories
  // =========================================================================

  /**
   * Fetch all categories with optional filters
   */
  getCategories(filters?: CategoryFilters) {
    return apiClient.get<PaginatedResponse<Category>>("/api/v1/categories", {
      params: filters,
    });
  },

  /**
   * Get a single category by ID
   */
  getCategory(id: string) {
    return apiClient.get<Category>(`/api/v1/categories/${id}`);
  },

  /**
   * Create a new category
   */
  createCategory(data: CreateCategoryRequest) {
    return apiClient.post<Category>("/api/v1/categories", data);
  },

  /**
   * Update an existing category
   */
  updateCategory(id: string, data: UpdateCategoryRequest) {
    return apiClient.put<Category>(`/api/v1/categories/${id}`, data);
  },

  /**
   * Delete a category
   */
  deleteCategory(id: string) {
    return apiClient.delete<void>(`/api/v1/categories/${id}`);
  },

  // =========================================================================
  // Brands
  // =========================================================================

  /**
   * Fetch all brands with optional filters
   */
  getBrands(filters?: BrandFilters) {
    return apiClient.get<PaginatedResponse<Brand>>("/api/v1/brands", {
      params: filters,
    });
  },

  /**
   * Get a single brand by ID
   */
  getBrand(id: string) {
    return apiClient.get<Brand>(`/api/v1/brands/${id}`);
  },

  /**
   * Create a new brand
   */
  createBrand(data: CreateBrandRequest) {
    return apiClient.post<Brand>("/api/v1/brands", data);
  },

  /**
   * Update an existing brand
   */
  updateBrand(id: string, data: UpdateBrandRequest) {
    return apiClient.put<Brand>(`/api/v1/brands/${id}`, data);
  },

  /**
   * Delete a brand
   */
  deleteBrand(id: string) {
    return apiClient.delete<void>(`/api/v1/brands/${id}`);
  },
};
