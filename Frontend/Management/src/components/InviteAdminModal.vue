<template>
  <div class="modal-overlay" @click="close">
    <div class="modal-content" @click.stop>
      <div class="modal-header">
        <h2>Invite Administrator</h2>
        <button @click="close" class="btn-close">×</button>
      </div>

      <form @submit.prevent="handleInviteAdmin">
        <div class="form-group">
          <label for="email">Email Address</label>
          <input
            id="email"
            v-model="form.email"
            type="email"
            required
            placeholder="admin@example.com"
          />
        </div>

        <div class="form-group">
          <label for="firstName">First Name</label>
          <input
            id="firstName"
            v-model="form.firstName"
            type="text"
            required
            placeholder="John"
          />
        </div>

        <div class="form-group">
          <label for="lastName">Last Name</label>
          <input
            id="lastName"
            v-model="form.lastName"
            type="text"
            required
            placeholder="Doe"
          />
        </div>

        <div class="form-group">
          <label for="role">Role</label>
          <select id="role" v-model="form.role" required>
            <option value="TenantAdmin">Tenant Admin</option>
            <option value="StoreManager">Store Manager</option>
            <option value="SuperAdmin">Super Admin</option>
          </select>
        </div>

        <div v-if="error" class="error-message">{{ error }}</div>

        <div class="modal-actions">
          <button type="button" @click="close" class="btn-cancel">Cancel</button>
          <button type="submit" :disabled="loading" class="btn-submit">
            {{ loading ? 'Sending Invite...' : 'Send Invite' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import type { Administrator } from '@/stores/adminStore'

const emit = defineEmits<{
  close: []
  invited: [admin: Administrator]
}>()

const form = ref({
  email: '',
  firstName: '',
  lastName: '',
  role: 'TenantAdmin' as 'TenantAdmin' | 'StoreManager' | 'SuperAdmin'
})

const loading = ref(false)
const error = ref('')

const close = () => {
  emit('close')
}

const handleInviteAdmin = async () => {
  loading.value = true
  error.value = ''

  try {
    const newAdmin: Administrator = {
      id: Math.random().toString(36).substr(2, 9),
      email: form.value.email,
      firstName: form.value.firstName,
      lastName: form.value.lastName,
      role: form.value.role,
      tenantId: 'default-tenant',
      status: 'active',
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    }

    emit('invited', newAdmin)
    form.value = { email: '', firstName: '', lastName: '', role: 'TenantAdmin' }
  } catch (err: any) {
    error.value = err.message || 'Failed to invite administrator'
  } finally {
    loading.value = false
  }
}
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
