<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useCartStore } from '@/stores/cart';

const router = useRouter();
const cartStore = useCartStore();

// Types
interface ShippingForm {
  firstName: string;
  lastName: string;
  street: string;
  zipCode: string;
  city: string;
  country: string;
}

interface ShippingMethod {
  id: string;
  name: string;
  description: string;
  price: number;
  days: number;
  selected?: boolean;
}

interface PaymentMethod {
  id: string;
  name: string;
  description: string;
  icon: string;
}

// State - Wizard
const currentStep = ref<'shipping' | 'shipping-method' | 'review'>('shipping');
const steps = ['Adresse', 'Versand', 'Überprüfung'];

// State - Forms
const form = ref<ShippingForm>({
  firstName: '',
  lastName: '',
  street: '',
  zipCode: '',
  city: '',
  country: 'Germany',
});

// State - Shipping Methods
const shippingMethods = ref<ShippingMethod[]>([
  {
    id: 'standard',
    name: 'Standardversand',
    description: 'Lieferung in 5-7 Werktagen',
    price: 5.99,
    days: 5,
    selected: true,
  },
  {
    id: 'express',
    name: 'Expressversand',
    description: 'Lieferung in 2-3 Werktagen',
    price: 12.99,
    days: 2,
  },
  {
    id: 'overnight',
    name: 'Overnight',
    description: 'Lieferung am nächsten Werktag',
    price: 24.99,
    days: 1,
  },
]);

const selectedShippingMethod = ref<ShippingMethod>(shippingMethods.value[0]);

// State - Payment Methods
const paymentMethods = ref<PaymentMethod[]>([
  {
    id: 'card',
    name: 'Kreditkarte',
    description: 'Visa, Mastercard, Amex',
    icon: '💳',
  },
  {
    id: 'paypal',
    name: 'PayPal',
    description: 'Schnell und sicher mit PayPal',
    icon: '🅿️',
  },
  {
    id: 'sepa',
    name: 'SEPA Überweisung',
    description: 'Direktüberweisung von Ihrem Bankkonto',
    icon: '🏦',
  },
]);

const selectedPaymentMethod = ref<PaymentMethod | null>(paymentMethods.value[0]);

// State - UI
const agreedToTerms = ref(false);
const isSubmitting = ref(false);
const errors = ref<Record<string, string>>({});

// Computed properties
const subtotal = computed(() => {
  return cartStore.items.reduce((sum, item) => sum + item.price * item.quantity, 0);
});

const vatAmount = computed(() => {
  return Number((subtotal.value * 0.19).toFixed(2));
});

const shippingCost = computed(() => selectedShippingMethod.value.price);

const total = computed(() => {
  return Number((subtotal.value + vatAmount.value + shippingCost.value).toFixed(2));
});

const currentStepIndex = computed(() => {
  switch (currentStep.value) {
    case 'shipping':
      return 0;
    case 'shipping-method':
      return 1;
    case 'review':
      return 2;
    default:
      return 0;
  }
});

const isFormValid = computed(() => {
  switch (currentStep.value) {
    case 'shipping':
      return (
        form.value.firstName.trim() !== '' &&
        form.value.lastName.trim() !== '' &&
        form.value.street.trim() !== '' &&
        form.value.zipCode.trim() !== '' &&
        form.value.city.trim() !== '' &&
        form.value.country.trim() !== ''
      );
    case 'shipping-method':
      return selectedShippingMethod.value !== null;
    case 'review':
      return selectedPaymentMethod.value !== null && agreedToTerms.value;
    default:
      return false;
  }
});

const stepCompletion = computed(() => {
  return [
    !!(form.value.firstName && form.value.lastName && form.value.street),
    selectedShippingMethod.value !== null,
    agreedToTerms.value && selectedPaymentMethod.value !== null,
  ];
});

// Methods
const formatPrice = (price: number): string => {
  return new Intl.NumberFormat('de-DE', {
    style: 'currency',
    currency: 'EUR',
  }).format(price);
};

