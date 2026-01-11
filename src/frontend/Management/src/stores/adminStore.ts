import { defineStore } from 'pinia';
import { ref } from 'vue';

export interface Administrator {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: 'SuperAdmin' | 'TenantAdmin' | 'StoreManager';
  tenantId: string;
  status: 'active' | 'inactive' | 'suspended';
  lastLogin?: string;
  createdAt: string;
  updatedAt: string;
}

export const useAdminStore = defineStore('admins', () => {
  const admins = ref<Administrator[]>([]);
  const selectedAdmin = ref<Administrator | null>(null);
  const loading = ref(false);

  const setAdmins = (newAdmins: Administrator[]) => {
    admins.value = newAdmins;
  };

  const selectAdmin = (admin: Administrator) => {
    selectedAdmin.value = admin;
  };

  const addAdmin = (admin: Administrator) => {
    admins.value.push(admin);
  };

  const updateAdmin = (id: string, updatedAdmin: Partial<Administrator>) => {
    const index = admins.value.findIndex(a => a.id === id);
    if (index > -1) {
      admins.value[index] = { ...admins.value[index], ...updatedAdmin };
      if (selectedAdmin.value?.id === id) {
        selectedAdmin.value = admins.value[index];
      }
    }
  };

  const deleteAdmin = (id: string) => {
    admins.value = admins.value.filter(a => a.id !== id);
    if (selectedAdmin.value?.id === id) {
      selectedAdmin.value = null;
    }
  };

  return {
    admins,
    selectedAdmin,
    loading,
    setAdmins,
    selectAdmin,
    addAdmin,
    updateAdmin,
    deleteAdmin,
  };
});
