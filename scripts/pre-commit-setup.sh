#!/usr/bin/env bash
set -euo pipefail

# Install pre-commit and detect-secrets, create baseline and install git hook

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT_DIR"

echo "Installing pre-commit and detect-secrets (Python required)..."
if ! command -v python >/dev/null 2>&1; then
  echo "Python is required. Please install Python 3." >&2
  exit 2
fi

python -m pip install --user --upgrade pip
python -m pip install --user pre-commit detect-secrets

echo "Generating detect-secrets baseline (if not present)..."
if [ ! -f .secrets.baseline ]; then
  detect-secrets scan > .secrets.baseline || true
  echo ".secrets.baseline created. Review and commit the baseline if acceptable." 
else
  echo "Baseline .secrets.baseline already exists." 
fi

echo "Installing pre-commit hook..."
if command -v pre-commit >/dev/null 2>&1; then
  pre-commit install || true
  echo "pre-commit installed. Hooks:"
  pre-commit run --all-files || true
else
  echo "pre-commit not available on PATH after install. Please add ~/.local/bin to your PATH or install pre-commit manually." >&2
fi

echo "Done."
