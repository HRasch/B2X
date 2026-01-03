<script setup lang="ts">
import { ref, computed } from 'vue';

interface VatValidationResult {
  isValid: boolean;
  vatId: string;
  companyName?: string;
  companyAddress?: string;
  reverseChargeApplies: boolean;
  message: string;
  expiresAt: string;
}

const props = withDefaults(
  defineProps<{
    modelValue?: string;
    sellerCountry?: string;
    buyerCountry?: string;
  }>(),
  {
    modelValue: '',
    sellerCountry: 'DE',
    buyerCountry: '',
  }
);

const emit = defineEmits<{
  'update:modelValue': [value: string];
  'validation-result': [result: VatValidationResult];
}>();

// State
const vatNumber = ref(props.modelValue);
const countryCode = ref('DE');
const isValidating = ref(false);
const validationResult = ref<VatValidationResult | null>(null);
const error = ref<string | null>(null);

// Computed
const fullVatId = computed(() => `${countryCode.value}${vatNumber.value}`);
const isValid = computed(() => validationResult.value?.isValid ?? false);

// Methods
const validateVatId = async () => {
  if (!countryCode.value || !vatNumber.value) {
    error.value = 'Country code and VAT number are required';
    return;
  }

  isValidating.value = true;
  error.value = null;
  validationResult.value = null;

  try {
    const response = await fetch('/api/validatevatid', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        countryCode: countryCode.value,
        vatNumber: vatNumber.value,
        buyerCountry: props.buyerCountry,
        sellerCountry: props.sellerCountry,
      }),
    });

    if (!response.ok) {
      throw new Error(`HTTP ${response.status}`);
    }

    const result: VatValidationResult = await response.json();
    validationResult.value = result;

    if (!result.isValid) {
      error.value = result.message || 'VAT ID could not be validated. Please verify and try again.';
    }

    emit('validation-result', result);
  } catch (e) {
    error.value = `Validation failed: ${e instanceof Error ? e.message : 'Unknown error'}`;
    console.error('VAT validation error:', e);
  } finally {
    isValidating.value = false;
  }
};

const handleCountryChange = () => {
  validationResult.value = null;
  error.value = null;
};

const handleVatNumberChange = () => {
  emit('update:modelValue', vatNumber.value);
};

const clearValidation = () => {
  vatNumber.value = '';
  countryCode.value = 'DE';
  validationResult.value = null;
  error.value = null;
  emit('update:modelValue', '');
};
</script>

<template>
  <div class="space-y-4">
    <!-- Country Code Select -->
    <div class="form-control">
      <label for="countryCode" class="label">
        <span class="label-text font-bold">Country Code</span>
      </label>
      <select
        id="countryCode"
        v-model="countryCode"
        :disabled="isValidating"
        @change="handleCountryChange"
        class="select select-bordered"
      >
        <option value="AT">Austria (AT)</option>
        <option value="BE">Belgium (BE)</option>
        <option value="BG">Bulgaria (BG)</option>
        <option value="HR">Croatia (HR)</option>
        <option value="CY">Cyprus (CY)</option>
        <option value="CZ">Czech Republic (CZ)</option>
        <option value="DK">Denmark (DK)</option>
        <option value="DE">Germany (DE)</option>
        <option value="EE">Estonia (EE)</option>
        <option value="FI">Finland (FI)</option>
        <option value="FR">France (FR)</option>
        <option value="GR">Greece (GR)</option>
        <option value="HU">Hungary (HU)</option>
        <option value="IE">Ireland (IE)</option>
        <option value="IT">Italy (IT)</option>
        <option value="LV">Latvia (LV)</option>
        <option value="LT">Lithuania (LT)</option>
        <option value="LU">Luxembourg (LU)</option>
        <option value="MT">Malta (MT)</option>
        <option value="NL">Netherlands (NL)</option>
        <option value="PL">Poland (PL)</option>
        <option value="PT">Portugal (PT)</option>
        <option value="RO">Romania (RO)</option>
        <option value="SK">Slovakia (SK)</option>
        <option value="SI">Slovenia (SI)</option>
        <option value="ES">Spain (ES)</option>
        <option value="SE">Sweden (SE)</option>
      </select>
    </div>

    <!-- VAT Number Input -->
    <div class="form-control">
      <label for="vatNumber" class="label">
        <span class="label-text font-bold">VAT Number</span>
      </label>
      <div class="flex gap-2">
        <input
          id="vatNumber"
          v-model="vatNumber"
          type="text"
          placeholder="e.g., 123456789"
          :disabled="isValidating"
          @change="handleVatNumberChange"
          class="input input-bordered flex-1"
          :class="{
            'input-success': isValid,
            'input-error': error,
          }"
        />
        <button
          type="button"
          :disabled="isValidating || !countryCode || !vatNumber"
          @click="validateVatId"
          class="btn btn-primary"
        >
          <span v-if="isValidating" class="loading loading-spinner loading-sm"></span>
          {{ isValidating ? 'Validating...' : 'Validate' }}
        </button>
      </div>
      <label class="label">
        <span class="label-text-alt font-mono">{{ fullVatId }}</span>
      </label>
    </div>

    <!-- Error Message -->
    <Transition name="fade">
      <div v-if="error" class="alert alert-error shadow-lg">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="stroke-current shrink-0 h-6 w-6"
          fill="none"
          viewBox="0 0 24 24"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M10 14l-2-2m0 0l-2-2m2 2l2-2m-2 2l-2 2m2-2l2 2m1-2a9 9 0 11-18 0 9 9 0 0118 0z"
          />
        </svg>
        <span>{{ error }}</span>
      </div>
    </Transition>

    <!-- Validation Result -->
    <Transition name="fade">
      <div
        v-if="validationResult"
        class="card shadow-lg"
        :class="{
          'bg-success/10': validationResult.isValid,
          'bg-error/10': !validationResult.isValid,
        }"
      >
        <div class="card-body">
          <!-- Result Header -->
          <div class="flex items-center gap-2 mb-4">
            <div
              class="badge badge-lg"
              :class="{
                'badge-success': validationResult.isValid,
                'badge-error': !validationResult.isValid,
              }"
            >
              <span v-if="validationResult.isValid" class="text-lg">✓</span>
              <span v-else class="text-lg">✗</span>
            </div>
            <span class="font-bold">{{ validationResult.message }}</span>
          </div>

          <!-- Company Info -->
          <div v-if="validationResult.isValid" class="space-y-3">
            <div v-if="validationResult.companyName" class="flex justify-between">
              <span class="font-bold">Company Name:</span>
              <span>{{ validationResult.companyName }}</span>
            </div>
            <div v-if="validationResult.companyAddress" class="flex flex-col gap-1">
              <span class="font-bold">Address:</span>
              <span class="text-sm">{{ validationResult.companyAddress }}</span>
            </div>
            <div class="flex justify-between">
              <span class="font-bold">Reverse Charge:</span>
              <span
                :class="{
                  'badge badge-success': validationResult.reverseChargeApplies,
                  'badge badge-warning': !validationResult.reverseChargeApplies,
                }"
              >
                {{
                  validationResult.reverseChargeApplies ? '0% VAT (applies)' : 'Standard VAT rate'
                }}
              </span>
            </div>
          </div>

          <!-- Action Buttons -->
          <div class="flex gap-2 mt-4 border-t pt-4">
            <button type="button" @click="clearValidation" class="btn btn-ghost flex-1">
              Clear & Start Over
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Additional Help Text -->
    <div v-if="error && !validationResult" class="alert alert-info shadow-lg">
      <svg
        xmlns="http://www.w3.org/2000/svg"
        fill="none"
        viewBox="0 0 24 24"
        class="stroke-current shrink-0 w-6 h-6"
      >
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
        ></path>
      </svg>
      <div>
        <h3 class="font-bold">VAT Validation Help</h3>
        <div class="text-xs">
          If you cannot provide a valid VAT-ID, you can continue as a B2C customer (standard VAT
          applies).
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* All styling is handled by DaisyUI and Tailwind classes */

