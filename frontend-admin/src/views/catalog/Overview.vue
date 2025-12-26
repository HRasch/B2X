<template>
  <div class="catalog-overview">
    <!-- Header -->
    <div class="header">
      <div>
        <h1>Catalog Management</h1>
        <p class="subtitle">Manage products, categories, and brands</p>
      </div>
    </div>

    <!-- Statistics Cards -->
    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon products-icon">📦</div>
        <div class="stat-content">
          <div class="stat-value">{{ productsCount }}</div>
          <div class="stat-label">Total Products</div>
          <router-link to="/catalog/products" class="stat-link">
            View all →
          </router-link>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon categories-icon">📂</div>
        <div class="stat-content">
          <div class="stat-value">{{ categoriesCount }}</div>
          <div class="stat-label">Total Categories</div>
          <router-link to="/catalog/categories" class="stat-link">
            View all →
          </router-link>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon brands-icon">🏢</div>
        <div class="stat-content">
          <div class="stat-value">{{ brandsCount }}</div>
          <div class="stat-label">Total Brands</div>
          <router-link to="/catalog/brands" class="stat-link">
            View all →
          </router-link>
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="quick-actions">
      <h2>Quick Actions</h2>
      <div class="actions-grid">
        <router-link to="/catalog/products/create" class="action-card">
          <div class="action-icon">➕</div>
          <div class="action-title">Create Product</div>
          <div class="action-description">Add a new product to the catalog</div>
        </router-link>

        <router-link to="/catalog/categories/create" class="action-card">
          <div class="action-icon">➕</div>
          <div class="action-title">Create Category</div>
          <div class="action-description">Add a new product category</div>
        </router-link>

        <router-link to="/catalog/brands/create" class="action-card">
          <div class="action-icon">➕</div>
          <div class="action-title">Create Brand</div>
          <div class="action-description">Add a new product brand</div>
        </router-link>
      </div>
    </div>

    <!-- Management Sections -->
    <div class="management-sections">
      <h2>Manage</h2>
      <div class="sections-grid">
        <router-link to="/catalog/products" class="section-card">
          <div class="section-title">Products</div>
          <div class="section-description">View and manage all products</div>
          <div class="section-action">Manage Products →</div>
        </router-link>

        <router-link to="/catalog/categories" class="section-card">
          <div class="section-title">Categories</div>
          <div class="section-description">
            Organize products with categories
          </div>
          <div class="section-action">Manage Categories →</div>
        </router-link>

        <router-link to="/catalog/brands" class="section-card">
          <div class="section-title">Brands</div>
          <div class="section-description">Manage product brands</div>
          <div class="section-action">Manage Brands →</div>
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, computed } from "vue";
import { useCatalogStore } from "@/stores/catalog";

const catalogStore = useCatalogStore();

// Computed Properties
const productsCount = computed(() => catalogStore.productsTotal);
const categoriesCount = computed(() => catalogStore.categoriesTotal);
const brandsCount = computed(() => catalogStore.brandsTotalitres);

// Lifecycle
onMounted(async () => {
  // Fetch counts
  const productsResponse = await catalogStore.fetchProducts({ take: 1 });
  const categoriesResponse = await catalogStore.fetchCategories({ take: 1 });
  const brandsResponse = await catalogStore.fetchBrands({ take: 1 });
});
</script>

<style scoped lang="css">
.catalog-overview {
  padding: 2rem;
  @media (prefers-color-scheme: dark) {
    background: #1a202c;
    color: #e2e8f0;
  }
}

.header {
  margin-bottom: 3rem;
}

.header h1 {
  margin: 0 0 0.5rem 0;
  font-size: 2.5rem;
  color: #1f2937;
}

.subtitle {
  margin: 0;
  color: #6b7280;
  font-size: 1rem;
}

/* Statistics Cards */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.5rem;
  margin-bottom: 3rem;
}

