<template>
  <div class="space-y-6">
    <!-- Header -->
    <div>
      <h1 class="text-3xl font-bold">
        {{ isNewCustomer ? 'Neue Registrierung' : 'Willkommen zurück' }}
      </h1>
      <p class="mt-2 text-base-content/70">
        {{
          isNewCustomer
            ? 'Geben Sie Ihre E-Mail-Adresse ein, um zu beginnen'
            : 'Kundeninformationen gefunden'
        }}
      </p>
    </div>

    <!-- Email Lookup Section -->
    <div class="card bg-base-100 shadow-lg">
      <div class="card-body space-y-4">
        <!-- Email Input -->
        <div class="form-control w-full">
          <label class="label">
            <span class="label-text font-medium">E-Mail-Adresse *</span>
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
          <span class="text-sm">Suche läuft...</span>
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
            <h3 class="font-medium">Fehler bei der Kundensuche</h3>
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
            <h3 class="font-medium">Kunde gefunden!</h3>
            <div class="text-sm mt-1">
              Willkommen zurück, <strong>{{ customer.customerName }}</strong
              >!
            </div>
          </div>
        </div>

        <!-- Customer Details Card -->
        <div class="mt-4 space-y-4">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <p class="text-xs uppercase tracking-wide opacity-70">Kundennummer</p>
              <p class="mt-1 text-lg font-semibold">
                {{ customer!.customerNumber }}
              </p>
            </div>
            <div>
              <p class="text-xs uppercase tracking-wide opacity-70">Kundentyp</p>
              <p class="mt-1">
                <span v-if="isPrivateCustomer" class="badge badge-primary"> Privatperson </span>
                <span v-else class="badge badge-secondary"> Geschäftskunde </span>
              </p>
            </div>
          </div>

          <!-- Business Details -->
          <div v-if="isBusinessCustomer" class="mt-4 border-t border-base-300 pt-4">
            <p class="text-sm font-medium mb-2">Geschäftsinformationen</p>
            <div class="space-y-1 text-sm">
              <div><strong>Firma:</strong> {{ customer!.customerName }}</div>
              <div v-if="customer!.phone"><strong>Telefon:</strong> {{ customer!.phone }}</div>
              <div v-if="customer!.country"><strong>Land:</strong> {{ customer!.country }}</div>
              <div v-if="customer!.creditLimit">
                <strong>Kreditlimit:</strong> €{{ customer!.creditLimit.toLocaleString('de-DE') }}
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
            {{ isLoading ? 'Suche läuft...' : 'Kundensuche' }}
          </button>

          <button v-if="hasCustomer" @click="proceedWithCustomer" class="btn btn-success flex-1">
            Weiter
          </button>

          <button @click="clearForm" class="btn btn-neutral flex-1">
            {{ hasCustomer ? 'Neue Suche' : 'Abbrechen' }}
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
        <h2 class="font-semibold mb-1">Sind Sie ein neuer Kunde?</h2>
        <p class="text-sm">
          Sie können sich jetzt registrieren und später von Ihren gespeicherten Informationen
          profitieren.
        </p>
        <button @click="$emit('register')" class="btn btn-sm btn-primary mt-3">
          Neue Registrierung
        </button>
      </div>
    </div>

    <!-- Diagnostic Info (Development only) -->
    <div v-if="isDevelopment" class="card bg-base-200 shadow-sm">
      <div class="card-body">
        <p class="font-bold mb-2 text-sm">🔧 Diagnostic Info (Dev Only)</p>
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
import { useErpIntegration } from '@/composables/useErpIntegration';

defineProps<{
  isDevelopment?: boolean;
}>();

defineEmits<{
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
    emailError.value = result.message || 'Kunde nicht gefunden';
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

// Emit
const emit = defineEmits<{
  register: [];
  proceed: [customerNumber: string];
}>();
</script>

<style scoped>
/* Smooth input transitions */
input:focus {
  @apply ring-2 ring-primary;
}
</style>
