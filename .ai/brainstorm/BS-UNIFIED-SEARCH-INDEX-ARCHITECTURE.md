---
docid: BS-UNIFIED-SEARCH
title: Unified Search Index Architecture - Pre-Localized Catalog with Semantic Search
status: Brainstorm
owner: @Architect, @Backend
created: 2026-01-11
related: [ADR-057, KB-152, KB-154]
---

# 🔍 Unified Search Index Architecture

## Vision Statement

> **Der gesamte Shop basiert auf einem einheitlichen, vorlokalisierten Suchindex in Elasticsearch. Alle Shopdaten – Produkte, Varianten, Kategorien, Marken, Attribute – werden bei der Aufbereitung lokalisiert und mit semantischen Embeddings angereichert. Dies ermöglicht Top-Performance, echte semantische Suche und vollständige Durchgängigkeit im gesamten Shop.**

---

## BMEcat-Kompatibilität

> **Diese Architektur orientiert sich am BMEcat 2005-Standard für maximale Kompatibilität mit ERP-Systemen und Katalogaustausch.**

| BMEcat-Element | Elasticsearch-Feld | Beschreibung |
|----------------|-------------------|--------------|
| **PRODUCT_REFERENCE** | `product_references` | Beziehungen zwischen Produkten/Varianten |
| **FEATURE_SYSTEM** | `feature_system` | Klassifikationssystem (ECLASS, ETIM) |
| **FEATURE_GROUP** | `feature_group_*` | Merkmalgruppen |
| **FEATURE/FNAME** | `fname` | Merkmalcode |
| **FEATURE/FVALUE** | `fvalue` | Merkmalwert |
| **FEATURE/FUNIT** | `funit` | Einheit |
| **FEATURE/FORDER** | `forder` | Sortierung |
| **ALLOWED_VALUES** | `allowed_values` | Vordefinierte Werte |
| **CATALOG_GROUP_SYSTEM** | `category_assignments` | Kategoriezuordnungen |

**BMEcat PRODUCT_REFERENCE Typen:**
- `accessories` → Zubehör
- `sparepart` → Ersatzteil  
- `mandatory` → Pflicht-Zusatzposition
- `select` → Optionale Auswahl
- `followup` → Nachfolger
- `consists_of` → Bestandteil/Set
- `diff_orderunit` → Alternative Bestelleinheit
- `similar` → Ähnliches Produkt
- `others` → Sonstige (Cross-Selling)

---

## 1. Architektur-Übersicht

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        ERP / Catalog Import                                  │
│  (BMEcat, CSV, API, Manual Entry)                                           │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                     CATALOG PREPARATION PIPELINE                             │
│  ┌────────────────┐  ┌────────────────┐  ┌────────────────┐                │
│  │  Normalization │→ │  Localization  │→ │   Embedding    │                │
│  │  & Validation  │  │  (de/en/fr/...)│  │   Generation   │                │
│  └────────────────┘  └────────────────┘  └────────────────┘                │
│                              │                    │                         │
│                              ▼                    ▼                         │
│  ┌────────────────────────────────────────────────────────────────────┐    │
│  │            PRE-LOCALIZED DOCUMENT MODEL                             │    │
│  │   { product_id, tenant_id, language,                                │    │
│  │     name, description, category_path[], brand,                      │    │
│  │     attributes{}, variants[], embedding_vector[768] }              │    │
│  └────────────────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                    ELASTICSEARCH CLUSTER                                     │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │  UNIFIED SEARCH INDEX (per Tenant + Language)                        │   │
│  │  Index: b2x_{tenant}_{language} (e.g., b2x_acme_de)                 │   │
│  │                                                                      │   │
│  │  Document Types (via type field):                                    │   │
│  │  • product    - Produkte mit Varianten (nested)                     │   │
│  │  • category   - Kategorien mit Hierarchie                           │   │
│  │  • brand      - Marken/Hersteller                                   │   │
│  │  • attribute  - Filterable Attribute (Farben, Größen, ...)          │   │
│  │  • content    - CMS-Inhalte (optional)                              │   │
│  │                                                                      │   │
│  │  Features:                                                           │   │
│  │  • BM25 Full-Text Search                                            │   │
│  │  • HNSW Vector Search (semantic embeddings)                         │   │
│  │  • Faceted Aggregations                                             │   │
│  │  • Category Graph Navigation (DAG, not Tree!)                        │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                        UNIFIED SEARCH API                                    │
│  GET /api/search?q=...&type=product,category,brand&lang=de                  │
│                                                                              │
│  Response:                                                                   │
│  {                                                                          │
│    "products": [...],                                                       │
│    "categories": [{ "id", "name", "paths": [...], "children": [...] }],    │
│    "variants": [...],   // Eigenständige Entitäten!                         │
│    "brands": [...],                                                         │
│    "facets": { "colors": [...], "sizes": [...], "price_ranges": [...] },   │
│    "semantic_score": 0.87                                                   │
│  }                                                                          │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                         SHOP FRONTEND                                        │
│  • Navigation Menu (categories from search)                                 │
│  • Product Listing (products from search)                                   │
│  • Faceted Filters (aggregations from search)                               │
│  • Search Suggestions (autocomplete from search)                            │
│  • Semantic Search ("Ich suche etwas für den Garten")                       │
│  • Multi-Path Navigation (Produkt in mehreren Kategorien)                   │
│  • Bundle/Set Support (Variante in mehreren Produkten)                      │
│  • Varianten-Beziehungen (Zubehör, Ersatzteile, Nachfolger, ...)            │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 2. Kernprinzipien

> **⚠️ Zentrale Erkenntnis: Der gesamte Katalog ist ein GRAPH, kein Baum!**
> - Produkte → Kategorien: **N:M** (Produkt in mehreren Kategorien)
> - Produkte → Varianten: **N:M** (Variante in mehreren Produkten/Sets)
> - Kategorien → Parent: **N:M** (DAG, virtuelle Kategorien)
> - Varianten → Varianten: **N:M mit Typ** (Zubehör, Ersatzteil, Nachfolger, ...)

### 2.1 Category Graph (DAG) - Nicht Tree!

> **⚠️ Kritische Design-Entscheidung: Kategorien sind ein Directed Acyclic Graph (DAG), kein Baum!**

**Warum Graph statt Baum?**

| Szenario | Tree (1 Parent) | Graph (N Parents) |
|----------|-----------------|-------------------|
| Produkt in Sortiment + Angebote | ❌ Nur einer möglich | ✅ Beide gleichzeitig |
| Saisonale Kategorien (Weihnachten) | ❌ Produkt verschieben | ✅ Zusätzliche Zuordnung |
| Markenshops | ❌ Duplizieren | ✅ Parallele Struktur |
| Cross-Selling Kategorien | ❌ Nicht möglich | ✅ "Passt zu..." Kategorien |

**Implikationen:**

1. **Produkte haben MEHRERE Kategorie-Zuordnungen** (nicht eine)
2. **Eine Zuordnung ist "primary"** (für Breadcrumb, Canonical URL)
3. **Zuordnungen haben Typen** (permanent, promotion, virtual, seasonal)
4. **Virtuelle Kategorien werden dynamisch berechnet** (Neuheiten, Bestseller)
5. **Facetten zählen über ALLE Zuordnungen** (Überlappung möglich)

---

### 2.2 Pre-Localization (Vorab-Lokalisierung)

**Problem mit Runtime-Lokalisierung:**
- Jede Anfrage erfordert Lookup in Übersetzungstabellen
- Performance-Overhead bei jedem Request
- Komplexe Join-Logik zwischen Produktdaten und Übersetzungen
- Caching-Komplexität

**Lösung: Pre-Localized Index**
```
Index per Language:
├── b2x_acme_de   → Alle Daten bereits auf Deutsch
├── b2x_acme_en   → Alle Daten bereits auf Englisch
├── b2x_acme_fr   → Alle Daten bereits auf Französisch
└── ...
```

**Vorteile:**
- ✅ **Zero Runtime Overhead**: Keine Übersetzungs-Lookups
- ✅ **Optimale Analyzer**: Language-spezifische Analyzer (german, english, french)
- ✅ **Einfaches Caching**: Index = fertige Daten
- ✅ **Konsistente Facetten**: Attributnamen bereits lokalisiert

### 2.2 Unified Document Model

Statt separater Indizes für Produkte, Kategorien, Marken → **Ein Index mit Typ-Feld**:

```json
{
  "mappings": {
    "properties": {
      "doc_type": { "type": "keyword" },  // product | category | brand | attribute
      "id": { "type": "keyword" },
      "tenant_id": { "type": "keyword" },
      "language": { "type": "keyword" },
      
      // Common fields
      "name": { "type": "text", "analyzer": "german" },
      "name_suggest": { "type": "search_as_you_type" },
      "slug": { "type": "keyword" },
      
      // Embedding for semantic search
      "embedding": {
        "type": "dense_vector",
        "dims": 768,
        "index": true,
        "similarity": "cosine"
      },
      
      // Product-specific
      "sku": { "type": "keyword" },
      "description": { "type": "text" },
      "price": { "type": "scaled_float", "scaling_factor": 100 },
      "product_type": { "type": "keyword" },  // "simple" | "configurable" | "bundle" | "kit" | "set"
      
      // ARTIKELSET-INFORMATIONEN
      // Gilt für product_type: "bundle" | "kit" | "set"
      "set_info": {
        "type": "object",
        "properties": {
          "is_set": { "type": "boolean" },            // Ist dies ein Artikelset?
          "set_type": { "type": "keyword" },          // "bundle" | "kit" | "set" | "package"
          "component_count": { "type": "integer" },   // Anzahl der Bestandteile
          "total_items_count": { "type": "integer" }, // Gesamtanzahl aller Items (inkl. Mengen)
          "is_fixed_set": { "type": "boolean" },      // Festes Set (nicht änderbar)
          "is_configurable_set": { "type": "boolean" },// Konfigurierbares Set (Auswahl möglich)
          "min_components": { "type": "integer" },    // Min. Bestandteile (bei konfigurierbaren Sets)
          "max_components": { "type": "integer" },    // Max. Bestandteile (bei konfigurierbaren Sets)
          // Preis-Logik
          "pricing_type": { "type": "keyword" },      // "fixed" | "calculated" | "discounted"
          "set_price": { "type": "scaled_float", "scaling_factor": 100 },     // Fixpreis für Set
          "sum_of_parts_price": { "type": "scaled_float", "scaling_factor": 100 }, // Summe Einzelpreise
          "set_discount_percent": { "type": "float" },// Rabatt gegenüber Einzelkauf
          "set_savings": { "type": "scaled_float", "scaling_factor": 100 },   // Ersparnis in EUR
          // Verfügbarkeit
          "all_components_available": { "type": "boolean" }, // Alle Teile verfügbar?
          "limiting_component_sku": { "type": "keyword" }    // SKU des limitierenden Teils
        }
      },
      // Set-Bestandteile (wenn is_set = true)
      "set_components": {
        "type": "nested",
        "properties": {
          "component_id": { "type": "keyword" },      // Variante/Produkt-ID
          "sku": { "type": "keyword" },               // SKU der Komponente
          "name": { "type": "text" },                 // Name der Komponente
          "quantity": { "type": "integer" },          // Menge im Set
          "is_required": { "type": "boolean" },       // Pflicht-Bestandteil?
          "is_selectable": { "type": "boolean" },     // Vom Kunden wählbar?
          "alternatives": { "type": "keyword" },      // Alternative SKUs (bei Auswahl)
          "unit_price": { "type": "scaled_float", "scaling_factor": 100 },  // Einzelpreis
          "component_value": { "type": "scaled_float", "scaling_factor": 100 }, // Wert im Set
          "sort_order": { "type": "integer" }        // Reihenfolge in Anzeige
        }
      },
      // Flache Felder für schnellen Zugriff
      "is_set": { "type": "boolean" },                // Ist Artikelset?
      "set_component_count": { "type": "integer" },   // Anzahl Bestandteile
      "set_component_skus": { "type": "keyword" },    // SKUs aller Bestandteile
      
      // Variant-specific (Varianten sind eigenständige Dokumente)
      
      // Schlüsselfelder / Identifikatoren
      "sku": { "type": "keyword" },                     // Stock Keeping Unit (intern)
      "ean": { "type": "keyword" },                     // EAN/GTIN (13-stellig)
      "gtin": { "type": "keyword" },                    // GTIN-14 (für Handelseinheiten)
      "match_code": { "type": "keyword" },              // MatchCode für Schnellsuche
      "unified_number": { "type": "keyword" },          // Vereinheitlichte Artikelnummer
      "manufacturer_sku": { "type": "keyword" },        // Hersteller-Artikelnummer
      
      // Werksangaben (Hersteller/Fabrik)
      "factory_number": { "type": "keyword" },          // Werksnummer / Fabriknummer
      "factory_name": { "type": "text" },               // Werksbezeichnung
      "factory_code": { "type": "keyword" },            // Werkscode (Kürzel)
      "production_plant": { "type": "keyword" },        // Produktionsstandort
      
      // Klassifikation
      "eclass": {
        "type": "object",
        "properties": {
          "version": { "type": "keyword" },             // z.B. "14.0"
          "code": { "type": "keyword" },                // z.B. "27-02-01-01"
          "code_path": { "type": "keyword" },           // Hierarchie als Array
          "name": { "type": "text" }                    // Klassenbezeichnung
        }
      },
      
      // Beschreibungen
      "short_description": { "type": "text" },          // Kurzbeschreibung (1-2 Sätze)
      "long_description": { "type": "text" },           // Ausführliche Beschreibung
      
      // Marke
      "brand_id": { "type": "keyword" },                // Marke auf Varianten-Ebene!
      "brand_name": { "type": "keyword" },              // Markenname (denormalisiert)
      
      // GPSR - EU-Produktsicherheitsverordnung (General Product Safety Regulation)
      // Pflichtangaben gemäß EU 2023/988 ab 13.12.2024
      "gpsr": {
        "type": "object",
        "properties": {
          // Hersteller (Pflicht)
          "manufacturer": {
            "type": "object",
            "properties": {
              "name": { "type": "text" },
              "address": { "type": "text" },
              "country": { "type": "keyword" },
              "email": { "type": "keyword" },
              "phone": { "type": "keyword" },
              "website": { "type": "keyword" }
            }
          },
          // EU-Bevollmächtigter (wenn Hersteller außerhalb EU)
          "eu_representative": {
            "type": "object",
            "properties": {
              "name": { "type": "text" },
              "address": { "type": "text" },
              "country": { "type": "keyword" },
              "email": { "type": "keyword" },
              "phone": { "type": "keyword" }
            }
          },
          // Verantwortliche Person in der EU (Importeur/Händler)
          "responsible_person": {
            "type": "object",
            "properties": {
              "name": { "type": "text" },
              "address": { "type": "text" },
              "country": { "type": "keyword" },
              "email": { "type": "keyword" },
              "phone": { "type": "keyword" },
              "role": { "type": "keyword" }          // "importer" | "distributor" | "fulfillment"
            }
          },
          // Produktidentifikation
          "product_identifiers": {
            "type": "object",
            "properties": {
              "model": { "type": "keyword" },
              "batch_number": { "type": "keyword" },
              "serial_number": { "type": "keyword" },
              "type_designation": { "type": "keyword" }
            }
          },
          // Warnhinweise & Sicherheit
          "safety_info": {
            "type": "object",
            "properties": {
              "warnings": { "type": "text" },          // Warnhinweise (lokalisiert)
              "hazard_pictograms": { "type": "keyword" }, // GHS-Piktogramme
              "age_restrictions": { "type": "keyword" }, // Altersbeschränkung
              "instructions_url": { "type": "keyword" }, // Link zu Anleitungen
              "safety_data_sheet_url": { "type": "keyword" } // Sicherheitsdatenblatt
            }
          },
          // Konformität
          "compliance": {
            "type": "object",
            "properties": {
              "ce_marking": { "type": "boolean" },
              "declarations": { "type": "keyword" },   // DoC-Referenzen
              "certifications": { "type": "keyword" }, // TÜV, GS, etc.
              "test_reports": { "type": "keyword" }    // Prüfberichte
            }
          },
          "country_of_origin": { "type": "keyword" },   // Ursprungsland
          "is_complete": { "type": "boolean" },         // Alle Pflichtfelder ausgefüllt?
          "last_verified": { "type": "date" }           // Letzte Prüfung der Daten
        }
      },
      
      // PREISLISTEN (Mehrere Listenpreise pro Variante)
      // Unterschiedliche Preise für Branchen, Kundengruppen, Regionen
      "prices": {
        "type": "nested",
        "properties": {
          "price_list_id": { "type": "keyword" },        // ID der Preisliste
          "price_list_name": { "type": "keyword" },      // "Handwerk", "Industrie", "Endkunde"
          "price_list_type": { "type": "keyword" },      // "default" | "industry" | "customer_group" | "region"
          "industry_code": { "type": "keyword" },        // Branchencode (z.B. WZ-2008)
          "customer_group_id": { "type": "keyword" },    // Kundengruppen-ID
          "region_code": { "type": "keyword" },          // Regionsode (z.B. "DE", "AT", "CH")
          "currency": { "type": "keyword" },             // Währung (EUR, CHF, etc.)
          "net_price": { "type": "scaled_float", "scaling_factor": 100 },
          "gross_price": { "type": "scaled_float", "scaling_factor": 100 },
          "tax_rate": { "type": "float" },               // MwSt-Satz (0.19, 0.07, etc.)
          // PREIS PRO VERPACKUNGSMENGE / MENGENEINHEIT
          "price_unit": { "type": "keyword" },           // Preiseinheit: "C62" (Stück), "MTR" (Meter), "KGM" (kg)
          "price_quantity": { "type": "integer" },       // Preis gilt für X Einheiten (z.B. 100 = Preis pro 100 St.)
          "price_quantity_unit": { "type": "keyword" },  // Einheit der Preismenge ("ST", "M", "KG")
          "base_price": { "type": "scaled_float", "scaling_factor": 100 }, // Grundpreis (pro Basiseinheit)
          "base_price_unit": { "type": "keyword" },      // Grundpreis-Einheit ("1 ST", "1 M", "1 KG")
          "content_quantity": { "type": "float" },       // Inhaltsmenge (z.B. 0.5 bei 500ml)
          "content_unit": { "type": "keyword" },         // Inhaltseinheit ("LTR", "KGM", "MTR")
          // Staffelpreise
          "min_quantity": { "type": "integer" },         // Ab welcher Menge gilt dieser Preis?
          "valid_from": { "type": "date" },
          "valid_until": { "type": "date" },
          "is_default": { "type": "boolean" },           // Standard-Preisliste
          "priority": { "type": "integer" }              // Priorität bei Überlappung
        }
      },
      
      // SONDERPREISE (Kundengruppen-spezifische Aktionspreise)
      // Rabatte, Aktionen, zeitlich begrenzte Angebote pro Kundengruppe
      "special_prices": {
        "type": "nested",
        "properties": {
          "special_price_id": { "type": "keyword" },     // Eindeutige ID
          "customer_group_id": { "type": "keyword" },    // Kundengruppe (null = alle)
          "customer_group_name": { "type": "keyword" },  // "VIP", "Großkunde", "Neukunde"
          "customer_ids": { "type": "keyword" },         // Spezifische Kunden-IDs (optional)
          "price_type": { "type": "keyword" },           // "fixed" | "discount_percent" | "discount_amount"
          // Preisangaben
          "net_price": { "type": "scaled_float", "scaling_factor": 100 },  // Fester Sonderpreis
          "gross_price": { "type": "scaled_float", "scaling_factor": 100 },
          "discount_percent": { "type": "float" },       // Rabatt in % (z.B. 15.0 = 15%)
          "discount_amount": { "type": "scaled_float", "scaling_factor": 100 }, // Rabatt in EUR
          // Gültigkeitsregeln
          "valid_from": { "type": "date" },              // Startdatum
          "valid_until": { "type": "date" },             // Enddatum
          "min_quantity": { "type": "integer" },         // Mindestmenge
          "max_quantity": { "type": "integer" },         // Maximalmenge (Kontingent)
          "remaining_quantity": { "type": "integer" },   // Verbleibende Menge
          // Kombinierbarkeit
          "is_combinable": { "type": "boolean" },        // Mit anderen Rabatten kombinierbar?
          "priority": { "type": "integer" },             // Bei Überlappung: höhere Prio gewinnt
          // Anzeige
          "label": { "type": "keyword" },                // "SALE", "VIP-Preis", "-20%"
          "badge_color": { "type": "keyword" },          // Badge-Farbe (#FF0000)
          "show_original_price": { "type": "boolean" },  // Streichpreis anzeigen?
          "show_savings": { "type": "boolean" },         // "Sie sparen X €" anzeigen?
          // Kampagne
          "campaign_id": { "type": "keyword" },          // Zugehörige Kampagne
          "campaign_name": { "type": "keyword" }         // "Black Friday 2026"
        }
      },
      // Aggregierte Sonderpreis-Felder
      "has_special_price": { "type": "boolean" },        // Hat aktive Sonderpreise?
      "best_special_discount": { "type": "float" },      // Höchster Rabatt in %
      "special_price_labels": { "type": "keyword" },     // ["SALE", "VIP"]
      
      // Aggregierte Preis-Felder (für Facetten/Sortierung)
      "min_price": { "type": "scaled_float", "scaling_factor": 100 },
      "max_price": { "type": "scaled_float", "scaling_factor": 100 },
      "default_price": { "type": "scaled_float", "scaling_factor": 100 },
      
      // VERKAUFSEINHEITEN (Alternative Verpackungs-/Bestelleinheiten)
      // Jede Verkaufseinheit kann eigene EAN/GTIN haben
      // BMEcat: ORDER_UNIT, CONTENT_UNIT, PRICE_QUANTITY
      "sales_units": {
        "type": "nested",
        "properties": {
          "unit_id": { "type": "keyword" },              // Eindeutige ID der Verkaufseinheit
          "unit_code": { "type": "keyword" },            // Einheitencode (C62=Stück, KGM=kg, MTR=m)
          "unit_name": { "type": "keyword" },            // "Stück", "10er Pack", "Karton", "Palette"
          "unit_description": { "type": "text" },        // Beschreibung der Einheit
          "quantity": { "type": "integer" },             // Anzahl Basiseinheiten (1, 10, 100, 1200)
          "ean": { "type": "keyword" },                  // Eigene EAN für diese VE
          "gtin": { "type": "keyword" },                 // GTIN-14 für Handelseinheit
          "sku_suffix": { "type": "keyword" },           // SKU-Suffix (z.B. "-10PK", "-KTN")
          // PREIS PRO VERPACKUNGSMENGE
          "price_per_unit": { "type": "scaled_float", "scaling_factor": 100 },   // Preis für diese VE
          "price_quantity": { "type": "integer" },       // Preis gilt für X Basiseinheiten (100 = pro 100 St.)
          "price_per_base_unit": { "type": "scaled_float", "scaling_factor": 100 }, // Umgerechneter Grundpreis
          "base_unit_code": { "type": "keyword" },       // Basiseinheit ("C62", "MTR", "KGM")
          "base_unit_name": { "type": "keyword" },       // "Stück", "Meter", "Kilogramm"
          // Preismodifikation
          "price_factor": { "type": "float" },           // Preisfaktor (0.95 = 5% Rabatt)
          "fixed_price": { "type": "scaled_float", "scaling_factor": 100 }, // Fester VE-Preis
          "savings_percent": { "type": "float" },        // Ersparnis in % ggb. Einzelkauf
          // NICHT IM INDEX (Logistik-Details aus Stammdaten bei Bestellung):
          // - weight_kg, length_mm, width_mm, height_mm, volume_dm3
          // → Werden nur für Versandkostenberechnung benötigt
          // Bestellregeln
          "min_order_quantity": { "type": "integer" },   // Mindestbestellmenge in dieser VE
          "order_increment": { "type": "integer" },      // Bestellschritt (z.B. nur 5er-Schritte)
          "is_default": { "type": "boolean" },           // Standard-Verkaufseinheit
          "is_orderable": { "type": "boolean" },         // Kann direkt bestellt werden?
          "sort_order": { "type": "integer" }
        }
      },
      // Flache Felder für Suche/Filter
      "available_units": { "type": "keyword" },          // ["C62", "PCK", "KTN"]
      "available_unit_names": { "type": "keyword" },     // ["Stück", "10er Pack", "Karton"]
      "unit_eans": { "type": "keyword" },                // Alle EANs aller Verkaufseinheiten
      "has_bulk_pricing": { "type": "boolean" },         // Hat Staffelpreise / Mengenrabatte?
      "min_base_price": { "type": "scaled_float", "scaling_factor": 100 }, // Günstigster Grundpreis
      
      // TAGS (flexibles Tagging für Produkte und Varianten)
      "tags": {
        "type": "nested",
        "properties": {
          "tag_id": { "type": "keyword" },
          "tag_name": { "type": "keyword" },            // "Neuheit", "Bestseller", "Auslaufend"
          "tag_group": { "type": "keyword" },           // "status", "promotion", "season", "custom"
          "tag_color": { "type": "keyword" },           // Hex-Farbe für Badge (#FF5722)
          "tag_icon": { "type": "keyword" },            // Icon-Name (mdi-star, mdi-fire)
          "priority": { "type": "integer" },            // Anzeigereihenfolge
          "valid_from": { "type": "date" },
          "valid_until": { "type": "date" },
          "is_visible": { "type": "boolean" }           // Im Frontend anzeigen?
        }
      },
      // Flache Tag-Liste für einfache Filterung
      "tag_ids": { "type": "keyword" },
      "tag_names": { "type": "keyword" },
      "tag_groups": { "type": "keyword" },
      
      // MEDIEN (Bilder, Dokumente, Videos mit Typ-Klassifizierung)
      "media": {
        "type": "nested",
        "properties": {
          "media_id": { "type": "keyword" },
          "media_type": { "type": "keyword" },       // "image" | "document" | "video"
          "media_subtype": { "type": "keyword" },    // Detaillierter Typ (siehe unten)
          "url": { "type": "keyword" },              // CDN-URL oder Pfad
          "thumbnail_url": { "type": "keyword" },    // Vorschaubild (für Videos/Dokumente)
          "title": { "type": "text" },               // Beschreibung/Alt-Text (lokalisiert)
          "alt_text": { "type": "text" },            // Barrierefreiheit
          "mime_type": { "type": "keyword" },        // image/jpeg, application/pdf, video/mp4
          "file_size": { "type": "long" },           // Bytes
          "width": { "type": "integer" },            // Pixel (für Bilder/Videos)
          "height": { "type": "integer" },
          "duration": { "type": "integer" },         // Sekunden (für Videos)
          "sort_order": { "type": "integer" },       // Anzeigereihenfolge
          "is_primary": { "type": "boolean" },       // Hauptbild/-dokument
          "is_public": { "type": "boolean" },        // Öffentlich sichtbar?
          "language": { "type": "keyword" },         // Sprache (für Dokumente)
          "valid_from": { "type": "date" },
          "valid_until": { "type": "date" }
        }
      },
      // Bild-Subtypen
      // - product_image:     Produktbild
      // - technical_drawing: Technische Zeichnung / Maßzeichnung
      // - packaging:         Verpackungsbild
      // - lifestyle:         Lifestyle / Anwendungsbild
      // - 360_view:          360°-Ansicht (Frame)
      // - thumbnail:         Thumbnail / Icon
      // - detail:            Detailansicht
      // - color_swatch:      Farbmuster
      // - installation:      Einbau- / Montagebilder
      // - size_chart:        Größentabelle
      
      // Dokument-Subtypen
      // - datasheet:         Technisches Datenblatt
      // - manual:            Bedienungsanleitung
      // - safety_sheet:      Sicherheitsdatenblatt (SDS/MSDS)
      // - certificate:       Zertifikat (CE, TÜV, etc.)
      // - cad_file:          CAD-Datei (STEP, DWG, etc.)
      // - brochure:          Produktbroschüre
      // - declaration:       Konformitätserklärung (DoC)
      // - warranty:          Garantiebedingungen
      // - spare_parts:       Ersatzteilliste
      // - calibration:       Kalibrierprotokoll
      
      // Video-Subtypen
      // - product_video:     Produktpräsentation
      // - installation:      Montage- / Einbauvideo
      // - tutorial:          How-To / Anleitung
      // - 360_video:         360°-Produktvideo
      // - testimonial:       Kundenmeinung / Review
      // - webinar:           Schulungsvideo
      
      // Aggregierte Medien-Zähler (für Facetten)
      "image_count": { "type": "integer" },
      "document_count": { "type": "integer" },
      "video_count": { "type": "integer" },
      "has_360_view": { "type": "boolean" },
      "has_video": { "type": "boolean" },
      "has_cad": { "type": "boolean" },
      "has_datasheet": { "type": "boolean" },
      
      // PRIMARY & SECONDARY IMAGE (Schnellzugriff für Listings)
      // Primary: Hauptbild für Produktlisten, Suchergebnisse, Warenkorb
      // Secondary: Hover-Bild, alternative Ansicht (z.B. Rückseite, Detail)
      "primary_image": {
        "type": "object",
        "properties": {
          "url": { "type": "keyword" },
          "thumbnail_url": { "type": "keyword" },
          "alt_text": { "type": "text" },
          "width": { "type": "integer" },
          "height": { "type": "integer" }
        }
      },
      "secondary_image": {
        "type": "object",
        "properties": {
          "url": { "type": "keyword" },
          "thumbnail_url": { "type": "keyword" },
          "alt_text": { "type": "text" },
          "width": { "type": "integer" },
          "height": { "type": "integer" }
        }
      },
      // Legacy-Feld für Abwärtskompatibilität
      "primary_image_url": { "type": "keyword" },    // Schnellzugriff auf Hauptbild
      
      // SUCHBEGRIFFE (Keywords für verbesserte Auffindbarkeit)
      "search_terms": { "type": "text", "analyzer": "german" },  // Freitext-Suchbegriffe
      "search_terms_exact": { "type": "keyword" },               // Exakte Matches (SKU, Codes)
      "synonyms": { "type": "text", "analyzer": "german" },      // Synonyme/alternative Bezeichnungen
      "common_misspellings": { "type": "text" },                 // Häufige Tippfehler
      
      // MULTI-CATEGORY SUPPORT (Graph, not Tree!)
      // Ein Produkt kann in mehreren Kategorien liegen
      "category_assignments": {
        "type": "nested",
        "properties": {
          "category_id": { "type": "keyword" },
          "category_path": { "type": "keyword" },      // ["Werkzeug", "Elektrowerkzeug", "Bohrmaschinen"]
          "category_path_ids": { "type": "keyword" },  // ["cat-1", "cat-5", "cat-23"]
          "is_primary": { "type": "boolean" },         // Hauptkategorie für Breadcrumb
          "assignment_type": { "type": "keyword" }     // "permanent" | "promotion" | "seasonal"
        }
      },
      
      // ⚠️ HINWEIS: brand_id/brand_name sind auf VARIANTEN-Ebene definiert (siehe oben)
      // Produkte aggregieren Marken aus ihren Varianten
      
      // PRODUCT_FEATURES (BMEcat-kompatibel, siehe Sektion 2.6)
      // ⚠️ WICHTIG: Nicht alle Attribute werden für die Suche verwendet!
      // - is_searchable: Attributwert wird in Volltext-Suche einbezogen
      // - is_filterable: Attribut erscheint in Facetten-Navigation
      // - is_comparable: Attribut im Produktvergleich anzeigen
      // - is_visible: Attribut in Produktdetails anzeigen
      "features": {
        "type": "nested",
        "properties": {
          // BMEcat FEATURE Mapping
          "fname": { "type": "keyword" },             // FNAME - Merkmalcode
          "fname_display": { "type": "keyword" },     // Lokalisierter Anzeigename
          "fvalue": { "type": "keyword" },            // FVALUE - Merkmalwert (Text)
          "fvalue_display": { "type": "keyword" },    // Lokalisierter Anzeigewert
          "fvalue_type": { "type": "keyword" },       // FVALUE_TYPE: text | number | boolean | range
          "fvalue_number": { "type": "double" },      // Für numerische Werte
          "fvalue_boolean": { "type": "boolean" },    // Für Ja/Nein
          "fvalue_min": { "type": "double" },         // Für Bereichswerte
          "fvalue_max": { "type": "double" },
          "fvalue_color_hex": { "type": "keyword" },  // B2X: Farbwerte (#FF0000)
          "funit": { "type": "keyword" },             // FUNIT - Einheit (mm, kg, ...)
          "forder": { "type": "integer" },            // FORDER - Sortierung
          "forder_value": { "type": "keyword" },      // Berechneter Sortierwert
          // Sichtbarkeits- und Such-Flags
          "is_searchable": { "type": "boolean" },     // In Volltext-Suche einbeziehen?
          "is_filterable": { "type": "boolean" },     // In Facetten anzeigen?
          "is_comparable": { "type": "boolean" },     // Im Produktvergleich zeigen?
          "is_visible": { "type": "boolean" },        // In Produktdetails zeigen?
          "search_boost": { "type": "float" }         // Such-Gewichtung (1.0 = normal)
        }
      },
      
      // AGGREGIERTE SUCHFELDER (nur suchbare Attribute!)
      // Diese Felder werden beim Indexieren aus features[] mit is_searchable=true befüllt
      // Vorteil: Einfache Volltextsuche ohne nested query
      "searchable_attributes": { "type": "text", "analyzer": "german" },  // Alle suchbaren Werte
      "filterable_attributes": {                      // Für Facetten-Aggregation
        "type": "nested",
        "properties": {
          "fname": { "type": "keyword" },
          "fvalue": { "type": "keyword" },
          "fvalue_number": { "type": "double" }
        }
      },
      
      // VARIANT REFERENCES (Graph, not nested ownership!)
      // Varianten sind eigenständige Entitäten - können in mehreren Produkten sein
      "variant_assignments": { 
        "type": "nested",
        "properties": {
          "variant_id": { "type": "keyword" },        // Referenz auf Varianten-Dokument
          "sku": { "type": "keyword" },
          "is_primary": { "type": "boolean" },        // Hauptprodukt dieser Variante
          "quantity": { "type": "integer" },          // Für Sets: Anzahl dieser Variante
          "price_override": { "type": "scaled_float" }, // Optionaler Preis in diesem Kontext
          "sort_order": { "type": "integer" }
        }
      },
      
      // Category-specific (Graph-aware)
      "parent_ids": { "type": "keyword" },       // MULTIPLE parents allowed!
      "level": { "type": "integer" },            // Minimum level in graph
      "paths": { "type": "keyword" },            // ALL paths: ["1/5/23", "99/23"]
      "category_type": { "type": "keyword" },    // "navigation" | "virtual" | "promotion"
      "children_count": { "type": "integer" },
      
      // LAGERBESTÄNDE PRO ABHOLLAGER (Multi-Warehouse)
      // Bestand ist nicht absolut, sondern pro Lager definiert
      "warehouses": {
        "type": "nested",
        "properties": {
          "warehouse_id": { "type": "keyword" },       // Lager-ID
          "warehouse_name": { "type": "keyword" },     // "Hauptlager Berlin", "Filiale München"
          "warehouse_code": { "type": "keyword" },     // Kurzcode "BER", "MUC"
          "warehouse_type": { "type": "keyword" },     // "central" | "branch" | "partner" | "dropship"
          // Artikeltyp pro Lager
          "item_type": { "type": "keyword" },          // "stock_item" | "order_item" | "dropship"
          "is_stock_item": { "type": "boolean" },      // Lagerartikel (wird bevorratet)
          "is_order_item": { "type": "boolean" },      // Bestellware (wird bei Bedarf bestellt)
          "supplier_lead_time_days": { "type": "integer" }, // Lieferzeit vom Lieferanten (bei Bestellware)
          // Lieferstatus pro Lager
          "availability_status": { "type": "keyword" }, // "available" | "end_of_life" | "discontinued"
          "is_available": { "type": "boolean" },       // Lieferbar aus diesem Lager?
          "is_end_of_life": { "type": "boolean" },     // Auslaufartikel (noch lieferbar, aber nicht mehr nachbestellt)
          "is_discontinued": { "type": "boolean" },    // Ausgelaufen (nicht mehr lieferbar)
          "discontinued_at": { "type": "date" },       // Ausgelaufen seit
          "end_of_life_at": { "type": "date" },        // Auslauf gestartet seit
          "successor_sku": { "type": "keyword" },      // Nachfolge-Artikel SKU
          // Bestand (nur suchrelevante Felder)
          "stock_quantity": { "type": "integer" },     // Bestand in diesem Lager
          "available_quantity": { "type": "integer" }, // Verfügbar (für Filterung)
          // NICHT IM INDEX (ERP-intern, kein Suchnutzen):
          // - reserved_quantity → bei Bestellung aus DB
          // - reorder_level, reorder_quantity → reine ERP-Logik
          // Status
          "status": { "type": "keyword" },             // "in_stock" | "low_stock" | "out_of_stock"
          "is_available": { "type": "boolean" },       // Bestellbar aus diesem Lager?
          // Lieferung
          "delivery_time_days": { "type": "integer" }, // Lieferzeit von diesem Lager
          "delivery_time_text": { "type": "keyword" }, // "1-2 Werktage"
          "cutoff_time": { "type": "keyword" },        // Bestellschluss "14:00" (statisch pro Lager)
          // HINWEIS: next_dispatch_at gehört NICHT in Index (zu dynamisch)
          // → Wird zur Laufzeit aus cutoff_time + Öffnungstagen berechnet
          // Abholung (Click & Collect)
          "pickup_available": { "type": "boolean" },   // Abholung möglich?
          // NICHT IM INDEX (zur Laufzeit berechnet/aus Stammdaten):
          // - pickup_time_hours, pickup_time_text → abhängig von Uhrzeit
          // Standort (für Umkreissuche)
          "location": { "type": "geo_point" },         // Lat/Lon für Umkreissuche
          // NICHT IM INDEX (Stammdaten aus Warehouse-Service):
          // - address (street, postal_code) → bei Bedarf nachladen
          "city": { "type": "keyword" },               // Stadt für Filter
          "country": { "type": "keyword" },            // Land für Filter
          // Priorität
          "priority": { "type": "integer" },           // Bevorzugtes Lager (1 = höchste Prio)
          "is_default": { "type": "boolean" }          // Standard-Lager für Versand
        }
      },
      // Aggregierte Lager-Felder (berechnet aus warehouses[])
      "warehouse_ids": { "type": "keyword" },          // Alle Lager-IDs
      "warehouse_codes": { "type": "keyword" },        // ["BER", "MUC", "HH"]
      "total_stock_quantity": { "type": "integer" },   // Summe aller Lagerbestände
      "available_warehouse_count": { "type": "integer" }, // Anzahl Lager mit Bestand
      "has_pickup_option": { "type": "boolean" },      // Mindestens 1 Lager mit Abholung?
      "min_delivery_time_days": { "type": "integer" }, // Schnellste Lieferzeit
      "has_stock_item": { "type": "boolean" },         // Mindestens 1 Lager mit Lagerartikel?
      "is_pure_order_item": { "type": "boolean" },     // NUR Bestellware (kein Lager hat Bestand)?
      // Lieferstatus (aggregiert)
      "global_availability_status": { "type": "keyword" }, // "available" | "end_of_life" | "discontinued"
      "has_available_warehouse": { "type": "boolean" },    // Mind. 1 Lager lieferbar?
      "is_globally_end_of_life": { "type": "boolean" },    // Alle Lager auf Auslauf?
      "is_globally_discontinued": { "type": "boolean" },   // Alle Lager ausgelaufen?
      "available_warehouse_ids": { "type": "keyword" },    // Lager-IDs mit Status "available"
      
      // VERFÜGBARKEIT (aggregiert über alle Lager)
      "availability": {
        "type": "object",
        "properties": {
          "status": { "type": "keyword" },           // "in_stock" | "low_stock" | "out_of_stock" | "preorder" | "discontinued"
          "stock_quantity": { "type": "integer" },   // Gesamtbestand (Summe aller Lager)
          "stock_display": { "type": "keyword" },    // "Auf Lager" | "Nur noch 3" | "Nicht verfügbar"
          "delivery_time_days": { "type": "integer" },// Schnellste Lieferzeit
          "delivery_time_text": { "type": "keyword" },// "1-3 Werktage" | "2-4 Wochen"
          "next_available_at": { "type": "date" },   // Für Vorbestellungen
          "backorder_allowed": { "type": "boolean" }, // Nachbestellung möglich?
          "max_order_quantity": { "type": "integer" } // Max. bestellbare Menge
        }
      },
      // Flache Felder für schnelle Filterung
      "is_in_stock": { "type": "boolean" },
      "is_orderable": { "type": "boolean" },
      "delivery_time_days": { "type": "integer" },
      
      // KUNDENBEWERTUNGEN (nur aggregierte Werte für Suche/Sortierung)
      "reviews": {
        "type": "object",
        "properties": {
          "average_rating": { "type": "float" },     // 4.7 (1-5 Sterne) - für Sortierung
          "review_count": { "type": "integer" }      // Anzahl - für Sortierung/Filter
          // NICHT IM INDEX (nur für Detailansicht):
          // - rating_distribution (stars_1..5) → aus Review-Service
          // - recommendation_rate → selten Filterkriterium
        }
      },
      // Flache Felder für Sortierung
      "review_rating": { "type": "float" },
      "review_count": { "type": "integer" },
      
      // RANKING & BUSINESS SIGNALE
      "ranking": {
        "type": "object",
        "properties": {
          "popularity_score": { "type": "float" },   // 0-100, berechnet aus Clicks/Orders
          "sales_rank": { "type": "integer" },       // Bestseller-Rang (1 = bester)
          "order_count_30d": { "type": "integer" },  // Bestellungen letzte 30 Tage
          "view_count_30d": { "type": "integer" },   // Ansichten letzte 30 Tage
          "conversion_rate": { "type": "float" },    // Views → Orders
          "margin_score": { "type": "float" },       // Interner Marge-Score (nicht öffentlich)
          "boost_factor": { "type": "float" }        // Manueller Boost (1.0 = normal)
        }
      },
      // Flache Felder für Sortierung
      "popularity_score": { "type": "float" },
      "sales_rank": { "type": "integer" },
      
      // BOOSTING-FELDER (Admin-konfigurierbar)
      "boosting": {
        "type": "object",
        "properties": {
          // Entity-spezifische Boosts (von Admin gesetzt)
          "variant_boost": { "type": "float" },      // Boost auf dieser Variante (1.0 = normal)
          "product_boost": { "type": "float" },      // Geerbt vom Produkt
          "category_boost": { "type": "float" },     // Geerbt von Kategorie(n)
          "brand_boost": { "type": "float" },        // Geerbt von Marke
          // Automatische Boosts (berechnet)
          "stock_boost": { "type": "float" },        // Lagerware > Bestellware
          "new_boost": { "type": "float" },          // Neuheiten-Boost
          "promo_boost": { "type": "float" },        // Aktionsartikel-Boost
          // Kombinierter Score
          "total_boost": { "type": "float" }         // Produkt aller Boosts
        }
      },
      // Flache Boost-Felder für schnellen Zugriff
      "total_boost": { "type": "float" },           // Kopie für Sortierung
      
      // PROMOTION-FLAGS für Boosting
      "is_promoted": { "type": "boolean" },         // Aktionsartikel?
      "is_featured": { "type": "boolean" },         // Hervorgehoben?
      "is_bestseller": { "type": "boolean" },       // Bestseller-Badge?
      "promo_priority": { "type": "integer" },      // Aktions-Priorität (1 = höchste)
      
      // PRODUKT-STATUS & LIFECYCLE
      "lifecycle": {
        "type": "object",
        "properties": {
          "status": { "type": "keyword" },           // "draft" | "active" | "discontinued" | "archived"
          "is_new": { "type": "boolean" },          // Neuheit-Badge
          "new_until": { "type": "date" },          // "Neu" bis Datum
          "discontinued_at": { "type": "date" },    // Auslaufdatum
          "end_of_life_at": { "type": "date" },     // Verkaufsende
          "successor_id": { "type": "keyword" },    // Nachfolge-Produkt
          "launch_date": { "type": "date" }         // Erscheinungsdatum
        }
      },
      "is_new": { "type": "boolean" },
      "is_discontinued": { "type": "boolean" },
      
      // SICHTBARKEIT & BERECHTIGUNGEN
      "visibility": {
        "type": "object",
        "properties": {
          "is_visible": { "type": "boolean" },       // Generell sichtbar?
          "is_searchable": { "type": "boolean" },    // In Suche findbar?
          "is_listed": { "type": "boolean" },        // In Kategorielisten?
          "visible_from": { "type": "date" },        // Sichtbar ab
          "visible_until": { "type": "date" },       // Sichtbar bis
          "customer_groups": { "type": "keyword" },  // Nur für bestimmte Gruppen
          "price_visible": { "type": "boolean" },    // Preis anzeigen?
          "price_visible_for": { "type": "keyword" } // Preis sichtbar für Gruppen
        }
      },
      
      // Zeitstempel (nur für Index-Management, nicht für häufige Updates)
      "created_at": { "type": "date" },             // Artikel-Erstellung (stabil)
      "indexed_at": { "type": "date" }              // Letzter Index-Zeitpunkt
      // HINWEIS: updated_at gehört NICHT in Index
      // → Führt zu ständigen Reindexierungen bei jeder Änderung
      // → Stattdessen: Änderungserkennung über Datenbank-Trigger/Events
    }
  }
}
```

