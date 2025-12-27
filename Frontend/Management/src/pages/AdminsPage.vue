<template>
  <div class="admins-page">
    <div class="page-header">
      <h1>Administrator Management</h1>
      <button @click="isInviteModalOpen = true" class="btn-primary">+ Invite Administrator</button>
    </div>

    <div v-if="loading" class="loading">Loading administrators...</div>
    <div v-else-if="adminList.length === 0" class="empty-state">
      <p>No administrators found</p>
      <button @click="isInviteModalOpen = true" class="btn-secondary">Invite First Admin</button>
    </div>
    <div v-else class="admins-table">
      <table>
        <thead>
          <tr>
            <th>Name</th>
            <th>Email</th>
            <th>Role</th>
            <th>Status</th>
            <th>Last Login</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="admin in adminList" :key="admin.id">
            <td>{{ admin.firstName }} {{ admin.lastName }}</td>
            <td>{{ admin.email }}</td>
            <td>{{ admin.role }}</td>
            <td :class="`status ${admin.status}`">{{ admin.status }}</td>
            <td>{{ admin.lastLogin ? formatDate(admin.lastLogin) : 'Never' }}</td>
            <td class="actions">
              <router-link :to="`/admins/${admin.id}`" class="btn-link">Edit</router-link>
              <button @click="deleteAdmin(admin.id)" class="btn-delete">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <InviteAdminModal v-if="isInviteModalOpen" @close="isInviteModalOpen = false" @invited="addNewAdmin" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAdminStore } from '@/stores/adminStore'
import type { Administrator } from '@/stores/adminStore'
import InviteAdminModal from '@/components/InviteAdminModal.vue'

const adminStore = useAdminStore()
const adminList = ref<Administrator[]>([])
const loading = ref(false)
const isInviteModalOpen = ref(false)

onMounted(async () => {
  loading.value = true
  // Fetch admins from API
  adminList.value = adminStore.admins
  loading.value = false
})

const formatDate = (dateStr: string) => {
  const date = new Date(dateStr)
  return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

const deleteAdmin = (id: string) => {
  if (confirm('Are you sure you want to delete this administrator?')) {
    adminStore.deleteAdmin(id)
    adminList.value = adminStore.admins
  }
}

const addNewAdmin = (admin: Administrator) => {
  adminStore.addAdmin(admin)
  adminList.value = adminStore.admins
  isInviteModalOpen.value = false
}
</script>

<style scoped>
.admins-page {
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
}

.admins-table {
  background: white;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

table {
  width: 100%;
  border-collapse: collapse;
}

thead {
  background-color: #f5f5f5;
  border-bottom: 2px solid #e0e0e0;
}

th {
  padding: 1rem;
  text-align: left;
  font-weight: 600;
  color: #333;
}

td {
  padding: 1rem;
  border-bottom: 1px solid #e0e0e0;
}

tbody tr:hover {
  background-color: #fafafa;
}

.status {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 4px;
  font-size: 0.85rem;
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

.actions {
  display: flex;
  gap: 0.5rem;
}

.btn-link,
.btn-delete {
  padding: 0.4rem 0.8rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.85rem;
  text-decoration: none;
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
