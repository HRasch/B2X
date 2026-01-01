import { describe, it, expect, beforeEach, vi, afterEach } from "vitest";
import { setActivePinia, createPinia } from "pinia";
import { useAuthStore } from "@/stores/authStore";

// Mock localStorage
const localStorageMock = {
  getItem: vi.fn(),
  setItem: vi.fn(),
  removeItem: vi.fn(),
  clear: vi.fn(),
};

Object.defineProperty(window, "localStorage", {
  value: localStorageMock,
  writable: true,
});

describe("Auth Store", () => {
  beforeEach(() => {
    setActivePinia(createPinia());
    vi.clearAllMocks();
  });

  afterEach(() => {
    localStorageMock.clear();
  });

  describe("Initial State", () => {
    it("should initialize with null values when localStorage is empty", () => {
      localStorageMock.getItem.mockReturnValue(null);

      const store = useAuthStore();

      expect(store.token).toBeNull();
      expect(store.userId).toBeNull();
      expect(store.email).toBeNull();
      expect(store.isAuthenticated).toBe(false);
    });

    it("should restore values from localStorage", () => {
      localStorageMock.getItem.mockImplementation((key: string) => {
        switch (key) {
          case "auth_token":
            return "test-token";
          case "user_id":
            return "user-123";
          case "user_email":
            return "test@example.com";
          default:
            return null;
        }
      });

      const store = useAuthStore();

      expect(store.token).toBe("test-token");
      expect(store.userId).toBe("user-123");
      expect(store.email).toBe("test@example.com");
      expect(store.isAuthenticated).toBe(true);
    });
  });

  describe("setAuth", () => {
    it("should set authentication data and store in localStorage", () => {
      const store = useAuthStore();

      store.setAuth("new-token", "user-456", "new@example.com");

      expect(store.token).toBe("new-token");
      expect(store.userId).toBe("user-456");
      expect(store.email).toBe("new@example.com");
      expect(store.isAuthenticated).toBe(true);

      expect(localStorageMock.setItem).toHaveBeenCalledWith(
        "auth_token",
        "new-token"
      );
      expect(localStorageMock.setItem).toHaveBeenCalledWith(
        "user_id",
        "user-456"
      );
      expect(localStorageMock.setItem).toHaveBeenCalledWith(
        "user_email",
        "new@example.com"
      );
    });

    it("should update authentication state when called multiple times", () => {
      const store = useAuthStore();

      store.setAuth("token1", "user1", "email1@example.com");
      expect(store.token).toBe("token1");
      expect(store.userId).toBe("user1");
      expect(store.email).toBe("email1@example.com");

      store.setAuth("token2", "user2", "email2@example.com");
      expect(store.token).toBe("token2");
      expect(store.userId).toBe("user2");
      expect(store.email).toBe("email2@example.com");
    });
  });

  describe("logout", () => {
    it("should clear authentication data and remove from localStorage", () => {
      const store = useAuthStore();

      // First set some auth data
      store.setAuth("test-token", "user-123", "test@example.com");
      expect(store.isAuthenticated).toBe(true);

      // Then logout
      store.logout();

      expect(store.token).toBeNull();
      expect(store.userId).toBeNull();
      expect(store.email).toBeNull();
      expect(store.isAuthenticated).toBe(false);

      expect(localStorageMock.removeItem).toHaveBeenCalledWith("auth_token");
      expect(localStorageMock.removeItem).toHaveBeenCalledWith("user_id");
      expect(localStorageMock.removeItem).toHaveBeenCalledWith("user_email");
    });

    it("should handle logout when already logged out", () => {
      const store = useAuthStore();

      // Ensure we're logged out
      store.logout();

      // Already null values
      expect(store.isAuthenticated).toBe(false);

      store.logout();

      expect(store.token).toBeNull();
      expect(store.userId).toBeNull();
      expect(store.email).toBeNull();
      expect(store.isAuthenticated).toBe(false);
    });
  });

  describe("isAuthenticated computed property", () => {
    it("should be true when token exists", () => {
      const store = useAuthStore();

      store.setAuth("some-token", "user-id", "email@example.com");
      expect(store.isAuthenticated).toBe(true);
    });

    it("should be false when token is null", () => {
      const store = useAuthStore();

      store.logout();
      expect(store.isAuthenticated).toBe(false);
    });

    it("should be false when token is empty string", () => {
      const store = useAuthStore();

      // Manually set empty token (not through setAuth)
      store.token = "";
      expect(store.isAuthenticated).toBe(false);
    });
  });

  describe("Persistence", () => {
    it("should persist authentication state across store instances", () => {
      // Set up localStorage to return persisted values
      localStorageMock.getItem.mockImplementation((key: string) => {
        switch (key) {
          case "auth_token":
            return "persistent-token";
          case "user_id":
            return "persistent-user";
          case "user_email":
            return "persistent@example.com";
          default:
            return null;
        }
      });

      // Create new store instance (simulating app restart)
      const store = useAuthStore();

      expect(store.token).toBe("persistent-token");
      expect(store.userId).toBe("persistent-user");
      expect(store.email).toBe("persistent@example.com");
      expect(store.isAuthenticated).toBe(true);
    });
  });

  describe("Edge Cases", () => {
    it("should handle empty strings in localStorage", () => {
      localStorageMock.getItem.mockReturnValue("");

      const store = useAuthStore();

      expect(store.token).toBe("");
      expect(store.isAuthenticated).toBe(false);
    });

    it("should handle malformed data gracefully", () => {
      // This would be handled by the ref initialization
      const store = useAuthStore();

      // Manually set invalid data
      store.token = undefined as any;
      expect(store.isAuthenticated).toBe(false);
    });
  });
});
