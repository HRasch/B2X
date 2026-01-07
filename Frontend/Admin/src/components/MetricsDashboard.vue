<template>
  <div class="metrics-dashboard">
    <div class="dashboard-header">
      <h2>{{ $t('dashboard.metrics.title') }}</h2>
      <div class="time-range-selector">
        <select v-model="selectedTimeRange" @change="updateDashboard">
          <option value="1h">{{ $t('dashboard.metrics.timeRanges.lastHour') }}</option>
          <option value="24h">{{ $t('dashboard.metrics.timeRanges.last24Hours') }}</option>
          <option value="7d">{{ $t('dashboard.metrics.timeRanges.last7Days') }}</option>
          <option value="30d">{{ $t('dashboard.metrics.timeRanges.last30Days') }}</option>
        </select>
      </div>
    </div>

    <div class="dashboard-grid">
      <!-- AI Resource Optimization Metrics -->
      <div class="metric-card">
        <h3>{{ $t('dashboard.metrics.aiResourceEfficiency') }}</h3>
        <div class="metric-value">
          <span class="value">{{ aiEfficiency }}%</span>
          <span
            class="change"
            :class="{ positive: aiEfficiencyChange > 0, negative: aiEfficiencyChange < 0 }"
          >
            {{ aiEfficiencyChange > 0 ? '+' : '' }}{{ aiEfficiencyChange }}%
          </span>
        </div>
        <div class="metric-chart">
          <canvas ref="aiEfficiencyChart"></canvas>
        </div>
      </div>

      <!-- Compliance Coverage -->
      <div class="metric-card">
        <h3>{{ $t('dashboard.metrics.complianceCoverage') }}</h3>
        <div class="compliance-status">
          <div
            v-for="jurisdiction in jurisdictions"
            :key="jurisdiction.code"
            class="jurisdiction-item"
          >
            <span class="jurisdiction-name">{{ jurisdiction.name }}</span>
            <span class="compliance-indicator" :class="{ compliant: jurisdiction.compliant }">
              {{ jurisdiction.compliant ? '✓' : '⚠' }}
            </span>
          </div>
        </div>
      </div>

      <!-- NLP Feedback Analytics -->
      <div class="metric-card">
        <h3>Feedback Analytics</h3>
        <div class="sentiment-breakdown">
          <div class="sentiment-item">
            <span class="label">Positive</span>
            <span class="value">{{ feedbackSentiment.positive }}%</span>
          </div>
          <div class="sentiment-item">
            <span class="label">Neutral</span>
            <span class="value">{{ feedbackSentiment.neutral }}%</span>
          </div>
          <div class="sentiment-item">
            <span class="label">Negative</span>
            <span class="value">{{ feedbackSentiment.negative }}%</span>
          </div>
        </div>
        <div class="feedback-insights">
          <h4>Key Insights</h4>
          <ul>
            <li v-for="insight in feedbackInsights" :key="insight">{{ insight }}</li>
          </ul>
        </div>
      </div>

      <!-- Performance Benchmarking -->
      <div class="metric-card">
        <h3>Performance Benchmarks</h3>
        <div class="benchmark-results">
          <div v-for="benchmark in benchmarks" :key="benchmark.name" class="benchmark-item">
            <span class="benchmark-name">{{ benchmark.name }}</span>
            <span class="benchmark-time">{{ benchmark.averageTime }}s</span>
            <span
              class="benchmark-change"
              :class="{ improved: benchmark.change < 0, degraded: benchmark.change > 0 }"
            >
              {{ benchmark.change > 0 ? '+' : '' }}{{ benchmark.change }}%
            </span>
          </div>
        </div>
      </div>

      <!-- Global Metrics Overview -->
      <div class="metric-card full-width">
        <h3>Global Metrics Overview</h3>
        <div class="global-metrics">
          <div class="metric-item">
            <span class="label">Active Users</span>
            <span class="value">{{ globalMetrics.activeUsers }}</span>
          </div>
          <div class="metric-item">
            <span class="label">API Requests</span>
            <span class="value">{{ globalMetrics.apiRequests }}</span>
          </div>
          <div class="metric-item">
            <span class="label">Error Rate</span>
            <span class="value">{{ globalMetrics.errorRate }}%</span>
          </div>
          <div class="metric-item">
            <span class="label">Avg Response Time</span>
            <span class="value">{{ globalMetrics.avgResponseTime }}ms</span>
          </div>
        </div>
        <div class="global-chart">
          <canvas ref="globalMetricsChart"></canvas>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { Chart, registerables } from 'chart.js';

// Register Chart.js components
Chart.register(...registerables);

// i18n
const { t } = useI18n();

// Reactive data
const selectedTimeRange = ref('24h');
const aiEfficiency = ref(85.2);
const aiEfficiencyChange = ref(12.5);
const aiEfficiencyChart = ref<HTMLCanvasElement>();

const jurisdictions = ref([
  { code: 'GDPR', name: 'EU GDPR', compliant: true },
  { code: 'CCPA', name: 'California CCPA', compliant: true },
  { code: 'PIPEDA', name: 'Canada PIPEDA', compliant: true },
]);

