import { ref, onMounted, onUnmounted } from 'vue';

export type Theme = 'light' | 'dark';

export function useTheme() {
  const theme = ref<Theme>('light');
  let mediaQuery: MediaQueryList | null = null;

  const detectSystemTheme = (): Theme => {
    if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
      return 'dark';
    }
    return 'light';
  };

  const handleThemeChange = (e: MediaQueryListEvent) => {
    theme.value = e.matches ? 'dark' : 'light';
  };

  const toggleTheme = () => {
    theme.value = theme.value === 'light' ? 'dark' : 'light';
  };

  const setTheme = (newTheme: Theme) => {
    theme.value = newTheme;
  };

  onMounted(() => {
    // Initialize theme based on system preference
    theme.value = detectSystemTheme();

    // Listen for system theme changes
    if (window.matchMedia) {
      mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
      mediaQuery.addEventListener('change', handleThemeChange);
    }
  });

  onUnmounted(() => {
    if (mediaQuery) {
      mediaQuery.removeEventListener('change', handleThemeChange);
    }
  });

  return {
    theme,
    toggleTheme,
    setTheme,
    isDark: () => theme.value === 'dark',
  };
}