const validateForm = (): boolean => {
  errors.value = {};

  if (!form.value.firstName.trim()) {
    errors.value.firstName = 'Vorname ist erforderlich';
  }

  if (!form.value.lastName.trim()) {
    errors.value.lastName = 'Nachname ist erforderlich';
  }

  if (!form.value.street.trim()) {
    errors.value.street = 'Straße und Hausnummer sind erforderlich';
  }

  if (!form.value.zipCode.trim()) {
    errors.value.zipCode = 'Postleitzahl ist erforderlich';
  } else if (!/^\d{5}$/.test(form.value.zipCode)) {
    errors.value.zipCode = 'Postleitzahl muss 5 Ziffern haben';
  }

  if (!form.value.city.trim()) {
    errors.value.city = 'Stadt ist erforderlich';
  }

  return Object.keys(errors.value).length === 0;
};

const nextStep = () => {
  if (currentStep.value === 'shipping') {
    if (!validateForm()) {
      console.error('Form validation failed');
      return;
    }
    currentStep.value = 'shipping-method';
  } else if (currentStep.value === 'shipping-method') {
    if (!selectedShippingMethod.value) {
      errors.value.shippingMethod = 'Bitte wählen Sie eine Versandart';
      return;
    }
    currentStep.value = 'review';
  }
  errors.value = {};
};

const prevStep = () => {
  if (currentStep.value === 'shipping-method') {
    currentStep.value = 'shipping';
  } else if (currentStep.value === 'review') {
    currentStep.value = 'shipping-method';
  }
  errors.value = {};
};

const selectShippingMethod = (method: ShippingMethod) => {
  selectedShippingMethod.value = method;
  shippingMethods.value.forEach(m => (m.selected = m.id === method.id));
  errors.value.shippingMethod = '';
};

const selectPaymentMethod = (method: PaymentMethod) => {
  selectedPaymentMethod.value = method;
};

const completeOrder = async () => {
  if (!agreedToTerms.value) {
    errors.value.terms = 'Sie müssen den Bedingungen zustimmen';
    return;
  }

  if (!selectedPaymentMethod.value) {
    errors.value.payment = 'Bitte wählen Sie eine Zahlungsmethode';
    return;
  }

  isSubmitting.value = true;

  try {
    // Simulate API call to create order
    await new Promise(resolve => setTimeout(resolve, 1500));

    console.log('Order completed with data:', {
      items: cartStore.items,
      shipping: form.value,
      shippingMethod: selectedShippingMethod.value,
      paymentMethod: selectedPaymentMethod.value,
      total: total.value,
    });

    // Clear cart
    cartStore.clearCart();

    // Redirect to confirmation page
    await router.push('/order-confirmation');
  } catch (error) {
    console.error('Error completing order:', error);
    errors.value.submit =
      'Fehler beim Abschließen der Bestellung. Bitte versuchen Sie es später erneut.';
  } finally {
    isSubmitting.value = false;
  }
};

const goBack = () => {
  router.push('/cart');
};

onMounted(() => {
  if (cartStore.items.length === 0) {
    router.push('/shop');
  }
});
</script>

