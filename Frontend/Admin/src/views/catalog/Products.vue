<template>
  <div class="catalog-products">
    <!-- Header -->
    <div class="header">
      <div class="title-section">
        <h1>Products</h1>
        <p class="subtitle">Manage your catalog products</p>
      </div>
      <div class="actions">
        <button @click="goToCreate" class="btn btn-primary">
          <span class="icon">+</span> New Product
        </button>
      </div>
    </div>

    <!-- Filters -->
    <div class="filters">
      <input
        v-model="searchQuery"
        type="text"
        placeholder="Search products..."
        @input="applyFilters"
        class="search-input"
      />
      <select v-model="selectedCategory" @change="applyFilters" class="select">
        <option value="">All Categories</option>
        <option v-for="cat in categories" :key="cat.id" :value="cat.id">
          {{ getLocalizedName(cat.name) }}
        </option>
      </select>
      <select v-model="selectedBrand" @change="applyFilters" class="select">
        <option value="">All Brands</option>
        <option v-for="brand in brands" :key="brand.id" :value="brand.id">
          {{ getLocalizedName(brand.name) }}
        </option>
      </select>
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
      Loading products...
    </div>

    <!-- Products Table -->
    <div v-else class="table-container">
      <table class="table">
        <thead>
          <tr>
            <th>SKU</th>
            <th>Name</th>
            <th>Category</th>
            <th>Brand</th>
            <th>Price</th>
            <th>Stock</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="product in products" :key="product.id">
            <td class="sku">{{ product.sku }}</td>
            <td class="name">{{ getLocalizedName(product.name) }}</td>
            <td>{{ getCategoryName(product.categoryId) }}</td>
            <td>{{ product.brandId ? getBrandName(product.brandId) : "-" }}</td>
            <td class="price">
              {{ formatPrice(product.basePrice, product.currency) }}
            </td>
            <td class="stock">
              <span
                :class="[
                  'stock-badge',
                  product.stock > 0 ? 'in-stock' : 'out-of-stock',
                ]"
              >
                {{ product.stock }}
              </span>
            </td>
            <td>
              <span
                :class="[
                  'status-badge',
                  product.isActive ? 'active' : 'inactive',
                ]"
              >
                {{ product.isActive ? "Active" : "Inactive" }}
              </span>
            </td>
            <td class="actions">
              <button
                @click="goToEdit(product.id)"
                class="btn btn-sm btn-secondary"
              >
                Edit
              </button>
              <button
                @click="confirmDelete(product.id, product.sku)"
                class="btn btn-sm btn-danger"
              >
                Delete
              </button>
            </td>
          </tr>
        </tbody>
      </table>

      <!-- Empty State -->
      <div v-if="products.length === 0" class="empty-state">
        <p>No products found</p>
      </div>
    </div>

    <!-- Pagination -->
    <div v-if="!loading && productsTotal > 0" class="pagination">
      <button
        @click="previousPage"
        :disabled="productsPagination.skip === 0"
        class="btn btn-sm"
      >
        Previous
      </button>
      <span class="page-info">
        Page {{ currentPage }} of {{ totalPages }} ({{ productsTotal }} total)
      </span>
      <button @click="nextPage" :disabled="!hasMoreProducts" class="btn btn-sm">
        Next
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, computed } from "vue";
import { useRouter } from "vue-router";
import { useCatalogStore } from "@/stores/catalog";
import type { ProductFilters } from "@/types/catalog";

const router = useRouter();
const catalogStore = useCatalogStore();

// State
const searchQuery = ref("");
const selectedCategory = ref("");
const selectedBrand = ref("");

// Computed Properties
const products = computed(() => catalogStore.products);
const categories = computed(() => catalogStore.categories);
const brands = computed(() => catalogStore.brands);
const loading = computed(() => catalogStore.loading);
const error = computed(() => catalogStore.error);
const successMessage = computed(() => catalogStore.successMessage);
const productsTotal = computed(() => catalogStore.productsTotal);
const productsPagination = computed(() => catalogStore.productsPagination);
const hasMoreProducts = computed(() => catalogStore.hasMoreProducts);

