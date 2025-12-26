import { apiClient } from "../client";
import type { Product, Category, PricingRule, Discount } from "@/types/shop";
import type { PaginatedResponse, PaginationParams } from "@/types/api";

export const shopApi = {
  // Products
  getProducts(filters?: any, pagination?: PaginationParams) {
    return apiClient.get<PaginatedResponse<Product>>("/products", {
      params: { ...filters, ...pagination },
    });
  },

  getProduct(id: string) {
    return apiClient.get<Product>(`/products/${id}`);
  },

  createProduct(data: Omit<Product, "id" | "createdAt" | "updatedAt">) {
    return apiClient.post<Product>("/products", data);
  },

  updateProduct(id: string, data: Partial<Product>) {
    return apiClient.put<Product>(`/products/${id}`, data);
  },

  deleteProduct(id: string) {
    return apiClient.delete<void>(`/products/${id}`);
  },

  bulkImportProducts(file: File) {
    const formData = new FormData();
    formData.append("file", file);
    return apiClient.post<{ imported: number; skipped: number; errors: any[] }>(
      "/products/bulk-import",
      formData,
      { headers: { "Content-Type": "multipart/form-data" } }
    );
  },

  // Categories
  getCategories() {
    return apiClient.get<Category[]>("/categories");
  },

  getCategory(id: string) {
    return apiClient.get<Category>(`/categories/${id}`);
  },

  createCategory(data: Omit<Category, "id">) {
    return apiClient.post<Category>("/categories", data);
  },

  updateCategory(id: string, data: Partial<Category>) {
    return apiClient.put<Category>(`/categories/${id}`, data);
  },

  deleteCategory(id: string) {
    return apiClient.delete<void>(`/categories/${id}`);
  },

  // Pricing Rules
  getPricingRules(pagination?: PaginationParams) {
    return apiClient.get<PaginatedResponse<PricingRule>>("/pricing/rules", {
      params: pagination,
    });
  },

  getPricingRule(id: string) {
    return apiClient.get<PricingRule>(`/pricing/rules/${id}`);
  },

  createPricingRule(data: Omit<PricingRule, "id">) {
    return apiClient.post<PricingRule>("/pricing/rules", data);
  },

  updatePricingRule(id: string, data: Partial<PricingRule>) {
    return apiClient.put<PricingRule>(`/pricing/rules/${id}`, data);
  },

  deletePricingRule(id: string) {
    return apiClient.delete<void>(`/pricing/rules/${id}`);
  },

  // Discounts
  getDiscounts(pagination?: PaginationParams) {
    return apiClient.get<PaginatedResponse<Discount>>("/discounts", {
      params: pagination,
    });
  },

  getDiscount(id: string) {
    return apiClient.get<Discount>(`/discounts/${id}`);
  },

  createDiscount(data: Omit<Discount, "id" | "usedCount">) {
    return apiClient.post<Discount>("/discounts", data);
  },

  updateDiscount(id: string, data: Partial<Discount>) {
    return apiClient.put<Discount>(`/discounts/${id}`, data);
  },

  deleteDiscount(id: string) {
    return apiClient.delete<void>(`/discounts/${id}`);
  },
};
