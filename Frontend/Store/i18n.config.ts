// i18n.config.ts

import commonEn from './locales/default/en/common.json';
import navigationEn from './locales/default/en/navigation.json';
import notFoundEn from './locales/default/en/notFound.json';
import appEn from './locales/default/en/app.json';
import homeEn from './locales/default/en/home.json';
import vatEn from './locales/default/en/vat.json';
import cartEn from './locales/default/en/cart.json';
import legalEn from './locales/default/en/legal.json';
import registrationEn from './locales/default/en/registration.json';
import dashboardEn from './locales/default/en/dashboard.json';
import customerTypeSelectionEn from './locales/default/en/customerTypeSelection.json';
import loginEn from './locales/default/en/login.json';
import productListingEn from './locales/default/en/productListing.json';
import customerLookupEn from './locales/default/en/customerLookup.json';

export default defineI18nConfig(() => ({
  legacy: false,
  locale: 'en',
  fallbackLocale: 'en',
  messages: {
    // Default fallback messages - will be overridden by tenant-specific ones
    en: {
      common: commonEn,
      navigation: navigationEn,
      notFound: notFoundEn,
      app: appEn,
      home: homeEn,
      vat: vatEn,
      cart: cartEn,
      legal: legalEn,
      registration: registrationEn,
      dashboard: dashboardEn,
      customerTypeSelection: customerTypeSelectionEn,
      login: loginEn,
      productListing: productListingEn,
      customerLookup: customerLookupEn,
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
        validation: {
          required: 'Ländercode und Umsatzsteuer-Nummer sind erforderlich',
        },
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
        acceptance: {
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
      invoice: {
        loading: 'Rechnung wird geladen...',
        error: 'Fehler beim Laden der Rechnung',
        retry: 'Erneut versuchen',
        noInvoice: 'Keine Rechnung zum Anzeigen',
        status: {
          invoice: 'Rechnung',
          reverseCharge: '⚠️ Reverse Charge (0% MwSt.)',
          overdue: 'Überfällig',
        },
        labels: {
          issued: 'Ausgestellt',
          due: 'Fällig',
          payment: 'Zahlung',
          paidOn: 'Bezahlt am',
          from: 'Von',
          billTo: 'Rechnung an',
          reverseCharge: '(Reverse Charge)',
        },
        table: {
          headers: {
            product: 'Produkt',
            qty: 'Menge',
            unitPrice: 'Einzelpreis',
            subtotal: 'Zwischensumme',
            tax: 'Steuer',
            total: 'Gesamt',
          },
        },
        pricing: {
          subtotal: 'Zwischensumme:',
          shipping: 'Versand:',
          vat: 'MwSt. ({{rate}}%):',
          reverseCharge: 'Reverse Charge (0% MwSt.):',
          total: 'Gesamt:',
        },
        actions: {
          downloadPdf: 'PDF herunterladen',
          sendEmail: 'E-Mail senden',
          modify: 'Ändern',
          print: 'Drucken',
        },
        compliance: {
          vatNotice: 'Die oben gezeigte MwSt. entspricht den gesetzlichen Vorschriften.',
          paymentInfo: 'Zahlungsinformationen',
          method: 'Methode:',
        },
      },
      cart: {
        title: 'Warenkorb',
        itemCount: 'Artikel in Ihrem Warenkorb',
        empty: {
          title: 'Ihr Warenkorb ist leer',
          description: 'Entdecken Sie tolle Produkte und fügen Sie sie Ihrem Warenkorb hinzu.',
          continueShopping: 'Einkaufen fortsetzen',
        },
        table: {
          headers: {
            product: 'Produkt',
            price: 'Preis',
            quantity: 'Menge',
            total: 'Gesamt',
          },
        },
        actions: {
          continueShopping: 'Einkaufen fortsetzen',
          remove: 'Aus Warenkorb entfernen',
        },
        summary: {
          title: 'Bestellübersicht',
          coupon: {
            label: 'Haben Sie einen Gutscheincode?',
            placeholder: 'Gutscheincode eingeben',
            apply: 'Anwenden',
          },
          pricing: {
            subtotal: 'Zwischensumme',
            shipping: 'Versand',
            free: 'KOSTENLOS',
            netPrice: 'Nettopreis (exkl. MwSt.)',
            vat: 'MwSt. ({{rate}}%)',
            total: 'Gesamt (inkl. MwSt.)',
          },
          checkout: 'Zur Kasse →',
          guestCheckout: 'Als Gast fortfahren',
          secure: '🔒 Sichere Kasse',
        },
        trust: {
          moneyBack: '✓ 30-tägige Geld-zurück-Garantie',
          returns: '✓ Kostenlose Rückgaben & Umtausch',
          ssl: '✓ Sichere SSL-verschlüsselte Kasse',
        },
      },
      registration: {
        check: {
          title: 'Registrierungstyp Prüfen',
          subtitle: 'Überprüfen Sie, ob Sie bereits als Bestandskunde registriert sind',
          form: {
            email: {
              label: 'E-Mail-Adresse',
              placeholder: 'beispiel@unternehmen.de',
            },
            businessType: {
              label: 'Unternehmenstyp',
              placeholder: '-- Bitte wählen --',
              b2c: 'B2C (Privatperson / Einzelunternehmer)',
              b2b: 'B2B (Unternehmen / GmbH / GmbH & Co. KG)',
            },
            firstName: {
              label: 'Vorname',
              placeholder: 'Max',
            },
            lastName: {
              label: 'Nachname',
              placeholder: 'Mustermann',
            },
            companyName: {
              label: 'Firmenname',
              placeholder: 'Mustercompany GmbH',
            },
            phone: {
              label: 'Telefon',
              placeholder: '+49 (0) 123 456789',
            },
          },
          buttons: {
            check: 'Prüfen',
            checking: 'Prüfen läuft...',
            newCheck: 'Neue Prüfung',
            continueWithData: 'Mit Kundendaten fortfahren',
            continueRegistration: 'Registrierung fortsetzen',
            back: 'Zurück',
          },
          alerts: {
            error: 'Fehler',
          },
          results: {
            existingCustomer: {
              title: 'Willkommen zurück!',
              description:
                'Sie sind bereits in unserem System registriert. Ihre Daten werden automatisch vorausgefüllt.',
            },
            newCustomer: {
              title: 'Neukundenregistrierung',
              description: 'Sie werden zur regulären Registrierung weitergeleitet.',
            },
            customerData: 'Ihre Kundendaten:',
            customerNumber: 'Kundennummer:',
            name: 'Name:',
            email: 'E-Mail:',
            phone: 'Telefon:',
            address: 'Adresse:',
            matchScore: 'Übereinstimmungsquote:',
          },
          info: {
            title: 'Informationen',
            existingCustomer:
              'Bestandskunde: Sie sind bereits in unserem System registriert. Ihre Daten werden automatisch vorausgefüllt.',
            newCustomer: 'Neukunde: Sie werden zur regulären Registrierung weitergeleitet.',
            checkDetails:
              'Die Prüfung wird anhand von E-Mail, Name und optional Telefon/Adresse durchgeführt.',
          },
        },
        privateCustomerRegistration: {
          title: 'Erstellen Sie Ihr Konto',
          subtitle: 'Treten Sie B2Connect bei und beginnen Sie noch heute mit dem Einkaufen',
          form: {
            email: {
              label: 'E-Mail-Adresse',
              placeholder: 'ihre@email.com',
              ariaLabel: 'E-Mail-Adresse',
            },
            password: {
              label: 'Passwort',
              placeholder: '••••••••',
              ariaLabel: 'Passwort',
            },
            confirmPassword: {
              label: 'Passwort bestätigen',
              placeholder: '••••••••',
              ariaLabel: 'Passwort bestätigen',
            },
            firstName: {
              label: 'Vorname',
              placeholder: 'Max',
              ariaLabel: 'Vorname',
            },
            lastName: {
              label: 'Nachname',
              placeholder: 'Mustermann',
              ariaLabel: 'Nachname',
            },
            phone: {
              label: 'Telefonnummer',
              placeholder: '+49 123 456789',
              ariaLabel: 'Telefonnummer',
            },
            streetAddress: {
              label: 'Straße und Hausnummer',
              placeholder: 'Musterstraße 123',
              ariaLabel: 'Straße und Hausnummer',
            },
            city: {
              label: 'Stadt',
              placeholder: 'Berlin',
              ariaLabel: 'Stadt',
            },
            postalCode: {
              label: 'Postleitzahl',
              placeholder: '10115',
              ariaLabel: 'Postleitzahl',
            },
            country: {
              label: 'Land',
              placeholder: 'Land auswählen',
              ariaLabel: 'Land',
              options: {
                select: 'Land auswählen',
                DE: 'Deutschland (DE)',
                AT: 'Österreich (AT)',
                CH: 'Schweiz (CH)',
                FR: 'Frankreich (FR)',
                NL: 'Niederlande (NL)',
                BE: 'Belgien (BE)',
                LU: 'Luxemburg (LU)',
                PL: 'Polen (PL)',
                CZ: 'Tschechische Republik (CZ)',
              },
            },
            state: {
              label: 'Bundesland / Provinz',
              placeholder: 'Bundesland / Provinz',
              ariaLabel: 'Bundesland oder Provinz',
            },
            dateOfBirth: {
              label: 'Geburtsdatum',
              ariaLabel: 'Geburtsdatum',
            },
            ageConfirmation: {
              ariaLabel: 'Ich bestätige, dass ich mindestens 18 Jahre alt bin',
            },
            acceptTerms: {
              ariaLabel: 'Ich akzeptiere die Allgemeinen Geschäftsbedingungen',
            },
            acceptPrivacy: {
              ariaLabel: 'Ich akzeptiere die Datenschutzrichtlinie',
            },
            acceptMarketing: {
              ariaLabel: 'Ich möchte Marketingmitteilungen erhalten',
            },
          },
          actions: {
            createAccount: 'Konto erstellen',
            creating: 'Konto wird erstellt...',
          },
          links: {
            termsLink: 'Allgemeine Geschäftsbedingungen',
            privacyLink: 'Datenschutzrichtlinie',
            loginLink: 'Hier anmelden',
          },
          messages: {
            alreadyHaveAccount: 'Haben Sie bereits ein Konto?',
            ageConfirmation: 'Ich bestätige, dass ich mindestens {{age}} Jahre alt bin',
            acceptTerms: 'Ich akzeptiere die',
            acceptPrivacy: 'Ich akzeptiere die',
            acceptMarketing: 'Ich möchte Marketingmitteilungen erhalten',
            withdrawalNotice: 'Widerrufsrecht',
            error: 'Fehler',
            networkError: 'Netzwerkfehler. Bitte versuchen Sie es erneut.',
          },
        },
      },
      dashboard: {
        title: 'Dashboard',
        welcome: 'Willkommen, {{firstName}} {{lastName}}!',
        email: 'E-Mail',
        tenantId: 'Mandanten-ID',
        statistics: {
          title: 'Statistiken',
          description: 'Ihre Dashboard-Statistiken werden hier angezeigt.',
        },
        recentActivity: {
          title: 'Kürzliche Aktivitäten',
          description: 'Kürzliche Aktivitäten werden hier angezeigt.',
        },
        quickActions: {
          title: 'Schnellaktionen',
          manageTenants: 'Mandanten verwalten',
          accountSettings: 'Kontoeinstellungen',
        },
        alerts: {
          settingsComingSoon: 'Einstellungen werden bald implementiert',
        },
      },
      customerTypeSelection: {
        title: 'Wie möchten Sie sich registrieren?',
        subtitle: 'Wählen Sie den Kontotyp, der am besten zu Ihren Bedürfnissen passt',
        private: {
          ariaLabel: 'Als Privatkunde registrieren',
          title: 'Privatkunde',
          description: 'Einzelner Einkäufer',
          details: 'Für persönliche Einkäufe und Shopping',
        },
        business: {
          ariaLabel: 'Als Geschäftskunde registrieren',
          title: 'Geschäftskunde',
          description: 'Unternehmen oder Organisation',
          details: 'Für Geschäftseinkäufe und B2B-Operationen',
        },
        actions: {
          continue: 'Weiter',
        },
        login: {
          prompt: 'Haben Sie bereits ein Konto?',
          link: 'Hier anmelden',
        },
      },
      login: {
        title: 'Bei B2Connect anmelden',
        e2eMode: {
          title: 'E2E-Testmodus aktiv',
          description: 'Jede E-Mail/Passwort-Kombination funktioniert. Backend nicht erforderlich.',
        },
        devHelp: {
          hint: '💡 Verwenden Sie Testzugangsdaten: {{email}} / {{password}}',
          email: 'admin@example.com',
          password: 'password',
        },
        form: {
          email: {
            label: 'E-Mail',
            placeholder: 'Geben Sie Ihre E-Mail ein',
          },
          password: {
            label: 'Passwort',
            placeholder: 'Geben Sie Ihr Passwort ein',
          },
        },
        actions: {
          loggingIn: 'Anmeldung läuft...',
          login: 'Anmelden',
        },
        signup: {
          prompt: 'Haben Sie noch kein Konto?',
          link: 'Registrieren',
        },
      },
      productListing: {
        title: 'B2Connect Shop',
        subtitle: 'Finden Sie die besten Produkte für Ihr Unternehmen',
        search: {
          label: 'Produkte suchen',
          placeholder: 'Nach Name, SKU oder Beschreibung suchen...',
        },
        sort: {
          label: 'Sortieren nach',
          options: {
            name: 'Name (A-Z)',
            priceAsc: 'Preis (Aufsteigend)',
            priceDesc: 'Preis (Absteigend)',
            rating: 'Bewertung (Absteigend)',
          },
        },
        filters: {
          title: 'Filter',
        },
        category: {
          label: 'Kategorie',
        },
        priceRange: {
          label: 'Preisbereich',
          placeholder: '€0 - €5000 (bald verfügbar)',
        },
        inStockOnly: 'Nur auf Lager',
        results: {
          foundFor: 'Gefunden für:',
          loading: 'Produkte werden geladen...',
          noProducts: 'Keine Produkte gefunden',
          noProductsMessage: 'Versuchen Sie, Ihre Filter oder Suchanfrage anzupassen',
          clearFilters: 'Filter löschen',
          retry: 'Erneut versuchen',
        },
        pagination: {
          previous: '← Zurück',
          next: 'Weiter →',
        },
      },
      customerLookup: {
        header: {
          newRegistration: 'Neue Registrierung',
          welcomeBack: 'Willkommen zurück',
          enterEmailPrompt: 'Geben Sie Ihre E-Mail-Adresse ein, um zu beginnen',
          customerInfoFound: 'Kundeninformationen gefunden',
        },
        form: {
          email: {
            label: 'E-Mail-Adresse *',
            placeholder: 'name@example.com',
            ariaLabel: 'E-Mail-Adresse',
          },
          status: {
            searching: 'Suche läuft...',
          },
          error: {
            title: 'Fehler bei der Kundensuche',
          },
          success: {
            title: 'Kunde gefunden!',
            welcomeMessage: 'Willkommen zurück, {name}!',
          },
          customerDetails: {
            customerNumber: 'Kundennummer',
            customerType: 'Kundentyp',
            privateCustomer: 'Privatperson',
            businessCustomer: 'Geschäftskunde',
          },
          businessDetails: {
            title: 'Geschäftsinformationen',
            company: 'Firma:',
            phone: 'Telefon:',
            country: 'Land:',
            creditLimit: 'Kreditlimit:',
          },
          actions: {
            searchCustomer: 'Kundensuche',
            searching: 'Suche läuft...',
            proceed: 'Weiter',
            newSearch: 'Neue Suche',
            cancel: 'Abbrechen',
          },
        },
        newCustomer: {
          title: 'Sind Sie ein neuer Kunde?',
          message:
            'Sie können sich jetzt registrieren und später von Ihren gespeicherten Informationen profitieren.',
          registerButton: 'Neue Registrierung',
        },
        diagnostic: {
          title: '🔧 Diagnostic Info (Dev Only)',
        },
      },
      productDetail: {
        breadcrumb: {
          home: 'Startseite',
          products: 'Produkte',
        },
        loading: {
          message: 'Produktdetails werden geladen...',
        },
        error: {
          retry: 'Erneut versuchen',
        },
        price: {
          overview: 'Preisübersicht',
          vatNotice: 'Alle Preise enthalten MwSt. gemäß PAngV (Preisangabenverordnung)',
        },
        stock: {
          inStock: '✓ Auf Lager',
          outOfStock: '✗ Nicht verfügbar',
          available: '({count} verfügbar)',
        },
        actions: {
          addToCart: 'In den Warenkorb',
        },
        share: {
          label: 'Teilen:',
        },
        specifications: {
          title: 'Spezifikationen',
        },
        reviews: {
          title: 'Kundenbewertungen',
          verified: '✓ Verifiziert',
          byAuthor: 'von {author}',
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
        validation: {
          required: 'Le code pays et le numéro de TVA sont requis',
        },
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
        acceptance: {
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
      invoice: {
        loading: 'Chargement de la facture...',
        error: 'Erreur lors du chargement de la facture',
        retry: 'Réessayer',
        noInvoice: 'Aucune facture à afficher',
        status: {
          invoice: 'Facture',
          reverseCharge: '⚠️ Reverse Charge (0% TVA)',
          overdue: 'En retard',
        },
        labels: {
          issued: 'Émise',
          due: 'Échéance',
          payment: 'Paiement',
          paidOn: 'Payé le',
          from: 'De',
          billTo: 'Facturé à',
          reverseCharge: '(Reverse Charge)',
        },
        table: {
          headers: {
            product: 'Produit',
            qty: 'Qté',
            unitPrice: 'Prix unitaire',
            subtotal: 'Sous-total',
            tax: 'Taxe',
            total: 'Total',
          },
        },
        pricing: {
          subtotal: 'Sous-total :',
          shipping: 'Livraison :',
          vat: 'TVA ({{rate}} %) :',
          reverseCharge: 'Reverse Charge (0% TVA) :',
          total: 'Total :',
        },
        actions: {
          downloadPdf: 'Télécharger PDF',
          sendEmail: 'Envoyer par e-mail',
          modify: 'Modifier',
          print: 'Imprimer',
        },
        compliance: {
          vatNotice: 'La TVA indiquée ci-dessus est conforme à la législation.',
          paymentInfo: 'Informations de paiement',
          method: 'Méthode :',
        },
      },
      cart: {
        title: 'Panier',
        itemCount: 'article(s) dans votre panier',
        empty: {
          title: 'Votre panier est vide',
          description: 'Découvrez des produits étonnants et ajoutez-les à votre panier.',
          continueShopping: 'Continuer les achats',
        },
        table: {
          headers: {
            product: 'Produit',
            price: 'Prix',
            quantity: 'Quantité',
            total: 'Total',
          },
        },
        actions: {
          continueShopping: 'Continuer les achats',
          remove: 'Retirer du panier',
        },
        summary: {
          title: 'Résumé de la commande',
          coupon: {
            label: 'Vous avez un code promo ?',
            placeholder: 'Entrez le code promo',
            apply: 'Appliquer',
          },
          pricing: {
            subtotal: 'Sous-total',
            shipping: 'Livraison',
            free: 'GRATUIT',
            netPrice: 'Prix net (HT)',
            vat: 'TVA ({{rate}} %)',
            total: 'Total (TTC)',
          },
          checkout: 'Procéder au paiement →',
          guestCheckout: "Continuer en tant qu'invité",
          secure: '🔒 Paiement sécurisé',
        },
        trust: {
          moneyBack: '✓ Garantie de remboursement 30 jours',
          returns: '✓ Retours et échanges gratuits',
          ssl: '✓ Paiement SSL chiffré sécurisé',
        },
      },
      registration: {
        check: {
          title: "Vérifier le type d'inscription",
          subtitle: 'Vérifiez si vous êtes déjà inscrit en tant que client existant',
          form: {
            email: {
              label: 'Adresse e-mail',
              placeholder: 'exemple@entreprise.fr',
            },
            businessType: {
              label: "Type d'entreprise",
              placeholder: '-- Veuillez sélectionner --',
              b2c: 'B2C (Particulier / Entrepreneur individuel)',
              b2b: 'B2B (Entreprise / SARL / SAS)',
            },
            firstName: {
              label: 'Prénom',
              placeholder: 'Jean',
            },
            lastName: {
              label: 'Nom',
              placeholder: 'Dupont',
            },
            companyName: {
              label: "Nom de l'entreprise",
              placeholder: 'Exemple SARL',
            },
            phone: {
              label: 'Téléphone',
              placeholder: '+33 (0) 1 23 45 67 89',
            },
          },
          buttons: {
            check: 'Vérifier',
            checking: 'Vérification en cours...',
            newCheck: 'Nouvelle vérification',
            continueWithData: 'Continuer avec les données client',
            continueRegistration: "Continuer l'inscription",
            back: 'Retour',
          },
          alerts: {
            error: 'Erreur',
          },
          results: {
            existingCustomer: {
              title: 'Bienvenue !',
              description:
                'Vous êtes déjà inscrit dans notre système. Vos données seront pré-remplies automatiquement.',
            },
            newCustomer: {
              title: 'Inscription nouveau client',
              description: "Vous serez redirigé vers le processus d'inscription régulier.",
            },
            customerData: 'Vos données client :',
            customerNumber: 'Numéro client :',
            name: 'Nom :',
            email: 'E-mail :',
            phone: 'Téléphone :',
            address: 'Adresse :',
            matchScore: 'Score de correspondance :',
          },
          info: {
            title: 'Informations',
            existingCustomer:
              'Client existant : Vous êtes déjà inscrit dans notre système. Vos données seront pré-remplies automatiquement.',
            newCustomer:
              "Nouveau client : Vous serez redirigé vers le processus d'inscription régulier.",
            checkDetails:
              "La vérification est effectuée sur la base de l'e-mail, du nom et éventuellement du téléphone/adresse.",
          },
        },
        privateCustomerRegistration: {
          title: 'Créez votre compte',
          subtitle: "Rejoignez B2Connect et commencez à magasiner dès aujourd'hui",
          form: {
            email: {
              label: 'Adresse e-mail',
              placeholder: 'vous@exemple.com',
              ariaLabel: 'Adresse e-mail',
            },
            password: {
              label: 'Mot de passe',
              placeholder: '••••••••',
              ariaLabel: 'Mot de passe',
            },
            confirmPassword: {
              label: 'Confirmer le mot de passe',
              placeholder: '••••••••',
              ariaLabel: 'Confirmer le mot de passe',
            },
            firstName: {
              label: 'Prénom',
              placeholder: 'Jean',
              ariaLabel: 'Prénom',
            },
            lastName: {
              label: 'Nom de famille',
              placeholder: 'Dupont',
              ariaLabel: 'Nom de famille',
            },
            phone: {
              label: 'Numéro de téléphone',
              placeholder: '+33 1 23 45 67 89',
              ariaLabel: 'Numéro de téléphone',
            },
            streetAddress: {
              label: 'Adresse',
              placeholder: '123 Rue Principale',
              ariaLabel: 'Adresse',
            },
            city: {
              label: 'Ville',
              placeholder: 'Paris',
              ariaLabel: 'Ville',
            },
            postalCode: {
              label: 'Code postal',
              placeholder: '75001',
              ariaLabel: 'Code postal',
            },
            country: {
              label: 'Pays',
              placeholder: 'Sélectionnez un pays',
              ariaLabel: 'Pays',
              options: {
                select: 'Sélectionnez un pays',
                DE: 'Allemagne (DE)',
                AT: 'Autriche (AT)',
                CH: 'Suisse (CH)',
                FR: 'France (FR)',
                NL: 'Pays-Bas (NL)',
                BE: 'Belgique (BE)',
                LU: 'Luxembourg (LU)',
                PL: 'Pologne (PL)',
                CZ: 'République tchèque (CZ)',
              },
            },
            state: {
              label: 'État / Province',
              placeholder: 'État / Province',
              ariaLabel: 'État ou province',
            },
            dateOfBirth: {
              label: 'Date de naissance',
              ariaLabel: 'Date de naissance',
            },
            ageConfirmation: {
              ariaLabel: "Je confirme que j'ai au moins 18 ans",
            },
            acceptTerms: {
              ariaLabel: "J'accepte les conditions générales",
            },
            acceptPrivacy: {
              ariaLabel: "J'accepte la politique de confidentialité",
            },
            acceptMarketing: {
              ariaLabel: 'Je souhaite recevoir des communications marketing',
            },
          },
          actions: {
            createAccount: 'Créer un compte',
            creating: 'Création du compte...',
          },
          links: {
            termsLink: 'Conditions générales',
            privacyLink: 'Politique de confidentialité',
            loginLink: 'Connectez-vous ici',
          },
          messages: {
            alreadyHaveAccount: 'Vous avez déjà un compte ?',
            ageConfirmation: "Je confirme que j'ai au moins {{age}} ans",
            acceptTerms: "J'accepte les",
            acceptPrivacy: "J'accepte la",
            acceptMarketing: 'Je souhaite recevoir des communications marketing',
            withdrawalNotice: 'Droit de rétractation',
            error: 'Erreur',
            networkError: 'Erreur réseau. Veuillez réessayer.',
          },
        },
      },
      dashboard: {
        title: 'Tableau de bord',
        welcome: 'Bienvenue, {{firstName}} {{lastName}} !',
        email: 'E-mail',
        tenantId: 'ID du locataire',
        statistics: {
          title: 'Statistiques',
          description: 'Vos statistiques du tableau de bord apparaîtront ici.',
        },
        recentActivity: {
          title: 'Activité récente',
          description: 'Les activités récentes seront affichées ici.',
        },
        quickActions: {
          title: 'Actions rapides',
          manageTenants: 'Gérer les locataires',
          accountSettings: 'Paramètres du compte',
        },
        alerts: {
          settingsComingSoon: 'Les paramètres seront bientôt implémentés',
        },
      },
      customerTypeSelection: {
        title: 'Comment vous inscrivez-vous ?',
        subtitle: 'Choisissez le type de compte qui correspond le mieux à vos besoins',
        private: {
          ariaLabel: "S'inscrire en tant que client privé",
          title: 'Client privé',
          description: 'Acheteur individuel',
          details: 'Pour les achats personnels et le shopping',
        },
        business: {
          ariaLabel: "S'inscrire en tant que client professionnel",
          title: 'Client professionnel',
          description: 'Entreprise ou organisation',
          details: 'Pour les achats professionnels et les opérations B2B',
        },
        actions: {
          continue: 'Continuer',
        },
        login: {
          prompt: 'Vous avez déjà un compte ?',
          link: 'Connectez-vous ici',
        },
      },
      login: {
        title: 'Connexion à B2Connect',
        e2eMode: {
          title: 'Mode test E2E actif',
          description: 'Tout e-mail/mot de passe fonctionnera. Backend non requis.',
        },
        devHelp: {
          hint: '💡 Utilisez les identifiants de test : {{email}} / {{password}}',
          email: 'admin@example.com',
          password: 'password',
        },
        form: {
          email: {
            label: 'E-mail',
            placeholder: 'Entrez votre e-mail',
          },
          password: {
            label: 'Mot de passe',
            placeholder: 'Entrez votre mot de passe',
          },
        },
        actions: {
          loggingIn: 'Connexion en cours...',
          login: 'Se connecter',
        },
        signup: {
          prompt: "Vous n'avez pas de compte ?",
          link: "S'inscrire",
        },
      },
      productListing: {
        title: 'Boutique B2Connect',
        subtitle: 'Trouvez les meilleurs produits pour votre entreprise',
        search: {
          label: 'Rechercher des produits',
          placeholder: 'Rechercher par nom, SKU ou description...',
        },
        sort: {
          label: 'Trier par',
          options: {
            name: 'Nom (A-Z)',
            priceAsc: 'Prix (Croissant)',
            priceDesc: 'Prix (Décroissant)',
            rating: 'Évaluation (Décroissant)',
          },
        },
        filters: {
          title: 'Filtres',
        },
        category: {
          label: 'Catégorie',
        },
        priceRange: {
          label: 'Fourchette de prix',
          placeholder: '€0 - €5000 (bientôt disponible)',
        },
        inStockOnly: 'En stock uniquement',
        results: {
          foundFor: 'Trouvé pour :',
          loading: 'Chargement des produits...',
          noProducts: 'Aucun produit trouvé',
          noProductsMessage: "Essayez d'ajuster vos filtres ou votre recherche",
          clearFilters: 'Effacer les filtres',
          retry: 'Réessayer',
        },
        pagination: {
          previous: '← Précédent',
          next: 'Suivant →',
        },
      },
      customerLookup: {
        header: {
          newRegistration: 'Nouvelle inscription',
          welcomeBack: 'Bienvenue de retour',
          enterEmailPrompt: 'Entrez votre adresse e-mail pour commencer',
          customerInfoFound: 'Informations client trouvées',
        },
        form: {
          email: {
            label: 'Adresse e-mail *',
            placeholder: 'nom@exemple.com',
            ariaLabel: 'Adresse e-mail',
          },
          status: {
            searching: 'Recherche en cours...',
          },
          error: {
            title: 'Erreur de recherche client',
          },
          success: {
            title: 'Client trouvé !',
            welcomeMessage: 'Bienvenue de retour, {name} !',
          },
          customerDetails: {
            customerNumber: 'Numéro de client',
            customerType: 'Type de client',
            privateCustomer: 'Client privé',
            businessCustomer: 'Client professionnel',
          },
          businessDetails: {
            title: 'Informations professionnelles',
            company: 'Entreprise :',
            phone: 'Téléphone :',
            country: 'Pays :',
            creditLimit: 'Limite de crédit :',
          },
          actions: {
            searchCustomer: 'Rechercher client',
            searching: 'Recherche en cours...',
            proceed: 'Continuer',
            newSearch: 'Nouvelle recherche',
            cancel: 'Annuler',
          },
        },
        newCustomer: {
          title: 'Êtes-vous un nouveau client ?',
          message:
            'Vous pouvez vous inscrire maintenant et bénéficier de vos informations sauvegardées plus tard.',
          registerButton: 'Nouvelle inscription',
        },
        diagnostic: {
          title: '🔧 Infos de diagnostic (Dev uniquement)',
        },
      },
      productDetail: {
        breadcrumb: {
          home: 'Accueil',
          products: 'Produits',
        },
        loading: {
          message: 'Chargement des détails du produit...',
        },
        error: {
          retry: 'Réessayer',
        },
        price: {
          overview: 'Aperçu des prix',
          vatNotice: "Tous les prix incluent la TVA conformément à la loi sur l'affichage des prix",
        },
        stock: {
          inStock: '✓ En stock',
          outOfStock: '✗ Rupture de stock',
          available: '({count} disponible(s))',
        },
        actions: {
          addToCart: 'Ajouter au panier',
        },
        share: {
          label: 'Partager :',
        },
        specifications: {
          title: 'Spécifications',
        },
        reviews: {
          title: 'Avis clients',
          verified: '✓ Vérifié',
          byAuthor: 'par {author}',
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
        validation: {
          required: 'Se requiere el código de país y el número de IVA',
        },
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
        acceptance: {
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
      invoice: {
        loading: 'Cargando factura...',
        error: 'Error al cargar la factura',
        retry: 'Reintentar',
        noInvoice: 'No hay factura para mostrar',
        status: {
          invoice: 'Factura',
          reverseCharge: '⚠️ Reverse Charge (0% IVA)',
          overdue: 'Vencida',
        },
        labels: {
          issued: 'Emitida',
          due: 'Vencimiento',
          payment: 'Pago',
          paidOn: 'Pagada el',
          from: 'De',
          billTo: 'Facturar a',
          reverseCharge: '(Reverse Charge)',
        },
        table: {
          headers: {
            product: 'Producto',
            qty: 'Cant.',
            unitPrice: 'Precio unit.',
            subtotal: 'Subtotal',
            tax: 'Impuesto',
            total: 'Total',
          },
        },
        pricing: {
          subtotal: 'Subtotal:',
          shipping: 'Envío:',
          vat: 'IVA ({{rate}}%):',
          reverseCharge: 'Reverse Charge (0% IVA):',
          total: 'Total:',
        },
        actions: {
          downloadPdf: 'Descargar PDF',
          sendEmail: 'Enviar por email',
          modify: 'Modificar',
          print: 'Imprimir',
        },
        compliance: {
          vatNotice: 'El IVA mostrado arriba cumple con la legislación aplicable.',
          paymentInfo: 'Información de pago',
          method: 'Método:',
        },
      },
      cart: {
        title: 'Carrito de compras',
        itemCount: '{{count}} artículo(s) en tu carrito',
        empty: {
          title: 'Tu carrito está vacío',
          message: 'Descubre productos increíbles y comienza a comprar',
          button: 'Continuar comprando',
        },
        table: {
          headers: {
            product: 'Producto',
            price: 'Precio',
            quantity: 'Cantidad',
            total: 'Total',
          },
        },
        actions: {
          continueShopping: 'Continuar comprando',
          remove: 'Eliminar del carrito',
        },
        orderSummary: {
          title: 'Resumen del pedido',
          coupon: {
            label: '¿Tienes un código de cupón?',
            placeholder: 'Ingresa código de cupón',
            apply: 'Aplicar',
          },
          pricing: {
            subtotal: 'Subtotal',
            shipping: 'Envío',
            free: 'GRATIS',
            netPrice: 'Precio neto (sin IVA)',
            vat: 'IVA ({{rate}}%)',
            total: 'Total (con IVA)',
          },
        },
        checkout: {
          button: 'Proceder al pago →',
          guest: 'Continuar como invitado',
          secure: '🔒 Pago seguro',
        },
        trustBadges: {
          moneyBack: '✓ Garantía de devolución de 30 días',
          returns: '✓ Devoluciones y cambios gratuitos',
          ssl: '✓ Pago SSL encriptado seguro',
        },
      },
      registration: {
        check: {
          title: 'Verificar tipo de registro',
          subtitle: 'Verifique si ya está registrado como cliente existente',
          form: {
            email: {
              label: 'Dirección de correo electrónico',
              placeholder: 'ejemplo@empresa.es',
            },
            businessType: {
              label: 'Tipo de empresa',
              placeholder: '-- Por favor seleccione --',
              b2c: 'B2C (Persona física / Autónomo)',
              b2b: 'B2B (Empresa / SL / SA)',
            },
            firstName: {
              label: 'Nombre',
              placeholder: 'Juan',
            },
            lastName: {
              label: 'Apellidos',
              placeholder: 'García',
            },
            companyName: {
              label: 'Nombre de la empresa',
              placeholder: 'Ejemplo SL',
            },
            phone: {
              label: 'Teléfono',
              placeholder: '+34 91 123 45 67',
            },
          },
          buttons: {
            check: 'Verificar',
            checking: 'Verificando...',
            newCheck: 'Nueva verificación',
            continueWithData: 'Continuar con datos de cliente',
            continueRegistration: 'Continuar registro',
            back: 'Atrás',
          },
          alerts: {
            error: 'Error',
          },
          results: {
            existingCustomer: {
              title: '¡Bienvenido de nuevo!',
              description:
                'Ya está registrado en nuestro sistema. Sus datos se rellenarán automáticamente.',
            },
            newCustomer: {
              title: 'Registro de nuevo cliente',
              description: 'Será redirigido al proceso de registro regular.',
            },
            customerData: 'Sus datos de cliente:',
            customerNumber: 'Número de cliente:',
            name: 'Nombre:',
            email: 'Correo electrónico:',
            phone: 'Teléfono:',
            address: 'Dirección:',
            matchScore: 'Puntuación de coincidencia:',
          },
          info: {
            title: 'Información',
            existingCustomer:
              'Cliente existente: Ya está registrado en nuestro sistema. Sus datos se rellenarán automáticamente.',
            newCustomer: 'Nuevo cliente: Será redirigido al proceso de registro regular.',
            checkDetails:
              'La verificación se realiza basándose en el correo electrónico, nombre y opcionalmente teléfono/dirección.',
          },
        },
        privateCustomerRegistration: {
          title: 'Crea tu cuenta',
          subtitle: 'Únete a B2Connect y comienza a comprar hoy',
          form: {
            email: {
              label: 'Dirección de correo electrónico',
              placeholder: 'tu@ejemplo.com',
              ariaLabel: 'Dirección de correo electrónico',
            },
            password: {
              label: 'Contraseña',
              placeholder: '••••••••',
              ariaLabel: 'Contraseña',
            },
            confirmPassword: {
              label: 'Confirmar contraseña',
              placeholder: '••••••••',
              ariaLabel: 'Confirmar contraseña',
            },
            firstName: {
              label: 'Nombre',
              placeholder: 'Juan',
              ariaLabel: 'Nombre',
            },
            lastName: {
              label: 'Apellido',
              placeholder: 'Pérez',
              ariaLabel: 'Apellido',
            },
            phone: {
              label: 'Número de teléfono',
              placeholder: '+34 123 456 789',
              ariaLabel: 'Número de teléfono',
            },
            streetAddress: {
              label: 'Dirección',
              placeholder: 'Calle Principal 123',
              ariaLabel: 'Dirección',
            },
            city: {
              label: 'Ciudad',
              placeholder: 'Madrid',
              ariaLabel: 'Ciudad',
            },
            postalCode: {
              label: 'Código postal',
              placeholder: '28001',
              ariaLabel: 'Código postal',
            },
            country: {
              label: 'País',
              placeholder: 'Selecciona un país',
              ariaLabel: 'País',
              options: {
                select: 'Selecciona un país',
                DE: 'Alemania (DE)',
                AT: 'Austria (AT)',
                CH: 'Suiza (CH)',
                FR: 'Francia (FR)',
                NL: 'Países Bajos (NL)',
                BE: 'Bélgica (BE)',
                LU: 'Luxemburgo (LU)',
                PL: 'Polonia (PL)',
                CZ: 'República Checa (CZ)',
              },
            },
            state: {
              label: 'Estado / Provincia',
              placeholder: 'Estado / Provincia',
              ariaLabel: 'Estado o provincia',
            },
            dateOfBirth: {
              label: 'Fecha de nacimiento',
              ariaLabel: 'Fecha de nacimiento',
            },
            ageConfirmation: {
              ariaLabel: 'Confirmo que tengo al menos 18 años',
            },
            acceptTerms: {
              ariaLabel: 'Acepto los términos y condiciones',
            },
            acceptPrivacy: {
              ariaLabel: 'Acepto la política de privacidad',
            },
            acceptMarketing: {
              ariaLabel: 'Quiero recibir comunicaciones de marketing',
            },
          },
          actions: {
            createAccount: 'Crear cuenta',
            creating: 'Creando cuenta...',
          },
          links: {
            termsLink: 'Términos y condiciones',
            privacyLink: 'Política de privacidad',
            loginLink: 'Inicia sesión aquí',
          },
          messages: {
            alreadyHaveAccount: '¿Ya tienes una cuenta?',
            ageConfirmation: 'Confirmo que tengo al menos {{age}} años',
            acceptTerms: 'Acepto los',
            acceptPrivacy: 'Acepto la',
            acceptMarketing: 'Quiero recibir comunicaciones de marketing',
            withdrawalNotice: 'Derecho de desistimiento',
            error: 'Error',
            networkError: 'Error de red. Por favor, inténtalo de nuevo.',
          },
        },
      },
      dashboard: {
        title: 'Panel',
        welcome: '¡Bienvenido, {{firstName}} {{lastName}}!',
        email: 'Correo electrónico',
        tenantId: 'ID del inquilino',
        statistics: {
          title: 'Estadísticas',
          description: 'Sus estadísticas del panel aparecerán aquí.',
        },
        recentActivity: {
          title: 'Actividad reciente',
          description: 'Las actividades recientes se mostrarán aquí.',
        },
        quickActions: {
          title: 'Acciones rápidas',
          manageTenants: 'Administrar inquilinos',
          accountSettings: 'Configuración de la cuenta',
        },
        alerts: {
          settingsComingSoon: 'La configuración se implementará pronto',
        },
      },
      customerTypeSelection: {
        title: '¿Cómo se registra?',
        subtitle: 'Elija el tipo de cuenta que mejor se adapte a sus necesidades',
        private: {
          ariaLabel: 'Registrarse como cliente privado',
          title: 'Cliente privado',
          description: 'Comprador individual',
          details: 'Para compras personales y compras',
        },
        business: {
          ariaLabel: 'Registrarse como cliente empresarial',
          title: 'Cliente empresarial',
          description: 'Empresa u organización',
          details: 'Para compras comerciales y operaciones B2B',
        },
        actions: {
          continue: 'Continuar',
        },
        login: {
          prompt: '¿Ya tiene una cuenta?',
          link: 'Inicie sesión aquí',
        },
      },
      login: {
        title: 'Iniciar sesión en B2Connect',
        e2eMode: {
          title: 'Modo de prueba E2E activo',
          description: 'Cualquier correo electrónico/contraseña funcionará. Backend no requerido.',
        },
        devHelp: {
          hint: '💡 Use credenciales de prueba: {{email}} / {{password}}',
          email: 'admin@example.com',
          password: 'password',
        },
        form: {
          email: {
            label: 'Correo electrónico',
            placeholder: 'Ingrese su correo electrónico',
          },
          password: {
            label: 'Contraseña',
            placeholder: 'Ingrese su contraseña',
          },
        },
        actions: {
          loggingIn: 'Iniciando sesión...',
          login: 'Iniciar sesión',
        },
        signup: {
          prompt: '¿No tiene una cuenta?',
          link: 'Registrarse',
        },
      },
      productListing: {
        title: 'Tienda B2Connect',
        subtitle: 'Encuentra los mejores productos para tu empresa',
        search: {
          label: 'Buscar productos',
          placeholder: 'Buscar por nombre, SKU o descripción...',
        },
        sort: {
          label: 'Ordenar por',
          options: {
            name: 'Nombre (A-Z)',
            priceAsc: 'Precio (Ascendente)',
            priceDesc: 'Precio (Descendente)',
            rating: 'Valoración (Descendente)',
          },
        },
        filters: {
          title: 'Filtros',
        },
        category: {
          label: 'Categoría',
        },
        priceRange: {
          label: 'Rango de precios',
          placeholder: '€0 - €5000 (próximamente)',
        },
        inStockOnly: 'Solo en stock',
        results: {
          foundFor: 'Encontrado para:',
          loading: 'Cargando productos...',
          noProducts: 'No se encontraron productos',
          noProductsMessage: 'Intenta ajustar tus filtros o consulta de búsqueda',
          clearFilters: 'Limpiar filtros',
          retry: 'Reintentar',
        },
        pagination: {
          previous: '← Anterior',
          next: 'Siguiente →',
        },
      },
      customerLookup: {
        header: {
          newRegistration: 'Nuevo registro',
          welcomeBack: 'Bienvenido de vuelta',
          enterEmailPrompt: 'Ingrese su dirección de correo electrónico para comenzar',
          customerInfoFound: 'Información del cliente encontrada',
        },
        form: {
          email: {
            label: 'Dirección de correo electrónico *',
            placeholder: 'nombre@ejemplo.com',
            ariaLabel: 'Dirección de correo electrónico',
          },
          status: {
            searching: 'Buscando...',
          },
          error: {
            title: 'Error en la búsqueda de cliente',
          },
          success: {
            title: '¡Cliente encontrado!',
            welcomeMessage: '¡Bienvenido de vuelta, {name}!',
          },
          customerDetails: {
            customerNumber: 'Número de cliente',
            customerType: 'Tipo de cliente',
            privateCustomer: 'Cliente privado',
            businessCustomer: 'Cliente empresarial',
          },
          businessDetails: {
            title: 'Información empresarial',
            company: 'Empresa:',
            phone: 'Teléfono:',
            country: 'País:',
            creditLimit: 'Límite de crédito:',
          },
          actions: {
            searchCustomer: 'Buscar cliente',
            searching: 'Buscando...',
            proceed: 'Continuar',
            newSearch: 'Nueva búsqueda',
            cancel: 'Cancelar',
          },
        },
        newCustomer: {
          title: '¿Es usted un cliente nuevo?',
          message: 'Puede registrarse ahora y beneficiarse de su información guardada más tarde.',
          registerButton: 'Nuevo registro',
        },
        diagnostic: {
          title: '🔧 Información de diagnóstico (Solo Dev)',
        },
      },
      productDetail: {
        breadcrumb: {
          home: 'Inicio',
          products: 'Productos',
        },
        loading: {
          message: 'Cargando detalles del producto...',
        },
        error: {
          retry: 'Reintentar',
        },
        price: {
          overview: 'Resumen de precios',
          vatNotice:
            'Todos los precios incluyen IVA de acuerdo con la normativa de indicación de precios',
        },
        stock: {
          inStock: '✓ En stock',
          outOfStock: '✗ Agotado',
          available: '({count} disponible(s))',
        },
        actions: {
          addToCart: 'Añadir al carrito',
        },
        share: {
          label: 'Compartir:',
        },
        specifications: {
          title: 'Especificaciones',
        },
        reviews: {
          title: 'Opiniones de clientes',
          verified: '✓ Verificado',
          byAuthor: 'por {author}',
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
        validation: {
          required: 'Il codice paese e il numero IVA sono obbligatori',
        },
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
        acceptance: {
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
      invoice: {
        loading: 'Caricamento fattura...',
        error: 'Errore nel caricamento della fattura',
        retry: 'Riprova',
        noInvoice: 'Nessuna fattura da visualizzare',
        status: {
          invoice: 'Fattura',
          reverseCharge: '⚠️ Reverse Charge (0% IVA)',
          overdue: 'Scaduta',
        },
        labels: {
          issued: 'Emessa',
          due: 'Scadenza',
          payment: 'Pagamento',
          paidOn: 'Pagata il',
          from: 'Da',
          billTo: 'Fatturare a',
          reverseCharge: '(Reverse Charge)',
        },
        table: {
          headers: {
            product: 'Prodotto',
            qty: 'Qtà',
            unitPrice: 'Prezzo unit.',
            subtotal: 'Subtotale',
            tax: 'Tassa',
            total: 'Totale',
          },
        },
        pricing: {
          subtotal: 'Subtotale:',
          shipping: 'Spedizione:',
          vat: 'IVA ({{rate}}%):',
          reverseCharge: 'Reverse Charge (0% IVA):',
          total: 'Totale:',
        },
        actions: {
          downloadPdf: 'Scarica PDF',
          sendEmail: 'Invia email',
          modify: 'Modifica',
          print: 'Stampa',
        },
        compliance: {
          vatNotice: "L'IVA mostrata sopra è conforme alla legislazione applicabile.",
          paymentInfo: 'Informazioni di pagamento',
          method: 'Metodo:',
        },
      },
      cart: {
        title: 'Carrello della spesa',
        itemCount: '{{count}} articolo(i) nel tuo carrello',
        empty: {
          title: 'Il tuo carrello è vuoto',
          message: 'Scopri prodotti straordinari e inizia a fare acquisti',
          button: 'Continua a fare acquisti',
        },
        table: {
          headers: {
            product: 'Prodotto',
            price: 'Prezzo',
            quantity: 'Quantità',
            total: 'Totale',
          },
        },
        actions: {
          continueShopping: 'Continua a fare acquisti',
          remove: 'Rimuovi dal carrello',
        },
        orderSummary: {
          title: 'Riepilogo ordine',
          coupon: {
            label: 'Hai un codice coupon?',
            placeholder: 'Inserisci codice coupon',
            apply: 'Applica',
          },
          pricing: {
            subtotal: 'Subtotale',
            shipping: 'Spedizione',
            free: 'GRATIS',
            netPrice: 'Prezzo netto (IVA esclusa)',
            vat: 'IVA ({{rate}}%)',
            total: 'Totale (IVA inclusa)',
          },
        },
        checkout: {
          button: 'Procedi al pagamento →',
          guest: 'Continua come ospite',
          secure: '🔒 Pagamento sicuro',
        },
        trustBadges: {
          moneyBack: '✓ Garanzia di rimborso 30 giorni',
          returns: '✓ Resi e cambi gratuiti',
          ssl: '✓ Pagamento SSL criptato sicuro',
        },
      },
      registration: {
        check: {
          title: 'Verifica tipo di registrazione',
          subtitle: 'Verifica se sei già registrato come cliente esistente',
          form: {
            email: {
              label: 'Indirizzo e-mail',
              placeholder: 'esempio@azienda.it',
            },
            businessType: {
              label: 'Tipo di azienda',
              placeholder: '-- Seleziona --',
              b2c: 'B2C (Persona fisica / Partita IVA individuale)',
              b2b: 'B2B (Azienda / SRL / SPA)',
            },
            firstName: {
              label: 'Nome',
              placeholder: 'Mario',
            },
            lastName: {
              label: 'Cognome',
              placeholder: 'Rossi',
            },
            companyName: {
              label: 'Nome azienda',
              placeholder: 'Esempio SRL',
            },
            phone: {
              label: 'Telefono',
              placeholder: '+39 02 123 4567',
            },
          },
          buttons: {
            check: 'Verifica',
            checking: 'Verifica in corso...',
            newCheck: 'Nuova verifica',
            continueWithData: 'Continua con dati cliente',
            continueRegistration: 'Continua registrazione',
            back: 'Indietro',
          },
          alerts: {
            error: 'Errore',
          },
          results: {
            existingCustomer: {
              title: 'Benvenuto!',
              description:
                'Sei già registrato nel nostro sistema. I tuoi dati verranno compilati automaticamente.',
            },
            newCustomer: {
              title: 'Registrazione nuovo cliente',
              description: 'Verrai reindirizzato al processo di registrazione regolare.',
            },
            customerData: 'I tuoi dati cliente:',
            customerNumber: 'Numero cliente:',
            name: 'Nome:',
            email: 'E-mail:',
            phone: 'Telefono:',
            address: 'Indirizzo:',
            matchScore: 'Punteggio corrispondenza:',
          },
          info: {
            title: 'Informazioni',
            existingCustomer:
              'Cliente esistente: Sei già registrato nel nostro sistema. I tuoi dati verranno compilati automaticamente.',
            newCustomer:
              'Nuovo cliente: Verrai reindirizzato al processo di registrazione regolare.',
            checkDetails:
              'La verifica viene effettuata sulla base di e-mail, nome e opzionalmente telefono/indirizzo.',
          },
        },
        privateCustomerRegistration: {
          title: 'Crea il tuo account',
          subtitle: 'Unisciti a B2Connect e inizia a fare acquisti oggi',
          form: {
            email: {
              label: 'Indirizzo e-mail',
              placeholder: 'tua@email.com',
              ariaLabel: 'Indirizzo e-mail',
            },
            password: {
              label: 'Password',
              placeholder: '••••••••',
              ariaLabel: 'Password',
            },
            confirmPassword: {
              label: 'Conferma password',
              placeholder: '••••••••',
              ariaLabel: 'Conferma password',
            },
            firstName: {
              label: 'Nome',
              placeholder: 'Mario',
              ariaLabel: 'Nome',
            },
            lastName: {
              label: 'Cognome',
              placeholder: 'Rossi',
              ariaLabel: 'Cognome',
            },
            phone: {
              label: 'Numero di telefono',
              placeholder: '+39 123 456 789',
              ariaLabel: 'Numero di telefono',
            },
            streetAddress: {
              label: 'Indirizzo',
              placeholder: 'Via Principale 123',
              ariaLabel: 'Indirizzo',
            },
            city: {
              label: 'Città',
              placeholder: 'Roma',
              ariaLabel: 'Città',
            },
            postalCode: {
              label: 'Codice postale',
              placeholder: '00100',
              ariaLabel: 'Codice postale',
            },
            country: {
              label: 'Paese',
              placeholder: 'Seleziona un paese',
              ariaLabel: 'Paese',
              options: {
                select: 'Seleziona un paese',
                DE: 'Germania (DE)',
                AT: 'Austria (AT)',
                CH: 'Svizzera (CH)',
                FR: 'Francia (FR)',
                NL: 'Paesi Bassi (NL)',
                BE: 'Belgio (BE)',
                LU: 'Lussemburgo (LU)',
                PL: 'Polonia (PL)',
                CZ: 'Repubblica Ceca (CZ)',
              },
            },
            state: {
              label: 'Stato / Provincia',
              placeholder: 'Stato / Provincia',
              ariaLabel: 'Stato o provincia',
            },
            dateOfBirth: {
              label: 'Data di nascita',
              ariaLabel: 'Data di nascita',
            },
            ageConfirmation: {
              ariaLabel: 'Confermo di avere almeno 18 anni',
            },
            acceptTerms: {
              ariaLabel: 'Accetto i termini e le condizioni',
            },
            acceptPrivacy: {
              ariaLabel: "Accetto l'informativa sulla privacy",
            },
            acceptMarketing: {
              ariaLabel: 'Desidero ricevere comunicazioni di marketing',
            },
          },
          actions: {
            createAccount: 'Crea account',
            creating: 'Creazione account...',
          },
          links: {
            termsLink: 'Termini e condizioni',
            privacyLink: 'Informativa sulla privacy',
            loginLink: 'Accedi qui',
          },
          messages: {
            alreadyHaveAccount: 'Hai già un account?',
            ageConfirmation: 'Confermo di avere almeno {{age}} anni',
            acceptTerms: 'Accetto i',
            acceptPrivacy: "Accetto l'",
            acceptMarketing: 'Desidero ricevere comunicazioni di marketing',
            withdrawalNotice: 'Diritto di recesso',
            error: 'Errore',
            networkError: 'Errore di rete. Riprova.',
          },
        },
      },
      dashboard: {
        title: 'Dashboard',
        welcome: 'Benvenuto, {{firstName}} {{lastName}}!',
        email: 'E-mail',
        tenantId: 'ID tenant',
        statistics: {
          title: 'Statistiche',
          description: 'Le tue statistiche del dashboard appariranno qui.',
        },
        recentActivity: {
          title: 'Attività recente',
          description: 'Le attività recenti verranno visualizzate qui.',
        },
        quickActions: {
          title: 'Azioni rapide',
          manageTenants: 'Gestisci tenant',
          accountSettings: 'Impostazioni account',
        },
        alerts: {
          settingsComingSoon: 'Le impostazioni saranno implementate presto',
        },
      },
      customerTypeSelection: {
        title: 'Come ti registri?',
        subtitle: 'Scegli il tipo di account che meglio si adatta alle tue esigenze',
        private: {
          ariaLabel: 'Registrati come cliente privato',
          title: 'Cliente privato',
          description: 'Acquirente individuale',
          details: 'Per acquisti personali e shopping',
        },
        business: {
          ariaLabel: 'Registrati come cliente aziendale',
          title: 'Cliente aziendale',
          description: 'Azienda o organizzazione',
          details: 'Per acquisti aziendali e operazioni B2B',
        },
        actions: {
          continue: 'Continua',
        },
        login: {
          prompt: 'Hai già un account?',
          link: 'Accedi qui',
        },
      },
      login: {
        title: 'Accedi a B2Connect',
        e2eMode: {
          title: 'Modalità test E2E attiva',
          description: 'Qualsiasi e-mail/password funzionerà. Backend non richiesto.',
        },
        devHelp: {
          hint: '💡 Usa credenziali di test: {{email}} / {{password}}',
          email: 'admin@example.com',
          password: 'password',
        },
        form: {
          email: {
            label: 'E-mail',
            placeholder: 'Inserisci la tua e-mail',
          },
          password: {
            label: 'Password',
            placeholder: 'Inserisci la tua password',
          },
        },
        actions: {
          loggingIn: 'Accesso in corso...',
          login: 'Accedi',
        },
        signup: {
          prompt: 'Non hai un account?',
          link: 'Registrati',
        },
      },
      productListing: {
        title: 'Negozio B2Connect',
        subtitle: 'Trova i migliori prodotti per la tua azienda',
        search: {
          label: 'Cerca prodotti',
          placeholder: 'Cerca per nome, SKU o descrizione...',
        },
        sort: {
          label: 'Ordina per',
          options: {
            name: 'Nome (A-Z)',
            priceAsc: 'Prezzo (Crescente)',
            priceDesc: 'Prezzo (Decrescente)',
            rating: 'Valutazione (Decrescente)',
          },
        },
        filters: {
          title: 'Filtri',
        },
        category: {
          label: 'Categoria',
        },
        priceRange: {
          label: 'Fascia di prezzo',
          placeholder: '€0 - €5000 (presto disponibile)',
        },
        inStockOnly: 'Solo disponibili',
        results: {
          foundFor: 'Trovato per:',
          loading: 'Caricamento prodotti...',
          noProducts: 'Nessun prodotto trovato',
          noProductsMessage: 'Prova a regolare i tuoi filtri o la ricerca',
          clearFilters: 'Cancella filtri',
          retry: 'Riprova',
        },
        pagination: {
          previous: '← Precedente',
          next: 'Successivo →',
        },
      },
      customerLookup: {
        header: {
          newRegistration: 'Nuova registrazione',
          welcomeBack: 'Bentornato',
          enterEmailPrompt: 'Inserisci il tuo indirizzo e-mail per iniziare',
          customerInfoFound: 'Informazioni cliente trovate',
        },
        form: {
          email: {
            label: 'Indirizzo e-mail *',
            placeholder: 'nome@esempio.com',
            ariaLabel: 'Indirizzo e-mail',
          },
          status: {
            searching: 'Ricerca in corso...',
          },
          error: {
            title: 'Errore nella ricerca cliente',
          },
          success: {
            title: 'Cliente trovato!',
            welcomeMessage: 'Bentornato, {name}!',
          },
          customerDetails: {
            customerNumber: 'Numero cliente',
            customerType: 'Tipo cliente',
            privateCustomer: 'Cliente privato',
            businessCustomer: 'Cliente aziendale',
          },
          businessDetails: {
            title: 'Informazioni aziendali',
            company: 'Azienda:',
            phone: 'Telefono:',
            country: 'Paese:',
            creditLimit: 'Limite di credito:',
          },
          actions: {
            searchCustomer: 'Cerca cliente',
            searching: 'Ricerca in corso...',
            proceed: 'Continua',
            newSearch: 'Nuova ricerca',
            cancel: 'Annulla',
          },
        },
        newCustomer: {
          title: 'Sei un nuovo cliente?',
          message: 'Puoi registrarti ora e beneficiare delle tue informazioni salvate in seguito.',
          registerButton: 'Nuova registrazione',
        },
        diagnostic: {
          title: '🔧 Info diagnostiche (Solo Dev)',
        },
      },
      productDetail: {
        breadcrumb: {
          home: 'Home',
          products: 'Prodotti',
        },
        loading: {
          message: 'Caricamento dettagli prodotto...',
        },
        error: {
          retry: 'Riprova',
        },
        price: {
          overview: 'Panoramica prezzi',
          vatNotice: 'Tutti i prezzi includono IVA in conformità con la normativa sui prezzi',
        },
        stock: {
          inStock: '✓ Disponibile',
          outOfStock: '✗ Esaurito',
          available: '({count} disponibile/i)',
        },
        actions: {
          addToCart: 'Aggiungi al carrello',
        },
        share: {
          label: 'Condividi:',
        },
        specifications: {
          title: 'Specifiche',
        },
        reviews: {
          title: 'Recensioni clienti',
          verified: '✓ Verificato',
          byAuthor: 'di {author}',
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
        validation: {
          required: 'O código do país e o número de IVA são obrigatórios',
        },
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
        acceptance: {
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
      invoice: {
        loading: 'A carregar fatura...',
        error: 'Erro ao carregar fatura',
        retry: 'Tentar novamente',
        noInvoice: 'Nenhuma fatura para mostrar',
        status: {
          invoice: 'Fatura',
          reverseCharge: '⚠️ Reverse Charge (0% IVA)',
          overdue: 'Vencida',
        },
        labels: {
          issued: 'Emitida',
          due: 'Vencimento',
          payment: 'Pagamento',
          paidOn: 'Paga em',
          from: 'De',
          billTo: 'Faturar a',
          reverseCharge: '(Reverse Charge)',
        },
        table: {
          headers: {
            product: 'Produto',
            qty: 'Qtd.',
            unitPrice: 'Preço unit.',
            subtotal: 'Subtotal',
            tax: 'Imposto',
            total: 'Total',
          },
        },
        pricing: {
          subtotal: 'Subtotal:',
          shipping: 'Envio:',
          vat: 'IVA ({{rate}}%):',
          reverseCharge: 'Reverse Charge (0% IVA):',
          total: 'Total:',
        },
        actions: {
          downloadPdf: 'Descarregar PDF',
          sendEmail: 'Enviar email',
          modify: 'Modificar',
          print: 'Imprimir',
        },
        compliance: {
          vatNotice: 'O IVA mostrado acima está em conformidade com a legislação aplicável.',
          paymentInfo: 'Informações de pagamento',
          method: 'Método:',
        },
      },
      cart: {
        title: 'Carrinho de compras',
        itemCount: '{{count}} item(ns) no seu carrinho',
        empty: {
          title: 'O seu carrinho está vazio',
          message: 'Descubra produtos incríveis e comece a comprar',
          button: 'Continuar a comprar',
        },
        table: {
          headers: {
            product: 'Produto',
            price: 'Preço',
            quantity: 'Quantidade',
            total: 'Total',
          },
        },
        actions: {
          continueShopping: 'Continuar a comprar',
          remove: 'Remover do carrinho',
        },
        orderSummary: {
          title: 'Resumo do pedido',
          coupon: {
            label: 'Tem um código de cupão?',
            placeholder: 'Introduza código de cupão',
            apply: 'Aplicar',
          },
          pricing: {
            subtotal: 'Subtotal',
            shipping: 'Envio',
            free: 'GRÁTIS',
            netPrice: 'Preço líquido (IVA excluído)',
            vat: 'IVA ({{rate}}%)',
            total: 'Total (IVA incluído)',
          },
        },
        checkout: {
          button: 'Proceder ao pagamento →',
          guest: 'Continuar como convidado',
          secure: '🔒 Pagamento seguro',
        },
        trustBadges: {
          moneyBack: '✓ Garantia de devolução de 30 dias',
          returns: '✓ Devoluções e trocas gratuitas',
          ssl: '✓ Pagamento SSL encriptado seguro',
        },
      },
      registration: {
        check: {
          title: 'Verificar tipo de registo',
          subtitle: 'Verifique se já está registado como cliente existente',
          form: {
            email: {
              label: 'Endereço de e-mail',
              placeholder: 'exemplo@empresa.pt',
            },
            businessType: {
              label: 'Tipo de empresa',
              placeholder: '-- Por favor selecione --',
              b2c: 'B2C (Pessoa singular / Trabalhador independente)',
              b2b: 'B2B (Empresa / Lda / SA)',
            },
            firstName: {
              label: 'Nome próprio',
              placeholder: 'João',
            },
            lastName: {
              label: 'Apelido',
              placeholder: 'Silva',
            },
            companyName: {
              label: 'Nome da empresa',
              placeholder: 'Exemplo Lda',
            },
            phone: {
              label: 'Telefone',
              placeholder: '+351 21 123 4567',
            },
          },
          buttons: {
            check: 'Verificar',
            checking: 'A verificar...',
            newCheck: 'Nova verificação',
            continueWithData: 'Continuar com dados de cliente',
            continueRegistration: 'Continuar registo',
            back: 'Voltar',
          },
          alerts: {
            error: 'Erro',
          },
          results: {
            existingCustomer: {
              title: 'Bem-vindo de volta!',
              description:
                'Já está registado no nosso sistema. Os seus dados serão preenchidos automaticamente.',
            },
            newCustomer: {
              title: 'Registo de novo cliente',
              description: 'Será redireccionado para o processo de registo regular.',
            },
            customerData: 'Os seus dados de cliente:',
            customerNumber: 'Número de cliente:',
            name: 'Nome:',
            email: 'E-mail:',
            phone: 'Telefone:',
            address: 'Morada:',
            matchScore: 'Pontuação de correspondência:',
          },
          info: {
            title: 'Informação',
            existingCustomer:
              'Cliente existente: Já está registado no nosso sistema. Os seus dados serão preenchidos automaticamente.',
            newCustomer: 'Novo cliente: Será redireccionado para o processo de registo regular.',
            checkDetails:
              'A verificação é efectuada com base no e-mail, nome e opcionalmente telefone/morada.',
          },
        },
        privateCustomerRegistration: {
          title: 'Crie a sua conta',
          subtitle: 'Junte-se ao B2Connect e comece a comprar hoje',
          form: {
            email: {
              label: 'Endereço de e-mail',
              placeholder: 'seu@exemplo.com',
              ariaLabel: 'Endereço de e-mail',
            },
            password: {
              label: 'Palavra-passe',
              placeholder: '••••••••',
              ariaLabel: 'Palavra-passe',
            },
            confirmPassword: {
              label: 'Confirmar palavra-passe',
              placeholder: '••••••••',
              ariaLabel: 'Confirmar palavra-passe',
            },
            firstName: {
              label: 'Primeiro nome',
              placeholder: 'João',
              ariaLabel: 'Primeiro nome',
            },
            lastName: {
              label: 'Último nome',
              placeholder: 'Silva',
              ariaLabel: 'Último nome',
            },
            phone: {
              label: 'Número de telefone',
              placeholder: '+351 123 456 789',
              ariaLabel: 'Número de telefone',
            },
            streetAddress: {
              label: 'Morada',
              placeholder: 'Rua Principal 123',
              ariaLabel: 'Morada',
            },
            city: {
              label: 'Cidade',
              placeholder: 'Lisboa',
              ariaLabel: 'Cidade',
            },
            postalCode: {
              label: 'Código postal',
              placeholder: '1000-001',
              ariaLabel: 'Código postal',
            },
            country: {
              label: 'País',
              placeholder: 'Selecionar um país',
              ariaLabel: 'País',
              options: {
                select: 'Selecionar um país',
                DE: 'Alemanha (DE)',
                AT: 'Áustria (AT)',
                CH: 'Suíça (CH)',
                FR: 'França (FR)',
                NL: 'Países Baixos (NL)',
                BE: 'Bélgica (BE)',
                LU: 'Luxemburgo (LU)',
                PL: 'Polónia (PL)',
                CZ: 'República Checa (CZ)',
              },
            },
            state: {
              label: 'Estado / Província',
              placeholder: 'Estado / Província',
              ariaLabel: 'Estado ou província',
            },
            dateOfBirth: {
              label: 'Data de nascimento',
              ariaLabel: 'Data de nascimento',
            },
            ageConfirmation: {
              ariaLabel: 'Confirmo que tenho pelo menos 18 anos',
            },
            acceptTerms: {
              ariaLabel: 'Aceito os termos e condições',
            },
            acceptPrivacy: {
              ariaLabel: 'Aceito a política de privacidade',
            },
            acceptMarketing: {
              ariaLabel: 'Quero receber comunicações de marketing',
            },
          },
          actions: {
            createAccount: 'Criar conta',
            creating: 'Criando conta...',
          },
          links: {
            termsLink: 'Termos e condições',
            privacyLink: 'Política de privacidade',
            loginLink: 'Iniciar sessão aqui',
          },
          messages: {
            alreadyHaveAccount: 'Já tem uma conta?',
            ageConfirmation: 'Confirmo que tenho pelo menos {{age}} anos',
            acceptTerms: 'Aceito os',
            acceptPrivacy: 'Aceito a',
            acceptMarketing: 'Quero receber comunicações de marketing',
            withdrawalNotice: 'Direito de rescisão',
            error: 'Erro',
            networkError: 'Erro de rede. Tente novamente.',
          },
        },
      },
      dashboard: {
        title: 'Painel',
        welcome: 'Bem-vindo, {{firstName}} {{lastName}}!',
        email: 'E-mail',
        tenantId: 'ID do inquilino',
        statistics: {
          title: 'Estatísticas',
          description: 'As suas estatísticas do painel aparecerão aqui.',
        },
        recentActivity: {
          title: 'Atividade recente',
          description: 'As atividades recentes serão exibidas aqui.',
        },
        quickActions: {
          title: 'Ações rápidas',
          manageTenants: 'Gerir inquilinos',
          accountSettings: 'Definições da conta',
        },
        alerts: {
          settingsComingSoon: 'As definições serão implementadas em breve',
        },
      },
      customerTypeSelection: {
        title: 'Como se regista?',
        subtitle: 'Escolha o tipo de conta que melhor se adapta às suas necessidades',
        private: {
          ariaLabel: 'Registar como cliente privado',
          title: 'Cliente privado',
          description: 'Comprador individual',
          details: 'Para compras pessoais e compras',
        },
        business: {
          ariaLabel: 'Registar como cliente empresarial',
          title: 'Cliente empresarial',
          description: 'Empresa ou organização',
          details: 'Para compras empresariais e operações B2B',
        },
        actions: {
          continue: 'Continuar',
        },
        login: {
          prompt: 'Já tem uma conta?',
          link: 'Inicie sessão aqui',
        },
      },
      login: {
        title: 'Iniciar sessão no B2Connect',
        e2eMode: {
          title: 'Modo de teste E2E ativo',
          description: 'Qualquer e-mail/palavra-passe funcionará. Backend não necessário.',
        },
        devHelp: {
          hint: '💡 Use credenciais de teste: {{email}} / {{password}}',
          email: 'admin@example.com',
          password: 'password',
        },
        form: {
          email: {
            label: 'E-mail',
            placeholder: 'Introduza o seu e-mail',
          },
          password: {
            label: 'Palavra-passe',
            placeholder: 'Introduza a sua palavra-passe',
          },
        },
        actions: {
          loggingIn: 'A iniciar sessão...',
          login: 'Iniciar sessão',
        },
        signup: {
          prompt: 'Não tem uma conta?',
          link: 'Registar-se',
        },
      },
      productListing: {
        title: 'Loja B2Connect',
        subtitle: 'Encontre os melhores produtos para sua empresa',
        search: {
          label: 'Pesquisar produtos',
          placeholder: 'Pesquisar por nome, SKU ou descrição...',
        },
        sort: {
          label: 'Ordenar por',
          options: {
            name: 'Nome (A-Z)',
            priceAsc: 'Preço (Crescente)',
            priceDesc: 'Preço (Decrescente)',
            rating: 'Avaliação (Decrescente)',
          },
        },
        filters: {
          title: 'Filtros',
        },
        category: {
          label: 'Categoria',
        },
        priceRange: {
          label: 'Faixa de preço',
          placeholder: '€0 - €5000 (em breve)',
        },
        inStockOnly: 'Apenas em estoque',
        results: {
          foundFor: 'Encontrado para:',
          loading: 'Carregando produtos...',
          noProducts: 'Nenhum produto encontrado',
          noProductsMessage: 'Tente ajustar seus filtros ou consulta de pesquisa',
          clearFilters: 'Limpar filtros',
          retry: 'Tentar novamente',
        },
        pagination: {
          previous: '← Anterior',
          next: 'Próximo →',
        },
      },
      customerLookup: {
        header: {
          newRegistration: 'Novo registo',
          welcomeBack: 'Bem-vindo de volta',
          enterEmailPrompt: 'Introduza o seu endereço de e-mail para começar',
          customerInfoFound: 'Informações do cliente encontradas',
        },
        form: {
          email: {
            label: 'Endereço de e-mail *',
            placeholder: 'nome@exemplo.com',
            ariaLabel: 'Endereço de e-mail',
          },
          status: {
            searching: 'A procurar...',
          },
          error: {
            title: 'Erro na pesquisa de cliente',
          },
          success: {
            title: 'Cliente encontrado!',
            welcomeMessage: 'Bem-vindo de volta, {name}!',
          },
          customerDetails: {
            customerNumber: 'Número do cliente',
            customerType: 'Tipo de cliente',
            privateCustomer: 'Cliente privado',
            businessCustomer: 'Cliente empresarial',
          },
          businessDetails: {
            title: 'Informações empresariais',
            company: 'Empresa:',
            phone: 'Telefone:',
            country: 'País:',
            creditLimit: 'Limite de crédito:',
          },
          actions: {
            searchCustomer: 'Procurar cliente',
            searching: 'A procurar...',
            proceed: 'Continuar',
            newSearch: 'Nova pesquisa',
            cancel: 'Cancelar',
          },
        },
        newCustomer: {
          title: 'É um novo cliente?',
          message: 'Pode registar-se agora e beneficiar das suas informações guardadas mais tarde.',
          registerButton: 'Novo registo',
        },
        diagnostic: {
          title: '🔧 Info de diagnóstico (Apenas Dev)',
        },
      },
      productDetail: {
        breadcrumb: {
          home: 'Início',
          products: 'Produtos',
        },
        loading: {
          message: 'Carregando detalhes do produto...',
        },
        error: {
          retry: 'Tentar novamente',
        },
        price: {
          overview: 'Visão geral de preços',
          vatNotice:
            'Todos os preços incluem IVA de acordo com a legislação de indicação de preços',
        },
        stock: {
          inStock: '✓ Em stock',
          outOfStock: '✗ Esgotado',
          available: '({count} disponível(is))',
        },
        actions: {
          addToCart: 'Adicionar ao carrinho',
        },
        share: {
          label: 'Partilhar:',
        },
        specifications: {
          title: 'Especificações',
        },
        reviews: {
          title: 'Avaliações de clientes',
          verified: '✓ Verificado',
          byAuthor: 'por {author}',
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
        validation: {
          required: 'Landcode en BTW-nummer zijn vereist',
        },
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
        acceptance: {
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
      invoice: {
        loading: 'Factuur laden...',
        error: 'Fout bij laden factuur',
        retry: 'Opnieuw proberen',
        noInvoice: 'Geen factuur om weer te geven',
        status: {
          invoice: 'Factuur',
          reverseCharge: '⚠️ Reverse Charge (0% BTW)',
          overdue: 'Achterstallig',
        },
        labels: {
          issued: 'Uitgegeven',
          due: 'Vervaldatum',
          payment: 'Betaling',
          paidOn: 'Betaald op',
          from: 'Van',
          billTo: 'Factureren aan',
          reverseCharge: '(Reverse Charge)',
        },
        table: {
          headers: {
            product: 'Product',
            qty: 'Aantal',
            unitPrice: 'Eenheidsprijs',
            subtotal: 'Subtotaal',
            tax: 'Belasting',
            total: 'Totaal',
          },
        },
        pricing: {
          subtotal: 'Subtotaal:',
          shipping: 'Verzending:',
          vat: 'BTW ({{rate}}%):',
          reverseCharge: 'Reverse Charge (0% BTW):',
          total: 'Totaal:',
        },
        actions: {
          downloadPdf: 'PDF downloaden',
          sendEmail: 'E-mail verzenden',
          modify: 'Wijzigen',
          print: 'Afdrukken',
        },
        compliance: {
          vatNotice: 'De bovenstaande BTW voldoet aan de toepasselijke wetgeving.',
          paymentInfo: 'Betalingsinformatie',
          method: 'Methode:',
        },
      },
      cart: {
        title: 'Winkelwagen',
        itemCount: '{{count}} item(s) in uw winkelwagen',
        empty: {
          title: 'Uw winkelwagen is leeg',
          message: 'Ontdek geweldige producten en begin met winkelen',
          button: 'Verder winkelen',
        },
        table: {
          headers: {
            product: 'Product',
            price: 'Prijs',
            quantity: 'Aantal',
            total: 'Totaal',
          },
        },
        actions: {
          continueShopping: 'Verder winkelen',
          remove: 'Verwijderen uit winkelwagen',
        },
        orderSummary: {
          title: 'Orderoverzicht',
          coupon: {
            label: 'Heeft u een couponcode?',
            placeholder: 'Voer couponcode in',
            apply: 'Toepassen',
          },
          pricing: {
            subtotal: 'Subtotaal',
            shipping: 'Verzending',
            free: 'GRATIS',
            netPrice: 'Netto prijs (excl. BTW)',
            vat: 'BTW ({{rate}}%)',
            total: 'Totaal (incl. BTW)',
          },
        },
        checkout: {
          button: 'Doorgaan naar afrekenen →',
          guest: 'Doorgaan als gast',
          secure: '🔒 Veilig betalen',
        },
        trustBadges: {
          moneyBack: '✓ 30 dagen geld-terug-garantie',
          returns: '✓ Gratis retourneren en ruilen',
          ssl: '✓ Veilig SSL versleutelde betaling',
        },
      },
      registration: {
        check: {
          title: 'Registratietype controleren',
          subtitle: 'Controleer of u al geregistreerd bent als bestaande klant',
          form: {
            email: {
              label: 'E-mailadres',
              placeholder: 'voorbeeld@bedrijf.nl',
            },
            businessType: {
              label: 'Bedrijfstype',
              placeholder: '-- Selecteer alstublieft --',
              b2c: 'B2C (Particulier / ZZP-er)',
              b2b: 'B2B (Bedrijf / BV / NV)',
            },
            firstName: {
              label: 'Voornaam',
              placeholder: 'Jan',
            },
            lastName: {
              label: 'Achternaam',
              placeholder: 'Jansen',
            },
            companyName: {
              label: 'Bedrijfsnaam',
              placeholder: 'Voorbeeld BV',
            },
            phone: {
              label: 'Telefoon',
              placeholder: '+31 20 123 4567',
            },
          },
          buttons: {
            check: 'Controleren',
            checking: 'Bezig met controleren...',
            newCheck: 'Nieuwe controle',
            continueWithData: 'Doorgaan met klantgegevens',
            continueRegistration: 'Registratie voortzetten',
            back: 'Terug',
          },
          alerts: {
            error: 'Fout',
          },
          results: {
            existingCustomer: {
              title: 'Welkom terug!',
              description:
                'U bent al geregistreerd in ons systeem. Uw gegevens worden automatisch ingevuld.',
            },
            newCustomer: {
              title: 'Nieuwe klant registratie',
              description: 'U wordt doorgestuurd naar het reguliere registratieproces.',
            },
            customerData: 'Uw klantgegevens:',
            customerNumber: 'Klantnummer:',
            name: 'Naam:',
            email: 'E-mail:',
            phone: 'Telefoon:',
            address: 'Adres:',
            matchScore: 'Overeenkomstscore:',
          },
          info: {
            title: 'Informatie',
            existingCustomer:
              'Bestaande klant: U bent al geregistreerd in ons systeem. Uw gegevens worden automatisch ingevuld.',
            newCustomer: 'Nieuwe klant: U wordt doorgestuurd naar het reguliere registratieproces.',
            checkDetails:
              'De controle wordt uitgevoerd op basis van e-mail, naam en optioneel telefoon/adres.',
          },
        },
        privateCustomerRegistration: {
          title: 'Maak uw account aan',
          subtitle: 'Word lid van B2Connect en begin vandaag met winkelen',
          form: {
            email: {
              label: 'E-mailadres',
              placeholder: 'uw@email.com',
              ariaLabel: 'E-mailadres',
            },
            password: {
              label: 'Wachtwoord',
              placeholder: '••••••••',
              ariaLabel: 'Wachtwoord',
            },
            confirmPassword: {
              label: 'Wachtwoord bevestigen',
              placeholder: '••••••••',
              ariaLabel: 'Wachtwoord bevestigen',
            },
            firstName: {
              label: 'Voornaam',
              placeholder: 'Jan',
              ariaLabel: 'Voornaam',
            },
            lastName: {
              label: 'Achternaam',
              placeholder: 'Jansen',
              ariaLabel: 'Achternaam',
            },
            phone: {
              label: 'Telefoonnummer',
              placeholder: '+31 123 456 789',
              ariaLabel: 'Telefoonnummer',
            },
            streetAddress: {
              label: 'Adres',
              placeholder: 'Hoofdstraat 123',
              ariaLabel: 'Adres',
            },
            city: {
              label: 'Stad',
              placeholder: 'Amsterdam',
              ariaLabel: 'Stad',
            },
            postalCode: {
              label: 'Postcode',
              placeholder: '1000 AA',
              ariaLabel: 'Postcode',
            },
            country: {
              label: 'Land',
              placeholder: 'Selecteer een land',
              ariaLabel: 'Land',
              options: {
                select: 'Selecteer een land',
                DE: 'Duitsland (DE)',
                AT: 'Oostenrijk (AT)',
                CH: 'Zwitserland (CH)',
                FR: 'Frankrijk (FR)',
                NL: 'Nederland (NL)',
                BE: 'België (BE)',
                LU: 'Luxemburg (LU)',
                PL: 'Polen (PL)',
                CZ: 'Tsjechië (CZ)',
              },
            },
            state: {
              label: 'Staat / Provincie',
              placeholder: 'Staat / Provincie',
              ariaLabel: 'Staat of provincie',
            },
            dateOfBirth: {
              label: 'Geboortedatum',
              ariaLabel: 'Geboortedatum',
            },
            ageConfirmation: {
              ariaLabel: 'Ik bevestig dat ik ten minste 18 jaar oud ben',
            },
            acceptTerms: {
              ariaLabel: 'Ik accepteer de algemene voorwaarden',
            },
            acceptPrivacy: {
              ariaLabel: 'Ik accepteer het privacybeleid',
            },
            acceptMarketing: {
              ariaLabel: 'Ik wil marketingcommunicatie ontvangen',
            },
          },
          actions: {
            createAccount: 'Account aanmaken',
            creating: 'Account aanmaken...',
          },
          links: {
            termsLink: 'Algemene voorwaarden',
            privacyLink: 'Privacybeleid',
            loginLink: 'Hier inloggen',
          },
          messages: {
            alreadyHaveAccount: 'Heeft u al een account?',
            ageConfirmation: 'Ik bevestig dat ik ten minste {{age}} jaar oud ben',
            acceptTerms: 'Ik accepteer de',
            acceptPrivacy: 'Ik accepteer het',
            acceptMarketing: 'Ik wil marketingcommunicatie ontvangen',
            withdrawalNotice: 'Herroepingsrecht',
            error: 'Fout',
            networkError: 'Netwerkfout. Probeer het opnieuw.',
          },
        },
      },
      dashboard: {
        title: 'Dashboard',
        welcome: 'Welkom, {{firstName}} {{lastName}}!',
        email: 'E-mail',
        tenantId: 'Tenant-ID',
        statistics: {
          title: 'Statistieken',
          description: 'Uw dashboardstatistieken verschijnen hier.',
        },
        recentActivity: {
          title: 'Recente activiteit',
          description: 'Recente activiteiten worden hier weergegeven.',
        },
        quickActions: {
          title: 'Snelle acties',
          manageTenants: 'Tenants beheren',
          accountSettings: 'Accountinstellingen',
        },
        alerts: {
          settingsComingSoon: 'Instellingen worden binnenkort geïmplementeerd',
        },
      },
      customerTypeSelection: {
        title: 'Hoe registreert u zich?',
        subtitle: 'Kies het accounttype dat het beste bij uw behoeften past',
        private: {
          ariaLabel: 'Registreren als particuliere klant',
          title: 'Particuliere klant',
          description: 'Individuele shopper',
          details: 'Voor persoonlijke aankopen en winkelen',
        },
        business: {
          ariaLabel: 'Registreren als zakelijke klant',
          title: 'Zakelijke klant',
          description: 'Bedrijf of organisatie',
          details: 'Voor zakelijke aankopen en B2B-operaties',
        },
        actions: {
          continue: 'Doorgaan',
        },
        login: {
          prompt: 'Heeft u al een account?',
          link: 'Meld u hier aan',
        },
      },
      login: {
        title: 'Inloggen bij B2Connect',
        e2eMode: {
          title: 'E2E-testmodus actief',
          description: 'Elke e-mail/wachtwoord combinatie werkt. Backend niet vereist.',
        },
        devHelp: {
          hint: '💡 Gebruik testreferenties: {{email}} / {{password}}',
          email: 'admin@example.com',
          password: 'password',
        },
        form: {
          email: {
            label: 'E-mail',
            placeholder: 'Voer uw e-mail in',
          },
          password: {
            label: 'Wachtwoord',
            placeholder: 'Voer uw wachtwoord in',
          },
        },
        actions: {
          loggingIn: 'Bezig met inloggen...',
          login: 'Inloggen',
        },
        signup: {
          prompt: 'Heeft u nog geen account?',
          link: 'Registreren',
        },
      },
      productListing: {
        title: 'B2Connect Winkel',
        subtitle: 'Vind de beste producten voor uw bedrijf',
        search: {
          label: 'Producten zoeken',
          placeholder: 'Zoeken op naam, SKU of beschrijving...',
        },
        sort: {
          label: 'Sorteren op',
          options: {
            name: 'Naam (A-Z)',
            priceAsc: 'Prijs (Oplopend)',
            priceDesc: 'Prijs (Aflopend)',
            rating: 'Beoordeling (Aflopend)',
          },
        },
        filters: {
          title: 'Filters',
        },
        category: {
          label: 'Categorie',
        },
        priceRange: {
          label: 'Prijsklasse',
          placeholder: '€0 - €5000 (binnenkort beschikbaar)',
        },
        inStockOnly: 'Alleen op voorraad',
        results: {
          foundFor: 'Gevonden voor:',
          loading: 'Producten laden...',
          noProducts: 'Geen producten gevonden',
          noProductsMessage: 'Probeer uw filters of zoekopdracht aan te passen',
          clearFilters: 'Filters wissen',
          retry: 'Opnieuw proberen',
        },
        pagination: {
          previous: '← Vorige',
          next: 'Volgende →',
        },
      },
      customerLookup: {
        header: {
          newRegistration: 'Nieuwe registratie',
          welcomeBack: 'Welkom terug',
          enterEmailPrompt: 'Voer uw e-mailadres in om te beginnen',
          customerInfoFound: 'Klantinformatie gevonden',
        },
        form: {
          email: {
            label: 'E-mailadres *',
            placeholder: 'naam@voorbeeld.com',
            ariaLabel: 'E-mailadres',
          },
          status: {
            searching: 'Zoeken...',
          },
          error: {
            title: 'Fout bij klant zoeken',
          },
          success: {
            title: 'Klant gevonden!',
            welcomeMessage: 'Welkom terug, {name}!',
          },
          customerDetails: {
            customerNumber: 'Klantnummer',
            customerType: 'Klanttype',
            privateCustomer: 'Particuliere klant',
            businessCustomer: 'Zakelijke klant',
          },
          businessDetails: {
            title: 'Bedrijfsinformatie',
            company: 'Bedrijf:',
            phone: 'Telefoon:',
            country: 'Land:',
            creditLimit: 'Kredietlimiet:',
          },
          actions: {
            searchCustomer: 'Klant zoeken',
            searching: 'Zoeken...',
            proceed: 'Doorgaan',
            newSearch: 'Nieuwe zoekopdracht',
            cancel: 'Annuleren',
          },
        },
        newCustomer: {
          title: 'Bent u een nieuwe klant?',
          message: 'U kunt zich nu registreren en later profiteren van uw opgeslagen informatie.',
          registerButton: 'Nieuwe registratie',
        },
        diagnostic: {
          title: '🔧 Diagnostische info (Alleen Dev)',
        },
      },
      productDetail: {
        breadcrumb: {
          home: 'Home',
          products: 'Producten',
        },
        loading: {
          message: 'Productdetails laden...',
        },
        error: {
          retry: 'Opnieuw proberen',
        },
        price: {
          overview: 'Prijs overzicht',
          vatNotice:
            'Alle prijzen zijn inclusief BTW in overeenstemming met de prijsaanduidingsverordening',
        },
        stock: {
          inStock: '✓ Op voorraad',
          outOfStock: '✗ Niet op voorraad',
          available: '({count} beschikbaar)',
        },
        actions: {
          addToCart: 'Toevoegen aan winkelwagen',
        },
        share: {
          label: 'Delen:',
        },
        specifications: {
          title: 'Specificaties',
        },
        reviews: {
          title: 'Klantbeoordelingen',
          verified: '✓ Geverifieerd',
          byAuthor: 'door {author}',
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
        validation: {
          required: 'Kod kraju i numer VAT są wymagane',
        },
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
        acceptance: {
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
      invoice: {
        loading: 'Ładowanie faktury...',
        error: 'Błąd podczas ładowania faktury',
        retry: 'Spróbuj ponownie',
        noInvoice: 'Brak faktury do wyświetlenia',
        status: {
          invoice: 'Faktura',
          reverseCharge: '⚠️ Reverse Charge (0% VAT)',
          overdue: 'Zaległa',
        },
        labels: {
          issued: 'Wystawiona',
          due: 'Termin',
          payment: 'Płatność',
          paidOn: 'Zapłacona dnia',
          from: 'Od',
          billTo: 'Faktura dla',
          reverseCharge: '(Reverse Charge)',
        },
        table: {
          headers: {
            product: 'Produkt',
            qty: 'Ilość',
            unitPrice: 'Cena jedn.',
            subtotal: 'Suma częściowa',
            tax: 'Podatek',
            total: 'Razem',
          },
        },
        pricing: {
          subtotal: 'Suma częściowa:',
          shipping: 'Wysyłka:',
          vat: 'VAT ({{rate}}%):',
          reverseCharge: 'Reverse Charge (0% VAT):',
          total: 'Razem:',
        },
        actions: {
          downloadPdf: 'Pobierz PDF',
          sendEmail: 'Wyślij e-mail',
          modify: 'Modyfikuj',
          print: 'Drukuj',
        },
        compliance: {
          vatNotice: 'Powyższy VAT jest zgodny z obowiązującymi przepisami.',
          paymentInfo: 'Informacje o płatności',
          method: 'Metoda:',
        },
      },
      cart: {
        title: 'Koszyk zakupów',
        itemCount: '{{count}} przedmiot(ów) w koszyku',
        empty: {
          title: 'Twój koszyk jest pusty',
          message: 'Odkryj niesamowite produkty i zacznij robić zakupy',
          button: 'Kontynuuj zakupy',
        },
        table: {
          headers: {
            product: 'Produkt',
            price: 'Cena',
            quantity: 'Ilość',
            total: 'Razem',
          },
        },
        actions: {
          continueShopping: 'Kontynuuj zakupy',
          remove: 'Usuń z koszyka',
        },
        orderSummary: {
          title: 'Podsumowanie zamówienia',
          coupon: {
            label: 'Masz kod kuponu?',
            placeholder: 'Wprowadź kod kuponu',
            apply: 'Zastosuj',
          },
          pricing: {
            subtotal: 'Suma częściowa',
            shipping: 'Wysyłka',
            free: 'ZA DARMO',
            netPrice: 'Cena netto (bez VAT)',
            vat: 'VAT ({{rate}}%)',
            total: 'Razem (z VAT)',
          },
        },
        checkout: {
          button: 'Przejdź do płatności →',
          guest: 'Kontynuuj jako gość',
          secure: '🔒 Bezpieczna płatność',
        },
        trustBadges: {
          moneyBack: '✓ 30-dniowa gwarancja zwrotu pieniędzy',
          returns: '✓ Darmowe zwroty i wymiany',
          ssl: '✓ Bezpieczna płatność SSL zaszyfrowana',
        },
      },
      registration: {
        check: {
          title: 'Sprawdź typ rejestracji',
          subtitle: 'Sprawdź, czy jesteś już zarejestrowany jako istniejący klient',
          form: {
            email: {
              label: 'Adres e-mail',
              placeholder: 'przyklad@firma.pl',
            },
            businessType: {
              label: 'Typ firmy',
              placeholder: '-- Proszę wybrać --',
              b2c: 'B2C (Osoba fizyczna / Jednoosobowa działalność gospodarcza)',
              b2b: 'B2B (Firma / Sp. z o.o. / S.A.)',
            },
            firstName: {
              label: 'Imię',
              placeholder: 'Jan',
            },
            lastName: {
              label: 'Nazwisko',
              placeholder: 'Kowalski',
            },
            companyName: {
              label: 'Nazwa firmy',
              placeholder: 'Przykład Sp. z o.o.',
            },
            phone: {
              label: 'Telefon',
              placeholder: '+48 22 123 45 67',
            },
          },
          buttons: {
            check: 'Sprawdź',
            checking: 'Sprawdzanie...',
            newCheck: 'Nowe sprawdzenie',
            continueWithData: 'Kontynuuj z danymi klienta',
            continueRegistration: 'Kontynuuj rejestrację',
            back: 'Wstecz',
          },
          alerts: {
            error: 'Błąd',
          },
          results: {
            existingCustomer: {
              title: 'Witaj ponownie!',
              description:
                'Jesteś już zarejestrowany w naszym systemie. Twoje dane zostaną automatycznie wypełnione.',
            },
            newCustomer: {
              title: 'Rejestracja nowego klienta',
              description: 'Zostaniesz przekierowany do regularnego procesu rejestracji.',
            },
            customerData: 'Twoje dane klienta:',
            customerNumber: 'Numer klienta:',
            name: 'Nazwa:',
            email: 'E-mail:',
            phone: 'Telefon:',
            address: 'Adres:',
            matchScore: 'Wynik dopasowania:',
          },
          info: {
            title: 'Informacje',
            existingCustomer:
              'Istniejący klient: Jesteś już zarejestrowany w naszym systemie. Twoje dane zostaną automatycznie wypełnione.',
            newCustomer:
              'Nowy klient: Zostaniesz przekierowany do regularnego procesu rejestracji.',
            checkDetails:
              'Sprawdzenie jest wykonywane na podstawie e-maila, nazwiska i opcjonalnie telefonu/adresu.',
          },
        },
        privateCustomerRegistration: {
          title: 'Utwórz swoje konto',
          subtitle: 'Dołącz do B2Connect i zacznij robić zakupy już dziś',
          form: {
            email: {
              label: 'Adres e-mail',
              placeholder: 'twoj@przyklad.com',
              ariaLabel: 'Adres e-mail',
            },
            password: {
              label: 'Hasło',
              placeholder: '••••••••',
              ariaLabel: 'Hasło',
            },
            confirmPassword: {
              label: 'Potwierdź hasło',
              placeholder: '••••••••',
              ariaLabel: 'Potwierdź hasło',
            },
            firstName: {
              label: 'Imię',
              placeholder: 'Jan',
              ariaLabel: 'Imię',
            },
            lastName: {
              label: 'Nazwisko',
              placeholder: 'Kowalski',
              ariaLabel: 'Nazwisko',
            },
            phone: {
              label: 'Numer telefonu',
              placeholder: '+48 123 456 789',
              ariaLabel: 'Numer telefonu',
            },
            streetAddress: {
              label: 'Adres',
              placeholder: 'Główna 123',
              ariaLabel: 'Adres',
            },
            city: {
              label: 'Miasto',
              placeholder: 'Warszawa',
              ariaLabel: 'Miasto',
            },
            postalCode: {
              label: 'Kod pocztowy',
              placeholder: '00-001',
              ariaLabel: 'Kod pocztowy',
            },
            country: {
              label: 'Kraj',
              placeholder: 'Wybierz kraj',
              ariaLabel: 'Kraj',
              options: {
                select: 'Wybierz kraj',
                DE: 'Niemcy (DE)',
                AT: 'Austria (AT)',
                CH: 'Szwajcaria (CH)',
                FR: 'Francja (FR)',
                NL: 'Holandia (NL)',
                BE: 'Belgia (BE)',
                LU: 'Luksemburg (LU)',
                PL: 'Polska (PL)',
                CZ: 'Czechy (CZ)',
              },
            },
            state: {
              label: 'Województwo / Prowincja',
              placeholder: 'Województwo / Prowincja',
              ariaLabel: 'Województwo lub prowincja',
            },
            dateOfBirth: {
              label: 'Data urodzenia',
              ariaLabel: 'Data urodzenia',
            },
            ageConfirmation: {
              ariaLabel: 'Potwierdzam, że mam co najmniej 18 lat',
            },
            acceptTerms: {
              ariaLabel: 'Akceptuję regulamin',
            },
            acceptPrivacy: {
              ariaLabel: 'Akceptuję politykę prywatności',
            },
            acceptMarketing: {
              ariaLabel: 'Chcę otrzymywać komunikaty marketingowe',
            },
          },
          actions: {
            createAccount: 'Utwórz konto',
            creating: 'Tworzenie konta...',
          },
          links: {
            termsLink: 'Regulamin',
            privacyLink: 'Polityka prywatności',
            loginLink: 'Zaloguj się tutaj',
          },
          messages: {
            alreadyHaveAccount: 'Masz już konto?',
            ageConfirmation: 'Potwierdzam, że mam co najmniej {{age}} lat',
            acceptTerms: 'Akceptuję',
            acceptPrivacy: 'Akceptuję',
            acceptMarketing: 'Chcę otrzymywać komunikaty marketingowe',
            withdrawalNotice: 'Prawo odstąpienia',
            error: 'Błąd',
            networkError: 'Błąd sieci. Spróbuj ponownie.',
          },
        },
      },
      dashboard: {
        title: 'Panel',
        welcome: 'Witaj, {{firstName}} {{lastName}}!',
        email: 'E-mail',
        tenantId: 'ID najemcy',
        statistics: {
          title: 'Statystyki',
          description: 'Twoje statystyki panelu pojawią się tutaj.',
        },
        recentActivity: {
          title: 'Ostatnia aktywność',
          description: 'Ostatnie aktywności będą wyświetlane tutaj.',
        },
        quickActions: {
          title: 'Szybkie działania',
          manageTenants: 'Zarządzaj najemcami',
          accountSettings: 'Ustawienia konta',
        },
        alerts: {
          settingsComingSoon: 'Ustawienia zostaną wkrótce zaimplementowane',
        },
      },
      customerTypeSelection: {
        title: 'Jak się rejestrujesz?',
        subtitle: 'Wybierz typ konta, który najlepiej odpowiada Twoim potrzebom',
        private: {
          ariaLabel: 'Zarejestruj się jako klient prywatny',
          title: 'Klient prywatny',
          description: 'Pojedynczy kupujący',
          details: 'Do zakupów osobistych i zakupów',
        },
        business: {
          ariaLabel: 'Zarejestruj się jako klient biznesowy',
          title: 'Klient biznesowy',
          description: 'Firma lub organizacja',
          details: 'Do zakupów biznesowych i operacji B2B',
        },
        actions: {
          continue: 'Kontynuuj',
        },
        login: {
          prompt: 'Masz już konto?',
          link: 'Zaloguj się tutaj',
        },
      },
      login: {
        title: 'Zaloguj się do B2Connect',
        e2eMode: {
          title: 'Tryb testowy E2E aktywny',
          description: 'Dowolny e-mail/hasło będzie działać. Backend nie jest wymagany.',
        },
        devHelp: {
          hint: '💡 Użyj danych testowych: {{email}} / {{password}}',
          email: 'admin@example.com',
          password: 'password',
        },
        form: {
          email: {
            label: 'E-mail',
            placeholder: 'Wprowadź swój e-mail',
          },
          password: {
            label: 'Hasło',
            placeholder: 'Wprowadź swoje hasło',
          },
        },
        actions: {
          loggingIn: 'Logowanie...',
          login: 'Zaloguj się',
        },
        signup: {
          prompt: 'Nie masz konta?',
          link: 'Zarejestruj się',
        },
      },
      productListing: {
        title: 'Sklep B2Connect',
        subtitle: 'Znajdź najlepsze produkty dla swojej firmy',
        search: {
          label: 'Szukaj produktów',
          placeholder: 'Szukaj według nazwy, SKU lub opisu...',
        },
        sort: {
          label: 'Sortuj według',
          options: {
            name: 'Nazwa (A-Z)',
            priceAsc: 'Cena (Rosnąco)',
            priceDesc: 'Cena (Malejąco)',
            rating: 'Ocena (Malejąco)',
          },
        },
        filters: {
          title: 'Filtry',
        },
        category: {
          label: 'Kategoria',
        },
        priceRange: {
          label: 'Zakres cenowy',
          placeholder: '€0 - €5000 (wkrótce dostępne)',
        },
        inStockOnly: 'Tylko dostępne',
        results: {
          foundFor: 'Znaleziono dla:',
          loading: 'Ładowanie produktów...',
          noProducts: 'Nie znaleziono produktów',
          noProductsMessage: 'Spróbuj dostosować filtry lub zapytanie wyszukiwania',
          clearFilters: 'Wyczyść filtry',
          retry: 'Spróbuj ponownie',
        },
        pagination: {
          previous: '← Poprzedni',
          next: 'Następny →',
        },
      },
      customerLookup: {
        header: {
          newRegistration: 'Nowa rejestracja',
          welcomeBack: 'Witaj ponownie',
          enterEmailPrompt: 'Wprowadź swój adres e-mail, aby rozpocząć',
          customerInfoFound: 'Znaleziono informacje o kliencie',
        },
        form: {
          email: {
            label: 'Adres e-mail *',
            placeholder: 'imie@przyklad.com',
            ariaLabel: 'Adres e-mail',
          },
          status: {
            searching: 'Wyszukiwanie...',
          },
          error: {
            title: 'Błąd wyszukiwania klienta',
          },
          success: {
            title: 'Znaleziono klienta!',
            welcomeMessage: 'Witaj ponownie, {name}!',
          },
          customerDetails: {
            customerNumber: 'Numer klienta',
            customerType: 'Typ klienta',
            privateCustomer: 'Klient prywatny',
            businessCustomer: 'Klient biznesowy',
          },
          businessDetails: {
            title: 'Informacje biznesowe',
            company: 'Firma:',
            phone: 'Telefon:',
            country: 'Kraj:',
            creditLimit: 'Limit kredytowy:',
          },
          actions: {
            searchCustomer: 'Wyszukaj klienta',
            searching: 'Wyszukiwanie...',
            proceed: 'Kontynuuj',
            newSearch: 'Nowe wyszukiwanie',
            cancel: 'Anuluj',
          },
        },
        newCustomer: {
          title: 'Czy jesteś nowym klientem?',
          message:
            'Możesz zarejestrować się teraz i skorzystać ze swoich zapisanych informacji później.',
          registerButton: 'Nowa rejestracja',
        },
        diagnostic: {
          title: '🔧 Informacje diagnostyczne (Tylko Dev)',
        },
      },
      productDetail: {
        breadcrumb: {
          home: 'Home',
          products: 'Products',
        },
        loading: {
          message: 'Loading product details...',
        },
        error: {
          retry: 'Retry',
        },
        price: {
          overview: 'Price Overview',
          vatNotice: 'All prices include VAT in accordance with PAngV (Price Indication Ordinance)',
        },
        stock: {
          inStock: '✓ In Stock',
          outOfStock: '✗ Out of Stock',
          available: '({count} available)',
        },
        actions: {
          addToCart: 'Add to Cart',
        },
        share: {
          label: 'Share:',
        },
        specifications: {
          title: 'Specifications',
        },
        reviews: {
          title: 'Customer Reviews',
          verified: '✓ Verified',
          byAuthor: 'by {author}',
        },
      },
    },
  },
}));
