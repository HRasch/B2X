<template>
  <div class="categories-page">
    <!-- Breadcrumbs -->
    <nav v-if="currentCategory" class="breadcrumbs mb-6" aria-label="Breadcrumb">
      <ol class="flex items-center space-x-2 text-sm">
        <li>
          <NuxtLink to="/categories" class="text-gray-500 hover:text-gray-700">
            {{ $t('categories.allCategories') }}
          </NuxtLink>
        </li>
        <li v-for="(crumb, index) in breadcrumbs" :key="crumb.id" class="flex items-center">
          <span class="mx-2 text-gray-400">/</span>
          <NuxtLink
            v-if="index < breadcrumbs.length - 1"
            :to="`/categories/${crumb.slug}`"
            class="text-gray-500 hover:text-gray-700"
          >
            {{ crumb.name }}
          </NuxtLink>
          <span v-else class="text-gray-900 font-medium">
            {{ crumb.name }}
          </span>
        </li>
      </ol>
    </nav>

    <!-- Page Header -->
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 mb-2">
        {{ currentCategory ? currentCategory.name : $t('categories.title') }}
      </h1>
      <p v-if="currentCategory?.description" class="text-gray-600">
        {{ currentCategory.description }}
      </p>
      <p v-else class="text-gray-600">
        {{ $t('categories.subtitle') }}
      </p>
    </div>

    <!-- Loading State -->
    <div v-if="categoriesStore.loading" class="flex justify-center py-12">
      <div class="loading loading-spinner loading-lg"></div>
    </div>

    <!-- Error State -->
    <div v-else-if="categoriesStore.error" class="alert alert-error mb-8">
      <span>{{ categoriesStore.error }}</span>
      <button
        class="btn btn-sm btn-outline"
        @click="retryLoad"
      >
        {{ $t('common.retry') }}
      </button>
    </div>

    <!-- Categories Grid -->
    <div v-else>
      <!-- Subcategories -->
      <div v-if="subcategories.length > 0" class="mb-12">
        <h2 class="text-2xl font-semibold mb-6">{{ $t('categories.subcategories') }}</h2>
        <div class="categories-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          <div
            v-for="category in subcategories"
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
              <p v-if="category.description" class="text-sm text-gray-600 mb-3 line-clamp-2">
                {{ category.description }}
              </p>
              <div class="flex justify-between items-center">
                <span class="text-sm text-gray-500">
                  {{ $t('categories.productsCount', { count: category.productCount || 0 }) }}
                </span>
                <div class="card-actions">
                  <button class="btn btn-primary btn-sm">
                    {{ $t('categories.viewProducts') }}
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Products in Category -->
      <div v-if="currentCategory && categoryProducts.length > 0" class="mb-12">
        <h2 class="text-2xl font-semibold mb-6">
          {{ $t('categories.productsInCategory', { category: currentCategory.name }) }}
        </h2>
        <div class="products-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          <!-- Product cards would go here -->
          <div class="text-center py-8 text-gray-500">
            {{ $t('categories.productsComingSoon') }}
          </div>
        </div>
      </div>

      <!-- Root Categories -->
      <div v-if="!currentCategory">
        <div class="categories-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          <div
            v-for="category in rootCategories"
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
              <p v-if="category.description" class="text-sm text-gray-600 mb-3 line-clamp-2">
                {{ category.description }}
              </p>
              <div class="flex justify-between items-center">
                <span class="text-sm text-gray-500">
                  {{ $t('categories.productsCount', { count: category.productCount || 0 }) }}
                </span>
                <div class="card-actions">
                  <button class="btn btn-primary btn-sm">
                    {{ $t('categories.viewProducts') }}
                  </button>
                </div>
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
          class="text-center py-12"
        >
          <div class="text-6xl mb-4">📂</div>
          <h3 class="text-xl font-semibold mb-2">{{ $t('categories.noCategories') }}</h3>
          <p class="text-gray-600">{{ $t('categories.noCategoriesDescription') }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue';
import { useCategoriesStore, type Category } from '~/stores/categories';

definePageMeta({
  layout: 'unified-store',
  title: 'Categories',
});

// Route params
const route = useRoute();
const router = useRouter();

// Composables
const categoriesStore = useCategoriesStore();

// Computed properties
const currentCategorySlug = computed(() => route.params.slug as string);

const currentCategory = computed(() => {
  if (!currentCategorySlug.value) return null;
  return categoriesStore.getCategoryBySlug.value(currentCategorySlug.value);
});

const breadcrumbs = computed(() => {
  if (!currentCategory.value) return [];
  return categoriesStore.getCategoryHierarchy.value(currentCategory.value.id);
});

const rootCategories = computed(() => categoriesStore.rootCategories);

const subcategories = computed(() => {
  if (!currentCategory.value) return [];
  return categoriesStore.categories.filter(cat => cat.parentId === currentCategory.value!.id);
});

const categoryProducts = computed(() => {
  // TODO: Implement product fetching for category
  return [];
});

// Methods
const navigateToCategory = async (category: Category) => {
  await navigateTo(`/categories/${category.slug}`);
};

const retryLoad = async () => {
  try {
    await categoriesStore.fetchCategories();
  } catch (error) {
    console.error('Failed to retry loading categories:', error);
  }
};

// Lifecycle
onMounted(async () => {
  try {
    await categoriesStore.fetchCategories();
  } catch (error) {
    console.error('Failed to load categories:', error);
  }
});

// Watch for route changes
watch(currentCategorySlug, async (newSlug) => {
  if (newSlug && !currentCategory.value) {
    // Category not found, redirect to categories index
    await navigateTo('/categories');
  }
});
</script>

<style scoped>
.categories-page {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}

.category-card {
  transition: all 0.3s ease;
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
