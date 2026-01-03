<script setup lang="ts">
/**
 * ProductGridWidget - Grid of product cards with filters and pagination
 */
import { computed } from 'vue';
import type { ProductGridWidgetConfig } from '@/types/widgets';

const props = defineProps<{
  config: ProductGridWidgetConfig;
  isEditing?: boolean;
}>();

// Mock products for preview
const mockProducts = [
  {
    id: '1',
    name: 'Akku-Bohrschrauber Pro',
    price: 199.99,
    image: 'https://placehold.co/300x300/e2e8f0/475569?text=1',
    badge: 'Neu',
  },
  {
    id: '2',
    name: 'Kreissäge 1800W',
    price: 149.5,
    originalPrice: 179.99,
    image: 'https://placehold.co/300x300/e2e8f0/475569?text=2',
    badge: 'Sale',
  },
  {
    id: '3',
    name: 'Werkzeugkoffer 156-tlg',
    price: 89.99,
    image: 'https://placehold.co/300x300/e2e8f0/475569?text=3',
  },
  {
    id: '4',
    name: 'Schlagbohrmaschine',
    price: 129.0,
    image: 'https://placehold.co/300x300/e2e8f0/475569?text=4',
  },
  {
    id: '5',
    name: 'Winkelschleifer 125mm',
    price: 79.95,
    image: 'https://placehold.co/300x300/e2e8f0/475569?text=5',
  },
  {
    id: '6',
    name: 'Stichsäge mit Pendelhub',
    price: 99.0,
    originalPrice: 119.0,
    image: 'https://placehold.co/300x300/e2e8f0/475569?text=6',
    badge: 'Sale',
  },
  {
    id: '7',
    name: 'Bandschleifer 750W',
    price: 159.99,
    image: 'https://placehold.co/300x300/e2e8f0/475569?text=7',
  },
  {
    id: '8',
    name: 'Exzenterschleifer',
    price: 69.95,
    image: 'https://placehold.co/300x300/e2e8f0/475569?text=8',
    badge: 'Bestseller',
  },
];

const getResponsiveValue = <T,>(value: T | { mobile?: T; tablet?: T; desktop?: T }): T => {
  if (typeof value === 'object' && value !== null && 'desktop' in value) {
    return (
      (value as { mobile?: T; tablet?: T; desktop?: T }).desktop ??
      (value as { mobile?: T; tablet?: T; desktop?: T }).tablet ??
      ((value as { mobile?: T; tablet?: T; desktop?: T }).mobile as T)
    );
  }
  return value as T;
};

const gridColumns = computed(() => getResponsiveValue(props.config.columns));
const gridGap = computed(() => getResponsiveValue(props.config.gap));
const totalProducts = computed(() =>
  Math.min(gridColumns.value * props.config.rows, mockProducts.length)
);
const displayProducts = computed(() => mockProducts.slice(0, totalProducts.value));

const sourceLabel = computed(() => {
  const labels: Record<string, string> = {
    category: 'Kategorie',
    manual: 'Manuell',
    bestseller: 'Bestseller',
    new: 'Neuheiten',
    sale: 'Angebote',
  };
  return labels[props.config.source] || props.config.source;
});
</script>

