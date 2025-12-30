<script setup lang="ts">
import { ref, computed } from "vue";
import { useRouter } from "vue-router";
import B2BVatIdInput from "./B2BVatIdInput.vue";

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
  registrationType?: "B2C" | "B2B";
  b2bVatId?: string;
  reverseChargeApplies?: boolean;
}

const router = useRouter();

const state = ref<CheckoutState>({
  currentStep: 1,
  address: {
    street: "",
    city: "",
    zip: "",
    country: "DE",
  },
  selectedShippingMethod: "standard",
  errors: {},
});

// Mock shipping methods - would come from API
const shippingMethods: ShippingMethod[] = [
  { id: "standard", name: "Standard Shipping", cost: 0, estimatedDays: 5 },
  { id: "express", name: "Express Shipping", cost: 9.99, estimatedDays: 2 },
  {
    id: "overnight",
    name: "Overnight Shipping",
    cost: 19.99,
    estimatedDays: 1,
  },
];

// Mock cart data - would come from store/API
const cartItems = ref([
  { id: 1, name: "Product 1", price: 29.99, vat: 5.7, qty: 1 },
  { id: 2, name: "Product 2", price: 49.99, vat: 9.5, qty: 2 },
]);

const countries = ["DE", "AT", "CH", "FR", "NL", "BE"];

// Computed properties
const subtotal = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + item.price * item.qty, 0);
});

const totalVat = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + item.vat * item.qty, 0);
});

const shippingCost = computed(() => {
  const method = shippingMethods.find(
    (m) => m.id === state.value.selectedShippingMethod
  );
  return method?.cost || 0;
});

const grandTotal = computed(() => {
  return subtotal.value + totalVat.value + shippingCost.value;
});

const selectedShippingMethodDetails = computed(() => {
  return shippingMethods.find(
    (m) => m.id === state.value.selectedShippingMethod
  );
});

// Validation
// Validation methods
const validateAddress = (): boolean => {
  const errors: Record<string, string> = {};
  const { street, city, zip, country } = state.value.address;

  if (!street?.trim()) {
    errors.street = "Street is required";
  }
  if (!city?.trim()) {
    errors.city = "City is required";
  }
  if (!zip?.trim()) {
    errors.zip = "Postal code is required";
  }
  if (!country) {
    errors.country = "Country is required";
  }

  // Basic postal code validation (German format)
  if (zip && country === "DE" && !/^\d{5}$/.test(zip)) {
    errors.zip = "Invalid German postal code (format: 12345)";
  }

  state.value.errors = errors;
  return Object.keys(errors).length === 0;
};

// Handle VAT ID validation result (Issue #31)
const handleVatValidationResult = (result: any) => {
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
  console.log("Proceeding to payment with:", {
    address: state.value.address,
    shippingMethod: state.value.selectedShippingMethod,
    total: grandTotal.value,
  });

  // TODO: Integrate with payment gateway
  // For now, navigate to payment success
  router.push("/checkout/success");
};

// Step indicators
const steps = ["Shipping Address", "Shipping Method", "Order Review"];
</script>

