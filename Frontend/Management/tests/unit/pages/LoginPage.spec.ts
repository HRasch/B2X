import { describe, it, expect, vi, beforeEach } from "vitest";
import { mount } from "@vue/test-utils";
import { createPinia, setActivePinia } from "pinia";
import { createRouter, createWebHistory } from "vue-router";
import LoginPage from "@/pages/LoginPage.vue";
import { useAuthStore } from "@/stores/authStore";

// Mock the API client
vi.mock("@/services/api", () => ({
  default: {
    post: vi.fn(),
  },
}));

import api from "@/services/api";

// Mock router
const mockRouter = {
  push: vi.fn(),
};

vi.mock("vue-router", () => ({
  useRouter: () => mockRouter,
}));

describe("LoginPage.vue", () => {
  let pinia: any;
  let authStore: any;
  let mockApi: any;

  beforeEach(() => {
    pinia = createPinia();
    setActivePinia(pinia);
    authStore = useAuthStore();
    mockApi = vi.mocked(api);

    // Reset store state
    authStore.logout();

    // Reset mocks
    vi.clearAllMocks();
  });

  const createWrapper = () => {
    return mount(LoginPage, {
      global: {
        plugins: [pinia],
      },
    });
  };

  describe("Rendering", () => {
    it("should render the login form", () => {
      const wrapper = createWrapper();
      expect(wrapper.find("h2").text()).toBe("Tenant Management Portal");
      expect(wrapper.find("form").exists()).toBe(true);
      expect(wrapper.find('input[type="email"]').exists()).toBe(true);
      expect(wrapper.find('input[type="password"]').exists()).toBe(true);
      expect(wrapper.find('button[type="submit"]').exists()).toBe(true);
    });

    it("should have correct form labels and placeholders", () => {
      const wrapper = createWrapper();
      const emailInput = wrapper.find('input[type="email"]');
      const passwordInput = wrapper.find('input[type="password"]');

      expect(emailInput.attributes("placeholder")).toBe("admin@example.com");
      expect(passwordInput.attributes("placeholder")).toBe("••••••••");
    });
  });

  describe("Form Interaction", () => {
    it("should update email and password on input", async () => {
      const wrapper = createWrapper();
      const emailInput = wrapper.find('input[type="email"]');
      const passwordInput = wrapper.find('input[type="password"]');

      await emailInput.setValue("test@example.com");
      await passwordInput.setValue("password123");

      expect(emailInput.element.value).toBe("test@example.com");
      expect(passwordInput.element.value).toBe("password123");
    });

    it("should show loading state during submission", async () => {
      const wrapper = createWrapper();
      const button = wrapper.find('button[type="submit"]');

      // Initially not loading
      expect(button.text()).toBe("Sign in");

      // Simulate loading state by setting the ref
      await wrapper.vm.$nextTick();
      wrapper.vm.loading = true;
      await wrapper.vm.$nextTick();

      expect(button.text()).toBe("Signing in...");
      expect(button.attributes("disabled")).toBeDefined();
    });
  });

  describe("Form Submission", () => {
    it("should call API on form submission", async () => {
      const wrapper = createWrapper();
      mockApi.post.mockResolvedValue({
        data: {
          data: {
            token: "fake-token",
            userId: "user-123",
            email: "test@example.com",
          },
        },
      });

      const emailInput = wrapper.find('input[type="email"]');
      const passwordInput = wrapper.find('input[type="password"]');
      const form = wrapper.find("form");

      await emailInput.setValue("test@example.com");
      await passwordInput.setValue("password123");
      await form.trigger("submit");

      expect(mockApi.post).toHaveBeenCalledWith("/auth/login", {
        email: "test@example.com",
        password: "password123",
      });
    });

    it("should handle successful login", async () => {
      const wrapper = createWrapper();
      mockApi.post.mockResolvedValue({
        data: {
          data: {
            token: "fake-token",
            userId: "user-123",
            email: "test@example.com",
          },
        },
      });

      const emailInput = wrapper.find('input[type="email"]');
      const passwordInput = wrapper.find('input[type="password"]');
      const form = wrapper.find("form");

      await emailInput.setValue("test@example.com");
      await passwordInput.setValue("password123");
      await form.trigger("submit");

      // Wait for async operations
      await wrapper.vm.$nextTick();

      expect(authStore.token).toBe("fake-token");
      expect(authStore.userId).toBe("user-123");
      expect(authStore.email).toBe("test@example.com");
      expect(authStore.isAuthenticated).toBe(true);
      expect(mockRouter.push).toHaveBeenCalledWith("/dashboard");
    });

    it("should handle login error", async () => {
      const wrapper = createWrapper();
      const errorMessage = "Invalid credentials";
      mockApi.post.mockRejectedValue({
        response: {
          data: { message: errorMessage },
        },
      });

      const emailInput = wrapper.find('input[type="email"]');
      const passwordInput = wrapper.find('input[type="password"]');
      const form = wrapper.find("form");

      await emailInput.setValue("test@example.com");
      await passwordInput.setValue("wrongpassword");
      await form.trigger("submit");

      await wrapper.vm.$nextTick();

      expect(wrapper.find(".error-message").text()).toBe(errorMessage);
      expect(authStore.token).toBeNull();
      expect(authStore.isAuthenticated).toBe(false);
      expect(mockRouter.push).not.toHaveBeenCalled();
    });

    it("should handle network error", async () => {
      const wrapper = createWrapper();
      mockApi.post.mockRejectedValue(new Error("Network error"));

      const emailInput = wrapper.find('input[type="email"]');
      const passwordInput = wrapper.find('input[type="password"]');
      const form = wrapper.find("form");

      await emailInput.setValue("test@example.com");
      await passwordInput.setValue("password123");
      await form.trigger("submit");

      await wrapper.vm.$nextTick();

      expect(wrapper.find(".error-message").text()).toBe(
        "Login failed. Please try again."
      );
    });
  });

  describe("Accessibility", () => {
    it("should have proper labels for inputs", () => {
      const wrapper = createWrapper();
      const emailInput = wrapper.find('input[type="email"]');
      const passwordInput = wrapper.find('input[type="password"]');

      expect(emailInput.attributes("id")).toBe("email");
      expect(passwordInput.attributes("id")).toBe("password");
      expect(wrapper.find('label[for="email"]').exists()).toBe(true);
      expect(wrapper.find('label[for="password"]').exists()).toBe(true);
    });

    it("should have required attributes on inputs", () => {
      const wrapper = createWrapper();
      const emailInput = wrapper.find('input[type="email"]');
      const passwordInput = wrapper.find('input[type="password"]');

      expect(emailInput.attributes("required")).toBeDefined();
      expect(passwordInput.attributes("required")).toBeDefined();
    });
  });
});
