// https://nuxt.com/docs/api/configuration/nuxt-config
import path from 'path';
import tailwindcss from 'tailwindcss';
import autoprefixer from 'autoprefixer';
import { visualizer } from 'rollup-plugin-visualizer';

export default defineNuxtConfig({
  devtools: { enabled: true },

  // Dev server configuration
  devServer: {
    host: 'localhost',
    port: 3000,
  },

  // SSR enabled by default in Nuxt 3
  ssr: true,

  // TypeScript
  typescript: {
    strict: true,
    typeCheck: true,
  },

  // Modules
  modules: ['@nuxtjs/i18n', '@pinia/nuxt'],

  // CSS
  css: ['./assets/css/main.css'],

  // Vite configuration for Tailwind CSS v4 and SSR
  vite: {
    plugins: [
      // Bundle analyzer for development
      process.env.NODE_ENV === 'development'
        ? visualizer({
            filename: 'dist/bundle-analysis.html',
            open: false,
            gzipSize: true,
            brotliSize: true,
          })
        : null,
    ].filter(Boolean),
    resolve: {
      alias: {
        '@': path.resolve(__dirname),
        '~': path.resolve(__dirname),
      },
    },
    css: {
      postcss: {
        plugins: [tailwindcss, autoprefixer],
      },
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
    // Optimize for SSR
    ssr: {
      noExternal: ['@nuxtjs/i18n'],
    },
  },

  // i18n configuration for tenant-customizable translations
  i18n: {
    // Disable built-in locale detection for tenant-based routing
    detectBrowserLanguage: false,
    defaultLocale: 'en',
    // Custom strategy for tenant-scoped routing
    strategy: 'no_prefix',
    bundle: {
      optimizeTranslationDirective: false,
    },
  },

  // Pinia store configuration
  pinia: {
    // storesDirs is not needed in Nuxt 3 - auto-discovery works
  },

  // Runtime config for tenant-specific settings
  runtimeConfig: {
    // Private keys (only available on server-side)
    apiSecret: process.env.NUXT_API_SECRET,
    // Public keys (exposed to client-side)
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || '/api',
      tenantId: process.env.NUXT_PUBLIC_TENANT_ID,
      baseUrl: process.env.NUXT_PUBLIC_BASE_URL || 'http://localhost:3000',
    },
  },

  // Build configuration
  nitro: {
    // Enable experimental features for better SSR
    experimental: {
      wasm: true,
    },
  },

  // SEO and meta configuration
  app: {
    head: {
      htmlAttrs: {
        lang: 'en',
      },
      meta: [
        { charset: 'utf-8' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' },
        { name: 'format-detection', content: 'telephone=no' },
      ],
      link: [{ rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' }],
    },
  },
});
