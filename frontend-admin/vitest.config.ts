import { defineConfig } from "vitest/config";
import vue from "@vitejs/plugin-vue";
import { fileURLToPath } from "node:url";

export default defineConfig({
  plugins: [vue()],
  test: {
    globals: false, // CHANGED: Disable globals to prevent vitest expect conflict with playwright
    environment: "happy-dom",
    setupFiles: ["./tests/setup.ts"],
    coverage: {
      provider: "v8",
      reporter: ["text", "json", "html", "lcov"],
      include: ["src/**/*.{ts,vue}"],
      exclude: ["src/main.ts", "src/**/*.d.ts", "src/**/*.stories.ts"],
      statements: 70,
      branches: 70,
      functions: 70,
      lines: 70,
    },
  },
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
});
