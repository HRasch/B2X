# Admin Frontend Integration Guide

## Overview

This guide shows how to integrate the Admin CRUD API into the Admin Frontend application.

## Architecture

```
Admin Frontend (frontend-admin)
    ↓
Auth Service (JWT Token)
    ↓
Admin API (/api/admin/*)
    ↓ (JWT + Admin role check)
    ↓
Catalog Service Database
```

## Store Frontend vs Admin Frontend

### Store Frontend (frontend)
```
- Public Read-only API: /api/products, /api/categories, /api/brands
- No authentication required
- No CRUD operations
- Product browsing, search, filtering
```

### Admin Frontend (frontend-admin)
```
- Admin CRUD API: /api/admin/products, /api/admin/categories, /api/admin/brands, /api/admin/attributes
- JWT authentication required
- Full CRUD operations
- Product management, category management, attribute management
```

## Setup Steps

### 1. Environment Configuration

Create `.env` or `vite.config.ts` with:

```typescript
// vite.config.ts
export default defineConfig({
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5008',
        changeOrigin: true
      }
    }
  }
})
```

### 2. Create API Service

```typescript
// src/services/adminCatalogApi.ts
import axios from 'axios';

class AdminCatalogApi {
  private baseURL = '/api/admin';
  private client = axios.create({
    baseURL: this.baseURL,
    headers: {
      'Content-Type': 'application/json'
    }
  });

  // Set authorization token
  setToken(token: string) {
    this.client.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  }

  // PRODUCTS
  async getProducts() {
    const response = await this.client.get('/products');
    return response.data;
  }

  async getProduct(id: string) {
    const response = await this.client.get(`/products/${id}`);
    return response.data;
  }

  async createProduct(data: any) {
    const response = await this.client.post('/products', data);
    return response.data;
  }

  async updateProduct(id: string, data: any) {
    const response = await this.client.put(`/products/${id}`, data);
    return response.data;
  }

  async deleteProduct(id: string) {
    await this.client.delete(`/products/${id}`);
  }

  async getProductsPaged(page: number = 1, pageSize: number = 20) {
    const response = await this.client.get('/products/paged', {
      params: { page, pageSize }
    });
    return response.data;
  }

  async batchUpdateProductStatus(productIds: string[], isActive: boolean) {
    const response = await this.client.patch('/products/batch/status', {
      productIds,
      isActive
    });
    return response.data;
  }

  // CATEGORIES
  async getCategories() {
    const response = await this.client.get('/categories');
    return response.data;
  }

  async getCategory(id: string) {
    const response = await this.client.get(`/categories/${id}`);
    return response.data;
  }

  async createCategory(data: any) {
    const response = await this.client.post('/categories', data);
    return response.data;
  }

  async updateCategory(id: string, data: any) {
    const response = await this.client.put(`/categories/${id}`, data);
    return response.data;
  }

  async deleteCategory(id: string) {
    await this.client.delete(`/categories/${id}`);
  }

  async getCategoryHierarchy() {
    const response = await this.client.get('/categories/hierarchy/full');
    return response.data;
  }

  async getChildCategories(parentId: string) {
    const response = await this.client.get(`/categories/${parentId}/children`);
    return response.data;
  }

  // BRANDS
  async getBrands() {
    const response = await this.client.get('/brands');
    return response.data;
  }

  async getBrand(id: string) {
    const response = await this.client.get(`/brands/${id}`);
    return response.data;
  }

  async createBrand(data: any) {
    const response = await this.client.post('/brands', data);
    return response.data;
  }

  async updateBrand(id: string, data: any) {
    const response = await this.client.put(`/brands/${id}`, data);
    return response.data;
  }

  async deleteBrand(id: string) {
    await this.client.delete(`/brands/${id}`);
  }

  async getBrandsPaged(page: number = 1, pageSize: number = 20) {
    const response = await this.client.get('/brands/paged', {
      params: { page, pageSize }
    });
    return response.data;
  }

  // ATTRIBUTES
  async getAttributes() {
    const response = await this.client.get('/attributes');
    return response.data;
  }

  async getAttribute(id: string) {
    const response = await this.client.get(`/attributes/${id}`);
    return response.data;
  }

  async createAttribute(data: any) {
    const response = await this.client.post('/attributes', data);
    return response.data;
  }

  async updateAttribute(id: string, data: any) {
    const response = await this.client.put(`/attributes/${id}`, data);
    return response.data;
  }

  async deleteAttribute(id: string) {
    await this.client.delete(`/attributes/${id}`);
  }

  async addAttributeOption(attributeId: string, data: any) {
    const response = await this.client.post(`/attributes/${attributeId}/options`, data);
    return response.data;
  }

  async updateAttributeOption(attributeId: string, optionId: string, data: any) {
    const response = await this.client.put(
      `/attributes/${attributeId}/options/${optionId}`,
      data
    );
    return response.data;
  }

  async deleteAttributeOption(attributeId: string, optionId: string) {
    await this.client.delete(`/attributes/${attributeId}/options/${optionId}`);
  }
}

export default new AdminCatalogApi();
```

