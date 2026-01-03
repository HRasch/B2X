/**
 * Widget System Types for Page Builder
 * Phase 1: MVP Widgets (Layout & Content)
 */

// ============================================================================
// Base Widget Types
// ============================================================================

export type WidgetCategory =
  | 'layout' // Grid, Section, Container, Spacer, Divider
  | 'content' // Text, Image, Button
  | 'navigation' // Breadcrumb, StickyNav, Tabs
  | 'ecommerce' // ProductGrid, Cart, Filter
  | 'account' // Profile, Orders, Addresses
  | 'analytics' // KPI, Charts, Stats
  | 'integration'; // Oxomi, Nexmart, CMS

export type WidgetSize = 'xs' | 'sm' | 'md' | 'lg' | 'xl' | 'full';

export type ResponsiveValue<T> = {
  mobile?: T;
  tablet?: T;
  desktop?: T;
};

export type Alignment = 'left' | 'center' | 'right' | 'justify';
export type VerticalAlignment = 'top' | 'middle' | 'bottom' | 'stretch';

// ============================================================================
// Widget Configuration Base
// ============================================================================

export interface WidgetBase {
  id: string;
  type: WidgetType;
  version: number;
  config: WidgetConfig;
  style?: WidgetStyle;
  visibility?: WidgetVisibility;
  metadata?: WidgetMetadata;
}

export interface WidgetConfig {
  [key: string]: unknown;
}

export interface WidgetStyle {
  // Spacing
  margin?: ResponsiveValue<string>;
  padding?: ResponsiveValue<string>;

  // Dimensions
  width?: ResponsiveValue<string>;
  height?: ResponsiveValue<string>;
  minHeight?: ResponsiveValue<string>;
  maxWidth?: ResponsiveValue<string>;

  // Background
  backgroundColor?: string;
  backgroundImage?: string;
  backgroundPosition?: string;
  backgroundSize?: 'cover' | 'contain' | 'auto';

  // Border
  borderRadius?: string;
  borderWidth?: string;
  borderColor?: string;
  borderStyle?: 'solid' | 'dashed' | 'dotted' | 'none';

  // Shadow
  boxShadow?: 'none' | 'sm' | 'md' | 'lg' | 'xl';

  // Custom CSS class
  customClass?: string;
}

export interface WidgetVisibility {
  showOnMobile?: boolean;
  showOnTablet?: boolean;
  showOnDesktop?: boolean;
  showWhenLoggedIn?: boolean;
  showWhenLoggedOut?: boolean;
  showForRoles?: string[];
}

export interface WidgetMetadata {
  createdAt: string;
  updatedAt: string;
  createdBy: string;
  label?: string; // Admin-friendly name
  notes?: string; // Internal notes
}

// ============================================================================
// Widget Type Registry (Phase 1 MVP)
// ============================================================================

export type WidgetType =
  // Layout Widgets
  | 'grid'
  | 'section'
  | 'container'
  | 'spacer'
  | 'divider'
  // Content Widgets
  | 'text'
  | 'image'
  | 'button'
  // Account Widgets (Phase 3)
  | 'account-dashboard'
  | 'order-history'
  | 'address-book'
  | 'profile-form'
  | 'wishlist';

// ============================================================================
// Phase 1: Layout Widget Configs
// ============================================================================

export interface GridWidgetConfig extends WidgetConfig {
  columns: ResponsiveValue<number>; // 1-12
  gap: ResponsiveValue<string>;
  alignItems?: VerticalAlignment;
  justifyContent?: 'start' | 'center' | 'end' | 'between' | 'around' | 'evenly';
  children: WidgetBase[];
}

export interface GridWidget extends WidgetBase {
  type: 'grid';
  config: GridWidgetConfig;
}

export interface SectionWidgetConfig extends WidgetConfig {
  fullWidth: boolean;
  backgroundColor?: string;
  backgroundImage?: string;
  backgroundOverlay?: string;
  paddingTop: ResponsiveValue<string>;
  paddingBottom: ResponsiveValue<string>;
  children: WidgetBase[];
}

