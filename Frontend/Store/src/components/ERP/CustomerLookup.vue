<template>
  <div class="space-y-6">
    <!-- Header -->
    <div>
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white">
        {{ isNewCustomer ? "Neue Registrierung" : "Willkommen zurück" }}
      </h1>
      <p class="mt-2 text-gray-600 dark:text-gray-400">
        {{
          isNewCustomer
            ? "Geben Sie Ihre E-Mail-Adresse ein, um zu beginnen"
            : "Kundeninformationen gefunden"
        }}
      </p>
    </div>

    <!-- Email Lookup Section -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
      <div class="space-y-4">
        <!-- Email Input -->
        <div>
          <label
            for="email"
            class="block text-sm font-medium text-gray-700 dark:text-gray-300"
          >
            E-Mail-Adresse *
          </label>
          <input
            id="email"
            v-model="email"
            type="email"
            placeholder="name@example.com"
            :disabled="isLoading || hasCustomer"
            @blur="validateEmail"
            class="mt-1 block w-full rounded-md border-gray-300 shadow-sm px-3 py-2 border disabled:bg-gray-100 dark:disabled:bg-gray-700 dark:bg-gray-700 dark:text-white dark:border-gray-600"
            aria-label="E-Mail-Adresse"
          />
          <p v-if="emailError" class="mt-1 text-sm text-red-600">
            {{ emailError }}
          </p>
        </div>

        <!-- Lookup Status -->
        <div
          v-if="isLoading"
          class="flex items-center space-x-2 text-blue-600 dark:text-blue-400"
        >
          <div
            class="animate-spin rounded-full h-4 w-4 border-b-2 border-current"
          ></div>
          <span class="text-sm">Suche läuft...</span>
        </div>

        <!-- Error Alert -->
        <div
          v-if="error && !isLoading"
          class="rounded-md bg-red-50 dark:bg-red-900/20 p-4"
          role="alert"
        >
          <div class="flex">
            <div class="ml-3">
              <h3 class="text-sm font-medium text-red-800 dark:text-red-200">
                Fehler bei der Kundensuche
              </h3>
              <div class="mt-2 text-sm text-red-700 dark:text-red-300">
                {{ error }}
              </div>
            </div>
          </div>
        </div>

        <!-- Customer Found Alert -->
        <div
          v-if="hasCustomer && customer"
          class="rounded-md bg-green-50 dark:bg-green-900/20 p-4"
          role="alert"
        >
          <div class="flex">
            <div class="ml-3">
              <h3
                class="text-sm font-medium text-green-800 dark:text-green-200"
              >
                Kunde gefunden!
              </h3>
              <div class="mt-2 text-sm text-green-700 dark:text-green-300">
                Willkommen zurück, <strong>{{ customer.customerName }}</strong
                >!
              </div>
            </div>
          </div>

          <!-- Customer Details Card -->
          <div class="mt-4 space-y-2">
            <div class="grid grid-cols-2 gap-4">
              <div>
                <p
                  class="text-xs uppercase tracking-wide text-gray-500 dark:text-gray-400"
                >
                  Kundennummer
                </p>
                <p
                  class="mt-1 text-lg font-semibold text-gray-900 dark:text-white"
                >
                  {{ customer.customerNumber }}
                </p>
              </div>
              <div>
                <p
                  class="text-xs uppercase tracking-wide text-gray-500 dark:text-gray-400"
                >
                  Kundentyp
                </p>
                <p
                  class="mt-1 text-lg font-semibold text-gray-900 dark:text-white"
                >
                  <span
                    v-if="isPrivateCustomer"
                    class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-blue-100 dark:bg-blue-900/30 text-blue-800 dark:text-blue-200"
                  >
                    Privatperson
                  </span>
                  <span
                    v-else
                    class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-purple-100 dark:bg-purple-900/30 text-purple-800 dark:text-purple-200"
                  >
                    Geschäftskunde
                  </span>
                </p>
              </div>
            </div>

            <!-- Business Details -->
            <div
              v-if="isBusinessCustomer"
              class="mt-4 border-t border-gray-200 dark:border-gray-700 pt-4"
            >
              <p class="text-sm font-medium text-gray-900 dark:text-white mb-2">
                Geschäftsinformationen
              </p>
              <div class="space-y-1 text-sm text-gray-600 dark:text-gray-400">
                <div><strong>Firma:</strong> {{ customer.customerName }}</div>
                <div v-if="customer.phone">
                  <strong>Telefon:</strong> {{ customer.phone }}
                </div>
                <div v-if="customer.country">
                  <strong>Land:</strong> {{ customer.country }}
                </div>
                <div v-if="customer.creditLimit">
                  <strong>Kreditlimit:</strong> €{{
                    customer.creditLimit.toLocaleString("de-DE")
                  }}
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Lookup Performance -->
        <div
          v-if="lastLookupTime"
          class="text-xs text-gray-500 dark:text-gray-400 text-right"
        >
          Suche: {{ lastLookupTime }}ms
        </div>

        <!-- Action Buttons -->
        <div class="flex space-x-3">
          <button
            v-if="!hasCustomer"
            @click="validateEmail"
            :disabled="!email || isLoading"
            class="flex-1 rounded-md bg-blue-600 px-3 py-2 text-white font-medium hover:bg-blue-700 disabled:bg-gray-300 dark:disabled:bg-gray-700 transition-colors"
          >
            {{ isLoading ? "Suche läuft..." : "Kundensuche" }}
          </button>

          <button
            v-if="hasCustomer"
            @click="proceedWithCustomer"
            class="flex-1 rounded-md bg-green-600 px-3 py-2 text-white font-medium hover:bg-green-700 transition-colors"
          >
            Weiter
          </button>

          <button
            @click="clearForm"
            class="flex-1 rounded-md bg-gray-200 dark:bg-gray-700 px-3 py-2 text-gray-900 dark:text-white font-medium hover:bg-gray-300 dark:hover:bg-gray-600 transition-colors"
          >
            {{ hasCustomer ? "Neue Suche" : "Abbrechen" }}
          </button>
        </div>
      </div>
    </div>

    <!-- New Customer Section -->
    <div
      v-if="isNewCustomer"
      class="bg-blue-50 dark:bg-blue-900/20 rounded-lg p-6"
    >
      <h2 class="text-lg font-semibold text-blue-900 dark:text-blue-100 mb-2">
        Sind Sie ein neuer Kunde?
      </h2>
      <p class="text-sm text-blue-800 dark:text-blue-200 mb-4">
        Sie können sich jetzt registrieren und später von Ihren gespeicherten
        Informationen profitieren.
      </p>
      <button
        @click="$emit('register')"
        class="rounded-md bg-blue-600 px-4 py-2 text-white text-sm font-medium hover:bg-blue-700 transition-colors"
      >
        Neue Registrierung
      </button>
    </div>

    <!-- Diagnostic Info (Development only) -->
    <div
      v-if="isDevelopment"
      class="bg-gray-100 dark:bg-gray-800 rounded p-4 text-xs font-mono"
    >
      <p class="font-bold mb-2">🔧 Diagnostic Info (Dev Only)</p>
      <div class="space-y-1 text-gray-700 dark:text-gray-300">
        <div>Email: {{ email || "(empty)" }}</div>
        <div>Loading: {{ isLoading }}</div>
        <div>Has Customer: {{ hasCustomer }}</div>
        <div>Provider: {{ customer ? "Real ERP" : "Faker" }}</div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from "vue";
