<script setup lang="ts">
/**
 * ProductCardWidget - Single product display card
 */
import { computed } from 'vue';
import type { ProductCardWidgetConfig } from '@/types/widgets';

const props = defineProps<{
  config: ProductCardWidgetConfig;
  isEditing?: boolean;
}>();

// Mock product for preview
const mockProduct = {
  id: 'prod-1',
  name: 'Premium Werkzeugset 42-teilig',
  price: 149.99,
  originalPrice: 189.99,
  image: 'https://placehold.co/400x400/e2e8f0/475569?text=Produkt',
  description: 'Hochwertiges Werkzeugset für professionelle Anwendungen mit robustem Koffer.',
  badges: ['Sale', 'Bestseller'],
  inStock: true,
};

const aspectRatioClass = computed(() => {
  const ratios: Record<string, string> = {
    '1:1': 'aspect-square',
    '4:3': 'aspect-[4/3]',
    '3:4': 'aspect-[3/4]',
    '16:9': 'aspect-video',
  };
  return ratios[props.config.imageAspectRatio] || 'aspect-square';
});

const variantClass = computed(() => {
  const variants: Record<string, string> = {
    default: 'product-card--default',
    compact: 'product-card--compact',
    horizontal: 'product-card--horizontal',
  };
  return variants[props.config.variant] || 'product-card--default';
});

const discount = computed(() => {
  if (!mockProduct.originalPrice || mockProduct.originalPrice <= mockProduct.price) return null;
  return Math.round((1 - mockProduct.price / mockProduct.originalPrice) * 100);
});
</script>

<template>
  <div :class="['product-card', variantClass]">
    <!-- Image -->
    <div v-if="config.showImage" class="product-card__image-wrapper">
      <div :class="['product-card__image-container', aspectRatioClass]">
        <img
          :src="mockProduct.image"
          :alt="mockProduct.name"
          class="product-card__image"
        />
      </div>
      
      <!-- Badges -->
      <div v-if="config.showBadges && mockProduct.badges.length" class="product-card__badges">
        <span
          v-for="badge in mockProduct.badges"
          :key="badge"
          :class="['product-card__badge', badge === 'Sale' ? 'product-card__badge--sale' : '']"
        >
          {{ badge }}
        </span>
      </div>
      
      <!-- Wishlist Button -->
      <button
        v-if="config.showWishlistButton"
        class="product-card__wishlist-btn"
        title="Auf Merkzettel"
      >
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M20.84 4.61a5.5 5.5 0 00-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 00-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 000-7.78z" />
        </svg>
      </button>
    </div>

    <!-- Content -->
    <div class="product-card__content">
      <!-- Title -->
      <h3
        v-if="config.showTitle"
        class="product-card__title"
        :style="{ '-webkit-line-clamp': config.titleLines }"
      >
        {{ mockProduct.name }}
      </h3>

      <!-- Description -->
      <p
        v-if="config.showDescription"
        class="product-card__description"
        :style="{ '-webkit-line-clamp': config.descriptionLines }"
      >
        {{ mockProduct.description }}
      </p>

      <!-- Price -->
      <div v-if="config.showPrice" class="product-card__price-wrapper">
        <span class="product-card__price">{{ mockProduct.price.toFixed(2) }} €</span>
        <span v-if="discount" class="product-card__original-price">
          {{ mockProduct.originalPrice?.toFixed(2) }} €
        </span>
        <span v-if="discount" class="product-card__discount">
          -{{ discount }}%
        </span>
      </div>

      <!-- Add to Cart -->
      <button v-if="config.showAddToCart" class="product-card__cart-btn">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <circle cx="9" cy="21" r="1" />
          <circle cx="20" cy="21" r="1" />
          <path d="M1 1h4l2.68 13.39a2 2 0 002 1.61h9.72a2 2 0 002-1.61L23 6H6" />
        </svg>
        In den Warenkorb
      </button>
    </div>

    <!-- Edit Mode Indicator -->
    <div v-if="isEditing && !config.productId" class="product-card__placeholder">
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
        <path d="M20.25 7.5l-.625 10.632a2.25 2.25 0 01-2.247 2.118H6.622a2.25 2.25 0 01-2.247-2.118L3.75 7.5M10 11.25h4M3.375 7.5h17.25c.621 0 1.125-.504 1.125-1.125v-1.5c0-.621-.504-1.125-1.125-1.125H3.375c-.621 0-1.125.504-1.125 1.125v1.5c0 .621.504 1.125 1.125 1.125z" />
      </svg>
      <span>Produkt auswählen</span>
    </div>
  </div>
</template>

<style scoped>
.product-card {
  position: relative;
  display: flex;
  flex-direction: column;
  background: white;
  border-radius: 0.5rem;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  transition: box-shadow 0.2s, transform 0.2s;
}

.product-card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  transform: translateY(-2px);
}

.product-card--horizontal {
  flex-direction: row;
}

.product-card--horizontal .product-card__image-wrapper {
  width: 40%;
  flex-shrink: 0;
}

.product-card--compact .product-card__content {
  padding: 0.75rem;
}

.product-card__image-wrapper {
  position: relative;
}

.product-card__image-container {
  width: 100%;
  overflow: hidden;
  background: #f3f4f6;
}

.product-card__image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s;
}

.product-card:hover .product-card__image {
  transform: scale(1.05);
}

.product-card__badges {
  position: absolute;
  top: 0.5rem;
  left: 0.5rem;
  display: flex;
  flex-wrap: wrap;
  gap: 0.25rem;
}

.product-card__badge {
  padding: 0.25rem 0.5rem;
  font-size: 0.75rem;
  font-weight: 600;
  background: #3b82f6;
  color: white;
  border-radius: 0.25rem;
}

.product-card__badge--sale {
  background: #ef4444;
}

.product-card__wishlist-btn {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  width: 2rem;
  height: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
  background: white;
  border: none;
  border-radius: 50%;
  cursor: pointer;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  transition: background 0.2s;
}

.product-card__wishlist-btn:hover {
  background: #fef2f2;
}

.product-card__wishlist-btn svg {
  width: 1rem;
  height: 1rem;
  color: #6b7280;
}

.product-card__wishlist-btn:hover svg {
  color: #ef4444;
}

.product-card__content {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  padding: 1rem;
  flex: 1;
}

.product-card__title {
  margin: 0;
  font-size: 0.9375rem;
  font-weight: 500;
  color: #111827;
  display: -webkit-box;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.product-card__description {
  margin: 0;
  font-size: 0.875rem;
  color: #6b7280;
  display: -webkit-box;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.product-card__price-wrapper {
  display: flex;
  align-items: baseline;
  gap: 0.5rem;
  margin-top: auto;
}

.product-card__price {
  font-size: 1.125rem;
  font-weight: 600;
  color: #111827;
}

.product-card__original-price {
  font-size: 0.875rem;
  color: #9ca3af;
  text-decoration: line-through;
}

.product-card__discount {
  font-size: 0.75rem;
  font-weight: 600;
  color: #ef4444;
}

.product-card__cart-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding: 0.75rem 1rem;
  background: #3b82f6;
  color: white;
  border: none;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: background 0.2s;
}

.product-card__cart-btn:hover {
  background: #2563eb;
}

.product-card__cart-btn svg {
  width: 1rem;
  height: 1rem;
}

.product-card__placeholder {
  position: absolute;
  inset: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  background: rgba(255, 255, 255, 0.9);
  color: #6b7280;
  font-size: 0.875rem;
}

.product-card__placeholder svg {
  width: 2rem;
  height: 2rem;
}
</style>
