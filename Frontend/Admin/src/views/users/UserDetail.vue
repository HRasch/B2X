<template>
  <div class="user-detail-container">
    <div v-if="userStore.isLoading" class="loading">
      <div class="spinner"></div>
      <p>Benutzer wird geladen...</p>
    </div>

    <div v-else-if="userStore.error" class="alert alert-danger">
      {{ userStore.error }}
    </div>

    <div v-else-if="userStore.currentUser" class="detail-content">
      <!-- Header -->
      <div class="detail-header">
        <div class="header-top">
          <router-link to="/users" class="btn-back">← Zurück</router-link>
          <div class="header-actions">
            <router-link
              :to="`/users/${userStore.currentUser.id}/edit`"
              class="btn btn-primary"
            >
              Bearbeiten
            </router-link>
            <button @click="handleDelete" class="btn btn-danger">
              Löschen
            </button>
          </div>
        </div>

        <div class="header-profile">
          <div class="user-avatar">
            {{ userStore.currentUser.firstName.charAt(0).toUpperCase() }}
          </div>
          <div class="user-profile-info">
            <h1>
              {{ userStore.currentUser.firstName }}
              {{ userStore.currentUser.lastName }}
            </h1>
            <p class="user-email">{{ userStore.currentUser.email }}</p>
            <span
              :class="[
                'status-badge',
                userStore.currentUser.isActive
                  ? 'status-active'
                  : 'status-inactive',
              ]"
            >
              {{ userStore.currentUser.isActive ? "Aktiv" : "Inaktiv" }}
            </span>
          </div>
        </div>
      </div>

      <!-- Tabs -->
      <div class="tabs">
        <button
          v-for="tab in tabs"
          :key="tab"
          @click="activeTab = tab"
          :class="['tab-btn', { active: activeTab === tab }]"
        >
          {{ tabLabels[tab] }}
        </button>
      </div>

      <!-- Tab Content -->
      <div class="tab-content">
        <!-- Overview Tab -->
        <div v-show="activeTab === 'overview'" class="tab-pane">
          <div class="info-grid">
            <div class="info-item">
              <label>E-Mail</label>
              <p>{{ userStore.currentUser.email }}</p>
              <span
                v-if="userStore.currentUser.isEmailVerified"
                class="badge badge-success"
              >
                Verifiziert
              </span>
              <span v-else class="badge badge-warning">Unverifiziert</span>
            </div>

            <div class="info-item">
              <label>Telefon</label>
              <p>{{ userStore.currentUser.phoneNumber || "—" }}</p>
              <span
                v-if="userStore.currentUser.isPhoneVerified"
                class="badge badge-success"
              >
                Verifiziert
              </span>
              <span v-else class="badge badge-warning">Unverifiziert</span>
            </div>

            <div class="info-item">
              <label>Beigetreten</label>
              <p>{{ formatDate(userStore.currentUser.createdAt) }}</p>
            </div>

            <div class="info-item">
              <label>Letzter Login</label>
              <p>
                {{
                  userStore.currentUser.lastLoginAt
                    ? formatDate(userStore.currentUser.lastLoginAt)
                    : "Nie"
                }}
              </p>
            </div>
          </div>
        </div>

        <!-- Addresses Tab -->
        <div v-show="activeTab === 'addresses'" class="tab-pane">
          <div v-if="loadingAddresses" class="loading">
            <div class="spinner"></div>
          </div>

          <div v-else-if="addresses.length === 0" class="empty-state">
            <p>Keine Adressen gespeichert</p>
            <button @click="showAddressForm = true" class="btn btn-primary">
              Adresse hinzufügen
            </button>
          </div>

          <div v-else class="addresses-list">
            <div
              v-for="address in addresses"
              :key="address.id"
              class="address-card"
            >
              <div class="address-header">
                <h3>{{ address.recipientName }}</h3>
                <span class="address-type">{{ address.addressType }}</span>
              </div>
              <p>{{ address.streetAddress }}</p>
              <p v-if="address.streetAddress2">{{ address.streetAddress2 }}</p>
              <p>{{ address.postalCode }} {{ address.city }}</p>
              <p v-if="address.state">{{ address.state }}</p>
              <p>{{ address.country }}</p>
              <button
                @click="deleteAddress(address.id)"
                class="btn-icon btn-danger"
              >
                Löschen
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Delete Modal -->
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
            <button @click="showDeleteModal = false" class="btn btn-secondary">
              Abbrechen
            </button>
            <button
              @click="confirmDelete"
              class="btn btn-danger"
              :disabled="deleting"
            >
              {{ deleting ? "Wird gelöscht..." : "Löschen" }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useUserStore } from "@/stores/users";
import { userService } from "@/services/api/userService";
import type { Address } from "@/types/user";

const route = useRoute();
const router = useRouter();
const userStore = useUserStore();

const activeTab = ref("overview");
const tabs = ["overview", "addresses"];
const tabLabels = {
  overview: "Übersicht",
  addresses: "Adressen",
};

