# Backend Developer - Zugeordnete Issues

**Status**: 2/18 Assigned (HRasch)  
**Gesamtaufwand**: ~80 Story Points  
**Kritischer Pfad**: Sprint 1 (14 Issues mit P0.6)

---

## Sprint 1 (P0.6 - Höchste Priorität)

| # | Titel | Punkte | Abhängigkeiten |
|---|-------|--------|-----------------|
| #5 | User Registration Handler | 3 | - |
| #6 | Email Verification Logic | 3 | #5 |
| #7 | Password Reset Flow | 3 | #5 |
| #9 | Multi-Tenant Context Middleware | 5 | #5 |
| #10 | JWT Token Generation & Refresh | 5 | #5 |
| #11 | User Roles & Permissions (RBAC) | 5 | #10 |
| #12 | Registration Endpoints (HTTP Handlers) | 5 | #5, #6, #7 |
| #29 | VVVG 14-Day Return Policy (Backend) | 5 | - |
| #30 | VAT-ID Validation (B2B) | 8 | ⚠️ **Assigned: HRasch** |
| #31 | Reverse Charge Logic (Gutschriften) | 8 | ⚠️ **Assigned: HRasch** |

**Summe Sprint 1**: 50 Story Points

---

## Sprint 2 (P0.6 - Compliance)

| # | Titel | Punkte | Abhängigkeiten |
|---|-------|--------|-----------------|
| #20 | Price Calculation Service (VAT Included) | 8 | #30 |
| #21 | Invoice Generation API (ZUGFeRD/UBL) | 8 | #30, #31 |
| #27 | Database Migrations & Schemas | 5 | - |
| #18 | Theme Configuration API | 3 | - |

**Summe Sprint 2**: 24 Story Points

---

## Sprint 3 (P0.6 - Extended)

| # | Titel | Punkte | Abhängigkeiten |
|---|-------|--------|-----------------|
| #22 | B2B Payment Terms Service | 5 | #20 |
| #23 | Order Return Management | 5 | #29 |
| #24 | Withdrawal Period Tracking | 3 | #29 |
| #25 | Email Notification Service | 3 | #21 |
| #26 | Admin Dashboard APIs | 5 | #12, #11 |

**Summe Sprint 3**: 21 Story Points

---

## Sprint 4 (P0.6 - Final)

| # | Titel | Punkte | Abhängigkeiten |
|---|-------|--------|-----------------|
| #28 | Webhook Integration (ERP Sync) | 5 | #21 |
| #8 | Duplicate Detection Service | 3 | #5 |

**Summe Sprint 4**: 8 Story Points

---

## Aktuell Unzugewiesen (16 Issues)

```bash
# Diese Issues benötigen Zuweisung:
#5, #6, #7, #8, #9, #10, #11, #12, #20, #21, #22, #23, #24, #25, #26, #27, #28, #29

# Zugewiesen:
#30 → HRasch
#31 → HRasch
```

---

## Erforderliche Fähigkeiten

- ✅ C# / .NET 10
- ✅ Entity Framework Core (EF 8)
- ✅ Wolverine (HTTP Handlers, CQRS)
- ✅ PostgreSQL 16
- ✅ JWT / OAuth2
- ✅ Tax/Compliance: VAT, Reverse Charge, German E-Commerce Law (PAngV, VVVG)
- ⚠️ Optional: Elasticsearch, Redis

---

## Blockiert durch

- **Security Engineer**: #30, #31 (VAT Validation muss vor #20, #21 done sein)
- **Legal Officer**: #29 (Return Policy Specifications erforderlich)

---

## Nächste Schritte

1. **2-3 Backend Developer zuweisen** (insgesamt 18 Issues für 4+ Wochen)
2. **HRasch fokussiert auf VAT/Tax-Module** (#30, #31)
3. **Dependency Chain respektieren**: #5 → #6,#7 → #12 → #20,#21
