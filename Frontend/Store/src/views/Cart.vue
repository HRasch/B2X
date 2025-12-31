<template>
  <div class="min-h-screen bg-base-100">
    <!-- Breadcrumbs -->
    <div class="max-w-7xl mx-auto px-4 pt-8">
      <BaseBreadcrumb
        :items="[
          { label: 'Home', to: '/' },
          { label: 'Shop', to: '/shop' },
          { label: 'Cart' }
        ]"
      />
    </div>

    <!-- Page Header -->
    <div class="bg-base-200 py-8">
      <div class="max-w-7xl mx-auto px-4">
        <h1 class="text-4xl font-bold text-base-content">Shopping Cart</h1>
        <p class="text-base-content/70 mt-2">
          {{ cartStore.items.length }} item{{ cartStore.items.length !== 1 ? 's' : '' }} in your cart
        </p>
      </div>
    </div>

    <div class="max-w-7xl mx-auto px-4 py-8">
      <!-- Empty Cart State -->
      <div v-if="cartStore.items.length === 0" class="text-center py-16">
        <div class="max-w-md mx-auto">
          <div class="text-8xl mb-6">🛒</div>
          <h2 class="text-3xl font-bold text-base-content mb-4">Your cart is empty</h2>
          <p class="text-base-content/70 mb-8">
            Looks like you haven't added any items to your cart yet.
          </p>
          <BaseButton variant="primary" size="lg" to="/shop">
            Continue Shopping
          </BaseButton>
        </div>
      </div>

      <!-- Cart Content -->
      <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Cart Items -->
        <div class="lg:col-span-2 space-y-4">
          <div
            v-for="item in cartStore.items"
            :key="item.id"
            class="card bg-base-100 shadow-sm border border-base-300"
          >
            <div class="card-body p-6">
              <div class="flex flex-col sm:flex-row gap-4">
                <!-- Product Image -->
                <div class="flex-shrink-0">
                  <img
                    :src="item.image"
                    :alt="item.name"
                    class="w-24 h-24 object-cover rounded-lg"
                    loading="lazy"
                  />
                </div>

                <!-- Product Details -->
                <div class="flex-1 min-w-0">
                  <h3 class="text-lg font-semibold text-base-content mb-2">
                    {{ item.name }}
                  </h3>
                  <p class="text-base-content/70 text-sm mb-3">
                    Unit Price: €{{ item.price.toFixed(2) }}
                  </p>

                  <!-- Quantity Controls -->
                  <div class="flex items-center gap-3">
                    <div class="flex items-center border border-base-300 rounded-lg">
                      <BaseButton
                        variant="ghost"
                        size="sm"
                        @click="decreaseQuantity(item.id)"
                        :disabled="item.quantity <= 1"
                        class="btn-square"
                        aria-label="Decrease quantity"
                      >
                        <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M3 10a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z" clip-rule="evenodd" />
                        </svg>
                      </BaseButton>

                      <span class="px-4 py-2 text-center min-w-[3rem] font-medium">
                        {{ item.quantity }}
                      </span>

                      <BaseButton
                        variant="ghost"
                        size="sm"
                        @click="increaseQuantity(item.id)"
                        class="btn-square"
                        aria-label="Increase quantity"
                      >
                        <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z" clip-rule="evenodd" />
                        </svg>
                      </BaseButton>
                    </div>

                    <!-- Remove Button -->
                    <BaseButton
                      variant="ghost"
                      size="sm"
                      @click="removeItem(item.id)"
                      class="text-error hover:bg-error/10"
                      aria-label="Remove item from cart"
                    >
                      <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M9 2a1 1 0 00-.894.553L7.382 4H4a1 1 0 000 2v10a2 2 0 002 2h8a2 2 0 002-2V6a1 1 0 100-2h-3.382l-.724-1.447A1 1 0 0011 2H9zM7 8a1 1 0 012 0v6a1 1 0 11-2 0V8zm5-1a1 1 0 00-1 1v6a1 1 0 102 0V8a1 1 0 00-1-1z" clip-rule="evenodd" />
                      </svg>
                    </BaseButton>
                  </div>
                </div>

                <!-- Item Total -->
                <div class="text-right">
                  <div class="text-xl font-bold text-base-content">
                    €{{ (item.price * item.quantity).toFixed(2) }}
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Order Summary -->
        <div class="lg:col-span-1">
          <div class="card bg-base-100 shadow-sm border border-base-300 sticky top-4">
            <div class="card-body">
              <h2 class="card-title text-xl">Order Summary</h2>

              <div class="space-y-3">
                <div class="flex justify-between">
                  <span class="text-base-content/70">Subtotal</span>
                  <span class="font-medium">€{{ subtotal.toFixed(2) }}</span>
                </div>

                <div class="flex justify-between">
                  <span class="text-base-content/70">Tax (19%)</span>
                  <span class="font-medium">€{{ tax.toFixed(2) }}</span>
                </div>

                <!-- Shipping -->
                <div class="divider my-2"></div>
                <div class="space-y-2">
                  <label class="label">
                    <span class="label-text font-medium">Shipping Method</span>
                  </label>
                  <BaseSelect
                    v-model="selectedShipping"
                    :options="shippingOptions"
                    placeholder="Select shipping method"
                    size="sm"
                  />
                </div>

                <div class="flex justify-between">
                  <span class="text-base-content/70">Shipping</span>
                  <span class="font-medium">€{{ shippingCostDisplay.toFixed(2) }}</span>
                </div>

                <div class="divider my-2"></div>

                <div class="flex justify-between text-lg font-bold">
                  <span>Total</span>
                  <span class="text-primary">€{{ total.toFixed(2) }}</span>
                </div>
              </div>

              <div class="card-actions mt-6">
                <BaseButton
                  variant="primary"
                  size="lg"
                  to="/checkout"
                  class="w-full"
                  :disabled="cartStore.items.length === 0"
                >
                  Proceed to Checkout
                </BaseButton>

                <BaseButton
                  variant="outline"
                  size="md"
                  to="/shop"
                  class="w-full"
                >
                  Continue Shopping
                </BaseButton>
              </div>
            </div>
          </div>
        </div>
      </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, onMounted, watch } from "vue";
