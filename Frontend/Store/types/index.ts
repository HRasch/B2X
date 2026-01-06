export type LocaleCode = 'en' | 'de' | 'fr' | 'es' | 'it' | 'pt' | 'nl' | 'pl';

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

export interface Product {
  id: string;
  name: string;
  price: number;
  b2bPrice?: number;
  image?: string;
  category?: string;
  description?: string;
  inStock?: boolean;
  rating?: number;
  sku?: string;
  brand?: string;
  tags?: string;
  material?: string;
  stockQuantity?: number;
  relevanceScore?: number;
}

export interface Category {
  id: string;
  name: string;
  slug: string;
  description?: string;
  image?: string;
  parentId?: string;
  children?: Category[];
  productCount?: number;
}

export interface CartItem extends Product {
  quantity: number;
}

export interface NuxtI18nRuntime {
  locale: { value: string };
  getLocaleMessage(locale: string): Record<string, any> | any;
  setLocaleMessage(locale: string, messages: Record<string, any>): void;
}

export interface Order {
  id: string;
  userId: string;
  items: CartItem[];
  total: number;
  status: 'pending' | 'confirmed' | 'shipped' | 'delivered' | 'cancelled';
  createdAt: Date;
  shippingAddress: Address;
  billingAddress: Address;
}

export interface Address {
  firstName: string;
  lastName: string;
  company?: string;
  street: string;
  city: string;
  postalCode: string;
  country: string;
  phone?: string;
}

export interface UserProfile {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  avatar?: string;
  addresses: Address[];
  preferences: {
    language: LocaleCode;
    currency: string;
    notifications: boolean;
  };
}
