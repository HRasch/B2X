<template>
  <div class="users-container">
    <!-- Header -->
    <div class="users-header">
      <div class="header-content">
        <h1>Benutzerverwaltung</h1>
        <p class="subtitle">Verwalten Sie Kundenkonten, Profile und Adressen</p>
      </div>
      <div class="header-actions">
        <router-link
          to="/users/create"
          class="btn btn-primary"
          data-testid="create-user-btn"
        >
          <i class="icon-plus"></i> Neuer Benutzer
        </router-link>
      </div>
    </div>

    <!-- Search & Filter -->
    <div class="search-filter-section">
      <div class="search-box">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Nach E-Mail, Name oder Telefon suchen..."
          @keyup.enter="handleSearch"
          class="search-input"
          data-testid="user-search-input"
        />
        <button
          @click="handleSearch"
          class="search-btn"
          data-testid="search-btn"
        >
          <i class="icon-search"></i> Suchen
        </button>
      </div>

      <div class="filter-options">
        <select v-model="filterStatus" class="filter-select">
          <option value="">Alle Status</option>
          <option value="active">Aktiv</option>
          <option value="inactive">Inaktiv</option>
        </select>

        <select v-model="sortBy" class="filter-select">
          <option value="updated">Neueste</option>
          <option value="name">Name A-Z</option>
          <option value="email">E-Mail A-Z</option>
        </select>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="userStore.isLoading" class="loading-state" data-testid="loading">
      <div class="spinner"></div>
      <p>Benutzer werden geladen...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="userStore.error" class="alert alert-danger" role="alert">
      <i class="icon-alert-circle"></i>
      <div>
        <strong>Fehler beim Laden der Benutzer</strong>
        <p>{{ userStore.error }}</p>
      </div>
      <button
        @click="userStore.clearError"
        class="close-btn"
        data-testid="close-error"
      >
        ×
      </button>
    </div>

    <!-- Empty State -->
    <div
      v-else-if="!userStore.hasUsers"
      class="empty-state"
      data-testid="empty-state"
    >
      <i class="icon-users"></i>
      <h3>Keine Benutzer gefunden</h3>
      <p>Beginnen Sie damit, neue Benutzer zu erstellen</p>
      <router-link to="/users/create" class="btn btn-primary">
        Ersten Benutzer erstellen
      </router-link>
    </div>

    <!-- Users Table -->
    <div v-else class="users-table-container">
      <table class="users-table">
        <thead>
          <tr>
            <th>Name</th>
            <th>E-Mail</th>
            <th>Telefon</th>
            <th>Status</th>
            <th>Beigetreten</th>
            <th>Letzter Login</th>
            <th>Aktionen</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in sortedUsers" :key="user.id" class="user-row">
            <td class="name-cell">
              <div class="user-info">
                <div class="user-avatar">
                  {{ user.firstName.charAt(0).toUpperCase() }}
                </div>
                <div class="user-details">
                  <strong>{{ user.firstName }} {{ user.lastName }}</strong>
                </div>
              </div>
            </td>
            <td class="email-cell">
              <a :href="`mailto:${user.email}`" class="email-link">
                {{ user.email }}
              </a>
              <span v-if="user.isEmailVerified" class="badge badge-success"
                >Verifiziert</span
              >
              <span v-else class="badge badge-warning">Unverifiziert</span>
            </td>
            <td class="phone-cell">
              {{ user.phoneNumber || "—" }}
            </td>
            <td class="status-cell">
              <span
                :class="[
                  'status-badge',
                  user.isActive ? 'status-active' : 'status-inactive',
                ]"
              >
                {{ user.isActive ? "Aktiv" : "Inaktiv" }}
              </span>
            </td>
            <td class="date-cell">
              {{ formatDate(user.createdAt) }}
            </td>
            <td class="date-cell">
              {{ formatDate(user.lastLoginAt) }}
            </td>
            <td class="actions-cell">
              <div class="action-buttons">
                <router-link
                  :to="`/users/${user.id}`"
                  class="btn-icon"
                  title="Ansehen"
                  data-testid="view-user-btn"
                >
                  <i class="icon-eye"></i>
                </router-link>
                <router-link
                  :to="`/users/${user.id}/edit`"
                  class="btn-icon"
                  title="Bearbeiten"
                  data-testid="edit-user-btn"
                >
                  <i class="icon-edit"></i>
                </router-link>
                <button
                  @click="handleDelete(user.id)"
                  class="btn-icon btn-danger"
                  title="Löschen"
                  data-testid="delete-user-btn"
                >
                  <i class="icon-trash"></i>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>

      <!-- Pagination -->
      <div v-if="userStore.totalPages > 1" class="pagination">
        <button
          @click="previousPage"
          :disabled="userStore.pagination.page === 1"
          class="pagination-btn"
        >
          ← Zurück
        </button>

        <div class="pagination-info">
          Seite {{ userStore.pagination.page }} von {{ userStore.totalPages }}
        </div>

        <button
          @click="nextPage"
          :disabled="userStore.pagination.page >= userStore.totalPages"
          class="pagination-btn"
        >
          Weiter →
        </button>
      </div>
    </div>

    <!-- Delete Confirmation Modal -->
    <div v-if="showDeleteModal" class="modal-overlay">
      <div class="modal-dialog">
        <div class="modal-header">
          <h4>Benutzer löschen?</h4>
        </div>
        <div class="modal-body">
          <p>
            Möchten Sie diesen Benutzer wirklich löschen? Diese Aktion kann
            nicht rückgängig gemacht werden.
          </p>
        </div>
        <div class="modal-footer">
          <button
            @click="showDeleteModal = false"
            class="btn btn-secondary"
            data-testid="cancel-delete-btn"
          >
            Abbrechen
          </button>
          <button
            @click="confirmDelete"
            class="btn btn-danger"
            :disabled="deleting"
            data-testid="confirm-delete-btn"
          >
            {{ deleting ? "Wird gelöscht..." : "Löschen" }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { useUserStore } from "@/stores/users";

const userStore = useUserStore();
const searchQuery = ref("");
const filterStatus = ref("");
const sortBy = ref("updated");
const showDeleteModal = ref(false);
const userToDelete = ref<string | null>(null);
const deleting = ref(false);

onMounted(async () => {
  await userStore.fetchUsers();
});

// Sorted and filtered users
const sortedUsers = computed(() => {
  let filtered = [...userStore.users];

  // Apply status filter
  if (filterStatus.value === "active") {
    filtered = filtered.filter((u) => u.isActive);
  } else if (filterStatus.value === "inactive") {
    filtered = filtered.filter((u) => !u.isActive);
  }

  // Apply sorting
  filtered.sort((a, b) => {
    switch (sortBy.value) {
      case "name":
        return `${a.firstName} ${a.lastName}`.localeCompare(
          `${b.firstName} ${b.lastName}`
        );
      case "email":
        return a.email.localeCompare(b.email);
      case "updated":
      default:
        return (
          new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime()
        );
    }
  });

  return filtered;
});

// Formatters
const formatDate = (dateStr?: string) => {
  if (!dateStr) return "—";
  const date = new Date(dateStr);
  return date.toLocaleDateString("de-DE", {
    year: "numeric",
    month: "short",
    day: "numeric",
  });
};

// Actions
const handleSearch = async () => {
  if (searchQuery.value.trim()) {
    await userStore.searchUsers(searchQuery.value);
  } else {
    await userStore.fetchUsers();
  }
};

const handleDelete = (userId: string) => {
  userToDelete.value = userId;
  showDeleteModal.value = true;
};

const confirmDelete = async () => {
  if (!userToDelete.value) return;

  deleting.value = true;
  try {
    await userStore.deleteUser(userToDelete.value);
    showDeleteModal.value = false;
    userToDelete.value = null;
  } catch (error) {
    console.error("Error deleting user:", error);
  } finally {
    deleting.value = false;
  }
};

const previousPage = async () => {
  if (userStore.pagination.page > 1) {
    await userStore.fetchUsers(userStore.pagination.page - 1);
  }
};

const nextPage = async () => {
  if (userStore.pagination.page < userStore.totalPages) {
    await userStore.fetchUsers(userStore.pagination.page + 1);
  }
};
</script>

<style scoped>
.users-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 2rem;
}

