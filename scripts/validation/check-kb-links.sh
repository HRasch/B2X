#!/usr/bin/env bash
set -euo pipefail

out=".ai/logs/broken-kb-links-2025-12-31.md"
mkdir -p .ai/logs
echo "Broken .ai/knowledgebase link report — $(date +%F)" > "$out"

repo_root=$(git rev-parse --show-toplevel)
files=$(git grep -l "\.ai/knowledgebase/" || true)

if [ -z "$files" ]; then
  echo "No files reference .ai/knowledgebase/" >> "$out"
else
  while IFS= read -r file; do
    grep -n --binary-files=without-match -E "\.ai/knowledgebase/" "$file" | while IFS=: read -r lineno line; do
      echo "$line" | grep -oE "(\.?\.?/)?\.ai/knowledgebase/[^)\] \\\"'\,]+" | while IFS= read -r m; do
        [ -z "$m" ] && continue
        if [[ "$m" == .* || "$m" == ..* ]]; then
          dir=$(dirname "$file")
          resolved_abs=$(python3 - <<PY
import os,sys
print(os.path.normpath(os.path.abspath(os.path.join(sys.argv[1], sys.argv[2]))))
PY
 "$dir" "$m")
          rel=${resolved_abs#"$repo_root"/}
        else
          rel="${m#./}"
          rel="${rel#/}"
        fi
        if [ ! -f "$rel" ]; then
          {
            echo "File: $file (line $lineno)"
            echo "  Link: $m"
            echo "  Resolved: $rel"
            echo "  Exists: NO"
            echo ""
          } >> "$out"
        fi
      done
    done
  done <<< "$files"
fi

if [ -s "$out" ]; then
  echo "Broken links report written to $out"
else
  echo "No broken .ai/knowledgebase links detected. Report: $out (empty)"
fi
