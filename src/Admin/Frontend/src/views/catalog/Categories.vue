<template>
  <div class="catalog-categories">
    <PageHeader
      :title="$t('catalog.categories.title')"
      :subtitle="$t('catalog.categories.subtitle')"
    >
      <template #actions>
        <button @click="goToCreate" class="btn btn-primary">
          <span class="icon">+</span> {{ $t('catalog.categories.actions.new') }}
        </button>
      </template>
    </PageHeader>

    <!-- Alert Messages -->
    <div v-if="error" class="alert alert-error">
      {{ error }}
      <button @click="clearError" class="close">×</button>
    </div>
    <div v-if="successMessage" class="alert alert-success">
      {{ successMessage }}
      <button @click="clearSuccess" class="close">×</button>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      {{ $t('catalog.categories.loading') }}
    </div>

    <div v-else>
      <CardContainer>
        <!-- Categories List -->
        <div class="categories-container">
          <div v-if="rootCategories.length === 0" class="empty-state">
            <p>{{ $t('catalog.categories.no_categories') }}</p>
          </div>
          <div v-else>
            <div v-for="category in rootCategories" :key="category.id" class="category-item">
              <div class="category-header">
                <div class="category-info">
                  <h3>{{ getLocalizedName(category.name) }}</h3>
                  <p class="category-description">
                    {{ getLocalizedName(category.description) }}
                  </p>
                </div>
                <div class="category-actions">
                  <button @click="goToEdit(category.id)" class="btn btn-sm btn-secondary">
                    {{ $t('catalog.categories.actions.edit') }}
                  </button>
                  <button @click="confirmDelete(category.id)" class="btn btn-sm btn-danger">
                    {{ $t('catalog.categories.actions.delete') }}
                  </button>
                </div>
              </div>
              <!-- Subcategories -->
              <div v-if="getCategoryChildren(category.id).length > 0" class="subcategories">
                <div
                  v-for="subcat in getCategoryChildren(category.id)"
                  :key="subcat.id"
                  class="subcategory-item"
                >
                  <div class="subcategory-header">
                    <div class="category-info">
                      <h4>{{ getLocalizedName(subcat.name) }}</h4>
                      <p class="category-description">
                        {{ getLocalizedName(subcat.description) }}
                      </p>
                    </div>
                    <div class="category-actions">
                      <button @click="goToEdit(subcat.id)" class="btn btn-sm btn-secondary">
                        {{ $t('catalog.categories.actions.edit') }}
                      </button>
                      <button @click="confirmDelete(subcat.id)" class="btn btn-sm btn-danger">
                        {{ $t('catalog.categories.actions.delete') }}
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </CardContainer>
    </div>

    <!-- Pagination -->
    <div v-if="!loading && categoriesTotal > 0" class="pagination">
      <button @click="previousPage" :disabled="categoriesPagination.skip === 0" class="btn btn-sm">
        Previous
      </button>
      <span class="page-info">
        Page {{ currentPage }} of {{ totalPages }} ({{ categoriesTotal }} total)
      </span>
      <button @click="nextPage" :disabled="!hasMoreCategories" class="btn btn-sm">Next</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';
import { useCatalogStore } from '@/stores/catalog';
import type { Category, LocalizedContent, LocalizedString } from '@/types/catalog';

const router = useRouter();
const catalogStore = useCatalogStore();
const { t } = useI18n();

// Computed Properties
const categories = computed(() => catalogStore.categories);
const loading = computed(() => catalogStore.loading);
const error = computed(() => catalogStore.error);
const successMessage = computed(() => catalogStore.successMessage);
const categoriesTotal = computed(() => catalogStore.categoriesTotal);
const categoriesPagination = computed(() => catalogStore.categoriesPagination);
const hasMoreCategories = computed(() => catalogStore.hasMoreCategories);

const currentPage = computed(
  () => Math.floor(categoriesPagination.value.skip / categoriesPagination.value.take) + 1
);
const totalPages = computed(() =>
  Math.ceil(categoriesTotal.value / categoriesPagination.value.take)
);

// Root categories (no parent)
const rootCategories = computed(() => {
  return categories.value.filter((cat: Category) => !cat.parentCategoryId);
});

// Methods
function getLocalizedName(localized: LocalizedContent | undefined | null): string {
  if (!localized || !localized.localizedStrings) return 'N/A';
  const english = localized.localizedStrings.find(
    (s: LocalizedString) => s.languageCode === 'en-US'
  );
  return english ? english.value : localized.localizedStrings[0]?.value || 'N/A';
}

function getCategoryChildren(parentId: string): Category[] {
  return categories.value.filter((cat: Category) => cat.parentCategoryId === parentId);
}

function goToCreate() {
  router.push('/catalog/categories/create');
}

function goToEdit(id: string) {
  router.push(`/catalog/categories/${id}/edit`);
}

