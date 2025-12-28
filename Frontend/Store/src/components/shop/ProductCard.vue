<template>
  <div class="product-card">
    <div class="product-image">
      <img :src="product.image" :alt="product.name" />
      <div v-if="!product.inStock" class="out-of-stock">Nicht verfügbar</div>
    </div>

    <div class="product-info">
      <h3 class="product-name">{{ product.name }}</h3>

      <div class="product-rating">
        <span class="stars">★ {{ product.rating }}</span>
      </div>

      <p class="product-description">{{ product.description }}</p>

      <div class="product-pricing">
        <div class="price-section">
          <span class="price"
            >{{ formatPrice(product.priceBreakdown.PriceIncludingVat) }}€</span
          >
          <span class="vat-text"
            >inkl. MwSt
            {{ (product.priceBreakdown.VatRate * 100).toFixed(0) }}%</span
          >
          <span
            v-if="product.priceBreakdown.OriginalPrice"
            class="original-price"
          >
            <s>{{ formatPrice(product.priceBreakdown.OriginalPrice) }}€</s>
          </span>
          <span
            v-if="product.priceBreakdown.DiscountAmount"
            class="discount-badge"
          >
            -{{ formatPrice(product.priceBreakdown.DiscountAmount) }}€
          </span>
        </div>
      </div>

      <button
        v-if="product.inStock"
        @click="$emit('add-to-cart', product)"
        class="add-to-cart-btn"
      >
        In Warenkorb
      </button>
      <button v-else class="add-to-cart-btn disabled" disabled>
        Nicht verfügbar
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
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

interface Product {
  id: string;
  name: string;
  priceBreakdown: PriceBreakdown;
  image: string;
  category: string;
  description: string;
  inStock: boolean;
  rating: number;
}

defineProps<{
  product: Product;
}>();

defineEmits<{
  "add-to-cart": [product: Product];
}>();

// Format currency for display
const formatPrice = (price: number): string => {
  return new Intl.NumberFormat("de-DE", {
    style: "currency",
    currency: "EUR",
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(price);
};
</script>

<style scoped>
.product-card {
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  overflow: hidden;
  background-color: white;
  transition: all 0.3s;
  display: flex;
  flex-direction: column;
  height: 100%;
}

.product-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  transform: translateY(-4px);
}

.product-image {
  position: relative;
  width: 100%;
  height: 200px;
  background-color: #f5f5f5;
  overflow: hidden;
}

.product-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.out-of-stock {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: bold;
  font-size: 0.9rem;
}

.product-info {
  padding: 1rem;
  flex: 1;
  display: flex;
  flex-direction: column;
}

.product-name {
  font-size: 1rem;
  font-weight: 600;
  margin: 0 0 0.5rem 0;
  color: #333;
  line-height: 1.4;
}

.product-rating {
  margin-bottom: 0.5rem;
}

.stars {
  color: #ffc107;
  font-size: 0.9rem;
}

.product-description {
  font-size: 0.85rem;
  color: #666;
  margin: 0.5rem 0;
  flex-grow: 1;
  line-height: 1.4;
}

.product-pricing {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin: 1rem 0;
  padding: 0.75rem;
  background-color: #f9f9f9;
  border-radius: 4px;
}

.price-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.label {
  font-size: 0.85rem;
  color: #666;
  font-weight: 500;
}

.price {
  font-size: 1.2rem;
  font-weight: 700;
  color: #2563eb;
}

.price.b2b {
  color: #059669;
}

.add-to-cart-btn {
  padding: 0.75rem;
  background-color: #2563eb;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  transition: background-color 0.3s;
  margin-top: auto;
}

.add-to-cart-btn:hover {
  background-color: #1d4ed8;
}

.add-to-cart-btn.disabled {
  background-color: #ccc;
  cursor: not-allowed;
  opacity: 0.6;
}

.add-to-cart-btn.disabled:hover {
  background-color: #ccc;
}
</style>
