<script setup lang="ts">
import { ref, watch } from 'vue';
import type { PriceBreakdown } from '@/types/pricing';

interface Props {
  productPrice: number;
  destinationCountry?: string;
  shippingCost?: number;
  currencyCode?: string;
  showBreakdown?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  destinationCountry: 'DE',
  shippingCost: 0,
  currencyCode: 'EUR',
  showBreakdown: true,
});

const breakdown = ref<PriceBreakdown | null>(null);
const isLoading = ref(false);
const error = ref<string | null>(null);

// Fetch price breakdown from API
const fetchPriceBreakdown = async () => {
  isLoading.value = true;
  error.value = null;

  try {
    const response = await fetch('/api/catalog/getpricebreakdown', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        productPrice: props.productPrice,
        destinationCountry: props.destinationCountry,
        shippingCost: props.shippingCost,
      }),
    });

    const data = await response.json();
    if (data.success) {
      breakdown.value = data.breakdown;
    } else {
      error.value = data.message || 'Failed to calculate price';
    }
  } catch (err) {
    error.value = 'Error fetching price breakdown';
    console.error(err);
  } finally {
    isLoading.value = false;
  }
};

// Auto-fetch when props change
watch(
  () => [props.productPrice, props.destinationCountry, props.shippingCost],
  fetchPriceBreakdown,
  { immediate: true }
);

const formatPrice = (price: number, currency: string = 'EUR') => {
  return new Intl.NumberFormat('de-DE', {
    style: 'currency',
    currency,
  }).format(price);
};
</script>

<template>
  <div class="space-y-2">
    <!-- Main Price Display -->
    <div class="text-lg font-semibold" v-if="breakdown">
      <span class="text-2xl">{{ formatPrice(breakdown.finalTotal, currencyCode) }}</span>
      <span class="text-sm text-gray-600 ml-1">inkl. {{ breakdown.vatRate }}% MwSt.</span>
    </div>

    <!-- Error State -->
    <div v-if="error" class="text-red-600 text-sm">{{ error }}</div>

    <!-- Loading State -->
    <div v-if="isLoading" class="text-gray-400 text-sm">{{ $t('product.price.calculating') }}</div>

    <!-- Price Breakdown (Details) -->
    <div v-if="showBreakdown && breakdown && !isLoading" class="border-t pt-2 text-sm">
      <div class="flex justify-between text-gray-700">
        <span>{{ $t('product.price.productPrice') }}</span>
        <span>{{ formatPrice(breakdown.productPrice, currencyCode) }}</span>
      </div>

      <div v-if="breakdown.shippingCost > 0" class="flex justify-between text-gray-700">
        <span>{{ $t('product.price.shipping') }}</span>
        <span>{{ formatPrice(breakdown.shippingCost, currencyCode) }}</span>
      </div>

      <div class="flex justify-between font-semibold border-t mt-1 pt-1">
        <span>{{ $t('product.price.subtotalExclVat') }}</span>
        <span>{{
          formatPrice(breakdown.productPrice + (breakdown.shippingCost || 0), currencyCode)
        }}</span>
      </div>

      <div class="flex justify-between text-green-700 font-semibold">
        <span>MwSt. ({{ breakdown.vatRate }}%):</span>
        <span>{{ formatPrice(breakdown.vatAmount, currencyCode) }}</span>
      </div>

      <div class="flex justify-between font-bold border-t mt-1 pt-1 text-lg">
        <span>{{ $t('product.price.totalPrice') }}</span>
        <span>{{ formatPrice(breakdown.finalTotal, currencyCode) }}</span>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Tailwind classes handle styling */
</style>
