<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import B2BVatIdInput from './B2BVatIdInput.vue';
import type { ValidateVatIdResponse } from '@/types/vat-validation';

/**
 * Checkout.vue - 3-Step Wizard Component
 *
 * Features:
 * - Step 1: Shipping Address Form
 * - Step 2: B2B VAT-ID Validation (if B2B customer) + Shipping Method Selection
 * - Step 3: Order Review & Payment
 * - PriceCalculation: Updates VAT based on reverse charge
 *
 * WCAG 2.1 AA Compliant
 * Responsive: 320px - 1920px
 * Issue #30: B2C Price Transparency
 * Issue #31: B2B VAT-ID Validation (AStV Reverse Charge)
 */

interface Address {
  street: string;
  city: string;
  zip: string;
  country: string;
}

interface ShippingMethod {
  id: string;
  name: string;
  cost: number;
  estimatedDays: number;
}

interface CheckoutState {
  currentStep: number;
  address: Address;
  selectedShippingMethod: string;
  errors: Record<string, string>;
  registrationType?: 'B2C' | 'B2B';
  b2bVatId?: string;
  reverseChargeApplies?: boolean;
}

const router = useRouter();
const { t } = useI18n();

const state = ref<CheckoutState>({
  currentStep: 1,
  address: {
    street: '',
    city: '',
    zip: '',
    country: 'DE',
  },
  selectedShippingMethod: 'standard',
  errors: {},
});

// Mock shipping methods - would come from API
const shippingMethods: ShippingMethod[] = [
  {
    id: 'standard',
    name: t('checkout.shippingMethod.options.standard.name'),
    cost: 0,
    estimatedDays: 5,
  },
  {
    id: 'express',
    name: t('checkout.shippingMethod.options.express.name'),
    cost: 9.99,
    estimatedDays: 2,
  },
  {
    id: 'overnight',
    name: t('checkout.shippingMethod.options.overnight.name'),
    cost: 19.99,
    estimatedDays: 1,
  },
];

// Mock cart data - would come from store/API
const cartItems = ref([
  { id: 1, name: 'Product 1', price: 29.99, vat: 5.7, qty: 1 },
  { id: 2, name: 'Product 2', price: 49.99, vat: 9.5, qty: 2 },
]);

const countries = [
  { code: 'DE', name: t('checkout.form.countries.DE') },
  { code: 'AT', name: t('checkout.form.countries.AT') },
  { code: 'CH', name: t('checkout.form.countries.CH') },
  { code: 'FR', name: t('checkout.form.countries.FR') },
  { code: 'NL', name: t('checkout.form.countries.NL') },
  { code: 'BE', name: t('checkout.form.countries.BE') },
];

// Computed properties
const subtotal = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + item.price * item.qty, 0);
});

const totalVat = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + item.vat * item.qty, 0);
});

const shippingCost = computed(() => {
  const method = shippingMethods.find(m => m.id === state.value.selectedShippingMethod);
  return method?.cost || 0;
});

const grandTotal = computed(() => {
  return subtotal.value + totalVat.value + shippingCost.value;
});

const selectedShippingMethodDetails = computed(() => {
  return shippingMethods.find(m => m.id === state.value.selectedShippingMethod);
});

// Validation
// Validation methods
const validateAddress = (): boolean => {
  const errors: Record<string, string> = {};
  const { street, city, zip, country } = state.value.address;

  if (!street?.trim()) {
    errors.street = 'Street is required';
  }
  if (!city?.trim()) {
    errors.city = 'City is required';
  }
  if (!zip?.trim()) {
    errors.zip = 'Postal code is required';
  }
  if (!country) {
    errors.country = t('checkout.validation.countryRequired');
  }

  // Basic postal code validation (German format)
  if (zip && country === 'DE' && !/^\d{5}$/.test(zip)) {
    errors.zip = 'Invalid German postal code (format: 12345)';
  }

  state.value.errors = errors;
  return Object.keys(errors).length === 0;
};

