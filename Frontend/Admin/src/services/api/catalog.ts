/**
 * Catalog API Service
 * Endpoints für Produkte, Kategorien und Marken
 */

import { apiClient } from '../client';
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
} from '@/types/catalog';

// Demo mode - matches auth.ts configuration
const DEMO_MODE = import.meta.env.VITE_ENABLE_DEMO_MODE === 'true';

// Demo data for testing
const DEMO_CATEGORIES: Category[] = [
  {
    id: 'cat-1',
    name: { de: 'Elektronik', en: 'Electronics' },
    slug: 'electronics',
    description: { de: 'Elektronische Geräte', en: 'Electronic devices' },
    isActive: true,
    sortOrder: 1,
    parentId: null,
    children: [],
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  },
  {
    id: 'cat-2',
    name: { de: 'Bekleidung', en: 'Clothing' },
    slug: 'clothing',
    description: { de: 'Mode und Bekleidung', en: 'Fashion and clothing' },
    isActive: true,
    sortOrder: 2,
    parentId: null,
    children: [],
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  },
];

const DEMO_BRANDS: Brand[] = [
  {
    id: 'brand-1',
    name: 'TechBrand',
    slug: 'techbrand',
    description: { de: 'Premium Technologie', en: 'Premium Technology' },
    logoUrl: null,
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  },
  {
    id: 'brand-2',
    name: 'FashionCo',
    slug: 'fashionco',
    description: { de: 'Moderne Mode', en: 'Modern Fashion' },
    logoUrl: null,
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  },
];

const DEMO_PRODUCTS: Product[] = [
  {
    id: 'prod-1',
    sku: 'DEMO-001',
    name: { de: 'Demo Produkt 1', en: 'Demo Product 1' },
    description: { de: 'Ein Beispielprodukt', en: 'A sample product' },
    shortDescription: { de: 'Kurzbeschreibung', en: 'Short description' },
    slug: 'demo-product-1',
    price: 99.99,
    compareAtPrice: 129.99,
    costPrice: 50.0,
    categoryId: 'cat-1',
    brandId: 'brand-1',
    isActive: true,
    isFeatured: true,
    stockQuantity: 100,
    lowStockThreshold: 10,
    weight: 0.5,
    dimensions: { length: 10, width: 5, height: 3 },
    images: [],
    variants: [],
    attributes: {},
    metaTitle: { de: 'Demo Produkt', en: 'Demo Product' },
    metaDescription: { de: 'SEO Beschreibung', en: 'SEO Description' },
    tags: ['demo', 'sample'],
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  },
  {
    id: 'prod-2',
    sku: 'DEMO-002',
    name: { de: 'Demo Produkt 2', en: 'Demo Product 2' },
    description: { de: 'Zweites Beispielprodukt', en: 'Second sample product' },
    shortDescription: { de: 'Kurz', en: 'Brief' },
    slug: 'demo-product-2',
    price: 49.99,
    compareAtPrice: null,
    costPrice: 25.0,
    categoryId: 'cat-2',
    brandId: 'brand-2',
    isActive: true,
    isFeatured: false,
    stockQuantity: 50,
    lowStockThreshold: 5,
    weight: 0.2,
    dimensions: { length: 5, width: 3, height: 1 },
    images: [],
    variants: [],
    attributes: {},
    metaTitle: { de: 'Produkt 2', en: 'Product 2' },
    metaDescription: { de: 'Beschreibung 2', en: 'Description 2' },
    tags: ['demo'],
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  },
];

function delay<T>(data: T, ms = 200): Promise<T> {
  return new Promise(resolve => setTimeout(() => resolve(data), ms));
}

