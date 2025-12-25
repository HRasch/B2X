<template>
  <div class="catalog-brands">
    <!-- Header -->
    <div class="header">
      <div class="title-section">
        <h1>Brands</h1>
        <p class="subtitle">Manage product brands</p>
      </div>
      <div class="actions">
        <button @click="goToCreate" class="btn btn-primary">
          <span class="icon">+</span> New Brand
        </button>
      </div>
    </div>

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
      Loading brands...
    </div>

    <!-- Brands Grid -->
    <div v-else class="brands-grid">
      <div v-if="brands.length === 0" class="empty-state">
        <p>No brands found. Create your first brand!</p>
      </div>
      <div v-else class="grid-items">
        <div v-for="brand in brands" :key="brand.id" class="brand-card">
          <div class="brand-image">
            <img
              v-if="brand.logoUrl"
              :src="brand.logoUrl"
              :alt="getLocalizedName(brand.name)"
            />
            <div v-else class="placeholder">
              <span>{{
                getLocalizedName(brand.name).substring(0, 2).toUpperCase()
              }}</span>
            </div>
          </div>
          <div class="brand-info">
            <h3>{{ getLocalizedName(brand.name) }}</h3>
            <p v-if="brand.description" class="description">
              {{ getLocalizedName(brand.description) }}
            </p>
            <div class="brand-meta">
              <span v-if="brand.websiteUrl" class="meta-item">
                <a :href="brand.websiteUrl" target="_blank">Website</a>
              </span>
              <span :class="['status', brand.isActive ? 'active' : 'inactive']">
                {{ brand.isActive ? "Active" : "Inactive" }}
              </span>
            </div>
          </div>
          <div class="brand-actions">
            <button
              @click="goToEdit(brand.id)"
              class="btn btn-sm btn-secondary"
            >
              Edit
            </button>
            <button
              @click="confirmDelete(brand.id)"
              class="btn btn-sm btn-danger"
            >
              Delete
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Pagination -->
    <div v-if="!loading && brandsTotalitres > 0" class="pagination">
      <button
        @click="previousPage"
        :disabled="brandsPagination.skip === 0"
        class="btn btn-sm"
      >
        Previous
      </button>
      <span class="page-info">
        Page {{ currentPage }} of {{ totalPages }} ({{
          brandsTotalitres
        }}
        total)
      </span>
      <button @click="nextPage" :disabled="!hasMoreBrands" class="btn btn-sm">
        Next
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, computed } from "vue";
import { useRouter } from "vue-router";
import { useCatalogStore } from "@/stores/catalog";

const router = useRouter();
const catalogStore = useCatalogStore();

// Computed Properties
const brands = computed(() => catalogStore.brands);
const loading = computed(() => catalogStore.loading);
const error = computed(() => catalogStore.error);
const successMessage = computed(() => catalogStore.successMessage);
const brandsTotalitres = computed(() => catalogStore.brandsTotalitres);
const brandsPagination = computed(() => catalogStore.brandsPagination);
const hasMoreBrands = computed(() => catalogStore.hasMoreBrands);

const currentPage = computed(
  () =>
    Math.floor(brandsPagination.value.skip / brandsPagination.value.take) + 1
);
const totalPages = computed(() =>
  Math.ceil(brandsTotalitres.value / brandsPagination.value.take)
);

// Methods
function getLocalizedName(localized: any): string {
  if (!localized || !localized.localizedStrings) return "N/A";
  const english = localized.localizedStrings.find(
    (s: any) => s.languageCode === "en-US"
  );
  return english
    ? english.value
    : localized.localizedStrings[0]?.value || "N/A";
}

function goToCreate() {
  router.push("/catalog/brands/create");
}

function goToEdit(id: string) {
  router.push(`/catalog/brands/${id}/edit`);
}

function confirmDelete(id: string) {
  const brand = brands.value.find((b: any) => b.id === id);
  if (
    brand &&
    confirm(
      `Are you sure you want to delete "${getLocalizedName(brand.name)}"?`
    )
  ) {
    catalogStore.deleteBrand(id);
  }
}

function previousPage() {
  const newSkip = Math.max(
    0,
    brandsPagination.value.skip - brandsPagination.value.take
  );
  catalogStore.setBrandsPagination(newSkip, brandsPagination.value.take);
  catalogStore.fetchBrands();
}

function nextPage() {
  if (hasMoreBrands.value) {
    const newSkip = brandsPagination.value.skip + brandsPagination.value.take;
    catalogStore.setBrandsPagination(newSkip, brandsPagination.value.take);
    catalogStore.fetchBrands();
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
  await catalogStore.fetchBrands();
});
</script>

<style scoped lang="css">
.catalog-brands {
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
}

.subtitle {
  margin: 0.5rem 0 0 0;
  color: #6b7280;
  font-size: 0.9rem;
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

.brands-grid {
  background: white;
  border-radius: 0.375rem;
  border: 1px solid #d1d5db;
  padding: 1.5rem;
}

.empty-state {
  text-align: center;
  padding: 3rem;
  color: #6b7280;
}

.grid-items {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}

.brand-card {
  border: 1px solid #e5e7eb;
  border-radius: 0.375rem;
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  transition: all 0.2s;
}

.brand-card:hover {
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  border-color: #d1d5db;
}

.brand-image {
  width: 100%;
  height: 150px;
  background-color: #f9fafb;
  border-radius: 0.375rem;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 1rem;
  overflow: hidden;
}

.brand-image img {
  max-width: 100%;
  max-height: 100%;
  object-fit: contain;
}

.placeholder {
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2rem;
  font-weight: bold;
}

.brand-info {
  flex: 1;
  margin-bottom: 1rem;
}

.brand-info h3 {
  margin: 0 0 0.5rem 0;
  font-size: 1.1rem;
  color: #1f2937;
}

.description {
  margin: 0 0 1rem 0;
  color: #6b7280;
  font-size: 0.875rem;
  line-height: 1.4;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.brand-meta {
  display: flex;
  gap: 0.75rem;
  align-items: center;
  flex-wrap: wrap;
}

.meta-item {
  font-size: 0.75rem;
}

.meta-item a {
  color: #3b82f6;
  text-decoration: none;
  transition: color 0.2s;
}

.meta-item a:hover {
  color: #2563eb;
  text-decoration: underline;
}

.status {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  font-weight: 500;
}

.status.active {
  background-color: #d1fae5;
  color: #065f46;
}

.status.inactive {
  background-color: #f3f4f6;
  color: #6b7280;
}

.brand-actions {
  display: flex;
  gap: 0.5rem;
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
  flex: 1;
}

.btn-secondary:hover {
  background-color: #4b5563;
}

.btn-danger {
  background-color: #ef4444;
  color: white;
  flex: 1;
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
}

@media (max-width: 768px) {
  .header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }

  .grid-items {
    grid-template-columns: 1fr;
  }

  .brand-actions {
    width: 100%;
  }

  .brand-actions .btn {
    flex: 1;
  }
}
</style>
