<template>
  <div class="flex items-center gap-2">
    <!-- Toggle Button -->
    <button
      @click="themeStore.toggleTheme"
      :title="toggleTitle"
      class="relative inline-flex items-center justify-center p-2 rounded-lg transition-all duration-300 hover:bg-soft-100 dark:hover:bg-soft-800"
    >
      <!-- Sun Icon (Light Mode) -->
      <svg
        v-if="themeStore.effectiveTheme === 'dark'"
        class="w-5 h-5 text-soft-600 dark:text-soft-400 transition-all duration-300"
        fill="currentColor"
        viewBox="0 0 24 24"
      >
        <path
          d="M12 18a6 6 0 1 0 0-12 6 6 0 0 0 0 12zm0-2a4 4 0 1 1 0-8 4 4 0 0 1 0 8zM11 1h2v3h-2V1zm0 18h2v3h-2v-3zM3.515 4.929l1.414-1.414L7.071 6.05 5.657 7.464 3.515 4.93zM16.95 16.95l1.414-1.414 2.121 2.121-1.414 1.414-2.121-2.121zm2.121-6.364l3 .001V12h-3v-1.414zM1 11v2h3v-2H1zm6.05-6.05L7.464 5.657 5.343 3.536l1.414-1.414 2.121 2.121z"
        />
      </svg>

      <!-- Moon Icon (Dark Mode) -->
      <svg
        v-else
        class="w-5 h-5 text-soft-600 dark:text-soft-400 transition-all duration-300"
        fill="currentColor"
        viewBox="0 0 24 24"
      >
        <path
          d="M10 7a7 7 0 0 0 12 5.338A9.002 9.002 0 1 1 10 7zm0 2a5 5 0 0 1 7.953 2.495A7.001 7.001 0 1 0 10 9z"
        />
      </svg>
    </button>

    <!-- Label (optional) -->
    <span v-if="showLabel" class="text-sm font-medium text-soft-700 dark:text-soft-300">
      {{ themeStore.effectiveTheme === 'dark' ? 'Dark' : 'Light' }}
    </span>

    <!-- Menu (optional) -->
    <div
      v-if="showMenu"
      class="flex items-center gap-1 ml-2 pl-2 border-l border-soft-200 dark:border-soft-700"
    >
      <button
        @click="themeStore.setTheme('light')"
        :class="[
          'px-2 py-1 text-xs rounded transition-colors',
          themeStore.theme === 'light'
            ? 'bg-primary-100 text-primary-600 dark:bg-primary-900 dark:text-primary-300'
            : 'text-soft-600 dark:text-soft-400 hover:bg-soft-100 dark:hover:bg-soft-800',
        ]"
        title="Light Mode"
      >
        Light
      </button>
      <button
        @click="themeStore.setTheme('dark')"
        :class="[
          'px-2 py-1 text-xs rounded transition-colors',
          themeStore.theme === 'dark'
            ? 'bg-primary-100 text-primary-600 dark:bg-primary-900 dark:text-primary-300'
            : 'text-soft-600 dark:text-soft-400 hover:bg-soft-100 dark:hover:bg-soft-800',
        ]"
        title="Dark Mode"
      >
        Dark
      </button>
      <button
        @click="themeStore.setTheme('auto')"
        :class="[
          'px-2 py-1 text-xs rounded transition-colors',
          themeStore.theme === 'auto'
            ? 'bg-primary-100 text-primary-600 dark:bg-primary-900 dark:text-primary-300'
            : 'text-soft-600 dark:text-soft-400 hover:bg-soft-100 dark:hover:bg-soft-800',
        ]"
        title="Auto (Follow System)"
      >
        Auto
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useThemeStore } from '@/stores/theme';
import { computed } from 'vue';

interface Props {
  showLabel?: boolean;
  showMenu?: boolean;
}

withDefaults(defineProps<Props>(), {
  showLabel: false,
  showMenu: false,
});

const themeStore = useThemeStore();

const toggleTitle = computed(() => {
  return themeStore.effectiveTheme === 'dark' ? 'Switch to Light Mode' : 'Switch to Dark Mode';
});
</script>

<style scoped>
button {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}
</style>
