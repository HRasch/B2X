// plugins/i18n.client.ts
export default defineNuxtPlugin(async (nuxtApp) => {
  const { $i18n } = nuxtApp
  const { loadTenantTranslations } = useTenantI18n()

  // Load tenant translations for current locale on client-side hydration
  await loadTenantTranslations($i18n.locale.value)
})