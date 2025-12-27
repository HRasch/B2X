import { apiClient } from "../client";
import type { User, UserProfile, Address } from "@/types/user";

export const userService = {
  // Get all users
  async getUsers(page: number = 1, pageSize: number = 20): Promise<any> {
    return apiClient.get(`/api/admin/users?page=${page}&pageSize=${pageSize}`);
  },

  // Get user by ID
  async getUserById(userId: string): Promise<User> {
    return apiClient.get(`/api/admin/users/${userId}`);
  },

  // Create user
  async createUser(data: Partial<User>): Promise<User> {
    return apiClient.post("/api/admin/users", data);
  },

  // Update user
  async updateUser(userId: string, data: Partial<User>): Promise<User> {
    return apiClient.put(`/api/admin/users/${userId}`, data);
  },

  // Delete user (soft delete)
  async deleteUser(userId: string): Promise<void> {
    return apiClient.delete(`/api/admin/users/${userId}`);
  },

  // Get user profile
  async getUserProfile(userId: string): Promise<UserProfile> {
    return apiClient.get(`/api/admin/users/${userId}/profile`);
  },

  // Update user profile
  async updateUserProfile(
    userId: string,
    data: Partial<UserProfile>
  ): Promise<UserProfile> {
    return apiClient.put(`/api/admin/users/${userId}/profile`, data);
  },

  // Get user addresses
  async getUserAddresses(userId: string): Promise<Address[]> {
    return apiClient.get(`/api/admin/users/${userId}/addresses`);
  },

  // Create address
  async createAddress(
    userId: string,
    data: Partial<Address>
  ): Promise<Address> {
    return apiClient.post(`/api/admin/users/${userId}/addresses`, data);
  },

  // Update address
  async updateAddress(
    userId: string,
    addressId: string,
    data: Partial<Address>
  ): Promise<Address> {
    return apiClient.put(
      `/api/admin/users/${userId}/addresses/${addressId}`,
      data
    );
  },

  // Delete address
  async deleteAddress(userId: string, addressId: string): Promise<void> {
    return apiClient.delete(
      `/api/admin/users/${userId}/addresses/${addressId}`
    );
  },

  // Search users
  async searchUsers(query: string): Promise<User[]> {
    return apiClient.get(`/api/admin/users/search?q=${query}`);
  },

  // Verify email
  async verifyEmail(userId: string): Promise<void> {
    return apiClient.post(`/api/admin/users/${userId}/verify-email`, {});
  },

  // Reset password
  async resetPassword(userId: string, newPassword: string): Promise<void> {
    return apiClient.post(`/api/admin/users/${userId}/reset-password`, {
      password: newPassword,
    });
  },
};