import { useRouter } from "vue-router";
import { useCartStore } from "../stores/cart";
import BaseButton from "@/components/ui/BaseButton.vue";
import BaseSelect from "@/components/ui/BaseSelect.vue";
import BaseBreadcrumb from "@/components/ui/BaseBreadcrumb.vue";

interface ShippingMethod {
  id: string;
  name: string;
  provider: string;
  cost: number;
  description: string;
  estimatedDaysMin: number;
  estimatedDaysMax: number;
}

const router = useRouter();
const cartStore = useCartStore();

const selectedCountry = ref("DE");
const selectedShippingMethodId = ref("");
const selectedShipping = ref("standard");
const shippingMethods = ref<ShippingMethod[]>([]);
const shippingCost = ref(5.99);
const shippingError = ref("");

const shippingCostDisplay = computed(() => {
  const costs = {
    standard: 5.99,
    express: 12.99,
    overnight: 24.99,
  };
  return costs[selectedShipping.value as keyof typeof costs] || 0;
});

watch(selectedShipping, (newValue) => {
  const costs = {
    standard: 5.99,
    express: 12.99,
    overnight: 24.99,
  };
  shippingCost.value = costs[newValue as keyof typeof costs] || 0;
});

const shippingOptions = [
  { value: "standard", label: "Standard Shipping (3-5 days) - €5.99" },
  { value: "express", label: "Express Shipping (1-2 days) - €12.99" },
  { value: "overnight", label: "Overnight Shipping - €24.99" },
];

const subtotal = computed(() => {
  return cartStore.items.reduce(
    (sum, item) => sum + item.price * item.quantity,
    0
  );
});

const tax = computed(() => subtotal.value * 0.19);

const total = computed(() => subtotal.value + tax.value + shippingCostDisplay.value);

const increaseQuantity = (itemId: string) => {
  const item = cartStore.items.find((i) => i.id === itemId);
  if (item) {
    item.quantity++;
  }
};

const decreaseQuantity = (itemId: string) => {
  const item = cartStore.items.find((i) => i.id === itemId);
  if (item && item.quantity > 1) {
    item.quantity--;
  }
};

const removeItem = (itemId: string) => {
  cartStore.removeItem(itemId);
};

const onCountryChange = async () => {
  shippingMethods.value = [];
  selectedShippingMethodId.value = "";
  shippingCost.value = 0;
  shippingError.value = "";

  if (!selectedCountry.value) {
    return;
  }

  try {
    const response = await fetch(
      `http://localhost:7005/api/cart/shipping-methods?destinationCountry=${selectedCountry.value}`
    );

    if (!response.ok) {
      shippingError.value = "Versand zu diesem Land nicht verfügbar";
      return;
    }

    const data = await response.json();

    if (data.success && data.methods) {
      shippingMethods.value = data.methods;
      if (shippingMethods.value.length > 0) {
        selectedShippingMethodId.value = shippingMethods.value[0].id;
        const method = shippingMethods.value[0];
        shippingCost.value = method.cost;
      }
    } else {
      shippingError.value =
        data.message || "Fehler beim Laden der Versandarten";
    }
  } catch (error) {
    console.error("Error fetching shipping methods:", error);
    shippingError.value = "Fehler beim Laden der Versandarten";
  }
};

const onShippingChange = () => {
  const selected = shippingMethods.value.find(
    (m) => m.id === selectedShippingMethodId.value
  );
  if (selected) {
    shippingCost.value = selected.cost;
  }
};

const checkout = () => {
  if (!selectedShippingMethodId.value) {
    shippingError.value = "Bitte wählen Sie eine Versandart aus";
    return;
  }
  router.push("/checkout");
};

onMounted(async () => {
  if (selectedCountry.value) {
    await onCountryChange();
  }
});
</script>

<style scoped>
.cart-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
}

