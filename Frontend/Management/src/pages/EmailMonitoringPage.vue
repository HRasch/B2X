<template>
  <div class="email-monitoring-page">
    <div class="header">
      <h1>Email Monitoring</h1>
      <p>Monitor email delivery, failures, and performance metrics</p>
    </div>

    <!-- Health Status Cards -->
    <div class="health-cards">
      <div
        class="health-card"
        :class="{ healthy: healthStatus.isHealthy, unhealthy: !healthStatus.isHealthy }"
      >
        <h3>System Health</h3>
        <div class="health-indicator">
          <span
            class="status-dot"
            :class="{ healthy: healthStatus.isHealthy, unhealthy: !healthStatus.isHealthy }"
          ></span>
          <span class="status-text">{{
            healthStatus.isHealthy ? 'Healthy' : 'Issues Detected'
          }}</span>
        </div>
      </div>

      <div class="metric-card">
        <h3>Emails Sent (24h)</h3>
        <p class="metric-value">{{ healthStatus.totalEmailsLast24Hours }}</p>
      </div>

      <div class="metric-card" :class="{ error: healthStatus.failedEmailsLast24Hours > 0 }">
        <h3>Failed Emails (24h)</h3>
        <p class="metric-value">{{ healthStatus.failedEmailsLast24Hours }}</p>
      </div>

      <div class="metric-card" :class="{ warning: healthStatus.bouncedEmailsLast24Hours > 0 }">
        <h3>Bounced Emails (24h)</h3>
        <p class="metric-value">{{ healthStatus.bouncedEmailsLast24Hours }}</p>
      </div>
    </div>

    <!-- Date Range Selector -->
    <div class="date-range-selector">
      <label for="dateRange">Time Range:</label>
      <select id="dateRange" v-model="selectedRange" @change="updateDateRange">
        <option value="24h">Last 24 Hours</option>
        <option value="7d">Last 7 Days</option>
        <option value="30d">Last 30 Days</option>
        <option value="custom">Custom Range</option>
      </select>

      <div v-if="selectedRange === 'custom'" class="custom-range">
        <input type="date" v-model="fromDate" @change="loadData" />
        <span>to</span>
        <input type="date" v-model="toDate" @change="loadData" />
      </div>
    </div>

    <!-- Statistics Cards -->
    <div class="stats-grid" v-if="statistics">
      <div class="stat-card">
        <h3>Total Sent</h3>
        <p class="stat-value">{{ statistics.totalSent }}</p>
      </div>

      <div class="stat-card">
        <h3>Delivered</h3>
        <p class="stat-value">{{ statistics.totalDelivered }}</p>
        <p class="stat-rate">{{ statistics.deliveryRate.toFixed(1) }}%</p>
      </div>

      <div class="stat-card">
        <h3>Opened</h3>
        <p class="stat-value">{{ statistics.totalOpened }}</p>
        <p class="stat-rate">{{ statistics.openRate.toFixed(1) }}%</p>
      </div>

      <div class="stat-card">
        <h3>Clicked</h3>
        <p class="stat-value">{{ statistics.totalClicked }}</p>
        <p class="stat-rate">{{ statistics.clickRate.toFixed(1) }}%</p>
      </div>

      <div class="stat-card error">
        <h3>Bounced</h3>
        <p class="stat-value">{{ statistics.totalBounced }}</p>
        <p class="stat-rate">{{ statistics.bounceRate.toFixed(1) }}%</p>
      </div>

      <div class="stat-card error">
        <h3>Failed</h3>
        <p class="stat-value">{{ statistics.totalFailed }}</p>
      </div>
    </div>

    <!-- Recent Events Table -->
    <div class="events-section">
      <h2>Recent Email Events</h2>
      <div class="events-table-container">
        <table class="events-table" v-if="recentEvents.length > 0">
          <thead>
            <tr>
              <th>Time</th>
              <th>Event Type</th>
              <th>Email ID</th>
              <th>Details</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="event in recentEvents"
              :key="event.id"
              :class="getEventClass(event.eventType)"
            >
              <td>{{ formatDateTime(event.occurredAt) }}</td>
              <td>{{ getEventTypeLabel(event.eventType) }}</td>
              <td>{{ event.emailId }}</td>
              <td>{{ event.metadata || '-' }}</td>
            </tr>
          </tbody>
        </table>
        <p v-else class="no-events">No recent email events found.</p>
      </div>
    </div>

    <!-- Error Events Section -->
    <div class="errors-section" v-if="errorEvents.length > 0">
      <h2>Email Errors & Issues</h2>
      <div class="events-table-container">
        <table class="events-table">
          <thead>
            <tr>
              <th>Time</th>
              <th>Error Type</th>
              <th>Email ID</th>
              <th>Error Details</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="error in errorEvents" :key="error.id" class="error-row">
              <td>{{ formatDateTime(error.occurredAt) }}</td>
              <td>{{ getEventTypeLabel(error.eventType) }}</td>
              <td>{{ error.emailId }}</td>
              <td>{{ error.metadata || 'Unknown error' }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Test Email Section -->
    <div class="test-email-section">
      <h2>Send Test Email</h2>
      <form @submit.prevent="sendTestEmail" class="test-email-form">
        <div class="form-group">
          <label for="testEmail">Test Email Address:</label>
          <input
            id="testEmail"
            type="email"
            v-model="testEmailAddress"
            placeholder="your.email@example.com"
            required
          />
        </div>

        <div class="form-group">
          <label for="testSubject">Subject:</label>
          <input
            id="testSubject"
            type="text"
            v-model="testSubject"
            placeholder="Test Email Subject"
            required
          />
        </div>

        <div class="form-group">
          <label for="testBody">Message:</label>
          <textarea
            id="testBody"
            v-model="testBody"
            placeholder="Test email content..."
            rows="4"
            required
          ></textarea>
        </div>

        <div class="form-actions">
          <button type="submit" :disabled="sendingTest" class="send-test-btn">
            {{ sendingTest ? 'Sending...' : 'Send Test Email' }}
          </button>
        </div>

        <div
          v-if="testResult"
          class="test-result"
          :class="{ success: testResult.success, error: !testResult.success }"
        >
          <p>{{ testResult.message }}</p>
          <p v-if="testResult.messageId">Message ID: {{ testResult.messageId }}</p>
        </div>
      </form>
    </div>

    <!-- SMTP Configuration Section -->
    <div class="smtp-config-section">
      <h2>SMTP Configuration</h2>
      <p>Configure SMTP settings for email delivery</p>

      <div class="config-tabs">
        <button
          :class="{ active: activeTab === 'settings' }"
          @click="activeTab = 'settings'"
          class="tab-button"
        >
          Settings
        </button>
        <button
          :class="{ active: activeTab === 'test' }"
          @click="activeTab = 'test'"
          class="tab-button"
        >
          Test Connection
        </button>
      </div>

      <!-- SMTP Settings Tab -->
      <div v-if="activeTab === 'settings'" class="tab-content">
        <form @submit.prevent="saveSmtpSettings" class="smtp-form">
          <div class="form-grid">
            <div class="form-group">
              <label for="smtpHost">SMTP Host:</label>
              <input
                id="smtpHost"
                type="text"
                v-model="smtpSettings.host"
                placeholder="smtp.example.com"
                required
              />
            </div>

            <div class="form-group">
              <label for="smtpPort">Port:</label>
              <input
                id="smtpPort"
                type="number"
                v-model.number="smtpSettings.port"
                min="1"
                max="65535"
                placeholder="587"
                required
              />
            </div>

            <div class="form-group">
              <label for="smtpUsername">Username:</label>
              <input
                id="smtpUsername"
                type="text"
                v-model="smtpSettings.username"
                placeholder="your-username"
              />
            </div>

            <div class="form-group">
              <label for="smtpPassword">Password:</label>
              <input
                id="smtpPassword"
                type="password"
                v-model="smtpSettings.password"
                placeholder="your-password"
              />
            </div>

            <div class="form-group">
              <label for="fromEmail">From Email:</label>
              <input
                id="fromEmail"
                type="email"
                v-model="smtpSettings.fromEmail"
                placeholder="noreply@yourdomain.com"
                required
              />
            </div>

            <div class="form-group">
              <label for="fromName">From Name:</label>
              <input
                id="fromName"
                type="text"
                v-model="smtpSettings.fromName"
                placeholder="Your Company Name"
              />
            </div>

            <div class="form-group">
              <label for="domain">Domain:</label>
              <input
                id="domain"
                type="text"
                v-model="smtpSettings.domain"
                placeholder="yourdomain.com"
              />
            </div>

            <div class="form-group">
              <label for="timeout">Timeout (ms):</label>
              <input
                id="timeout"
                type="number"
                v-model.number="smtpSettings.timeout"
                min="1000"
                max="120000"
                placeholder="30000"
              />
            </div>
          </div>

          <div class="checkbox-group">
            <label class="checkbox-label">
              <input type="checkbox" v-model="smtpSettings.enableSsl" />
              Enable SSL/TLS
            </label>
          </div>

          <div class="form-actions">
            <button type="submit" :disabled="savingSettings" class="save-btn">
              {{ savingSettings ? 'Saving...' : 'Save Settings' }}
            </button>
            <button type="button" @click="loadSmtpSettings" class="load-btn">
              Reload Settings
            </button>
          </div>

          <div
            v-if="settingsResult"
            class="settings-result"
            :class="{ success: settingsResult.success, error: !settingsResult.success }"
          >
            <p>{{ settingsResult.message }}</p>
          </div>
        </form>
      </div>

      <!-- SMTP Test Connection Tab -->
      <div v-if="activeTab === 'test'" class="tab-content">
        <div class="test-connection-form">
          <p>Test the SMTP connection with your current settings.</p>

          <div class="form-actions">
            <button
              @click="testSmtpConnection"
              :disabled="testingConnection"
              class="test-connection-btn"
            >
              {{ testingConnection ? 'Testing...' : 'Test Connection' }}
            </button>
          </div>

          <div
            v-if="connectionTestResult"
            class="connection-result"
            :class="{ success: connectionTestResult.success, error: !connectionTestResult.success }"
          >
            <p>{{ connectionTestResult.message }}</p>
            <p v-if="connectionTestResult.responseTime">
              Response Time: {{ connectionTestResult.responseTime }}ms
            </p>
            <p v-if="connectionTestResult.testedAt">
              Tested At: {{ formatDateTime(connectionTestResult.testedAt) }}
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { emailMonitoringService } from '@/services/emailMonitoringService';

// Types
interface EmailStatistics {
  totalSent: number;
  totalDelivered: number;
  totalOpened: number;
  totalClicked: number;
  totalBounced: number;
  totalFailed: number;
  openRate: number;
  clickRate: number;
  bounceRate: number;
  deliveryRate: number;
}

interface EmailEvent {
  id: string;
  emailId: string;
  eventType: number;
  metadata?: string;
  occurredAt: string;
}

interface EmailHealthStatus {
  totalEmailsLast24Hours: number;
  failedEmailsLast24Hours: number;
  bouncedEmailsLast24Hours: number;
  lastEmailSent?: string;
  isHealthy: boolean;
}

// Reactive data
const selectedRange = ref('7d');
const fromDate = ref('');
const toDate = ref('');
const statistics = ref<EmailStatistics | null>(null);
const recentEvents = ref<EmailEvent[]>([]);
const errorEvents = ref<EmailEvent[]>([]);
const healthStatus = ref<EmailHealthStatus>({
  totalEmailsLast24Hours: 0,
  failedEmailsLast24Hours: 0,
  bouncedEmailsLast24Hours: 0,
  isHealthy: true,
});
const loading = ref(false);
const error = ref('');

// Test email data
const testEmailAddress = ref('');
const testSubject = ref('Email System Test');
const testBody = ref(
  'This is a test email to verify the email monitoring system is working correctly.'
);
const sendingTest = ref(false);
const testResult = ref<{ success: boolean; message: string; messageId?: string } | null>(null);

// SMTP Configuration data
const activeTab = ref('settings');
const smtpSettings = ref({
  host: '',
  port: 587,
  username: '',
  password: '',
  enableSsl: true,
  timeout: 30000,
  fromEmail: '',
  fromName: '',
  domain: '',
});
const savingSettings = ref(false);
const settingsResult = ref<{ success: boolean; message: string } | null>(null);
const testingConnection = ref(false);
const connectionTestResult = ref<{
  success: boolean;
  message: string;
  responseTime?: number;
  testedAt?: string;
} | null>(null);

// Methods
const updateDateRange = () => {
  const now = new Date();
  const today = now.toISOString().split('T')[0];

  switch (selectedRange.value) {
    case '24h':
      fromDate.value = new Date(now.getTime() - 24 * 60 * 60 * 1000).toISOString().split('T')[0];
      toDate.value = today;
      break;
    case '7d':
      fromDate.value = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000)
        .toISOString()
        .split('T')[0];
      toDate.value = today;
      break;
    case '30d':
      fromDate.value = new Date(now.getTime() - 30 * 24 * 60 * 60 * 1000)
        .toISOString()
        .split('T')[0];
      toDate.value = today;
      break;
  }

  loadData();
};

