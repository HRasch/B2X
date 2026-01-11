<template>
  <div class="email-templates-page">
    <PageHeader :title="$t('email.templates.title')" :subtitle="$t('email.templates.subtitle')">
      <template #actions>
        <button @click="createTemplate" class="btn-primary">
          <svg class="icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M12 4v16m8-8H4"
            ></path>
          </svg>
          {{ $t('email.templates.create') }}
        </button>
      </template>
    </PageHeader>

    <CardContainer>
      <!-- Filters and Search -->
      <div class="filters-section">
        <div class="search-box">
          <input
            v-model="searchQuery"
            type="text"
            :placeholder="$t('email.templates.search')"
            class="search-input"
          />
          <svg class="search-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"
            ></path>
          </svg>
        </div>

        <div class="filter-controls">
          <select v-model="selectedLocale" class="filter-select">
            <option value="">{{ $t('email.templates.allLocales') }}</option>
            <option value="en">{{ $t('email.templates.english') }}</option>
            <option value="de">{{ $t('email.templates.german') }}</option>
            <option value="fr">{{ $t('email.templates.french') }}</option>
          </select>

          <select v-model="selectedStatus" class="filter-select">
            <option value="">{{ $t('ui.filter') }}</option>
            <option value="active">{{ $t('email.templates.active') }}</option>
            <option value="draft">{{ $t('ui.edit') }}</option>
            <option value="archived">{{ $t('ui.view') }}</option>
          </select>
        </div>
      </div>

      <!-- Templates Grid -->
      <div class="templates-grid">
        <div
          v-for="template in filteredTemplates"
          :key="template.id"
          class="template-card"
          @click="editTemplate(template)"
        >
          <div class="card-header">
            <div class="template-info">
              <h3>{{ template.name }}</h3>
              <span class="template-key">{{ template.templateKey }}</span>
            </div>
            <div class="card-actions">
              <span :class="['status-badge', template.status]">
                {{ $t(`email.templates.${template.status}`) }}
              </span>
              <button
                @click.stop="duplicateTemplate(template)"
                class="action-btn"
                :title="$t('ui.edit')"
              >
                <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z"
                  ></path>
                </svg>
              </button>
              <button
                @click.stop="deleteTemplate(template)"
                class="action-btn delete"
                :title="$t('ui.delete')"
              >
                <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
                  ></path>
                </svg>
              </button>
            </div>
          </div>

          <div class="card-content">
            <p class="description">
              {{ template.description || $t('email.templates.noTemplates') }}
            </p>
            <div class="template-meta">
              <span class="locale">{{ template.locale.toUpperCase() }}</span>
              <span class="updated"
                >{{ $t('email.templates.updated') }} {{ formatDate(template.updatedAt) }}</span
              >
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div v-if="filteredTemplates.length === 0" class="empty-state">
          <svg class="empty-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M3 8l7.89 4.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
            ></path>
          </svg>
          <h3>{{ $t('email.templates.noTemplates') }}</h3>
          <p>
            {{
              searchQuery || selectedLocale || selectedStatus
                ? $t('ui.filter')
                : $t('email.templates.create')
            }}
          </p>
          <button @click="createTemplate" class="btn-primary">
            {{ $t('email.templates.create') }}
          </button>
        </div>
      </div>
    </CardContainer>

    <!-- Pagination -->
    <div v-if="totalPages > 1" class="pagination">
      <button @click="currentPage--" :disabled="currentPage === 1" class="page-btn">
        {{ $t('ui.previous') }}
      </button>

      <span class="page-info">
        {{ $t('ui.view') }} {{ currentPage }} {{ $t('ui.next').toLowerCase() }} {{ totalPages }}
      </span>

      <button @click="currentPage++" :disabled="currentPage === totalPages" class="page-btn">
        {{ $t('ui.next') }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { useEmailStore } from '@/stores/email';
import type { EmailTemplate } from '@/types/email';

const router = useRouter();
const { t } = useI18n();
const emailStore = useEmailStore();

// Reactive data
const templates = ref<EmailTemplate[]>([]);
const searchQuery = ref('');
const selectedLocale = ref('');
const selectedStatus = ref('');
const currentPage = ref(1);
const pageSize = ref(12);

// Computed
const filteredTemplates = computed(() => {
  let filtered = templates.value;

  // Search filter
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    filtered = filtered.filter(
      template =>
        template.name.toLowerCase().includes(query) ||
        template.templateKey.toLowerCase().includes(query) ||
        template.description?.toLowerCase().includes(query)
    );
  }

  // Locale filter
  if (selectedLocale.value) {
    filtered = filtered.filter(template => template.locale === selectedLocale.value);
  }

  // Status filter
  if (selectedStatus.value) {
    filtered = filtered.filter(template => template.status === selectedStatus.value);
  }

  return filtered;
});

const totalPages = computed(() => {
  return Math.ceil(filteredTemplates.value.length / pageSize.value);
});

// Methods
const loadTemplates = async () => {
  try {
    templates.value = await emailStore.fetchTemplates();
  } catch (error) {
    console.error('Failed to load templates:', error);
    // Handle error
  }
};

const createTemplate = () => {
  router.push('/email/templates/create');
};

const editTemplate = (template: EmailTemplate) => {
  router.push(`/email/templates/${template.id}/edit`);
};

const duplicateTemplate = async (template: EmailTemplate) => {
  try {
    await emailStore.duplicateTemplate(template.id);
    await loadTemplates(); // Refresh list
  } catch (error) {
    console.error('Failed to duplicate template:', error);
  }
};