.users-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 2rem;
  padding-bottom: 1.5rem;
  border-bottom: 1px solid #e5e7eb;
}

.header-content h1 {
  margin: 0 0 0.5rem 0;
  font-size: 2rem;
  font-weight: 600;
  color: #1f2937;
}

.subtitle {
  margin: 0;
  color: #6b7280;
  font-size: 0.95rem;
}

.header-actions .btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
}

.search-filter-section {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  flex-wrap: wrap;
}

.search-box {
  flex: 1;
  min-width: 250px;
  display: flex;
  gap: 0.5rem;
}

.search-input {
  flex: 1;
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  font-size: 0.95rem;
}

.search-btn {
  padding: 0.75rem 1.5rem;
  background: #3b82f6;
  color: white;
  border: none;
  border-radius: 0.5rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: 500;
}

.search-btn:hover {
  background: #2563eb;
}

.filter-options {
  display: flex;
  gap: 0.5rem;
}

.filter-select {
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  background: white;
  font-size: 0.95rem;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 2rem;
  color: #6b7280;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #e5e7eb;
  border-top-color: #3b82f6;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.alert {
  padding: 1rem;
  margin-bottom: 2rem;
  border-radius: 0.5rem;
  display: flex;
  align-items: flex-start;
  gap: 1rem;
  position: relative;
}