export interface SectionWidget extends WidgetBase {
  type: 'section';
  config: SectionWidgetConfig;
}

export interface ContainerWidgetConfig extends WidgetConfig {
  maxWidth: 'sm' | 'md' | 'lg' | 'xl' | '2xl' | 'full';
  centered: boolean;
  children: WidgetBase[];
}

export interface ContainerWidget extends WidgetBase {
  type: 'container';
  config: ContainerWidgetConfig;
}

export interface SpacerWidgetConfig extends WidgetConfig {
  height: ResponsiveValue<string>;
}

export interface SpacerWidget extends WidgetBase {
  type: 'spacer';
  config: SpacerWidgetConfig;
}

export interface DividerWidgetConfig extends WidgetConfig {
  style: 'solid' | 'dashed' | 'dotted';
  thickness: string;
  color: string;
  width: string; // percentage or fixed
  alignment: Alignment;
}

export interface DividerWidget extends WidgetBase {
  type: 'divider';
  config: DividerWidgetConfig;
}

// ============================================================================
// Phase 1: Content Widget Configs
// ============================================================================

export interface TextWidgetConfig extends WidgetConfig {
  content: string; // HTML content
  alignment: ResponsiveValue<Alignment>;
  fontSize: ResponsiveValue<string>;
  fontWeight?: 'normal' | 'medium' | 'semibold' | 'bold';
  lineHeight?: string;
  textColor?: string;
  maxWidth?: string;
}

export interface TextWidget extends WidgetBase {
  type: 'text';
  config: TextWidgetConfig;
}

export interface ImageWidgetConfig extends WidgetConfig {
  src: string;
  alt: string;
  width?: ResponsiveValue<string>;
  height?: ResponsiveValue<string>;
  objectFit: 'cover' | 'contain' | 'fill' | 'none';
  objectPosition?: string;
  lazyLoad: boolean;
  link?: string;
  linkTarget?: '_self' | '_blank';
  caption?: string;
  rounded?: 'none' | 'sm' | 'md' | 'lg' | 'full';
}

export interface ImageWidget extends WidgetBase {
  type: 'image';
  config: ImageWidgetConfig;
}

export interface ButtonWidgetConfig extends WidgetConfig {
  text: string;
  link: string;
  linkTarget: '_self' | '_blank';
  variant: 'primary' | 'secondary' | 'outline' | 'ghost' | 'link';
  size: 'sm' | 'md' | 'lg';
  fullWidth: ResponsiveValue<boolean>;
  icon?: string;
  iconPosition?: 'left' | 'right';
  alignment: Alignment;
}

export interface ButtonWidget extends WidgetBase {
  type: 'button';
  config: ButtonWidgetConfig;
}

// ============================================================================
// Phase 3: Account Widget Configs
// ============================================================================

export interface AccountDashboardWidgetConfig extends WidgetConfig {
  showRecentOrders: boolean;
  recentOrdersCount: number;
  showQuickLinks: boolean;
  quickLinks: Array<{ label: string; href: string; icon?: string }>;
  showWelcomeMessage: boolean;
  layout: 'grid' | 'list';
}

export interface AccountDashboardWidget extends WidgetBase {
  type: 'account-dashboard';
  config: AccountDashboardWidgetConfig;
}

export interface OrderHistoryWidgetConfig extends WidgetConfig {
  ordersPerPage: number;
  showFilters: boolean;
  showSearch: boolean;
  showStatus: boolean;
  showTracking: boolean;
  columns: Array<'orderNumber' | 'date' | 'status' | 'total' | 'items' | 'actions'>;
  defaultSort: 'date-desc' | 'date-asc' | 'status' | 'total';
}

export interface OrderHistoryWidget extends WidgetBase {
  type: 'order-history';
  config: OrderHistoryWidgetConfig;
}

