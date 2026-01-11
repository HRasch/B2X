import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { useAuthStore } from './auth';

export const useAdminStore = defineStore('admin', () => {
  const isActive = ref(false);
  const currentPage = ref<string>('');
  const editableElements = ref<Element[]>([]);

  const authStore = useAuthStore();

  // Role-based feature availability
  const canEditContent = computed(
    () => authStore.isAdmin || authStore.isContentEditor || authStore.isEditor
  );

  const canManageLayout = computed(() => authStore.isAdmin || authStore.isEditor);

  const canAccessAdminBackend = computed(() => authStore.isAdmin);

  const toggleMode = () => {
    if (!authStore.hasAdminAccess) return;

    isActive.value = !isActive.value;
    if (isActive.value) {
      scanEditableElements();
    } else {
      clearEditableElements();
    }
  };

  const scanEditableElements = () => {
    // Scan DOM for elements with data-admin-edit attribute
    const elements = Array.from(document.querySelectorAll('[data-admin-edit]'));
    editableElements.value = elements;

    // Add hover effects for editable elements
    elements.forEach(el => {
      el.classList.add('admin-editable');
    });
  };

  const clearEditableElements = () => {
    editableElements.value.forEach(el => {
      el.classList.remove('admin-editable');
    });
    editableElements.value = [];
  };

  const editElement = (elementId: string) => {
    // Implementation for editing specific element
    console.log('Edit element:', elementId);
  };

  const addContent = (type: string, position: string) => {
    // Implementation for adding content
    console.log('Add content:', type, 'at', position);
  };

  const gotoAdminBackend = (context?: string) => {
    // Open admin backend with optional context
    const adminUrl = `${window.location.origin.replace('store', 'admin')}`;
    const url = context ? `${adminUrl}/${context}` : adminUrl;
    window.open(url, '_blank');
  };

  return {
    isActive,
    currentPage,
    editableElements,
    canEditContent,
    canManageLayout,
    canAccessAdminBackend,
    toggleMode,
    scanEditableElements,
    clearEditableElements,
    editElement,
    addContent,
    gotoAdminBackend,
  };
});
