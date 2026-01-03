<template>
  <div class="tenants">
    <h1>Tenants Management</h1>

    <div class="tenants-controls">
      <button class="btn btn-primary" @click="showCreateModal = true">+ New Tenant</button>
    </div>

    <div v-if="loading" class="loading">Loading tenants...</div>

    <div v-else-if="tenants.length === 0" class="empty-state">
      <p>No tenants found. Create your first tenant to get started.</p>
    </div>

    <div v-else class="tenants-list">
      <div v-for="tenant in tenants" :key="tenant.id" class="tenant-card">
        <h3>{{ tenant.name }}</h3>
        <p class="slug">{{ tenant.slug }}</p>
        <p v-if="tenant.description" class="description">{{ tenant.description }}</p>
        <div class="tenant-status">
          <span class="status-badge" :class="tenant.status.toLowerCase()">
            {{ tenant.status }}
          </span>
        </div>
        <div class="tenant-actions">
          <button class="btn btn-small">Edit</button>
          <button class="btn btn-small btn-danger">Delete</button>
        </div>
      </div>
    </div>

    <div v-if="showCreateModal" class="modal">
      <div class="modal-content">
        <h2>Create New Tenant</h2>
        <form @submit.prevent="createTenant">
          <div class="form-group">
            <label>Name</label>
            <input v-model="newTenant.name" type="text" required />
          </div>
          <div class="form-group">
            <label>Slug</label>
            <input v-model="newTenant.slug" type="text" required />
          </div>
          <div class="form-group">
            <label>Description</label>
            <textarea v-model="newTenant.description"></textarea>
          </div>
          <div class="modal-actions">
            <button type="submit" class="btn btn-primary">Create</button>
            <button type="button" class="btn btn-secondary" @click="showCreateModal = false">
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { api } from '../services/api';
import type { TenantDto } from '../types';

const tenants = ref<TenantDto[]>([]);
const loading = ref(false);
const showCreateModal = ref(false);
const newTenant = ref({ name: '', slug: '', description: '' });

const loadTenants = async () => {
  loading.value = true;
  try {
    const response = await api.get<TenantDto[]>('/tenants');
    tenants.value = response.data;
  } catch (error) {
    console.error('Failed to load tenants:', error);
  } finally {
    loading.value = false;
  }
};

const createTenant = async () => {
  try {
    await api.post('/tenants', newTenant.value);
    showCreateModal.value = false;
    newTenant.value = { name: '', slug: '', description: '' };
    await loadTenants();
  } catch (error) {
    console.error('Failed to create tenant:', error);
  }
};

onMounted(() => {
  loadTenants();
});
</script>

<style scoped>
.tenants {
  padding: 1rem 0;
}

h1 {
  color: #333;
  margin-bottom: 2rem;
}

.tenants-controls {
  margin-bottom: 2rem;
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.95rem;
  transition: all 0.3s;
}

.btn-primary {
  background-color: #007bff;
  color: white;
}

.btn-primary:hover {
  background-color: #0056b3;
}

.btn-small {
  padding: 0.4rem 0.8rem;
  font-size: 0.85rem;
}

.btn-danger {
  background-color: #dc3545;
  color: white;
}

.btn-danger:hover {
  background-color: #c82333;
}

.btn-secondary {
  background-color: #6c757d;
  color: white;
}

.btn-secondary:hover {
  background-color: #5a6268;
}

.loading {
  text-align: center;
  padding: 2rem;
  color: #666;
}

.empty-state {
  text-align: center;
  padding: 3rem;
  background-color: #f9f9f9;
  border-radius: 8px;
  color: #666;
}

.tenants-list {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}

.tenant-card {
  padding: 1.5rem;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  background-color: #ffffff;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.tenant-card h3 {
  margin: 0 0 0.5rem 0;
  color: #333;
}

.slug {
  color: #999;
  font-size: 0.9rem;
  margin: 0.5rem 0;
}

.description {
  color: #666;
  margin: 0.5rem 0;
  font-size: 0.95rem;
}

.tenant-status {
  margin: 1rem 0;
}

.status-badge {
  display: inline-block;
  padding: 0.3rem 0.8rem;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 500;
}

.status-badge.active {
  background-color: #d4edda;
  color: #155724;
}

.status-badge.pending {
  background-color: #fff3cd;
  color: #856404;
}

.status-badge.suspended {
  background-color: #f8d7da;
  color: #721c24;
}

.tenant-actions {
  display: flex;
  gap: 0.5rem;
  margin-top: 1rem;
}

.modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background-color: white;
  padding: 2rem;
  border-radius: 8px;
  max-width: 500px;
  width: 90%;
}

.modal-content h2 {
  margin: 0 0 1.5rem 0;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  color: #333;
  font-weight: 500;
}

.form-group input,
.form-group textarea {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  font-family: inherit;
}

.form-group input:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #007bff;
}

.modal-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
}
</style>