### 2.3 Category Graph vs. Tree

**Problem: Kategorien sind ein Graph (DAG), kein Baum!**

```
                    ┌─────────────┐
                    │   ROOT      │
                    └─────────────┘
                          │
           ┌──────────────┼──────────────┐
           ▼              ▼              ▼
    ┌──────────┐   ┌──────────┐   ┌──────────┐
    │ Werkzeug │   │ Angebote │   │ Neuheiten│
    └──────────┘   └──────────┘   └──────────┘
           │              │              │
           ▼              │              │
    ┌──────────┐          │              │
    │ Elektro- │          │              │
    │ werkzeug │          │              │
    └──────────┘          │              │
           │              │              │
           ▼              ▼              ▼
    ┌─────────────────────────────────────────┐
    │         Bosch Bohrmaschine              │  ← Produkt in 3 "Kategorien"!
    │  • Werkzeug > Elektrowerkzeug (primary) │
    │  • Angebote (promotion)                 │
    │  • Neuheiten (virtual/temporal)         │
    └─────────────────────────────────────────┘
```

**Kategorie-Typen:**

| Typ | Beschreibung | Beispiele |
|-----|--------------|-----------|
| `navigation` | Permanente Sortimentskategorie | Werkzeug, Elektronik, Garten |
| `virtual` | Dynamische/berechnete Kategorie | Neuheiten, Bestseller, Zuletzt angesehen |
| `promotion` | Temporäre Aktionskategorie | Angebote, Sale, Black Friday |
| `brand` | Markenshop-Kategorie | Bosch, Makita, DeWalt |

### 2.4 Variant Graph - Varianten als eigenständige Entitäten

> **⚠️ Kritische Design-Entscheidung: Varianten können in MEHREREN Produkten enthalten sein!**

#### Shop-Settings: Varianten-Limits

Da Produkte sehr viele Varianten enthalten können (z.B. Schrauben-Sortimente mit 500+ Größen), werden Limits über Shop-Settings konfiguriert:

```json
{
  "catalog": {
    "variants": {
      "maxVariantsPerProduct": 1000,           // Standard-Limit
      "maxVariantsPerBundle": 100,             // Bundles haben meist weniger
      "maxVariantRelationsPerVariant": 50,     // Beziehungen zu anderen Varianten
      "warnThreshold": 500,                    // Admin-Warnung ab dieser Anzahl
      "paginationSize": 50                     // Varianten-Anzeige im Frontend
    },
    "categories": {
      "maxCategoryAssignmentsPerProduct": 20,  // Max. Kategorie-Zuordnungen
      "maxCategoryDepth": 10                   // Max. Tiefe im DAG
    },
    "pricing": {
      "maxPriceListsPerVariant": 50,           // Max. Preislisten pro Variante
      "defaultCurrency": "EUR",                 // Standard-Währung
      "supportedCurrencies": ["EUR", "CHF", "USD", "GBP"],
      "priceListTypes": [
        "default",          // Standard-Preisliste
        "industry",         // Branchenspezifisch
        "customer_group",   // Kundengruppen
        "region",           // Regional
        "promotion",        // Aktionspreise
        "contract"          // Vertragskunden
      ],
      "priceResolutionOrder": [     // Reihenfolge der Preisermittlung
        "contract",         // 1. Vertragspreis (höchste Priorität)
        "customer_group",   // 2. Kundengruppenpreis
        "industry",         // 3. Branchenpreis
        "region",           // 4. Regionaler Preis
        "promotion",        // 5. Aktionspreis
        "default"           // 6. Standardpreis (Fallback)
      ],
      "quantityBreaks": true,       // Mengenstaffeln aktiviert
      "showNetPrices": true,        // B2B: Nettopreise anzeigen
      "showGrossPrices": false      // Bruttopreise ausblenden
    }
  }
}
```

**Validierung bei Import/Anlage:**

```csharp
public class VariantLimitValidator
{
    private readonly IShopSettingsService _settings;
    
    public async Task<ValidationResult> ValidateAsync(Product product)
    {
        var limits = await _settings.GetCatalogLimitsAsync(product.TenantId);
        var errors = new List<string>();
        
        if (product.VariantAssignments.Count > limits.MaxVariantsPerProduct)
        {
            errors.Add($"Produkt überschreitet Varianten-Limit: " +
                $"{product.VariantAssignments.Count}/{limits.MaxVariantsPerProduct}");
        }
        
        if (product.VariantAssignments.Count > limits.WarnThreshold)
        {
            // Warnung loggen, aber erlauben
            _logger.LogWarning("Produkt {ProductId} hat {Count} Varianten (Schwellenwert: {Threshold})",
                product.Id, product.VariantAssignments.Count, limits.WarnThreshold);
        }
        
        return errors.Any() 
            ? ValidationResult.Failure(errors) 
            : ValidationResult.Success();
    }
}
```

**Use Cases für Varianten-Sharing:**

| Szenario | Beschreibung |
|----------|--------------|
| **Artikelsets/Bundles** | Set "Bohrer-Komplett" enthält dieselben Varianten wie Einzelprodukte |
| **Cross-Selling** | Variante "Akku 18V" ist in Bohrmaschine UND Stichsäge verwendbar |
| **Ersatzteile** | Variante "Filter XY" passt zu mehreren Staubsauger-Modellen |
| **Zubehör-Bundles** | "Starter-Kit" enthält Varianten aus verschiedenen Produkten |

**Graph-Struktur:**

```
┌──────────────────────────────────────────────────────────────────────────────┐
│                           VARIANT GRAPH                                       │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                                               │
│  ┌─────────────────┐         ┌─────────────────┐         ┌─────────────────┐ │
│  │ Produkt A       │         │ Produkt B       │         │ Produkt C       │ │
│  │ "Bohrer-Set"    │         │ "Bohrer 8mm"    │         │ "Profi-Set"     │ │
│  │ (type: bundle)  │         │ (type: simple)  │         │ (type: bundle)  │ │
│  └────────┬────────┘         └────────┬────────┘         └────────┬────────┘ │
│           │                           │                           │          │
│     ┌─────┼─────┐                     │                     ┌─────┼─────┐    │
│     ▼     ▼     ▼                     ▼                     ▼     ▼     ▼    │
│  ┌─────┐┌─────┐┌─────┐             ┌─────┐             ┌─────┐┌─────┐┌─────┐ │
│  │Var 1││Var 2││Var 3│             │Var 2│             │Var 2││Var 4││Var 5│ │
│  │5mm  ││8mm  ││10mm │             │8mm  │             │8mm  ││12mm ││16mm │ │
│  └─────┘└─────┘└─────┘             └─────┘             └─────┘└─────┘└─────┘ │
│                                        ▲                   ▲                 │
│                                        │                   │                 │
│                    VARIANTE "8mm" IST IN 3 PRODUKTEN! ─────┘                 │
│                                                                               │
└──────────────────────────────────────────────────────────────────────────────┘
```

**Implikationen:**

1. **Varianten sind eigene Dokumente** (nicht nested in Produkten)
2. **Bestandsführung auf Varianten-Ebene** (nicht pro Produkt)
3. **Preis kann kontextabhängig sein** (im Set günstiger als einzeln)
4. **Verfügbarkeit vererbt sich** (Set nur verfügbar wenn alle Varianten da)

**Varianten-Dokument im Index:**

```json
{
  "doc_type": "variant",
  "id": "var-8mm-bohrer",
  
  // Schlüsselfelder / Identifikatoren
  "sku": "BOHR-8MM-HSS",
  "ean": "4014364100523",
  "gtin": "04014364100523",
  "match_code": "BOHRER8HSS",
  "unified_number": "27-02-01-01-0815",
  "manufacturer_sku": "2608577123",
  
  // Werksangaben
  "factory_number": "WK-2608577123-DE",
  "factory_name": "Bosch Werk Leinfelden-Echterdingen",
  "factory_code": "BOSCH-LE",
  "production_plant": "DE-LE-01",
  
  // Klassifikation
  "eclass": {
    "version": "14.0",
    "code": "27-02-01-01",
    "code_path": ["27", "27-02", "27-02-01", "27-02-01-01"],
    "name": "Spiralbohrer"
  },
  
  // Bezeichnungen
  "name": "HSS Bohrer 8mm",
  "short_description": "Präzisions-Spiralbohrer für Metall, 8mm Durchmesser",
  "long_description": "Hochwertiger HSS-Spiralbohrer mit 118° Spitzenwinkel. Geeignet für alle gängigen Metalle wie Stahl, Aluminium und Gusseisen. Präzisionsgeschliffen für saubere Bohrlöcher.",
  
  // Marke (auf Varianten-Ebene definiert!)
  "brand_id": "brand-bosch",
  "brand_name": "Bosch Professional",
  
  // GPSR - EU-Produktsicherheitsverordnung
  "gpsr": {
    "manufacturer": {
      "name": "Robert Bosch Power Tools GmbH",
      "address": "Max-Lang-Straße 40-46, 70771 Leinfelden-Echterdingen",
      "country": "DE",
      "email": "pt-compliance@bosch.com",
      "phone": "+49 711 758-0",
      "website": "https://www.bosch-professional.com"
    },
    "eu_representative": null,  // Nicht erforderlich (Hersteller in EU)
    "responsible_person": {
      "name": "Bosch Power Tools",
      "address": "Max-Lang-Straße 40-46, 70771 Leinfelden-Echterdingen, Germany",
      "country": "DE",
      "email": "pt-compliance@bosch.com",
      "phone": "+49 711 758-0",
      "role": "manufacturer"
    },
    "product_identifiers": {
      "model": "HSS-G",
      "batch_number": "2026-01-A",
      "serial_number": null,
      "type_designation": "2608577123"
    },
    "safety_info": {
      "warnings": "Schutzbrille tragen. Nicht für Holz oder Kunststoff geeignet.",
      "hazard_pictograms": [],
      "age_restrictions": null,
      "instructions_url": "https://www.bosch-professional.com/manuals/2608577123",
      "safety_data_sheet_url": null
    },
    "compliance": {
      "ce_marking": false,  // Nicht CE-pflichtig (einfaches Werkzeug)
      "declarations": [],
      "certifications": ["ISO 9001"],
      "test_reports": []
    },
    "country_of_origin": "DE",
    "is_complete": true,
    "last_verified": "2026-01-10"
  },
  
  // FEATURES/ATTRIBUTE (mit Such- und Filter-Flags)
  // ⚠️ Nicht alle Attribute werden für die Suche verwendet!
  "features": [
    {
      "fname": "diameter",
      "fname_display": "Durchmesser",
      "fvalue": "8",
      "fvalue_display": "8 mm",
      "fvalue_type": "number",
      "fvalue_number": 8.0,
      "funit": "mm",
      "forder": 1,
      "is_searchable": true,      // ✅ In Suche: "8mm Bohrer"
      "is_filterable": true,      // ✅ In Facetten
      "is_comparable": true,      // ✅ Im Vergleich
      "is_visible": true,
      "search_boost": 1.5         // Höhere Gewichtung
    },
    {
      "fname": "material",
      "fname_display": "Material",
      "fvalue": "HSS",
      "fvalue_display": "HSS (Hochleistungsschnellstahl)",
      "fvalue_type": "text",
      "forder": 2,
      "is_searchable": true,      // ✅ In Suche: "HSS Bohrer"
      "is_filterable": true,      // ✅ In Facetten
      "is_comparable": true,
      "is_visible": true,
      "search_boost": 1.2
    },
    {
      "fname": "length",
      "fname_display": "Gesamtlänge",
      "fvalue": "120",
      "fvalue_display": "120 mm",
      "fvalue_type": "number",
      "fvalue_number": 120.0,
      "funit": "mm",
      "forder": 3,
      "is_searchable": false,     // ❌ Nicht in Suche (irrelevant für Suchbegriff)
      "is_filterable": true,      // ✅ Aber filterbar
      "is_comparable": true,
      "is_visible": true,
      "search_boost": 1.0
    },
    {
      "fname": "weight",
      "fname_display": "Gewicht",
      "fvalue": "15",
      "fvalue_display": "15 g",
      "fvalue_type": "number",
      "fvalue_number": 15.0,
      "funit": "g",
      "forder": 10,
      "is_searchable": false,     // ❌ Nicht in Suche
      "is_filterable": false,     // ❌ Nicht in Facetten (zu viele Werte)
      "is_comparable": true,      // ✅ Aber im Vergleich
      "is_visible": true,
      "search_boost": 1.0
    },
    {
      "fname": "internal_code",
      "fname_display": "Interner Code",
      "fvalue": "B2X-INT-12345",
      "fvalue_display": "B2X-INT-12345",
      "fvalue_type": "text",
      "forder": 99,
      "is_searchable": false,     // ❌ Nicht in Suche
      "is_filterable": false,     // ❌ Nicht in Facetten
      "is_comparable": false,     // ❌ Nicht im Vergleich
      "is_visible": false,        // ❌ Nicht sichtbar (intern)
      "search_boost": 0
    }
  ],
  
  // AGGREGIERTE SUCHFELDER (automatisch beim Indexieren befüllt)
  // Nur Werte aus features mit is_searchable=true
  "searchable_attributes": "8mm HSS Durchmesser Material Hochleistungsschnellstahl",
  "filterable_attributes": [
    { "fname": "diameter", "fvalue": "8", "fvalue_number": 8.0 },
    { "fname": "material", "fvalue": "HSS" },
    { "fname": "length", "fvalue": "120", "fvalue_number": 120.0 }
  ],
  
  // PREISLISTEN - Mehrere Preise für verschiedene Branchen/Kundengruppen
  // Mit Preis pro Verpackungsmenge / Grundpreis-Angabe
  "prices": [
    {
      "price_list_id": "pl-default",
      "price_list_name": "Standard",
      "price_list_type": "default",
      "currency": "EUR",
      "net_price": 8.49,
      "gross_price": 10.10,
      "tax_rate": 0.19,
      // PREIS PRO VERPACKUNGSMENGE (z.B. für Grundpreis-Anzeige)
      "price_unit": "C62",            // Preiseinheit: Stück
      "price_quantity": 1,            // Preis gilt für 1 Stück
      "price_quantity_unit": "ST",    // Einheit: Stück
      "base_price": 8.49,             // Grundpreis = Nettopreis / price_quantity
      "base_price_unit": "1 ST",      // Grundpreis pro 1 Stück
      "min_quantity": 1,
      "is_default": true,
      "priority": 0
    },
    {
      "price_list_id": "pl-handwerk",
      "price_list_name": "Handwerk",
      "price_list_type": "industry",
      "industry_code": "43",  // WZ-2008: Bauinstallation
      "currency": "EUR",
      "net_price": 7.49,
      "gross_price": 8.91,
      "tax_rate": 0.19,
      // PREIS PRO VERPACKUNGSMENGE
      "price_unit": "C62",
      "price_quantity": 1,
      "price_quantity_unit": "ST",
      "base_price": 7.49,
      "base_price_unit": "1 ST",
      "min_quantity": 1,
      "is_default": false,
      "priority": 10
    },
    {
      "price_list_id": "pl-industrie",
      "price_list_name": "Industrie",
      "price_list_type": "industry",
      "industry_code": "25",  // WZ-2008: Metallerzeugnisse
      "currency": "EUR",
      "net_price": 6.99,
      "gross_price": 8.32,
      "tax_rate": 0.19,
      // PREIS PRO VERPACKUNGSMENGE
      "price_unit": "C62",
      "price_quantity": 1,
      "price_quantity_unit": "ST",
      "base_price": 6.99,
      "base_price_unit": "1 ST",
      "min_quantity": 1,
      "is_default": false,
      "priority": 10
    },
    {
      "price_list_id": "pl-grosshandel-100",
      "price_list_name": "Großhandel (ab 100 Stück)",
      "price_list_type": "customer_group",
      "customer_group_id": "cg-wholesale",
      "currency": "EUR",
      // STAFFELPREIS PRO 100 STÜCK
      "net_price": 637.00,            // Preis für 100 Stück
      "gross_price": 758.03,
      "tax_rate": 0.19,
      "price_unit": "C62",
      "price_quantity": 100,          // Preis gilt für 100 Stück!
      "price_quantity_unit": "ST",
      "base_price": 6.37,             // Grundpreis pro Stück = 637 / 100
      "base_price_unit": "1 ST",
      "min_quantity": 100,            // Erst ab 100 Stück
      "is_default": false,
      "priority": 20
    },
    {
      "price_list_id": "pl-schweiz",
      "price_list_name": "Schweiz",
      "price_list_type": "region",
      "region_code": "CH",
      "currency": "CHF",
      "net_price": 9.20,
      "gross_price": 9.91,
      "tax_rate": 0.077,  // CH MwSt
      // PREIS PRO VERPACKUNGSMENGE
      "price_unit": "C62",
      "price_quantity": 1,
      "price_quantity_unit": "ST",
      "base_price": 9.20,
      "base_price_unit": "1 ST",
      "min_quantity": 1,
      "is_default": false,
      "priority": 5
    }
  ],
  
  // Aggregierte Preise
  "min_price": 5.52,              // Günstigster Grundpreis (Palette)
  "max_price": 10.10,             // Höchster Bruttopreis
  "default_price": 10.10,         // Standard-Bruttopreis
  
  // SONDERPREISE - Kundengruppen-spezifische Aktionspreise
  "special_prices": [
    {
      "special_price_id": "sp-vip-winter",
      "customer_group_id": "cg-vip",
      "customer_group_name": "VIP-Kunden",
      "customer_ids": null,
      "price_type": "discount_percent",
      "discount_percent": 20.0,     // 20% Rabatt
      "valid_from": "2026-01-01",
      "valid_until": "2026-01-31",
      "min_quantity": 1,
      "max_quantity": null,
      "is_combinable": false,
      "priority": 100,
      "label": "VIP -20%",
      "badge_color": "#9C27B0",
      "show_original_price": true,
      "show_savings": true,
      "campaign_id": "camp-winter-2026",
      "campaign_name": "Winter-Sale 2026"
    },
    {
      "special_price_id": "sp-neukunde",
      "customer_group_id": "cg-new-customer",
      "customer_group_name": "Neukunden",
      "customer_ids": null,
      "price_type": "fixed",
      "net_price": 3.49,
      "gross_price": 4.15,
      "valid_from": "2026-01-01",
      "valid_until": "2026-12-31",
      "min_quantity": 1,
      "max_quantity": 10,           // Max. 10 Stück zum Sonderpreis
      "is_combinable": false,
      "priority": 50,
      "label": "Neukundenpreis",
      "badge_color": "#4CAF50",
      "show_original_price": true,
      "show_savings": true,
      "campaign_id": null,
      "campaign_name": null
    },
    {
      "special_price_id": "sp-grossabnehmer",
      "customer_group_id": "cg-wholesale",
      "customer_group_name": "Großkunden",
      "customer_ids": null,
      "price_type": "discount_amount",
      "discount_amount": 0.50,      // 0,50 € Rabatt pro Stück
      "valid_from": null,           // Unbegrenzt gültig
      "valid_until": null,
      "min_quantity": 500,          // Erst ab 500 Stück
      "max_quantity": null,
      "is_combinable": true,        // Mit Mengenrabatt kombinierbar
      "priority": 80,
      "label": "Großkundenrabatt",
      "badge_color": "#2196F3",
      "show_original_price": false,
      "show_savings": false,
      "campaign_id": null,
      "campaign_name": null
    }
  ],
  "has_special_price": true,
  "best_special_discount": 20.0,
  "special_price_labels": ["VIP -20%", "Neukundenpreis", "Großkundenrabatt"],
  
  // VERKAUFSEINHEITEN - Alternative Bestelleinheiten mit eigener EAN
  "sales_units": [
    {
      "unit_id": "su-stueck",
      "unit_code": "C62",           // UN/ECE: Stück
      "unit_name": "Stück",
      "unit_description": "Einzelner Bohrer",
      "quantity": 1,
      "ean": "4014364100523",       // Basis-EAN
      "gtin": "04014364100523",
      "sku_suffix": "",
      // PREIS PRO VERPACKUNGSMENGE
      "price_per_unit": 8.49,       // Preis für 1 Stück
      "price_quantity": 1,          // Preis gilt für 1 Einheit
      "price_per_base_unit": 8.49,  // = price_per_unit / price_quantity
      "base_unit_code": "C62",
      "base_unit_name": "Stück",
      // Preismodifikation
      "price_factor": 1.0,          // Basispreis
      "savings_percent": 0,
      "weight_kg": 0.015,
      "min_order_quantity": 1,
      "order_increment": 1,
      "is_default": true,
      "is_orderable": true,
      "sort_order": 1
    },
    {
      "unit_id": "su-10er-pack",
      "unit_code": "PCK",           // Packung
      "unit_name": "10er Pack",
      "unit_description": "10 Bohrer in Kunststoffbox",
      "quantity": 10,
      "ean": "4014364100530",       // Eigene EAN für 10er Pack!
      "gtin": "14014364100530",     // GTIN-14 mit Verpackungsindikator
      "sku_suffix": "-10PK",
      // PREIS PRO VERPACKUNGSMENGE (10er Pack)
      "price_per_unit": 76.41,      // Preis für 10er Pack (8.49 * 10 * 0.90)
      "price_quantity": 10,         // Preis gilt für 10 Stück
      "price_per_base_unit": 7.64,  // Grundpreis pro Stück = 76.41 / 10
      "base_unit_code": "C62",
      "base_unit_name": "Stück",
      // Preismodifikation
      "price_factor": 0.90,         // 10% Rabatt auf Stückpreis
      "savings_percent": 10,        // 10% Ersparnis
      // Logistik-Details (weight, length, width, height) → Stammdaten
      "min_order_quantity": 1,
      "order_increment": 1,
      "is_default": false,
      "is_orderable": true,
      "sort_order": 2
    },
    {
      "unit_id": "su-karton",
      "unit_code": "KTN",           // Karton
      "unit_name": "Karton (100 Stück)",
      "unit_description": "Karton mit 10x 10er Packs",
      "quantity": 100,
      "ean": "4014364100547",       // Eigene EAN für Karton!
      "gtin": "24014364100547",     // GTIN-14 mit Karton-Indikator
      "sku_suffix": "-KTN",
      // PREIS PRO VERPACKUNGSMENGE (Karton 100 Stück)
      "price_per_unit": 636.75,     // Preis für Karton (8.49 * 100 * 0.75)
      "price_quantity": 100,        // Preis gilt für 100 Stück
      "price_per_base_unit": 6.37,  // Grundpreis pro Stück = 636.75 / 100
      "base_unit_code": "C62",
      "base_unit_name": "Stück",
      // Preismodifikation
      "price_factor": 0.75,         // 25% Rabatt auf Stückpreis
      "savings_percent": 25,        // 25% Ersparnis
      // Logistik-Details (weight, length, width, height, volume) → Stammdaten
      "min_order_quantity": 1,
      "order_increment": 1,
      "is_default": false,
      "is_orderable": true,
      "sort_order": 3
    },
    {
      "unit_id": "su-palette",
      "unit_code": "PF",            // Palette
      "unit_name": "Palette (1.200 Stück)",
      "unit_description": "Euro-Palette mit 12 Kartons",
      "quantity": 1200,
      "ean": "4014364100554",       // Eigene EAN für Palette!
      "gtin": "34014364100554",     // GTIN-14 mit Paletten-Indikator
      "sku_suffix": "-PAL",
      // PREIS PRO VERPACKUNGSMENGE (Palette 1.200 Stück)
      "price_per_unit": 6622.20,    // Preis für Palette (8.49 * 1200 * 0.65)
      "price_quantity": 1200,       // Preis gilt für 1.200 Stück
      "price_per_base_unit": 5.52,  // Grundpreis pro Stück = 6622.20 / 1200
      "base_unit_code": "C62",
      "base_unit_name": "Stück",
      // Preismodifikation
      "price_factor": 0.65,         // 35% Rabatt auf Stückpreis
      "savings_percent": 35,        // 35% Ersparnis
      // Logistik-Details (weight, length, width, height, volume) → Stammdaten
      "min_order_quantity": 1,
      "order_increment": 1,
      "is_default": false,
      "is_orderable": true,
      "sort_order": 4
    }
  ],
  "available_units": ["C62", "PCK", "KTN", "PF"],
  "available_unit_names": ["Stück", "10er Pack", "Karton (100 Stück)", "Palette (1.200 Stück)"],
  "unit_eans": ["4014364100523", "4014364100530", "4014364100547", "4014364100554"],
  "has_bulk_pricing": true,          // Hat Staffelpreise / Mengenrabatte
  "min_base_price": 5.52,            // Günstigster Grundpreis (Palette)
  
  // Tags
  "tags": [
    {
      "tag_id": "tag-bestseller",
      "tag_name": "Bestseller",
      "tag_group": "status",
      "tag_color": "#FF9800",
      "tag_icon": "mdi-fire",
      "priority": 1,
      "is_visible": true
    },
    {
      "tag_id": "tag-made-in-germany",
      "tag_name": "Made in Germany",
      "tag_group": "quality",
      "tag_color": "#1976D2",
      "tag_icon": "mdi-check-decagram",
      "priority": 2,
      "is_visible": true
    }
  ],
  "tag_ids": ["tag-bestseller", "tag-made-in-germany"],
  "tag_names": ["Bestseller", "Made in Germany"],
  "tag_groups": ["status", "quality"],
  
  // MEDIEN
  "media": [
    {
      "media_id": "img-001",
      "media_type": "image",
      "media_subtype": "product_image",
      "url": "https://cdn.b2x.com/products/bosch/hss-g-8mm-main.jpg",
      "thumbnail_url": "https://cdn.b2x.com/products/bosch/hss-g-8mm-thumb.jpg",
      "title": "HSS-G Spiralbohrer 8mm - Hauptansicht",
      "alt_text": "Bosch Professional HSS-G Spiralbohrer 8mm für Metall",
      "mime_type": "image/jpeg",
      "file_size": 245000,
      "width": 1200,
      "height": 1200,
      "sort_order": 1,
      "is_primary": true,
      "is_public": true
    },
    {
      "media_id": "img-002",
      "media_type": "image",
      "media_subtype": "technical_drawing",
      "url": "https://cdn.b2x.com/products/bosch/hss-g-8mm-drawing.png",
      "title": "Technische Zeichnung mit Maßen",
      "alt_text": "Maßzeichnung HSS-G 8mm: Länge 117mm, Arbeitslänge 75mm",
      "mime_type": "image/png",
      "file_size": 89000,
      "width": 800,
      "height": 400,
      "sort_order": 2,
      "is_primary": false,
      "is_public": true
    },
    {
      "media_id": "doc-001",
      "media_type": "document",
      "media_subtype": "datasheet",
      "url": "https://cdn.b2x.com/docs/bosch/hss-g-datasheet-de.pdf",
      "thumbnail_url": "https://cdn.b2x.com/docs/bosch/hss-g-datasheet-thumb.jpg",
      "title": "Technisches Datenblatt HSS-G Spiralbohrer",
      "mime_type": "application/pdf",
      "file_size": 1250000,
      "sort_order": 1,
      "is_primary": true,
      "is_public": true,
      "language": "de"
    },
    {
      "media_id": "doc-002",
      "media_type": "document",
      "media_subtype": "certificate",
      "url": "https://cdn.b2x.com/docs/bosch/iso-9001-cert.pdf",
      "title": "ISO 9001:2015 Zertifikat",
      "mime_type": "application/pdf",
      "file_size": 520000,
      "sort_order": 2,
      "is_primary": false,
      "is_public": true
    },
    {
      "media_id": "vid-001",
      "media_type": "video",
      "media_subtype": "tutorial",
      "url": "https://cdn.b2x.com/videos/bosch/hss-g-anwendung.mp4",
      "thumbnail_url": "https://cdn.b2x.com/videos/bosch/hss-g-anwendung-thumb.jpg",
      "title": "Richtige Anwendung von HSS-G Bohrern in Metall",
      "alt_text": "Video-Tutorial: Metallbohren mit HSS-G Spiralbohrern",
      "mime_type": "video/mp4",
      "file_size": 45000000,
      "width": 1920,
      "height": 1080,
      "duration": 180,
      "sort_order": 1,
      "is_primary": true,
      "is_public": true
    }
  ],
  "image_count": 2,
  "document_count": 2,
  "video_count": 1,
  "has_360_view": false,
  "has_video": true,
  "has_cad": false,
  "has_datasheet": true,
  
  // PRIMARY & SECONDARY IMAGE (Schnellzugriff für Listings)
  "primary_image": {
    "url": "https://cdn.b2x.com/products/bosch/hss-g-8mm-main.jpg",
    "thumbnail_url": "https://cdn.b2x.com/products/bosch/hss-g-8mm-thumb.jpg",
    "alt_text": "Bosch Professional HSS-G Spiralbohrer 8mm für Metall",
    "width": 1200,
    "height": 1200
  },
  "secondary_image": {
    "url": "https://cdn.b2x.com/products/bosch/hss-g-8mm-drawing.png",
    "thumbnail_url": "https://cdn.b2x.com/products/bosch/hss-g-8mm-drawing-thumb.png",
    "alt_text": "Maßzeichnung HSS-G 8mm: Länge 117mm, Arbeitslänge 75mm",
    "width": 800,
    "height": 400
  },
  "primary_image_url": "https://cdn.b2x.com/products/bosch/hss-g-8mm-main.jpg",
  
  // Suchbegriffe für verbesserte Auffindbarkeit
  "search_terms": "Metallbohrer Stahlbohrer Präzisionsbohrer Spiralbohrer HSS-Bohrer Bosch-Bohrer",
  "search_terms_exact": ["2608577123", "BOHR-8", "HSS-8"],
  "synonyms": "Bohrwerkzeug Bohreinsatz Bohrbit Drill",
  "common_misspellings": "Borer Borher Boherer",
  
  "stock": 250,
  "embedding": [0.12, -0.34, ...],
  
  // Rückwärts-Referenzen zu Produkten
  "product_assignments": [
    { "product_id": "prod-bohrer-set", "is_primary": false, "quantity": 1 },
    { "product_id": "prod-bohrer-8mm", "is_primary": true, "quantity": 1 },
    { "product_id": "prod-profi-set", "is_primary": false, "quantity": 2 }
  ],
  
  // LAGERBESTÄNDE PRO ABHOLLAGER (Multi-Warehouse)
  "warehouses": [
    {
      "warehouse_id": "wh-berlin",
      "warehouse_name": "Zentrallager Berlin",
      "warehouse_code": "BER",
      "warehouse_type": "central",
      // Artikeltyp: Lagerartikel
      "item_type": "stock_item",
      "is_stock_item": true,
      "is_order_item": false,
      "supplier_lead_time_days": null,
      // Lieferstatus: Lieferbar
      "availability_status": "available",
      "is_available": true,
      "is_end_of_life": false,
      "is_discontinued": false,
      "discontinued_at": null,
      "end_of_life_at": null,
      "successor_sku": null,
      // Bestand (nur suchrelevante Felder)
      "stock_quantity": 150,
      "available_quantity": 145,
      // reserved_quantity, reorder_level, reorder_quantity → ERP
      "status": "in_stock",
      "is_available": true,
      "delivery_time_days": 2,
      "delivery_time_text": "1-3 Werktage",
      "cutoff_time": "14:00",
      // next_dispatch_at wird zur Laufzeit berechnet (nicht im Index)
      "pickup_available": true,
      // pickup_time_hours/text zur Laufzeit berechnet
      "location": { "lat": 52.5200, "lon": 13.4050 },
      "city": "Berlin",
      "country": "DE",
      // Volle Adresse aus Warehouse-Stammdaten bei Bedarf
      "priority": 1,
      "is_default": true
    },
    {
      "warehouse_id": "wh-munich",
      "warehouse_name": "Filiale München",
      "warehouse_code": "MUC",
      "warehouse_type": "branch",
      // Artikeltyp: Lagerartikel
      "item_type": "stock_item",
      "is_stock_item": true,
      "is_order_item": false,
      "supplier_lead_time_days": null,
      // Lieferstatus: Auslaufartikel (noch lieferbar, aber kein Nachschub mehr)
      "availability_status": "end_of_life",
      "is_available": true,
      "is_end_of_life": true,
      "is_discontinued": false,
      "discontinued_at": null,
      "end_of_life_at": "2026-01-01",
      "successor_sku": "BOHR-8-V2",
      // Bestand (nur suchrelevante Felder)
      "stock_quantity": 75,
      "available_quantity": 73,
      // reserved_quantity, reorder_level, reorder_quantity → ERP
      "status": "in_stock",
      "is_available": true,
      "delivery_time_days": 3,
      "delivery_time_text": "2-4 Werktage",
      "cutoff_time": "12:00",
      // next_dispatch_at wird zur Laufzeit berechnet (nicht im Index)
      "pickup_available": true,
      // pickup_time_hours/text zur Laufzeit berechnet
      "location": { "lat": 48.1351, "lon": 11.5820 },
      "city": "München",
      "country": "DE",
      "priority": 2,
      "is_default": false
    },
    {
      "warehouse_id": "wh-hamburg",
      "warehouse_name": "Außenlager Hamburg",
      "warehouse_code": "HH",
      "warehouse_type": "partner",
      // Artikeltyp: Bestellware (wird vom Partner bei Bedarf bestellt)
      "item_type": "order_item",
      "is_stock_item": false,
      "is_order_item": true,
      "supplier_lead_time_days": 5,
      // Lieferstatus: Lieferbar (Bestellware)
      "availability_status": "available",
      "is_available": true,
      "is_end_of_life": false,
      "is_discontinued": false,
      "discontinued_at": null,
      "end_of_life_at": null,
      "successor_sku": null,
      // Bestand (nur suchrelevante Felder)
      "stock_quantity": 0,
      "available_quantity": 0,
      // reserved_quantity, reorder_level, reorder_quantity → ERP
      "status": "order_item",
      "is_available": true,
      "delivery_time_days": 7,
      "delivery_time_text": "5-7 Werktage (Bestellware)",
      "cutoff_time": "10:00",
      // next_dispatch_at wird zur Laufzeit berechnet (nicht im Index)
      "pickup_available": false,
      // pickup_time_hours/text nicht relevant
      "location": { "lat": 53.5511, "lon": 9.9937 },
      "city": "Hamburg",
      "country": "DE",
      "priority": 3,
      "is_default": false
    }
  ],
  // Aggregierte Lager-Felder
  "warehouse_ids": ["wh-berlin", "wh-munich", "wh-hamburg"],
  "warehouse_codes": ["BER", "MUC", "HH"],
  "total_stock_quantity": 225,        // Summe Lagerware: 150 + 75 (Hamburg = Bestellware)
  "available_warehouse_count": 3,
  "has_pickup_option": true,
  "min_delivery_time_days": 2,        // Schnellste: Berlin
  "has_stock_item": true,             // Berlin + München = Lagerartikel
  "is_pure_order_item": false,        // Nicht reine Bestellware
  // Lieferstatus (aggregiert)
  "global_availability_status": "available",  // Mindestens 1 Lager lieferbar
  "has_available_warehouse": true,            // Berlin + Hamburg lieferbar
  "is_globally_end_of_life": false,           // Nicht alle auf Auslauf
  "is_globally_discontinued": false,          // Nicht alle ausgelaufen
  "available_warehouse_ids": ["wh-berlin", "wh-hamburg"],  // Lieferbare Lager

  // VERFÜGBARKEIT (aggregiert über alle Lager)
  "availability": {
    "status": "in_stock",
    "stock_quantity": 225,            // Gesamtbestand (nur Lagerware)
    "stock_display": "Auf Lager",
    "delivery_time_days": 2,          // Schnellste Lieferzeit
    "delivery_time_text": "1-3 Werktage",
    "next_available_at": null,
    "backorder_allowed": true,
    "max_order_quantity": 1000
  },
  "is_in_stock": true,
  "is_orderable": true,
  "delivery_time_days": 2,
  
  // KUNDENBEWERTUNGEN (nur aggregierte Werte für Suche/Sortierung)
  "reviews": {
    "average_rating": 4.7,
    "review_count": 234
    // rating_distribution, recommendation_rate → aus Review-Service bei Bedarf
  },
  "review_rating": 4.7,
  "review_count": 234,
  
  // RANKING & BUSINESS SIGNALE
  "ranking": {
    "popularity_score": 87.5,
    "sales_rank": 12,
    "order_count_30d": 156,
    "view_count_30d": 2340,
    "conversion_rate": 0.067,
    "margin_score": 0.35,
    "boost_factor": 1.0
  },
  "popularity_score": 87.5,
  "sales_rank": 12,
  
  // BOOSTING (Admin-konfigurierbar + System-berechnet)
  "boosting": {
    // Entity-Boosts (Admin-gesetzt)
    "variant_boost": 1.0,      // Kein spezieller Boost auf dieser Variante
    "product_boost": 1.2,      // Produkt "Bohrer-Set" ist promoted
    "category_boost": 1.1,     // Kategorie "Bohrer" hat leichten Boost
    "brand_boost": 1.4,        // Bosch-Marke hat hohen Boost
    // Automatische Boosts (System)
    "stock_boost": 1.5,        // Ist auf Lager → Boost
    "new_boost": 1.0,          // Kein Neuheiten-Boost (nicht mehr "neu")
    "promo_boost": 1.0,        // Kein Aktions-Boost aktiv
    // Kombiniert
    "total_boost": 2.772       // 1.0 × 1.2 × 1.1 × 1.4 × 1.5 × 1.0 × 1.0
  },
  "total_boost": 2.772,
  
  // PROMOTION-FLAGS
  "is_promoted": false,        // Kein Aktionsartikel
  "is_featured": false,        // Nicht hervorgehoben
  "is_bestseller": true,       // Bestseller-Badge
  "promo_priority": null,      // Keine Aktion
  
  // PRODUKT-STATUS & LIFECYCLE
  "lifecycle": {
    "status": "active",
    "is_new": false,
    "new_until": "2025-12-31",
    "discontinued_at": null,
    "end_of_life_at": null,
    "successor_id": null,
    "launch_date": "2024-06-15"
  },
  "is_new": false,
  "is_discontinued": false,
  
  // SICHTBARKEIT
  "visibility": {
    "is_visible": true,
    "is_searchable": true,
    "is_listed": true,
    "visible_from": null,
    "visible_until": null,
    "customer_groups": [],
    "price_visible": true,
    "price_visible_for": []
  },
  
  // Zeitstempel (nur für Index-Management)
  "created_at": "2024-06-15T10:00:00Z",
  "indexed_at": "2026-01-11T08:15:00Z"
  // updated_at nicht im Index (ändert sich zu häufig)
}
```

