// plugins/i18n.client.ts
export default defineNuxtPlugin(async nuxtApp => {
  const runtimeConfig = useRuntimeConfig();

  // Get tenant from runtime config
  const tenantId = runtimeConfig.public.tenantId;

  const loadTenantTranslations = async (locale: string) => {
    try {
      // Load tenant-specific translations from backend
      const response = await $fetch(`/api/translations/${tenantId}/${locale}`, {
        baseURL: runtimeConfig.public.apiBase,
      });

      // Merge with existing messages using the i18n instance
      const i18n = nuxtApp.$i18n as any;
      i18n.setLocaleMessage(locale, {
        ...i18n.getLocaleMessage(locale),
        ...(response && typeof response === 'object' ? response : {}),
      });
    } catch (error) {
      console.warn(`Failed to load translations for tenant ${tenantId}, locale ${locale}:`, error);
      // Fallback to default translations
    }
  };

  // Load tenant translations for current locale on client-side hydration
  const i18n = nuxtApp.$i18n as any;
  await loadTenantTranslations(i18n.locale.value);
});
