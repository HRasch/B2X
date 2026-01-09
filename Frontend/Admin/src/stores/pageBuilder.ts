/**
 * Page Builder Store - Pinia State Management
 * Handles page state, widget operations, undo/redo
 */

import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import type {
  WidgetBase,
  WidgetConfig,
  WidgetStyle,
  WidgetType,
  PageData,
  PageSEO,
} from '@/types/widgets';
import { createWidget } from '@/components/page-builder/widget-registry';

const MAX_HISTORY = 50;

export const usePageBuilderStore = defineStore('pageBuilder', () => {
  // ============================================================================
  // State
  // ============================================================================

  const page = ref<PageData>({
    id: '',
    title: 'Neue Seite',
    slug: '',
    widgets: [],
    seo: {},
    status: 'draft',
    version: 1,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  });

  const selectedWidgetId = ref<string | null>(null);
  const hoveredWidgetId = ref<string | null>(null);
  const clipboard = ref<WidgetBase | null>(null);
  const history = ref<PageData[]>([]);
  const historyIndex = ref(-1);
  const isDirty = ref(false);
  const isPreviewMode = ref(false);
  const previewDevice = ref<'mobile' | 'tablet' | 'desktop'>('desktop');
  const isSaving = ref(false);

  // ============================================================================
  // Getters
  // ============================================================================

  const selectedWidget = computed(() => {
    if (!selectedWidgetId.value) return null;
    return findWidgetById(page.value.widgets, selectedWidgetId.value);
  });

  const canUndo = computed(() => historyIndex.value > 0);
  const canRedo = computed(() => historyIndex.value < history.value.length - 1);

  const flatWidgetList = computed(() => {
    const widgets: WidgetBase[] = [];
    const flatten = (items: WidgetBase[]) => {
      for (const widget of items) {
        widgets.push(widget);
        if ('children' in widget.config && Array.isArray(widget.config.children)) {
          flatten(widget.config.children as WidgetBase[]);
        }
      }
    };
    flatten(page.value.widgets);
    return widgets;
  });

  // ============================================================================
  // Helper Functions
  // ============================================================================

  function findWidgetById(widgets: WidgetBase[], id: string): WidgetBase | null {
    for (const widget of widgets) {
      if (widget.id === id) return widget;
      if ('children' in widget.config && Array.isArray(widget.config.children)) {
        const found = findWidgetById(widget.config.children as WidgetBase[], id);
        if (found) return found;
      }
    }
    return null;
  }

  function findWidgetParent(
    widgets: WidgetBase[],
    id: string,
    parent: WidgetBase | null = null
  ): { parent: WidgetBase | null; index: number } | null {
    for (let i = 0; i < widgets.length; i++) {
      if (widgets[i].id === id) {
        return { parent, index: i };
      }
      if ('children' in widgets[i].config && Array.isArray(widgets[i].config.children)) {
        const found = findWidgetParent(widgets[i].config.children as WidgetBase[], id, widgets[i]);
        if (found) return found;
      }
    }
    return null;
  }

  function removeWidgetById(widgets: WidgetBase[], id: string): boolean {
    const index = widgets.findIndex(w => w.id === id);
    if (index !== -1) {
      widgets.splice(index, 1);
      return true;
    }
    for (const widget of widgets) {
      if ('children' in widget.config && Array.isArray(widget.config.children)) {
        if (removeWidgetById(widget.config.children as WidgetBase[], id)) {
          return true;
        }
      }
    }
    return false;
  }

  function deepClone<T>(obj: T): T {
    return JSON.parse(JSON.stringify(obj));
  }

  // ============================================================================
  // History Management
  // ============================================================================

  function saveToHistory() {
    // Remove any redo states
    if (historyIndex.value < history.value.length - 1) {
      history.value = history.value.slice(0, historyIndex.value + 1);
    }

    // Add current state
    history.value.push(deepClone(page.value));

    // Limit history size
    if (history.value.length > MAX_HISTORY) {
      history.value.shift();
    } else {
      historyIndex.value++;
    }

    isDirty.value = true;
  }

  // ============================================================================
  // Actions
  // ============================================================================

  function initializePage(pageData: PageData) {
    page.value = deepClone(pageData);
    selectedWidgetId.value = null;
    hoveredWidgetId.value = null;
    history.value = [deepClone(pageData)];
    historyIndex.value = 0;
    isDirty.value = false;
  }

  function createNewPage() {
    const newPage: PageData = {
      id: crypto.randomUUID(),
      title: 'Neue Seite',
      slug: '',
      widgets: [],
      seo: {},
      status: 'draft',
      version: 1,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };
    initializePage(newPage);
  }

  function selectWidget(widgetId: string | null) {
    selectedWidgetId.value = widgetId;
  }

  function hoverWidget(widgetId: string | null) {
    hoveredWidgetId.value = widgetId;
  }

  function addWidget(type: WidgetType, parentId?: string, index?: number) {
    saveToHistory();

    const newWidget = createWidget(type) as WidgetBase;

    if (parentId) {
      const parent = findWidgetById(page.value.widgets, parentId);
      if (parent && 'children' in parent.config && Array.isArray(parent.config.children)) {
        const children = parent.config.children as WidgetBase[];
        if (index !== undefined) {
          children.splice(index, 0, newWidget);
        } else {
          children.push(newWidget);
        }
      }
    } else {
      if (index !== undefined) {
        page.value.widgets.splice(index, 0, newWidget);
      } else {
        page.value.widgets.push(newWidget);
      }
    }

    selectedWidgetId.value = newWidget.id;
    page.value.updatedAt = new Date().toISOString();
  }

  function updateWidget(widgetId: string, config: Partial<WidgetConfig>) {
    saveToHistory();

    const widget = findWidgetById(page.value.widgets, widgetId);
    if (widget) {
      widget.config = { ...widget.config, ...config };
      widget.version++;
      page.value.updatedAt = new Date().toISOString();
    }
  }

  function updateWidgetStyle(widgetId: string, style: Partial<WidgetStyle>) {
    saveToHistory();

    const widget = findWidgetById(page.value.widgets, widgetId);
    if (widget) {
      widget.style = { ...widget.style, ...style };
      page.value.updatedAt = new Date().toISOString();
    }
  }

  function removeWidget(widgetId: string) {
    saveToHistory();

    if (selectedWidgetId.value === widgetId) {
      selectedWidgetId.value = null;
    }

    removeWidgetById(page.value.widgets, widgetId);
    page.value.updatedAt = new Date().toISOString();
  }

  function moveWidget(widgetId: string, targetParentId?: string, targetIndex?: number) {
    saveToHistory();

    const widget = findWidgetById(page.value.widgets, widgetId);
    if (!widget) return;

    // Remove from current position
    removeWidgetById(page.value.widgets, widgetId);

    // Add to new position
    if (targetParentId) {
      const parent = findWidgetById(page.value.widgets, targetParentId);
      if (parent && 'children' in parent.config && Array.isArray(parent.config.children)) {
        const children = parent.config.children as WidgetBase[];
        if (targetIndex !== undefined) {
          children.splice(targetIndex, 0, widget);
        } else {
          children.push(widget);
        }
      }
    } else {
      if (targetIndex !== undefined) {
        page.value.widgets.splice(targetIndex, 0, widget);
      } else {
        page.value.widgets.push(widget);
      }
    }

    page.value.updatedAt = new Date().toISOString();
  }

  function copyWidget(widgetId: string) {
    const widget = findWidgetById(page.value.widgets, widgetId);
    if (widget) {
      clipboard.value = deepClone(widget);
    }
  }

  function pasteWidget(parentId?: string, index?: number) {
    if (!clipboard.value) return;

    saveToHistory();

    // Create new widget with new ID
    const newWidget = deepClone(clipboard.value);
    newWidget.id = crypto.randomUUID();

    // Recursively update IDs for children
    const updateIds = (widget: WidgetBase) => {
      widget.id = crypto.randomUUID();
      if ('children' in widget.config && Array.isArray(widget.config.children)) {
        (widget.config.children as WidgetBase[]).forEach(updateIds);
      }
    };
    updateIds(newWidget);

    if (parentId) {
      const parent = findWidgetById(page.value.widgets, parentId);
      if (parent && 'children' in parent.config && Array.isArray(parent.config.children)) {
        const children = parent.config.children as WidgetBase[];
        if (index !== undefined) {
          children.splice(index, 0, newWidget);
        } else {
          children.push(newWidget);
        }
      }
    } else {
      if (index !== undefined) {
        page.value.widgets.splice(index, 0, newWidget);
      } else {
        page.value.widgets.push(newWidget);
      }
    }

    selectedWidgetId.value = newWidget.id;
    page.value.updatedAt = new Date().toISOString();
  }

  function duplicateWidget(widgetId: string) {
    const widget = findWidgetById(page.value.widgets, widgetId);
    if (!widget) return;

    copyWidget(widgetId);

    // Find parent and index
    const location = findWidgetParent(page.value.widgets, widgetId);
    if (location) {
      pasteWidget(location.parent?.id, location.index + 1);
    }
  }

  function undo() {
    if (!canUndo.value) return;

    historyIndex.value--;
    page.value = deepClone(history.value[historyIndex.value]);
    selectedWidgetId.value = null;
  }

  function redo() {
    if (!canRedo.value) return;

    historyIndex.value++;
    page.value = deepClone(history.value[historyIndex.value]);
    selectedWidgetId.value = null;
  }

  function updatePageMeta(meta: Partial<Pick<PageData, 'title' | 'slug' | 'template' | 'status'>>) {
    saveToHistory();
    Object.assign(page.value, meta);
    page.value.updatedAt = new Date().toISOString();
  }

  function updatePageSEO(seo: Partial<PageSEO>) {
    saveToHistory();
    page.value.seo = { ...page.value.seo, ...seo };
    page.value.updatedAt = new Date().toISOString();
  }

  function togglePreviewMode() {
    isPreviewMode.value = !isPreviewMode.value;
    if (isPreviewMode.value) {
      selectedWidgetId.value = null;
    }
  }

  function setPreviewDevice(device: 'mobile' | 'tablet' | 'desktop') {
    previewDevice.value = device;
  }

  function markAsSaved() {
    isDirty.value = false;
  }

  // ============================================================================
  // Return
  // ============================================================================

  return {
    // State
    page,
    selectedWidgetId,
    hoveredWidgetId,
    clipboard,
    history,
    historyIndex,
    isDirty,
    isPreviewMode,
    previewDevice,
    isSaving,

    // Getters
    selectedWidget,
    canUndo,
    canRedo,
    flatWidgetList,

    // Actions
    initializePage,
    createNewPage,
    selectWidget,
    hoverWidget,
    addWidget,
    updateWidget,
    updateWidgetStyle,
    removeWidget,
    moveWidget,
    copyWidget,
    pasteWidget,
    duplicateWidget,
    undo,
    redo,
    updatePageMeta,
    updatePageSEO,
    togglePreviewMode,
    setPreviewDevice,
    markAsSaved,
  };
});
