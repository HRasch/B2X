/**
 * Vitest Unit Tests for RegistrationCheck Component
 * Story 8: Check Customer Registration Type
 */

import { describe, it, expect, beforeEach, vi } from "vitest";
import { mount } from "@vue/test-utils";
import { createRouter, createMemoryHistory } from "vue-router";
import RegistrationCheck from "./RegistrationCheck.vue";
import * as registrationService from "@/services/registrationService";
import type { CheckRegistrationTypeResponse } from "@/services/registrationService";

// Mock the registration service
vi.mock("@/services/registrationService", () => ({
  checkRegistrationType: vi.fn(),
  validateEmail: vi.fn((email) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)),
  formatRegistrationType: vi.fn((type) => {
    const map: Record<string, string> = {
      NewCustomer: "Neukunde",
      ExistingCustomer: "Bestandskunde",
      Bestandskunde: "Bestandskunde",
    };
    return map[type] || type;
  }),
}));

describe("RegistrationCheck.vue", () => {
  let router: any;

  beforeEach(() => {
    router = createRouter({
      history: createMemoryHistory(),
      routes: [
        {
          path: "/registration-check",
          component: RegistrationCheck,
        },
        {
          path: "/registration-bestandskunde/:customerId",
          name: "registration-bestandskunde",
          component: { template: "<div>Bestandskunde Registration</div>" },
        },
        {
          path: "/registration-new",
          name: "registration-new",
          component: { template: "<div>New Registration</div>" },
        },
      ],
    });
  });

  it("renders registration check form", async () => {
    const wrapper = mount(RegistrationCheck, {
      global: {
        plugins: [router],
      },
    });

    expect(wrapper.find("h1").text()).toContain("Registrierungstyp Prüfen");
    expect(wrapper.find('[data-testid="email-input"]').exists()).toBe(true);
    expect(wrapper.find('[data-testid="business-type-select"]').exists()).toBe(
      true
    );
    expect(wrapper.find('[data-testid="submit-button"]').exists()).toBe(true);
  });

  it("updates email on input change", async () => {
    const wrapper = mount(RegistrationCheck, {
      global: {
        plugins: [router],
      },
    });

    const emailInput = wrapper.find('[data-testid="email-input"]');
    await emailInput.setValue("test@example.com");

    expect((wrapper.vm as any).formData.email).toBe("test@example.com");
  });

  it("updates business type on select change", async () => {
    const wrapper = mount(RegistrationCheck, {
      global: {
        plugins: [router],
      },
    });

    const select = wrapper.find('[data-testid="business-type-select"]');
    await select.setValue("B2B");

    expect((wrapper.vm as any).formData.businessType).toBe("B2B");
  });

  it("disables submit button when email or business type is empty", async () => {
    const wrapper = mount(RegistrationCheck, {
      global: {
        plugins: [router],
      },
    });

    const submitButton = wrapper.find('[data-testid="submit-button"]');
    expect(submitButton.attributes("disabled")).toBeDefined();
  });

  it("enables submit button when form is valid", async () => {
    const wrapper = mount(RegistrationCheck, {
      global: {
        plugins: [router],
      },
    });

    await wrapper
      .find('[data-testid="email-input"]')
      .setValue("test@example.com");
    await wrapper.find('[data-testid="business-type-select"]').setValue("B2B");

    const submitButton = wrapper.find('[data-testid="submit-button"]');
    expect(submitButton.attributes("disabled")).toBeUndefined();
  });

  it("validates email format on blur", async () => {
    const wrapper = mount(RegistrationCheck, {
      global: {
        plugins: [router],
      },
    });

    const emailInput = wrapper.find('[data-testid="email-input"]');
    await emailInput.setValue("invalid-email");
    await emailInput.trigger("blur");

    await wrapper.vm.$nextTick();
    expect((wrapper.vm as any).emailError).toBeTruthy();
  });

  it("shows success message on successful registration check", async () => {
    const mockResponse: CheckRegistrationTypeResponse = {
      success: true,
      registrationType: "Bestandskunde" as const,
      erpData: {
        customerNumber: "123456",
        name: "Max Mustermann",
        email: "max@example.com",
        isActive: true,
      },
    };

    vi.mocked(registrationService.checkRegistrationType).mockResolvedValueOnce(
      mockResponse
    );

    const wrapper = mount(RegistrationCheck, {
      global: {
        plugins: [router],
      },
    });

    await wrapper
      .find('[data-testid="email-input"]')
      .setValue("max@example.com");
    await wrapper.find('[data-testid="business-type-select"]').setValue("B2B");
    await wrapper.find('[data-testid="submit-button"]').trigger("click");

    await new Promise((resolve) => setTimeout(resolve, 100));
    await wrapper.vm.$nextTick();

    expect(wrapper.find('[data-testid="success-message"]').exists()).toBe(true);
  });

  it("shows error message on API error", async () => {
    const mockError = new Error("API Error");
    vi.mocked(registrationService.checkRegistrationType).mockRejectedValueOnce(
      mockError
    );

    const wrapper = mount(RegistrationCheck, {
      global: {
        plugins: [router],
      },
    });

    await wrapper
      .find('[data-testid="email-input"]')
      .setValue("test@example.com");
    await wrapper.find('[data-testid="business-type-select"]').setValue("B2C");
    await wrapper.find('[data-testid="submit-button"]').trigger("click");

    await new Promise((resolve) => setTimeout(resolve, 100));
    await wrapper.vm.$nextTick();

    expect(wrapper.find('[data-testid="error-message"]').exists()).toBe(true);
  });

  it("displays ERP data when found", async () => {
    const mockResponse: CheckRegistrationTypeResponse = {
      success: true,
      registrationType: "Bestandskunde" as const,
      erpData: {
        customerNumber: "123456",
        name: "Max Mustermann",
        email: "max@example.com",
        phone: "0123456789",
        address: "Musterstr. 1",
        postalCode: "12345",
        city: "Berlin",
        country: "Germany",
        isActive: true,
      },
    };

    vi.mocked(registrationService.checkRegistrationType).mockResolvedValueOnce(
      mockResponse
    );

    const wrapper = mount(RegistrationCheck, {
      global: {
        plugins: [router],
      },
    });

    await wrapper
      .find('[data-testid="email-input"]')
      .setValue("max@example.com");
    await wrapper.find('[data-testid="business-type-select"]').setValue("B2B");
    await wrapper.find('[data-testid="submit-button"]').trigger("click");

    await new Promise((resolve) => setTimeout(resolve, 100));
    await wrapper.vm.$nextTick();

    expect(wrapper.html()).toContain("123456");
    expect(wrapper.html()).toContain("Max Mustermann");
  });

  it("resets form on reset button click", async () => {
    const mockResponse: CheckRegistrationTypeResponse = {
      success: true,
      registrationType: "NewCustomer" as const,
    };

    vi.mocked(registrationService.checkRegistrationType).mockResolvedValueOnce(
      mockResponse
    );

    const wrapper = mount(RegistrationCheck, {
      global: {
        plugins: [router],
      },
    });

    await wrapper
      .find('[data-testid="email-input"]')
      .setValue("test@example.com");
    await wrapper.find('[data-testid="business-type-select"]').setValue("B2B");

    // Submit to show results
    await wrapper.find('[data-testid="submit-button"]').trigger("click");
    await new Promise((resolve) => setTimeout(resolve, 100));
    await wrapper.vm.$nextTick();

    // Find and click reset button
    const resetButtons = wrapper.findAll(".btn-secondary");
    if (resetButtons.length > 0) {
      await resetButtons[0].trigger("click");
      await wrapper.vm.$nextTick();

      expect((wrapper.vm as any).formData.email).toBe("");
      expect((wrapper.vm as any).result).toBeNull();
    }
  });

  it("closes alert when close button is clicked", async () => {
    const wrapper = mount(RegistrationCheck, {
      global: {
        plugins: [router],
      },
    });

    // Set error manually
    (wrapper.vm as any).error = "Test error message";
    await wrapper.vm.$nextTick();

    expect(wrapper.find('[data-testid="error-message"]').exists()).toBe(true);

    // Click close button
    await wrapper.find(".alert-close").trigger("click");
    await wrapper.vm.$nextTick();

    expect(wrapper.find('[data-testid="error-message"]').exists()).toBe(false);
  });

  it("displays confidence score when available", async () => {
    const mockResponse: CheckRegistrationTypeResponse = {
      success: true,
      registrationType: "ExistingCustomer" as const,
      confidenceScore: 85,
      erpData: {
        customerNumber: "123456",
        name: "Max Mustermann",
        email: "max@example.com",
        isActive: true,
      },
    };

    vi.mocked(registrationService.checkRegistrationType).mockResolvedValueOnce(
      mockResponse
    );

    const wrapper = mount(RegistrationCheck, {
      global: {
        plugins: [router],
      },
    });

    await wrapper
      .find('[data-testid="email-input"]')
      .setValue("max@example.com");
    await wrapper.find('[data-testid="business-type-select"]').setValue("B2B");
    await wrapper.find('[data-testid="submit-button"]').trigger("click");

    await new Promise((resolve) => setTimeout(resolve, 100));
    await wrapper.vm.$nextTick();

    expect(wrapper.html()).toContain("85%");
  });
});
