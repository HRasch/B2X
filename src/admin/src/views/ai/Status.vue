<template>
  <div class="mcp-status">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white">{{ $t('ai.status.title') }}</h1>
      <p class="mt-2 text-gray-600 dark:text-gray-400">
        {{ $t('ai.status.subtitle') }}
      </p>
    </div>

    <!-- Server Status Overview -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="shrink-0">
            <div
              :class="[
                'w-8 h-8 rounded-lg flex items-center justify-center',
                serverStatus === 'healthy'
                  ? 'bg-green-100 dark:bg-green-900'
                  : serverStatus === 'warning'
                    ? 'bg-yellow-100 dark:bg-yellow-900'
                    : 'bg-red-100 dark:bg-red-900',
              ]"
            >
              <svg
                class="w-5 h-5"
                :class="
                  serverStatus === 'healthy'
                    ? 'text-green-600 dark:text-green-400'
                    : serverStatus === 'warning'
                      ? 'text-yellow-600 dark:text-yellow-400'
                      : 'text-red-600 dark:text-red-400'
                "
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600 dark:text-gray-400">
              {{ $t('ai.status.serverStatus') }}
            </p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white capitalize">
              {{ serverStatus }}
            </p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="shrink-0">
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
                  d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"
                />
              </svg>
            </div>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600 dark:text-gray-400">
              {{ $t('ai.status.activeClients') }}
            </p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">
              {{ activeClients }}
            </p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="shrink-0">
            <div
              class="w-8 h-8 bg-purple-100 dark:bg-purple-900 rounded-lg flex items-center justify-center"
            >
              <svg
                class="w-5 h-5 text-purple-600 dark:text-purple-400"
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
            <p class="text-sm font-medium text-gray-600 dark:text-gray-400">
              {{ $t('ai.status.activeSessions') }}
            </p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">
              {{ activeSessions }}
            </p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center">
          <div class="shrink-0">
            <div
              class="w-8 h-8 bg-indigo-100 dark:bg-indigo-900 rounded-lg flex items-center justify-center"
            >
              <svg
                class="w-5 h-5 text-indigo-600 dark:text-indigo-400"
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
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600 dark:text-gray-400">
              {{ $t('ai.status.uptime') }}
            </p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">
              {{ formatUptime(uptime) }}
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- System Health -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-8">
      <!-- Server Metrics -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
          {{ $t('ai.status.serverMetrics') }}
        </h3>
        <div class="space-y-4">
          <div class="flex items-center justify-between">
            <span class="text-sm text-gray-600 dark:text-gray-400">{{
              $t('ai.status.cpuUsage')
            }}</span>
            <div class="flex items-center">
              <div class="w-24 bg-gray-200 dark:bg-gray-700 rounded-full h-2 mr-2">
                <div class="bg-blue-600 h-2 rounded-full" :style="{ width: cpuUsage + '%' }"></div>
              </div>
              <span class="text-sm font-medium text-gray-900 dark:text-white">{{ cpuUsage }}%</span>
            </div>
          </div>
          <div class="flex items-center justify-between">
            <span class="text-sm text-gray-600 dark:text-gray-400">{{
              $t('ai.status.memoryUsage')
            }}</span>
            <div class="flex items-center">
              <div class="w-24 bg-gray-200 dark:bg-gray-700 rounded-full h-2 mr-2">
                <div
                  class="bg-green-600 h-2 rounded-full"
                  :style="{ width: memoryUsage + '%' }"
                ></div>
              </div>
              <span class="text-sm font-medium text-gray-900 dark:text-white"
                >{{ memoryUsage }}%</span
              >
            </div>
          </div>
          <div class="flex items-center justify-between">
            <span class="text-sm text-gray-600 dark:text-gray-400">{{
              $t('ai.status.responseTime')
            }}</span>
            <span class="text-sm font-medium text-gray-900 dark:text-white"
              >{{ avgResponseTime }}ms</span
            >
          </div>
          <div class="flex items-center justify-between">
            <span class="text-sm text-gray-600 dark:text-gray-400">{{
              $t('ai.status.throughput')
            }}</span>
            <span class="text-sm font-medium text-gray-900 dark:text-white"
              >{{ throughput }}/min</span
            >
          </div>
        </div>
      </div>

      <!-- Recent Events -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
          {{ $t('ai.status.recentEvents') }}
        </h3>
        <div class="space-y-3 max-h-64 overflow-y-auto">
          <div v-for="event in recentEvents" :key="event.id" class="flex items-start space-x-3">
            <div
              :class="[
                'w-2 h-2 rounded-full mt-2',
                event.type === 'info'
                  ? 'bg-blue-500'
                  : event.type === 'warning'
                    ? 'bg-yellow-500'
                    : 'bg-red-500',
              ]"
            ></div>
            <div class="flex-1 min-w-0">
              <p class="text-sm text-gray-900 dark:text-white">
                {{ event.message }}
              </p>
              <p class="text-xs text-gray-500 dark:text-gray-400">
                {{ formatDate(event.timestamp) }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Active Connections -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden mb-8">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white">
          {{ $t('ai.status.activeConnections') }}
        </h3>
      </div>
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider"
              >
                {{ $t('ai.status.clientId') }}
              </th>
              <th
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider"
              >
                {{ $t('ai.status.tenant') }}
              </th>
              <th
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider"
              >
                {{ $t('ai.status.connectedSince') }}
              </th>
              <th
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider"
              >
                {{ $t('ai.status.lastActivity') }}
              </th>
              <th
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider"
              >
                {{ $t('ai.status.status') }}
              </th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
            <tr v-for="connection in activeConnections" :key="connection.id">
              <td
                class="px-6 py-4 whitespace-nowrap text-sm font-mono text-gray-900 dark:text-white"
              >
                {{ connection.clientId }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ connection.tenant }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ formatDate(connection.connectedSince) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ formatRelativeTime(connection.lastActivity) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span
                  :class="[
                    'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                    connection.status === 'active'
                      ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
                      : 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300',
                  ]"
                >
                  {{ connection.status }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Control Panel -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
      <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
        {{ $t('ai.status.serverControls') }}
      </h3>
      <div class="flex flex-wrap gap-4">
        <button
          @click="restartServer"
          class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          :disabled="restarting"
        >
          {{ restarting ? $t('ai.status.restarting') : $t('ai.status.restartServer') }}
        </button>
        <button
          @click="clearCache"
          class="px-4 py-2 bg-yellow-600 text-white rounded-lg hover:bg-yellow-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          :disabled="clearingCache"
        >
          {{ clearingCache ? $t('ai.status.clearing') : $t('ai.status.clearCache') }}
        </button>
        <button
          @click="exportLogs"
          class="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors"
        >
          {{ $t('ai.status.exportLogs') }}
        </button>
        <button
          @click="runHealthCheck"
          class="px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          :disabled="runningHealthCheck"
        >
          {{ runningHealthCheck ? $t('ai.status.checking') : $t('ai.status.runHealthCheck') }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';

interface Event {
  id: string;
  type: 'info' | 'warning' | 'error';
  message: string;
  timestamp: Date;
}

interface Connection {
  id: string;
  clientId: string;
  tenant: string;
  connectedSince: Date;
  lastActivity: Date;
  status: 'active' | 'idle';
}

const serverStatus = ref<'healthy' | 'warning' | 'error'>('healthy');
const activeClients = ref(12);
const activeSessions = ref(8);
const uptime = ref(345600); // seconds

const cpuUsage = ref(45);
const memoryUsage = ref(67);
const avgResponseTime = ref(125);
const throughput = ref(234);

const restarting = ref(false);
const clearingCache = ref(false);
const runningHealthCheck = ref(false);

const recentEvents = ref<Event[]>([
  {
    id: '1',
    type: 'info',
    message: 'New client connected: tenant-abc-123',
    timestamp: new Date(Date.now() - 1000 * 60 * 2),
  },
  {
    id: '2',
    type: 'warning',
    message: 'High memory usage detected (78%)',
    timestamp: new Date(Date.now() - 1000 * 60 * 15),
  },
  {
    id: '3',
    type: 'info',
    message: 'Cache cleared successfully',
    timestamp: new Date(Date.now() - 1000 * 60 * 30),
  },
  {
    id: '4',
    type: 'error',
    message: 'Failed to connect to AI provider: OpenAI',
    timestamp: new Date(Date.now() - 1000 * 60 * 45),
  },
]);

const activeConnections = ref<Connection[]>([
  {
    id: '1',
    clientId: 'client-abc-123',
    tenant: 'tenant-alpha',
    connectedSince: new Date(Date.now() - 1000 * 60 * 60 * 2),
    lastActivity: new Date(Date.now() - 1000 * 60 * 5),
    status: 'active',
  },
  {
    id: '2',
    clientId: 'client-def-456',
    tenant: 'tenant-beta',
    connectedSince: new Date(Date.now() - 1000 * 60 * 60 * 1),
    lastActivity: new Date(Date.now() - 1000 * 60 * 2),
    status: 'active',
  },
  {
    id: '3',
    clientId: 'client-ghi-789',
    tenant: 'tenant-gamma',
    connectedSince: new Date(Date.now() - 1000 * 60 * 60 * 3),
    lastActivity: new Date(Date.now() - 1000 * 60 * 30),
    status: 'idle',
  },
]);

const formatUptime = (seconds: number): string => {
  const days = Math.floor(seconds / 86400);
  const hours = Math.floor((seconds % 86400) / 3600);
  const minutes = Math.floor((seconds % 3600) / 60);

  if (days > 0) return `${days}d ${hours}h`;
  if (hours > 0) return `${hours}h ${minutes}m`;
  return `${minutes}m`;
};

const formatDate = (date: Date): string => {
  return new Intl.DateTimeFormat('en-US', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  }).format(date);
};

const formatRelativeTime = (date: Date): string => {
  const now = new Date();
  const diffMs = now.getTime() - date.getTime();
  const diffMins = Math.floor(diffMs / (1000 * 60));

  if (diffMins < 1) return 'Just now';
  if (diffMins < 60) return `${diffMins}m ago`;

  const diffHours = Math.floor(diffMins / 60);
  if (diffHours < 24) return `${diffHours}h ago`;

  const diffDays = Math.floor(diffHours / 24);
  return `${diffDays}d ago`;
};

const restartServer = async () => {
  restarting.value = true;
  // TODO: Implement server restart API call
  console.log('Restarting MCP server...');

  // Mock restart
  setTimeout(() => {
    restarting.value = false;
    // Add restart event
    recentEvents.value.unshift({
      id: Date.now().toString(),
      type: 'info',
      message: 'MCP server restarted successfully',
      timestamp: new Date(),
    });
  }, 3000);
};

const clearCache = async () => {
  clearingCache.value = true;
  // TODO: Implement cache clearing API call
  console.log('Clearing MCP server cache...');

  // Mock cache clear
  setTimeout(() => {
    clearingCache.value = false;
    // Add cache clear event
    recentEvents.value.unshift({
      id: Date.now().toString(),
      type: 'info',
      message: 'MCP server cache cleared successfully',
      timestamp: new Date(),
    });
  }, 2000);
};

const exportLogs = () => {
  // TODO: Implement log export API call
  console.log('Exporting MCP server logs...');
  // This would typically trigger a download
};

const runHealthCheck = async () => {
  runningHealthCheck.value = true;
  // TODO: Implement health check API call
  console.log('Running MCP server health check...');

  // Mock health check
  setTimeout(() => {
    runningHealthCheck.value = false;
    // Add health check event
    recentEvents.value.unshift({
      id: Date.now().toString(),
      type: 'info',
      message: 'Health check completed successfully',
      timestamp: new Date(),
    });
  }, 1500);
};

onMounted(() => {
  // TODO: Load MCP status data from API
  // loadMcpStatus()
});
</script>