<template>
  <div class="product-grid">
    <!-- Header -->
    <div v-if="config.title || config.showSorting" class="product-grid__header">
      <h2 v-if="config.title" class="product-grid__title">{{ config.title }}</h2>
      <div v-if="config.showSorting" class="product-grid__sorting">
        <label>Sortieren:</label>
        <select class="product-grid__sort-select">
          <option value="relevance">Relevanz</option>
          <option value="price-asc">Preis aufsteigend</option>
          <option value="price-desc">Preis absteigend</option>
          <option value="name-asc">Name A-Z</option>
          <option value="name-desc">Name Z-A</option>
          <option value="newest">Neueste</option>
        </select>
      </div>
    </div>

    <!-- Filters -->
    <div v-if="config.showFilters" class="product-grid__filters">
      <button class="product-grid__filter-btn">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M3 6h18M6 12h12M9 18h6" />
        </svg>
        Filter
      </button>
      <div class="product-grid__active-filters">
        <span class="product-grid__filter-tag">
          Preis: 50€ - 200€
          <button>×</button>
        </span>
      </div>
    </div>

    <!-- Grid -->
    <div
      class="product-grid__grid"
      :style="{
        gridTemplateColumns: `repeat(${gridColumns}, 1fr)`,
        gap: gridGap,
      }"
    >
      <div v-for="product in displayProducts" :key="product.id" class="product-grid__item">
        <div class="product-grid__card">
          <div class="product-grid__image-wrapper">
            <img :src="product.image" :alt="product.name" class="product-grid__image" />
            <span
              v-if="product.badge"
              :class="[
                'product-grid__badge',
                product.badge === 'Sale' ? 'product-grid__badge--sale' : '',
              ]"
            >
              {{ product.badge }}
            </span>
            <button class="product-grid__wishlist">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path
                  d="M20.84 4.61a5.5 5.5 0 00-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 00-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 000-7.78z"
                />
              </svg>
            </button>
          </div>
          <div class="product-grid__info">
            <h3 class="product-grid__product-name">{{ product.name }}</h3>
            <div class="product-grid__price-row">
              <span class="product-grid__price">{{ product.price.toFixed(2) }} €</span>
              <span v-if="product.originalPrice" class="product-grid__old-price">
                {{ product.originalPrice.toFixed(2) }} €
              </span>
            </div>
            <button class="product-grid__add-btn">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="9" cy="21" r="1" />
                <circle cx="20" cy="21" r="1" />
                <path d="M1 1h4l2.68 13.39a2 2 0 002 1.61h9.72a2 2 0 002-1.61L23 6H6" />
              </svg>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-if="displayProducts.length === 0" class="product-grid__empty">
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
        <path
          d="M20.25 7.5l-.625 10.632a2.25 2.25 0 01-2.247 2.118H6.622a2.25 2.25 0 01-2.247-2.118L3.75 7.5M10 11.25h4M3.375 7.5h17.25c.621 0 1.125-.504 1.125-1.125v-1.5c0-.621-.504-1.125-1.125-1.125H3.375c-.621 0-1.125.504-1.125 1.125v1.5c0 .621.504 1.125 1.125 1.125z"
        />
      </svg>
      <p>{{ config.emptyStateMessage }}</p>
    </div>

    <!-- Pagination -->
    <div v-if="config.showPagination" class="product-grid__pagination">
      <button class="product-grid__page-btn" disabled>
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M15 19l-7-7 7-7" />
        </svg>
      </button>
      <button class="product-grid__page-btn product-grid__page-btn--active">1</button>
      <button class="product-grid__page-btn">2</button>
      <button class="product-grid__page-btn">3</button>
      <span class="product-grid__page-dots">...</span>
      <button class="product-grid__page-btn">12</button>
      <button class="product-grid__page-btn">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <path d="M9 5l7 7-7 7" />
        </svg>
      </button>
    </div>

    <!-- Edit Mode Info -->
    <div v-if="isEditing" class="product-grid__edit-info">
      <span class="product-grid__source-badge">Quelle: {{ sourceLabel }}</span>
    </div>
  </div>
</template>

<style scoped>
.product-grid {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.product-grid__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 1rem;
}

.product-grid__title {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 600;
  color: #111827;
}

.product-grid__sorting {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.875rem;
  color: #6b7280;
}

.product-grid__sort-select {
  padding: 0.5rem 2rem 0.5rem 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  background: white;
  cursor: pointer;
}

.product-grid__filters {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 0.75rem;
  background: #f9fafb;
  border-radius: 0.5rem;
}

