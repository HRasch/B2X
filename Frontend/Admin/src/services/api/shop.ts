import { apiClient } from "../client";
import type { Product, Category, PricingRule, Discount } from "@/types/shop";
import type { PaginatedResponse, PaginationParams } from "@/types/api";

// Demo mode - matches auth.ts configuration
const DEMO_MODE = import.meta.env.VITE_ENABLE_DEMO_MODE === "true";

// Demo data for shop
const DEMO_PRODUCTS: Product[] = [
  {
    id: "shop-prod-1",
    name: "Demo Shop Product",
    sku: "SHOP-001",
    price: 29.99,
    stock: 100,
    categoryId: "cat-1",
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  },
];

const DEMO_CATEGORIES: Category[] = [
  {
    id: "shop-cat-1",
    name: "Shop Category",
    slug: "shop-category",
    parentId: null,
  },
];

const DEMO_PRICING_RULES: PricingRule[] = [];
const DEMO_DISCOUNTS: Discount[] = [];

function delay<T>(data: T, ms = 200): Promise<T> {
  return new Promise((resolve) => setTimeout(() => resolve(data), ms));
}

export const shopApi = {
  // Products
  async getProducts(
    filters?: unknown,
    pagination?: PaginationParams
  ): Promise<PaginatedResponse<Product>> {
    if (DEMO_MODE) {
      console.warn("[SHOP] Demo mode active - returning mock products");
      return delay({
        items: DEMO_PRODUCTS,
        total: DEMO_PRODUCTS.length,
        skip: pagination?.skip || 0,
        take: pagination?.take || 10,
      });
    }
    return apiClient.get<PaginatedResponse<Product>>("/api/products", {
      params: { ...filters, ...pagination },
    });
  },

  async getProduct(id: string): Promise<Product> {
    if (DEMO_MODE) {
      const product = DEMO_PRODUCTS.find((p) => p.id === id);
      if (product) return delay(product);
      throw new Error("Product not found");
    }
    return apiClient.get<Product>(`/api/products/${id}`);
  },

  async createProduct(
    data: Omit<Product, "id" | "createdAt" | "updatedAt">
  ): Promise<Product> {
    if (DEMO_MODE) {
      console.warn("[SHOP] Demo mode - create product simulated");
      return delay({
        ...data,
        id: `shop-prod-${Date.now()}`,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      } as Product);
    }
    return apiClient.post<Product>("/api/products", data);
  },

  async updateProduct(id: string, data: Partial<Product>): Promise<Product> {
    if (DEMO_MODE) {
      console.warn("[SHOP] Demo mode - update product simulated");
      const existing = DEMO_PRODUCTS.find((p) => p.id === id);
      return delay({
        ...existing,
        ...data,
        updatedAt: new Date().toISOString(),
      } as Product);
    }
    return apiClient.put<Product>(`/api/products/${id}`, data);
  },

  async deleteProduct(id: string): Promise<void> {
    if (DEMO_MODE) {
      console.warn("[SHOP] Demo mode - delete product simulated");
      return delay(undefined);
    }
    return apiClient.delete<void>(`/api/products/${id}`);
  },

  bulkImportProducts(file: File) {
    if (DEMO_MODE) {
      console.warn("[SHOP] Demo mode - bulk import simulated");
      return delay({ imported: 0, skipped: 0, errors: [] });
    }
    const formData = new FormData();
    formData.append("file", file);
    return apiClient.post<{
      imported: number;
      skipped: number;
      errors: unknown[];
    }>("/api/products/bulk-import", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
  },

  // Categories
  async getCategories(): Promise<Category[]> {
    if (DEMO_MODE) {
      console.warn("[SHOP] Demo mode active - returning mock categories");
      return delay(DEMO_CATEGORIES);
    }
    return apiClient.get<Category[]>("/api/categories");
  },

  async getCategory(id: string): Promise<Category> {
    if (DEMO_MODE) {
      const category = DEMO_CATEGORIES.find((c) => c.id === id);
      if (category) return delay(category);
      throw new Error("Category not found");
    }
    return apiClient.get<Category>(`/api/categories/${id}`);
  },

  async createCategory(data: Omit<Category, "id">): Promise<Category> {
    if (DEMO_MODE) {
      console.warn("[SHOP] Demo mode - create category simulated");
      return delay({ ...data, id: `shop-cat-${Date.now()}` } as Category);
    }
    return apiClient.post<Category>("/api/categories", data);
  },

  async updateCategory(id: string, data: Partial<Category>): Promise<Category> {
    if (DEMO_MODE) {
      console.warn("[SHOP] Demo mode - update category simulated");
      const existing = DEMO_CATEGORIES.find((c) => c.id === id);
      return delay({ ...existing, ...data } as Category);
    }
    return apiClient.put<Category>(`/api/categories/${id}`, data);
  },

  async deleteCategory(id: string): Promise<void> {
    if (DEMO_MODE) {
      console.warn("[SHOP] Demo mode - delete category simulated");
      return delay(undefined);
    }
    return apiClient.delete<void>(`/api/categories/${id}`);
  },

  // Pricing Rules
  async getPricingRules(
    pagination?: PaginationParams
  ): Promise<PaginatedResponse<PricingRule>> {
    if (DEMO_MODE) {
      console.warn("[SHOP] Demo mode active - returning mock pricing rules");
      return delay({
        items: DEMO_PRICING_RULES,
        total: DEMO_PRICING_RULES.length,
        skip: pagination?.skip || 0,
        take: pagination?.take || 10,
      });
    }
    return apiClient.get<PaginatedResponse<PricingRule>>("/api/pricing/rules", {
      params: pagination,
    });
  },

  async getPricingRule(id: string): Promise<PricingRule> {
    if (DEMO_MODE) {
      const rule = DEMO_PRICING_RULES.find((r) => r.id === id);
      if (rule) return delay(rule);
      throw new Error("Pricing rule not found");
    }
    return apiClient.get<PricingRule>(`/api/pricing/rules/${id}`);
  },

  async createPricingRule(data: Omit<PricingRule, "id">): Promise<PricingRule> {
    if (DEMO_MODE) {
      console.warn("[SHOP] Demo mode - create pricing rule simulated");
      return delay({ ...data, id: `rule-${Date.now()}` } as PricingRule);
    }
    return apiClient.post<PricingRule>("/api/pricing/rules", data);
  },

  async updatePricingRule(
    id: string,
    data: Partial<PricingRule>
  ): Promise<PricingRule> {
    if (DEMO_MODE) {
      console.warn("[SHOP] Demo mode - update pricing rule simulated");
      const existing = DEMO_PRICING_RULES.find((r) => r.id === id);
      return delay({ ...existing, ...data } as PricingRule);
    }
    return apiClient.put<PricingRule>(`/api/pricing/rules/${id}`, data);
  },

  async deletePricingRule(id: string): Promise<void> {
    if (DEMO_MODE) {
      console.warn("[SHOP] Demo mode - delete pricing rule simulated");
      return delay(undefined);
    }
    return apiClient.delete<void>(`/api/pricing/rules/${id}`);
  },

  // Discounts
  async getDiscounts(
    pagination?: PaginationParams
  ): Promise<PaginatedResponse<Discount>> {
    if (DEMO_MODE) {
      console.warn("[SHOP] Demo mode active - returning mock discounts");
      return delay({
        items: DEMO_DISCOUNTS,
        total: DEMO_DISCOUNTS.length,
        skip: pagination?.skip || 0,
        take: pagination?.take || 10,
      });
    }
    return apiClient.get<PaginatedResponse<Discount>>("/api/discounts", {
      params: pagination,
    });
  },

  async getDiscount(id: string): Promise<Discount> {
    if (DEMO_MODE) {
      const discount = DEMO_DISCOUNTS.find((d) => d.id === id);
      if (discount) return delay(discount);
      throw new Error("Discount not found");
    }
    return apiClient.get<Discount>(`/api/discounts/${id}`);
  },

  async createDiscount(
    data: Omit<Discount, "id" | "usedCount">
  ): Promise<Discount> {
    if (DEMO_MODE) {
      console.warn("[SHOP] Demo mode - create discount simulated");
      return delay({
        ...data,
        id: `discount-${Date.now()}`,
        usedCount: 0,
      } as Discount);
    }
    return apiClient.post<Discount>("/api/discounts", data);
  },

  async updateDiscount(id: string, data: Partial<Discount>): Promise<Discount> {
    if (DEMO_MODE) {
      console.warn("[SHOP] Demo mode - update discount simulated");
      const existing = DEMO_DISCOUNTS.find((d) => d.id === id);
      return delay({ ...existing, ...data } as Discount);
    }
    return apiClient.put<Discount>(`/api/discounts/${id}`, data);
  },

  async deleteDiscount(id: string): Promise<void> {
    if (DEMO_MODE) {
      console.warn("[SHOP] Demo mode - delete discount simulated");
      return delay(undefined);
    }
    return apiClient.delete<void>(`/api/discounts/${id}`);
  },
};
