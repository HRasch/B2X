#!/usr/bin/env bash
set -euo pipefail

# Quick script to gather external update signals for the KB sync process.
# Produces a timestamped report in the artifacts/ directory.

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
OUT_DIR="$ROOT_DIR/artifacts"
mkdir -p "$OUT_DIR"
REPORT="$OUT_DIR/kb-sync-report-$(date +%Y%m%d-%H%M%S).txt"

echo "KB Sync Report" > "$REPORT"
echo "Generated: $(date --iso-8601=seconds)" >> "$REPORT"
echo "Repository: $ROOT_DIR" >> "$REPORT"
echo "" >> "$REPORT"

echo "-- dotnet outdated (backend) --" >> "$REPORT"
if command -v dotnet >/dev/null 2>&1; then
  if dotnet --version >/dev/null 2>&1; then
    pushd "$ROOT_DIR" >/dev/null
    # Try listing outdated packages for solution projects
    if dotnet list package --outdated >/dev/null 2>&1; then
      dotnet list package --outdated || true >> "$REPORT" 2>&1
    else
      echo "dotnet list package --outdated not supported on this SDK; skipping." >> "$REPORT"
    fi
    popd >/dev/null
  fi
else
  echo "dotnet not installed; skipping backend package checks." >> "$REPORT"
fi

echo "" >> "$REPORT"
echo "-- npm outdated (frontend) --" >> "$REPORT"
for d in frontend/Store frontend/Admin frontend/Management; do
  if [ -d "$ROOT_DIR/$d" ]; then
    echo "Checking $d" >> "$REPORT"
    pushd "$ROOT_DIR/$d" >/dev/null
    if command -v npm >/dev/null 2>&1; then
      npm ci --silent || true
      (npm outdated --long 2>&1) >> "$REPORT" || true
    else
      echo "npm not installed; skipping $d" >> "$REPORT"
    fi
    popd >/dev/null
  fi
done

echo "" >> "$REPORT"
echo "-- Optional upstream checks (Playwright / Node / .NET release notes) --" >> "$REPORT"
echo "(Run manually or enable if network/policy allows)" >> "$REPORT"

echo "" >> "$REPORT"
echo "Report saved to: $REPORT"
echo "To push findings: create an issue or PR referencing this report and update KB entries as needed." >> "$REPORT"

echo "KB sync report generated: $REPORT"