const addresses = ref<Address[]>([]);
const loadingAddresses = ref(false);
const showDeleteModal = ref(false);
const showAddressForm = ref(false);
const deleting = ref(false);

onMounted(async () => {
  if (route.params.id) {
    await userStore.fetchUser(route.params.id as string);
    await loadAddresses();
  }
});

const loadAddresses = async () => {
  if (!userStore.currentUser) return;

  loadingAddresses.value = true;
  try {
    addresses.value = await userService.getUserAddresses(
      userStore.currentUser.id
    );
  } catch (error) {
    console.error("Error loading addresses:", error);
  } finally {
    loadingAddresses.value = false;
  }
};

const formatDate = (dateStr: string) => {
  const date = new Date(dateStr);
  return date.toLocaleDateString("de-DE", {
    year: "numeric",
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
};

const deleteAddress = async (addressId: string) => {
  if (!userStore.currentUser) return;

  try {
    await userService.deleteAddress(userStore.currentUser.id, addressId);
    addresses.value = addresses.value.filter((a) => a.id !== addressId);
  } catch (error) {
    console.error("Error deleting address:", error);
  }
};

const handleDelete = () => {
  showDeleteModal.value = true;
};

const confirmDelete = async () => {
  if (!userStore.currentUser) return;

  deleting.value = true;
  try {
    await userStore.deleteUser(userStore.currentUser.id);
    await router.push("/users");
  } catch (error) {
    console.error("Error deleting user:", error);
  } finally {
    deleting.value = false;
    showDeleteModal.value = false;
  }
};
</script>

<style scoped>
.user-detail-container {
  max-width: 1000px;
  margin: 0 auto;
  padding: 2rem;
}

.loading {
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

.alert-danger {
  background: #fee2e2;
  color: #991b1b;
  border: 1px solid #fca5a5;
  padding: 1rem;
  border-radius: 0.5rem;
}

.detail-content {
  background: white;
  border-radius: 0.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.detail-header {
  padding: 2rem;
  border-bottom: 1px solid #e5e7eb;
}

.header-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.btn-back {
  color: #3b82f6;
  text-decoration: none;
  font-weight: 500;
}

.btn-back:hover {
  color: #2563eb;
}

.header-actions {
  display: flex;
  gap: 0.5rem;
}

.header-profile {
  display: flex;
  align-items: flex-start;
  gap: 1.5rem;
}

.user-avatar {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background: linear-gradient(135deg, #3b82f6, #1f2937);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 2rem;
  flex-shrink: 0;
}

.user-profile-info h1 {
  margin: 0 0 0.5rem 0;
  font-size: 1.875rem;
  color: #1f2937;
}

.user-email {
  margin: 0 0 0.75rem 0;
  color: #6b7280;
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

.tabs {
  display: flex;
  border-bottom: 2px solid #e5e7eb;
  background: #f9fafb;
}

.tab-btn {
  padding: 1rem 1.5rem;
  border: none;
  background: none;
  color: #6b7280;
  font-weight: 500;
  cursor: pointer;
  border-bottom: 3px solid transparent;
  transition: all 0.2s;
}

.tab-btn:hover {
  color: #1f2937;
}

.tab-btn.active {
  color: #3b82f6;
  border-bottom-color: #3b82f6;
}

.tab-content {
  padding: 2rem;
}

.tab-pane {
  display: block;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
}

.info-item {
  padding: 1rem;
  background: #f9fafb;
  border-radius: 0.5rem;
  border: 1px solid #e5e7eb;
}

.info-item label {
  display: block;
  font-weight: 600;
  color: #374151;
  margin-bottom: 0.5rem;
  text-transform: uppercase;
  font-size: 0.75rem;
  letter-spacing: 0.05em;
}

.info-item p {
  margin: 0 0 0.5rem 0;
  color: #1f2937;
  font-size: 1.125rem;
}

.badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.badge-success {
  background: #dcfce7;
  color: #166534;
}

.badge-warning {
  background: #fef3c7;
  color: #92400e;
}

.empty-state {
  text-align: center;
  padding: 2rem;
  color: #6b7280;
}

.addresses-list {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}

.address-card {
  padding: 1.5rem;
  border: 1px solid #e5e7eb;
  border-radius: 0.5rem;
  background: #f9fafb;
}

.address-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid #e5e7eb;
}

.address-header h3 {
  margin: 0;
  color: #1f2937;
}

.address-type {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  background: #e0e7ff;
  color: #3730a3;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
}

.address-card p {
  margin: 0.5rem 0;
  color: #6b7280;
  line-height: 1.5;
}

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 0.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  text-decoration: none;
  display: inline-block;
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
  cursor: not-allowed;
}

.btn-icon {
  padding: 0.5rem 1rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  background: white;
  color: #6b7280;
  cursor: pointer;
  font-size: 0.875rem;
  font-weight: 500;
}

.btn-icon:hover {
  background: #f3f4f6;
  border-color: #9ca3af;
}

.btn-icon.btn-danger {
  border-color: #fca5a5;
  color: #991b1b;
}

.btn-icon.btn-danger:hover {
  background: #fee2e2;
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
</style>
