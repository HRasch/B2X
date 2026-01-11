/**
 * Widget Registry - Defines all available widgets and their defaults
 */

import type { WidgetRegistry, WidgetType, WidgetDefinition } from '@/types/widgets';

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
      children: [],
    },
    allowedChildren: ['text', 'image', 'button', 'container', 'spacer', 'divider'],
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
      children: [],
    },
    allowedChildren: ['grid', 'container', 'text', 'image', 'button', 'spacer', 'divider'],
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
      children: [],
    },
    allowedChildren: ['grid', 'text', 'image', 'button', 'spacer', 'divider'],
  },

  spacer: {
    type: 'spacer',
    name: 'Spacer',
    description: 'Vertikaler Abstand',
    category: 'layout',
    icon: 'move-vertical',
    defaultConfig: {
      height: { mobile: '1rem', tablet: '1.5rem', desktop: '2rem' },
    },
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
      alignment: 'center',
    },
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
      maxWidth: undefined,
    },
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
      rounded: 'none',
    },
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
      alignment: 'left',
    },
  },

  // ============================================================================
  // Account Widgets (Phase 3)
  // ============================================================================

  'account-dashboard': {
    type: 'account-dashboard',
    name: 'Account Dashboard',
    description: 'Übersicht für den Kundenbereich',
    category: 'account',
    icon: 'layout-dashboard',
    defaultConfig: {
      showRecentOrders: true,
      recentOrdersCount: 3,
      showQuickLinks: true,
      quickLinks: [
        { label: 'Bestellungen', href: '/account/orders', icon: 'package' },
        { label: 'Adressen', href: '/account/addresses', icon: 'map-pin' },
        { label: 'Profil', href: '/account/profile', icon: 'user' },
        { label: 'Merkzettel', href: '/account/wishlist', icon: 'heart' },
      ],
      showWelcomeMessage: true,
      layout: 'grid',
    },
    pageTypes: ['account'],
  },

  'order-history': {
    type: 'order-history',
    name: 'Bestellhistorie',
    description: 'Liste aller Kundenbestellungen',
    category: 'account',
    icon: 'clipboard-list',
    defaultConfig: {
      ordersPerPage: 10,
      showFilters: true,
      showSearch: true,
      showStatus: true,
      showTracking: true,
      columns: ['orderNumber', 'date', 'status', 'total', 'actions'],
      defaultSort: 'date-desc',
    },
    pageTypes: ['account'],
  },

  'address-book': {
    type: 'address-book',
    name: 'Adressbuch',
    description: 'Verwaltung von Liefer- und Rechnungsadressen',
    category: 'account',
    icon: 'map-pin',
    defaultConfig: {
      maxAddresses: 10,
      showDefaultBadge: true,
      allowAddNew: true,
      allowEdit: true,
      allowDelete: true,
      layout: 'grid',
      addressTypes: ['billing', 'shipping'],
    },
    pageTypes: ['account'],
  },

  'profile-form': {
    type: 'profile-form',
    name: 'Profil-Formular',
    description: 'Bearbeitung der Kundendaten',
    category: 'account',
    icon: 'user-circle',
    defaultConfig: {
      fields: ['name', 'email', 'phone', 'company'],
      allowPasswordChange: true,
      allowEmailChange: false,
      showAvatar: true,
      allowAvatarUpload: true,
      layout: 'two-column',
    },
    pageTypes: ['account'],
  },

  wishlist: {
    type: 'wishlist',
    name: 'Merkzettel',
    description: 'Gespeicherte Produkte des Kunden',
    category: 'account',
    icon: 'heart',
    defaultConfig: {
      itemsPerPage: 12,
      showPrice: true,
      showAddToCart: true,
      showRemove: true,
      showShare: false,
      layout: 'grid',
      gridColumns: { mobile: 2, tablet: 3, desktop: 4 },
      emptyStateMessage: 'Dein Merkzettel ist leer.',
    },
    pageTypes: ['account'],
  },

  // ============================================================================
  // E-Commerce Widgets (Phase 4)
  // ============================================================================

  'product-card': {
    type: 'product-card',
    name: 'Produkt-Karte',
    description: 'Einzelne Produktdarstellung',
    category: 'ecommerce',
    icon: 'shopping-bag',
    defaultConfig: {
      productId: undefined,
      showImage: true,
      showTitle: true,
      showPrice: true,
      showDescription: false,
      showAddToCart: true,
      showWishlistButton: true,
      showBadges: true,
      imageAspectRatio: '1:1',
      titleLines: 2,
      descriptionLines: 2,
      variant: 'default',
    },
  },

  'product-grid': {
    type: 'product-grid',
    name: 'Produkt-Raster',
    description: 'Raster mit mehreren Produkten',
    category: 'ecommerce',
    icon: 'layout-grid',
    defaultConfig: {
      title: undefined,
      source: 'category',
      categoryId: undefined,
      productIds: [],
      columns: { mobile: 2, tablet: 3, desktop: 4 },
      rows: 3,
      showPagination: true,
      showFilters: false,
      showSorting: true,
      cardVariant: 'default',
      gap: { mobile: '1rem', tablet: '1.5rem', desktop: '2rem' },
      emptyStateMessage: 'Keine Produkte gefunden.',
    },
  },

  'category-nav': {
    type: 'category-nav',
    name: 'Kategorie-Navigation',
    description: 'Kategoriebaum oder Menü',
    category: 'ecommerce',
    icon: 'folder-tree',
    defaultConfig: {
      title: undefined,
      rootCategoryId: undefined,
      depth: 2,
      layout: 'vertical',
      showProductCount: true,
      showImages: false,
      expandable: true,
      highlightActive: true,
      maxItems: undefined,
    },
  },

  'search-box': {
    type: 'search-box',
    name: 'Suchfeld',
    description: 'Produktsuche mit Vorschlägen',
    category: 'ecommerce',
    icon: 'search',
    defaultConfig: {
      placeholder: 'Produkte suchen...',
      showSuggestions: true,
      suggestionsCount: 5,
      showCategories: true,
      showRecentSearches: true,
      recentSearchesCount: 3,
      minChars: 2,
      size: 'md',
      variant: 'default',
      showSearchButton: true,
      searchButtonText: 'Suchen',
    },
  },

  'cart-summary': {
    type: 'cart-summary',
    name: 'Warenkorb-Übersicht',
    description: 'Zusammenfassung des Warenkorbs',
    category: 'ecommerce',
    icon: 'receipt',
    defaultConfig: {
      title: 'Zusammenfassung',
      showItemCount: true,
      showSubtotal: true,
      showShipping: true,
      showTax: true,
      showTotal: true,
      showCheckoutButton: true,
      checkoutButtonText: 'Zur Kasse',
      showContinueShopping: true,
      continueShoppingText: 'Weiter einkaufen',
      continueShoppingUrl: '/',
      showPromoCode: true,
      emptyCartMessage: 'Dein Warenkorb ist leer.',
    },
    pageTypes: ['cart'],
  },

  'mini-cart': {
    type: 'mini-cart',
    name: 'Mini-Warenkorb',
    description: 'Kompakter Warenkorb für Header',
    category: 'ecommerce',
    icon: 'shopping-cart',
    defaultConfig: {
      showItemCount: true,
      showTotal: true,
      maxItemsPreview: 3,
      showCheckoutButton: true,
      checkoutButtonText: 'Zur Kasse',
      showViewCartButton: true,
      viewCartButtonText: 'Warenkorb ansehen',
      position: 'dropdown',
      triggerIcon: 'cart',
      emptyCartMessage: 'Dein Warenkorb ist leer.',
    },
  },

  breadcrumb: {
    type: 'breadcrumb',
    name: 'Breadcrumb',
    description: 'Brotkrumen-Navigation',
    category: 'ecommerce',
    icon: 'chevrons-right',
    defaultConfig: {
      showHome: true,
      homeLabel: 'Start',
      homeUrl: '/',
      separator: 'chevron',
      maxItems: 5,
      truncateMiddle: true,
      showCurrentPage: true,
      currentPageClickable: false,
    },
  },
};

