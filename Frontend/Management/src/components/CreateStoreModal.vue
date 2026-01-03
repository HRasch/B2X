<template>
  <div class="modal-overlay" @click="close">
    <div class="modal-content" @click.stop>
      <div class="modal-header">
        <h2>Create Store Instance</h2>
        <button @click="close" class="btn-close">×</button>
      </div>

      <form @submit.prevent="handleCreateStore">
        <div class="form-group">
          <label for="name">Store Name</label>
          <input id="name" v-model="form.name" type="text" required placeholder="My Store" />
        </div>

        <div class="form-group">
          <label for="domain">Domain</label>
          <input
            id="domain"
            v-model="form.domain"
            type="text"
            required
            placeholder="mystore.example.com"
          />
        </div>

        <div class="form-group">
          <label for="status">Status</label>
          <select id="status" v-model="form.status" required>
            <option value="active">Active</option>
            <option value="inactive">Inactive</option>
            <option value="suspended">Suspended</option>
          </select>
        </div>

        <div v-if="error" class="error-message">{{ error }}</div>

        <div class="modal-actions">
          <button type="button" @click="close" class="btn-cancel">Cancel</button>
          <button type="submit" :disabled="loading" class="btn-submit">
            {{ loading ? 'Creating...' : 'Create Store' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import type { StoreInstance } from '@/stores/storeStore';

const emit = defineEmits<{
  close: [];
  created: [store: StoreInstance];
}>();

const form = ref({
  name: '',
  domain: '',
  tenantId: '',
  status: 'active' as const,
});

const loading = ref(false);
const error = ref('');

const close = () => {
  emit('close');
};

const handleCreateStore = async () => {
  loading.value = true;
  error.value = '';

  try {
    const newStore: StoreInstance = {
      id: Math.random().toString(36).substr(2, 9),
      name: form.value.name,
      domain: form.value.domain,
      tenantId: form.value.tenantId || 'default-tenant',
      status: form.value.status,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };

    emit('created', newStore);
    form.value = { name: '', domain: '', tenantId: '', status: 'active' };
  } catch (err: unknown) {
    error.value = (err as Error).message || 'Failed to create store';
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 8px;
  max-width: 500px;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid #e0e0e0;
}

.modal-header h2 {
  margin: 0;
  color: #333;
}

.btn-close {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #666;
}

form {
  padding: 1.5rem;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  color: #333;
  margin-bottom: 0.5rem;
  font-weight: 500;
}

.form-group input,
.form-group select {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  font-family: inherit;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.error-message {
  padding: 0.75rem;
  background-color: #fee;
  color: #c33;
  border-radius: 4px;
  margin-bottom: 1.5rem;
  font-size: 0.9rem;
}

.modal-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
}

.btn-cancel,
.btn-submit {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 600;
  transition: background-color 0.3s;
}

.btn-cancel {
  background-color: #f0f0f0;
  color: #333;
}

.btn-cancel:hover {
  background-color: #e0e0e0;
}

.btn-submit {
  background-color: #667eea;
  color: white;
}

.btn-submit:hover:not(:disabled) {
  background-color: #5568d3;
}

.btn-submit:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
</style>
