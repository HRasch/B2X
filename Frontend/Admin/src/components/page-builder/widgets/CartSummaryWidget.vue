<script setup lang="ts">
/**
 * CartSummaryWidget - Cart summary with totals and checkout button
 */
import type { CartSummaryWidgetConfig } from '@/types/widgets';

const props = defineProps<{
  config: CartSummaryWidgetConfig;
  isEditing?: boolean;
}>();

// Mock cart data for preview
const mockCart = {
  items: [
    { id: '1', name: 'Akku-Bohrschrauber Pro', quantity: 1, price: 199.99 },
    { id: '2', name: 'Bit-Set 32-teilig', quantity: 2, price: 29.95 },
    { id: '3', name: 'Werkzeugkoffer leer', quantity: 1, price: 49.00 },
  ],
  subtotal: 308.89,
  shipping: 5.95,
  tax: 58.69,
  total: 373.53,
};

const itemCount = mockCart.items.reduce((sum, item) => sum + item.quantity, 0);
</script>

<template>
  <div class="cart-summary">
    <h2 class="cart-summary__title">{{ config.title }}</h2>

    <!-- Empty State -->
    <div v-if="mockCart.items.length === 0" class="cart-summary__empty">
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
        <circle cx="9" cy="21" r="1" />
        <circle cx="20" cy="21" r="1" />
        <path d="M1 1h4l2.68 13.39a2 2 0 002 1.61h9.72a2 2 0 002-1.61L23 6H6" />
      </svg>
      <p>{{ config.emptyCartMessage }}</p>
    </div>

    <template v-else>
      <!-- Item Count -->
      <div v-if="config.showItemCount" class="cart-summary__row cart-summary__row--muted">
        <span>Artikel</span>
        <span>{{ itemCount }} Stück</span>
      </div>

      <!-- Subtotal -->
      <div v-if="config.showSubtotal" class="cart-summary__row">
        <span>Zwischensumme</span>
        <span>{{ mockCart.subtotal.toFixed(2) }} €</span>
      </div>

      <!-- Shipping -->
      <div v-if="config.showShipping" class="cart-summary__row">
        <span>Versand</span>
        <span v-if="mockCart.shipping > 0">{{ mockCart.shipping.toFixed(2) }} €</span>
        <span v-else class="cart-summary__free">Kostenlos</span>
      </div>

      <!-- Tax -->
      <div v-if="config.showTax" class="cart-summary__row cart-summary__row--muted">
        <span>inkl. MwSt.</span>
        <span>{{ mockCart.tax.toFixed(2) }} €</span>
      </div>

      <!-- Promo Code -->
      <div v-if="config.showPromoCode" class="cart-summary__promo">
        <input type="text" placeholder="Gutscheincode" class="cart-summary__promo-input" />
        <button class="cart-summary__promo-btn">Einlösen</button>
      </div>

      <!-- Total -->
      <div v-if="config.showTotal" class="cart-summary__row cart-summary__row--total">
        <span>Gesamtsumme</span>
        <span>{{ mockCart.total.toFixed(2) }} €</span>
      </div>

      <!-- Actions -->
      <div class="cart-summary__actions">
        <button v-if="config.showCheckoutButton" class="cart-summary__checkout-btn">
          {{ config.checkoutButtonText }}
        </button>
        <a
          v-if="config.showContinueShopping"
          :href="config.continueShoppingUrl"
          class="cart-summary__continue"
        >
          {{ config.continueShoppingText }}
        </a>
      </div>

      <!-- Trust Badges -->
      <div class="cart-summary__trust">
        <div class="cart-summary__trust-item">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z" />
          </svg>
          <span>SSL-verschlüsselt</span>
        </div>
        <div class="cart-summary__trust-item">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <span>Käuferschutz</span>
        </div>
      </div>
    </template>

    <!-- Edit Mode Info -->
    <div v-if="isEditing" class="cart-summary__edit-info">
      <span>Vorschau mit Beispieldaten</span>
    </div>
  </div>
</template>

<style scoped>
.cart-summary {
  position: relative;
  padding: 1.5rem;
  background: white;
  border-radius: 0.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.cart-summary__title {
  margin: 0 0 1.5rem;
  font-size: 1.25rem;
  font-weight: 600;
  color: #111827;
}

.cart-summary__empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 2rem;
  text-align: center;
  color: #6b7280;
}

.cart-summary__empty svg {
  width: 3rem;
  height: 3rem;
  margin-bottom: 1rem;
  color: #d1d5db;
}

.cart-summary__row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 0;
  border-bottom: 1px solid #e5e7eb;
  font-size: 0.9375rem;
  color: #374151;
}

.cart-summary__row--muted {
  color: #6b7280;
  font-size: 0.875rem;
}

.cart-summary__row--total {
  border-bottom: none;
  padding-top: 1rem;
  font-size: 1.125rem;
  font-weight: 600;
  color: #111827;
}

.cart-summary__free {
  color: #059669;
  font-weight: 500;
}

.cart-summary__promo {
  display: flex;
  gap: 0.5rem;
  margin: 1rem 0;
}

.cart-summary__promo-input {
  flex: 1;
  padding: 0.625rem 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  font-size: 0.875rem;
}

.cart-summary__promo-input:focus {
  outline: none;
  border-color: #3b82f6;
}

.cart-summary__promo-btn {
  padding: 0.625rem 1rem;
  background: #f3f4f6;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: background 0.2s;
}

.cart-summary__promo-btn:hover {
  background: #e5e7eb;
}

.cart-summary__actions {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin-top: 1.5rem;
}

.cart-summary__checkout-btn {
  width: 100%;
  padding: 1rem;
  background: #3b82f6;
  color: white;
  border: none;
  border-radius: 0.5rem;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s;
}

.cart-summary__checkout-btn:hover {
  background: #2563eb;
}

.cart-summary__continue {
  display: block;
  text-align: center;
  color: #3b82f6;
  text-decoration: none;
  font-size: 0.9375rem;
}

.cart-summary__continue:hover {
  text-decoration: underline;
}

.cart-summary__trust {
  display: flex;
  justify-content: center;
  gap: 1.5rem;
  margin-top: 1.5rem;
  padding-top: 1rem;
  border-top: 1px solid #e5e7eb;
}

.cart-summary__trust-item {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  font-size: 0.75rem;
  color: #6b7280;
}

.cart-summary__trust-item svg {
  width: 1rem;
  height: 1rem;
  color: #059669;
}

.cart-summary__edit-info {
  position: absolute;
  top: 0.5rem;
  right: 0.5rem;
  padding: 0.25rem 0.5rem;
  background: #dbeafe;
  color: #1e40af;
  font-size: 0.75rem;
  border-radius: 0.25rem;
}
</style>
