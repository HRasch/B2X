
import type { DefineComponent, SlotsType } from 'vue'
type IslandComponent<T> = DefineComponent<{}, {refresh: () => Promise<void>}, {}, {}, {}, {}, {}, {}, {}, {}, {}, {}, SlotsType<{ fallback: { error: unknown } }>> & T

type HydrationStrategies = {
  hydrateOnVisible?: IntersectionObserverInit | true
  hydrateOnIdle?: number | true
  hydrateOnInteraction?: keyof HTMLElementEventMap | Array<keyof HTMLElementEventMap> | true
  hydrateOnMediaQuery?: string
  hydrateAfter?: number
  hydrateWhen?: boolean
  hydrateNever?: true
}
type LazyComponent<T> = DefineComponent<HydrationStrategies, {}, {}, {}, {}, {}, {}, { hydrated: () => void }> & T

interface _GlobalComponents {
  'AdminFab': typeof import("../../components/AdminFab.vue").default
  'B2BVatIdInput': typeof import("../../components/B2BVatIdInput.vue").default
  'Checkout': typeof import("../../components/Checkout.vue").default
  'CheckoutTermsStep': typeof import("../../components/CheckoutTermsStep.vue").default
  'ERPCustomerLookup': typeof import("../../components/ERP/CustomerLookup.vue").default
  'EditableText': typeof import("../../components/EditableText.vue").default
  'InvoiceDisplay': typeof import("../../components/InvoiceDisplay.vue").default
  'ProductPrice': typeof import("../../components/ProductPrice.vue").default
  'RegistrationCheck': typeof import("../../components/RegistrationCheck.vue").default
  'CmsPageLayout': typeof import("../../components/cms/CmsPageLayout.vue").default
  'CmsRegionRenderer': typeof import("../../components/cms/RegionRenderer.vue").default
  'CmsWidgetNotFound': typeof import("../../components/cms/WidgetNotFound.vue").default
  'CmsWidgetRenderer': typeof import("../../components/cms/WidgetRenderer.vue").default
  'CmsWidgetsHeroBanner': typeof import("../../components/cms/widgets/HeroBanner.vue").default
  'CmsWidgetsNewsletterSignup': typeof import("../../components/cms/widgets/NewsletterSignup.vue").default
  'CmsWidgetsProductGrid': typeof import("../../components/cms/widgets/ProductGrid.vue").default
  'CmsWidgetsTestimonials': typeof import("../../components/cms/widgets/Testimonials.vue").default
  'CommonErrorBoundary': typeof import("../../components/common/ErrorBoundary.vue").default
  'CommonLanguageSwitcher': typeof import("../../components/common/LanguageSwitcher.vue").default
  'CommonLoadingSpinner': typeof import("../../components/common/LoadingSpinner.vue").default
  'ShopProductCard': typeof import("../../components/shop/ProductCard.vue").default
  'ShopProductCardModern': typeof import("../../components/shop/ProductCardModern.vue").default
  'WidgetsCallToAction': typeof import("../../components/widgets/CallToAction.vue").default
  'WidgetsFeatureGrid': typeof import("../../components/widgets/FeatureGrid.vue").default
  'WidgetsHeroBanner': typeof import("../../components/widgets/HeroBanner.vue").default
  'WidgetsNewsletterSignup': typeof import("../../components/widgets/NewsletterSignup.vue").default
  'WidgetsProductGrid': typeof import("../../components/widgets/ProductGrid.vue").default
  'WidgetsTestimonials': typeof import("../../components/widgets/Testimonials.vue").default
  'WidgetsTextBlock': typeof import("../../components/widgets/TextBlock.vue").default
  'WidgetsVideo': typeof import("../../components/widgets/Video.vue").default
  'NuxtWelcome': typeof import("../../../node_modules/nuxt/dist/app/components/welcome.vue").default
  'NuxtLayout': typeof import("../../../node_modules/nuxt/dist/app/components/nuxt-layout").default
  'NuxtErrorBoundary': typeof import("../../../node_modules/nuxt/dist/app/components/nuxt-error-boundary.vue").default
  'ClientOnly': typeof import("../../../node_modules/nuxt/dist/app/components/client-only").default
  'DevOnly': typeof import("../../../node_modules/nuxt/dist/app/components/dev-only").default
  'ServerPlaceholder': typeof import("../../../node_modules/nuxt/dist/app/components/server-placeholder").default
  'NuxtLink': typeof import("../../../node_modules/nuxt/dist/app/components/nuxt-link").default
  'NuxtLoadingIndicator': typeof import("../../../node_modules/nuxt/dist/app/components/nuxt-loading-indicator").default
  'NuxtTime': typeof import("../../../node_modules/nuxt/dist/app/components/nuxt-time.vue").default
  'NuxtRouteAnnouncer': typeof import("../../../node_modules/nuxt/dist/app/components/nuxt-route-announcer").default
  'NuxtImg': typeof import("../../../node_modules/nuxt/dist/app/components/nuxt-stubs").NuxtImg
  'NuxtPicture': typeof import("../../../node_modules/nuxt/dist/app/components/nuxt-stubs").NuxtPicture
  'NuxtLinkLocale': typeof import("../../../node_modules/@nuxtjs/i18n/dist/runtime/components/NuxtLinkLocale").default
  'SwitchLocalePathLink': typeof import("../../../node_modules/@nuxtjs/i18n/dist/runtime/components/SwitchLocalePathLink").default
  'NuxtPage': typeof import("../../../node_modules/nuxt/dist/pages/runtime/page").default
  'NoScript': typeof import("../../../node_modules/nuxt/dist/head/runtime/components").NoScript
  'Link': typeof import("../../../node_modules/nuxt/dist/head/runtime/components").Link
  'Base': typeof import("../../../node_modules/nuxt/dist/head/runtime/components").Base
  'Title': typeof import("../../../node_modules/nuxt/dist/head/runtime/components").Title
  'Meta': typeof import("../../../node_modules/nuxt/dist/head/runtime/components").Meta
  'Style': typeof import("../../../node_modules/nuxt/dist/head/runtime/components").Style
  'Head': typeof import("../../../node_modules/nuxt/dist/head/runtime/components").Head
  'Html': typeof import("../../../node_modules/nuxt/dist/head/runtime/components").Html
  'Body': typeof import("../../../node_modules/nuxt/dist/head/runtime/components").Body
  'NuxtIsland': typeof import("../../../node_modules/nuxt/dist/app/components/nuxt-island").default
  'LazyAdminFab': LazyComponent<typeof import("../../components/AdminFab.vue").default>
  'LazyB2BVatIdInput': LazyComponent<typeof import("../../components/B2BVatIdInput.vue").default>
  'LazyCheckout': LazyComponent<typeof import("../../components/Checkout.vue").default>
  'LazyCheckoutTermsStep': LazyComponent<typeof import("../../components/CheckoutTermsStep.vue").default>
  'LazyERPCustomerLookup': LazyComponent<typeof import("../../components/ERP/CustomerLookup.vue").default>
  'LazyEditableText': LazyComponent<typeof import("../../components/EditableText.vue").default>
  'LazyInvoiceDisplay': LazyComponent<typeof import("../../components/InvoiceDisplay.vue").default>
  'LazyProductPrice': LazyComponent<typeof import("../../components/ProductPrice.vue").default>
  'LazyRegistrationCheck': LazyComponent<typeof import("../../components/RegistrationCheck.vue").default>
  'LazyCmsPageLayout': LazyComponent<typeof import("../../components/cms/CmsPageLayout.vue").default>
  'LazyCmsRegionRenderer': LazyComponent<typeof import("../../components/cms/RegionRenderer.vue").default>
  'LazyCmsWidgetNotFound': LazyComponent<typeof import("../../components/cms/WidgetNotFound.vue").default>
  'LazyCmsWidgetRenderer': LazyComponent<typeof import("../../components/cms/WidgetRenderer.vue").default>
  'LazyCmsWidgetsHeroBanner': LazyComponent<typeof import("../../components/cms/widgets/HeroBanner.vue").default>
  'LazyCmsWidgetsNewsletterSignup': LazyComponent<typeof import("../../components/cms/widgets/NewsletterSignup.vue").default>
  'LazyCmsWidgetsProductGrid': LazyComponent<typeof import("../../components/cms/widgets/ProductGrid.vue").default>
  'LazyCmsWidgetsTestimonials': LazyComponent<typeof import("../../components/cms/widgets/Testimonials.vue").default>
  'LazyCommonErrorBoundary': LazyComponent<typeof import("../../components/common/ErrorBoundary.vue").default>
  'LazyCommonLanguageSwitcher': LazyComponent<typeof import("../../components/common/LanguageSwitcher.vue").default>
  'LazyCommonLoadingSpinner': LazyComponent<typeof import("../../components/common/LoadingSpinner.vue").default>
  'LazyShopProductCard': LazyComponent<typeof import("../../components/shop/ProductCard.vue").default>
  'LazyShopProductCardModern': LazyComponent<typeof import("../../components/shop/ProductCardModern.vue").default>
  'LazyWidgetsCallToAction': LazyComponent<typeof import("../../components/widgets/CallToAction.vue").default>
  'LazyWidgetsFeatureGrid': LazyComponent<typeof import("../../components/widgets/FeatureGrid.vue").default>
  'LazyWidgetsHeroBanner': LazyComponent<typeof import("../../components/widgets/HeroBanner.vue").default>
  'LazyWidgetsNewsletterSignup': LazyComponent<typeof import("../../components/widgets/NewsletterSignup.vue").default>
  'LazyWidgetsProductGrid': LazyComponent<typeof import("../../components/widgets/ProductGrid.vue").default>
  'LazyWidgetsTestimonials': LazyComponent<typeof import("../../components/widgets/Testimonials.vue").default>
  'LazyWidgetsTextBlock': LazyComponent<typeof import("../../components/widgets/TextBlock.vue").default>
  'LazyWidgetsVideo': LazyComponent<typeof import("../../components/widgets/Video.vue").default>
  'LazyNuxtWelcome': LazyComponent<typeof import("../../../node_modules/nuxt/dist/app/components/welcome.vue").default>
  'LazyNuxtLayout': LazyComponent<typeof import("../../../node_modules/nuxt/dist/app/components/nuxt-layout").default>
  'LazyNuxtErrorBoundary': LazyComponent<typeof import("../../../node_modules/nuxt/dist/app/components/nuxt-error-boundary.vue").default>
  'LazyClientOnly': LazyComponent<typeof import("../../../node_modules/nuxt/dist/app/components/client-only").default>
  'LazyDevOnly': LazyComponent<typeof import("../../../node_modules/nuxt/dist/app/components/dev-only").default>
  'LazyServerPlaceholder': LazyComponent<typeof import("../../../node_modules/nuxt/dist/app/components/server-placeholder").default>
  'LazyNuxtLink': LazyComponent<typeof import("../../../node_modules/nuxt/dist/app/components/nuxt-link").default>
  'LazyNuxtLoadingIndicator': LazyComponent<typeof import("../../../node_modules/nuxt/dist/app/components/nuxt-loading-indicator").default>
  'LazyNuxtTime': LazyComponent<typeof import("../../../node_modules/nuxt/dist/app/components/nuxt-time.vue").default>
  'LazyNuxtRouteAnnouncer': LazyComponent<typeof import("../../../node_modules/nuxt/dist/app/components/nuxt-route-announcer").default>
  'LazyNuxtImg': LazyComponent<typeof import("../../../node_modules/nuxt/dist/app/components/nuxt-stubs").NuxtImg>
  'LazyNuxtPicture': LazyComponent<typeof import("../../../node_modules/nuxt/dist/app/components/nuxt-stubs").NuxtPicture>
  'LazyNuxtLinkLocale': LazyComponent<typeof import("../../../node_modules/@nuxtjs/i18n/dist/runtime/components/NuxtLinkLocale").default>
  'LazySwitchLocalePathLink': LazyComponent<typeof import("../../../node_modules/@nuxtjs/i18n/dist/runtime/components/SwitchLocalePathLink").default>
  'LazyNuxtPage': LazyComponent<typeof import("../../../node_modules/nuxt/dist/pages/runtime/page").default>
  'LazyNoScript': LazyComponent<typeof import("../../../node_modules/nuxt/dist/head/runtime/components").NoScript>
  'LazyLink': LazyComponent<typeof import("../../../node_modules/nuxt/dist/head/runtime/components").Link>
  'LazyBase': LazyComponent<typeof import("../../../node_modules/nuxt/dist/head/runtime/components").Base>
  'LazyTitle': LazyComponent<typeof import("../../../node_modules/nuxt/dist/head/runtime/components").Title>
  'LazyMeta': LazyComponent<typeof import("../../../node_modules/nuxt/dist/head/runtime/components").Meta>
  'LazyStyle': LazyComponent<typeof import("../../../node_modules/nuxt/dist/head/runtime/components").Style>
  'LazyHead': LazyComponent<typeof import("../../../node_modules/nuxt/dist/head/runtime/components").Head>
  'LazyHtml': LazyComponent<typeof import("../../../node_modules/nuxt/dist/head/runtime/components").Html>
  'LazyBody': LazyComponent<typeof import("../../../node_modules/nuxt/dist/head/runtime/components").Body>
  'LazyNuxtIsland': LazyComponent<typeof import("../../../node_modules/nuxt/dist/app/components/nuxt-island").default>
}

declare module 'vue' {
  export interface GlobalComponents extends _GlobalComponents { }
}

export {}
