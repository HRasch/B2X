<template>
  <div class="store-container">
    <!-- Header with Cart -->
    <div class="store-header">
      <h1>{{ $t('store.title') }}</h1>
      <div class="header-actions">
        <div class="search-bar">
          <input
            v-model="searchQuery"
            type="text"
            :placeholder="$t('store.searchPlaceholder')"
            @input="filterProducts"
            :disabled="loading"
          />
          <div v-if="queryExecutionTime > 0" class="search-time">
            {{ $t('store.searchTime', { time: queryExecutionTime }) }}
          </div>
        </div>
        <router-link to="/cart" class="cart-button">
          🛒 {{ $t('store.cart', { count: cartStore.items.length }) }}
        </router-link>
      </div>
    </div>

    <!-- Category Filter -->
    <div class="category-filter">
      <button
        v-for="cat in categories"
        :key="cat"
        @click="
          () => {
            selectedCategory = cat;
            onCategoryChange();
          }
        "
        :class="['category-btn', { active: selectedCategory === cat }]"
        :disabled="loading"
      >
        {{ cat }}
      </button>
    </div>

    <!-- Product Count & Search Results Info -->
    <div v-if="!loading && searchQuery" class="search-info">
      <p>
        {{ $t('store.productsCount', { count: totalProducts })
        }}{{ searchQuery ? ` ${$t('store.foundFor')} "${searchQuery}"` : '' }}
      </p>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="loading-container">
      <div class="spinner"></div>
      <p>{{ $t('ui.loadingProducts') }}</p>
    </div>

    <!-- Error State -->
    <div v-if="error && !loading" class="error-container">
      <p class="error-message">❌ {{ $t('store.errorLoadingProducts', { error }) }}</p>
      <button @click="loadProducts" class="retry-button">{{ $t('ui.retry') }}</button>
    </div>

    <!-- Products Grid -->
    <div v-if="!loading && !error" class="products-grid">
      <div v-if="filteredProducts.length === 0" class="no-products">
        <p>{{ $t('store.noProductsFound') }}</p>
        <p v-if="searchQuery" class="suggestion">
          {{ $t('store.tryAdjustingFilters') }}
        </p>
      </div>

      <ProductCard
        v-for="product in filteredProducts"
        :key="product.id"
        :product="product"
        @add-to-cart="addToCart"
      />
    </div>

    <!-- Pagination -->
    <div v-if="!loading && filteredProducts.length > 0" class="pagination">
      <button
        @click="goToPage(currentPage - 1)"
        :disabled="!hasPreviousPage"
        class="pagination-btn"
      >
        {{ $t('store.previousPage') }}
      </button>

      <div class="pagination-info">
        {{ $t('store.pageOf', { current: currentPage, total: totalPages }) }}
      </div>

      <button @click="goToPage(currentPage + 1)" :disabled="!hasNextPage" class="pagination-btn">
        {{ $t('store.nextPage') }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useCartStore } from '../stores/cart';
import ProductCard from '../components/shop/ProductCard.vue';
import { ProductService, type Product, type SearchResponse } from '../services/productService';

const cartStore = useCartStore();

// Search state
const products = ref<Product[]>([]);
const loading = ref(false);
const error = ref<string | null>(null);
const searchQuery = ref('');
const selectedCategory = ref('Alle');
const currentPage = ref(1);
const pageSize = ref(20);
const totalPages = ref(1);
const totalProducts = ref(0);
const queryExecutionTime = ref(0);
const categories = ref<string[]>(['Alle', 'Elektronik', 'Zubehör']);

/**
 * Load products from ElasticSearch
 * Called on component mount and when filters change
 */
const loadProducts = async () => {
  loading.value = true;
  error.value = null;

  try {
    // Build filters
    const filters = {
      searchTerm: searchQuery.value.trim() || '*', // ElasticSearch uses * for all documents
      category: selectedCategory.value !== 'Alle' ? selectedCategory.value : undefined,
      language: 'de',
      onlyAvailable: true,
    };

    // Use ElasticSearch for search, or fall back to paginated list
    let response: SearchResponse;
    if (searchQuery.value.trim()) {
      response = await ProductService.searchProducts(filters, currentPage.value, pageSize.value);
    } else {
      response = await ProductService.getProducts(currentPage.value, pageSize.value, filters);
    }

    products.value = response.items;
    currentPage.value = response.page;
    totalPages.value = response.totalPages;
    totalProducts.value = response.totalCount;
    if (response.searchMetadata) {
      queryExecutionTime.value = response.searchMetadata.queryExecutionTimeMs;
    }
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to load products';
    console.error('Product loading error:', err);
    // Fallback to empty list on error
    products.value = [];
  } finally {
    loading.value = false;
  }
};

/**
 * Handle search input with debounce
 * Resets to page 1 when search query changes
 */