.cart-container h1 {
  font-size: 2rem;
  margin-bottom: 2rem;
  color: #333;
}

.empty-cart {
  text-align: center;
  padding: 3rem;
  background-color: #f9f9f9;
  border-radius: 8px;
}

.empty-cart p {
  font-size: 1.2rem;
  color: #666;
  margin-bottom: 1.5rem;
}

.continue-shopping {
  display: inline-block;
  padding: 0.75rem 1.5rem;
  background-color: #2563eb;
  color: white;
  text-decoration: none;
  border-radius: 8px;
  transition: background-color 0.3s;
}

.continue-shopping:hover {
  background-color: #1d4ed8;
}

.cart-content {
  display: grid;
  grid-template-columns: 1fr 300px;
  gap: 2rem;
}

.cart-items {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.cart-item {
  display: grid;
  grid-template-columns: 100px 1fr auto auto auto;
  gap: 1rem;
  align-items: center;
  padding: 1rem;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  background-color: white;
}

.item-image {
  width: 100px;
  height: 100px;
  object-fit: cover;
  border-radius: 4px;
}

.item-details h3 {
  margin: 0 0 0.5rem 0;
  color: #333;
}

.item-details p {
  margin: 0;
  color: #666;
  font-size: 0.9rem;
}

.item-quantity {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.item-quantity button {
  width: 30px;
  height: 30px;
  padding: 0;
  border: 1px solid #e0e0e0;
  background-color: white;
  border-radius: 4px;
  cursor: pointer;
  font-weight: bold;
}

.item-quantity button:hover {
  background-color: #f5f5f5;
}

.item-quantity input {
  width: 50px;
  text-align: center;
  padding: 0.4rem;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
}

.item-subtotal {
  font-weight: 600;
  color: #2563eb;
  min-width: 70px;
  text-align: right;
}

.remove-btn {
  padding: 0.5rem;
  background-color: #f3f4f6;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  cursor: pointer;
  color: #ef4444;
  font-weight: bold;
  transition: background-color 0.3s;
}

.remove-btn:hover {
  background-color: #fee2e2;
}

.cart-summary {
  background-color: #f9f9f9;
  padding: 1.5rem;
  border-radius: 8px;
  height: fit-content;
  position: sticky;
  top: 20px;
}

.cart-summary h2 {
  margin: 0 0 1rem 0;
  font-size: 1.2rem;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  padding: 0.75rem 0;
  border-bottom: 1px solid #e0e0e0;
}

.summary-row.total {
  font-size: 1.1rem;
  font-weight: 700;
  border-bottom: none;
  margin-top: 0.5rem;
  padding-top: 1rem;
  color: #2563eb;
}

.checkout-btn {
  width: 100%;
  padding: 1rem;
  background-color: #059669;
  color: white;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  margin-top: 1rem;
  transition: background-color 0.3s;
}

.checkout-btn:hover {
  background-color: #047857;
}

.cart-summary .continue-shopping {
  display: block;
  text-align: center;
  margin-top: 1rem;
  padding: 0.75rem;
}

@media (max-width: 768px) {
  .cart-content {
    grid-template-columns: 1fr;
  }

  .cart-item {
    grid-template-columns: 80px 1fr;
    gap: 0.75rem;
  }

  .item-image {
    width: 80px;
    height: 80px;
  }

  .item-quantity,
  .item-subtotal,
  .remove-btn {
    grid-column: 2;
  }

  .cart-summary {
    position: static;
  }
}

/* Shipping Styles */
.shipping-section {
  margin: 2rem 0;
  padding: 1.5rem;
  background-color: #f9f9f9;
  border-radius: 8px;
  border-left: 4px solid #2563eb;
}

.shipping-section h3 {
  margin: 0 0 1rem 0;
  color: #333;
  font-size: 1.1rem;
}

.shipping-info {
  margin-bottom: 1.5rem;
}

.shipping-info label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #333;
}

.shipping-info select {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  background-color: white;
}

.shipping-methods {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin-bottom: 1rem;
}

.shipping-option {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  background-color: white;
  cursor: pointer;
  transition: all 0.2s;
}

.shipping-option:hover {
  border-color: #2563eb;
  background-color: #f0f7ff;
}

.shipping-option input {
  margin-top: 0.25rem;
  cursor: pointer;
}

.shipping-label {
  flex: 1;
  cursor: pointer;
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
}

.method-name {
  font-weight: 500;
  color: #333;
}

.method-description {
  font-size: 0.875rem;
  color: #666;
  flex: 1;
}

.method-cost {
  font-weight: 600;
  color: #2563eb;
  white-space: nowrap;
}

.shipping-error {
  padding: 0.75rem;
  background-color: #ffebee;
  border: 1px solid #ef5350;
  border-radius: 4px;
  color: #c62828;
  font-size: 0.875rem;
  margin-top: 0.5rem;
}

.shipping-row {
  background-color: #e3f2fd;
  padding: 0.75rem;
  margin-top: 1rem;
  border-radius: 4px;
}
</style>
