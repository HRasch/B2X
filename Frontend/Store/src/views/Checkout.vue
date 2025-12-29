<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { useRouter } from "vue-router";
import { useCartStore } from "@/stores/cartStore";

const router = useRouter();
const cartStore = useCartStore();

interface ShippingForm {
  firstName: string;
  lastName: string;
  street: string;
  zipCode: string;
  city: string;
  country: string;
}

const form = ref<ShippingForm>({
  firstName: "",
  lastName: "",
  street: "",
  zipCode: "",
  city: "",
  country: "Germany",
});

const agreedToTerms = ref(false);
const isSubmitting = ref(false);
const errors = ref<Record<string, string>>({});

// Computed properties
const subtotal = computed(() => {
  return cartStore.items.reduce(
    (sum, item) => sum + item.price * item.quantity,
    0
  );
});

const vatAmount = computed(() => {
  return Number((subtotal.value * 0.19).toFixed(2));
});

const shippingCost = computed(() => 5.99);

const total = computed(() => {
  return Number(
    (subtotal.value + vatAmount.value + shippingCost.value).toFixed(2)
  );
});

const isFormValid = computed(() => {
  return (
    form.value.firstName.trim() !== "" &&
    form.value.lastName.trim() !== "" &&
    form.value.street.trim() !== "" &&
    form.value.zipCode.trim() !== "" &&
    form.value.city.trim() !== "" &&
    form.value.country.trim() !== "" &&
    agreedToTerms.value
  );
});

// Methods
const formatPrice = (price: number): string => {
  return new Intl.NumberFormat("de-DE", {
    style: "currency",
    currency: "EUR",
  }).format(price);
};

const validateForm = (): boolean => {
  errors.value = {};

  if (!form.value.firstName.trim()) {
    errors.value.firstName = "Vorname ist erforderlich";
  }

  if (!form.value.lastName.trim()) {
    errors.value.lastName = "Nachname ist erforderlich";
  }

  if (!form.value.street.trim()) {
    errors.value.street = "Straße und Hausnummer sind erforderlich";
  }

  if (!form.value.zipCode.trim()) {
    errors.value.zipCode = "Postleitzahl ist erforderlich";
  } else if (!/^\d{5}$/.test(form.value.zipCode)) {
    errors.value.zipCode = "Postleitzahl muss 5 Ziffern haben";
  }

  if (!form.value.city.trim()) {
    errors.value.city = "Stadt ist erforderlich";
  }

  return Object.keys(errors.value).length === 0;
};

const completeOrder = async () => {
  if (!validateForm()) {
    console.error("Form validation failed");
    return;
  }

  if (!agreedToTerms.value) {
    errors.value.terms = "Sie müssen den Bedingungen zustimmen";
    return;
  }

  isSubmitting.value = true;

  try {
    // Simulate API call to create order
    await new Promise((resolve) => setTimeout(resolve, 1500));

    console.log("Order completed with data:", {
      items: cartStore.items,
      shipping: form.value,
      total: total.value,
    });

    // Clear cart
    cartStore.clearCart();

    // Redirect to confirmation page
    await router.push("/order-confirmation");
  } catch (error) {
    console.error("Error completing order:", error);
    errors.value.submit =
      "Fehler beim Abschließen der Bestellung. Bitte versuchen Sie es später erneut.";
  } finally {
    isSubmitting.value = false;
  }
};

const goBack = () => {
  router.push("/cart");
};

onMounted(() => {
  if (cartStore.items.length === 0) {
    router.push("/shop");
  }
});
</script>