.product-grid__filter-btn {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.5rem 1rem;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  background: white;
  font-size: 0.875rem;
  cursor: pointer;
}

.product-grid__filter-btn svg {
  width: 1rem;
  height: 1rem;
}

.product-grid__active-filters {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.product-grid__filter-tag {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.75rem;
  background: #e0e7ff;
  color: #4338ca;
  border-radius: 1rem;
  font-size: 0.75rem;
}

.product-grid__filter-tag button {
  background: none;
  border: none;
  color: #4338ca;
  cursor: pointer;
  font-size: 1rem;
  line-height: 1;
}

.product-grid__grid {
  display: grid;
}

.product-grid__card {
  background: white;
  border-radius: 0.5rem;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  transition:
    box-shadow 0.2s,
    transform 0.2s;
}

.product-grid__card:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  transform: translateY(-2px);
}

.product-grid__image-wrapper {
  position: relative;
  aspect-ratio: 1;
  background: #f3f4f6;
  overflow: hidden;
}

.product-grid__image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s;
}

.product-grid__card:hover .product-grid__image {
  transform: scale(1.05);
}

.product-grid__badge {
  position: absolute;
  top: 0.5rem;
  left: 0.5rem;
  padding: 0.25rem 0.5rem;
  background: #3b82f6;
  color: white;
  font-size: 0.75rem;
  font-weight: 600;
  border-radius: 0.25rem;
}

.product-grid__badge--sale {
  background: #ef4444;
}

.product-grid__wishlist {
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
  opacity: 0;
  transition: opacity 0.2s;
}

.product-grid__card:hover .product-grid__wishlist {
  opacity: 1;
}

.product-grid__wishlist svg {
  width: 1rem;
  height: 1rem;
  color: #6b7280;
}

.product-grid__info {
  position: relative;
  padding: 1rem;
}

.product-grid__product-name {
  margin: 0 0 0.5rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: #111827;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.product-grid__price-row {
  display: flex;
  align-items: baseline;
  gap: 0.5rem;
}

.product-grid__price {
  font-size: 1rem;
  font-weight: 600;
  color: #111827;
}

.product-grid__old-price {
  font-size: 0.75rem;
  color: #9ca3af;
  text-decoration: line-through;
}

.product-grid__add-btn {
  position: absolute;
  right: 1rem;
  bottom: 1rem;
  width: 2.5rem;
  height: 2.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #3b82f6;
  border: none;
  border-radius: 50%;
  cursor: pointer;
  opacity: 0;
  transition:
    opacity 0.2s,
    background 0.2s;
}

.product-grid__card:hover .product-grid__add-btn {
  opacity: 1;
}

.product-grid__add-btn:hover {
  background: #2563eb;
}

.product-grid__add-btn svg {
  width: 1.25rem;
  height: 1.25rem;
  color: white;
}

.product-grid__empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem;
  text-align: center;
  color: #6b7280;
}

.product-grid__empty svg {
  width: 3rem;
  height: 3rem;
  margin-bottom: 1rem;
}

.product-grid__pagination {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.25rem;
}

.product-grid__page-btn {
  min-width: 2.5rem;
  height: 2.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  background: white;
  font-size: 0.875rem;
  cursor: pointer;
  transition:
    background 0.2s,
    border-color 0.2s;
}

.product-grid__page-btn:hover:not(:disabled) {
  background: #f3f4f6;
}

.product-grid__page-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.product-grid__page-btn--active {
  background: #3b82f6;
  border-color: #3b82f6;
  color: white;
}

.product-grid__page-btn svg {
  width: 1rem;
  height: 1rem;
}

.product-grid__page-dots {
  color: #9ca3af;
}

.product-grid__edit-info {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
}

.product-grid__source-badge {
  padding: 0.25rem 0.5rem;
  background: #dbeafe;
  color: #1e40af;
  font-size: 0.75rem;
  border-radius: 0.25rem;
}
</style>
