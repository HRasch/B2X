**Security & Secret-Scanning Guide**

This file documents the local developer workflow for secret scanning and creating pull requests for security-related changes.

Local scan

1. Run the repository secret scanner (tries `trufflehog` then `detect-secrets`):

```bash
./scripts/validate-no-secrets.sh
```

2. If you want pre-commit checks, run the setup script (requires Python and pip):

```bash
./scripts/pre-commit-setup.sh
```

3. If you use `detect-secrets`, create or update the baseline and commit it:

```bash
detect-secrets scan > .secrets.baseline
git add .secrets.baseline
git commit -m "chore(secrets): update detect-secrets baseline"
```

Creating a PR with helper

Use the provided helper to create a branch and open a PR with the GH CLI (optional):

```bash
./scripts/create-pr.sh fix/template-secrets "Security: template secrets and add scan workflow" "Templatize docker-compose secrets and add CI secret-scan workflow"
```

If `gh` is not available the script will print push instructions.

CI

A GitHub Action `secret-scan.yml` was added at `.github/workflows/secret-scan.yml` to run `trufflehog` on push/PR.

Notes

- Never commit production secrets. Use Key Vault or another secret manager.
- If the CI workflow finds secrets, rotate the exposed credentials immediately.
