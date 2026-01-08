<template>
  <div class="unified-store-layout min-h-screen flex flex-col bg-base-200 text-base-content">
    <!-- Skip to main content link for accessibility -->
    <a
      href="#main-content"
      class="sr-only focus:not-sr-only focus:absolute focus:top-4 focus:left-4 bg-primary text-primary-content px-4 py-2 rounded-md z-50"
      :aria-label="$t('accessibility.skipToMain')"
    >
      {{ $t('accessibility.skipToMain') }}
    </a>

    <!-- Header -->
    <header
      role="banner"
      class="bg-base-100 border-b border-base-300"
      :aria-label="$t('layout.header')"
    >
      <div class="max-w-7xl mx-auto p-lg">
        <h1 class="text-heading-3 text-base-content">
          {{ $route.meta?.title || 'B2X Store' }}
        </h1>
      </div>
    </header>

    <!-- Main Layout Container -->
    <div class="flex flex-1">
      <!-- Sidebar Slot (optional) -->
      <aside
        v-if="$slots.sidebar"
        role="complementary"
        class="hidden lg:block w-64 bg-base-100 border-r border-base-300"
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
          class="relative w-64 h-full bg-base-100 shadow-xl"
          :aria-label="$t('layout.sidebar')"
        >
          <slot name="sidebar" />
        </aside>
      </div>

      <!-- Main Content -->
      <main
        id="main-content"
        role="main"
        class="flex-1 p-lg overflow-auto"
        :aria-label="$t('layout.mainContent')"
      >
        <slot />
      </main>
    </div>

    <!-- Footer -->
    <footer
      role="contentinfo"
      class="bg-base-100 border-t border-base-300 mt-auto"
      :aria-label="$t('layout.footer')"
    >
      <div class="max-w-7xl mx-auto p-lg">
        <p class="text-center text-body-sm text-base-content/70">
          {{ $t('footer.copyright') }}
        </p>
      </div>
    </footer>

    <!-- Mobile menu button (if sidebar exists) -->
    <button
      v-if="$slots.sidebar"
      class="fixed bottom-4 right-4 lg:hidden btn-primary-standard p-md rounded-full shadow-lg z-30"
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
import { useHead } from '#app';
import { useI18n } from 'vue-i18n';

// i18n setup
const { t, locale } = useI18n();

// Reactive state for mobile sidebar
const showMobileSidebar = ref(false);

// SEO and meta configuration
useHead({
  title: t('common.brand'),
  htmlAttrs: {
    lang: locale,
  },
  meta: [
    { charset: 'utf-8' },
    { name: 'viewport', content: 'width=device-width, initial-scale=1' },
    { name: 'theme-color', content: '#2563eb' },
    // Accessibility meta
    { name: 'color-scheme', content: 'light dark' },
    // Open Graph
    { property: 'og:type', content: 'website' },
    { property: 'og:site_name', content: 'B2X Store' },
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