<template>
  <div class="checkout-page">
    <div class="container">
      <div class="page-header">
        <h1>{{ $t('legal.checkout.header.title') }}</h1>
        <p class="breadcrumb">
          <router-link to="/shop">{{ $t('legal.checkout.header.breadcrumb.shop') }}</router-link> /
          <router-link to="/cart">{{ $t('legal.checkout.header.breadcrumb.cart') }}</router-link>
          {{ $t('legal.checkout.header.breadcrumb.checkout') }}
        </p>
      </div>

      <!-- Progress Indicator -->
      <div class="progress-container">
        <div class="progress-steps">
          <div
            v-for="(step, index) in steps"
            :key="index"
            class="progress-step"
            :class="{
              active: currentStepIndex === index,
              completed: stepCompletion[index],
            }"
          >
            <div class="step-number">
              {{ stepCompletion[index] && currentStepIndex > index ? '✓' : index + 1 }}
            </div>
            <div class="step-label">{{ step }}</div>
          </div>
        </div>
        <div class="progress-bar">
          <div
            class="progress-fill"
            :style="{
              width: `${((currentStepIndex + 1) / steps.length) * 100}%`,
            }"
          ></div>
        </div>
      </div>

      <div class="checkout-content">
        <!-- Order Summary Sidebar (Sticky) -->
        <aside class="order-summary-sidebar">
          <h3>{{ $t('legal.checkout.orderSummary.title') }}</h3>

          <div class="order-items">
            <div v-for="item in cartStore.items" :key="item.id" class="order-item-summary">
              <div class="item-summary-info">
                <span class="item-name">{{ item.name }}</span>
                <span class="item-qty">{{ item.quantity }}×</span>
              </div>
              <span class="item-summary-price">
                {{ formatPrice(item.price * item.quantity) }}
              </span>
            </div>
          </div>

          <div class="price-summary">
            <div class="summary-row">
              <span>{{ $t('legal.checkout.orderSummary.netto') }}</span>
              <span>{{ formatPrice(subtotal) }}</span>
            </div>
            <div class="summary-row vat">
              <span>{{ $t('legal.checkout.orderSummary.vat') }}</span>
              <span>{{ formatPrice(vatAmount) }}</span>
            </div>
            <div class="summary-row shipping">
              <span>{{ $t('legal.checkout.orderSummary.shipping') }}</span>
              <span>{{ formatPrice(shippingCost) }}</span>
            </div>
            <div class="summary-row total">
              <span>{{ $t('legal.checkout.orderSummary.total') }}</span>
              <span class="total-amount">{{ formatPrice(total) }}</span>
            </div>
          </div>

          <!-- Trust Badges -->
          <div class="trust-badges">
            <div class="badge">
              <span class="badge-icon">🔒</span>
              <span class="badge-text">{{
                $t('legal.checkout.orderSummary.trustBadges.ssl')
              }}</span>
            </div>
            <div class="badge">
              <span class="badge-icon">↩️</span>
              <span class="badge-text">{{
                $t('legal.checkout.orderSummary.trustBadges.returns')
              }}</span>
            </div>
            <div class="badge">
              <span class="badge-icon">🚚</span>
              <span class="badge-text">{{
                $t('legal.checkout.orderSummary.trustBadges.insured')
              }}</span>
            </div>
          </div>
        </aside>

        <!-- Main Checkout Form -->
        <main class="checkout-form-main">
          <!-- STEP 1: Shipping Address -->
          <section v-if="currentStep === 'shipping'" class="checkout-step">
            <h2>{{ $t('legal.checkout.steps.shippingAddress') }}</h2>
            <p class="step-description">{{ $t('legal.checkout.form.description') }}</p>

            <form @submit.prevent="nextStep">
              <div class="form-grid">
                <div class="form-group">
                  <label for="firstName">{{ $t('legal.checkout.form.labels.firstName') }}</label>
                  <input
                    id="firstName"
                    v-model="form.firstName"
                    type="text"
                    :placeholder="$t('legal.checkout.form.placeholders.firstName')"
                    required
                    :aria-invalid="!!errors.firstName"
                    :aria-describedby="errors.firstName ? 'firstName-error' : undefined"
                  />
                  <p v-if="errors.firstName" id="firstName-error" class="error-message">
                    {{ errors.firstName }}
                  </p>
                </div>

                <div class="form-group">
                  <label for="lastName">{{ $t('legal.checkout.form.labels.lastName') }}</label>
                  <input
                    id="lastName"
                    v-model="form.lastName"
                    type="text"
                    :placeholder="$t('legal.checkout.form.placeholders.lastName')"
                    required
                    :aria-invalid="!!errors.lastName"
                    :aria-describedby="errors.lastName ? 'lastName-error' : undefined"
                  />
                  <p v-if="errors.lastName" id="lastName-error" class="error-message">
                    {{ errors.lastName }}
                  </p>
                </div>

                <div class="form-group full-width">
                  <label for="street">{{ $t('legal.checkout.form.labels.streetAddress') }}</label>
                  <input
                    id="street"
                    v-model="form.street"
                    type="text"
                    :placeholder="$t('legal.checkout.form.placeholders.streetAddress')"
                    required
                    :aria-invalid="!!errors.street"
                    :aria-describedby="errors.street ? 'street-error' : undefined"
                  />
                  <p v-if="errors.street" id="street-error" class="error-message">
                    {{ errors.street }}
                  </p>
                </div>

                <div class="form-group">
                  <label for="zipCode">{{ $t('legal.checkout.form.labels.postalCode') }}</label>
                  <input
                    id="zipCode"
                    v-model="form.zipCode"
                    type="text"
                    :placeholder="$t('legal.checkout.form.placeholders.postalCode')"
                    pattern="\d{5}"
                    required
                    :aria-invalid="!!errors.zipCode"
                    :aria-describedby="errors.zipCode ? 'zipCode-error' : undefined"
                  />
                  <p v-if="errors.zipCode" id="zipCode-error" class="error-message">
                    {{ errors.zipCode }}
                  </p>
                </div>

                <div class="form-group">
                  <label for="city">{{ $t('legal.checkout.form.labels.city') }}</label>
                  <input
                    id="city"
                    v-model="form.city"
                    type="text"
                    :placeholder="$t('legal.checkout.form.placeholders.city')"
                    required
                    :aria-invalid="!!errors.city"
                    :aria-describedby="errors.city ? 'city-error' : undefined"
                  />
                  <p v-if="errors.city" id="city-error" class="error-message">
                    {{ errors.city }}
                  </p>
                </div>

                <div class="form-group">
                  <label for="country">{{ $t('legal.checkout.form.labels.country') }}</label>
                  <select
                    id="country"
                    v-model="form.country"
                    required
                    :aria-invalid="!!errors.country"
                    :aria-describedby="errors.country ? 'country-error' : undefined"
                  >
                    <option value="Germany">
                      {{ $t('legal.checkout.form.countries.germany') }}
                    </option>
                    <option value="Austria">
                      {{ $t('legal.checkout.form.countries.austria') }}
                    </option>
                    <option value="Belgium">
                      {{ $t('legal.checkout.form.countries.belgium') }}
                    </option>
                    <option value="France">{{ $t('legal.checkout.form.countries.france') }}</option>
                    <option value="Netherlands">
                      {{ $t('legal.checkout.form.countries.netherlands') }}
                    </option>
                  </select>
                  <p v-if="errors.country" id="country-error" class="error-message">
                    {{ errors.country }}
                  </p>
                </div>
              </div>

              <div class="step-actions">
                <button type="button" class="btn btn-secondary" @click="goBack">
                  {{ $t('legal.checkout.buttons.backToCart') }}
                </button>
                <button type="submit" class="btn btn-primary" :disabled="!isFormValid">
                  {{ $t('legal.checkout.buttons.continueToShipping') }}
                </button>
              </div>
            </form>
          </section>

          <!-- STEP 2: Shipping Method Selection -->
          <section v-if="currentStep === 'shipping-method'" class="checkout-step">
            <h2>{{ $t('legal.checkout.steps.shippingMethod') }}</h2>
            <p class="step-description">{{ $t('legal.checkout.shipping.description') }}</p>

            <div class="shipping-options">
              <div
                v-for="method in shippingMethods"
                :key="method.id"
                class="shipping-option"
                :class="{ selected: selectedShippingMethod.id === method.id }"
                @click="selectShippingMethod(method)"
                role="radio"
                :aria-checked="selectedShippingMethod.id === method.id"
                tabindex="0"
                @keyup.enter="selectShippingMethod(method)"
              >
                <div class="shipping-radio">
                  <input
                    type="radio"
                    :name="`shipping-${method.id}`"
                    :checked="selectedShippingMethod.id === method.id"
                    :aria-label="`${method.name}: ${formatPrice(method.price)}`"
                  />
                </div>
                <div class="shipping-info">
                  <div class="shipping-name">{{ method.name }}</div>
                  <div class="shipping-description">
                    {{ method.description }}
                  </div>
                  <div class="shipping-days">⏱️ Lieferzeit: ca. {{ method.days }} Werktag(e)</div>
                </div>
                <div class="shipping-price">
                  {{ formatPrice(method.price) }}
                </div>
              </div>
            </div>

            <p v-if="errors.shippingMethod" class="error-message">
              {{ errors.shippingMethod }}
            </p>

            <div class="step-actions">
              <button type="button" class="btn btn-secondary" @click="prevStep">
                {{ $t('legal.checkout.buttons.backToAddress') }}
              </button>
              <button
                type="button"
                class="btn btn-primary"
                @click="nextStep"
                :disabled="!selectedShippingMethod"
              >
                {{ $t('legal.checkout.buttons.continueToReview') }}
              </button>
            </div>
          </section>

          <!-- STEP 3: Order Review & Payment -->
          <section v-if="currentStep === 'review'" class="checkout-step">
            <h2>{{ $t('legal.checkout.orderReview.title') }}</h2>

            <!-- Address Review -->
            <div class="review-section">
              <h3>{{ $t('legal.checkout.orderReview.shippingAddress') }}</h3>
              <div class="review-content">
                <p>
                  {{ form.firstName }} {{ form.lastName }}<br />
                  {{ form.street }}<br />
                  {{ form.zipCode }} {{ form.city }}<br />
                  {{ form.country }}
                </p>
                <button type="button" class="link-button" @click="currentStep = 'shipping'">
                  {{ $t('legal.checkout.orderReview.edit') }}
                </button>
              </div>
            </div>

            <!-- Shipping Review -->
            <div class="review-section">
              <h3>{{ $t('legal.checkout.orderReview.shippingMethod') }}</h3>
              <div class="review-content">
                <p>
                  <strong>{{ selectedShippingMethod.name }}</strong
                  ><br />
                  {{ selectedShippingMethod.description }}<br />
                  Kosten: {{ formatPrice(selectedShippingMethod.price) }}
                </p>
                <button type="button" class="link-button" @click="currentStep = 'shipping-method'">
                  {{ $t('legal.checkout.orderReview.edit') }}
                </button>
              </div>
            </div>

            <!-- Payment Method Selection -->
            <div class="review-section">
              <h3>{{ $t('legal.checkout.orderReview.paymentMethod') }}</h3>
              <div class="payment-options">
                <div
                  v-for="method in paymentMethods"
                  :key="method.id"
                  class="payment-option"
                  :class="{ selected: selectedPaymentMethod?.id === method.id }"
                  @click="selectPaymentMethod(method)"
                  role="radio"
                  :aria-checked="selectedPaymentMethod?.id === method.id"
                  tabindex="0"
                  @keyup.enter="selectPaymentMethod(method)"
                >
                  <div class="payment-radio">
                    <input
                      type="radio"
                      :name="`payment-${method.id}`"
                      :checked="selectedPaymentMethod?.id === method.id"
                      :aria-label="method.name"
                    />
                  </div>
                  <div class="payment-icon">{{ method.icon }}</div>
                  <div class="payment-info">
                    <div class="payment-name">{{ method.name }}</div>
                    <div class="payment-description">
                      {{ method.description }}
                    </div>
                  </div>
                </div>
              </div>

              <p v-if="errors.payment" class="error-message">
                {{ errors.payment }}
              </p>
            </div>

            <!-- Terms & Conditions -->
            <div class="terms-section">
              <label class="terms-checkbox">
                <input
                  v-model="agreedToTerms"
                  type="checkbox"
                  required
                  :aria-invalid="!!errors.terms"
                  :aria-describedby="errors.terms ? 'terms-error' : undefined"
                />
                <span>
                  {{ $t('legal.checkout.terms.acceptText') }}
                  <router-link to="/terms">{{ $t('legal.checkout.terms.termsLink') }}</router-link>
                  {{ $t('legal.checkout.terms.and') }}
                  <router-link to="/privacy">{{
                    $t('legal.checkout.terms.privacyLink')
                  }}</router-link>
                  {{ $t('legal.checkout.terms.required') }}
                </span>
              </label>
              <p v-if="errors.terms" id="terms-error" class="error-message">
                {{ errors.terms }}
              </p>
            </div>

            <!-- Compliance Notice -->
            <div class="compliance-notice">
              <p class="notice-icon">✓</p>
              <div class="notice-content">
                <p class="notice-title">{{ $t('legal.checkout.compliance.title') }}</p>
                <p class="notice-text">
                  {{ $t('legal.checkout.compliance.content') }}
                </p>
              </div>
            </div>

            <p v-if="errors.submit" class="error-message submit-error">
              {{ errors.submit }}
            </p>

            <div class="step-actions">
              <button type="button" class="btn btn-secondary" @click="prevStep">
                {{ $t('legal.checkout.buttons.backToShipping') }}
              </button>
              <button
                type="button"
                class="btn btn-primary btn-submit"
                @click="completeOrder"
                :disabled="!isFormValid || isSubmitting"
                :aria-busy="isSubmitting"
              >
                {{
                  isSubmitting
                    ? 'Bestellung wird verarbeitet...'
                    : `Bestellung abschließen (${formatPrice(total)})`
                }}
              </button>
            </div>
          </section>
        </main>
      </div>
    </div>
  </div>
