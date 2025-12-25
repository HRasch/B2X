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
    return apiClient.get<PaginatedResponse<Product>>("/catalog/products", {
      params: filters,
    });
  },

  /**
   * Get a single product by ID
   */
  getProduct(id: string) {
    return apiClient.get<Product>(`/catalog/products/${id}`);
  },

  /**
   * Create a new product
   */
  createProduct(data: CreateProductRequest) {
    return apiClient.post<Product>("/catalog/products", data);
  },

  /**
   * Update an existing product
   */
  updateProduct(id: string, data: UpdateProductRequest) {
    return apiClient.put<Product>(`/catalog/products/${id}`, data);
  },

  /**
   * Delete a product
   */
  deleteProduct(id: string) {
    return apiClient.delete<void>(`/catalog/products/${id}`);
  },

  /**
   * Bulk import products from CSV
   */
  bulkImportProducts(file: File) {
    const formData = new FormData();
    formData.append("file", file);
    return apiClient.post<{ imported: number; errors: any[] }>(
      "/catalog/products/bulk-import",
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
    return apiClient.get<PaginatedResponse<Category>>("/catalog/categories", {
      params: filters,
    });
  },

  /**
   * Get a single category by ID
   */
  getCategory(id: string) {
    return apiClient.get<Category>(`/catalog/categories/${id}`);
  },

  /**
   * Create a new category
   */
  createCategory(data: CreateCategoryRequest) {
    return apiClient.post<Category>("/catalog/categories", data);
  },

  /**
   * Update an existing category
   */
  updateCategory(id: string, data: UpdateCategoryRequest) {
    return apiClient.put<Category>(`/catalog/categories/${id}`, data);
  },

  /**
   * Delete a category
   */
  deleteCategory(id: string) {
    return apiClient.delete<void>(`/catalog/categories/${id}`);
  },

  // =========================================================================
  // Brands
  // =========================================================================

  /**
   * Fetch all brands with optional filters
   */
  getBrands(filters?: BrandFilters) {
    return apiClient.get<PaginatedResponse<Brand>>("/catalog/brands", {
      params: filters,
    });
  },

  /**
   * Get a single brand by ID
   */
  getBrand(id: string) {
    return apiClient.get<Brand>(`/catalog/brands/${id}`);
  },

  /**
   * Create a new brand
   */
  createBrand(data: CreateBrandRequest) {
    return apiClient.post<Brand>("/catalog/brands", data);
  },

  /**
   * Update an existing brand
   */
  updateBrand(id: string, data: UpdateBrandRequest) {
    return apiClient.put<Brand>(`/catalog/brands/${id}`, data);
  },

  /**
   * Delete a brand
   */
  deleteBrand(id: string) {
    return apiClient.delete<void>(`/catalog/brands/${id}`);
  },
};
