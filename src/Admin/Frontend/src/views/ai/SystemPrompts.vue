<template>
  <div class="system-prompts">
    <div class="mb-8">
      <div class="flex justify-between items-center">
        <div>
          <h1 class="text-3xl font-bold text-gray-900 dark:text-white">
            {{ $t('ai.systemPrompts.title') }}
          </h1>
          <p class="mt-2 text-gray-600 dark:text-gray-400">
            {{ $t('ai.systemPrompts.subtitle') }}
          </p>
        </div>
        <button
          @click="showCreateModal = true"
          class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-medium transition-colors"
        >
          <svg class="w-5 h-5 inline mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M12 6v6m0 0v6m0-6h6m-6 0H6"
            />
          </svg>
          {{ $t('ai.systemPrompts.new') }}
        </button>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow mb-6">
      <div class="p-6">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              {{ $t('ai.systemPrompts.toolType') }}
            </label>
            <select
              v-model="filters.toolType"
              class="w-full border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-2 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
            >
              <option value="">{{ $t('ai.systemPrompts.allTools') }}</option>
              <option value="cms_page_design">{{ $t('ai.systemPrompts.cmsPageDesign') }}</option>
              <option value="email_template_design">
                {{ $t('ai.systemPrompts.emailTemplateDesign') }}
              </option>
              <option value="content_optimization">
                {{ $t('ai.systemPrompts.contentOptimization') }}
              </option>
              <option value="user_management">{{ $t('ai.systemPrompts.userManagement') }}</option>
              <option value="system_health">
                {{ $t('ai.systemPrompts.systemHealthAnalysis') }}
              </option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              {{ $t('ai.systemPrompts.status') }}
            </label>
            <select
              v-model="filters.status"
              class="w-full border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-2 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
            >
              <option value="">{{ $t('ai.systemPrompts.allStatus') }}</option>
              <option value="active">{{ $t('ai.systemPrompts.active') }}</option>
              <option value="inactive">{{ $t('ai.systemPrompts.inactive') }}</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              {{ $t('ai.systemPrompts.search') }}
            </label>
            <input
              v-model="filters.search"
              type="text"
              :placeholder="$t('ai.systemPrompts.searchPlaceholder')"
              class="w-full border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-2 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- Prompts List -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h2 class="text-lg font-medium text-gray-900 dark:text-white">
          {{ $t('ai.systemPrompts.promptsCount') }} ({{ filteredPrompts.length }})
        </h2>
      </div>
      <div class="divide-y divide-gray-200 dark:divide-gray-700">
        <div
          v-for="prompt in filteredPrompts"
          :key="`${prompt.toolType}-${prompt.key}`"
          class="p-6 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
        >
          <div class="flex items-center justify-between">
            <div class="flex-1">
              <div class="flex items-center">
                <h3 class="text-lg font-medium text-gray-900 dark:text-white">
                  {{ prompt.key.replace(/_/g, ' ').replace(/\b\w/g, l => l.toUpperCase()) }}
                </h3>
                <span
                  :class="[
                    'ml-2 inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                    prompt.isActive
                      ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
                      : 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300',
                  ]"
                >
                  {{
                    prompt.isActive
                      ? $t('ai.systemPrompts.active')
                      : $t('ai.systemPrompts.inactive')
                  }}
                </span>
              </div>
              <p class="mt-1 text-sm text-gray-600 dark:text-gray-400">
                {{ $t('ai.systemPrompts.toolLabel') }}
                {{ prompt.toolType.replace(/_/g, ' ').replace(/\b\w/g, l => l.toUpperCase()) }}
              </p>
              <p class="mt-2 text-sm text-gray-500 dark:text-gray-400 line-clamp-2">
                {{ prompt.content.substring(0, 200) }}...
              </p>
            </div>
            <div class="ml-4 flex items-center space-x-2">
              <router-link
                :to="`/ai/prompts/${prompt.toolType}/${prompt.key}`"
                class="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300 text-sm font-medium"
              >
                {{ $t('ai.systemPrompts.edit') }}
              </router-link>
              <button
                @click="togglePromptStatus(prompt)"
                :class="[
                  'text-sm font-medium px-3 py-1 rounded-lg transition-colors',
                  prompt.isActive
                    ? 'text-red-600 hover:text-red-800 dark:text-red-400 dark:hover:text-red-300'
                    : 'text-green-600 hover:text-green-800 dark:text-green-400 dark:hover:text-green-300',
                ]"
              >
                {{
                  prompt.isActive
                    ? $t('ai.systemPrompts.deactivate')
                    : $t('ai.systemPrompts.activate')
                }}
              </button>
            </div>
          </div>
        </div>
      </div>

      <div v-if="filteredPrompts.length === 0" class="text-center py-12">
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
            d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
          />
        </svg>
        <h3 class="mt-2 text-sm font-medium text-gray-900 dark:text-white">
          {{ $t('ai.systemPrompts.noPrompts') }}
        </h3>
        <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
          {{ $t('ai.systemPrompts.getStarted') }}
        </p>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <div
      v-if="showCreateModal"
      class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50"
      @click="showCreateModal = false"
    >
      <div
        class="relative top-20 mx-auto p-5 border w-11/12 md:w-3/4 lg:w-1/2 shadow-lg rounded-md bg-white dark:bg-gray-800"
        @click.stop
      >
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
            {{ $t('ai.systemPrompts.createNew') }}
          </h3>
          <form @submit.prevent="createPrompt" class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                {{ $t('ai.systemPrompts.toolType') }}
              </label>
              <select
                v-model="newPrompt.toolType"
                required
                class="w-full border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-2 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              >
                <option value="">{{ $t('ai.systemPrompts.selectToolType') }}</option>
                <option value="cms_page_design">{{ $t('ai.systemPrompts.cmsPageDesign') }}</option>
                <option value="email_template_design">
                  {{ $t('ai.systemPrompts.emailTemplateDesign') }}
                </option>
                <option value="content_optimization">
                  {{ $t('ai.systemPrompts.contentOptimization') }}
                </option>
                <option value="user_management">{{ $t('ai.systemPrompts.userManagement') }}</option>
                <option value="system_health">
                  {{ $t('ai.systemPrompts.systemHealthAnalysis') }}
                </option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                {{ $t('ai.systemPrompts.promptKey') }}
              </label>
              <input
                v-model="newPrompt.key"
                type="text"
                required
                :placeholder="$t('ai.systemPrompts.promptKeyPlaceholder')"
                class="w-full border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-2 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                {{ $t('ai.systemPrompts.promptContent') }}
              </label>
              <textarea
                v-model="newPrompt.content"
                required
                rows="8"
                :placeholder="$t('ai.systemPrompts.promptContentPlaceholder')"
                class="w-full border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-2 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              ></textarea>
            </div>
            <div class="flex justify-end space-x-3">
              <button
                type="button"
                @click="showCreateModal = false"
                class="px-4 py-2 text-gray-700 dark:text-gray-300 border border-gray-300 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
              >
                {{ $t('ai.systemPrompts.cancel') }}
              </button>
              <button
                type="submit"
                class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
              >
                {{ $t('ai.systemPrompts.createPrompt') }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';

interface SystemPrompt {
  tenantId: string;
  toolType: string;
  key: string;
  content: string;
  version: number;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

const showCreateModal = ref(false);
const filters = ref({
  toolType: '',
  status: '',
  search: '',
});

const newPrompt = ref({
  toolType: '',
  key: '',
  content: '',
});

// Mock data - will be replaced with real API calls
const prompts = ref<SystemPrompt[]>([
  {
    tenantId: 'tenant-1',
    toolType: 'cms_page_design',
    key: 'page_layout_guidelines',
    content:
      'You are an expert web designer. When designing CMS pages, focus on user experience, accessibility, and modern design principles...',
    version: 1,
    isActive: true,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
  },
  {
    tenantId: 'tenant-1',
    toolType: 'email_template_design',
    key: 'newsletter_template',
    content:
      'Design professional email templates that are mobile-responsive and follow marketing best practices...',
    version: 1,
    isActive: true,
    createdAt: '2024-01-02T00:00:00Z',
    updatedAt: '2024-01-02T00:00:00Z',
  },
]);

const filteredPrompts = computed(() => {
  return prompts.value.filter(prompt => {
    const matchesToolType = !filters.value.toolType || prompt.toolType === filters.value.toolType;
    const matchesStatus =
      !filters.value.status ||
      (filters.value.status === 'active' && prompt.isActive) ||
      (filters.value.status === 'inactive' && !prompt.isActive);
    const matchesSearch =
      !filters.value.search ||
      prompt.key.toLowerCase().includes(filters.value.search.toLowerCase()) ||
      prompt.content.toLowerCase().includes(filters.value.search.toLowerCase());

    return matchesToolType && matchesStatus && matchesSearch;
  });
});

const createPrompt = async () => {
  // TODO: Implement API call to create prompt
  console.log('Creating prompt:', newPrompt.value);

  // Mock adding to list
  prompts.value.push({
    tenantId: 'tenant-1',
    toolType: newPrompt.value.toolType,
    key: newPrompt.value.key,
    content: newPrompt.value.content,
    version: 1,
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  });

  showCreateModal.value = false;
  newPrompt.value = { toolType: '', key: '', content: '' };
};

const togglePromptStatus = async (prompt: SystemPrompt) => {
  // TODO: Implement API call to toggle status
  prompt.isActive = !prompt.isActive;
  console.log('Toggled prompt status:', prompt);
};

onMounted(() => {
  // TODO: Load prompts from API
  // loadPrompts()
});
</script>
