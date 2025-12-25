import { apiClient } from '../client'
import type { Product, Category, PricingRule, Discount } from '@/types/shop'
import type { PaginatedResponse, PaginationParams } from '@/types/api'

export const shopApi = {
  // Products
  getProducts(filters?: any, pagination?: PaginationParams) {
    return apiClient.get<PaginatedResponse<Product>>('/shop/products', { 
      params: { ...filters, ...pagination } 
    })
  },

  getProduct(id: string) {
    return apiClient.get<Product>(`/shop/products/${id}`)
  },

  createProduct(data: Omit<Product, 'id' | 'createdAt' | 'updatedAt'>) {
    return apiClient.post<Product>('/shop/products', data)
  },

  updateProduct(id: string, data: Partial<Product>) {
    return apiClient.put<Product>(`/shop/products/${id}`, data)
  },

  deleteProduct(id: string) {
    return apiClient.delete<void>(`/shop/products/${id}`)
  },

  bulkImportProducts(file: File) {
    const formData = new FormData()
    formData.append('file', file)
    return apiClient.post<{ imported: number; skipped: number; errors: any[] }>(
      '/shop/products/bulk-import',
      formData,
      { headers: { 'Content-Type': 'multipart/form-data' } }
    )
  },

  // Categories
  getCategories() {
    return apiClient.get<Category[]>('/shop/categories')
  },

  getCategory(id: string) {
    return apiClient.get<Category>(`/shop/categories/${id}`)
  },

  createCategory(data: Omit<Category, 'id'>) {
    return apiClient.post<Category>('/shop/categories', data)
  },

  updateCategory(id: string, data: Partial<Category>) {
    return apiClient.put<Category>(`/shop/categories/${id}`, data)
  },

  deleteCategory(id: string) {
    return apiClient.delete<void>(`/shop/categories/${id}`)
  },

  // Pricing Rules
  getPricingRules(pagination?: PaginationParams) {
    return apiClient.get<PaginatedResponse<PricingRule>>('/shop/pricing/rules', { params: pagination })
  },

  getPricingRule(id: string) {
    return apiClient.get<PricingRule>(`/shop/pricing/rules/${id}`)
  },

  createPricingRule(data: Omit<PricingRule, 'id'>) {
    return apiClient.post<PricingRule>('/shop/pricing/rules', data)
  },

  updatePricingRule(id: string, data: Partial<PricingRule>) {
    return apiClient.put<PricingRule>(`/shop/pricing/rules/${id}`, data)
  },

  deletePricingRule(id: string) {
    return apiClient.delete<void>(`/shop/pricing/rules/${id}`)
  },

  // Discounts
  getDiscounts(pagination?: PaginationParams) {
    return apiClient.get<PaginatedResponse<Discount>>('/shop/discounts', { params: pagination })
  },

  getDiscount(id: string) {
    return apiClient.get<Discount>(`/shop/discounts/${id}`)
  },

  createDiscount(data: Omit<Discount, 'id' | 'usedCount'>) {
    return apiClient.post<Discount>('/shop/discounts', data)
  },

  updateDiscount(id: string, data: Partial<Discount>) {
    return apiClient.put<Discount>(`/shop/discounts/${id}`, data)
  },

  deleteDiscount(id: string) {
    return apiClient.delete<void>(`/shop/discounts/${id}`)
  },
}
