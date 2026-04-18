<script setup lang="ts">
/**
 * MiniCartWidget - Compact cart for header with dropdown/slide-out
 */
import { ref, computed } from 'vue';
import type { MiniCartWidgetConfig } from '@/types/widgets';

const props = defineProps<{
  config: MiniCartWidgetConfig;
  isEditing?: boolean;
}>();

const isOpen = ref(props.isEditing || false);

// Mock cart data for preview
const mockCart = {
  items: [
    {
      id: '1',
      name: 'Akku-Bohrschrauber Pro 18V',
      quantity: 1,
      price: 199.99,
      image: 'https://placehold.co/60x60/e2e8f0/475569?text=1',
    },
    {
      id: '2',
      name: 'Bit-Set 32-teilig Professional',
      quantity: 2,
      price: 29.95,
      image: 'https://placehold.co/60x60/e2e8f0/475569?text=2',
    },
    {
      id: '3',
      name: 'Werkzeugkoffer Aluminium',
      quantity: 1,
      price: 49.0,
      image: 'https://placehold.co/60x60/e2e8f0/475569?text=3',
    },
  ],
  total: 308.89,
};

const itemCount = computed(() => mockCart.items.reduce((sum, item) => sum + item.quantity, 0));
const displayItems = computed(() => mockCart.items.slice(0, props.config.maxItemsPreview));
const hasMoreItems = computed(() => mockCart.items.length > props.config.maxItemsPreview);

const triggerIcons: Record<string, string> = {
  cart: 'M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z',
  bag: 'M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z',
  basket:
    'M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z',
};

const iconPath = computed(() => triggerIcons[props.config.triggerIcon] || triggerIcons.cart);

const positionClass = computed(() => {
  return props.config.position === 'slide-out' ? 'mini-cart--slide-out' : 'mini-cart--dropdown';
});

const toggle = () => {
  isOpen.value = !isOpen.value;
};
</script>

<template>
  <div :class="['mini-cart', positionClass]">
    <!-- Trigger Button -->
    <button class="mini-cart__trigger" @click="toggle">
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <path :d="iconPath" />
      </svg>
      <span v-if="config.showItemCount && itemCount > 0" class="mini-cart__badge">
        {{ itemCount }}
      </span>
      <span v-if="config.showTotal" class="mini-cart__total">
        {{ mockCart.total.toFixed(2) }} €
      </span>
    </button>

    <!-- Dropdown/Panel -->
    <div v-if="isOpen || isEditing" class="mini-cart__panel">
      <div class="mini-cart__header">
        <h3 class="mini-cart__title">{{ $t('pageBuilder.miniCart.title') }}</h3>
        <button class="mini-cart__close" @click="isOpen = false">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      <!-- Empty State -->
      <div v-if="mockCart.items.length === 0" class="mini-cart__empty">
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <circle cx="9" cy="21" r="1" />
          <circle cx="20" cy="21" r="1" />
          <path d="M1 1h4l2.68 13.39a2 2 0 002 1.61h9.72a2 2 0 002-1.61L23 6H6" />
        </svg>
        <p>{{ config.emptyCartMessage }}</p>
      </div>

      <!-- Items -->
      <div v-else class="mini-cart__items">
        <div v-for="item in displayItems" :key="item.id" class="mini-cart__item">
          <img :src="item.image" :alt="item.name" class="mini-cart__item-image" />
          <div class="mini-cart__item-info">
            <span class="mini-cart__item-name">{{ item.name }}</span>
            <span class="mini-cart__item-meta"
              >{{ item.quantity }} × {{ item.price.toFixed(2) }} €</span
            >
          </div>
          <button class="mini-cart__item-remove" title="Entfernen">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div v-if="hasMoreItems" class="mini-cart__more">
          + {{ mockCart.items.length - config.maxItemsPreview }} weitere Artikel
        </div>
      </div>

      <!-- Footer -->
      <div v-if="mockCart.items.length > 0" class="mini-cart__footer">
        <div class="mini-cart__subtotal">
          <span>{{ $t('pageBuilder.miniCart.subtotal') }}</span>
          <span class="mini-cart__subtotal-value">{{ mockCart.total.toFixed(2) }} €</span>
        </div>

        <div class="mini-cart__actions">
          <a v-if="config.showViewCartButton" href="/cart" class="mini-cart__view-btn">
            {{ config.viewCartButtonText }}
          </a>
          <button v-if="config.showCheckoutButton" class="mini-cart__checkout-btn">
            {{ config.checkoutButtonText }}
          </button>
        </div>
      </div>
    </div>

    <!-- Overlay for slide-out -->
    <div
      v-if="isOpen && config.position === 'slide-out'"
      class="mini-cart__overlay"
      @click="isOpen = false"
    />
  </div>
