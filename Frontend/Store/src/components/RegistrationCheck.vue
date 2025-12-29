<template>
  <div class="registration-check-container">
    <!-- Page Header -->
    <div class="header-section">
      <h1>Registrierungstyp Prüfen</h1>
      <p class="subtitle">
        Überprüfen Sie, ob Sie bereits als Bestandskunde registriert sind
      </p>
    </div>

    <!-- Error Alert -->
    <Transition name="fade">
      <div v-if="error" class="alert alert-error" data-testid="error-message">
        <span class="alert-icon">⚠️</span>
        <div class="alert-content">
          <h4>Fehler</h4>
          <p>{{ error }}</p>
        </div>
        <button class="alert-close" @click="error = null">✕</button>
      </div>
    </Transition>

    <!-- Success Alert -->
    <Transition name="fade">
      <div
        v-if="successMessage"
        class="alert alert-success"
        data-testid="success-message"
      >
        <span class="alert-icon">✓</span>
        <div class="alert-content">
          <p>{{ successMessage }}</p>
        </div>
        <button class="alert-close" @click="successMessage = null">✕</button>
      </div>
    </Transition>

    <!-- Registration Check Form -->
    <form @submit.prevent="handleCheckRegistration" class="registration-form">
      <div class="form-grid">
        <!-- Email Input -->
        <div class="form-group">
          <label for="email" class="form-label">
            E-Mail-Adresse <span class="required">*</span>
          </label>
          <input
            id="email"
            v-model="formData.email"
            type="email"
            class="form-input"
            placeholder="beispiel@unternehmen.de"
            required
            data-testid="email-input"
            @blur="validateEmailField"
          />
          <small v-if="emailError" class="form-error">{{ emailError }}</small>
        </div>

        <!-- Business Type Selector -->
        <div class="form-group">
          <label for="businessType" class="form-label">
            Unternehmenstyp <span class="required">*</span>
          </label>
          <select
            id="businessType"
            v-model="formData.businessType"
            class="form-input"
            required
            data-testid="business-type-select"
          >
            <option value="">-- Bitte wählen --</option>
            <option value="B2C">B2C (Privatperson / Einzelunternehmer)</option>
            <option value="B2B">
              B2B (Unternehmen / GmbH / GmbH & Co. KG)
            </option>
          </select>
        </div>

        <!-- Optional: First Name -->
        <div class="form-group">
          <label for="firstName" class="form-label">Vorname</label>
          <input
            id="firstName"
            v-model="formData.firstName"
            type="text"
            class="form-input"
            placeholder="Max"
            data-testid="first-name-input"
          />
        </div>

        <!-- Optional: Last Name -->
        <div class="form-group">
          <label for="lastName" class="form-label">Nachname</label>
          <input
            id="lastName"
            v-model="formData.lastName"
            type="text"
            class="form-input"
            placeholder="Mustermann"
            data-testid="last-name-input"
          />
        </div>

        <!-- Optional: Company Name -->
        <div class="form-group">
          <label for="companyName" class="form-label">Firmenname</label>
          <input
            id="companyName"
            v-model="formData.companyName"
            type="text"
            class="form-input"
            placeholder="Mustercompany GmbH"
            data-testid="company-name-input"
          />
        </div>

        <!-- Optional: Phone -->
        <div class="form-group">
          <label for="phone" class="form-label">Telefon</label>
          <input
            id="phone"
            v-model="formData.phone"
            type="tel"
            class="form-input"
            placeholder="+49 (0) 123 456789"
            data-testid="phone-input"
          />
        </div>
      </div>

      <!-- Submit Button -->
      <div class="form-actions">
        <button
          type="submit"
          class="btn btn-primary"
          :disabled="isLoading || !formData.email || !formData.businessType"
          data-testid="submit-button"
        >
          <span v-if="isLoading" class="loading-spinner">⟳</span>
          {{ isLoading ? "Prüfen läuft..." : "Prüfen" }}
        </button>
      </div>
    </form>

    <!-- Results Section -->
    <Transition name="fade-scale">
      <div v-if="result" class="results-section">
        <div
          :class="[
            'result-card',
            `result-${result.registrationType.toLowerCase()}`,
          ]"
        >
          <!-- Registration Type Badge -->
          <div class="result-badge">
            <span class="badge" :class="`badge-${getRegistrationBadgeClass()}`">
              {{ formatRegistrationType(result.registrationType) }}
            </span>
          </div>

          <!-- Result Message -->
          <div class="result-message">
            <h3>{{ getResultTitle() }}</h3>
            <p>{{ getResultDescription() }}</p>
          </div>

          <!-- ERP Data (if found) -->
          <div v-if="result.erpData" class="erp-data-section">
            <h4>Ihre Kundendaten:</h4>
            <table class="data-table">
              <tbody>
                <tr>
                  <td class="label">Kundennummer:</td>
                  <td class="value">{{ result.erpData.customerNumber }}</td>
                </tr>
                <tr>
                  <td class="label">Name:</td>
                  <td class="value">{{ result.erpData.name }}</td>
                </tr>
                <tr>
                  <td class="label">E-Mail:</td>
                  <td class="value">{{ result.erpData.email }}</td>
                </tr>
                <tr v-if="result.erpData.phone">
                  <td class="label">Telefon:</td>
                  <td class="value">{{ result.erpData.phone }}</td>
                </tr>
                <tr v-if="result.erpData.address">
                  <td class="label">Adresse:</td>
                  <td class="value">
                    {{ result.erpData.address }}<br />
                    {{ result.erpData.postalCode }} {{ result.erpData.city
                    }}<br />
                    {{ result.erpData.country }}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- Confidence Score (if duplicate detected) -->
          <div v-if="result.confidenceScore" class="confidence-section">
            <p class="confidence-text">
              Übereinstimmungsquote:
              <strong>{{ result.confidenceScore }}%</strong>
            </p>
            <div class="confidence-bar">
              <div
                class="confidence-fill"
                :style="{ width: `${result.confidenceScore}%` }"
              ></div>
            </div>
          </div>

          <!-- Action Buttons -->
          <div class="result-actions">
            <template v-if="result.registrationType === 'Bestandskunde'">
              <button
                class="btn btn-primary"
                @click="continueWithBestandskunde"
              >
                Mit Kundendaten fortfahren
              </button>
              <button class="btn btn-secondary" @click="resetForm">
                Neue Prüfung
              </button>
            </template>
            <template v-else>
              <button
                class="btn btn-primary"
                @click="continueWithNewRegistration"
              >
                Registrierung fortsetzen
              </button>
              <button class="btn btn-secondary" @click="resetForm">
                Zurück
              </button>
            </template>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Info Section -->
    <div class="info-section">
      <h4>📋 Informationen</h4>
      <ul>
        <li>
          <strong>Bestandskunde:</strong> Sie sind bereits in unserem System
          registriert. Ihre Daten werden automatisch vorausgefüllt.
        </li>
        <li>
          <strong>Neukunde:</strong> Sie werden zur regulären Registrierung
          weitergeleitet.
        </li>
        <li>
          Die Prüfung wird anhand von E-Mail, Name und optional Telefon/Adresse
          durchgeführt.
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from "vue";
import { useRouter } from "vue-router";
import {
  checkRegistrationType,
  validateEmail,
  formatRegistrationType,
  type CheckRegistrationTypeRequest,
  type CheckRegistrationTypeResponse,
} from "@/services/registrationService";

