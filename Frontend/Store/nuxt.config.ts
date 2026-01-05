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
    // Will be configured dynamically based on tenant
    locales: [
      { code: 'en', name: 'English', flag: '🇺🇸', file: 'en.json' },
      { code: 'de', name: 'Deutsch', flag: '🇩🇪', file: 'de.json' },
      { code: 'fr', name: 'Français', flag: '🇫🇷', file: 'fr.json' },
      { code: 'es', name: 'Español', flag: '🇪🇸', file: 'es.json' },
      { code: 'it', name: 'Italiano', flag: '🇮🇹', file: 'it.json' },
      { code: 'pt', name: 'Português', flag: '🇵🇹', file: 'pt.json' },
      { code: 'nl', name: 'Nederlands', flag: '🇳🇱', file: 'nl.json' },
      { code: 'pl', name: 'Polski', flag: '🇵🇱', file: 'pl.json' },
    ],
    defaultLocale: 'en',
    // Custom strategy for tenant-scoped routing
    strategy: 'no_prefix',
    // Disable default loading - we'll handle it manually
    lazy: false,
  },

  // Pinia store configuration
  // pinia: {
  //   storesDirs: ['./stores/**'],
  // },

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
