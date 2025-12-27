import { apiClient } from "../client";

export interface UserAccount {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: string[];
  tenantId: string;
  isActive: boolean;
  isTwoFactorEnabled: boolean;
}

export interface CreateUserRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  tenantId?: string;
  roles?: string[];
}

export interface UpdateUserRequest {
  firstName?: string;
  lastName?: string;
  roles?: string[];
  isActive?: boolean;
}

export interface UsersResponse {
  data: UserAccount[];
  total: number;
}

const baseURL = import.meta.env.VITE_ADMIN_API_URL || "/api";

export const usersApi = {
  /**
   * Get all users
   */
  async getAll(params?: {
    page?: number;
    pageSize?: number;
    search?: string;
  }): Promise<UserAccount[]> {
    const response = await apiClient.get<UsersResponse | UserAccount[]>(
      "/api/auth/users",
      { params }
    );
    // Handle both array and wrapped response
    return Array.isArray(response)
      ? response
      : (response as UsersResponse).data;
  },

  /**
   * Get a single user by ID
   */
  async getById(id: string): Promise<UserAccount> {
    const response = await apiClient.get<UserAccount>(`/api/auth/users/${id}`);
    return response;
  },

  /**
   * Create a new user
   */
  async create(user: CreateUserRequest): Promise<UserAccount> {
    const response = await apiClient.post<UserAccount>("/api/auth/users", user);
    return response;
  },

  /**
   * Update an existing user
   */
  async update(id: string, user: UpdateUserRequest): Promise<UserAccount> {
    const response = await apiClient.put<UserAccount>(
      `/api/auth/users/${id}`,
      user
    );
    return response;
  },

  /**
   * Toggle user active status
   */
  async toggleStatus(id: string, isActive: boolean): Promise<void> {
    await apiClient.patch(`/api/auth/users/${id}/status`, { isActive });
  },

  /**
   * Delete a user (soft delete)
   */
  async delete(id: string): Promise<void> {
    await apiClient.delete(`/api/auth/users/${id}`);
  },

  /**
   * Reset user password
   */
  async resetPassword(id: string): Promise<{ temporaryPassword: string }> {
    const response = await apiClient.post<{ temporaryPassword: string }>(
      `/api/auth/users/${id}/reset-password`
    );
    return response;
  },

  /**
   * Assign roles to a user
   */
  async assignRoles(id: string, roles: string[]): Promise<void> {
    await apiClient.put(`/api/auth/users/${id}/roles`, { roles });
  },
};

export default usersApi;