.stat-card {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 0.5rem;
  padding: 1.5rem;
  display: flex;
  gap: 1.5rem;
  transition: all 0.3s;
  @media (prefers-color-scheme: dark) {
    background: #2c3e50;
    border-color: #4b5563;
  }
}

.stat-card:hover {
  border-color: #d1d5db;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  @media (prefers-color-scheme: dark) {
    border-color: #4b5563;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.3);
  }
}

.stat-icon {
  font-size: 2.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 60px;
  height: 60px;
  border-radius: 0.375rem;
  flex-shrink: 0;
}

.products-icon {
  background-color: #dbeafe;
}

.categories-icon {
  background-color: #fef3c7;
}

.brands-icon {
  background-color: #ddd6fe;
}

.stat-content {
  flex: 1;
}

.stat-value {
  font-size: 2rem;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 0.25rem;
  @media (prefers-color-scheme: dark) {
    color: #f3f4f6;
  }
}

.stat-label {
  font-size: 0.875rem;
  color: #6b7280;
  margin-bottom: 0.75rem;
  @media (prefers-color-scheme: dark) {
    color: #a6adb8;
  }
}

.stat-link {
  color: #3b82f6;
  text-decoration: none;
  font-size: 0.875rem;
  font-weight: 500;
  transition: color 0.2s;
}

.stat-link:hover {
  color: #2563eb;
  text-decoration: underline;
}

/* Quick Actions */
.quick-actions {
  margin-bottom: 3rem;
}

.quick-actions h2,
.management-sections h2 {
  margin: 0 0 1.5rem 0;
  font-size: 1.5rem;
  color: #1f2937;
  @media (prefers-color-scheme: dark) {
    color: #f3f4f6;
  }
}

.actions-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
  gap: 1rem;
}

.action-card {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 0.5rem;
  padding: 1.5rem;
  text-decoration: none;
  color: inherit;
  transition: all 0.3s;
  @media (prefers-color-scheme: dark) {
    background: #2c3e50;
    border-color: #4b5563;
    color: #d1d5db;
  }
}

.action-card:hover {
  border-color: #3b82f6;
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.15);
  transform: translateY(-2px);
  @media (prefers-color-scheme: dark) {
    box-shadow: 0 4px 12px rgba(59, 130, 246, 0.3);
  }
}

.action-icon {
  font-size: 2rem;
  margin-bottom: 1rem;
}

.action-title {
  font-size: 1rem;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 0.5rem;
  @media (prefers-color-scheme: dark) {
    color: #f3f4f6;
  }
}

.action-description {
  font-size: 0.875rem;
  color: #6b7280;
  @media (prefers-color-scheme: dark) {
    color: #a6adb8;
  }
}

/* Management Sections */
.management-sections {
  margin-top: 3rem;
}

.sections-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
}

.section-card {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 0.5rem;
  padding: 2rem;
  text-decoration: none;
  color: white;
  transition: all 0.3s;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  min-height: 180px;
}

.section-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 10px 25px rgba(102, 126, 234, 0.4);
}

.section-title {
  font-size: 1.5rem;
  font-weight: 700;
  margin-bottom: 0.5rem;
}

.section-description {
  font-size: 0.95rem;
  opacity: 0.9;
  margin-bottom: auto;
}

.section-action {
  font-size: 0.875rem;
  font-weight: 600;
  margin-top: 1rem;
  opacity: 0.8;
  transition: opacity 0.2s;
}

.section-card:hover .section-action {
  opacity: 1;
}

/* Responsive */
@media (max-width: 768px) {
  .catalog-overview {
    padding: 1rem;
  }

  .header h1 {
    font-size: 1.875rem;
  }

  .stats-grid,
  .actions-grid,
  .sections-grid {
    grid-template-columns: 1fr;
  }

  .stat-card {
    flex-direction: column;
    align-items: center;
    text-align: center;
  }

  .stat-icon {
    width: 50px;
    height: 50px;
    font-size: 2rem;
  }
}
</style>
