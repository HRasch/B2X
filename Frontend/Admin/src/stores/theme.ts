/**
 * Theme Store (Pinia)
 * @todo Refactor globalThis type checks to proper typing
 */
/* eslint-disable @typescript-eslint/no-explicit-any -- globalThis browser detection */

import { defineStore } from 'pinia';
import { ref, watch } from 'vue';

export type Theme = 'light' | 'dark' | 'auto';

// Helper to check if we're in browser
const isBrowserEnv = (): boolean => typeof window !== 'undefined';

// Safe localStorage access
const getStoredTheme = (): Theme => {
  if (!isBrowserEnv()) return 'auto';
  try {
    return (window.localStorage?.getItem('theme') as Theme) || 'auto';
  } catch {
    return 'auto';
  }
};

// Safe localStorage set
const setStoredTheme = (theme: Theme) => {
  if (!isBrowserEnv()) return;
  try {
    window.localStorage?.setItem('theme', theme);
  } catch {
    // Ignore localStorage errors
  }
};

export const useThemeStore = defineStore('theme', () => {
  const theme = ref<Theme>(getStoredTheme());

  // Determine effective theme (what's actually being used)
  const effectiveTheme = ref<'light' | 'dark'>('light');

  // Initialize theme
  const initializeTheme = () => {
    if (!isBrowserEnv()) return;

    updateEffectiveTheme();
    applyTheme();

    // Listen to system theme changes
    try {
      const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
      mediaQuery.addEventListener('change', () => {
        if (theme.value === 'auto') {
          updateEffectiveTheme();
          applyTheme();
        }
      });
    } catch {
      // Ignore media query errors
    }
  };

  // Determine what the effective theme should be
  const updateEffectiveTheme = () => {
    if (!isBrowserEnv()) return;

    try {
      if (theme.value === 'auto') {
        effectiveTheme.value = window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
      } else {
        effectiveTheme.value = theme.value;
      }
    } catch {
      effectiveTheme.value = theme.value;
    }
  };

  // Apply theme to DOM
  const applyTheme = () => {
    if (!isBrowserEnv()) return;

    try {
      const isDark = effectiveTheme.value === 'dark';
      const htmlElement = document?.documentElement;
      if (!htmlElement) return;

      if (isDark) {
        htmlElement.classList.add('dark');
      } else {
        htmlElement.classList.remove('dark');
      }
    } catch {
      // Ignore DOM errors
    }
  };

  // Toggle theme
  const toggleTheme = () => {
    if (effectiveTheme.value === 'dark') {
      setTheme('light');
    } else {
      setTheme('dark');
    }
  };

  // Set theme
  const setTheme = (newTheme: Theme) => {
    theme.value = newTheme;
    setStoredTheme(newTheme);
    updateEffectiveTheme();
    applyTheme();
  };

  // Watch theme changes
  watch(
    () => theme.value,
    () => {
      updateEffectiveTheme();
      applyTheme();
    }
  );

  return {
    theme,
    effectiveTheme,
    initializeTheme,
    toggleTheme,
    setTheme,
  };
});
