<template>
  <div class="space-y-6">
    <!-- Header -->
    <div>
      <h1 class="text-3xl font-bold">
        {{
          isNewCustomer
            ? $t('erp.customerLookup.newRegistration')
            : $t('erp.customerLookup.welcomeBack')
        }}
      </h1>
      <p class="mt-2 text-base-content/70">
        {{
          isNewCustomer
            ? $t('erp.customerLookup.enterEmailToStart')
            : $t('erp.customerLookup.customerInfoFound')
        }}
      </p>
    </div>

    <!-- Email Lookup Section -->
    <div class="card bg-base-100 shadow-lg">
      <div class="card-body space-y-4">
        <!-- Email Input -->
        <div class="form-control w-full">
          <label class="label">
            <span class="label-text font-medium">{{ $t('erp.customerLookup.emailLabel') }}</span>
          </label>
          <input
            id="email"
            v-model="email"
            type="email"
            placeholder="name@example.com"
            :disabled="isLoading || hasCustomer"
            @blur="validateEmail"
            class="input input-bordered w-full"
            aria-label="E-Mail-Adresse"
          />
          <label v-if="emailError" class="label">
            <span class="label-text-alt text-error">{{ emailError }}</span>
          </label>
        </div>

        <!-- Lookup Status -->
        <div v-if="isLoading" class="flex items-center gap-2 text-info">
          <div class="loading loading-spinner loading-sm"></div>
          <span class="text-sm">{{ $t('erp.customerLookup.searching') }}</span>
        </div>

        <!-- Error Alert -->
        <div v-if="error && !isLoading" class="alert alert-error" role="alert">
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
          <div>
            <h3 class="font-medium">{{ $t('erp.customerLookup.searchError') }}</h3>
            <div class="text-xs mt-1">{{ error }}</div>
          </div>
        </div>

        <!-- Customer Found Alert -->
        <div v-if="hasCustomer && customer" class="alert alert-success" role="alert">
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
              d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          <div>
            <h3 class="font-medium">{{ $t('erp.customerLookup.customerFound') }}</h3>
            <div class="text-sm mt-1">
              {{ $t('erp.customerLookup.welcomeBackWithName') }}
              <strong>{{ customer.customerName }}</strong
              >!
            </div>
          </div>
        </div>

        <!-- Customer Details Card -->
        <div class="mt-4 space-y-4">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <p class="text-xs uppercase tracking-wide opacity-70">
                {{ $t('erp.customerLookup.customerNumber') }}
              </p>
              <p class="mt-1 text-lg font-semibold">
                {{ customer!.customerNumber }}
              </p>
            </div>
            <div>
              <p class="text-xs uppercase tracking-wide opacity-70">
                {{ $t('erp.customerLookup.customerType') }}
              </p>
              <p class="mt-1">
                <span v-if="isPrivateCustomer" class="badge badge-primary">
                  {{ $t('erp.customerLookup.privateCustomer') }}
                </span>
                <span v-else class="badge badge-secondary">
                  {{ $t('erp.customerLookup.businessCustomer') }}
                </span>
              </p>
            </div>
          </div>

          <!-- Business Details -->
          <div v-if="isBusinessCustomer" class="mt-4 border-t border-base-300 pt-4">
            <p class="text-sm font-medium mb-2">{{ $t('erp.customerLookup.businessInfo') }}</p>
            <div class="space-y-1 text-sm">
              <div>
                <strong>{{ $t('erp.customerLookup.company') }}</strong> {{ customer!.customerName }}
              </div>
              <div v-if="customer!.phone">
                <strong>{{ $t('erp.customerLookup.phone') }}</strong> {{ customer!.phone }}
              </div>
              <div v-if="customer!.country">
                <strong>{{ $t('erp.customerLookup.country') }}</strong> {{ customer!.country }}
              </div>
              <div v-if="customer!.creditLimit">
                <strong>{{ $t('erp.customerLookup.creditLimit') }}</strong> €{{
                  customer!.creditLimit.toLocaleString('de-DE')
                }}
              </div>
            </div>
          </div>
        </div>

        <!-- Lookup Performance -->
        <div v-if="lastLookupTime" class="text-xs text-base-content/60 text-right">
          Suche: {{ lastLookupTime }}ms
        </div>

        <!-- Action Buttons -->
        <div class="flex gap-3 flex-col sm:flex-row">
          <button
            v-if="!hasCustomer"
            @click="validateEmail"
            :disabled="!email || isLoading"
            class="btn btn-primary flex-1"
          >
            {{
              isLoading ? $t('erp.customerLookup.searching') : $t('erp.customerLookup.searchButton')
            }}
          </button>

          <button v-if="hasCustomer" @click="proceedWithCustomer" class="btn btn-success flex-1">
            {{ $t('erp.customerLookup.continue') }}
          </button>

          <button @click="clearForm" class="btn btn-neutral flex-1">
            {{ hasCustomer ? $t('erp.customerLookup.newSearch') : $t('erp.customerLookup.cancel') }}
          </button>
        </div>
      </div>
    </div>

    <!-- New Customer Section -->
    <div v-if="isNewCustomer" class="alert alert-info">
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
          d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
        />
      </svg>
      <div>
        <h2 class="font-semibold mb-1">{{ $t('erp.customerLookup.newCustomerQuestion') }}</h2>
        <p class="text-sm">
          {{ $t('erp.customerLookup.newCustomerDescription') }}
        </p>
        <button @click="$emit('register')" class="btn btn-sm btn-primary mt-3">
          {{ $t('erp.customerLookup.newRegistration') }}
        </button>
      </div>
    </div>

    <!-- Diagnostic Info (Development only) -->
    <div v-if="isDevelopment" class="card bg-base-200 shadow-sm">
      <div class="card-body">
        <p class="font-bold mb-2 text-sm">{{ $t('erp.customerLookup.diagnosticInfo') }}</p>
        <div class="space-y-1 text-xs font-mono">
          <div>Email: {{ email || '(empty)' }}</div>
          <div>Loading: {{ isLoading }}</div>
          <div>Has Customer: {{ hasCustomer }}</div>
          <div>Provider: {{ customer ? 'Real ERP' : 'Faker' }}</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { useErpIntegration } from '@/composables/useErpIntegration';

const { t } = useI18n();

defineProps<{
  isDevelopment?: boolean;
}>();

const emit = defineEmits<{
  register: [];
  proceed: [customerNumber: string];
}>();

// ERP Integration
const { validateCustomerEmail, hasCustomer, customer, isLoading, error, clearCustomer } =
  useErpIntegration();

// Form State
const email = ref('');
const emailError = ref<string | null>(null);
const lastLookupTime = ref<number | null>(null);

// Computed
const isNewCustomer = computed(() => !hasCustomer.value);
const isPrivateCustomer = computed(() => customer.value?.businessType === 'PRIVATE');
const isBusinessCustomer = computed(() => customer.value?.businessType === 'BUSINESS');

// Methods
const validateEmail = async () => {
  emailError.value = null;
  const result = await validateCustomerEmail(email.value);
  lastLookupTime.value = result.loadingMs || null;

  if (!result.isValid) {
    emailError.value = result.message || t('erp.customerLookup.customerNotFound');
  }
};

const proceedWithCustomer = () => {
  if (customer.value) {
    emit('proceed', customer.value.customerNumber);
  }
};

const clearForm = () => {
  email.value = '';
  emailError.value = null;
  clearCustomer();
};
</script>

<style scoped>
/* Smooth input transitions */
input:focus {
  @apply ring-2 ring-primary;
}
</style>
