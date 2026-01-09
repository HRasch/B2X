<template>
  <div class="ai-providers">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white">
        {{ $t('ai.providers.title') }}
      </h1>
      <p class="mt-2 text-gray-600 dark:text-gray-400">
        {{ $t('ai.providers.subtitle') }}
      </p>
    </div>

    <!-- Provider Status Overview -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
      <div
        v-for="provider in providers"
        :key="provider.name"
        class="bg-white dark:bg-gray-800 rounded-lg shadow p-6"
      >
        <div class="flex items-center justify-between">
          <div class="flex items-center">
            <div class="shrink-0">
              <div
                :class="[
                  'w-10 h-10 rounded-lg flex items-center justify-center',
                  provider.isConfigured
                    ? 'bg-green-100 dark:bg-green-900'
                    : 'bg-gray-100 dark:bg-gray-700',
                ]"
              >
                <span
                  class="text-lg font-semibold"
                  :class="
                    provider.isConfigured
                      ? 'text-green-600 dark:text-green-400'
                      : 'text-gray-400 dark:text-gray-500'
                  "
                >
                  {{ provider.name.charAt(0) }}
                </span>
              </div>
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">
                {{ provider.name }}
              </h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">
                {{
                  provider.isConfigured
                    ? $t('ai.providers.configured')
                    : $t('ai.providers.notConfigured')
                }}
              </p>
            </div>
          </div>
          <button
            @click="editProvider(provider)"
            class="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"
              />
            </svg>
          </button>
        </div>

        <div class="mt-4">
          <div class="flex items-center justify-between text-sm">
            <span class="text-gray-500 dark:text-gray-400">{{ $t('ai.providers.models') }}</span>
            <span class="font-medium text-gray-900 dark:text-white">
              {{ provider.configuredModels.length }} / {{ provider.availableModels.length }}
            </span>
          </div>
          <div class="mt-2 flex flex-wrap gap-1">
            <span
              v-for="model in provider.availableModels.slice(0, 3)"
              :key="model"
              :class="[
                'inline-flex items-center px-2 py-0.5 rounded text-xs font-medium',
                provider.configuredModels.includes(model)
                  ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
                  : 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300',
              ]"
            >
              {{ model }}
            </span>
            <span
              v-if="provider.availableModels.length > 3"
              class="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300"
            >
              +{{ provider.availableModels.length - 3 }} more
            </span>
          </div>
        </div>
      </div>
    </div>

    <!-- Configuration Modal -->
    <div
      v-if="showConfigModal"
      class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50"
      @click="showConfigModal = false"
    >
      <div
        class="relative top-20 mx-auto p-5 border w-11/12 md:w-2/3 shadow-lg rounded-md bg-white dark:bg-gray-800"
        @click.stop
      >
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
            Configure {{ selectedProvider?.name }}
          </h3>

          <form @submit.prevent="saveProviderConfig" class="space-y-6">
            <!-- API Key -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                {{ $t('ai.providers.apiKey') }}
              </label>
              <input
                v-model="configForm.apiKey"
                type="password"
                required
                :placeholder="$t('ai.providers.apiKey')"
                class="w-full border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-2 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              />
              <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
                {{ $t('ai.providers.apiKeyDescription') }}
              </p>
            </div>

            <!-- Model Selection -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                {{ $t('ai.providers.availableModels') }}
              </label>
              <div
                class="space-y-2 max-h-40 overflow-y-auto border border-gray-300 dark:border-gray-600 rounded-lg p-3"
              >
                <label
                  v-for="model in selectedProvider?.availableModels"
                  :key="model"
                  class="flex items-center"
                >
                  <input
                    v-model="configForm.selectedModels"
                    :value="model"
                    type="checkbox"
                    class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                  />
                  <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">
                    {{ model }}
                  </span>
                </label>
              </div>
            </div>

            <!-- Default Model -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                {{ $t('ai.providers.defaultModel') }}
              </label>
              <select
                v-model="configForm.defaultModel"
                class="w-full border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-2 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              >
                <option value="">{{ $t('ai.providers.defaultModelPlaceholder') }}</option>
                <option v-for="model in configForm.selectedModels" :key="model" :value="model">
                  {{ model }}
                </option>
              </select>
            </div>

            <!-- Monthly Budget -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                {{ $t('ai.providers.monthlyBudget') }}
              </label>
              <input
                v-model.number="configForm.monthlyBudget"
                type="number"
                min="0"
                step="0.01"
                placeholder="0.00"
                class="w-full border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-2 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              />
              <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
                {{ $t('ai.providers.monthlyBudgetDescription') }}
              </p>
            </div>

            <!-- Status -->
            <div class="flex items-center">
              <input
                v-model="configForm.isActive"
                type="checkbox"
                class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
              />
              <label class="ml-2 text-sm text-gray-700 dark:text-gray-300">
                {{ $t('ai.providers.enableProvider') }}
              </label>
            </div>

            <div
              class="flex justify-end space-x-3 pt-4 border-t border-gray-200 dark:border-gray-700"
            >
              <button
                type="button"
                @click="showConfigModal = false"
                class="px-4 py-2 text-gray-700 dark:text-gray-300 border border-gray-300 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
              >
                {{ $t('ai.providers.cancel') }}
              </button>
              <button
                type="submit"
                class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
              >
                {{ $t('ai.providers.saveConfiguration') }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Test Connection Modal -->
    <div
      v-if="showTestModal"
      class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50"
      @click="showTestModal = false"
    >
      <div
        class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white dark:bg-gray-800"
        @click.stop
      >
        <div class="mt-3 text-center">
          <div
            class="mx-auto flex items-center justify-center h-12 w-12 rounded-full bg-blue-100 dark:bg-blue-900"
          >
            <svg
              class="h-6 w-6 text-blue-600 dark:text-blue-400"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M13 10V3L4 14h7v7l9-11h-7z"
              />
            </svg>
          </div>
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mt-4">
            {{ $t('ai.providers.testingConnection') }}
          </h3>
          <p class="text-sm text-gray-500 dark:text-gray-400 mt-2">
            {{ testStatus }}
          </p>
          <div class="mt-4">
            <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';

interface Provider {
  name: string;
  isConfigured: boolean;
  availableModels: string[];
  configuredModels: string[];
}

const showConfigModal = ref(false);
const showTestModal = ref(false);
const selectedProvider = ref<Provider | null>(null);
const testStatus = ref('Testing API connection...');

const configForm = reactive({
  apiKey: '',
  selectedModels: [] as string[],
  defaultModel: '',
  monthlyBudget: 0,
  isActive: true,
});

// Mock data - will be replaced with real API calls
const providers = ref<Provider[]>([
  {
    name: 'OpenAI',
    isConfigured: true,
    availableModels: ['gpt-4', 'gpt-3.5-turbo', 'gpt-4-turbo'],
    configuredModels: ['gpt-4', 'gpt-3.5-turbo'],
  },
  {
    name: 'Anthropic',
    isConfigured: false,
    availableModels: ['claude-3-opus', 'claude-3-sonnet', 'claude-3-haiku'],
    configuredModels: [],
  },
  {
    name: 'Azure OpenAI',
    isConfigured: true,
    availableModels: ['gpt-4', 'gpt-35-turbo'],
    configuredModels: ['gpt-4'],
  },
]);

const editProvider = (provider: Provider) => {
  selectedProvider.value = provider;
  // Load existing config - mock data for now
  configForm.apiKey = provider.isConfigured ? '••••••••••••••••' : '';
  configForm.selectedModels = [...provider.configuredModels];
  configForm.defaultModel = provider.configuredModels[0] || '';
  configForm.monthlyBudget = 100; // Mock value
  configForm.isActive = provider.isConfigured;
  showConfigModal.value = true;
};

const saveProviderConfig = async () => {
  // TODO: Implement API call to save configuration
  console.log('Saving config for', selectedProvider.value?.name, configForm);

  // Mock update
  if (selectedProvider.value) {
    selectedProvider.value.isConfigured = configForm.isActive;
    selectedProvider.value.configuredModels = [...configForm.selectedModels];
  }

  showConfigModal.value = false;

  // Optionally test connection
  if (configForm.isActive) {
    await testConnection();
  }
};

const testConnection = async () => {
  showTestModal.value = true;
  testStatus.value = 'Testing API connection...';

  // Mock test
  setTimeout(() => {
    testStatus.value = 'Connection successful!';
    setTimeout(() => {
      showTestModal.value = false;
    }, 1500);
  }, 2000);
};

onMounted(() => {
  // TODO: Load providers from API
  // loadProviders()
});
</script>
