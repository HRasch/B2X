#!/usr/bin/env bash
set -euo pipefail

# Helper to create a branch, commit all changes and open a PR using the GitHub CLI (`gh`).
# Usage: ./scripts/create-pr.sh branch-name "PR title" "PR body"

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT_DIR"

if [ "$#" -lt 2 ]; then
  echo "Usage: $0 <branch-name> \"PR title\" [PR body]" >&2
  exit 2
fi

BRANCH="$1"
TITLE="$2"
BODY="${3-}" 

git checkout -b "$BRANCH"
git add -A
git commit -m "$TITLE" || echo "No changes to commit"

if command -v gh >/dev/null 2>&1; then
  if [ -z "$BODY" ]; then
    gh pr create --title "$TITLE" --fill
  else
    gh pr create --title "$TITLE" --body "$BODY"
  fi
else
  echo "gh CLI not found. To create a PR manually:" >&2
  echo "  git push -u origin $BRANCH" >&2
  echo "  Then open a PR on GitHub with title: $TITLE" >&2
fi
