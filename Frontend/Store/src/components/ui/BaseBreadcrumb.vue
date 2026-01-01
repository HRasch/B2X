<template>
  <nav aria-label="Breadcrumb" class="breadcrumb">
    <ul class="flex flex-wrap items-center gap-2 text-sm">
      <li v-for="(crumb, index) in breadcrumbs" :key="index">
        <component
          :is="crumb.href ? 'a' : crumb.to ? 'router-link' : 'span'"
          :href="crumb.href"
          :to="crumb.to"
          :class="[
            'flex items-center gap-1 px-2 py-1 rounded-md transition-colors',
            crumb.href || crumb.to
              ? 'text-primary hover:bg-base-200 hover:text-primary-focus'
              : 'text-base-content/60 cursor-default',
          ]"
          :aria-current="index === breadcrumbs.length - 1 ? 'page' : undefined"
        >
          <!-- Icon -->
          <span v-if="crumb.icon" class="shrink-0" aria-hidden="true">
            <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
              <path :d="crumb.icon" />
            </svg>
          </span>

          <!-- Text -->
          <span>{{ crumb.label }}</span>
        </component>

        <!-- Separator -->
        <span
          v-if="index < breadcrumbs.length - 1"
          class="mx-2 text-base-content/40"
          aria-hidden="true"
        >
          <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
            <path
              fill-rule="evenodd"
              d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z"
              clip-rule="evenodd"
            />
          </svg>
        </span>
      </li>
    </ul>
  </nav>
</template>

<script setup lang="ts">
import { computed } from "vue";
export interface BreadcrumbItem {
  label: string;
  href?: string;
  to?: string;
  icon?: string;
}

interface Props {
  items: BreadcrumbItem[];
}

const props = defineProps<Props>();

const breadcrumbs = computed(() => props.items);
</script>

<style scoped>
/* Focus styles for accessibility */
.breadcrumb a:focus-visible,
.breadcrumb [role="link"]:focus-visible {
  @apply outline-none ring-2 ring-primary ring-offset-2;
}
</style>
