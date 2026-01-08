<template>
  <div class="dropdown dropdown-end" data-testid="language-switcher">
    <!-- Dropdown trigger button -->
    <button
      tabindex="0"
      class="btn btn-ghost btn-sm gap-xs"
      :title="`Switch language: ${currentLocale?.name ?? 'Unknown'}`"
      data-testid="language-switcher-button"
    >
      <span class="text-lg">{{ currentLocale?.flag ?? '🌐' }}</span>
      <span class="text-body-sm font-semibold">{{ currentLocale?.code.toUpperCase() ?? '?' }}</span>
      <svg
        class="w-4 h-4 transition-transform"
        fill="none"
        stroke="currentColor"
        viewBox="0 0 24 24"
      >
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
      </svg>
    </button>

    <!-- Dropdown menu -->
    <ul
      tabindex="0"
      class="dropdown-content z-[1] menu p-xs shadow bg-base-100 rounded-box w-52"
      data-testid="language-dropdown"
    >
      <li
        v-for="loc in supportedLocales"
        :key="loc.code"
        :data-testid="`language-option-${loc.code}`"
      >
        <a @click="handleSelectLocale(loc.code)" :class="{ active: locale === loc.code }">
          <span class="text-lg">{{ loc.flag }}</span>
          <span class="text-body-sm">{{ loc.name }}</span>
          <svg
            v-if="locale === loc.code"
            class="w-4 h-4 text-primary"
            fill="currentColor"
            viewBox="0 0 16 16"
          >
            <path
              d="M13.78 4.22a.75.75 0 010 1.06l-7.25 7.25a.75.75 0 01-1.06 0L2.22 9.28a.75.75 0 011.06-1.06L6 11.94l6.72-6.72a.75.75 0 011.06 0z"
            />
          </svg>
        </a>
      </li>
    </ul>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { useTenantStore } from '~/stores/tenant';
import { ALL_SUPPORTED_LOCALES } from '~/locales';

const { locale } = useI18n();
const tenantStore = useTenantStore();

// Get supported locales based on tenant configuration
const supportedLocales = computed(() => {
  const tenantLanguages = tenantStore.supportedLanguages;
  return ALL_SUPPORTED_LOCALES.filter(loc => tenantLanguages.includes(loc.code));
});

const currentLocale = computed(() => supportedLocales.value.find(loc => loc.code === locale.value));

const handleSelectLocale = async (code: string) => {
  try {
    // Update i18n locale
    locale.value = code as 'en' | 'de' | 'fr' | 'es' | 'it' | 'pt' | 'nl' | 'pl';

    // Store in localStorage
    localStorage.setItem('locale', code);

    // Update document language
    document.documentElement.lang = code;
  } catch (error) {
    console.error('Failed to switch language:', error);
  }
};
</script>

<style scoped>
/* Custom styles for active menu item */
.menu li > a.active {
  @apply bg-primary text-primary-content;
}
</style>
