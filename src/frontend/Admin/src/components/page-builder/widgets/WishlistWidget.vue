<script setup lang="ts">
/**
 * WishlistWidget - Customer saved products list
 */
import { computed } from 'vue';
import type { WishlistWidgetConfig } from '@/types/widgets';

const props = defineProps<{
  config: WishlistWidgetConfig;
  isEditing?: boolean;
}>();

// Mock wishlist data
const mockProducts = [
  {
    id: '1',
    name: 'Premium Werkzeugset 120-teilig',
    sku: 'WZ-120-PRO',
    price: '249,00 €',
    originalPrice: '299,00 €',
    image: 'https://placehold.co/200x200/e2e8f0/475569?text=Werkzeug',
    inStock: true,
  },
  {
    id: '2',
    name: 'Bosch Akkuschrauber Professional',
    sku: 'BS-AKK-PRO',
    price: '189,00 €',
    image: 'https://placehold.co/200x200/e2e8f0/475569?text=Bosch',
    inStock: true,
  },
  {
    id: '3',
    name: 'Laser-Entfernungsmesser 50m',
    sku: 'LE-50M',
    price: '79,99 €',
    image: 'https://placehold.co/200x200/e2e8f0/475569?text=Laser',
    inStock: false,
  },
  {
    id: '4',
    name: 'Arbeitshandschuhe Profi-Set',
    sku: 'AH-SET-10',
    price: '34,50 €',
    image: 'https://placehold.co/200x200/e2e8f0/475569?text=Handschuhe',
    inStock: true,
  },
];

const layout = computed(() => props.config.layout ?? 'grid');

function getResponsiveValue<T>(value: T | { mobile?: T; tablet?: T; desktop?: T }): T {
  if (typeof value === 'object' && value !== null && 'mobile' in value) {
    return (
      (value as { mobile?: T; tablet?: T; desktop?: T }).desktop ??
      ((value as { mobile?: T; tablet?: T; desktop?: T }).mobile as T)
    );
  }
  return value;
}

const gridColumns = computed(() => getResponsiveValue(props.config.gridColumns ?? 4));
</script>

<template>
  <div :class="['wishlist', { 'wishlist--editing': isEditing }]">
    <div class="wishlist__header">
      <h2 class="wishlist__title">{{ $t('pageBuilder.wishlist.title') }}</h2>
      <span class="wishlist__count">{{ mockProducts.length }} Artikel</span>
    </div>

    <!-- Empty State -->
    <div v-if="mockProducts.length === 0" class="wishlist__empty">
      <svg
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        stroke-width="1.5"
        class="wishlist__empty-icon"
      >
        <path
          d="M20.84 4.61a5.5 5.5 0 00-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 00-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 000-7.78z"
        />
      </svg>
      <p class="wishlist__empty-text">
        {{ config.emptyStateMessage ?? 'Dein Merkzettel ist leer.' }}
      </p>
      <button class="wishlist__empty-btn">{{ $t('pageBuilder.wishlist.discoverProducts') }}</button>
    </div>

    <!-- Product List -->
    <div
      v-else
      :class="['wishlist__products', `wishlist__products--${layout}`]"
      :style="layout === 'grid' ? `--columns: ${gridColumns}` : undefined"
    >
      <div v-for="product in mockProducts" :key="product.id" class="wishlist__product">
        <div class="wishlist__product-image">
          <img :src="product.image" :alt="product.name" />
          <button
            v-if="config.showRemove"
            class="wishlist__remove-btn"
            :disabled="isEditing"
            title="Entfernen"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div class="wishlist__product-info">
          <h3 class="wishlist__product-name">{{ product.name }}</h3>
          <p class="wishlist__product-sku">Art.-Nr.: {{ product.sku }}</p>

          <div v-if="config.showPrice" class="wishlist__product-price">
            <span class="wishlist__price-current">{{ product.price }}</span>
            <span v-if="product.originalPrice" class="wishlist__price-original">{{
              product.originalPrice
            }}</span>
          </div>

          <div class="wishlist__product-stock">
            <span
              :class="[
                'wishlist__stock-badge',
                product.inStock
                  ? 'wishlist__stock-badge--available'
                  : 'wishlist__stock-badge--unavailable',
              ]"
            >
              {{ product.inStock ? 'Auf Lager' : 'Nicht verfügbar' }}
            </span>
          </div>

          <div class="wishlist__product-actions">
            <button
              v-if="config.showAddToCart"
              class="wishlist__add-to-cart"
              :disabled="isEditing || !product.inStock"
            >
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path
                  d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z"
                />
              </svg>
              {{ $t('pageBuilder.wishlist.addToCart') }}
            </button>
            <button
              v-if="config.showShare"
              class="wishlist__share-btn"
              :disabled="isEditing"
              title="Teilen"
            >
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path
                  d="M8.684 13.342C8.886 12.938 9 12.482 9 12c0-.482-.114-.938-.316-1.342m0 2.684a3 3 0 110-2.684m0 2.684l6.632 3.316m-6.632-6l6.632-3.316m0 0a3 3 0 105.367-2.684 3 3 0 00-5.367 2.684zm0 9.316a3 3 0 105.368 2.684 3 3 0 00-5.368-2.684z"
                />
              </svg>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Edit Mode Indicator -->
    <div v-if="isEditing" class="wishlist__edit-hint">
      <span>Merkzettel Widget - {{ layout === 'grid' ? `${gridColumns} Spalten` : 'Liste' }}</span>
    </div>
  </div>
