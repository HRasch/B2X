<script setup lang="ts">
import { computed } from 'vue';
import { useRouter } from 'vue-router';
import { useCartStore, type CartItem } from '@/stores/cart';

const router = useRouter();
const cartStore = useCartStore();

const vatRate = 0.19; // 19% VAT

// Computed
const subtotal = computed(() => {
  return cartStore.items.reduce(
    (sum: number, item: CartItem) => sum + item.price * item.quantity,
    0
  );
});

const vatAmount = computed(() => {
  return subtotal.value * vatRate;
});

const total = computed(() => {
  return subtotal.value + vatAmount.value;
});

const itemCount = computed(() => {
  return cartStore.items.reduce((sum: number, item: CartItem) => sum + item.quantity, 0);
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
  router.push('/checkout');
};

const continueShopping = () => {
  router.push('/products');
};

const applyCoupon = () => {
  // TODO: Implement coupon logic
  console.log('Apply coupon');
};
</script>

<template>
  <div class="min-h-screen bg-base-100">
    <!-- Header -->
    <header class="bg-primary text-primary-content py-6 px-4">
      <div class="max-w-7xl mx-auto">
        <h1 class="text-3xl font-bold">{{ $t('cart.title') }}</h1>
        <p class="text-primary-content/90">{{ $t('cart.itemCount', { count: itemCount }) }}</p>
      </div>
    </header>

    <div class="max-w-7xl mx-auto px-4 py-8">
      <!-- Empty Cart -->
      <div v-if="isEmpty" class="card bg-base-200 shadow-sm">
        <div class="card-body text-center py-12">
          <div class="text-6xl mb-4">🛒</div>
          <h2 class="card-title text-2xl mb-2 justify-center">{{ $t('cart.empty.title') }}</h2>
          <p class="text-base-content/70 mb-6">
            {{ $t('cart.empty.message') }}
          </p>
          <button @click="continueShopping" class="btn btn-primary w-full">
            {{ $t('cart.empty.button') }}
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
                    <th>{{ $t('cart.table.headers.product') }}</th>
                    <th>{{ $t('cart.table.headers.price') }}</th>
                    <th>{{ $t('cart.table.headers.quantity') }}</th>
                    <th>{{ $t('cart.table.headers.total') }}</th>
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
                        <div class="w-16 h-16 bg-base-300 rounded-lg overflow-hidden flex-shrink-0">
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
                            {{ $t('product.skuLabel') }} {{ item.sku || 'ABC-123' }}
                          </p>
                        </div>
                      </div>
                    </td>

                    <!-- Price -->
                    <td class="font-semibold">€{{ item.price.toFixed(2) }}</td>

                    <!-- Quantity -->
                    <td>
                      <div class="flex items-center border border-base-300 rounded-lg w-fit">
                        <button
                          @click="updateQuantity(item.id, item.quantity - 1)"
                          class="btn btn-ghost btn-sm rounded-none px-3"
                        >
                          −
                        </button>
                        <input
                          :value="item.quantity"
                          @input="
                            e =>
                              updateQuantity(
                                item.id,
                                parseInt((e.target as HTMLInputElement).value) || 1
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
                    <td class="font-semibold">€{{ (item.price * item.quantity).toFixed(2) }}</td>

                    <!-- Remove -->
                    <td>
                      <button
                        @click="removeItem(item.id)"
                        class="btn btn-ghost btn-sm btn-circle"
                        :title="$t('cart.actions.remove')"
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
            ← {{ $t('cart.actions.continueShopping') }}
          </button>
        </div>

        <!-- Order Summary -->
        <div class="lg:col-span-1">
          <div class="card bg-base-200 shadow-sm sticky top-4">
            <div class="card-body">
              <h3 class="card-title text-lg mb-4">{{ $t('cart.orderSummary.title') }}</h3>

              <!-- Coupon Section -->
              <div class="form-control mb-4">
                <label class="label">
                  <span class="label-text text-sm">{{ $t('cart.orderSummary.coupon.label') }}</span>
                </label>
                <div class="input-group">
                  <input
                    type="text"
                    :placeholder="$t('cart.orderSummary.coupon.placeholder')"
                    class="input input-bordered input-sm w-full"
                    disabled
                  />
                  <button @click="applyCoupon" class="btn btn-ghost btn-sm" disabled>
                    {{ $t('cart.orderSummary.coupon.apply') }}
                  </button>
                </div>
              </div>

              <!-- Pricing Breakdown -->
              <div class="space-y-3">
                <!-- Subtotal -->
                <div class="flex justify-between">
                  <span class="text-base-content/70">{{
                    $t('cart.orderSummary.pricing.subtotal')
                  }}</span>
                  <span class="font-semibold">€{{ subtotal.toFixed(2) }}</span>
                </div>

                <!-- Shipping (TODO: Dynamic) -->
                <div class="flex justify-between">
                  <span class="text-base-content/70">{{
                    $t('cart.orderSummary.pricing.shipping')
                  }}</span>
                  <span class="font-semibold">{{ $t('cart.orderSummary.pricing.free') }}</span>
                </div>

                <!-- Divider -->
                <div class="divider my-2"></div>

                <!-- Net (before VAT) -->
                <div class="flex justify-between">
                  <span class="text-base-content/70">{{
                    $t('cart.orderSummary.pricing.netPrice')
                  }}</span>
                  <span class="font-semibold">€{{ subtotal.toFixed(2) }}</span>
                </div>

                <!-- VAT -->
                <div class="flex justify-between">
                  <span class="text-base-content/70">{{
                    $t('cart.orderSummary.pricing.vat', { rate: '19' })
                  }}</span>
                  <span class="font-semibold text-success">€{{ vatAmount.toFixed(2) }}</span>
                </div>

                <!-- Divider -->
                <div class="divider my-2"></div>

                <!-- Total -->
                <div class="flex justify-between items-center text-lg">
                  <span class="font-bold">{{ $t('cart.orderSummary.pricing.total') }}</span>
                  <span class="text-2xl font-bold text-primary">€{{ total.toFixed(2) }}</span>
                </div>
              </div>

              <!-- Checkout Button -->
              <button @click="proceedToCheckout" class="btn btn-primary w-full mt-6">
                {{ $t('cart.checkout.button') }}
              </button>

              <!-- Guest Checkout -->
              <button class="btn btn-ghost w-full mt-2">{{ $t('cart.checkout.guest') }}</button>

              <!-- Security Badge -->
              <div
                class="flex items-center justify-center gap-2 mt-4 pt-4 border-t border-base-300"
              >
                <span class="text-xs text-base-content/70">{{ $t('cart.checkout.secure') }}</span>
              </div>
            </div>
          </div>

          <!-- Trust Badges -->
          <div class="space-y-2 mt-4">
            <div class="card bg-base-200 shadow-sm">
              <div class="card-body py-3 px-4">
                <p class="text-xs text-center text-base-content/70">
                  {{ $t('cart.trustBadges.moneyBack') }}
                </p>
              </div>
            </div>
            <div class="card bg-base-200 shadow-sm">
              <div class="card-body py-3 px-4">
                <p class="text-xs text-center text-base-content/70">
                  {{ $t('cart.trustBadges.returns') }}
                </p>
              </div>
            </div>
            <div class="card bg-base-200 shadow-sm">
              <div class="card-body py-3 px-4">
                <p class="text-xs text-center text-base-content/70">
                  {{ $t('cart.trustBadges.ssl') }}
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