const loadData = async () => {
  loading.value = true;
  error.value = '';

  try {
    // Load health status
    const healthResponse = await emailMonitoringService.getHealthStatus();
    healthStatus.value = healthResponse.data;

    // Load statistics
    const statsResponse = await emailMonitoringService.getStatistics(fromDate.value, toDate.value);
    statistics.value = statsResponse.data;

    // Load recent events
    const eventsResponse = await emailMonitoringService.getRecentEvents();
    recentEvents.value = eventsResponse.data;

    // Load error events
    const errorsResponse = await emailMonitoringService.getErrorEvents(
      fromDate.value,
      toDate.value
    );
    errorEvents.value = errorsResponse.data;
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Unknown error occurred';
    console.error('Error loading email monitoring data:', err);
  } finally {
    loading.value = false;
  }
};

const formatDateTime = (dateString: string) => {
  return new Date(dateString).toLocaleString();
};

const getEventTypeLabel = (eventType: number) => {
  const types = [
    'Sent',
    'Delivered',
    'Opened',
    'Clicked',
    'Bounced',
    'Complained',
    'Unsubscribed',
    'Failed',
  ];
  return types[eventType] || 'Unknown';
};

const sendTestEmail = async () => {
  sendingTest.value = true;
  testResult.value = null;

  try {
    const response = await emailMonitoringService.sendTestEmail(
      testEmailAddress.value,
      testSubject.value,
      testBody.value
    );

    testResult.value = {
      success: response.data.success,
      message: response.data.success
        ? 'Test email sent successfully!'
        : `Failed to send test email: ${response.data.errorMessage}`,
      messageId: response.data.messageId,
    };

    // Reload data to show the new test email event
    await loadData();
  } catch (err) {
    testResult.value = {
      success: false,
      message: err instanceof Error ? err.message : 'Unknown error occurred',
    };
  }
};

