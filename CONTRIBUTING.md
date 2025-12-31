## Contributing

Thank you for contributing to this repository. This guide is intentionally short and low-friction — suitable for a solo developer workflow.

### Commit Messages
- Use Conventional Commits style: `<type>(scope?): subject` — e.g. `feat(auth): add refresh token`.
- Types to use: `feat`, `fix`, `chore`, `docs`, `refactor`, `test`, `build`.
- Keep the subject <= 72 characters and use present tense.

Recommended: squash-and-merge for pull requests to keep history tidy.

### Branch Naming
- Use descriptive branch names: `feat/`, `fix/`, `chore/` prefixes, e.g. `feat/add-pr-template`.

### Pull Requests
- Create a PR from a short-lived branch; target `master`.
- Fill in the PR template. Include a short summary, testing steps, and related issue(s).
- For a solo project: prefer small PRs and `squash-and-merge` on merge.

### Tests & Checks
- Run existing tests before opening a PR. See `backend/` and `frontend/` tasks in the repo README.
- CI should pass before merging; enable branch protection if desired.

### Automation (optional)
- You may enable `commitlint` + `husky` locally to enforce commit message style.
- Consider adding a lightweight GitHub Action that validates PR titles and runs tests.

### Issues
- Use labels to classify issues. If an issue needs rewording, propose edits in a comment before applying changes.

If you want, I can:
- create a `CONTRIBUTING.md` PR (this file),
- scaffold `commitlint` + `husky` and a GH Action to validate PR titles, or
- run the `scripts/update-vision-issues.js` helper to propose rewrites (requires a token).

--
Short and practical guidance to keep development flowing. Ask me to implement any of the optional items.
