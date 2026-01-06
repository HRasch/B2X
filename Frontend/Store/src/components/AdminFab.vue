<template>
  <div v-if="authStore.hasAdminAccess && adminStore.isActive" class="fixed bottom-6 right-6 z-50">
    <!-- Speed Dial Actions -->
    <TransitionGroup name="fab-actions" tag="div" class="mb-4 space-y-2">
      <!-- Content Editor Actions (for ContentEditor, Editor, Admin) -->
      <button
        v-if="adminStore.canEditContent"
        key="edit-content"
        @click="editContent"
        class="btn btn-circle btn-primary btn-sm shadow-lg hover:scale-110 transition-transform"
        title="Inhalte bearbeiten"
      >
        ✏️
      </button>

      <button
        v-if="adminStore.canEditContent"
        key="add-content"
        @click="addContent"
        class="btn btn-circle btn-secondary btn-sm shadow-lg hover:scale-110 transition-transform"
        title="Inhalt hinzufügen"
      >
        ➕
      </button>

      <!-- Layout Management (for Editor, Admin) -->
      <button
        v-if="adminStore.canManageLayout"
        key="manage-layout"
        @click="manageLayout"
        class="btn btn-circle btn-accent btn-sm shadow-lg hover:scale-110 transition-transform"
        title="Layout verwalten"
      >
        🎨
      </button>

      <!-- Admin Backend Access (for Admin only) -->
      <button
        v-if="adminStore.canAccessAdminBackend"
        key="admin-backend"
        @click="gotoAdminBackend"
        class="btn btn-circle btn-warning btn-sm shadow-lg hover:scale-110 transition-transform"
        title="Zum Admin-Backend"
      >
        🔧
      </button>
    </TransitionGroup>

    <!-- Main FAB Toggle -->
    <button
      @click="toggleDial"
      class="btn btn-circle btn-primary btn-lg shadow-lg hover:scale-110 transition-transform"
      :title="showDial ? 'Admin-Menü schließen' : 'Admin-Menü öffnen'"
    >
      <svg
        v-if="!showDial"
        class="w-6 h-6 transition-transform"
        fill="currentColor"
        viewBox="0 0 20 20"
      >
        <path
          d="M10.894 2.553a1 1 0 00-1.788 0l-7 14a1 1 0 001.169 1.409l5-1.429A1 1 0 009 15.571V11a1 1 0 112 0v4.571a1 1 0 00.725.962l5 1.428a1 1 0 001.17-1.409l-7-14z"
        />
      </svg>
      <svg
        v-else
        class="w-6 h-6 transition-transform rotate-45"
        fill="currentColor"
        viewBox="0 0 20 20"
      >
        <path
          fill-rule="evenodd"
          d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-11a1 1 0 10-2 0v2H7a1 1 0 100 2h2v2a1 1 0 102 0v-2h2a1 1 0 100-2h-2V7z"
          clip-rule="evenodd"
        />
      </svg>
    </button>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useAuthStore } from '../stores/auth';
import { useAdminStore } from '../stores/admin';

const authStore = useAuthStore();
const adminStore = useAdminStore();

const showDial = ref(false);

const toggleDial = () => {
  showDial.value = !showDial.value;
};

const editContent = () => {
  // Trigger content editing mode
  console.log('Content editing mode activated');
  // Could open a content editor modal or enable inline editing
};

const addContent = () => {
  // Open content addition dialog
  console.log('Add content dialog opened');
  // Could show a modal with content type selection
};

const manageLayout = () => {
  // Open layout management
  console.log('Layout management opened');
  // Could show layout editor or widget manager
};

const gotoAdminBackend = () => {
  adminStore.gotoAdminBackend();
};
</script>

<style scoped>
.fab-actions-enter-active,
.fab-actions-leave-active {
  transition: all 0.3s ease;
}

.fab-actions-enter-from {
  opacity: 0;
  transform: translateY(20px) scale(0.8);
}

.fab-actions-leave-to {
  opacity: 0;
  transform: translateY(20px) scale(0.8);
}

.fab-actions-move {
  transition: transform 0.3s ease;
}

/* Admin editable elements styling */
.admin-editable {
  position: relative;
  outline: 2px dashed rgba(59, 130, 246, 0.5);
  outline-offset: 2px;
}

.admin-editable:hover {
  outline-color: rgba(59, 130, 246, 0.8);
}

.admin-editable::after {
  content: '✏️';
  position: absolute;
  top: -10px;
  right: -10px;
  background: #3b82f6;
  color: white;
  border-radius: 50%;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}
</style>
