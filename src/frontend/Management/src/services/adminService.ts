import apiClient from './api';
import type { Administrator } from '@/stores/adminStore';

export const adminService = {
  async getAdmins(tenantId: string): Promise<Administrator[]> {
    const response = await apiClient.get(`/tenant/admins`, {
      headers: { 'X-Tenant-ID': tenantId },
    });
    return response.data.data;
  },

  async getAdmin(id: string, tenantId: string): Promise<Administrator> {
    const response = await apiClient.get(`/tenant/admins/${id}`, {
      headers: { 'X-Tenant-ID': tenantId },
    });
    return response.data.data;
  },

  async createAdmin(
    admin: Omit<Administrator, 'id' | 'createdAt' | 'updatedAt'>,
    tenantId: string
  ): Promise<Administrator> {
    const response = await apiClient.post(`/tenant/admins`, admin, {
      headers: { 'X-Tenant-ID': tenantId },
    });
    return response.data.data;
  },

  async updateAdmin(
    id: string,
    admin: Partial<Administrator>,
    tenantId: string
  ): Promise<Administrator> {
    const response = await apiClient.put(`/tenant/admins/${id}`, admin, {
      headers: { 'X-Tenant-ID': tenantId },
    });
    return response.data.data;
  },

  async deleteAdmin(id: string, tenantId: string): Promise<void> {
    await apiClient.delete(`/tenant/admins/${id}`, {
      headers: { 'X-Tenant-ID': tenantId },
    });
  },

  async inviteAdmin(
    email: string,
    role: string,
    tenantId: string
  ): Promise<{ invitationId: string }> {
    const response = await apiClient.post(
      `/tenant/admins/invite`,
      { email, role },
      {
        headers: { 'X-Tenant-ID': tenantId },
      }
    );
    return response.data.data;
  },
};
