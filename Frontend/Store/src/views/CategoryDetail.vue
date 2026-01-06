<script setup lang="ts">
import { onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { useCategories } from '@/composables/useCategories';
import { useCartStore } from '@/stores/cart';
import ProductCardModern from '@/components/shop/ProductCardModern.vue';

const route = useRoute();
const router = useRouter();
const { t } = useI18n();
const cartStore = useCartStore();
const { currentCategory, loading, error, loadCategoryWithProducts } = useCategories();

// Breadcrumbs
const breadcrumbs = computed(() => {
  const crumbs = [{ name: t('categories.title'), path: '/categories' }];

  if (currentCategory.value) {
    crumbs.push({
      name: currentCategory.value.name,
      path: `/categories/${currentCategory.value.slug}`,
    });
  }

  return crumbs;
});

onMounted(async () => {
  const slug = route.params.slug as string;
  if (slug) {
    await loadCategoryWithProducts(slug);
  }
});

const goBack = () => {
  router.push('/categories');
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

        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-3xl font-bold text-base-content">
              {{ currentCategory?.name || t('categories.category') }}
            </h1>
            <p v-if="currentCategory" class="text-base-content/70 mt-2">
              {{ t('categories.productCount', { count: currentCategory.productCount }) }}
            </p>
          </div>

          <button @click="goBack" class="btn btn-outline btn-sm">
            <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M15 19l-7-7 7-7"
              />
            </svg>
            {{ t('common.back') }}
          </button>
        </div>
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

      <!-- Category Not Found -->
      <div v-else-if="!currentCategory" class="text-center py-12">
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
            d="M9.172 16.172a4 4 0 015.656 0M9 12h6m-6-4h6m2 5.291A7.962 7.962 0 0112 15c-2.34 0-4.29-.98-5.5-2.5m-.5-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
          />
        </svg>
        <h3 class="text-xl font-semibold text-base-content mb-2">
          {{ t('categories.categoryNotFound') }}
        </h3>
        <p class="text-base-content/60 mb-6">
          {{ t('categories.categoryNotFoundDescription') }}
        </p>
        <router-link to="/categories" class="btn btn-primary">
          {{ t('categories.viewAllCategories') }}
        </router-link>
      </div>

      <!-- Products Grid -->
      <div v-else-if="currentCategory.products.length > 0" class="space-y-6">
        <!-- Subcategories (Future) -->
        <div v-if="currentCategory.children && currentCategory.children.length > 0" class="mb-8">
          <h2 class="text-xl font-semibold text-base-content mb-4">
            {{ t('categories.subcategories') }}
          </h2>
          <!-- Subcategory grid would go here -->
        </div>

        <!-- Products -->
        <div>
          <h2 class="text-xl font-semibold text-base-content mb-6">
            {{ t('categories.productsInCategory', { category: currentCategory.name }) }}
          </h2>

          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            <ProductCardModern
              v-for="product in currentCategory.products"
              :key="product.id"
              :product="product"
              @add-to-cart="cartStore.addItem(product)"
            />
          </div>
        </div>
      </div>

      <!-- No Products -->
      <div v-else-if="currentCategory" class="text-center py-12">
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
            d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2M4 13h2m8-5v2m0 0v2m0-2h2m-2 0h-2"
          />
        </svg>
        <h3 class="text-xl font-semibold text-base-content mb-2">
          {{ t('categories.noProducts') }}
        </h3>
        <p class="text-base-content/60 mb-6">
          {{ t('categories.noProductsDescription') }}
        </p>
        <router-link to="/categories" class="btn btn-primary">
          {{ t('categories.browseOtherCategories') }}
        </router-link>
      </div>
    </div>
  </div>
</template>
