// plugins/i18n.client.ts
export default defineNuxtPlugin(async nuxtApp => {
  const { loadTenantTranslations } = useTenantI18n();

  // Load tenant translations for current locale on client-side hydration
  const { locale } = useI18n();
  await loadTenantTranslations(locale.value);
});
