<template>
  <div class="ai-dashboard">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white">AI Management Dashboard</h1>
      <p class="mt-2 text-gray-600 dark:text-gray-400">
        Monitor and manage your tenant's AI systems and MCP server
      </p>
    </div>

    <!-- Status Overview -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
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
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">MCP Server</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">Online</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
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
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Active Providers</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">
              {{ activeProviders }} / {{ totalProviders }}
            </p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
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
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Monthly Usage</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">
              {{ monthlyTokens.toLocaleString() }} tokens
            </p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
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
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Monthly Cost</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">${{ monthlyCost.toFixed(2) }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow mb-8">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h2 class="text-lg font-medium text-gray-900 dark:text-white">Quick Actions</h2>
      </div>
      <div class="p-6">
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <router-link
            to="/ai/prompts"
            class="flex items-center p-4 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
          >
            <div class="flex-shrink-0">
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
                Manage System Prompts
              </h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">
                Configure AI assistant behaviors
              </p>
            </div>
          </router-link>

          <router-link
            to="/ai/providers"
            class="flex items-center p-4 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
          >
            <div class="flex-shrink-0">
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
              <h3 class="text-sm font-medium text-gray-900 dark:text-white">Configure Providers</h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">Set up AI provider connections</p>
            </div>
          </router-link>

          <router-link
            to="/ai/consumption"
            class="flex items-center p-4 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
          >
            <div class="flex-shrink-0">
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
              <h3 class="text-sm font-medium text-gray-900 dark:text-white">View Consumption</h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">Monitor AI usage and costs</p>
            </div>
          </router-link>
        </div>
      </div>
    </div>

    <!-- Recent Activity -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h2 class="text-lg font-medium text-gray-900 dark:text-white">Recent AI Activity</h2>
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
          <h3 class="mt-2 text-sm font-medium text-gray-900 dark:text-white">No recent activity</h3>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            AI usage data will appear here once tools are used.
          </p>
        </div>
        <div v-else class="space-y-4">
          <div
            v-for="activity in recentActivity"
            :key="activity.id"
            class="flex items-center justify-between py-3 border-b border-gray-200 dark:border-gray-700 last:border-b-0"
          >
            <div class="flex items-center">
              <div class="flex-shrink-0">
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

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString();
};

onMounted(() => {
  // TODO: Load real data from API
  // loadDashboardData()
});
</script>
