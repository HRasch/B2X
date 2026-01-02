import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import { fileURLToPath } from "node:url";

// Suppress noisy proxy errors in demo/test mode
const isTestMode = process.env.NODE_ENV === "test" || process.env.CI === "true";

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  // Custom logger to suppress proxy ECONNREFUSED errors in test mode
  customLogger: isTestMode
    ? {
        info: (msg) => {
          if (!msg.includes("ECONNREFUSED") && !msg.includes("proxy error")) {
            console.log(msg);
          }
        },
        warn: (msg) => {
          if (!msg.includes("ECONNREFUSED") && !msg.includes("proxy error")) {
            console.warn(msg);
          }
        },
        error: (msg) => {
          if (!msg.includes("ECONNREFUSED") && !msg.includes("proxy error")) {
            console.error(msg);
          }
        },
        warnOnce: (msg) => {
          if (!msg.includes("ECONNREFUSED") && !msg.includes("proxy error")) {
            console.warn(msg);
          }
        },
        clearScreen: () => {},
        hasErrorLogged: () => false,
        hasWarned: false,
      }
    : undefined,
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
        configure: (proxy) => {
          // Suppress ECONNREFUSED errors when backend is not running (demo mode)
          proxy.on("error", (err, _req, res) => {
            if (err.message?.includes("ECONNREFUSED")) {
              // Silently ignore - demo mode handles these requests
              if (res && "writeHead" in res) {
                res.writeHead(503, { "Content-Type": "application/json" });
                res.end(JSON.stringify({ error: "Backend unavailable" }));
              }
            }
          });
        },
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
