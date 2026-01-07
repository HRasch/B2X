<template>
  <div class="checkout-page">
    <div class="container mx-auto px-4 py-8">
      <!-- Progress Indicator -->
      <div class="progress-container mb-8">
        <div class="progress-steps flex justify-between">
          <div
            v-for="(step, index) in steps"
            :key="index"
            class="progress-step text-center"
            :class="{
              active: currentStepIndex === index,
              completed: stepCompletion[index],
            }"
          >
            <div
              class="step-number w-8 h-8 mx-auto mb-2 rounded-full border-2 flex items-center justify-center"
              :class="
                stepCompletion[index] && currentStepIndex > index
                  ? 'bg-green-500 border-green-500 text-white'
                  : currentStepIndex === index
                    ? 'border-blue-500 text-blue-500'
                    : 'border-gray-300 text-gray-300'
              "
            >
              {{ stepCompletion[index] && currentStepIndex > index ? '✓' : index + 1 }}
            </div>
            <div class="step-label text-sm">{{ step }}</div>
          </div>
        </div>
        <div class="progress-bar mt-4">
          <div
            class="progress-fill h-2 bg-blue-500 rounded"
            :style="{
              width: `${((currentStepIndex + 1) / steps.length) * 100}%`,
            }"
          ></div>
        </div>
      </div>

      <div class="checkout-content grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Main Checkout Form -->
        <main class="lg:col-span-2">
          <!-- Step 1: Shipping Information -->
          <div v-if="currentStepIndex === 0" class="checkout-step">
            <h2 class="text-2xl font-bold mb-6">{{ $t('checkout.steps.shipping') }}</h2>

            <form @submit.prevent="nextStep" class="space-y-6">
              <!-- Shipping Address -->
              <div class="card bg-white shadow">
                <div class="card-body">
                  <h3 class="card-title">{{ $t('checkout.shipping.address.title') }}</h3>

                  <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div class="form-control">
                      <label class="label">
                        <span class="label-text">{{
                          $t('checkout.shipping.address.firstName')
                        }}</span>
                      </label>
                      <input
                        type="text"
                        v-model="shippingAddress.firstName"
                        class="input input-bordered"
                        required
                      />
                    </div>

                    <div class="form-control">
                      <label class="label">
                        <span class="label-text">{{
                          $t('checkout.shipping.address.lastName')
                        }}</span>
                      </label>
                      <input
                        type="text"
                        v-model="shippingAddress.lastName"
                        class="input input-bordered"
                        required
                      />
                    </div>

                    <div class="form-control md:col-span-2">
                      <label class="label">
                        <span class="label-text">{{ $t('checkout.shipping.address.street') }}</span>
                      </label>
                      <input
                        type="text"
                        v-model="shippingAddress.street"
                        class="input input-bordered"
                        required
                      />
                    </div>

                    <div class="form-control">
                      <label class="label">
                        <span class="label-text">{{ $t('checkout.shipping.address.city') }}</span>
                      </label>
                      <input
                        type="text"
                        v-model="shippingAddress.city"
                        class="input input-bordered"
                        required
                      />
                    </div>

                    <div class="form-control">
                      <label class="label">
                        <span class="label-text">{{
                          $t('checkout.shipping.address.postalCode')
                        }}</span>
                      </label>
                      <input
                        type="text"
                        v-model="shippingAddress.postalCode"
                        class="input input-bordered"
                        required
                      />
                    </div>

                    <div class="form-control md:col-span-2">
                      <label class="label">
                        <span class="label-text">{{
                          $t('checkout.shipping.address.country')
                        }}</span>
                      </label>
                      <select
                        v-model="shippingAddress.country"
                        class="select select-bordered"
                        required
                      >
                        <option value="DE">Deutschland</option>
                        <option value="AT">Österreich</option>
                        <option value="CH">Schweiz</option>
                      </select>
                    </div>
                  </div>
                </div>
              </div>

              <div class="flex justify-end">
                <button type="submit" class="btn btn-primary">
                  {{ $t('checkout.actions.continue') }}
                </button>
              </div>
            </form>
          </div>

          <!-- Step 2: Payment Information -->
          <div v-if="currentStepIndex === 1" class="checkout-step">
            <h2 class="text-2xl font-bold mb-6">{{ $t('checkout.steps.payment') }}</h2>

            <form @submit.prevent="nextStep" class="space-y-6">
              <!-- Payment Method -->
              <div class="card bg-white shadow">
                <div class="card-body">
                  <h3 class="card-title">{{ $t('checkout.payment.method.title') }}</h3>

                  <div class="space-y-4">
                    <label class="label cursor-pointer">
                      <input
                        type="radio"
                        v-model="paymentMethod"
                        value="card"
                        class="radio radio-primary"
                      />
                      <span class="label-text">{{ $t('checkout.payment.method.card') }}</span>
                    </label>

                    <label class="label cursor-pointer">
                      <input
                        type="radio"
                        v-model="paymentMethod"
                        value="paypal"
                        class="radio radio-primary"
                      />
                      <span class="label-text">{{ $t('checkout.payment.method.paypal') }}</span>
                    </label>

                    <label class="label cursor-pointer">
                      <input
                        type="radio"
                        v-model="paymentMethod"
                        value="invoice"
                        class="radio radio-primary"
                      />
                      <span class="label-text">{{ $t('checkout.payment.method.invoice') }}</span>
                    </label>
                  </div>

                  <!-- Card Details -->
                  <div v-if="paymentMethod === 'card'" class="mt-6 space-y-4">
                    <div class="form-control">
                      <label class="label">
                        <span class="label-text">{{ $t('checkout.payment.card.number') }}</span>
                      </label>
                      <input
                        type="text"
                        v-model="cardDetails.number"
                        class="input input-bordered"
                        placeholder="1234 5678 9012 3456"
                      />
                    </div>

                    <div class="grid grid-cols-2 gap-4">
                      <div class="form-control">
                        <label class="label">
                          <span class="label-text">{{ $t('checkout.payment.card.expiry') }}</span>
                        </label>
                        <input
                          type="text"
                          v-model="cardDetails.expiry"
                          class="input input-bordered"
                          placeholder="MM/YY"
                        />
                      </div>

                      <div class="form-control">
                        <label class="label">
                          <span class="label-text">{{ $t('checkout.payment.card.cvc') }}</span>
                        </label>
                        <input
                          type="text"
                          v-model="cardDetails.cvc"
                          class="input input-bordered"
                          placeholder="123"
                        />
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <div class="flex justify-between">
                <button type="button" @click="previousStep" class="btn btn-outline">
                  {{ $t('checkout.actions.back') }}
                </button>
                <button type="submit" class="btn btn-primary">
                  {{ $t('checkout.actions.continue') }}
                </button>
              </div>
            </form>
          </div>

          <!-- Step 3: Review Order -->
          <div v-if="currentStepIndex === 2" class="checkout-step">
            <h2 class="text-2xl font-bold mb-6">{{ $t('checkout.steps.review') }}</h2>

            <div class="space-y-6">
              <!-- Order Review -->
              <div class="card bg-white shadow">
                <div class="card-body">
                  <h3 class="card-title">{{ $t('checkout.review.title') }}</h3>

                  <div class="space-y-4">
                    <div>
                      <h4 class="font-semibold">{{ $t('checkout.review.shipping') }}</h4>
                      <p>{{ shippingAddress.firstName }} {{ shippingAddress.lastName }}</p>
                      <p>{{ shippingAddress.street }}</p>
                      <p>{{ shippingAddress.postalCode }} {{ shippingAddress.city }}</p>
                      <p>{{ shippingAddress.country }}</p>
                    </div>

                    <div>
                      <h4 class="font-semibold">{{ $t('checkout.review.payment') }}</h4>
                      <p>
                        {{
                          paymentMethod === 'card'
                            ? $t('checkout.payment.method.card')
                            : paymentMethod === 'paypal'
                              ? $t('checkout.payment.method.paypal')
                              : $t('checkout.payment.method.invoice')
                        }}
                      </p>
                    </div>
                  </div>
                </div>
              </div>

              <div class="flex justify-between">
                <button type="button" @click="previousStep" class="btn btn-outline">
                  {{ $t('checkout.actions.back') }}
                </button>
                <button
                  type="button"
                  @click="placeOrder"
                  class="btn btn-primary"
                  :disabled="placingOrder"
                >
                  {{
                    placingOrder
                      ? $t('checkout.actions.processing')
                      : $t('checkout.actions.placeOrder')
                  }}
                </button>
              </div>
            </div>
          </div>
        </main>

        <!-- Order Summary Sidebar -->
        <aside class="order-summary-sidebar">
          <div class="card bg-white shadow sticky top-4">
            <div class="card-body">
              <h3 class="card-title">{{ $t('checkout.orderSummary.title') }}</h3>

              <div class="order-items space-y-4">
                <div v-for="item in cartStore.items" :key="item.id" class="flex gap-4">
                  <img :src="item.image" :alt="item.name" class="w-16 h-16 object-cover rounded" />
                  <div class="flex-1">
                    <h4 class="font-semibold">{{ item.name }}</h4>
                    <p class="text-sm text-gray-600">
                      {{ $t('checkout.orderSummary.quantity') }}: {{ item.quantity }}
                    </p>
                    <p class="font-semibold">{{ (item.price * item.quantity).toFixed(2) }}€</p>
                  </div>
                </div>
              </div>

              <div class="divider"></div>

              <div class="order-totals space-y-2">
                <div class="flex justify-between">
                  <span>{{ $t('checkout.orderSummary.subtotal') }}</span>
                  <span>{{ subtotal.toFixed(2) }}€</span>
                </div>

                <div class="flex justify-between">
                  <span>{{ $t('checkout.orderSummary.shipping') }}</span>
                  <span>{{ shippingCost.toFixed(2) }}€</span>
                </div>

                <div class="flex justify-between">
                  <span>{{ $t('checkout.orderSummary.tax') }}</span>
                  <span>{{ tax.toFixed(2) }}€</span>
                </div>

                <div class="divider"></div>

                <div class="flex justify-between font-bold text-lg">
                  <span>{{ $t('checkout.orderSummary.total') }}</span>
                  <span>{{ total.toFixed(2) }}€</span>
                </div>
              </div>
            </div>
          </div>
        </aside>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'unified-store',
  title: 'Checkout',
});

