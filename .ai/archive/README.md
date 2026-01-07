# Archive Directory

This directory contains historical documents, old analysis files, and superseded reports that are no longer actively maintained but retained for reference.

## Contents by Type

### Phase Reports (Archived Jan 7, 2026)
- `PHASE_2_TO_3_TRANSITION_SUMMARY.md` - Legacy phase transition doc
- `PHASE_3_BATCH_*.md` - Old batch migration results
- `PHASE_3_RESULTS_FIRST_BATCH.md` - Legacy results
- `PHASE_3_STATUS_UPDATE.md` - Old status tracking
- `PHASE_3_FULL_EXPANSION_RESULTS.md` - Final phase 3 results

→ **These documents are superseded by current sprint tracking in `.ai/sprint/`**

### REQ-007 Analysis (Archived Jan 7, 2026)
- `REQ-007-backend-analysis.md` & `REQ-007-backend-analysis 2.md`
- `REQ-007-consolidated-analysis.md` & `REQ-007-consolidated-analysis 2.md`
- `REQ-007-frontend-analysis.md` & `REQ-007-frontend-analysis 2.md`
- `REQ-007-productowner-analysis.md` & `REQ-007-productowner-analysis 2.md`
- `REQ-007-security-analysis.md` & `REQ-007-security-analysis 2.md`

→ **Consolidated into `.ai/requirements/REQ-007-email-wysiwyg-builder.md`**

### Security Audit Reports (Archived Jan 7, 2026)
- `SECURITY_AUDIT_REPORT.md` (versions 1, 2, 3)

→ **Historical security audits; current security docs in `.ai/decisions/`**

### Test & Utility Files (Archived Jan 7, 2026)
- `TestXsdValidation.cs` - Temporary test file
- `TestXsdValidation.csproj` - Test project
- `analyze-any-types.js` - Temporary analysis script
- `audit-script.js` - Temporary audit script
- `sample-erp-data.json` - Test data
- `test-erp-data.json` - Test data
- `test-erp-validation.json` - Test data

→ **Temporary files from development iterations, kept for reference only**

## Cleanup Policy

- **Retention period**: Archived documents kept for 6 months from archive date
- **Deletion**: Documents older than 6 months (after July 7, 2026) will be purged
- **Review frequency**: Quarterly
- **Exceptions**: Keep if referenced in active ADRs or requirements

## Searching Archive

To find a document:
```bash
# Find by type
grep -r "PHASE" .ai/archive/
grep -r "REQ-007" .ai/archive/
grep -r "SECURITY" .ai/archive/

# Find by date
ls -lt .ai/archive/ | head -20
```

## Moving Items Out of Archive

If you need to resurrect an archived document:
1. Copy from archive to appropriate location (`.ai/requirements/`, `.ai/decisions/`, etc.)
2. Update DocID if needed
3. Create link in [DOCUMENT_REGISTRY.md](../../.ai/DOCUMENT_REGISTRY.md)
4. Remove from archive

## Guidelines

- **Do NOT** delete archive files manually - coordinate with @SARAH
- **Do NOT** move files into archive without documenting why
- **Do** check archive before creating duplicate documents
- **Do** link to archive docs when referencing historical decisions

---

**Archive Created**: 2026-01-07  
**Next Review**: 2026-04-07  
**Responsible**: @SARAH, @DocMaintainer
