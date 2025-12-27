import apiClient from './api'
import type { StoreInstance } from '@/stores/storeStore'

export const storeService = {
  async getStores(tenantId: string): Promise<StoreInstance[]> {
    const response = await apiClient.get(`/tenant/stores`, {
      headers: { 'X-Tenant-ID': tenantId }
    })
    return response.data.data
  },

  async getStore(id: string, tenantId: string): Promise<StoreInstance> {
    const response = await apiClient.get(`/tenant/stores/${id}`, {
      headers: { 'X-Tenant-ID': tenantId }
    })
    return response.data.data
  },

  async createStore(store: Omit<StoreInstance, 'id' | 'createdAt' | 'updatedAt'>, tenantId: string): Promise<StoreInstance> {
    const response = await apiClient.post(`/tenant/stores`, store, {
      headers: { 'X-Tenant-ID': tenantId }
    })
    return response.data.data
  },

  async updateStore(id: string, store: Partial<StoreInstance>, tenantId: string): Promise<StoreInstance> {
    const response = await apiClient.put(`/tenant/stores/${id}`, store, {
      headers: { 'X-Tenant-ID': tenantId }
    })
    return response.data.data
  },

  async deleteStore(id: string, tenantId: string): Promise<void> {
    await apiClient.delete(`/tenant/stores/${id}`, {
      headers: { 'X-Tenant-ID': tenantId }
    })
  },

  async getStoreStats(id: string, tenantId: string): Promise<{
    productCount: number
    orderCount: number
    revenue: number
    lastUpdated: string
  }> {
    const response = await apiClient.get(`/tenant/stores/${id}/stats`, {
      headers: { 'X-Tenant-ID': tenantId }
    })
    return response.data.data
  }
}
