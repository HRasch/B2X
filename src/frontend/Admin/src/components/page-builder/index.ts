/**
 * Page Builder Components Export
 */
export { default as PageBuilder } from './PageBuilder.vue';
export { default as WidgetRenderer } from './WidgetRenderer.vue';
export { default as WidgetPalette } from './WidgetPalette.vue';
export { default as WidgetConfigurator } from './WidgetConfigurator.vue';

// Re-export widget registry utilities
export {
  widgetRegistry,
  createWidget,
  getWidgetDefinition,
  getWidgetsByCategory,
  canNestWidget,
} from './widget-registry';

// Re-export widget components
export { widgetComponents, getWidgetComponent } from './widgets';
