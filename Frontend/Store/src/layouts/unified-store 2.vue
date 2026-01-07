<template>
  <div
    class="unified-store-layout min-h-screen flex flex-col bg-gray-50 dark:bg-gray-900 text-gray-900 dark:text-gray-100"
  >
    <!-- Skip to main content link for accessibility -->
    <a
      href="#main-content"
      class="sr-only focus:not-sr-only focus:absolute focus:top-4 focus:left-4 bg-blue-600 text-white px-4 py-2 rounded-md z-50"
      :aria-label="$t('accessibility.skipToMain')"
    >
      {{ $t('accessibility.skipToMain') }}
    </a>

    <!-- Header -->
    <header
      role="banner"
      class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700"
      :aria-label="$t('layout.header')"
    >
      <div class="max-w-7xl mx-auto px-4 py-4">
        <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100">
          {{ $route.meta?.title || 'B2Connect Store' }}
        </h1>
      </div>
    </header>

    <!-- Main Layout Container -->
    <div class="flex flex-1">
      <!-- Sidebar Slot (optional) -->
      <aside
        v-if="$slots.sidebar"
        role="complementary"
        class="hidden lg:block w-64 bg-white dark:bg-gray-800 border-r border-gray-200 dark:border-gray-700"
        :aria-label="$t('layout.sidebar')"
      >
        <slot name="sidebar" />
      </aside>

      <!-- Mobile Sidebar Overlay -->
      <div
        v-if="showMobileSidebar"
        class="fixed inset-0 z-40 lg:hidden"
        @click="closeMobileSidebar"
      >
        <div class="absolute inset-0 bg-black bg-opacity-50" />
        <aside
          role="complementary"
          class="relative w-64 h-full bg-white dark:bg-gray-800 shadow-xl"
          :aria-label="$t('layout.sidebar')"
        >
          <slot name="sidebar" />
        </aside>
      </div>

      <!-- Main Content -->
      <main
        id="main-content"
        role="main"
        class="flex-1 p-4 lg:p-6 overflow-auto"
        :aria-label="$t('layout.mainContent')"
      >
        <slot />
      </main>
    </div>

    <!-- Footer -->
    <footer
      role="contentinfo"
      class="bg-white dark:bg-gray-800 border-t border-gray-200 dark:border-gray-700 mt-auto"
      :aria-label="$t('layout.footer')"
    >
      <div class="max-w-7xl mx-auto px-4 py-6">
        <p class="text-center text-gray-600 dark:text-gray-400">
          © 2024 B2Connect. All rights reserved.
        </p>
      </div>
    </footer>

    <!-- Mobile menu button (if sidebar exists) -->
    <button
      v-if="$slots.sidebar"
      class="fixed bottom-4 right-4 lg:hidden bg-blue-600 hover:bg-blue-700 text-white p-3 rounded-full shadow-lg z-30"
      @click="toggleMobileSidebar"
      :aria-label="$t('layout.toggleSidebar')"
      aria-expanded="false"
    >
      <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M4 6h16M4 12h16M4 18h16"
        />
      </svg>
    </button>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import { useHead } from '@nuxtjs/seo';

// Reactive state for mobile sidebar
const showMobileSidebar = ref(false);

// SEO and meta configuration
useHead({
  htmlAttrs: {
    lang: 'en', // Will be overridden by i18n locale
  },
  meta: [
    { charset: 'utf-8' },
    { name: 'viewport', content: 'width=device-width, initial-scale=1' },
    { name: 'theme-color', content: '#2563eb' },
    // Accessibility meta
    { name: 'color-scheme', content: 'light dark' },
    // Open Graph
    { property: 'og:type', content: 'website' },
    { property: 'og:site_name', content: 'B2Connect Store' },
    // Twitter Card
    { name: 'twitter:card', content: 'summary_large_image' },
    // WCAG compliance
    { name: 'format-detection', content: 'telephone=no' },
  ],
  link: [
    { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
    // Preconnect for performance
    { rel: 'preconnect', href: 'https://fonts.googleapis.com' },
    { rel: 'preconnect', href: 'https://fonts.gstatic.com', crossorigin: '' },
  ],
});

// Mobile sidebar functions
const toggleMobileSidebar = () => {
  showMobileSidebar.value = !showMobileSidebar.value;
  document.body.style.overflow = showMobileSidebar.value ? 'hidden' : '';
};

const closeMobileSidebar = () => {
  showMobileSidebar.value = false;
  document.body.style.overflow = '';
};

// Keyboard navigation for accessibility
const handleKeydown = (event: KeyboardEvent) => {
  if (event.key === 'Escape' && showMobileSidebar.value) {
    closeMobileSidebar();
  }
};

onMounted(() => {
  document.addEventListener('keydown', handleKeydown);
});

onUnmounted(() => {
  document.removeEventListener('keydown', handleKeydown);
  document.body.style.overflow = '';
});
</script>

<style scoped>
/* Ensure high contrast in dark mode */
.dark {
  color-scheme: dark;
}

/* Focus styles for accessibility */
.unified-store-layout :focus-visible {
  outline: 2px solid #2563eb;
  outline-offset: 2px;
}

/* Screen reader only class */
.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border: 0;
}

.sr-only.focus:not-sr-only {
  position: static;
  width: auto;
  height: auto;
  padding: inherit;
  margin: inherit;
  overflow: visible;
  clip: auto;
  white-space: normal;
}

/* Responsive design utilities */
@media (max-width: 1023px) {
  .unified-store-layout main {
    padding: 1rem;
  }
}

@media (max-width: 767px) {
  .unified-store-layout main {
    padding: 0.75rem;
  }
}

/* Print styles */
@media print {
  .unified-store-layout aside,
  .unified-store-layout button[aria-label*='toggle'] {
    display: none !important;
  }

  .unified-store-layout main {
    padding: 0 !important;
  }
}
</style>
