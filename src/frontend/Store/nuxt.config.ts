// https://nuxt.com/docs/api/configuration/nuxt-config
import { defineNuxtConfig } from 'nuxt/config';

export default defineNuxtConfig({
  devtools: {
    enabled: true,

    timeline: {
      enabled: true,
    },
  },
  compatibilityDate: '2026-01-08',
  devServer: {
    host: 'localhost',
    port: 3000,
  },
  srcDir: 'src',
  typescript: {
    strict: true,
    typeCheck: false,
  },
  modules: ['@nuxtjs/i18n', '@pinia/nuxt'],
  plugins: [
    '~/plugins/debug-init.js',
    '~/plugins/debug-guard.js',
    '~/plugins/i18n.server.ts',
    '~/plugins/i18n.client.ts',
  ],
  css: ['~/assets/css/main.css'],
  postcss: {
    plugins: {
      '@tailwindcss/postcss': {},
      autoprefixer: {},
    },
  },
  i18n: {
    locales: [
      { code: 'en', name: 'English', flag: '🇬🇧' },
      { code: 'de', name: 'Deutsch', flag: '🇩🇪' },
      { code: 'fr', name: 'Français', flag: '🇫🇷' },
      { code: 'es', name: 'Español', flag: '🇪🇸' },
      { code: 'it', name: 'Italiano', flag: '🇮🇹' },
      { code: 'pt', name: 'Português', flag: '🇵🇹' },
      { code: 'nl', name: 'Nederlands', flag: '🇳🇱' },
      { code: 'pl', name: 'Polski', flag: '🇵🇱' },
    ],
    defaultLocale: 'en',
    strategy: 'no_prefix',
    detectBrowserLanguage: false,
    vueI18n: './i18n.config.ts',
  },
});
