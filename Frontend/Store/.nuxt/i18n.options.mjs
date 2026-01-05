
// @ts-nocheck
import locale_en_46json_a4f3a9c8 from "#nuxt-i18n/a4f3a9c8";
import locale_de_46json_e239feab from "#nuxt-i18n/e239feab";
import locale_fr_46json_09af47d5 from "#nuxt-i18n/09af47d5";
import locale_es_46json_b78d90e0 from "#nuxt-i18n/b78d90e0";
import locale_it_46json_fdc7c58d from "#nuxt-i18n/fdc7c58d";
import locale_pt_46json_85cd2cea from "#nuxt-i18n/85cd2cea";
import locale_nl_46json_16d6f550 from "#nuxt-i18n/16d6f550";
import locale_pl_46json_599edce8 from "#nuxt-i18n/599edce8";

export const localeCodes =  [
  "en",
  "de",
  "fr",
  "es",
  "it",
  "pt",
  "nl",
  "pl"
]

export const localeLoaders = {
  en: [
    {
      key: "locale_en_46json_a4f3a9c8",
      load: () => Promise.resolve(locale_en_46json_a4f3a9c8),
      cache: true
    }
  ],
  de: [
    {
      key: "locale_de_46json_e239feab",
      load: () => Promise.resolve(locale_de_46json_e239feab),
      cache: true
    }
  ],
  fr: [
    {
      key: "locale_fr_46json_09af47d5",
      load: () => Promise.resolve(locale_fr_46json_09af47d5),
      cache: true
    }
  ],
  es: [
    {
      key: "locale_es_46json_b78d90e0",
      load: () => Promise.resolve(locale_es_46json_b78d90e0),
      cache: true
    }
  ],
  it: [
    {
      key: "locale_it_46json_fdc7c58d",
      load: () => Promise.resolve(locale_it_46json_fdc7c58d),
      cache: true
    }
  ],
  pt: [
    {
      key: "locale_pt_46json_85cd2cea",
      load: () => Promise.resolve(locale_pt_46json_85cd2cea),
      cache: true
    }
  ],
  nl: [
    {
      key: "locale_nl_46json_16d6f550",
      load: () => Promise.resolve(locale_nl_46json_16d6f550),
      cache: true
    }
  ],
  pl: [
    {
      key: "locale_pl_46json_599edce8",
      load: () => Promise.resolve(locale_pl_46json_599edce8),
      cache: true
    }
  ]
}

export const vueI18nConfigs = []

export const nuxtI18nOptions = {
  restructureDir: "i18n",
  experimental: {
    localeDetector: "",
    switchLocalePathLinkSSR: false,
    autoImportTranslationFunctions: false,
    typedPages: true,
    typedOptionsAndMessages: false,
    generatedLocaleFilePathFormat: "absolute",
    alternateLinkCanonicalQueries: false,
    hmr: true
  },
  bundle: {
    compositionOnly: true,
    runtimeOnly: false,
    fullInstall: true,
    dropMessageCompiler: false,
    optimizeTranslationDirective: true
  },
  compilation: {
    strictMessage: true,
    escapeHtml: false
  },
  customBlocks: {
    defaultSFCLang: "json",
    globalSFCScope: false
  },
  locales: [
    {
      code: "en",
      name: "English",
      flag: "🇺🇸",
      files: [
        {
          path: "/Users/holger/Documents/Projekte/B2Connect/frontend/Store/i18n/locales/en.json",
          cache: undefined
        }
      ]
    },
    {
      code: "de",
      name: "Deutsch",
      flag: "🇩🇪",
      files: [
        {
          path: "/Users/holger/Documents/Projekte/B2Connect/frontend/Store/i18n/locales/de.json",
          cache: undefined
        }
      ]
    },
    {
      code: "fr",
      name: "Français",
      flag: "🇫🇷",
      files: [
        {
          path: "/Users/holger/Documents/Projekte/B2Connect/frontend/Store/i18n/locales/fr.json",
          cache: undefined
        }
      ]
    },
    {
      code: "es",
      name: "Español",
      flag: "🇪🇸",
      files: [
        {
          path: "/Users/holger/Documents/Projekte/B2Connect/frontend/Store/i18n/locales/es.json",
          cache: undefined
        }
      ]
    },
    {
      code: "it",
      name: "Italiano",
      flag: "🇮🇹",
      files: [
        {
          path: "/Users/holger/Documents/Projekte/B2Connect/frontend/Store/i18n/locales/it.json",
          cache: undefined
        }
      ]
    },
    {
      code: "pt",
      name: "Português",
      flag: "🇵🇹",
      files: [
        {
          path: "/Users/holger/Documents/Projekte/B2Connect/frontend/Store/i18n/locales/pt.json",
          cache: undefined
        }
      ]
    },
    {
      code: "nl",
      name: "Nederlands",
      flag: "🇳🇱",
      files: [
        {
          path: "/Users/holger/Documents/Projekte/B2Connect/frontend/Store/i18n/locales/nl.json",
          cache: undefined
        }
      ]
    },
    {
      code: "pl",
      name: "Polski",
      flag: "🇵🇱",
      files: [
        {
          path: "/Users/holger/Documents/Projekte/B2Connect/frontend/Store/i18n/locales/pl.json",
          cache: undefined
        }
      ]
    }
  ],
  defaultLocale: "en",
  defaultDirection: "ltr",
  routesNameSeparator: "___",
  trailingSlash: false,
  defaultLocaleRouteNameSuffix: "default",
  strategy: "no_prefix",
  lazy: false,
  langDir: "locales",
  rootRedirect: undefined,
  detectBrowserLanguage: false,
  differentDomains: false,
  baseUrl: "",
  customRoutes: "page",
  pages: {},
  skipSettingLocaleOnNavigate: false,
  types: "composition",
  debug: false,
  parallelPlugin: false,
  multiDomainLocales: false,
  i18nModules: []
}