</template>

<style scoped>
.wishlist {
  padding: 1.5rem;
}

.wishlist--editing {
  border: 2px dashed #d1d5db;
  border-radius: 8px;
  background-color: #fafafa;
}

.wishlist__header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.wishlist__title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #111827;
  margin: 0;
}

.wishlist__count {
  font-size: 0.875rem;
  color: #6b7280;
}

.wishlist__empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 3rem 1rem;
  text-align: center;
}

.wishlist__empty-icon {
  width: 64px;
  height: 64px;
  color: #d1d5db;
  margin-bottom: 1rem;
}

.wishlist__empty-text {
  font-size: 0.875rem;
  color: #6b7280;
  margin: 0 0 1.5rem;
}

.wishlist__empty-btn {
  padding: 0.625rem 1.25rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: white;
  background-color: var(--color-primary, #3b82f6);
  border: none;
  border-radius: 6px;
  cursor: pointer;
}

.wishlist__products--grid {
  display: grid;
  grid-template-columns: repeat(var(--columns, 4), 1fr);
  gap: 1.5rem;
}

.wishlist__products--list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.wishlist__products--list .wishlist__product {
  flex-direction: row;
  align-items: flex-start;
}

.wishlist__products--list .wishlist__product-image {
  width: 120px;
  flex-shrink: 0;
}

.wishlist__product {
  display: flex;
  flex-direction: column;
  background-color: white;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  overflow: hidden;
  transition: box-shadow 0.2s;
}

.wishlist__product:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
}

.wishlist__product-image {
  position: relative;
  aspect-ratio: 1;
  background-color: #f9fafb;
}

.wishlist__product-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.wishlist__remove-btn {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: white;
  border: 1px solid #e5e7eb;
  border-radius: 50%;
  cursor: pointer;
  opacity: 0;
  transition:
    opacity 0.2s,
    background-color 0.2s;
}

.wishlist__product:hover .wishlist__remove-btn {
  opacity: 1;
}

.wishlist__remove-btn:hover:not(:disabled) {
  background-color: #fee2e2;
  border-color: #fca5a5;
}

.wishlist__remove-btn:disabled {
  cursor: not-allowed;
}

.wishlist__remove-btn svg {
  width: 14px;
  height: 14px;
  color: #6b7280;
}

.wishlist__remove-btn:hover:not(:disabled) svg {
  color: #dc2626;
}

.wishlist__product-info {
  padding: 1rem;
  display: flex;
  flex-direction: column;
  flex: 1;
}

.wishlist__product-name {
  font-size: 0.875rem;
  font-weight: 500;
  color: #111827;
  margin: 0 0 0.25rem;
  line-height: 1.4;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.wishlist__product-sku {
  font-size: 0.75rem;
  color: #9ca3af;
  margin: 0 0 0.75rem;
}

.wishlist__product-price {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
}

.wishlist__price-current {
  font-size: 1rem;
  font-weight: 600;
  color: #111827;
}

.wishlist__price-original {
  font-size: 0.875rem;
  color: #9ca3af;
  text-decoration: line-through;
}

.wishlist__product-stock {
  margin-bottom: 0.75rem;
}

.wishlist__stock-badge {
  display: inline-block;
  padding: 0.125rem 0.5rem;
  font-size: 0.625rem;
  font-weight: 600;
  text-transform: uppercase;
  border-radius: 9999px;
}

.wishlist__stock-badge--available {
  background-color: #dcfce7;
  color: #166534;
}

.wishlist__stock-badge--unavailable {
  background-color: #fee2e2;
  color: #991b1b;
}

.wishlist__product-actions {
  display: flex;
  gap: 0.5rem;
  margin-top: auto;
}

.wishlist__add-to-cart {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.375rem;
  flex: 1;
  padding: 0.5rem 0.75rem;
  font-size: 0.75rem;
  font-weight: 500;
  color: white;
  background-color: var(--color-primary, #3b82f6);
  border: none;
  border-radius: 6px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.wishlist__add-to-cart:hover:not(:disabled) {
  background-color: #2563eb;
}

.wishlist__add-to-cart:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.wishlist__add-to-cart svg {
  width: 14px;
  height: 14px;
}

.wishlist__share-btn {
  width: 36px;
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: white;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.wishlist__share-btn:hover:not(:disabled) {
  background-color: #f3f4f6;
}

.wishlist__share-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.wishlist__share-btn svg {
  width: 16px;
  height: 16px;
  color: #6b7280;
}

.wishlist__edit-hint {
  margin-top: 1rem;
  padding: 0.5rem;
  background-color: #fef3c7;
  border-radius: 4px;
  text-align: center;
  font-size: 0.75rem;
  color: #92400e;
}

@media (max-width: 768px) {
  .wishlist__products--grid {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>
