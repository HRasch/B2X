import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import { fileURLToPath } from "node:url";

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  server: {
    port: parseInt(process.env.PORT || process.env.VITE_PORT || "5174"),
    host: "0.0.0.0",
    strictPort: true, // Fail if port is not available
    proxy: {
      "/api": {
        target: process.env.VITE_API_GATEWAY_URL || "http://localhost:8080",
        changeOrigin: true,
        ws: true,
        secure: false,
        timeout: 30000,
      },
    },
  },
  build: {
    sourcemap: "hidden",
    rollupOptions: {
      output: {
        manualChunks: {
          vue: ["vue", "vue-router", "pinia"],
        },
      },
    },
  },
});