import { useErpIntegration } from "@/composables/useErpIntegration";

defineProps<{
  isDevelopment?: boolean;
}>();

defineEmits<{
  register: [];
  proceed: [customerNumber: string];
}>();

// ERP Integration
const {
  validateCustomerEmail,
  hasCustomer,
  customer,
  isLoading,
  error,
  clearCustomer,
} = useErpIntegration();

// Form State
const email = ref("");
const emailError = ref<string | null>(null);

// Computed
const isNewCustomer = computed(() => !hasCustomer.value);
const isPrivateCustomer = computed(
  () => customer.value?.businessType === "PRIVATE"
);
const isBusinessCustomer = computed(
  () => customer.value?.businessType === "BUSINESS"
);
const lastLookupTime = ref<number | null>(null);

// Methods
const validateEmail = async () => {
  emailError.value = null;
  const result = await validateCustomerEmail(email.value);
  lastLookupTime.value = result.loadingMs || null;

  if (!result.isValid) {
    emailError.value = result.message || "Kunde nicht gefunden";
  }
};

const proceedWithCustomer = () => {
  if (customer.value) {
    emit("proceed", customer.value.customerNumber);
  }
};

const clearForm = () => {
  email.value = "";
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
/* Smooth transitions */
input {
  @apply transition-colors focus:outline-none focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400;
}
</style>