const deleteTemplate = async (template: EmailTemplate) => {
  if (!confirm(`${t('email.templates.deleteConfirm')} "${template.name}"?`)) {
    return;
  }

  try {
    await emailStore.deleteTemplate(template.id);
    await loadTemplates(); // Refresh list
  } catch (error) {
    console.error('Failed to delete template:', error);
  }
};

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString();
};

// Lifecycle
onMounted(() => {
  loadTemplates();
});
</script>

<style scoped>
.email-templates-page {
  padding: 2rem;
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 2rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid #e5e7eb;
}

.header-content h1 {
  margin: 0 0 0.5rem 0;
  font-size: 2rem;
  font-weight: 700;
  color: #111827;
}

.header-content p {
  margin: 0;
  color: #6b7280;
}

.header-actions {
  display: flex;
  gap: 1rem;
}

.btn-primary {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: #3b82f6;
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 0.5rem;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.2s;
}

.btn-primary:hover {
  background: #2563eb;
}

.icon {
  width: 1rem;
  height: 1rem;
}

.filters-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  gap: 1rem;
}

.search-box {
  position: relative;
  flex: 1;
  max-width: 400px;
}

.search-input {
  width: 100%;
  padding: 0.75rem 1rem 0.75rem 3rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  font-size: 0.875rem;
}

.search-icon {
  position: absolute;
  left: 0.75rem;
  top: 50%;
  transform: translateY(-50%);
  width: 1rem;
  height: 1rem;
  color: #6b7280;
}

.filter-controls {
  display: flex;
  gap: 1rem;
}

.filter-select {
  padding: 0.75rem 1rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  font-size: 0.875rem;
  background: white;
}

.templates-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.template-card {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 0.75rem;
  padding: 1.5rem;
  cursor: pointer;
  transition: all 0.2s;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.template-card:hover {
  border-color: #3b82f6;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 1rem;
}

.template-info h3 {
  margin: 0 0 0.25rem 0;
  font-size: 1.125rem;
  font-weight: 600;
  color: #111827;
}

.template-key {
  font-size: 0.875rem;
  color: #6b7280;
  font-family: 'Monaco', 'Menlo', monospace;
}

.card-actions {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.status-badge {
  padding: 0.25rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
}

.status-badge.active {
  background: #dcfce7;
  color: #166534;
}

.status-badge.draft {
  background: #fef3c7;
  color: #92400e;
}

.status-badge.archived {
  background: #f3f4f6;
  color: #374151;
}

.action-btn {
  background: none;
  border: none;
  padding: 0.5rem;
  border-radius: 0.375rem;
  cursor: pointer;
  color: #6b7280;
  transition: all 0.2s;
}

.action-btn:hover {
  background: #f3f4f6;
  color: #374151;
}

.action-btn.delete:hover {
  background: #fee2e2;
  color: #dc2626;
}

.card-content {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.description {
  margin: 0;
  color: #4b5563;
  line-height: 1.5;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.template-meta {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 0.875rem;
  color: #6b7280;
}

.locale {
  font-weight: 600;
  color: #3b82f6;
}

.empty-state {
  grid-column: 1 / -1;
  text-align: center;
  padding: 4rem 2rem;
  color: #6b7280;
}

.empty-icon {
  width: 4rem;
  height: 4rem;
  margin: 0 auto 1rem;
  color: #d1d5db;
}

.empty-state h3 {
  margin: 0 0 0.5rem 0;
  font-size: 1.25rem;
  color: #374151;
}

.empty-state p {
  margin: 0 0 2rem 0;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
}

.page-btn {
  padding: 0.5rem 1rem;
  border: 1px solid #d1d5db;
  background: white;
  border-radius: 0.375rem;
  cursor: pointer;
  font-weight: 500;
}

.page-btn:hover:not(:disabled) {
  background: #f9fafb;
}

.page-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.page-info {
  font-weight: 500;
  color: #374151;
}

/* Dark Mode */
html.dark .email-templates-page {
  background: #0f172a;
}

html.dark .page-header {
  background: #1e293b;
  border-bottom-color: #334155;
}

html.dark .header-content h1,
html.dark .header-content p {
  color: #f1f5f9;
}

html.dark .filters-section {
  background: #1e293b;
  border-color: #334155;
}

html.dark .search-input {
  background: #334155;
  border-color: #475569;
  color: #f1f5f9;
}

html.dark .search-input::placeholder {
  color: #94a3b8;
}

html.dark .filter-select {
  background: #334155;
  border-color: #475569;
  color: #f1f5f9;
}

html.dark .template-card {
  background: #1e293b;
  border-color: #334155;
}

html.dark .template-card:hover {
  border-color: #3b82f6;
  background: #334155;
}

html.dark .template-info h3 {
  color: #f1f5f9;
}

html.dark .template-key {
  color: #94a3b8;
}

html.dark .description {
  color: #cbd5e1;
}

html.dark .template-meta {
  color: #94a3b8;
}

html.dark .action-btn {
  color: #94a3b8;
}

html.dark .action-btn:hover {
  background: #334155;
  color: #f1f5f9;
}

html.dark .action-btn.delete:hover {
  background: #7f1d1d;
  color: #fca5a5;
}

html.dark .empty-state h3,
html.dark .empty-state p {
  color: #cbd5e1;
}

html.dark .page-btn {
  background: #334155;
  border-color: #475569;
  color: #f1f5f9;
}

html.dark .page-btn:hover:not(:disabled) {
  background: #475569;
}

html.dark .page-info {
  color: #f1f5f9;
}
</style>
