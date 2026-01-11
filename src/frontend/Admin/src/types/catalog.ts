/**
 * Catalog Service Types
 * Types für Produkte, Kategorien, Marken und Validierung
 */

// ============================================================================
// Localized Content
// ============================================================================

export interface LocalizedString {
  languageCode: string;
  value: string;
}

export interface LocalizedContent {
  localizedStrings: LocalizedString[];
}

// ============================================================================
// Brand
// ============================================================================

export interface Brand {
  id: string;
  name: LocalizedContent;
  description?: LocalizedContent;
  logoUrl?: string;
  websiteUrl?: string;
  isActive: boolean;
  tenantId: string;
  createdAt: Date;
  updatedAt: Date;
}

export type CreateBrandRequest = Omit<Brand, 'id' | 'tenantId' | 'createdAt' | 'updatedAt'>;

export type UpdateBrandRequest = Partial<
  Omit<Brand, 'id' | 'tenantId' | 'createdAt' | 'updatedAt'>
>;

// ============================================================================
// Category
// ============================================================================

export interface Category {
  id: string;
  name: LocalizedContent;
  description?: LocalizedContent;
  parentCategoryId?: string;
  imageUrl?: string;
  displayOrder: number;
  isActive: boolean;
  tenantId: string;
  createdAt: Date;
  updatedAt: Date;
}

export type CreateCategoryRequest = Omit<Category, 'id' | 'tenantId' | 'createdAt' | 'updatedAt'>;

export type UpdateCategoryRequest = Partial<
  Omit<Category, 'id' | 'tenantId' | 'createdAt' | 'updatedAt'>
>;

// ============================================================================
// Product
// ============================================================================

export interface Product {
  id: string;
  sku: string;
  name: LocalizedContent;
  description?: LocalizedContent;
  basePrice: number;
  currency: string;
  stock: number;
  brandId?: string;
  categoryId: string;
  imageUrl?: string;
  tags: LocalizedContent[];
  specifications?: Record<string, string>;
  isActive: boolean;
  tenantId: string;
  createdAt: Date;
  updatedAt: Date;
}

export type CreateProductRequest = Omit<Product, 'id' | 'tenantId' | 'createdAt' | 'updatedAt'>;

export type UpdateProductRequest = Partial<
  Omit<Product, 'id' | 'tenantId' | 'createdAt' | 'updatedAt'>
>;

// ============================================================================
// API Responses
// ============================================================================

export interface CatalogError {
  field?: string;
  message: string;
  code?: string;
}

export interface ValidationError {
  status: 'ValidationFailed';
  message: string;
  errors: Record<string, string[]>;
  timestamp: Date;
}

export interface ServerError {
  status: 'ServerError';
  message: string;
  errorId: string;
  timestamp: Date;
}

export type ApiError = ValidationError | ServerError | Error;

// ============================================================================
// Filters & Search
// ============================================================================

export interface ProductFilters {
  search?: string;
  categoryId?: string;
  brandId?: string;
  minPrice?: number;
  maxPrice?: number;
  isActive?: boolean;
  skip?: number;
  take?: number;
}

export interface CategoryFilters {
  search?: string;
  parentCategoryId?: string;
  isActive?: boolean;
  skip?: number;
  take?: number;
}

export interface BrandFilters {
  search?: string;
  isActive?: boolean;
  skip?: number;
  take?: number;
}

// ============================================================================
// Paginated Response
// ============================================================================

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  skip: number;
  take: number;
  hasMore: boolean;
}
