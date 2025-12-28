# Praktische Anleitung: GitHub Issues Aktualisieren

**Zielgruppe:** Project Manager, Scrum Master, Team Lead  
**Aufwand:** 15-20 Minuten  
**Status:** ✅ Ready to Execute

---

## 🎯 Was wurde aktualisiert?

### Datei:
```
.github/ISSUE_TEMPLATE/customer-registration-flow.md
```

### Changes:
- ✅ **Effort erhöht**: 13 SP → 21 SP (8 SP hinzugefügt)
- ✅ **4 neue Stories** (8-11) vollständig dokumentiert
- ✅ **Integration Points** mit bestehenden Stories
- ✅ **API Endpoints & Database Changes** definiert
- ✅ **Links zu Dokumentationen** eingebunden

---

## 📋 Schritt-für-Schritt Anleitung

### Schritt 1: GitHub WebUI öffnen

```
https://github.com/[YOUR-ORG]/B2Connect
→ Issues → customer-registration-flow.md
```

Oder direkt:
```
.github/ISSUE_TEMPLATE/customer-registration-flow.md
```

### Schritt 2: Issue im GitHub Web-Editor öffnen

1. Click "Edit" (Stift-Icon) neben der Datei
2. Oder direkt in der Raw-Ansicht: "Edit this file"

### Schritt 3: Inhalte aktualisieren

Die Änderungen sind bereits gemacht in der lokalen Datei:

```
Backend/Verzeichnis/
├── .github/ISSUE_TEMPLATE/
│   └── customer-registration-flow.md  ← AKTUALISIERT
```

Zum Überprüfen: Suchterm verwenden
```
"Story 8: Check Customer Type"
"Story 9: Existing Customer Registration"
"Story 10: Duplicate Detection"
"Story 11: ERP Integration"
```

### Schritt 4: Changes committen (falls lokal)

Falls du lokal arbeitest:

```bash
git add .github/ISSUE_TEMPLATE/customer-registration-flow.md
git commit -m "feat: Add existing customer registration stories (8-11) to epic

- Story 8: Check customer type (2 SP)
- Story 9: Existing customer registration form (3 SP)
- Story 10: Duplicate detection & prevention (2 SP)
- Story 11: ERP integration & data validation (1 SP)

Total effort: 13 SP → 21 SP
Target: KW 2 2026"

git push origin feature/customer-registration-epic
```

### Schritt 5: GitHub Issue Labels aktualisieren

**Im GitHub Web-Interface:**

1. Issues Tab → Suche "Customer Registration Flow"
2. Issue öffnen
3. Rechts: "Labels" section
4. Ändern zu:
   ```
   - epic (neu/bestehend)
   - registration (neu/bestehend)
   - erp-integration (NEU)
   - p1-high (neu/bestehend)
   - backend (neu/bestehend)
   - frontend (neu/bestehend)
   - 21-story-points (UPDATE: 13 → 21)
   ```

### Schritt 6: Milestone aktualisieren

**Im GitHub Web-Interface:**

1. Issues Tab → "Customer Registration Flow" Issue öffnen
2. Rechts: "Milestone" section
3. Wählen: "Q1 2026 Sprint 2 (KW 2)"
4. Oder erstellen falls nicht existiert:
   ```
   Milestone: "Q1 2026 Sprint 2"
   Description: "KW 2 2026 (6. Januar - 10. Januar)"
   Due Date: 10. Januar 2026
   ```

### Schritt 7: Assignees aktualisieren

**Im GitHub Web-Interface:**

1. Issues Tab → Issue öffnen
2. Rechts: "Assignees" section
3. Hinzufügen:
   - Backend Lead
   - Frontend Lead
   - (Optional) Architect für Stories 10-11

### Schritt 8: Linked Issues erstellen (optional, aber empfohlen)

Falls GitHub Projects verwendet wird:

```
Parent Issue: "Customer Registration Flow Epic"
Child Issues (optional):
  - Story 8: Check Customer Type
  - Story 9: Existing Customer Registration
  - Story 10: Duplicate Detection
  - Story 11: ERP Integration
```

### Schritt 9: Team benachrichtigen

**E-Mail an Team:**