.vat-input-group {
  margin-bottom: 1.5rem;
}

.label {
  display: block;
  font-weight: 600;
  font-size: 0.875rem;
  color: #111827;
  margin-bottom: 0.5rem;
}

.country-select,
.vat-input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 1rem;
  transition: all 0.2s;
}

.country-select:focus,
.vat-input:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.country-select:disabled,
.vat-input:disabled {
  background-color: #f3f4f6;
  color: #9ca3af;
  cursor: not-allowed;
}

.input-with-button {
  display: flex;
  gap: 0.75rem;
}

.vat-input {
  flex: 1;
}

.vat-input.validating {
  border-color: #f59e0b;
}

.vat-input.valid {
  border-color: #10b981;
  background-color: #ecfdf5;
}

.vat-input.error {
  border-color: #ef4444;
  background-color: #fef2f2;
}

.validate-btn {
  padding: 0.75rem 1.5rem;
  background-color: #3b82f6;
  color: white;
  border: none;
  border-radius: 6px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  white-space: nowrap;
}

.validate-btn:hover:not(:disabled) {
  background-color: #2563eb;
}

.validate-btn:disabled {
  background-color: #d1d5db;
  cursor: not-allowed;
}

.full-vat-id {
  font-size: 0.875rem;
  color: #6b7280;
  margin-top: 0.5rem;
  margin-bottom: 0;
}

.validation-result {
  padding: 1rem;
  border-radius: 6px;
  margin-top: 1rem;
  border-left: 4px solid;
}

.validation-result.success {
  background-color: #ecfdf5;
  border-color: #10b981;
}

.validation-result.failed {
  background-color: #fef2f2;
  border-color: #ef4444;
}

.result-header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1rem;
  font-weight: 600;
}

.status-icon {
  font-size: 1.25rem;
}

.validation-result.success .status-icon {
  color: #10b981;
}

.validation-result.failed .status-icon {
  color: #ef4444;
}

.status-text {
  color: #111827;
}

.company-info {
  margin-top: 1rem;
}

.info-row {
  display: flex;
  justify-content: space-between;
  padding: 0.75rem 0;
  border-bottom: 1px solid rgba(16, 185, 129, 0.2);
}

.info-row:last-child {
  border-bottom: none;
}

.info-row .label {
  font-size: 0.875rem;
  font-weight: 600;
  color: #374151;
  margin: 0;
}

.info-row .value {
  font-size: 0.875rem;
  color: #111827;
  font-weight: 500;
}

.info-row .value.active {
  color: #10b981;
  font-weight: 600;
}

.error-message {
  padding: 1rem;
  background-color: #fef2f2;
  border: 1px solid #fecaca;
  border-radius: 6px;
  margin-top: 1rem;
}

.error-message p {
  margin: 0.5rem 0;
  font-size: 0.875rem;
  color: #dc2626;
}

.error-hint {
  color: #7f1d1d;
  margin-top: 0.75rem !important;
}

.action-buttons {
  margin-top: 1.5rem;
  display: flex;
  gap: 0.75rem;
  justify-content: flex-end;
}

.btn-secondary {
  padding: 0.75rem 1.5rem;
  background-color: #e5e7eb;
  color: #111827;
  border: none;
  border-radius: 6px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-secondary:hover {
  background-color: #d1d5db;
}
</style>
