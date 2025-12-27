import { vi, beforeAll, afterAll } from "vitest";
import { config } from "@vue/test-utils";

// Mock localStorage
const localStorageMock = (() => {
  let store: Record<string, string> = {};

  return {
    getItem: (key: string) => store[key] || null,
    setItem: (key: string, value: string) => {
      store[key] = value.toString();
    },
    removeItem: (key: string) => {
      delete store[key];
    },
    clear: () => {
      store = {};
    },
  };
})();

Object.defineProperty(window, "localStorage", {
  value: localStorageMock,
});

// Mock sessionStorage
Object.defineProperty(window, "sessionStorage", {
  value: localStorageMock,
});

// Mock window.matchMedia
Object.defineProperty(window, "matchMedia", {
  writable: true,
  value: vi.fn().mockImplementation((query) => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: vi.fn(),
    removeListener: vi.fn(),
    addEventListener: vi.fn(),
    removeEventListener: vi.fn(),
    dispatchEvent: vi.fn(),
  })),
});

// Configure Vue Test Utils
config.global.stubs = {
  teleport: true,
  Transition: false,
};

// Mock window.scrollTo
Object.defineProperty(window, "scrollTo", {
  value: vi.fn(),
});

// Suppress console warnings in tests
const originalError = console.error;
const originalWarn = console.warn;

beforeAll(() => {
  console.error = vi.fn((...args) => {
    if (
      typeof args[0] === "string" &&
      (args[0].includes("Not implemented: HTMLFormElement.prototype.submit") ||
        args[0].includes("Not implemented: HTMLFormElement.prototype.reset") ||
        args[0].includes("Navigation"))
    ) {
      return;
    }
    originalError.call(console, ...args);
  });

  console.warn = vi.fn((...args) => {
    if (
      typeof args[0] === "string" &&
      args[0].includes(
        "Avoid app logic that relies on enumerating keys on a component instance"
      )
    ) {
      return;
    }
    originalWarn.call(console, ...args);
  });
});

afterAll(() => {
  console.error = originalError;
  console.warn = originalWarn;
});
