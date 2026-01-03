<script setup lang="ts">
/**
 * PageBuilder - Main component for visual page editing
 * Features: Canvas, Drag-and-Drop, Toolbar, Preview Mode
 */
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { usePageBuilderStore } from '@/stores/pageBuilder';
import WidgetPalette from './WidgetPalette.vue';
import WidgetConfigurator from './WidgetConfigurator.vue';
import WidgetRenderer from './WidgetRenderer.vue';
import type { WidgetType } from '@/types/widgets';

const props = defineProps<{
  pageId: string;
}>();

const emit = defineEmits<{
  (e: 'save'): void;
  (e: 'publish'): void;
}>();

const store = usePageBuilderStore();

// Local state
const isDragOver = ref(false);
const canvasRef = ref<HTMLElement | null>(null);

// Computed
const isPreviewMode = computed(() => store.isPreviewMode);
const canUndo = computed(() => store.canUndo);
const canRedo = computed(() => store.canRedo);
const hasClipboard = computed(() => store.clipboard !== null);
const widgets = computed(() => store.page?.widgets ?? []);

// Handlers
function handleDragOver(event: DragEvent) {
  event.preventDefault();
  isDragOver.value = true;
  if (event.dataTransfer) {
    event.dataTransfer.dropEffect = 'copy';
  }
}

function handleDragLeave() {
  isDragOver.value = false;
}

function handleDrop(event: DragEvent) {
  event.preventDefault();
  isDragOver.value = false;

  if (!event.dataTransfer) return;

  const widgetType = event.dataTransfer.getData('widget-type') as WidgetType;
  if (widgetType) {
    store.addWidget(widgetType);
  }
}

function handleCanvasClick(event: MouseEvent) {
  // Deselect if clicking on canvas (not on a widget)
  if (event.target === canvasRef.value) {
    store.selectWidget(null);
  }
}

// Keyboard shortcuts
function handleKeyDown(event: KeyboardEvent) {
  // Ignore if typing in input
  if (event.target instanceof HTMLInputElement || event.target instanceof HTMLTextAreaElement) {
    return;
  }

  const isMac = navigator.platform.toUpperCase().indexOf('MAC') >= 0;
  const cmdKey = isMac ? event.metaKey : event.ctrlKey;

  if (cmdKey && event.key === 'z') {
    event.preventDefault();
    if (event.shiftKey) {
      store.redo();
    } else {
      store.undo();
    }
  }

  if (cmdKey && event.key === 'y') {
    event.preventDefault();
    store.redo();
  }

  if (cmdKey && event.key === 'c') {
    event.preventDefault();
    store.copyWidget();
  }

  if (cmdKey && event.key === 'v') {
    event.preventDefault();
    store.pasteWidget();
  }

  if (event.key === 'Delete' || event.key === 'Backspace') {
    if (store.selectedWidgetId) {
      event.preventDefault();
      store.removeWidget(store.selectedWidgetId);
    }
  }

  if (event.key === 'Escape') {
    store.selectWidget(null);
  }
}

// Lifecycle
onMounted(() => {
  store.loadPage(props.pageId);
  window.addEventListener('keydown', handleKeyDown);
});

onUnmounted(() => {
  window.removeEventListener('keydown', handleKeyDown);
});

// Save/Publish handlers
function handleSave() {
  emit('save');
}

function handlePublish() {
  emit('publish');
}
</script>

