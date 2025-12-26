<template>
  <div class="pimsync-dashboard">
    <!-- Header -->
    <div class="dashboard-header">
      <h1>📊 PIM Sync Dashboard</h1>
      <button @click="refreshDashboard" class="btn-refresh">🔄 Refresh</button>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      Loading dashboard data...
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="error-banner">
      ⚠️ Error: {{ error }}
      <button @click="refreshDashboard">Retry</button>
    </div>

    <!-- Dashboard Content -->
    <div v-else class="dashboard-content">
      <!-- Active Syncs Section -->
      <section
        class="section active-syncs"
        v-if="dashboard.activeSyncCount > 0"
      >
        <h2>🔄 Active Syncs ({{ dashboard.activeSyncCount }})</h2>

        <div
          v-for="sync in dashboard.activeSyncs"
          :key="sync.syncRunId"
          class="sync-card"
        >
          <!-- Provider Name -->
          <div class="sync-header">
            <h3>{{ sync.providerName || "🌐 All Providers" }}</h3>
            <span class="status-badge running">{{ sync.status }}</span>
          </div>

          <!-- Progress Bar -->
          <div class="progress-section">
            <div class="progress-bar-container">
              <div class="progress-bar">
                <div
                  class="progress-fill"
                  :style="{ width: sync.progressPercentage + '%' }"
                >
                  <span class="progress-text"
                    >{{ sync.progressPercentage.toFixed(1) }}%</span
                  >
                </div>
              </div>
            </div>
          </div>

          <!-- Details Grid -->
          <div class="details-grid">
            <div class="detail-item">
              <label>Products Processed</label>
              <span class="value"
                >{{ sync.productsProcessed }} / {{ sync.totalProducts }}</span
              >
            </div>
            <div class="detail-item">
              <label>Indexed</label>
              <span class="value success">{{ sync.productsIndexed }}</span>
            </div>
            <div class="detail-item">
              <label>Failed</label>
              <span class="value" :class="{ warning: sync.productsFailed > 0 }">
                {{ sync.productsFailed }}
              </span>
            </div>
            <div class="detail-item">
              <label>Current Language</label>
              <span class="value">{{ sync.currentLanguage || "-" }}</span>
            </div>
            <div class="detail-item">
              <label>Duration</label>
              <span class="value">{{ formatDuration(sync.duration) }}</span>
            </div>
            <div class="detail-item">
              <label>ETA</label>
              <span class="value">
                {{
                  sync.estimatedTimeRemaining
                    ? formatDuration(sync.estimatedTimeRemaining)
                    : "Calculating..."
                }}
              </span>
            </div>
          </div>

          <!-- Started At -->
          <div class="timestamp">
            Started: {{ formatDateTime(sync.startedAt) }}
          </div>
        </div>
      </section>

      <!-- No Active Syncs -->
      <div v-else class="no-active-syncs">✅ No active syncs</div>

      <!-- Latest Sync Section -->
      <section class="section latest-sync" v-if="dashboard.latestSync">
        <h2>📋 Latest Sync</h2>

        <div
          class="sync-card"
          :class="dashboard.latestSync.status.toLowerCase()"
        >
          <div class="sync-header">
            <h3>{{ dashboard.latestSync.providerName || "All Providers" }}</h3>
            <span
              class="status-badge"
              :class="dashboard.latestSync.status.toLowerCase()"
            >
              {{ dashboard.latestSync.status }}
            </span>
          </div>

          <!-- Details -->
          <div class="details-grid">
            <div class="detail-item">
              <label>Products Indexed</label>
              <span class="value">{{
                dashboard.latestSync.productsIndexed
              }}</span>
            </div>
            <div class="detail-item">
              <label>Duration</label>
              <span class="value">{{
                formatDuration(dashboard.latestSync.duration)
              }}</span>
            </div>
            <div class="detail-item">
              <label>Completed</label>
              <span class="value">{{
                formatDateTime(dashboard.latestSync.completedAt)
              }}</span>
            </div>
          </div>

          <!-- Error Message -->
          <div v-if="dashboard.latestSync.errorMessage" class="error-message">
            <strong>Error:</strong> {{ dashboard.latestSync.errorMessage }}
          </div>

          <!-- Detailed Errors -->
          <div
            v-if="dashboard.latestSync.detailedErrors.length > 0"
            class="detailed-errors"
          >
            <h4>Detailed Errors:</h4>
            <ul>
              <li
                v-for="(err, idx) in dashboard.latestSync.detailedErrors.slice(
                  0,
                  5
                )"
                :key="idx"
              >
                {{ err }}
              </li>
              <li v-if="dashboard.latestSync.detailedErrors.length > 5">
                ... and
                {{ dashboard.latestSync.detailedErrors.length - 5 }} more
              </li>
            </ul>
          </div>
        </div>
      </section>

      <!-- Statistics Section -->
      <section class="section statistics">
        <h2>📈 Statistics</h2>

        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-value">
              {{ dashboard.statistics.totalSyncsCompleted }}
            </div>
            <div class="stat-label">Completed Syncs</div>
          </div>

          <div class="stat-card">
            <div class="stat-value">
              {{ dashboard.statistics.totalSyncsFailed }}
            </div>
            <div class="stat-label">Failed Syncs</div>
          </div>

          <div class="stat-card">
            <div class="stat-value success">
              {{ dashboard.statistics.successRate.toFixed(2) }}%
            </div>
            <div class="stat-label">Success Rate</div>
          </div>

          <div class="stat-card">
            <div class="stat-value">
              {{ formatNumber(dashboard.statistics.totalProductsIndexed) }}
            </div>
            <div class="stat-label">Total Products Indexed</div>
          </div>

          <div class="stat-card">
            <div class="stat-value">
              {{ formatDuration(dashboard.statistics.averageSyncDuration) }}
            </div>
            <div class="stat-label">Avg Sync Duration</div>
          </div>
        </div>
      </section>

      <!-- Recent History Section -->
      <section
        class="section recent-history"
        v-if="dashboard.recentHistory.length > 0"
      >
        <h2>🕐 Recent History</h2>

        <div class="history-table-container">
          <table class="history-table">
            <thead>
              <tr>
                <th>Provider</th>
                <th>Status</th>
                <th>Products</th>
                <th>Duration</th>
                <th>Completed</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="sync in dashboard.recentHistory"
                :key="sync.syncRunId"
                :class="sync.status.toLowerCase()"
              >
                <td>{{ sync.providerName || "All" }}</td>
                <td>
                  <span class="status-badge" :class="sync.status.toLowerCase()">
                    {{ sync.status }}
                  </span>
                </td>
                <td>{{ sync.productsIndexed }}</td>
                <td>{{ formatDuration(sync.duration) }}</td>
                <td>{{ formatDateTime(sync.completedAt) }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from "vue";

const dashboard = ref(null);
const loading = ref(true);
const error = ref(null);
let pollInterval = null;

// Load dashboard data
const loadDashboard = async () => {
  try {
    const response = await fetch("/api/v2/pimsync/dashboard");
    if (!response.ok) throw new Error(`HTTP ${response.status}`);

    dashboard.value = await response.json();
    error.value = null;

    // Continue polling if there are active syncs
    if (dashboard.value.activeSyncCount === 0 && pollInterval) {
      clearInterval(pollInterval);
      pollInterval = null;
    }
  } catch (err) {
    error.value = err.message;
    console.error("Failed to load dashboard:", err);
  } finally {
    loading.value = false;
  }
};

const refreshDashboard = async () => {
  loading.value = true;
  await loadDashboard();
};

// Format helpers
const formatDateTime = (isoString) => {
  if (!isoString) return "-";
  return new Date(isoString).toLocaleString("de-DE", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
  });
};