const router = useRouter();

// Form State
const formData = ref<CheckRegistrationTypeRequest>({
  email: "",
  businessType: "B2C",
  firstName: "",
  lastName: "",
  companyName: "",
  phone: "",
});

// UI State
const isLoading = ref(false);
const error = ref<string | null>(null);
const successMessage = ref<string | null>(null);
const emailError = ref<string | null>(null);
const result = ref<CheckRegistrationTypeResponse | null>(null);

/**
 * Validate email format
 */
function validateEmailField() {
  emailError.value = null;
  if (formData.value.email && !validateEmail(formData.value.email)) {
    emailError.value = "Bitte geben Sie eine gültige E-Mail-Adresse ein";
  }
}

/**
 * Handle form submission
 */
async function handleCheckRegistration() {
  // Reset previous states
  error.value = null;
  result.value = null;

  // Validate email
  if (!validateEmail(formData.value.email)) {
    emailError.value = "Bitte geben Sie eine gültige E-Mail-Adresse ein";
    return;
  }

  isLoading.value = true;

  try {
    // Call Wolverine CheckRegistrationTypeService endpoint
    const response = await checkRegistrationType(formData.value);

    if (response.success) {
      result.value = response;
      successMessage.value = `Registrierungstyp bestimmt: ${formatRegistrationType(
        response.registrationType
      )}`;

      // Scroll to results
      setTimeout(() => {
        document
          .querySelector(".results-section")
          ?.scrollIntoView({ behavior: "smooth" });
      }, 100);
    } else {
      error.value =
        response.message ||
        response.error ||
        "Fehler beim Prüfen der Registrierung";
    }
  } catch (err) {
    const errorMsg = err instanceof Error ? err.message : String(err);
    error.value = `Fehler: ${errorMsg}`;
    console.error("CheckRegistrationType error:", err);
  } finally {
    isLoading.value = false;
  }
}