let searchTimeout: ReturnType<typeof setTimeout>;
const filterProducts = () => {
  clearTimeout(searchTimeout);
  currentPage.value = 1; // Reset to first page on new search
  searchTimeout = setTimeout(() => {
    loadProducts();
  }, 300); // 300ms debounce
};

/**
 * Handle category filter change
 */
const onCategoryChange = () => {
  currentPage.value = 1; // Reset to first page on category change
  loadProducts();
};

/**
 * Handle pagination
 */
const goToPage = (page: number) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page;
    loadProducts();
  }
};

/**
 * Add product to cart
 */
const addToCart = (product: Product) => {
  cartStore.addItem({
    id: product.id,
    name: product.name,
    price: product.price,
    quantity: 1,
    image: product.image || 'https://via.placeholder.com/100?text=No+Image',
  });
};

/**
 * Load initial products on component mount
 */
onMounted(() => {
  loadProducts();
});

/**
 * Computed property for display
 */
const filteredProducts = computed(() => {
  return products.value;
});

const hasNextPage = computed(() => {
  return currentPage.value < totalPages.value;
});

const hasPreviousPage = computed(() => {
  return currentPage.value > 1;
});
</script>

<style scoped>
.store-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}

.store-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  gap: 2rem;
}

.store-header h1 {
  font-size: 2rem;
  color: #333;
  margin: 0;
  flex-shrink: 0;
}

.header-actions {
  display: flex;
  gap: 1rem;
  flex: 1;
  align-items: center;
}

.search-bar {
  flex: 1;
  min-width: 250px;
  position: relative;
}

.search-bar input {
  width: 100%;
  padding: 0.75rem;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  font-size: 1rem;
  transition: border-color 0.3s;
}

.search-bar input:focus {
  outline: none;
  border-color: #2563eb;
}

.search-bar input:disabled {
  background-color: #f5f5f5;
  cursor: not-allowed;
}

.search-time {
  font-size: 0.75rem;
  color: #999;
  margin-top: 0.25rem;
  text-align: right;
}

.cart-button {
  padding: 0.75rem 1.5rem;
  background-color: #2563eb;
  color: white;
  border: none;
  border-radius: 8px;
  text-decoration: none;
  cursor: pointer;
  font-weight: 600;
  transition: background-color 0.3s;
  white-space: nowrap;
}

.cart-button:hover {
  background-color: #1d4ed8;
}

.category-filter {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  flex-wrap: wrap;
}

.category-btn {
  padding: 0.5rem 1rem;
  border: 2px solid #e0e0e0;
  background-color: white;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s;
  font-weight: 500;
}

.category-btn:hover:not(:disabled) {
  border-color: #2563eb;
  color: #2563eb;
}

.category-btn.active {
  background-color: #2563eb;
  color: white;
  border-color: #2563eb;
}

.category-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.search-info {
  background-color: #f0f4ff;
  padding: 1rem;
  border-radius: 8px;
  margin-bottom: 1.5rem;
  color: #2563eb;
  font-weight: 500;
}

.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  text-align: center;
  color: #666;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #e0e0e0;
  border-top-color: #2563eb;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.error-container {
  background-color: #fee;
  border: 2px solid #f66;
  padding: 2rem;
  border-radius: 8px;
  text-align: center;
  margin-bottom: 2rem;
}

.error-message {
  color: #c33;
  font-weight: 600;
  margin: 0 0 1rem 0;
}

.retry-button {
  padding: 0.5rem 1.5rem;
  background-color: #2563eb;
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  transition: background-color 0.3s;
}

.retry-button:hover {
  background-color: #1d4ed8;
}

.products-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 2rem;
  margin-bottom: 2rem;
}

.no-products {
  grid-column: 1 / -1;
  text-align: center;
  padding: 3rem;
  color: #999;
  font-size: 1.1rem;
}

.suggestion {
  font-size: 0.9rem;
  color: #bbb;
  margin-top: 0.5rem;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 2rem;
  padding: 2rem;
  margin-top: 2rem;
  background-color: #f9f9f9;
  border-radius: 8px;
}

.pagination-info {
  font-weight: 600;
  color: #333;
  min-width: 150px;
  text-align: center;
}

.pagination-btn {
  padding: 0.75rem 1.5rem;
  background-color: #2563eb;
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  transition: background-color 0.3s;
}

.pagination-btn:hover:not(:disabled) {
  background-color: #1d4ed8;
}

.pagination-btn:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

@media (max-width: 768px) {
  .store-header {
    flex-direction: column;
    align-items: stretch;
  }

  .header-actions {
    flex-direction: column;
  }

  .search-bar input {
    min-width: unset;
  }

  .products-grid {
    grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
    gap: 1rem;
  }

  .pagination {
    flex-direction: column;
    gap: 1rem;
  }
}
</style>
