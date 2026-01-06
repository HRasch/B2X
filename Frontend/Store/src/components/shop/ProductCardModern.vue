<script setup lang="ts">
import { computed } from 'vue';

export interface Product {
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

export interface PriceBreakdown {
  net: number;
  vatRate: number;
  vat: number;
  total: number;
}

const props = defineProps<{
  product: Product;
  priceBreakdown?: PriceBreakdown;
}>();

const emit = defineEmits<{
  'add-to-cart': [product: Product];
}>();

// Format price with currency
const formattedPrice = computed(() => {
  return new Intl.NumberFormat('de-DE', {
    style: 'currency',
    currency: 'EUR',
  }).format(props.product.price);
});

// VAT breakdown (assuming 19% VAT for Germany)
const vatInfo = computed(() => {
  const vatRate = 0.19;
  const net = props.product.price / (1 + vatRate);
  const vat = props.product.price - net;
  return {
    net: net.toFixed(2),
    vat: vat.toFixed(2),
    total: props.product.price.toFixed(2),
  };
});

const handleAddToCart = () => {
  emit('add-to-cart', props.product);
};
</script>

<template>
  <div class="card bg-base-100 shadow-md hover:shadow-xl transition-shadow duration-300">
    <!-- Product Image Section -->
    <figure class="relative bg-base-200 overflow-hidden h-48">
      <img
        :src="product.image"
        :alt="product.name"
        class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
        loading="lazy"
      />
      <!-- Stock Badge -->
      <div class="absolute top-2 right-2">
        <div :class="['badge', product.inStock ? 'badge-success' : 'badge-error']">
          {{ product.inStock ? 'In Stock' : 'Out of Stock' }}
        </div>
      </div>
      <!-- Category Badge -->
      <div class="absolute top-2 left-2">
        <div class="badge badge-primary">{{ product.category }}</div>
      </div>
    </figure>

    <!-- Product Info Section -->
    <div class="card-body p-4">
      <!-- Product Name & Rating -->
      <div class="flex justify-between items-start gap-2">
        <h2 class="card-title text-base line-clamp-2">{{ product.name }}</h2>
        <!-- Star Rating -->
        <div class="flex items-center gap-1 flex-shrink-0">
          <div class="rating rating-sm">
            <input
              type="radio"
              name="rating"
              class="mask mask-star-2 bg-orange-400"
              :checked="Math.round(product.rating) >= 1"
              disabled
            />
            <input
              type="radio"
              name="rating"
              class="mask mask-star-2 bg-orange-400"
              :checked="Math.round(product.rating) >= 2"
              disabled
            />
            <input
              type="radio"
              name="rating"
              class="mask mask-star-2 bg-orange-400"
              :checked="Math.round(product.rating) >= 3"
              disabled
            />
            <input
              type="radio"
              name="rating"
              class="mask mask-star-2 bg-orange-400"
              :checked="Math.round(product.rating) >= 4"
              disabled
            />
            <input
              type="radio"
              name="rating"
              class="mask mask-star-2 bg-orange-400"
              :checked="Math.round(product.rating) >= 5"
              disabled
            />
          </div>
          <span class="text-xs text-base-content/70">{{ product.rating.toFixed(1) }}</span>
        </div>
      </div>

      <!-- Description (truncated) -->
      <p class="text-sm text-base-content/70 line-clamp-2">
        {{ product.description }}
      </p>

      <!-- SKU (if available) -->
      <p v-if="product.sku" class="text-xs text-base-content/50 mt-1">SKU: {{ product.sku }}</p>

      <!-- Price Breakdown Section -->
      <div class="divider my-2"></div>
      <div class="price-section">
        <!-- Main Price -->
        <div class="flex justify-between items-baseline mb-1">
          <span class="text-sm font-semibold">{{ $t('product.price.total') }}</span>
          <span class="text-2xl font-bold text-primary">
            {{ formattedPrice }}
          </span>
        </div>

        <!-- VAT Breakdown (collapsed) -->
        <details class="collapse bg-base-200 rounded-lg p-0">
          <summary class="collapse-title text-xs text-base-content/70 py-1 px-2 min-h-auto">
            incl. {{ (19).toFixed(0) }}% VAT ({{ vatInfo.vat }}€)
          </summary>
          <div class="collapse-content text-xs space-y-1 p-2">
            <div class="flex justify-between">
              <span>{{ $t('product.price.netPrice') }}</span>
              <span>€{{ vatInfo.net }}</span>
            </div>
            <div class="flex justify-between">
              <span>{{ $t('product.price.vat', { rate: 19 }) }}</span>
              <span>€{{ vatInfo.vat }}</span>
            </div>
            <div class="divider my-1"></div>
            <div class="flex justify-between font-semibold">
              <span>{{ $t('product.price.total') }}</span>
              <span>€{{ vatInfo.total }}</span>
            </div>
          </div>
        </details>
      </div>

      <!-- Action Buttons -->
      <div class="card-actions justify-between items-center mt-4">
        <!-- Quick View (Link to Detail) -->
        <router-link :to="`/product/${product.id}`" class="link link-hover text-sm">
          {{ $t('product.viewDetails') }}
        </router-link>

        <!-- Add to Cart Button -->
        <button
          @click="handleAddToCart"
          :disabled="!product.inStock"
          :class="['btn btn-sm', product.inStock ? 'btn-primary' : 'btn-disabled']"
          aria-label="`Add ${product.name} to cart`"
        >
          <span v-if="product.inStock">{{ $t('product.addToCart') }}</span>
          <span v-else>{{ $t('product.notAvailable') }}</span>
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.card {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.card-body {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.price-section {
  margin-top: auto;
}

/* Rating stars styling */
.rating {
  gap: 0.25rem;
}

.rating input[type='radio']:disabled:checked {
  background-color: currentColor;
}

/* Hover effects */
.card:hover {
  @apply shadow-xl;
}

figure img {
  transition: transform 0.3s ease-in-out;
}

.card:hover figure img {
  transform: scale(1.05);
}

/* Responsive adjustments */
@media (max-width: 640px) {
  .card-body {
    padding: 0.75rem;
  }

  .card-title {
    font-size: 0.875rem;
  }

  .price-section {
    margin-top: 0.5rem;
  }
}
</style>
