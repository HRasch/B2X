import { ref, computed } from "vue";
import type {
  ValidateVatIdRequest,
  ValidateVatIdResponse,
} from "../types/vat-validation";

/**
 * Composable for B2B VAT ID validation
 * Issue #31: B2B VAT-ID Validation (AStV Reverse Charge)
 */
export function useVatIdValidation(sellerCountry: string = "DE") {
  const vatValidation = ref<ValidateVatIdResponse | null>(null);
  const isValidating = ref(false);
  const error = ref<string | null>(null);

  const isValid = computed(() => vatValidation.value?.isValid ?? false);
  const reverseChargeApplies = computed(
    () => vatValidation.value?.reverseChargeApplies ?? false
  );
  const companyName = computed(() => vatValidation.value?.companyName);

  /**
   * Validate a VAT ID against the VIES API
   */
  const validateVatId = async (
    countryCode: string,
    vatNumber: string,
    buyerCountry?: string
  ): Promise<ValidateVatIdResponse | null> => {
    isValidating.value = true;
    error.value = null;

    try {
      const request: ValidateVatIdRequest = {
        countryCode,
        vatNumber,
        sellerCountry,
        buyerCountry,
      };

      const response = await fetch("/api/validatevatid", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(request),
      });

      if (!response.ok) {
        throw new Error(`HTTP ${response.status}`);
      }

      const result: ValidateVatIdResponse = await response.json();
      vatValidation.value = result;

      if (!result.isValid) {
        error.value = result.message || "VAT ID validation failed";
      }

      return result;
    } catch (e) {
      const errorMessage = e instanceof Error ? e.message : "Unknown error";
      error.value = `Validation failed: ${errorMessage}`;
      console.error("VAT validation error:", e);
      return null;
    } finally {
      isValidating.value = false;
    }
  };

  /**
   * Clear validation state
   */
  const clearValidation = () => {
    vatValidation.value = null;
    error.value = null;
    isValidating.value = false;
  };

  /**
   * Format full VAT ID with country code
   */
  const formatVatId = (countryCode: string, vatNumber: string): string => {
    return `${countryCode}${vatNumber}`;
  };

  return {
    // State
    vatValidation,
    isValidating,
    error,

    // Computed
    isValid,
    reverseChargeApplies,
    companyName,

    // Methods
    validateVatId,
    clearValidation,
    formatVatId,
  };
}
