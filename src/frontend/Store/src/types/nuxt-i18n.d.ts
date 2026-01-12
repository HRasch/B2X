declare module 'nuxt-i18n' {
  // Minimal shim for defineI18nConfig used in the project during migration
  export function defineI18nConfig(configFactory: () => any): any; // eslint-disable-line @typescript-eslint/no-explicit-any
}