**Produkt-Dokument mit Varianten-Referenzen:**

```json
{
  "doc_type": "product",
  "id": "prod-bohrer-set",
  "name": "Bohrer Komplett-Set 5-teilig",
  "product_type": "bundle",
  "base_price": 19.99,  // Set-Preis (günstiger als Summe)
  
  // ARTIKELSET-INFORMATIONEN
  "set_info": {
    "is_set": true,
    "set_type": "bundle",
    "component_count": 3,
    "total_items_count": 3,           // 1 + 1 + 1 = 3 Einzelteile
    "is_fixed_set": true,
    "is_configurable_set": false,
    "min_components": null,
    "max_components": null,
    "pricing_type": "discounted",
    "set_price": 19.99,               // Set-Preis
    "sum_of_parts_price": 25.47,      // Wäre 25,47 € einzeln
    "set_discount_percent": 21.5,     // 21,5% Ersparnis
    "set_savings": 5.48,              // 5,48 € gespart
    "all_components_available": true,
    "limiting_component_sku": null
  },
  // Set-Bestandteile
  "set_components": [
    {
      "component_id": "var-5mm",
      "sku": "BOHR-5MM",
      "name": "HSS Spiralbohrer 5mm",
      "quantity": 1,
      "is_required": true,
      "is_selectable": false,
      "alternatives": null,
      "unit_price": 7.49,
      "component_value": 7.49,
      "sort_order": 1
    },
    {
      "component_id": "var-8mm",
      "sku": "BOHR-8MM",
      "name": "HSS Spiralbohrer 8mm",
      "quantity": 1,
      "is_required": true,
      "is_selectable": false,
      "alternatives": null,
      "unit_price": 8.99,
      "component_value": 8.99,
      "sort_order": 2
    },
    {
      "component_id": "var-10mm",
      "sku": "BOHR-10MM",
      "name": "HSS Spiralbohrer 10mm",
      "quantity": 1,
      "is_required": true,
      "is_selectable": false,
      "alternatives": null,
      "unit_price": 8.99,
      "component_value": 8.99,
      "sort_order": 3
    }
  ],
  // Flache Set-Felder
  "is_set": true,
  "set_component_count": 3,
  "set_component_skus": ["BOHR-5MM", "BOHR-8MM", "BOHR-10MM"],
  
  "variant_assignments": [
    { "variant_id": "var-5mm", "sku": "BOHR-5MM", "quantity": 1, "sort_order": 1 },
    { "variant_id": "var-8mm", "sku": "BOHR-8MM", "quantity": 1, "sort_order": 2 },
    { "variant_id": "var-10mm", "sku": "BOHR-10MM", "quantity": 1, "sort_order": 3 }
  ],
  
  // Berechnete Felder für Suche (denormalisiert)
  "all_variant_skus": ["BOHR-5MM", "BOHR-8MM", "BOHR-10MM"],
  "all_variant_eans": ["4014364100521", "4014364100523", "4014364100525"],
  "all_variant_gtins": ["04014364100521", "04014364100523", "04014364100525"],
  "all_eclass_codes": ["27-02-01-01"],  // Eindeutige EClass-Codes
  "min_variant_stock": 45,  // Minimum aller Varianten-Bestände
  "is_available": true,     // Alle Varianten verfügbar?
  
  // Produkt-Tags
  "tags": [
    {
      "tag_id": "tag-bundle-deal",
      "tag_name": "Set-Angebot",
      "tag_group": "promotion",
      "tag_color": "#E91E63",
      "tag_icon": "mdi-package-variant",
      "priority": 1,
      "is_visible": true
    },
    {
      "tag_id": "tag-spring-2026",
      "tag_name": "Frühjahrsaktion 2026",
      "tag_group": "season",
      "tag_color": "#4CAF50",
      "tag_icon": "mdi-flower",
      "priority": 2,
      "valid_from": "2026-03-01",
      "valid_until": "2026-05-31",
      "is_visible": true
    }
  ],
  "tag_ids": ["tag-bundle-deal", "tag-spring-2026"],
  "tag_names": ["Set-Angebot", "Frühjahrsaktion 2026"],
  "tag_groups": ["promotion", "season"],
  
  // Produkt-Suchbegriffe
  "search_terms": "Bohrer-Sortiment Bohrer-Sammlung Bohrer-Kit Bohrer-Kollektion Komplett-Paket",
  "search_terms_exact": ["SET-BOHR-5", "BUNDLE-001"],
  "synonyms": "Bohrerset Bohrersammlung Bohrerkit Starterpaket",
  "common_misspellings": "Borer-Set Borher-Kit",
  
  // Aggregierte Tags aus allen Varianten
  "all_variant_tag_ids": ["tag-bestseller", "tag-made-in-germany"],
  
  // Aggregierte Suchbegriffe aus allen Varianten
  "all_variant_search_terms": "Metallbohrer Stahlbohrer Präzisionsbohrer HSS-Bohrer"
}
```

**Verfügbarkeits-Berechnung für Bundles:**

```csharp
public class BundleAvailabilityService
{
    public async Task<BundleAvailability> CalculateAvailabilityAsync(
        Product bundle, 
        IReadOnlyList<Variant> variants)
    {
        // Bundle ist nur verfügbar, wenn ALLE Varianten in ausreichender Menge da sind
        var minAvailable = int.MaxValue;
        
        foreach (var assignment in bundle.VariantAssignments)
        {
            var variant = variants.First(v => v.Id == assignment.VariantId);
            var availableSets = variant.Stock / assignment.Quantity;
            minAvailable = Math.Min(minAvailable, availableSets);
        }
        
        return new BundleAvailability
        {
            IsAvailable = minAvailable > 0,
            AvailableQuantity = minAvailable,
            LimitingVariant = FindLimitingVariant(bundle, variants)
        };
    }
}
```

**Elasticsearch Query: "Finde alle Produkte mit Variante X"**

```json
{
  "query": {
    "nested": {
      "path": "variant_assignments",
      "query": {
        "term": { "variant_assignments.variant_id": "var-8mm-bohrer" }
      }
    }
  }
}
```

**Produkt-Typen:**

| Typ | Beschreibung | Varianten |
|-----|--------------|-----------|
| `simple` | Einzelprodukt | 0-1 Varianten (Produkt = Variante) |
| `configurable` | Konfigurierbares Produkt | N Varianten (Farbe, Größe auswählbar) |
| `bundle` | Festes Set | N Varianten in festen Mengen |
| `kit` | Zusammenstellbares Set | Varianten aus Pool wählbar |

### 2.5 Varianten-Beziehungen (Typed Edges)

> **⚠️ Varianten haben typisierte Beziehungen untereinander!**

**Beziehungstypen (BMEcat PRODUCT_REFERENCE kompatibel):**

