<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { useCategories } from '@/composables/useCategories';
import type { Category } from '@/types/catalog';

const router = useRouter();
const { t } = useI18n();
const { categories, loading, error, loadCategories } = useCategories();

// Local state for breadcrumbs
const breadcrumbs = ref<Array<{ name: string; path?: string }>>([
  { name: t('categories.title'), path: '/categories' },
]);

onMounted(async () => {
  await loadCategories();
});

const navigateToCategory = (category: Category) => {
  router.push(`/categories/${category.slug}`);
};
</script>

<template>
  <div class="min-h-screen bg-base-200">
    <!-- Header -->
    <div class="bg-base-100 shadow-sm">
      <div class="container mx-auto px-4 py-6">
        <nav class="flex items-center space-x-2 text-sm text-base-content/70 mb-4">
          <router-link
            v-for="(crumb, index) in breadcrumbs"
            :key="index"
            :to="crumb.path || '#'"
            class="hover:text-primary transition-colors"
            :class="{ 'text-primary font-medium': index === breadcrumbs.length - 1 }"
          >
            {{ crumb.name }}
            <span v-if="index < breadcrumbs.length - 1" class="ml-2">/</span>
          </router-link>
        </nav>

        <h1 class="text-3xl font-bold text-base-content">
          {{ t('categories.title') }}
        </h1>
        <p class="text-base-content/70 mt-2">
          {{ t('categories.subtitle') }}
        </p>
      </div>
    </div>

    <!-- Content -->
    <div class="container mx-auto px-4 py-8">
      <!-- Loading State -->
      <div v-if="loading" class="flex justify-center items-center py-12">
        <div class="loading loading-spinner loading-lg text-primary"></div>
        <span class="ml-4 text-lg">{{ t('common.loading') }}</span>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="alert alert-error max-w-md mx-auto">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="stroke-current shrink-0 h-6 w-6"
          fill="none"
          viewBox="0 0 24 24"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z"
          />
        </svg>
        <span>{{ error }}</span>
      </div>

      <!-- Categories Grid -->
      <div
        v-else-if="categories.length > 0"
        class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6"
      >
        <div
          v-for="category in categories"
          :key="category.id"
          @click="navigateToCategory(category)"
          class="card bg-base-100 shadow-md hover:shadow-lg transition-all duration-200 cursor-pointer group"
        >
          <div class="card-body p-6">
            <!-- Category Icon/Placeholder -->
            <div
              class="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center mb-4 group-hover:bg-primary/20 transition-colors"
            >
              <svg
                class="w-6 h-6 text-primary"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"
                />
              </svg>
            </div>

            <!-- Category Info -->
            <h3
              class="card-title text-lg font-semibold text-base-content group-hover:text-primary transition-colors"
            >
              {{ category.name }}
            </h3>

            <p class="text-sm text-base-content/60 mt-1">
              {{ t('categories.productCount', { count: category.productCount }) }}
            </p>

            <!-- Action -->
            <div class="card-actions justify-end mt-4">
              <button
                class="btn btn-primary btn-sm opacity-0 group-hover:opacity-100 transition-opacity"
              >
                {{ t('categories.viewProducts') }}
                <svg class="w-4 h-4 ml-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M9 5l7 7-7 7"
                  />
                </svg>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else class="text-center py-12">
        <svg
          class="w-16 h-16 text-base-content/30 mx-auto mb-4"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"
          />
        </svg>
        <h3 class="text-xl font-semibold text-base-content mb-2">
          {{ t('categories.noCategories') }}
        </h3>
        <p class="text-base-content/60">
          {{ t('categories.noCategoriesDescription') }}
        </p>
      </div>
    </div>
  </div>
</template>
