# Admin Frontend Feature Integration Guide

**Last Updated**: December 26, 2025  
**Status**: Production-Ready  
**Version**: 1.0.0

---

## Table of Contents

1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Feature Development Workflow](#feature-development-workflow)
4. [Core Modules](#core-modules)
5. [Integration Patterns](#integration-patterns)
6. [API Integration](#api-integration)
7. [State Management](#state-management)
8. [Component Development](#component-development)
9. [Routing & Navigation](#routing--navigation)
10. [Testing Strategy](#testing-strategy)
11. [Common Use Cases](#common-use-cases)
12. [Troubleshooting](#troubleshooting)

---

## Overview

The Admin Frontend is a comprehensive management interface for B2X administrators. It provides:

- **Dashboard**: Quick stats, recent activities, quick actions
- **Content Management (CMS)**: Pages, templates, media library
- **Shop Configuration**: Products, categories, pricing, inventory
- **Job Monitoring**: Task queue, execution logs, scheduling
- **Analytics & Reporting**: Metrics, data exports
- **Tenant Management**: Multi-tenant configuration
- **System Settings**: User roles, security, configuration

### Technology Stack

| Technology | Purpose |
|-----------|---------|
| **Vue 3** | Progressive framework |
| **TypeScript** | Type-safe development |
| **Pinia** | State management |
| **Vue Router** | Routing & navigation |
| **Axios** | HTTP client |
| **Tailwind CSS** | Styling |
| **Vite** | Build tooling |
| **Vitest** | Unit testing |
| **Playwright** | E2E testing |
| **vue-i18n** | Internationalization |

---

## Architecture

### Project Structure

```
frontend-admin/
├── src/
│   ├── components/                 # Reusable components
│   │   ├── layout/                # Layout components
│   │   │   ├── MainLayout.vue     # Main app layout
│   │   │   ├── Navbar.vue         # Top navigation
│   │   │   └── Sidebar.vue        # Side navigation
│   │   ├── common/                # Shared components
│   │   │   ├── DataTable.vue      # Reusable table
│   │   │   ├── Modal.vue          # Modal dialog
│   │   │   ├── Form.vue           # Base form
│   │   │   ├── Alert.vue          # Alert component
│   │   │   └── LoadingSpinner.vue # Loading indicator
│   │   ├── catalog/               # Catalog-specific components
│   │   ├── cms/                   # CMS-specific components
│   │   └── jobs/                  # Job-specific components
│   │
│   ├── views/                     # Page-level components (routes)
│   │   ├── Dashboard.vue          # Admin dashboard
│   │   ├── auth/
│   │   │   └── Login.vue          # Login page
│   │   ├── catalog/
│   │   │   ├── Products.vue       # Product list/management
│   │   │   ├── Categories.vue     # Category management
│   │   │   ├── ProductForm.vue    # Product create/edit
│   │   │   └── CategoryForm.vue   # Category create/edit
│   │   ├── cms/
│   │   │   ├── Pages.vue          # Page management
│   │   │   ├── Templates.vue      # Template library
│   │   │   ├── MediaLibrary.vue   # Asset management
│   │   │   └── PageForm.vue       # Page editor
│   │   └── jobs/
│   │       ├── JobQueue.vue       # Active jobs
│   │       ├── JobLogs.vue        # Job history
│   │       └── JobDetail.vue      # Job details
│   │
│   ├── stores/                    # Pinia state management
│   │   ├── auth.ts                # Authentication state
│   │   ├── catalog.ts             # Catalog state
│   │   ├── cms.ts                 # CMS state
│   │   ├── jobs.ts                # Jobs state
│   │   ├── notifications.ts       # Notifications state
│   │   └── tenant.ts              # Tenant context
│   │
│   ├── services/                  # API services
│   │   ├── client.ts              # Axios configuration
│   │   └── api/
│   │       ├── auth.ts            # Auth API
│   │       ├── catalog.ts         # Catalog API
│   │       ├── cms.ts             # CMS API
│   │       ├── jobs.ts            # Jobs API
│   │       └── admin.ts           # Admin API
│   │
│   ├── types/                     # TypeScript interfaces
│   │   ├── auth.ts                # Auth types
│   │   ├── catalog.ts             # Catalog types
│   │   ├── cms.ts                 # CMS types
│   │   ├── jobs.ts                # Jobs types
│   │   ├── api.ts                 # Common API types
│   │   └── entity.ts              # Entity types
│   │
│   ├── router/                    # Routing configuration
│   │   ├── index.ts               # Router setup
│   │   ├── routes.ts              # Route definitions
│   │   └── guards.ts              # Route guards
│   │
│   ├── middleware/                # Custom middleware
│   │   ├── auth.ts                # Authentication guard
│   │   ├── tenant.ts              # Tenant context
│   │   └── permission.ts          # Role-based access
│   │
│   ├── composables/               # Reusable logic hooks
│   │   ├── useAuth.ts             # Auth utilities
│   │   ├── usePagination.ts       # Pagination logic
│   │   ├── useForm.ts             # Form handling
│   │   ├── useNotification.ts     # Toast notifications
│   │   └── useFetch.ts            # Data fetching
│   │
│   ├── utils/                     # Utility functions
│   │   ├── constants.ts           # App constants
│   │   ├── format.ts              # Format utilities
│   │   ├── validation.ts          # Form validation
│   │   └── helpers.ts             # Helper functions
│   │
│   ├── directives/                # Custom Vue directives
│   │   ├── v-focus.ts             # Auto-focus
│   │   ├── v-loading.ts           # Loading state
│   │   └── v-permission.ts        # Permission check
│   │
│   ├── locales/                   # i18n translations
│   │   ├── en.json                # English
│   │   └── de.json                # German
│   │
│   ├── styles/                    # Global styles
│   │   ├── main.css               # Global CSS
│   │   └── variables.css           # CSS variables
│   │
│   ├── App.vue                    # Root component
│   └── main.ts                    # Entry point
│
├── tests/
│   ├── setup.ts                   # Test configuration
│   ├── unit/                      # Unit tests
│   │   ├── stores/
│   │   ├── services/
│   │   ├── composables/
│   │   └── utils/
│   ├── components/                # Component tests
│   └── e2e/                       # End-to-end tests
│       ├── auth.spec.ts
│       ├── catalog.spec.ts
│       ├── cms.spec.ts
│       └── jobs.spec.ts
│
├── package.json
├── vite.config.ts
├── vitest.config.ts
├── playwright.config.ts
├── tsconfig.json
└── README.md
```

### Dependency Flow

```
Views/Pages
    ↓
Composables + Components
    ↓
Stores (Pinia)
    ↓
Services (API)
    ↓
HTTP Client (Axios)
    ↓
Backend API Gateway
```

---

## Feature Development Workflow

### Step 1: Define Feature Requirements

Create a feature specification document that includes:

```markdown
## Feature: [Feature Name]

### Requirements
- [ ] Functional requirements
- [ ] User stories
- [ ] API endpoints needed
- [ ] Data models

### Design
- [ ] UI/UX mockups
- [ ] Component hierarchy
- [ ] State structure

### Testing
- [ ] Unit test cases
- [ ] E2E test scenarios
```

### Step 2: Create Type Definitions

**File**: `src/types/[feature].ts`

```typescript
// src/types/products.ts

export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  currency: string;
  sku: string;
  status: 'active' | 'draft' | 'archived';
  categories: string[];
  images: ProductImage[];
  createdAt: string;
  updatedAt: string;
}

export interface ProductImage {
  id: string;
  url: string;
  alt: string;
  isPrimary: boolean;
}

export interface ProductFilters {
  search?: string;
  status?: Product['status'];
  category?: string;
  minPrice?: number;
  maxPrice?: number;
}

export interface ProductResponse {
  items: Product[];
  total: number;
  page: number;
  pageSize: number;
}
```

### Step 3: Create API Service

**File**: `src/services/api/[feature].ts`

```typescript
// src/services/api/products.ts

import { client } from '@/services/client';
import type { 
  Product, 
  ProductResponse, 
  ProductFilters 
} from '@/types/products';

export const productsApi = {
  async list(
    page: number = 1,
    pageSize: number = 20,
    filters?: ProductFilters
  ): Promise<ProductResponse> {
    return client.get('/api/admin/products', {
      params: { page, pageSize, ...filters }
    });
  },

  async get(id: string): Promise<Product> {
    return client.get(`/api/admin/products/${id}`);
  },

  async create(data: Omit<Product, 'id' | 'createdAt' | 'updatedAt'>): Promise<Product> {
    return client.post('/api/admin/products', data);
  },

  async update(id: string, data: Partial<Product>): Promise<Product> {
    return client.patch(`/api/admin/products/${id}`, data);
  },

  async delete(id: string): Promise<void> {
    return client.delete(`/api/admin/products/${id}`);
  },

  async bulkDelete(ids: string[]): Promise<void> {
    return client.post('/api/admin/products/bulk-delete', { ids });
  }
};
```

### Step 4: Create Pinia Store

**File**: `src/stores/[feature].ts`

```typescript
// src/stores/products.ts

import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { productsApi } from '@/services/api/products';
import type { Product, ProductFilters, ProductResponse } from '@/types/products';

export const useProductsStore = defineStore('products', () => {
  // State
  const products = ref<Product[]>([]);
  const total = ref(0);
  const page = ref(1);
  const pageSize = ref(20);
  const loading = ref(false);
  const error = ref<string | null>(null);
  const selectedProducts = ref<Set<string>>(new Set());

  // Computed
  const isLoading = computed(() => loading.value);
  const hasError = computed(() => error !== null);
  const totalPages = computed(() => Math.ceil(total.value / pageSize.value));

  // Actions
  async function fetchProducts(filters?: ProductFilters) {
    loading.value = true;
    error.value = null;
    try {
      const response = await productsApi.list(page.value, pageSize.value, filters);
      products.value = response.items;
      total.value = response.total;
      page.value = response.page;
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to fetch products';
    } finally {
      loading.value = false;
    }
  }

  async function createProduct(data: Omit<Product, 'id' | 'createdAt' | 'updatedAt'>) {
    try {
      const newProduct = await productsApi.create(data);
      products.value.unshift(newProduct);
      return newProduct;
    } catch (err) {
      throw new Error(err instanceof Error ? err.message : 'Failed to create product');
    }
  }

  async function updateProduct(id: string, data: Partial<Product>) {
    try {
      const updated = await productsApi.update(id, data);
      const index = products.value.findIndex(p => p.id === id);
      if (index !== -1) {
        products.value[index] = updated;
      }
      return updated;
    } catch (err) {
      throw new Error(err instanceof Error ? err.message : 'Failed to update product');
    }
  }

  async function deleteProduct(id: string) {
    try {
      await productsApi.delete(id);
      products.value = products.value.filter(p => p.id !== id);
      selectedProducts.value.delete(id);
    } catch (err) {
      throw new Error(err instanceof Error ? err.message : 'Failed to delete product');
    }
  }

  function toggleSelection(id: string) {
    if (selectedProducts.value.has(id)) {
      selectedProducts.value.delete(id);
    } else {
      selectedProducts.value.add(id);
    }
  }

  function clearSelection() {
    selectedProducts.value.clear();
  }

  return {
    // State
    products,
    total,
    page,
    pageSize,
    loading,
    error,
    selectedProducts,
    // Computed
    isLoading,
    hasError,
    totalPages,
    // Actions
    fetchProducts,
    createProduct,
    updateProduct,
    deleteProduct,
    toggleSelection,
    clearSelection
  };
});
```

### Step 5: Create Components

**File**: `src/components/[feature]/[Component].vue`

```vue
<!-- src/components/catalog/ProductList.vue -->

<template>
  <div class="product-list">
    <!-- Header with filters and actions -->
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold">Products</h1>
      <button 
        @click="openCreateModal"
        class="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
      >
        + New Product
      </button>
    </div>

    <!-- Search and filters -->
    <div class="mb-6 space-y-4">
      <input 
        v-model="searchQuery"
        type="text"
        placeholder="Search products..."
        class="w-full px-4 py-2 border rounded"
      />
    </div>

    <!-- Loading state -->
    <div v-if="store.isLoading" class="text-center py-8">
      <LoadingSpinner />
    </div>

    <!-- Error state -->
    <Alert 
      v-if="store.hasError" 
      type="error" 
      :message="store.error"
      @close="store.error = null"
    />

    <!-- Data table -->
    <DataTable
      v-else
      :columns="columns"
      :rows="store.products"
      :total="store.total"
      :current-page="store.page"
      :page-size="store.pageSize"
      :selectable="true"
      :selected="store.selectedProducts"
      @page-change="handlePageChange"
      @select="handleSelect"
      @row-click="handleEditProduct"
    >
      <!-- Custom columns -->
      <template #status="{ item }">
        <span 
          :class="{
            'px-2 py-1 rounded text-xs font-semibold': true,
            'bg-green-100 text-green-800': item.status === 'active',
            'bg-yellow-100 text-yellow-800': item.status === 'draft',
            'bg-gray-100 text-gray-800': item.status === 'archived'
          }"
        >
          {{ item.status }}
        </span>
      </template>

      <!-- Actions column -->
      <template #actions="{ item }">
        <div class="flex gap-2">
          <button 
            @click.stop="handleEditProduct(item)"
            class="text-blue-600 hover:text-blue-900"
          >
            Edit
          </button>
          <button 
            @click.stop="handleDeleteProduct(item.id)"
            class="text-red-600 hover:text-red-900"
          >
            Delete
          </button>
        </div>
      </template>
    </DataTable>

    <!-- Create/Edit modal -->
    <Modal 
      v-if="showModal"
      :title="editingProduct ? 'Edit Product' : 'Create Product'"
      @close="closeModal"
    >
      <ProductForm
        :product="editingProduct"
        @submit="handleSubmit"
        @cancel="closeModal"
      />
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useProductsStore } from '@/stores/products';
import type { Product } from '@/types/products';
import DataTable from '@/components/common/DataTable.vue';
import Modal from '@/components/common/Modal.vue';
import Alert from '@/components/common/Alert.vue';
import LoadingSpinner from '@/components/common/LoadingSpinner.vue';
import ProductForm from './ProductForm.vue';

const store = useProductsStore();
const searchQuery = ref('');
const showModal = ref(false);
const editingProduct = ref<Product | null>(null);

const columns = [
  { key: 'id', label: 'ID', width: '80px' },
  { key: 'name', label: 'Name' },
  { key: 'price', label: 'Price' },
  { key: 'status', label: 'Status', width: '100px' },
  { key: 'actions', label: 'Actions', width: '150px' }
];

onMounted(() => {
  store.fetchProducts();
});

const handlePageChange = (newPage: number) => {
  store.page = newPage;
  store.fetchProducts();
};

const handleSelect = (id: string) => {
  store.toggleSelection(id);
};

const openCreateModal = () => {
  editingProduct.value = null;
  showModal.value = true;
};

const handleEditProduct = (product: Product) => {
  editingProduct.value = product;
  showModal.value = true;
};

const closeModal = () => {
  showModal.value = false;
  editingProduct.value = null;
};

const handleSubmit = async (data: Omit<Product, 'id' | 'createdAt' | 'updatedAt'>) => {
  try {
    if (editingProduct.value) {
      await store.updateProduct(editingProduct.value.id, data);
    } else {
      await store.createProduct(data);
    }
    closeModal();
  } catch (err) {
    console.error('Failed to save product:', err);
  }
};

const handleDeleteProduct = async (id: string) => {
  if (confirm('Are you sure you want to delete this product?')) {
    try {
      await store.deleteProduct(id);
    } catch (err) {
      console.error('Failed to delete product:', err);
    }
  }
};
</script>

<style scoped>
.product-list {
  padding: 1.5rem;
}
</style>
```

### Step 6: Create Routes

**File**: `src/router/routes.ts`

```typescript
// Add to routes array
{
  path: '/products',
  component: () => import('@/views/catalog/Products.vue'),
  meta: { 
    requiresAuth: true,
    title: 'Products',
    breadcrumb: 'Products'
  }
},
{
  path: '/products/:id',
  component: () => import('@/views/catalog/ProductDetail.vue'),
  meta: { 
    requiresAuth: true,
    title: 'Product Details'
  }
}
```

### Step 7: Create Tests

**File**: `tests/unit/stores/products.spec.ts`

```typescript
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';
import { useProductsStore } from '@/stores/products';
import * as productsApi from '@/services/api/products';

vi.mock('@/services/api/products');

describe('Products Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia());
  });

  it('fetches products successfully', async () => {
    const store = useProductsStore();
    const mockResponse = {
      items: [
        { id: '1', name: 'Product 1', price: 99.99 }
      ],
      total: 1,
      page: 1,
      pageSize: 20
    };
    
    vi.mocked(productsApi.productsApi.list).mockResolvedValue(mockResponse);
    
    await store.fetchProducts();
    
    expect(store.products).toEqual(mockResponse.items);
    expect(store.total).toBe(1);
  });

  it('handles errors during fetch', async () => {
    const store = useProductsStore();
    const error = new Error('API Error');
    
    vi.mocked(productsApi.productsApi.list).mockRejectedValue(error);
    
    await store.fetchProducts();
    
    expect(store.error).toBe('API Error');
    expect(store.products).toEqual([]);
  });
});
```

---

## Core Modules

### Authentication Module

```typescript
// src/composables/useAuth.ts

export function useAuth() {
  const store = useAuthStore();
  
  const login = async (email: string, password: string) => {
    const response = await authApi.login(email, password);
    store.setToken(response.token);
    store.setUser(response.user);
    return response;
  };

  const logout = () => {
    store.clearAuth();
    router.push('/login');
  };

  const isAuthenticated = computed(() => store.hasToken);
  const currentUser = computed(() => store.user);
  const userRole = computed(() => store.user?.role);

  return { login, logout, isAuthenticated, currentUser, userRole };
}
```

### Pagination Module

```typescript
// src/composables/usePagination.ts

export function usePagination(pageSize = 20) {
  const page = ref(1);
  const total = ref(0);
  const items = ref([]);

  const totalPages = computed(() => Math.ceil(total.value / pageSize));
  const hasNextPage = computed(() => page.value < totalPages.value);
  const hasPrevPage = computed(() => page.value > 1);

  const goToPage = (newPage: number) => {
    if (newPage >= 1 && newPage <= totalPages.value) {
      page.value = newPage;
    }
  };

  const nextPage = () => goToPage(page.value + 1);
  const prevPage = () => goToPage(page.value - 1);
  const reset = () => { page.value = 1; };

  return {
    page, total, items, pageSize,
    totalPages, hasNextPage, hasPrevPage,
    goToPage, nextPage, prevPage, reset
  };
}
```

### Form Handling Module

```typescript
// src/composables/useForm.ts

export function useForm<T>(initialValues: T) {
  const values = ref<T>(initialValues);
  const errors = ref<Partial<Record<keyof T, string>>>({});
  const touched = ref<Partial<Record<keyof T, boolean>>>({});

  const setFieldValue = (field: keyof T, value: any) => {
    values.value[field] = value;
  };

  const setFieldError = (field: keyof T, error: string) => {
    errors.value[field] = error;
  };

  const setFieldTouched = (field: keyof T) => {
    touched.value[field] = true;
  };

  const getFieldProps = (field: keyof T) => ({
    modelValue: values.value[field],
    error: errors.value[field],
    touched: touched.value[field],
    'onUpdate:modelValue': (val: any) => setFieldValue(field, val),
    onBlur: () => setFieldTouched(field)
  });

  const resetForm = () => {
    values.value = initialValues;
    errors.value = {};
    touched.value = {};
  };

  return { values, errors, touched, setFieldValue, setFieldError, getFieldProps, resetForm };
}
```

---

## Integration Patterns

### Pattern 1: List with Search & Filters

```vue
<template>
  <div>
    <SearchBar v-model="searchQuery" @search="handleSearch" />
    <FilterPanel v-model="filters" @apply="applyFilters" />
    <DataTable :rows="items" :columns="columns" />
    <Pagination 
      :current="page"
      :total="totalPages"
      @change="changePage"
    />
  </div>
</template>

<script setup>
const searchQuery = ref('');
const filters = ref({});
const page = ref(1);
const store = useProductsStore();

const handleSearch = async (query) => {
  page.value = 1;
  await store.fetchProducts({ search: query });
};

const applyFilters = async () => {
  page.value = 1;
  await store.fetchProducts(filters.value);
};

const changePage = (newPage) => {
  page.value = newPage;
  store.page = newPage;
  store.fetchProducts();
};
</script>
```

### Pattern 2: Create/Edit Form Modal

```vue
<template>
  <Modal :open="isOpen" @close="close">
    <Form @submit="submit">
      <FormField name="title" type="text" label="Title" />
      <FormField name="description" type="textarea" label="Description" />
      <FormField name="status" type="select" label="Status" :options="statusOptions" />
      <button type="submit">{{ isEditing ? 'Update' : 'Create' }}</button>
    </Form>
  </Modal>
</template>

<script setup>
const props = defineProps({ item: Object });
const isOpen = ref(false);
const store = useItemStore();

const submit = async (formData) => {
  if (props.item) {
    await store.update(props.item.id, formData);
  } else {
    await store.create(formData);
  }
  close();
};

const close = () => { isOpen.value = false; };
const isEditing = computed(() => !!props.item);
</script>
```

### Pattern 3: Real-time Status Updates

```typescript
// src/composables/useRealtime.ts

export function useRealtime() {
  const notifications = useNotificationsStore();
  
  onMounted(() => {
    const ws = new WebSocket(import.meta.env.VITE_WS_URL);
    
    ws.onmessage = (event) => {
      const message = JSON.parse(event.data);
      
      switch (message.type) {
        case 'job:completed':
          notifications.success(`Job ${message.jobId} completed`);
          break;
        case 'job:failed':
          notifications.error(`Job ${message.jobId} failed`);
          break;
        case 'product:updated':
          store.updateProduct(message.productId, message.data);
          break;
      }
    };
    
    onUnmounted(() => ws.close());
  });
}
```

---

## API Integration

### HTTP Client Configuration

```typescript
// src/services/client.ts

import axios from 'axios';
import { useAuthStore } from '@/stores/auth';

export const client = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Request interceptor
client.interceptors.request.use((config) => {
  const auth = useAuthStore();
  if (auth.token) {
    config.headers.Authorization = `Bearer ${auth.token}`;
  }
  return config;
});

// Response interceptor
client.interceptors.response.use(
  (response) => response.data,
  (error) => {
    if (error.response?.status === 401) {
      useAuthStore().logout();
    }
    return Promise.reject(error);
  }
);
```

### API Service Pattern

```typescript
// src/services/api/cms.ts

import { client } from '@/services/client';
import type { Page, PageResponse } from '@/types/cms';

export const cmsApi = {
  pages: {
    async list(params?: any): Promise<PageResponse> {
      return client.get('/api/admin/cms/pages', { params });
    },
    
    async get(id: string): Promise<Page> {
      return client.get(`/api/admin/cms/pages/${id}`);
    },
    
    async create(data: any): Promise<Page> {
      return client.post('/api/admin/cms/pages', data);
    },
    
    async update(id: string, data: any): Promise<Page> {
      return client.patch(`/api/admin/cms/pages/${id}`, data);
    },
    
    async delete(id: string): Promise<void> {
      await client.delete(`/api/admin/cms/pages/${id}`);
    },
    
    async publish(id: string): Promise<Page> {
      return client.post(`/api/admin/cms/pages/${id}/publish`);
    },
    
    async unpublish(id: string): Promise<Page> {
      return client.post(`/api/admin/cms/pages/${id}/unpublish`);
    }
  }
};
```

---

## State Management

### Store Structure (Pinia)

```typescript
// src/stores/[feature].ts

import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

export const use[Feature]Store = defineStore('[feature]', () => {
  // ===== STATE =====
  const items = ref([]);
  const total = ref(0);
  const loading = ref(false);
  const error = ref(null);
  const selectedItems = ref(new Set());

  // ===== COMPUTED =====
  const isLoading = computed(() => loading.value);
  const hasError = computed(() => error.value !== null);
  const itemCount = computed(() => items.value.length);

  // ===== ACTIONS =====
  async function fetch(options = {}) {
    loading.value = true;
    try {
      const response = await api.list(options);
      items.value = response.items;
      total.value = response.total;
    } catch (err) {
      error.value = err.message;
    } finally {
      loading.value = false;
    }
  }

  async function create(data) {
    const item = await api.create(data);
    items.value.unshift(item);
    return item;
  }

  async function update(id, data) {
    const item = await api.update(id, data);
    const index = items.value.findIndex(i => i.id === id);
    if (index !== -1) items.value[index] = item;
    return item;
  }

  async function delete(id) {
    await api.delete(id);
    items.value = items.value.filter(i => i.id !== id);
  }

  return {
    items, total, loading, error, selectedItems,
    isLoading, hasError, itemCount,
    fetch, create, update, delete
  };
});
```

---

## Component Development

### Component Checklist

- [ ] Type-safe props with TypeScript
- [ ] Emits for parent communication
- [ ] Comprehensive slots for customization
- [ ] Error and loading states
- [ ] Accessibility (a11y)
- [ ] Responsive design
- [ ] Dark mode support (if applicable)
- [ ] Unit tests
- [ ] Storybook stories (optional)

### Reusable Component Template

```vue
<template>
  <div class="component-wrapper" :class="{ 'is-loading': loading }">
    <!-- Loading state -->
    <LoadingSpinner v-if="loading" />

    <!-- Error state -->
    <Alert v-else-if="error" type="error" :message="error" />

    <!-- Content -->
    <div v-else class="component-content">
      <slot />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

interface Props {
  loading?: boolean;
  error?: string | null;
  disabled?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  error: null,
  disabled: false
});

const emit = defineEmits<{
  action: [value: any];
}>();
</script>

<style scoped>
.component-wrapper {
  position: relative;
}

.component-wrapper.is-loading {
  opacity: 0.6;
  pointer-events: none;
}
</style>
```

---

## Routing & Navigation

### Route Definition

```typescript
// src/router/routes.ts

export const routes = [
  {
    path: '/',
    component: MainLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: 'dashboard',
        component: () => import('@/views/Dashboard.vue'),
        meta: {
          title: 'Dashboard',
          breadcrumb: 'Dashboard'
        }
      },
      {
        path: 'products',
        component: () => import('@/views/catalog/Products.vue'),
        meta: {
          title: 'Products',
          breadcrumb: 'Products',
          requiresRole: 'admin'
        }
      }
    ]
  },
  {
    path: '/login',
    component: () => import('@/views/auth/Login.vue'),
    meta: { requiresAuth: false }
  }
];
```

### Route Guards

```typescript
// src/router/guards.ts

router.beforeEach((to, from, next) => {
  const auth = useAuthStore();
  const requiresAuth = to.meta.requiresAuth !== false;
  
  if (requiresAuth && !auth.isAuthenticated) {
    next('/login');
  } else if (to.meta.requiresRole && auth.user?.role !== to.meta.requiresRole) {
    next('/unauthorized');
  } else {
    next();
  }
});
```

---

## Testing Strategy

### Unit Testing Store

```typescript
// tests/unit/stores/products.spec.ts

describe('Products Store', () => {
  let store;

  beforeEach(() => {
    setActivePinia(createPinia());
    store = useProductsStore();
  });

  describe('actions', () => {
    it('fetches products', async () => {
      await store.fetch();
      expect(store.items.length).toBeGreaterThan(0);
    });

    it('creates product', async () => {
      const product = await store.create({ name: 'Test' });
      expect(product.id).toBeDefined();
      expect(store.items[0]).toEqual(product);
    });
  });

  describe('getters', () => {
    it('returns loading state', () => {
      store.loading = true;
      expect(store.isLoading).toBe(true);
    });
  });
});
```

### Component Testing

```typescript
// tests/unit/components/DataTable.spec.ts

describe('DataTable Component', () => {
  it('renders table with data', () => {
    const { getByText } = render(DataTable, {
      props: {
        rows: [{ id: 1, name: 'Product 1' }],
        columns: [{ key: 'name', label: 'Name' }]
      }
    });
    
    expect(getByText('Product 1')).toBeDefined();
  });

  it('emits row-click event', async () => {
    const { emitted } = render(DataTable, { props: { rows: [{ id: 1 }] } });
    await userEvent.click(screen.getByRole('row'));
    expect(emitted()['row-click']).toBeDefined();
  });
});
```

### E2E Testing

```typescript
// tests/e2e/products.spec.ts

test.describe('Products Management', () => {
  test('should create product', async ({ page }) => {
    await page.goto('/products');
    await page.click('button:has-text("New Product")');
    await page.fill('[placeholder="Product name"]', 'Test Product');
    await page.click('button:has-text("Create")');
    await expect(page).toContainText('Test Product');
  });
});
```

---

## Common Use Cases

### Use Case 1: Create CRUD Feature

1. Create types in `src/types/[feature].ts`
2. Create API service in `src/services/api/[feature].ts`
3. Create Pinia store in `src/stores/[feature].ts`
4. Create components in `src/components/[feature]/`
5. Create views in `src/views/[feature]/`
6. Add routes to `src/router/routes.ts`
7. Write tests in `tests/`

### Use Case 2: Add Custom Dialog

```typescript
// src/composables/useDialog.ts

export function useDialog<T = any>() {
  const isOpen = ref(false);
  const data = ref<T | null>(null);

  const open = (value?: T) => {
    data.value = value || null;
    isOpen.value = true;
  };

  const close = () => {
    isOpen.value = false;
    data.value = null;
  };

  return { isOpen, data, open, close };
}

// Usage in component
const dialog = useDialog<Product>();

const openEdit = (product: Product) => {
  dialog.open(product);
};
```

### Use Case 3: Implement Search with Debounce

```typescript
// src/composables/useSearch.ts

export function useSearch(onSearch, delay = 500) {
  const query = ref('');
  const debounceTimer = ref<NodeJS.Timeout>();

  const search = (value: string) => {
    query.value = value;
    clearTimeout(debounceTimer.value);
    debounceTimer.value = setTimeout(() => {
      onSearch(value);
    }, delay);
  };

  onUnmounted(() => clearTimeout(debounceTimer.value));

  return { query, search };
}

// Usage
const { query, search } = useSearch((q) => store.fetch({ search: q }));
```

---

## Troubleshooting

### Common Issues

#### 1. Type Safety Errors

**Problem**: TypeScript errors in template
```vue
<error>Object is of type 'unknown' (Property 'items' does not exist)</error>
```

**Solution**: Ensure store is properly typed
```typescript
const store = useProductsStore(); // Pinia auto-types this
const items = computed(() => store.items); // Use computed
```

#### 2. State Not Updating

**Problem**: Pinia state doesn't update after API call
```typescript
// ❌ Wrong
store.items = newItems; // Direct mutation outside action

// ✅ Correct
store.updateItems(newItems); // Call action
```

#### 3. API Errors

**Problem**: 401 Unauthorized on API calls
```typescript
// Add token to all requests
client.interceptors.request.use((config) => {
  const token = useAuthStore().token;
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});
```

#### 4. Component Not Re-rendering

**Problem**: Props change but component doesn't update
```typescript
// ✅ Use computed or reactive
const computedProp = computed(() => props.item);
const reactiveData = reactive({ ...data });
```

---

## References

- [Vue 3 Documentation](https://vuejs.org)
- [Pinia Store Documentation](https://pinia.vuejs.org)
- [TypeScript Handbook](https://www.typescriptlang.org/docs)
- [Vitest Documentation](https://vitest.dev)
- [Playwright Documentation](https://playwright.dev)

---

**Maintained by**: B2X Development Team  
**Last Updated**: December 26, 2025  
**Status**: Production Ready