export const normalizedLocales = [
  {
    code: "en",
    name: "English",
    flag: "🇺🇸",
    files: [
      {
        path: "/Users/holger/Documents/Projekte/B2Connect/frontend/Store/i18n/locales/en.json",
        cache: undefined
      }
    ]
  },
  {
    code: "de",
    name: "Deutsch",
    flag: "🇩🇪",
    files: [
      {
        path: "/Users/holger/Documents/Projekte/B2Connect/frontend/Store/i18n/locales/de.json",
        cache: undefined
      }
    ]
  },
  {
    code: "fr",
    name: "Français",
    flag: "🇫🇷",
    files: [
      {
        path: "/Users/holger/Documents/Projekte/B2Connect/frontend/Store/i18n/locales/fr.json",
        cache: undefined
      }
    ]
  },
  {
    code: "es",
    name: "Español",
    flag: "🇪🇸",
    files: [
      {
        path: "/Users/holger/Documents/Projekte/B2Connect/frontend/Store/i18n/locales/es.json",
        cache: undefined
      }
    ]
  },
  {
    code: "it",
    name: "Italiano",
    flag: "🇮🇹",
    files: [
      {
        path: "/Users/holger/Documents/Projekte/B2Connect/frontend/Store/i18n/locales/it.json",
        cache: undefined
      }
    ]
  },
  {
    code: "pt",
    name: "Português",
    flag: "🇵🇹",
    files: [
      {
        path: "/Users/holger/Documents/Projekte/B2Connect/frontend/Store/i18n/locales/pt.json",
        cache: undefined
      }
    ]
  },
  {
    code: "nl",
    name: "Nederlands",
    flag: "🇳🇱",
    files: [
      {
        path: "/Users/holger/Documents/Projekte/B2Connect/frontend/Store/i18n/locales/nl.json",
        cache: undefined
      }
    ]
  },
  {
    code: "pl",
    name: "Polski",
    flag: "🇵🇱",
    files: [
      {
        path: "/Users/holger/Documents/Projekte/B2Connect/frontend/Store/i18n/locales/pl.json",
        cache: undefined
      }
    ]
  }
]

export const NUXT_I18N_MODULE_ID = "@nuxtjs/i18n"
export const parallelPlugin = false
export const isSSG = false
export const hasPages = true

export const DEFAULT_COOKIE_KEY = "i18n_redirected"
export const DEFAULT_DYNAMIC_PARAMS_KEY = "nuxtI18nInternal"
export const SWITCH_LOCALE_PATH_LINK_IDENTIFIER = "nuxt-i18n-slp"
/** client **/

/** client-end **/