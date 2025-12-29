<script setup lang="ts">
import { ref, computed } from "vue";
import { useRouter } from "vue-router";
import { useCartStore } from "@/stores/cartStore";

const router = useRouter();
const cartStore = useCartStore();

const vatRate = 0.19; // 19% VAT

// Computed
const subtotal = computed(() => {
  return cartStore.items.reduce(
    (sum, item) => sum + item.price * item.quantity,
    0
  );
});

const vatAmount = computed(() => {
  return subtotal.value * vatRate.value;
});

const total = computed(() => {
  return subtotal.value + vatAmount.value;
});

const itemCount = computed(() => {
  return cartStore.items.reduce((sum, item) => sum + item.quantity, 0);
});

const isEmpty = computed(() => cartStore.items.length === 0);

// Methods
const updateQuantity = (itemId: string, quantity: number) => {
  if (quantity <= 0) {
    removeItem(itemId);
  } else {
    cartStore.updateQuantity(itemId, quantity);
  }
};

const removeItem = (itemId: string) => {
  cartStore.removeItem(itemId);
};

const proceedToCheckout = () => {
  router.push("/checkout");
};

const continueShopping = () => {
  router.push("/products");
};

const applyCoupon = () => {
  // TODO: Implement coupon logic
  console.log("Apply coupon");
};
</script>

