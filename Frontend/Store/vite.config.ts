import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import path from "path";
import { fileURLToPath, URL } from "node:url";

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "src"),
      "~": path.resolve(__dirname, "src"),
    },
  },
  server: {
    proxy: {
      "/api": {
        target: process.env.VITE_API_GATEWAY_URL || "http://localhost:8100",
        changeOrigin: true,
        secure: false,
        ws: true,
        timeout: 30000,
      },
      "/ws": {
        target: process.env.VITE_WS_URL || "ws://localhost:8100",
        changeOrigin: true,
        ws: true,
        secure: false,
        timeout: 30000,
      },
    },
  },
  build: {
    target: "esnext",
    minify: "terser",
    rollupOptions: {
      output: {
        manualChunks: {
          vue: ["vue", "vue-router", "pinia"],
          vendor: ["axios", "date-fns"],
        },
      },
    },
  },
});