// Handle VAT ID validation result (Issue #31)
const handleVatValidationResult = (result: ValidateVatIdResponse) => {
  state.value.b2bVatId = result.vatId;
  state.value.reverseChargeApplies = result.reverseChargeApplies;

  if (result.isValid) {
    console.log(`B2B VAT ID validated: ${result.vatId}`);
    console.log(`Reverse charge applies: ${result.reverseChargeApplies}`);

    // Re-calculate totals based on reverse charge
    // TODO: Recalculate order totals with reverse charge if applicable
  }
};

// Step navigation
const goToStep = (step: number) => {
  if (step === 1) {
    state.value.currentStep = 1;
  } else if (step === 2 && validateAddress()) {
    state.value.currentStep = 2;
  } else if (step === 3) {
    state.value.currentStep = 3;
  }
};

const previousStep = () => {
  if (state.value.currentStep > 1) {
    state.value.currentStep--;
  }
};

const nextStep = () => {
  if (state.value.currentStep === 1 && validateAddress()) {
    state.value.currentStep = 2;
  } else if (state.value.currentStep === 2) {
    state.value.currentStep = 3;
  }
};

// Checkout submission (placeholder for payment gateway integration)
const proceedToPayment = () => {
  console.log('Proceeding to payment with:', {
    address: state.value.address,
    shippingMethod: state.value.selectedShippingMethod,
    total: grandTotal.value,
  });

  // TODO: Integrate with payment gateway
  // For now, navigate to payment success
  router.push('/checkout/success');
};

// Step indicators
const steps = [
  t('checkout.steps.shippingAddress'),
  t('checkout.steps.shippingMethod'),
  t('checkout.steps.orderReview'),
];
</script>