/**
 * Get widget definition by type
 */
export function getWidgetDefinition(type: WidgetType): WidgetDefinition | undefined {
  return widgetRegistry[type];
}

/**
 * Get all widgets by category
 */
export function getWidgetsByCategory(category: string): WidgetDefinition[] {
  return Object.values(widgetRegistry).filter(w => w.category === category);
}

/**
 * Get all widget categories
 */
export function getWidgetCategories(): string[] {
  const categories = new Set(Object.values(widgetRegistry).map(w => w.category));
  return Array.from(categories);
}

/**
 * Check if a widget type can be nested inside another
 */
export function canNestWidget(parentType: WidgetType, childType: WidgetType): boolean {
  const parent = widgetRegistry[parentType];
  if (!parent?.allowedChildren) return false;
  return parent.allowedChildren.includes(childType);
}

/**
 * Create a new widget instance with default config
 */
export function createWidget(
  type: WidgetType,
  overrides?: Partial<Record<string, unknown>>
): {
  id: string;
  type: WidgetType;
  version: number;
  config: Record<string, unknown>;
} {
  const definition = widgetRegistry[type];
  if (!definition) {
    throw new Error(`Unknown widget type: ${type}`);
  }

  return {
    id: crypto.randomUUID(),
    type,
    version: 1,
    config: {
      ...definition.defaultConfig,
      ...overrides,
    },
  };
}
