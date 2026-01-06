<template>
  <div class="categories-page">
    <div class="container mx-auto px-4 py-8">
      <!-- Breadcrumbs -->
      <nav class="breadcrumbs mb-6">
        <NuxtLink to="/" class="breadcrumb-link">{{ $t('categories.home') }}</NuxtLink>
        <span class="breadcrumb-separator">/</span>
        <span class="breadcrumb-current">{{ $t('categories.allCategories') }}</span>
      </nav>

      <!-- Page Header -->
      <div class="page-header mb-8">
        <h1 class="text-3xl font-bold mb-4">{{ $t('categories.allCategories') }}</h1>
        <p class="text-gray-600">{{ $t('categories.browseByCategory') }}</p>
      </div>

      <!-- Categories Grid -->
      <div v-if="categoriesStore.loading" class="flex justify-center py-12">
        <div class="loading loading-spinner loading-lg"></div>
      </div>

      <div v-else-if="categoriesStore.error" class="alert alert-error mb-8">
        <span>{{ categoriesStore.error }}</span>
      </div>

      <div
        v-else
        class="categories-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6"
      >
        <div
          v-for="category in categoriesStore.categories"
          :key="category.id"
          class="category-card card bg-white shadow-md hover:shadow-lg transition-shadow cursor-pointer"
          @click="navigateToCategory(category)"
        >
          <figure class="category-image">
            <img
              :src="category.image || '/images/category-placeholder.jpg'"
              :alt="category.name"
              class="w-full h-48 object-cover"
            />
          </figure>
          <div class="card-body p-4">
            <h3 class="card-title text-lg font-semibold mb-2">{{ category.name }}</h3>
            <p v-if="category.description" class="text-sm text-gray-600 mb-3">
              {{ category.description }}
            </p>
            <div class="flex justify-between items-center">
              <span class="text-sm text-gray-500">
                {{ $t('categories.productsCount', { count: category.productCount || 0 }) }}
              </span>
              <div class="card-actions">
                <button class="btn btn-primary btn-sm">
                  {{ $t('categories.browse') }}
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div
        v-if="!categoriesStore.loading && categoriesStore.categories.length === 0"
        class="text-center py-12"
      >
        <div class="text-6xl mb-4">📂</div>
        <h3 class="text-xl font-semibold mb-2">{{ $t('categories.noCategories') }}</h3>
        <p class="text-gray-600">{{ $t('categories.noCategoriesDescription') }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue';
import { useCategoriesStore, type Category } from '~/stores/categories';

// Composables
const categoriesStore = useCategoriesStore();

// Methods
const navigateToCategory = (category: Category) => {
  navigateTo(`/categories/${category.slug}`);
};

// Lifecycle
onMounted(async () => {
  await categoriesStore.fetchCategories();
});

// SEO
useHead({
  title: 'Categories',
  meta: [{ name: 'description', content: 'Browse our product categories' }],
});
</script>

<style scoped>
.categories-page {
  min-height: 60vh;
}

.breadcrumbs {
  font-size: 0.875rem;
}

.breadcrumb-link {
  color: #6b7280;
  text-decoration: none;
}

.breadcrumb-link:hover {
  color: #374151;
  text-decoration: underline;
}

.breadcrumb-separator {
  color: #d1d5db;
  margin: 0 0.5rem;
}

.breadcrumb-current {
  color: #374151;
  font-weight: 500;
}

.category-card {
  transition: all 0.2s ease;
}

.category-card:hover {
  transform: translateY(-2px);
}

.category-image {
  margin: 0;
  overflow: hidden;
}

.category-image img {
  transition: transform 0.3s ease;
}

.category-card:hover .category-image img {
  transform: scale(1.05);
}
</style>
