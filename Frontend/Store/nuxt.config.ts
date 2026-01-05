// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  devtools: { enabled: true },

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
  css: ['~/assets/css/main.css'],

  // Vite configuration for Tailwind CSS v4 and SSR
  vite: {
    css: {
      postcss: {
        plugins: [require('@tailwindcss/postcss')],
      },
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

  // Development server
  devServer: {
    port: 3000,
    host: '0.0.0.0',
  },
});
