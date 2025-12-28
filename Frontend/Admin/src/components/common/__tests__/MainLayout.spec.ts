import { describe, it, expect, beforeEach, vi } from "vitest";
import { mount } from "@vue/test-utils";
import { createRouter, createMemoryHistory } from "vue-router";
import MainLayout from "../MainLayout.vue";
import { createPinia, setActivePinia } from "pinia";

describe("MainLayout.vue", () => {
  let router: any;
  let pinia: any;

  beforeEach(() => {
    // Setup Pinia
    pinia = createPinia();
    setActivePinia(pinia);

    // Setup Vue Router
    router = createRouter({
      history: createMemoryHistory(),
      routes: [
        {
          path: "/",
          name: "Dashboard",
          component: { template: "<div>Dashboard</div>" },
        },
        {
          path: "/users",
          name: "Users",
          component: { template: "<div>Users</div>" },
        },
        {
          path: "/products",
          name: "Products",
          component: { template: "<div>Products</div>" },
        },
      ],
    });
  });

  describe("Component Rendering", () => {
    it("should render MainLayout component", () => {
      const wrapper = mount(MainLayout, {
        global: {
          plugins: [router, pinia],
          stubs: {
            RouterLink: true,
            RouterView: true,
          },
        },
      });

      expect(wrapper.exists()).toBe(true);
      expect(wrapper.find('[data-test="main-layout"]').exists()).toBe(true);
    });

    it("should render sidebar navigation", () => {
      const wrapper = mount(MainLayout, {
        global: {
          plugins: [router, pinia],
          stubs: { RouterView: true },
        },
      });

      const sidebar = wrapper.find('[data-test="sidebar"]');
      expect(sidebar.exists()).toBe(true);
    });

    it("should render top navigation bar", () => {
      const wrapper = mount(MainLayout, {
        global: {
          plugins: [router, pinia],
          stubs: { RouterView: true },
        },
      });

      const topNav = wrapper.find('[data-test="top-nav"]');
      expect(topNav.exists()).toBe(true);
    });

    it("should render user menu in header", () => {
      const wrapper = mount(MainLayout, {
        global: {
          plugins: [router, pinia],
          stubs: { RouterView: true },
        },
      });

      const userMenu = wrapper.find('[data-test="user-menu"]');
      expect(userMenu.exists()).toBe(true);
    });
  });

  describe("Sidebar Navigation", () => {
    it("should render all navigation links", () => {
      const wrapper = mount(MainLayout, {
        global: {
          plugins: [router, pinia],
          stubs: { RouterView: true },
        },
      });

      const navLinks = wrapper.findAll('[data-test="nav-link"]');
      expect(navLinks.length).toBeGreaterThanOrEqual(3); // Dashboard, Users, Products
    });

    it("should highlight active nav link", async () => {
      const wrapper = mount(MainLayout, {
        global: {
          plugins: [router, pinia],
          stubs: { RouterView: true },
        },
      });

      // Navigate to a route
      await router.push("/");
      await wrapper.vm.$nextTick();

      const activeLink = wrapper.find('[data-test="nav-link"].active');
      expect(activeLink.exists()).toBe(true);
    });

    it("should toggle sidebar on mobile", async () => {
      const wrapper = mount(MainLayout, {
        global: {
          plugins: [router, pinia],
          stubs: { RouterView: true },
        },
      });

      const toggleButton = wrapper.find('[data-test="sidebar-toggle"]');
      expect(toggleButton.exists()).toBe(true);

      // Simulate toggle
      await toggleButton.trigger("click");
      await wrapper.vm.$nextTick();

      const sidebar = wrapper.find('[data-test="sidebar"]');
      expect(sidebar.classes()).toContain("open") ||
        expect(sidebar.classes()).toContain("closed");
    });
  });

  describe("User Menu", () => {
    it("should display user name in header", () => {
      const wrapper = mount(MainLayout, {
        global: {
          plugins: [router, pinia],
          stubs: { RouterView: true },
        },
      });

      const userNameElement = wrapper.find('[data-test="user-name"]');
      expect(userNameElement.exists()).toBe(true);
    });

    it("should open user menu dropdown on click", async () => {
      const wrapper = mount(MainLayout, {
        global: {
          plugins: [router, pinia],
          stubs: { RouterView: true },
        },
      });

      const userMenuButton = wrapper.find('[data-test="user-menu-button"]');
      await userMenuButton.trigger("click");
      await wrapper.vm.$nextTick();

      const dropdown = wrapper.find('[data-test="user-menu-dropdown"]');
      expect(dropdown.isVisible()).toBe(true);
    });

    it("should display logout option in user menu", () => {
      const wrapper = mount(MainLayout, {
        global: {
          plugins: [router, pinia],
          stubs: { RouterView: true },
        },
      });

      const logoutButton = wrapper.find('[data-test="logout-button"]');
      expect(logoutButton.exists()).toBe(true);
    });

    it("should emit logout event on logout click", async () => {
      const wrapper = mount(MainLayout, {
        global: {
          plugins: [router, pinia],
          stubs: { RouterView: true },
        },
      });

      const logoutButton = wrapper.find('[data-test="logout-button"]');
      await logoutButton.trigger("click");

      expect(wrapper.emitted("logout")).toBeTruthy();
    });
  });

  describe("Accessibility", () => {
    it("should have proper ARIA labels on sidebar", () => {
      const wrapper = mount(MainLayout, {
        global: {
          plugins: [router, pinia],
          stubs: { RouterView: true },
        },
      });

      const sidebar = wrapper.find('[data-test="sidebar"]');
      expect(sidebar.attributes("aria-label")).toBeTruthy();
    });

    it("should have proper ARIA labels on user menu", () => {
      const wrapper = mount(MainLayout, {
        global: {
          plugins: [router, pinia],
          stubs: { RouterView: true },
        },
      });

      const userMenuButton = wrapper.find('[data-test="user-menu-button"]');
      expect(userMenuButton.attributes("aria-haspopup")).toBe("menu");
      expect(userMenuButton.attributes("aria-label")).toBeTruthy();
    });

    it("should be keyboard navigable", async () => {
      const wrapper = mount(MainLayout, {
        global: {
          plugins: [router, pinia],
          stubs: { RouterView: true },
        },
      });

      const navLinks = wrapper.findAll('[data-test="nav-link"]');
      for (const link of navLinks) {
        expect(link.attributes("tabindex")).not.toBe("-1");
      }
    });
  });

  describe("Layout Styling", () => {
    it("should have correct CSS classes applied", () => {
      const wrapper = mount(MainLayout, {
        global: {
          plugins: [router, pinia],
          stubs: { RouterView: true },
        },
      });

      const mainLayout = wrapper.find('[data-test="main-layout"]');
      expect(mainLayout.classes()).toContain("main-layout");
    });

    it("should apply responsive classes on mobile", () => {
      const wrapper = mount(MainLayout, {
        global: {
          plugins: [router, pinia],
          stubs: { RouterView: true },
        },
      });

      const sidebar = wrapper.find('[data-test="sidebar"]');
      // Check for mobile-specific responsive classes
      expect(
        sidebar.classes("sidebar-mobile") ||
          sidebar.classes("sidebar-desktop") ||
          sidebar.classes("sidebar")
      ).toBe(true);
    });
  });
});
