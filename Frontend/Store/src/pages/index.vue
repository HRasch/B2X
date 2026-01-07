<template>
  <div class="home-page">
    <!-- Hero Section -->
    <section class="hero bg-gradient-to-r from-blue-600 to-purple-700 text-white py-20">
      <div class="container mx-auto px-4 text-center">
        <h1 class="text-5xl font-bold mb-6">
          {{ $t('home.hero.title') }}
        </h1>
        <p class="text-xl mb-8 max-w-2xl mx-auto">
          {{ $t('home.hero.subtitle') }}
        </p>
        <div class="flex flex-col sm:flex-row gap-4 justify-center">
          <NuxtLink to="/products" class="btn btn-primary btn-lg">
            {{ $t('home.hero.shopNow') }}
          </NuxtLink>
          <NuxtLink to="/categories" class="btn btn-outline btn-lg">
            {{ $t('home.hero.browseCategories') }}
          </NuxtLink>
        </div>
      </div>
    </section>

    <!-- Featured Products -->
    <section class="py-16 bg-gray-50">
      <div class="container mx-auto px-4">
        <h2 class="text-3xl font-bold text-center mb-12">
          {{ $t('home.featuredProducts.title') }}
        </h2>

        <div v-if="productsStore.loading" class="flex justify-center">
          <div class="loading loading-spinner loading-lg"></div>
        </div>

        <div v-else-if="productsStore.error" class="alert alert-error">
          {{ productsStore.error }}
        </div>

        <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <ProductCard
            v-for="product in productsStore.featuredProducts"
            :key="product.id"
            :product="product"
            class="card bg-white shadow-lg hover:shadow-xl transition-shadow"
          />
        </div>

        <div class="text-center mt-8">
          <NuxtLink to="/products" class="btn btn-outline btn-lg">
            {{ $t('home.featuredProducts.viewAll') }}
          </NuxtLink>
        </div>
      </div>
    </section>

    <!-- Categories Overview -->
    <section class="py-16">
      <div class="container mx-auto px-4">
        <h2 class="text-3xl font-bold text-center mb-12">
          {{ $t('home.categories.title') }}
        </h2>

        <div class="grid grid-cols-2 md:grid-cols-4 gap-6">
          <NuxtLink
            v-for="category in categories"
            :key="category.id"
            :to="`/categories/${category.slug}`"
            class="card bg-white shadow hover:shadow-lg transition-shadow text-center p-6"
          >
            <div
              class="w-16 h-16 mx-auto mb-4 bg-blue-100 rounded-full flex items-center justify-center"
            >
              <svg
                class="w-8 h-8 text-blue-600"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"
                ></path>
              </svg>
            </div>
            <h3 class="font-semibold text-lg">{{ category.name }}</h3>
            <p class="text-gray-600 text-sm">
              {{ category.productCount }} {{ $t('common.products') }}
            </p>
          </NuxtLink>
        </div>
      </div>
    </section>

    <!-- Features Section -->
    <section class="py-16 bg-gray-50">
      <div class="container mx-auto px-4">
        <h2 class="text-3xl font-bold text-center mb-12">
          {{ $t('home.features.title') }}
        </h2>

        <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
          <div class="text-center">
            <div
              class="w-16 h-16 mx-auto mb-4 bg-green-100 rounded-full flex items-center justify-center"
            >
              <svg
                class="w-8 h-8 text-green-600"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
                ></path>
              </svg>
            </div>
            <h3 class="font-semibold text-xl mb-2">{{ $t('home.features.quality.title') }}</h3>
            <p class="text-gray-600">{{ $t('home.features.quality.description') }}</p>
          </div>

          <div class="text-center">
            <div
              class="w-16 h-16 mx-auto mb-4 bg-blue-100 rounded-full flex items-center justify-center"
            >
              <svg
                class="w-8 h-8 text-blue-600"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M13 10V3L4 14h7v7l9-11h-7z"
                ></path>
              </svg>
            </div>
            <h3 class="font-semibold text-xl mb-2">{{ $t('home.features.fast.title') }}</h3>
            <p class="text-gray-600">{{ $t('home.features.fast.description') }}</p>
          </div>

          <div class="text-center">
            <div
              class="w-16 h-16 mx-auto mb-4 bg-purple-100 rounded-full flex items-center justify-center"
            >
              <svg
                class="w-8 h-8 text-purple-600"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z"
                ></path>
              </svg>
            </div>
            <h3 class="font-semibold text-xl mb-2">{{ $t('home.features.support.title') }}</h3>
            <p class="text-gray-600">{{ $t('home.features.support.description') }}</p>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'unified-store',
  title: 'Home',
});

import { onMounted } from 'vue';
import { useProductsStore } from '~/stores/products';
import ProductCard from '~/components/shop/ProductCard.vue';

const productsStore = useProductsStore();

// Mock categories - in real app, load from API
const categories = [
  { id: '1', name: 'Electronics', slug: 'electronics', productCount: 150 },
  { id: '2', name: 'Clothing', slug: 'clothing', productCount: 89 },
  { id: '3', name: 'Home & Garden', slug: 'home-garden', productCount: 67 },
  { id: '4', name: 'Sports', slug: 'sports', productCount: 43 },
];

onMounted(async () => {
  try {
    await productsStore.loadFeaturedProducts();
  } catch (error) {
    console.error('Failed to load featured products:', error);
  }
});
</script>

<style scoped>
.hero {
  background-image:
    linear-gradient(rgba(0, 0, 0, 0.3), rgba(0, 0, 0, 0.3)), url('/images/hero-bg.jpg');
  background-size: cover;
  background-position: center;
}

.subtitle {
  font-size: 1.2rem;
  color: #666;
  margin-bottom: 2rem;
}

.hero {
  margin: 2rem 0;
}

.btn {
  display: inline-block;
  padding: 0.75rem 1.5rem;
  border-radius: 4px;
  text-decoration: none;
  transition: all 0.3s;
  border: none;
  cursor: pointer;
  font-size: 1rem;
}

.btn-primary {
  background-color: #007bff;
  color: white;
}

.btn-primary:hover {
  background-color: #0056b3;
}

.btn-large {
  padding: 1rem 2rem;
  font-size: 1.1rem;
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
  margin-top: 3rem;
}

.feature {
  padding: 2rem;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  background-color: #f9f9f9;
}

.feature h3 {
  color: #333;
  margin-bottom: 0.5rem;
}

.feature p {
  color: #666;
  line-height: 1.6;
}
</style>