<template>
  <div class="page-builder">
    <!-- Toolbar -->
    <header class="page-builder__toolbar">
      <div class="page-builder__toolbar-left">
        <h1 class="page-builder__title">Page Builder</h1>
        <span v-if="store.page" class="page-builder__page-name">{{ store.page.name }}</span>
      </div>

      <div class="page-builder__toolbar-center">
        <!-- Undo/Redo -->
        <div class="page-builder__toolbar-group">
          <button
            class="page-builder__toolbar-btn"
            :disabled="!canUndo"
            @click="store.undo()"
            title="Rückgängig (Strg+Z)"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M3 10h10a8 8 0 018 8v2M3 10l6 6m-6-6l6-6" />
            </svg>
          </button>
          <button
            class="page-builder__toolbar-btn"
            :disabled="!canRedo"
            @click="store.redo()"
            title="Wiederholen (Strg+Y)"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M21 10H11a8 8 0 00-8 8v2M21 10l-6 6m6-6l-6-6" />
            </svg>
          </button>
        </div>

        <!-- Copy/Paste -->
        <div class="page-builder__toolbar-group">
          <button
            class="page-builder__toolbar-btn"
            :disabled="!store.selectedWidgetId"
            @click="store.copyWidget()"
            title="Kopieren (Strg+C)"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path
                d="M8 5H6a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2v-1M8 5a2 2 0 002 2h2a2 2 0 002-2M8 5a2 2 0 012-2h2a2 2 0 012 2m0 0h2a2 2 0 012 2v3m2 4H10m0 0l3-3m-3 3l3 3"
              />
            </svg>
          </button>
          <button
            class="page-builder__toolbar-btn"
            :disabled="!hasClipboard"
            @click="store.pasteWidget()"
            title="Einfügen (Strg+V)"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path
                d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"
              />
            </svg>
          </button>
        </div>

        <!-- Preview Toggle -->
        <div class="page-builder__toolbar-group">
          <button
            :class="[
              'page-builder__toolbar-btn',
              'page-builder__toolbar-btn--preview',
              { 'page-builder__toolbar-btn--active': isPreviewMode },
            ]"
            @click="store.togglePreviewMode()"
            :title="isPreviewMode ? 'Bearbeiten' : 'Vorschau'"
          >
            <svg
              v-if="!isPreviewMode"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2"
            >
              <path d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
              <path
                d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"
              />
            </svg>
            <svg v-else viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path
                d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z"
              />
            </svg>
            <span>{{ isPreviewMode ? 'Bearbeiten' : 'Vorschau' }}</span>
          </button>
        </div>
      </div>

      <div class="page-builder__toolbar-right">
        <button
          class="page-builder__toolbar-btn page-builder__toolbar-btn--secondary"
          @click="handleSave"
        >
          Speichern
        </button>
        <button
          class="page-builder__toolbar-btn page-builder__toolbar-btn--primary"
          @click="handlePublish"
        >
          Veröffentlichen
        </button>
      </div>
    </header>

    <!-- Main Content -->
    <div class="page-builder__main">
      <!-- Widget Palette (left sidebar) -->
      <aside v-if="!isPreviewMode" class="page-builder__sidebar page-builder__sidebar--left">
        <WidgetPalette />
      </aside>

      <!-- Canvas -->
      <main
        ref="canvasRef"
        :class="[
          'page-builder__canvas',
          {
            'page-builder__canvas--preview': isPreviewMode,
            'page-builder__canvas--drag-over': isDragOver,
          },
        ]"
        @dragover="handleDragOver"
        @dragleave="handleDragLeave"
        @drop="handleDrop"
        @click="handleCanvasClick"
      >
        <div class="page-builder__canvas-inner">
          <!-- Empty State -->
          <div v-if="widgets.length === 0" class="page-builder__empty">
            <svg
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="1"
              class="page-builder__empty-icon"
            >
              <path
                d="M4 5a1 1 0 011-1h14a1 1 0 011 1v2a1 1 0 01-1 1H5a1 1 0 01-1-1V5zM4 13a1 1 0 011-1h6a1 1 0 011 1v6a1 1 0 01-1 1H5a1 1 0 01-1-1v-6zM16 13a1 1 0 011-1h2a1 1 0 011 1v6a1 1 0 01-1 1h-2a1 1 0 01-1-1v-6z"
              />
            </svg>
            <h2 class="page-builder__empty-title">Seite ist leer</h2>
            <p class="page-builder__empty-text">
              Ziehe Widgets aus der linken Palette hierher<br />
              oder klicke auf ein Widget, um es hinzuzufügen.
            </p>
          </div>

          <!-- Widgets -->
          <div v-else class="page-builder__widgets">
            <WidgetRenderer v-for="widget in widgets" :key="widget.id" :widget="widget" />
          </div>
        </div>
      </main>

      <!-- Widget Configurator (right sidebar) -->
      <aside v-if="!isPreviewMode" class="page-builder__sidebar page-builder__sidebar--right">
        <WidgetConfigurator />
      </aside>
    </div>
  </div>
</template>

<style scoped>
.page-builder {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background-color: #f3f4f6;
}

/* Toolbar */
.page-builder__toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.75rem 1rem;
  background-color: white;
  border-bottom: 1px solid #e5e7eb;
  gap: 1rem;
}

