export interface UserDto {
  id: string;
  tenantId: string;
  email: string;
  firstName: string;
  lastName: string;
  avatar?: string;
  status: string;
  lastLoginAt: Date;
  emailConfirmed: boolean;
  roles?: string[];
  permissions?: string[];
}

export interface TenantDto {
  id: string;
  name: string;
  slug: string;
  description?: string;
  logoUrl?: string;
  status: string;
}

export interface CartItem {
  id: string | number;
  name: string;
  price: number;
  quantity: number;
  image?: string;
  // Compatibility fields used across components/tests
  qty?: number;
  category?: string;
  rating?: number;
  b2bPrice?: number;
  description?: string;
  inStock?: boolean;
}

export interface NuxtI18nRuntime {
  locale: { value: string };
  getLocaleMessage(locale: string): Record<string, any> | any; // eslint-disable-line @typescript-eslint/no-explicit-any
  setLocaleMessage(locale: string, messages: Record<string, any>): void; // eslint-disable-line @typescript-eslint/no-explicit-any
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: UserDto;
}

export interface ApiResponse<T> {
  data: T;
  success: boolean;
  message?: string;
  errors?: Record<string, string>;
  timestamp: Date;
}

export interface PaginatedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface LoginRequest {
  email: string;
  password: string;
  tenantId?: string;
}

export type LocaleCode = 'en' | 'de' | 'fr' | 'es' | 'it' | 'pt' | 'nl' | 'pl';

export interface Address {
  street: string;
  city: string;
  postalCode: string;
  country: string;
}

export interface Order {
  id: string;
  items: CartItem[];
  total: number;
  status: string;
  createdAt: Date;
}

export interface UserProfile {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  addresses: Address[];
}

export interface Product {
  id: string;
  name: string;
  price: number;
  b2bPrice: number;
  image: string;
  categories: string[];
  description: string;
  inStock: boolean;
  rating: number;
  sku?: string;
  brand?: string;
  tags?: string;
  material?: string;
  stockQuantity?: number;
  relevanceScore?: number;
}
