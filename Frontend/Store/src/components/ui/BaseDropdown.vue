<template>
  <div
    class="dropdown"
    :class="{
      'dropdown-open': isOpen,
      'dropdown-end': position === 'end',
      'dropdown-top': position === 'top',
      'dropdown-bottom': position === 'bottom',
      'dropdown-left': position === 'left',
      'dropdown-right': position === 'right',
    }"
  >
    <!-- Trigger -->
    <div
      @click="toggle"
      @keydown="handleKeydown"
      :tabindex="0"
      role="button"
      :aria-expanded="isOpen"
      :aria-haspopup="true"
      :aria-label="ariaLabel"
      class="dropdown-toggle"
      :class="triggerClasses"
    >
      <slot name="trigger">
        <button class="btn btn-outline">
          <slot name="trigger-text">Menu</slot>
          <svg class="w-4 h-4 ml-1" fill="currentColor" viewBox="0 0 20 20">
            <path
              fill-rule="evenodd"
              d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 010-1.414L5.293 7.293a1 1 0 01-1.414-1.414z"
              clip-rule="evenodd"
            />
          </svg>
        </button>
      </slot>
    </div>

    <!-- Menu -->
    <Transition name="dropdown-menu" appear>
      <ul
        v-if="isOpen"
        class="dropdown-content menu bg-base-100 rounded-box z-50 w-52 p-2 shadow-lg"
        :class="menuClasses"
        role="menu"
        @click.stop
      >
        <slot />
      </ul>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { computed, nextTick, onMounted, onUnmounted, ref, watch } from "vue";
export type DropdownPosition = "bottom" | "top" | "left" | "right" | "end";

interface Props {
  position?: DropdownPosition;
  triggerClass?: string;
  menuClass?: string;
  ariaLabel?: string;
  closeOnClick?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  position: "bottom",
  closeOnClick: true,
});

const emit = defineEmits<{
  open: [];
  close: [];
  toggle: [isOpen: boolean];
}>();

const isOpen = ref(false);

const triggerClasses = computed(() => props.triggerClass);
const menuClasses = computed(() => props.menuClass);

const toggle = () => {
  isOpen.value = !isOpen.value;
  emit("toggle", isOpen.value);
  if (isOpen.value) {
    emit("open");
  } else {
    emit("close");
  }
};

const close = () => {
  isOpen.value = false;
  emit("close");
};

const handleKeydown = (event: KeyboardEvent) => {
  switch (event.key) {
    case "Enter":
    case " ":
      event.preventDefault();
      toggle();
      break;
    case "Escape":
      if (isOpen.value) {
        event.preventDefault();
        close();
      }
      break;
    case "ArrowDown":
      if (!isOpen.value) {
        event.preventDefault();
        toggle();
      }
      break;
  }
};

// Close dropdown when clicking outside
const handleClickOutside = (event: Event) => {
  const target = event.target as HTMLElement;
  if (!target.closest(".dropdown")) {
    close();
  }
};

onMounted(() => {
  document.addEventListener("click", handleClickOutside);
});

onUnmounted(() => {
  document.removeEventListener("click", handleClickOutside);
});

// Focus management
watch(isOpen, (newIsOpen) => {
  if (newIsOpen) {
    nextTick(() => {
      const menu = document.querySelector(".dropdown-content") as HTMLElement;
      if (menu) {
        const firstItem = menu.querySelector(
          "[role='menuitem']"
        ) as HTMLElement;
        if (firstItem) {
          firstItem.focus();
        }
      }
    });
  }
});
</script>

<style scoped>
.dropdown-toggle:focus-visible {
  @apply outline-none ring-2 ring-primary ring-offset-2;
}

/* Dropdown menu transition animations */
.dropdown-menu-enter-active,
.dropdown-menu-leave-active {
  transition: all 0.2s ease;
}

.dropdown-menu-enter-from {
  opacity: 0;
  transform: translateY(-10px) scale(0.95);
}

.dropdown-menu-leave-to {
  opacity: 0;
  transform: translateY(-10px) scale(0.95);
}
</style>
