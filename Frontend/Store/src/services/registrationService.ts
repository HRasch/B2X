/**
 * Registration API Service
 * Story 8: Check Customer Registration Type
 *
 * Calls Wolverine HTTP endpoint: POST /checkregistrationtype
 * Location: B2Connect.Identity.API on port 7002
 */

import axios from "axios";
import type { AxiosError } from "axios";

const API_BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:7002";
const REGISTRATION_API = `${API_BASE_URL}/checkregistrationtype`;

/**
 * Request body for CheckRegistrationTypeCommand
 * Story 8: Check Customer Type Wolverine Handler
 */
export interface CheckRegistrationTypeRequest {
  email: string;
  businessType: "B2C" | "B2B";
  firstName?: string;
  lastName?: string;
  companyName?: string;
  phone?: string;
}

/**
 * Response from Wolverine CheckRegistrationTypeService
 * Returns registration type and ERP data if found
 */
export interface CheckRegistrationTypeResponse {
  success: boolean;
  registrationType: "NewCustomer" | "ExistingCustomer" | "Bestandskunde";
  erpCustomerId?: string;
  erpData?: {
    customerNumber: string;
    name: string;
    email: string;
    phone?: string;
    address?: string;
    city?: string;
    postalCode?: string;
    country?: string;
    taxId?: string;
    isActive: boolean;
  };
  error?: string;
  message?: string;
  confidenceScore?: number;
}

/**
 * Check registration type using Wolverine HTTP endpoint
 *
 * @param request CheckRegistrationTypeRequest
 * @returns Promise<CheckRegistrationTypeResponse>
 * @throws AxiosError on HTTP error
 */
export async function checkRegistrationType(
  request: CheckRegistrationTypeRequest
): Promise<CheckRegistrationTypeResponse> {
  try {
    const response = await axios.post<CheckRegistrationTypeResponse>(
      REGISTRATION_API,
      request,
      {
        headers: {
          "Content-Type": "application/json",
          "X-Tenant-ID": getTenantId(), // Multi-tenant isolation
        },
        timeout: 10000, // 10 second timeout
      }
    );

    return response.data;
  } catch (error) {
    const axiosError = error as AxiosError;
    console.error("CheckRegistrationType API Error:", {
      status: axiosError.response?.status,
      message: axiosError.message,
      data: axiosError.response?.data,
    });

    throw {
      success: false,
      registrationType: "NewCustomer",
      error: "API_ERROR",
      message:
        axiosError.response?.status === 503
          ? "Service temporarily unavailable"
          : axiosError.message || "Failed to check registration type",
    } as CheckRegistrationTypeResponse;
  }
}

/**
 * Get tenant ID from session/localStorage
 * For multi-tenant isolation
 */
function getTenantId(): string {
  // This would come from your auth context
  return localStorage.getItem("tenantId") || "";
}

/**
 * Format registration type for display
 */
export function formatRegistrationType(type: string): string {
  const typeMap: Record<string, string> = {
    NewCustomer: "Neukunde",
    ExistingCustomer: "Bestandskunde",
    Bestandskunde: "Bestandskunde",
  };
  return typeMap[type] || type;
}

/**
 * Validate email format
 */
export function validateEmail(email: string): boolean {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
}

/**
 * Normalize phone number
 */
export function normalizePhone(phone: string): string {
  // Remove all non-numeric characters
  return phone.replace(/\D/g, "");
}
