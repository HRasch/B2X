<template>
  <div class="ai-consumption">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white">AI Consumption</h1>
      <p class="mt-2 text-gray-600 dark:text-gray-400">
        Monitor AI usage, costs, and performance metrics
      </p>
    </div>

    <!-- Summary Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div
              class="w-8 h-8 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center"
            >
              <svg
                class="w-5 h-5 text-blue-600 dark:text-blue-400"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"
                />
              </svg>
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Requests</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">
              {{ formatNumber(totalRequests) }}
            </p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div
              class="w-8 h-8 bg-green-100 dark:bg-green-900 rounded-lg flex items-center justify-center"
            >
              <svg
                class="w-5 h-5 text-green-600 dark:text-green-400"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1"
                />
              </svg>
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Cost</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">
              ${{ formatCurrency(totalCost) }}
            </p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div
              class="w-8 h-8 bg-yellow-100 dark:bg-yellow-900 rounded-lg flex items-center justify-center"
            >
              <svg
                class="w-5 h-5 text-yellow-600 dark:text-yellow-400"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Avg Response Time</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">
              {{ formatTime(avgResponseTime) }}
            </p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div
              class="w-8 h-8 bg-red-100 dark:bg-red-900 rounded-lg flex items-center justify-center"
            >
              <svg
                class="w-5 h-5 text-red-600 dark:text-red-400"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z"
                />
              </svg>
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Error Rate</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">
              {{ formatPercentage(errorRate) }}
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Time Range Selector -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 mb-8">
      <div class="flex items-center justify-between">
        <h2 class="text-lg font-medium text-gray-900 dark:text-white">Usage Trends</h2>
        <div class="flex space-x-2">
          <button
            v-for="range in timeRanges"
            :key="range.value"
            @click="selectedTimeRange = range.value"
            :class="[
              'px-3 py-2 text-sm font-medium rounded-lg transition-colors',
              selectedTimeRange === range.value
                ? 'bg-blue-100 text-blue-700 dark:bg-blue-900 dark:text-blue-300'
                : 'text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700',
            ]"
          >
            {{ range.label }}
          </button>
        </div>
      </div>
    </div>

    <!-- Charts Placeholder -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-8">
      <!-- Usage Over Time -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Requests Over Time</h3>
        <div class="h-64 flex items-center justify-center bg-gray-50 dark:bg-gray-700 rounded-lg">
          <div class="text-center">
            <svg
              class="w-12 h-12 text-gray-400 mx-auto mb-4"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"
              />
            </svg>
            <p class="text-gray-500 dark:text-gray-400">Chart will be implemented with Chart.js</p>
          </div>
        </div>
      </div>

      <!-- Cost Breakdown -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Cost by Provider</h3>
        <div class="h-64 flex items-center justify-center bg-gray-50 dark:bg-gray-700 rounded-lg">
          <div class="text-center">
            <svg
              class="w-12 h-12 text-gray-400 mx-auto mb-4"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M11 3.055A9.001 9.001 0 1020.945 13H11V3.055z"
              />
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M20.488 9H15V3.512A9.025 9.025 0 0120.488 9z"
              />
            </svg>
            <p class="text-gray-500 dark:text-gray-400">Chart will be implemented with Chart.js</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Recent Activity -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white">Recent Activity</h3>
      </div>
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider"
              >
                Timestamp
              </th>
              <th
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider"
              >
                Provider
              </th>
              <th
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider"
              >
                Model
              </th>
              <th
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider"
              >
                Tokens
              </th>
              <th
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider"
              >
                Cost
              </th>
              <th
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider"
              >
                Status
              </th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
            <tr v-for="activity in recentActivity" :key="activity.id">
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ formatDate(activity.timestamp) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ activity.provider }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ activity.model }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ formatNumber(activity.tokens) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                ${{ formatCurrency(activity.cost) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span
                  :class="[
                    'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                    activity.status === 'success'
                      ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
                      : 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300',
                  ]"
                >
                  {{ activity.status }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';

interface Activity {
  id: string;
  timestamp: Date;
  provider: string;
  model: string;
  tokens: number;
  cost: number;
  status: 'success' | 'error';
}

const selectedTimeRange = ref('7d');

const timeRanges = [
  { label: '24h', value: '1d' },
  { label: '7d', value: '7d' },
  { label: '30d', value: '30d' },
  { label: '90d', value: '90d' },
];

// Mock data - will be replaced with real API calls
const totalRequests = ref(15420);
const totalCost = ref(2847.32);
const avgResponseTime = ref(1250); // ms
const errorRate = ref(0.023);

const recentActivity = ref<Activity[]>([
  {
    id: '1',
    timestamp: new Date(Date.now() - 1000 * 60 * 5),
    provider: 'OpenAI',
    model: 'gpt-4',
    tokens: 1250,
    cost: 0.045,
    status: 'success',
  },
  {
    id: '2',
    timestamp: new Date(Date.now() - 1000 * 60 * 15),
    provider: 'Anthropic',
    model: 'claude-3-opus',
    tokens: 890,
    cost: 0.032,
    status: 'success',
  },
  {
    id: '3',
    timestamp: new Date(Date.now() - 1000 * 60 * 30),
    provider: 'OpenAI',
    model: 'gpt-3.5-turbo',
    tokens: 450,
    cost: 0.009,
    status: 'error',
  },
  {
    id: '4',
    timestamp: new Date(Date.now() - 1000 * 60 * 45),
    provider: 'Azure OpenAI',
    model: 'gpt-4',
    tokens: 2100,
    cost: 0.063,
    status: 'success',
  },
]);

const formatNumber = (num: number): string => {
  return new Intl.NumberFormat().format(num);
};

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-US', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(amount);
};

const formatTime = (ms: number): string => {
  if (ms < 1000) return `${ms}ms`;
  return `${(ms / 1000).toFixed(1)}s`;
};

const formatPercentage = (rate: number): string => {
  return `${(rate * 100).toFixed(2)}%`;
};

const formatDate = (date: Date): string => {
  return new Intl.DateTimeFormat('en-US', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  }).format(date);
};

onMounted(() => {
  // TODO: Load consumption data from API
  // loadConsumptionData()
});
</script>
