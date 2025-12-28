import { describe, it, expect, beforeEach, vi } from "vitest";
import { mount } from "@vue/test-utils";
import { createI18n } from "vue-i18n";
import DisputeResolution from "../DisputeResolution.vue";

// Mock i18n messages
const messages = {
  en: {
    disputes: {
      title: "Online Dispute Resolution",
      subtitle: "Resolve disputes through our EU-compliant platform",
      order_number: "Order Number",
      email: "Email Address",
      name: "Full Name",
      description: "Dispute Description",
      dispute_type: "Type of Dispute",
      select_type: "Select a type",
      submit_button: "Submit Dispute",
      submitting: "Submitting...",
      submit_success: "Dispute submitted successfully!",
      submit_error: "Error submitting dispute. Please try again.",
      error_order_required: "Order number is required",
      error_email_required: "Email is required",
      error_email_invalid: "Please enter a valid email",
      error_name_required: "Name is required",
      error_name_too_short: "Name is too short",
      error_description_required: "Description is required",
      error_description_too_short: "Description is too short",
      error_type_required: "Please select a type",
      error_consent_required: "You must accept the terms",
    },
  },
};

describe("DisputeResolution.vue", () => {
  let i18n: any;

  beforeEach(() => {
    i18n = createI18n({
      locale: "en",
      messages,
    });
  });

  it("renders dispute resolution form", () => {
    const wrapper = mount(DisputeResolution, {
      global: {
        plugins: [i18n],
      },
    });

    expect(wrapper.find("h1").text()).toContain("Online Dispute Resolution");
    expect(wrapper.find("form").exists()).toBe(true);
  });

  it("displays all form fields", () => {
    const wrapper = mount(DisputeResolution, {
      global: {
        plugins: [i18n],
      },
    });

    expect(wrapper.find('input[type="text"]').exists()).toBe(true); // order number
    expect(wrapper.find('input[type="email"]').exists()).toBe(true); // email
    expect(wrapper.find("textarea").exists()).toBe(true); // description
    expect(wrapper.find("select").exists()).toBe(true); // dispute type
    expect(wrapper.find('input[type="checkbox"]').exists()).toBe(true); // consent
  });

  it("validates required fields", async () => {
    const wrapper = mount(DisputeResolution, {
      global: {
        plugins: [i18n],
      },
    });

    const form = wrapper.find("form");
    await form.trigger("submit");

    expect(wrapper.vm.errors.orderNumber).toBeDefined();
    expect(wrapper.vm.errors.email).toBeDefined();
    expect(wrapper.vm.errors.name).toBeDefined();
  });

  it("validates email format", async () => {
    const wrapper = mount(DisputeResolution, {
      global: {
        plugins: [i18n],
      },
    });

    wrapper.vm.form.email = "invalid-email";
    await wrapper.vm.validateField("email");

    expect(wrapper.vm.errors.email).toBeDefined();
  });

  it("accepts valid form data", async () => {
    const wrapper = mount(DisputeResolution, {
      global: {
        plugins: [i18n],
      },
    });

    wrapper.vm.form.orderNumber = "12345";
    wrapper.vm.form.email = "test@example.com";
    wrapper.vm.form.name = "John Doe";
    wrapper.vm.form.description = "Product quality issue";
    wrapper.vm.form.type = "product_quality";
    wrapper.vm.form.consent = true;

    const isValid = wrapper.vm.validateForm();

    expect(isValid).toBe(true);
    expect(Object.keys(wrapper.vm.errors).length).toBe(0);
  });

  it("toggles FAQ items", async () => {
    const wrapper = mount(DisputeResolution, {
      global: {
        plugins: [i18n],
      },
    });

    expect(wrapper.vm.expandedFaq[0]).toBeFalsy();

    wrapper.vm.toggleFaq(0);
    await wrapper.vm.$nextTick();

    expect(wrapper.vm.expandedFaq[0]).toBeTruthy();

    wrapper.vm.toggleFaq(0);
    await wrapper.vm.$nextTick();

    expect(wrapper.vm.expandedFaq[0]).toBeFalsy();
  });

  it("disables submit button while submitting", async () => {
    const wrapper = mount(DisputeResolution, {
      global: {
        plugins: [i18n],
      },
    });

    wrapper.vm.isSubmitting = true;
    await wrapper.vm.$nextTick();

    const submitButton = wrapper.find('button[type="submit"]');
    expect(submitButton.attributes("disabled")).toBeDefined();
  });

  it("displays success message after successful submission", async () => {
    const wrapper = mount(DisputeResolution, {
      global: {
        plugins: [i18n],
      },
    });

    wrapper.vm.form.orderNumber = "12345";
    wrapper.vm.form.email = "test@example.com";
    wrapper.vm.form.name = "John Doe";
    wrapper.vm.form.description = "Product quality issue";
    wrapper.vm.form.type = "product_quality";
    wrapper.vm.form.consent = true;

    await wrapper.vm.handleSubmit();

    expect(wrapper.vm.submitSuccess).toBe(true);
  });

  it("displays error message on submission failure", async () => {
    const wrapper = mount(DisputeResolution, {
      global: {
        plugins: [i18n],
      },
    });

    // Try to submit without valid data
    wrapper.vm.form.orderNumber = "";
    await wrapper.vm.handleSubmit();

    expect(wrapper.vm.submitSuccess).toBe(false);
  });

  it("resets form after successful submission", async () => {
    const wrapper = mount(DisputeResolution, {
      global: {
        plugins: [i18n],
      },
    });

    wrapper.vm.form.orderNumber = "12345";
    wrapper.vm.form.email = "test@example.com";
    wrapper.vm.form.name = "John Doe";
    wrapper.vm.form.description = "Product quality issue";
    wrapper.vm.form.type = "product_quality";
    wrapper.vm.form.consent = true;

    await wrapper.vm.handleSubmit();

    expect(wrapper.vm.form.orderNumber).toBe("");
    expect(wrapper.vm.form.email).toBe("");
    expect(wrapper.vm.form.name).toBe("");
    expect(wrapper.vm.form.consent).toBe(false);
  });

  it("renders EU ODR platform link", () => {
    const wrapper = mount(DisputeResolution, {
      global: {
        plugins: [i18n],
      },
    });

    const link = wrapper.find('a[href="https://ec.europa.eu/consumers/odr/"]');
    expect(link.exists()).toBe(true);
    expect(link.attributes("target")).toBe("_blank");
  });
});