<template>
  <div class="checkout-page">
    <div class="container">
      <div class="page-header">
        <h1>Bestellübersicht</h1>
        <p class="breadcrumb">
          <router-link to="/shop">Shop</router-link> /
          <router-link to="/cart">Warenkorb</router-link> / Kasse
        </p>
      </div>

      <div class="checkout-content">
        <!-- Order Summary Section -->
        <section class="order-review">
          <h2>Ihre Bestellung</h2>

          <div class="order-items">
            <div
              v-for="item in cartStore.items"
              :key="item.id"
              class="order-item"
            >
              <div class="item-info">
                <h3>{{ item.name }}</h3>
                <p class="item-details">
                  Menge: {{ item.quantity }} × {{ formatPrice(item.price) }}
                </p>
              </div>
              <div class="item-total">
                {{ formatPrice(item.price * item.quantity) }}
              </div>
            </div>
          </div>

          <!-- Price Breakdown -->
          <div class="price-breakdown">
            <div class="breakdown-row">
              <span class="label">Netto-Summe:</span>
              <span class="value">{{ formatPrice(subtotal) }}</span>
            </div>
            <div class="breakdown-row vat-row">
              <span class="label">zzgl. MwSt (19%):</span>
              <span class="value">{{ formatPrice(vatAmount) }}</span>
            </div>
            <div class="breakdown-row shipping-row">
              <span class="label">Versandkosten:</span>
              <span class="value">{{ formatPrice(shippingCost) }}</span>
            </div>
            <div class="breakdown-row total-row">
              <span class="label">
                <strong>Gesamtbetrag (inkl. MwSt):</strong>
              </span>
              <span class="value total-value">
                <strong>{{ formatPrice(total) }}</strong>
              </span>
            </div>
          </div>

          <!-- Compliance Notice -->
          <div class="compliance-notice">
            <p class="notice-icon">✓</p>
            <div class="notice-content">
              <p class="notice-title">Preisangabenverordnung (PAngV)</p>
              <p class="notice-text">
                Alle angezeigten Preise sind Endpreise und enthalten bereits die
                gesetzliche Mehrwertsteuer (MwSt) in Höhe von 19%.
              </p>
            </div>
          </div>
        </section>

        <!-- Checkout Form Section -->
        <section class="checkout-form">
          <h2>Lieferadresse</h2>

          <form @submit.prevent="completeOrder">
            <div class="form-grid">
              <!-- First Name -->
              <div class="form-group">
                <label for="firstName">Vorname *</label>
                <input
                  id="firstName"
                  v-model="form.firstName"
                  type="text"
                  required
                  :aria-invalid="!!errors.firstName"
                  :aria-describedby="
                    errors.firstName ? 'firstName-error' : undefined
                  "
                />
                <p
                  v-if="errors.firstName"
                  id="firstName-error"
                  class="error-message"
                >
                  {{ errors.firstName }}
                </p>
              </div>

              <!-- Last Name -->
              <div class="form-group">
                <label for="lastName">Nachname *</label>
                <input
                  id="lastName"
                  v-model="form.lastName"
                  type="text"
                  required
                  :aria-invalid="!!errors.lastName"
                  :aria-describedby="
                    errors.lastName ? 'lastName-error' : undefined
                  "
                />
                <p
                  v-if="errors.lastName"
                  id="lastName-error"
                  class="error-message"
                >
                  {{ errors.lastName }}
                </p>
              </div>

              <!-- Street -->
              <div class="form-group full-width">
                <label for="street">Straße und Hausnummer *</label>
                <input
                  id="street"
                  v-model="form.street"
                  type="text"
                  required
                  :aria-invalid="!!errors.street"
                  :aria-describedby="errors.street ? 'street-error' : undefined"
                />
                <p v-if="errors.street" id="street-error" class="error-message">
                  {{ errors.street }}
                </p>
              </div>

              <!-- Zip Code -->
              <div class="form-group">
                <label for="zipCode">Postleitzahl *</label>
                <input
                  id="zipCode"
                  v-model="form.zipCode"
                  type="text"
                  required
                  pattern="\d{5}"
                  :aria-invalid="!!errors.zipCode"
                  :aria-describedby="
                    errors.zipCode ? 'zipCode-error' : undefined
                  "
                />
                <p
                  v-if="errors.zipCode"
                  id="zipCode-error"
                  class="error-message"
                >
                  {{ errors.zipCode }}
                </p>
              </div>

              <!-- City -->
              <div class="form-group">
                <label for="city">Stadt *</label>
                <input
                  id="city"
                  v-model="form.city"
                  type="text"
                  required
                  :aria-invalid="!!errors.city"
                  :aria-describedby="errors.city ? 'city-error' : undefined"
                />
                <p v-if="errors.city" id="city-error" class="error-message">
                  {{ errors.city }}
                </p>
              </div>

              <!-- Country -->
              <div class="form-group">
                <label for="country">Land *</label>
                <select
                  id="country"
                  v-model="form.country"
                  required
                  :aria-invalid="!!errors.country"
                  :aria-describedby="
                    errors.country ? 'country-error' : undefined
                  "
                >
                  <option value="Germany">Deutschland</option>
                  <option value="Austria">Österreich</option>
                  <option value="Belgium">Belgien</option>
                  <option value="France">Frankreich</option>
                  <option value="Netherlands">Niederlande</option>
                </select>
                <p
                  v-if="errors.country"
                  id="country-error"
                  class="error-message"
                >
                  {{ errors.country }}
                </p>
              </div>
            </div>

            <!-- Terms Acceptance -->
            <div class="form-group terms-group">
              <label for="terms" class="terms-label">
                <input
                  id="terms"
                  v-model="agreedToTerms"
                  type="checkbox"
                  required
                  :aria-invalid="!!errors.terms"
                  :aria-describedby="errors.terms ? 'terms-error' : undefined"
                />
                <span>
                  Ich akzeptiere die
                  <router-link to="/terms">Geschäftsbedingungen</router-link>
                  und die
                  <router-link to="/privacy">Datenschutzerklärung</router-link>
                  *
                </span>
              </label>
              <p v-if="errors.terms" id="terms-error" class="error-message">
                {{ errors.terms }}
              </p>
            </div>

            <!-- Submit Error -->
            <p v-if="errors.submit" class="error-message submit-error">
              {{ errors.submit }}
            </p>

            <!-- Action Buttons -->
            <div class="form-actions">
              <button type="button" class="btn btn-secondary" @click="goBack">
                Zurück zum Warenkorb
              </button>
              <button
                type="submit"
                class="btn btn-primary"
                :disabled="!isFormValid || isSubmitting"
                :aria-busy="isSubmitting"
              >
                {{
                  isSubmitting
                    ? "Wird verarbeitet..."
                    : "Bestellung abschließen"
                }}
              </button>
            </div>
          </form>
        </section>
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

