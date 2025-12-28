import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import path from "path";
import { fileURLToPath, URL } from "node:url";

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  server: {
    port: parseInt(process.env.VITE_PORT || "5173"),
    host: "0.0.0.0",
    strictPort: false,
    proxy: {
      "/api": {
        target: process.env.VITE_API_GATEWAY_URL || "http://localhost:8000",
        changeOrigin: true,
        secure: false,
        ws: true,
        timeout: 30000,
      },
      "/ws": {
        target: process.env.VITE_WS_URL || "ws://localhost:8000",
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