### 3. Setup Auth Context

```typescript
// src/context/AuthContext.tsx
import { createContext, useContext, useState, useEffect } from 'react';
import adminCatalogApi from '../services/adminCatalogApi';

interface AuthContextType {
  token: string | null;
  user: any | null;
  login: (token: string) => void;
  logout: () => void;
  isAdmin: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [token, setToken] = useState<string | null>(
    localStorage.getItem('authToken')
  );
  const [user, setUser] = useState<any | null>(null);

  useEffect(() => {
    if (token) {
      // Decode JWT to get user info
      const decoded = parseJwt(token);
      setUser(decoded);
      adminCatalogApi.setToken(token);
    }
  }, [token]);

  const login = (newToken: string) => {
    localStorage.setItem('authToken', newToken);
    setToken(newToken);
  };

  const logout = () => {
    localStorage.removeItem('authToken');
    setToken(null);
    setUser(null);
  };

  return (
    <AuthContext.Provider
      value={{
        token,
        user,
        login,
        logout,
        isAdmin: user?.role === 'Admin'
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
}

function parseJwt(token: string) {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(jsonPayload);
  } catch (error) {
    return null;
  }
}
```

### 4. Product Management Page

```typescript
// src/pages/ProductsPage.tsx
import { useEffect, useState } from 'react';
import adminCatalogApi from '../services/adminCatalogApi';
import { useAuth } from '../context/AuthContext';

interface Product {
  id: string;
  sku: string;
  name: { en: string; de: string; fr: string };
  price: number;
  stockQuantity: number;
  isActive: boolean;
}

export default function ProductsPage() {
  const { isAdmin } = useAuth();
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchProducts();
  }, []);

  const fetchProducts = async () => {
    setLoading(true);
    try {
      const data = await adminCatalogApi.getProducts();
      setProducts(data);
      setError(null);
    } catch (err) {
      setError(
        err instanceof Error ? err.message : 'Failed to fetch products'
      );
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (confirm('Are you sure?')) {
      try {
        await adminCatalogApi.deleteProduct(id);
        setProducts(products.filter((p) => p.id !== id));
      } catch (err) {
        setError(
          err instanceof Error ? err.message : 'Failed to delete product'
        );
      }
    }
  };

  const handleToggleActive = async (productId: string, currentStatus: boolean) => {
    try {
      const product = products.find((p) => p.id === productId);
      if (!product) return;

      await adminCatalogApi.batchUpdateProductStatus([productId], !currentStatus);
      setProducts(
        products.map((p) =>
          p.id === productId ? { ...p, isActive: !currentStatus } : p
        )
      );
    } catch (err) {
      setError(
        err instanceof Error ? err.message : 'Failed to update product'
      );
    }
  };

  if (!isAdmin) {
    return <div>Access denied. Admin role required.</div>;
  }

  if (loading) return <div>Loading...</div>;
  if (error) return <div className="error">{error}</div>;

  return (
    <div className="products-admin">
      <h1>Product Management</h1>

      <button onClick={() => window.location.href = '/products/new'}>
        + Create Product
      </button>

      <table>
        <thead>
          <tr>
            <th>SKU</th>
            <th>Name</th>
            <th>Price</th>
            <th>Stock</th>
            <th>Active</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {products.map((product) => (
            <tr key={product.id}>
              <td>{product.sku}</td>
              <td>{product.name.en}</td>
              <td>${product.price.toFixed(2)}</td>
              <td>{product.stockQuantity}</td>
              <td>
                <input
                  type="checkbox"
                  checked={product.isActive}
                  onChange={() =>
                    handleToggleActive(product.id, product.isActive)
                  }
                />
              </td>
              <td>
                <button
                  onClick={() =>
                    window.location.href = `/products/${product.id}/edit`
                  }
                >
                  Edit
                </button>
                <button onClick={() => handleDelete(product.id)}>
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
```