.page-header {
  margin-bottom: 2rem;
}

.page-header h1 {
  font-size: 2.5rem;
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

.checkout-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
  margin-bottom: 3rem;
}

/* Order Review Section */
.order-review {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.order-review h2 {
  font-size: 1.5rem;
  margin: 0 0 1.5rem 0;
  color: #1a1a1a;
  border-bottom: 2px solid #f0f0f0;
  padding-bottom: 1rem;
}

.order-items {
  margin-bottom: 2rem;
}

.order-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem 0;
  border-bottom: 1px solid #f0f0f0;
}

.order-item:last-child {
  border-bottom: none;
}

.item-info h3 {
  font-size: 1.125rem;
  margin: 0 0 0.5rem 0;
  color: #1a1a1a;
}

.item-details {
  font-size: 0.875rem;
  color: #666;
  margin: 0;
}

.item-total {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1a1a1a;
}

/* Price Breakdown */
.price-breakdown {
  background: #fafafa;
  padding: 1.5rem;
  border-radius: 6px;
  margin-bottom: 1.5rem;
}

.breakdown-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 0;
  font-size: 0.95rem;
}

.breakdown-row .label {
  color: #666;
}

.breakdown-row .value {
  font-weight: 500;
  color: #1a1a1a;
}

.breakdown-row.vat-row {
  background-color: rgba(76, 175, 80, 0.05);
  padding: 0.75rem 1rem;
  border-radius: 4px;
  margin: 0.5rem -1.5rem;
  padding-left: 0.5rem;
  padding-right: 0.5rem;
}

