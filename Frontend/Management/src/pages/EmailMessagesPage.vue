<template>
  <div class="email-messages-page">
    <div class="page-header">
      <h1>Email Message Management</h1>
      <p>Monitor and manage queued, sent, and failed email messages</p>
    </div>

    <!-- Filters and Controls -->
    <div class="filters-section">
      <div class="filter-controls">
        <div class="filter-group">
          <label for="tenant-filter">Tenant:</label>
          <select
            id="tenant-filter"
            v-model="selectedTenant"
            @change="loadEmails"
            class="filter-select"
          >
            <option value="">All Tenants</option>
            <option value="default-tenant">Default Tenant</option>
            <!-- Add more tenant options as needed -->
          </select>
        </div>
        <div class="filter-group">
          <label for="status-filter">Status:</label>
          <select
            id="status-filter"
            v-model="filters.status"
            @change="loadEmails"
            class="filter-select"
          >
            <option value="">All Statuses</option>
            <option value="Queued">Queued</option>
            <option value="Processing">Processing</option>
            <option value="Sent">Sent</option>
            <option value="Failed">Failed</option>
            <option value="Cancelled">Cancelled</option>
            <option value="Scheduled">Scheduled</option>
          </select>
        </div>

        <div class="filter-group">
          <label for="search-input">Search:</label>
          <input
            id="search-input"
            v-model="filters.search"
            @input="debouncedSearch"
            placeholder="Search by subject, recipient..."
            class="search-input"
          />
        </div>

        <button @click="loadEmails" class="refresh-btn">
          <i class="fas fa-sync-alt"></i>
          Refresh
        </button>
      </div>
    </div>

    <!-- Email Messages Table -->
    <div class="messages-table-container">
      <table v-if="emails.length > 0" class="messages-table">
        <thead>
          <tr>
            <th @click="sortBy('createdAt')" class="sortable">
              Created
              <i :class="getSortIcon('createdAt')"></i>
            </th>
            <th @click="sortBy('to')" class="sortable">
              To
              <i :class="getSortIcon('to')"></i>
            </th>
            <th @click="sortBy('subject')" class="sortable">
              Subject
              <i :class="getSortIcon('subject')"></i>
            </th>
            <th @click="sortBy('status')" class="sortable">
              Status
              <i :class="getSortIcon('status')"></i>
            </th>
            <th @click="sortBy('retryCount')" class="sortable">
              Retries
              <i :class="getSortIcon('retryCount')"></i>
            </th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="email in paginatedEmails" :key="email.id" :class="getRowClass(email)">
            <td>{{ formatDate(email.createdAt) }}</td>
            <td>{{ email.to }}</td>
            <td class="subject-cell">{{ email.subject }}</td>
            <td>
              <span :class="getStatusClass(email.status)">
                {{ email.status }}
              </span>
            </td>
            <td>{{ email.retryCount }}/{{ email.maxRetries }}</td>
            <td class="actions-cell">
              <button
                @click="viewEmailDetails(email)"
                class="action-btn view-btn"
                title="View Details"
              >
                <i class="fas fa-eye"></i>
              </button>

              <button
                v-if="email.status === 'Failed' && email.retryCount < email.maxRetries"
                @click="retryEmail(email)"
                class="action-btn retry-btn"
                title="Retry"
                :disabled="retryingEmails.has(email.id)"
              >
                <i class="fas fa-redo" :class="{ 'fa-spin': retryingEmails.has(email.id) }"></i>
              </button>

              <button
                v-if="canCancel(email)"
                @click="cancelEmail(email)"
                class="action-btn cancel-btn"
                title="Cancel"
                :disabled="cancellingEmails.has(email.id)"
              >
                <i class="fas fa-times" :class="{ 'fa-spin': cancellingEmails.has(email.id) }"></i>
              </button>
            </td>
          </tr>
        </tbody>
      </table>

      <div v-else class="no-data">
        <i class="fas fa-envelope-open"></i>
        <p>No email messages found</p>
      </div>
    </div>

    <!-- Pagination -->
    <div v-if="totalPages > 1" class="pagination">
      <button @click="goToPage(currentPage - 1)" :disabled="currentPage === 1" class="page-btn">
        <i class="fas fa-chevron-left"></i>
      </button>

      <span class="page-info">
        Page {{ currentPage }} of {{ totalPages }} ({{ totalEmails }} total)
      </span>

      <button
        @click="goToPage(currentPage + 1)"
        :disabled="currentPage === totalPages"
        class="page-btn"
      >
        <i class="fas fa-chevron-right"></i>
      </button>
    </div>

    <!-- Email Details Modal -->
    <div v-if="selectedEmail" class="modal-overlay" @click="closeModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2>Email Details</h2>
          <button @click="closeModal" class="close-btn">
            <i class="fas fa-times"></i>
          </button>
        </div>

        <div class="modal-body">
          <div class="detail-grid">
            <div class="detail-item">
              <label>ID:</label>
              <span>{{ selectedEmail.id }}</span>
            </div>

            <div class="detail-item">
              <label>To:</label>
              <span>{{ selectedEmail.to }}</span>
            </div>

            <div v-if="selectedEmail.cc" class="detail-item">
              <label>CC:</label>
              <span>{{ selectedEmail.cc }}</span>
            </div>

            <div v-if="selectedEmail.bcc" class="detail-item">
              <label>BCC:</label>
              <span>{{ selectedEmail.bcc }}</span>
            </div>

            <div class="detail-item">
              <label>Subject:</label>
              <span>{{ selectedEmail.subject }}</span>
            </div>

            <div class="detail-item">
              <label>Status:</label>
              <span :class="getStatusClass(selectedEmail.status)">
                {{ selectedEmail.status }}
              </span>
            </div>

            <div class="detail-item">
              <label>Priority:</label>
              <span>{{ selectedEmail.priority }}</span>
            </div>

            <div class="detail-item">
              <label>Created:</label>
              <span>{{ formatDate(selectedEmail.createdAt) }}</span>
            </div>

            <div v-if="selectedEmail.sentAt" class="detail-item">
              <label>Sent:</label>
              <span>{{ formatDate(selectedEmail.sentAt) }}</span>
            </div>

            <div v-if="selectedEmail.scheduledFor" class="detail-item">
              <label>Scheduled:</label>
              <span>{{ formatDate(selectedEmail.scheduledFor) }}</span>
            </div>

            <div class="detail-item">
              <label>Retries:</label>
              <span>{{ selectedEmail.retryCount }}/{{ selectedEmail.maxRetries }}</span>
            </div>

            <div v-if="selectedEmail.messageId" class="detail-item">
              <label>Message ID:</label>
              <span>{{ selectedEmail.messageId }}</span>
            </div>

            <div v-if="selectedEmail.lastError" class="detail-item full-width">
              <label>Last Error:</label>
              <span class="error-text">{{ selectedEmail.lastError }}</span>
            </div>
          </div>

          <div class="email-body">
            <label>Body:</label>
            <div class="body-content" v-html="safeEmailBody"></div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue';