### 5. Create/Edit Product Form

```typescript
// src/pages/ProductFormPage.tsx
import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import adminCatalogApi from '../services/adminCatalogApi';

export default function ProductFormPage() {
  const { id } = useParams();
  const [formData, setFormData] = useState({
    sku: '',
    slug: '',
    name: { en: '', de: '', fr: '' },
    description: { en: '', de: '', fr: '' },
    shortDescription: { en: '', de: '', fr: '' },
    price: 0,
    stockQuantity: 0,
    brandId: '',
    isActive: true
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (id) {
      loadProduct();
    }
  }, [id]);

  const loadProduct = async () => {
    try {
      const product = await adminCatalogApi.getProduct(id);
      setFormData(product);
    } catch (err) {
      setError(
        err instanceof Error ? err.message : 'Failed to load product'
      );
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      if (id) {
        await adminCatalogApi.updateProduct(id, formData);
      } else {
        await adminCatalogApi.createProduct(formData);
      }
      window.location.href = '/products';
    } catch (err) {
      setError(
        err instanceof Error ? err.message : 'Failed to save product'
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <h1>{id ? 'Edit Product' : 'Create Product'}</h1>

      {error && <div className="error">{error}</div>}

      <div>
        <label>SKU:</label>
        <input
          type="text"
          value={formData.sku}
          onChange={(e) => setFormData({ ...formData, sku: e.target.value })}
        />
      </div>

      <div>
        <label>Name (English):</label>
        <input
          type="text"
          value={formData.name.en}
          onChange={(e) =>
            setFormData({
              ...formData,
              name: { ...formData.name, en: e.target.value }
            })
          }
        />
      </div>

      <div>
        <label>Price:</label>
        <input
          type="number"
          step="0.01"
          value={formData.price}
          onChange={(e) =>
            setFormData({ ...formData, price: parseFloat(e.target.value) })
          }
        />
      </div>

      <div>
        <label>Stock Quantity:</label>
        <input
          type="number"
          value={formData.stockQuantity}
          onChange={(e) =>
            setFormData({
              ...formData,
              stockQuantity: parseInt(e.target.value)
            })
          }
        />
      </div>

      <div>
        <label>
          <input
            type="checkbox"
            checked={formData.isActive}
            onChange={(e) =>
              setFormData({ ...formData, isActive: e.target.checked })
            }
          />
          Active
        </label>
      </div>

      <button type="submit" disabled={loading}>
        {loading ? 'Saving...' : 'Save Product'}
      </button>
    </form>
  );
}
```

## Error Handling

```typescript
// Handle 401 Unauthorized (Token expired)
client.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token expired, redirect to login
      window.location.href = '/login';
    } else if (error.response?.status === 403) {
      // Not authorized (not admin role)
      alert('You do not have permission to perform this action');
    }
    return Promise.reject(error);
  }
);
```

## Summary

The Admin Frontend can now:
1. ✅ Authenticate with JWT token
2. ✅ Access `/api/admin/*` endpoints with Admin role check
3. ✅ Perform CRUD operations on products, categories, brands, attributes
4. ✅ Display admin-only management interfaces
5. ✅ No overlap with store frontend (different endpoints)

**Store Frontend remains public and read-only.**
