<script setup lang="ts">
/**
 * WidgetConfigurator - Panel to configure selected widget
 */
import { computed } from 'vue'
import { usePageBuilderStore } from '@/stores/pageBuilder'
import type { WidgetConfig, TextWidgetConfig, ImageWidgetConfig, ButtonWidgetConfig, GridWidgetConfig, SectionWidgetConfig, SpacerWidgetConfig, DividerWidgetConfig, ContainerWidgetConfig } from '@/types/widgets'

const store = usePageBuilderStore()

const selectedWidget = computed(() => store.selectedWidget)

const hasSelection = computed(() => selectedWidget.value !== null)

// Update widget config
function updateConfig<T extends WidgetConfig>(updates: Partial<T>) {
  if (!selectedWidget.value) return
  store.updateWidget(selectedWidget.value.id, {
    ...selectedWidget.value,
    config: { ...selectedWidget.value.config, ...updates }
  } as typeof selectedWidget.value)
}

// Get responsive value for current breakpoint
function getResponsiveValue<T>(value: T | { mobile: T; tablet?: T; desktop?: T }): T {
  if (typeof value === 'object' && value !== null && 'mobile' in value) {
    return value.desktop ?? value.tablet ?? value.mobile
  }
  return value
}

// Update responsive value
function setResponsiveValue<T>(field: string, value: T) {
  const currentConfig = selectedWidget.value?.config as Record<string, unknown>
  const currentValue = currentConfig?.[field]
  
  if (typeof currentValue === 'object' && currentValue !== null && 'mobile' in currentValue) {
    // Update desktop breakpoint for responsive values
    updateConfig({ [field]: { ...currentValue, desktop: value } })
  } else {
    updateConfig({ [field]: value })
  }
}
</script>

