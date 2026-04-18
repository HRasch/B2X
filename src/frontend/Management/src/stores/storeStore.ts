import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

export interface StoreInstance {
  id: string;
  name: string;
  tenantId: string;
  domain: string;
  status: 'active' | 'inactive' | 'suspended';
  createdAt: string;
  updatedAt: string;
}

export const useStoreStore = defineStore('stores', () => {
  const stores = ref<StoreInstance[]>([]);
  const selectedStore = ref<StoreInstance | null>(null);
  const loading = ref(false);

  const storeCount = computed(() => stores.value.length);

  const setStores = (newStores: StoreInstance[]) => {
    stores.value = newStores;
  };

  const selectStore = (store: StoreInstance) => {
    selectedStore.value = store;
  };

  const addStore = (store: StoreInstance) => {
    stores.value.push(store);
  };

  const updateStore = (id: string, updatedStore: Partial<StoreInstance>) => {
    const index = stores.value.findIndex(s => s.id === id);
    if (index > -1) {
      stores.value[index] = { ...stores.value[index], ...updatedStore };
      if (selectedStore.value?.id === id) {
        selectedStore.value = stores.value[index];
      }
    }
  };

  const deleteStore = (id: string) => {
    stores.value = stores.value.filter(s => s.id !== id);
    if (selectedStore.value?.id === id) {
      selectedStore.value = null;
    }
  };

  return {
    stores,
    selectedStore,
    loading,
    storeCount,
    setStores,
    selectStore,
    addStore,
    updateStore,
    deleteStore,
  };
});