import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { useCartStore } from '../stores/cart';

const { t } = useI18n();
const router = useRouter();
const cartStore = useCartStore();

// Checkout steps
const steps = [
  t('checkout.steps.shipping'),
  t('checkout.steps.payment'),
  t('checkout.steps.review'),
];

const currentStepIndex = ref(0);
const stepCompletion = ref([false, false, false]);

// Form data
const shippingAddress = ref({
  firstName: '',
  lastName: '',
  street: '',
  city: '',
  postalCode: '',
  country: 'DE',
});

const paymentMethod = ref('card');
const cardDetails = ref({
  number: '',
  expiry: '',
  cvc: '',
});

const placingOrder = ref(false);

// Computed
const subtotal = computed(() => {
  return cartStore.items.reduce((sum, item) => sum + item.price * item.quantity, 0);
});

const shippingCost = computed(() => 5.99); // Fixed shipping cost

const tax = computed(() => (subtotal.value + shippingCost.value) * 0.19);

const total = computed(() => subtotal.value + shippingCost.value + tax.value);

// Methods
const nextStep = () => {
  if (currentStepIndex.value < steps.length - 1) {
    stepCompletion.value[currentStepIndex.value] = true;
    currentStepIndex.value++;
  }
};

const previousStep = () => {
  if (currentStepIndex.value > 0) {
    currentStepIndex.value--;
  }
};