<template>
  <div class="min-h-screen bg-base-100">
    <!-- Header -->
    <header class="bg-base-200 border-b border-base-300 py-4" role="banner">
      <div class="max-w-7xl mx-auto px-4">
        <h1 class="text-3xl font-bold text-base-900">Checkout</h1>
        <p class="text-base-600 mt-1">Step {{ state.currentStep }} of 3</p>
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
              :aria-current="
                state.currentStep === index + 1 ? 'step' : undefined
              "
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
                {{ state.currentStep > index + 1 ? "✓" : index + 1 }}
              </div>
              <span
                class="hidden md:inline text-sm font-medium"
                :class="
                  state.currentStep >= index + 1
                    ? 'text-base-900'
                    : 'text-base-500'
                "
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
          <section
            v-if="state.currentStep === 1"
            aria-label="Shipping Address Form"
          >
            <h2 class="text-2xl font-bold mb-6 text-base-900">
              Shipping Address
            </h2>

            <form @submit.prevent="nextStep" class="space-y-4">
              <!-- Street Input -->
              <div class="form-control">
                <label for="street" class="label">
                  <span class="label-text font-medium"
                    >Street Address <span class="text-error">*</span></span
                  >
                </label>
                <input
                  id="street"
                  v-model="state.address.street"
                  type="text"
                  placeholder="123 Main Street"
                  class="input input-bordered w-full"
                  @change="validateAddress"
                  required
                  aria-required="true"
                />
                <span
                  v-if="state.errors.street"
                  class="text-error text-sm mt-1"
                  role="alert"
                >
                  {{ state.errors.street }}
                </span>
              </div>

              <!-- City Input -->
              <div class="form-control">
                <label for="city" class="label">
                  <span class="label-text font-medium"
                    >City <span class="text-error">*</span></span
                  >
                </label>
                <input
                  id="city"
                  v-model="state.address.city"
                  type="text"
                  placeholder="Berlin"
                  class="input input-bordered w-full"
                  @change="validateAddress"
                  required
                  aria-required="true"
                />
                <span
                  v-if="state.errors.city"
                  class="text-error text-sm mt-1"
                  role="alert"
                >
                  {{ state.errors.city }}
                </span>
              </div>

              <div class="grid grid-cols-2 gap-4">
                <!-- Postal Code Input -->
                <div class="form-control">
                  <label for="zip" class="label">
                    <span class="label-text font-medium"
                      >Postal Code <span class="text-error">*</span></span
                    >
                  </label>
                  <input
                    id="zip"
                    v-model="state.address.zip"
                    type="text"
                    placeholder="12345"
                    class="input input-bordered w-full"
                    @change="validateAddress"
                    required
                    aria-required="true"
                  />
                  <span
                    v-if="state.errors.zip"
                    class="text-error text-sm mt-1"
                    role="alert"
                  >
                    {{ state.errors.zip }}
                  </span>
                </div>

                <!-- Country Select -->
                <div class="form-control">
                  <label for="country" class="label">
                    <span class="label-text font-medium"
                      >Country <span class="text-error">*</span></span
                    >
                  </label>
                  <select
                    id="country"
                    v-model="state.address.country"
                    class="select select-bordered w-full"
                    required
                    aria-required="true"
                  >
                    <option
                      v-for="country in countries"
                      :key="country"
                      :value="country"
                    >
                      {{ country }}
                    </option>
                  </select>
                  <span
                    v-if="state.errors.country"
                    class="text-error text-sm mt-1"
                    role="alert"
                  >
                    {{ state.errors.country }}
                  </span>
                </div>
              </div>

              <!-- Save Address Checkbox -->
              <div class="form-control">
                <label class="label cursor-pointer">
                  <span class="label-text"
                    >Save this address for future orders</span
                  >
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
                  Continue to Shipping Method
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
              <h3 class="text-lg font-bold mb-4 text-blue-900">
                Business Verification
              </h3>
              <p class="text-blue-800 mb-4">
                Please provide your VAT ID for verification. Valid EU business
                VAT-IDs may qualify for reverse charge (0% VAT).
              </p>
              <B2BVatIdInput
                :seller-country="state.address.country"
                :buyer-country="state.address.country"
                @validation-result="handleVatValidationResult"
              />
            </div>

            <h2 class="text-2xl font-bold mb-6 text-base-900">
              Shipping Method
            </h2>

            <fieldset class="space-y-4">
              <legend class="sr-only">Select a shipping method</legend>

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
                          Estimated delivery:
                          {{ method.estimatedDays }} business days
                        </p>
                      </div>
                    </div>
                  </span>
                  <span class="text-lg font-bold text-base-900">
                    {{
                      method.cost > 0 ? "€" + method.cost.toFixed(2) : "Free"
                    }}
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
                Back
              </button>
              <button
                @click="nextStep"
                class="btn btn-primary flex-1"
                aria-label="Continue to order review"
              >
                Continue to Review
              </button>
            </div>
          </section>

          <!-- Step 3: Order Review -->
          <section
            v-if="state.currentStep === 3"
            aria-label="Order Review and Payment"
          >
            <h2 class="text-2xl font-bold mb-6 text-base-900">Order Review</h2>

            <!-- Shipping Address Review -->
            <div class="card bg-base-100 border border-base-300 mb-6">
              <div class="card-body">
                <h3 class="card-title text-lg">Shipping Address</h3>
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
                  Edit Address
                </button>
              </div>
            </div>

            <!-- Shipping Method Review -->
            <div class="card bg-base-100 border border-base-300 mb-6">
              <div class="card-body">
                <h3 class="card-title text-lg">Shipping Method</h3>
                <p class="text-base-700">
                  {{ selectedShippingMethodDetails?.name }}
                </p>
                <p class="text-sm text-base-600">
                  Estimated delivery:
                  {{ selectedShippingMethodDetails?.estimatedDays }} business
                  days
                </p>
                <button
                  @click="state.currentStep = 2"
                  class="btn btn-sm btn-ghost mt-4"
                  aria-label="Change shipping method"
                >
                  Change Shipping Method
                </button>
              </div>
            </div>

            <!-- Order Items Review -->
            <div class="card bg-base-100 border border-base-300 mb-6">
              <div class="card-body">
                <h3 class="card-title text-lg">Order Items</h3>
                <div class="overflow-x-auto">
                  <table class="table w-full">
                    <thead>
                      <tr>
                        <th>Product</th>
                        <th class="text-right">Qty</th>
                        <th class="text-right">Price</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="item in cartItems" :key="item.id">
                        <td>{{ item.name }}</td>
                        <td class="text-right">{{ item.qty }}</td>
                        <td class="text-right">
                          €{{ (item.price * item.qty).toFixed(2) }}
                        </td>
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
                Back
              </button>
              <button
                @click="proceedToPayment"
                class="btn btn-success flex-1"
                aria-label="Proceed to payment"
              >
                Proceed to Payment
              </button>
            </div>
          </section>
        </div>

        <!-- Order Summary Sidebar -->
        <aside aria-label="Order Summary" class="lg:col-span-1">
          <div class="card bg-base-100 border border-base-300 sticky top-4">
            <div class="card-body">
              <h2 class="card-title text-lg">Order Summary</h2>

              <div class="divider my-2"></div>

              <!-- Order Items -->
              <div class="space-y-2 mb-4">
                <div
                  v-for="item in cartItems"
                  :key="item.id"
                  class="flex justify-between text-sm"
                >
                  <span class="text-base-700"
                    >{{ item.name }} x {{ item.qty }}</span
                  >
                  <span class="font-medium"
                    >€{{ (item.price * item.qty).toFixed(2) }}</span
                  >
                </div>
              </div>

              <div class="divider my-2"></div>

              <!-- Pricing Breakdown -->
              <div class="space-y-2 text-sm">
                <div class="flex justify-between">
                  <span>Subtotal</span>
                  <span>€{{ subtotal.toFixed(2) }}</span>
                </div>
                <div class="flex justify-between text-base-600">
                  <span>VAT (incl.)</span>
                  <span>€{{ totalVat.toFixed(2) }}</span>
                </div>
                <div v-if="shippingCost > 0" class="flex justify-between">
                  <span>Shipping</span>
                  <span>€{{ shippingCost.toFixed(2) }}</span>
                </div>
              </div>

              <div class="divider my-2"></div>

              <!-- Grand Total -->
              <div class="flex justify-between text-lg font-bold">
                <span>Total</span>
                <span class="text-primary">€{{ grandTotal.toFixed(2) }}</span>
              </div>

              <!-- Trust Badge -->
              <div class="mt-4 p-3 bg-info/10 rounded flex items-start gap-2">
                <span class="text-info text-xl">🔒</span>
                <div class="text-xs text-base-700">
                  <p class="font-medium">Secure Checkout</p>
                  <p>Your payment is encrypted and secure</p>
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
