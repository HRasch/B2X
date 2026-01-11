<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { useCartStore } from '@/stores/cart';
import ProductPrice from '@/components/ProductPrice.vue';

interface Product {
  id: string;
  name: string;
  sku?: string;
  description: string;
  longDescription?: string;
  price: number;
  b2bPrice?: number;
  images: string[];
  category: string;
  rating: number;
  reviewCount?: number;
  inStock: boolean;
  stockQuantity?: number;
  specifications?: Record<string, string>;
  tags?: string[];
}

interface Review {
  id: string;
  author: string;
  rating: number;
  title: string;
  comment: string;
  date: string;
  verified: boolean;
}

const route = useRoute();
const cartStore = useCartStore();

// State
const product = ref<Product | null>(null);
const reviews = ref<Review[]>([]);
const loading = ref(false);
const error = ref<string | null>(null);

const selectedImage = ref(0);
const quantity = ref(1);
const addingToCart = ref(false);

// Computed

const currentImage = computed(() => {
  return product.value?.images?.[selectedImage.value] || '';
});

// Methods
const loadProduct = async () => {
  loading.value = true;
  error.value = null;

  try {
    const productId = route.params.id as string;

    // TODO: Replace with actual API call
    // const response = await productService.getProduct(productId)
    // product.value = response

    // Mock product data
    product.value = {
      id: productId,
      name: 'Premium Laptop Pro 15"',
      sku: 'LP-2024-PRO',
      description: 'High-performance laptop for professionals and creators',
      longDescription: `The Premium Laptop Pro is engineered for professionals who demand 
        the best performance. With the latest processor, ample RAM, and SSD storage, 
        this laptop handles demanding tasks with ease. Perfect for developers, designers, 
        and content creators.`,
      price: 1299.99,
      images: [
        'https://via.placeholder.com/500x400?text=Laptop+Front',
        'https://via.placeholder.com/500x400?text=Laptop+Side',
        'https://via.placeholder.com/500x400?text=Laptop+Open',
      ],
      category: 'Electronics',
      rating: 4.7,
      reviewCount: 48,
      inStock: true,
      stockQuantity: 5,
      specifications: {
        Processor: 'Intel Core i9',
        RAM: '32GB DDR5',
        Storage: '1TB SSD NVMe',
        Display: '15.6" 4K OLED',
        Battery: '12 hours',
        Weight: '1.8 kg',
      },
      tags: ['professional', 'gaming', 'portable', 'business'],
    };

    // Mock reviews
    reviews.value = [
      {
        id: '1',
        author: 'John Developer',
        rating: 5,
        title: 'Excellent for development work',
        comment: 'This laptop is fantastic for programming. Fast compilation, great performance.',
        date: '2024-12-20',
        verified: true,
      },
      {
        id: '2',
        author: 'Sarah Designer',
        rating: 4,
        title: 'Great display, good performance',
        comment:
          'The display is absolutely stunning for design work. Only minor issue is the price.',
        date: '2024-12-15',
        verified: true,
      },
    ];
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to load product';
  } finally {
    loading.value = false;
  }
};

const selectImage = (index: number) => {
  selectedImage.value = index;
};

const incrementQuantity = () => {
  if (product.value && quantity.value < (product.value.stockQuantity || 1)) {
    quantity.value++;
  }
};

const decrementQuantity = () => {
  if (quantity.value > 1) {
    quantity.value--;
  }
};

const addToCart = async () => {
  if (!product.value) return;

  addingToCart.value = true;

  try {
    cartStore.addItem({
      id: product.value.id,
      name: product.value.name,
      price: product.value.price,
      image: product.value.images[0],
      quantity: quantity.value,
    });

    // Show success message
    // TODO: Implement toast notification
    console.log(`Added ${quantity.value} × ${product.value.name} to cart`);

    // Reset quantity
    quantity.value = 1;
  } finally {
    addingToCart.value = false;
  }
};

// Lifecycle
onMounted(() => {
  loadProduct();
});
</script>

