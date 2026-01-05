// i18n.config.ts
import { useTenantI18n } from '~/composables/useTenantI18n'

export default defineI18nConfig(() => ({
  legacy: false,
  locale: 'en',
  fallbackLocale: 'en',
  messages: {
    // Default fallback messages - will be overridden by tenant-specific ones
    en: {
      common: {
        loading: 'Loading...',
        error: 'An error occurred',
        save: 'Save',
        cancel: 'Cancel',
        delete: 'Delete',
        edit: 'Edit',
        add: 'Add',
        search: 'Search',
        filter: 'Filter',
        sort: 'Sort',
        next: 'Next',
        previous: 'Previous',
        page: 'Page',
        of: 'of',
        items: 'items'
      },
      navigation: {
        home: 'Home',
        shop: 'Shop',
        cart: 'Cart',
        dashboard: 'Dashboard',
        tenants: 'Tenants',
        login: 'Login',
        logout: 'Logout'
      }
    },
    de: {
      common: {
        loading: 'Laden...',
        error: 'Ein Fehler ist aufgetreten',
        save: 'Speichern',
        cancel: 'Abbrechen',
        delete: 'Löschen',
        edit: 'Bearbeiten',
        add: 'Hinzufügen',
        search: 'Suchen',
        filter: 'Filtern',
        sort: 'Sortieren',
        next: 'Weiter',
        previous: 'Zurück',
        page: 'Seite',
        of: 'von',
        items: 'Artikel'
      },
      navigation: {
        home: 'Startseite',
        shop: 'Shop',
        cart: 'Warenkorb',
        dashboard: 'Dashboard',
        tenants: 'Mandanten',
        login: 'Anmelden',
        logout: 'Abmelden'
      }
    }
  }
}))