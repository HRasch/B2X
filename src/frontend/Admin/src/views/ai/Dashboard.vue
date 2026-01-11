<template>
  <div class="ai-dashboard">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white">
        {{ $t('ai.dashboard.title') }}
      </h1>
      <p class="mt-2 text-gray-600 dark:text-gray-400">
        {{ $t('ai.dashboard.subtitle') }}
      </p>
    </div>

    <!-- Status Overview -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="shrink-0">
            <div
              class="w-8 h-8 bg-green-100 dark:bg-green-900 rounded-full flex items-center justify-center"
            >
              <svg
                class="w-5 h-5 text-green-600 dark:text-green-400"
                fill="currentColor"
                viewBox="0 0 20 20"
              >
                <path
                  fill-rule="evenodd"
                  d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z"
                  clip-rule="evenodd"
                />
              </svg>
            </div>
          </div>
          <div class="ml-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">
              {{ $t('ai.dashboard.mcpServer') }}
            </h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">{{ $t('ai.dashboard.online') }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="shrink-0">
            <div
              class="w-8 h-8 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center"
            >
              <svg
                class="w-5 h-5 text-blue-600 dark:text-blue-400"
                fill="currentColor"
                viewBox="0 0 20 20"
              >
                <path
                  d="M13 6a3 3 0 11-6 0 3 3 0 016 0zM18 8a2 2 0 11-4 0 2 2 0 014 0zM14 15a4 4 0 00-8 0v3h8v-3z"
                />
              </svg>
            </div>
          </div>
          <div class="ml-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">
              {{ $t('ai.dashboard.activeProviders') }}
            </h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">
              {{ activeProviders }} / {{ totalProviders }}
            </p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="shrink-0">
            <div
              class="w-8 h-8 bg-yellow-100 dark:bg-yellow-900 rounded-full flex items-center justify-center"
            >
              <svg
                class="w-5 h-5 text-yellow-600 dark:text-yellow-400"
                fill="currentColor"
                viewBox="0 0 20 20"
              >
                <path
                  fill-rule="evenodd"
                  d="M12 7a1 1 0 110-2h5a1 1 0 011 1v5a1 1 0 11-2 0V8.414l-4.293 4.293a1 1 0 01-1.414 0L8 10.414l-4.293 4.293a1 1 0 01-1.414-1.414l5-5a1 1 0 011.414 0L11 10.586 14.586 7H12z"
                  clip-rule="evenodd"
                />
              </svg>
            </div>
          </div>
          <div class="ml-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">
              {{ $t('ai.dashboard.monthlyUsage') }}
            </h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">
              {{ monthlyTokens.toLocaleString() }} tokens
            </p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="shrink-0">
            <div
              class="w-8 h-8 bg-purple-100 dark:bg-purple-900 rounded-full flex items-center justify-center"
            >
              <svg
                class="w-5 h-5 text-purple-600 dark:text-purple-400"
                fill="currentColor"
                viewBox="0 0 20 20"
              >
                <path
                  fill-rule="evenodd"
                  d="M4 4a2 2 0 00-2 2v4a2 2 0 002 2V6h10a2 2 0 00-2-2H4zm2 6a2 2 0 012-2h8a2 2 0 012 2v4a2 2 0 01-2 2H8a2 2 0 01-2-2v-4zm6 4a2 2 0 100-4 2 2 0 000 4z"
                  clip-rule="evenodd"
                />
              </svg>
            </div>
          </div>
          <div class="ml-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">
              {{ $t('ai.dashboard.monthlyCost') }}
            </h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">${{ monthlyCost.toFixed(2) }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow mb-8">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h2 class="text-lg font-medium text-gray-900 dark:text-white">
          {{ $t('ai.dashboard.quickActions') }}
        </h2>
      </div>
      <div class="p-6">
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <router-link
            to="/ai/prompts"
            class="flex items-center p-4 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
          >
            <div class="shrink-0">
              <svg
                class="w-6 h-6 text-blue-600 dark:text-blue-400"
                fill="currentColor"
                viewBox="0 0 20 20"
              >
                <path
                  fill-rule="evenodd"
                  d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-8-3a1 1 0 00-.867.5 1 1 0 11-1.731-1A3 3 0 0113 8a3.001 3.001 0 01-2 2.83V11a1 1 0 11-2 0v-1a1 1 0 011-1 1 1 0 100-2zm0 8a1 1 0 100-2 1 1 0 000 2z"
                  clip-rule="evenodd"
                />
              </svg>
            </div>
            <div class="ml-3">
              <h3 class="text-sm font-medium text-gray-900 dark:text-white">
                {{ $t('ai.dashboard.manageSystemPrompts') }}
              </h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">
                {{ $t('ai.dashboard.manageSystemPromptsDesc') }}
              </p>
            </div>
          </router-link>

          <router-link
            to="/ai/providers"
            class="flex items-center p-4 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
          >
            <div class="shrink-0">
              <svg
                class="w-6 h-6 text-green-600 dark:text-green-400"
                fill="currentColor"
                viewBox="0 0 20 20"
              >
                <path
                  fill-rule="evenodd"
                  d="M3 4a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zm0 4a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zm0 4a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z"
                  clip-rule="evenodd"
                />
              </svg>
            </div>
            <div class="ml-3">
              <h3 class="text-sm font-medium text-gray-900 dark:text-white">
                {{ $t('ai.dashboard.configureProviders') }}
              </h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">
                {{ $t('ai.dashboard.configureProvidersDesc') }}
              </p>
            </div>
          </router-link>

          <router-link
            to="/ai/consumption"
            class="flex items-center p-4 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
          >
            <div class="shrink-0">
              <svg
                class="w-6 h-6 text-purple-600 dark:text-purple-400"
                fill="currentColor"
                viewBox="0 0 20 20"
              >
                <path
                  d="M2 11a1 1 0 011-1h2a1 1 0 011 1v5a1 1 0 01-1 1H3a1 1 0 01-1-1v-5zM8 7a1 1 0 011-1h2a1 1 0 011 1v9a1 1 0 01-1 1H9a1 1 0 01-1-1V7zM14 4a1 1 0 011-1h2a1 1 0 011 1v12a1 1 0 01-1 1h-2a1 1 0 01-1-1V4z"
                />
              </svg>
            </div>
            <div class="ml-3">
              <h3 class="text-sm font-medium text-gray-900 dark:text-white">
                {{ $t('ai.dashboard.viewConsumption') }}
              </h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">
                {{ $t('ai.dashboard.viewConsumptionDesc') }}
              </p>
            </div>
          </router-link>
        </div>
      </div>
    </div>

    <!-- AI Assistant Input -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow mb-8">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h2 class="text-lg font-medium text-gray-900 dark:text-white">
          {{ $t('ai.dashboard.aiAssistant') }}
        </h2>
        <p class="mt-1 text-sm text-gray-600 dark:text-gray-400">
          {{ $t('ai.dashboard.aiAssistantDesc') }}
        </p>
      </div>
      <div class="p-6">
        <div class="space-y-4">
          <div class="flex space-x-4">
            <div class="flex-1">
              <input
                v-model="aiQuery"
                type="text"
                placeholder="Ask me anything about AI management..."
                class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:text-white"
                @keyup.enter="submitQuery"
              />
            </div>
            <button
              @click="submitQuery"
              :disabled="!aiQuery.trim() || isQuerying"
              class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed flex items-center space-x-2"
            >
              <svg v-if="isQuerying" class="animate-spin h-4 w-4" fill="none" viewBox="0 0 24 24">
                <circle
                  class="opacity-25"
                  cx="12"
                  cy="12"
                  r="10"
                  stroke="currentColor"
                  stroke-width="4"
                />
                <path
                  class="opacity-75"
                  fill="currentColor"
                  d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                />
              </svg>
              <span v-else>
                <svg class="h-4 w-4" fill="currentColor" viewBox="0 0 20 20">
                  <path
                    fill-rule="evenodd"
                    d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-8.293l-3-3a1 1 0 00-1.414 0l-3 3a1 1 0 001.414 1.414L9 9.414V13a1 1 0 102 0V9.414l1.293 1.293a1 1 0 001.414-1.414z"
                    clip-rule="evenodd"
                  />
                </svg>
              </span>
              <span>{{ isQuerying ? 'Processing...' : 'Ask' }}</span>
            </button>
          </div>

          <!-- Response Area -->
          <div v-if="aiResponse" class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4">
            <div class="flex items-start space-x-3">
              <div class="shrink-0">
                <div
                  class="w-8 h-8 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center"
                >
                  <svg
                    class="w-5 h-5 text-blue-600 dark:text-blue-400"
                    fill="currentColor"
                    viewBox="0 0 20 20"
                  >
                    <path
                      fill-rule="evenodd"
                      d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-8-3a1 1 0 00-.867.5 1 1 0 11-1.731-1A3 3 0 0113 8a3.001 3.001 0 01-2 2.83V11a1 1 0 11-2 0v-1a1 1 0 011-1 1 1 0 100-2zm0 8a1 1 0 100-2 1 1 0 000 2z"
                      clip-rule="evenodd"
                    />
                  </svg>
                </div>
              </div>
              <div class="flex-1">
                <p class="text-sm text-gray-900 dark:text-white whitespace-pre-wrap">
                  {{ aiResponse }}
                </p>
              </div>
            </div>
          </div>

          <!-- Suggested Queries -->
          <div class="border-t border-gray-200 dark:border-gray-600 pt-4">
            <p class="text-sm text-gray-600 dark:text-gray-400 mb-3">
              {{ $t('ai.dashboard.tryAsking') }}
            </p>
            <div class="flex flex-wrap gap-2">
              <button
                v-for="suggestion in querySuggestions"
                :key="suggestion"
                @click="aiQuery = suggestion"
                class="px-3 py-1 text-xs bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded-full hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors"
              >
                {{ suggestion }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Recent Activity -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h2 class="text-lg font-medium text-gray-900 dark:text-white">
          {{ $t('ai.dashboard.recentActivity') }}
        </h2>
      </div>
      <div class="p-6">
        <div v-if="recentActivity.length === 0" class="text-center py-8">
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
              d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"
            />
          </svg>
          <h3 class="mt-2 text-sm font-medium text-gray-900 dark:text-white">
            {{ $t('ai.dashboard.noRecentActivity') }}
          </h3>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            {{ $t('ai.dashboard.noRecentActivityDesc') }}
          </p>
        </div>
        <div v-else class="space-y-4">
          <div
            v-for="activity in recentActivity"
            :key="activity.id"
            class="flex items-center justify-between py-3 border-b border-gray-200 dark:border-gray-700 last:border-b-0"
          >
            <div class="flex items-center">
              <div class="shrink-0">
                <div
                  class="w-8 h-8 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center"
                >
                  <svg
                    class="w-4 h-4 text-blue-600 dark:text-blue-400"
                    fill="currentColor"
                    viewBox="0 0 20 20"
                  >
                    <path
                      fill-rule="evenodd"
                      d="M18 10a8 8 0 11-16 0 8 8 0 000 16zm-8-3a1 1 0 00-.867.5 1 1 0 11-1.731-1A3 3 0 0113 8a3.001 3.001 0 01-2 2.83V11a1 1 0 11-2 0v-1a1 1 0 011-1 1 1 0 100-2zm0 8a1 1 0 100-2 1 1 0 000 2z"
                      clip-rule="evenodd"
                    />
                  </svg>
                </div>
              </div>
              <div class="ml-3">
                <p class="text-sm font-medium text-gray-900 dark:text-white">
                  {{ activity.toolName }}
                </p>
                <p class="text-sm text-gray-500 dark:text-gray-400">{{ activity.description }}</p>
              </div>
            </div>
            <div class="text-right">
              <p class="text-sm text-gray-900 dark:text-white">{{ activity.tokens }} tokens</p>
              <p class="text-sm text-gray-500 dark:text-gray-400">
                {{ formatDate(activity.timestamp) }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';

// Mock data - will be replaced with real API calls
const activeProviders = ref(2);
const totalProviders = ref(3);
const monthlyTokens = ref(125000);
const monthlyCost = ref(45.67);

const recentActivity = ref([
  // Mock data - will be replaced with real API calls
]);

// AI Assistant state
const aiQuery = ref('');
const aiResponse = ref('');
const isQuerying = ref(false);
const querySuggestions = ref([
  'How do I optimize AI costs?',
  'What providers are available?',
  'Show me usage trends',
  'Help configure a new prompt',
  'Explain token consumption',
]);

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString();
};

const submitQuery = async () => {
  if (!aiQuery.value.trim()) return;

  isQuerying.value = true;
  aiResponse.value = '';

  try {
    // TODO: Replace with actual AI API call
    // For now, simulate a response
    await new Promise(resolve => setTimeout(resolve, 2000));

    // Mock responses based on query
    const query = aiQuery.value.toLowerCase();
    if (query.includes('cost') || query.includes('optimize')) {
      aiResponse.value =
        'To optimize AI costs:\n\n1. Use smaller models for simple tasks\n2. Implement caching for repeated queries\n3. Set usage limits per user/department\n4. Monitor and analyze consumption patterns\n5. Consider on-premise models for high-volume tasks\n\nCurrent monthly cost: $45.67 (125K tokens)';
    } else if (query.includes('provider')) {
      aiResponse.value =
        'Available AI providers:\n\n• OpenAI (GPT-4, GPT-3.5)\n• Anthropic (Claude)\n• Google (Gemini)\n• Local models (Ollama)\n\nCurrently active: 2/3 providers\n\nTo add a provider, go to Configure Providers page.';
    } else if (query.includes('usage') || query.includes('trend')) {
      aiResponse.value =
        'Usage Trends (Last 30 days):\n\n📈 Token consumption: +15%\n💰 Cost increase: +12%\n🔄 API calls: +8%\n\nPeak usage: Weekdays 9-11 AM\nLowest usage: Weekends\n\nRecommendation: Implement usage quotas during peak hours.';
    } else if (query.includes('prompt')) {
      aiResponse.value =
        'To configure system prompts:\n\n1. Go to "Manage System Prompts" page\n2. Create a new prompt or edit existing\n3. Define role, context, and constraints\n4. Test with sample queries\n5. Set as default for specific use cases\n\nCurrent prompts: 5 active, 2 draft';
    } else if (query.includes('token')) {
      aiResponse.value =
        'Token Consumption Explained:\n\n• Input tokens: Text you send to AI\n• Output tokens: AI response text\n• Current usage: 125,000 tokens/month\n• Average cost: $0.0004 per token\n\nTo reduce: Use shorter prompts, avoid verbose instructions, cache common responses.';
    } else {
      aiResponse.value = `I understand you're asking about: "${aiQuery.value}"\n\nAs your AI management assistant, I can help with:\n\n• Cost optimization strategies\n• Provider configuration\n• Usage monitoring and trends\n• System prompt management\n• Token consumption analysis\n• Security and compliance guidance\n\nPlease try one of the suggested queries above or rephrase your question!`;
    }
  } catch {
    aiResponse.value =
      'Sorry, I encountered an error processing your query. Please try again or contact support if the issue persists.';
  } finally {
    isQuerying.value = false;
  }
};

onMounted(() => {
  // TODO: Load real data from API
  // loadDashboardData()
});
</script>
