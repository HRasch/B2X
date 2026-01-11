/**
 * Users Store (Pinia)
 * Properly typed error handling and API responses
 */

import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { userService } from '@/services/api/userService';
import type { User, UserApiError } from '@/types/user';
import type { ApiError } from '@/types/api';

export const useUserStore = defineStore('users', () => {
  const users = ref<User[]>([]);
  const currentUser = ref<User | null>(null);
  const loading = ref(false);
  const error = ref<string | null>(null);
  const pagination = ref({
    page: 1,
    pageSize: 20,
    total: 0,
  });

  // Get all users
  const fetchUsers = async (page: number = 1, pageSize: number = 20) => {
    loading.value = true;
    error.value = null;
    try {
      const response = await userService.getUsers(page, pageSize);
      users.value = response.data;
      pagination.value = {
        page: response.pagination.page,
        pageSize: response.pagination.pageSize,
        total: response.pagination.total,
      };
    } catch (err: unknown) {
      const apiError = err as ApiError | UserApiError;
      error.value = apiError.message || 'Failed to fetch users';
      console.error(error.value);
    } finally {
      loading.value = false;
    }
  };

  // Get single user
  const fetchUser = async (userId: string) => {
    loading.value = true;
    error.value = null;
    try {
      const user = await userService.getUserById(userId);
      currentUser.value = user;
      return user;
    } catch (err: unknown) {
      const apiError = err as ApiError | UserApiError;
      error.value = apiError.message || 'Failed to fetch user';
      console.error(error.value);
      throw err;
    } finally {
      loading.value = false;
    }
  };

  // Create user
  const createUser = async (data: Partial<User>) => {
    loading.value = true;
    error.value = null;
    try {
      const newUser = await userService.createUser(data);
      users.value.unshift(newUser);
      return newUser;
    } catch (err: unknown) {
      const apiError = err as ApiError | UserApiError;
      error.value = apiError.message || 'Failed to create user';
      console.error(error.value);
      throw err;
    } finally {
      loading.value = false;
    }
  };

  // Update user
  const updateUser = async (userId: string, data: Partial<User>) => {
    loading.value = true;
    error.value = null;
    try {
      const updated = await userService.updateUser(userId, data);
      const index = users.value.findIndex(u => u.id === userId);
      if (index !== -1) {
        users.value[index] = updated;
      }
      if (currentUser.value?.id === userId) {
        currentUser.value = updated;
      }
      return updated;
    } catch (err: unknown) {
      const apiError = err as ApiError | UserApiError;
      error.value = apiError.message || 'Failed to update user';
      console.error(error.value);
      throw err;
    } finally {
      loading.value = false;
    }
  };

  // Delete user
  const deleteUser = async (userId: string) => {
    loading.value = true;
    error.value = null;
    try {
      await userService.deleteUser(userId);
      users.value = users.value.filter(u => u.id !== userId);
      if (currentUser.value?.id === userId) {
        currentUser.value = null;
      }
    } catch (err: unknown) {
      const apiError = err as ApiError | UserApiError;
      error.value = apiError.message || 'Failed to delete user';
      console.error(error.value);
      throw err;
    } finally {
      loading.value = false;
    }
  };

  // Search users
  const searchUsers = async (query: string) => {
    loading.value = true;
    error.value = null;
    try {
      users.value = await userService.searchUsers(query);
      return users.value;
    } catch (err: unknown) {
      const apiError = err as ApiError | UserApiError;
      error.value = apiError.message || 'Failed to search users';
      console.error(error.value);
      throw err;
    } finally {
      loading.value = false;
    }
  };

  // Clear current user
  const clearCurrentUser = () => {
    currentUser.value = null;
  };

  // Clear error
  const clearError = () => {
    error.value = null;
  };

  // Computed properties
  const hasUsers = computed(() => users.value.length > 0);
  const totalPages = computed(() => Math.ceil(pagination.value.total / pagination.value.pageSize));
  const isLoading = computed(() => loading.value);

  return {
    // State
    users,
    currentUser,
    loading,
    error,
    pagination,

    // Actions
    fetchUsers,
    fetchUser,
    createUser,
    updateUser,
    deleteUser,
    searchUsers,
    clearCurrentUser,
    clearError,

    // Computed
    hasUsers,
    totalPages,
    isLoading,
  };
});