/**
 * Get result title based on registration type
 */
function getResultTitle(): string {
  if (!result.value) return "";

  switch (result.value.registrationType) {
    case "Bestandskunde":
      return "✓ Sie sind bereits registriert";
    case "ExistingCustomer":
      return "✓ Sie sind bereits Kunde";
    case "NewCustomer":
    default:
      return "Registrierung erforderlich";
  }
}

/**
 * Get result description
 */
function getResultDescription(): string {
  if (!result.value) return "";

  switch (result.value.registrationType) {
    case "Bestandskunde":
      return "Ihre Kundendaten wurden gefunden. Sie können sich mit vereinfachter Registrierung anmelden.";
    case "ExistingCustomer":
      return "Wir haben Ihre Daten in unserem System gefunden.";
    case "NewCustomer":
    default:
      return "Sie werden als Neukunde registriert. Füllen Sie das Registrierungsformular aus.";
  }
}

/**
 * Get badge class for result
 */
function getRegistrationBadgeClass(): string {
  if (!result.value) return "default";

  switch (result.value.registrationType) {
    case "Bestandskunde":
      return "success";
    case "ExistingCustomer":
      return "info";
    case "NewCustomer":
    default:
      return "warning";
  }
}

/**
 * Continue with existing customer registration
 */
function continueWithBestandskunde() {
  if (result.value?.erpData) {
    // Pass ERP data to next step
    router.push({
      name: "registration-bestandskunde",
      params: { customerId: result.value.erpCustomerId },
      query: {
        email: result.value.erpData.email,
        name: result.value.erpData.name,
      },
    });
  }
}

/**
 * Continue with new customer registration
 */
function continueWithNewRegistration() {
  router.push({
    name: "registration-new",
    query: {
      email: formData.value.email,
      businessType: formData.value.businessType,
    },
  });
}

/**
 * Reset form to initial state
 */
function resetForm() {
  formData.value = {
    email: "",
    businessType: "B2C",
    firstName: "",
    lastName: "",
    companyName: "",
    phone: "",
  };
  result.value = null;
  error.value = null;
  successMessage.value = null;
  emailError.value = null;
}
</script>

<style scoped lang="css">
.registration-check-container {
  max-width: 900px;
  margin: 0 auto;
  padding: 2rem 1rem;
}

/* Header */
.header-section {
  text-align: center;
  margin-bottom: 3rem;
}

.header-section h1 {
  font-size: 2rem;
  font-weight: 700;
  margin-bottom: 0.5rem;
  color: #1f2937;
}

.subtitle {
  font-size: 1.1rem;
  color: #6b7280;
  margin: 0;
}

/* Alerts */
.alert {
  display: flex;
  gap: 1rem;
  padding: 1rem 1.5rem;
  margin-bottom: 2rem;
  border-radius: 0.5rem;
  border-left: 4px solid;
}

.alert-error {
  background-color: #fee;
  border-color: #dc2626;
  color: #7f1d1d;
}

.alert-success {
  background-color: #ecfdf5;
  border-color: #10b981;
  color: #065f46;
}

.alert-icon {
  font-size: 1.5rem;
  flex-shrink: 0;
}

.alert-content {
  flex: 1;
}

.alert-content h4 {
  margin: 0 0 0.25rem 0;
  font-weight: 600;
}

.alert-content p {
  margin: 0;
  font-size: 0.95rem;
}

.alert-close {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  padding: 0;
  color: inherit;
  opacity: 0.7;
  transition: opacity 0.2s;
}

.alert-close:hover {
  opacity: 1;
}