.breakdown-row.vat-row .label {
  color: #2e7d32;
  font-weight: 500;
}

.breakdown-row.shipping-row {
  color: #ff9800;
}

.breakdown-row.total-row {
  border-top: 2px solid #ddd;
  padding-top: 1rem;
  margin-top: 1rem;
  font-size: 1.125rem;
}

.breakdown-row.total-row .value {
  font-size: 1.35rem;
  color: #1a1a1a;
}

.total-value {
  color: #0066cc;
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
  font-size: 0.875rem;
  line-height: 1.5;
}

/* Checkout Form Section */
.checkout-form {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  height: fit-content;
  position: sticky;
  top: 2rem;
}

.checkout-form h2 {
  font-size: 1.5rem;
  margin: 0 0 1.5rem 0;
  color: #1a1a1a;
  border-bottom: 2px solid #f0f0f0;
  padding-bottom: 1rem;
}

/* Form Grid */
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
  transition: border-color 0.2s;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #0066cc;
  box-shadow: 0 0 0 3px rgba(0, 102, 204, 0.1);
}

.form-group input[aria-invalid="true"],
.form-group select[aria-invalid="true"] {
  border-color: #d32f2f;
}

.error-message {
  font-size: 0.75rem;
  color: #d32f2f;
  margin: 0.25rem 0 0 0;
}

.submit-error {
  background-color: rgba(211, 47, 47, 0.1);
  padding: 0.75rem;
  border-radius: 4px;
  border-left: 3px solid #d32f2f;
  margin-bottom: 1rem;
}

/* Terms Group */
.terms-group {
  margin-bottom: 1.5rem;
}

.terms-label {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  font-weight: normal;
  margin-bottom: 0;
  cursor: pointer;
}

.terms-label input {
  margin-top: 0.25rem;
  cursor: pointer;
  width: auto;
  height: auto;
}

.terms-label span {
  font-size: 0.95rem;
  color: #555;
  line-height: 1.5;
}

.terms-label a {
  color: #0066cc;
  text-decoration: none;
}

.terms-label a:hover {
  text-decoration: underline;
}

/* Form Actions */
.form-actions {
  display: flex;
  gap: 1rem;
  margin-top: 2rem;
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
}

.btn-primary:disabled {
  background-color: #ccc;
  cursor: not-allowed;
  opacity: 0.6;
}

.btn-secondary {
  background-color: #f0f0f0;
  color: #1a1a1a;
  flex: 0 0 auto;
}

.btn-secondary:hover {
  background-color: #e0e0e0;
}

/* Mobile Responsiveness */
@media (max-width: 768px) {
  .checkout-content {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }

  .checkout-form {
    position: static;
    top: auto;
  }

  .form-grid {
    grid-template-columns: 1fr;
  }

  .page-header h1 {
    font-size: 1.75rem;
  }

  .order-review,
  .checkout-form {
    padding: 1.5rem 1rem;
  }

  .order-item {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.5rem;
  }

  .item-total {
    align-self: flex-end;
  }

  .compliance-notice {
    flex-direction: column;
    gap: 0.5rem;
  }

  .form-actions {
    flex-direction: column;
    gap: 0.75rem;
  }

  .btn {
    width: 100%;
  }

  .btn-secondary {
    flex: 1;
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
    margin-bottom: 0.25rem;
  }

  .order-review h2,
  .checkout-form h2 {
    font-size: 1.25rem;
  }

  .form-group label {
    font-size: 0.875rem;
  }

  .form-group input,
  .form-group select {
    padding: 0.65rem;
    font-size: 16px;
  }

  .breakdown-row {
    font-size: 0.875rem;
  }

  .breakdown-row.total-row {
    font-size: 1rem;
  }

  .breakdown-row.total-row .value {
    font-size: 1.15rem;
  }
}
</style>
