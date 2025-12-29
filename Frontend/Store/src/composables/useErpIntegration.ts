/**
 * Composable for ERP Integration in Frontend
 *
 * Provides reactive customer lookup using the ERP Provider Pattern
 * Works seamlessly with Faker (development) or real ERP (production)
 */

import { ref, computed, reactive } from "vue";
import type { Ref, ComputedRef } from "vue";

/**
 * ERP Customer DTO from backend
 */
export interface ErpCustomer {
  customerNumber: string;
  customerName: string;
  email: string;
  phone?: string;
  shippingAddress?: string;
  billingAddress?: string;
  country: string;
  businessType: "PRIVATE" | "BUSINESS";
  isActive: boolean;
  creditLimit?: number;
  lastOrderDate?: string;
}

/**
 * Validation result from ERP lookup
 */
export interface ValidationResult {
  isValid: boolean;
  customer: ErpCustomer | null;
  error?: string;
  message?: string;
  loadingMs?: number;
}

/**
 * useErpIntegration Composable
 *
 * Usage:
 * ```vue
 * <script setup lang="ts">
 * const {
 *   validateCustomerEmail,
 *   validateCustomerNumber,
 *   isLoading,
 *   customer,
 *   error
 * } = useErpIntegration()
 *
 * const email = ref('')
 * const handleLookup = async () => {
 *   const result = await validateCustomerEmail(email.value)
 *   if (result.isValid) {
 *     console.log('Customer found:', result.customer)
 *   }
 * }
 * </script>
 * ```
 */
export function useErpIntegration() {
  const isLoading: Ref<boolean> = ref(false);
  const customer: Ref<ErpCustomer | null> = ref(null);
  const error: Ref<string | null> = ref(null);
  const lastLookupTime: Ref<number | null> = ref(null);

  // Computed states
  const hasCustomer: ComputedRef<boolean> = computed(
    () => customer.value !== null
  );
  const isPrivateCustomer: ComputedRef<boolean> = computed(
    () => customer.value?.businessType === "PRIVATE" || false
  );
  const isBusinessCustomer: ComputedRef<boolean> = computed(
    () => customer.value?.businessType === "BUSINESS" || false
  );

  /**
   * Lookup customer by email (common registration scenario)
   * @param email Customer email address
   * @returns Validation result with customer data
   */
  const validateCustomerEmail = async (
    email: string
  ): Promise<ValidationResult> => {
    if (!email || !email.includes("@")) {
      return {
        isValid: false,
        customer: null,
        error: "INVALID_EMAIL",
        message: "Bitte geben Sie eine gültige E-Mail-Adresse ein.",
      };
    }

    isLoading.value = true;
    error.value = null;
    const startTime = performance.now();

    try {
      const response = await fetch("/api/auth/erp/validate-email", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email }),
      });

      const loadingMs = Math.round(performance.now() - startTime);
      lastLookupTime.value = loadingMs;

      if (!response.ok) {
        const data = await response.json();
        return {
          isValid: false,
          customer: null,
          error: data.error || "LOOKUP_FAILED",
          message: data.message || "Fehler bei der Kundensuche",
          loadingMs,
        };
      }

      const data: ErpCustomer = await response.json();
      customer.value = data;
      return {
        isValid: true,
        customer: data,
        loadingMs,
      };
    } catch (err) {
      const loadingMs = Math.round(performance.now() - startTime);
      const errorMessage =
        err instanceof Error ? err.message : "Unbekannter Fehler";
      error.value = errorMessage;

      return {
        isValid: false,
        customer: null,
        error: "NETWORK_ERROR",
        message: `Verbindungsfehler: ${errorMessage}`,
        loadingMs,
      };
    } finally {
      isLoading.value = false;
    }
  };

  /**
   * Lookup customer by customer number
   * @param customerNumber Customer number (e.g., "CUST-001")
   * @returns Validation result with customer data
   */
  const validateCustomerNumber = async (
    customerNumber: string
  ): Promise<ValidationResult> => {
    if (!customerNumber || customerNumber.trim().length === 0) {
      return {
        isValid: false,
        customer: null,
        error: "INVALID_CUSTOMER_NUMBER",
        message: "Kundennummer erforderlich",
      };
    }

    isLoading.value = true;
    error.value = null;
    const startTime = performance.now();

    try {
      const response = await fetch("/api/auth/erp/validate-number", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ customerNumber }),
      });

      const loadingMs = Math.round(performance.now() - startTime);
      lastLookupTime.value = loadingMs;

      if (!response.ok) {
        const data = await response.json();
        return {
          isValid: false,
          customer: null,
          error: data.error || "LOOKUP_FAILED",
          message: data.message || "Kunde nicht gefunden",
          loadingMs,
        };
      }

      const data: ErpCustomer = await response.json();
      customer.value = data;
      return {
        isValid: true,
        customer: data,
        loadingMs,
      };
    } catch (err) {
      const loadingMs = Math.round(performance.now() - startTime);
      const errorMessage =
        err instanceof Error ? err.message : "Unbekannter Fehler";
      error.value = errorMessage;

      return {
        isValid: false,
        customer: null,
        error: "NETWORK_ERROR",
        message: `Verbindungsfehler: ${errorMessage}`,
        loadingMs,
      };
    } finally {
      isLoading.value = false;
    }
  };

  /**
   * Clear customer data and error state
   */
  const clearCustomer = () => {
    customer.value = null;
    error.value = null;
    lastLookupTime.value = null;
  };

  return {
    // State
    isLoading,
    customer,
    error,
    lastLookupTime,

    // Computed
    hasCustomer,
    isPrivateCustomer,
    isBusinessCustomer,

    // Methods
    validateCustomerEmail,
    validateCustomerNumber,
    clearCustomer,
  };
}