> **Hinweis:** Die Beziehungstypen orientieren sich am BMEcat 2005-Standard (PRODUCT_REFERENCE).
> Siehe: [BMEcat 2005 Spezifikation](https://www.bmecat.org)

| BMEcat-Typ | Intern | Beschreibung | Richtung | Beispiel |
|------------|--------|--------------|----------|----------|
| `accessories` | `accessories` | Zubehör | Unidirektional | Bohrer → Bohrfutter |
| `sparepart` | `sparepart` | Ersatzteil | Unidirektional | Staubsauger → Filter |
| `mandatory` | `mandatory` | Pflicht-Zusatzposition | Unidirektional | Maschine → Sicherheitsausrüstung |
| `select` | `select` | Optionale Auswahl | Unidirektional | Drucker → Papier (wähle 1) |
| `followup` | `followup` | Nachfolger | Bidirektional | Modell 2025 ↔ Modell 2026 |
| `consists_of` | `consists_of` | Bestandteil/Set-Artikel | Bidirektional | Bohrer-Set ↔ Bohrer 8mm |
| `diff_orderunit` | `diff_orderunit` | Alternative Bestelleinheit | Bidirektional | Einzeln ↔ 10er-Pack |
| `similar` | `similar` | Ähnliches Produkt | Bidirektional | Akku ↔ Ladegerät |
| `others` | `cross_sell` | Sonstige (Cross-Selling) | Unidirektional | Kamera → Tasche |
| *(custom)* | `base_product` | Basis-Produkt (Variante gehört zu) | Unidirektional | Variante → Hauptprodukt |

**Graph-Visualisierung:**

```
┌──────────────────────────────────────────────────────────────────────────────┐
│                     VARIANT RELATIONSHIP GRAPH                                │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                                               │
│  ┌─────────────┐  successor   ┌─────────────┐  successor   ┌─────────────┐  │
│  │ Bohrer V1   │─────────────→│ Bohrer V2   │─────────────→│ Bohrer V3   │  │
│  │ (auslaufend)│←─────────────│ (aktuell)   │←─────────────│ (neu)       │  │
│  └─────────────┘  predecessor └──────┬──────┘  predecessor └─────────────┘  │
│                                      │                                       │
│                                      │ accessory                             │
│                                      ▼                                       │
│                               ┌─────────────┐                                │
│                               │ Bohrfutter  │                                │
│                               └──────┬──────┘                                │
│                                      │                                       │
│                      ┌───────────────┼───────────────┐                       │
│                      │ mandatory     │ optional      │                       │
│                      ▼               ▼               ▼                       │
│               ┌───────────┐   ┌───────────┐   ┌───────────┐                 │
│               │ Schutz-   │   │ Schmier-  │   │ Reinigungs│                 │
│               │ brille    │   │ mittel    │   │ tuch      │                 │
│               └───────────┘   └───────────┘   └───────────┘                 │
│                                                                              │
│  ┌─────────────┐  alternative_unit  ┌─────────────┐                         │
│  │ Schraube    │◄──────────────────►│ Schraube    │                         │
│  │ (Einzeln)   │                    │ (100er VE)  │                         │
│  └─────────────┘                    └─────────────┘                         │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```

**Varianten-Dokument mit Beziehungen:**

```json
{
  "doc_type": "variant",
  "id": "var-bohrer-v2",
  "sku": "BOHR-8MM-V2",
  "name": "HSS Bohrer 8mm (2026)",
  "short_description": "Neue Generation des bewährten 8mm Spiralbohrers",
  "long_description": "Der überarbeitete HSS-Spiralbohrer mit verbesserter Titan-Nitrid-Beschichtung. 20% längere Standzeit gegenüber dem Vorgängermodell. Optimiert für CNC-Maschinen und manuelle Bohrständer.",
  
  // Marke (auf Varianten-Ebene definiert!)
  "brand_id": "brand-bosch",
  "brand_name": "Bosch Professional",
  
  // Typisierte Beziehungen zu anderen Varianten
  "variant_relations": [
    {
      "target_variant_id": "var-bohrer-v1",
      "relation_type": "predecessor",
      "is_bidirectional": true,
      "metadata": { "discontinued_date": "2025-06-01" }
    },
    {
      "target_variant_id": "var-bohrer-v3",
      "relation_type": "successor",
      "is_bidirectional": true,
      "metadata": { "available_from": "2026-03-01" }
    },
    {
      "target_variant_id": "var-bohrfutter",
      "relation_type": "accessory",
      "is_bidirectional": false,
      "display_context": ["product_page", "cart"],
      "sort_order": 1
    },
    {
      "target_variant_id": "var-schutzbrille",
      "relation_type": "mandatory_addon",
      "is_bidirectional": false,
      "is_auto_add": true,  // Automatisch in Warenkorb
      "quantity": 1
    },
    {
      "target_variant_id": "var-schmiermittel",
      "relation_type": "optional_addon",
      "is_bidirectional": false,
      "discount_percent": 10,  // Rabatt bei Kombikauf
      "display_context": ["cart", "checkout"]
    },
    {
      "target_variant_id": "var-bohrer-10er",
      "relation_type": "alternative_unit",
      "is_bidirectional": true,
      "unit_factor": 10,  // 10er-Pack = 10x Einzeln
      "price_advantage_percent": 15
    }
  ],
  
  // Denormalisierte Felder für schnelle Suche
  "has_accessories": true,
  "has_mandatory_addons": true,
  "has_successors": true,
  "accessory_count": 3,
  "relation_types": ["predecessor", "successor", "accessory", "mandatory_addon", "optional_addon", "alternative_unit"]
}
```

**Domain Model:**

```csharp
public class VariantRelation
{
    public Guid SourceVariantId { get; set; }
    public Guid TargetVariantId { get; set; }
    public VariantRelationType Type { get; set; }
    public bool IsBidirectional { get; set; }
    public int SortOrder { get; set; }
    public string[] DisplayContexts { get; set; }  // product_page, cart, checkout
    public VariantRelationMetadata Metadata { get; set; }
}

/// <summary>
/// Beziehungstypen gemäß BMEcat 2005 PRODUCT_REFERENCE.
/// Zusätzliche Typen für B2X-spezifische Anforderungen.
/// </summary>
public enum VariantRelationType
{
    // === BMEcat 2005 Standard-Typen ===
    
    /// <summary>Zubehör (accessories)</summary>
    Accessories,
    
    /// <summary>Ersatzteil (sparepart)</summary>
    Sparepart,
    
    /// <summary>Pflicht-Zusatzposition (mandatory)</summary>
    Mandatory,
    
    /// <summary>Optionale Auswahl (select) - mindestens eines auswählen</summary>
    Select,
    
    /// <summary>Nachfolge-Artikel (followup)</summary>
    Followup,
    
    /// <summary>Bestandteil eines Sets (consists_of)</summary>
    ConsistsOf,
    
    /// <summary>Alternative Bestelleinheit (diff_orderunit)</summary>
    DiffOrderunit,
    
    /// <summary>Ähnliches Produkt (similar)</summary>
    Similar,
    
    /// <summary>Sonstige Beziehung (others) - für Cross-Selling</summary>
    Others,
    
    // === B2X Erweiterungen ===
    
    /// <summary>Basis-Produkt (Variante gehört zu diesem Produkt)</summary>
    BaseProduct
}

public class VariantRelationMetadata
{
    public int? Quantity { get; set; }           // Bei mandatory: Menge
    public decimal? UnitFactor { get; set; }     // Bei alternative_unit: Faktor
    public decimal? DiscountPercent { get; set; } // Rabatt bei Kombikauf
    public bool IsAutoAdd { get; set; }          // Automatisch in Warenkorb
    public DateTime? AvailableFrom { get; set; } // Bei successor
    public DateTime? DiscontinuedDate { get; set; } // Bei predecessor
}
```

**Business Rules für Beziehungen:**

```csharp
public class VariantRelationService
{
    /// <summary>
    /// Pflicht-Zusatzpositionen automatisch zum Warenkorb hinzufügen
    /// </summary>
    public async Task<CartUpdateResult> AddMandatoryAddonsAsync(
        Cart cart, 
        Variant variant)
    {
        var mandatoryAddons = await GetRelationsAsync(
            variant.Id, 
            VariantRelationType.MandatoryAddon);
        
        foreach (var addon in mandatoryAddons.Where(a => a.Metadata.IsAutoAdd))
        {
            await cart.AddItemAsync(
                addon.TargetVariantId, 
                addon.Metadata.Quantity ?? 1,
                linkedToVariantId: variant.Id);
        }
        
        return new CartUpdateResult { AddedMandatoryItems = mandatoryAddons.Count };
    }
    
    /// <summary>
    /// Nachfolger-Hinweis bei auslaufenden Artikeln
    /// </summary>
    public async Task<SuccessorInfo?> GetSuccessorInfoAsync(Variant variant)
    {
        if (!variant.IsDiscontinued) return null;
        
        var successor = await GetRelationsAsync(
            variant.Id, 
            VariantRelationType.Successor);
        
        return successor.FirstOrDefault() is { } rel 
            ? new SuccessorInfo 
            {
                SuccessorVariantId = rel.TargetVariantId,
                AvailableFrom = rel.Metadata.AvailableFrom,
                Message = $"Dieser Artikel wird durch {rel.TargetVariant.Name} ersetzt."
            }
            : null;
    }
    
    /// <summary>
    /// Alternative Verkaufseinheiten mit Preisvorteil anzeigen
    /// </summary>
    public async Task<AlternativeUnitOffer[]> GetAlternativeUnitsAsync(
        Variant variant, 
        int requestedQuantity)
    {
        var alternatives = await GetRelationsAsync(
            variant.Id, 
            VariantRelationType.AlternativeUnit);
        
        return alternatives
            .Where(a => IsMoreEconomical(variant, a, requestedQuantity))
            .Select(a => new AlternativeUnitOffer
            {
                VariantId = a.TargetVariantId,
                UnitFactor = a.Metadata.UnitFactor,
                PriceAdvantagePercent = a.Metadata.PriceAdvantagePercent,
                SuggestedQuantity = CalculateSuggestedQuantity(requestedQuantity, a)
            })
            .ToArray();
    }
}
```

**Elasticsearch Queries für Beziehungen:**

```json
// Finde alle Zubehör-Varianten für Variante X
{
  "query": {
    "nested": {
      "path": "variant_relations",
      "query": {
        "bool": {
          "must": [
            { "term": { "variant_relations.target_variant_id": "var-bohrer-v2" } },
            { "terms": { "variant_relations.relation_type": ["accessory", "optional_addon"] } }
          ]
        }
      }
    }
  }
}

// Finde Nachfolger für auslaufende Artikel
{
  "query": {
    "bool": {
      "must": [
        { "term": { "is_discontinued": true } },
        { "term": { "has_successors": true } }
      ]
    }
  }
}
```

**Frontend-Anzeige nach Kontext:**

| Kontext | Angezeigte Beziehungen |
|---------|------------------------|
| **Produktseite** | Zubehör, Ersatzteile, Nachfolger, Alternativen |
| **Warenkorb** | Optional Addons, Alternative VE (Sparangebot) |
| **Checkout** | Mandatory Addons (falls nicht im Warenkorb) |
| **Suche** | Cross-Sell, Compatible |
| **Auslaufend-Banner** | Successor mit Verfügbarkeitsdatum |

---

### 2.6 Typisierte Attribute mit Sortierungsoptionen

**Problem:** Merkmale/Attribute haben unterschiedliche Datentypen und erfordern verschiedene Sortierungslogik in Facetten.

| Datentyp | Beispiele | Facetten-Darstellung | Sortierung |
|----------|-----------|---------------------|------------|
| **Text** | Farbe, Material, Marke | Terms (Chips/Checkboxen) | Alphabetisch, Numerisch, Manuell |
| **Nummer** | Länge, Gewicht, Leistung | Range-Slider oder Terms | Numerisch |
| **Boolean** | "Bio", "Wasserdicht" | Toggle/Checkbox | N/A |
| **Range** | Temperaturbereich | Zwei Slider | Min/Max |
| **Farbe** | Farbton mit Hex-Code | Farbpalette | Manuell |
| **Multi-Select** | Eigenschaften | Mehrfachauswahl | Alphabetisch |

**Sortierungsmodi für Text-Attribute:**

```
┌──────────────────────────────────────────────────────────────────────────────┐
│                        ATTRIBUTE SORTING MODES                                │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                                               │
│  ALPHABETISCH (sort_mode: "alpha")                                           │
│  ┌─────────────────────────────────────────────────────────────────────────┐ │
│  │  [ ] Blau   [ ] Gelb   [ ] Grün   [ ] Rot   [ ] Schwarz   [ ] Weiß     │ │
│  └─────────────────────────────────────────────────────────────────────────┘ │
│                                                                               │
│  NUMERISCH (sort_mode: "numeric")  - bei Größen wie "S", "M", "L", "XL"     │
│  ┌─────────────────────────────────────────────────────────────────────────┐ │
│  │  [ ] XS   [ ] S   [ ] M   [ ] L   [ ] XL   [ ] XXL                      │ │
│  │  (sort_value: 1, 2, 3, 4, 5, 6)                                        │ │
│  └─────────────────────────────────────────────────────────────────────────┘ │
│                                                                               │
│  MANUELL (sort_mode: "manual")  - für strategische Sortierung               │
│  ┌─────────────────────────────────────────────────────────────────────────┐ │
│  │  [ ] Premium   [ ] Standard   [ ] Economy                               │ │
│  │  (sort_order: 1, 2, 3 - vom Fachadmin gepflegt)                        │ │
│  └─────────────────────────────────────────────────────────────────────────┘ │
│                                                                               │
│  POPULARITY (sort_mode: "popular")  - basierend auf Produktanzahl           │
│  ┌─────────────────────────────────────────────────────────────────────────┐ │
│  │  [ ] Edelstahl (234)   [ ] Kunststoff (189)   [ ] Holz (45)            │ │
│  └─────────────────────────────────────────────────────────────────────────┘ │
│                                                                               │
└──────────────────────────────────────────────────────────────────────────────┘
```

**Attribut-Definition (BMEcat FEATURE_SYSTEM kompatibel):**

> **Hinweis:** Die Attributstruktur orientiert sich am BMEcat 2005-Standard (FEATURE/FEATURE_GROUP).
> - `FEATURE_SYSTEM_NAME` → `feature_system` (z.B. "ECLASS", "ETIM", "Custom")
> - `FEATURE_GROUP_ID/NAME` → `feature_group_id/name`
> - `FNAME` → `fname` (Merkmalname)
> - `FVALUE` → `fvalue` (Merkmalwert)
> - `FUNIT` → `funit` (Einheit)
> - `FORDER` → `forder` (Sortierung)
> - `FVALUE_TYPE` → `fvalue_type` (Datentyp)

```json
{
  "doc_type": "feature_definition",
  "id": "feat-size",
  
  // BMEcat FEATURE_SYSTEM Referenz
  "feature_system": "ECLASS",             // ECLASS | ETIM | UNSPSC | Custom
  "feature_system_version": "12.0",
  "feature_group_id": "FG-DIMENSIONS",
  "feature_group_name": "Abmessungen",
  
  // BMEcat FEATURE Mapping
  "fname": "size",                        // FNAME - Merkmalname (Code)
  "fname_display": "Größe",               // Pre-lokalisiert für Anzeige
  "fdescription": "Kleidungsgröße",       // FDESCR
  "fvalue_type": "text",                  // FVALUE_TYPE: text | number | boolean | range | set
  "funit": null,                          // FUNIT - Einheit (mm, kg, etc.)
  "forder": 10,                           // FORDER - Sortierung in Merkmalgruppe
  
  // Sortierungs-Konfiguration (B2X-Erweiterung)
  "sort_mode": "numeric",                 // alpha | numeric | manual | popular
  "sort_direction": "asc",                // asc | desc
  
  // Facetten-Konfiguration (B2X-Erweiterung)
  "is_filterable": true,                  // In Facetten anzeigen
  "is_searchable": true,                  // In Volltextsuche einbeziehen
  "is_comparable": true,                  // Im Produktvergleich anzeigen
  "display_type": "chips",                // chips | checkboxes | dropdown | slider | colorpicker
  "collapse_threshold": 10,               // Ab X Werten: "Mehr anzeigen"
  
  // Für numerische Attribute (BMEcat FVALUE_DETAILS)
  "fvalue_min": null,                     // Minimalwert
  "fvalue_max": null,                     // Maximalwert
  "fvalue_step": null,                    // Schrittweite
  
  // ALLOWED_VALUES für manuelle Sortierung (BMEcat ALLOWED_VALUES)
  "allowed_values": [
    { "fvalue": "XS", "forder": 1, "fvalue_display": "Extra Small" },
    { "fvalue": "S",  "forder": 2, "fvalue_display": "Small" },
    { "fvalue": "M",  "forder": 3, "fvalue_display": "Medium" },
    { "fvalue": "L",  "forder": 4, "fvalue_display": "Large" },
    { "fvalue": "XL", "forder": 5, "fvalue_display": "Extra Large" },
    { "fvalue": "XXL","forder": 6, "fvalue_display": "Double Extra Large" }
  ],
  
  // Kategorien, für die dieses Merkmal relevant ist (BMEcat-kompatibel)
  "applicable_category_ids": ["cat-clothing", "cat-shoes"],
  
  // Merkmalgruppe für UI-Gruppierung
  "feature_group": "dimensions",
  "feature_group_order": 1,
  
  "embedding": [0.12, -0.34, ...]         // Für semantische Suche nach Merkmalen
}
```

**Produkt-Merkmalwerte im Index (BMEcat PRODUCT_FEATURES):**

```json
{
  "doc_type": "product",
  "id": "prod-shirt-123",
  "name": "Premium T-Shirt",
  
  // PRODUCT_FEATURES gemäß BMEcat-Struktur
  "features": [
    {
      // BMEcat FEATURE Mapping
      "fname": "size",                    // FNAME - Merkmalcode
      "fname_display": "Größe",           // Lokalisierter Anzeigename
      "fvalue": "M",                      // FVALUE - Merkmalwert
      "fvalue_display": "Medium",         // Lokalisierter Anzeigewert
      "fvalue_type": "text",              // FVALUE_TYPE
      "funit": null,                      // FUNIT - Einheit
      "forder": 3,                        // FORDER - Sortierung
      "forder_value": "0003",             // Berechneter Sortierwert (padded)
      
      // B2X-Erweiterungen
      "is_filterable": true,
      "is_searchable": true
    },
    {
      "fname": "color",
      "fname_display": "Farbe",
      "fvalue": "Marineblau",
      "fvalue_type": "text",              // Oder "color" als Erweiterung
      "fvalue_color_hex": "#001F3F",      // B2X-Erweiterung für Farbdarstellung
      "forder": 5,
      "forder_value": "marineblau",
      "is_filterable": true,
      "is_searchable": true
    },
    {
      "code": "weight",
      "name": "Gewicht",
      "data_type": "number",
      "value_number": 0.25,
      "unit": "kg",
      "is_filterable": true,
      "is_searchable": false,
      "is_comparable": true
    },
    {
      "fname": "temperature_range",
      "fname_display": "Einsatztemperatur",
      "fvalue_type": "range",             // BMEcat: interval
      "fvalue_min": -10,                  // FVALUE_DETAILS/INTERVALS
      "fvalue_max": 40,
      "funit": "°C",
      "is_filterable": true,
      "is_searchable": false
    },
    {
      "fname": "is_waterproof",
      "fname_display": "Wasserdicht",
      "fvalue_type": "boolean",
      "fvalue": true,
      "fvalue_display": "Ja",             // Lokalisiert
      "is_filterable": true,
      "is_searchable": true
    }
  ]
}
```

**Elasticsearch Aggregation mit korrekter Sortierung:**

```json
{
  "aggs": {
    "size_facet": {
      "nested": { "path": "features" },
      "aggs": {
        "filter_size": {
          "filter": { "term": { "features.fname": "size" } },
          "aggs": {
            "values": {
              "terms": {
                "field": "features.fvalue",
                "size": 50,
                "order": { "sort_key": "asc" }
              },
              "aggs": {
                "sort_key": {
                  "min": { "field": "features.forder" }
                }
              }
            }
          }
        }
      }
    },
    
    "weight_facet": {
      "nested": { "path": "features" },
      "aggs": {
        "filter_weight": {
          "filter": { "term": { "attributes.code": "weight" } },
          "aggs": {
            "stats": {
              "extended_stats": { "field": "attributes.value_number" }
            },
            "histogram": {
              "histogram": {
                "field": "attributes.value_number",
                "interval": 0.5
              }
            }
          }
        }
      }
    },
    
    "color_facet": {
      "nested": { "path": "features" },
      "aggs": {
        "filter_color": {
          "filter": { "term": { "features.fname": "color" } },
          "aggs": {
            "values": {
              "terms": {
                "field": "features.fvalue",
                "size": 100
              },
              "aggs": {
                "hex_code": {
                  "top_hits": {
                    "size": 1,
                    "_source": ["features.fvalue_color_hex"]
                  }
                },
                "sort_key": {
                  "min": { "field": "features.forder" }
                }
              }
            }
          }
        }
      }
    }
  }
}
```

**Domain Model (BMEcat FEATURE kompatibel):**

```csharp
/// <summary>
/// Attribut-Definition mit BMEcat FEATURE-Mapping.
/// Interne Klassennamen bleiben "Attribute" für Konsistenz.
/// </summary>
public class AttributeDefinition
{
    public Guid Id { get; set; }
    
    // BMEcat FEATURE_SYSTEM
    public string FeatureSystem { get; set; }         // ECLASS, ETIM, UNSPSC, Custom
    public string? FeatureSystemVersion { get; set; }
    public string FeatureGroupId { get; set; }
    public LocalizedString FeatureGroupName { get; set; }
    
    // BMEcat FEATURE → Interne Benennung
    public string Code { get; set; }                  // FNAME → Code
    public LocalizedString Name { get; set; }         // Lokalisierter Name
    public AttributeDataType DataType { get; set; }   // FVALUE_TYPE
    public string? Unit { get; set; }                 // FUNIT
    public int SortOrder { get; set; }                // FORDER
    
    // Sortierungs-Konfiguration (B2X-Erweiterung)
    public AttributeSortMode SortMode { get; set; }
    public SortDirection SortDirection { get; set; }
    
    // Facetten-Konfiguration (B2X-Erweiterung)
    public bool IsFilterable { get; set; }
    public bool IsSearchable { get; set; }
    public bool IsComparable { get; set; }
    public AttributeDisplayType DisplayType { get; set; }
    public int? CollapseThreshold { get; set; }
    
    // Für Range-Attribute (BMEcat FVALUE_DETAILS)
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public decimal? Step { get; set; }
}

/// <summary>
/// Datentypen gemäß BMEcat FVALUE_TYPE.
/// </summary>
public enum AttributeDataType
{
    Text,       // Einfacher Text (Marke, Material)
    Number,     // Numerisch (Gewicht, Länge) - BMEcat: numeric
    Boolean,    // Ja/Nein (Wasserdicht, Bio) - BMEcat: boolean
    Range,      // Bereich (Temperatur von-bis) - BMEcat: interval
    Set         // Mehrfachauswahl - BMEcat: set
}

/// <summary>
/// Sortierungsmodi für Facetten (B2X-Erweiterung).
/// </summary>
public enum AttributeSortMode
{
    Alphabetic,  // A-Z oder Z-A
    Numeric,     // 1-100 (Text wird als Zahl interpretiert: "S"=1, "M"=2)
    Manual,      // Nach SortOrder aus Stammdaten
    Popular      // Nach Anzahl Produkte mit diesem Wert
}

/// <summary>
/// Anzeigetypen für Facetten im Frontend (B2X-Erweiterung).
/// </summary>
public enum AttributeDisplayType
{
    Chips,       // Horizontale Tags
    Checkboxes,  // Vertikale Liste mit Checkboxen
    Dropdown,    // Aufklappbare Liste
    Slider,      // Range-Slider (für numerische Werte)
    ColorPicker, // Farbpalette
    Toggle       // Ein/Aus Schalter (für Boolean)
}
```

**Indexierung mit Sortier-Wert-Berechnung:**

```csharp
public class AttributeIndexingService
{
    /// <summary>
    /// Mappt Produkt-Attribut auf indizierbare Struktur.
    /// BMEcat-Feldnamen (fname, fvalue, etc.) werden für ES verwendet,
    /// interne Klassen behalten "Attribute"-Benennung.
    /// </summary>
    public IndexableAttribute MapAttribute(
        ProductAttribute productAttribute,
        AttributeDefinition definition)
    {
        return new IndexableAttribute
        {
            // BMEcat-kompatible Feldnamen für Elasticsearch
            FName = definition.Code,                              // Code → fname
            FNameDisplay = definition.Name.GetLocalized(_culture),
            FValueType = definition.DataType.ToString().ToLowerInvariant(),
            
            // Wert je nach Datentyp (BMEcat FVALUE)
            FValue = productAttribute.TextValue,
            FValueDisplay = productAttribute.DisplayValue?.GetLocalized(_culture),
            FValueNumber = productAttribute.NumericValue,
            FValueBoolean = productAttribute.BooleanValue,
            FValueMin = productAttribute.RangeMin,
            FValueMax = productAttribute.RangeMax,
            FValueColorHex = productAttribute.ColorHex,
            
            // Sortierung berechnen (FORDER)
            FOrder = CalculateFOrder(productFeature, definition),
            FOrderValue = CalculateSortValue(productAttribute, definition),
            
            // Einheit (FUNIT)
            FUnit = definition.Unit,
            
            // Flags (B2X-Erweiterung)
            IsFilterable = definition.IsFilterable,
            IsSearchable = definition.IsSearchable
        };
    }
    
    private int CalculateSortOrder(ProductAttribute attr, AttributeDefinition def)
    {
        return def.SortMode switch
        {
            AttributeSortMode.Manual => attr.ManualSortOrder ?? 999,
            
            AttributeSortMode.Numeric when attr.NumericValue.HasValue =>
                (int)(attr.NumericValue.Value * 1000),  // Precision: 0.001
            
            _ => 0  // Für alphabetisch: sort_value wird verwendet
        };
    }
    
    private string CalculateSortValue(ProductAttribute attr, AttributeDefinition def)
    {
        return def.SortMode switch
        {
            AttributeSortMode.Alphabetic => 
                attr.TextValue?.ToLowerInvariant() ?? "",
            
            AttributeSortMode.Numeric => 
                attr.NumericValue?.ToString("D10") ??
                attr.TextValue?.PadLeft(10, '0') ?? "0000000000",
            
            AttributeSortMode.Manual =>
                (attr.ManualSortOrder ?? 999).ToString("D5"),
            
            AttributeSortMode.Popular => "0",  // Wird durch Aggregation bestimmt
            
            _ => attr.TextValue ?? ""
        };
    }
}
```

**Frontend-Darstellung der Facetten:**

```typescript
interface AttributeFacet {
  code: string;
  name: string;
  dataType: 'text' | 'number' | 'boolean' | 'range' | 'color' | 'multi';
  displayType: 'chips' | 'checkboxes' | 'dropdown' | 'slider' | 'colorpicker' | 'toggle';
  values: AttributeFacetValue[];
  stats?: { min: number; max: number; avg: number };  // Für numerische
  collapsed?: boolean;  // Wenn values.length > collapseThreshold
}

interface AttributeFacetValue {
  value: string;
  label: string;
  count: number;
  sortOrder: number;
  colorHex?: string;
  selected: boolean;
}

// Rendering basierend auf displayType
const renderFacet = (facet: AttributeFacet) => {
  switch (facet.displayType) {
    case 'chips':
      return <ChipGroup values={facet.values} onSelect={handleSelect} />;
    case 'colorpicker':
      return <ColorPalette values={facet.values} onSelect={handleSelect} />;
    case 'slider':
      return <RangeSlider 
        min={facet.stats?.min} 
        max={facet.stats?.max} 
        onChange={handleRangeChange} 
      />;
    case 'toggle':
      return <Toggle checked={facet.values[0]?.selected} onChange={handleToggle} />;
    default:
      return <CheckboxList values={facet.values} onSelect={handleSelect} />;
  }
};
```

---

### 2.8 Multi-Path Kategorisierung (Details)

**Lösungsansatz:**

```csharp
public class ProductCategoryAssignment
{
    public Guid CategoryId { get; set; }
    public string[] CategoryPath { get; set; }      // Lokalisierte Namen
    public Guid[] CategoryPathIds { get; set; }     // IDs für Navigation
    public bool IsPrimary { get; set; }             // Für Breadcrumb/Canonical
    public CategoryAssignmentType Type { get; set; }
    public DateTime? ValidFrom { get; set; }        // Für temporäre Zuordnungen
    public DateTime? ValidUntil { get; set; }
}

public enum CategoryAssignmentType
{
    Permanent,    // Sortimentszuordnung
    Promotion,    // Aktionszuordnung (zeitlich begrenzt)
    Seasonal,     // Saisonale Zuordnung
    Virtual       // Berechnete Zuordnung (Bestseller, Neuheiten)
}
```

**Elasticsearch Nested Query für Multi-Kategorie:**

```json
{
  "query": {
    "nested": {
      "path": "category_assignments",
      "query": {
        "bool": {
          "must": [
            { "term": { "category_assignments.category_id": "cat-angebote" } }
          ],
          "filter": [
            { "term": { "category_assignments.assignment_type": "promotion" } },
            { "range": { "category_assignments.valid_until": { "gte": "now" } } }
          ]
        }
      }
    }
  }
}
```

**Aggregation über alle Kategorie-Pfade:**

```json
{
  "aggs": {
    "categories": {
      "nested": { "path": "category_assignments" },
      "aggs": {
        "by_category": {
          "terms": { 
            "field": "category_assignments.category_path",
            "size": 100
          }
        }
      }
    }
  }
}
```

### 2.4 Semantic Search mit Embeddings

**Embedding-Generierung beim Import:**

```csharp
public class EmbeddingService
{
    private readonly IEmbeddingModel _model; // e.g., Azure OpenAI, Local ONNX
    
    public async Task<float[]> GenerateProductEmbeddingAsync(
        LocalizedProduct product)
    {
        // Combine relevant text for embedding
        var text = $"{product.Name} {product.Description} {product.Category} {product.Brand}";
        
        // Generate 768-dim embedding
        return await _model.EmbedAsync(text);
    }
}
```

**Hybrid Search Query:**

```json
{
  "query": {
    "bool": {
      "should": [
        {
          "multi_match": {
            "query": "kabelloser Bluetooth Kopfhörer",
            "fields": ["name^3", "description", "brand_name", "category_path"],
            "fuzziness": "AUTO"
          }
        },
        {
          "knn": {
            "field": "embedding",
            "query_vector": [0.12, -0.34, ...],  // Query embedding
            "k": 50,
            "boost": 0.5
          }
        }
      ],
      "filter": [
        { "term": { "doc_type": "product" } },
        { "term": { "tenant_id": "acme" } }
      ]
    }
  },
  "aggs": {
    "categories": { "terms": { "field": "category_path" } },
    "brands": { "terms": { "field": "brand_name" } },
    "price_ranges": { "range": { "field": "price", "ranges": [...] } }
  }
}
```

---

### 2.5 Synonym- & Abkürzungs-Management (MVP ✓)

> **⚠️ KRITISCHES MVP-FEATURE: Ohne Synonym-Handling ist die Suche für B2B unbrauchbar!**

#### Das Problem: Fachsprache im B2B

```
┌─────────────────────────────────────────────────────────────────┐
│                    DAS SYNONYM-CHAOS IM B2B                      │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  PRODUKT: Waschtisch-Armatur mit Einhebelbedienung              │
│                                                                  │
│  WIE SUCHT DER KUNDE?                                           │
│  ─────────────────────────────────────────────────────────────  │
│  • "WT-Armatur"                    ← Abkürzung                  │
│  • "Waschtischarmatur"             ← Zusammenschreibung         │
│  • "Waschtisch Armatur"            ← Getrennt                   │
│  • "Einhebelmischer"               ← Funktionsbezeichnung       │
│  • "Einhandmischer"                ← Synonym                    │
│  • "Zentralmischarmatur"           ← Fachbegriff (alt)          │
│  • "Waschtisch-Mischbatterie"      ← Regional/Österreich        │
│  • "Lavabo-Armatur"                ← Schweiz                    │
│  • "basin tap"                     ← Englisch (Katalog)         │
│  • "Grohe Eurosmart"               ← Marke + Modell             │
│                                                                  │
│  ALLE müssen dasselbe Produkt finden!                           │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Typische B2B-Abkürzungen (Beispiele SHK-Branche)

| Abkürzung | Vollform | Weitere Synonyme |
|-----------|----------|------------------|
| **WT** | Waschtisch | Waschbecken, Lavabo, Basin |
| **WC** | Water Closet | Toilette, Klosett, Abort |
| **UP** | Unterputz | Einbau, verdeckt, flush-mounted |
| **AP** | Aufputz | Wandmontage, exposed |
| **DN** | Durchmesser Nennweite | Nennweite, Rohrdurchmesser |
| **HT** | Hochtemperatur (Rohr) | Abwasserrohr, drainage pipe |
| **PE** | Polyethylen | Kunststoffrohr |
| **KG** | Kanalgrundrohr | Abwasser, Kanal |
| **EHM** | Einhebelmischer | Einhandmischer, single-lever |
| **THM** | Thermostatmischer | Thermostat, thermostatic |
| **BWM** | Brauchwassermischer | - |
| **FB** | Fußbodenheizung | Flächenheizung, underfloor |
| **HK** | Heizkörper | Radiator, Heater |

#### Elasticsearch Synonym-Architektur

```
┌────────────────────────────────────────────────────────────────────────────┐
│                         SYNONYM PROCESSING PIPELINE                         │
├────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  USER QUERY: "WT-Armatur"                                                   │
│       │                                                                     │
│       ▼                                                                     │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │ 1. ANALYZER: Query-Time Synonym Expansion                          │   │
│  │    "WT-Armatur" → "WT-Armatur" OR "Waschtisch-Armatur"             │   │
│  │                   OR "Waschtischarmatur" OR "Lavabo-Armatur"        │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│       │                                                                     │
│       ▼                                                                     │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │ 2. TOKENIZER: Compound Word Splitting (Decompounding)              │   │
│  │    "Waschtischarmatur" → ["Waschtisch", "Armatur"]                  │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│       │                                                                     │
│       ▼                                                                     │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │ 3. SEMANTIC FALLBACK: Embedding-basierte Ähnlichkeit               │   │
│  │    Falls keine exakten Synonyme → kNN-Search findet "ähnliche"     │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│       │                                                                     │
│       ▼                                                                     │
│  RESULTS: Alle Waschtisch-Armaturen gefunden!                              │
│                                                                             │
└────────────────────────────────────────────────────────────────────────────┘
```

#### Elasticsearch Index Settings mit Synonymen

```json
{
  "settings": {
    "analysis": {
      "filter": {
        "german_synonym_filter": {
          "type": "synonym_graph",
          "synonyms_path": "synonyms/de_shk.txt",
          "updateable": true
        },
        "german_abbreviation_filter": {
          "type": "synonym_graph",
          "synonyms": [
            "wt, waschtisch, waschbecken, lavabo",
            "wc, toilette, klosett, water closet",
            "up, unterputz, einbau, verdeckt, flush mounted",
            "ap, aufputz, wandmontage, exposed",
            "dn, durchmesser nennweite, nennweite",
            "ehm, einhebelmischer, einhandmischer, single lever mixer",
            "thm, thermostatmischer, thermostat, thermostatic mixer"
          ]
        },
        "german_decompound_filter": {
          "type": "hyphenation_decompounder",
          "word_list_path": "analysis/de_compounds.txt",
          "hyphenation_patterns_path": "analysis/de_hyph.xml",
          "min_word_size": 5,
          "min_subword_size": 4,
          "max_subword_size": 15
        },
        "german_stemmer": {
          "type": "stemmer",
          "language": "light_german"
        },
        "german_stop": {
          "type": "stop",
          "stopwords": "_german_"
        }
      },
      "analyzer": {
        "german_product_analyzer": {
          "type": "custom",
          "tokenizer": "standard",
          "filter": [
            "lowercase",
            "german_abbreviation_filter",
            "german_synonym_filter",
            "german_decompound_filter",
            "german_stop",
            "german_stemmer"
          ]
        },
        "german_search_analyzer": {
          "type": "custom",
          "tokenizer": "standard",
          "filter": [
            "lowercase",
            "german_abbreviation_filter",
            "german_synonym_filter"
          ]
        }
      }
    }
  },
  "mappings": {
    "properties": {
      "name": {
        "type": "text",
        "analyzer": "german_product_analyzer",
        "search_analyzer": "german_search_analyzer",
        "fields": {
          "exact": { "type": "keyword" },
          "suggest": { "type": "completion" }
        }
      },
      "description": {
        "type": "text",
        "analyzer": "german_product_analyzer"
      },
      "search_keywords": {
        "type": "text",
        "analyzer": "german_product_analyzer",
        "boost": 2.0
      }
    }
  }
}
```

#### Synonym-Dateien: Struktur & Wartung

**Datei: `synonyms/de_shk.txt` (SHK-Branche)**

```text
# ═══════════════════════════════════════════════════════════════
# B2X Synonym-Wörterbuch: Sanitär, Heizung, Klima (SHK)
# Format: term1, term2, term3 => canonical_term (optional)
# Letzte Aktualisierung: 2026-01-11
# ═══════════════════════════════════════════════════════════════

# ─── ARMATUREN ─────────────────────────────────────────────────
wt-armatur, wt armatur, waschtischarmatur, waschtisch-armatur, waschtisch armatur, lavabo-armatur, basin tap, basin mixer
einhebelmischer, einhandmischer, einhebel-mischbatterie, single lever mixer, monoblock tap
zweigrifarmatur, zweigriff-armatur, zwei-griff-armatur, two handle mixer
thermostatarmatur, thermostat-armatur, thermostatmischer, thermostatic mixer
brausearmatur, duscharmatur, shower mixer, brause-mischbatterie
wannenarmatur, badewannenarmatur, bath mixer, wannen-mischbatterie
spültischarmatur, küchenarmatur, kitchen tap, kitchen mixer
wandarmatur, wand-armatur, wall mounted tap

# ─── SANITÄROBJEKTE ────────────────────────────────────────────
waschtisch, waschbecken, lavabo, handwaschbecken, hwb, basin, sink
wc, toilette, klosett, water closet, wc-schüssel, toilet
urinal, pissoir, urinal-becken
bidet, bidet-becken
dusche, brause, shower
badewanne, wanne, bathtub, bath

# ─── MONTAGEARTEN ──────────────────────────────────────────────
unterputz, up, einbau, verdeckt, flush mounted, concealed
aufputz, ap, wandmontage, exposed, surface mounted
standmontage, stand, floor mounted, freistehend

# ─── ROHRE & VERBINDUNGEN ──────────────────────────────────────
ht-rohr, hochtemperaturrohr, abwasserrohr, drainage pipe
kg-rohr, kanalgrundrohr, abwasserrohr außen
pe-rohr, polyethylenrohr, kunststoffrohr pe
pvc-rohr, polyvinylchloridrohr, kunststoffrohr pvc
kupferrohr, cu-rohr, copper pipe
edelstahlrohr, va-rohr, inox-rohr, stainless steel pipe
pressverbindung, pressfitting, press fitting
klemmverbindung, klemmfitting, compression fitting
lötverbindung, lötfitting, solder fitting

# ─── HEIZUNG ───────────────────────────────────────────────────
heizkörper, hk, radiator, heater, konvektor
fußbodenheizung, fb, fbh, flächenheizung, underfloor heating
heizkreisverteiler, hkv, verteiler, manifold
thermostatventil, thermostat, trv, thermostatic valve
rücklaufverschraubung, rlv, return valve
vorlauf, vl, flow, supply
rücklauf, rl, return

# ─── MAßE & NORMEN ─────────────────────────────────────────────
dn15, 1/2 zoll, 1/2", 15mm, g1/2
dn20, 3/4 zoll, 3/4", 20mm, g3/4
dn25, 1 zoll, 1", 25mm, g1
dn32, 1 1/4 zoll, 1 1/4", 32mm
dn40, 1 1/2 zoll, 1 1/2", 40mm
dn50, 2 zoll, 2", 50mm

# ─── ELEKTRO (Cross-Domain) ────────────────────────────────────
led, leuchtdiode, light emitting diode
lsi, lastschalteinrichtung, leistungsschalter
fi, rcd, fehlerstromschutzschalter, residual current device
```

#### Tenant-spezifische Synonyme

```typescript
// Jeder Tenant kann eigene Synonyme pflegen
interface TenantSynonymConfig {
  tenant_id: string;
  industry: 'shk' | 'elektro' | 'bau' | 'industrie' | 'custom';
  
  // Standard-Wörterbücher aktivieren
  enabled_dictionaries: string[];  // ['de_shk', 'de_elektro', 'de_general']
  
  // Tenant-eigene Synonyme
  custom_synonyms: SynonymEntry[];
  
  // Produkt-spezifische Aliases (aus ERP/PIM)
  product_aliases: ProductAlias[];
}

interface SynonymEntry {
  terms: string[];          // ['WT-Armatur', 'Waschtischarmatur']
  canonical?: string;       // Optional: Bevorzugter Begriff
  category?: string;        // Nur in dieser Kategorie anwenden
  priority: number;         // Bei Konflikten
}

interface ProductAlias {
  sku: string;              // Artikelnummer
  aliases: string[];        // ['Bestseller 2024', 'Messeaktion']
  search_boost: number;     // Extra-Boost für diese Aliases
}
```

#### Admin-UI: Synonym-Management

```
┌─────────────────────────────────────────────────────────────────┐
│  📚 SYNONYM-VERWALTUNG                          [+ Neu] [Import]│
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  🔍 Filter: [Branche: SHK ▼] [Kategorie: Alle ▼] [________]     │
│                                                                  │
│  ┌─────────────────────────────────────────────────────────────┐│
│  │ 🏷️ Gruppe: Waschtisch-Armaturen                    [Bearbeiten]│
│  │ ───────────────────────────────────────────────────────────  ││
│  │ Begriffe: WT-Armatur, Waschtischarmatur, Lavabo-Armatur,    ││
│  │           Waschtisch-Mischbatterie, basin tap, basin mixer   ││
│  │ Kanonisch: Waschtisch-Armatur                                ││
│  │ Kategorie: Armaturen > Bad > Waschtisch                      ││
│  │ Treffer letzte 30 Tage: 1.245                                ││
│  └─────────────────────────────────────────────────────────────┘│
│                                                                  │
│  ┌─────────────────────────────────────────────────────────────┐│
│  │ 🏷️ Gruppe: Abkürzungen Montage                     [Bearbeiten]│
│  │ ───────────────────────────────────────────────────────────  ││
│  │ Begriffe: UP, Unterputz, Einbau, verdeckt, flush-mounted     ││
│  │ Kategorie: [Global - alle Kategorien]                        ││
│  │ Treffer letzte 30 Tage: 892                                  ││
│  └─────────────────────────────────────────────────────────────┘│
│                                                                  │
│  💡 SYNONYM-VORSCHLÄGE (aus Suchanfragen ohne Treffer)          │
│  ───────────────────────────────────────────────────────────────│
│  │ "WT Mischer" (45 Suchen) → Vorschlag: Zu "Waschtisch-Armatur"│
│  │ "Badhahn" (23 Suchen)    → Vorschlag: Zu "Waschtisch-Armatur"│
│  │ [Übernehmen] [Ignorieren] [Andere Gruppe]                    │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Automatische Synonym-Erkennung (ML-gestützt)

```typescript
// Aus erfolglosen Suchanfragen lernen
class SynonymSuggestionService {
  
  // Analysiert Suchanfragen ohne/mit wenig Treffern
  async analyzeMissedSearches(tenantId: string): Promise<SynonymSuggestion[]> {
    const missedQueries = await this.getMissedQueries(tenantId, { days: 30 });
    
    const suggestions: SynonymSuggestion[] = [];
    
    for (const query of missedQueries) {
      // 1. Embedding-Vergleich: Welches Produkt ist semantisch ähnlich?
      const similarProducts = await this.findSemanticallySimilar(query.term);
      
      // 2. Edit-Distance: Ist es ein Tippfehler?
      const typoCorrections = await this.findTypoCorrections(query.term);
      
      // 3. Substring-Match: Ist es eine Abkürzung?
      const abbreviationMatches = await this.findAbbreviationExpansions(query.term);
      
      if (similarProducts.length > 0) {
        suggestions.push({
          originalTerm: query.term,
          searchCount: query.count,
          suggestedSynonymGroup: similarProducts[0].synonymGroup,
          confidence: similarProducts[0].score,
          suggestionType: 'semantic'
        });
      }
    }
    
    return suggestions.filter(s => s.confidence > 0.7);
  }
  
  // Nutzt GPT/Claude für Branchenkontext
  async expandWithLLM(term: string, industry: string): Promise<string[]> {
    const prompt = `
      Du bist ein Experte für ${industry}-Fachbegriffe.
      Finde alle deutschen Synonyme, Abkürzungen und Fachbegriffe für: "${term}"
      
      Antworte als JSON-Array: ["Begriff1", "Begriff2", ...]
    `;
    
    const response = await this.llm.complete(prompt);
    return JSON.parse(response);
  }
}
```

#### Compound-Word Handling (Zerlegung deutscher Komposita)

```json
{
  "analysis": {
    "filter": {
      "german_decompounder": {
        "type": "hyphenation_decompounder",
        "word_list": [
          "waschtisch", "armatur", "mischer", "batterie",
          "heiz", "körper", "thermostat", "ventil",
          "rohr", "leitung", "schlauch", "fitting",
          "wasser", "abwasser", "brause", "dusche"
        ],
        "hyphenation_patterns_path": "analysis/de_hyph.xml",
        "min_word_size": 8,
        "min_subword_size": 4,
        "max_subword_size": 15,
        "only_longest_match": false
      }
    }
  }
}
```

**Beispiel-Zerlegung:**

| Kompositum | Zerlegung | Suchvorteil |
|------------|-----------|-------------|
| Waschtischarmatur | waschtisch + armatur | Findet auch "Armatur für Waschtisch" |
| Heizkreisverteiler | heiz + kreis + verteiler | Findet auch "Verteiler Heizkreis" |
| Druckminderer | druck + mindern | Findet auch "Druck Regler" |
| Rückschlagventil | rückschlag + ventil | Findet auch "Ventil Rückschlag" |

#### Query-Time vs. Index-Time Synonym-Expansion

```
┌─────────────────────────────────────────────────────────────────┐
│                    SYNONYM-EXPANSION STRATEGIEN                  │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  OPTION A: INDEX-TIME (Beim Indexieren)                         │
│  ───────────────────────────────────────                        │
│  Produkt: "WT-Armatur"                                          │
│  Indexiert als: [wt, armatur, waschtisch, lavabo, basin, ...]   │
│                                                                  │
│  ✓ Schnellere Suche (keine Expansion nötig)                     │
│  ✗ Index wird größer                                            │
│  ✗ Synonym-Änderung erfordert Re-Indexierung                    │
│                                                                  │
│  ════════════════════════════════════════════════════════════   │
│                                                                  │
│  OPTION B: QUERY-TIME (Bei Suchanfrage)                         │
│  ───────────────────────────────────────                        │
│  Query: "WT-Armatur"                                            │
│  Expandiert zu: "wt-armatur OR waschtisch-armatur OR lavabo"    │
│                                                                  │
│  ✓ Index bleibt klein                                           │
│  ✓ Synonym-Änderung sofort wirksam                              │
│  ✗ Langsamere Suche (Expansion bei jedem Query)                 │
│                                                                  │
│  ════════════════════════════════════════════════════════════   │
│                                                                  │
│  EMPFEHLUNG B2X: HYBRID                                         │
│  ─────────────────────────                                      │
│  • Häufige Abkürzungen: Index-Time (Performance)                │
│  • Tenant-spezifische Synonyme: Query-Time (Flexibilität)       │
│  • Produkt-Aliases: Dediziertes "search_keywords" Feld          │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Produkt-Aliases im Index

```typescript
// Beim BMEcat-Import: Alias-Feld befüllen
interface ProductSearchDocument {
  sku: string;
  name: string;
  description: string;
  
  // Dediziertes Feld für alle Suchbegriffe
  search_keywords: string[];  // Alle Varianten, Aliases, Abkürzungen
}

// Beispiel:
const product: ProductSearchDocument = {
  sku: "12345",
  name: "Grohe Eurosmart Waschtisch-Einhebelmischer",
  description: "Einhand-Waschtischbatterie, DN 15, S-Size...",
  
  search_keywords: [
    // Aus Produktname extrahiert
    "grohe", "eurosmart", "waschtisch", "einhebelmischer",
    
    // Abkürzungen
    "wt-armatur", "wt armatur", "ehm",
    
    // Synonyme
    "einhandmischer", "waschtischarmatur", "lavabo",
    
    // Technische Daten als Keywords
    "dn15", "1/2 zoll", "chrom",
    
    // Marketing-Keywords vom Hersteller
    "bestseller", "made in germany",
    
    // Frühere Produktnamen (Kompatibilität)
    "eurosmart 2023", "eurosmart new"
  ]
};
```

#### Performance-Optimierung für Synonyme

```json
{
  "settings": {
    "index": {
      "refresh_interval": "1s",
      "number_of_shards": 3,
      "number_of_replicas": 1,
      
      "analysis": {
        "filter": {
          "synonym_filter": {
            "type": "synonym_graph",
            "synonyms_path": "synonyms/de_shk.txt",
            "updateable": true,
            "lenient": true
          }
        }
      }
    },
    
    "similarity": {
      "custom_bm25": {
        "type": "BM25",
        "k1": 1.2,
        "b": 0.75
      }
    }
  }
}
```

**Synonym-Reload ohne Downtime:**

```bash
# Synonyme aktualisieren (Hot Reload)
POST /b2x_tenant_de/_reload_search_analyzers

# Neue Synonyme sind sofort aktiv!
```

#### Multilingual Synonym-Mapping

```typescript
// Cross-Language Synonyme für internationale Kataloge
const multilingualSynonyms = {
  "waschtischarmatur": {
    de: ["wt-armatur", "waschtisch-mischbatterie", "lavabo-armatur"],
    en: ["basin tap", "basin mixer", "lavatory faucet"],
    fr: ["robinet de lavabo", "mitigeur de lavabo"],
    nl: ["wastafelkraan", "mengkraan wastafel"],
    pl: ["bateria umywalkowa"]
  }
};

// Bei Suche: Auch englische Katalog-Begriffe finden
// Query "basin tap" → Findet auch deutsche Produkte mit "Waschtischarmatur"
```

#### Monitoring & Analytics für Synonyme

```typescript
// Dashboard: Synonym-Effektivität
interface SynonymAnalytics {
  synonym_group: string;
  
  // Nutzungsstatistiken
  queries_matched: number;          // Wie oft wurde Synonym getriggert
  queries_converted: number;        // Wie oft führte es zum Kauf
  
  // Quality Metrics
  avg_result_count: number;         // Durchschn. Ergebnisse
  zero_result_rate: number;         // Wie oft trotzdem 0 Treffer
  
  // Verbesserungsvorschläge
  suggested_additions: string[];    // Weitere Begriffe hinzufügen?
  potential_conflicts: string[];    // Überschneidung mit anderer Gruppe?
}
```

```
┌─────────────────────────────────────────────────────────────────┐
│  📊 SYNONYM ANALYTICS                                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Top 10 Synonym-Gruppen (30 Tage)                               │
│  ───────────────────────────────────────────────────────────────│
│  1. WT-Armatur / Waschtisch     │ 1.245 Matches │ 89% Conv.    │
│  2. Unterputz / UP               │   892 Matches │ 76% Conv.    │
│  3. Einhebelmischer / EHM        │   654 Matches │ 82% Conv.    │
│  ...                                                             │
│                                                                  │
│  ⚠️ PROBLEME ERKANNT                                            │
│  ───────────────────────────────────────────────────────────────│
│  • "Badhahn" (45 Suchen) → Kein Synonym definiert               │
│    [+ Zu "Waschtisch-Armatur" hinzufügen]                       │
│  • "HT Rohr DN 50" (23 Suchen) → 0 Treffer                      │
│    [Produkt fehlt] [Synonym prüfen]                             │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

### 2.7 Produktkonfiguratoren

> **⚠️ Komplexe B2B-Anforderung: Konfigurierbare Produkte mit Kompatibilitätsregeln!**

**Use Cases:**

| Szenario | Beschreibung | Beispiel |
|----------|--------------|----------|
| **Maschinen-Konfigurator** | Antrieb + Getriebe + Steuerung | Motor 5kW nur mit Getriebe Typ A oder B |
| **Möbel-Konfigurator** | Korpus + Türen + Griffe | Glasfront nur mit Softclose-Scharnieren |
| **PC-Konfigurator** | Gehäuse + CPU + RAM + GPU | ATX-Mainboard nur in ATX/E-ATX Gehäuse |
| **Werkzeug-Sets** | Basis + Module + Zubehör | Akku-Plattform bestimmt kompatible Geräte |

#### Shop-Settings: Konfigurator-Limits

```json
{
  "catalog": {
    "configurators": {
      "maxConfiguratorSteps": 10,              // Max. Konfigurationsschritte
      "maxOptionsPerStep": 100,                // Optionen pro Schritt
      "maxRulesPerConfigurator": 500,          // Kompatibilitätsregeln
      "maxDependenciesPerOption": 20,          // Abhängigkeiten pro Option
      "enablePriceCalculation": true,          // Live-Preisberechnung
      "enableStockValidation": true,           // Verfügbarkeitsprüfung
      "enableVisualization": false             // 3D/2D-Vorschau (Premium)
    }
  }
}
```

#### Domain Model

```csharp
/// <summary>
/// Produktkonfigurator - definiert konfigurierbare Produktzusammenstellungen
/// </summary>
public class ProductConfigurator
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; }              // "PC-Konfigurator", "Küchen-Planer"
    public string Slug { get; set; }
    public string Description { get; set; }
    
    // Basis-Produkt (optional - Konfigurator kann auch standalone sein)
    public Guid? BaseProductId { get; set; }
    
    // Konfigurationsschritte in Reihenfolge
    public List<ConfiguratorStep> Steps { get; set; } = [];
    
    // Globale Regeln (gelten für alle Schritte)
    public List<ConfiguratorRule> GlobalRules { get; set; } = [];
    
    // Preisberechnungsmodus
    public PricingMode PricingMode { get; set; }
    
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Konfigurationsschritt - eine Auswahlstufe im Konfigurator
/// </summary>
public class ConfiguratorStep
{
    public Guid Id { get; set; }
    public int SortOrder { get; set; }
    public string Name { get; set; }              // "Gehäuse wählen", "CPU auswählen"
    public string Description { get; set; }
    
    // Auswahlmodus
    public SelectionMode SelectionMode { get; set; }
    
    // Verfügbare Optionen in diesem Schritt
    public List<ConfiguratorOption> Options { get; set; } = [];
    
    // Schritt-spezifische Regeln
    public List<ConfiguratorRule> Rules { get; set; } = [];
    
    // Pflichtfeld?
    public bool IsRequired { get; set; }
    
    // Min/Max Auswahl (bei MultiSelect)
    public int MinSelections { get; set; } = 1;
    public int MaxSelections { get; set; } = 1;
}

public enum SelectionMode
{
    Single,           // Genau eine Option wählen
    MultiSelect,      // Mehrere Optionen wählbar
    Optional,         // Keine Auswahl erforderlich
    Quantity          // Option mit Mengenangabe
}

/// <summary>
/// Option innerhalb eines Konfigurationsschritts
/// </summary>
public class ConfiguratorOption
{
    public Guid Id { get; set; }
    public int SortOrder { get; set; }
    
    // Referenz auf Variante ODER Produkt
    public Guid? VariantId { get; set; }
    public Guid? ProductId { get; set; }
    
    // Oder freie Option ohne Produktbezug
    public string? CustomName { get; set; }
    public string? CustomDescription { get; set; }
    
    // Preis-Modifikator
    public PriceModifier? PriceModifier { get; set; }
    
    // Vorausgewählt?
    public bool IsDefault { get; set; }
    
    // Abhängigkeiten zu anderen Optionen
    public List<OptionDependency> Dependencies { get; set; } = [];
    
    // Ausschlüsse
    public List<Guid> ExcludesOptionIds { get; set; } = [];
    
    // Attribut-basierte Kompatibilität
    public List<CompatibilityConstraint> Constraints { get; set; } = [];
}

/// <summary>
/// Abhängigkeit zwischen Optionen
/// </summary>
public class OptionDependency
{
    public Guid RequiredOptionId { get; set; }    // Diese Option muss gewählt sein
    public DependencyType Type { get; set; }
    public string? ValidationMessage { get; set; } // "ATX-Mainboard benötigt ATX-Gehäuse"
}

public enum DependencyType
{
    Requires,         // Option A erfordert Option B
    RequiresAny,      // Option A erfordert eine aus [B, C, D]
    Excludes,         // Option A schließt Option B aus
    Recommends        // Option A empfiehlt Option B (Soft-Dependency)
}

/// <summary>
/// Attribut-basierte Kompatibilitätsregel
/// </summary>
public class CompatibilityConstraint
{
    public string AttributeCode { get; set; }     // "socket_type", "form_factor"
    public ConstraintOperator Operator { get; set; }
    public string Value { get; set; }
    public string? SourceStepId { get; set; }     // Attribut aus welchem Schritt?
}

public enum ConstraintOperator
{
    Equals,           // socket_type == "AM5"
    NotEquals,        // form_factor != "ITX"
    Contains,         // features contains "DDR5"
    GreaterThan,      // power_output > 500
    LessThan,         // tdp < 125
    InRange           // length between 200 and 350
}

/// <summary>
/// Preismodifikator für Optionen
/// </summary>
public class PriceModifier
{
    public PriceModifierType Type { get; set; }
    public decimal Value { get; set; }
}

public enum PriceModifierType
{
    Fixed,            // +49,99 €
    Percentage,       // +10%
    Replace,          // Ersetzt Basispreis
    PerUnit           // Pro Stück/Meter/etc.
}

public enum PricingMode
{
    Additive,         // Basispreis + Summe aller Optionen
    Replacement,      // Höchste Option bestimmt Preis
    Matrix,           // Preismatrix basierend auf Kombination
    Custom            // Benutzerdefinierte Formel
}
```

#### Konfigurator-Regeln (Rule Engine)

```csharp
/// <summary>
/// Regel im Konfigurator - kann Abhängigkeiten, Ausschlüsse und Validierungen definieren
/// </summary>
public class ConfiguratorRule
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Priority { get; set; }             // Höher = wird zuerst geprüft
    
    // Bedingung (WENN)
    public RuleCondition Condition { get; set; }
    
    // Aktionen (DANN)
    public List<RuleAction> Actions { get; set; } = [];
    
    // Fehlermeldung bei Regelverstoß
    public Dictionary<string, string> ErrorMessages { get; set; } = new();  // Lokalisiert
    
    public bool IsActive { get; set; }
}

public class RuleCondition
{
    public RuleConditionType Type { get; set; }
    public List<RuleCondition>? SubConditions { get; set; }  // Für AND/OR
    
    // Für einfache Bedingungen
    public Guid? StepId { get; set; }
    public Guid? OptionId { get; set; }
    public string? AttributeCode { get; set; }
    public ConstraintOperator? Operator { get; set; }
    public string? Value { get; set; }
}

public enum RuleConditionType
{
    And,              // Alle SubConditions müssen wahr sein
    Or,               // Mindestens eine SubCondition muss wahr sein
    Not,              // SubCondition muss falsch sein
    OptionSelected,   // Bestimmte Option ist gewählt
    StepCompleted,    // Schritt wurde abgeschlossen
    AttributeEquals,  // Attribut hat bestimmten Wert
    AttributeCompare  // Attribut-Vergleich (>, <, etc.)
}

public class RuleAction
{
    public RuleActionType Type { get; set; }
    public Guid? TargetStepId { get; set; }
    public Guid? TargetOptionId { get; set; }
    public decimal? PriceModifier { get; set; }
    public string? Message { get; set; }
}

public enum RuleActionType
{
    HideOption,       // Option ausblenden
    ShowOption,       // Option einblenden
    DisableOption,    // Option deaktivieren (ausgegraut)
    EnableOption,     // Option aktivieren
    SelectOption,     // Option automatisch wählen
    DeselectOption,   // Option automatisch abwählen
    SetPrice,         // Preis setzen/überschreiben
    AddDiscount,      // Rabatt hinzufügen
    ShowWarning,      // Warnung anzeigen
    ShowError,        // Fehler anzeigen (blockiert)
    SkipStep,         // Schritt überspringen
    RequireStep       // Schritt erforderlich machen
}
```

#### Elasticsearch-Mapping für Konfiguratoren

```json
{
  "mappings": {
    "properties": {
      "doc_type": { "type": "keyword" },  // "configurator"
      "id": { "type": "keyword" },
      "tenant_id": { "type": "keyword" },
      "name": { "type": "text", "analyzer": "german" },
      "name_suggest": { "type": "search_as_you_type" },
      "slug": { "type": "keyword" },
      "description": { "type": "text" },
      
      // Verknüpftes Basisprodukt
      "base_product_id": { "type": "keyword" },
      "base_product_name": { "type": "text" },
      
      // Konfigurationsschritte (denormalisiert für Suche)
      "steps": {
        "type": "nested",
        "properties": {
          "step_id": { "type": "keyword" },
          "step_name": { "type": "text" },
          "sort_order": { "type": "integer" },
          "is_required": { "type": "boolean" },
          "option_count": { "type": "integer" }
        }
      },
      
      // Alle enthaltenen Varianten/Produkte (für Suche)
      "included_variant_ids": { "type": "keyword" },
      "included_product_ids": { "type": "keyword" },
      "included_brand_ids": { "type": "keyword" },
      "included_category_ids": { "type": "keyword" },
      
      // Preisbereich der Konfiguration
      "min_configuration_price": { "type": "scaled_float", "scaling_factor": 100 },
      "max_configuration_price": { "type": "scaled_float", "scaling_factor": 100 },
      
      // Metadaten
      "step_count": { "type": "integer" },
      "total_option_count": { "type": "integer" },
      "rule_count": { "type": "integer" },
      
      "is_active": { "type": "boolean" },
      "created_at": { "type": "date" },
      "indexed_at": { "type": "date" },  // Statt updated_at
      
      // Embedding für semantische Suche
      "embedding": {
        "type": "dense_vector",
        "dims": 768,
        "index": true,
        "similarity": "cosine"
      }
    }
  }
}
```

#### Konfigurator-Validierungsservice

```csharp
public class ConfiguratorValidationService
{
    private readonly IRuleEngine _ruleEngine;
    private readonly IVariantService _variantService;
    
    /// <summary>
    /// Validiert eine Konfigurationsauswahl in Echtzeit
    /// </summary>
    public async Task<ConfigurationValidationResult> ValidateSelectionAsync(
        Guid configuratorId,
        Dictionary<Guid, List<Guid>> selections,  // StepId -> OptionIds
        CancellationToken ct = default)
    {
        var configurator = await _repository.GetByIdAsync(configuratorId, ct);
        var result = new ConfigurationValidationResult();
        
        // 1. Pflichtfelder prüfen
        foreach (var step in configurator.Steps.Where(s => s.IsRequired))
        {
            if (!selections.TryGetValue(step.Id, out var selected) || !selected.Any())
            {
                result.Errors.Add(new ValidationError
                {
                    StepId = step.Id,
                    Code = "REQUIRED_STEP",
                    Message = $"Bitte wählen Sie eine Option für '{step.Name}'"
                });
            }
        }
        
        // 2. Auswahlgrenzen prüfen
        foreach (var (stepId, optionIds) in selections)
        {
            var step = configurator.Steps.First(s => s.Id == stepId);
            
            if (optionIds.Count < step.MinSelections)
            {
                result.Errors.Add(new ValidationError
                {
                    StepId = stepId,
                    Code = "MIN_SELECTIONS",
                    Message = $"Mindestens {step.MinSelections} Option(en) erforderlich"
                });
            }
            
            if (optionIds.Count > step.MaxSelections)
            {
                result.Errors.Add(new ValidationError
                {
                    StepId = stepId,
                    Code = "MAX_SELECTIONS",
                    Message = $"Maximal {step.MaxSelections} Option(en) erlaubt"
                });
            }
        }
        
        // 3. Abhängigkeiten prüfen
        foreach (var (stepId, optionIds) in selections)
        {
            foreach (var optionId in optionIds)
            {
                var option = GetOption(configurator, stepId, optionId);
                
                foreach (var dependency in option.Dependencies)
                {
                    var isResolved = IsDependencyResolved(selections, dependency);
                    
                    if (!isResolved && dependency.Type == DependencyType.Requires)
                    {
                        result.Errors.Add(new ValidationError
                        {
                            StepId = stepId,
                            OptionId = optionId,
                            Code = "UNMET_DEPENDENCY",
                            Message = dependency.ValidationMessage 
                                ?? "Erforderliche Abhängigkeit nicht erfüllt"
                        });
                    }
                }
            }
        }
        
        // 4. Ausschlüsse prüfen
        var allSelectedOptions = selections.Values.SelectMany(x => x).ToHashSet();
        
        foreach (var (stepId, optionIds) in selections)
        {
            foreach (var optionId in optionIds)
            {
                var option = GetOption(configurator, stepId, optionId);
                
                var conflicts = option.ExcludesOptionIds
                    .Where(excluded => allSelectedOptions.Contains(excluded))
                    .ToList();
                
                if (conflicts.Any())
                {
                    result.Errors.Add(new ValidationError
                    {
                        StepId = stepId,
                        OptionId = optionId,
                        Code = "EXCLUSION_CONFLICT",
                        Message = "Diese Option ist nicht mit anderen gewählten Optionen kompatibel",
                        ConflictingOptionIds = conflicts
                    });
                }
            }
        }
        
        // 5. Attribut-Constraints prüfen
        result.Errors.AddRange(await ValidateAttributeConstraintsAsync(
            configurator, selections, ct));
        
        // 6. Globale Regeln ausführen
        var ruleResults = await _ruleEngine.EvaluateAsync(
            configurator.GlobalRules, 
            selections, 
            ct);
        
        result.Errors.AddRange(ruleResults.Errors);
        result.Warnings.AddRange(ruleResults.Warnings);
        result.AppliedActions.AddRange(ruleResults.Actions);
        
        // 7. Verfügbarkeit prüfen
        if (result.IsValid)
        {
            result.AvailabilityResult = await CheckAvailabilityAsync(
                configurator, selections, ct);
        }
        
        // 8. Preis berechnen
        if (result.IsValid)
        {
            result.CalculatedPrice = await CalculatePriceAsync(
                configurator, selections, ct);
        }
        
        return result;
    }
}

public class ConfigurationValidationResult
{
    public bool IsValid => !Errors.Any();
    public List<ValidationError> Errors { get; set; } = [];
    public List<ValidationWarning> Warnings { get; set; } = [];
    public List<RuleAction> AppliedActions { get; set; } = [];
    public AvailabilityResult? AvailabilityResult { get; set; }
    public PriceCalculationResult? CalculatedPrice { get; set; }
}
```

#### Frontend: Konfigurator-State

```typescript
interface ConfiguratorState {
  configuratorId: string;
  currentStepIndex: number;
  selections: Map<string, string[]>;  // stepId -> optionIds[]
  
  // Berechnete Werte
  totalPrice: number;
  priceBreakdown: PriceBreakdownItem[];
  
  // Validierung
  validationResult: ValidationResult | null;
  availableOptions: Map<string, OptionAvailability>;  // optionId -> availability
  
  // UI-State
  isLoading: boolean;
  isSaving: boolean;
}

interface OptionAvailability {
  isAvailable: boolean;
  isEnabled: boolean;           // Nicht durch Regeln deaktiviert
  isVisible: boolean;           // Nicht durch Regeln versteckt
  disabledReason?: string;
  recommendation?: string;      // "Empfohlen für Ihre Auswahl"
  priceImpact: number;          // Preisänderung bei Auswahl
  stockLevel?: 'in_stock' | 'low_stock' | 'out_of_stock';
}

// Pinia Store für Konfigurator
export const useConfiguratorStore = defineStore('configurator', {
  state: (): ConfiguratorState => ({
    configuratorId: '',
    currentStepIndex: 0,
    selections: new Map(),
    totalPrice: 0,
    priceBreakdown: [],
    validationResult: null,
    availableOptions: new Map(),
    isLoading: false,
    isSaving: false
  }),
  
  actions: {
    async selectOption(stepId: string, optionId: string) {
      // Optimistisches Update
      const step = this.getStep(stepId);
      
      if (step.selectionMode === 'Single') {
        this.selections.set(stepId, [optionId]);
      } else {
        const current = this.selections.get(stepId) ?? [];
        if (!current.includes(optionId)) {
          this.selections.set(stepId, [...current, optionId]);
        }
      }
      
      // Serverseitige Validierung
      await this.validateConfiguration();
    },
    
    async validateConfiguration() {
      this.isLoading = true;
      
      try {
        const result = await $fetch('/api/configurator/validate', {
          method: 'POST',
          body: {
            configuratorId: this.configuratorId,
            selections: Object.fromEntries(this.selections)
          }
        });
        
        this.validationResult = result;
        this.totalPrice = result.calculatedPrice?.total ?? 0;
        this.priceBreakdown = result.calculatedPrice?.breakdown ?? [];
        
        // Option-Verfügbarkeiten aktualisieren
        this.updateOptionAvailabilities(result.appliedActions);
        
      } finally {
        this.isLoading = false;
      }
    },
    
    async addToCart() {
      if (!this.validationResult?.isValid) return;
      
      this.isSaving = true;
      
      try {
        await $fetch('/api/cart/add-configuration', {
          method: 'POST',
          body: {
            configuratorId: this.configuratorId,
            selections: Object.fromEntries(this.selections),
            calculatedPrice: this.totalPrice
          }
        });
        
        navigateTo('/cart');
        
      } finally {
        this.isSaving = false;
      }
    }
  }
});
```

#### Beispiel: PC-Konfigurator

```json
{
  "id": "cfg-pc-builder",
  "name": "PC-Konfigurator",
  "steps": [
    {
      "id": "step-case",
      "sort_order": 1,
      "name": "Gehäuse wählen",
      "selection_mode": "Single",
      "is_required": true,
      "options": [
        {
          "id": "opt-case-atx",
          "variant_id": "var-case-atx-1",
          "constraints": [
            { "attribute_code": "form_factor", "operator": "Equals", "value": "ATX" }
          ]
        },
        {
          "id": "opt-case-itx",
          "variant_id": "var-case-itx-1",
          "constraints": [
            { "attribute_code": "form_factor", "operator": "Equals", "value": "ITX" }
          ]
        }
      ]
    },
    {
      "id": "step-mainboard",
      "sort_order": 2,
      "name": "Mainboard wählen",
      "selection_mode": "Single",
      "is_required": true,
      "options": [
        {
          "id": "opt-mb-atx-am5",
          "variant_id": "var-mb-atx-am5",
          "constraints": [
            { 
              "attribute_code": "form_factor", 
              "operator": "Equals", 
              "value": "ATX",
              "source_step_id": "step-case"  // Muss zu Gehäuse passen!
            }
          ],
          "dependencies": [
            {
              "type": "RequiresAny",
              "required_option_ids": ["opt-case-atx", "opt-case-eatx"],
              "validation_message": "ATX-Mainboard passt nicht in ITX-Gehäuse"
            }
          ]
        }
      ]
    },
    {
      "id": "step-cpu",
      "sort_order": 3,
      "name": "Prozessor wählen",
      "selection_mode": "Single",
      "is_required": true,
      "options": [
        {
          "id": "opt-cpu-amd-7800x3d",
          "variant_id": "var-cpu-7800x3d",
          "constraints": [
            {
              "attribute_code": "socket_type",
              "operator": "Equals",
              "value": "AM5",
              "source_step_id": "step-mainboard"  // Socket vom Mainboard
            }
          ]
        }
      ]
    }
  ],
  "global_rules": [
    {
      "id": "rule-tdp-check",
      "name": "TDP-Prüfung",
      "condition": {
        "type": "AttributeCompare",
        "attribute_code": "tdp",
        "source_step_id": "step-cpu",
        "operator": "GreaterThan",
        "value": "125"
      },
      "actions": [
        {
          "type": "ShowWarning",
          "message": "CPU mit hohem TDP - leistungsstarke Kühlung empfohlen"
        }
      ]
    }
  ]
}
```

---

### 2.8 Visual Search / Image Index (Separater Index)

> **🎁 PREMIUM FEATURE (Phase 2)** - Nicht MVP-kritisch, aber hoher Differenzierungswert

> **⚠️ Bildsuche erfordert separaten Index für optimale Performance!**

#### Warum separater Image-Index?

| Aspekt | Produkt-Index | Image-Index |
|--------|---------------|-------------|
| **Embedding-Größe** | 768 dim (Text) | 512-1024 dim (CLIP/ViT) |
| **Speicher/Doc** | ~3 KB | ~8-16 KB (nur Embeddings) |
| **Update-Frequenz** | Häufig (Preise, Bestand) | Selten (Bilder stabil) |
| **Modell** | Text-Encoder | Vision-Encoder (OpenAI CLIP) |
| **Skalierung** | Nach Varianten | Nach Media-Assets |
| **Anzahl Docs** | 1 pro Variante | N pro Variante (mehrere Bilder) |

**Empfehlung: Separater `b2x_{tenant}_images` Index!**

#### Image Index Mapping

```json
{
  "settings": {
    "index": {
      "number_of_shards": 1,
      "number_of_replicas": 1,
      "knn": true
    }
  },
  "mappings": {
    "properties": {
      "doc_type": { "type": "keyword" },           // "product_image"
      "image_id": { "type": "keyword" },           // Unique Image ID
      "media_id": { "type": "keyword" },           // Reference zu Media-Asset
      
      // Referenz zum Hauptindex
      "variant_id": { "type": "keyword" },         // FK zur Variante
      "variant_sku": { "type": "keyword" },        // SKU für schnellen Lookup
      "product_id": { "type": "keyword" },         // FK zum Produkt
      
      // Image-Metadaten (für Filter)
      "image_type": { "type": "keyword" },         // "main" | "gallery" | "360" | "lifestyle"
      "image_position": { "type": "integer" },     // Reihenfolge (1 = Hauptbild)
      "aspect_ratio": { "type": "keyword" },       // "1:1" | "4:3" | "16:9"
      "background_type": { "type": "keyword" },    // "white" | "transparent" | "lifestyle"
      
      // CLIP Embedding für Visual Search
      "clip_embedding": {
        "type": "dense_vector",
        "dims": 512,                               // CLIP ViT-B/32 = 512 dim
        "index": true,
        "similarity": "cosine"
      },
      
      // Optional: Höherdimensionale Embeddings für Präzision
      "clip_large_embedding": {
        "type": "dense_vector",
        "dims": 768,                               // CLIP ViT-L/14 = 768 dim
        "index": true,
        "similarity": "cosine"
      },
      
      // Bildanalyse-Features (optional, für erweiterte Filter)
      "dominant_colors": { "type": "keyword" },    // ["#1976D2", "#FFFFFF", "#212121"]
      "detected_objects": { "type": "keyword" },   // ["drill", "tool", "hand"]
      "scene_type": { "type": "keyword" },         // "product_shot" | "in_use" | "detail"
      
      // Produkt-Kontext (denormalisiert für Ergebnis-Anzeige)
      "product_name": { "type": "text" },
      "brand_name": { "type": "keyword" },
      "category_path": { "type": "keyword" },
      "price": { "type": "scaled_float", "scaling_factor": 100 },
      "is_in_stock": { "type": "boolean" },
      
      // Zeitstempel
      "created_at": { "type": "date" },
      "indexed_at": { "type": "date" }
    }
  }
}
```

#### Visual Search Use Cases

**1. Image-to-Image Search (Bildsuche mit Upload)**

```typescript
// User lädt Foto hoch → CLIP Embedding → kNN Search
async function searchByImage(imageFile: File): Promise<SearchResult[]> {
  // 1. Bild an CLIP-Service senden
  const embedding = await clipService.encodeImage(imageFile);
  
  // 2. kNN-Suche im Image-Index
  const query = {
    knn: {
      field: "clip_embedding",
      query_vector: embedding,
      k: 20,
      num_candidates: 100
    },
    // Nur Hauptbilder, auf Lager
    filter: {
      bool: {
        must: [
          { term: { "image_type": "main" } },
          { term: { "is_in_stock": true } }
        ]
      }
    }
  };
  
  // 3. Deduplizieren (nur 1 Bild pro Produkt)
  return deduplicateByProductId(results);
}
```

**2. Text-to-Image Search (Text beschreibt gewünschtes Bild)**

```typescript
// "roter Akkuschrauber" → CLIP Text-Embedding → Bilder finden
async function searchByDescription(text: string): Promise<SearchResult[]> {
  // CLIP kann Text UND Bilder in denselben Vektorraum embedden!
  const textEmbedding = await clipService.encodeText(text);
  
  const query = {
    knn: {
      field: "clip_embedding",
      query_vector: textEmbedding,
      k: 20,
      num_candidates: 100
    }
  };
  
  return searchImageIndex(query);
}
```

**3. "Mehr wie dieses" (Similar Products)**

```typescript
// Produkt X → Finde visuell ähnliche Produkte
async function findSimilarProducts(variantId: string): Promise<SearchResult[]> {
  // 1. Hole CLIP-Embedding des Hauptbilds
  const mainImage = await getMainImage(variantId);
  
  // 2. kNN-Suche, aber eigenes Produkt ausschließen
  const query = {
    knn: {
      field: "clip_embedding",
      query_vector: mainImage.clip_embedding,
      k: 12,
      num_candidates: 50
    },
    filter: {
      bool: {
        must_not: [
          { term: { "product_id": mainImage.product_id } }
        ]
      }
    }
  };
  
  return searchImageIndex(query);
}
```

#### CLIP Embedding Pipeline

```typescript
// Backend-Service für CLIP-Embeddings
class ClipEmbeddingService {
  private readonly modelEndpoint: string; // OpenAI CLIP oder Self-hosted
  
  // Bild → Embedding (512 dim für ViT-B/32)
  async encodeImage(image: Buffer | string): Promise<number[]> {
    // Option A: OpenAI CLIP API
    // Option B: Self-hosted mit HuggingFace Transformers
    // Option C: Azure Cognitive Services (Florence)
    const response = await fetch(this.modelEndpoint, {
      method: 'POST',
      body: JSON.stringify({ image: base64Encode(image) })
    });
    return response.json().embedding;
  }
  
  // Text → Embedding (gleicher Vektorraum wie Bilder!)
  async encodeText(text: string): Promise<number[]> {
    const response = await fetch(this.modelEndpoint, {
      method: 'POST',
      body: JSON.stringify({ text })
    });
    return response.json().embedding;
  }
}
```

#### Indexierung von Produkt-Bildern

```typescript
// Bei Bild-Upload: CLIP-Embedding generieren und indexieren
async function indexProductImage(
  mediaAsset: MediaAsset, 
  variant: Variant
): Promise<void> {
  // 1. CLIP-Embedding generieren
  const embedding = await clipService.encodeImage(mediaAsset.url);
  
  // 2. Dominante Farben extrahieren (optional)
  const colors = await colorAnalysisService.extractColors(mediaAsset.url);
  
  // 3. Image-Dokument erstellen
  const imageDoc = {
    doc_type: "product_image",
    image_id: `img-${mediaAsset.id}`,
    media_id: mediaAsset.id,
    variant_id: variant.id,
    variant_sku: variant.sku,
    product_id: variant.productId,
    image_type: mediaAsset.type, // "main" | "gallery"
    image_position: mediaAsset.position,
    clip_embedding: embedding,
    dominant_colors: colors,
    // Denormalisierte Produktdaten
    product_name: variant.name,
    brand_name: variant.brandName,
    category_path: variant.categoryPath,
    price: variant.defaultPrice,
    is_in_stock: variant.isInStock,
    indexed_at: new Date()
  };
  
  // 4. In Image-Index speichern
  await elasticClient.index({
    index: `b2x_${tenantId}_images`,
    id: imageDoc.image_id,
    body: imageDoc
  });
}
```

#### Architektur: Trennung der Indizes

```
┌─────────────────────────────────────────────────────────────────┐
│                        FRONTEND                                  │
├─────────────────────────────────────────────────────────────────┤
│  Text-Suche    │  Bild-Upload     │  "Mehr wie dieses"          │
│  "bosch bohrer"│  📷 Foto         │  Similar Products           │
└────────┬───────┴────────┬─────────┴──────────┬──────────────────┘
         │                │                    │
         ▼                ▼                    ▼
┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐
│  Search API     │ │  CLIP Service   │ │  Product API    │
│  (Elasticsearch)│ │  (Embeddings)   │ │  (Lookup)       │
└────────┬────────┘ └────────┬────────┘ └────────┬────────┘
         │                   │                   │
         ▼                   ▼                   ▼
┌─────────────────────────────────────────────────────────────────┐
│                      ELASTICSEARCH                               │
├──────────────────────┬──────────────────────────────────────────┤
│  b2x_{tenant}_{lang} │  b2x_{tenant}_images                     │
│  ─────────────────── │  ─────────────────────                   │
│  • Text-Embeddings   │  • CLIP-Embeddings (512 dim)             │
│  • Preise, Bestand   │  • Referenz zu Variante                  │
│  • Attribute, Tags   │  • Denormalisierte Produktdaten          │
│  • Kategorien        │  • Dominante Farben                      │
│  768 dim             │  • 1-N Bilder pro Produkt                │
│  ~3 KB/Doc           │  ~10 KB/Doc                              │
│  100.000 Docs        │  300.000 Docs (Ø 3 Bilder/Produkt)       │
└──────────────────────┴──────────────────────────────────────────┘
```

#### Performance-Optimierung

**Separate HNSW-Konfiguration für Image-Index:**

```json
{
  "settings": {
    "index": {
      "knn": true,
      "knn.algo_param.ef_construction": 256,  // Höher für bessere Recall
      "knn.algo_param.m": 32                  // Mehr Connections
    }
  }
}
```

**Caching von CLIP-Embeddings:**

```typescript
// Redis-Cache für bereits berechnete Embeddings
const cacheKey = `clip:${sha256(imageUrl)}`;
let embedding = await redis.get(cacheKey);

if (!embedding) {
  embedding = await clipService.encodeImage(imageUrl);
  await redis.set(cacheKey, embedding, { EX: 86400 * 30 }); // 30 Tage
}
```

**Batch-Indexierung bei Katalog-Import:**

```typescript
// BMEcat-Import mit Bildern → Batch-CLIP-Embedding
async function indexCatalogImages(catalog: BmecatCatalog): Promise<void> {
  const batches = chunk(catalog.products, 100);
  
  for (const batch of batches) {
    // Parallel CLIP-Embedding für alle Bilder im Batch
    const embeddings = await Promise.all(
      batch.flatMap(p => p.images.map(img => 
        clipService.encodeImage(img.url)
      ))
    );
    
    // Bulk-Index in Elasticsearch
    await elasticClient.bulk({
      body: buildBulkBody(batch, embeddings)
    });
  }
}
```

#### Kosten-Betrachtung

| Aspekt | Self-Hosted CLIP | OpenAI CLIP API |
|--------|------------------|-----------------|
| **Latenz** | ~50ms | ~200ms |
| **Kosten/1000 Bilder** | ~$0 (Server) | ~$0.10 |
| **GPU-Anforderung** | 8GB VRAM | Keine |
| **Wartung** | Hoch | Niedrig |

**Empfehlung für B2X:**
- **Development**: OpenAI CLIP API (einfach)
- **Production**: Self-hosted CLIP auf GPU-Server (günstiger bei Volumen)

---

### 2.9 Real-Time Camera Search (Live Visual Scanner)

> **� PREMIUM FEATURE (Phase 3)** - Zusatzmodul für Enterprise-Kunden

> **�🎯 Killer-Feature für B2B: Handwerker filmt defektes Teil → findet sofort Ersatzteil!**

#### Konzept: Visual Product Scanner

```
┌─────────────────────────────────────────────────────────────────┐
│                    📱 MOBILE APP / PWA                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│   ┌─────────────────────────────────────────────────────────┐   │
│   │                                                         │   │
│   │              📷 LIVE KAMERA-VORSCHAU                    │   │
│   │                                                         │   │
│   │         ┌───────────────────────────┐                   │   │
│   │         │    🔍 Erkanntes Objekt    │                   │   │
│   │         │    ═══════════════════    │                   │   │
│   │         │                           │                   │   │
│   │         │  ▸ Bosch HSS-G 8mm        │  ← Live-Ergebnis │   │
│   │         │    ★★★★★ €8.49            │                   │   │
│   │         │    ✓ Auf Lager            │                   │   │
│   │         │                           │                   │   │
│   │         │  ▸ Heller HSS-R 8mm       │                   │   │
│   │         │    ★★★★☆ €6.99            │                   │   │
│   │         │                           │                   │   │
│   │         └───────────────────────────┘                   │   │
│   │                                                         │   │
│   └─────────────────────────────────────────────────────────┘   │
│                                                                  │
│   [ 📸 Foto aufnehmen ]  [ 🛒 Direkt bestellen ]                │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Technische Architektur

```
┌────────────────────────────────────────────────────────────────────┐
│                        MOBILE DEVICE                                │
├────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────────────────┐ │
│  │ Camera API  │───▶│ Frame       │───▶│ WebSocket / HTTP Stream │ │
│  │ (MediaStream)│   │ Extraction  │    │ (alle 500ms)            │ │
│  └─────────────┘    │ (Canvas)    │    └───────────┬─────────────┘ │
│                     └─────────────┘                │               │
│                                                    │               │
│  ┌─────────────────────────────────────────────────┴─────────────┐ │
│  │  Local State: currentResults[], isSearching, confidence      │ │
│  └───────────────────────────────────────────────────────────────┘ │
└────────────────────────────────────────────────────────────────────┘
                                │
                                │ JPEG/WebP (komprimiert)
                                │ ~50-100KB pro Frame
                                ▼
┌────────────────────────────────────────────────────────────────────┐
│                          BACKEND                                    │
├────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────────────────┐ │
│  │ API Gateway │───▶│ CLIP Service│───▶│ Elasticsearch kNN      │ │
│  │ (Rate Limit)│    │ (GPU/CPU)   │    │ Image Index            │ │
│  └─────────────┘    └─────────────┘    └───────────┬─────────────┘ │
│                           │                        │               │
│                           │ Embedding (512 dim)    │ Top-5 Results │
│                           ▼                        ▼               │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  Response: { products: [...], confidence: 0.87 }            │   │
│  └─────────────────────────────────────────────────────────────┘   │
└────────────────────────────────────────────────────────────────────┘
```

#### Vue.js/Nuxt Komponente: CameraScanner

```vue
<template>
  <div class="camera-scanner">
    <!-- Live Camera Preview -->
    <div class="camera-container">
      <video 
        ref="videoRef" 
        autoplay 
        playsinline 
        muted
        class="camera-preview"
      />
      
      <!-- Scanning Overlay -->
      <div v-if="isScanning" class="scan-overlay">
        <div class="scan-frame" :class="{ 'found': hasResults }">
          <div class="corner tl" />
          <div class="corner tr" />
          <div class="corner bl" />
          <div class="corner br" />
        </div>
        <p class="scan-hint">
          {{ hasResults ? 'Produkt erkannt!' : 'Objekt in den Rahmen halten...' }}
        </p>
      </div>
      
      <!-- Live Results Overlay -->
      <Transition name="slide-up">
        <div v-if="results.length > 0" class="results-overlay">
          <ProductCard 
            v-for="product in results.slice(0, 3)" 
            :key="product.id"
            :product="product"
            :confidence="product.confidence"
            compact
            @click="navigateToProduct(product)"
          />
        </div>
      </Transition>
    </div>
    
    <!-- Controls -->
    <div class="controls">
      <button @click="captureAndSearch" :disabled="isProcessing">
        <Icon name="camera" /> Foto aufnehmen
      </button>
      <button v-if="topResult" @click="addToCart(topResult)">
        <Icon name="cart" /> {{ topResult.name }} bestellen
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import { useCameraScanner } from '~/composables/useCameraScanner';

const videoRef = ref<HTMLVideoElement>();
const { 
  isScanning, 
  isProcessing, 
  results, 
  hasResults,
  topResult,
  startScanning, 
  stopScanning,
  captureAndSearch 
} = useCameraScanner(videoRef);

onMounted(() => {
  startScanning();
});

onUnmounted(() => {
  stopScanning();
});
</script>
```

#### Composable: useCameraScanner

```typescript
// composables/useCameraScanner.ts
import { ref, computed, type Ref } from 'vue';
import { useVisualSearchApi } from '~/composables/useVisualSearchApi';

export function useCameraScanner(videoRef: Ref<HTMLVideoElement | undefined>) {
  const isScanning = ref(false);
  const isProcessing = ref(false);
  const results = ref<VisualSearchResult[]>([]);
  const stream = ref<MediaStream | null>(null);
  const scanInterval = ref<number | null>(null);
  
  const { searchByImage } = useVisualSearchApi();
  
  // Kamera starten
  async function startScanning() {
    try {
      // Rückkamera bevorzugen (Handy)
      stream.value = await navigator.mediaDevices.getUserMedia({
        video: {
          facingMode: 'environment',  // Rückkamera
          width: { ideal: 1280 },
          height: { ideal: 720 }
        }
      });
      
      if (videoRef.value) {
        videoRef.value.srcObject = stream.value;
      }
      
      isScanning.value = true;
      
      // Kontinuierliche Suche alle 500ms
      scanInterval.value = window.setInterval(scanFrame, 500);
      
    } catch (error) {
      console.error('Kamera-Zugriff verweigert:', error);
      throw new Error('Kamera-Berechtigung erforderlich');
    }
  }
  
  // Einzelnes Frame analysieren
  async function scanFrame() {
    if (!videoRef.value || isProcessing.value) return;
    
    isProcessing.value = true;
    
    try {
      // Frame aus Video extrahieren
      const canvas = document.createElement('canvas');
      canvas.width = 640;  // Reduzierte Auflösung für Speed
      canvas.height = 480;
      
      const ctx = canvas.getContext('2d')!;
      ctx.drawImage(videoRef.value, 0, 0, canvas.width, canvas.height);
      
      // Als JPEG komprimieren (Qualität 0.7 = ~50KB)
      const imageBlob = await new Promise<Blob>((resolve) => {
        canvas.toBlob(blob => resolve(blob!), 'image/jpeg', 0.7);
      });
      
      // API-Suche
      const searchResults = await searchByImage(imageBlob);
      
      // Nur aktualisieren wenn Confidence > 0.5
      if (searchResults.length > 0 && searchResults[0].confidence > 0.5) {
        results.value = searchResults;
        
        // Haptic Feedback bei Fund (Mobile)
        if ('vibrate' in navigator) {
          navigator.vibrate(50);
        }
      }
      
    } finally {
      isProcessing.value = false;
    }
  }
  
  // Manuelles Foto aufnehmen (höhere Qualität)
  async function captureAndSearch() {
    if (!videoRef.value) return;
    
    isProcessing.value = true;
    
    try {
      const canvas = document.createElement('canvas');
      canvas.width = 1280;  // Volle Auflösung
      canvas.height = 720;
      
      const ctx = canvas.getContext('2d')!;
      ctx.drawImage(videoRef.value, 0, 0, canvas.width, canvas.height);
      
      const imageBlob = await new Promise<Blob>((resolve) => {
        canvas.toBlob(blob => resolve(blob!), 'image/jpeg', 0.9);
      });
      
      results.value = await searchByImage(imageBlob);
      
    } finally {
      isProcessing.value = false;
    }
  }
  
  // Kamera stoppen
  function stopScanning() {
    if (scanInterval.value) {
      clearInterval(scanInterval.value);
    }
    if (stream.value) {
      stream.value.getTracks().forEach(track => track.stop());
    }
    isScanning.value = false;
  }
  
  return {
    isScanning,
    isProcessing,
    results,
    hasResults: computed(() => results.value.length > 0),
    topResult: computed(() => results.value[0] ?? null),
    startScanning,
    stopScanning,
    captureAndSearch
  };
}
```

#### Backend API: Visual Search Endpoint

```csharp
// POST /api/visual-search/scan
[ApiController]
[Route("api/visual-search")]
public class VisualSearchController : ControllerBase
{
    private readonly IClipEmbeddingService _clipService;
    private readonly IElasticClient _elastic;
    private readonly ILogger<VisualSearchController> _logger;
    
    [HttpPost("scan")]
    [RequestSizeLimit(5_000_000)] // Max 5MB
    public async Task<ActionResult<VisualSearchResponse>> ScanImage(
        IFormFile image,
        [FromQuery] int limit = 5,
        CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        // 1. Bild validieren
        if (image.Length == 0 || image.Length > 5_000_000)
            return BadRequest("Bild muss zwischen 1 Byte und 5MB sein");
        
        using var imageStream = image.OpenReadStream();
        using var ms = new MemoryStream();
        await imageStream.CopyToAsync(ms, ct);
        var imageBytes = ms.ToArray();
        
        // 2. CLIP Embedding generieren (~50ms)
        var embedding = await _clipService.EncodeImageAsync(imageBytes, ct);
        
        // 3. kNN-Suche im Image-Index (~20ms)
        var searchResponse = await _elastic.SearchAsync<ImageDocument>(s => s
            .Index($"b2x_{TenantId}_images")
            .Size(limit)
            .Query(q => q
                .Knn(k => k
                    .Field(f => f.ClipEmbedding)
                    .QueryVector(embedding)
                    .K(limit)
                    .NumCandidates(100)
                )
            )
            .Source(src => src
                .Includes(i => i
                    .Fields(
                        f => f.VariantId,
                        f => f.VariantSku,
                        f => f.ProductName,
                        f => f.BrandName,
                        f => f.Price,
                        f => f.IsInStock,
                        f => f.ImageUrl
                    )
                )
            ),
            ct
        );
        
        stopwatch.Stop();
        
        // 4. Ergebnisse mit Confidence Score
        var results = searchResponse.Hits.Select(hit => new VisualSearchResult
        {
            VariantId = hit.Source.VariantId,
            Sku = hit.Source.VariantSku,
            Name = hit.Source.ProductName,
            Brand = hit.Source.BrandName,
            Price = hit.Source.Price,
            IsInStock = hit.Source.IsInStock,
            ImageUrl = hit.Source.ImageUrl,
            // Confidence = 1 - Cosine Distance (je näher an 1, desto besser)
            Confidence = hit.Score.HasValue ? (float)hit.Score.Value : 0f
        }).ToList();
        
        return Ok(new VisualSearchResponse
        {
            Results = results,
            ProcessingTimeMs = stopwatch.ElapsedMilliseconds,
            EmbeddingDimensions = embedding.Length
        });
    }
}
```

#### Optimierungen für Real-Time Performance

**1. Frame Throttling & Debouncing:**

```typescript
// Nicht jedes Frame analysieren - intelligentes Throttling
const SCAN_INTERVAL_MS = 500;      // Basis: alle 500ms
const MIN_MOVEMENT_THRESHOLD = 20; // Pixel-Differenz für "neues Bild"

let lastFrameHash: string | null = null;

async function scanFrame() {
  const currentHash = await computeFrameHash(videoRef.value);
  
  // Nur scannen wenn sich Bild signifikant geändert hat
  if (currentHash === lastFrameHash) {
    return; // Kamera still → kein API-Call
  }
  
  lastFrameHash = currentHash;
  // ... API-Call
}
```

**2. Progressive Quality (Grob → Fein):**

```typescript
// Erst schnelle Vorschau, dann präzise Suche
async function progressiveScan() {
  // Phase 1: Schnelle Vorschau (320x240, JPEG 50%)
  const quickResults = await searchByImage(captureFrame(320, 240, 0.5));
  
  if (quickResults.length > 0 && quickResults[0].confidence > 0.7) {
    results.value = quickResults; // Schnelles Ergebnis zeigen
    
    // Phase 2: Präzise Suche im Hintergrund (1280x720, JPEG 90%)
    const preciseResults = await searchByImage(captureFrame(1280, 720, 0.9));
    results.value = preciseResults; // Aktualisieren
  }
}
```

**3. WebSocket für Streaming (optional):**

```typescript
// Für Ultra-Low-Latency: WebSocket statt HTTP
const ws = new WebSocket('wss://api.b2x.de/visual-search/stream');

ws.onopen = () => {
  // Kontinuierlicher Frame-Stream
  setInterval(() => {
    const frame = captureFrame(640, 480, 0.7);
    ws.send(frame); // Binary WebSocket
  }, 200); // 5 FPS
};

ws.onmessage = (event) => {
  const results = JSON.parse(event.data);
  updateResults(results);
};
```

**4. Edge Computing (optional):**

```
┌──────────────────────────────────────────────────────────────┐
│                    EDGE ARCHITECTURE                          │
├──────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌─────────────┐    ┌─────────────────────────────────────┐  │
│  │ Mobile App  │───▶│ Cloudflare Workers / Lambda@Edge   │  │
│  │ (Camera)    │    │ ─────────────────────────────────── │  │
│  └─────────────┘    │ • Bild-Preprocessing                │  │
│                     │ • CLIP Embedding (ONNX Runtime)     │  │
│                     │ • Latenz: <50ms                     │  │
│                     └───────────────┬─────────────────────┘  │
│                                     │                        │
│                                     ▼                        │
│                     ┌─────────────────────────────────────┐  │
│                     │ Elasticsearch (Zentraler Index)     │  │
│                     │ kNN Search                          │  │
│                     └─────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────┘
```

#### UX Patterns für Camera Search

**1. Scan-Frame Animation:**

```css
.scan-frame {
  border: 2px solid rgba(255, 255, 255, 0.5);
  transition: all 0.3s ease;
}

.scan-frame.found {
  border-color: #4CAF50;
  box-shadow: 0 0 20px rgba(76, 175, 80, 0.5);
}

/* Animierte Ecken */
.corner {
  position: absolute;
  width: 20px;
  height: 20px;
  border: 3px solid #2196F3;
}

.scan-frame.found .corner {
  border-color: #4CAF50;
  animation: pulse 0.5s ease;
}
```

**2. Confidence Indicator:**

```vue
<template>
  <div class="confidence-bar">
    <div 
      class="confidence-fill" 
      :style="{ width: `${confidence * 100}%` }"
      :class="{
        'low': confidence < 0.5,
        'medium': confidence >= 0.5 && confidence < 0.8,
        'high': confidence >= 0.8
      }"
    />
    <span>{{ Math.round(confidence * 100) }}% Übereinstimmung</span>
  </div>
