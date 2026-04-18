<template>
  <div class="system-prompt-detail">
    <div class="mb-8">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-3xl font-bold text-gray-900 dark:text-white">
            {{ promptKey.replace(/_/g, ' ').replace(/\b\w/g, l => l.toUpperCase()) }}
          </h1>
          <p class="mt-2 text-gray-600 dark:text-gray-400">
            Tool: {{ toolType.replace(/_/g, ' ').replace(/\b\w/g, l => l.toUpperCase()) }}
          </p>
        </div>
        <div class="flex items-center space-x-3">
          <span
            :class="[
              'inline-flex items-center px-3 py-1 rounded-full text-sm font-medium',
              currentPrompt?.isActive
                ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
                : 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300',
            ]"
          >
            {{ currentPrompt?.isActive ? 'Active' : 'Inactive' }}
          </span>
          <button
            @click="toggleStatus"
            :class="[
              'px-4 py-2 rounded-lg font-medium transition-colors',
              currentPrompt?.isActive
                ? 'bg-red-600 hover:bg-red-700 text-white'
                : 'bg-green-600 hover:bg-green-700 text-white',
            ]"
          >
            {{ currentPrompt?.isActive ? 'Deactivate' : 'Activate' }}
          </button>
        </div>
      </div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <!-- Main Content -->
      <div class="lg:col-span-2 space-y-6">
        <!-- Current Version -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
          <div
            class="px-6 py-4 border-b border-gray-200 dark:border-gray-700 flex items-center justify-between"
          >
            <h2 class="text-lg font-medium text-gray-900 dark:text-white">
              {{ $t('ai.systemPromptDetail.currentVersion') }} (v{{ currentPrompt?.version }})
            </h2>
            <button
              @click="showEditModal = true"
              class="bg-blue-600 hover:bg-blue-700 text-white px-3 py-1 rounded-lg text-sm font-medium transition-colors"
            >
              {{ $t('ai.systemPromptDetail.edit') }}
            </button>
          </div>
          <div class="p-6">
            <div class="prose dark:prose-invert max-w-none">
              <pre
                class="whitespace-pre-wrap text-sm text-gray-700 dark:text-gray-300 bg-gray-50 dark:bg-gray-900 p-4 rounded-lg border"
                >{{ currentPrompt?.content }}</pre
              >
            </div>
          </div>
        </div>

        <!-- Version History -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <h2 class="text-lg font-medium text-gray-900 dark:text-white">
              {{ $t('ai.systemPromptDetail.versionHistory') }}
            </h2>
          </div>
          <div class="divide-y divide-gray-200 dark:divide-gray-700">
            <div
              v-for="version in promptVersions"
              :key="version.version"
              class="p-6 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
            >
              <div class="flex items-center justify-between">
                <div>
                  <div class="flex items-center">
                    <h3 class="text-sm font-medium text-gray-900 dark:text-white">
                      Version {{ version.version }}
                    </h3>
                    <span
                      v-if="version.version === currentPrompt?.version"
                      class="ml-2 inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300"
                    >
                      {{ $t('ai.systemPromptDetail.current') }}
                    </span>
                  </div>
                  <p class="text-sm text-gray-500 dark:text-gray-400">
                    {{ $t('ai.systemPromptDetail.updated') }} {{ formatDate(version.createdAt) }}
                  </p>
                </div>
                <div class="flex items-center space-x-2">
                  <button
                    @click="viewVersion(version)"
                    class="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300 text-sm"
                  >
                    {{ $t('ai.systemPromptDetail.view') }}
                  </button>
                  <button
                    v-if="version.version !== currentPrompt?.version"
                    @click="restoreVersion(version)"
                    class="text-green-600 hover:text-green-800 dark:text-green-400 dark:hover:text-green-300 text-sm"
                  >
                    {{ $t('ai.systemPromptDetail.restore') }}
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Sidebar -->
      <div class="space-y-6">
        <!-- Statistics -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <h3 class="text-sm font-medium text-gray-900 dark:text-white">
              {{ $t('ai.systemPromptDetail.statistics') }}
            </h3>
          </div>
          <div class="p-6 space-y-4">
            <div class="flex justify-between">
              <span class="text-sm text-gray-500 dark:text-gray-400">{{
                $t('ai.systemPromptDetail.totalVersions')
              }}</span>
              <span class="text-sm font-medium text-gray-900 dark:text-white">{{
                promptVersions.length
              }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-sm text-gray-500 dark:text-gray-400">{{
                $t('ai.systemPromptDetail.created')
              }}</span>
              <span class="text-sm font-medium text-gray-900 dark:text-white">{{
                formatDate(currentPrompt?.createdAt)
              }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-sm text-gray-500 dark:text-gray-400">{{
                $t('ai.systemPromptDetail.lastUpdated')
              }}</span>
              <span class="text-sm font-medium text-gray-900 dark:text-white">{{
                formatDate(currentPrompt?.updatedAt)
              }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-sm text-gray-500 dark:text-gray-400">{{
                $t('ai.systemPromptDetail.characterCount')
              }}</span>
              <span class="text-sm font-medium text-gray-900 dark:text-white">{{
                currentPrompt?.content.length
              }}</span>
            </div>
          </div>
        </div>

        <!-- Usage -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
          <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
            <h3 class="text-sm font-medium text-gray-900 dark:text-white">
              {{ $t('ai.systemPromptDetail.recentUsage') }}
            </h3>
          </div>
          <div class="p-6">
            <div v-if="recentUsage.length === 0" class="text-center py-4">
              <p class="text-sm text-gray-500 dark:text-gray-400">
                {{ $t('ai.systemPromptDetail.noRecentUsage') }}
              </p>
            </div>
            <div v-else class="space-y-3">
              <div
                v-for="usage in recentUsage"
                :key="usage.id"
                class="flex justify-between items-center"
              >
                <span class="text-sm text-gray-600 dark:text-gray-400">{{
                  formatDate(usage.timestamp)
                }}</span>
                <span class="text-sm font-medium text-gray-900 dark:text-white"
                  >{{ usage.tokens }} tokens</span
                >
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Edit Modal -->
    <div
      v-if="showEditModal"
      class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50"
      @click="showEditModal = false"
    >
      <div
        class="relative top-20 mx-auto p-5 border w-11/12 md:w-3/4 shadow-lg rounded-md bg-white dark:bg-gray-800"
        @click.stop
      >
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
            {{ $t('ai.systemPromptDetail.editSystemPrompt') }}
          </h3>
          <form @submit.prevent="saveChanges" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                {{ $t('ai.systemPromptDetail.promptContent') }}
              </label>
              <textarea
                v-model="editContent"
                required
                rows="12"
                class="w-full border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-2 bg-white dark:bg-gray-700 text-gray-900 dark:text-white font-mono text-sm"
                placeholder="Enter the updated prompt content..."
              ></textarea>
            </div>
            <div class="flex justify-end space-x-3">
              <button
                type="button"
                @click="showEditModal = false"
                class="px-4 py-2 text-gray-700 dark:text-gray-300 border border-gray-300 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
              >
                {{ $t('ai.systemPromptDetail.cancel') }}
              </button>
              <button
                type="submit"
                class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
              >
                {{ $t('ai.systemPromptDetail.saveChanges') }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Version View Modal -->
    <div
      v-if="showVersionModal"
      class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50"
      @click="showVersionModal = false"
    >
      <div
        class="relative top-20 mx-auto p-5 border w-11/12 md:w-3/4 shadow-lg rounded-md bg-white dark:bg-gray-800"
        @click.stop
      >
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
            {{ $t('ai.systemPromptDetail.version') }} {{ selectedVersion?.version }} ({{
              formatDate(selectedVersion?.createdAt)
            }})
          </h3>
          <div class="max-h-96 overflow-y-auto">
            <pre
              class="whitespace-pre-wrap text-sm text-gray-700 dark:text-gray-300 bg-gray-50 dark:bg-gray-900 p-4 rounded-lg border"
              >{{ selectedVersion?.content }}</pre
            >
          </div>
          <div class="mt-4 flex justify-end">
            <button
              v-if="selectedVersion && selectedVersion.version !== currentPrompt?.version"
              @click="restoreVersion(selectedVersion)"
              class="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors mr-2"
            >
              {{ $t('ai.systemPromptDetail.restoreThisVersion') }}
            </button>
            <button
              @click="showVersionModal = false"
              class="px-4 py-2 text-gray-700 dark:text-gray-300 border border-gray-300 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
            >
              {{ $t('ai.systemPromptDetail.close') }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';

interface Version {
  version: number;
  content: string;
  createdAt: string;
  createdBy: string;
}

const route = useRoute();
const toolType = route.params.toolType as string;
const promptKey = route.params.key as string;

const showEditModal = ref(false);
const showVersionModal = ref(false);
const editContent = ref('');
const selectedVersion = ref(null);

// Mock data - will be replaced with real API calls
const currentPrompt = ref({
  tenantId: 'tenant-1',
  toolType: toolType,
  key: promptKey,
  content:
    'You are an expert AI assistant. When responding to user queries, be helpful, accurate, and concise...',
  version: 3,
  isActive: true,
  createdAt: '2024-01-01T00:00:00Z',
  updatedAt: '2024-01-03T00:00:00Z',
});

const promptVersions = ref([
  {
    version: 3,
    content:
      'You are an expert AI assistant. When responding to user queries, be helpful, accurate, and concise...',
    createdAt: '2024-01-03T00:00:00Z',
    createdBy: 'admin@example.com',
  },
  {
    version: 2,
    content: 'You are an AI assistant. Be helpful and accurate...',
    createdAt: '2024-01-02T00:00:00Z',
    createdBy: 'admin@example.com',
  },
  {
    version: 1,
    content: 'You are an AI assistant.',
    createdAt: '2024-01-01T00:00:00Z',
    createdBy: 'admin@example.com',
  },
]);

const recentUsage = ref([
  { id: 1, timestamp: '2024-01-03T10:00:00Z', tokens: 150 },
  { id: 2, timestamp: '2024-01-03T09:00:00Z', tokens: 200 },
  { id: 3, timestamp: '2024-01-02T15:00:00Z', tokens: 175 },
]);

const formatDate = (date: string | undefined) => {
  if (!date) return '';
  return new Date(date).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
};

const toggleStatus = async () => {
  if (!currentPrompt.value) return;

  // TODO: Implement API call to toggle status
  currentPrompt.value.isActive = !currentPrompt.value.isActive;
  console.log('Toggled prompt status:', currentPrompt.value);
};

const saveChanges = async () => {
  // TODO: Implement API call to save changes
  console.log('Saving changes:', editContent.value);

  // Mock update
  if (currentPrompt.value) {
    currentPrompt.value.content = editContent.value;
    currentPrompt.value.version++;
    currentPrompt.value.updatedAt = new Date().toISOString();

    // Add to version history
    promptVersions.value.unshift({
      version: currentPrompt.value.version,
      content: editContent.value,
      createdAt: currentPrompt.value.updatedAt,
      createdBy: 'admin@example.com',
    });
  }

  showEditModal.value = false;
};

const viewVersion = (version: Version) => {
  selectedVersion.value = version;
  showVersionModal.value = true;
};

const restoreVersion = async (version: Version) => {
  // TODO: Implement API call to restore version
  console.log('Restoring version:', version);

  if (currentPrompt.value) {
    currentPrompt.value.content = version.content;
    currentPrompt.value.version++;
    currentPrompt.value.updatedAt = new Date().toISOString();

    // Add to version history
    promptVersions.value.unshift({
      version: currentPrompt.value.version,
      content: version.content,
      createdAt: currentPrompt.value.updatedAt,
      createdBy: 'admin@example.com',
    });
  }

  showVersionModal.value = false;
};

onMounted(() => {
  editContent.value = currentPrompt.value?.content || '';
  // TODO: Load real data from API
  // loadPromptData()
});
</script>
