/**
 * Catalog Store (Pinia)
 * State Management für Produkte, Kategorien und Marken
 *
 * Properly typed error handling and API responses
 */

import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { catalogApi } from '@/services/api/catalog';
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
  ApiError,
  CatalogError,
} from '@/types/catalog';

export const useCatalogStore = defineStore('catalog', () => {
  // =========================================================================
  // State
  // =========================================================================

  // Products
  const products = ref<Product[]>([]);
  const currentProduct = ref<Product | null>(null);
  const productsTotal = ref(0);
  const productsPagination = ref({ skip: 0, take: 10 });

  // Categories
  const categories = ref<Category[]>([]);
  const currentCategory = ref<Category | null>(null);
  const categoriesTotal = ref(0);
  const categoriesPagination = ref({ skip: 0, take: 10 });

  // Brands
  const brands = ref<Brand[]>([]);
  const currentBrand = ref<Brand | null>(null);
  const brandsTotal = ref(0);
  const brandsPagination = ref({ skip: 0, take: 10 });

  // UI State
  const loading = ref(false);
  const error = ref<string | null>(null);
  const successMessage = ref<string | null>(null);

  // =========================================================================
  // Computed Properties
  // =========================================================================

  const categoryMap = computed(() => {
    const map = new Map<string, Category>();
    categories.value.forEach(cat => map.set(cat.id, cat));
    return map;
  });

  const brandMap = computed(() => {
    const map = new Map<string, Brand>();
    brands.value.forEach(brand => map.set(brand.id, brand));
    return map;
  });

  const hasMoreProducts = computed(
    () => productsPagination.value.skip + productsPagination.value.take < productsTotal.value
  );

  const hasMoreCategories = computed(
    () => categoriesPagination.value.skip + categoriesPagination.value.take < categoriesTotal.value
  );

  const hasMoreBrands = computed(
    () => brandsPagination.value.skip + brandsPagination.value.take < brandsTotal.value
  );

  // =========================================================================
  // Product Actions
  // =========================================================================

  async function fetchProducts(filters?: ProductFilters) {
    loading.value = true;
    error.value = null;
    try {
      const response = await catalogApi.getProducts({
        ...filters,
        skip: productsPagination.value.skip,
        take: productsPagination.value.take,
      });
      products.value = response.items;
      productsTotal.value = response.totalCount;
      successMessage.value = null;
    } catch (err: unknown) {
      const apiError = err as ApiError | CatalogError;
      error.value = apiError.message || 'Failed to fetch products';
    } finally {
      loading.value = false;
    }
  }

  async function fetchProduct(id: string) {
    loading.value = true;
    error.value = null;
    try {
      currentProduct.value = await catalogApi.getProduct(id);
    } catch (err: unknown) {
      const apiError = err as ApiError | CatalogError;
      error.value = apiError.message || 'Failed to fetch product';
    } finally {
      loading.value = false;
    }
  }

  async function createProduct(data: CreateProductRequest) {
    loading.value = true;
    error.value = null;
    try {
      const created = await catalogApi.createProduct(data);
      products.value.push(created);
      currentProduct.value = created;
      successMessage.value = 'Product created successfully';
      return created;
    } catch (err: unknown) {
      const apiError = err as ApiError | CatalogError;
      error.value = apiError.message || 'Failed to create product';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function updateProduct(id: string, data: UpdateProductRequest) {
    loading.value = true;
    error.value = null;
    try {
      const updated = await catalogApi.updateProduct(id, data);
      const index = products.value.findIndex(p => p.id === id);
      if (index !== -1) {
        products.value[index] = updated;
      }
      currentProduct.value = updated;
      successMessage.value = 'Product updated successfully';
      return updated;
    } catch (err: unknown) {
      const apiError = err as ApiError | CatalogError;
      error.value = apiError.message || 'Failed to update product';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function deleteProduct(id: string) {
    loading.value = true;
    error.value = null;
    try {
      await catalogApi.deleteProduct(id);
      products.value = products.value.filter(p => p.id !== id);
      if (currentProduct.value?.id === id) currentProduct.value = null;
      successMessage.value = 'Product deleted successfully';
    } catch (err: unknown) {
      const apiError = err as ApiError | CatalogError;
      error.value = apiError.message || 'Failed to delete product';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  function setProductsPagination(skip: number, take: number) {
    productsPagination.value = { skip, take };
  }

  // =========================================================================
  // Category Actions
  // =========================================================================

  async function fetchCategories(filters?: CategoryFilters) {
    loading.value = true;
    error.value = null;
    try {
      const response = await catalogApi.getCategories({
        ...filters,
        skip: categoriesPagination.value.skip,
        take: categoriesPagination.value.take,
      });
      categories.value = response.items;
      categoriesTotal.value = response.totalCount;
      successMessage.value = null;
    } catch (err: unknown) {
      const apiError = err as ApiError | CatalogError;
      error.value = apiError.message || 'Failed to fetch categories';
    } finally {
      loading.value = false;
    }
  }

  async function fetchCategory(id: string) {
    loading.value = true;
    error.value = null;
    try {
      currentCategory.value = await catalogApi.getCategory(id);
    } catch (err: unknown) {
      const apiError = err as ApiError | CatalogError;
      error.value = apiError.message || 'Failed to fetch category';
    } finally {
      loading.value = false;
    }
  }

  async function createCategory(data: CreateCategoryRequest) {
    loading.value = true;
    error.value = null;
    try {
      const created = await catalogApi.createCategory(data);
      categories.value.push(created);
      currentCategory.value = created;
      successMessage.value = 'Category created successfully';
      return created;
    } catch (err: unknown) {
      const apiError = err as ApiError | CatalogError;
      error.value = apiError.message || 'Failed to create category';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function updateCategory(id: string, data: UpdateCategoryRequest) {
    loading.value = true;
    error.value = null;
    try {
      const updated = await catalogApi.updateCategory(id, data);
      const index = categories.value.findIndex(c => c.id === id);
      if (index !== -1) {
        categories.value[index] = updated;
      }
      currentCategory.value = updated;
      successMessage.value = 'Category updated successfully';
      return updated;
    } catch (err: unknown) {
      const apiError = err as ApiError | CatalogError;
      error.value = apiError.message || 'Failed to update category';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function deleteCategory(id: string) {
    loading.value = true;
    error.value = null;
    try {
      await catalogApi.deleteCategory(id);
      categories.value = categories.value.filter(c => c.id !== id);
      if (currentCategory.value?.id === id) currentCategory.value = null;
      successMessage.value = 'Category deleted successfully';
    } catch (err: unknown) {
      const apiError = err as ApiError | CatalogError;
      error.value = apiError.message || 'Failed to delete category';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  function setCategoriesPagination(skip: number, take: number) {
    categoriesPagination.value = { skip, take };
  }

  // =========================================================================
  // Brand Actions
  // =========================================================================

  async function fetchBrands(filters?: BrandFilters) {
    loading.value = true;
    error.value = null;
    try {
      const response = await catalogApi.getBrands({
        ...filters,
        skip: brandsPagination.value.skip,
        take: brandsPagination.value.take,
      });
      brands.value = response.items;
      brandsTotal.value = response.totalCount;
      successMessage.value = null;
    } catch (err: unknown) {
      const apiError = err as ApiError | CatalogError;
      error.value = apiError.message || 'Failed to fetch brands';
    } finally {
      loading.value = false;
    }
  }

  async function fetchBrand(id: string) {
    loading.value = true;
    error.value = null;
    try {
      currentBrand.value = await catalogApi.getBrand(id);
    } catch (err: unknown) {
      const apiError = err as ApiError | CatalogError;
      error.value = apiError.message || 'Failed to fetch brand';
    } finally {
      loading.value = false;
    }
  }

  async function createBrand(data: CreateBrandRequest) {
    loading.value = true;
    error.value = null;
    try {
      const created = await catalogApi.createBrand(data);
      brands.value.push(created);
      currentBrand.value = created;
      successMessage.value = 'Brand created successfully';
      return created;
    } catch (err: unknown) {
      const apiError = err as ApiError | CatalogError;
      error.value = apiError.message || 'Failed to create brand';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function updateBrand(id: string, data: UpdateBrandRequest) {
    loading.value = true;
    error.value = null;
    try {
      const updated = await catalogApi.updateBrand(id, data);
      const index = brands.value.findIndex(b => b.id === id);
      if (index !== -1) {
        brands.value[index] = updated;
      }
      currentBrand.value = updated;
      successMessage.value = 'Brand updated successfully';
      return updated;
    } catch (err: unknown) {
      const apiError = err as ApiError | CatalogError;
      error.value = apiError.message || 'Failed to update brand';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  async function deleteBrand(id: string) {
    loading.value = true;
    error.value = null;
    try {
      await catalogApi.deleteBrand(id);
      brands.value = brands.value.filter(b => b.id !== id);
      if (currentBrand.value?.id === id) currentBrand.value = null;
      successMessage.value = 'Brand deleted successfully';
    } catch (err: unknown) {
      const apiError = err as ApiError | CatalogError;
      error.value = apiError.message || 'Failed to delete brand';
      throw err;
    } finally {
      loading.value = false;
    }
  }

  function setBrandsPagination(skip: number, take: number) {
    brandsPagination.value = { skip, take };
  }

  // =========================================================================
  // UI Actions
  // =========================================================================

  function clearError() {
    error.value = null;
  }

  function clearSuccess() {
    successMessage.value = null;
  }

  function clearCurrentProduct() {
    currentProduct.value = null;
  }

  function clearCurrentCategory() {
    currentCategory.value = null;
  }

  function clearCurrentBrand() {
    currentBrand.value = null;
  }

  return {
    // Products
    products,
    currentProduct,
    productsTotal,
    productsPagination,
    hasMoreProducts,
    fetchProducts,
    fetchProduct,
    createProduct,
    updateProduct,
    deleteProduct,
    setProductsPagination,
    clearCurrentProduct,

    // Categories
    categories,
    currentCategory,
    categoriesTotal,
    categoriesPagination,
    hasMoreCategories,
    categoryMap,
    fetchCategories,
    fetchCategory,
    createCategory,
    updateCategory,
    deleteCategory,
    setCategoriesPagination,
    clearCurrentCategory,

    // Brands
    brands,
    currentBrand,
    brandsTotal,
    brandsPagination,
    hasMoreBrands,
    brandMap,
    fetchBrands,
    fetchBrand,
    createBrand,
    updateBrand,
    deleteBrand,
    setBrandsPagination,
    clearCurrentBrand,

    // UI
    loading,
    error,
    successMessage,
    clearError,
    clearSuccess,
  };
});