// SMTP Configuration methods
const loadSmtpSettings = async () => {
  try {
    const response = await emailMonitoringService.getSmtpConfiguration();
    smtpSettings.value = response.data;
  } catch (err) {
    console.error('Error loading SMTP settings:', err);
    // Keep default values if loading fails
  }
};

const saveSmtpSettings = async () => {
  savingSettings.value = true;
  settingsResult.value = null;

  try {
    await emailMonitoringService.updateSmtpConfiguration(smtpSettings.value);
    settingsResult.value = {
      success: true,
      message: 'SMTP settings saved successfully!',
    };
  } catch (err) {
    settingsResult.value = {
      success: false,
      message: err instanceof Error ? err.message : 'Failed to save SMTP settings',
    };
  } finally {
    savingSettings.value = false;
  }
};

const testSmtpConnection = async () => {
  testingConnection.value = true;
  connectionTestResult.value = null;

  try {
    const response = await emailMonitoringService.testSmtpConnection(smtpSettings.value);
    connectionTestResult.value = response.data;
  } catch (err) {
    connectionTestResult.value = {
      success: false,
      message: err instanceof Error ? err.message : 'Failed to test SMTP connection',
    };
  } finally {
    testingConnection.value = false;
  }
};

// Initialize
onMounted(() => {
  updateDateRange();
  loadSmtpSettings();
});
</script>

