<template>
  <Teleport to="body">
    <!-- Backdrop -->
    <div
      v-if="isOpen"
      class="modal modal-open"
      @click.self="handleBackdropClick"
      @keydown.escape="close"
    >
      <div
        class="modal-box"
        :class="modalClasses"
        role="dialog"
        :aria-labelledby="titleId"
        :aria-describedby="contentId"
      >
        <!-- Close button -->
        <button
          v-if="showCloseButton"
          class="btn btn-sm btn-circle btn-ghost absolute right-2 top-2"
          @click="close"
          aria-label="Close modal"
        >
          <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
            <path
              fill-rule="evenodd"
              d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 011.414-1.414z"
              clip-rule="evenodd"
            />
          </svg>
        </button>

        <!-- Modal header -->
        <div v-if="$slots.header || title" class="modal-header">
          <h3 :id="titleId" class="text-lg font-bold">
            <slot name="header">{{ title }}</slot>
          </h3>
        </div>

        <!-- Modal body -->
        <div :id="contentId" class="modal-body">
          <slot />
        </div>

        <!-- Modal actions -->
        <div v-if="$slots.actions" class="modal-actions">
          <slot name="actions" />
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { computed, nextTick, watch, watchEffect } from 'vue'
interface Props {
  isOpen: boolean;
  title?: string;
  size?: "sm" | "md" | "lg" | "xl";
  showCloseButton?: boolean;
  closeOnBackdropClick?: boolean;
  closeOnEscape?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  size: "md",
  showCloseButton: true,
  closeOnBackdropClick: true,
  closeOnEscape: true,
});

const emit = defineEmits<{
  close: [];
  open: [];
}>();

const titleId = computed(
  () => `modal-title-${Math.random().toString(36).substr(2, 9)}`
);
const contentId = computed(
  () => `modal-content-${Math.random().toString(36).substr(2, 9)}`
);

const modalClasses = computed(() => ({
  "max-w-sm": props.size === "sm",
  "max-w-md": props.size === "md",
  "max-w-lg": props.size === "lg",
  "max-w-4xl": props.size === "xl",
}));

const close = () => {
  emit("close");
};

const handleBackdropClick = () => {
  if (props.closeOnBackdropClick) {
    close();
  }
};

// Handle escape key
watchEffect(() => {
  if (props.isOpen && props.closeOnEscape) {
    const handleEscape = (event: KeyboardEvent) => {
      if (event.key === "Escape") {
        close();
      }
    };

    document.addEventListener("keydown", handleEscape);
    return () => document.removeEventListener("keydown", handleEscape);
  }
});

// Focus management
watch(
  () => props.isOpen,
  (isOpen) => {
    if (isOpen) {
      // Store the currently focused element
      const focusedElement = document.activeElement as HTMLElement;

      // Focus the modal
      nextTick(() => {
        const modalBox = document.querySelector(".modal-box") as HTMLElement;
        if (modalBox) {
          modalBox.focus();
        }
      });

      // Restore focus when modal closes
      return () => {
        if (focusedElement && typeof focusedElement.focus === "function") {
          focusedElement.focus();
        }
      };
    }
  }
);
</script>

<style scoped>
.modal {
  @apply fixed inset-0 z-50 flex items-center justify-center;
}

.modal::before {
  content: "";
  @apply absolute inset-0 bg-black/50;
}

.modal-box {
  @apply relative bg-base-100 rounded-box shadow-xl p-6 max-h-[90vh] overflow-y-auto;
  @apply transform transition-all duration-200;
}

.modal-header {
  @apply mb-4;
}

.modal-body {
  @apply mb-6;
}

.modal-actions {
  @apply flex justify-end gap-2;
}

/* Focus trap styles */
.modal-box:focus {
  @apply outline-none;
}
</style>
