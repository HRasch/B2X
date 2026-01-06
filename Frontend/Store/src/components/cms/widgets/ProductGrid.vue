<template>
  <div class="product-grid">
    <div class="grid-container" :style="gridStyles">
      <div v-for="product in products" :key="product.id" class="product-card">
        <img v-if="product.image" :src="product.image" :alt="product.name" class="product-image" />
        <div class="product-info">
          <h3 class="product-name">{{ product.name }}</h3>
          <p v-if="product.description" class="product-description">
            {{ product.description }}
          </p>
          <div class="product-footer">
            <span v-if="product.price" class="product-price">
              {{ formatPrice(product.price) }}
            </span>
            <button class="add-to-cart-btn">{{ $t('product.addToCart') }}</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

interface Product {
  id: string;
  name: string;
  description?: string;
  price?: number;
  image?: string;
}

interface Props {
  settings?: {
    columns?: number;
    gap?: string;
    products?: Product[];
  };
  widgetId?: string;
}

const props = withDefaults(defineProps<Props>(), {
  settings: () => ({
    columns: 3,
    gap: '1.5rem',
    products: [] as Product[],
  }),
});

const products = computed(() => props.settings.products || []);

const gridStyles = computed(() => ({
  display: 'grid',
  gridTemplateColumns: `repeat(${props.settings.columns || 3}, 1fr)`,
  gap: props.settings.gap || '1.5rem',
}));

const formatPrice = (price: number) => {
  return new Intl.NumberFormat('de-DE', {
    style: 'currency',
    currency: 'EUR',
  }).format(price);
};
</script>

<style scoped>
.product-grid {
  width: 100%;
  padding: 1rem;
}

.grid-container {
  width: 100%;
}

.product-card {
  background: white;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  overflow: hidden;
  transition: box-shadow 0.3s;
}

.product-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.product-image {
  width: 100%;
  height: 250px;
  object-fit: cover;
}

.product-info {
  padding: 1rem;
}

.product-name {
  font-size: 1.125rem;
  font-weight: bold;
  margin: 0 0 0.5rem 0;
}

.product-description {
  font-size: 0.875rem;
  color: #666;
  margin: 0 0 1rem 0;
  line-height: 1.4;
}

.product-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
}

.product-price {
  font-size: 1.25rem;
  font-weight: bold;
  color: #007bff;
}

.add-to-cart-btn {
  flex: 1;
  padding: 0.5rem 1rem;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.875rem;
  transition: background-color 0.3s;
}

.add-to-cart-btn:hover {
  background-color: #0056b3;
}
</style>
