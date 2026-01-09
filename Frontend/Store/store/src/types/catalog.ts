import type { Product } from './index';

export interface Category {
  id: string;
  name: string;
  slug: string;
  description?: string;
  parentId?: string;
  children?: Category[];
  productCount: number;
  image?: string;
}

export interface CategoryWithProducts extends Category {
  products: Product[];
}