</template>

<style scoped>
.checkout-page {
  min-height: 100vh;
  background-color: #f8f9fa;
  padding: 2rem 1rem;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
}

/* Header */
.page-header {
  margin-bottom: 2rem;
}

.page-header h1 {
  font-size: 2rem;
  margin: 0 0 0.5rem 0;
  color: #1a1a1a;
}

.breadcrumb {
  font-size: 0.875rem;
  color: #666;
  margin: 0;
}

.breadcrumb a {
  color: #0066cc;
  text-decoration: none;
}

.breadcrumb a:hover {
  text-decoration: underline;
}

/* Progress Indicator */
.progress-container {
  margin-bottom: 3rem;
}

.progress-steps {
  display: flex;
  justify-content: space-between;
  margin-bottom: 2rem;
  position: relative;
  z-index: 1;
}

.progress-step {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  flex: 1;
  position: relative;
}

.progress-step::before {
  content: '';
  position: absolute;
  top: 1.5rem;
  left: 0;
  right: 0;
  height: 2px;
  background-color: #e0e0e0;
  z-index: -1;
}

.progress-step:first-child::before {
  left: 50%;
}

.progress-step:last-child::before {
  right: 50%;
}

.progress-step.completed::before,
.progress-step.active::before {
  background-color: #4caf50;
}

