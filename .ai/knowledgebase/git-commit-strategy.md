# Git Commit Strategy — Best Practices

This note summarizes recommended commit-message and commit-history practices for the project, with examples and suggested next steps for automation.

## High-level recommendations

- Adopt a concise commit message style inspired by Chris Beams' "seven rules" and the Conventional Commits specification.
- Keep commits small and atomic: one logical change per commit.
- Use PRs for feature work and prefer a clear merge strategy (squash or merge commits) agreed by the team.

## Commit message format (recommended)

1) Subject line (short):

  - Format: `type(scope): Short description`
  - Keep to ~50 characters; imperative mood; capitalize; no trailing period.
  - Examples: `feat(store): add product filtering by brand`
              `fix(api): handle missing price in product payload`

2) Blank line

3) Body (optional):

  - Explain *what* and *why*, not *how*; wrap at ~72 characters.
  - Include short bullet points for non-trivial side effects or migration steps.

4) Footer (optional):

  - Use for metadata and references, e.g. `Resolves: #123`, `Co-authored-by: Name <email>`.
  - For breaking changes include `BREAKING CHANGE: description` or use `!` after type/scope.

## Recommended commit types (Conventional Commits)

- `feat`: new feature
- `fix`: bug fix
- `chore`: build or tooling changes, infrastructure
- `docs`: documentation only changes
- `style`: formatting, no code change
- `refactor`: code change that neither fixes a bug nor adds a feature
- `perf`: performance improvements
- `test`: adding or fixing tests

Examples

```
feat(cart): add discount code support

Support percent and fixed discounts for cart totals.

Resolves: #452
```

```
fix(api)!: change product price field to decimal

BREAKING CHANGE: product.price now uses decimal instead of int; update integrations.
```

## Workflow & history hygiene

- Prefer small commits during development; squash or rebase locally before opening a PR when appropriate.
- Use the project's chosen merge strategy consistently (e.g., `Squash and merge` for tidy main history, or `Merge commit` to preserve branch history).
- If the team uses squash merges, ensure the final merge commit message follows this repository's conventions.

## Tooling & enforcement (optional, recommended)

- Add `commitlint` + `husky` for local validation of commit messages. Example config to enforce Conventional Commits is widely available (`@commitlint/config-conventional`).
- Add a lightweight GitHub Action to validate commit messages on PRs (helpful when contributors don't run local hooks).
- Provide a `CONTRIBUTING.md` entry with examples and a short template for commit messages.

## Suggested next steps I can implement

- Add this file to the knowledgebase (done) and open a PR (I will do it now).
- Add `commitlint` + `husky` scaffolding (package.json, config) and a GitHub Action to lint commit messages on PRs. (I can prepare a follow-up PR.)
- Add a short `CONTRIBUTING.md` section with examples and copy to repository root.

---

Maintainers: @TechLead, @GitManager, @SARAH