const formatDuration = (timeSpan) => {
  if (!timeSpan) return "-";

  // Parse ISO 8601 duration or timespan object
  let totalSeconds = 0;

  if (typeof timeSpan === "string") {
    // ISO 8601: PT5H30M45S
    const regex = /PT(?:(\d+)H)?(?:(\d+)M)?(?:(\d+(?:\.\d+)?)S)?/;
    const match = timeSpan.match(regex);
    if (match) {
      totalSeconds =
        (parseInt(match[1]) || 0) * 3600 +
        (parseInt(match[2]) || 0) * 60 +
        (parseFloat(match[3]) || 0);
    }
  } else if (typeof timeSpan === "object" && "totalSeconds" in timeSpan) {
    totalSeconds = timeSpan.totalSeconds;
  }

  const hours = Math.floor(totalSeconds / 3600);
  const minutes = Math.floor((totalSeconds % 3600) / 60);
  const seconds = Math.floor(totalSeconds % 60);

  if (hours > 0) return `${hours}h ${minutes}m ${seconds}s`;
  if (minutes > 0) return `${minutes}m ${seconds}s`;
  return `${seconds}s`;
};

const formatNumber = (num) => {
  return new Intl.NumberFormat("de-DE").format(num);
};

// Lifecycle
onMounted(async () => {
  await loadDashboard();

  // Start polling if there are active syncs
  if (dashboard.value?.activeSyncCount > 0) {
    pollInterval = setInterval(loadDashboard, 5000);
  }
});

onUnmounted(() => {
  if (pollInterval) clearInterval(pollInterval);
});
</script>

<style scoped>
.pimsync-dashboard {
  padding: 24px;
  max-width: 1400px;
  margin: 0 auto;
}

.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 32px;
}

