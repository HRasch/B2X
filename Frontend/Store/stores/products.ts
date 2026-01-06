import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { ProductService, Product, SearchFilters, SearchResponse } from '~/services/productService';
import type { Category } from '~/stores/categories';

export const useProductsStore = defineStore('products', () => {
  const products = ref<Product[]>([]);
  const featuredProducts = ref<Product[]>([]);
  const categories = ref<Category[]>([]);
  const loading = ref(false);
  const error = ref<string | null>(null);

  const getProductById = computed(() => {
    return (id: string) => products.value.find(p => p.id === id);
  });

  const getProductsByCategory = computed(() => {
    return (category: string) => products.value.filter(p => p.category === category);
  });

  const loadProducts = async (filters?: SearchFilters, page = 1, pageSize = 20) => {
    loading.value = true;
    error.value = null;
    try {
      let response: SearchResponse;
      if (filters?.searchTerm) {
        response = await ProductService.searchProducts(filters, page, pageSize);
      } else {
        response = await ProductService.getProducts(page, pageSize, filters);
      }
      products.value = response.items;
      return response;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load products';
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const loadFeaturedProducts = async () => {
    loading.value = true;
    error.value = null;
    try {
      // Load featured products - assuming we can filter by some criteria
      const response = await ProductService.getProducts(1, 8, {
        /* featured: true */
      });
      featuredProducts.value = response.items;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load featured products';
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const loadProductById = async (id: string) => {
    loading.value = true;
    error.value = null;
    try {
      const product = await ProductService.getProductById(id);
      // Update or add to products array
      const existingIndex = products.value.findIndex(p => p.id === id);
      if (existingIndex >= 0) {
        products.value[existingIndex] = product;
      } else {
        products.value.push(product);
      }
      return product;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load product';
      throw err;
    } finally {
      loading.value = false;
    }
  };

  return {
    products,
    featuredProducts,
    categories,
    loading,
    error,
    getProductById,
    getProductsByCategory,
    loadProducts,
    loadFeaturedProducts,
    loadProductById,
  };
});
