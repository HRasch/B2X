// i18n.config.ts

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
        items: 'items',
      },
      navigation: {
        home: 'Home',
        shop: 'Shop',
        cart: 'Cart',
        dashboard: 'Dashboard',
        tenants: 'Tenants',
        login: 'Login',
        logout: 'Logout',
      },
      notFound: {
        title: 'Page Not Found',
        message: 'The page you are looking for does not exist.',
        goHome: 'Go Back Home',
      },
      app: {
        skipToMain: 'Skip to main content',
        brand: 'B2Connect',
        admin: 'Admin',
        services: 'Services',
        branding: 'Branding',
        design: 'Design',
        marketing: 'Marketing',
        advertisement: 'Advertisement',
        company: 'Company',
        aboutUs: 'About us',
        contact: 'Contact',
        jobs: 'Jobs',
        pressKit: 'Press kit',
        legal: 'Legal',
        termsOfUse: 'Terms of use',
        privacyPolicy: 'Privacy policy',
        cookiePolicy: 'Cookie policy',
        newsletter: 'Newsletter',
        enterEmail: 'Enter your email address',
        subscribe: 'Subscribe',
      },
      home: {
        hero: {
          title: 'Welcome to B2Connect Store',
          subtitle: 'Discover amazing products with seamless integration and exceptional service',
          shopNow: 'Shop Now',
          browseCategories: 'Browse Categories',
        },
        featuredProducts: {
          title: 'Featured Products',
          viewAll: 'View All Products',
        },
        categories: {
          title: 'Shop by Category',
        },
        features: {
          title: 'Why Choose Us',
          quality: {
            title: 'Premium Quality',
            description: 'We ensure the highest quality standards for all our products',
          },
          fast: {
            title: 'Fast Delivery',
            description: 'Quick and reliable shipping to get your orders to you fast',
          },
          support: {
            title: '24/7 Support',
            description: 'Our customer service team is always here to help you',
          },
        },
        newsletter: {
          title: 'Stay Updated',
          subtitle: 'Subscribe to our newsletter for the latest updates and offers',
          subscribe: 'Subscribe',
        },
        table: {
          headers: {
            product: 'Product',
            qty: 'Qty',
            price: 'Price',
          },
        },
      },
      vat: {
        countryCode: 'Country Code',
        vatNumber: 'VAT Number',
        validate: 'Validate',
        validating: 'Validating...',
        companyName: 'Company Name:',
        address: 'Address:',
        reverseCharge: 'Reverse Charge:',
        reverseChargeApplies: '0% VAT (applies)',
        standardVatRate: 'Standard VAT rate',
        clearAndStartOver: 'Clear & Start Over',
        validationHelp: {
          title: 'VAT Validation Help',
          description:
            'If you cannot provide a valid VAT number, you can continue as a private customer or contact our support team.',
        },
        countries: {
          AT: 'Austria (AT)',
          BE: 'Belgium (BE)',
          BG: 'Bulgaria (BG)',
          HR: 'Croatia (HR)',
          CY: 'Cyprus (CY)',
          CZ: 'Czech Republic (CZ)',
          DK: 'Denmark (DK)',
          DE: 'Germany (DE)',
          EE: 'Estonia (EE)',
          FI: 'Finland (FI)',
          FR: 'France (FR)',
          GR: 'Greece (GR)',
          HU: 'Hungary (HU)',
          IE: 'Ireland (IE)',
          IT: 'Italy (IT)',
          LV: 'Latvia (LV)',
          LT: 'Lithuania (LT)',
          LU: 'Luxembourg (LU)',
          MT: 'Malta (MT)',
          NL: 'Netherlands (NL)',
          PL: 'Poland (PL)',
          PT: 'Portugal (PT)',
          RO: 'Romania (RO)',
          SK: 'Slovakia (SK)',
          SI: 'Slovenia (SI)',
          ES: 'Spain (ES)',
          SE: 'Sweden (SE)',
        },
      },
      legal: {
        checkout: {
          title: 'Terms & Conditions',
          subtitle: 'Please accept the required terms to continue',
          requiredFields: '* Required fields',
          back: 'Back',
          continueToPayment: 'Continue to Payment',
          processing: 'Processing...',
          acceptTerms: 'I accept the Terms and Conditions',
          acceptPrivacy: 'I accept the Privacy Policy',
          understandWithdrawal: 'I understand my 14-day withdrawal right',
          acceptTermsError: 'Please accept the Terms and Conditions and Privacy Policy',
          acceptTermsSuccess: 'Terms accepted!',
          saveError: 'Error saving terms acceptance',
          generalError: 'An error occurred. Please try again later.',
        },
        checkout: {
          header: {
            title: 'Checkout',
            stepIndicator: 'Step {{currentStep}} of {{totalSteps}}',
          },
          steps: {
            shippingAddress: 'Shipping Address',
            shippingMethod: 'Shipping Method',
            orderReview: 'Order Review',
          },
          form: {
            labels: {
              streetAddress: 'Street Address',
              city: 'City',
              postalCode: 'Postal Code',
              country: 'Country',
            },
            placeholders: {
              streetAddress: '123 Main Street',
              city: 'Berlin',
              postalCode: '12345',
            },
            required: '* Required',
            saveAddress: 'Save this address for future orders',
          },
          validation: {
            streetRequired: 'Street is required',
            cityRequired: 'City is required',
            postalCodeRequired: 'Postal code is required',
            countryRequired: 'Country is required',
            invalidPostalCode: 'Invalid German postal code (format: 12345)',
          },
          shipping: {
            title: 'Shipping Method',
            selectMethod: 'Select a shipping method',
            estimatedDelivery: 'Estimated delivery: {{days}} business days',
            free: 'Free',
            businessVerification: {
              title: 'Business Verification',
              description:
                'Please provide your VAT ID for verification. Valid EU business VAT-IDs may qualify for reverse charge (0% VAT).',
            },
          },
          orderReview: {
            title: 'Order Review',
            shippingAddress: 'Shipping Address',
            shippingMethod: 'Shipping Method',
            orderItems: 'Order Items',
            editAddress: 'Edit Address',
            changeShippingMethod: 'Change Shipping Method',
            estimatedDelivery: 'Estimated delivery: {{days}} business days',
          },
          orderSummary: {
            title: 'Order Summary',
            subtotal: 'Subtotal',
            vatIncluded: 'VAT (incl.)',
            shipping: 'Shipping',
            total: 'Total',
            secureCheckout: 'Secure Checkout',
            secureDescription: 'Your payment is encrypted and secure',
          },
          buttons: {
            back: 'Back',
            continueToShipping: 'Continue to Shipping Method',
            continueToReview: 'Continue to Review',
            proceedToPayment: 'Proceed to Payment',
            editAddress: 'Edit Address',
            changeShippingMethod: 'Change Shipping Method',
          },
        },
        termsAndConditions: {
          title: 'Terms and Conditions',
          understood: 'Understood',
          sections: {
            general: {
              title: '1. General Provisions',
              content:
                'These General Terms and Conditions regulate the relationship between the operator of this online shop and the buyer.',
            },
            products: {
              title: '2. Product Descriptions',
              content:
                'All product descriptions are offers for sale. A contract is only concluded when you place an order and we accept it.',
            },
            pricing: {
              title: '3. Prices and Payment Terms',
              content:
                'All prices include the applicable value added tax. Shipping costs are calculated separately and displayed at checkout.',
            },
            delivery: {
              title: '4. Delivery',
              content:
                'Delivery times are non-binding. We are only liable for delays due to our fault.',
            },
            withdrawal: {
              title: '5. Right of Withdrawal',
              content:
                'You have a 14-day right of withdrawal from receipt of the goods. See below for details.',
            },
            liability: {
              title: '6. Liability',
              content:
                'Liability for damages is limited to direct damages up to the amount of the purchase price.',
            },
            privacy: {
              title: '7. Data Protection',
              content: 'See Privacy Policy for the handling of your data.',
            },
            final: {
              title: '8. Final Provisions',
              content:
                'German law applies. The place of jurisdiction is the registered office of the company.',
            },
          },
        },
        privacyPolicy: {
          title: 'Privacy Policy',
          understood: 'Understood',
          sections: {
            responsible: {
              title: '1. Responsible Party',
              content:
                'The operator of this shop is responsible for data processing (see imprint).',
            },
            collection: {
              title: '2. Collection and Processing',
              content:
                'We only collect your data for the processing of your purchase and shipping.',
            },
            storage: {
              title: '3. Storage Duration',
              content: 'Personal data is stored for 10 years to fulfill tax obligations.',
            },
            rights: {
              title: '4. Your Rights',
              content:
                'You have the right to information, correction, deletion and data portability.',
            },
            cookies: {
              title: '5. Cookies',
              content:
                'We use technically necessary cookies. Other cookies are stored with your consent.',
            },
            security: {
              title: '6. Security',
              content: 'We protect your data through encryption and secure transmission.',
            },
            contact: {
              title: '7. Data Protection Officer',
              content: 'For questions: privacy@example.com',
            },
          },
        },
        withdrawalRights: {
          title: 'Right of Withdrawal (14 Days)',
          understood: 'Understood',
          sections: {
            yourRights: {
              title: 'Your Right of Withdrawal',
              content:
                'You have the right to withdraw from your purchase within 14 days of receipt of the goods without giving any reason.',
            },
            deadlines: {
              title: 'Withdrawal Deadlines',
              start: 'Start: Day after receipt of goods',
              duration: 'Duration: 14 days',
              form: 'Form: Simple written notification by email is sufficient',
            },
            exceptions: {
              title: 'Exceptions',
              intro: 'Right of withdrawal does NOT apply to:',
              digital: 'Digital content after download',
              customized: 'Customized or personalized goods',
              damaged: 'Goods damaged after delivery (your fault)',
            },
            returnProcess: {
              title: 'Return Process',
              content:
                'Return the goods immediately. Shipping costs are borne by the buyer (except for justified returns).',
            },
            contact: {
              title: 'Contact',
              content: 'Send withdrawals to: withdrawal@example.com',
            },
            legalBasis: 'Legal basis: §§ 355-359 BGB (Distance Selling Act)',
          },
        },
      },
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
        items: 'Artikel',
      },
      navigation: {
        home: 'Startseite',
        shop: 'Shop',
        cart: 'Warenkorb',
        dashboard: 'Dashboard',
        tenants: 'Mandanten',
        login: 'Anmelden',
        logout: 'Abmelden',
      },
      notFound: {
        title: 'Seite nicht gefunden',
        message: 'Die gesuchte Seite existiert nicht.',
        goHome: 'Zurück zur Startseite',
      },
      app: {
        skipToMain: 'Zum Hauptinhalt springen',
        brand: 'B2Connect',
        admin: 'Admin',
        services: 'Dienstleistungen',
        branding: 'Branding',
        design: 'Design',
        marketing: 'Marketing',
        advertisement: 'Werbung',
        company: 'Unternehmen',
        aboutUs: 'Über uns',
        contact: 'Kontakt',
        jobs: 'Jobs',
        pressKit: 'Press Kit',
        legal: 'Rechtliches',
        termsOfUse: 'Nutzungsbedingungen',
        privacyPolicy: 'Datenschutz',
        cookiePolicy: 'Cookie-Richtlinie',
        newsletter: 'Newsletter',
        enterEmail: 'Geben Sie Ihre E-Mail-Adresse ein',
        subscribe: 'Abonnieren',
      },
      home: {
        hero: {
          title: 'Willkommen im B2Connect Store',
          subtitle:
            'Entdecken Sie tolle Produkte mit nahtloser Integration und erstklassigem Service',
          shopNow: 'Jetzt einkaufen',
          browseCategories: 'Kategorien durchsuchen',
        },
        featuredProducts: {
          title: 'Ausgewählte Produkte',
          viewAll: 'Alle Produkte ansehen',
        },
        categories: {
          title: 'Nach Kategorie einkaufen',
        },
        features: {
          title: 'Warum uns wählen',
          quality: {
            title: 'Premium Qualität',
            description: 'Wir garantieren höchste Qualitätsstandards für alle unsere Produkte',
          },
          fast: {
            title: 'Schnelle Lieferung',
            description:
              'Schnell und zuverlässig versenden, um Ihre Bestellungen schnell zu Ihnen zu bringen',
          },
          support: {
            title: '24/7 Support',
            description: 'Unser Kundenservice-Team ist immer für Sie da',
          },
        },
        newsletter: {
          title: 'Bleiben Sie auf dem Laufenden',
          subtitle: 'Abonnieren Sie unseren Newsletter für die neuesten Updates und Angebote',
          subscribe: 'Abonnieren',
        },
        table: {
          headers: {
            product: 'Produkt',
            qty: 'Menge',
            price: 'Preis',
          },
        },
      },
      vat: {
        countryCode: 'Ländercode',
        vatNumber: 'Umsatzsteuer-Identifikationsnummer',
        validate: 'Überprüfen',
        validating: 'Überprüfung läuft...',
        companyName: 'Firmenname:',
        address: 'Adresse:',
        reverseCharge: 'Reverse Charge:',
        reverseChargeApplies: '0% MwSt (gilt)',
        standardVatRate: 'Standard-Mehrwertsteuersatz',
        clearAndStartOver: 'Löschen & Neu Starten',
        validationHelp: {
          title: 'Hilfe zur Umsatzsteuer-Validierung',
          description:
            'Wenn Sie keine gültige Umsatzsteuer-Identifikationsnummer angeben können, können Sie als Privatkunde fortfahren oder unser Support-Team kontaktieren.',
        },
        countries: {
          AT: 'Österreich (AT)',
          BE: 'Belgien (BE)',
          BG: 'Bulgarien (BG)',
          HR: 'Kroatien (HR)',
          CY: 'Zypern (CY)',
          CZ: 'Tschechische Republik (CZ)',
          DK: 'Dänemark (DK)',
          DE: 'Deutschland (DE)',
          EE: 'Estland (EE)',
          FI: 'Finnland (FI)',
          FR: 'Frankreich (FR)',
          GR: 'Griechenland (GR)',
          HU: 'Ungarn (HU)',
          IE: 'Irland (IE)',
          IT: 'Italien (IT)',
          LV: 'Lettland (LV)',
          LT: 'Litauen (LT)',
          LU: 'Luxemburg (LU)',
          MT: 'Malta (MT)',
          NL: 'Niederlande (NL)',
          PL: 'Polen (PL)',
          PT: 'Portugal (PT)',
          RO: 'Rumänien (RO)',
          SK: 'Slowakei (SK)',
          SI: 'Slowenien (SI)',
          ES: 'Spanien (ES)',
          SE: 'Schweden (SE)',
        },
      },
      legal: {
        checkout: {
          title: 'Bedingungen',
          subtitle: 'Bitte akzeptieren Sie die erforderlichen Bedingungen, um fortzufahren',
          requiredFields: '* Erforderliche Felder',
          back: 'Zurück',
          continueToPayment: 'Zur Zahlung',
          processing: 'Wird verarbeitet...',
          acceptTerms: 'Ich akzeptiere die Allgemeinen Geschäftsbedingungen',
          acceptPrivacy: 'Ich akzeptiere die Datenschutzerklärung',
          understandWithdrawal: 'Ich verstehe mein Widerrufsrecht (14 Tage)',
          acceptTermsError:
            'Bitte akzeptieren Sie die Allgemeinen Geschäftsbedingungen und Datenschutzerklärung',
          acceptTermsSuccess: 'Bedingungen akzeptiert!',
          saveError: 'Fehler beim Speichern der Bedingungsannahme',
          generalError: 'Ein Fehler ist aufgetreten. Bitte versuchen Sie es später erneut.',
        },
        checkout: {
          header: {
            title: 'Bestellabschluss',
            breadcrumb: {
              shop: 'Shop',
              cart: 'Warenkorb',
              checkout: '/ Kasse',
            },
          },
          steps: {
            shippingAddress: 'Lieferadresse',
            shippingMethod: 'Versandart',
            orderReview: 'Bestellübersicht',
          },
          form: {
            labels: {
              firstName: 'Vorname *',
              lastName: 'Nachname *',
              streetAddress: 'Straße und Hausnummer *',
              postalCode: 'Postleitzahl *',
              city: 'Stadt *',
              country: 'Land *',
            },
            placeholders: {
              firstName: 'Max',
              lastName: 'Mustermann',
              streetAddress: 'Hauptstraße 123',
              postalCode: '12345',
              city: 'Berlin',
            },
            countries: {
              germany: 'Deutschland',
              austria: 'Österreich',
              belgium: 'Belgien',
              france: 'Frankreich',
              netherlands: 'Niederlande',
            },
            required: '* Erforderlich',
            description: 'Geben Sie bitte Ihre Lieferadresse ein',
          },
          validation: {
            firstNameRequired: 'Vorname ist erforderlich',
            lastNameRequired: 'Nachname ist erforderlich',
            streetRequired: 'Straße ist erforderlich',
            cityRequired: 'Stadt ist erforderlich',
            postalCodeRequired: 'Postleitzahl ist erforderlich',
            countryRequired: 'Land ist erforderlich',
            invalidPostalCode: 'Ungültige deutsche Postleitzahl (Format: 12345)',
          },
          shipping: {
            title: 'Versandart',
            description: 'Wählen Sie Ihre bevorzugte Versandart',
            deliveryTime: '⏱️ Lieferzeit: ca. {{days}} Werktag(e)',
          },
          orderReview: {
            title: 'Überprüfung & Zahlungsart',
            shippingAddress: 'Lieferadresse',
            shippingMethod: 'Versandart',
            paymentMethod: 'Zahlungsart',
            edit: '✏️ Bearbeiten',
          },
          orderSummary: {
            title: 'Bestellübersicht',
            netto: 'Netto:',
            vat: 'MwSt (19%):',
            shipping: 'Versand:',
            total: 'Gesamt:',
            trustBadges: {
              ssl: 'SSL verschlüsselt',
              returns: '30 Tage Rückgabe',
              insured: 'Versand versichert',
            },
          },
          terms: {
            acceptText: 'Ich akzeptiere die',
            termsLink: 'Geschäftsbedingungen',
            and: 'und die',
            privacyLink: 'Datenschutzerklärung',
            required: '*',
          },
          compliance: {
            title: 'Preisangabenverordnung (PAngV)',
            content:
              'Alle angezeigten Preise sind Endpreise und enthalten bereits die gesetzliche Mehrwertsteuer (MwSt) in Höhe von 19%.',
          },
          buttons: {
            backToCart: '← Zurück zum Warenkorb',
            continueToShipping: 'Weiter zu Versand →',
            backToAddress: '← Zurück zur Adresse',
            continueToReview: 'Weiter zur Überprüfung →',
            backToShipping: '← Zurück zur Versandart',
            processing: 'Bestellung wird verarbeitet...',
            completeOrder: 'Bestellung abschließen',
          },
        },
        termsAndConditions: {
          title: 'Allgemeine Geschäftsbedingungen',
          understood: 'Verstanden',
          sections: {
            general: {
              title: '1. Allgemeine Bestimmungen',
              content:
                'Diese Allgemeinen Geschäftsbedingungen regeln die Beziehung zwischen dem Betreiber dieses Online-Shops und dem Käufer.',
            },
            products: {
              title: '2. Produktbeschreibungen',
              content:
                'Alle Produktbeschreibungen sind Angebote zum Verkauf. Ein Vertrag kommt nur zustande, wenn Sie eine Bestellung aufgeben und wir diese akzeptieren.',
            },
            pricing: {
              title: '3. Preise und Zahlungsbedingungen',
              content:
                'Alle Preise enthalten die gültige Mehrwertsteuer. Versandkosten werden separat berechnet und beim Checkout angezeigt.',
            },
            delivery: {
              title: '4. Lieferung',
              content:
                'Lieferzeiten sind unverbindlich. Bei Verzug haften wir nur bei Verschulden.',
            },
            withdrawal: {
              title: '5. Widerrufsrecht',
              content:
                'Sie haben ein Widerrufsrecht von 14 Tagen ab Erhalt der Ware. Siehe unten für Details.',
            },
            liability: {
              title: '6. Haftung',
              content:
                'Haftung für Schäden begrenzt auf Direktschäden bis zur Höhe des Kaufpreises.',
            },
            privacy: {
              title: '7. Datenschutz',
              content: 'Siehe Datenschutzerklärung für die Behandlung Ihrer Daten.',
            },
            final: {
              title: '8. Schlussbestimmungen',
              content: 'Es gilt deutsches Recht. Gerichtsstand ist der Sitz des Unternehmens.',
            },
          },
        },
        privacyPolicy: {
          title: 'Datenschutzerklärung',
          understood: 'Verstanden',
          sections: {
            responsible: {
              title: '1. Verantwortlicher',
              content:
                'Verantwortlich für die Datenverarbeitung ist der Betreiber dieses Shops (siehe Impressum).',
            },
            collection: {
              title: '2. Erhebung und Verarbeitung',
              content: 'Wir erheben Ihre Daten nur zur Abwicklung Ihres Einkaufs und zum Versand.',
            },
            storage: {
              title: '3. Speicherdauer',
              content:
                'Persönliche Daten werden 10 Jahre zur Erfüllung von Steuerpflichten gespeichert.',
            },
            rights: {
              title: '4. Ihre Rechte',
              content:
                'Sie haben das Recht auf Auskunft, Berichtigung, Löschung und Datenportabilität.',
            },
            cookies: {
              title: '5. Cookies',
              content:
                'Wir verwenden technisch notwendige Cookies. Andere Cookies werden mit Ihrer Einwilligung gespeichert.',
            },
            security: {
              title: '6. Sicherheit',
              content: 'Wir schützen Ihre Daten durch Verschlüsselung und sichere Übertragung.',
            },
            contact: {
              title: '7. Datenschutzbeauftragter',
              content: 'Bei Fragen: datenschutz@example.com',
            },
          },
        },
        withdrawalRights: {
          title: 'Widerrufsrecht (14 Tage)',
          understood: 'Verstanden',
          sections: {
            yourRights: {
              title: 'Ihr Widerrufsrecht',
              content:
                'Sie haben das Recht, Ihren Kauf innerhalb von 14 Tagen nach Erhalt der Ware zu widerrufen, ohne einen Grund angeben zu müssen.',
            },
            deadlines: {
              title: 'Widerrufsfristen',
              start: 'Beginn: Tag nach Erhalt der Ware',
              duration: 'Dauer: 14 Tage',
              form: 'Form: Einfache schriftliche Mitteilung per E-Mail genügt',
            },
            exceptions: {
              title: 'Ausnahmen',
              intro: 'Widerrufsrecht gilt NICHT für:',
              digital: 'Digitale Inhalte nach Abruf',
              customized: 'Maßgeschneiderte oder personalisierte Waren',
              damaged: 'Waren, die nach Zustellung beschädigt wurden (Ihr Verschulden)',
            },
            returnProcess: {
              title: 'Rückgabeverfahren',
              content:
                'Senden Sie die Ware unverzüglich zurück. Versandkosten trägt der Käufer (außer bei berechtigter Rückgabe).',
            },
            contact: {
              title: 'Kontakt',
              content: 'Widerrufe richten Sie an: widerruf@example.com',
            },
            legalBasis: 'Rechtsgrundlage: §§ 355-359 BGB (Fernabsatzgesetz)',
          },
        },
      },
    },
    fr: {
      common: {
        loading: 'Chargement...',
        error: "Une erreur s'est produite",
        save: 'Enregistrer',
        cancel: 'Annuler',
        delete: 'Supprimer',
        edit: 'Modifier',
        add: 'Ajouter',
        search: 'Rechercher',
        filter: 'Filtrer',
        sort: 'Trier',
        next: 'Suivant',
        previous: 'Précédent',
        page: 'Page',
        of: 'de',
        items: 'éléments',
      },
      navigation: {
        home: 'Accueil',
        products: 'Produits',
        categories: 'Catégories',
        cart: 'Panier',
        dashboard: 'Tableau de bord',
        tenants: 'Locataires',
        login: 'Se connecter',
        logout: 'Se déconnecter',
      },
      notFound: {
        title: 'Page non trouvée',
        message: "La page que vous recherchez n'existe pas.",
        goHome: "Retour à l'accueil",
      },
      app: {
        skipToMain: 'Aller au contenu principal',
        brand: 'B2Connect',
        admin: 'Admin',
        services: 'Services',
        branding: 'Marque',
        design: 'Design',
        marketing: 'Marketing',
        advertisement: 'Publicité',
        company: 'Entreprise',
        aboutUs: 'À propos de nous',
        contact: 'Contact',
        jobs: 'Emplois',
        pressKit: 'Kit presse',
        legal: 'Légal',
        termsOfUse: "Conditions d'utilisation",
        privacyPolicy: 'Politique de confidentialité',
        cookiePolicy: 'Politique de cookies',
        newsletter: 'Newsletter',
        enterEmail: 'Entrez votre adresse e-mail',
        subscribe: "S'abonner",
      },
      home: {
        hero: {
          title: 'Bienvenue sur B2Connect Store',
          subtitle:
            'Découvrez des produits exceptionnels avec une intégration transparente et un service de qualité',
          shopNow: 'Acheter maintenant',
          browseCategories: 'Parcourir les catégories',
        },
        featuredProducts: {
          title: 'Produits en vedette',
          viewAll: 'Voir tous les produits',
        },
        categories: {
          title: 'Acheter par catégorie',
        },
        features: {
          title: 'Pourquoi nous choisir',
          quality: {
            title: 'Qualité Premium',
            description:
              'Nous garantissons les plus hauts standards de qualité pour tous nos produits',
          },
          fast: {
            title: 'Livraison rapide',
            description: 'Expédition rapide et fiable pour recevoir vos commandes rapidement',
          },
          support: {
            title: 'Support 24/7',
            description: 'Notre équipe de service client est toujours là pour vous aider',
          },
        },
        newsletter: {
          title: 'Restez informé',
          subtitle: 'Abonnez-vous à notre newsletter pour les dernières mises à jour et offres',
          subscribe: "S'abonner",
        },
        table: {
          headers: {
            product: 'Produit',
            qty: 'Qté',
            price: 'Prix',
          },
        },
      },
      vat: {
        countryCode: 'Code pays',
        vatNumber: 'Numéro de TVA',
        validate: 'Valider',
        validating: 'Validation en cours...',
        companyName: "Nom de l'entreprise:",
        address: 'Adresse:',
        reverseCharge: 'Reverse Charge:',
        reverseChargeApplies: "0% TVA (s'applique)",
        standardVatRate: 'Taux de TVA standard',
        clearAndStartOver: 'Effacer et recommencer',
        validationHelp: {
          title: 'Aide à la validation TVA',
          description:
            'Si vous ne pouvez pas fournir un numéro de TVA valide, vous pouvez continuer en tant que client privé ou contacter notre équipe de support.',
        },
        countries: {
          AT: 'Autriche (AT)',
          BE: 'Belgique (BE)',
          BG: 'Bulgarie (BG)',
          HR: 'Croatie (HR)',
          CY: 'Chypre (CY)',
          CZ: 'République tchèque (CZ)',
          DK: 'Danemark (DK)',
          DE: 'Allemagne (DE)',
          EE: 'Estonie (EE)',
          FI: 'Finlande (FI)',
          FR: 'France (FR)',
          GR: 'Grèce (GR)',
          HU: 'Hongrie (HU)',
          IE: 'Irlande (IE)',
          IT: 'Italie (IT)',
          LV: 'Lettonie (LV)',
          LT: 'Lituanie (LT)',
          LU: 'Luxembourg (LU)',
          MT: 'Malte (MT)',
          NL: 'Pays-Bas (NL)',
          PL: 'Pologne (PL)',
          PT: 'Portugal (PT)',
          RO: 'Roumanie (RO)',
          SK: 'Slovaquie (SK)',
          SI: 'Slovénie (SI)',
          ES: 'Espagne (ES)',
          SE: 'Suède (SE)',
        },
      },
      legal: {
        checkout: {
          title: 'Conditions',
          subtitle: 'Veuillez accepter les conditions requises pour continuer',
          requiredFields: '* Champs obligatoires',
          back: 'Retour',
          continueToPayment: 'Continuer vers le paiement',
          processing: 'Traitement en cours...',
          acceptTerms: "J'accepte les conditions générales",
          acceptPrivacy: "J'accepte la politique de confidentialité",
          understandWithdrawal: 'Je comprends mon droit de rétractation (14 jours)',
          acceptTermsError:
            'Veuillez accepter les conditions générales et la politique de confidentialité',
          acceptTermsSuccess: 'Conditions acceptées !',
          saveError: "Erreur lors de la sauvegarde de l'acceptation des conditions",
          generalError: "Une erreur s'est produite. Veuillez réessayer plus tard.",
        },
        checkout: {
          header: {
            title: 'Finalisation de commande',
            breadcrumb: {
              shop: 'Boutique',
              cart: 'Panier',
              checkout: '/ Paiement',
            },
          },
          steps: {
            shippingAddress: 'Adresse de livraison',
            shippingMethod: 'Méthode de livraison',
            orderReview: 'Révision de commande',
          },
          form: {
            labels: {
              firstName: 'Prénom *',
              lastName: 'Nom *',
              streetAddress: 'Adresse *',
              postalCode: 'Code postal *',
              city: 'Ville *',
              country: 'Pays *',
            },
            placeholders: {
              firstName: 'Jean',
              lastName: 'Dupont',
              streetAddress: '123 Rue Principale',
              postalCode: '75001',
              city: 'Paris',
            },
            countries: {
              germany: 'Allemagne',
              austria: 'Autriche',
              belgium: 'Belgique',
              france: 'France',
              netherlands: 'Pays-Bas',
            },
            required: '* Obligatoire',
            description: 'Veuillez saisir votre adresse de livraison',
          },
          validation: {
            firstNameRequired: 'Le prénom est obligatoire',
            lastNameRequired: 'Le nom est obligatoire',
            streetRequired: "L'adresse est obligatoire",
            cityRequired: 'La ville est obligatoire',
            postalCodeRequired: 'Le code postal est obligatoire',
            countryRequired: 'Le pays est obligatoire',
            invalidPostalCode: 'Code postal français invalide (format: 75001)',
          },
          shipping: {
            title: 'Méthode de livraison',
            description: 'Choisissez votre méthode de livraison préférée',
            deliveryTime: '⏱️ Délai de livraison: environ {{days}} jour(s) ouvrable(s)',
          },
          orderReview: {
            title: 'Vérification & Mode de paiement',
            shippingAddress: 'Adresse de livraison',
            shippingMethod: 'Méthode de livraison',
            paymentMethod: 'Mode de paiement',
            edit: '✏️ Modifier',
          },
          orderSummary: {
            title: 'Résumé de commande',
            netto: 'Net:',
            vat: 'TVA (20%):',
            shipping: 'Livraison:',
            total: 'Total:',
            trustBadges: {
              ssl: 'SSL chiffré',
              returns: '30 jours retour',
              insured: 'Livraison assurée',
            },
          },
          terms: {
            acceptText: "J'accepte les",
            termsLink: 'conditions générales',
            and: 'et la',
            privacyLink: 'politique de confidentialité',
            required: '*',
          },
          compliance: {
            title: "Réglementation sur l'affichage des prix",
            content:
              'Tous les prix affichés sont des prix finaux et incluent déjà la taxe sur la valeur ajoutée (TVA) légale de 20%.',
          },
          buttons: {
            backToCart: '← Retour au panier',
            continueToShipping: 'Continuer vers la livraison →',
            backToAddress: "← Retour à l'adresse",
            continueToReview: 'Continuer vers la vérification →',
            backToShipping: '← Retour à la livraison',
            processing: 'Commande en cours de traitement...',
            completeOrder: 'Finaliser la commande',
          },
        },
        termsAndConditions: {
          title: 'Conditions générales',
          understood: 'Compris',
          sections: {
            general: {
              title: '1. Dispositions générales',
              content:
                "Ces conditions générales régissent la relation entre l'exploitant de cette boutique en ligne et l'acheteur.",
            },
            products: {
              title: '2. Descriptions de produits',
              content:
                "Toutes les descriptions de produits sont des offres de vente. Un contrat n'est conclu que lorsque vous passez une commande et que nous l'acceptons.",
            },
            pricing: {
              title: '3. Prix et conditions de paiement',
              content:
                'Tous les prix incluent la TVA applicable. Les frais de port sont calculés séparément et affichés lors du paiement.',
            },
            delivery: {
              title: '4. Livraison',
              content:
                "Les délais de livraison ne sont pas contraignants. Nous ne sommes responsables des retards qu'en cas de faute de notre part.",
            },
            withdrawal: {
              title: '5. Droit de rétractation',
              content:
                'Vous avez un droit de rétractation de 14 jours à compter de la réception des marchandises. Voir ci-dessous pour les détails.',
            },
            liability: {
              title: '6. Responsabilité',
              content:
                "La responsabilité des dommages est limitée aux dommages directs jusqu'au montant du prix d'achat.",
            },
            privacy: {
              title: '7. Protection des données',
              content: 'Voir la politique de confidentialité pour le traitement de vos données.',
            },
            final: {
              title: '8. Dispositions finales',
              content:
                "Le droit allemand s'applique. Le tribunal compétent est le siège de l'entreprise.",
            },
          },
        },
        privacyPolicy: {
          title: 'Politique de confidentialité',
          understood: 'Compris',
          sections: {
            responsible: {
              title: '1. Responsable',
              content:
                "L'exploitant de cette boutique est responsable du traitement des données (voir mentions légales).",
            },
            collection: {
              title: '2. Collecte et traitement',
              content:
                "Nous ne collectons vos données que pour le traitement de votre achat et l'expédition.",
            },
            storage: {
              title: '3. Durée de stockage',
              content:
                'Les données personnelles sont stockées pendant 10 ans pour remplir les obligations fiscales.',
            },
            rights: {
              title: '4. Vos droits',
              content:
                "Vous avez le droit à l'information, à la rectification, à l'effacement et à la portabilité des données.",
            },
            cookies: {
              title: '5. Cookies',
              content:
                "Nous utilisons des cookies techniquement nécessaires. D'autres cookies sont stockés avec votre consentement.",
            },
            security: {
              title: '6. Sécurité',
              content:
                'Nous protégeons vos données par le chiffrement et la transmission sécurisée.',
            },
            contact: {
              title: '7. Délégué à la protection des données',
              content: 'Pour les questions : protection-des-donnees@example.com',
            },
          },
        },
        withdrawalRights: {
          title: 'Droit de rétractation (14 jours)',
          understood: 'Compris',
          sections: {
            yourRights: {
              title: 'Votre droit de rétractation',
              content:
                'Vous avez le droit de vous rétracter de votre achat dans les 14 jours suivant la réception des marchandises, sans donner de raison.',
            },
            deadlines: {
              title: 'Délais de rétractation',
              start: 'Début : Jour suivant la réception des marchandises',
              duration: 'Durée : 14 jours',
              form: 'Forme : Une simple notification écrite par e-mail suffit',
            },
            exceptions: {
              title: 'Exceptions',
              intro: "Le droit de rétractation ne s'applique PAS à :",
              digital: 'Contenu numérique après téléchargement',
              customized: 'Marchandises sur mesure ou personnalisées',
              damaged: 'Marchandises endommagées après livraison (votre faute)',
            },
            returnProcess: {
              title: 'Procédure de retour',
              content:
                "Retournez immédiatement les marchandises. Les frais de port sont à la charge de l'acheteur (sauf en cas de retour justifié).",
            },
            contact: {
              title: 'Contact',
              content: 'Envoyez les rétractations à : retractation@example.com',
            },
            legalBasis: 'Base juridique : §§ 355-359 BGB (loi sur la vente à distance)',
          },
        },
      },
    },
    es: {
      common: {
        loading: 'Cargando...',
        error: 'Ocurrió un error',
        save: 'Guardar',
        cancel: 'Cancelar',
        delete: 'Eliminar',
        edit: 'Editar',
        add: 'Agregar',
        search: 'Buscar',
        filter: 'Filtrar',
        sort: 'Ordenar',
        next: 'Siguiente',
        previous: 'Anterior',
        page: 'Página',
        of: 'de',
        items: 'elementos',
      },
      navigation: {
        home: 'Inicio',
        products: 'Productos',
        categories: 'Categorías',
        cart: 'Carrito',
        dashboard: 'Panel',
        tenants: 'Inquilinos',
        login: 'Iniciar sesión',
        logout: 'Cerrar sesión',
      },
      notFound: {
        title: 'Página no encontrada',
        message: 'La página que buscas no existe.',
        goHome: 'Volver al inicio',
      },
      app: {
        skipToMain: 'Saltar al contenido principal',
        brand: 'B2Connect',
        admin: 'Admin',
        services: 'Servicios',
        branding: 'Marca',
        design: 'Diseño',
        marketing: 'Marketing',
        advertisement: 'Publicidad',
        company: 'Empresa',
        aboutUs: 'Sobre nosotros',
        contact: 'Contacto',
        jobs: 'Empleos',
        pressKit: 'Kit de prensa',
        legal: 'Legal',
        termsOfUse: 'Términos de uso',
        privacyPolicy: 'Política de privacidad',
        cookiePolicy: 'Política de cookies',
        newsletter: 'Newsletter',
        enterEmail: 'Ingresa tu dirección de correo electrónico',
        subscribe: 'Suscribirse',
      },
      home: {
        hero: {
          title: 'Bienvenido a B2Connect Store',
          subtitle: 'Descubre productos increíbles con integración perfecta y servicio excepcional',
          shopNow: 'Comprar ahora',
          browseCategories: 'Explorar categorías',
        },
        featuredProducts: {
          title: 'Productos destacados',
          viewAll: 'Ver todos los productos',
        },
        categories: {
          title: 'Comprar por categoría',
        },
        features: {
          title: 'Por qué elegirnos',
          quality: {
            title: 'Calidad Premium',
            description:
              'Garantizamos los más altos estándares de calidad para todos nuestros productos',
          },
          fast: {
            title: 'Entrega rápida',
            description: 'Envío rápido y confiable para llevar tus pedidos rápidamente',
          },
          support: {
            title: 'Soporte 24/7',
            description: 'Nuestro equipo de servicio al cliente siempre está aquí para ayudarte',
          },
        },
        newsletter: {
          title: 'Mantente actualizado',
          subtitle: 'Suscríbete a nuestro newsletter para las últimas actualizaciones y ofertas',
          subscribe: 'Suscribirse',
        },
        table: {
          headers: {
            product: 'Producto',
            qty: 'Cant.',
            price: 'Precio',
          },
        },
      },
      vat: {
        countryCode: 'Código de país',
        vatNumber: 'Número de IVA',
        validate: 'Validar',
        validating: 'Validando...',
        companyName: 'Nombre de la empresa:',
        address: 'Dirección:',
        reverseCharge: 'Reverse Charge:',
        reverseChargeApplies: '0% IVA (aplica)',
        standardVatRate: 'Tasa de IVA estándar',
        clearAndStartOver: 'Limpiar y empezar de nuevo',
        validationHelp: {
          title: 'Ayuda de validación de IVA',
          description:
            'Si no puedes proporcionar un número de IVA válido, puedes continuar como cliente privado o contactar a nuestro equipo de soporte.',
        },
        countries: {
          AT: 'Austria (AT)',
          BE: 'Bélgica (BE)',
          BG: 'Bulgaria (BG)',
          HR: 'Croacia (HR)',
          CY: 'Chipre (CY)',
          CZ: 'República Checa (CZ)',
          DK: 'Dinamarca (DK)',
          DE: 'Alemania (DE)',
          EE: 'Estonia (EE)',
          FI: 'Finlandia (FI)',
          FR: 'Francia (FR)',
          GR: 'Grecia (GR)',
          HU: 'Hungría (HU)',
          IE: 'Irlanda (IE)',
          IT: 'Italia (IT)',
          LV: 'Letonia (LV)',
          LT: 'Lituania (LT)',
          LU: 'Luxemburgo (LU)',
          MT: 'Malta (MT)',
          NL: 'Países Bajos (NL)',
          PL: 'Polonia (PL)',
          PT: 'Portugal (PT)',
          RO: 'Rumania (RO)',
          SK: 'Eslovaquia (SK)',
          SI: 'Eslovenia (SI)',
          ES: 'España (ES)',
          SE: 'Suecia (SE)',
        },
      },
      legal: {
        checkout: {
          title: 'Condiciones',
          subtitle: 'Por favor, acepte las condiciones requeridas para continuar',
          requiredFields: '* Campos obligatorios',
          back: 'Atrás',
          continueToPayment: 'Continuar al pago',
          processing: 'Procesando...',
          acceptTerms: 'Acepto las condiciones generales',
          acceptPrivacy: 'Acepto la política de privacidad',
          understandWithdrawal: 'Entiendo mi derecho de desistimiento (14 días)',
          acceptTermsError:
            'Por favor, acepte las condiciones generales y la política de privacidad',
          acceptTermsSuccess: '¡Condiciones aceptadas!',
          saveError: 'Error al guardar la aceptación de condiciones',
          generalError: 'Se produjo un error. Por favor, inténtelo de nuevo más tarde.',
        },
        checkout: {
          header: {
            title: 'Finalización del pedido',
            breadcrumb: {
              shop: 'Tienda',
              cart: 'Carrito',
              checkout: '/ Pago',
            },
          },
          steps: {
            shippingAddress: 'Dirección de envío',
            shippingMethod: 'Método de envío',
            orderReview: 'Revisión del pedido',
          },
          form: {
            labels: {
              firstName: 'Nombre *',
              lastName: 'Apellidos *',
              streetAddress: 'Dirección *',
              postalCode: 'Código postal *',
              city: 'Ciudad *',
              country: 'País *',
            },
            placeholders: {
              firstName: 'Juan',
              lastName: 'García',
              streetAddress: 'Calle Principal 123',
              postalCode: '28001',
              city: 'Madrid',
            },
            countries: {
              germany: 'Alemania',
              austria: 'Austria',
              belgium: 'Bélgica',
              france: 'Francia',
              netherlands: 'Países Bajos',
            },
            required: '* Obligatorio',
            description: 'Por favor, introduzca su dirección de envío',
          },
          validation: {
            firstNameRequired: 'El nombre es obligatorio',
            lastNameRequired: 'Los apellidos son obligatorios',
            streetRequired: 'La dirección es obligatoria',
            cityRequired: 'La ciudad es obligatoria',
            postalCodeRequired: 'El código postal es obligatorio',
            countryRequired: 'El país es obligatorio',
            invalidPostalCode: 'Código postal español inválido (formato: 28001)',
          },
          shipping: {
            title: 'Método de envío',
            description: 'Seleccione su método de envío preferido',
            deliveryTime: '⏱️ Tiempo de entrega: aprox. {{days}} día(s) laborable(s)',
          },
          orderReview: {
            title: 'Verificación & Método de pago',
            shippingAddress: 'Dirección de envío',
            shippingMethod: 'Método de envío',
            paymentMethod: 'Método de pago',
            edit: '✏️ Editar',
          },
          orderSummary: {
            title: 'Resumen del pedido',
            netto: 'Neto:',
            vat: 'IVA (21%):',
            shipping: 'Envío:',
            total: 'Total:',
            trustBadges: {
              ssl: 'SSL encriptado',
              returns: '30 días devolución',
              insured: 'Envío asegurado',
            },
          },
          terms: {
            acceptText: 'Acepto las',
            termsLink: 'condiciones generales',
            and: 'y la',
            privacyLink: 'política de privacidad',
            required: '*',
          },
          compliance: {
            title: 'Reglamento de indicación de precios',
            content:
              'Todos los precios mostrados son precios finales e incluyen ya el impuesto sobre el valor añadido (IVA) legal del 21%.',
          },
          buttons: {
            backToCart: '← Volver al carrito',
            continueToShipping: 'Continuar al envío →',
            backToAddress: '← Volver a la dirección',
            continueToReview: 'Continuar a la verificación →',
            backToShipping: '← Volver al envío',
            processing: 'Procesando pedido...',
            completeOrder: 'Completar pedido',
          },
        },
        termsAndConditions: {
          title: 'Condiciones generales',
          understood: 'Entendido',
          sections: {
            general: {
              title: '1. Disposiciones generales',
              content:
                'Estas condiciones generales regulan la relación entre el operador de esta tienda online y el comprador.',
            },
            products: {
              title: '2. Descripciones de productos',
              content:
                'Todas las descripciones de productos son ofertas de venta. Un contrato solo se concluye cuando usted realiza un pedido y nosotros lo aceptamos.',
            },
            pricing: {
              title: '3. Precios y condiciones de pago',
              content:
                'Todos los precios incluyen el IVA aplicable. Los gastos de envío se calculan por separado y se muestran en el checkout.',
            },
            delivery: {
              title: '4. Entrega',
              content:
                'Los plazos de entrega no son vinculantes. Solo somos responsables de los retrasos por culpa nuestra.',
            },
            withdrawal: {
              title: '5. Derecho de desistimiento',
              content:
                'Tiene un derecho de desistimiento de 14 días desde la recepción de la mercancía. Ver detalles abajo.',
            },
            liability: {
              title: '6. Responsabilidad',
              content:
                'La responsabilidad por daños se limita a daños directos hasta el importe del precio de compra.',
            },
            privacy: {
              title: '7. Protección de datos',
              content: 'Ver política de privacidad para el tratamiento de sus datos.',
            },
            final: {
              title: '8. Disposiciones finales',
              content:
                'Se aplica la ley alemana. El lugar de jurisdicción es la sede de la empresa.',
            },
          },
        },
        privacyPolicy: {
          title: 'Política de privacidad',
          understood: 'Entendido',
          sections: {
            responsible: {
              title: '1. Responsable',
              content:
                'El operador de esta tienda es responsable del procesamiento de datos (ver imprint).',
            },
            collection: {
              title: '2. Recogida y procesamiento',
              content: 'Solo recopilamos sus datos para el procesamiento de su compra y envío.',
            },
            storage: {
              title: '3. Duración del almacenamiento',
              content:
                'Los datos personales se almacenan durante 10 años para cumplir con las obligaciones fiscales.',
            },
            rights: {
              title: '4. Sus derechos',
              content:
                'Tiene derecho a la información, rectificación, eliminación y portabilidad de datos.',
            },
            cookies: {
              title: '5. Cookies',
              content:
                'Utilizamos cookies técnicamente necesarias. Otras cookies se almacenan con su consentimiento.',
            },
            security: {
              title: '6. Seguridad',
              content: 'Protegemos sus datos mediante cifrado y transmisión segura.',
            },
            contact: {
              title: '7. Delegado de protección de datos',
              content: 'Para preguntas: proteccion-datos@example.com',
            },
          },
        },
        withdrawalRights: {
          title: 'Derecho de desistimiento (14 días)',
          understood: 'Entendido',
          sections: {
            yourRights: {
              title: 'Su derecho de desistimiento',
              content:
                'Tiene derecho a desistir de su compra dentro de 14 días desde la recepción de la mercancía sin dar motivo alguno.',
            },
            deadlines: {
              title: 'Plazos de desistimiento',
              start: 'Inicio: Día siguiente a la recepción de la mercancía',
              duration: 'Duración: 14 días',
              form: 'Forma: Una simple notificación escrita por correo electrónico es suficiente',
            },
            exceptions: {
              title: 'Excepciones',
              intro: 'El derecho de desistimiento NO se aplica a:',
              digital: 'Contenido digital después de la descarga',
              customized: 'Mercancías personalizadas o a medida',
              damaged: 'Mercancías dañadas después de la entrega (su culpa)',
            },
            returnProcess: {
              title: 'Proceso de devolución',
              content:
                'Devuelva inmediatamente la mercancía. Los gastos de envío corren a cargo del comprador (excepto en caso de devolución justificada).',
            },
            contact: {
              title: 'Contacto',
              content: 'Envíe desistimientos a: desistimiento@example.com',
            },
            legalBasis: 'Base legal: §§ 355-359 BGB (Ley de venta a distancia)',
          },
        },
      },
    },
    it: {
      common: {
        loading: 'Caricamento...',
        error: 'Si è verificato un errore',
        save: 'Salva',
        cancel: 'Annulla',
        delete: 'Elimina',
        edit: 'Modifica',
        add: 'Aggiungi',
        search: 'Cerca',
        filter: 'Filtra',
        sort: 'Ordina',
        next: 'Successivo',
        previous: 'Precedente',
        page: 'Pagina',
        of: 'di',
        items: 'elementi',
      },
      navigation: {
        home: 'Home',
        products: 'Prodotti',
        categories: 'Categorie',
        cart: 'Carrello',
        dashboard: 'Dashboard',
        tenants: 'Affittuari',
        login: 'Accedi',
        logout: 'Esci',
      },
      notFound: {
        title: 'Pagina non trovata',
        message: 'La pagina che stai cercando non esiste.',
        goHome: 'Torna alla home',
      },
      app: {
        skipToMain: 'Vai al contenuto principale',
        brand: 'B2Connect',
        admin: 'Admin',
        services: 'Servizi',
        branding: 'Branding',
        design: 'Design',
        marketing: 'Marketing',
        advertisement: 'Pubblicità',
        company: 'Azienda',
        aboutUs: 'Chi siamo',
        contact: 'Contatto',
        jobs: 'Lavori',
        pressKit: 'Press kit',
        legal: 'Legale',
        termsOfUse: 'Termini di utilizzo',
        privacyPolicy: 'Informativa sulla privacy',
        cookiePolicy: 'Politica sui cookie',
        newsletter: 'Newsletter',
        enterEmail: 'Inserisci il tuo indirizzo email',
        subscribe: 'Iscriviti',
      },
      home: {
        hero: {
          title: 'Benvenuto su B2Connect Store',
          subtitle: 'Scopri prodotti straordinari con integrazione perfetta e servizio eccezionale',
          shopNow: 'Acquista ora',
          browseCategories: 'Sfoglia categorie',
        },
        featuredProducts: {
          title: 'Prodotti in evidenza',
          viewAll: 'Vedi tutti i prodotti',
        },
        categories: {
          title: 'Acquista per categoria',
        },
        features: {
          title: 'Perché sceglierci',
          quality: {
            title: 'Qualità Premium',
            description: 'Garantiamo i più alti standard di qualità per tutti i nostri prodotti',
          },
          fast: {
            title: 'Consegna veloce',
            description: 'Spedizione veloce e affidabile per ricevere i tuoi ordini rapidamente',
          },
          support: {
            title: 'Supporto 24/7',
            description: 'Il nostro team di assistenza clienti è sempre qui per aiutarti',
          },
        },
        newsletter: {
          title: 'Rimani aggiornato',
          subtitle: 'Iscriviti alla nostra newsletter per gli ultimi aggiornamenti e offerte',
          subscribe: 'Iscriviti',
        },
        table: {
          headers: {
            product: 'Prodotto',
            qty: 'Qtà',
            price: 'Prezzo',
          },
        },
      },
      vat: {
        countryCode: 'Codice paese',
        vatNumber: 'Numero IVA',
        validate: 'Convalida',
        validating: 'Convalida in corso...',
        companyName: 'Nome azienda:',
        address: 'Indirizzo:',
        reverseCharge: 'Reverse Charge:',
        reverseChargeApplies: '0% IVA (applica)',
        standardVatRate: 'Aliquota IVA standard',
        clearAndStartOver: 'Cancella e ricomincia',
        validationHelp: {
          title: 'Aiuto validazione IVA',
          description:
            'Se non puoi fornire un numero IVA valido, puoi continuare come cliente privato o contattare il nostro team di supporto.',
        },
        countries: {
          AT: 'Austria (AT)',
          BE: 'Belgio (BE)',
          BG: 'Bulgaria (BG)',
          HR: 'Croazia (HR)',
          CY: 'Cipro (CY)',
          CZ: 'Repubblica Ceca (CZ)',
          DK: 'Danimarca (DK)',
          DE: 'Germania (DE)',
          EE: 'Estonia (EE)',
          FI: 'Finlandia (FI)',
          FR: 'Francia (FR)',
          GR: 'Grecia (GR)',
          HU: 'Ungheria (HU)',
          IE: 'Irlanda (IE)',
          IT: 'Italia (IT)',
          LV: 'Lettonia (LV)',
          LT: 'Lituania (LT)',
          LU: 'Lussemburgo (LU)',
          MT: 'Malta (MT)',
          NL: 'Paesi Bassi (NL)',
          PL: 'Polonia (PL)',
          PT: 'Portogallo (PT)',
          RO: 'Romania (RO)',
          SK: 'Slovacchia (SK)',
          SI: 'Slovenia (SI)',
          ES: 'Spagna (ES)',
          SE: 'Svezia (SE)',
        },
      },
      legal: {
        checkout: {
          title: 'Condizioni',
          subtitle: 'Si prega di accettare le condizioni richieste per continuare',
          requiredFields: '* Campi obbligatori',
          back: 'Indietro',
          continueToPayment: 'Continua al pagamento',
          processing: 'Elaborazione...',
          acceptTerms: 'Accetto le condizioni generali',
          acceptPrivacy: "Accetto l'informativa sulla privacy",
          understandWithdrawal: 'Comprendo il mio diritto di recesso (14 giorni)',
          acceptTermsError:
            "Si prega di accettare le condizioni generali e l'informativa sulla privacy",
          acceptTermsSuccess: 'Condizioni accettate!',
          saveError: "Errore nel salvare l'accettazione delle condizioni",
          generalError: 'Si è verificato un errore. Si prega di riprovare più tardi.',
        },
        checkout: {
          header: {
            title: 'Completamento ordine',
            breadcrumb: {
              shop: 'Negozio',
              cart: 'Carrello',
              checkout: '/ Cassa',
            },
          },
          steps: {
            shippingAddress: 'Indirizzo di spedizione',
            shippingMethod: 'Metodo di spedizione',
            orderReview: 'Revisione ordine',
          },
          form: {
            labels: {
              firstName: 'Nome *',
              lastName: 'Cognome *',
              streetAddress: 'Indirizzo *',
              postalCode: 'Codice postale *',
              city: 'Città *',
              country: 'Paese *',
            },
            placeholders: {
              firstName: 'Mario',
              lastName: 'Rossi',
              streetAddress: 'Via Principale 123',
              postalCode: '00100',
              city: 'Roma',
            },
            countries: {
              germany: 'Germania',
              austria: 'Austria',
              belgium: 'Belgio',
              france: 'Francia',
              netherlands: 'Paesi Bassi',
            },
            required: '* Obbligatorio',
            description: 'Si prega di inserire il proprio indirizzo di spedizione',
          },
          validation: {
            firstNameRequired: 'Il nome è obbligatorio',
            lastNameRequired: 'Il cognome è obbligatorio',
            streetRequired: "L'indirizzo è obbligatorio",
            cityRequired: 'La città è obbligatoria',
            postalCodeRequired: 'Il codice postale è obbligatorio',
            countryRequired: 'Il paese è obbligatorio',
            invalidPostalCode: 'Codice postale italiano non valido (formato: 00100)',
          },
          shipping: {
            title: 'Metodo di spedizione',
            description: 'Seleziona il metodo di spedizione preferito',
            deliveryTime: '⏱️ Tempo di consegna: circa {{days}} giorno(i) lavorativo(i)',
          },
          orderReview: {
            title: 'Verifica & Metodo di pagamento',
            shippingAddress: 'Indirizzo di spedizione',
            shippingMethod: 'Metodo di spedizione',
            paymentMethod: 'Metodo di pagamento',
            edit: '✏️ Modifica',
          },
          orderSummary: {
            title: 'Riepilogo ordine',
            netto: 'Netto:',
            vat: 'IVA (22%):',
            shipping: 'Spedizione:',
            total: 'Totale:',
            trustBadges: {
              ssl: 'SSL criptato',
              returns: '30 giorni reso',
              insured: 'Spedizione assicurata',
            },
          },
          terms: {
            acceptText: 'Accetto le',
            termsLink: 'condizioni generali',
            and: "e l'",
            privacyLink: 'informativa sulla privacy',
            required: '*',
          },
          compliance: {
            title: "Regolamento sull'indicazione dei prezzi",
            content:
              "Tutti i prezzi indicati sono prezzi finali e includono già l'imposta sul valore aggiunto (IVA) legale del 22%.",
          },
          buttons: {
            backToCart: '← Torna al carrello',
            continueToShipping: 'Continua alla spedizione →',
            backToAddress: "← Torna all'indirizzo",
            continueToReview: 'Continua alla verifica →',
            backToShipping: '← Torna alla spedizione',
            processing: 'Elaborazione ordine...',
            completeOrder: 'Completa ordine',
          },
        },
        termsAndConditions: {
          title: 'Condizioni generali',
          understood: 'Capito',
          sections: {
            general: {
              title: '1. Disposizioni generali',
              content:
                "Queste condizioni generali regolano il rapporto tra l'operatore di questo negozio online e l'acquirente.",
            },
            products: {
              title: '2. Descrizioni dei prodotti',
              content:
                'Tutte le descrizioni dei prodotti sono offerte di vendita. Un contratto si conclude solo quando effettui un ordine e noi lo accettiamo.',
            },
            pricing: {
              title: '3. Prezzi e condizioni di pagamento',
              content:
                "Tutti i prezzi includono l'IVA applicabile. Le spese di spedizione vengono calcolate separatamente e mostrate nel checkout.",
            },
            delivery: {
              title: '4. Consegna',
              content:
                'I tempi di consegna non sono vincolanti. Siamo responsabili solo dei ritardi per colpa nostra.',
            },
            withdrawal: {
              title: '5. Diritto di recesso',
              content:
                'Hai un diritto di recesso di 14 giorni dalla ricezione della merce. Vedi dettagli sotto.',
            },
            liability: {
              title: '6. Responsabilità',
              content:
                "La responsabilità per danni si limita ai danni diretti fino all'importo del prezzo di acquisto.",
            },
            privacy: {
              title: '7. Protezione dei dati',
              content: 'Vedi informativa sulla privacy per il trattamento dei tuoi dati.',
            },
            final: {
              title: '8. Disposizioni finali',
              content: "Si applica la legge italiana. Il foro competente è la sede dell'azienda.",
            },
          },
        },
        privacyPolicy: {
          title: 'Informativa sulla privacy',
          understood: 'Capito',
          sections: {
            responsible: {
              title: '1. Responsabile',
              content:
                "L'operatore di questo negozio è responsabile del trattamento dei dati (vedi imprint).",
            },
            collection: {
              title: '2. Raccolta e trattamento',
              content:
                "Raccogliamo i tuoi dati solo per l'elaborazione del tuo acquisto e spedizione.",
            },
            storage: {
              title: '3. Durata della conservazione',
              content:
                'I dati personali vengono conservati per 10 anni per adempiere agli obblighi fiscali.',
            },
            rights: {
              title: '4. I tuoi diritti',
              content:
                "Hai diritto all'informazione, rettifica, cancellazione e portabilità dei dati.",
            },
            cookies: {
              title: '5. Cookie',
              content:
                'Utilizziamo cookie tecnicamente necessari. Altri cookie vengono memorizzati con il tuo consenso.',
            },
            security: {
              title: '6. Sicurezza',
              content: 'Proteggiamo i tuoi dati mediante crittografia e trasmissione sicura.',
            },
            contact: {
              title: '7. Responsabile della protezione dei dati',
              content: 'Per domande: privacy@example.com',
            },
          },
        },
        withdrawalRights: {
          title: 'Diritto di recesso (14 giorni)',
          understood: 'Capito',
          sections: {
            yourRights: {
              title: 'Il tuo diritto di recesso',
              content:
                'Hai diritto di recedere dal tuo acquisto entro 14 giorni dalla ricezione della merce senza fornire alcuna motivazione.',
            },
            deadlines: {
              title: 'Termini di recesso',
              start: 'Inizio: Giorno successivo alla ricezione della merce',
              duration: 'Durata: 14 giorni',
              form: 'Forma: Una semplice comunicazione scritta via email è sufficiente',
            },
            exceptions: {
              title: 'Eccezioni',
              intro: 'Il diritto di recesso NON si applica a:',
              digital: 'Contenuto digitale dopo il download',
              customized: 'Merci personalizzate o su misura',
              damaged: 'Merci danneggiate dopo la consegna (tua colpa)',
            },
            returnProcess: {
              title: 'Processo di reso',
              content:
                "Restituisci immediatamente la merce. Le spese di spedizione sono a carico dell'acquirente (tranne in caso di reso giustificato).",
            },
            contact: {
              title: 'Contatto',
              content: 'Invia recessi a: recesso@example.com',
            },
            legalBasis: 'Base legale: Art. 52-58 Codice del Consumo',
          },
        },
      },
    },
    pt: {
      common: {
        loading: 'Carregando...',
        error: 'Ocorreu um erro',
        save: 'Salvar',
        cancel: 'Cancelar',
        delete: 'Excluir',
        edit: 'Editar',
        add: 'Adicionar',
        search: 'Buscar',
        filter: 'Filtrar',
        sort: 'Ordenar',
        next: 'Próximo',
        previous: 'Anterior',
        page: 'Página',
        of: 'de',
        items: 'itens',
      },
      navigation: {
        home: 'Início',
        products: 'Produtos',
        categories: 'Categorias',
        cart: 'Carrinho',
        dashboard: 'Painel',
        tenants: 'Inquilinos',
        login: 'Entrar',
        logout: 'Sair',
      },
      notFound: {
        title: 'Página não encontrada',
        message: 'A página que você está procurando não existe.',
        goHome: 'Voltar ao início',
      },
      app: {
        skipToMain: 'Pular para o conteúdo principal',
        brand: 'B2Connect',
        admin: 'Admin',
        services: 'Serviços',
        branding: 'Marca',
        design: 'Design',
        marketing: 'Marketing',
        advertisement: 'Publicidade',
        company: 'Empresa',
        aboutUs: 'Sobre nós',
        contact: 'Contato',
        jobs: 'Empregos',
        pressKit: 'Kit de imprensa',
        legal: 'Legal',
        termsOfUse: 'Termos de uso',
        privacyPolicy: 'Política de privacidade',
        cookiePolicy: 'Política de cookies',
        newsletter: 'Newsletter',
        enterEmail: 'Digite seu endereço de e-mail',
        subscribe: 'Inscrever-se',
      },
      home: {
        hero: {
          title: 'Bem-vindo à B2Connect Store',
          subtitle: 'Descubra produtos incríveis com integração perfeita e serviço excepcional',
          shopNow: 'Comprar agora',
          browseCategories: 'Navegar categorias',
        },
        featuredProducts: {
          title: 'Produtos em destaque',
          viewAll: 'Ver todos os produtos',
        },
        categories: {
          title: 'Comprar por categoria',
        },
        features: {
          title: 'Por que nos escolher',
          quality: {
            title: 'Qualidade Premium',
            description:
              'Garantimos os mais altos padrões de qualidade para todos os nossos produtos',
          },
          fast: {
            title: 'Entrega rápida',
            description: 'Envio rápido e confiável para levar seus pedidos rapidamente',
          },
          support: {
            title: 'Suporte 24/7',
            description: 'Nossa equipe de atendimento ao cliente está sempre aqui para ajudá-lo',
          },
        },
        newsletter: {
          title: 'Mantenha-se atualizado',
          subtitle: 'Inscreva-se em nossa newsletter para as últimas atualizações e ofertas',
          subscribe: 'Inscrever-se',
        },
        table: {
          headers: {
            product: 'Produto',
            qty: 'Qtd.',
            price: 'Preço',
          },
        },
      },
      vat: {
        countryCode: 'Código do país',
        vatNumber: 'Número de IVA',
        validate: 'Validar',
        validating: 'Validando...',
        companyName: 'Nome da empresa:',
        address: 'Endereço:',
        reverseCharge: 'Reverse Charge:',
        reverseChargeApplies: '0% IVA (aplica)',
        standardVatRate: 'Taxa de IVA padrão',
        clearAndStartOver: 'Limpar e começar novamente',
        validationHelp: {
          title: 'Ajuda de validação de IVA',
          description:
            'Se não puder fornecer um número de IVA válido, pode continuar como cliente privado ou contactar a nossa equipa de suporte.',
        },
        countries: {
          AT: 'Áustria (AT)',
          BE: 'Bélgica (BE)',
          BG: 'Bulgária (BG)',
          HR: 'Croácia (HR)',
          CY: 'Chipre (CY)',
          CZ: 'República Checa (CZ)',
          DK: 'Dinamarca (DK)',
          DE: 'Alemanha (DE)',
          EE: 'Estónia (EE)',
          FI: 'Finlândia (FI)',
          FR: 'França (FR)',
          GR: 'Grécia (GR)',
          HU: 'Hungria (HU)',
          IE: 'Irlanda (IE)',
          IT: 'Itália (IT)',
          LV: 'Letónia (LV)',
          LT: 'Lituânia (LT)',
          LU: 'Luxemburgo (LU)',
          MT: 'Malta (MT)',
          NL: 'Países Baixos (NL)',
          PL: 'Polónia (PL)',
          PT: 'Portugal (PT)',
          RO: 'Roménia (RO)',
          SK: 'Eslováquia (SK)',
          SI: 'Eslovénia (SI)',
          ES: 'Espanha (ES)',
          SE: 'Suécia (SE)',
        },
      },
      legal: {
        checkout: {
          title: 'Condições',
          subtitle: 'Por favor, aceite as condições necessárias para continuar',
          requiredFields: '* Campos obrigatórios',
          back: 'Voltar',
          continueToPayment: 'Continuar para o pagamento',
          processing: 'Processando...',
          acceptTerms: 'Aceito as condições gerais',
          acceptPrivacy: 'Aceito a política de privacidade',
          understandWithdrawal: 'Compreendo o meu direito de rescisão (14 dias)',
          acceptTermsError: 'Por favor, aceite as condições gerais e a política de privacidade',
          acceptTermsSuccess: 'Condições aceites!',
          saveError: 'Erro ao guardar a aceitação das condições',
          generalError: 'Ocorreu um erro. Por favor, tente novamente mais tarde.',
        },
        checkout: {
          header: {
            title: 'Finalização do pedido',
            breadcrumb: {
              shop: 'Loja',
              cart: 'Carrinho',
              checkout: '/ Pagamento',
            },
          },
          steps: {
            shippingAddress: 'Endereço de envio',
            shippingMethod: 'Método de envio',
            orderReview: 'Revisão do pedido',
          },
          form: {
            labels: {
              firstName: 'Nome próprio *',
              lastName: 'Apelido *',
              streetAddress: 'Endereço *',
              postalCode: 'Código postal *',
              city: 'Cidade *',
              country: 'País *',
            },
            placeholders: {
              firstName: 'João',
              lastName: 'Silva',
              streetAddress: 'Rua Principal 123',
              postalCode: '1000-001',
              city: 'Lisboa',
            },
            countries: {
              germany: 'Alemanha',
              austria: 'Áustria',
              belgium: 'Bélgica',
              france: 'França',
              netherlands: 'Países Baixos',
            },
            required: '* Obrigatório',
            description: 'Por favor, introduza o seu endereço de envio',
          },
          validation: {
            firstNameRequired: 'O nome próprio é obrigatório',
            lastNameRequired: 'O apelido é obrigatório',
            streetRequired: 'O endereço é obrigatório',
            cityRequired: 'A cidade é obrigatória',
            postalCodeRequired: 'O código postal é obrigatório',
            countryRequired: 'O país é obrigatório',
            invalidPostalCode: 'Código postal português inválido (formato: 1000-001)',
          },
          shipping: {
            title: 'Método de envio',
            description: 'Selecione o seu método de envio preferido',
            deliveryTime: '⏱️ Tempo de entrega: aprox. {{days}} dia(s) útil(is)',
          },
          orderReview: {
            title: 'Verificação & Método de pagamento',
            shippingAddress: 'Endereço de envio',
            shippingMethod: 'Método de envio',
            paymentMethod: 'Método de pagamento',
            edit: '✏️ Editar',
          },
          orderSummary: {
            title: 'Resumo do pedido',
            netto: 'Líquido:',
            vat: 'IVA (23%):',
            shipping: 'Envio:',
            total: 'Total:',
            trustBadges: {
              ssl: 'SSL encriptado',
              returns: '30 dias devolução',
              insured: 'Envio seguro',
            },
          },
          terms: {
            acceptText: 'Aceito as',
            termsLink: 'condições gerais',
            and: 'e a',
            privacyLink: 'política de privacidade',
            required: '*',
          },
          compliance: {
            title: 'Regulamento de indicação de preços',
            content:
              'Todos os preços apresentados são preços finais e já incluem o imposto sobre o valor acrescentado (IVA) legal de 23%.',
          },
          buttons: {
            backToCart: '← Voltar ao carrinho',
            continueToShipping: 'Continuar para envio →',
            backToAddress: '← Voltar ao endereço',
            continueToReview: 'Continuar para verificação →',
            backToShipping: '← Voltar ao envio',
            processing: 'A processar pedido...',
            completeOrder: 'Finalizar pedido',
          },
        },
        termsAndConditions: {
          title: 'Condições gerais',
          understood: 'Entendido',
          sections: {
            general: {
              title: '1. Disposições gerais',
              content:
                'Estas condições gerais regulam a relação entre o operador desta loja online e o comprador.',
            },
            products: {
              title: '2. Descrições de produtos',
              content:
                'Todas as descrições de produtos são ofertas de venda. Um contrato só é concluído quando faz uma encomenda e nós a aceitamos.',
            },
            pricing: {
              title: '3. Preços e condições de pagamento',
              content:
                'Todos os preços incluem o IVA aplicável. Os custos de envio são calculados separadamente e mostrados no checkout.',
            },
            delivery: {
              title: '4. Entrega',
              content:
                'Os prazos de entrega não são vinculativos. Só somos responsáveis por atrasos por culpa nossa.',
            },
            withdrawal: {
              title: '5. Direito de rescisão',
              content:
                'Tem um direito de rescisão de 14 dias a partir da receção da mercadoria. Ver detalhes abaixo.',
            },
            liability: {
              title: '6. Responsabilidade',
              content:
                'A responsabilidade por danos limita-se a danos diretos até ao montante do preço de compra.',
            },
            privacy: {
              title: '7. Proteção de dados',
              content: 'Ver política de privacidade para o tratamento dos seus dados.',
            },
            final: {
              title: '8. Disposições finais',
              content: 'Aplica-se a lei portuguesa. O foro competente é a sede da empresa.',
            },
          },
        },
        privacyPolicy: {
          title: 'Política de privacidade',
          understood: 'Entendido',
          sections: {
            responsible: {
              title: '1. Responsável',
              content:
                'O operador desta loja é responsável pelo tratamento de dados (ver imprint).',
            },
            collection: {
              title: '2. Recolha e tratamento',
              content:
                'Recolhemos os seus dados apenas para o processamento da sua compra e envio.',
            },
            storage: {
              title: '3. Duração do armazenamento',
              content:
                'Os dados pessoais são armazenados durante 10 anos para cumprir obrigações fiscais.',
            },
            rights: {
              title: '4. Os seus direitos',
              content:
                'Tem direito à informação, retificação, eliminação e portabilidade de dados.',
            },
            cookies: {
              title: '5. Cookies',
              content:
                'Utilizamos cookies tecnicamente necessários. Outros cookies são armazenados com o seu consentimento.',
            },
            security: {
              title: '6. Segurança',
              content: 'Protegemos os seus dados mediante encriptação e transmissão segura.',
            },
            contact: {
              title: '7. Encarregado da proteção de dados',
              content: 'Para perguntas: protecao-dados@example.com',
            },
          },
        },
        withdrawalRights: {
          title: 'Direito de rescisão (14 dias)',
          understood: 'Entendido',
          sections: {
            yourRights: {
              title: 'O seu direito de rescisão',
              content:
                'Tem direito de rescindir a sua compra dentro de 14 dias a partir da receção da mercadoria sem dar qualquer motivo.',
            },
            deadlines: {
              title: 'Prazos de rescisão',
              start: 'Início: Dia seguinte à receção da mercadoria',
              duration: 'Duração: 14 dias',
              form: 'Forma: Uma simples comunicação escrita por email é suficiente',
            },
            exceptions: {
              title: 'Exceções',
              intro: 'O direito de rescisão NÃO se aplica a:',
              digital: 'Conteúdo digital após o download',
              customized: 'Mercadorias personalizadas ou à medida',
              damaged: 'Mercadorias danificadas após a entrega (sua culpa)',
            },
            returnProcess: {
              title: 'Processo de devolução',
              content:
                'Devolva imediatamente a mercadoria. Os custos de envio são suportados pelo comprador (exceto em caso de devolução justificada).',
            },
            contact: {
              title: 'Contacto',
              content: 'Envie rescisões para: rescisao@example.com',
            },
            legalBasis: 'Base legal: Art. 10-17 Lei da Venda à Distância',
          },
        },
      },
    },
    nl: {
      common: {
        loading: 'Laden...',
        error: 'Er is een fout opgetreden',
        save: 'Opslaan',
        cancel: 'Annuleren',
        delete: 'Verwijderen',
        edit: 'Bewerken',
        add: 'Toevoegen',
        search: 'Zoeken',
        filter: 'Filteren',
        sort: 'Sorteren',
        next: 'Volgende',
        previous: 'Vorige',
        page: 'Pagina',
        of: 'van',
        items: 'items',
      },
      navigation: {
        home: 'Home',
        products: 'Producten',
        categories: 'Categorieën',
        cart: 'Winkelwagen',
        dashboard: 'Dashboard',
        tenants: 'Huurders',
        login: 'Inloggen',
        logout: 'Uitloggen',
      },
      notFound: {
        title: 'Pagina niet gevonden',
        message: 'De pagina die je zoekt bestaat niet.',
        goHome: 'Terug naar home',
      },
      app: {
        skipToMain: 'Ga naar hoofdinhoud',
        brand: 'B2Connect',
        admin: 'Admin',
        services: 'Diensten',
        branding: 'Branding',
        design: 'Design',
        marketing: 'Marketing',
        advertisement: 'Advertentie',
        company: 'Bedrijf',
        aboutUs: 'Over ons',
        contact: 'Contact',
        jobs: 'Vacatures',
        pressKit: 'Perskit',
        legal: 'Juridisch',
        termsOfUse: 'Gebruiksvoorwaarden',
        privacyPolicy: 'Privacybeleid',
        cookiePolicy: 'Cookiebeleid',
        newsletter: 'Nieuwsbrief',
        enterEmail: 'Voer je e-mailadres in',
        subscribe: 'Abonneren',
      },
      home: {
        hero: {
          title: 'Welkom bij B2Connect Store',
          subtitle: 'Ontdek geweldige producten met naadloze integratie en uitzonderlijke service',
          shopNow: 'Nu winkelen',
          browseCategories: 'Blader door categorieën',
        },
        featuredProducts: {
          title: 'Uitgelichte producten',
          viewAll: 'Bekijk alle producten',
        },
        categories: {
          title: 'Winkelen per categorie',
        },
        features: {
          title: 'Waarom ons kiezen',
          quality: {
            title: 'Premium Kwaliteit',
            description: 'Wij garanderen de hoogste kwaliteitsnormen voor al onze producten',
          },
          fast: {
            title: 'Snelle levering',
            description:
              'Snelle en betrouwbare verzending om je bestellingen snel bij je te krijgen',
          },
          support: {
            title: '24/7 Ondersteuning',
            description: 'Ons klantenserviceteam is altijd hier om je te helpen',
          },
        },
        newsletter: {
          title: 'Blijf op de hoogte',
          subtitle: 'Abonneer je op onze nieuwsbrief voor de laatste updates en aanbiedingen',
          subscribe: 'Abonneren',
        },
        table: {
          headers: {
            product: 'Product',
            qty: 'Aantal',
            price: 'Prijs',
          },
        },
      },
      vat: {
        countryCode: 'Landcode',
        vatNumber: 'BTW-nummer',
        validate: 'Valideren',
        validating: 'Bezig met valideren...',
        companyName: 'Bedrijfsnaam:',
        address: 'Adres:',
        reverseCharge: 'Reverse Charge:',
        reverseChargeApplies: '0% BTW (geldt)',
        standardVatRate: 'Standaard BTW-tarief',
        clearAndStartOver: 'Wissen en opnieuw beginnen',
        validationHelp: {
          title: 'BTW-validatie hulp',
          description:
            'Als je geen geldig BTW-nummer kunt opgeven, kun je doorgaan als particuliere klant of contact opnemen met ons ondersteuningsteam.',
        },
        countries: {
          AT: 'Oostenrijk (AT)',
          BE: 'België (BE)',
          BG: 'Bulgarije (BG)',
          HR: 'Kroatië (HR)',
          CY: 'Cyprus (CY)',
          CZ: 'Tsjechië (CZ)',
          DK: 'Denemarken (DK)',
          DE: 'Duitsland (DE)',
          EE: 'Estland (EE)',
          FI: 'Finland (FI)',
          FR: 'Frankrijk (FR)',
          GR: 'Griekenland (GR)',
          HU: 'Hongarije (HU)',
          IE: 'Ierland (IE)',
          IT: 'Italië (IT)',
          LV: 'Letland (LV)',
          LT: 'Litouwen (LT)',
          LU: 'Luxemburg (LU)',
          MT: 'Malta (MT)',
          NL: 'Nederland (NL)',
          PL: 'Polen (PL)',
          PT: 'Portugal (PT)',
          RO: 'Roemenië (RO)',
          SK: 'Slowakije (SK)',
          SI: 'Slovenië (SI)',
          ES: 'Spanje (ES)',
          SE: 'Zweden (SE)',
        },
      },
      legal: {
        checkout: {
          title: 'Voorwaarden',
          subtitle: 'Accepteer de vereiste voorwaarden om door te gaan',
          requiredFields: '* Verplichte velden',
          back: 'Terug',
          continueToPayment: 'Doorgaan naar betaling',
          processing: 'Verwerken...',
          acceptTerms: 'Ik accepteer de algemene voorwaarden',
          acceptPrivacy: 'Ik accepteer het privacybeleid',
          understandWithdrawal: 'Ik begrijp mijn herroepingsrecht (14 dagen)',
          acceptTermsError: 'Accepteer de algemene voorwaarden en het privacybeleid',
          acceptTermsSuccess: 'Voorwaarden geaccepteerd!',
          saveError: 'Fout bij opslaan van voorwaardenacceptatie',
          generalError: 'Er is een fout opgetreden. Probeer het later opnieuw.',
        },
        checkout: {
          header: {
            title: 'Order afronden',
            breadcrumb: {
              shop: 'Winkel',
              cart: 'Winkelwagen',
              checkout: '/ Afrekenen',
            },
          },
          steps: {
            shippingAddress: 'Verzendadres',
            shippingMethod: 'Verzendmethode',
            orderReview: 'Orderbeoordeling',
          },
          form: {
            labels: {
              firstName: 'Voornaam *',
              lastName: 'Achternaam *',
              streetAddress: 'Adres *',
              postalCode: 'Postcode *',
              city: 'Stad *',
              country: 'Land *',
            },
            placeholders: {
              firstName: 'Jan',
              lastName: 'Jansen',
              streetAddress: 'Hoofdstraat 123',
              postalCode: '1000 AA',
              city: 'Amsterdam',
            },
            countries: {
              germany: 'Duitsland',
              austria: 'Oostenrijk',
              belgium: 'België',
              france: 'Frankrijk',
              netherlands: 'Nederland',
            },
            required: '* Verplicht',
            description: 'Voer uw verzendadres in',
          },
          validation: {
            firstNameRequired: 'Voornaam is verplicht',
            lastNameRequired: 'Achternaam is verplicht',
            streetRequired: 'Adres is verplicht',
            cityRequired: 'Stad is verplicht',
            postalCodeRequired: 'Postcode is verplicht',
            countryRequired: 'Land is verplicht',
            invalidPostalCode: 'Ongeldige Nederlandse postcode (formaat: 1000 AA)',
          },
          shipping: {
            title: 'Verzendmethode',
            description: 'Selecteer uw voorkeursverzendmethode',
            deliveryTime: '⏱️ Levertijd: ca. {{days}} werkdag(en)',
          },
          orderReview: {
            title: 'Verificatie & Betaalmethode',
            shippingAddress: 'Verzendadres',
            shippingMethod: 'Verzendmethode',
            paymentMethod: 'Betaalmethode',
            edit: '✏️ Bewerken',
          },
          orderSummary: {
            title: 'Ordersamenvatting',
            netto: 'Netto:',
            vat: 'BTW (21%):',
            shipping: 'Verzending:',
            total: 'Totaal:',
            trustBadges: {
              ssl: 'SSL versleuteld',
              returns: '30 dagen retour',
              insured: 'Verzending verzekerd',
            },
          },
          terms: {
            acceptText: 'Ik accepteer de',
            termsLink: 'algemene voorwaarden',
            and: 'en het',
            privacyLink: 'privacybeleid',
            required: '*',
          },
          compliance: {
            title: 'Prijsaanduidingsverordening',
            content:
              'Alle weergegeven prijzen zijn eindprijzen en bevatten reeds de wettelijke belasting over de toegevoegde waarde (BTW) van 21%.',
          },
          buttons: {
            backToCart: '← Terug naar winkelwagen',
            continueToShipping: 'Doorgaan naar verzending →',
            backToAddress: '← Terug naar adres',
            continueToReview: 'Doorgaan naar verificatie →',
            backToShipping: '← Terug naar verzending',
            processing: 'Order verwerken...',
            completeOrder: 'Order voltooien',
          },
        },
        termsAndConditions: {
          title: 'Algemene voorwaarden',
          understood: 'Begrepen',
          sections: {
            general: {
              title: '1. Algemene bepalingen',
              content:
                'Deze algemene voorwaarden regelen de relatie tussen de exploitant van deze online winkel en de koper.',
            },
            products: {
              title: '2. Productbeschrijvingen',
              content:
                'Alle productbeschrijvingen zijn verkoopaanbiedingen. Een contract wordt alleen gesloten wanneer u een bestelling plaatst en wij deze accepteren.',
            },
            pricing: {
              title: '3. Prijzen en betalingsvoorwaarden',
              content:
                'Alle prijzen zijn inclusief de toepasselijke BTW. Verzendkosten worden apart berekend en weergegeven in de checkout.',
            },
            delivery: {
              title: '4. Levering',
              content:
                'Levertijden zijn niet bindend. Wij zijn alleen verantwoordelijk voor vertragingen door onze schuld.',
            },
            withdrawal: {
              title: '5. Herroepingsrecht',
              content:
                'U heeft een herroepingsrecht van 14 dagen vanaf ontvangst van de goederen. Zie details hieronder.',
            },
            liability: {
              title: '6. Aansprakelijkheid',
              content:
                'Aansprakelijkheid voor schade is beperkt tot directe schade tot het bedrag van de koopprijs.',
            },
            privacy: {
              title: '7. Gegevensbescherming',
              content: 'Zie privacybeleid voor de verwerking van uw gegevens.',
            },
            final: {
              title: '8. Slotbepalingen',
              content:
                'Het Nederlandse recht is van toepassing. De bevoegde rechter is de vestigingsplaats van het bedrijf.',
            },
          },
        },
        privacyPolicy: {
          title: 'Privacybeleid',
          understood: 'Begrepen',
          sections: {
            responsible: {
              title: '1. Verantwoordelijke',
              content:
                'De exploitant van deze winkel is verantwoordelijk voor de gegevensverwerking (zie imprint).',
            },
            collection: {
              title: '2. Verzameling en verwerking',
              content:
                'Wij verzamelen uw gegevens alleen voor de verwerking van uw aankoop en verzending.',
            },
            storage: {
              title: '3. Opslagduur',
              content:
                'Persoonlijke gegevens worden 10 jaar opgeslagen om fiscale verplichtingen na te komen.',
            },
            rights: {
              title: '4. Uw rechten',
              content:
                'U heeft recht op informatie, rectificatie, verwijdering en gegevensoverdraagbaarheid.',
            },
            cookies: {
              title: '5. Cookies',
              content:
                'Wij gebruiken technisch noodzakelijke cookies. Andere cookies worden opgeslagen met uw toestemming.',
            },
            security: {
              title: '6. Beveiliging',
              content:
                'Wij beschermen uw gegevens door middel van encryptie en veilige transmissie.',
            },
            contact: {
              title: '7. Functionaris voor gegevensbescherming',
              content: 'Voor vragen: privacy@example.com',
            },
          },
        },
        withdrawalRights: {
          title: 'Herroepingsrecht (14 dagen)',
          understood: 'Begrepen',
          sections: {
            yourRights: {
              title: 'Uw herroepingsrecht',
              content:
                'U heeft het recht uw aankoop te herroepen binnen 14 dagen na ontvangst van de goederen zonder opgave van redenen.',
            },
            deadlines: {
              title: 'Herroepingstermijnen',
              start: 'Begin: Dag na ontvangst van de goederen',
              duration: 'Duur: 14 dagen',
              form: 'Vorm: Een eenvoudige schriftelijke mededeling per e-mail is voldoende',
            },
            exceptions: {
              title: 'Uitzonderingen',
              intro: 'Het herroepingsrecht geldt NIET voor:',
              digital: 'Digitale inhoud na download',
              customized: 'Op maat gemaakte of gepersonaliseerde goederen',
              damaged: 'Goederen beschadigd na levering (uw schuld)',
            },
            returnProcess: {
              title: 'Retourproces',
              content:
                'Stuur de goederen onmiddellijk terug. Verzendkosten zijn voor rekening van de koper (behalve bij gerechtvaardigde retourzending).',
            },
            contact: {
              title: 'Contact',
              content: 'Stuur herroepingen naar: herroeping@example.com',
            },
            legalBasis: 'Wettelijke basis: Art. 7:46d-7:46j BW',
          },
        },
      },
    },
    pl: {
      common: {
        loading: 'Ładowanie...',
        error: 'Wystąpił błąd',
        save: 'Zapisz',
        cancel: 'Anuluj',
        delete: 'Usuń',
        edit: 'Edytuj',
        add: 'Dodaj',
        search: 'Szukaj',
        filter: 'Filtruj',
        sort: 'Sortuj',
        next: 'Następny',
        previous: 'Poprzedni',
        page: 'Strona',
        of: 'z',
        items: 'elementów',
      },
      navigation: {
        home: 'Strona główna',
        products: 'Produkty',
        categories: 'Kategorie',
        cart: 'Koszyk',
        dashboard: 'Panel',
        tenants: 'Najemcy',
        login: 'Zaloguj się',
        logout: 'Wyloguj się',
      },
      notFound: {
        title: 'Strona nie znaleziona',
        message: 'Strona, której szukasz, nie istnieje.',
        goHome: 'Wróć do strony głównej',
      },
      app: {
        skipToMain: 'Przejdź do treści głównej',
        brand: 'B2Connect',
        admin: 'Admin',
        services: 'Usługi',
        branding: 'Branding',
        design: 'Design',
        marketing: 'Marketing',
        advertisement: 'Reklama',
        company: 'Firma',
        aboutUs: 'O nas',
        contact: 'Kontakt',
        jobs: 'Praca',
        pressKit: 'Zestaw prasowy',
        legal: 'Prawne',
        termsOfUse: 'Warunki użytkowania',
        privacyPolicy: 'Polityka prywatności',
        cookiePolicy: 'Polityka cookies',
        newsletter: 'Newsletter',
        enterEmail: 'Wprowadź swój adres e-mail',
        subscribe: 'Subskrybuj',
      },
      home: {
        hero: {
          title: 'Witamy w B2Connect Store',
          subtitle: 'Odkryj niesamowite produkty z płynną integracją i wyjątkową obsługą',
          shopNow: 'Kup teraz',
          browseCategories: 'Przeglądaj kategorie',
        },
        featuredProducts: {
          title: 'Polecane produkty',
          viewAll: 'Zobacz wszystkie produkty',
        },
        categories: {
          title: 'Kupuj według kategorii',
        },
        features: {
          title: 'Dlaczego warto nas wybrać',
          quality: {
            title: 'Jakość Premium',
            description:
              'Gwarantujemy najwyższe standardy jakości dla wszystkich naszych produktów',
          },
          fast: {
            title: 'Szybka dostawa',
            description: 'Szybka i niezawodna wysyłka, aby szybko dostarczyć Twoje zamówienia',
          },
          support: {
            title: 'Wsparcie 24/7',
            description: 'Nasz zespół obsługi klienta jest zawsze tutaj, aby Ci pomóc',
          },
        },
        newsletter: {
          title: 'Bądź na bieżąco',
          subtitle: 'Subskrybuj nasz newsletter, aby otrzymywać najnowsze aktualizacje i oferty',
          subscribe: 'Subskrybuj',
        },
        table: {
          headers: {
            product: 'Produkt',
            qty: 'Ilość',
            price: 'Cena',
          },
        },
      },
      vat: {
        countryCode: 'Kod kraju',
        vatNumber: 'Numer VAT',
        validate: 'Zwaliduj',
        validating: 'Weryfikowanie...',
        companyName: 'Nazwa firmy:',
        address: 'Adres:',
        reverseCharge: 'Reverse Charge:',
        reverseChargeApplies: '0% VAT (dotyczy)',
        standardVatRate: 'Standardowa stawka VAT',
        clearAndStartOver: 'Wyczyść i zacznij od nowa',
        validationHelp: {
          title: 'Pomoc w walidacji VAT',
          description:
            'Jeśli nie możesz podać prawidłowego numeru VAT, możesz kontynuować jako klient prywatny lub skontaktować się z naszym zespołem wsparcia.',
        },
        countries: {
          AT: 'Austria (AT)',
          BE: 'Belgia (BE)',
          BG: 'Bułgaria (BG)',
          HR: 'Chorwacja (HR)',
          CY: 'Cypr (CY)',
          CZ: 'Czechy (CZ)',
          DK: 'Dania (DK)',
          DE: 'Niemcy (DE)',
          EE: 'Estonia (EE)',
          FI: 'Finlandia (FI)',
          FR: 'Francja (FR)',
          GR: 'Grecja (GR)',
          HU: 'Węgry (HU)',
          IE: 'Irlandia (IE)',
          IT: 'Włochy (IT)',
          LV: 'Łotwa (LV)',
          LT: 'Litwa (LT)',
          LU: 'Luksemburg (LU)',
          MT: 'Malta (MT)',
          NL: 'Holandia (NL)',
          PL: 'Polska (PL)',
          PT: 'Portugalia (PT)',
          RO: 'Rumunia (RO)',
          SK: 'Słowacja (SK)',
          SI: 'Słowenia (SI)',
          ES: 'Hiszpania (ES)',
          SE: 'Szwecja (SE)',
        },
      },
      legal: {
        checkout: {
          title: 'Warunki',
          subtitle: 'Proszę zaakceptować wymagane warunki, aby kontynuować',
          requiredFields: '* Pola obowiązkowe',
          back: 'Wstecz',
          continueToPayment: 'Przejdź do płatności',
          processing: 'Przetwarzanie...',
          acceptTerms: 'Akceptuję ogólne warunki',
          acceptPrivacy: 'Akceptuję politykę prywatności',
          understandWithdrawal: 'Rozumiem moje prawo do odstąpienia (14 dni)',
          acceptTermsError: 'Proszę zaakceptować ogólne warunki i politykę prywatności',
          acceptTermsSuccess: 'Warunki zaakceptowane!',
          saveError: 'Błąd podczas zapisywania akceptacji warunków',
          generalError: 'Wystąpił błąd. Spróbuj ponownie później.',
        },
        checkout: {
          header: {
            title: 'Finalizacja zamówienia',
            breadcrumb: {
              shop: 'Sklep',
              cart: 'Koszyk',
              checkout: '/ Zamówienie',
            },
          },
          steps: {
            shippingAddress: 'Adres wysyłki',
            shippingMethod: 'Metoda wysyłki',
            orderReview: 'Przegląd zamówienia',
          },
          form: {
            labels: {
              firstName: 'Imię *',
              lastName: 'Nazwisko *',
              streetAddress: 'Adres *',
              postalCode: 'Kod pocztowy *',
              city: 'Miasto *',
              country: 'Kraj *',
            },
            placeholders: {
              firstName: 'Jan',
              lastName: 'Kowalski',
              streetAddress: 'Główna 123',
              postalCode: '00-001',
              city: 'Warszawa',
            },
            countries: {
              germany: 'Niemcy',
              austria: 'Austria',
              belgium: 'Belgia',
              france: 'Francja',
              netherlands: 'Holandia',
            },
            required: '* Wymagane',
            description: 'Proszę wprowadzić adres wysyłki',
          },
          validation: {
            firstNameRequired: 'Imię jest wymagane',
            lastNameRequired: 'Nazwisko jest wymagane',
            streetRequired: 'Adres jest wymagany',
            cityRequired: 'Miasto jest wymagane',
            postalCodeRequired: 'Kod pocztowy jest wymagany',
            countryRequired: 'Kraj jest wymagany',
            invalidPostalCode: 'Nieprawidłowy polski kod pocztowy (format: 00-001)',
          },
          shipping: {
            title: 'Metoda wysyłki',
            description: 'Wybierz preferowaną metodę wysyłki',
            deliveryTime: '⏱️ Czas dostawy: ok. {{days}} dni roboczych',
          },
          orderReview: {
            title: 'Weryfikacja & Metoda płatności',
            shippingAddress: 'Adres wysyłki',
            shippingMethod: 'Metoda wysyłki',
            paymentMethod: 'Metoda płatności',
            edit: '✏️ Edytuj',
          },
          orderSummary: {
            title: 'Podsumowanie zamówienia',
            netto: 'Netto:',
            vat: 'VAT (23%):',
            shipping: 'Wysyłka:',
            total: 'Razem:',
            trustBadges: {
              ssl: 'SSL zaszyfrowane',
              returns: '30 dni zwrot',
              insured: 'Wysyłka ubezpieczona',
            },
          },
          terms: {
            acceptText: 'Akceptuję',
            termsLink: 'ogólne warunki',
            and: 'oraz',
            privacyLink: 'politykę prywatności',
            required: '*',
          },
          compliance: {
            title: 'Rozporządzenie w sprawie oznaczania cen',
            content:
              'Wszystkie wyświetlane ceny są cenami końcowymi i zawierają już prawny podatek od wartości dodanej (VAT) w wysokości 23%.',
          },
          buttons: {
            backToCart: '← Powrót do koszyka',
            continueToShipping: 'Przejdź do wysyłki →',
            backToAddress: '← Powrót do adresu',
            continueToReview: 'Przejdź do weryfikacji →',
            backToShipping: '← Powrót do wysyłki',
            processing: 'Przetwarzanie zamówienia...',
            completeOrder: 'Złóż zamówienie',
          },
        },
        termsAndConditions: {
          title: 'Ogólne warunki',
          understood: 'Zrozumiano',
          sections: {
            general: {
              title: '1. Postanowienia ogólne',
              content:
                'Niniejsze ogólne warunki regulują stosunek między operatorem tego sklepu internetowego a kupującym.',
            },
            products: {
              title: '2. Opisy produktów',
              content:
                'Wszystkie opisy produktów są ofertami sprzedaży. Umowa zostaje zawarta dopiero wtedy, gdy złożysz zamówienie, a my je zaakceptujemy.',
            },
            pricing: {
              title: '3. Ceny i warunki płatności',
              content:
                'Wszystkie ceny zawierają obowiązujący VAT. Koszty wysyłki są kalkulowane oddzielnie i wyświetlane w checkout.',
            },
            delivery: {
              title: '4. Dostawa',
              content:
                'Terminy dostawy nie są wiążące. Odpowiadamy tylko za opóźnienia z naszej winy.',
            },
            withdrawal: {
              title: '5. Prawo odstąpienia',
              content:
                'Masz prawo odstąpienia od umowy w ciągu 14 dni od odbioru towarów. Zobacz szczegóły poniżej.',
            },
            liability: {
              title: '6. Odpowiedzialność',
              content:
                'Odpowiedzialność za szkody jest ograniczona do szkód bezpośrednich do kwoty ceny zakupu.',
            },
            privacy: {
              title: '7. Ochrona danych',
              content: 'Zobacz politykę prywatności dotyczącą przetwarzania Twoich danych.',
            },
            final: {
              title: '8. Postanowienia końcowe',
              content: 'Obowiązuje prawo polskie. Sądem właściwym jest siedziba firmy.',
            },
          },
        },
        privacyPolicy: {
          title: 'Polityka prywatności',
          understood: 'Zrozumiano',
          sections: {
            responsible: {
              title: '1. Odpowiedzialny',
              content:
                'Operator tego sklepu jest odpowiedzialny za przetwarzanie danych (zobacz imprint).',
            },
            collection: {
              title: '2. Zbieranie i przetwarzanie',
              content: 'Zbieramy Twoje dane tylko w celu przetworzenia Twojego zakupu i wysyłki.',
            },
            storage: {
              title: '3. Czas przechowywania',
              content:
                'Dane osobowe są przechowywane przez 10 lat w celu wypełnienia obowiązków podatkowych.',
            },
            rights: {
              title: '4. Twoje prawa',
              content: 'Masz prawo do informacji, sprostowania, usunięcia i przenoszenia danych.',
            },
            cookies: {
              title: '5. Cookies',
              content:
                'Używamy technicznie niezbędnych cookies. Inne cookies są przechowywane za Twoją zgodą.',
            },
            security: {
              title: '6. Bezpieczeństwo',
              content: 'Chrońmy Twoje dane poprzez szyfrowanie i bezpieczne przesyłanie.',
            },
            contact: {
              title: '7. Inspektor ochrony danych',
              content: 'W przypadku pytań: ochrona-danych@example.com',
            },
          },
        },
        withdrawalRights: {
          title: 'Prawo odstąpienia (14 dni)',
          understood: 'Zrozumiano',
          sections: {
            yourRights: {
              title: 'Twoje prawo odstąpienia',
              content:
                'Masz prawo odstąpić od zakupu w ciągu 14 dni od odbioru towarów bez podania przyczyny.',
            },
            deadlines: {
              title: 'Terminy odstąpienia',
              start: 'Początek: Następny dzień po odbiorze towarów',
              duration: 'Czas trwania: 14 dni',
              form: 'Forma: Wystarczy proste pisemne powiadomienie drogą mailową',
            },
            exceptions: {
              title: 'Wyjątki',
              intro: 'Prawo odstąpienia NIE dotyczy:',
              digital: 'Treści cyfrowych po pobraniu',
              customized: 'Towarów spersonalizowanych lub na zamówienie',
              damaged: 'Towarów uszkodzonych po dostawie (Twoja wina)',
            },
            returnProcess: {
              title: 'Proces zwrotu',
              content:
                'Natychmiast odeślij towary. Koszty wysyłki ponosi kupujący (z wyjątkiem uzasadnionych zwrotów).',
            },
            contact: {
              title: 'Kontakt',
              content: 'Wyślij odstąpienia na: odstąpienie@example.com',
            },
            legalBasis: 'Podstawa prawna: Art. 27-38 Ustawy o prawach konsumenta',
          },
        },
      },
    },
  },
}));
