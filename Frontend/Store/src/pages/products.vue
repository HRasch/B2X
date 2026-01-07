<template>
  <div class="products-page">
    <!-- Search & Sort Bar -->
    <div class="flex flex-col md:flex-row gap-4 mb-8">
      <!-- Search Input -->
      <div class="flex-1">
        <div class="form-control">
          <label class="label">
            <span class="label-text">{{ $t('products.search.label') }}</span>
          </label>
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search by name, SKU, or description..."
            class="input input-bordered w-full"
            :disabled="loading"
          />
        </div>
      </div>

      <!-- Sort Dropdown -->
      <div class="w-full md:w-48">
        <div class="form-control">
          <label class="label">
            <span class="label-text">{{ $t('products.sort.label') }}</span>
          </label>
          <select v-model="sortBy" class="select select-bordered" :disabled="loading">
            <option value="name">{{ $t('products.sort.nameAsc') }}</option>
            <option value="price-asc">{{ $t('products.sort.priceAsc') }}</option>
            <option value="price-desc">{{ $t('products.sort.priceDesc') }}</option>
            <option value="rating">{{ $t('products.sort.ratingDesc') }}</option>
          </select>
        </div>
      </div>
    </div>

    <div class="flex flex-col lg:flex-row gap-8">
      <!-- Sidebar Filters -->
      <aside class="lg:w-48 flex-shrink-0">
        <div class="card bg-base-200 shadow-sm">
          <div class="card-body">
            <h3 class="card-title text-base mb-4">{{ $t('products.filters.title') }}</h3>

            <!-- Category Filter -->
            <div class="form-control">
              <label class="label">
                <span class="label-text font-semibold">{{ $t('products.filters.category') }}</span>
              </label>
              <div class="space-y-2">
                <label v-for="cat in uniqueCategories" :key="cat" class="label cursor-pointer">
                  <input
                    type="radio"
                    :value="cat"
                    v-model="selectedCategory"
                    class="radio radio-primary"
                  />
                  <span class="label-text">{{ cat }}</span>
                </label>
              </div>
            </div>

            <!-- Price Range (TODO: Implement) -->
            <div class="form-control mt-4">
              <label class="label">
                <span class="label-text font-semibold">{{
                  $t('products.filters.priceRange')
                }}</span>
              </label>
              <div class="flex gap-2 items-center">
                <input type="range" min="0" max="5000" class="range range-sm flex-1" disabled />
              </div>
            </div>
          </div>
        </div>
      </aside>

      <!-- Products Grid -->
      <main class="flex-1">
        <!-- Loading State -->
        <div v-if="loading" class="flex justify-center py-12">
          <div class="loading loading-spinner loading-lg"></div>
        </div>

        <!-- Error State -->
        <div v-else-if="error" class="alert alert-error mb-8">
          <span>{{ error }}</span>
        </div>

        <!-- Products -->
        <div v-else>
          <!-- Results Info -->
          <div class="flex justify-between items-center mb-6">
            <p class="text-sm text-gray-600">
              {{
                $t('products.results.showing', {
                  count: filteredProducts.length,
                  total: totalProducts,
                })
              }}
            </p>
            <div class="flex items-center gap-2">
              <span class="text-sm">{{ $t('products.results.perPage') }}</span>
              <select v-model="itemsPerPage" class="select select-bordered select-sm">
                <option :value="12">12</option>
                <option :value="24">24</option>
                <option :value="48">48</option>
              </select>
            </div>
          </div>

          <!-- Products Grid -->
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 mb-8">
            <ProductCardModern
              v-for="product in filteredProducts"
              :key="product.id"
              :product="product"
              @add-to-cart="addToCart"
            />
          </div>

          <!-- Pagination -->
          <div v-if="totalPages > 1" class="flex justify-center">
            <div class="join">
              <button
                class="join-item btn"
                :disabled="!hasPreviousPage"
                @click="goToPage(currentPage - 1)"
              >
                «
              </button>
              <button
                v-for="page in visiblePages"
                :key="page"
                class="join-item btn"
                :class="{ 'btn-active': page === currentPage }"
                @click="goToPage(page)"
              >
                {{ page }}
              </button>
              <button
                class="join-item btn"
                :disabled="!hasNextPage"
                @click="goToPage(currentPage + 1)"
              >
                »
              </button>
            </div>
          </div>

          <!-- Empty State -->
          <div v-if="filteredProducts.length === 0" class="text-center py-12">
            <div class="text-6xl mb-4">🔍</div>
            <h3 class="text-xl font-semibold mb-2">{{ $t('products.noResults') }}</h3>
            <p class="text-gray-600">{{ $t('products.noResultsDescription') }}</p>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'unified-store',
  title: 'Products',
});