const feedbackSentiment = ref({
  positive: 68,
  neutral: 22,
  negative: 10,
});

const feedbackInsights = ref([
  'Performance improvements highly appreciated',
  'UI/UX feedback indicates need for mobile optimization',
  'Feature requests focused on integration capabilities',
]);

const benchmarks = ref([
  { name: 'Backend Build', averageTime: 45.2, change: -5.1 },
  { name: 'API Startup', averageTime: 12.8, change: 2.3 },
  { name: 'Database Query', averageTime: 0.8, change: -8.7 },
]);

const globalMetrics = ref({
  activeUsers: '12,543',
  apiRequests: '1.2M',
  errorRate: 0.12,
  avgResponseTime: 245,
});

const globalMetricsChart = ref<HTMLCanvasElement>();

// Methods
const updateDashboard = async () => {
  // Fetch new data based on time range
  console.log(`Updating dashboard for time range: ${selectedTimeRange.value}`);
  // Implementation would fetch real data from APIs
};

const initializeCharts = () => {
  // AI Efficiency Chart
  if (aiEfficiencyChart.value) {
    new Chart(aiEfficiencyChart.value, {
      type: 'line',
      data: {
        labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
        datasets: [
          {
            label: 'AI Efficiency %',
            data: [78, 82, 79, 85, 88, 86, 85],
            borderColor: '#4CAF50',
            backgroundColor: 'rgba(76, 175, 80, 0.1)',
            tension: 0.4,
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false },
        },
        scales: {
          y: { beginAtZero: false },
        },
      },
    });
  }

  // Global Metrics Chart
  if (globalMetricsChart.value) {
    new Chart(globalMetricsChart.value, {
      type: 'bar',
      data: {
        labels: ['Users', 'Requests', 'Errors', 'Response Time'],
        datasets: [
          {
            label: 'Current Metrics',
            data: [12453, 1200000, 0.12, 245],
            backgroundColor: ['#2196F3', '#4CAF50', '#FF9800', '#9C27B0'],
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false },
        },
      },
    });
  }
};

// Lifecycle
onMounted(() => {
  initializeCharts();
  updateDashboard();
});

onUnmounted(() => {
  // Cleanup charts if needed
});
</script>

<style scoped>
.metrics-dashboard {
  padding: 20px;
  background: #f5f5f5;
  min-height: 100vh;
}

.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
  background: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.dashboard-header h2 {
  margin: 0;
  color: #333;
}

.time-range-selector select {
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  background: white;
}

.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 20px;
}

.metric-card {
  background: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.metric-card.full-width {
  grid-column: 1 / -1;
}

.metric-card h3 {
  margin: 0 0 15px 0;
  color: #333;
  font-size: 18px;
}

.metric-value {
  display: flex;
  align-items: baseline;
  gap: 10px;
  margin-bottom: 15px;
}

.metric-value .value {
  font-size: 32px;
  font-weight: bold;
  color: #2196f3;
}

.metric-value .change {
  font-size: 14px;
  padding: 2px 6px;
  border-radius: 4px;
}

.metric-value .change.positive {
  background: #e8f5e8;
  color: #4caf50;
}

.metric-value .change.negative {
  background: #ffebee;
  color: #f44336;
}

.metric-chart {
  height: 200px;
}

.compliance-status {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.jurisdiction-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 12px;
  background: #f8f9fa;
  border-radius: 4px;
}

.compliance-indicator.compliant {
  color: #4caf50;
  font-weight: bold;
}

.sentiment-breakdown {
  display: flex;
  justify-content: space-around;
  margin-bottom: 20px;
}

.sentiment-item {
  text-align: center;
}

.sentiment-item .label {
  display: block;
  font-size: 12px;
  color: #666;
  margin-bottom: 5px;
}

.sentiment-item .value {
  display: block;
  font-size: 24px;
  font-weight: bold;
  color: #2196f3;
}

.feedback-insights h4 {
  margin: 15px 0 10px 0;
  color: #333;
}

.feedback-insights ul {
  margin: 0;
  padding-left: 20px;
}

.feedback-insights li {
  margin-bottom: 5px;
  color: #555;
}

.benchmark-results {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.benchmark-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 12px;
  background: #f8f9fa;
  border-radius: 4px;
}

.benchmark-name {
  flex: 1;
}

.benchmark-time {
  font-weight: bold;
  color: #2196f3;
}

.benchmark-change.improved {
  color: #4caf50;
}

.benchmark-change.degraded {
  color: #f44336;
}

.global-metrics {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 15px;
  margin-bottom: 20px;
}

.metric-item {
  text-align: center;
  padding: 15px;
  background: #f8f9fa;
  border-radius: 4px;
}

.metric-item .label {
  display: block;
  font-size: 12px;
  color: #666;
  margin-bottom: 5px;
}

.metric-item .value {
  display: block;
  font-size: 24px;
  font-weight: bold;
  color: #2196f3;
}

.global-chart {
  height: 300px;
}

@media (max-width: 768px) {
  .dashboard-grid {
    grid-template-columns: 1fr;
  }

  .dashboard-header {
    flex-direction: column;
    gap: 15px;
    text-align: center;
  }

  .global-metrics {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>