</template>
```

**3. "Kein Treffer" Fallback:**

```vue
<template>
  <div v-if="noResultsTimeout" class="no-results-hint">
    <Icon name="search-off" />
    <p>Produkt nicht erkannt</p>
    <button @click="switchToTextSearch">
      Stattdessen Artikelnummer eingeben
    </button>
    <button @click="captureAndSendToSupport">
      Foto an Kundenservice senden
    </button>
  </div>
</template>
```

#### B2B Use Cases

| Szenario | Ablauf | Vorteil |
|----------|--------|---------|
| **Ersatzteil-Suche** | Techniker filmt defektes Ventil → Sofort Ersatzteil gefunden | Keine Artikelnummer nötig |
| **Nachbestellung** | Handwerker scannt leere Verpackung → Direkt nachbestellen | 1-Click Reorder |
| **Kompatibilitäts-Check** | Scannt vorhandenes Teil → "Passt zu diesem Gerät" | Cross-Selling |
| **Inventur** | Scannt Regal → Automatische Bestandserfassung | Zeitersparnis |

#### Latenz-Budget

| Phase | Ziel | Technik |
|-------|------|---------|
| Frame Capture | <10ms | Canvas API |
| Kompression | <20ms | JPEG 70% |
| Upload | <50ms | 50KB @ 5Mbps |
| CLIP Embedding | <50ms | GPU / Edge |
| kNN Search | <20ms | HNSW Index |
| Response | <20ms | JSON |
| **Gesamt** | **<200ms** | Real-Time UX |

---

#### 💰 Kostenanalyse: Real-Time Produkterkennung

##### Nutzungs-Szenarien (B2B)

| Szenario | Scans/Monat | Annahmen |
|----------|-------------|----------|
| **Klein** (10 Handwerker) | ~3.000 | 10 Scans/Tag × 10 User × 30 Tage |
| **Mittel** (100 Techniker) | ~30.000 | 10 Scans/Tag × 100 User × 30 Tage |
| **Groß** (1.000 Außendienst) | ~300.000 | 10 Scans/Tag × 1.000 User × 30 Tage |
| **Enterprise** (Industrie) | ~3.000.000 | Automatisierte Systeme + User |

##### Option 1: Self-Hosted CLIP (GPU-Server)

```
┌─────────────────────────────────────────────────────────────────┐
│                    SELF-HOSTED KOSTEN                           │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  GPU-Server (Hetzner/OVH):                                      │
│  ├─ NVIDIA RTX 4090 (24GB VRAM)                                 │
│  │   → €180-250/Monat                                           │
│  │   → Kapazität: ~50 Embeddings/Sekunde                        │
│  │   → = 130 Mio Embeddings/Monat (24/7)                        │
│  │                                                               │
│  └─ NVIDIA A100 (40GB VRAM) - für Enterprise                    │
│      → €1.500-2.500/Monat                                       │
│      → Kapazität: ~200 Embeddings/Sekunde                       │
│                                                                  │
│  Zusätzliche Kosten:                                            │
│  ├─ Elasticsearch Cluster: €50-200/Monat                        │
│  ├─ Bandbreite (50KB × Scans): ~€10-50/Monat                    │
│  └─ DevOps/Wartung: ~4h/Monat                                   │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

