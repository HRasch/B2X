/**
 * Widget Components Index
 * Exports all Phase 1 MVP widgets
 */

// Layout Widgets
export { default as GridWidget } from './GridWidget.vue'
export { default as SectionWidget } from './SectionWidget.vue'
export { default as ContainerWidget } from './ContainerWidget.vue'
export { default as SpacerWidget } from './SpacerWidget.vue'
export { default as DividerWidget } from './DividerWidget.vue'

// Content Widgets
export { default as TextWidget } from './TextWidget.vue'
export { default as ImageWidget } from './ImageWidget.vue'
export { default as ButtonWidget } from './ButtonWidget.vue'

// Widget type to component mapping
import GridWidget from './GridWidget.vue'
import SectionWidget from './SectionWidget.vue'
import ContainerWidget from './ContainerWidget.vue'
import SpacerWidget from './SpacerWidget.vue'
import DividerWidget from './DividerWidget.vue'
import TextWidget from './TextWidget.vue'
import ImageWidget from './ImageWidget.vue'
import ButtonWidget from './ButtonWidget.vue'

import type { Component } from 'vue'
import type { WidgetType } from '@/types/widgets'

export const widgetComponents: Record<WidgetType, Component> = {
  grid: GridWidget,
  section: SectionWidget,
  container: ContainerWidget,
  spacer: SpacerWidget,
  divider: DividerWidget,
  text: TextWidget,
  image: ImageWidget,
  button: ButtonWidget
}

/**
 * Get widget component by type
 */
export function getWidgetComponent(type: WidgetType): Component | undefined {
  return widgetComponents[type]
}
