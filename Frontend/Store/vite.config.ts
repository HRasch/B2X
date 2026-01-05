import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import path from 'path';
import { visualizer } from 'rollup-plugin-visualizer';

export default defineConfig({
  plugins: [
    vue(),
    visualizer({
      filename: 'dist/bundle-analysis.html',
      open: false,
      gzipSize: true,
      brotliSize: true,
    }),
  ],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, 'src'),
      '~': path.resolve(__dirname, 'src'),
    },
  },
  css: {
    preprocessorOptions: {
      scss: {
        // Make abstracts (functions, mixins) available in all SCSS files
        additionalData: `
          @use "@/scss/abstracts/functions" as *;
          @use "@/scss/abstracts/mixins" as *;
        `,
      },
    },
    devSourcemap: true,
  },
  server: {
    proxy: {
      '/api': {
        target: process.env.VITE_API_GATEWAY_URL || 'http://localhost:8000',
        changeOrigin: true,
        secure: false,
        ws: true,
        timeout: 30000,
      },
      '/ws': {
        target: process.env.VITE_WS_URL || 'ws://localhost:8000',
        changeOrigin: true,
        ws: true,
        secure: false,
        timeout: 30000,
      },
    },
  },
  build: {
    target: 'esnext',
    minify: 'terser',
    rollupOptions: {
      output: {
        manualChunks: {
          vue: ['vue', 'vue-router', 'pinia'],
          vendor: ['axios', 'date-fns'],
        },
      },
    },
    reportCompressedSize: true,
    chunkSizeWarningLimit: 1000,
  },
});
