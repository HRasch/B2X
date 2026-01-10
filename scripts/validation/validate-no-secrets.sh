#!/usr/bin/env bash
set -euo pipefail

# Simple local secret scanner wrapper.
# Prefers trufflehog (if installed), falls back to a lightweight grep scan.

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT_DIR"

echo "Running local secret scan..."

if command -v trufflehog >/dev/null 2>&1; then
  echo "Using trufflehog"
  trufflehog filesystem --entropy=True . || {
    echo "Secrets found by trufflehog" >&2
    exit 1
  }
  echo "No secrets found by trufflehog."
  exit 0
fi

if command -v detect-secrets >/dev/null 2>&1; then
  echo "Using detect-secrets (scan)"
  # detect-secrets prints baseline; if any plugin finds secrets it will be visible
  detect-secrets scan || true
  echo "Consider running 'detect-secrets scan > .secrets.baseline' to create a baseline."
  exit 0
fi

echo "Neither trufflehog nor detect-secrets found. Running conservative grep fallback."
# Conservative patterns
patterns=("JWT_SECRET" "POSTGRES_PASSWORD" "PRIVATE KEY" "BEGIN RSA PRIVATE KEY" "---BEGIN" "api_key" "apikey" "secret_key")
matches=0
for p in "${patterns[@]}"; do
  if grep -RI --exclude-dir=.git --exclude=.secrets.baseline -nI "$p" . || true; then
    matches=$((matches+1))
  fi
done

if [ "$matches" -gt 0 ]; then
  echo "Potential secret-like strings found by grep. Inspect output above." >&2
  exit 1
fi

echo "No obvious secrets detected by fallback scan."
exit 0
