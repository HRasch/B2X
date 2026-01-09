<script setup lang="ts">
/**
 * WidgetPalette - Sidebar with available widgets to add
 */
import { computed } from 'vue';
import { usePageBuilderStore } from '@/stores/pageBuilder';
import { widgetRegistry, getWidgetsByCategory } from './widget-registry';
import type { WidgetType, WidgetCategory } from '@/types/widgets';

const store = usePageBuilderStore();

// Group widgets by category
const categories: { id: WidgetCategory; label: string; icon: string }[] = [
  { id: 'layout', label: 'Layout', icon: 'layout' },
  { id: 'content', label: 'Inhalt', icon: 'type' },
  { id: 'account', label: 'Kundenkonto', icon: 'user' },
  { id: 'ecommerce', label: 'E-Commerce', icon: 'shopping-bag' },
];

const widgetsByCategory = computed(() => {
  const result: Record<string, (typeof widgetRegistry)[WidgetType][]> = {};
  for (const category of categories) {
    result[category.id] = getWidgetsByCategory(category.id);
  }
  return result;
});

function handleDragStart(event: DragEvent, type: WidgetType) {
  if (event.dataTransfer) {
    event.dataTransfer.setData('widget-type', type);
    event.dataTransfer.effectAllowed = 'copy';
  }
}

function handleAddWidget(type: WidgetType) {
  store.addWidget(type);
}

// Icon mapping
const icons: Record<string, string> = {
  layout:
    'M4 5a1 1 0 011-1h14a1 1 0 011 1v2a1 1 0 01-1 1H5a1 1 0 01-1-1V5zM4 13a1 1 0 011-1h6a1 1 0 011 1v6a1 1 0 01-1 1H5a1 1 0 01-1-1v-6zM16 13a1 1 0 011-1h2a1 1 0 011 1v6a1 1 0 01-1 1h-2a1 1 0 01-1-1v-6z',
  type: 'M4 6h16M4 12h16M4 18h7',
  user: 'M20 21v-2a4 4 0 00-4-4H8a4 4 0 00-4 4v2M12 11a4 4 0 100-8 4 4 0 000 8z',
  'shopping-bag': 'M6 2L3 6v14a2 2 0 002 2h14a2 2 0 002-2V6l-3-4zM3 6h18M16 10a4 4 0 01-8 0',
  'grid-3x3': 'M3 3h7v7H3V3zm11 0h7v7h-7V3zm0 11h7v7h-7v-7zM3 14h7v7H3v-7z',
  'rectangle-horizontal': 'M2 7a2 2 0 012-2h16a2 2 0 012 2v10a2 2 0 01-2 2H4a2 2 0 01-2-2V7z',
  square: 'M3 3h18v18H3V3z',
  'move-vertical': 'M12 2v20M5 9l7-7 7 7M5 15l7 7 7-7',
  minus: 'M5 12h14',
  image:
    'M4 4h16a2 2 0 012 2v12a2 2 0 01-2 2H4a2 2 0 01-2-2V6a2 2 0 012-2zm0 0l6 6 4-4 6 6M14 10a2 2 0 11-4 0 2 2 0 014 0z',
  'mouse-pointer-click': 'M9 9l10 5-5 2-2 5L9 9z',
  'layout-dashboard': 'M3 3h7v9H3V3zm11 0h7v5h-7V3zm0 9h7v9h-7v-9zM3 16h7v5H3v-5z',
  'clipboard-list':
    'M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01',
  'map-pin': 'M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0118 0zM12 13a3 3 0 100-6 3 3 0 000 6z',
  'user-circle':
    'M5.121 17.804A13.937 13.937 0 0112 16c2.5 0 4.847.655 6.879 1.804M15 10a3 3 0 11-6 0 3 3 0 016 0zm6 2a9 9 0 11-18 0 9 9 0 0118 0z',
  heart:
    'M20.84 4.61a5.5 5.5 0 00-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 00-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 000-7.78z',
  // E-Commerce icons
  'layout-grid': 'M3 3h7v7H3V3zm11 0h7v7h-7V3zm0 11h7v7h-7v-7zM3 14h7v7H3v-7z',
  'folder-tree':
    'M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2h-6l-2-2H5a2 2 0 00-2 2zM8 12h8M8 16h5',
  search: 'M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z',
  receipt: 'M4 4v16h16V4H4zm4 4h8M8 12h8M8 16h5',
  'shopping-cart':
    'M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 100 4 2 2 0 000-4z',
  'chevrons-right': 'M13 17l5-5-5-5M6 17l5-5-5-5',
};

