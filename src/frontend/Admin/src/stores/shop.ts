/**
 * Shop Store (Pinia)
 * Properly typed error handling and API responses
 */

import { defineStore } from 'pinia';
import { ref } from 'vue';
import type {
  Product,
  Category,
  PricingRule,
  Discount,
  ShopApiError,
  ProductFilters,
} from '@/types/shop';
import type { ApiError } from '@/types/api';
import { shopApi } from '@/services/api/shop';

export const useShopStore = defineStore('shop', () => {
  const products = ref<Product[]>([]);
  const categories = ref<Category[]>([]);
  const currentProduct = ref<Product | null>(null);
  const pricingRules = ref<PricingRule[]>([]);
  const discounts = ref<Discount[]>([]);
  const loading = ref(false);
  const error = ref<string | null>(null);

  async function fetchProducts(filters?: ProductFilters) {
    loading.value = true;
    error.value = null;
    try {
      const response = await shopApi.getProducts(filters);
      products.value = response.items;
    } catch (err: unknown) {
      const apiError = err as ApiError | ShopApiError;
      error.value = apiError.message || 'Failed to fetch products';
    } finally {
      loading.value = false;
    }
  }

  async function fetchProduct(id: string) {
    loading.value = true;
    error.value = null;
    try {
      currentProduct.value = await shopApi.getProduct(id);
    } catch (err: unknown) {
      const apiError = err as ApiError | ShopApiError;
      error.value = apiError.message || 'Failed to fetch product';
    } finally {
      loading.value = false;
    }
  }

  async function saveProduct(product: Product | Omit<Product, 'id' | 'createdAt' | 'updatedAt'>) {
    loading.value = true;
    error.value = null;
    try {
      if ('id' in product) {
        const updated = await shopApi.updateProduct(product.id, product);
        currentProduct.value = updated;
        const index = products.value.findIndex((p: Product) => p.id === product.id);
        if (index !== -1) products.value[index] = updated;
        return updated;
      } else {
        const created = await shopApi.createProduct(product);
        products.value.push(created);
        currentProduct.value = created;
        return created;
      }
    } catch (err: unknown) {
      const apiError = err as ApiError | ShopApiError;
      error.value = apiError.message || 'Failed to save product';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function deleteProduct(productId: string) {
    loading.value = true;
    try {
      await shopApi.deleteProduct(productId);
      products.value = products.value.filter((p: Product) => p.id !== productId);
      if (currentProduct.value?.id === productId) currentProduct.value = null;
    } catch (err: unknown) {
      const apiError = err as ApiError | ShopApiError;
      error.value = apiError.message || 'Failed to delete product';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function fetchCategories() {
    loading.value = true;
    error.value = null;
    try {
      categories.value = await shopApi.getCategories();
    } catch (err: unknown) {
      const apiError = err as ApiError | ShopApiError;
      error.value = apiError.message || 'Failed to fetch categories';
    } finally {
      loading.value = false;
    }
  }

  async function fetchPricingRules() {
    loading.value = true;
    error.value = null;
    try {
      const response = await shopApi.getPricingRules();
      pricingRules.value = response.items;
    } catch (err: unknown) {
      const apiError = err as ApiError | ShopApiError;
      error.value = apiError.message || 'Failed to fetch pricing rules';
    } finally {
      loading.value = false;
    }
  }

  async function savePricingRule(rule: PricingRule | Omit<PricingRule, 'id'>) {
    loading.value = true;
    error.value = null;
    try {
      if ('id' in rule) {
        const updated = await shopApi.updatePricingRule(rule.id, rule);
        const index = pricingRules.value.findIndex((r: PricingRule) => r.id === rule.id);
        if (index !== -1) pricingRules.value[index] = updated;
        return updated;
      } else {
        const created = await shopApi.createPricingRule(rule);
        pricingRules.value.push(created);
        return created;
      }
    } catch (err: unknown) {
      const apiError = err as ApiError | ShopApiError;
      error.value = apiError.message || 'Failed to save pricing rule';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function fetchDiscounts() {
    loading.value = true;
    error.value = null;
    try {
      const response = await shopApi.getDiscounts();
      discounts.value = response.items;
    } catch (err: unknown) {
      const apiError = err as ApiError | ShopApiError;
      error.value = apiError.message || 'Failed to fetch discounts';
    } finally {
      loading.value = false;
    }
  }

  return {
    products,
    categories,
    currentProduct,
    pricingRules,
    discounts,
    loading,
    error,
    fetchProducts,
    fetchProduct,
    saveProduct,
    deleteProduct,
    fetchCategories,
    fetchPricingRules,
    savePricingRule,
    fetchDiscounts,
  };
});
