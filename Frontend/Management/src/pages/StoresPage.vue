<template>
  <div class="stores-page">
    <div class="page-header">
      <h1>Store Instances</h1>
      <button @click="isCreateModalOpen = true" class="btn-primary">+ Create Store</button>
    </div>

    <div v-if="loading" class="loading">Loading stores...</div>
    <div v-else-if="stores.length === 0" class="empty-state">
      <p>No store instances found</p>
      <button @click="isCreateModalOpen = true" class="btn-secondary">Create First Store</button>
    </div>
    <div v-else class="stores-grid">
      <div v-for="store in stores" :key="store.id" class="store-card">
        <h3>{{ store.name }}</h3>
        <p class="domain">{{ store.domain }}</p>
        <p class="status" :class="store.status">{{ store.status }}</p>
        <div class="card-actions">
          <router-link :to="`/stores/${store.id}`" class="btn-link">View Details</router-link>
          <button @click="deleteStoreInstance(store.id)" class="btn-delete">Delete</button>
        </div>
      </div>
    </div>

    <CreateStoreModal
      v-if="isCreateModalOpen"
      @close="isCreateModalOpen = false"
      @created="addNewStore"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useStoreStore } from '@/stores/storeStore';
import type { StoreInstance } from '@/stores/storeStore';
import CreateStoreModal from '@/components/CreateStoreModal.vue';

const storeStore = useStoreStore();
const stores = ref<StoreInstance[]>([]);
const loading = ref(false);
const isCreateModalOpen = ref(false);

onMounted(async () => {
  loading.value = true;
  // Fetch stores from API
  stores.value = storeStore.stores;
  loading.value = false;
});

const deleteStoreInstance = (id: string) => {
  if (confirm('Are you sure you want to delete this store?')) {
    storeStore.deleteStore(id);
    stores.value = storeStore.stores;
  }
};

const addNewStore = (store: StoreInstance) => {
  storeStore.addStore(store);
  stores.value = storeStore.stores;
  isCreateModalOpen.value = false;
};
</script>

<style scoped>
.stores-page {
  padding: 2rem 0;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.page-header h1 {
  margin: 0;
  color: #333;
}

.btn-primary {
  background-color: #667eea;
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 600;
  transition: background-color 0.3s;
}

.btn-primary:hover {
  background-color: #5568d3;
}

.loading,
.empty-state {
  text-align: center;
  padding: 3rem;
  color: #666;
}

.empty-state .btn-secondary {
  margin-top: 1rem;
  background-color: #f0f0f0;
  color: #333;
  border: 1px solid #ddd;
  padding: 0.75rem 1.5rem;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s;
}

.empty-state .btn-secondary:hover {
  background-color: #e0e0e0;
}

.stores-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 2rem;
}

.store-card {
  background: white;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition:
    transform 0.3s,
    box-shadow 0.3s;
}

.store-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
}

.store-card h3 {
  margin: 0 0 0.5rem 0;
  color: #333;
}

.domain {
  color: #667eea;
  font-size: 0.9rem;
  margin: 0.5rem 0;
}

.status {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 4px;
  font-size: 0.85rem;
  margin: 0.5rem 0;
}

.status.active {
  background-color: #d4edda;
  color: #155724;
}

.status.inactive {
  background-color: #f8d7da;
  color: #721c24;
}

.status.suspended {
  background-color: #fff3cd;
  color: #856404;
}

.card-actions {
  display: flex;
  gap: 0.5rem;
  margin-top: 1rem;
}

.btn-link,
.btn-delete {
  flex: 1;
  padding: 0.5rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.9rem;
  text-decoration: none;
  text-align: center;
  transition: background-color 0.3s;
}

.btn-link {
  background-color: #667eea;
  color: white;
}

.btn-link:hover {
  background-color: #5568d3;
}

.btn-delete {
  background-color: #ff6b6b;
  color: white;
}

.btn-delete:hover {
  background-color: #ff5252;
}
</style>