<template>
  <div class="min-h-screen bg-base-100">
    <!-- Header -->
    <header class="bg-primary text-primary-content py-6 px-4">
      <div class="max-w-7xl mx-auto">
        <h1 class="text-3xl font-bold">Shopping Cart</h1>
        <p class="text-primary-content/90">
          {{ itemCount }} item(s) in your cart
        </p>
      </div>
    </header>

    <div class="max-w-7xl mx-auto px-4 py-8">
      <!-- Empty Cart -->
      <div v-if="isEmpty" class="card bg-base-200 shadow-sm">
        <div class="card-body text-center py-12">
          <div class="text-6xl mb-4">🛒</div>
          <h2 class="card-title text-2xl mb-2 justify-center">
            Your cart is empty
          </h2>
          <p class="text-base-content/70 mb-6">
            Discover amazing products and add them to your cart.
          </p>
          <button @click="continueShopping" class="btn btn-primary w-full">
            Continue Shopping
          </button>
        </div>
      </div>

      <!-- Cart Content -->
      <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Cart Items -->
        <div class="lg:col-span-2">
          <div class="card bg-base-200 shadow-sm overflow-hidden">
            <div class="overflow-x-auto">
              <table class="table w-full">
                <thead>
                  <tr class="border-b border-base-300">
                    <th>Product</th>
                    <th>Price</th>
                    <th>Quantity</th>
                    <th>Total</th>
                    <th></th>
                  </tr>
                </thead>
                <tbody>
                  <tr
                    v-for="item in cartStore.items"
                    :key="item.id"
                    class="border-b border-base-300"
                  >
                    <!-- Product Info -->
                    <td>
                      <div class="flex gap-4 items-center">
                        <div
                          class="w-16 h-16 bg-base-300 rounded-lg overflow-hidden flex-shrink-0"
                        >
                          <img
                            :src="item.image"
                            :alt="item.name"
                            class="w-full h-full object-cover"
                          />
                        </div>
                        <div>
                          <router-link
                            :to="`/product/${item.id}`"
                            class="font-semibold hover:text-primary transition-colors"
                          >
                            {{ item.name }}
                          </router-link>
                          <p class="text-sm text-base-content/70">
                            SKU: ABC-123
                          </p>
                        </div>
                      </div>
                    </td>

                    <!-- Price -->
                    <td class="font-semibold">€{{ item.price.toFixed(2) }}</td>

                    <!-- Quantity -->
                    <td>
                      <div
                        class="flex items-center border border-base-300 rounded-lg w-fit"
                      >
                        <button
                          @click="updateQuantity(item.id, item.quantity - 1)"
                          class="btn btn-ghost btn-sm rounded-none px-3"
                        >
                          −
                        </button>
                        <input
                          :value="item.quantity"
                          @input="
                            (e) =>
                              updateQuantity(
                                item.id,
                                parseInt(
                                  (e.target as HTMLInputElement).value
                                ) || 1
                              )
                          "
                          type="number"
                          min="1"
                          class="input input-ghost w-12 text-center no-spinner"
                        />
                        <button
                          @click="updateQuantity(item.id, item.quantity + 1)"
                          class="btn btn-ghost btn-sm rounded-none px-3"
                        >
                          +
                        </button>
                      </div>
                    </td>

                    <!-- Total -->
                    <td class="font-semibold">
                      €{{ (item.price * item.quantity).toFixed(2) }}
                    </td>

                    <!-- Remove -->
                    <td>
                      <button
                        @click="removeItem(item.id)"
                        class="btn btn-ghost btn-sm btn-circle"
                        title="Remove from cart"
                      >
                        ✕
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- Continue Shopping -->
          <button @click="continueShopping" class="btn btn-ghost w-full mt-4">
            ← Continue Shopping
          </button>
        </div>

        <!-- Order Summary -->
        <div class="lg:col-span-1">
          <div class="card bg-base-200 shadow-sm sticky top-4">
            <div class="card-body">
              <h3 class="card-title text-lg mb-4">Order Summary</h3>

              <!-- Coupon Section -->
              <div class="form-control mb-4">
                <label class="label">
                  <span class="label-text text-sm">Have a coupon code?</span>
                </label>
                <div class="input-group">
                  <input
                    type="text"
                    placeholder="Enter coupon code"
                    class="input input-bordered input-sm w-full"
                    disabled
                  />
                  <button
                    @click="applyCoupon"
                    class="btn btn-ghost btn-sm"
                    disabled
                  >
                    Apply
                  </button>
                </div>
              </div>

              <!-- Pricing Breakdown -->
              <div class="space-y-3">
                <!-- Subtotal -->
                <div class="flex justify-between">
                  <span class="text-base-content/70">Subtotal</span>
                  <span class="font-semibold">€{{ subtotal.toFixed(2) }}</span>
                </div>

                <!-- Shipping (TODO: Dynamic) -->
                <div class="flex justify-between">
                  <span class="text-base-content/70">Shipping</span>
                  <span class="font-semibold">FREE</span>
                </div>

                <!-- Divider -->
                <div class="divider my-2"></div>

                <!-- Net (before VAT) -->
                <div class="flex justify-between">
                  <span class="text-base-content/70"
                    >Net Price (excl. VAT)</span
                  >
                  <span class="font-semibold">€{{ subtotal.toFixed(2) }}</span>
                </div>

                <!-- VAT -->
                <div class="flex justify-between">
                  <span class="text-base-content/70">VAT (19%)</span>
                  <span class="font-semibold text-success"
                    >€{{ vatAmount.toFixed(2) }}</span
                  >
                </div>

                <!-- Divider -->
                <div class="divider my-2"></div>

                <!-- Total -->
                <div class="flex justify-between items-center text-lg">
                  <span class="font-bold">Total (incl. VAT)</span>
                  <span class="text-2xl font-bold text-primary"
                    >€{{ total.toFixed(2) }}</span
                  >
                </div>
              </div>

              <!-- Checkout Button -->
              <button
                @click="proceedToCheckout"
                class="btn btn-primary w-full mt-6"
              >
                Proceed to Checkout →
              </button>

              <!-- Guest Checkout -->
              <button class="btn btn-ghost w-full mt-2">
                Continue as Guest
              </button>

              <!-- Security Badge -->
              <div
                class="flex items-center justify-center gap-2 mt-4 pt-4 border-t border-base-300"
              >
                <span class="text-xs text-base-content/70"
                  >🔒 Secure Checkout</span
                >
              </div>
            </div>
          </div>

          <!-- Trust Badges -->
          <div class="space-y-2 mt-4">
            <div class="card bg-base-200 shadow-sm">
              <div class="card-body py-3 px-4">
                <p class="text-xs text-center text-base-content/70">
                  ✓ 30-day money-back guarantee
                </p>
              </div>
            </div>
            <div class="card bg-base-200 shadow-sm">
              <div class="card-body py-3 px-4">
                <p class="text-xs text-center text-base-content/70">
                  ✓ Free returns & exchanges
                </p>
              </div>
            </div>
            <div class="card bg-base-200 shadow-sm">
              <div class="card-body py-3 px-4">
                <p class="text-xs text-center text-base-content/70">
                  ✓ Secure SSL encrypted checkout
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Remove number input spinner */
.no-spinner::-webkit-outer-spin-button,
.no-spinner::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

.no-spinner {
  -moz-appearance: textfield;
}
</style>
