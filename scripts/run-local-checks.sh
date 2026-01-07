#!/usr/bin/env bash
set -euo pipefail

# Quick local validation script for developers.
# Runs format verification, build, unit tests, frontend lint and a smoke visual check (non-blocking).

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT_DIR"

echo "[run-local-checks] Starting quick local checks in $ROOT_DIR"

echo "[1/5] dotnet format --verify-no-changes"
if command -v dotnet >/dev/null 2>&1; then
  if ! dotnet format B2X.slnx --verify-no-changes; then
    echo "run-local-checks: dotnet format reported issues. Run 'dotnet format B2X.slnx' to fix them." >&2
    exit 1
  fi
else
  echo "dotnet not found; skipping format verification" >&2
fi

echo "[2/5] dotnet build"
dotnet build B2X.slnx --configuration Debug

echo "[3/5] dotnet test (unit/integration)"
dotnet test B2X.slnx --no-build --verbosity minimal

if [ -d frontend/Store ]; then
  echo "[4/5] Frontend lint (Store)"
  pushd frontend/Store >/dev/null
  if command -v npm >/dev/null 2>&1; then
    npm ci --silent
    if npm run lint --silent --if-present; then
      echo "eslint: OK"
    else
      echo "eslint: issues found; run 'npm run lint' and fix them." >&2
      popd >/dev/null
      exit 1
    fi
  else
    echo "npm not found; skipping frontend lint" >&2
  fi
  popd >/dev/null
fi

if [ -d frontend/Admin ]; then
  echo "[5/5] Playwright smoke visual check (Admin) — non-blocking"
  pushd frontend/Admin >/dev/null
  if command -v npm >/dev/null 2>&1; then
    npm ci --silent
    npx playwright install --with-deps --silent || true
    # Run a focused playwright test; allow failures locally (CI will enforce)
    npx playwright test tests/e2e/visual-regression.spec.ts -g "Theme consistency" --project=chromium || true
  else
    echo "npm not found; skipping Playwright checks" >&2
  fi
  popd >/dev/null
fi

echo "run-local-checks: All quick checks completed. For CI-equivalent strict checks, run the CI pipeline."