export const catalogApi = {
  // =========================================================================
  // Products
  // =========================================================================

  /**
   * Fetch all products with optional filters
   */
  async getProducts(filters?: ProductFilters): Promise<PaginatedResponse<Product>> {
    if (DEMO_MODE) {
      console.warn('[CATALOG] Demo mode active - returning mock products');
      return delay({
        items: DEMO_PRODUCTS,
        total: DEMO_PRODUCTS.length,
        skip: filters?.skip || 0,
        take: filters?.take || 10,
      });
    }
    return apiClient.get<PaginatedResponse<Product>>('/api/products', {
      params: filters,
    });
  },

  /**
   * Get a single product by ID
   */
  async getProduct(id: string): Promise<Product> {
    if (DEMO_MODE) {
      const product = DEMO_PRODUCTS.find(p => p.id === id);
      if (product) return delay(product);
      throw new Error('Product not found');
    }
    return apiClient.get<Product>(`/api/products/${id}`);
  },

  /**
   * Create a new product
   */
  async createProduct(data: CreateProductRequest): Promise<Product> {
    if (DEMO_MODE) {
      console.warn('[CATALOG] Demo mode - create product simulated');
      const newProduct: Product = {
        ...data,
        id: `prod-${Date.now()}`,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      } as Product;
      return delay(newProduct);
    }
    return apiClient.post<Product>('/api/products', data);
  },

  /**
   * Update an existing product
   */
  async updateProduct(id: string, data: UpdateProductRequest): Promise<Product> {
    if (DEMO_MODE) {
      console.warn('[CATALOG] Demo mode - update product simulated');
      const existing = DEMO_PRODUCTS.find(p => p.id === id);
      return delay({
        ...existing,
        ...data,
        updatedAt: new Date().toISOString(),
      } as Product);
    }
    return apiClient.put<Product>(`/api/products/${id}`, data);
  },

  /**
   * Delete a product
   */
  async deleteProduct(id: string): Promise<void> {
    if (DEMO_MODE) {
      console.warn('[CATALOG] Demo mode - delete product simulated');
      return delay(undefined);
    }
    return apiClient.delete<void>(`/api/products/${id}`);
  },

  /**
   * Bulk import products from CSV
   */
  bulkImportProducts(file: File) {
    if (DEMO_MODE) {
      console.warn('[CATALOG] Demo mode - bulk import simulated');
      return delay({ imported: 0, errors: [] });
    }
    const formData = new FormData();
    formData.append('file', file);
    return apiClient.post<{ imported: number; errors: unknown[] }>(
      '/api/products/bulk-import',
      formData,
      { headers: { 'Content-Type': 'multipart/form-data' } }
    );
  },

  // =========================================================================
  // Categories
  // =========================================================================

  /**
   * Fetch all categories with optional filters
   */
  async getCategories(filters?: CategoryFilters): Promise<PaginatedResponse<Category>> {
    if (DEMO_MODE) {
      console.warn('[CATALOG] Demo mode active - returning mock categories');
      return delay({
        items: DEMO_CATEGORIES,
        total: DEMO_CATEGORIES.length,
        skip: filters?.skip || 0,
        take: filters?.take || 10,
      });
    }
    return apiClient.get<PaginatedResponse<Category>>('/api/categories', {
      params: filters,
    });
  },

  /**
   * Get a single category by ID
   */
  async getCategory(id: string): Promise<Category> {
    if (DEMO_MODE) {
      const category = DEMO_CATEGORIES.find(c => c.id === id);
      if (category) return delay(category);
      throw new Error('Category not found');
    }
    return apiClient.get<Category>(`/api/categories/${id}`);
  },

  /**
   * Create a new category
   */
  async createCategory(data: CreateCategoryRequest): Promise<Category> {
    if (DEMO_MODE) {
      console.warn('[CATALOG] Demo mode - create category simulated');
      const newCategory: Category = {
        ...data,
        id: `cat-${Date.now()}`,
        children: [],
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      } as Category;
      return delay(newCategory);
    }
    return apiClient.post<Category>('/api/categories', data);
  },

  /**
   * Update an existing category
   */
  async updateCategory(id: string, data: UpdateCategoryRequest): Promise<Category> {
    if (DEMO_MODE) {
      console.warn('[CATALOG] Demo mode - update category simulated');
      const existing = DEMO_CATEGORIES.find(c => c.id === id);
      return delay({
        ...existing,
        ...data,
        updatedAt: new Date().toISOString(),
      } as Category);
    }
    return apiClient.put<Category>(`/api/categories/${id}`, data);
  },

  /**
   * Delete a category
   */
  async deleteCategory(id: string): Promise<void> {
    if (DEMO_MODE) {
      console.warn('[CATALOG] Demo mode - delete category simulated');
      return delay(undefined);
    }
    return apiClient.delete<void>(`/api/categories/${id}`);
  },

  // =========================================================================
  // Brands
  // =========================================================================

  /**
   * Fetch all brands with optional filters
   */
  async getBrands(filters?: BrandFilters): Promise<PaginatedResponse<Brand>> {
    if (DEMO_MODE) {
      console.warn('[CATALOG] Demo mode active - returning mock brands');
      return delay({
        items: DEMO_BRANDS,
        total: DEMO_BRANDS.length,
        skip: filters?.skip || 0,
        take: filters?.take || 10,
      });
    }
    return apiClient.get<PaginatedResponse<Brand>>('/api/brands', {
      params: filters,
    });
  },

  /**
   * Get a single brand by ID
   */
  async getBrand(id: string): Promise<Brand> {
    if (DEMO_MODE) {
      const brand = DEMO_BRANDS.find(b => b.id === id);
      if (brand) return delay(brand);
      throw new Error('Brand not found');
    }
    return apiClient.get<Brand>(`/api/brands/${id}`);
  },

  /**
   * Create a new brand
   */
  async createBrand(data: CreateBrandRequest): Promise<Brand> {
    if (DEMO_MODE) {
      console.warn('[CATALOG] Demo mode - create brand simulated');
      const newBrand: Brand = {
        ...data,
        id: `brand-${Date.now()}`,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      } as Brand;
      return delay(newBrand);
    }
    return apiClient.post<Brand>('/api/brands', data);
  },

  /**
   * Update an existing brand
   */
  async updateBrand(id: string, data: UpdateBrandRequest): Promise<Brand> {
    if (DEMO_MODE) {
      console.warn('[CATALOG] Demo mode - update brand simulated');
      const existing = DEMO_BRANDS.find(b => b.id === id);
      return delay({
        ...existing,
        ...data,
        updatedAt: new Date().toISOString(),
      } as Brand);
    }
    return apiClient.put<Brand>(`/api/brands/${id}`, data);
  },

  /**
   * Delete a brand
   */
  async deleteBrand(id: string): Promise<void> {
    if (DEMO_MODE) {
      console.warn('[CATALOG] Demo mode - delete brand simulated');
      return delay(undefined);
    }
    return apiClient.delete<void>(`/api/brands/${id}`);
  },
};