import { emailMonitoringService } from '@/services/emailMonitoringService';
import { useSafeHtml } from '@/utils/sanitize';
import type { EmailMessage } from '@/services/emailMonitoringService';

// Types
interface EmailFilters {
  status: string;
  search: string;
}

// Reactive data
const emails = ref<EmailMessage[]>([]);
const selectedEmail = ref<EmailMessage | null>(null);
const loading = ref(false);
const retryingEmails = ref(new Set<string>());
const cancellingEmails = ref(new Set<string>());
const selectedTenant = ref('');

// Filters and pagination
const filters = ref<EmailFilters>({
  status: '',
  search: '',
});

const currentPage = ref(1);
const pageSize = ref(25);
const totalEmails = ref(0);

// Sorting
const sortField = ref<string>('createdAt');
const sortDirection = ref<'asc' | 'desc'>('desc');

// Computed properties
const safeEmailBody = useSafeHtml(selectedEmail.value?.body || '');

// Computed
const totalPages = computed(() => Math.ceil(totalEmails.value / pageSize.value));

const paginatedEmails = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value;
  const end = start + pageSize.value;
  return emails.value.slice(start, end);
});

// Methods
const loadEmails = async () => {
  loading.value = true;
  try {
    const response = await emailMonitoringService.getEmailMessages(
      {
        status: filters.value.status || undefined,
        search: filters.value.search || undefined,
        skip: (currentPage.value - 1) * pageSize.value,
        take: pageSize.value,
      },
      selectedTenant.value || undefined
    );
    emails.value = response.data;
    totalEmails.value = response.total || response.data.length;
  } catch (error) {
    console.error('Failed to load emails:', error);
  } finally {
    loading.value = false;
  }
};