| Volumen | GPU-Server | ES Cluster | Bandbreite | **Gesamt/Monat** | **Pro 1.000 Scans** |
|---------|------------|------------|------------|------------------|---------------------|
| 3.000 Scans | €200 | €50 | €5 | **€255** | **€85.00** |
| 30.000 Scans | €200 | €100 | €15 | **€315** | **€10.50** |
| 300.000 Scans | €200 | €200 | €50 | **€450** | **€1.50** |
| 3.000.000 Scans | €250 | €500 | €200 | **€950** | **€0.32** |

**Break-Even**: Ab ~20.000 Scans/Monat günstiger als Cloud-APIs

##### Option 2: Cloud Vision APIs (Pay-per-Use)

| Anbieter | Preis/1.000 Bilder | 30K Scans/Monat | 300K Scans/Monat |
|----------|-------------------|-----------------|------------------|
| **Google Vision AI** | $1.50 | €42 | €420 |
| **AWS Rekognition** | $1.00 | €28 | €280 |
| **Azure Computer Vision** | $1.00 | €28 | €280 |
| **OpenAI CLIP API** (hypothetisch) | ~$0.10 | €3 | €30 |
| **Replicate (CLIP)** | $0.0002/run | €6 | €60 |

> ⚠️ **Achtung**: Google/AWS/Azure Vision sind für **Objekterkennung** (Labels), nicht für **Produktsuche** (Embedding-Matching). Für echte Produkterkennung braucht man CLIP oder eigenes Training.

##### Option 3: Cloudflare Workers AI (Edge)

```
┌─────────────────────────────────────────────────────────────────┐
│                    CLOUDFLARE WORKERS AI                         │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Modell: @cf/openai/clip-vit-base-patch32                       │
│                                                                  │
│  Preismodell (Stand 2025):                                      │
│  ├─ Inklusive: 10.000 Neurons/Tag (Free Tier)                   │
│  ├─ CLIP Embedding: ~10 Neurons pro Bild                        │
│  └─ Überschuss: $0.011 pro 1.000 Neurons                        │
│                                                                  │
│  Berechnung für 1.000 Bilder:                                   │
│  → 1.000 × 10 Neurons = 10.000 Neurons                          │
│  → Kosten: $0.11 pro 1.000 Bilder                               │
│                                                                  │
│  Vorteile:                                                       │
│  ✓ Extrem niedrige Latenz (<50ms weltweit)                      │
│  ✓ Keine GPU-Infrastruktur nötig                                │
│  ✓ Auto-Scaling                                                  │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

| Volumen | Cloudflare AI | ES Cluster | **Gesamt/Monat** | **Pro 1.000 Scans** |
|---------|---------------|------------|------------------|---------------------|
| 3.000 Scans | €0.30 | €50 | **€50** | **€16.70** |
| 30.000 Scans | €3.00 | €100 | **€103** | **€3.43** |
| 300.000 Scans | €30 | €200 | **€230** | **€0.77** |
| 3.000.000 Scans | €300 | €500 | **€800** | **€0.27** |

##### Kostenvergleich: Alle Optionen

```
Kosten pro 1.000 Scans (€)
│
│  €100 ┤ ████████████████████████████████████████ Self-Hosted (3K)
│       │
│   €50 ┤
│       │
│   €20 ┤ ████████████████ Cloudflare (3K)
│       │
│   €10 ┤ ██████████ Self-Hosted (30K)
│       │
│    €5 ┤
│       │ ███ Cloudflare (30K)
│    €3 ┤
│       │
│    €1 ┤ █ Self-Hosted (300K)
│       │ █ Cloudflare (300K)
│   €0.3┤ ▌ Both (3M)
│       └────────────────────────────────────────────────────────
│         3K        30K        300K        3M     Scans/Monat
```

##### Empfehlung nach Unternehmensgröße

| Größe | Empfehlung | Kosten/Monat | Begründung |
|-------|------------|--------------|------------|
| **Startup** (<10K Scans) | Cloudflare Workers AI | ~€50-100 | Kein DevOps-Aufwand |
| **KMU** (10K-100K Scans) | Cloudflare Workers AI | ~€100-250 | Beste Kosten/Nutzen |
| **Mittelstand** (100K-500K) | Self-Hosted GPU | ~€400-600 | Amortisiert sich |
| **Enterprise** (>500K) | Self-Hosted Multi-GPU | ~€1.000+ | Volle Kontrolle |

##### ROI-Betrachtung für B2B

```
┌─────────────────────────────────────────────────────────────────┐
│                    ROI-RECHNUNG (Beispiel)                       │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Annahmen:                                                       │
│  ├─ 100 Außendienst-Techniker                                   │
│  ├─ 10 Produktsuchen/Tag via Kamera                             │
│  ├─ Zeitersparnis: 2 Min/Suche (vs. Katalog/Tel. Hotline)       │
│  └─ Stundensatz Techniker: €50                                  │
│                                                                  │
│  Berechnung:                                                     │
│  ├─ Suchen/Monat: 100 × 10 × 22 = 22.000                        │
│  ├─ Zeitersparnis: 22.000 × 2 Min = 733 Stunden/Monat           │
│  ├─ Wert: 733 × €50 = €36.650/Monat                             │
│  │                                                               │
│  Kosten (Cloudflare):                                            │
│  └─ ~€150/Monat                                                  │
│                                                                  │
│  ════════════════════════════════════════════════════════════   │
│  ROI: €36.650 / €150 = 244x                                     │
│  Amortisation: < 1 Tag                                          │
│  ════════════════════════════════════════════════════════════   │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

##### Zusätzliche Kostenfaktoren

| Faktor | Einmalig | Laufend | Anmerkung |
|--------|----------|---------|-----------|
| **Initiale Indexierung** | 1-5 Tage GPU | - | Alle Produktbilder embedden |
| **Neue Produkte** | - | ~0.01€/Produkt | Bei Katalog-Import |
| **Speicher (ES)** | - | ~€0.10/GB/Monat | ~1KB pro Bild-Embedding |
| **App-Entwicklung** | €10-30K | - | PWA/Native App |
| **Monitoring** | - | €50-100/Monat | Grafana/Prometheus |

##### Fazit: Produkterkennung ist erschwinglich!

| Unternehmensgröße | Typische Kosten | Bewertung |
|-------------------|-----------------|-----------|
| 10 Handwerker | **€50-100/Monat** | ☕ Preis eines Team-Meetings |
| 100 Techniker | **€100-250/Monat** | 🎫 Preis einer Messe-Teilnahme |
| 1.000 User | **€400-800/Monat** | 💼 Preis eines Mitarbeiters (1%) |

> **Kernaussage**: Für typische B2B-Szenarien (10-100 User) kostet Real-Time Produkterkennung **€50-250/Monat** – ein Bruchteil des Zeitersparnisses!

---

### 2.9.1 OCR-Integration: Typenschilder, Seriennummern & Artikelnummern scannen

> **🎯 PREMIUM FEATURE (Phase 2/3)** - Kombiniert mit Visual Search = Killer-Kombi für B2B!

#### Das Problem: Informationen auf dem Produkt

```
┌─────────────────────────────────────────────────────────────────┐
│                    DAS TYPENSCHILD-PROBLEM                       │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Handwerker steht vor defektem Gerät:                           │
│                                                                  │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │  GRUNDFOS                                               │    │
│  │  ════════════════════════════════════════════════════   │    │
│  │  Type: UPS 25-60 180                                    │    │
│  │  P/N:  96281476                                         │    │
│  │  S/N:  A2023-04-12345                                   │    │
│  │  230V~ 50Hz 65W IP44                                    │    │
│  │  Made in Denmark                                        │    │
│  │  ════════════════════════════════════════════════════   │    │
│  │  [Barcode] [QR-Code]                                    │    │
│  └─────────────────────────────────────────────────────────┘    │
│                                                                  │
│  WAS WILL DER HANDWERKER?                                       │
│  ─────────────────────────────────────────────────────────────  │
│  • Ersatzteil finden (gleiche Pumpe oder kompatibel)            │
│  • Seriennummer für Garantiefall dokumentieren                  │
│  • Technische Daten nachschlagen (230V, 65W...)                 │
│  • Zubehör bestellen (Dichtungen, Flansche...)                  │
│                                                                  │
│  LÖSUNG: 📷 Typenschild scannen → Sofort alle Infos!            │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Hybrid-Ansatz: OCR + Visual Search + Barcode

```
┌────────────────────────────────────────────────────────────────────────────┐
│                    INTELLIGENTE BILD-ANALYSE PIPELINE                       │
├────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  📷 KAMERA-INPUT                                                            │
│       │                                                                     │
│       ▼                                                                     │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │ 1. BARCODE/QR-DETECTION (schnellste Methode, ~10ms)                │   │
│  │    ├─ EAN/GTIN gefunden? → Direkt Produkt-Lookup                   │   │
│  │    ├─ QR-Code mit URL? → Redirect oder Parse                       │   │
│  │    └─ DataMatrix? → GS1-Parse (Pharma, Medizin)                    │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│       │ Kein Barcode?                                                       │
│       ▼                                                                     │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │ 2. OCR-ERKENNUNG (Texterkennung, ~50-200ms)                        │   │
│  │    ├─ Artikelnummer erkannt? → Produkt-Suche                       │   │
│  │    ├─ Seriennummer erkannt? → Geräte-Lookup + Ersatzteile          │   │
│  │    ├─ Typenbezeichnung erkannt? → Fuzzy-Search                     │   │
│  │    └─ Technische Daten? → Attribut-basierte Suche                  │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│       │ Kein lesbarer Text?                                                 │
│       ▼                                                                     │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │ 3. VISUAL SEARCH / CLIP (Objekterkennung, ~100-200ms)              │   │
│  │    └─ Produkt visuell identifizieren                               │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│       │                                                                     │
│       ▼                                                                     │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │ 4. KOMBINIERTE ERGEBNISSE                                          │   │
│  │    • Produkt gefunden → Anzeigen                                   │   │
│  │    • Kompatible Ersatzteile → Vorschlagen                          │   │
│  │    • Zubehör → Cross-Selling                                       │   │
│  │    • Seriennummer → Für Garantie/Service speichern                 │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                                                             │
└────────────────────────────────────────────────────────────────────────────┘
```

#### OCR Use Cases im B2B

| Szenario | Was wird gescannt? | Ergebnis |
|----------|-------------------|----------|
| **Ersatzteil-Suche** | Typenschild der Pumpe | "Grundfos UPS 25-60" → Ersatzpumpe + Dichtungssatz |
| **Nachbestellung** | Artikelnummer auf Verpackung | "Hilti TE 500" → Direkt nachbestellen |
| **Garantiefall** | Seriennummer | S/N dokumentiert + Kaufdatum aus System |
| **Kompatibilität** | Motorleistung "1.5kW" | Passende Frequenzumrichter vorschlagen |
| **Elektriker** | Typenschild Schütz | "Siemens 3RT1025" → Ersatz + Hilfsschalter |
| **Wartung** | Herstellungsdatum | "MFG 2019" → Wartungsintervall prüfen |
| **Dokumentation** | Gesamtes Typenschild | Als Bild + strukturierte Daten speichern |

#### OCR-Service Implementation

```typescript
// Backend-Service für OCR + Strukturierte Extraktion
class OcrSearchService {
  private readonly ocrProvider: IOcrProvider;  // Tesseract, Google Vision, Azure
  private readonly elasticClient: ElasticsearchClient;
  
  async analyzeImage(imageBuffer: Buffer): Promise<OcrAnalysisResult> {
    // 1. OCR durchführen
    const rawText = await this.ocrProvider.extractText(imageBuffer);
    
    // 2. Strukturierte Daten extrahieren
    const extractedData = this.parseTypenschild(rawText);
    
    // 3. Multi-Strategie Suche
    const searchResults = await this.multiStrategySearch(extractedData);
    
    return {
      rawText,
      extractedData,
      searchResults,
      confidence: this.calculateConfidence(extractedData)
    };
  }
  
  // Typenschild-Parser mit Regex-Patterns
  private parseTypenschild(text: string): ExtractedTypenschildData {
    const patterns = {
      // Artikelnummern (verschiedene Formate)
      articleNumber: [
        /(?:Art\.?[-\s]?(?:Nr\.?)?|P\/N|PN|Part[-\s]?No\.?)[\s:]*([A-Z0-9\-\.]+)/gi,
        /(?:Bestell[-\s]?(?:Nr\.?)?|Order[-\s]?No\.?)[\s:]*([A-Z0-9\-\.]+)/gi,
      ],
      
      // Seriennummern
      serialNumber: [
        /(?:S\/N|SN|Serial|Serien[-\s]?Nr\.?)[\s:]*([A-Z0-9\-]+)/gi,
        /(?:Fabrik[-\s]?Nr\.?|Fab\.?[-\s]?Nr\.?)[\s:]*([A-Z0-9\-]+)/gi,
      ],
      
      // Typenbezeichnung
      typeDesignation: [
        /(?:Type|Typ|Model|Modell)[\s:]*([A-Z0-9\-\s]+)/gi,
      ],
      
      // Hersteller (aus bekannter Liste)
      manufacturer: this.knownManufacturers.map(m => new RegExp(`\\b${m}\\b`, 'gi')),
      
      // Technische Daten
      voltage: /(\d{2,3})\s*V(?:~|AC|DC)?/gi,
      power: /(\d+(?:[.,]\d+)?)\s*(?:k?W|VA)/gi,
      current: /(\d+(?:[.,]\d+)?)\s*A/gi,
      frequency: /(\d{2})\s*Hz/gi,
      ipRating: /IP\s*(\d{2})/gi,
      
      // Datum
      manufacturingDate: [
        /(?:MFG|Herst\.?|Prod\.?)[\s:]*(\d{4}[-\/]\d{2}(?:[-\/]\d{2})?)/gi,
        /(?:Date|Datum)[\s:]*(\d{2}[.\/]\d{2}[.\/]\d{2,4})/gi,
      ],
    };
    
    const extracted: ExtractedTypenschildData = {
      articleNumbers: [],
      serialNumbers: [],
      typeDesignations: [],
      manufacturers: [],
      technicalSpecs: {},
      rawText: text
    };
    
    // Pattern-Matching durchführen
    for (const [key, patternList] of Object.entries(patterns)) {
      const patternsArray = Array.isArray(patternList) ? patternList : [patternList];
      for (const pattern of patternsArray) {
        const matches = text.matchAll(pattern);
        for (const match of matches) {
          const value = match[1]?.trim();
          if (value) {
            if (key === 'articleNumber') extracted.articleNumbers.push(value);
            else if (key === 'serialNumber') extracted.serialNumbers.push(value);
            else if (key === 'typeDesignation') extracted.typeDesignations.push(value);
            else if (key === 'manufacturer') extracted.manufacturers.push(value);
            else extracted.technicalSpecs[key] = value;
          }
        }
      }
    }
    
    return extracted;
  }
  
  // Multi-Strategie Suche mit Priorisierung
  private async multiStrategySearch(data: ExtractedTypenschildData): Promise<SearchResult[]> {
    const strategies: SearchStrategy[] = [];
    
    // Strategie 1: Exakte Artikelnummer (höchste Priorität)
    if (data.articleNumbers.length > 0) {
      strategies.push({
        priority: 100,
        query: {
          bool: {
            should: data.articleNumbers.map(an => ({
              term: { "sku.exact": an }
            }))
          }
        }
      });
    }
    
    // Strategie 2: Hersteller + Typenbezeichnung
    if (data.manufacturers.length > 0 && data.typeDesignations.length > 0) {
      strategies.push({
        priority: 80,
        query: {
          bool: {
            must: [
              { terms: { "brand_name": data.manufacturers } },
              { 
                multi_match: {
                  query: data.typeDesignations.join(' '),
                  fields: ["name^3", "type_designation", "search_keywords"],
                  fuzziness: "AUTO"
                }
              }
            ]
          }
        }
      });
    }
    
    // Strategie 3: Typenbezeichnung mit Fuzzy-Match
    if (data.typeDesignations.length > 0) {
      strategies.push({
        priority: 60,
        query: {
          multi_match: {
            query: data.typeDesignations.join(' '),
            fields: ["name^2", "type_designation^3", "search_keywords"],
            fuzziness: "AUTO",
            operator: "or"
          }
        }
      });
    }
    
    // Strategie 4: Technische Daten als Filter
    if (Object.keys(data.technicalSpecs).length >= 2) {
      const filters = [];
      if (data.technicalSpecs.voltage) {
        filters.push({ term: { "attributes.voltage": data.technicalSpecs.voltage } });
      }
      if (data.technicalSpecs.power) {
        filters.push({ term: { "attributes.power": data.technicalSpecs.power } });
      }
      
      if (filters.length > 0) {
        strategies.push({
          priority: 40,
          query: {
            bool: {
              filter: filters,
              should: data.manufacturers.map(m => ({ term: { brand_name: m } }))
            }
          }
        });
      }
    }
    
    // Strategien nach Priorität ausführen
    strategies.sort((a, b) => b.priority - a.priority);
    
    for (const strategy of strategies) {
      const results = await this.elasticClient.search({
        index: `b2x_${this.tenantId}_*`,
        body: { query: strategy.query, size: 10 }
      });
      
      if (results.hits.total.value > 0) {
        return this.mapToSearchResults(results.hits.hits, strategy.priority);
      }
    }
    
    return [];
  }
  
  // Bekannte Hersteller für B2B
  private readonly knownManufacturers = [
    'Grundfos', 'Wilo', 'KSB', 'Ebara',           // Pumpen
    'Siemens', 'ABB', 'Schneider', 'Eaton',       // Elektro
    'Bosch', 'Makita', 'Hilti', 'DeWalt',         // Werkzeuge
    'Viessmann', 'Vaillant', 'Buderus', 'Wolf',   // Heizung
    'Grohe', 'Hansgrohe', 'Geberit', 'Ideal',     // Sanitär
    'Danfoss', 'Honeywell', 'Belimo', 'Oventrop', // Regelungstechnik
    'Festo', 'SMC', 'Parker', 'Rexroth',          // Pneumatik/Hydraulik
    'Phoenix', 'Wago', 'Weidmüller', 'Murr',      // Verbindungstechnik
    // ... erweiterbarer Katalog
  ];
}

interface ExtractedTypenschildData {
  articleNumbers: string[];
  serialNumbers: string[];
  typeDesignations: string[];
  manufacturers: string[];
  technicalSpecs: Record<string, string>;
  rawText: string;
}

interface OcrAnalysisResult {
  rawText: string;
  extractedData: ExtractedTypenschildData;
  searchResults: SearchResult[];
  confidence: number;
}
```

#### Frontend: Typenschild-Scanner UI

```vue
<template>
  <div class="typeplate-scanner">
    <!-- Kamera-Vorschau mit OCR-Overlay -->
    <div class="camera-container">
      <video ref="videoRef" autoplay playsinline />
      
      <!-- Erkannte Textbereiche highlighten -->
      <div 
        v-for="region in detectedRegions" 
        :key="region.id"
        class="text-region"
        :style="region.style"
      >
        <span class="region-label">{{ region.type }}</span>
      </div>
      
      <!-- Scan-Rahmen -->
      <div class="scan-frame" :class="{ 'found': hasResults }">
        <div class="corner top-left" />
        <div class="corner top-right" />
        <div class="corner bottom-left" />
        <div class="corner bottom-right" />
      </div>
    </div>
    
    <!-- Erkannte Daten -->
    <div v-if="extractedData" class="extracted-info">
      <h3>📋 Erkannte Informationen</h3>
      
      <div v-if="extractedData.manufacturers.length" class="info-row">
        <span class="label">Hersteller:</span>
        <span class="value">{{ extractedData.manufacturers.join(', ') }}</span>
      </div>
      
      <div v-if="extractedData.typeDesignations.length" class="info-row">
        <span class="label">Typ:</span>
        <span class="value highlight">{{ extractedData.typeDesignations[0] }}</span>
      </div>
      
      <div v-if="extractedData.articleNumbers.length" class="info-row">
        <span class="label">Artikelnummer:</span>
        <span class="value highlight">{{ extractedData.articleNumbers[0] }}</span>
      </div>
      
      <div v-if="extractedData.serialNumbers.length" class="info-row">
        <span class="label">Seriennummer:</span>
        <span class="value">{{ extractedData.serialNumbers[0] }}</span>
        <button @click="saveSerialNumber" class="btn-save">
          💾 Speichern
        </button>
      </div>
      
      <div v-if="Object.keys(extractedData.technicalSpecs).length" class="tech-specs">
        <span class="label">Technische Daten:</span>
        <div class="specs-grid">
          <span v-if="extractedData.technicalSpecs.voltage">
            ⚡ {{ extractedData.technicalSpecs.voltage }}V
          </span>
          <span v-if="extractedData.technicalSpecs.power">
            💪 {{ extractedData.technicalSpecs.power }}
          </span>
          <span v-if="extractedData.technicalSpecs.ipRating">
            🛡️ IP{{ extractedData.technicalSpecs.ipRating }}
          </span>
        </div>
      </div>
    </div>
    
    <!-- Suchergebnisse -->
    <div v-if="searchResults.length" class="search-results">
      <h3>🔍 Gefundene Produkte</h3>
      
      <div class="result-tabs">
        <button 
          :class="{ active: activeTab === 'exact' }"
          @click="activeTab = 'exact'"
        >
          Exakt ({{ exactMatches.length }})
        </button>
        <button 
          :class="{ active: activeTab === 'spare' }"
          @click="activeTab = 'spare'"
        >
          Ersatzteile ({{ spareParts.length }})
        </button>
        <button 
          :class="{ active: activeTab === 'compatible' }"
          @click="activeTab = 'compatible'"
        >
          Kompatibel ({{ compatible.length }})
        </button>
      </div>
      
      <ProductCard 
        v-for="product in currentTabResults" 
        :key="product.id" 
        :product="product"
        @add-to-cart="addToCart"
      />
    </div>
    
    <!-- Actions -->
    <div class="actions">
      <button @click="captureHighRes" class="btn-primary">
        📸 Hochauflösend scannen
      </button>
      <button @click="switchToVisualSearch" class="btn-secondary">
        🖼️ Produkt visuell suchen
      </button>
    </div>
  </div>
