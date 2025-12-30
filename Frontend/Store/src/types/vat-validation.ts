/**
 * VAT ID Validation Types
 * Issue #31: B2B VAT-ID Validation (AStV Reverse Charge)
 */

/**
 * Request to validate a B2B customer's VAT ID
 */
export interface ValidateVatIdRequest {
  countryCode: string;
  vatNumber: string;
  buyerCountry?: string;
  sellerCountry?: string;
}

/**
 * Response from VAT ID validation
 */
export interface ValidateVatIdResponse {
  isValid: boolean;
  vatId: string;
  companyName?: string;
  companyAddress?: string;
  reverseChargeApplies: boolean;
  message: string;
  expiresAt: string;
  error?: string;
}

/**
 * B2B Customer Information with VAT Validation
 */
export interface B2BCustomer {
  registrationType: "B2B";
  companyName: string;
  vatId: string;
  vatIdValidated: boolean;
  reverseChargeApplies: boolean;
  validatedAt?: Date;
}

/**
 * VAT Validation Result for caching/display
 */
export interface VatValidationCache {
  vatId: string;
  isValid: boolean;
  companyName?: string;
  companyAddress?: string;
  reverseChargeApplies: boolean;
  validatedAt: Date;
  expiresAt: Date;
}