<style scoped>
.email-monitoring-page {
  padding: 2rem 0;
  max-width: 1200px;
  margin: 0 auto;
}

.header {
  margin-bottom: 2rem;
}

.header h1 {
  color: #333;
  margin-bottom: 0.5rem;
}

.header p {
  color: #666;
  margin: 0;
}

.health-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.health-card,
.metric-card {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  border: 1px solid #e0e0e0;
}

.health-card.healthy {
  border-color: #10b981;
}

.health-card.unhealthy {
  border-color: #ef4444;
}

.health-indicator {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.status-dot {
  width: 12px;
  height: 12px;
  border-radius: 50%;
}

.status-dot.healthy {
  background-color: #10b981;
}

.status-dot.unhealthy {
  background-color: #ef4444;
}

.status-text {
  font-weight: 500;
}

.metric-card.error {
  border-color: #ef4444;
}

.metric-card.warning {
  border-color: #f59e0b;
}

.metric-value {
  font-size: 2rem;
  font-weight: bold;
  margin: 0.5rem 0;
  color: #333;
}

.date-range-selector {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  margin-bottom: 2rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  border: 1px solid #e0e0e0;
}

.date-range-selector label {
  font-weight: 500;
  margin-right: 1rem;
}

.custom-range {
  margin-top: 1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.stat-card {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  border: 1px solid #e0e0e0;
  text-align: center;
}

.stat-card.error {
  border-color: #ef4444;
}

.stat-card h3 {
  margin: 0 0 1rem 0;
  color: #666;
  font-size: 0.9rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.stat-value {
  font-size: 2rem;
  font-weight: bold;
  margin: 0;
  color: #333;
}

.stat-rate {
  font-size: 0.9rem;
  color: #666;
  margin: 0.5rem 0 0 0;
}

.events-section,
.errors-section {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  margin-bottom: 2rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  border: 1px solid #e0e0e0;
}

.events-section h2,
.errors-section h2 {
  margin: 0 0 1rem 0;
  color: #333;
}

.events-table-container {
  overflow-x: auto;
}

.events-table {
  width: 100%;
  border-collapse: collapse;
}

.events-table th,
.events-table td {
  padding: 0.75rem;
  text-align: left;
  border-bottom: 1px solid #e0e0e0;
}

.events-table th {
  background-color: #f8f9fa;
  font-weight: 600;
  color: #666;
}

.events-table tr.sent {
  background-color: #f0f9ff;
}

.events-table tr.delivered {
  background-color: #f0fdf4;
}

.events-table tr.opened {
  background-color: #fefce8;
}

.events-table tr.clicked {
  background-color: #f0f9ff;
}

.events-table tr.bounced,
.events-table tr.failed,
.error-row {
  background-color: #fef2f2;
}

.no-events {
  text-align: center;
  color: #666;
  padding: 2rem;
  margin: 0;
}

.loading,
.error-message {
  text-align: center;
  padding: 2rem;
}

.loading p {
  color: #666;
}

.error-message p {
  color: #ef4444;
  margin-bottom: 1rem;
}

.error-message button {
  background: #ef4444;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
}

.error-message button:hover {
  background: #dc2626;
}

.test-email-section {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  margin-bottom: 2rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  border: 1px solid #e0e0e0;
}

.test-email-section h2 {
  margin: 0 0 1rem 0;
  color: #333;
}

.test-email-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-group label {
  font-weight: 500;
  color: #333;
}

.form-group input,
.form-group textarea {
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  font-size: 1rem;
}

.form-group input:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-actions {
  display: flex;
  justify-content: flex-end;
}

.send-test-btn {
  background: #3b82f6;
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 4px;
  font-size: 1rem;
  cursor: pointer;
  transition: background-color 0.2s;
}

.send-test-btn:hover:not(:disabled) {
  background: #2563eb;
}

.send-test-btn:disabled {
  background: #9ca3af;
  cursor: not-allowed;
}

.test-result {
  margin-top: 1rem;
  padding: 1rem;
  border-radius: 4px;
  border: 1px solid;
}

.test-result.success {
  background-color: #f0fdf4;
  border-color: #10b981;
  color: #065f46;
}

.test-result.error {
  background-color: #fef2f2;
  border-color: #ef4444;
  color: #991b1b;
}

/* SMTP Configuration Styles */
.smtp-config-section {
  background: white;
  border-radius: 8px;
  padding: 2rem;
  margin-top: 2rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  border: 1px solid #e0e0e0;
}

.smtp-config-section h2 {
  color: #333;
  margin-bottom: 0.5rem;
}

.smtp-config-section > p {
  color: #666;
  margin-bottom: 2rem;
}

.config-tabs {
  display: flex;
  border-bottom: 1px solid #e0e0e0;
  margin-bottom: 2rem;
}

.tab-button {
  background: none;
  border: none;
  padding: 1rem 2rem;
  cursor: pointer;
  font-size: 1rem;
  color: #666;
  border-bottom: 2px solid transparent;
  transition: all 0.2s;
}

.tab-button:hover {
  color: #333;
}

.tab-button.active {
  color: #3b82f6;
  border-bottom-color: #3b82f6;
}

.tab-content {
  min-height: 300px;
}

.smtp-form {
  max-width: 800px;
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1.5rem;
  margin-bottom: 1.5rem;
}

.checkbox-group {
  margin-bottom: 2rem;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: 500;
  color: #333;
  cursor: pointer;
}

.checkbox-label input[type='checkbox'] {
  width: auto;
  margin: 0;
}

.save-btn,
.load-btn,
.test-connection-btn {
  background: #10b981;
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 4px;
  font-size: 1rem;
  cursor: pointer;
  transition: background-color 0.2s;
  margin-left: 1rem;
}

.save-btn:hover:not(:disabled),
.test-connection-btn:hover:not(:disabled) {
  background: #059669;
}

.load-btn {
  background: #6b7280;
}

.load-btn:hover {
  background: #4b5563;
}

.save-btn:disabled,
.test-connection-btn:disabled {
  background: #9ca3af;
  cursor: not-allowed;
}

.settings-result,
.connection-result {
  margin-top: 1rem;
  padding: 1rem;
  border-radius: 4px;
  border: 1px solid;
}

.settings-result.success,
.connection-result.success {
  background-color: #f0fdf4;
  border-color: #10b981;
  color: #065f46;
}

.settings-result.error,
.connection-result.error {
  background-color: #fef2f2;
  border-color: #ef4444;
  color: #991b1b;
}

.test-connection-form {
  max-width: 600px;
}

.test-connection-form p {
  color: #666;
  margin-bottom: 2rem;
}
</style>
