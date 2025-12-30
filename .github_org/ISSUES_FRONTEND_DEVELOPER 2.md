# Frontend Developer - Zugeordnete Issues

**Status**: 0/8 Assigned  
**Gesamtaufwand**: ~15 Story Points  
**Kritischer Pfad**: Sprint 1 (6 Issues mit P0.6)

---

## Sprint 1 (P0.6 - Legal Compliance)

| # | Titel | Punkte | Abhängigkeiten |
|---|-------|--------|-----------------|
| #41 | AGB & Datenschutz Checkbox | 2 | Legal Review (#41 from LEGAL) |
| #42 | Impressum & Datenschutz Links | 1 | Legal Review |
| #19 | Vue 3 Components Library (Basis) | 5 | - |

**Summe Sprint 1**: 8 Story Points

---

## Sprint 2 (P0.6 - Store Frontend)

| # | Titel | Punkte | Abhängigkeiten |
|---|-------|--------|-----------------|
| #33 | Price Display (Brutto mit MwSt) | 3 | Backend #20 (Price Calc) |
| #40 | Shipping Cost Transparency | 2 | Backend #20, #29 |
| #17 | Product Management UI | 3 | Backend #12 (Endpoints) |

**Summe Sprint 2**: 8 Story Points

---

## Sprint 3 (UX Enhancement)

| # | Titel | Punkte | Abhängigkeiten |
|---|-------|--------|-----------------|
| #15 | Admin Dashboard UI Framework | 5 | Backend #26 (APIs) |
| #16 | Theme Builder Component | 3 | Backend #18 (Config API) |

**Summe Sprint 3**: 8 Story Points

---

## Priorisierte Liste

```
🔴 CRITICAL (Sprint 1):
  #41 - AGB Checkbox (Legal muss Review machen)
  #42 - Impressum Links (schnell umzusetzen)
  #19 - Vue Components (Basis für alle anderen)

🟡 HIGH (Sprint 2):
  #33 - Brutto-Preisanzeige (sichtbar für Kunden)
  #40 - Shipping Transparenz (P0.6 Anforderung)
  #17 - Produkt-UI (nach Backend #12 ready)

🟡 MEDIUM (Sprint 3):
  #15 - Admin Dashboard
  #16 - Theme Builder
```

---

## Tech Stack

- ✅ Vue 3 (Composition API)
- ✅ TypeScript 5.x
- ✅ Tailwind CSS 4.1
- ✅ Vite 5.x
- ✅ Axios / Fetch API
- ✅ Pinia (State Management)
- ✅ i18n (Multi-language)
- ✅ WCAG 2.1 AA (Barrierefreiheit per #43)

---

## Abhängigkeiten von Backend

| Frontend Issue | Benötigt Backend | Status |
|----------------|-----------------|--------|
| #33 | #20 (Price Calc) | ⏳ Wartend |
| #40 | #20, #29 | ⏳ Wartend |
| #17 | #12 (Endpoints) | ⏳ Wartend |
| #15 | #26 (Admin APIs) | ⏳ Wartend |
| #16 | #18 (Theme Config) | ⏳ Wartend |

---

## Nächste Schritte

1. **1 Frontend Developer zuweisen** (8 Issues für 2-3 Wochen)
2. **Parallel starten**: #19 (Components Library) + #41/#42 (Legal Tasks)
3. **Nach Sprint 1**: Abhängigkeiten mit Backend synchronisieren