.step-number {
  width: 3rem;
  height: 3rem;
  border-radius: 50%;
  background-color: #f0f0f0;
  border: 2px solid #ddd;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  color: #666;
  transition: all 0.3s;
}

.progress-step.active .step-number {
  background-color: #0066cc;
  border-color: #0066cc;
  color: white;
  box-shadow: 0 0 0 4px rgba(0, 102, 204, 0.1);
}

.progress-step.completed .step-number {
  background-color: #4caf50;
  border-color: #4caf50;
  color: white;
}

.step-label {
  font-size: 0.875rem;
  color: #666;
  font-weight: 500;
  text-align: center;
}

.progress-step.active .step-label,
.progress-step.completed .step-label {
  color: #1a1a1a;
  font-weight: 600;
}

.progress-bar {
  height: 4px;
  background-color: #e0e0e0;
  border-radius: 2px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #0066cc, #4caf50);
  transition: width 0.3s ease;
}

/* Main Content Grid */
.checkout-content {
  display: grid;
  grid-template-columns: 1fr 2fr;
  gap: 2rem;
  margin-bottom: 3rem;
}

/* Order Summary Sidebar */
.order-summary-sidebar {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  height: fit-content;
  position: sticky;
  top: 2rem;
}

.order-summary-sidebar h3 {
  font-size: 1.25rem;
  margin: 0 0 1rem 0;
  color: #1a1a1a;
  border-bottom: 2px solid #f0f0f0;
  padding-bottom: 0.75rem;
}

