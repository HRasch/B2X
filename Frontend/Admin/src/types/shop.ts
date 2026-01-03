 

export interface Product {
  id: string;
  name: string;
  sku: string;
  description: string;
  basePrice: number;
  currency: string;
  images: string[];
  categoryId: string;
  attributes: Record<string, string>;
  stock: number;
  isActive: boolean;
  createdAt: Date;
  updatedAt: Date;
  tenantId: string;
}

export interface Category {
  id: string;
  name: string;
  description: string;
  parentId?: string;
  imageUrl?: string;
  slug: string;
  tenantId: string;
}

export interface PricingRule {
  id: string;
  name: string;
  type: 'fixed' | 'percentage' | 'tiered';
  conditions: PriceCondition[];
  effect: PriceEffect;
  isActive: boolean;
  startDate?: Date;
  endDate?: Date;
  tenantId: string;
}

export interface PriceCondition {
  field: 'quantity' | 'customer-type' | 'product-category';
  operator: 'equals' | 'greater' | 'less' | 'in';
  value: PriceConditionValue;
}

export type PriceConditionValue = string | number | string[];

export interface PriceEffect {
  type: 'discount' | 'markup';
  value: number;
}

export interface Discount {
  id: string;
  code: string;
  type: 'percentage' | 'fixed';
  value: number;
  maxUses?: number;
  usedCount: number;
  startDate: Date;
  endDate: Date;
  isActive: boolean;
  tenantId: string;
}

export interface ShopState {
  products: Product[];
  categories: Category[];
  currentProduct: Product | null;
  pricingRules: PricingRule[];
  discounts: Discount[];
  loading: boolean;
  error: string | null;
}

// API Response interfaces for shop operations
export interface ShopApiError {
  message: string;
  code?: string;
  details?: unknown[];
}

export interface ProductFilters {
  categoryId?: string;
  isActive?: boolean;
  search?: string;
  minPrice?: number;
  maxPrice?: number;
}

export interface PricingRuleFilters {
  isActive?: boolean;
  type?: PricingRule['type'];
  search?: string;
}

export interface DiscountFilters {
  isActive?: boolean;
  type?: Discount['type'];
  search?: string;
}