function getIcon(iconName: string): string {
  return icons[iconName] || icons['square'];
}
</script>

<template>
  <div class="widget-palette">
    <div class="widget-palette__header">
      <h3 class="widget-palette__title">{{ $t('pageBuilder.palette.title') }}</h3>
      <p class="widget-palette__subtitle">{{ $t('pageBuilder.palette.subtitle') }}</p>
    </div>

    <div class="widget-palette__categories">
      <div v-for="category in categories" :key="category.id" class="widget-palette__category">
        <div class="widget-palette__category-header">
          <svg class="widget-palette__category-icon" viewBox="0 0 24 24" fill="currentColor">
            <path :d="getIcon(category.icon)" />
          </svg>
          <span class="widget-palette__category-label">{{ category.label }}</span>
        </div>

        <div class="widget-palette__widgets">
          <button
            v-for="widget in widgetsByCategory[category.id]"
            :key="widget.type"
            class="widget-palette__widget"
            draggable="true"
            @dragstart="handleDragStart($event, widget.type)"
            @click="handleAddWidget(widget.type)"
          >
            <div class="widget-palette__widget-icon">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                <path :d="getIcon(widget.icon)" />
              </svg>
            </div>
            <div class="widget-palette__widget-info">
              <span class="widget-palette__widget-name">{{ widget.name }}</span>
              <span class="widget-palette__widget-desc">{{ widget.description }}</span>
            </div>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.widget-palette {
  display: flex;
  flex-direction: column;
  height: 100%;
  background-color: #f9fafb;
  border-right: 1px solid #e5e7eb;
}

.widget-palette__header {
  padding: 1rem;
  border-bottom: 1px solid #e5e7eb;
}

.widget-palette__title {
  font-size: 1rem;
  font-weight: 600;
  color: #111827;
  margin: 0;
}

.widget-palette__subtitle {
  font-size: 0.75rem;
  color: #6b7280;
  margin: 0.25rem 0 0;
}

.widget-palette__categories {
  flex: 1;
  overflow-y: auto;
  padding: 0.5rem;
}

.widget-palette__category {
  margin-bottom: 1rem;
}

.widget-palette__category-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem;
  color: #4b5563;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.widget-palette__category-icon {
  width: 16px;
  height: 16px;
}

.widget-palette__widgets {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.widget-palette__widget {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.625rem 0.75rem;
  background-color: white;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  cursor: grab;
  transition: all 0.2s;
  text-align: left;
  width: 100%;
}

.widget-palette__widget:hover {
  border-color: var(--color-primary, #3b82f6);
  background-color: #f0f9ff;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.widget-palette__widget:active {
  cursor: grabbing;
  transform: scale(0.98);
}

.widget-palette__widget-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  background-color: #f3f4f6;
  border-radius: 6px;
  flex-shrink: 0;
}

.widget-palette__widget-icon svg {
  width: 18px;
  height: 18px;
  color: #6b7280;
}

.widget-palette__widget:hover .widget-palette__widget-icon {
  background-color: #dbeafe;
}

.widget-palette__widget:hover .widget-palette__widget-icon svg {
  color: var(--color-primary, #3b82f6);
}

.widget-palette__widget-info {
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.widget-palette__widget-name {
  font-size: 0.875rem;
  font-weight: 500;
  color: #111827;
}

.widget-palette__widget-desc {
  font-size: 0.75rem;
  color: #6b7280;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
</style>