.order-items {
  margin-bottom: 1rem;
  max-height: 300px;
  overflow-y: auto;
}

.order-item-summary {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 0;
  border-bottom: 1px solid #f0f0f0;
  font-size: 0.9rem;
}

.order-item-summary:last-child {
  border-bottom: none;
}

.item-summary-info {
  flex: 1;
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.item-name {
  color: #1a1a1a;
  font-weight: 500;
}

.item-qty {
  color: #999;
  font-size: 0.8rem;
  background-color: #f5f5f5;
  padding: 0.25rem 0.5rem;
  border-radius: 3px;
}

.item-summary-price {
  font-weight: 600;
  color: #0066cc;
}

/* Price Summary */
.price-summary {
  background-color: #f9f9f9;
  padding: 1rem;
  border-radius: 6px;
  margin: 1rem 0;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  padding: 0.5rem 0;
  font-size: 0.9rem;
  color: #666;
}

.summary-row.vat {
  background-color: rgba(76, 175, 80, 0.05);
  padding: 0.5rem;
  border-radius: 4px;
  margin: 0.25rem -1rem;
  padding-left: 0.5rem;
  color: #2e7d32;
  font-weight: 500;
}

.summary-row.shipping {
  color: #ff9800;
}

.summary-row.total {
  border-top: 2px solid #ddd;
  padding-top: 0.75rem;
  font-size: 1rem;
  font-weight: 600;
  color: #1a1a1a;
}

.total-amount {
  color: #0066cc;
  font-size: 1.2rem;
}

/* Trust Badges */
.trust-badges {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 1px solid #e0e0e0;
}

.badge {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 0.8rem;
  color: #555;
}

.badge-icon {
  font-size: 1.2rem;
}

.badge-text {
  line-height: 1.3;
}

/* Main Form Section */
.checkout-form-main {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.checkout-step {
  animation: slideIn 0.3s ease;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.checkout-step h2 {
  font-size: 1.5rem;
  margin: 0 0 0.5rem 0;
  color: #1a1a1a;
}

.step-description {
  color: #666;
  margin: 0 0 1.5rem 0;
  font-size: 0.95rem;
}

/* Forms */
.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.form-group.full-width {
  grid-column: 1 / -1;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  font-weight: 500;
  margin-bottom: 0.5rem;
  color: #1a1a1a;
  font-size: 0.95rem;
}

.form-group input,
.form-group select {
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
  font-family: inherit;
  transition: all 0.2s;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #0066cc;
  box-shadow: 0 0 0 3px rgba(0, 102, 204, 0.1);
}

.form-group input[aria-invalid='true'],
.form-group select[aria-invalid='true'] {
  border-color: #d32f2f;
}

.error-message {
  font-size: 0.8rem;
  color: #d32f2f;
  margin-top: 0.25rem;
}

/* Shipping Options */
.shipping-options {
  display: grid;
  grid-template-columns: 1fr;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.shipping-option {
  display: flex;
  gap: 1rem;
  padding: 1rem;
  border: 2px solid #e0e0e0;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s;
}

.shipping-option:hover {
  border-color: #0066cc;
  background-color: rgba(0, 102, 204, 0.02);
}

.shipping-option.selected {
  border-color: #0066cc;
  background-color: rgba(0, 102, 204, 0.05);
}

.shipping-radio {
  flex-shrink: 0;
  padding-top: 0.25rem;
}

.shipping-radio input {
  cursor: pointer;
  width: 20px;
  height: 20px;
}

.shipping-info {
  flex: 1;
}

.shipping-name {
  font-weight: 600;
  color: #1a1a1a;
  margin-bottom: 0.25rem;
}

.shipping-description {
  font-size: 0.9rem;
  color: #666;
}

.shipping-days {
  font-size: 0.85rem;
  color: #999;
  margin-top: 0.25rem;
}

.shipping-price {
  font-size: 1.1rem;
  font-weight: 600;
  color: #0066cc;
  align-self: center;
}

/* Review Sections */
.review-section {
  margin-bottom: 1.5rem;
  padding-bottom: 1.5rem;
  border-bottom: 1px solid #e0e0e0;
}

.review-section h3 {
  font-size: 1rem;
  font-weight: 600;
  color: #1a1a1a;
  margin: 0 0 0.75rem 0;
}

.review-content {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 1rem;
}

.review-content p {
  margin: 0;
  color: #666;
  font-size: 0.95rem;
  line-height: 1.6;
}

.link-button {
  background: none;
  border: none;
  color: #0066cc;
  cursor: pointer;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  transition: background-color 0.2s;
  font-weight: 500;
  white-space: nowrap;
  flex-shrink: 0;
}

.link-button:hover {
  background-color: rgba(0, 102, 204, 0.1);
}

/* Payment Options */
.payment-options {
  display: grid;
  grid-template-columns: 1fr;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.payment-option {
  display: flex;
  gap: 1rem;
  padding: 1rem;
  border: 2px solid #e0e0e0;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s;
  align-items: center;
}

.payment-option:hover {
  border-color: #0066cc;
  background-color: rgba(0, 102, 204, 0.02);
}

.payment-option.selected {
  border-color: #0066cc;
  background-color: rgba(0, 102, 204, 0.05);
}

.payment-radio {
  flex-shrink: 0;
}

.payment-radio input {
  cursor: pointer;
  width: 20px;
  height: 20px;
}

.payment-icon {
  font-size: 1.5rem;
  flex-shrink: 0;
}

.payment-info {
  flex: 1;
}

.payment-name {
  font-weight: 600;
  color: #1a1a1a;
}

.payment-description {
  font-size: 0.85rem;
  color: #999;
  margin-top: 0.25rem;
}

/* Terms Section */
.terms-section {
  margin-bottom: 1.5rem;
  padding: 1rem;
  background-color: #f9f9f9;
  border-radius: 6px;
}

.terms-checkbox {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  cursor: pointer;
  font-size: 0.95rem;
  color: #555;
  line-height: 1.6;
}

.terms-checkbox input {
  margin-top: 0.25rem;
  cursor: pointer;
  width: auto;
  height: auto;
}

.terms-checkbox a {
  color: #0066cc;
  text-decoration: none;
}

.terms-checkbox a:hover {
  text-decoration: underline;
}

/* Compliance Notice */
.compliance-notice {
  display: flex;
  gap: 1rem;
  padding: 1rem;
  background-color: rgba(76, 175, 80, 0.08);
  border-left: 4px solid #4caf50;
  border-radius: 4px;
  margin-bottom: 1.5rem;
}

.notice-icon {
  font-size: 1.5rem;
  color: #4caf50;
  margin: 0;
  min-width: 2rem;
  text-align: center;
}

.notice-content {
  flex: 1;
}

.notice-title {
  font-weight: 600;
  margin: 0 0 0.5rem 0;
  color: #1a1a1a;
  font-size: 0.95rem;
}

.notice-text {
  margin: 0;
  color: #555;
  font-size: 0.85rem;
  line-height: 1.5;
}

.submit-error {
  background-color: rgba(211, 47, 47, 0.1);
  padding: 0.75rem;
  border-radius: 4px;
  border-left: 3px solid #d32f2f;
}

/* Step Actions */
.step-actions {
  display: flex;
  gap: 1rem;
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid #e0e0e0;
}

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  min-height: 44px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-primary {
  background-color: #0066cc;
  color: white;
  flex: 1;
}

.btn-primary:hover:not(:disabled) {
  background-color: #0052a3;
  box-shadow: 0 2px 8px rgba(0, 102, 204, 0.25);
}

.btn-primary:disabled {
  background-color: #ccc;
  cursor: not-allowed;
  opacity: 0.6;
}

.btn-secondary {
  background-color: #f0f0f0;
  color: #1a1a1a;
}

.btn-secondary:hover {
  background-color: #e0e0e0;
}

.btn-submit {
  background: linear-gradient(135deg, #0066cc, #0052a3);
  font-size: 1.05rem;
}

.btn-submit:hover:not(:disabled) {
  box-shadow: 0 4px 12px rgba(0, 102, 204, 0.3);
}

/* Mobile Responsiveness */
@media (max-width: 768px) {
  .checkout-content {
    grid-template-columns: 1fr;
  }

  .order-summary-sidebar {
    position: static;
    top: auto;
  }

  .form-grid {
    grid-template-columns: 1fr;
  }

  .progress-steps {
    gap: 0;
  }

  .step-number {
    width: 2.5rem;
    height: 2.5rem;
    font-size: 0.9rem;
  }

  .step-label {
    font-size: 0.75rem;
  }

  .checkout-form-main {
    padding: 1.5rem 1rem;
  }

  .step-actions {
    flex-direction: column;
  }

  .btn {
    width: 100%;
  }
}

@media (max-width: 480px) {
  .checkout-page {
    padding: 1rem 0.5rem;
  }

  .page-header {
    margin-bottom: 1rem;
  }

  .page-header h1 {
    font-size: 1.5rem;
  }

  .progress-container {
    margin-bottom: 2rem;
  }

  .step-number {
    width: 2.25rem;
    height: 2.25rem;
    font-size: 0.8rem;
  }

  .form-group label {
    font-size: 0.85rem;
  }

  .form-group input,
  .form-group select {
    padding: 0.65rem;
    font-size: 16px;
  }

  .shipping-option,
  .payment-option {
    padding: 0.75rem;
    flex-direction: column;
    text-align: center;
  }

  .shipping-price {
    align-self: auto;
  }
}
</style>
