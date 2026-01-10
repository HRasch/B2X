// composables/useTenantI18n.ts

export const useTenantI18n = () => {
  // Only call useI18n() when we're in a component context
  const { setLocaleMessage, getLocaleMessage, locale } = useI18n();
  const runtimeConfig = useRuntimeConfig();

  // Get tenant from server context or runtime config
  const tenantId = process.server
    ? useRequestEvent()?.context.tenantId || runtimeConfig.public.tenantId
    : runtimeConfig.public.tenantId;

  const loadTenantTranslations = async (locale: string) => {
    try {
      // Load tenant-specific translations from backend
      const response = await $fetch(`/api/translations/${tenantId}/${locale}`, {
        baseURL: runtimeConfig.public.apiBase,
      });

      // Merge with existing messages
      setLocaleMessage(locale, {
        ...getLocaleMessage(locale),
        ...(response && typeof response === 'object' ? response : {}),
      });
    } catch (error) {
      console.warn(`Failed to load translations for tenant ${tenantId}, locale ${locale}:`, error);
      // Fallback to default translations
    }
  };

  const setTenantLocale = async (localeCode: string) => {
    await loadTenantTranslations(localeCode);
    locale.value = localeCode;
  };

  return {
    tenantId,
    loadTenantTranslations,
    setTenantLocale,
  };
};