<template>
  <div class="min-h-screen bg-base-100">
    <!-- Header -->
    <header class="bg-base-200 border-b border-base-300 py-4" role="banner">
      <div class="max-w-7xl mx-auto px-4">
        <h1 class="text-3xl font-bold text-base-900">{{ t('checkout.header.title') }}</h1>
        <p class="text-base-600 mt-1">
          {{
            t('checkout.header.stepIndicator', { currentStep: state.currentStep, totalSteps: 3 })
          }}
        </p>
      </div>
    </header>

    <main class="max-w-7xl mx-auto px-4 py-8">
      <!-- Step Indicators -->
      <nav aria-label="Checkout progress" class="mb-8">
        <ol class="flex gap-4 md:gap-8">
          <li v-for="(step, index) in steps" :key="index" class="flex-1">
            <div
              class="flex items-center gap-3 cursor-pointer"
              @click="goToStep(index + 1)"
              :aria-current="state.currentStep === index + 1 ? 'step' : undefined"
              role="button"
              :tabindex="state.currentStep === index + 1 ? 0 : -1"
              @keypress.enter="goToStep(index + 1)"
            >
              <div
                :class="[
                  'w-10 h-10 rounded-full flex items-center justify-center font-bold',
                  state.currentStep > index + 1
                    ? 'bg-success text-success-content'
                    : state.currentStep === index + 1
                      ? 'bg-primary text-primary-content'
                      : 'bg-base-300 text-base-600',
                ]"
              >
                {{ state.currentStep > index + 1 ? '✓' : index + 1 }}
              </div>
              <span
                class="hidden md:inline text-sm font-medium"
                :class="state.currentStep >= index + 1 ? 'text-base-900' : 'text-base-500'"
              >
                {{ step }}
              </span>
            </div>
          </li>
        </ol>
      </nav>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Main Content -->
        <div class="lg:col-span-2">
          <!-- Step 1: Shipping Address -->
          <section v-if="state.currentStep === 1" aria-label="Shipping Address Form">
            <h2 class="text-2xl font-bold mb-6 text-base-900">
              {{ t('checkout.steps.shippingAddress') }}
            </h2>

            <form @submit.prevent="nextStep" class="space-y-4">
              <!-- Street Input -->
              <div class="form-control">
                <label for="street" class="label">
                  <span class="label-text font-medium"
                    >{{ t('checkout.form.labels.streetAddress') }}
                    <span class="text-error">*</span></span
                  >
                </label>
                <input
                  id="street"
                  v-model="state.address.street"
                  type="text"
                  :placeholder="t('checkout.form.placeholders.streetAddress')"
                  class="input input-bordered w-full"
                  @change="validateAddress"
                  required
                  aria-required="true"
                />
                <span v-if="state.errors.street" class="text-error text-sm mt-1" role="alert">
                  {{ state.errors.street }}
                </span>
              </div>

              <!-- City Input -->
              <div class="form-control">
                <label for="city" class="label">
                  <span class="label-text font-medium"
                    >{{ t('checkout.form.labels.city') }} <span class="text-error">*</span></span
                  >
                </label>
                <input
                  id="city"
                  v-model="state.address.city"
                  type="text"
                  :placeholder="t('checkout.form.placeholders.city')"
                  class="input input-bordered w-full"
                  @change="validateAddress"
                  required
                  aria-required="true"
                />
                <span v-if="state.errors.city" class="text-error text-sm mt-1" role="alert">
                  {{ state.errors.city }}
                </span>
              </div>

              <div class="grid grid-cols-2 gap-4">
                <!-- Postal Code Input -->
                <div class="form-control">
                  <label for="zip" class="label">
                    <span class="label-text font-medium"
                      >{{ t('checkout.form.labels.postalCode') }}
                      <span class="text-error">*</span></span
                    >
                  </label>
                  <input
                    id="zip"
                    v-model="state.address.zip"
                    type="text"
                    :placeholder="t('checkout.form.placeholders.postalCode')"
                    class="input input-bordered w-full"
                    @change="validateAddress"
                    required
                    aria-required="true"
                  />
                  <span v-if="state.errors.zip" class="text-error text-sm mt-1" role="alert">
                    {{ state.errors.zip }}
                  </span>
                </div>

                <!-- Country Select -->
                <div class="form-control">
                  <label for="country" class="label">
                    <span class="label-text font-medium"
                      >{{ t('checkout.form.labels.country') }}
                      <span class="text-error">*</span></span
                    >
                  </label>
                  <select
                    id="country"
                    v-model="state.address.country"
                    class="select select-bordered w-full"
                    required
                    aria-required="true"
                  >
                    <option v-for="country in countries" :key="country.code" :value="country.code">
                      {{ country.name }}
                    </option>
                  </select>
                  <span v-if="state.errors.country" class="text-error text-sm mt-1" role="alert">
                    {{ state.errors.country }}
                  </span>
                </div>
              </div>

              <!-- Save Address Checkbox -->
              <div class="form-control">
                <label class="label cursor-pointer">
                  <span class="label-text">{{ t('checkout.form.labels.saveAddress') }}</span>
                  <input
                    type="checkbox"
                    class="checkbox checkbox-primary"
                    aria-label="Save address for future orders"
                  />
                </label>
              </div>

              <!-- Action Buttons -->
              <div class="flex gap-4 mt-8">
                <button
                  type="submit"
                  class="btn btn-primary flex-1"
                  aria-label="Continue to shipping method selection"
                >
                  {{ t('checkout.form.buttons.continueToShipping') }}
                </button>
              </div>
            </form>
          </section>

          <!-- Step 2: B2B VAT-ID Validation (if B2B) + Shipping Method -->
          <section
            v-if="state.currentStep === 2"
            aria-label="Shipping Method Selection and B2B VAT Validation"
          >
            <!-- B2B VAT-ID Input (if customer is B2B) -->
            <div
              v-if="state.registrationType === 'B2B'"
              class="mb-8 bg-blue-50 p-6 rounded-lg border border-blue-200"
            >
              <h3 class="text-lg font-bold mb-4 text-blue-900">{{ t('checkout.b2b.title') }}</h3>
              <p class="text-blue-800 mb-4">
                {{ t('checkout.b2b.description') }}
              </p>
              <B2BVatIdInput
                :seller-country="state.address.country"
                :buyer-country="state.address.country"
                @validation-result="handleVatValidationResult"
              />
            </div>

            <h2 class="text-2xl font-bold mb-6 text-base-900">
              {{ t('checkout.shippingMethod.title') }}
            </h2>

            <fieldset class="space-y-4">
              <legend class="sr-only">{{ t('checkout.shippingMethod.legend') }}</legend>

              <div
                v-for="method in shippingMethods"
                :key="method.id"
                class="form-control border border-base-300 rounded-lg p-4 cursor-pointer hover:bg-base-100 transition"
              >
                <label class="label">
                  <span class="label-text flex-1">
                    <div class="flex items-start gap-4">
                      <input
                        type="radio"
                        :value="method.id"
                        v-model="state.selectedShippingMethod"
                        class="radio radio-primary mt-1"
                        :aria-label="`${method.name} - ${method.cost > 0 ? '€' + method.cost.toFixed(2) : 'Free'} - Estimated ${method.estimatedDays} days`"
                      />
                      <div>
                        <p class="font-medium text-base-900">
                          {{ method.name }}
                        </p>
                        <p class="text-sm text-base-600">
                          {{
                            t('checkout.shippingMethod.estimatedDelivery', {
                              days: method.estimatedDays,
                            })
                          }}
                        </p>
                      </div>
                    </div>
                  </span>
                  <span class="text-lg font-bold text-base-900">
                    {{ method.cost > 0 ? '€' + method.cost.toFixed(2) : 'Free' }}
                  </span>
                </label>
              </div>
            </fieldset>

            <!-- Action Buttons -->
            <div class="flex gap-4 mt-8">
              <button
                @click="previousStep"
                class="btn btn-outline flex-1"
                aria-label="Go back to shipping address"
              >
                {{ t('checkout.form.buttons.back') }}
              </button>
              <button
                @click="nextStep"
                class="btn btn-primary flex-1"
                aria-label="Continue to order review"
              >
                {{ t('checkout.form.buttons.continueToReview') }}
              </button>
            </div>
          </section>

          <!-- Step 3: Order Review -->
          <section v-if="state.currentStep === 3" aria-label="Order Review and Payment">
            <h2 class="text-2xl font-bold mb-6 text-base-900">
              {{ t('checkout.orderReview.title') }}
            </h2>

            <!-- Shipping Address Review -->
            <div class="card bg-base-100 border border-base-300 mb-6">
              <div class="card-body">
                <h3 class="card-title text-lg">
                  {{ t('checkout.orderReview.shippingAddress.title') }}
                </h3>
                <address class="not-italic text-base-700">
                  {{ state.address.street }}<br />
                  {{ state.address.zip }} {{ state.address.city }}<br />
                  {{ state.address.country }}
                </address>
                <button
                  @click="state.currentStep = 1"
                  class="btn btn-sm btn-ghost mt-4"
                  aria-label="Edit shipping address"
                >
                  {{ t('checkout.orderReview.shippingAddress.editButton') }}
                </button>
              </div>
            </div>

            <!-- Shipping Method Review -->
            <div class="card bg-base-100 border border-base-300 mb-6">
              <div class="card-body">
                <h3 class="card-title text-lg">
                  {{ t('checkout.orderReview.shippingMethod.title') }}
                </h3>
                <p class="text-base-700">
                  {{ selectedShippingMethodDetails?.name }}
                </p>
                <p class="text-sm text-base-600">
                  {{
                    t('checkout.shippingMethod.estimatedDelivery', {
                      days: selectedShippingMethodDetails?.estimatedDays,
                    })
                  }}
                </p>
                <button
                  @click="state.currentStep = 2"
                  class="btn btn-sm btn-ghost mt-4"
                  aria-label="Change shipping method"
                >
                  {{ t('checkout.orderReview.shippingMethod.changeButton') }}
                </button>
              </div>
            </div>

            <!-- Order Items Review -->
            <div class="card bg-base-100 border border-base-300 mb-6">
              <div class="card-body">
                <h3 class="card-title text-lg">{{ t('checkout.orderReview.orderItems.title') }}</h3>
                <div class="overflow-x-auto">
                  <table class="table w-full">
                    <thead>
                      <tr>
                        <th>{{ t('checkout.orderReview.orderItems.headers.product') }}</th>
                        <th class="text-right">
                          {{ t('checkout.orderReview.orderItems.headers.quantity') }}
                        </th>
                        <th class="text-right">
                          {{ t('checkout.orderReview.orderItems.headers.price') }}
                        </th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="item in cartItems" :key="item.id">
                        <td>{{ item.name }}</td>
                        <td class="text-right">{{ item.qty }}</td>
                        <td class="text-right">€{{ (item.price * item.qty).toFixed(2) }}</td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>

            <!-- Action Buttons -->
            <div class="flex gap-4 mt-8">
              <button
                @click="previousStep"
                class="btn btn-outline flex-1"
                aria-label="Go back to shipping method selection"
              >
                {{ t('checkout.form.buttons.back') }}
              </button>
              <button
                @click="proceedToPayment"
                class="btn btn-success flex-1"
                aria-label="Proceed to payment"
              >
                {{ t('checkout.form.buttons.proceedToPayment') }}
              </button>
            </div>
          </section>
        </div>

        <!-- Order Summary Sidebar -->
        <aside aria-label="Order Summary" class="lg:col-span-1">
          <div class="card bg-base-100 border border-base-300 sticky top-4">
            <div class="card-body">
              <h2 class="card-title text-lg">{{ t('checkout.orderSummary.title') }}</h2>

              <div class="divider my-2"></div>

              <!-- Order Items -->
              <div class="space-y-2 mb-4">
                <div v-for="item in cartItems" :key="item.id" class="flex justify-between text-sm">
                  <span class="text-base-700">{{ item.name }} x {{ item.qty }}</span>
                  <span class="font-medium">€{{ (item.price * item.qty).toFixed(2) }}</span>
                </div>
              </div>

              <div class="divider my-2"></div>

              <!-- Pricing Breakdown -->
              <div class="space-y-2 text-sm">
                <div class="flex justify-between">
                  <span>{{ t('checkout.orderSummary.subtotal') }}</span>
                  <span>€{{ subtotal.toFixed(2) }}</span>
                </div>
                <div class="flex justify-between text-base-600">
                  <span>{{ t('checkout.orderSummary.vat') }}</span>
                  <span>€{{ totalVat.toFixed(2) }}</span>
                </div>
                <div v-if="shippingCost > 0" class="flex justify-between">
                  <span>{{ t('checkout.orderSummary.shipping') }}</span>
                  <span>€{{ shippingCost.toFixed(2) }}</span>
                </div>
              </div>

              <div class="divider my-2"></div>

              <!-- Grand Total -->
              <div class="flex justify-between text-lg font-bold">
                <span>{{ t('checkout.orderSummary.total') }}</span>
                <span class="text-primary">€{{ grandTotal.toFixed(2) }}</span>
              </div>

              <!-- Trust Badge -->
              <div class="mt-4 p-3 bg-info/10 rounded flex items-start gap-2">
                <span class="text-info text-xl">🔒</span>
                <div class="text-xs text-base-700">
                  <p class="font-medium">{{ t('checkout.orderSummary.secureCheckout.title') }}</p>
                  <p>{{ t('checkout.orderSummary.secureCheckout.description') }}</p>
                </div>
              </div>
            </div>
          </div>
        </aside>
      </div>
    </main>
  </div>
</template>

<style scoped>
/* Smooth step transitions */
section {
  animation: fadeIn 0.3s ease-in-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Ensure focus indicators are visible for accessibility */
:focus {
  outline: 2px solid var(--color-primary);
  outline-offset: 2px;
}

/* Address styling for semantics */
address {
  white-space: pre-line;
}
</style>