</template>

<style scoped>
.mini-cart {
  position: relative;
}

.mini-cart__trigger {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem;
  background: none;
  border: none;
  cursor: pointer;
  position: relative;
}

.mini-cart__trigger svg {
  width: 1.5rem;
  height: 1.5rem;
  color: #374151;
}

.mini-cart__badge {
  position: absolute;
  top: 0;
  right: 0;
  min-width: 1.25rem;
  height: 1.25rem;
  padding: 0 0.25rem;
  background: #ef4444;
  color: white;
  font-size: 0.75rem;
  font-weight: 600;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.mini-cart__total {
  font-size: 0.875rem;
  font-weight: 500;
  color: #111827;
}

/* Dropdown Panel */
.mini-cart--dropdown .mini-cart__panel {
  position: absolute;
  top: 100%;
  right: 0;
  width: 360px;
  max-height: 480px;
  background: white;
  border-radius: 0.5rem;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.15);
  z-index: 50;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

/* Slide-out Panel */
.mini-cart--slide-out .mini-cart__panel {
  position: fixed;
  top: 0;
  right: 0;
  width: 400px;
  max-width: 90vw;
  height: 100vh;
  background: white;
  box-shadow: -4px 0 20px rgba(0, 0, 0, 0.15);
  z-index: 100;
  display: flex;
  flex-direction: column;
}

.mini-cart__overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  z-index: 99;
}

.mini-cart__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem;
  border-bottom: 1px solid #e5e7eb;
}

.mini-cart__title {
  margin: 0;
  font-size: 1.125rem;
  font-weight: 600;
  color: #111827;
}

.mini-cart__close {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 2rem;
  height: 2rem;
  background: none;
  border: none;
  cursor: pointer;
  color: #6b7280;
  border-radius: 0.375rem;
  transition: background 0.2s;
}

.mini-cart__close:hover {
  background: #f3f4f6;
}

.mini-cart__close svg {
  width: 1.25rem;
  height: 1.25rem;
}

.mini-cart__empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem;
  text-align: center;
  color: #6b7280;
}

.mini-cart__empty svg {
  width: 3rem;
  height: 3rem;
  margin-bottom: 1rem;
  color: #d1d5db;
}

.mini-cart__items {
  flex: 1;
  overflow-y: auto;
  padding: 0.5rem;
}

.mini-cart__item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.75rem;
  border-radius: 0.375rem;
  transition: background 0.2s;
}

.mini-cart__item:hover {
  background: #f9fafb;
}

.mini-cart__item-image {
  width: 3.5rem;
  height: 3.5rem;
  border-radius: 0.375rem;
  object-fit: cover;
  background: #f3f4f6;
}

.mini-cart__item-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  min-width: 0;
}

.mini-cart__item-name {
  font-size: 0.875rem;
  font-weight: 500;
  color: #111827;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.mini-cart__item-meta {
  font-size: 0.75rem;
  color: #6b7280;
}

.mini-cart__item-remove {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 1.5rem;
  height: 1.5rem;
  background: none;
  border: none;
  cursor: pointer;
  color: #9ca3af;
  opacity: 0;
  transition:
    opacity 0.2s,
    color 0.2s;
}

.mini-cart__item:hover .mini-cart__item-remove {
  opacity: 1;
}

.mini-cart__item-remove:hover {
  color: #ef4444;
}

.mini-cart__item-remove svg {
  width: 0.875rem;
  height: 0.875rem;
}

.mini-cart__more {
  padding: 0.75rem;
  text-align: center;
  font-size: 0.875rem;
  color: #6b7280;
  background: #f9fafb;
  border-radius: 0.375rem;
  margin-top: 0.5rem;
}

.mini-cart__footer {
  padding: 1rem;
  border-top: 1px solid #e5e7eb;
  background: #f9fafb;
}

.mini-cart__subtotal {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
  font-size: 0.9375rem;
}

.mini-cart__subtotal-value {
  font-weight: 600;
  color: #111827;
}

.mini-cart__actions {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.mini-cart__view-btn {
  display: block;
  padding: 0.75rem 1rem;
  text-align: center;
  color: #374151;
  text-decoration: none;
  background: white;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  font-weight: 500;
  transition: background 0.2s;
}

.mini-cart__view-btn:hover {
  background: #f3f4f6;
}

.mini-cart__checkout-btn {
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

.mini-cart__checkout-btn:hover {
  background: #2563eb;
}
</style>