/* Form */
.registration-form {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 0.75rem;
  padding: 2rem;
  margin-bottom: 2rem;
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-label {
  font-weight: 500;
  margin-bottom: 0.5rem;
  color: #1f2937;
  font-size: 0.95rem;
}

.required {
  color: #dc2626;
}

.form-input {
  padding: 0.75rem 1rem;
  border: 1px solid #d1d5db;
  border-radius: 0.375rem;
  font-size: 1rem;
  transition: all 0.2s;
  font-family: inherit;
}

.form-input:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-input:disabled {
  background-color: #f3f4f6;
  color: #9ca3af;
  cursor: not-allowed;
}

.form-error {
  color: #dc2626;
  font-size: 0.875rem;
  margin-top: 0.25rem;
}

.form-actions {
  display: flex;
  gap: 1rem;
}

/* Buttons */
.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 0.375rem;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-primary {
  background-color: #3b82f6;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: #2563eb;
}

.btn-secondary {
  background-color: #e5e7eb;
  color: #1f2937;
}

.btn-secondary:hover:not(:disabled) {
  background-color: #d1d5db;
}

.loading-spinner {
  display: inline-block;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

/* Results Section */
.results-section {
  margin-top: 2rem;
}

.result-card {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 0.75rem;
  padding: 2rem;
  border-left: 4px solid #d1d5db;
}

.result-card.result-bestandskunde {
  border-left-color: #10b981;
  background-color: #f0fdf4;
}

.result-card.result-existingcustomer {
  border-left-color: #3b82f6;
  background-color: #eff6ff;
}

.result-card.result-newcustomer {
  border-left-color: #f59e0b;
  background-color: #fffbeb;
}

.result-badge {
  margin-bottom: 1rem;
}

.badge {
  display: inline-block;
  padding: 0.5rem 1rem;
  border-radius: 9999px;
  font-size: 0.875rem;
  font-weight: 600;
}

.badge-success {
  background-color: #d1fae5;
  color: #065f46;
}

.badge-info {
  background-color: #dbeafe;
  color: #0c3880;
}

.badge-warning {
  background-color: #fef3c7;
  color: #78350f;
}

.result-message {
  margin-bottom: 1.5rem;
}

.result-message h3 {
  font-size: 1.3rem;
  font-weight: 600;
  margin: 0 0 0.5rem 0;
  color: #1f2937;
}

.result-message p {
  margin: 0;
  color: #6b7280;
}

/* ERP Data Section */
.erp-data-section {
  background: white;
  padding: 1rem;
  border-radius: 0.375rem;
  margin: 1.5rem 0;
  border: 1px solid #e5e7eb;
}

.erp-data-section h4 {
  margin: 0 0 1rem 0;
  font-size: 0.95rem;
  font-weight: 600;
  color: #1f2937;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
}

.data-table tr {
  border-bottom: 1px solid #e5e7eb;
}

.data-table tr:last-child {
  border-bottom: none;
}

.data-table td {
  padding: 0.75rem 0;
}

.data-table td.label {
  font-weight: 500;
  color: #6b7280;
  width: 150px;
}

.data-table td.value {
  color: #1f2937;
}

/* Confidence Section */
.confidence-section {
  margin: 1.5rem 0;
  padding: 1rem;
  background: white;
  border-radius: 0.375rem;
  border: 1px solid #e5e7eb;
}

.confidence-text {
  margin: 0 0 0.5rem 0;
  font-size: 0.95rem;
  color: #6b7280;
}

.confidence-bar {
  height: 8px;
  background-color: #e5e7eb;
  border-radius: 9999px;
  overflow: hidden;
}

.confidence-fill {
  height: 100%;
  background-color: #10b981;
  transition: width 0.3s ease;
}

/* Result Actions */
.result-actions {
  display: flex;
  gap: 1rem;
  margin-top: 1.5rem;
  flex-wrap: wrap;
}

.result-actions .btn {
  flex: 1;
  min-width: 200px;
}

/* Info Section */
.info-section {
  background-color: #f3f4f6;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  padding: 1.5rem;
  margin-top: 2rem;
}

.info-section h4 {
  margin: 0 0 1rem 0;
  font-size: 1rem;
  font-weight: 600;
  color: #1f2937;
}

.info-section ul {
  margin: 0;
  padding-left: 1.5rem;
  list-style: disc;
}

.info-section li {
  margin-bottom: 0.5rem;
  color: #6b7280;
  line-height: 1.5;
}

.info-section strong {
  color: #1f2937;
}

/* Animations */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.fade-scale-enter-active {
  transition:
    opacity 0.3s ease,
    transform 0.3s ease;
}

.fade-scale-enter-from {
  opacity: 0;
  transform: scale(0.95);
}

/* Responsive */
@media (max-width: 768px) {
  .registration-check-container {
    padding: 1rem;
  }

  .header-section h1 {
    font-size: 1.5rem;
  }

  .form-grid {
    grid-template-columns: 1fr;
  }

  .registration-form {
    padding: 1.5rem;
  }

  .result-actions {
    flex-direction: column;
  }

  .result-actions .btn {
    min-width: unset;
  }

  .data-table td.label {
    width: 100px;
    font-size: 0.875rem;
  }
}
</style>