import { ref, computed, onMounted } from 'vue';
import { useCartStore } from '@/stores/cart';
import ProductCardModern from '@/components/shop/ProductCardModern.vue';

interface Product {
  id: string;
  name: string;
  sku?: string;
  description: string;
  price: number;
  b2bPrice?: number;
  image: string;
  category: string;
  rating: number;
  inStock: boolean;
  stockQuantity?: number;
}

const cartStore = useCartStore();

// State
const products = ref<Product[]>([]);
const loading = ref(false);
const error = ref<string | null>(null);

// Filters
const searchQuery = ref('');
const selectedCategory = ref('All');
const sortBy = ref('name'); // name, price-asc, price-desc, rating
const itemsPerPage = ref(12);
const currentPage = ref(1);

// Computed
const uniqueCategories = computed(() => {
  const cats = new Set(products.value.map(p => p.category));
  return ['All', ...Array.from(cats).sort()];
});

const sortedAndFiltered = computed(() => {
  let result = products.value;

  // Filter by search
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    result = result.filter(
      p =>
        p.name.toLowerCase().includes(query) ||
        p.description.toLowerCase().includes(query) ||
        p.sku?.toLowerCase().includes(query)
    );
  }

  // Filter by category
  if (selectedCategory.value !== 'All') {
    result = result.filter(p => p.category === selectedCategory.value);
  }

  // Sort
  switch (sortBy.value) {
    case 'price-asc':
      result = [...result].sort((a, b) => a.price - b.price);
      break;
    case 'price-desc':
      result = [...result].sort((a, b) => b.price - a.price);
      break;
    case 'rating':
      result = [...result].sort((a, b) => b.rating - a.rating);
      break;
    case 'name':
    default:
      result = [...result].sort((a, b) => a.name.localeCompare(b.name));
  }

  return result;
});

const totalProducts = computed(() => sortedAndFiltered.value.length);
const totalPages = computed(() => Math.ceil(totalProducts.value / itemsPerPage.value));

const filteredProducts = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value;
  return sortedAndFiltered.value.slice(start, start + itemsPerPage.value);
});

const hasPreviousPage = computed(() => currentPage.value > 1);
const hasNextPage = computed(() => currentPage.value < totalPages.value);

const visiblePages = computed(() => {
  const pages = [];
  const start = Math.max(1, currentPage.value - 2);
  const end = Math.min(totalPages.value, currentPage.value + 2);

  for (let i = start; i <= end; i++) {
    pages.push(i);
  }

  return pages;
});

// Methods
const loadProducts = async () => {
  loading.value = true;
  error.value = null;

  try {
    // TODO: Replace with actual API call
    // const response = await productService.getProducts()
    // products.value = response

    // Mock data for now
    products.value = [
      {
        id: '1',
        name: 'Laptop Pro 15"',
        sku: 'LP-001',
        description: 'High-performance laptop for professionals',
        price: 1299.99,
        image: 'https://via.placeholder.com/300x200?text=Laptop',
        category: 'Electronics',
        rating: 4.8,
        inStock: true,
        stockQuantity: 5,
      },
      {
        id: '2',
        name: 'USB-C Cable',
        sku: 'AC-001',
        description: 'Durable USB-C to USB-C cable',
        price: 19.99,
        image: 'https://via.placeholder.com/300x200?text=USB-C',
        category: 'Accessories',
        rating: 4.5,
        inStock: true,
        stockQuantity: 50,
      },
      // Add more mock products...
    ];
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to load products';
  } finally {
    loading.value = false;
  }
};

const addToCart = (product: Product) => {
  cartStore.addItem({
    id: product.id,
    name: product.name,
    price: product.price,
    image: product.image,
    quantity: 1,
  });

  // Show toast notification
  // TODO: Implement toast
  console.log(`Added ${product.name} to cart`);
};

const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page;
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
};

// Lifecycle
onMounted(() => {
  loadProducts();
});
</script>

<style scoped>
.products-page {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}
</style>
