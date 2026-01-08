# Admin Frontend Implementation

Complete guide to the B2X Admin Frontend: components, state management, forms, and deployment.

## Overview

The Admin Frontend (`frontend-admin/`) provides a complete management interface for:

- **Catalog Management** - Products, Categories, Brands with full CRUD
- **Multi-language Support** - LocalizedContent for all entities
- **Search & Filters** - Full-text search with Elasticsearch
- **Forms** - Detailed form components for create/edit operations
- **Responsive UI** - Works on desktop, tablet, mobile

**Tech Stack:**
- Vue 3 + TypeScript
- Pinia for state management
- Vite for build/dev
- Axios for HTTP client
- Responsive CSS (Tailwind compatible)

## Project Structure

```
frontend-admin/
├── src/
│   ├── views/
│   │   ├── catalog/
│   │   │   ├── Overview.vue           # Dashboard, statistics
│   │   │   ├── Products.vue           # Product list, pagination
│   │   │   ├── Categories.vue         # Category tree view
│   │   │   ├── Brands.vue             # Brand grid
│   │   │   ├── ProductForm.vue        # Create/edit product
│   │   │   ├── CategoryForm.vue       # Create/edit category
│   │   │   └── BrandForm.vue          # Create/edit brand
│   │   └── Dashboard.vue              # Main admin dashboard
│   ├── components/
│   │   ├── common/
│   │   │   ├── Header.vue             # Navigation
│   │   │   ├── Sidebar.vue            # Menu
│   │   │   └── Pagination.vue         # Reusable pagination
│   │   └── forms/
│   │       ├── ProductFormFields.vue  # Form fields for product
│   │       └── LanguageInput.vue      # Multi-language input
│   ├── stores/
│   │   └── catalog.ts                 # Pinia store
│   ├── services/
│   │   └── api/
│   │       └── catalog.ts             # API client
│   ├── types/
│   │   └── catalog.ts                 # TypeScript types
│   ├── router/
│   │   └── index.ts                   # Vue Router config
│   └── App.vue                        # Root component
├── package.json
├── tsconfig.json
├── vite.config.ts
└── index.html
```

## Type System

**Location:** `frontend-admin/src/types/catalog.ts`

```typescript
// Core entities
export interface Product {
  id: string;
  sku: string;
  name: LocalizedContent;
  description: LocalizedContent;
  price: number;
  b2bPrice?: number;
  stockQuantity: number;
  tags: string[];
  attributes: Record<string, string>;
  imageUrls: string[];
  categoryId?: string;
  brandId?: string;
  tenantId: string;
  createdAt: string;
  updatedAt: string;
  isActive: boolean;
}

export interface Category {
  id: string;
  name: LocalizedContent;
  description: LocalizedContent;
  parentCategoryId?: string;
  iconUrl: string;
  displayOrder: number;
  tenantId: string;
  createdAt: string;
  isActive: boolean;
}

export interface Brand {
  id: string;
  name: string;
  description: string;
  logoUrl: string;
  website: string;
  tenantId: string;
  createdAt: string;
  isActive: boolean;
}

// Multi-language support
export interface LocalizedContent {
  values: Record<string, string>;  // { "en": "...", "de": "..." }
}

// Request DTOs
export interface CreateProductRequest {
  sku: string;
  name: LocalizedContent;
  description: LocalizedContent;
  price: number;
  b2bPrice?: number;
  stockQuantity: number;
  tags: string[];
  attributes: Record<string, string>;
  imageUrls: string[];
  categoryId?: string;
  brandId?: string;
}

// Filters
export interface ProductFilter {
  categoryId?: string;
  brandId?: string;
  minPrice?: number;
  maxPrice?: number;
  inStockOnly?: boolean;
  tags?: string[];
}
```

## API Service

**Location:** `frontend-admin/src/services/api/catalog.ts`

```typescript
import axios from 'axios';

const api = axios.create({
  baseURL: process.env.VITE_API_URL || 'http://localhost:9000',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${localStorage.getItem('token')}`
  }
});

// Products
export const getProducts = async (page: number = 1, pageSize: number = 20) => {
  const response = await api.get<PaginatedResponse<Product>>(
    `/api/v1/products`,
    { params: { page, pageSize } }
  );
  return response.data;
};

export const getProduct = async (id: string) => {
  const response = await api.get<Product>(`/api/v1/products/${id}`);
  return response.data;
};

export const createProduct = async (data: CreateProductRequest) => {
  const response = await api.post<Product>('/api/v1/products', data);
  return response.data;
};

export const updateProduct = async (id: string, data: Partial<CreateProductRequest>) => {
  const response = await api.put<Product>(`/api/v1/products/${id}`, data);
  return response.data;
};