</template>
```

#### OCR-Provider Optionen

```
┌─────────────────────────────────────────────────────────────────┐
│                    OCR-PROVIDER VERGLEICH                        │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │ 1. TESSERACT (Open Source, Self-Hosted)                 │    │
│  │    ───────────────────────────────────────────────────  │    │
│  │    ✓ Kostenlos                                          │    │
│  │    ✓ DSGVO-konform (on-premise)                         │    │
│  │    ✗ Geringere Genauigkeit bei schlechter Bildqualität  │    │
│  │    ✗ Langsamer (~200-500ms)                             │    │
│  │    → Gut für: Entwicklung, Budget-Lösung                │    │
│  └─────────────────────────────────────────────────────────┘    │
│                                                                  │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │ 2. GOOGLE CLOUD VISION OCR                              │    │
│  │    ───────────────────────────────────────────────────  │    │
│  │    ✓ Sehr hohe Genauigkeit                              │    │
│  │    ✓ Erkennt Handschrift                                │    │
│  │    ✓ Schnell (~100ms)                                   │    │
│  │    $ $1.50 / 1.000 Bilder                               │    │
│  │    → Gut für: Produktiv-Einsatz                         │    │
│  └─────────────────────────────────────────────────────────┘    │
│                                                                  │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │ 3. AZURE COMPUTER VISION (Read API)                     │    │
│  │    ───────────────────────────────────────────────────  │    │
│  │    ✓ Sehr hohe Genauigkeit                              │    │
│  │    ✓ Batch-Verarbeitung                                 │    │
│  │    ✓ Gut für gedruckten Text                            │    │
│  │    $ $1.00 / 1.000 Bilder                               │    │
│  │    → Gut für: Enterprise, Azure-Stack                   │    │
│  └─────────────────────────────────────────────────────────┘    │
│                                                                  │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │ 4. AWS TEXTRACT                                         │    │
│  │    ───────────────────────────────────────────────────  │    │
│  │    ✓ Strukturierte Daten-Extraktion                     │    │
│  │    ✓ Tabellen-Erkennung                                 │    │
│  │    ✓ Formulare verstehen                                │    │
│  │    $ $1.50 / 1.000 Seiten                               │    │
│  │    → Gut für: Dokumente, Rechnungen                     │    │
│  └─────────────────────────────────────────────────────────┘    │
│                                                                  │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │ 5. CLOUDFLARE WORKERS AI (neu!)                         │    │
│  │    ───────────────────────────────────────────────────  │    │
│  │    ✓ Edge-Processing (geringe Latenz)                   │    │
│  │    ✓ Kombinierbar mit CLIP                              │    │
│  │    $ ~$0.01 / 1.000 Bilder                              │    │
│  │    → Gut für: Real-Time Scanner, Budget                 │    │
│  └─────────────────────────────────────────────────────────┘    │
│                                                                  │
│  EMPFEHLUNG B2X:                                                │
│  • Development: Tesseract (kostenlos)                           │
│  • Production klein: Cloudflare Workers AI                       │
│  • Production Enterprise: Google Vision oder Azure              │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Kosten OCR + Visual Search kombiniert

| Szenario | Scans/Monat | OCR (Google) | CLIP (CF) | **Gesamt** |
|----------|-------------|--------------|-----------|------------|
| 10 Handwerker | 3.000 | €4.50 | €0.30 | **~€55** (+ ES) |
| 100 Techniker | 30.000 | €45 | €3 | **~€150** (+ ES) |
| 1.000 User | 300.000 | €450 | €30 | **~€700** (+ ES) |

#### Erweiterte Use Cases: Seriennummern-Tracking

```typescript
// Seriennummer mit Kundengerät verknüpfen
interface CustomerDevice {
  id: string;
  customerId: string;
  
  // Vom Typenschild gescannt
  serialNumber: string;
  manufacturer: string;
  typeDesignation: string;
  technicalSpecs: Record<string, string>;
  
  // Automatisch ermittelt
  productId?: string;          // Verknüpftes Produkt im Katalog
  purchaseDate?: Date;         // Aus Bestellhistorie
  warrantyUntil?: Date;        // Berechnet
  
  // Service-Historie
  serviceHistory: ServiceEvent[];
  
  // Für Nachbestellungen
  linkedSparePartsOrdered: string[];
  recommendedMaintenanceDate?: Date;
  
  // Gescannte Bilder
  typeplateImages: string[];   // URLs der Scan-Bilder
  scannedAt: Date;
  scannedBy: string;           // User-ID
  gpsLocation?: { lat: number; lng: number };  // Wo gescannt?
}

// Service: Gerät registrieren
class CustomerDeviceService {
  async registerDeviceFromScan(
    customerId: string,
    ocrResult: OcrAnalysisResult,
    image: Buffer
  ): Promise<CustomerDevice> {
    // 1. Prüfen ob Gerät bereits registriert
    const existing = await this.findBySerialNumber(
      ocrResult.extractedData.serialNumbers[0]
    );
    
    if (existing) {
      // Gerät bereits bekannt → Update
      return this.updateDevice(existing.id, { lastScannedAt: new Date() });
    }
    
    // 2. Neues Gerät anlegen
    const device: CustomerDevice = {
      id: generateId(),
      customerId,
      serialNumber: ocrResult.extractedData.serialNumbers[0],
      manufacturer: ocrResult.extractedData.manufacturers[0],
      typeDesignation: ocrResult.extractedData.typeDesignations[0],
      technicalSpecs: ocrResult.extractedData.technicalSpecs,
      productId: ocrResult.searchResults[0]?.id,
      typeplateImages: [await this.uploadImage(image)],
      scannedAt: new Date(),
      serviceHistory: []
    };
    
    // 3. Garantie prüfen
    if (device.productId) {
      const purchase = await this.findPurchase(customerId, device.productId);
      if (purchase) {
        device.purchaseDate = purchase.date;
        device.warrantyUntil = addYears(purchase.date, 2); // Standard 2 Jahre
      }
    }
    
    // 4. Wartungsempfehlung
    device.recommendedMaintenanceDate = this.calculateMaintenanceDate(device);
    
    await this.save(device);
    
    // 5. Benachrichtigung bei bald ablaufender Garantie
    if (device.warrantyUntil && isWithinMonths(device.warrantyUntil, 3)) {
      await this.notifyWarrantyExpiring(customerId, device);
    }
    
    return device;
  }
}
```

#### UX: "Meine Geräte" Dashboard

```
┌─────────────────────────────────────────────────────────────────┐
│  🔧 MEINE GERÄTE                              [+ Gerät scannen] │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │ 🏭 GRUNDFOS UPS 25-60 180                               │    │
│  │ ─────────────────────────────────────────────────────── │    │
│  │ S/N: A2023-04-12345                                     │    │
│  │ Gekauft: 15.03.2023 │ Garantie bis: 15.03.2025 ✓        │    │
│  │                                                         │    │
│  │ [🔧 Ersatzteile] [📄 Datenblatt] [📞 Service anfragen]  │    │
│  └─────────────────────────────────────────────────────────┘    │
│                                                                  │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │ ⚡ SIEMENS 3RT1025-1BB40                                 │    │
│  │ ─────────────────────────────────────────────────────── │    │
│  │ S/N: 2020-W45-78923                                     │    │
│  │ Gekauft: 08.11.2020 │ Garantie: ⚠️ Abgelaufen           │    │
│  │                                                         │    │
│  │ [🔧 Ersatzteile] [🔄 Nachfolger bestellen]              │    │
│  └─────────────────────────────────────────────────────────┘    │
│                                                                  │
│  💡 WARTUNGSERINNERUNGEN                                        │
│  ─────────────────────────────────────────────────────────────  │
│  • Grundfos-Pumpe: Dichtungswechsel fällig (letzte Wartung     │
│    vor 18 Monaten) → [Dichtungssatz bestellen]                 │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Technische Herausforderungen & Lösungen

| Herausforderung | Lösung |
|-----------------|--------|
| **Schlechte Bildqualität** | Mehrere Frames analysieren, bestes Ergebnis wählen |
| **Verschmutztes Typenschild** | Bildvorverarbeitung (Kontrast, Schärfe) |
| **Schräge Aufnahme** | Perspektivkorrektur vor OCR |
| **Verschiedene Sprachen** | Multi-Language OCR (DE, EN, FR) |
| **Handschrift** | Cloud-OCR mit Handschrifterkennung |
| **Reflexionen** | User anweisen: "Ohne Blitz fotografieren" |
| **Alte, verblasste Schilder** | Invertierung, Histogramm-Anpassung |

#### Integration in Visual Search Pipeline

```typescript
// Kombinierter Scanner: OCR + Visual + Barcode
async function intelligentProductSearch(image: Buffer): Promise<CombinedSearchResult> {
  // Parallel alle Methoden starten
  const [barcodeResult, ocrResult, visualResult] = await Promise.allSettled([
    barcodeScanner.scan(image),      // ~10ms
    ocrService.analyzeImage(image),   // ~100-200ms
    clipService.searchByImage(image)  // ~100-200ms
  ]);
  
  // Ergebnisse nach Confidence priorisieren
  const results: RankedResult[] = [];
  
  // Barcode hat höchste Priorität (100% exakt)
  if (barcodeResult.status === 'fulfilled' && barcodeResult.value) {
    results.push({
      source: 'barcode',
      confidence: 1.0,
      products: await lookupByBarcode(barcodeResult.value)
    });
  }
  
  // OCR mit extrahierter Artikelnummer (sehr zuverlässig)
  if (ocrResult.status === 'fulfilled' && ocrResult.value.extractedData.articleNumbers.length > 0) {
    results.push({
      source: 'ocr_article',
      confidence: 0.95,
      products: ocrResult.value.searchResults,
      extractedData: ocrResult.value.extractedData
    });
  }
  
  // OCR mit Typenbezeichnung (gut)
  if (ocrResult.status === 'fulfilled' && ocrResult.value.extractedData.typeDesignations.length > 0) {
    results.push({
      source: 'ocr_type',
      confidence: 0.8,
      products: ocrResult.value.searchResults,
      extractedData: ocrResult.value.extractedData
    });
  }
  
  // Visual Search als Fallback
  if (visualResult.status === 'fulfilled') {
    results.push({
      source: 'visual',
      confidence: visualResult.value.topConfidence,
      products: visualResult.value.products
    });
  }
  
  // Beste Ergebnisse zurückgeben
  return {
    primaryResult: results.sort((a, b) => b.confidence - a.confidence)[0],
    allResults: results,
    extractedData: ocrResult.status === 'fulfilled' ? ocrResult.value.extractedData : null
  };
}
```

---

### 2.10 Wettbewerbsanalyse: Search Features im E-Commerce

> **Was machen Amazon, Mercateo, Contorion & Co.?**

#### Feature-Matrix: Wettbewerb vs. B2X

```
┌────────────────────────────────────────────────────────────────────────────┐
│                    SEARCH FEATURE LANDSCAPE 2026                            │
├────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  BASIC (Table Stakes)          ADVANCED              CUTTING EDGE           │
│  ═══════════════════          ════════════          ═══════════════        │
│                                                                             │
│  ✓ Volltext-Suche             ◐ Personalisierung    ○ Visual Search        │
│  ✓ Facetten/Filter            ◐ Voice Search        ○ Camera Scanner       │
│  ✓ Autocomplete               ◐ Predictive Search   ○ AR "Passt das?"      │
│  ✓ Typo-Korrektur             ◐ Search Merchandis.  ○ KI-Berater           │
│  ✓ Mobile-Optimierung         ◐ A/B Testing         ○ Conversational       │
│                                                                             │
│  ────────────────────────────────────────────────────────────────────────  │
│  Legende: ✓ MVP  ◐ Phase 2  ○ Phase 3+                                     │
└────────────────────────────────────────────────────────────────────────────┘
```

#### Detaillierte Feature-Analyse

##### 1️⃣ Autocomplete & Instant Search (MVP ✓)

```
┌─────────────────────────────────────────────────────────────────┐
│  🔍 bosch akku                                                  │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  📦 PRODUKTE                        🏷️ KATEGORIEN              │
│  ─────────────────────────────────  ────────────────────────    │
│  Bosch Akkuschrauber GSR 18V   €89  Akkuwerkzeuge               │
│  Bosch Akku 18V 4.0Ah          €49  Bosch Professional          │
│  Bosch Akku-Winkelschleifer    €159 Ersatzakkus                 │
│                                                                  │
│  🔤 SUCHVORSCHLÄGE              📊 MARKEN                       │
│  ─────────────────────────────  ────────────────────────────    │
│  bosch akkuschrauber 18v       Bosch (234)                      │
│  bosch akku 5.0ah              Makita (156)                     │
│  bosch akku ladegerät          DeWalt (98)                      │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

**Wettbewerb:**
| Anbieter | Produkt-Preview | Kategorie-Suggest | Marken-Filter | Preis in Suggest |
|----------|-----------------|-------------------|---------------|------------------|
| Amazon | ✓ | ✓ | ✓ | ✓ |
| Mercateo | ✓ | ✓ | ○ | ○ |
| Contorion | ✓ | ✓ | ✓ | ✓ |
| Hoffmann | ◐ | ✓ | ○ | ○ |

**B2X Implementation:**
```typescript
// Elasticsearch Completion Suggester + Search
GET b2x_tenant_de/_search
{
  "suggest": {
    "product-suggest": {
      "prefix": "bosch akku",
      "completion": {
        "field": "suggest",
        "size": 5,
        "fuzzy": { "fuzziness": 1 }
      }
    }
  },
  "query": {
    "multi_match": {
      "query": "bosch akku",
      "fields": ["name^3", "brand^2", "sku"],
      "type": "phrase_prefix"
    }
  },
  "size": 3,
  "_source": ["name", "brand", "price", "image_url"]
}
```

---

##### 2️⃣ "Meinten Sie...?" / Typo-Korrektur (MVP ✓)

```
┌─────────────────────────────────────────────────────────────────┐
│  🔍 bosch akuschrauber                                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  💡 Meinten Sie: "bosch akkuschrauber"?                         │
│     ─────────────────────────────────                           │
│     234 Ergebnisse für "bosch akkuschrauber"                    │
│                                                                  │
│  Oder suchen Sie nach:                                          │
│  • Bosch Schrauber (ohne Akku)                                  │
│  • Akku-Schraubendreher                                         │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

**Elasticsearch Fuzzy + Did-You-Mean:**
```typescript
{
  "query": {
    "bool": {
      "should": [
        { "match": { "name": { "query": "bosch akuschrauber", "fuzziness": "AUTO" }}},
        { "match": { "name.phonetic": "bosch akuschrauber" }}  // Soundex/Metaphone
      ]
    }
  },
  "suggest": {
    "did-you-mean": {
      "text": "bosch akuschrauber",
      "phrase": {
        "field": "name.trigram",
        "gram_size": 3,
        "confidence": 1.0
      }
    }
  }
}
```

---

##### 3️⃣ Personalisierte Suche (Phase 2 ◐)

> **Konzept**: Suchergebnisse basierend auf Kaufhistorie, Branche, Präferenzen boosten

```
┌─────────────────────────────────────────────────────────────────┐
│                    PERSONALISIERUNGS-SIGNALE                     │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Kunde: Elektro-Meister GmbH (SHK-Branche)                      │
│                                                                  │
│  📊 KAUFHISTORIE              🎯 PRÄFERENZEN                    │
│  ───────────────────────────  ────────────────────────────────  │
│  • 80% Bosch Professional     • Bevorzugt: 18V-System           │
│  • Häufig: Installationsbedarf• Preissensitiv: Mittel           │
│  • Letzte Bestellung: Rohr    • Lieferung: Express bevorzugt    │
│                                                                  │
│  🔄 PERSONALISIERTES RANKING:                                   │
│  ───────────────────────────────────────────────────────────    │
│  Suche "Bohrer" →                                               │
│    1. Bosch SDS-Plus (Branche + Marke)    [+50 Boost]          │
│    2. Makita SDS-Plus (Alternative)        [+20 Boost]          │
│    3. Billig-Bohrer (Standard)             [0 Boost]            │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

**Elasticsearch mit Personalisierungs-Boost:**
```typescript
{
  "query": {
    "function_score": {
      "query": { "match": { "name": "bohrer" }},
      "functions": [
        {
          "filter": { "term": { "brand": "bosch" }},
          "weight": 1.5  // Kunde kauft oft Bosch
        },
        {
          "filter": { "term": { "categories": "shk-installation" }},
          "weight": 1.3  // Branchenrelevanz
        },
        {
          "filter": { "term": { "voltage": "18v" }},
          "weight": 1.2  // Kunde nutzt 18V-System
        },
        {
          "script_score": {
            "script": {
              "source": "cosineSimilarity(params.user_vector, 'preference_vector') + 1.0",
              "params": { "user_vector": [0.8, 0.2, 0.5, ...] }
            }
          }
        }
      ],
      "boost_mode": "multiply"
    }
  }
}
```

**Wettbewerb:**
| Anbieter | Kaufhistorie | Branche | Preispräferenz | ML-Ranking |
|----------|--------------|---------|----------------|------------|
| Amazon | ✓✓✓ | ✓ | ✓✓ | ✓✓✓ |
| Mercateo | ✓ | ✓✓ | ○ | ○ |
| Contorion | ✓ | ✓ | ✓ | ◐ |
| B2X (Ziel) | ✓✓ | ✓✓ | ✓ | ◐ |

---

##### 4️⃣ Voice Search (Phase 2 ◐)

> **B2B Use Case**: Handwerker auf Baustelle, Hände voll, spricht Bestellung

```
┌─────────────────────────────────────────────────────────────────┐
│                    🎤 VOICE SEARCH                               │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  "Hey B2X, bestell mir zehn Stück Spax                          │
│   fünf mal fünfzig in Edelstahl"                                │
│                                                                  │
│  ↓ Speech-to-Text (Whisper/Azure)                               │
│  ↓ NLU Intent Detection                                         │
│  ↓ Entity Extraction                                            │
│                                                                  │
│  Intent: ORDER                                                   │
│  Entities:                                                       │
│  ├─ quantity: 10                                                │
│  ├─ brand: "Spax"                                               │
│  ├─ dimensions: "5x50mm"                                        │
│  └─ material: "Edelstahl/A2"                                    │
│                                                                  │
│  → Produkt gefunden: SPAX 5x50 A2 (Art. 12345)                  │
│  → "Soll ich 10 Stück für €8,90 in den Warenkorb legen?"        │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

**Technologie-Stack:**
```
Sprache → Web Speech API / Whisper → NLU (GPT/Claude) → Elasticsearch → Bestätigung
```

**Kosten**: ~€0.006/Minute (Whisper) + ~€0.01/Query (GPT-4 Mini)

---

##### 5️⃣ Predictive Search / "Das brauchen Sie bald" (Phase 2 ◐)

> **Konzept**: Vorhersage basierend auf Verbrauchsmustern

```
┌─────────────────────────────────────────────────────────────────┐
│                    🔮 PREDICTIVE REORDERING                      │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Analyse: Kunde kauft alle 6 Wochen Schleifscheiben             │
│                                                                  │
│  📅 TIMELINE                                                     │
│  ─────────────────────────────────────────────────────────────  │
│  Jan    Feb    Mär    Apr    Mai    Jun    Jul                  │
│   ●      ●      ●      ●      ●      ●      ?                   │
│                                                                  │
│  💡 PROAKTIVE BENACHRICHTIGUNG:                                 │
│  "Sie haben vor 5 Wochen Schleifscheiben bestellt.              │
│   Nachbestellung fällig? [Jetzt bestellen] [Erinnere mich]"     │
│                                                                  │
│  🛒 SMART CART SUGGESTIONS:                                     │
│  "Kunden mit ähnlichem Warenkorb kauften auch:"                 │
│  • Schleifvlies (passt zur Schleifmaschine)                     │
│  • Staubsaugerbeutel (wird oft vergessen)                       │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

**Datenmodell für Predictions:**
```typescript
interface CustomerPurchasePattern {
  customer_id: string;
  product_id: string;
  avg_reorder_days: number;
  last_order_date: Date;
  predicted_next_order: Date;
  confidence: number;  // 0.0 - 1.0
  quantity_trend: 'stable' | 'increasing' | 'decreasing';
}
```

---

##### 6️⃣ Search Merchandising (Phase 2 ◐)

> **Konzept**: Marketing-gesteuerte Produktplatzierung in Suchergebnissen

```
┌─────────────────────────────────────────────────────────────────┐
│  🔍 akkuschrauber                                               │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ⭐ GESPONSERT                                                   │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │ 🏷️ AKTION: Bosch GSR 18V-55 - Jetzt €20 sparen!        │    │
│  │    Nur noch diese Woche! [Zum Angebot]                  │    │
│  └─────────────────────────────────────────────────────────┘    │
│                                                                  │
│  📦 ERGEBNISSE                                                   │
│  1. Makita DDF453 - €129 ★★★★★                                  │
│  2. Bosch GSR 12V - €89  ★★★★☆                                  │
│  3. DeWalt DCD771 - €99  ★★★★☆                                  │
│                                                                  │
│  📊 VERGLEICHSTABELLE                                           │
│  [Produkte vergleichen] [Alle Filter anzeigen]                  │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

**Admin-Interface für Merchandising:**
```typescript
interface SearchMerchandisingRule {
  id: string;
  name: string;
  trigger: {
    type: 'query' | 'category' | 'brand' | 'date_range';
    value: string | string[];
  };
  action: {
    type: 'boost' | 'bury' | 'pin_position' | 'banner';
    target_products?: string[];
    boost_factor?: number;
    banner_html?: string;
  };
  schedule: {
    start: Date;
    end: Date;
  };
  priority: number;
}

// Beispiel: Black Friday Aktion
const blackFridayRule: SearchMerchandisingRule = {
  id: 'bf-2026',
  name: 'Black Friday Deals',
  trigger: { type: 'date_range', value: ['2026-11-27', '2026-11-30'] },
  action: { 
    type: 'boost', 
    target_products: ['BF-DEAL-*'],
    boost_factor: 2.0 
  },
  schedule: { start: new Date('2026-11-27'), end: new Date('2026-11-30') },
  priority: 100
};
```

---

##### 7️⃣ Zero-Result-Handling (MVP ✓)

> **Niemals "Keine Ergebnisse" ohne Hilfe!**

```
┌─────────────────────────────────────────────────────────────────┐
│  🔍 xyz123abc (keine Treffer)                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  😕 Keine Ergebnisse für "xyz123abc"                            │
│                                                                  │
│  💡 VORSCHLÄGE:                                                  │
│  ───────────────────────────────────────────────────────────    │
│  • Prüfen Sie die Schreibweise                                  │
│  • Versuchen Sie allgemeinere Begriffe                          │
│  • Suchen Sie nach Artikelnummer ohne Sonderzeichen             │
│                                                                  │
│  🔥 BELIEBTE PRODUKTE IN IHRER BRANCHE:                         │
│  ───────────────────────────────────────────────────────────    │
│  [Produkt 1] [Produkt 2] [Produkt 3]                            │
│                                                                  │
│  📞 NICHT GEFUNDEN? WIR HELFEN!                                 │
│  ───────────────────────────────────────────────────────────    │
│  [Chat starten] [Rückruf anfordern] [Anfrage senden]            │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

##### 8️⃣ Barcode/QR-Code Scanner (Phase 2 ◐)

> **Schneller als Tippen!**

```
┌─────────────────────────────────────────────────────────────────┐
│                    📱 BARCODE SCANNER                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Unterstützte Codes:                                            │
│  ├─ EAN-13 / EAN-8 (Produktbarcodes)                            │
│  ├─ UPC-A / UPC-E (US-Produkte)                                 │
│  ├─ Code 128 / Code 39 (Industrie)                              │
│  ├─ QR-Code (Produktlinks, Artikelnummern)                      │
│  └─ DataMatrix (GS1 für Medizin/Pharma)                         │
│                                                                  │
│  📷 → [4006381333238] → Bosch GSR 18V-55 gefunden!              │
│                                                                  │
│  Vorteile B2B:                                                   │
│  ✓ Nachbestellung durch Scannen leerer Verpackung              │
│  ✓ Inventur-Unterstützung                                       │
│  ✓ Lieferschein-Kontrolle                                       │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

**Implementation:**
```typescript
import { BrowserMultiFormatReader } from '@zxing/library';

const codeReader = new BrowserMultiFormatReader();

async function scanBarcode(videoElement: HTMLVideoElement): Promise<string> {
  const result = await codeReader.decodeOnceFromVideoDevice(undefined, videoElement);
  return result.getText();  // z.B. "4006381333238"
}

// API-Suche nach EAN
async function searchByEan(ean: string) {
  return await searchApi.get('/products', { 
    params: { ean, exact: true } 
  });
}
```

**Kosten**: Kostenlos (Client-seitige Library)

---

##### 9️⃣ Conversational Search / KI-Berater (Phase 3 ○)

> **"Wie ein erfahrener Fachverkäufer"**

```
┌─────────────────────────────────────────────────────────────────┐
│                    🤖 KI-PRODUKTBERATER                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  👤 "Ich brauche einen Akkuschrauber für                        │
│      gelegentliche Heimwerkerarbeiten, nicht zu teuer"          │
│                                                                  │
│  🤖 "Für gelegentliche Heimwerkerarbeiten empfehle ich:         │
│                                                                  │
│      1. Bosch EasyDrill 12 (€79)                                │
│         ✓ Leicht, handlich, ausreichend Leistung                │
│         ✓ Ideal für Möbelmontage, Bilder aufhängen              │
│                                                                  │
│      2. Makita DF331D (€89)                                     │
│         ✓ Etwas mehr Power für härtere Materialien              │
│         ✓ Gutes Preis-Leistungs-Verhältnis                      │
│                                                                  │
│      Haben Sie bestimmte Materialien (Holz, Metall, Beton)?     │
│      Das hilft mir, genauer zu empfehlen."                      │
│                                                                  │
│  👤 [Antwort eingeben...] [Bosch EasyDrill ansehen]             │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

**Technologie**: RAG (Retrieval-Augmented Generation) mit Produktdaten

```typescript
async function conversationalSearch(userQuery: string, context: ChatContext) {
  // 1. Semantic Search für relevante Produkte
  const products = await elasticSearch({
    query: { knn: { embedding: await embed(userQuery) }},
    size: 10
  });
  
  // 2. LLM mit Produktkontext
  const response = await openai.chat.completions.create({
    model: 'gpt-4',
    messages: [
      { role: 'system', content: `Du bist ein erfahrener Fachverkäufer. 
        Verfügbare Produkte: ${JSON.stringify(products)}` },
      ...context.history,
      { role: 'user', content: userQuery }
    ]
  });
  
  return response.choices[0].message.content;
}
```

**Kosten**: ~€0.03-0.10 pro Konversation (GPT-4)

---

#### Feature-Priorisierung für B2X

| Feature | Aufwand | Impact | ROI | Phase |
|---------|---------|--------|-----|-------|
| Autocomplete + Preview | 3 Tage | ⭐⭐⭐⭐⭐ | Hoch | **MVP** |
| **Synonym-/Abkürzungs-Handling** | 3 Tage | ⭐⭐⭐⭐⭐ | **Kritisch** | **MVP** |
| Typo-Korrektur | 2 Tage | ⭐⭐⭐⭐ | Hoch | **MVP** |
| Zero-Result-Handling | 1 Tag | ⭐⭐⭐⭐ | Hoch | **MVP** |
| Compound-Word Dekomposition | 2 Tage | ⭐⭐⭐⭐ | Hoch | **MVP** |
| Barcode-Scanner | 3 Tage | ⭐⭐⭐⭐ | Mittel | Phase 2 |
| **OCR Typenschild-Scanner** | 1 Woche | ⭐⭐⭐⭐⭐ | **Hoch** | Phase 2 |
| Personalisierung | 2 Wochen | ⭐⭐⭐⭐ | Mittel | Phase 2 |
| Search Merchandising | 1 Woche | ⭐⭐⭐ | Mittel | Phase 2 |
| Voice Search | 1 Woche | ⭐⭐⭐ | Niedrig | Phase 2 |
| Predictive Reorder | 2 Wochen | ⭐⭐⭐⭐ | Hoch | Phase 2 |
| Visual Search (CLIP) | 2 Wochen | ⭐⭐⭐ | Mittel | Phase 2/3 |
| Camera Scanner (Live) | 3 Wochen | ⭐⭐⭐ | Niedrig | Phase 3 |
| KI-Berater | 4 Wochen | ⭐⭐⭐⭐ | Mittel | Phase 3 |

---

#### Wettbewerbs-Differenzierung B2X

```
┌────────────────────────────────────────────────────────────────────────────┐
│                    B2X SEARCH DIFFERENZIERUNG                               │
├────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  WAS ALLE HABEN:              WAS B2X BESSER MACHT:                        │
│  ══════════════════          ═══════════════════════════                   │
│  • Volltextsuche              • Multi-Tenant pre-localized                 │
│  • Filter                     • B2B-spezifische Facetten                   │
│  • Autocomplete               • Hybrid Search (BM25 + kNN)                 │
│                               • Staffelpreis-aware Suche                   │
│                               • Flexible Pricing-Strategie (s.u.)          │
│                               • Multi-Warehouse Verfügbarkeit              │
│                               • BMEcat-native Datenmodell                  │
│                               • **Branchenspezifische Synonyme**           │
│                               • **Abkürzungs-Expansion (WT→Waschtisch)**   │
│                               • **Compound-Word Dekomposition**            │
│                               • **OCR Typenschild-Scanner**                │
│                               • **Seriennummern-Tracking**                 │
│                                                                             │
│  UNIQUE SELLING POINTS:                                                     │
│  ═════════════════════                                                      │
│  🎯 "Suche versteht B2B" - Artikelnummern, OEM-Nummern, EANs              │
│  🎯 "Preis passt zum Kunden" - Flexible Konditionierung (siehe unten)     │
│  🎯 "Sofort lieferbar" - Lagerstatus aller Standorte                      │
│  🎯 "Spricht Ihre Sprache" - Pre-localized, keine Übersetzungsverzögerung │
│  🎯 "Typenschild scannen" - OCR erkennt Artikel, Serie, techn. Daten      │
│  🎯 "Meine Geräte" - Seriennummern-Tracking mit Wartungs-Erinnerung       │
│                                                                             │
└────────────────────────────────────────────────────────────────────────────┘
```

#### Pricing-Strategie: Einfach vs. ERP-Konditioniert

> **Problem**: Nicht alle Kunden haben denselben Preis!

```
┌────────────────────────────────────────────────────────────────────────────┐
│                    PRICING-STRATEGIEN                                       │
├────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  STRATEGIE A: "Index-Preise"          STRATEGIE B: "ERP-Live-Preise"       │
│  ═════════════════════════════        ══════════════════════════════       │
│                                                                             │
│  Für: Einfache Tenants                Für: Individuelle Konditionierung    │
│  ─────────────────────────────        ──────────────────────────────────   │
│  • Listenpreise im Index              • Nur "ab-Preis" im Index            │
│  • Staffelpreise im Index             • Echter Preis via ERP-API           │
│  • Kundengruppen-Preise               • Rahmenverträge                     │
│  • Schnell (keine API-Calls)          • Projekt-Konditionen                │
│                                       • Tagespreise (Metall, Rohstoffe)    │
│                                       • Währungsumrechnung live            │
│                                                                             │
│  Latenz: ~50ms                        Latenz: ~200-500ms (ERP-Call)        │
│  Accuracy: 95%                        Accuracy: 100%                        │
│                                                                             │
└────────────────────────────────────────────────────────────────────────────┘
```

##### Strategie A: Preise im Index (Standard)

```json
{
  "variant_id": "var-123",
  "prices": {
    "list_price": 99.99,
    "sale_price": 79.99,
    "customer_group_prices": {
      "retailer": 69.99,
      "wholesaler": 59.99
    },
    "quantity_breaks": [
      { "min_qty": 10, "price": 74.99 },
      { "min_qty": 50, "price": 64.99 },
      { "min_qty": 100, "price": 54.99 }
    ]
  }
}
```

**Anzeige in Suchergebnissen:**
```
Bosch GSR 18V-55
€99,99 (Listenpreis)
Ab €54,99 bei 100+ Stück
```

##### Strategie B: ERP-Live-Preise (Enterprise)

```typescript
// Suchergebnis enthält nur Platzhalter
interface SearchResult {
  variant_id: string;
  name: string;
  // Kein Preis! Nur Indikator
  price_indicator: {
    type: 'erp_lookup';           // Muss vom ERP geholt werden
    base_price?: number;          // Optional: "ab X €" für Orientierung
    requires_login: boolean;      // Preis nur für eingeloggte Kunden
  };
}

// Nach Suche: Batch-Abfrage ans ERP
async function enrichWithErpPrices(
  results: SearchResult[], 
  customerId: string
): Promise<SearchResultWithPrice[]> {
  
  const variantIds = results.map(r => r.variant_id);
  
  // ERP-Connector: Batch-Preisabfrage
  const prices = await erpConnector.getPrices({
    customer_id: customerId,
    variant_ids: variantIds,
    include_availability: true
  });
  
  return results.map(r => ({
    ...r,
    customer_price: prices[r.variant_id]?.price,
    availability: prices[r.variant_id]?.availability
  }));
}
```

**Anzeige in Suchergebnissen:**
```
┌─────────────────────────────────────────────────────────────────┐
│  🔍 bosch akkuschrauber                                         │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Während Preise geladen werden:                                 │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │ Bosch GSR 18V-55                                        │    │
│  │ ████████░░ Preis wird geladen...                        │    │
│  │ ✓ 23 Stück verfügbar                                    │    │
│  └─────────────────────────────────────────────────────────┘    │
│                                                                  │
│  Nach ERP-Response (~200ms):                                    │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │ Bosch GSR 18V-55                                        │    │
│  │ €67,50 (Ihr Preis)  ░░ UVP €99,99                       │    │
│  │ ✓ 23 Stück verfügbar                                    │    │
│  └─────────────────────────────────────────────────────────┘    │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

##### Hybrid-Ansatz: Tenant-Konfiguration

```typescript
// Tenant-Settings bestimmen Pricing-Strategie
interface TenantPricingConfig {
  strategy: 'index' | 'erp_live' | 'hybrid';
  
  // Für 'hybrid': Welche Kunden brauchen ERP-Preise?
  erp_lookup_conditions?: {
    customer_has_framework_contract: boolean;  // Rahmenvertrag
    product_categories?: string[];             // Bestimmte Kategorien
    price_volatility_threshold?: number;       // Volatile Preise (Metall)
  };
  
  // Fallback wenn ERP nicht erreichbar
  fallback: 'show_list_price' | 'hide_price' | 'show_range';
  
  // Cache-Dauer für ERP-Preise (Session-basiert)
  erp_price_cache_ttl_seconds: number;  // z.B. 300 = 5 Min
}

// Beispiel: Hybrid-Konfiguration
const hybridConfig: TenantPricingConfig = {
  strategy: 'hybrid',
  erp_lookup_conditions: {
    customer_has_framework_contract: true,  // Nur bei Rahmenvertrag
    product_categories: ['metall', 'kabel'], // Volatile Kategorien
    price_volatility_threshold: 5.0          // >5% Schwankung/Woche
  },
  fallback: 'show_range',  // "€50-80" wenn ERP down
  erp_price_cache_ttl_seconds: 300
};
```

##### Performance-Optimierung für ERP-Preise

```typescript
// 1. Prefetching: Preise vorladen während User tippt
async function prefetchPricesOnAutocomplete(query: string, customerId: string) {
  const topProducts = await quickSearch(query, { size: 10 });
  
  // Im Hintergrund ERP-Preise holen
  erpPriceCache.prefetch(topProducts.map(p => p.variant_id), customerId);
}

// 2. Skeleton Loading: UX während ERP-Call
<template>
  <div class="product-card">
    <h3>{{ product.name }}</h3>
    
    <div v-if="priceLoading" class="price-skeleton">
      <div class="skeleton-bar" />
    </div>
    
    <div v-else class="price">
      <span class="customer-price">{{ formatPrice(customerPrice) }}</span>
      <span class="list-price">UVP {{ formatPrice(listPrice) }}</span>
    </div>
  </div>
</template>

// 3. Batch-Requests: Alle Preise auf einmal
const BATCH_SIZE = 50;