<template>
  <div class="min-h-screen bg-base-100">
    <!-- Breadcrumb -->
    <nav class="breadcrumbs text-sm px-4 py-4 max-w-7xl mx-auto">
      <ul>
        <li>
          <router-link to="/">{{ $t('navigation.home') }}</router-link>
        </li>
        <li>
          <router-link to="/products">{{ $t('navigation.products') }}</router-link>
        </li>
        <li v-if="product" class="text-base-content/70">{{ product.name }}</li>
      </ul>
    </nav>

    <!-- Loading -->
    <div v-if="loading" class="flex justify-center items-center py-24">
      <div class="flex flex-col items-center gap-4">
        <span class="loading loading-spinner loading-lg text-primary"></span>
        <p class="text-base-content/70">{{ $t('ui.loadingProductDetails') }}</p>
      </div>
    </div>

    <!-- Error -->
    <div v-else-if="error" class="max-w-7xl mx-auto px-4 py-8">
      <div class="alert alert-error">
        <div>
          <span>{{ error }}</span>
        </div>
        <button @click="loadProduct" class="btn btn-sm btn-ghost">{{ $t('ui.retry') }}</button>
      </div>
    </div>

    <!-- Product Details -->
    <div v-else-if="product" class="max-w-7xl mx-auto px-4 py-8">
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
        <!-- Image Gallery -->
        <div class="flex flex-col gap-4">
          <!-- Main Image -->
          <div class="card bg-base-200 shadow-sm overflow-hidden h-96 lg:h-[500px]">
            <figure class="flex items-center justify-center h-full bg-base-300">
              <img
                :src="currentImage"
                :alt="product.name"
                class="h-full w-full object-cover"
                loading="lazy"
              />
            </figure>
          </div>

          <!-- Thumbnails -->
          <div class="flex gap-2">
            <button
              v-for="(image, index) in product.images"
              :key="index"
              @click="selectImage(index)"
              :class="[
                'card bg-base-200 shadow-sm overflow-hidden w-20 h-20 cursor-pointer transition-all',
                selectedImage === index
                  ? 'ring-2 ring-primary'
                  : 'hover:ring-2 hover:ring-primary/50',
              ]"
            >
              <figure class="flex items-center justify-center h-full">
                <img
                  :src="image"
                  :alt="`${product.name} - Image ${index + 1}`"
                  class="h-full w-full object-cover"
                  loading="lazy"
                />
              </figure>
            </button>
          </div>
        </div>

        <!-- Product Info -->
        <div>
          <!-- Breadcrumb Alt -->
          <div class="text-sm text-base-content/70 mb-2">
            {{ product.category }}
          </div>

          <!-- Title & Rating -->
          <h1 class="text-3xl lg:text-4xl font-bold mb-2">
            {{ product.name }}
          </h1>

          <div class="flex items-center gap-2 mb-4">
            <div class="rating rating-sm gap-1">
              <input
                v-for="i in 5"
                :key="i"
                type="radio"
                :checked="i <= Math.round(product.rating)"
                class="mask mask-star-2 bg-warning"
                disabled
              />
            </div>
            <span class="text-sm font-semibold">{{ product.rating }}</span>
            <span class="text-sm text-base-content/70">({{ product.reviewCount }} reviews)</span>
          </div>

          <!-- SKU -->
          <p v-if="product.sku" class="text-sm text-base-content/70 mb-4">
            {{ $t('product.sku') }} <span class="font-mono">{{ product.sku }}</span>
          </p>

          <!-- Description -->
          <p class="text-base-content/90 mb-6 leading-relaxed">
            {{ product.longDescription }}
          </p>

          <!-- Divider -->
          <div class="divider my-4"></div>

          <!-- Price Section with VAT Transparency (Issue #30) -->
          <div class="card bg-green-50 border-l-4 border-green-500 shadow-sm mb-6">
            <div class="card-body">
              <h3 class="card-title text-base mb-4">{{ $t('product.priceOverview') }}</h3>

              <!-- Use ProductPrice component for automatic VAT calculation -->
              <ProductPrice
                v-if="product"
                :product-price="product.price"
                destination-country="DE"
                :shipping-cost="0"
                show-breakdown
              />

              <p class="text-xs text-gray-500 mt-3">
                {{ $t('product.priceIncludesVat') }}
              </p>
            </div>
          </div>

          <!-- Stock Status -->
          <div
            :class="[
              'mb-6 p-4 rounded-lg',
              product.inStock
                ? 'bg-success/20 text-success border border-success'
                : 'bg-error/20 text-error border border-error',
            ]"
          >
            <span class="font-semibold">
              {{ product.inStock ? '✓ In Stock' : '✗ Out of Stock' }}
            </span>
            <span v-if="product.inStock && product.stockQuantity" class="text-sm ml-2">
              ({{ product.stockQuantity }} available)
            </span>
          </div>

          <!-- Quantity & Add to Cart -->
          <div class="flex gap-4 mb-6">
            <!-- Quantity Selector -->
            <div class="flex items-center border border-base-300 rounded-lg">
              <button
                @click="decrementQuantity"
                :disabled="quantity <= 1 || !product.inStock"
                class="btn btn-ghost btn-sm rounded-none"
              >
                −
              </button>
              <input
                v-model.number="quantity"
                type="number"
                min="1"
                :max="product.stockQuantity || 1"
                class="input input-ghost w-16 text-center no-spinner"
                :disabled="!product.inStock"
              />
              <button
                @click="incrementQuantity"
                :disabled="quantity >= (product.stockQuantity || 1) || !product.inStock"
                class="btn btn-ghost btn-sm rounded-none"
              >
                +
              </button>
            </div>

            <!-- Add to Cart Button -->
            <button
              @click="addToCart"
              :disabled="!product.inStock || addingToCart"
              class="btn btn-primary flex-1"
            >
              <span v-if="addingToCart" class="loading loading-spinner loading-sm"></span>
              <span v-else>{{ $t('product.addToCart') }}</span>
            </button>
          </div>

          <!-- Tags -->
          <div v-if="product.tags && product.tags.length > 0" class="flex gap-2 flex-wrap mb-6">
            <div v-for="tag in product.tags" :key="tag" class="badge badge-lg badge-outline">
              {{ tag }}
            </div>
          </div>

          <!-- Share -->
          <div class="flex gap-2 items-center pt-4 border-t border-base-300">
            <span class="text-sm text-base-content/70">{{ $t('product.share') }}</span>
            <button class="btn btn-ghost btn-circle btn-sm" title="Share on Facebook">f</button>
            <button class="btn btn-ghost btn-circle btn-sm" title="Share on Twitter">𝕏</button>
            <button class="btn btn-ghost btn-circle btn-sm" title="Copy link">🔗</button>
          </div>
        </div>
      </div>

      <!-- Specifications -->
      <div v-if="product.specifications" class="mt-12">
        <h2 class="text-2xl font-bold mb-6">{{ $t('product.specifications') }}</h2>
        <div class="card bg-base-200 shadow-sm overflow-hidden">
          <div class="overflow-x-auto">
            <table class="table w-full">
              <tbody>
                <tr v-for="(value, key) in product.specifications" :key="key">
                  <td class="font-semibold w-1/3">{{ key }}</td>
                  <td>{{ value }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <!-- Reviews Section -->
      <div v-if="reviews.length > 0" class="mt-12">
        <h2 class="text-2xl font-bold mb-6">{{ $t('product.customerReviews') }}</h2>

        <div class="space-y-4">
          <div v-for="review in reviews" :key="review.id" class="card bg-base-200 shadow-sm">
            <div class="card-body">
              <div class="flex justify-between items-start">
                <div>
                  <h3 class="card-title text-base">{{ review.title }}</h3>
                  <p class="text-sm text-base-content/70">
                    by <span class="font-semibold">{{ review.author }}</span>
                    <span v-if="review.verified" class="badge badge-sm badge-success ml-2">
                      {{ $t('product.verified') }}
                    </span>
                  </p>
                </div>
                <div class="text-right">
                  <div class="rating rating-sm gap-1">
                    <input
                      v-for="i in 5"
                      :key="i"
                      type="radio"
                      :checked="i <= review.rating"
                      class="mask mask-star-2 bg-warning"
                      disabled
                    />
                  </div>
                  <p class="text-xs text-base-content/70 mt-1">
                    {{ review.date }}
                  </p>
                </div>
              </div>

              <p class="mt-4 text-base-content/90">{{ review.comment }}</p>
            </div>
          </div>
        </div>

        <!-- View All Reviews -->
        <button class="btn btn-ghost w-full mt-6">
          View All Reviews ({{ reviews.length }} total)
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Remove number input spinner */
input[type='number']::-webkit-outer-spin-button,
input[type='number']::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

input[type='number'] {
  -moz-appearance: textfield;
}

.no-spinner::-webkit-outer-spin-button,
.no-spinner::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

.no-spinner {
  -moz-appearance: textfield;
}
</style>
