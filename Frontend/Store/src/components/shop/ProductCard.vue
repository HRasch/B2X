<template>
  <div
    class="card bg-base-100 shadow-lg hover:shadow-xl transition-all hover:-translate-y-1 h-full flex flex-col"
    data-testid="product-card"
  >
    <!-- Product Image -->
    <figure class="relative h-48 overflow-hidden">
      <img :src="product.image" :alt="product.name" class="w-full h-full object-cover" />
      <div
        v-if="!product.inStock"
        class="absolute inset-0 bg-black/50 flex items-center justify-center"
      >
        <span class="text-white font-bold text-sm">{{ $t('products.notAvailable') }}</span>
      </div>
    </figure>

    <!-- Product Info -->
    <div class="card-body flex flex-col flex-1">
      <!-- Product Name -->
      <h3 class="card-title text-base line-clamp-2" data-testid="product-name">
        {{ product.name }}
      </h3>

      <!-- Rating -->
      <div class="flex items-center gap-1">
        <span class="text-yellow-500">★</span>
        <span class="text-sm">{{ product.rating }}</span>
      </div>

      <!-- Description -->
      <p class="text-sm opacity-70 line-clamp-2">{{ product.description }}</p>

      <!-- Pricing Section -->
      <div class="divider my-2"></div>
      <div class="bg-base-200 rounded-lg p-3 mb-4">
        <div class="flex justify-between items-start mb-2">
          <span class="text-3xl font-bold text-primary" data-testid="product-price">
            {{ formatPrice(displayPrice()) }}
          </span>
        </div>
        <p class="text-xs opacity-70">inkl. MwSt {{ (vatRate() * 100).toFixed(0) }}%</p>
        <div
          v-if="product.priceBreakdown?.OriginalPrice || product.priceBreakdown?.DiscountAmount"
          class="mt-2 flex gap-2"
        >
          <span
            v-if="product.priceBreakdown?.OriginalPrice"
            class="text-xs line-through opacity-70"
          >
            {{ formatPrice(product.priceBreakdown.OriginalPrice) }}
          </span>
          <span v-if="product.priceBreakdown?.DiscountAmount" class="badge badge-error badge-sm">
            -{{ formatPrice(product.priceBreakdown.DiscountAmount) }}
          </span>
        </div>
      </div>

      <!-- Add to Cart Button -->
      <button
        v-if="product.inStock"
        @click="$emit('add-to-cart', product)"
        class="btn btn-primary w-full mt-auto"
      >
        {{ $t('products.addToCart') }}
      </button>
      <button v-else class="btn btn-disabled w-full mt-auto">
        {{ $t('products.notAvailable') }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Product } from '@/services/productService';

interface PriceBreakdown {
  PriceIncludingVat: number;
  VatAmount: number;
  VatRate: number;
  OriginalPrice: number | null;
  DiscountAmount: number | null;
  FinalPrice: number;
  Currency: string;
  DestinationCountry: string;
}

// Extend Product with optional priceBreakdown
interface ProductWithPricing extends Product {
  priceBreakdown?: PriceBreakdown;
}

const props = defineProps<{
  product: ProductWithPricing;
}>();

defineEmits<{
  'add-to-cart': [product: ProductWithPricing];
}>();

// Format currency for display
const formatPrice = (price: number): string => {
  return new Intl.NumberFormat('de-DE', {
    style: 'currency',
    currency: 'EUR',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(price);
};

// Get display price (supports both formats)
const displayPrice = () => {
  if (props.product.priceBreakdown) {
    return props.product.priceBreakdown.PriceIncludingVat;
  }
  return props.product.price ?? 0;
};

// Get VAT rate (defaults to 19% German VAT)
const vatRate = () => {
  if (props.product.priceBreakdown) {
    return props.product.priceBreakdown.VatRate;
  }
  return 0.19;
};
</script>