async function loadPricesForPage(variantIds: string[]) {
  // Ein Request für alle sichtbaren Produkte
  return await erpConnector.batchGetPrices({
    customer_id: currentCustomer.id,
    variant_ids: variantIds.slice(0, BATCH_SIZE)
  });
}
```

##### Entscheidungsmatrix: Welche Strategie?

| Kriterium | Index-Preise | ERP-Live | Hybrid |
|-----------|--------------|----------|--------|
| **Einfache Preislisten** | ✅ Ideal | ❌ Overkill | ◐ |
| **Kundengruppen-Rabatte** | ✅ Gut | ✅ Gut | ✅ Gut |
| **Individuelle Rahmenverträge** | ❌ Nicht möglich | ✅ Ideal | ✅ Gut |
| **Tagespreise (Börse)** | ❌ Veraltet | ✅ Aktuell | ✅ Selektiv |
| **Projekt-Konditionen** | ❌ Nicht möglich | ✅ Ideal | ✅ Gut |
| **Performance** | ⚡ 50ms | 🐢 200-500ms | ⚡/🐢 Mix |
| **ERP-Abhängigkeit** | Keine | Hoch | Mittel |
| **Offline-Fähigkeit** | ✅ Voll | ❌ Keine | ◐ Teilweise |

**Empfehlung:**
- **Kleine Händler**: Index-Preise (Strategie A)
- **Mittelstand mit Stammkunden**: Hybrid
- **Industrie/Großhandel**: ERP-Live (Strategie B)

---

## 3. Use Cases & Queries

### 3.1 Navigation Menu (Kategorien)

```csharp
// Alle Root-Kategorien mit Kinderzahl abrufen
GET /api/search?type=category&parent_id=null&include_children=true

// Response:
{
  "categories": [
    {
      "id": "cat-1",
      "name": "Elektronik",
      "slug": "elektronik",
      "category_type": "navigation",
      "children_count": 5,
      "children": [
        { "id": "cat-2", "name": "Computer", "children_count": 3 },
        { "id": "cat-3", "name": "Smartphones", "children_count": 2 }
      ]
    },
    {
      "id": "cat-promo-1",
      "name": "Angebote",
      "slug": "angebote",
      "category_type": "promotion",
      "children_count": 0,
      "product_count": 234
    }
  ]
}
```

### 3.1.1 Category Graph Navigation

**Graph-aware Breadcrumb (Primary Path):**

```csharp
// Produkt kann mehrere Pfade haben, aber nur einer ist "primary"
GET /api/products/{id}/breadcrumb

// Response:
{
  "primary_path": [
    { "id": "cat-1", "name": "Werkzeug", "slug": "werkzeug" },
    { "id": "cat-5", "name": "Elektrowerkzeug", "slug": "elektrowerkzeug" },
    { "id": "cat-23", "name": "Bohrmaschinen", "slug": "bohrmaschinen" }
  ],
  "additional_categories": [
    { "id": "cat-angebote", "name": "Angebote", "type": "promotion" },
    { "id": "cat-neuheiten", "name": "Neuheiten", "type": "virtual" }
  ]
}
```

**Kategorie-abhängige Produktansicht:**

```csharp
// Produkt in Kontext einer bestimmten Kategorie anzeigen
GET /api/categories/angebote/products/{productId}

// Breadcrumb zeigt: Angebote > Bosch Bohrmaschine
// Statt: Werkzeug > Elektrowerkzeug > Bohrmaschinen > Bosch Bohrmaschine
```

**Virtuelle Kategorien (dynamisch berechnet):**

```csharp
public class VirtualCategoryService
{
    // Neuheiten: Produkte der letzten 30 Tage
    public async Task<ProductAssignment[]> GetNewArrivalsAsync(Guid tenantId)
    {
        var query = new SearchRequest("products")
        {
            Query = new RangeQuery 
            { 
                Field = "created_at", 
                Gte = DateTime.UtcNow.AddDays(-30) 
            },
            Sort = new[] { new FieldSort { Field = "created_at", Order = SortOrder.Descending } }
        };
        // ...
    }
    
    // Bestseller: Top 100 nach Verkaufszahlen
    public async Task<ProductAssignment[]> GetBestsellersAsync(Guid tenantId)
    {
        var query = new SearchRequest("products")
        {
            Query = new MatchAllQuery(),
            Sort = new[] { new FieldSort { Field = "sales_count_30d", Order = SortOrder.Descending } },
            Size = 100
        };
        // ...
    }
}
```

**Elasticsearch Query für virtuelle Kategorie "Angebote":**

```json
{
  "query": {
    "bool": {
      "filter": [
        { "term": { "doc_type": "product" } },
        { "range": { "discount_percent": { "gt": 0 } } },
        { "range": { "promotion_end_date": { "gte": "now" } } }
      ]
    }
  },
  "sort": [
    { "discount_percent": "desc" },
    { "popularity_score": "desc" }
  ]
}
```

**Elasticsearch Query:**
```json
{
  "query": {
    "bool": {
      "filter": [
        { "term": { "doc_type": "category" } },
        { "term": { "level": 0 } }
      ]
    }
  },
  "aggs": {
    "categories": {
      "terms": { "field": "path" },
      "aggs": {
        "top_hit": { "top_hits": { "size": 1 } }
      }
    }
  }
}
```

### 3.2 Produktsuche mit Facetten (Graph-aware)

```csharp
// Suche "Laptop" mit Facetten
// WICHTIG: Facetten zählen Produkte über ALLE Kategorie-Zuordnungen
GET /api/search?q=laptop&type=product&facets=true

// Response:
{
  "products": [...],
  "total": 156,
  "facets": {
    // Kategorien: Produkt wird in JEDER zugeordneten Kategorie gezählt
    "categories": [
      { "key": "Laptops", "count": 120, "type": "navigation" },
      { "key": "Gaming Laptops", "count": 36, "type": "navigation" },
      { "key": "Angebote", "count": 23, "type": "promotion" },     // Überlappend!
      { "key": "Neuheiten", "count": 12, "type": "virtual" }       // Überlappend!
    ],
    "brands": [
      { "key": "Dell", "count": 45 },
      { "key": "HP", "count": 38 }
    ],
    "attributes": {
      "Bildschirmgröße": [
        { "key": "15.6 Zoll", "count": 67 },
        { "key": "14 Zoll", "count": 43 }
      ],
      "RAM": [
        { "key": "16 GB", "count": 89 },
        { "key": "32 GB", "count": 42 }
      ]
    },
    "price_ranges": [
      { "key": "500-1000€", "count": 45 },
      { "key": "1000-1500€", "count": 67 }
    ]
  }
}
```

### 3.3 Semantische Suche

```csharp
// Natürlichsprachliche Anfrage
GET /api/search?q=ich brauche etwas um musik zu hören wenn ich jogge&semantic=true

// Der Query-Embedding-Vektor wird generiert
// Findet: Bluetooth Kopfhörer, Sport-Earbuds, wasserdichte In-Ears
// Auch ohne exakte Keyword-Matches!
```

### 3.4 Unified Search (Multi-Type)

```csharp
// Suche über alle Typen für Autocomplete
GET /api/search/suggest?q=sam&types=product,category,brand

// Response:
{
  "suggestions": [
    { "type": "brand", "name": "Samsung", "count": 342 },
    { "type": "category", "name": "Samsung Smartphones", "count": 89 },
    { "type": "product", "name": "Samsung Galaxy S24", "price": 899 }
  ]
}
```

---

## 4. Data Pipeline

### 4.1 Catalog Preparation Workflow

```
┌──────────────────────────────────────────────────────────────────────┐
│                    WOLVERINE MESSAGE FLOW                             │
├──────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  1. ProductImported (Domain Event)                                   │
│     └→ LocalizeProductHandler                                        │
│         • Fetch translations for all supported languages             │
│         • Apply language-specific formatters (price, dates)          │
│         • Output: LocalizedProduct{de}, LocalizedProduct{en}, ...    │
│                                                                       │
│  2. ProductLocalized (Domain Event)                                  │
│     └→ GenerateEmbeddingHandler                                      │
│         • Generate semantic embedding for localized content          │
│         • Use Azure OpenAI / Local ONNX model                        │
│         • Output: LocalizedProductWithEmbedding                      │
│                                                                       │
│  3. ProductEmbeddingGenerated (Domain Event)                         │
│     └→ IndexProductHandler                                           │
│         • Transform to SearchDocument                                │
│         • Bulk index to Elasticsearch                                │
│         • Index: b2x_{tenant}_{language}                             │
│                                                                       │
│  4. ProductIndexed (Domain Event)                                    │
│     └→ CacheInvalidationHandler                                      │
│         • Invalidate related caches                                  │
│         • Notify connected clients (SignalR)                         │
│                                                                       │
└──────────────────────────────────────────────────────────────────────┘
```

### 4.2 Indexing Strategy

```csharp
public class CatalogIndexingService
{
    public async Task IndexCatalogAsync(Guid tenantId, string language)
    {
        var indexName = $"b2x_{tenantId}_{language}";
        var aliasName = $"b2x_{tenantId}_{language}_current";
        
        // 1. Create new index with timestamp
        var newIndex = $"{indexName}_{DateTime.UtcNow:yyyyMMddHHmmss}";
        await CreateIndexAsync(newIndex);
        
        // 2. Bulk index all documents
        await BulkIndexProductsAsync(tenantId, language, newIndex);
        await BulkIndexCategoriesAsync(tenantId, language, newIndex);
        await BulkIndexBrandsAsync(tenantId, language, newIndex);
        
        // 3. Atomic alias switch (zero-downtime)
        await SwitchAliasAsync(aliasName, newIndex);
        
        // 4. Cleanup old indices
        await DeleteOldIndicesAsync(indexName, keepLast: 2);
    }
}
```

---

## 5. Performance-Optimierungen

### 5.1 Index-Einstellungen

```json
{
  "settings": {
    "number_of_shards": 2,
    "number_of_replicas": 1,
    "refresh_interval": "1s",
    "analysis": {
      "analyzer": {
        "german_custom": {
          "type": "custom",
          "tokenizer": "standard",
          "filter": ["lowercase", "german_normalization", "german_stemmer"]
        }
      }
    },
    "index": {
      "knn": true,
      "knn.algo_param.ef_search": 100
    }
  }
}
```

### 5.2 Caching-Strategie

```
┌─────────────────────────────────────────────────────────────────┐
│                      CACHING LAYERS                              │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  L1: Elasticsearch Query Cache (automatic)                      │
│      • Identical queries cached at shard level                  │
│      • Invalidated on index refresh                             │
│                                                                  │
│  L2: Redis Cache (application level)                            │
│      • Navigation menu: 5 min TTL                               │
│      • Category tree: 5 min TTL                                 │
│      • Popular searches: 1 min TTL                              │
│      • Product details: 1 min TTL                               │
│                                                                  │
│  L3: CDN/Edge Cache (for static results)                        │
│      • Category pages: 1 min TTL                                │
│      • Brand pages: 1 min TTL                                   │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

### 5.3 Query-Optimierungen

```csharp
public class SearchQueryBuilder
{
    public SearchRequest BuildOptimizedQuery(SearchRequest request)
    {
        return new SearchRequest(request.IndexName)
        {
            // 1. Use filter context for non-scoring clauses
            Query = new BoolQuery
            {
                Must = BuildScoringQueries(request),
                Filter = BuildFilterQueries(request)  // No scoring overhead
            },
            
            // 2. Source filtering - only return needed fields
            Source = new SourceFilter
            {
                Includes = new[] { "id", "name", "price", "image_url", "slug" }
            },
            
            // 3. Terminate early for count-only queries
            TerminateAfter = request.CountOnly ? 10000 : null,
            
            // 4. Aggregations only when needed
            Aggregations = request.IncludeFacets ? BuildAggregations() : null
        };
    }
}
```

---

## 6. API Design

### 6.1 Unified Search Endpoint

```csharp
[ApiController]
[Route("api/search")]
public class UnifiedSearchController : ControllerBase
{
    /// <summary>
    /// Unified search across all catalog entities
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<UnifiedSearchResponse>> SearchAsync(
        [FromQuery] string q,
        [FromQuery] string[] types = null,        // product, category, brand
        [FromQuery] string language = "de",
        [FromQuery] bool semantic = false,
        [FromQuery] bool facets = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string[] filters = null)      // category:laptops, brand:dell
    {
        // Implementation...
    }
    
    /// <summary>
    /// Search suggestions (autocomplete)
    /// </summary>
    [HttpGet("suggest")]
    public async Task<ActionResult<SuggestionResponse>> SuggestAsync(
        [FromQuery] string q,
        [FromQuery] string[] types = null,
        [FromQuery] string language = "de",
        [FromQuery] int limit = 10)
    {
        // Implementation...
    }
    
    /// <summary>
    /// Get category tree for navigation
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<CategoryTreeResponse>> GetCategoriesAsync(
        [FromQuery] string? parentId = null,
        [FromQuery] int depth = 2,
        [FromQuery] string language = "de")
    {
        // Implementation...
    }
}
```

### 6.2 Response DTOs

```csharp
public record UnifiedSearchResponse
{
    public ProductSearchResult[] Products { get; init; }
    public CategorySearchResult[] Categories { get; init; }
    public BrandSearchResult[] Brands { get; init; }
    public FacetResult Facets { get; init; }
    public SearchMetadata Metadata { get; init; }
}

public record CategorySearchResult
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string Slug { get; init; }
    public string[] Path { get; init; }           // ["Elektronik", "Computer", "Laptops"]
    public string ParentId { get; init; }
    public int Level { get; init; }
    public int ProductCount { get; init; }
    public CategorySearchResult[] Children { get; init; }
}

public record FacetResult
{
    public FacetBucket[] Categories { get; init; }
    public FacetBucket[] Brands { get; init; }
    public Dictionary<string, FacetBucket[]> Attributes { get; init; }
    public PriceRangeBucket[] PriceRanges { get; init; }
}
```

---

## 7. Skalierbarkeit

### 7.1 Horizontale Skalierung

```
Scenario: 5M Products, 3M Variants, 50M Attributes, 100 Tenants, 8 Languages

Berechnung:
- Documents per Tenant/Language: ~5M products + 50K categories + 5K brands = ~5.05M docs
- Total Documents: 100 tenants × 8 languages × 5.05M = ~4 Billion docs

Index-Strategie:
- Index per Tenant/Language: b2x_{tenant}_{lang}
- 800 Indices (100 × 8)
- ~5M docs per Index

Elasticsearch Cluster:
- 3 Master Nodes (dedicated)
- 6 Hot Data Nodes (NVMe, 128GB RAM, 32 vCPUs)
- 3 Warm Data Nodes (SSD, 64GB RAM)
- 2 Coordinating Nodes (query routing)

Sharding:
- 2-3 Primary Shards per Index (target: 20-30GB per shard)
- 1 Replica per Shard
```

### 7.2 Multi-Tenant Isolation

```csharp
public class TenantAwareSearchService
{
    public async Task<SearchResponse> SearchAsync(
        Guid tenantId, 
        string language, 
        SearchRequest request)
    {
        // 1. Determine index name
        var indexAlias = $"b2x_{tenantId}_{language}_current";
        
        // 2. Add tenant filter (belt and suspenders)
        request.Query = new BoolQuery
        {
            Must = new[] { request.Query },
            Filter = new[] 
            { 
                new TermQuery { Field = "tenant_id", Value = tenantId.ToString() }
            }
        };
        
        // 3. Execute on tenant-specific index
        return await _client.SearchAsync(request, indexAlias);
    }
}
```

---

## 8. Migration Plan

### Phase 1: Foundation (2 Wochen)
- [ ] Unified Index Mapping erstellen
- [ ] Lokalisierungs-Pipeline implementieren
- [ ] Embedding-Service integrieren (Azure OpenAI / Local)

### Phase 2: Indexing (2 Wochen)  
- [ ] Wolverine Message Handlers für Indexierung
- [ ] Bulk-Import für bestehende Katalogdaten
- [ ] Zero-Downtime Reindexing

### Phase 3: API (1 Woche)
- [ ] UnifiedSearchController implementieren
- [ ] Category Tree Endpoint
- [ ] Suggestion/Autocomplete Endpoint

### Phase 4: Frontend Integration (2 Wochen)
- [ ] Navigation auf Search API umstellen
- [ ] Produktlisting auf Search API umstellen
- [ ] Facetten-Filter implementieren
- [ ] Semantische Suche aktivieren

### Phase 5: Optimization (1 Woche)
- [ ] Query Performance Tuning
- [ ] Caching Layer
- [ ] Monitoring & Alerting

---

## 9. Risiken & Mitigationen

| Risiko | Wahrscheinlichkeit | Impact | Mitigation |
|--------|-------------------|--------|------------|
| Embedding-Kosten (API) | Mittel | Mittel | Local ONNX Model, Batch-Processing |
| Index-Größe | Niedrig | Mittel | Quantized Vectors (int8), Flattened Attributes |
| Konsistenz (Eventual) | Mittel | Niedrig | Event Sourcing, Idempotente Handler |
| Query-Latenz | Niedrig | Hoch | Caching, Query Optimization, Replicas |
| Migration Downtime | Mittel | Hoch | Blue-Green Deployment, Alias Switching |

---

## 10. Erfolgskriterien

- [ ] **Performance**: P99 Query Latency < 100ms
- [ ] **Durchgängigkeit**: 100% der Shop-Daten aus Search API
- [ ] **Semantic Accuracy**: Relevante Ergebnisse bei natürlichsprachlichen Anfragen
- [ ] **Skalierbarkeit**: Linear mit Tenant-/Produktanzahl
- [ ] **Lokalisierung**: Zero Runtime Translation Overhead

---

## 11. Index Operations & Deployment

### 11.1 Versionierung & Alias-Strategie

**Problem**: Index-Schema-Änderungen erfordern Reindexierung. Wie ohne Downtime?

**Lösung: Versioned Indices + Aliases**

```
# Physische Indices (versioniert)
b2x_acme_de_v1  ← alte Version
b2x_acme_de_v2  ← neue Version (Reindexierung)

# Aliases (zeigen auf aktive Version)
b2x_acme_de     → b2x_acme_de_v2  (READ + WRITE)
b2x_acme_de_read  → b2x_acme_de_v2
b2x_acme_de_write → b2x_acme_de_v2
```

**Deployment-Workflow**:
```bash
# 1. Neuen Index erstellen
PUT b2x_acme_de_v2 { ... neues Mapping ... }

# 2. Reindexieren (async)
POST _reindex { "source": "b2x_acme_de_v1", "dest": "b2x_acme_de_v2" }

# 3. Alias atomisch umschalten
POST _aliases {
  "actions": [
    { "remove": { "index": "b2x_acme_de_v1", "alias": "b2x_acme_de" }},
    { "add": { "index": "b2x_acme_de_v2", "alias": "b2x_acme_de" }}
  ]
}

# 4. Alten Index löschen (nach Verifizierung)
DELETE b2x_acme_de_v1
```

### 11.2 Sync-Strategie

| Trigger | Strategie | Latenz |
|---------|-----------|--------|
| **Produktänderung** | Event-Driven (Wolverine) | < 5s |
| **Preisänderung** | Event-Driven | < 5s |
| **Bestandsänderung** | Event-Driven (High Priority) | < 2s |
| **Vollständige Reindexierung** | Scheduled (Nightly) | N/A |
| **Neue Sprache** | Manual Trigger | Minutes |

**Event-Driven Sync (Wolverine)**:
```csharp
public class ProductUpdatedHandler : IWolverineHandler
{
    public async Task Handle(ProductUpdated @event, IElasticClient elastic)
    {
        // Für alle Sprachen reindexieren
        foreach (var lang in tenant.SupportedLanguages)
        {
            var doc = await PrepareLocalizedDocument(@event.ProductId, lang);
            await elastic.IndexAsync(doc, i => i.Index($"b2x_{tenant}_{lang}"));
        }
    }
}
```

### 11.3 Boosting-Strategie (Field Weights)

**Standard-Gewichtungen für Textsuche**:

```json
{
  "query": {
    "multi_match": {
      "query": "bosch bohrer 8mm",
      "fields": [
        "sku^10",              // Exakte SKU-Treffer höchste Prio
        "ean^10",              // EAN-Treffer ebenfalls
        "manufacturer_sku^8",  // Hersteller-Artikelnummer
        "name^5",              // Produktname wichtig
        "brand_name^4",        // Marke relevant
        "short_description^2", // Beschreibung weniger
        "searchable_attributes^2",
        "search_terms^3",
        "synonyms^2",
        "long_description^1"   // Volltext niedrigste Prio
      ],
      "type": "best_fields",
      "fuzziness": "AUTO"
    }
  }
}
```

#### 11.3.1 Entity-Boosting (Admin-konfigurierbar)

**Boost-Hierarchie (von spezifisch zu allgemein):**

| Ebene | Feld | Beschreibung | Beispiel |
|-------|------|--------------|----------|
| 1. **Variante** | `boosting.variant_boost` | Direkt auf Variante gesetzt | SKU "BOHR-8-PRO" = 1.5 |
| 2. **Produkt** | `boosting.product_boost` | Vom Hauptprodukt geerbt | "Bohrer-Set Profi" = 1.3 |
| 3. **Kategorie** | `boosting.category_boost` | Von Kategorien geerbt (max) | "Elektrowerkzeuge" = 1.2 |
| 4. **Marke** | `boosting.brand_boost` | Von Marke geerbt | "Bosch" = 1.4 |

**Automatische Boosts (System-berechnet):**

| Typ | Feld | Bedingung | Boost-Wert |
|-----|------|-----------|------------|
| **Lagerware** | `boosting.stock_boost` | `is_in_stock = true` | 1.5 |
| **Neuware** | `boosting.new_boost` | `is_new = true` | 1.3 |
| **Aktionsartikel** | `boosting.promo_boost` | `is_promoted = true` | 1.4 |
| **Bestseller** | - | `is_bestseller = true` | 1.2 |
| **Featured** | - | `is_featured = true` | 1.5 |

**Boost-Berechnung bei Indexierung:**

```csharp
public class BoostCalculator
{
    public BoostingData CalculateBoosts(
        Variant variant,
        Product product,
        Brand brand,
        IReadOnlyList<Category> categories)
    {
        // Entity-Boosts (Admin-definiert, Default = 1.0)
        var variantBoost = variant.BoostFactor ?? 1.0f;
        var productBoost = product.BoostFactor ?? 1.0f;
        var brandBoost = brand?.BoostFactor ?? 1.0f;
        var categoryBoost = categories.Any() 
            ? categories.Max(c => c.BoostFactor ?? 1.0f) 
            : 1.0f;
        
        // Automatische Boosts (System-berechnet)
        var stockBoost = variant.IsInStock ? 1.5f : 1.0f;
        var newBoost = variant.IsNew ? 1.3f : 1.0f;
        var promoBoost = variant.IsPromoted ? 1.4f : 1.0f;
        
        // Total = Produkt aller Boosts
        var totalBoost = variantBoost 
            * productBoost 
            * brandBoost 
            * categoryBoost 
            * stockBoost 
            * newBoost 
            * promoBoost;
        
        return new BoostingData
        {
            VariantBoost = variantBoost,
            ProductBoost = productBoost,
            CategoryBoost = categoryBoost,
            BrandBoost = brandBoost,
            StockBoost = stockBoost,
            NewBoost = newBoost,
            PromoBoost = promoBoost,
            TotalBoost = totalBoost
        };
    }
}
```

**Admin-UI für Boost-Konfiguration:**

```typescript
interface BoostConfig {
  // Entity-Boosts (0.1 - 10.0, Default: 1.0)
  variantBoosts: Map<string, number>;   // SKU → Boost
  productBoosts: Map<string, number>;   // ProductId → Boost
  categoryBoosts: Map<string, number>;  // CategoryId → Boost
  brandBoosts: Map<string, number>;     // BrandId → Boost
  
  // Automatische Boost-Regeln (an/aus + Stärke)
  stockBoostEnabled: boolean;
  stockBoostValue: number;              // Default: 1.5
  
  newBoostEnabled: boolean;
  newBoostValue: number;                // Default: 1.3
  
  promoBoostEnabled: boolean;
  promoBoostValue: number;              // Default: 1.4
  
  // Bestseller/Featured
  bestsellerBoostValue: number;         // Default: 1.2
  featuredBoostValue: number;           // Default: 1.5
}
```

#### 11.3.2 Elasticsearch Function Score Query

**Vollständige Boosting-Query:**

```json
{
  "query": {
    "function_score": {
      "query": {
        "bool": {
          "must": [
            {
              "multi_match": {
                "query": "bosch bohrer",
                "fields": ["name^5", "brand_name^4", "search_terms^3", "sku^10"]
              }
            }
          ],
          "filter": [
            { "term": { "visibility.is_searchable": true }}
          ]
        }
      },
      "functions": [
        // 1. PRE-CALCULATED TOTAL BOOST (empfohlen für Performance)
        {
          "field_value_factor": {
            "field": "total_boost",
            "modifier": "none",
            "missing": 1
          }
        },
        
        // 2. ODER: Einzelne Boosts (flexibler, aber langsamer)
        // Lagerware-Boost
        {
          "filter": { "term": { "is_in_stock": true }},
          "weight": 1.5
        },
        // Neuware-Boost  
        {
          "filter": { "term": { "is_new": true }},
          "weight": 1.3
        },
        // Aktionsartikel-Boost
        {
          "filter": { "term": { "is_promoted": true }},
          "weight": 1.4
        },
        // Bestseller-Boost
        {
          "filter": { "term": { "is_bestseller": true }},
          "weight": 1.2
        },
        // Featured-Boost
        {
          "filter": { "term": { "is_featured": true }},
          "weight": 1.5
        },
        
        // 3. Entity-Boosts (wenn nicht pre-calculated)
        {
          "field_value_factor": {
            "field": "boosting.brand_boost",
            "modifier": "none",
            "missing": 1
          }
        },
        {
          "field_value_factor": {
            "field": "boosting.category_boost",
            "modifier": "none",
            "missing": 1
          }
        },
        
        // 4. Popularity/Rating Boosts
        {
          "field_value_factor": {
            "field": "review_rating",
            "modifier": "log1p",
            "factor": 0.3,
            "missing": 0
          }
        },
        {
          "field_value_factor": {
            "field": "popularity_score",
            "modifier": "log1p",
            "factor": 0.1,
            "missing": 0
          }
        }
      ],
      "score_mode": "multiply",
      "boost_mode": "multiply",
      "max_boost": 10  // Verhindert extreme Boost-Explosionen
    }
  }
}
```

#### 11.3.3 Boost-Szenarien

**Szenario 1: Saisonale Aktion (Weihnachten)**
```json
// Alle Produkte in Kategorie "Geschenke" boosten
PUT /b2x_acme_de/_update_by_query
{
  "query": {
    "nested": {
      "path": "category_assignments",
      "query": { "term": { "category_assignments.category_id": "cat-geschenke" }}
    }
  },
  "script": {
    "source": "ctx._source.boosting.category_boost = 2.0; ctx._source.is_promoted = true"
  }
}
```

**Szenario 2: Marken-Promotion (Bosch-Woche)**
```json
// Alle Bosch-Produkte boosten
PUT /b2x_acme_de/_update_by_query
{
  "query": { "term": { "brand_id": "brand-bosch" }},
  "script": {
    "source": "ctx._source.boosting.brand_boost = 1.8; ctx._source.is_featured = true"
  }
}
```

**Szenario 3: Lagerräumung (Abverkauf)**
```json
// Auslaufende Produkte boosten
PUT /b2x_acme_de/_update_by_query
{
  "query": { "term": { "lifecycle.status": "discontinued" }},
  "script": {
    "source": "ctx._source.boosting.promo_boost = 1.6; ctx._source.is_promoted = true"
  }
}
```

**Szenario 4: Einzelprodukt-Push (Tagesangebot)**
```json
// Einzelne Variante für 24h boosten
POST /b2x_acme_de/_update/var-bohrer-8mm
{
  "doc": {
    "boosting": { "variant_boost": 3.0, "promo_boost": 2.0 },
    "is_featured": true,
    "is_promoted": true
  }
}
```

#### 11.3.4 Boost-Reset (nach Aktion)

```csharp
public class BoostResetJob
{
    public async Task ResetExpiredPromotions(CancellationToken ct)
    {
        // Finde alle abgelaufenen Promotions
        var expiredPromos = await _db.Promotions
            .Where(p => p.EndDate < DateTime.UtcNow && !p.IsReset)
            .ToListAsync(ct);
        
        foreach (var promo in expiredPromos)
        {
            // Reset Boost-Werte auf Default
            await _elasticClient.UpdateByQueryAsync<VariantDocument>(
                u => u.Query(q => q.Term(t => t.Field(f => f.PromoId).Value(promo.Id)))
                      .Script(s => s.Source(@"
                          ctx._source.boosting.promo_boost = 1.0;
                          ctx._source.is_promoted = false;
                          ctx._source.total_boost = /* recalculate */;
                      ")),
                ct);
            
            promo.IsReset = true;
        }
        
        await _db.SaveChangesAsync(ct);
    }
}
```

#### 11.3.5 Boost-Monitoring

**Dashboard-Metriken:**

| Metrik | Beschreibung | Alert |
|--------|--------------|-------|
| `avg(total_boost)` | Durchschnittlicher Boost | > 2.0 = Review |
| `max(total_boost)` | Maximaler Boost | > 10.0 = Warning |
| `count(is_promoted)` | Aktive Promotions | - |
| `count(is_featured)` | Featured Produkte | > 100 = Review |

**Boost-Audit-Query:**
```json
{
  "size": 0,
  "aggs": {
    "boost_distribution": {
      "histogram": {
        "field": "total_boost",
        "interval": 0.5,
        "min_doc_count": 1
      }
    },
    "top_boosted": {
      "top_hits": {
        "size": 10,
        "sort": [{ "total_boost": "desc" }],
        "_source": ["sku", "name", "total_boost", "boosting"]
      }
    },
    "promo_count": {
      "filter": { "term": { "is_promoted": true }}
    },
    "featured_count": {
      "filter": { "term": { "is_featured": true }}
    }
  }
}
```

### 11.4 Monitoring & Alerting

**Kritische Metriken**:

| Metrik | Warnung | Kritisch | Aktion |
|--------|---------|----------|--------|
| **Query Latency P99** | > 200ms | > 500ms | Query Optimization |
| **Index Lag** | > 30s | > 5min | Sync prüfen |
| **Cluster Health** | Yellow | Red | Shards prüfen |
| **Disk Usage** | > 75% | > 90% | Cleanup/Scale |
| **JVM Heap** | > 75% | > 90% | Memory Config |

**Dashboards**:
- ES Cluster Health (Grafana)
- Query Performance (Slow Query Log)
- Index Size per Tenant
- Sync Lag per Event Type

### 11.5 Backup & Recovery

**Snapshot Policy**:
```json
PUT _slm/policy/daily-snapshots
{
  "schedule": "0 0 1 * * ?",  // Täglich 01:00
  "name": "<b2x-snap-{now/d}>",
  "repository": "b2x-backup-repo",
  "config": {
    "indices": ["b2x_*"],
    "ignore_unavailable": true
  },
  "retention": {
    "expire_after": "30d",
    "min_count": 5,
    "max_count": 50
  }
}
```

**Recovery-Szenario**:
```bash
# Snapshot wiederherstellen
POST _snapshot/b2x-backup-repo/b2x-snap-2026.01.10/_restore
{
  "indices": "b2x_acme_de",
  "rename_pattern": "(.+)",
  "rename_replacement": "restored_$1"
}
```

### 11.6 Multi-Warehouse Queries

**Umkreissuche: "Nächstes Abhollager mit Bestand"**:

```json
{
  "query": {
    "bool": {
      "must": [
        { "term": { "sku": "BOHR-8" }},
        { "nested": {
            "path": "warehouses",
            "query": {
              "bool": {
                "must": [
                  { "term": { "warehouses.is_available": true }},
                  { "term": { "warehouses.pickup_available": true }},
                  { "range": { "warehouses.available_quantity": { "gte": 1 }}}
                ]
              }
            },
            "inner_hits": {
              "size": 10,
              "_source": ["warehouse_id", "warehouse_name", "available_quantity", "pickup_time_text", "address"]
            }
          }
        }
      ]
    }
  },
  "sort": [
    {
      "_geo_distance": {
        "warehouses.location": {
          "lat": 52.52,
          "lon": 13.405
        },
        "order": "asc",
        "unit": "km",
        "nested": {
          "path": "warehouses",
          "filter": {
            "bool": {
              "must": [
                { "term": { "warehouses.pickup_available": true }},
                { "term": { "warehouses.is_available": true }}
              ]
            }
          }
        }
      }
    }
  ]
}
```

**Filtern: "Nur Produkte mit Bestand in Lager München"**:

```json
{
  "query": {
    "bool": {
      "must": [
        { "match": { "name": "Bohrer" }}
      ],
      "filter": [
        {
          "nested": {
            "path": "warehouses",
            "query": {
              "bool": {
                "must": [
                  { "term": { "warehouses.warehouse_id": "wh-munich" }},
                  { "term": { "warehouses.is_available": true }}
                ]
              }
            }
          }
        }
      ]
    }
  }
}
```

**Aggregation: "Bestandsübersicht pro Lager"**:

```json
{
  "aggs": {
    "warehouses": {
      "nested": { "path": "warehouses" },
      "aggs": {
        "by_warehouse": {
          "terms": { "field": "warehouses.warehouse_id", "size": 20 },
          "aggs": {
            "name": { "terms": { "field": "warehouses.warehouse_name", "size": 1 }},
            "total_stock": { "sum": { "field": "warehouses.stock_quantity" }},
            "available_stock": { "sum": { "field": "warehouses.available_quantity" }},
            "product_count": { "reverse_nested": {} }
          }
        },
        "pickup_locations": {
          "filter": { "term": { "warehouses.pickup_available": true }},
          "aggs": {
            "by_city": {
              "terms": { "field": "warehouses.address.city", "size": 20 }
            }
          }
        }
      }
    }
  }
}
```

**Click & Collect: "Produkte heute abholbar in meiner Nähe"**:

```json
{
  "query": {
    "bool": {
      "filter": [
        {
          "nested": {
            "path": "warehouses",
            "query": {
              "bool": {
                "must": [
                  { "term": { "warehouses.pickup_available": true }},
                  { "range": { "warehouses.available_quantity": { "gte": 1 }}},
                  { "range": { "warehouses.pickup_time_hours": { "lte": 4 }}},  // Heute abholbar
                  {
                    "geo_distance": {
                      "distance": "25km",
                      "warehouses.location": { "lat": 52.52, "lon": 13.405 }
                    }
                  }
                ]
              }
            },
            "inner_hits": {
              "size": 3,
              "_source": ["warehouse_name", "pickup_time_text", "address.city"]
            }
          }
        }
      ]
    }
  }
}
```

**Bestandswarnung: "Produkte mit niedrigem Bestand pro Lager"**:

```json
{
  "query": {
    "nested": {
      "path": "warehouses",
      "query": {
        "bool": {
          "must": [
            { "term": { "warehouses.status": "low_stock" }}
          ],
          "should": [
            { 
              "script_score": {
                "query": { "match_all": {} },
                "script": {
                  "source": "doc['warehouses.reorder_level'].value - doc['warehouses.available_quantity'].value"
                }
              }
            }
          ]
        }
      },
      "inner_hits": {
        "size": 1,
        "_source": ["warehouse_id", "warehouse_name", "stock_quantity", "reorder_level"]
      }
    }
  },
  "sort": [
    { "total_stock_quantity": "asc" }  // Kritischste zuerst
  ]
}
```

---

## 12. Nächste Schritte

1. **Review durch @Architect**: Architektur-Validierung
2. **ADR erstellen**: Formelle Entscheidungsdokumentation
3. **Proof of Concept**: Kleiner Tenant mit Pre-Localization + Embeddings
4. **Performance Baseline**: Benchmark aktuelle vs. neue Architektur

---

**Erstellt**: 2026-01-11  
**Status**: Brainstorm - Feedback erwünscht  
**Nächstes Review**: @Architect, @Backend, @TechLead
