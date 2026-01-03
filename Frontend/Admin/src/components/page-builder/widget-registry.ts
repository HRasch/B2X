/**
 * Widget Registry - Defines all available widgets and their defaults
 */

import type { WidgetRegistry, WidgetType, WidgetDefinition } from '@/types/widgets'

export const widgetRegistry: WidgetRegistry = {
  // ============================================================================
  // Layout Widgets
  // ============================================================================
  
  grid: {
    type: 'grid',
    name: 'Grid',
    description: 'Flexibles Grid-Layout mit 1-12 Spalten',
    category: 'layout',
    icon: 'grid-3x3',
    defaultConfig: {
      columns: { mobile: 1, tablet: 2, desktop: 3 },
      gap: { mobile: '1rem', tablet: '1.5rem', desktop: '2rem' },
      alignItems: 'stretch',
      justifyContent: 'start',
      children: []
    },
    allowedChildren: ['text', 'image', 'button', 'container', 'spacer', 'divider']
  },

  section: {
    type: 'section',
    name: 'Section',
    description: 'Vollbreite Sektion mit Hintergrund',
    category: 'layout',
    icon: 'rectangle-horizontal',
    defaultConfig: {
      fullWidth: true,
      backgroundColor: undefined,
      backgroundImage: undefined,
      backgroundOverlay: undefined,
      paddingTop: { mobile: '2rem', tablet: '3rem', desktop: '4rem' },
      paddingBottom: { mobile: '2rem', tablet: '3rem', desktop: '4rem' },
      children: []
    },
    allowedChildren: ['grid', 'container', 'text', 'image', 'button', 'spacer', 'divider']
  },

  container: {
    type: 'container',
    name: 'Container',
    description: 'Zentrierter Container mit max-width',
    category: 'layout',
    icon: 'square',
    defaultConfig: {
      maxWidth: 'xl',
      centered: true,
      children: []
    },
    allowedChildren: ['grid', 'text', 'image', 'button', 'spacer', 'divider']
  },

  spacer: {
    type: 'spacer',
    name: 'Spacer',
    description: 'Vertikaler Abstand',
    category: 'layout',
    icon: 'move-vertical',
    defaultConfig: {
      height: { mobile: '1rem', tablet: '1.5rem', desktop: '2rem' }
    }
  },

  divider: {
    type: 'divider',
    name: 'Divider',
    description: 'Horizontaler Trenner',
    category: 'layout',
    icon: 'minus',
    defaultConfig: {
      style: 'solid',
      thickness: '1px',
      color: '#e5e7eb',
      width: '100%',
      alignment: 'center'
    }
  },

  // ============================================================================
  // Content Widgets
  // ============================================================================

  text: {
    type: 'text',
    name: 'Text',
    description: 'Rich-Text Inhalt',
    category: 'content',
    icon: 'type',
    defaultConfig: {
      content: '<p>Text hier eingeben...</p>',
      alignment: { mobile: 'left', tablet: 'left', desktop: 'left' },
      fontSize: { mobile: '1rem', tablet: '1rem', desktop: '1rem' },
      fontWeight: 'normal',
      lineHeight: '1.6',
      textColor: undefined,
      maxWidth: undefined
    }
  },

  image: {
    type: 'image',
    name: 'Bild',
    description: 'Einzelbild mit optionalem Link',
    category: 'content',
    icon: 'image',
    defaultConfig: {
      src: '',
      alt: '',
      width: { mobile: '100%', tablet: '100%', desktop: '100%' },
      height: undefined,
      objectFit: 'cover',
      objectPosition: 'center',
      lazyLoad: true,
      link: undefined,
      linkTarget: '_self',
      caption: undefined,
      rounded: 'none'
    }
  },

  button: {
    type: 'button',
    name: 'Button',
    description: 'Call-to-Action Button',
    category: 'content',
    icon: 'mouse-pointer-click',
    defaultConfig: {
      text: 'Button',
      link: '#',
      linkTarget: '_self',
      variant: 'primary',
      size: 'md',
      fullWidth: { mobile: false, tablet: false, desktop: false },
      icon: undefined,
      iconPosition: 'left',
      alignment: 'left'
    }
  }
}

/**
 * Get widget definition by type
 */
export function getWidgetDefinition(type: WidgetType): WidgetDefinition | undefined {
  return widgetRegistry[type]
}

/**
 * Get all widgets by category
 */
export function getWidgetsByCategory(category: string): WidgetDefinition[] {
  return Object.values(widgetRegistry).filter(w => w.category === category)
}

/**
 * Get all widget categories
 */
export function getWidgetCategories(): string[] {
  const categories = new Set(Object.values(widgetRegistry).map(w => w.category))
  return Array.from(categories)
}

/**
 * Check if a widget type can be nested inside another
 */
export function canNestWidget(parentType: WidgetType, childType: WidgetType): boolean {
  const parent = widgetRegistry[parentType]
  if (!parent?.allowedChildren) return false
  return parent.allowedChildren.includes(childType)
}

/**
 * Create a new widget instance with default config
 */
export function createWidget(type: WidgetType, overrides?: Partial<Record<string, unknown>>): {
  id: string
  type: WidgetType
  version: number
  config: Record<string, unknown>
} {
  const definition = widgetRegistry[type]
  if (!definition) {
    throw new Error(`Unknown widget type: ${type}`)
  }

  return {
    id: crypto.randomUUID(),
    type,
    version: 1,
    config: {
      ...definition.defaultConfig,
      ...overrides
    }
  }
}