function confirmDelete(id: string) {
  const category = categories.value.find((c: Category) => c.id === id);
  if (
    category &&
    confirm(t('catalog.categories.deleteConfirm', { name: getLocalizedName(category.name) }))
  ) {
    catalogStore.deleteCategory(id);
  }
}

function previousPage() {
  const newSkip = Math.max(0, categoriesPagination.value.skip - categoriesPagination.value.take);
  catalogStore.setCategoriesPagination(newSkip, categoriesPagination.value.take);
  catalogStore.fetchCategories();
}

function nextPage() {
  if (hasMoreCategories.value) {
    const newSkip = categoriesPagination.value.skip + categoriesPagination.value.take;
    catalogStore.setCategoriesPagination(newSkip, categoriesPagination.value.take);
    catalogStore.fetchCategories();
  }
}

function clearError() {
  catalogStore.clearError();
}

function clearSuccess() {
  catalogStore.clearSuccess();
}

// Lifecycle
onMounted(async () => {
  await catalogStore.fetchCategories();
});
</script>

<style scoped lang="css">
.catalog-categories {
  padding: 2rem;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.title-section h1 {
  margin: 0;
  font-size: 2rem;
  color: #1f2937;
  @media (prefers-color-scheme: dark) {
    color: #f3f4f6;
  }
}

.subtitle {
  margin: 0.5rem 0 0 0;
  color: #6b7280;
  font-size: 0.9rem;
  @media (prefers-color-scheme: dark) {
    color: #a6adb8;
  }
}

.actions {
  display: flex;
  gap: 1rem;
}

.alert {
  padding: 1rem;
  border-radius: 0.375rem;
  margin-bottom: 1rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.alert-error {
  background-color: #fee2e2;
  color: #991b1b;
  border: 1px solid #fca5a5;
}

.alert-success {
  background-color: #dcfce7;
  color: #166534;
  border: 1px solid #86efac;
}

.alert .close {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: inherit;
}

.loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem;
  color: #6b7280;
}

.spinner {
  width: 2rem;
  height: 2rem;
  border: 3px solid #e5e7eb;
  border-top-color: #3b82f6;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.categories-container {
  background: white;
  border-radius: 0.375rem;
  border: 1px solid #d1d5db;
  padding: 1.5rem;
  @media (prefers-color-scheme: dark) {
    background: #1f2937;
    border-color: #4b5563;
  }
}

.empty-state {
  text-align: center;
  padding: 3rem;
  color: #6b7280;
  @media (prefers-color-scheme: dark) {
    color: #a6adb8;
  }
}

.category-item {
  margin-bottom: 2rem;
  padding-bottom: 1.5rem;
  border-bottom: 1px solid #e5e7eb;
  @media (prefers-color-scheme: dark) {
    border-color: #4b5563;
  }
}

.category-item:last-child {
  border-bottom: none;
  margin-bottom: 0;
  padding-bottom: 0;
}

.category-header {
  display: flex;
  justify-content: space-between;
  align-items: start;
  gap: 1rem;
}

.category-info h3 {
  margin: 0 0 0.25rem 0;
  font-size: 1.1rem;
  color: #1f2937;
  @media (prefers-color-scheme: dark) {
    color: #f3f4f6;
  }
}

.category-description {
  margin: 0;
  color: #6b7280;
  font-size: 0.875rem;
  @media (prefers-color-scheme: dark) {
    color: #a6adb8;
  }
}

.category-actions {
  display: flex;
  gap: 0.5rem;
  flex-shrink: 0;
}

.subcategories {
  margin-top: 1rem;
  padding-left: 2rem;
  border-left: 2px solid #e5e7eb;
  @media (prefers-color-scheme: dark) {
    border-color: #4b5563;
  }
}

.subcategory-item {
  margin-bottom: 1rem;
}

.subcategory-header {
  display: flex;
  justify-content: space-between;
  align-items: start;
  gap: 1rem;
}

.subcategory-header .category-info h4 {
  margin: 0 0 0.25rem 0;
  font-size: 1rem;
  color: #374151;
  @media (prefers-color-scheme: dark) {
    color: #d1d5db;
  }
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-primary {
  background-color: #3b82f6;
  color: white;
}

.btn-primary:hover {
  background-color: #2563eb;
}

.btn-secondary {
  background-color: #6b7280;
  color: white;
}

.btn-secondary:hover {
  background-color: #4b5563;
}

.btn-danger {
  background-color: #ef4444;
  color: white;
}

.btn-danger:hover {
  background-color: #dc2626;
}

.btn-sm {
  padding: 0.375rem 0.75rem;
  font-size: 0.75rem;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 1rem;
  margin-top: 2rem;
}

.page-info {
  color: #6b7280;
  font-size: 0.875rem;
  @media (prefers-color-scheme: dark) {
    color: #a6adb8;
  }
}

@media (max-width: 768px) {
  .header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }

  .category-header,
  .subcategory-header {
    flex-direction: column;
    align-items: flex-start;
  }

  .category-actions {
    width: 100%;
  }

  .subcategories {
    padding-left: 1rem;
  }
}
</style>
