<template>
  <div class="theme-toggle">
    <!-- Simple Icon Button -->
    <button
      v-if="!showMenu && !showLabel"
      type="button"
      @click="toggleTheme"
      :aria-label="
        currentTheme === 'dark' ? $t('layout.theme.switchToLight') : $t('layout.theme.switchToDark')
      "
      class="btn btn-ghost btn-circle"
    >
      <!-- Sun Icon (Light Mode) -->
      <svg
        v-if="currentTheme === 'light'"
        class="w-5 h-5"
        fill="none"
        stroke="currentColor"
        viewBox="0 0 24 24"
      >
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z"
        />
      </svg>
      <!-- Moon Icon (Dark Mode) -->
      <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z"
        />
      </svg>
    </button>

    <!-- Button with Label -->
    <button
      v-else-if="showLabel && !showMenu"
      type="button"
      @click="toggleTheme"
      class="btn btn-ghost gap-2"
    >
      <!-- Sun Icon (Light Mode) -->
      <svg
        v-if="currentTheme === 'light'"
        class="w-5 h-5"
        fill="none"
        stroke="currentColor"
        viewBox="0 0 24 24"
      >
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z"
        />
      </svg>
      <!-- Moon Icon (Dark Mode) -->
      <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z"
        />
      </svg>
      <span>{{
        currentTheme === 'dark' ? $t('layout.theme.lightMode') : $t('layout.theme.darkMode')
      }}</span>
    </button>

    <!-- Dropdown Menu -->
    <div v-else-if="showMenu" class="dropdown dropdown-end">
      <div tabindex="0" role="button" class="btn btn-ghost btn-circle">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z"
          />
        </svg>
      </div>
      <ul tabindex="0" class="dropdown-content z-[1] menu p-2 shadow bg-base-100 rounded-box w-52">
        <li>
          <a @click="setTheme('light')" :class="{ active: currentTheme === 'light' }">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z"
              />
            </svg>
            {{ $t('layout.theme.lightMode') }}
          </a>
        </li>
        <li>
          <a @click="setTheme('dark')" :class="{ active: currentTheme === 'dark' }">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z"
              />
            </svg>
            {{ $t('layout.theme.darkMode') }}
          </a>
        </li>
        <li>
          <a @click="setTheme('auto')" :class="{ active: currentTheme === 'auto' }">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M9.75 17L9 20l-1 1h8l-1-1-.75-3M3 13h18M5 17h14a2 2 0 002-2V5a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
              />
            </svg>
            {{ $t('layout.theme.autoSystem') }}
          </a>
        </li>
      </ul>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useThemeStore } from '@/stores/theme';

interface Props {
  showLabel?: boolean;
  showMenu?: boolean;
}

withDefaults(defineProps<Props>(), {
  showLabel: false,
  showMenu: false,
});

const themeStore = useThemeStore();

const currentTheme = computed(() => themeStore.currentTheme);

const toggleTheme = () => {
  themeStore.toggleTheme();
};

const setTheme = (theme: 'light' | 'dark' | 'auto') => {
  themeStore.setTheme(theme);
};
</script>

<style scoped>
.theme-toggle {
  display: inline-block;
}

.dropdown-content .active {
  @apply bg-primary text-primary-content;
}
</style>