.page-builder__toolbar-left,
.page-builder__toolbar-center,
.page-builder__toolbar-right {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.page-builder__toolbar-left {
  flex: 1;
}

.page-builder__toolbar-right {
  flex: 1;
  justify-content: flex-end;
}

.page-builder__title {
  font-size: 1rem;
  font-weight: 600;
  color: #111827;
  margin: 0;
}

.page-builder__page-name {
  font-size: 0.875rem;
  color: #6b7280;
  padding-left: 0.75rem;
  border-left: 1px solid #e5e7eb;
}

.page-builder__toolbar-group {
  display: flex;
  align-items: center;
  background-color: #f3f4f6;
  border-radius: 6px;
  padding: 2px;
}

.page-builder__toolbar-btn {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.5rem;
  background-color: transparent;
  border: none;
  border-radius: 4px;
  color: #374151;
  cursor: pointer;
  transition: all 0.2s;
}

.page-builder__toolbar-btn:hover:not(:disabled) {
  background-color: #e5e7eb;
}

.page-builder__toolbar-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.page-builder__toolbar-btn svg {
  width: 18px;
  height: 18px;
}

.page-builder__toolbar-btn--preview {
  padding: 0.5rem 0.75rem;
  font-size: 0.875rem;
  font-weight: 500;
}

.page-builder__toolbar-btn--active {
  background-color: #dbeafe;
  color: var(--color-primary, #3b82f6);
}

.page-builder__toolbar-btn--secondary {
  padding: 0.5rem 1rem;
  background-color: white;
  border: 1px solid #d1d5db;
  font-size: 0.875rem;
  font-weight: 500;
}

.page-builder__toolbar-btn--secondary:hover {
  background-color: #f9fafb;
}

.page-builder__toolbar-btn--primary {
  padding: 0.5rem 1rem;
  background-color: var(--color-primary, #3b82f6);
  border: 1px solid var(--color-primary, #3b82f6);
  color: white;
  font-size: 0.875rem;
  font-weight: 500;
}

.page-builder__toolbar-btn--primary:hover {
  background-color: #2563eb;
  border-color: #2563eb;
}

/* Main Layout */
.page-builder__main {
  display: flex;
  flex: 1;
  overflow: hidden;
}

.page-builder__sidebar {
  width: 280px;
  flex-shrink: 0;
  overflow: hidden;
}

.page-builder__sidebar--left {
  border-right: 1px solid #e5e7eb;
}

.page-builder__sidebar--right {
  border-left: 1px solid #e5e7eb;
}

/* Canvas */
.page-builder__canvas {
  flex: 1;
  overflow: auto;
  padding: 2rem;
  background-color: #e5e7eb;
  background-image:
    linear-gradient(45deg, #d1d5db 25%, transparent 25%),
    linear-gradient(-45deg, #d1d5db 25%, transparent 25%),
    linear-gradient(45deg, transparent 75%, #d1d5db 75%),
    linear-gradient(-45deg, transparent 75%, #d1d5db 75%);
  background-size: 20px 20px;
  background-position:
    0 0,
    0 10px,
    10px -10px,
    -10px 0px;
}

.page-builder__canvas--preview {
  background-image: none;
  background-color: white;
}

.page-builder__canvas--drag-over {
  background-color: #dbeafe;
}

.page-builder__canvas-inner {
  max-width: 1200px;
  min-height: 100%;
  margin: 0 auto;
  background-color: white;
  box-shadow:
    0 4px 6px -1px rgba(0, 0, 0, 0.1),
    0 2px 4px -1px rgba(0, 0, 0, 0.06);
  border-radius: 8px;
  overflow: hidden;
}

.page-builder__canvas--preview .page-builder__canvas-inner {
  box-shadow: none;
  border-radius: 0;
}

/* Empty State */
.page-builder__empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  text-align: center;
}

.page-builder__empty-icon {
  width: 80px;
  height: 80px;
  color: #d1d5db;
  margin-bottom: 1.5rem;
}

.page-builder__empty-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #374151;
  margin: 0 0 0.5rem;
}

.page-builder__empty-text {
  color: #6b7280;
  font-size: 0.875rem;
  margin: 0;
  line-height: 1.6;
}

/* Widgets Container */
.page-builder__widgets {
  min-height: 200px;
  padding: 1rem;
}
</style>