const currentPage = computed(
  () =>
    Math.floor(productsPagination.value.skip / productsPagination.value.take) +
    1
);
const totalPages = computed(() =>
  Math.ceil(productsTotal.value / productsPagination.value.take)
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

function getCategoryName(categoryId: string): string {
  const category = catalogStore.categoryMap.get(categoryId);
  return category ? getLocalizedName(category.name) : "Unknown";
}

function getBrandName(brandId: string): string {
  const brand = catalogStore.brandMap.get(brandId);
  return brand ? getLocalizedName(brand.name) : "Unknown";
}

function formatPrice(price: number, currency: string): string {
  return `${price.toFixed(2)} ${currency}`;
}

function applyFilters() {
  const filters: ProductFilters = {
    search: searchQuery.value || undefined,
    categoryId: selectedCategory.value || undefined,
    brandId: selectedBrand.value || undefined,
    skip: 0,
    take: productsPagination.value.take,
  };
  catalogStore.fetchProducts(filters);
}

function goToCreate() {
  router.push("/catalog/products/create");
}

function goToEdit(id: string) {
  router.push(`/catalog/products/${id}/edit`);
}

function confirmDelete(id: string, sku: string) {
  if (confirm(`Are you sure you want to delete product "${sku}"?`)) {
    catalogStore.deleteProduct(id);
  }
}

function previousPage() {
  const newSkip = Math.max(
    0,
    productsPagination.value.skip - productsPagination.value.take
  );
  catalogStore.setProductsPagination(newSkip, productsPagination.value.take);
  applyFilters();
}

function nextPage() {
  if (hasMoreProducts.value) {
    const newSkip =
      productsPagination.value.skip + productsPagination.value.take;
    catalogStore.setProductsPagination(newSkip, productsPagination.value.take);
    applyFilters();
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
  await catalogStore.fetchBrands();
  await catalogStore.fetchProducts();
});
</script>

<style scoped lang="css">
.catalog-products {
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

.filters {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  flex-wrap: wrap;
}

.search-input,
.select {
  padding: 0.5rem 1rem;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  @media (prefers-color-scheme: dark) {
    background-color: #2c3e50;
    border-color: #4b5563;
    color: #f3f4f6;
  }
}

.search-input {
  flex: 1;
  min-width: 200px;
}

.select {
  min-width: 150px;
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
  @media (prefers-color-scheme: dark) {
    color: #a6adb8;
  }
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

.table-container {
  background: white;
  border-radius: 0.375rem;
  border: 1px solid #d1d5db;
  overflow-x: auto;
  @media (prefers-color-scheme: dark) {
    background: #1f2937;
    border-color: #4b5563;
  }
}

.table {
  width: 100%;
  border-collapse: collapse;
}

.table th {
  background-color: #f9fafb;
  padding: 1rem;
  text-align: left;
  font-weight: 600;
  border-bottom: 1px solid #d1d5db;
  color: #374151;
  font-size: 0.875rem;
  @media (prefers-color-scheme: dark) {
    background-color: #2c3e50;
    border-color: #4b5563;
    color: #d1d5db;
  }
}

.table td {
  padding: 1rem;
  border-bottom: 1px solid #e5e7eb;
  @media (prefers-color-scheme: dark) {
    border-color: #4b5563;
    color: #d1d5db;
  }
}

.table tbody tr:hover {
  background-color: #f9fafb;
  @media (prefers-color-scheme: dark) {
    background-color: #3f5468;
  }
}

.sku {
  font-family: monospace;
  font-size: 0.875rem;
  color: #6b7280;
  @media (prefers-color-scheme: dark) {
    color: #a6adb8;
  }
}

.name {
  font-weight: 500;
  color: #1f2937;
  @media (prefers-color-scheme: dark) {
    color: #f3f4f6;
  }
}

.price {
  font-weight: 500;
  color: #059669;
}

.stock {
  text-align: center;
}

.stock-badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 0.25rem;
  font-size: 0.875rem;
  font-weight: 500;
}

.stock-badge.in-stock {
  background-color: #d1fae5;
  color: #065f46;
}

.stock-badge.out-of-stock {
  background-color: #fee2e2;
  color: #991b1b;
}

.status-badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  border-radius: 0.25rem;
  font-size: 0.875rem;
  font-weight: 500;
}

.status-badge.active {
  background-color: #d1fae5;
  color: #065f46;
}

.status-badge.inactive {
  background-color: #f3f4f6;
  color: #6b7280;
}

.actions {
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

.empty-state {
  padding: 3rem;
  text-align: center;
  color: #6b7280;
  @media (prefers-color-scheme: dark) {
    color: #a6adb8;
  }
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

.btn.icon {
  margin-right: 0.5rem;
}

@media (max-width: 768px) {
  .header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }

  .filters {
    flex-direction: column;
  }

  .search-input,
  .select {
    width: 100%;
  }

  .table-container {
    font-size: 0.875rem;
  }

  .table th,
  .table td {
    padding: 0.75rem;
  }
}
</style>