<template>
  <div class="widget-configurator">
    <div class="widget-configurator__header">
      <h3 class="widget-configurator__title">Konfiguration</h3>
    </div>

    <div v-if="!hasSelection" class="widget-configurator__empty">
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" class="widget-configurator__empty-icon">
        <path d="M9.879 7.519c1.171-1.025 3.071-1.025 4.242 0 1.172 1.025 1.172 2.687 0 3.712-.203.179-.43.326-.67.442-.745.361-1.45.999-1.45 1.827v.75M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-9 5.25h.008v.008H12v-.008z" />
      </svg>
      <p class="widget-configurator__empty-text">
        Wähle ein Widget aus, um es zu konfigurieren
      </p>
    </div>

    <div v-else class="widget-configurator__content">
      <!-- Widget Name -->
      <div class="widget-configurator__section">
        <label class="widget-configurator__label">Widget</label>
        <div class="widget-configurator__widget-info">
          <span class="widget-configurator__widget-type">{{ selectedWidget?.type }}</span>
        </div>
      </div>

      <!-- Common Styles -->
      <div class="widget-configurator__section">
        <label class="widget-configurator__label">Allgemein</label>
        <div class="widget-configurator__group">
          <!-- Visibility -->
          <label class="widget-configurator__checkbox">
            <input
              type="checkbox"
              :checked="selectedWidget?.style?.isVisible ?? true"
              @change="store.updateWidget(selectedWidget!.id, { 
                ...selectedWidget!, 
                style: { ...selectedWidget?.style, isVisible: ($event.target as HTMLInputElement).checked } 
              })"
            >
            <span>Sichtbar</span>
          </label>
        </div>
      </div>

      <!-- Text Widget Config -->
      <template v-if="selectedWidget?.type === 'text'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Text</label>
          <textarea
            :value="getResponsiveValue((selectedWidget.config as TextWidgetConfig).content || '')"
            @input="setResponsiveValue('content', ($event.target as HTMLTextAreaElement).value)"
            class="widget-configurator__textarea"
            rows="4"
            placeholder="Text eingeben..."
          />
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">HTML-Tag</label>
          <select
            :value="(selectedWidget.config as TextWidgetConfig).tag || 'p'"
            @change="updateConfig({ tag: ($event.target as HTMLSelectElement).value as 'p' | 'h1' | 'h2' | 'h3' | 'h4' | 'h5' | 'h6' | 'span' })"
            class="widget-configurator__select"
          >
            <option value="p">Absatz (p)</option>
            <option value="h1">Überschrift 1 (h1)</option>
            <option value="h2">Überschrift 2 (h2)</option>
            <option value="h3">Überschrift 3 (h3)</option>
            <option value="h4">Überschrift 4 (h4)</option>
            <option value="h5">Überschrift 5 (h5)</option>
            <option value="h6">Überschrift 6 (h6)</option>
            <option value="span">Inline (span)</option>
          </select>
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Ausrichtung</label>
          <div class="widget-configurator__button-group">
            <button
              :class="['widget-configurator__btn', { 'widget-configurator__btn--active': (selectedWidget.config as TextWidgetConfig).align === 'left' }]"
              @click="updateConfig({ align: 'left' })"
            >Links</button>
            <button
              :class="['widget-configurator__btn', { 'widget-configurator__btn--active': (selectedWidget.config as TextWidgetConfig).align === 'center' }]"
              @click="updateConfig({ align: 'center' })"
            >Mitte</button>
            <button
              :class="['widget-configurator__btn', { 'widget-configurator__btn--active': (selectedWidget.config as TextWidgetConfig).align === 'right' }]"
              @click="updateConfig({ align: 'right' })"
            >Rechts</button>
          </div>
        </div>
      </template>

      <!-- Image Widget Config -->
      <template v-else-if="selectedWidget?.type === 'image'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Bild-URL</label>
          <input
            type="text"
            :value="getResponsiveValue((selectedWidget.config as ImageWidgetConfig).src || '')"
            @input="setResponsiveValue('src', ($event.target as HTMLInputElement).value)"
            class="widget-configurator__input"
            placeholder="https://..."
          >
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Alt-Text</label>
          <input
            type="text"
            :value="(selectedWidget.config as ImageWidgetConfig).alt || ''"
            @input="updateConfig({ alt: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__input"
            placeholder="Bildbeschreibung..."
          >
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Größe</label>
          <div class="widget-configurator__button-group">
            <button
              :class="['widget-configurator__btn', { 'widget-configurator__btn--active': (selectedWidget.config as ImageWidgetConfig).objectFit === 'contain' }]"
              @click="updateConfig({ objectFit: 'contain' })"
            >Einpassen</button>
            <button
              :class="['widget-configurator__btn', { 'widget-configurator__btn--active': (selectedWidget.config as ImageWidgetConfig).objectFit === 'cover' }]"
              @click="updateConfig({ objectFit: 'cover' })"
            >Füllen</button>
          </div>
        </div>
      </template>

      <!-- Button Widget Config -->
      <template v-else-if="selectedWidget?.type === 'button'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Button-Text</label>
          <input
            type="text"
            :value="getResponsiveValue((selectedWidget.config as ButtonWidgetConfig).label || '')"
            @input="setResponsiveValue('label', ($event.target as HTMLInputElement).value)"
            class="widget-configurator__input"
            placeholder="Button"
          >
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Link</label>
          <input
            type="text"
            :value="(selectedWidget.config as ButtonWidgetConfig).href || ''"
            @input="updateConfig({ href: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__input"
            placeholder="/seite oder https://..."
          >
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Variante</label>
          <select
            :value="(selectedWidget.config as ButtonWidgetConfig).variant || 'primary'"
            @change="updateConfig({ variant: ($event.target as HTMLSelectElement).value as 'primary' | 'secondary' | 'outline' | 'ghost' })"
            class="widget-configurator__select"
          >
            <option value="primary">Primär</option>
            <option value="secondary">Sekundär</option>
            <option value="outline">Umriss</option>
            <option value="ghost">Ghost</option>
          </select>
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Größe</label>
          <select
            :value="(selectedWidget.config as ButtonWidgetConfig).size || 'md'"
            @change="updateConfig({ size: ($event.target as HTMLSelectElement).value as 'sm' | 'md' | 'lg' })"
            class="widget-configurator__select"
          >
            <option value="sm">Klein</option>
            <option value="md">Mittel</option>
            <option value="lg">Groß</option>
          </select>
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__checkbox">
            <input
              type="checkbox"
              :checked="(selectedWidget.config as ButtonWidgetConfig).fullWidth || false"
              @change="updateConfig({ fullWidth: ($event.target as HTMLInputElement).checked })"
            >
            <span>Volle Breite</span>
          </label>
        </div>
      </template>

      <!-- Grid Widget Config -->
      <template v-else-if="selectedWidget?.type === 'grid'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Spalten</label>
          <input
            type="number"
            :value="getResponsiveValue((selectedWidget.config as GridWidgetConfig).columns || 2)"
            @input="setResponsiveValue('columns', parseInt(($event.target as HTMLInputElement).value) || 2)"
            class="widget-configurator__input"
            min="1"
            max="12"
          >
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Abstand</label>
          <input
            type="text"
            :value="getResponsiveValue((selectedWidget.config as GridWidgetConfig).gap || '1rem')"
            @input="setResponsiveValue('gap', ($event.target as HTMLInputElement).value)"
            class="widget-configurator__input"
            placeholder="1rem"
          >
        </div>
      </template>

      <!-- Section Widget Config -->
      <template v-else-if="selectedWidget?.type === 'section'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Hintergrundfarbe</label>
          <input
            type="color"
            :value="(selectedWidget.config as SectionWidgetConfig).backgroundColor || '#ffffff'"
            @input="updateConfig({ backgroundColor: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__color"
          >
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Volle Breite</label>
          <label class="widget-configurator__checkbox">
            <input
              type="checkbox"
              :checked="(selectedWidget.config as SectionWidgetConfig).fullWidth || false"
              @change="updateConfig({ fullWidth: ($event.target as HTMLInputElement).checked })"
            >
            <span>Aktiviert</span>
          </label>
        </div>
      </template>

      <!-- Container Widget Config -->
      <template v-else-if="selectedWidget?.type === 'container'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Max. Breite</label>
          <select
            :value="getResponsiveValue((selectedWidget.config as ContainerWidgetConfig).maxWidth || '1200px')"
            @change="setResponsiveValue('maxWidth', ($event.target as HTMLSelectElement).value)"
            class="widget-configurator__select"
          >
            <option value="640px">Klein (640px)</option>
            <option value="768px">Mittel (768px)</option>
            <option value="1024px">Groß (1024px)</option>
            <option value="1200px">Extra Groß (1200px)</option>
            <option value="100%">Volle Breite</option>
          </select>
        </div>
      </template>

      <!-- Spacer Widget Config -->
      <template v-else-if="selectedWidget?.type === 'spacer'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Höhe</label>
          <input
            type="text"
            :value="getResponsiveValue((selectedWidget.config as SpacerWidgetConfig).height || '2rem')"
            @input="setResponsiveValue('height', ($event.target as HTMLInputElement).value)"
            class="widget-configurator__input"
            placeholder="2rem"
          >
        </div>
      </template>

      <!-- Divider Widget Config -->
      <template v-else-if="selectedWidget?.type === 'divider'">
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Stil</label>
          <select
            :value="(selectedWidget.config as DividerWidgetConfig).style || 'solid'"
            @change="updateConfig({ style: ($event.target as HTMLSelectElement).value as 'solid' | 'dashed' | 'dotted' })"
            class="widget-configurator__select"
          >
            <option value="solid">Durchgezogen</option>
            <option value="dashed">Gestrichelt</option>
            <option value="dotted">Gepunktet</option>
          </select>
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Farbe</label>
          <input
            type="color"
            :value="(selectedWidget.config as DividerWidgetConfig).color || '#e5e7eb'"
            @input="updateConfig({ color: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__color"
          >
        </div>
        <div class="widget-configurator__section">
          <label class="widget-configurator__label">Dicke</label>
          <input
            type="text"
            :value="(selectedWidget.config as DividerWidgetConfig).thickness || '1px'"
            @input="updateConfig({ thickness: ($event.target as HTMLInputElement).value })"
            class="widget-configurator__input"
            placeholder="1px"
          >
        </div>
      </template>

      <!-- Delete Widget -->
      <div class="widget-configurator__section widget-configurator__section--danger">
        <button
          class="widget-configurator__delete-btn"
          @click="store.removeWidget(selectedWidget!.id)"
        >
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
            <path d="M14.74 9l-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 01-2.244 2.077H8.084a2.25 2.25 0 01-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 00-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 013.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 00-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 00-7.5 0" />
          </svg>
          Widget löschen
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.widget-configurator {
  display: flex;
  flex-direction: column;
  height: 100%;
  background-color: #f9fafb;
  border-left: 1px solid #e5e7eb;
}