const viewEmailDetails = (email: EmailMessage) => {
  selectedEmail.value = email;
};

const closeModal = () => {
  selectedEmail.value = null;
};

const retryEmail = async (email: EmailMessage) => {
  retryingEmails.value.add(email.id);
  try {
    await emailMonitoringService.retryEmailMessage(email.id, selectedTenant.value || undefined);
    await loadEmails(); // Refresh the list
  } catch (error) {
    console.error('Failed to retry email:', error);
  } finally {
    retryingEmails.value.delete(email.id);
  }
};

const cancelEmail = async (email: EmailMessage) => {
  cancellingEmails.value.add(email.id);
  try {
    await emailMonitoringService.cancelEmailMessage(email.id, selectedTenant.value || undefined);
    await loadEmails(); // Refresh the list
  } catch (error) {
    console.error('Failed to cancel email:', error);
  } finally {
    cancellingEmails.value.delete(email.id);
  }
};

const sortBy = (field: string) => {
  if (sortField.value === field) {
    sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc';
  } else {
    sortField.value = field;
    sortDirection.value = 'asc';
  }
  // Note: In a real implementation, you'd sort on the server side
  // For now, we'll sort client-side
  sortEmails();
};

const sortEmails = () => {
  emails.value.sort((a, b) => {
    const aRecord = a as unknown as Record<string, unknown>;
    const bRecord = b as unknown as Record<string, unknown>;
    let aVal = aRecord[sortField.value];
    let bVal = bRecord[sortField.value];

    if (sortField.value === 'createdAt' && aVal && bVal) {
      aVal = new Date(aVal).getTime();
      bVal = new Date(bVal).getTime();
    }

    if (aVal < bVal) return sortDirection.value === 'asc' ? -1 : 1;
    if (aVal > bVal) return sortDirection.value === 'asc' ? 1 : -1;
    return 0;
  });
};

const getSortIcon = (field: string) => {
  if (sortField.value !== field) return 'fas fa-sort';
  return sortDirection.value === 'asc' ? 'fas fa-sort-up' : 'fas fa-sort-down';
};

const getRowClass = (email: EmailMessage) => {
  return {
    'failed-row': email.status === 'Failed',
    'processing-row': email.status === 'Processing',
    'sent-row': email.status === 'Sent',
  };
};

const getStatusClass = (status: string) => {
  return `status-badge status-${status.toLowerCase()}`;
};

const canCancel = (email: EmailMessage) => {
  return ['Queued', 'Scheduled', 'Failed'].includes(email.status);
};

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleString();
};

const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page;
    loadEmails();
  }
};

// Debounced search
let searchTimeout: NodeJS.Timeout | null = null;
const debouncedSearch = () => {
  if (searchTimeout) clearTimeout(searchTimeout);
  searchTimeout = setTimeout(() => {
    currentPage.value = 1;
    loadEmails();
  }, 300);
};

// Watchers
watch(
  () => filters.value.status,
  () => {
    currentPage.value = 1;
    loadEmails();
  }
);

// Lifecycle
onMounted(() => {
  loadEmails();
});
</script>

<style scoped>
.email-messages-page {
  padding: 2rem;
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  margin-bottom: 2rem;
}

.page-header h1 {
  color: #1f2937;
  margin-bottom: 0.5rem;
}

.page-header p {
  color: #6b7280;
}