.dashboard-header h1 {
  margin: 0;
  font-size: 28px;
}

.btn-refresh {
  padding: 8px 16px;
  background: #4caf50;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
}

.btn-refresh:hover {
  background: #45a049;
}

/* Loading State */
.loading {
  text-align: center;
  padding: 48px 24px;
}

.spinner {
  border: 4px solid #e0e0e0;
  border-top: 4px solid #4caf50;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  animation: spin 1s linear infinite;
  margin: 0 auto 16px;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

/* Error Banner */
.error-banner {
  background: #ffebee;
  border: 1px solid #ef5350;
  border-radius: 4px;
  padding: 16px;
  margin-bottom: 24px;
  color: #c62828;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.error-banner button {
  padding: 6px 12px;
  background: #ef5350;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

/* Sections */
.section {
  background: white;
  border-radius: 8px;
  padding: 24px;
  margin-bottom: 24px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.12);
}

.section h2 {
  margin-top: 0;
  margin-bottom: 20px;
  font-size: 18px;
  color: #333;
}

/* Active Syncs */
.no-active-syncs {
  background: #e8f5e9;
  border: 1px solid #4caf50;
  border-radius: 4px;
  padding: 20px;
  text-align: center;
  color: #2e7d32;
}

.sync-card {
  background: #fafafa;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  padding: 16px;
  margin-bottom: 16px;
  transition: box-shadow 0.2s;
}

.sync-card:hover {
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.sync-card.completed {
  border-left: 4px solid #4caf50;
}

.sync-card.failed {
  border-left: 4px solid #ef5350;
}

.sync-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.sync-header h3 {
  margin: 0;
  font-size: 16px;
}

/* Status Badge */
.status-badge {
  display: inline-block;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
  text-transform: uppercase;
}

.status-badge.running {
  background: #e3f2fd;
  color: #1976d2;
}

.status-badge.completed {
  background: #e8f5e9;
  color: #388e3c;
}

.status-badge.failed {
  background: #ffebee;
  color: #d32f2f;
}

/* Progress Bar */
.progress-bar-container {
  margin-bottom: 12px;
}

.progress-bar {
  width: 100%;
  height: 28px;
  background: #e0e0e0;
  border-radius: 4px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #4caf50, #45a049);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: bold;
  font-size: 12px;
  transition: width 0.3s ease;
  min-width: 30px;
}

/* Details Grid */
.details-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 16px;
  margin-bottom: 12px;
}

.detail-item {
  display: flex;
  flex-direction: column;
}

.detail-item label {
  font-size: 12px;
  color: #666;
  text-transform: uppercase;
  margin-bottom: 4px;
}

.detail-item .value {
  font-size: 16px;
  font-weight: 500;
  color: #333;
}

.detail-item .value.success {
  color: #4caf50;
}

.detail-item .value.warning {
  color: #ff9800;
}

/* Timestamp */
.timestamp {
  font-size: 12px;
  color: #999;
  margin-top: 8px;
}

/* Error Message */
.error-message {
  background: #ffebee;
  border: 1px solid #ef5350;
  border-radius: 4px;
  padding: 12px;
  margin-top: 12px;
  color: #c62828;
  font-size: 14px;
}

.detailed-errors {
  background: #fff3cd;
  border: 1px solid #ffc107;
  border-radius: 4px;
  padding: 12px;
  margin-top: 12px;
  font-size: 12px;
}

.detailed-errors h4 {
  margin-top: 0;
}

.detailed-errors ul {
  margin: 8px 0 0 20px;
  list-style: disc;
}

.detailed-errors li {
  margin-bottom: 4px;
  font-family: monospace;
}

/* Statistics Grid */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 16px;
}

.stat-card {
  background: #f5f5f5;
  border-radius: 4px;
  padding: 20px;
  text-align: center;
}

.stat-value {
  font-size: 28px;
  font-weight: bold;
  color: #333;
  margin-bottom: 8px;
}

.stat-value.success {
  color: #4caf50;
}

.stat-label {
  font-size: 12px;
  color: #666;
  text-transform: uppercase;
}

/* History Table */
.history-table-container {
  overflow-x: auto;
}

.history-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 14px;
}

.history-table thead {
  background: #f5f5f5;
  border-bottom: 2px solid #e0e0e0;
}

.history-table th {
  padding: 12px;
  text-align: left;
  font-weight: 500;
  color: #666;
}

.history-table td {
  padding: 12px;
  border-bottom: 1px solid #e0e0e0;
}

.history-table tr.completed {
  background: #f1f8e9;
}

.history-table tr.failed {
  background: #ffebee;
}

/* Responsive */
@media (max-width: 768px) {
  .pimsync-dashboard {
    padding: 16px;
  }

  .dashboard-header {
    flex-direction: column;
    gap: 16px;
  }

  .details-grid,
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>
