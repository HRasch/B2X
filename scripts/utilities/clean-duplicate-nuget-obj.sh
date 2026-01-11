#!/usr/bin/env bash
set -euo pipefail

# Clean duplicate generated NuGet/MSBuild artifacts that can cause MSB4011
repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
echo "Cleaning duplicate generated NuGet/MSBuild files under $repo_root..."
find "$repo_root" -type f \
  \( -name "*nuget.g 2.props" -o -name "*nuget.g 2.targets" -o -name "project.assets 2.json" -o -name "*nuget.dgspec 2.json" -o -name "*project.assets 2.json" \) -print -exec rm -v {} \;
echo "Cleanup complete."