.alert-danger {
  background: #fee2e2;
  color: #991b1b;
  border: 1px solid #fca5a5;
}

.alert strong {
  display: block;
  margin-bottom: 0.25rem;
}

.close-btn {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: inherit;
}

.empty-state {
  text-align: center;
  padding: 3rem 2rem;
  color: #6b7280;
}

.empty-state i {
  font-size: 3rem;
  margin-bottom: 1rem;
  color: #d1d5db;
}

.empty-state h3 {
  margin: 1rem 0;
  color: #1f2937;
}

.users-table-container {
  background: white;
  border-radius: 0.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.users-table {
  width: 100%;
  border-collapse: collapse;
}

.users-table thead {
  background: #f9fafb;
  border-bottom: 2px solid #e5e7eb;
}

.users-table th {
  padding: 1rem;
  text-align: left;
  font-weight: 600;
  color: #374151;
  font-size: 0.875rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.user-row:hover {
  background: #f9fafb;
}

.users-table td {
  padding: 1rem;
  border-bottom: 1px solid #e5e7eb;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.user-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: linear-gradient(135deg, #3b82f6, #1f2937);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 1rem;
  flex-shrink: 0;
}

.user-details strong {
  display: block;
  color: #1f2937;
  margin-bottom: 0.25rem;
}

.email-link {
  color: #3b82f6;
  text-decoration: none;
}

.email-link:hover {
  text-decoration: underline;
}

.badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
  margin-left: 0.5rem;
}

.badge-success {
  background: #dcfce7;
  color: #166534;
}

.badge-warning {
  background: #fef3c7;
  color: #92400e;
}

.status-badge {
  display: inline-block;
  padding: 0.5rem 1rem;
  border-radius: 0.25rem;
  font-weight: 500;
  font-size: 0.875rem;
}

.status-active {
  background: #dcfce7;
  color: #166534;
}

.status-inactive {
  background: #f3f4f6;
  color: #6b7280;
}

.actions-cell {
  display: flex;
  justify-content: flex-end;
}

.action-buttons {
  display: flex;
  gap: 0.5rem;
}

.btn-icon {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  background: white;
  color: #6b7280;
  cursor: pointer;
  text-decoration: none;
  transition: all 0.2s;
}

.btn-icon:hover {
  background: #f3f4f6;
  border-color: #9ca3af;
  color: #1f2937;
}

.btn-icon.btn-danger:hover {
  background: #fee2e2;
  border-color: #fca5a5;
  color: #991b1b;
}

.pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  padding: 2rem 1rem;
  border-top: 1px solid #e5e7eb;
}

.pagination-btn {
  padding: 0.75rem 1.5rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  background: white;
  color: #374151;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.2s;
}

.pagination-btn:hover:not(:disabled) {
  background: #f3f4f6;
  border-color: #9ca3af;
}

.pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination-info {
  color: #6b7280;
  font-size: 0.95rem;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-dialog {
  background: white;
  border-radius: 0.5rem;
  max-width: 400px;
  width: 90%;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
}

.modal-header {
  padding: 1.5rem;
  border-bottom: 1px solid #e5e7eb;
}

.modal-header h4 {
  margin: 0;
  color: #1f2937;
}

.modal-body {
  padding: 1.5rem;
  color: #6b7280;
}

.modal-footer {
  padding: 1.5rem;
  border-top: 1px solid #e5e7eb;
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
}

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 0.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-primary {
  background: #3b82f6;
  color: white;
}

.btn-primary:hover {
  background: #2563eb;
}

.btn-secondary {
  background: #e5e7eb;
  color: #374151;
}

.btn-secondary:hover {
  background: #d1d5db;
}

.btn-danger {
  background: #ef4444;
  color: white;
}

.btn-danger:hover:not(:disabled) {
  background: #dc2626;
}

.btn-danger:disabled {
  opacity: 0.7;
}
</style>