.widget-configurator__header {
  padding: 1rem;
  border-bottom: 1px solid #e5e7eb;
}

.widget-configurator__title {
  font-size: 1rem;
  font-weight: 600;
  color: #111827;
  margin: 0;
}

.widget-configurator__empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  text-align: center;
  flex: 1;
}

.widget-configurator__empty-icon {
  width: 48px;
  height: 48px;
  color: #d1d5db;
  margin-bottom: 1rem;
}

.widget-configurator__empty-text {
  color: #6b7280;
  font-size: 0.875rem;
  margin: 0;
}

.widget-configurator__content {
  flex: 1;
  overflow-y: auto;
  padding: 0.5rem;
}

.widget-configurator__section {
  padding: 0.75rem;
  background-color: white;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  margin-bottom: 0.5rem;
}

.widget-configurator__section--danger {
  background-color: #fef2f2;
  border-color: #fee2e2;
}

.widget-configurator__label {
  display: block;
  font-size: 0.75rem;
  font-weight: 600;
  color: #374151;
  margin-bottom: 0.5rem;
  text-transform: uppercase;
  letter-spacing: 0.025em;
}

.widget-configurator__widget-info {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.widget-configurator__widget-type {
  display: inline-block;
  padding: 0.25rem 0.5rem;
  background-color: #e5e7eb;
  color: #374151;
  font-size: 0.75rem;
  font-weight: 500;
  border-radius: 4px;
  text-transform: capitalize;
}

.widget-configurator__input,
.widget-configurator__select,
.widget-configurator__textarea {
  width: 100%;
  padding: 0.5rem 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 0.875rem;
  color: #111827;
  background-color: white;
  transition: border-color 0.2s;
}

.widget-configurator__input:focus,
.widget-configurator__select:focus,
.widget-configurator__textarea:focus {
  outline: none;
  border-color: var(--color-primary, #3b82f6);
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.widget-configurator__textarea {
  resize: vertical;
  min-height: 80px;
}

.widget-configurator__color {
  width: 100%;
  height: 36px;
  padding: 2px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  cursor: pointer;
}

.widget-configurator__checkbox {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
}

.widget-configurator__checkbox input {
  width: 16px;
  height: 16px;
  accent-color: var(--color-primary, #3b82f6);
}

.widget-configurator__checkbox span {
  font-size: 0.875rem;
  color: #374151;
}

.widget-configurator__group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.widget-configurator__button-group {
  display: flex;
  gap: 0.25rem;
}

.widget-configurator__btn {
  flex: 1;
  padding: 0.375rem 0.5rem;
  font-size: 0.75rem;
  font-weight: 500;
  color: #374151;
  background-color: white;
  border: 1px solid #d1d5db;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
}

.widget-configurator__btn:hover {
  background-color: #f3f4f6;
}

.widget-configurator__btn--active {
  color: white;
  background-color: var(--color-primary, #3b82f6);
  border-color: var(--color-primary, #3b82f6);
}

.widget-configurator__delete-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  width: 100%;
  padding: 0.625rem 1rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: #dc2626;
  background-color: white;
  border: 1px solid #fca5a5;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s;
}

.widget-configurator__delete-btn:hover {
  color: white;
  background-color: #dc2626;
  border-color: #dc2626;
}

.widget-configurator__delete-btn svg {
  width: 18px;
  height: 18px;
}
</style>
