<script setup lang="ts">
/**
 * BreadcrumbWidget - Breadcrumb navigation
 */
import { computed } from 'vue';
import type { BreadcrumbWidgetConfig } from '@/types/widgets';

const props = defineProps<{
  config: BreadcrumbWidgetConfig;
  isEditing?: boolean;
}>();

// Mock breadcrumb data for preview
const mockBreadcrumbs = [
  { label: 'Elektrowerkzeuge', href: '/kategorie/elektrowerkzeuge' },
  { label: 'Bohrmaschinen', href: '/kategorie/bohrmaschinen' },
  { label: 'Akku-Bohrschrauber', href: '/kategorie/akku-bohrschrauber' },
  { label: 'Akku-Bohrschrauber Pro 18V', href: '' },
];

const displayBreadcrumbs = computed(() => {
  let items = [...mockBreadcrumbs];

  // Add home
  if (props.config.showHome) {
    items.unshift({ label: props.config.homeLabel, href: props.config.homeUrl });
  }

  // Handle max items and truncation
  if (props.config.maxItems && items.length > props.config.maxItems) {
    if (props.config.truncateMiddle) {
      // Keep first, last, and truncation marker
      const first = items[0];
      const last = items.slice(-2);
      items = [first, { label: '...', href: '' }, ...last];
    } else {
      items = items.slice(-props.config.maxItems);
    }
  }

  // Handle current page
  if (!props.config.showCurrentPage) {
    items = items.slice(0, -1);
  }

  return items;
});

const separatorIcons: Record<string, string> = {
  slash: '/',
  chevron: 'M9 5l7 7-7 7',
  arrow: 'M5 12h14m-7-7l7 7-7 7',
  dot: '•',
};

const isLastItem = (index: number) => index === displayBreadcrumbs.value.length - 1;

const isClickable = (item: { href: string }, index: number) => {
  if (isLastItem(index) && !props.config.currentPageClickable) {
    return false;
  }
  return item.href !== '';
};
</script>

<template>
  <nav class="breadcrumb" aria-label="Breadcrumb">
    <ol class="breadcrumb__list">
      <li v-for="(item, index) in displayBreadcrumbs" :key="index" class="breadcrumb__item">
        <!-- Home Icon (first item) -->
        <template v-if="index === 0 && config.showHome">
          <a
            v-if="isClickable(item, index)"
            :href="item.href"
            class="breadcrumb__link breadcrumb__link--home"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M3 9l9-7 9 7v11a2 2 0 01-2 2H5a2 2 0 01-2-2z" />
              <polyline points="9 22 9 12 15 12 15 22" />
            </svg>
            <span class="breadcrumb__label">{{ item.label }}</span>
          </a>
          <span v-else class="breadcrumb__current breadcrumb__link--home">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M3 9l9-7 9 7v11a2 2 0 01-2 2H5a2 2 0 01-2-2z" />
              <polyline points="9 22 9 12 15 12 15 22" />
            </svg>
            <span class="breadcrumb__label">{{ item.label }}</span>
          </span>
        </template>

        <!-- Regular Items -->
        <template v-else>
          <!-- Separator -->
          <span v-if="index > 0" class="breadcrumb__separator" aria-hidden="true">
            <template v-if="config.separator === 'slash' || config.separator === 'dot'">
              {{ separatorIcons[config.separator] }}
            </template>
            <svg v-else viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path :d="separatorIcons[config.separator]" />
            </svg>
          </span>

          <!-- Link or Current -->
          <a v-if="isClickable(item, index)" :href="item.href" class="breadcrumb__link">
            {{ item.label }}
          </a>
          <span
            v-else
            :class="['breadcrumb__current', { 'breadcrumb__current--last': isLastItem(index) }]"
            :aria-current="isLastItem(index) ? 'page' : undefined"
          >
            {{ item.label }}
          </span>
        </template>
      </li>
    </ol>

    <!-- Edit Mode Info -->
    <div v-if="isEditing" class="breadcrumb__edit-info">
      <span>Trenner: {{ config.separator }} | Max: {{ config.maxItems }}</span>
    </div>
  </nav>
</template>

<style scoped>
.breadcrumb {
  position: relative;
}

.breadcrumb__list {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 0.25rem;
  list-style: none;
  margin: 0;
  padding: 0;
}

.breadcrumb__item {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.breadcrumb__separator {
  display: flex;
  align-items: center;
  justify-content: center;
  color: #9ca3af;
  font-size: 0.875rem;
  margin: 0 0.25rem;
}

.breadcrumb__separator svg {
  width: 1rem;
  height: 1rem;
}

.breadcrumb__link {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  color: #6b7280;
  text-decoration: none;
  font-size: 0.875rem;
  transition: color 0.2s;
}

.breadcrumb__link:hover {
  color: #3b82f6;
  text-decoration: underline;
}

.breadcrumb__link--home svg {
  width: 1rem;
  height: 1rem;
}

.breadcrumb__label {
  display: inline;
}

@media (max-width: 640px) {
  .breadcrumb__link--home .breadcrumb__label {
    display: none;
  }
}

.breadcrumb__current {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  color: #9ca3af;
  font-size: 0.875rem;
}

.breadcrumb__current--last {
  color: #111827;
  font-weight: 500;
}

.breadcrumb__edit-info {
  position: absolute;
  top: -1.5rem;
  right: 0;
  padding: 0.25rem 0.5rem;
  background: #dbeafe;
  color: #1e40af;
  font-size: 0.75rem;
  border-radius: 0.25rem;
}
</style>