```
Subject: 📢 GitHub Issue Update - Customer Registration Epic (21 SP)

Hallo Team,

die Issue "Customer Registration Flow Epic" wurde aktualisiert mit 
neuen Stories zur Bestandskunden-Registrierung:

📊 Summary:
- Story 8: Check Customer Type (2 SP)
- Story 9: Existing Customer Registration (3 SP)
- Story 10: Duplicate Detection (2 SP)
- Story 11: ERP Integration (1 SP)

Gesamtaufwand: 13 SP → 21 SP
Zieltermin: KW 2 2026 (6.-10. Januar)

📚 Dokumentationen:
1. Spezifikation: docs/features/BESTANDSKUNDEN_VEREINFACHTE_REGISTRIERUNG.md
2. Code Scaffold: docs/features/BESTANDSKUNDEN_IMPLEMENTIERUNGS_SCAFFOLD.md
3. Quick-Start: docs/features/BESTANDSKUNDEN_QUICK_START.md

🔗 Link zum Issue:
[GitHub Issue URL]

Bitte reviewt die Stories und gebt Feedback bis [Datum].

Grüße,
[Your Name]
```

---

## 🔍 Validierungs-Checkliste

Nach Update: Prüfe folgende Punkte

- [ ] Issue titel geändert von "13 SP" zu "21 SP"?
- [ ] Story 8-11 vollständig sichtbar im Issue?
- [ ] Links zu Dokumentationen funktionieren?
- [ ] Labels aktualisiert (erp-integration NEU)?
- [ ] Milestone gesetzt auf "KW 2 2026"?
- [ ] Assignees hinzugefügt (Backend, Frontend Lead)?
- [ ] Description erwähnt "Sub-Epic: Existing Customers"?

### Schnell-Check (CLI):

```bash
# Überprüfe ob Stories 8-11 in Datei vorhanden sind
grep -c "Story 8" .github/ISSUE_TEMPLATE/customer-registration-flow.md
grep -c "Story 11" .github/ISSUE_TEMPLATE/customer-registration-flow.md

# Sollte beide "1" zurückgeben
```

---

## 📱 Falls du ein Issue-Management-Tool nutzt (z.B. Azure DevOps, Jira)

### Azure DevOps:

1. Work Items → Epic "Customer Registration Flow"
2. Edit → Update Story Points: 13 → 21
3. Child Work Items hinzufügen:
   ```
   - Story 8: Check Customer Type (2 SP)
   - Story 9: Existing Customer Form (3 SP)
   - Story 10: Duplicate Detection (2 SP)
   - Story 11: ERP Integration (1 SP)
   ```
4. Sprint: "Q1 2026 Sprint 2"
5. Target Date: 10. Januar 2026

### Jira:

1. Epic "Customer Registration Flow"
2. Edit → Story Points: 13 → 21
3. Description: Bestandskunden-Registrierung Sektion hinzufügen
4. Sub-Tasks erstellen:
   ```
   Story 8, 9, 10, 11
   ```
5. Sprint: "KW 2 2026"

---

## 🆘 Häufige Probleme & Lösungen

### Problem: "Issue nicht in GitHub Web-UI sichtbar"
**Lösung:**
```bash
# Lokale Änderungen pushen
git add .github/ISSUE_TEMPLATE/customer-registration-flow.md
git commit -m "Update: Add existing customer registration stories"
git push

# Dann GitHub refreshen (F5)
```

### Problem: "Labels existieren nicht"
**Lösung:**
1. GitHub → Repository Settings → Labels
2. "New Label" Buttons für fehlende Labels
3. Dann Issue aktualisieren

### Problem: "Milestone existiert nicht"
**Lösung:**
1. GitHub → Issues → Milestones
2. "New Milestone" Button
3. Name: "Q1 2026 Sprint 2"
4. Due Date: 10. Januar 2026

---

## ✅ Abschluss-Checkliste

Nach Abschluss:

- [ ] GitHub Issue aktualisiert
- [ ] Labels korrekt gesetzt
- [ ] Milestone zugewiesen
- [ ] Assignees hinzugefügt
- [ ] Team per E-Mail benachrichtigt
- [ ] Dokumentationen verlinkt
- [ ] Sprint Planning für KW 2 angesetzt
- [ ] Review der neuen Stories durchgeführt

---

## 📞 Support

Falls Fragen auftauchen:

1. **Spezifikation:** Siehe `docs/features/BESTANDSKUNDEN_VEREINFACHTE_REGISTRIERUNG.md`
2. **Implementation:** Siehe `docs/features/BESTANDSKUNDEN_QUICK_START.md`
3. **Code-Scaffold:** Siehe `docs/features/BESTANDSKUNDEN_IMPLEMENTIERUNGS_SCAFFOLD.md`

---

**Zeitaufwand:** 15-20 Minuten  
**Schwierigkeit:** ⭐⭐ Einfach  
**Status:** 🟢 Ready to Execute

---

Fragen? Erstelle ein neues Issue mit Label `documentation-request` oder kontaktiere das Architecture Team.

**Letzte Aktualisierung:** 28. Dezember 2025
