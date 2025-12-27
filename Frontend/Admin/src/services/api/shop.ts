import { apiClient } from "../client";
import type { Product, Category, PricingRule, Discount } from "@/types/shop";
import type { PaginatedResponse, PaginationParams } from "@/types/api";

export const shopApi = {
  // Products
  getProducts(filters?: any, pagination?: PaginationParams) {
    return apiClient.get<PaginatedResponse<Product>>("/api/v1/products", {
      params: { ...filters, ...pagination },
    });
  },

  getProduct(id: string) {
    return apiClient.get<Product>(`/api/v1/products/${id}`);
  },

  createProduct(data: Omit<Product, "id" | "createdAt" | "updatedAt">) {
    return apiClient.post<Product>("/api/v1/products", data);
  },

  updateProduct(id: string, data: Partial<Product>) {
    return apiClient.put<Product>(`/api/v1/products/${id}`, data);
  },

  deleteProduct(id: string) {
    return apiClient.delete<void>(`/api/v1/products/${id}`);
  },

  bulkImportProducts(file: File) {
    const formData = new FormData();
    formData.append("file", file);
    return apiClient.post<{ imported: number; skipped: number; errors: any[] }>(
      "/api/v1/products/bulk-import",
      formData,
      { headers: { "Content-Type": "multipart/form-data" } }
    );
  },

  // Categories
  getCategories() {
    return apiClient.get<Category[]>("/api/v1/categories");
  },

  getCategory(id: string) {
    return apiClient.get<Category>(`/api/v1/categories/${id}`);
  },

  createCategory(data: Omit<Category, "id">) {
    return apiClient.post<Category>("/api/v1/categories", data);
  },

  updateCategory(id: string, data: Partial<Category>) {
    return apiClient.put<Category>(`/api/v1/categories/${id}`, data);
  },

  deleteCategory(id: string) {
    return apiClient.delete<void>(`/api/v1/categories/${id}`);
  },

  // Pricing Rules
  getPricingRules(pagination?: PaginationParams) {
    return apiClient.get<PaginatedResponse<PricingRule>>(
      "/api/v1/pricing/rules",
      {
        params: pagination,
      }
    );
  },

  getPricingRule(id: string) {
    return apiClient.get<PricingRule>(`/api/v1/pricing/rules/${id}`);
  },

  createPricingRule(data: Omit<PricingRule, "id">) {
    return apiClient.post<PricingRule>("/api/v1/pricing/rules", data);
  },

  updatePricingRule(id: string, data: Partial<PricingRule>) {
    return apiClient.put<PricingRule>(`/api/v1/pricing/rules/${id}`, data);
  },

  deletePricingRule(id: string) {
    return apiClient.delete<void>(`/api/v1/pricing/rules/${id}`);
  },

  // Discounts
  getDiscounts(pagination?: PaginationParams) {
    return apiClient.get<PaginatedResponse<Discount>>("/api/v1/discounts", {
      params: pagination,
    });
  },

  getDiscount(id: string) {
    return apiClient.get<Discount>(`/api/v1/discounts/${id}`);
  },

  createDiscount(data: Omit<Discount, "id" | "usedCount">) {
    return apiClient.post<Discount>("/api/v1/discounts", data);
  },

  updateDiscount(id: string, data: Partial<Discount>) {
    return apiClient.put<Discount>(`/api/v1/discounts/${id}`, data);
  },

  deleteDiscount(id: string) {
    return apiClient.delete<void>(`/api/v1/discounts/${id}`);
  },
};
