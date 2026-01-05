
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


export const AdminFab: typeof import("../components/AdminFab.vue").default
export const B2BVatIdInput: typeof import("../components/B2BVatIdInput.vue").default
export const Checkout: typeof import("../components/Checkout.vue").default
export const CheckoutTermsStep: typeof import("../components/CheckoutTermsStep.vue").default
export const ERPCustomerLookup: typeof import("../components/ERP/CustomerLookup.vue").default
export const EditableText: typeof import("../components/EditableText.vue").default
export const InvoiceDisplay: typeof import("../components/InvoiceDisplay.vue").default
export const ProductPrice: typeof import("../components/ProductPrice.vue").default
export const RegistrationCheck: typeof import("../components/RegistrationCheck.vue").default
export const CmsPageLayout: typeof import("../components/cms/CmsPageLayout.vue").default
export const CmsRegionRenderer: typeof import("../components/cms/RegionRenderer.vue").default
export const CmsWidgetNotFound: typeof import("../components/cms/WidgetNotFound.vue").default
export const CmsWidgetRenderer: typeof import("../components/cms/WidgetRenderer.vue").default
export const CmsWidgetsHeroBanner: typeof import("../components/cms/widgets/HeroBanner.vue").default
export const CmsWidgetsNewsletterSignup: typeof import("../components/cms/widgets/NewsletterSignup.vue").default
export const CmsWidgetsProductGrid: typeof import("../components/cms/widgets/ProductGrid.vue").default
export const CmsWidgetsTestimonials: typeof import("../components/cms/widgets/Testimonials.vue").default
export const CommonErrorBoundary: typeof import("../components/common/ErrorBoundary.vue").default
export const CommonLanguageSwitcher: typeof import("../components/common/LanguageSwitcher.vue").default
export const CommonLoadingSpinner: typeof import("../components/common/LoadingSpinner.vue").default
export const ShopProductCard: typeof import("../components/shop/ProductCard.vue").default
export const ShopProductCardModern: typeof import("../components/shop/ProductCardModern.vue").default
export const WidgetsCallToAction: typeof import("../components/widgets/CallToAction.vue").default
export const WidgetsFeatureGrid: typeof import("../components/widgets/FeatureGrid.vue").default
export const WidgetsHeroBanner: typeof import("../components/widgets/HeroBanner.vue").default
export const WidgetsNewsletterSignup: typeof import("../components/widgets/NewsletterSignup.vue").default
export const WidgetsProductGrid: typeof import("../components/widgets/ProductGrid.vue").default
export const WidgetsTestimonials: typeof import("../components/widgets/Testimonials.vue").default
export const WidgetsTextBlock: typeof import("../components/widgets/TextBlock.vue").default
export const WidgetsVideo: typeof import("../components/widgets/Video.vue").default
export const NuxtWelcome: typeof import("../../node_modules/nuxt/dist/app/components/welcome.vue").default
export const NuxtLayout: typeof import("../../node_modules/nuxt/dist/app/components/nuxt-layout").default
export const NuxtErrorBoundary: typeof import("../../node_modules/nuxt/dist/app/components/nuxt-error-boundary.vue").default
export const ClientOnly: typeof import("../../node_modules/nuxt/dist/app/components/client-only").default
export const DevOnly: typeof import("../../node_modules/nuxt/dist/app/components/dev-only").default
export const ServerPlaceholder: typeof import("../../node_modules/nuxt/dist/app/components/server-placeholder").default
export const NuxtLink: typeof import("../../node_modules/nuxt/dist/app/components/nuxt-link").default
export const NuxtLoadingIndicator: typeof import("../../node_modules/nuxt/dist/app/components/nuxt-loading-indicator").default
export const NuxtTime: typeof import("../../node_modules/nuxt/dist/app/components/nuxt-time.vue").default
export const NuxtRouteAnnouncer: typeof import("../../node_modules/nuxt/dist/app/components/nuxt-route-announcer").default
export const NuxtImg: typeof import("../../node_modules/nuxt/dist/app/components/nuxt-stubs").NuxtImg
export const NuxtPicture: typeof import("../../node_modules/nuxt/dist/app/components/nuxt-stubs").NuxtPicture
export const NuxtLinkLocale: typeof import("../../node_modules/@nuxtjs/i18n/dist/runtime/components/NuxtLinkLocale").default
export const SwitchLocalePathLink: typeof import("../../node_modules/@nuxtjs/i18n/dist/runtime/components/SwitchLocalePathLink").default
export const NuxtPage: typeof import("../../node_modules/nuxt/dist/pages/runtime/page").default
export const NoScript: typeof import("../../node_modules/nuxt/dist/head/runtime/components").NoScript
export const Link: typeof import("../../node_modules/nuxt/dist/head/runtime/components").Link
export const Base: typeof import("../../node_modules/nuxt/dist/head/runtime/components").Base
export const Title: typeof import("../../node_modules/nuxt/dist/head/runtime/components").Title
export const Meta: typeof import("../../node_modules/nuxt/dist/head/runtime/components").Meta
export const Style: typeof import("../../node_modules/nuxt/dist/head/runtime/components").Style
export const Head: typeof import("../../node_modules/nuxt/dist/head/runtime/components").Head
export const Html: typeof import("../../node_modules/nuxt/dist/head/runtime/components").Html
export const Body: typeof import("../../node_modules/nuxt/dist/head/runtime/components").Body
export const NuxtIsland: typeof import("../../node_modules/nuxt/dist/app/components/nuxt-island").default
export const LazyAdminFab: LazyComponent<typeof import("../components/AdminFab.vue").default>
export const LazyB2BVatIdInput: LazyComponent<typeof import("../components/B2BVatIdInput.vue").default>
export const LazyCheckout: LazyComponent<typeof import("../components/Checkout.vue").default>
export const LazyCheckoutTermsStep: LazyComponent<typeof import("../components/CheckoutTermsStep.vue").default>
export const LazyERPCustomerLookup: LazyComponent<typeof import("../components/ERP/CustomerLookup.vue").default>
export const LazyEditableText: LazyComponent<typeof import("../components/EditableText.vue").default>
export const LazyInvoiceDisplay: LazyComponent<typeof import("../components/InvoiceDisplay.vue").default>
export const LazyProductPrice: LazyComponent<typeof import("../components/ProductPrice.vue").default>
export const LazyRegistrationCheck: LazyComponent<typeof import("../components/RegistrationCheck.vue").default>
export const LazyCmsPageLayout: LazyComponent<typeof import("../components/cms/CmsPageLayout.vue").default>
export const LazyCmsRegionRenderer: LazyComponent<typeof import("../components/cms/RegionRenderer.vue").default>
export const LazyCmsWidgetNotFound: LazyComponent<typeof import("../components/cms/WidgetNotFound.vue").default>
export const LazyCmsWidgetRenderer: LazyComponent<typeof import("../components/cms/WidgetRenderer.vue").default>
export const LazyCmsWidgetsHeroBanner: LazyComponent<typeof import("../components/cms/widgets/HeroBanner.vue").default>
export const LazyCmsWidgetsNewsletterSignup: LazyComponent<typeof import("../components/cms/widgets/NewsletterSignup.vue").default>
export const LazyCmsWidgetsProductGrid: LazyComponent<typeof import("../components/cms/widgets/ProductGrid.vue").default>
export const LazyCmsWidgetsTestimonials: LazyComponent<typeof import("../components/cms/widgets/Testimonials.vue").default>
export const LazyCommonErrorBoundary: LazyComponent<typeof import("../components/common/ErrorBoundary.vue").default>
export const LazyCommonLanguageSwitcher: LazyComponent<typeof import("../components/common/LanguageSwitcher.vue").default>
export const LazyCommonLoadingSpinner: LazyComponent<typeof import("../components/common/LoadingSpinner.vue").default>
export const LazyShopProductCard: LazyComponent<typeof import("../components/shop/ProductCard.vue").default>
export const LazyShopProductCardModern: LazyComponent<typeof import("../components/shop/ProductCardModern.vue").default>
export const LazyWidgetsCallToAction: LazyComponent<typeof import("../components/widgets/CallToAction.vue").default>
export const LazyWidgetsFeatureGrid: LazyComponent<typeof import("../components/widgets/FeatureGrid.vue").default>
export const LazyWidgetsHeroBanner: LazyComponent<typeof import("../components/widgets/HeroBanner.vue").default>
export const LazyWidgetsNewsletterSignup: LazyComponent<typeof import("../components/widgets/NewsletterSignup.vue").default>
export const LazyWidgetsProductGrid: LazyComponent<typeof import("../components/widgets/ProductGrid.vue").default>
export const LazyWidgetsTestimonials: LazyComponent<typeof import("../components/widgets/Testimonials.vue").default>
export const LazyWidgetsTextBlock: LazyComponent<typeof import("../components/widgets/TextBlock.vue").default>
export const LazyWidgetsVideo: LazyComponent<typeof import("../components/widgets/Video.vue").default>
export const LazyNuxtWelcome: LazyComponent<typeof import("../../node_modules/nuxt/dist/app/components/welcome.vue").default>
export const LazyNuxtLayout: LazyComponent<typeof import("../../node_modules/nuxt/dist/app/components/nuxt-layout").default>
export const LazyNuxtErrorBoundary: LazyComponent<typeof import("../../node_modules/nuxt/dist/app/components/nuxt-error-boundary.vue").default>
export const LazyClientOnly: LazyComponent<typeof import("../../node_modules/nuxt/dist/app/components/client-only").default>
export const LazyDevOnly: LazyComponent<typeof import("../../node_modules/nuxt/dist/app/components/dev-only").default>
export const LazyServerPlaceholder: LazyComponent<typeof import("../../node_modules/nuxt/dist/app/components/server-placeholder").default>
export const LazyNuxtLink: LazyComponent<typeof import("../../node_modules/nuxt/dist/app/components/nuxt-link").default>
export const LazyNuxtLoadingIndicator: LazyComponent<typeof import("../../node_modules/nuxt/dist/app/components/nuxt-loading-indicator").default>
export const LazyNuxtTime: LazyComponent<typeof import("../../node_modules/nuxt/dist/app/components/nuxt-time.vue").default>
export const LazyNuxtRouteAnnouncer: LazyComponent<typeof import("../../node_modules/nuxt/dist/app/components/nuxt-route-announcer").default>
export const LazyNuxtImg: LazyComponent<typeof import("../../node_modules/nuxt/dist/app/components/nuxt-stubs").NuxtImg>
export const LazyNuxtPicture: LazyComponent<typeof import("../../node_modules/nuxt/dist/app/components/nuxt-stubs").NuxtPicture>
export const LazyNuxtLinkLocale: LazyComponent<typeof import("../../node_modules/@nuxtjs/i18n/dist/runtime/components/NuxtLinkLocale").default>
export const LazySwitchLocalePathLink: LazyComponent<typeof import("../../node_modules/@nuxtjs/i18n/dist/runtime/components/SwitchLocalePathLink").default>
export const LazyNuxtPage: LazyComponent<typeof import("../../node_modules/nuxt/dist/pages/runtime/page").default>
export const LazyNoScript: LazyComponent<typeof import("../../node_modules/nuxt/dist/head/runtime/components").NoScript>
export const LazyLink: LazyComponent<typeof import("../../node_modules/nuxt/dist/head/runtime/components").Link>
export const LazyBase: LazyComponent<typeof import("../../node_modules/nuxt/dist/head/runtime/components").Base>
export const LazyTitle: LazyComponent<typeof import("../../node_modules/nuxt/dist/head/runtime/components").Title>
export const LazyMeta: LazyComponent<typeof import("../../node_modules/nuxt/dist/head/runtime/components").Meta>
export const LazyStyle: LazyComponent<typeof import("../../node_modules/nuxt/dist/head/runtime/components").Style>
export const LazyHead: LazyComponent<typeof import("../../node_modules/nuxt/dist/head/runtime/components").Head>
export const LazyHtml: LazyComponent<typeof import("../../node_modules/nuxt/dist/head/runtime/components").Html>
export const LazyBody: LazyComponent<typeof import("../../node_modules/nuxt/dist/head/runtime/components").Body>
export const LazyNuxtIsland: LazyComponent<typeof import("../../node_modules/nuxt/dist/app/components/nuxt-island").default>

export const componentNames: string[]