export interface AddressBookWidgetConfig extends WidgetConfig {
  maxAddresses: number;
  showDefaultBadge: boolean;
  allowAddNew: boolean;
  allowEdit: boolean;
  allowDelete: boolean;
  layout: 'grid' | 'list';
  addressTypes: Array<'billing' | 'shipping' | 'both'>;
}

export interface AddressBookWidget extends WidgetBase {
  type: 'address-book';
  config: AddressBookWidgetConfig;
}

export interface ProfileFormWidgetConfig extends WidgetConfig {
  fields: Array<'name' | 'email' | 'phone' | 'company' | 'taxId' | 'birthday'>;
  allowPasswordChange: boolean;
  allowEmailChange: boolean;
  showAvatar: boolean;
  allowAvatarUpload: boolean;
  layout: 'single-column' | 'two-column';
}

export interface ProfileFormWidget extends WidgetBase {
  type: 'profile-form';
  config: ProfileFormWidgetConfig;
}

export interface WishlistWidgetConfig extends WidgetConfig {
  itemsPerPage: number;
  showPrice: boolean;
  showAddToCart: boolean;
  showRemove: boolean;
  showShare: boolean;
  layout: 'grid' | 'list';
  gridColumns: ResponsiveValue<number>;
  emptyStateMessage: string;
}

export interface WishlistWidget extends WidgetBase {
  type: 'wishlist';
  config: WishlistWidgetConfig;
}

// ============================================================================
// Widget Registry & Factory Types
// ============================================================================

export interface WidgetDefinition {
  type: WidgetType;
  name: string;
  description: string;
  category: WidgetCategory;
  icon: string;
  defaultConfig: WidgetConfig;
  defaultStyle?: WidgetStyle;
  allowedParents?: WidgetType[]; // Which widgets can contain this
  allowedChildren?: WidgetType[]; // Which widgets can be nested
  pageTypes?: string[]; // Restrict to specific page types
}

export type WidgetRegistry = Record<WidgetType, WidgetDefinition>;

// ============================================================================
// Page Builder State Types
// ============================================================================

export interface PageBuilderState {
  page: PageData;
  selectedWidgetId: string | null;
  hoveredWidgetId: string | null;
  clipboard: WidgetBase | null;
  history: PageData[];
  historyIndex: number;
  isDirty: boolean;
  isPreviewMode: boolean;
  previewDevice: 'mobile' | 'tablet' | 'desktop';
}

export interface PageData {
  id: string;
  title: string;
  slug: string;
  template?: string;
  widgets: WidgetBase[];
  seo: PageSEO;
  status: 'draft' | 'published' | 'scheduled';
  publishedAt?: string;
  scheduledAt?: string;
  version: number;
  createdAt: string;
  updatedAt: string;
}

export interface PageSEO {
  title?: string;
  description?: string;
  keywords?: string[];
  ogTitle?: string;
  ogDescription?: string;
  ogImage?: string;
  noIndex?: boolean;
  noFollow?: boolean;
  canonicalUrl?: string;
}

// ============================================================================
// Widget Events
// ============================================================================

export type WidgetEvent =
  | { type: 'SELECT'; widgetId: string }
  | { type: 'DESELECT' }
  | { type: 'ADD'; widget: WidgetBase; parentId?: string; index?: number }
  | { type: 'REMOVE'; widgetId: string }
  | { type: 'UPDATE'; widgetId: string; config: Partial<WidgetConfig> }
  | { type: 'UPDATE_STYLE'; widgetId: string; style: Partial<WidgetStyle> }
  | { type: 'MOVE'; widgetId: string; targetParentId?: string; targetIndex: number }
  | { type: 'COPY'; widgetId: string }
  | { type: 'PASTE'; parentId?: string; index?: number }
  | { type: 'DUPLICATE'; widgetId: string }
  | { type: 'UNDO' }
  | { type: 'REDO' };