const placeOrder = async () => {
  placingOrder.value = true;

  try {
    // TODO: Implement order placement
    console.log('Placing order...', {
      shippingAddress: shippingAddress.value,
      paymentMethod: paymentMethod.value,
      items: cartStore.items,
      total: total.value,
    });

    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 2000));

    // Clear cart and redirect to success page
    cartStore.clearCart();
    router.push('/order-success');
  } catch (error) {
    console.error('Order placement failed:', error);
    // TODO: Show error message
  } finally {
    placingOrder.value = false;
  }
};

onMounted(() => {
  if (cartStore.items.length === 0) {
    router.push('/cart');
  }
});
</script>

<style scoped>
.checkout-page {
  min-height: 100vh;
  background-color: #f5f5f5;
}

.progress-steps {
  max-width: 600px;
  margin: 0 auto;
}

.step-number {
  font-weight: bold;
}

.progress-bar {
  height: 4px;
  background-color: #e0e0e0;
  border-radius: 2px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background-color: #3b82f6;
  transition: width 0.3s ease;
}

.checkout-content {
  max-width: 1200px;
  margin: 0 auto;
}

.order-summary-sidebar {
  position: sticky;
  top: 20px;
}

.order-items {
  max-height: 300px;
  overflow-y: auto;
}

.divider {
  height: 1px;
  background-color: #e0e0e0;
  margin: 1rem 0;
}
</style>
