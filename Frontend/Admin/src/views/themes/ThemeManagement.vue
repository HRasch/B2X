<template>
  <div class="container mx-auto p-6">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 mb-2">Theme Management</h1>
      <p class="text-gray-600">
        Manage tenant themes, SCSS files, and design variables
      </p>
    </div>

    <!-- Error Alert -->
    <div
      v-if="error"
      class="mb-6 bg-red-50 border border-red-200 rounded-md p-4"
    >
      <div class="flex">
        <div class="shrink-0">
          <svg
            class="h-5 w-5 text-red-400"
            viewBox="0 0 20 20"
            fill="currentColor"
          >
            <path
              fill-rule="evenodd"
              d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z"
              clip-rule="evenodd"
            />
          </svg>
        </div>
        <div class="ml-3">
          <p class="text-sm text-red-800">{{ error }}</p>
        </div>
      </div>
    </div>

    <!-- Theme List -->
    <div class="bg-white rounded-lg shadow-sm border border-gray-200 mb-6">
      <div
        class="px-6 py-4 border-b border-gray-200 flex justify-between items-center"
      >
        <h2 class="text-xl font-semibold text-gray-900">Themes</h2>
        <div class="flex space-x-2">
          <button
            @click="createSampleTheme"
            class="px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            :disabled="loading"
          >
            Create Sample Theme
          </button>
          <button
            @click="showCreateModal = true"
            class="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
            :disabled="loading"
          >
            Create Theme
          </button>
        </div>
      </div>

      <div class="p-6">
        <!-- Loading State -->
        <div v-if="loading && themes.length === 0" class="text-center py-12">
          <div
            class="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600 mx-auto"
          ></div>
          <p class="mt-2 text-sm text-gray-500">Loading themes...</p>
        </div>

        <!-- Empty State -->
        <div v-else-if="themes.length === 0" class="text-center py-12">
          <svg
            class="mx-auto h-12 w-12 text-gray-400"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M7 21a4 4 0 01-4-4V5a2 2 0 012-2h4a2 2 0 012 2v12a4 4 0 01-4 4zM21 5a2 2 0 00-2-2h-4a2 2 0 00-2 2v12a4 4 0 004 4h4a2 2 0 002-2V5z"
            />
          </svg>
          <h3 class="mt-2 text-sm font-medium text-gray-900">No themes</h3>
          <p class="mt-1 text-sm text-gray-500">
            Get started by creating a new theme
          </p>
        </div>

        <!-- Themes List -->
        <div v-else class="space-y-4">
          <div
            v-for="theme in themes"
            :key="theme.id"
            class="border border-gray-200 rounded-lg p-4 hover:bg-gray-50"
          >
            <div class="flex justify-between items-start">
              <div class="flex-1">
                <h3 class="text-lg font-medium text-gray-900">
                  {{ theme.name }}
                </h3>
                <p v-if="theme.description" class="mt-1 text-sm text-gray-600">
                  {{ theme.description }}
                </p>
                <div class="mt-2 flex items-center space-x-4">
                  <span
                    class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                    :class="
                      theme.isActive
                        ? 'bg-green-100 text-green-800'
                        : 'bg-gray-100 text-gray-800'
                    "
                  >
                    {{ theme.isActive ? "Active" : "Inactive" }}
                  </span>
                  <span class="text-sm text-gray-500"
                    >v{{ theme.version }}</span
                  >
                  <span class="text-sm text-gray-500"
                    >{{ theme.variables.length }} variables</span
                  >
                  <span class="text-sm text-gray-500"
                    >{{ theme.scssFiles.length }} SCSS files</span
                  >
                </div>
              </div>
              <div class="flex space-x-2">
                <button
                  @click="editTheme(theme)"
                  class="px-3 py-1 border border-gray-300 rounded text-sm text-gray-700 hover:bg-gray-50"
                >
                  Edit
                </button>
                <button
                  @click="toggleThemeStatus(theme)"
                  class="px-3 py-1 border border-transparent rounded text-sm text-white"
                  :class="
                    theme.isActive
                      ? 'bg-red-600 hover:bg-red-700'
                      : 'bg-green-600 hover:bg-green-700'
                  "
                >
                  {{ theme.isActive ? "Deactivate" : "Activate" }}
                </button>
                <button
                  @click="deleteTheme(theme.id)"
                  class="px-3 py-1 border border-red-300 rounded text-sm text-red-700 hover:bg-red-50"
                >
                  Delete
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Theme Modal -->
    <div
      v-if="showCreateModal"
      class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50"
      @click="showCreateModal = false"
    >
      <div
        class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white"
        @click.stop
      >
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 mb-4">
            Create New Theme
          </h3>
          <form @submit.prevent="handleCreateTheme" class="space-y-4">
            <div>
              <label
                for="themeName"
                class="block text-sm font-medium text-gray-700"
                >Theme Name</label
              >
              <input
                id="themeName"
                v-model="newTheme.name"
                type="text"
                required
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                placeholder="Enter theme name"
              />
            </div>
            <div>
              <label
                for="themeDescription"
                class="block text-sm font-medium text-gray-700"
                >Description</label
              >
              <textarea
                id="themeDescription"
                v-model="newTheme.description"
                rows="3"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                placeholder="Enter theme description"
              ></textarea>
            </div>
            <div class="flex justify-end space-x-3">
              <button
                type="button"
                @click="showCreateModal = false"
                class="px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
              >
                Cancel
              </button>
              <button
                type="submit"
                :disabled="creating"
                class="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 disabled:opacity-50"
              >
                {{ creating ? "Creating..." : "Create Theme" }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { storeToRefs } from "pinia";
import { useTenantThemeStore } from "@/stores/tenantTheme";
import type { TenantTheme } from "@/stores/tenantTheme";

const themeStore = useTenantThemeStore();

// Use storeToRefs for reactive state (maintains reactivity)
const { themes, loading, error } = storeToRefs(themeStore);
// Direct access for methods (they don't need reactivity)
const {
  fetchThemes,
  createTheme,
  updateTheme,
  deleteTheme: storeDeleteTheme,
} = themeStore;

// Local reactive state
const showCreateModal = ref(false);
const newTheme = ref({
  name: "",
  description: "",
});
const creating = ref(false);

// Methods
const handleCreateTheme = async () => {
  if (!newTheme.value.name.trim()) return;

  creating.value = true;
  try {
    await createTheme({
      name: newTheme.value.name.trim(),
      description: newTheme.value.description.trim() || undefined,
    });

    // Reset form and close modal
    newTheme.value = { name: "", description: "" };
    showCreateModal.value = false;
  } catch (err) {
    console.error("Failed to create theme:", err);
  } finally {
    creating.value = false;
  }
};

const createSampleTheme = async () => {
  creating.value = true;
  try {
    await createTheme({
      name: "Sample Theme",
      description: "A sample theme with default colors and variables",
      isActive: false,
    });
  } catch (err) {
    console.error("Failed to create sample theme:", err);
  } finally {
    creating.value = false;
  }
};

const editTheme = (theme: TenantTheme) => {
  // TODO: Implement edit functionality
  console.log("Edit theme:", theme);
};

const toggleThemeStatus = async (theme: TenantTheme) => {
  try {
    await updateTheme(theme.id, {
      isActive: !theme.isActive,
    });
  } catch (err) {
    console.error("Failed to toggle theme status:", err);
  }
};

const deleteTheme = async (themeId: string) => {
  if (
    !confirm(
      "Are you sure you want to delete this theme? This action cannot be undone."
    )
  ) {
    return;
  }

  try {
    await storeDeleteTheme(themeId);
  } catch (err) {
    console.error("Failed to delete theme:", err);
  }
};

// Lifecycle
onMounted(() => {
  fetchThemes();
});
</script>

<style scoped>
/* Additional styles if needed */
</style>
