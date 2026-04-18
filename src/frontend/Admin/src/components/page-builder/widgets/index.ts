/**
 * Widget Components Index
 * Exports all Page Builder widgets
 */

// Layout Widgets
export { default as GridWidget } from './GridWidget.vue';
export { default as SectionWidget } from './SectionWidget.vue';
export { default as ContainerWidget } from './ContainerWidget.vue';
export { default as SpacerWidget } from './SpacerWidget.vue';
export { default as DividerWidget } from './DividerWidget.vue';

// Content Widgets
export { default as TextWidget } from './TextWidget.vue';
export { default as ImageWidget } from './ImageWidget.vue';
export { default as ButtonWidget } from './ButtonWidget.vue';

// Account Widgets (Phase 3)
export { default as AccountDashboardWidget } from './AccountDashboardWidget.vue';
export { default as OrderHistoryWidget } from './OrderHistoryWidget.vue';
export { default as AddressBookWidget } from './AddressBookWidget.vue';
export { default as ProfileFormWidget } from './ProfileFormWidget.vue';
export { default as WishlistWidget } from './WishlistWidget.vue';

// E-Commerce Widgets (Phase 4)
export { default as ProductCardWidget } from './ProductCardWidget.vue';
export { default as ProductGridWidget } from './ProductGridWidget.vue';
export { default as CategoryNavWidget } from './CategoryNavWidget.vue';
export { default as SearchBoxWidget } from './SearchBoxWidget.vue';
export { default as CartSummaryWidget } from './CartSummaryWidget.vue';
export { default as MiniCartWidget } from './MiniCartWidget.vue';
export { default as BreadcrumbWidget } from './BreadcrumbWidget.vue';

// Widget type to component mapping
import GridWidget from './GridWidget.vue';
import SectionWidget from './SectionWidget.vue';
import ContainerWidget from './ContainerWidget.vue';
import SpacerWidget from './SpacerWidget.vue';
import DividerWidget from './DividerWidget.vue';
import TextWidget from './TextWidget.vue';
import ImageWidget from './ImageWidget.vue';
import ButtonWidget from './ButtonWidget.vue';
import AccountDashboardWidget from './AccountDashboardWidget.vue';
import OrderHistoryWidget from './OrderHistoryWidget.vue';
import AddressBookWidget from './AddressBookWidget.vue';
import ProfileFormWidget from './ProfileFormWidget.vue';
import WishlistWidget from './WishlistWidget.vue';
import ProductCardWidget from './ProductCardWidget.vue';
import ProductGridWidget from './ProductGridWidget.vue';
import CategoryNavWidget from './CategoryNavWidget.vue';
import SearchBoxWidget from './SearchBoxWidget.vue';
import CartSummaryWidget from './CartSummaryWidget.vue';
import MiniCartWidget from './MiniCartWidget.vue';
import BreadcrumbWidget from './BreadcrumbWidget.vue';

import type { Component } from 'vue';
import type { WidgetType } from '@/types/widgets';

export const widgetComponents: Record<WidgetType, Component> = {
  grid: GridWidget,
  section: SectionWidget,
  container: ContainerWidget,
  spacer: SpacerWidget,
  divider: DividerWidget,
  text: TextWidget,
  image: ImageWidget,
  button: ButtonWidget,
  'account-dashboard': AccountDashboardWidget,
  'order-history': OrderHistoryWidget,
  'address-book': AddressBookWidget,
  'profile-form': ProfileFormWidget,
  wishlist: WishlistWidget,
  'product-card': ProductCardWidget,
  'product-grid': ProductGridWidget,
  'category-nav': CategoryNavWidget,
  'search-box': SearchBoxWidget,
  'cart-summary': CartSummaryWidget,
  'mini-cart': MiniCartWidget,
  breadcrumb: BreadcrumbWidget,
};

/**
 * Get widget component by type
 */
export function getWidgetComponent(type: WidgetType): Component | undefined {
  return widgetComponents[type];
}
