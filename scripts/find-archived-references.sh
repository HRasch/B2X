#!/usr/bin/env bash
set -euo pipefail

echo "Generating archived-docs reference report..."
OUT=.ai/logs/archived-docs-reference-report.md
mkdir -p "$(dirname "$OUT")"
echo "# Archived Docs Reference Report" > "$OUT"
echo "Generated: $(date -u)" >> "$OUT"
echo "" >> "$OUT"

PATTERNS=("docs/by-role" ".github_org" ".ai/" "docs/archive" "docs/processes" "docs/ai")

for p in "${PATTERNS[@]}"; do
  echo "## References to: $p" >> "$OUT"
  git grep -n -- "${p}" || true
  git grep -n -- "${p}" >> "$OUT" || true
  echo "" >> "$OUT"
done

echo "Report written to $OUT"
