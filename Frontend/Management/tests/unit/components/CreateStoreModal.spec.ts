import { describe, it, expect, vi } from "vitest";
import { mount } from "@vue/test-utils";
import CreateStoreModal from "@/components/CreateStoreModal.vue";

describe("CreateStoreModal.vue", () => {
  let wrapper: any;

  const createWrapper = (props = {}) => {
    return mount(CreateStoreModal, {
      props,
      global: {
        stubs: ["teleport"],
      },
    });
  };

  beforeEach(() => {
    wrapper = createWrapper();
  });

  describe("Rendering", () => {
    it("should render the modal with form", () => {
      expect(wrapper.find(".modal-overlay").exists()).toBe(true);
      expect(wrapper.find(".modal-content").exists()).toBe(true);
      expect(wrapper.find("h2").text()).toBe("Create Store Instance");
      expect(wrapper.find("form").exists()).toBe(true);
    });

    it("should render all form fields", () => {
      expect(wrapper.find('input[id="name"]').exists()).toBe(true);
      expect(wrapper.find('input[id="domain"]').exists()).toBe(true);
      expect(wrapper.find('select[id="status"]').exists()).toBe(true);
    });

    it("should render action buttons", () => {
      expect(wrapper.find(".btn-cancel").exists()).toBe(true);
      expect(wrapper.find(".btn-submit").exists()).toBe(true);
    });
  });

  describe("Form Fields", () => {
    it("should have correct placeholders", () => {
      const nameInput = wrapper.find('input[id="name"]');
      const domainInput = wrapper.find('input[id="domain"]');

      expect(nameInput.attributes("placeholder")).toBe("My Store");
      expect(domainInput.attributes("placeholder")).toBe("mystore.example.com");
    });

    it("should have required attributes", () => {
      const nameInput = wrapper.find('input[id="name"]');
      const domainInput = wrapper.find('input[id="domain"]');
      const statusSelect = wrapper.find('select[id="status"]');

      expect(nameInput.attributes("required")).toBeDefined();
      expect(domainInput.attributes("required")).toBeDefined();
      expect(statusSelect.attributes("required")).toBeDefined();
    });

    it("should have correct status options", () => {
      const options = wrapper.findAll("option");
      expect(options.length).toBe(3);
      expect(options[0].text()).toBe("Active");
      expect(options[1].text()).toBe("Inactive");
      expect(options[2].text()).toBe("Suspended");
    });
  });

  describe("Form Interaction", () => {
    it("should update form data on input", async () => {
      const nameInput = wrapper.find('input[id="name"]');
      const domainInput = wrapper.find('input[id="domain"]');
      const statusSelect = wrapper.find('select[id="status"]');

      await nameInput.setValue("Test Store");
      await domainInput.setValue("test.example.com");
      await statusSelect.setValue("inactive");

      expect(wrapper.vm.form.name).toBe("Test Store");
      expect(wrapper.vm.form.domain).toBe("test.example.com");
      expect(wrapper.vm.form.status).toBe("inactive");
    });

    it("should show loading state during submission", async () => {
      const submitButton = wrapper.find(".btn-submit");

      // Initially not loading
      expect(submitButton.text()).toBe("Create Store");

      // Set loading state
      wrapper.vm.loading = true;
      await wrapper.vm.$nextTick();

      expect(submitButton.text()).toBe("Creating...");
      expect(submitButton.attributes("disabled")).toBeDefined();
    });
  });

  describe("Form Submission", () => {
    it("should emit created event with store data on successful submission", async () => {
      const nameInput = wrapper.find('input[id="name"]');
      const domainInput = wrapper.find('input[id="domain"]');
      const form = wrapper.find("form");

      await nameInput.setValue("Test Store");
      await domainInput.setValue("test.example.com");
      await form.trigger("submit");

      expect(wrapper.emitted("created")).toBeTruthy();
      const emittedStore = wrapper.emitted("created")[0][0];

      expect(emittedStore.name).toBe("Test Store");
      expect(emittedStore.domain).toBe("test.example.com");
      expect(emittedStore.status).toBe("active");
      expect(emittedStore.id).toBeDefined();
      expect(emittedStore.createdAt).toBeDefined();
      expect(emittedStore.updatedAt).toBeDefined();
    });

    it("should reset form after successful submission", async () => {
      const nameInput = wrapper.find('input[id="name"]');
      const domainInput = wrapper.find('input[id="domain"]');
      const form = wrapper.find("form");

      await nameInput.setValue("Test Store");
      await domainInput.setValue("test.example.com");
      await form.trigger("submit");

      expect(wrapper.vm.form.name).toBe("");
      expect(wrapper.vm.form.domain).toBe("");
      expect(wrapper.vm.form.status).toBe("active");
    });

    it("should handle submission errors", async () => {
      // Set error state manually to simulate error handling
      wrapper.vm.error = "API Error";
      wrapper.vm.loading = false;

      await wrapper.vm.$nextTick();

      const errorElement = wrapper.find(".error-message");
      expect(errorElement.exists()).toBe(true);
      expect(errorElement.text()).toBe("API Error");
      expect(wrapper.vm.loading).toBe(false);
    });
  });

  describe("Modal Actions", () => {
    it("should emit close event when close button is clicked", async () => {
      const closeButton = wrapper.find(".btn-close");
      await closeButton.trigger("click");

      expect(wrapper.emitted("close")).toBeTruthy();
    });

    it("should emit close event when cancel button is clicked", async () => {
      const cancelButton = wrapper.find(".btn-cancel");
      await cancelButton.trigger("click");

      expect(wrapper.emitted("close")).toBeTruthy();
    });

    it("should emit close event when overlay is clicked", async () => {
      const overlay = wrapper.find(".modal-overlay");
      await overlay.trigger("click");

      expect(wrapper.emitted("close")).toBeTruthy();
    });

    it("should not emit close event when modal content is clicked", async () => {
      const modalContent = wrapper.find(".modal-content");
      await modalContent.trigger("click");

      expect(wrapper.emitted("close")).toBeFalsy();
    });
  });

  describe("Accessibility", () => {
    it("should have proper labels for inputs", () => {
      expect(wrapper.find('label[for="name"]').exists()).toBe(true);
      expect(wrapper.find('label[for="domain"]').exists()).toBe(true);
      expect(wrapper.find('label[for="status"]').exists()).toBe(true);
    });

    it("should have unique ids for form elements", () => {
      expect(wrapper.find("#name").exists()).toBe(true);
      expect(wrapper.find("#domain").exists()).toBe(true);
      expect(wrapper.find("#status").exists()).toBe(true);
    });
  });
});
