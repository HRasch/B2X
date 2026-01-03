<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useCartStore } from '@/stores/cart';
import ProductCardModern from '@/components/shop/ProductCardModern.vue';
import ProductPrice from '@/components/ProductPrice.vue';

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

const router = useRouter();
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

const categories = ['All', 'Electronics', 'Accessories', 'Software', 'Services'];

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

<template>
  <div class="min-h-screen bg-base-100">
    <!-- Header -->
    <header class="bg-primary text-primary-content py-6 px-4">
      <div class="max-w-7xl mx-auto">
        <h1 class="text-3xl font-bold mb-2">B2Connect Store</h1>
        <p class="text-primary-content/90">Find the best products for your business</p>
      </div>
    </header>

    <div class="max-w-7xl mx-auto px-4 py-8">
      <!-- Search & Sort Bar -->
      <div class="flex flex-col md:flex-row gap-4 mb-8">
        <!-- Search Input -->
        <div class="flex-1">
          <div class="form-control">
            <label class="label">
              <span class="label-text">Search products</span>
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
              <span class="label-text">Sort by</span>
            </label>
            <select v-model="sortBy" class="select select-bordered" :disabled="loading">
              <option value="name">Name (A-Z)</option>
              <option value="price-asc">Price (Low to High)</option>
              <option value="price-desc">Price (High to Low)</option>
              <option value="rating">Rating (High to Low)</option>
            </select>
          </div>
        </div>
      </div>

      <div class="flex flex-col lg:flex-row gap-8">
        <!-- Sidebar Filters -->
        <aside class="lg:w-48 flex-shrink-0">
          <div class="card bg-base-200 shadow-sm">
            <div class="card-body">
              <h3 class="card-title text-base mb-4">Filters</h3>

              <!-- Category Filter -->
              <div class="form-control">
                <label class="label">
                  <span class="label-text font-semibold">Category</span>
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
                  <span class="label-text font-semibold">Price Range</span>
                </label>
                <div class="flex gap-2 items-center">
                  <input type="range" min="0" max="5000" class="range range-sm flex-1" disabled />
                </div>
                <p class="text-xs text-base-content/70 mt-2">€0 - €5000 (coming soon)</p>
              </div>

              <!-- Availability Filter -->
              <div class="form-control mt-4">
                <label class="label cursor-pointer">
                  <span class="label-text">In Stock Only</span>
                  <input type="checkbox" class="checkbox checkbox-primary" disabled />
                </label>
              </div>
            </div>
          </div>
        </aside>

        <!-- Main Content -->
        <main class="flex-1">
          <!-- Results Info -->
          <div class="mb-6 flex justify-between items-center">
            <div>
              <h2 class="text-xl font-semibold">{{ totalProducts }} Products</h2>
              <p v-if="searchQuery" class="text-sm text-base-content/70">
                Found for:
                <span class="font-semibold">"{{ searchQuery }}"</span>
              </p>
            </div>
            <p class="text-sm text-base-content/70">Page {{ currentPage }} of {{ totalPages }}</p>
          </div>

          <!-- Loading State -->
          <div v-if="loading" class="flex justify-center items-center py-12">
            <div class="flex flex-col items-center gap-4">
              <span class="loading loading-spinner loading-lg text-primary"></span>
              <p class="text-base-content/70">Loading products...</p>
            </div>
          </div>

          <!-- Error State -->
          <div v-else-if="error" class="alert alert-error mb-6">
            <div>
              <span>{{ error }}</span>
            </div>
            <button @click="loadProducts" class="btn btn-sm btn-ghost">Retry</button>
          </div>

          <!-- Products Grid -->
          <div
            v-else-if="filteredProducts.length > 0"
            class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 mb-8"
          >
            <ProductCardModern
              v-for="product in filteredProducts"
              :key="product.id"
              :product="product"
              @add-to-cart="addToCart"
            />
          </div>

          <!-- No Products -->
          <div v-else class="text-center py-12">
            <div class="text-6xl mb-4">🔍</div>
            <h3 class="text-xl font-semibold mb-2">No products found</h3>
            <p class="text-base-content/70 mb-4">Try adjusting your filters or search query</p>
            <button
              @click="
                () => {
                  searchQuery = '';
                  selectedCategory = 'All';
                }
              "
              class="btn btn-primary"
            >
              Clear Filters
            </button>
          </div>

          <!-- Pagination -->
          <div
            v-if="!loading && filteredProducts.length > 0"
            class="flex justify-center items-center gap-2"
          >
            <button
              @click="goToPage(currentPage - 1)"
              :disabled="!hasPreviousPage"
              class="btn btn-sm"
            >
              ← Previous
            </button>

            <div class="space-x-1">
              <button
                v-for="page in Math.min(5, totalPages)"
                :key="page"
                @click="goToPage(page)"
                :class="['btn btn-sm', currentPage === page ? 'btn-primary' : 'btn-ghost']"
              >
                {{ page }}
              </button>
              <span v-if="totalPages > 5" class="px-2">...</span>
            </div>

            <button @click="goToPage(currentPage + 1)" :disabled="!hasNextPage" class="btn btn-sm">
              Next →
            </button>
          </div>
        </main>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Custom scroll for products grid */
:deep(.product-grid) {
  scroll-behavior: smooth;
}
</style>
