<template>
  <div class="language-switcher" data-testid="language-switcher">
    <!-- Dropdown trigger button -->
    <button
      class="language-button"
      :title="`Switch language: ${currentLocale?.name ?? 'Unknown'}`"
      @click.stop="isOpen = !isOpen"
      :disabled="isLoading"
      data-testid="language-switcher-button"
      :aria-disabled="isLoading"
    >
      <span class="language-flag">{{ currentLocale?.flag ?? '🌐' }}</span>
      <span class="language-code">{{ currentLocale?.code.toUpperCase() ?? '?' }}</span>
      <svg
        class="chevron-icon"
        :class="{ rotate: isOpen }"
        width="16"
        height="16"
        viewBox="0 0 16 16"
        fill="currentColor"
      >
        <path d="M4.5 6L8 10l3.5-4" stroke="currentColor" stroke-width="2" fill="none" />
      </svg>
    </button>

    <!-- Dropdown menu -->
    <transition name="fade">
      <div v-if="isOpen" class="language-dropdown" data-testid="language-dropdown" @click.stop>
        <button
          v-for="loc in locales"
          :key="loc.code"
          class="language-option"
          :class="{ active: locale === loc.code }"
          @click.stop="handleSelectLocale(loc.code)"
          :disabled="isLoading"
          :data-testid="`language-option-${loc.code}`"
        >
          <span class="option-flag">{{ loc.flag }}</span>
          <span class="option-name">{{ loc.name }}</span>
          <svg
            v-if="locale === loc.code"
            class="checkmark"
            width="16"
            height="16"
            viewBox="0 0 16 16"
            fill="currentColor"
          >
            <path
              d="M13.78 4.22a.75.75 0 010 1.06l-7.25 7.25a.75.75 0 01-1.06 0L2.22 9.28a.75.75 0 011.06-1.06L6 11.94l6.72-6.72a.75.75 0 011.06 0z"
            />
          </svg>
        </button>
      </div>
    </transition>

    <!-- Overlay to close dropdown -->
    <div v-if="isOpen" class="language-overlay" @click="isOpen = false" />
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { useLocale } from '@/composables/useLocale';

const { locale, currentLocale, locales, isLoading, setLocale } = useLocale();
const isOpen = ref(false);

const handleSelectLocale = async (code: string) => {
  isOpen.value = false;
  await setLocale(code);
};
</script>

<style scoped>
.language-switcher {
  position: relative;
  display: inline-block;
}

.language-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 1rem;
  background-color: var(--color-background-secondary, #f5f5f5);
  border: 1px solid var(--color-border, #e0e0e0);
  border-radius: 6px;
  cursor: pointer;
  font-size: 0.875rem;
  font-weight: 500;
  transition: all 0.2s ease;
  white-space: nowrap;
  position: relative;
  z-index: 1001;
}

.language-button:hover:not(:disabled) {
  background-color: var(--color-background-tertiary, #efefef);
  border-color: var(--color-primary, #0066cc);
}

.language-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.language-flag {
  font-size: 1.2rem;
  line-height: 1;
}

.language-code {
  font-weight: 600;
  letter-spacing: 0.05em;
}

.chevron-icon {
  transition: transform 0.2s ease;
  flex-shrink: 0;
}

.chevron-icon.rotate {
  transform: scaleY(-1);
}

.language-dropdown {
  position: absolute;
  top: 100%;
  right: 0;
  margin-top: 0.5rem;
  background-color: white;
  border: 1px solid var(--color-border, #e0e0e0);
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  min-width: 220px;
  z-index: 1000;
  overflow: hidden;
  animation: slideDown 0.2s ease;
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-8px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.language-option {
  display: flex;
  align-items: center;
  width: 100%;
  padding: 0.75rem 1rem;
  background: none;
  border: none;
  cursor: pointer;
  font-size: 0.875rem;
  text-align: left;
  transition: background-color 0.15s ease;
  gap: 0.75rem;
}

.language-option:hover:not(:disabled) {
  background-color: var(--color-background-secondary, #f5f5f5);
}

.language-option:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.language-option.active {
  background-color: var(--color-primary-light, #e8f0ff);
  color: var(--color-primary, #0066cc);
  font-weight: 600;
}

.option-flag {
  font-size: 1.1rem;
  line-height: 1;
}

.option-name {
  flex: 1;
}

.checkmark {
  flex-shrink: 0;
  color: var(--color-primary, #0066cc);
}

.language-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 999;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
