# Review: Archived Internal Documentation

Purpose: coordinate a team review of the documentation moved to `.archive/internal-docs/` (PR: https://github.com/HRasch/B2X/pull/95). The goal is to either: 1) retain and move relevant content into the official knowledgebase (`.ai/knowledgebase/` or `docs/`), 2) update or remove repo references that point to archived locations, or 3) permanently delete obsolete documents.

Checklist for agents (complete each item and write results to `.ai/status/archived-docs-review.md`):

- [ ] Scope: Confirm the list of archived folders and file counts in `.archive/internal-docs/`.
- [ ] Validate: For each file, decide if content is: `knowledgebase` (move to `.ai/knowledgebase/`), `developer-guide` (move to `docs/guides/`), `user-doc` (move to `docs/user-guides/`), or `obsolete` (delete).
- [ ] Update references: Search repo for references to archived paths (run `scripts/find-archived-references.sh`) and for each reference either update the link to the new location or remove it.
- [ ] Knowledgebase update: For files marked `knowledgebase`, add or update entries in `.ai/knowledgebase/INDEX.md` and ensure corresponding README links exist.
- [ ] Record decisions: For each processed file, append a short record to `.ai/issues/archived-docs-review-log.md` with: path, decision, rationale, and target action (move/delete/left-as-archive).
- [ ] Cleanup PRs: Where changes are made, open PRs with short descriptions and link this review issue.

How to run the quick reference report (local / CI):

```bash
# create a report of references to archived docs
bash scripts/find-archived-references.sh > .ai/logs/archived-docs-reference-report.md
git add .ai/logs/archived-docs-reference-report.md
git commit -m "docs: add archived docs reference report"
git push
```

Notes for reviewers:
- Be conservative: propose moving content into knowledgebase rather than immediate deletion if unsure.
- When moving files to `.ai/knowledgebase/`, follow the existing knowledgebase structure and add a one-line summary at the top of the moved file.
- Update internal references and agent prompts that reference `docs/by-role/*` or `.ai/*` to use the new canonical locations.

Owner: `@SARAH` to coordinate. Assign relevant domain agents (`@Backend`, `@Frontend`, `@Security`, `@Legal`, `@TechLead`) to validate domain-specific files.