.filters-section {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  margin-bottom: 2rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.filter-controls {
  display: flex;
  gap: 1.5rem;
  align-items: end;
  flex-wrap: wrap;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.filter-group label {
  font-weight: 500;
  color: #374151;
  font-size: 0.875rem;
}

.filter-select,
.search-input {
  padding: 0.5rem 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  font-size: 0.875rem;
}

.filter-select {
  min-width: 150px;
}

.search-input {
  min-width: 250px;
}

.refresh-btn {
  background: #3b82f6;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
}

.refresh-btn:hover {
  background: #2563eb;
}

.messages-table-container {
  background: white;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.messages-table {
  width: 100%;
  border-collapse: collapse;
}

.messages-table th,
.messages-table td {
  padding: 0.75rem 1rem;
  text-align: left;
  border-bottom: 1px solid #e5e7eb;
}

.messages-table th {
  background: #f9fafb;
  font-weight: 600;
  color: #374151;
  position: sticky;
  top: 0;
}

.sortable {
  cursor: pointer;
  user-select: none;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.sortable:hover {
  background: #f3f4f6;
}

.subject-cell {
  max-width: 300px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.status-badge {
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  font-size: 0.75rem;
  font-weight: 500;
  text-transform: uppercase;
}

.status-queued {
  background: #fef3c7;
  color: #92400e;
}

.status-processing {
  background: #dbeafe;
  color: #1e40af;
}

.status-sent {
  background: #d1fae5;
  color: #065f46;
}

.status-failed {
  background: #fee2e2;
  color: #991b1b;
}

.status-cancelled {
  background: #f3f4f6;
  color: #374151;
}

.status-scheduled {
  background: #e0e7ff;
  color: #3730a3;
}

.actions-cell {
  display: flex;
  gap: 0.5rem;
}

.action-btn {
  background: none;
  border: 1px solid #d1d5db;
  padding: 0.375rem;
  border-radius: 4px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  transition: all 0.2s;
}

.action-btn:hover {
  border-color: #9ca3af;
}

.view-btn:hover {
  background: #eff6ff;
  border-color: #3b82f6;
  color: #3b82f6;
}

.retry-btn:hover {
  background: #fef3c7;
  border-color: #f59e0b;
  color: #f59e0b;
}

.cancel-btn:hover {
  background: #fee2e2;
  border-color: #ef4444;
  color: #ef4444;
}

.action-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.failed-row {
  background: #fef2f2;
}

.processing-row {
  background: #eff6ff;
}

.sent-row {
  background: #f0fdf4;
}

.no-data {
  text-align: center;
  padding: 4rem 2rem;
  color: #6b7280;
}

.no-data i {
  font-size: 3rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  margin-top: 2rem;
}

.page-btn {
  background: white;
  border: 1px solid #d1d5db;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.page-btn:hover:not(:disabled) {
  background: #f9fafb;
  border-color: #9ca3af;
}

.page-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.page-info {
  color: #6b7280;
  font-size: 0.875rem;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 8px;
  max-width: 800px;
  max-height: 80vh;
  width: 90%;
  overflow: hidden;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1);
}

.modal-header {
  padding: 1.5rem;
  border-bottom: 1px solid #e5e7eb;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header h2 {
  margin: 0;
  color: #1f2937;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.25rem;
  cursor: pointer;
  color: #6b7280;
  padding: 0.25rem;
  border-radius: 4px;
}

.close-btn:hover {
  background: #f3f4f6;
  color: #374151;
}

.modal-body {
  padding: 1.5rem;
  overflow-y: auto;
  max-height: calc(80vh - 120px);
}

.detail-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.detail-item {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.detail-item.full-width {
  grid-column: 1 / -1;
}

.detail-item label {
  font-weight: 600;
  color: #374151;
  font-size: 0.875rem;
}

.detail-item span {
  color: #6b7280;
  word-break: break-word;
}

.error-text {
  color: #dc2626;
  font-family: monospace;
  background: #fef2f2;
  padding: 0.5rem;
  border-radius: 4px;
  border: 1px solid #fecaca;
}

.email-body {
  margin-top: 2rem;
}

.email-body label {
  display: block;
  font-weight: 600;
  color: #374151;
  margin-bottom: 0.5rem;
}

.body-content {
  border: 1px solid #e5e7eb;
  border-radius: 4px;
  padding: 1rem;
  background: #f9fafb;
  max-height: 300px;
  overflow-y: auto;
}

@media (max-width: 768px) {
  .email-messages-page {
    padding: 1rem;
  }

  .filter-controls {
    flex-direction: column;
    align-items: stretch;
  }

  .filter-group {
    width: 100%;
  }

  .filter-select,
  .search-input {
    width: 100%;
  }

  .messages-table {
    font-size: 0.875rem;
  }

  .messages-table th,
  .messages-table td {
    padding: 0.5rem;
  }

  .subject-cell {
    max-width: 150px;
  }

  .actions-cell {
    flex-direction: column;
    gap: 0.25rem;
  }

  .detail-grid {
    grid-template-columns: 1fr;
  }
}
</style>