export const deleteProduct = async (id: string) => {
  await api.delete(`/api/v1/products/${id}`);
};

export const searchProducts = async (query: string, filters?: ProductFilter) => {
  const response = await api.get<SearchResults>('/api/v1/products/search', {
    params: { q: query, ...filters }
  });
  return response.data;
};

// Categories (similar pattern)
export const getCategories = async () => {
  const response = await api.get<Category[]>('/api/v1/categories');
  return response.data;
};

export const createCategory = async (data: CreateCategoryRequest) => {
  const response = await api.post<Category>('/api/v1/categories', data);
  return response.data;
};

// Brands (similar pattern)
export const getBrands = async () => {
  const response = await api.get<Brand[]>('/api/v1/brands');
  return response.data;
};
```

## Pinia Store

**Location:** `frontend-admin/src/stores/catalog.ts`

```typescript
import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import * as catalogService from '@/services/api/catalog';

export const useCatalogStore = defineStore('catalog', () => {
  // State
  const products = ref<Product[]>([]);
  const categories = ref<Category[]>([]);
  const brands = ref<Brand[]>([]);
  
  const currentPage = ref(1);
  const pageSize = ref(20);
  const totalProducts = ref(0);
  
  const loading = ref(false);
  const error = ref<string | null>(null);
  const successMessage = ref<string | null>(null);
  
  const searchQuery = ref('');
  const activeFilters = ref<ProductFilter>({});

  // Computed properties
  const filteredProducts = computed(() => {
    if (!searchQuery.value) return products.value;
    
    return products.value.filter(p =>
      p.name.values['en']?.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      p.sku.toLowerCase().includes(searchQuery.value.toLowerCase())
    );
  });

  const categoryMap = computed(() =>
    Object.fromEntries(categories.value.map(c => [c.id, c]))
  );

  const brandMap = computed(() =>
    Object.fromEntries(brands.value.map(b => [b.id, b]))
  );

  // Actions
  const loadProducts = async (page = 1, pageSize = 20) => {
    loading.value = true;
    error.value = null;
    
    try {
      const response = await catalogService.getProducts(page, pageSize);
      products.value = response.items;
      totalProducts.value = response.total;
      currentPage.value = page;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load products';
    } finally {
      loading.value = false;
    }
  };

  const createProduct = async (data: CreateProductRequest) => {
    loading.value = true;
    error.value = null;
    
    try {
      const product = await catalogService.createProduct(data);
      products.value.unshift(product);
      successMessage.value = 'Product created successfully';
      setTimeout(() => { successMessage.value = null; }, 3000);
      return product;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to create product';
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const updateProduct = async (id: string, data: Partial<CreateProductRequest>) => {
    loading.value = true;
    error.value = null;
    
    try {
      const product = await catalogService.updateProduct(id, data);
      const index = products.value.findIndex(p => p.id === id);
      if (index !== -1) {
        products.value[index] = product;
      }
      successMessage.value = 'Product updated successfully';
      return product;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to update product';
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const deleteProduct = async (id: string) => {
    loading.value = true;
    error.value = null;
    
    try {
      await catalogService.deleteProduct(id);
      products.value = products.value.filter(p => p.id !== id);
      successMessage.value = 'Product deleted successfully';
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to delete product';
      throw err;
    } finally {
      loading.value = false;
    }
  };

  const searchProducts = async (query: string) => {
    searchQuery.value = query;
    
    if (!query.trim()) {
      await loadProducts(1, pageSize.value);
      return;
    }
    
    try {
      const results = await catalogService.searchProducts(query, activeFilters.value);
      products.value = results.items;
      totalProducts.value = results.total;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Search failed';
    }
  };

  return {
    // State
    products,
    categories,
    brands,
    currentPage,
    pageSize,
    totalProducts,
    loading,
    error,
    successMessage,
    searchQuery,
    activeFilters,
    
    // Computed
    filteredProducts,
    categoryMap,
    brandMap,
    
    // Actions
    loadProducts,
    createProduct,
    updateProduct,
    deleteProduct,
    searchProducts
  };
});
```

## Vue Components

### Products.vue

```vue
<template>
  <div class="products-container">
    <div class="header">
      <h1>Products</h1>
      <RouterLink to="/catalog/products/new" class="btn btn-primary">
        + New Product
      </RouterLink>
    </div>
    
    <div class="search-bar">
      <input
        v-model="searchQuery"
        type="text"
        placeholder="Search products..."
        @input="handleSearch"
      />
    </div>
    
    <div v-if="catalog.loading" class="loading">Loading...</div>
    <div v-else-if="catalog.error" class="error">{{ catalog.error }}</div>
    
    <table v-else class="products-table">
      <thead>
        <tr>
          <th>SKU</th>
          <th>Name</th>
          <th>Price</th>
          <th>Stock</th>
          <th>Category</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="product in catalog.products" :key="product.id">
          <td>{{ product.sku }}</td>
          <td>{{ product.name.values['en'] }}</td>
          <td>${{ product.price }}</td>
          <td>{{ product.stockQuantity }}</td>
          <td>{{ catalog.categoryMap[product.categoryId]?.name.values['en'] }}</td>
          <td>
            <RouterLink :to="`/catalog/products/${product.id}`" class="btn btn-sm">
              Edit
            </RouterLink>
            <button @click="deleteProduct(product.id)" class="btn btn-sm btn-danger">
              Delete
            </button>
          </td>
        </tr>
      </tbody>
    </table>
    
    <Pagination
      :current-page="catalog.currentPage"
      :total-items="catalog.totalProducts"
      :page-size="catalog.pageSize"
      @change="handlePageChange"
    />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useCatalogStore } from '@/stores/catalog';
import { useRouter } from 'vue-router';

const catalog = useCatalogStore();
const router = useRouter();
const searchQuery = ref('');

const handleSearch = async () => {
  await catalog.searchProducts(searchQuery.value);
};

const handlePageChange = async (page: number) => {
  await catalog.loadProducts(page, catalog.pageSize);
};

const deleteProduct = async (id: string) => {
  if (confirm('Are you sure?')) {
    await catalog.deleteProduct(id);
  }
};
</script>
```

## Forms

### ProductForm.vue (Placeholder)

Create/edit forms for products with multi-language support:

```vue
<template>
  <form @submit.prevent="handleSubmit">
    <div class="form-group">
      <label>SKU</label>
      <input v-model="form.sku" type="text" required />
    </div>
    
    <LanguageInput
      v-model="form.name"
      label="Product Name"
      :languages="['en', 'de']"
    />
    
    <div class="form-group">
      <label>Price</label>
      <input v-model.number="form.price" type="number" step="0.01" required />
    </div>
    
    <!-- More fields... -->
    
    <button type="submit" class="btn btn-primary">Save</button>
  </form>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useCatalogStore } from '@/stores/catalog';

const form = ref({
  sku: '',
  name: { values: { en: '', de: '' } },
  price: 0
  // ... other fields
});

const catalog = useCatalogStore();

const handleSubmit = async () => {
  await catalog.createProduct(form.value as CreateProductRequest);
};
</script>
```

## Routing

**Location:** `frontend-admin/src/router/index.ts`

```typescript
import { createRouter, createWebHistory } from 'vue-router';

const routes = [
  {
    path: '/catalog',
    name: 'Catalog',
    component: () => import('@/views/catalog/Overview.vue'),
    children: [
      {
        path: 'products',
        component: () => import('@/views/catalog/Products.vue')
      },
      {
        path: 'products/new',
        component: () => import('@/views/catalog/ProductForm.vue')
      },
      {
        path: 'products/:id',
        component: () => import('@/views/catalog/ProductForm.vue')
      },
      {
        path: 'categories',
        component: () => import('@/views/catalog/Categories.vue')
      },
      {
        path: 'brands',
        component: () => import('@/views/catalog/Brands.vue')
      }
    ]
  }
];

export default createRouter({
  history: createWebHistory(),
  routes
});
```

## Running the Admin Frontend

### Development

```bash
cd frontend-admin
npm install
npm run dev
# Opens on http://localhost:5174
```

### Build for Production

```bash
npm run build
# Creates optimized build in dist/
```

### Environment Variables

```bash
VITE_API_URL=http://localhost:9000           # Backend API URL
VITE_CATALOG_API=http://localhost:9001       # Catalog Service (if separate)
```

## Debugging in VS Code

**F5** → **"Frontend Admin (Debug)"**

- Sets VITE_API_URL and VITE_CATALOG_API
- Opens DevTools for Vue debugging
- Live reload on file changes

## Best Practices

**DO:**
- Use Pinia store for all state
- Type all components with TypeScript
- Create small, reusable components
- Use computed properties for derived data
- Handle loading/error states

**DON'T:**
- Store API data in component state
- Use synchronous operations
- Hard-code API URLs
- Skip TypeScript types
- Forget to handle errors

## References

- `.copilot-specs.md` Sections 5-9 (Frontend guidelines)
- `GETTING_STARTED.md` (Development environment)
- `VSCODE_ASPIRE_CONFIG.md` (Debug configurations)
- [Vue 3 Docs](https://vuejs.org/)
- [Pinia Docs](https://pinia.vuejs.org/)
